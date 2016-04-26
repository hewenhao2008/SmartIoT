Imports Client.Device
Imports System.Threading
Imports Client.TriggerFunctions
Public Class Trigger
    Public triggerDevice, triggeredDevice As Device ''private device
    Public triggerValue, triggeredValue, oper As String ''values to compare
    Private functionstop As Boolean = False, lambda As String = Nothing, lambda_params() As Object, _function As Thread = Nothing ''params for thread to run
    Public Sub New(triggerDevice As Device, operVal As String, triggerValue As String, triggeredDevice As Device, triggeredValue As String, Optional lambdafunction As String = Nothing, Optional lambdaparams() As Object = Nothing)
        If New List(Of String)(New String() {"=", "<", ">", "<=", ">="}).Contains(operVal) And TypeOf triggerDevice Is Device And TypeOf triggeredDevice Is Device Then
            Me.oper = operVal
            Me.triggerDevice = triggerDevice
            Me.triggeredDevice = triggeredDevice
            Me.triggeredValue = triggeredValue
            Me.triggerValue = triggerValue
            Me.lambda = lambdafunction
            Me.lambda_params = lambdaparams
        Else
            Throw New Exception("Not valid !")
        End If

    End Sub
    Private Sub runFunction()
        While functionstop = False 'variable to stop function
            getFuncByName(lambda, lambda_params) 'get our lambda function, there you can write all kinds of stuff to play with devices :)
            ''shoud think about modularity a bit here
        End While
    End Sub
    Public Sub changedDeviceTrigger(dev As Device)
        If evaluate() Then 'if we evaluate to true
            If lambda Is Nothing Then ''if we have no function to run
                triggeredDevice.changeState(triggeredDevice.devid, triggeredValue) ' just change state to desired
            Else
                If _function Is Nothing Then 'we must assure that function is nothing before starting new thread
                    If functionExists(lambda) Then ' if it exists in our lamba function definitions
                        functionstop = False ' don't stop the funciton
                        _function = New Thread(AddressOf runFunction) ''initialize and start a thread that will call the function while evaluate is true
                        _function.IsBackground = True
                        _function.Start()
                    End If
                End If
            End If

        Else
            stopFunction() ' if evaluate ever evaluates to false, shutdown thread with function caller
        End If
    End Sub
    Public Sub stopFunction()
        If Not _function Is Nothing Then 'if we have a thread assigned to job
            functionstop = True 'stopfunction 
            _function = Nothing
        End If
    End Sub
    Function evaluate() ''evaluate triger value and device current value 
        Try
            Select Case oper
                Case "="
                    Select Case triggerDevice.dtype
                        Case DevType.BOOL, DevType.PWM, DevType.RGB, DevType.SERVO, DevType.SENSOR
                            If triggerDevice.devstate_string.ToLower = triggerValue.ToLower Then
                                Return True
                            End If
                            Return False
                    End Select
                Case "<"
                    Select Case triggerDevice.dtype
                        Case DevType.BOOL, DevType.PWM, DevType.SERVO, DevType.SENSOR
                            If Int(triggerDevice.devstate_string.ToLower) < Int(triggerValue.ToLower) Then
                                Return True
                            End If
                        Case Else
                            Return False
                    End Select
                Case "<="
                    Select Case triggerDevice.dtype
                        Case DevType.BOOL, DevType.PWM, DevType.SERVO, DevType.SENSOR
                            If Int(triggerDevice.devstate_string.ToLower) <= Int(triggerValue.ToLower) Then
                                Return True
                            End If
                            Return False
                        Case Else
                            Return False
                    End Select
                Case ">="
                    Select Case triggerDevice.dtype
                        Case DevType.BOOL, DevType.PWM, DevType.SERVO, DevType.SENSOR
                            If Int(triggerDevice.devstate_string.ToLower) >= Int(triggerValue.ToLower) Then
                                Return True
                            End If
                            Return False
                        Case Else
                            Return False

                    End Select
                Case ">"
                    Select Case triggerDevice.dtype
                        Case DevType.BOOL, DevType.PWM, DevType.SERVO, DevType.SENSOR
                            If Int(triggerDevice.devstate_string.ToLower) > Int(triggerValue.ToLower) Then
                                Return True
                            End If
                            Return False
                        Case Else
                            Return False
                    End Select
            End Select
        Catch
        End Try
        Return False
    End Function

End Class
