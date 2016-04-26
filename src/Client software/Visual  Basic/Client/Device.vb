Imports System.IO.Ports
Imports System.Threading
Imports NCalc
Imports Client.GLOBALS



Public Class Device
#Region "DEVICE_COMMANDS AND CONSTANTS"
    Public OUTPUT = "0"
    Public INPUT = "1"
    Const timeout_time As Integer = 200
#End Region
    Public Enum DevType
        BOOL = 0 'can only be 1 or 0
        PWM = 1 'can be 0<=pwm<=255
        RGB = 2 ' three pwm's, statechangeargument is hex color
        SENSOR = 3 'sensor is analog input
        SERVO = 4 '0<=position<=180 in degreed
        DOOR = 8 ' open/close
        'more devices to come
    End Enum
    Public pin As List(Of Integer) 'pins we're using for device
    Public devid As String 'we use this for identifying the device
    Public dtype As Integer 'device type
    Public devstate As List(Of String) 'state of each pin of device
    Public devname As String 'name of device that will be presented on server and device manager
    Private cont As Controller = GLOBALS.controller 'controller of our device
    Public stateLabel As Label 'current state in device manager
    Private devnameLabel As Label 'name of device in dev. mgr
    Public lambdafi As Boolean 'is lambda running
    Public anonymus As Thread 'lambda thread
    Public logstatechange As Boolean = True 'default of device is to log state change
    Public devstate_string As String = "" ' string that combines the pin states in right format to send to server and devmgr
    Public formula As String = "" 'formula to put input from controller trough. Ussualy used in analog devices to have meaningful output
    Private timeout As New TimeItOut(timeout_time)
    Sub New(devname As String, pin As List(Of Integer), type As Integer, devid As Integer, Optional devstate As List(Of String) = Nothing, Optional fla As String = Nothing)
        Me.devname = devname 'set dev name
        Me.dtype = type 'set dev type
        If Not fla Is Nothing Then
            Me.formula = fla 'check for formula, and if exists set it
        End If
        If dtype = DevType.SENSOR Then
            logstatechange = False 'disable logging for sensors, this is only for txtLog , not for server
        End If
        If devstate Is Nothing Then 'devstate is optional param so it it's nothing set it..and otherwise use what's already suppliFed
            Select Case Me.dtype
                Case DevType.BOOL, DevType.PWM, DevType.SENSOR, DevType.DOOR, DevType.SERVO
                    Me.devstate = New List(Of String)(New String() {"1"})
                Case DevType.RGB
                    Me.devstate = New List(Of String)(New String() {"1", "1", "1"})
            End Select
        Else
            Me.devstate = devstate
        End If
        'check for conflicts with other pins, and if there is no conflict
        If pin.TrueForAll(AddressOf pinconflict) Then
            If type = DevType.BOOL Then
                If pin.Count = 1 And cont.isDigital(pin.Item(0)) Then 'for boolean pin must be only 1 pin and it must be digital
                    Me.pin = pin 'set pin
                    spdriver.Send("pinMode:" & pin.Item(0) & ":" & OUTPUT) 'set it to output , now we should add delay, or solve it using okawait :)
                Else
                    Throw New Exception("Requerments not met")
                End If
            ElseIf type = DevType.PWM Or type = DevType.SERVO Or type = DevType.DOOR Then
                If pin.Count = 1 Then
                    If cont.isPwm(pin.Item(0)) Then 'these mus be pwm and only one pin
                        Me.pin = pin
                        spdriver.Send("pinMode:" & pin.Item(0) & ":" & OUTPUT)
                    Else
                        Throw New Exception("Requerments not met")
                    End If
                Else
                    Throw New Exception("Requerments not met")
                End If
            ElseIf type = DevType.RGB Then 'this must be 3 pins and each must be pwm
                If pin.Count = 3 And pin.TrueForAll(Function(p)
                                                        If cont.isPwm(p) Then : Return True
                                                        Else : Return False
                                                        End If
                                                    End Function) Then
                    Me.pin = pin
                    For Each i As Integer In pin
                        okawait = True 'here's the example of okawait :)
                        timeout.reset()
                        spdriver.Send(String.Join(":", New String() {"pinMode", i, OUTPUT}))
                        While okawait And Not timeout.hasTimedOut
                            timeout.Tick()
                        End While
                        timeout.reset()
                    Next
                End If
            ElseIf type = DevType.SENSOR Then
                If pin.Count = 1 Then
                    If cont.isAnalog(pin.Item(0)) Then
                        Me.pin = pin
                        ' spdriver.Send("pinMode:" & pin.Item(0) & ":" & INPUT) there's no need for this, already done in arduino program
                    Else
                        Throw New Exception("Requerments not met")
                    End If
                End If
            End If
        Else

            elui.Log("Pin conflict occured")
        End If
        Me.devid = devid
        displayDeviceInManager() 'display it in devmgr

    End Sub

    Public Event StateChanged(dev As Device, pin As Integer)
    Private Function displayDeviceInManager()
        Dim p As New Panel
        p.MaximumSize = New Size(500, 100)
        p.BackColor = Color.Transparent
        p.BorderStyle = BorderStyle.FixedSingle
        AddHandler p.Resize, AddressOf dm.PanelResize
        Dim lblName As New Label
        lblName.Text = Me.devname
        lblName.Name = "devName"
        Dim lblStanje As New Label
        lblStanje.Text = Me.devstate_string
        lblStanje.Name = "devState"
        p.Controls.Add(lblStanje)
        p.Controls.Add(lblName)
        Me.stateLabel = lblStanje
        dm.Devices.Controls.Add(p)
        dm.PanelResize(p, Nothing)
        Return 0
    End Function

    Public Function getPin() As List(Of Integer)
        Return Me.pin
    End Function

    Private Function changeInnerState(dpin As Integer, state As String)
        If Me.dtype = DevType.BOOL Or Me.dtype = DevType.PWM Or Me.dtype = DevType.SENSOR Then 'one pin things
            If Not Me.devstate(0) = state Then 'if state changed
                Me.devstate(0) = state
                Me.devstate_string = state 'set new and raise event, formatting is not needed here but can be applied to devstate_string
                RaiseEvent StateChanged(Me, dpin)
            End If
        ElseIf Me.dtype = DevType.RGB Then
            Select Case Me.pin.IndexOf(dpin)
                Case 0
                    If Not Me.devstate(0) = state Then 'the same as above but for each pin 
                        Me.devstate(0) = state
                        Me.devstate_string = "#" & CByte(Int(Me.devstate(0))).ToString("X2") & CByte(Int(Me.devstate(1))).ToString("X2") & CByte(Int(Me.devstate(2))).ToString("X2")
                        RaiseEvent StateChanged(Me, dpin)

                    End If
                Case 1
                    If Not Me.devstate(1) = state Then
                        Me.devstate(1) = state
                        Me.devstate_string = "#" & CByte(Int(Me.devstate(0))).ToString("X2") & CByte(Int(Me.devstate(1))).ToString("X2") & CByte(Int(Me.devstate(2))).ToString("X2")
                        RaiseEvent StateChanged(Me, dpin)
                    End If

                Case 2
                    If Not Me.devstate(2) = state Then
                        Me.devstate(2) = state
                        Me.devstate_string = "#" & CByte(Int(Me.devstate(0))).ToString("X2") & CByte(Int(Me.devstate(1))).ToString("X2") & CByte(Int(Me.devstate(2))).ToString("X2")
                        RaiseEvent StateChanged(Me, dpin)
                    End If
                Case Else
                    MsgBox("Doslo je do neocekivanog odgovora uredjaja")
            End Select
        ElseIf Me.dtype = DevType.DOOR Then
            state = Int(state)
            'same as first, and can be transfered up into one pin  things here devstate_string formating is applied as is in RGB(they're not the same but both have format applied)

            If Not Int(Me.devstate(0)) = state Then
                If state < 80 Then
                    Me.devstate_string = "OPENED"
                    RaiseEvent StateChanged(Me, dpin)
                ElseIf state > 160 Then
                    Me.devstate_string = "CLOSED"
                    RaiseEvent StateChanged(Me, dpin)
                Else
                    Me.devstate_string = "IN BETWEEN" 'shouln't happen
                End If
                Me.devstate(0) = state
            End If
        End If
        Return 0
    End Function
    Public Sub changeState(dev_id As String, state As String)
        'do something based on state param, and device type
        state = state.Replace(vbCr, "").Replace(vbLf, "")
        If Me.devid = dev_id Then
            Select Case dtype 'serves for return if same value, no need to bug the device
                Case DevType.BOOL, DevType.PWM, DevType.SENSOR, DevType.SERVO, DevType.RGB
                    If state.ToLower = devstate_string.ToLower Then
                        Exit Sub
                    End If
                Case DevType.DOOR
                    Select Case state
                        Case "OTVORI"
                            If devstate_string = "OTVORENO" Then
                                Exit Sub
                            End If
                        Case "ZATVORI"
                            If devstate_string = "ZATVORENO" Then
                                Exit Sub
                            End If
                    End Select
            End Select
            If Me.dtype = DevType.BOOL Then
                If state = "0" Or state = "1" Then

BooleanError:

                    okawait = True
                    timeout.reset()
                    spdriver.Send(String.Join(":", New String() {"digitalWrite", Me.pin(0), state}))
                    While okawait And Not timeout.hasTimedOut
                        timeout.Tick()
                    End While
                    If timeout.hasTimedOut Then
                        GoTo BooleanError
                        elui.Log("Request timed out ! BooleanError")
                    End If
                    timeout.reset()
                End If
            ElseIf Me.dtype = DevType.RGB Then
                Try
                    Dim color As Color = ConvertToRbg(state)

RGBError_R:
                    okawait = True
                    timeout.reset()
                    spdriver.Send(String.Join(":", New String() {"analogWrite", Me.pin(0), color.R}))
                    While okawait And Not timeout.hasTimedOut
                        timeout.Tick()
                    End While
                    If timeout.hasTimedOut Then
                        GoTo RGBError_R
                        elui.Log("RGB_R ERROR")
                    End If
RGBError_G:
                    okawait = True
                    timeout.reset()
                    spdriver.Send(String.Join(":", New String() {"analogWrite", Me.pin(1), color.G}))
                    While okawait And Not timeout.hasTimedOut
                        timeout.Tick()
                    End While
                    If timeout.hasTimedOut Then
                        GoTo RGBError_G
                        elui.Log("RGB_B ERROR")
                    End If
RGBError_B:
                    timeout.reset()
                    okawait = True
                    spdriver.Send(String.Join(":", New String() {"analogWrite", Me.pin(2), color.B}))
                    While okawait And Not timeout.hasTimedOut
                        timeout.Tick()
                    End While
                    If timeout.hasTimedOut Then
                        GoTo RGBError_B
                        elui.Log("RGB_B ERROR")
                    End If
                    timeout.reset()
                    Console.WriteLine(String.Join("--", New String() {color.R, color.G, color.B}))
                Catch ex As Exception
                    Console.WriteLine(ex.Message)
                End Try

            ElseIf Me.dtype = DevType.PWM Then
                Try
PWM_Error:
                    Dim pwm As Integer = Int(state)
                    If pwm >= 0 And pwm <= 255 Then
                        okawait = True
                        timeout.reset()
                        spdriver.Send(String.Join(":", New String() {"analogWrite", Me.pin(0), pwm}))
                        While okawait And Not timeout.hasTimedOut
                            timeout.Tick()
                        End While
                        If timeout.hasTimedOut Then
                            GoTo PWM_Error
                        End If
                    End If
                Catch ex As Exception
                    Console.WriteLine(ex.Message)
                End Try
            ElseIf Me.dtype = DevType.DOOR Then
                state = state.Replace(vbCr, "").Replace(vbLf, "")
                If state = "OPEN" Then
                    okawait = True
                    spdriver.Send(String.Join(":", New String() {"servoMove", Me.pin(0), 165}))
                    While okawait And Not timeout.hasTimedOut
                        timeout.Tick()
                    End While
                    timeout.reset()
                ElseIf state = "CLOSE" Then
                    okawait = True
                    spdriver.Send(String.Join(":", New String() {"servoMove", Me.pin(0), 65}))
                    While okawait And Not timeout.hasTimedOut
                        timeout.Tick()
                    End While
                    timeout.reset()
                End If

            End If
        End If


    End Sub
    Public Function processSM(msg As String) 'called from message processor
        Try
            Dim process() As String = msg.Split(",")
            For Each dev_data As String In process
                If dev_data.Split(":").Length = 2 Then
                    Dim dev_pin = dev_data.Split(":")(0)
                    Dim dev_state = dev_data.Split(":")(1)
                    If Me.pin.Contains(dev_pin) Then
                        changeInnerState(dev_pin, dev_state) 'if we find the pin in pin:value config call changeinnerstate
                    End If
                End If
            Next
        Catch
        End Try

        Return 0
    End Function

    Public Function pinconflict(pin As Integer) As Boolean 'pinconflict function
        For Each dev In devicelist.ToList
            If dev.pin.Contains(pin) Then
                Return False
            End If
        Next
        Return True
    End Function


End Class
