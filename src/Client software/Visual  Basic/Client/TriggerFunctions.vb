Module TriggerFunctions
    Public functionList As New List(Of String)(New String() {"blink"})
    Public Function functionExists(function_name As String)
        If functionList.Contains(function_name.ToLower) Then
            Return True
        End If
        Return False
    End Function
    Public Sub getFuncByName(function_name As String, params() As Object)
        Select Case function_name.ToLower
            Case "blink"
                blink(params)
        End Select

    End Sub
    Sub blink(params() As Object)
        If params.Length = 1 Then
            Dim dev As Device = params(0)
            dev.changeState(dev.devid, "1")
            Threading.Thread.Sleep(1000)
            dev.changeState(dev.devid, "0")
            Threading.Thread.Sleep(1000)
        End If
    End Sub
End Module