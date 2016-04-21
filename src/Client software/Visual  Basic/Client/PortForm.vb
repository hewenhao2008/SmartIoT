Imports Client.GLOBALS

Public Class PortForm
    Sub RefreshSerialNames()
        portNames.Items.Clear() '' clear combo box portNames 
        For Each sp As String In My.Computer.Ports.SerialPortNames ''add all the 
            portNames.Items.Add(sp)
        Next
        portNames.Refresh()
    End Sub
    Private Sub ChoosePort_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        RefreshSerialNames()
    End Sub
    Private Sub btnRefresh_Click(sender As System.Object, e As System.EventArgs) Handles btnRefresh.Click
        RefreshSerialNames()
    End Sub
    Private Sub btnNastavi_Click(sender As System.Object, e As System.EventArgs) Handles btnContinue.Click
        If Not portNames.SelectedItem = "" Then
            comport = portNames.SelectedItem.ToString
            elui.Show()
            Me.Hide()
        End If
    End Sub

    Private Sub Panel1_Paint(sender As System.Object, e As System.Windows.Forms.PaintEventArgs) Handles Panel1.Paint

    End Sub
End Class