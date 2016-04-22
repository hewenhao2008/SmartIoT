Public Class Controller
    'uno and mega pin configurations

    Dim megaPwmPins As List(Of Integer) = New List(Of Integer)(New Integer() {2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 44, 45, 46})
    Dim unoPwmPins As List(Of Integer) = New List(Of Integer)(New Integer() {3, 5, 6, 9, 10, 11})
    Dim unoAnalogPins As List(Of Integer) = New List(Of Integer)(New Integer() {3, 5, 6, 9, 10})
    Dim megaAnalogPins As List(Of Integer) = New List(Of Integer)(New Integer() {54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 67, 68, 69})
    Public Enum engine
        A_UNO = 0
        A_MEGA = 1
    End Enum
    Private e As engine
    Public Sub New(e As engine)
        Me.e = e
    End Sub
    Public Function isDigital(p As Integer)
        If e = engine.A_MEGA Then
            If p > 1 And p < 54 And Not megaAnalogPins.Contains(p) Then
                Return True
            Else
                Return False
            End If
        ElseIf e = engine.A_UNO Then
            If p > 2 And p < 14 And Not megaAnalogPins(p) Then
                Return True
            Else
                Return False
            End If
        Else
            Throw New Exception("Not valid engine")
        End If
    End Function
    Public Function isPwm(p As Integer)
        If e = engine.A_MEGA Then
            If megaPwmPins.Contains(p) Then
                Return True
            Else
                Return False
            End If
        ElseIf e = engine.A_UNO Then
            If unoPwmPins.Contains(p) Then
                Return True
            Else
                Return False
            End If
        Else
            Throw New Exception("Not valid engine")
        End If
    End Function
    Public Function isAnalog(p As Integer)
        If e = engine.A_MEGA Then
            If megaAnalogPins.Contains(p) Then
                Return True
            Else
                Return False
            End If
        ElseIf e = engine.A_UNO Then
            If unoAnalogPins.Contains(p) Then
                Return True
            Else
                Return False

            End If
        Else
            Throw New Exception("Engine not valid")
        End If
    End Function

End Class
