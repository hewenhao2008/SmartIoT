Imports System.Data.SqlServerCe
Imports Client.GLOBALS

Public Class DeviceManager
    'delegate subs
    Delegate Sub _SUDL()
    Delegate Sub _ChangeLabelText(label As Label, text As String)

    Public Sub ChangeLabelText(label As Label, text As String)
        If InvokeRequired Then
            Invoke(New _ChangeLabelText(AddressOf ChangeLabelText), label, text) '' invoke with delegate
            Exit Sub
        End If
        label.Text = text 'change label text
    End Sub

    Public Sub PanelResize(sender As Object, e As EventArgs)
        Dim p As Panel = sender
        For Each c As Object In p.Controls
            If TypeOf c Is Label Then
                Dim l As Label = CType(c, Label)
                If l.Name = "devName" Then
                    l.Location = New Point(p.Width / 2 - l.Width / 2, 10)
                ElseIf l.Name = "devState" Then
                    l.Location = New Point(p.Width / 2 - l.Width / 2, p.Height / 2) ' put labels at center :)
                End If
            End If
        Next
    End Sub

    Public Sub Server_Update_Device_List()
        If InvokeRequired Then
            Invoke(New _SUDL(AddressOf Server_Update_Device_List)) 'invoke if required
            Exit Sub
        End If
        Dim device_info As New List(Of String) 'device info will contain string correctly formated to send to server
        Dim result As String = ""
        For Each dev As Device In devicelist.ToList
            device_info.Add("[" & dev.devid & ",'" & dev.devstate_string & "'," & dev.dtype & ",'" & dev.devname & "']")
        Next
        result = "["
        result &= Join(device_info.ToArray, ",")
        result &= "]"

        tcph.Send("ServUDL:" & result) 'servudl stands for server update device list. the format it accepts is python list, because i did python backend, but if 
            'needed I'll implement format for other server languages, but this project aims to be written on many languages
            ' so [[devid,devstate,devtype,devname],[devid1,devstate1,devtype1,devname1],....]



    End Sub
    Private Sub devStateChanged(dev As Device, pin As Integer) 'some device changed stat
        Server_Update_Device_List() 'update it's status on server
        dm.ChangeLabelText(dev.stateLabel, dev.devstate_string) 'change stateLabel text with devstate_string 
        If dev.logstatechange And devicescreated Then ''if device is logging , which all devices do except analog sensors (this can be changed but by default)
            elui.Log("Device: " & dev.devname & " changed state to: " & dev.devstate_string) 'log statechange
        End If
        For Each trig As Trigger In glob_triggers
            If trig.triggerDevice Is dev Or trig.triggeredDevice Is dev Then
                Dim t As New Threading.Thread(AddressOf trig.changedDeviceTrigger)
                t.Start()

            End If
        Next
    End Sub
    Public Sub Populate_Device_List() 'here we'll populate device list 
        Try 'try to connect to database and get devices
            cmd = New SqlCeCommand("SELECT * FROM devices", con) 'database query to get all devices
            If con.State = ConnectionState.Closed Then con.Open() ' if connection not open , open it
            Dim sdr As SqlCeDataReader = cmd.ExecuteReader() ' execute query
            devicelist.Clear() 'clear devicelist, devicelist is global list of device class
            While sdr.Read = True 'while there is result
                Try
                    'device(devname = String, pin = List(Of Integer),devtype = Integer, devid = Integer, devstate = List(Of String),formula = String)
                    Dim temp_device As Device = New Device(sdr.Item("devname"), sdr.Item("pin").ToString.Split(",").ToList.ConvertAll(New Converter(Of String, Integer)(AddressOf stoi)), Int(sdr.Item("devtype")), Int(sdr.Item("devid"))) 'new device
                    'should pass the formula field and devstate field, but must think about it
                    AddHandler temp_device.StateChanged, AddressOf devStateChanged 'add handler for device state changed
                    devicelist.Add(temp_device) '' ad it to devicelist
                Catch ex As Exception
                    MsgBox("Can't create device" & ex.Message) 'can't create device exit the program, something's wrong
                    elui.Close()
                End Try
            End While
            devicescreated = True 'if all successful then all devices are created , and the program can continue execution where this is needed
        Catch ex As Exception
            MsgBox("Database connection problem") ' msg the user and exit application
            elui.Close()
        End Try

    End Sub

    Private Sub DeviceManager_FormClosing(sender As Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Me.Hide() 'don't dispose the form
        e.Cancel = True
    End Sub

    Private Function stoi(ByVal conv As String) 'string to int function
        Return Int(conv) 'just return the int :)
    End Function

    Private Sub DeviceManager_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

    End Sub

    Private Sub Devices_Paint(sender As System.Object, e As System.Windows.Forms.PaintEventArgs) Handles Devices.Paint

    End Sub
End Class