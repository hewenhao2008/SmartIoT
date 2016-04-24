Imports System.Data.SqlServerCe
Imports Client.GLOBALS
Public Class Elisium_UI
    Delegate Sub _Log(msg As String)
    Private Sub frmClient_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        tcph.Connect() 'connect on one thread..it's async
        'login would be global that the client would change to True when logged in

        If Not comport = Nothing Then 'if the port is chosen, and it must be, try to connect to device
            spdriver.Connect(comport)
        End If
        dm.Populate_Device_List() 'after we connected to server, connected to device, populate device list from Database using DeviceManager class..

    End Sub


    Private Sub frmClient_FormClosing(sender As Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing '' close all
        Try
            tcph.Destroy(False) '' destroy tcp client
            dm.Close() 'close device manager
            dm.Dispose() 'dispose it
            Me.Dispose() 'dispose this form
            PortForm.Close() 'close initial form
        Catch ex As Exception
        End Try


    End Sub


    Private Sub ShowDeviceManager_Click(sender As System.Object, e As System.EventArgs) Handles showDevMgr.Click
        dm.Show() '' show device manager
    End Sub

    Private Sub SendToDevice_Click(sender As System.Object, e As System.EventArgs) Handles btnSendToDevice.Click
        spdriver.Send(txtCmd.Text) ' send to serial port 
    End Sub
    Public Sub Log(msg As String)
        If InvokeRequired Then
            Invoke(New _Log(AddressOf Log), msg) ' if called from another thread invoke delegate
            Exit Sub
        End If
        txtLog.Text &= msg & vbCrLf '' add text to text log

    End Sub



    
End Class
'
'there still needs to be implemented database handler class, so we could insert devices , delete devices and modify devices..
'