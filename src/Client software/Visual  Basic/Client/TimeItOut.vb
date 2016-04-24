Public Class TimeItOut
    Dim timeout_time As Integer = 0
    Dim currenttime As Integer = 0
    Dim mystate As Boolean = False
    Public Sub New(milliseconds As Integer)
        timeout_time = milliseconds
    End Sub
    Public Sub Tick()
        currenttime += 1
        Threading.Thread.Sleep(1)
        If currenttime >= timeout_time Then
            mystate = True
        End If
    End Sub
    Public Function hasTimedOut()
        Return mystate
    End Function
    Public Sub reset()
        currenttime = 0
    End Sub
End Class
