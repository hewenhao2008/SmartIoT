Imports System.Net, System.Net.Sockets, System.Text
Imports System.Data.SqlServerCe
Imports System.IO.Ports

Module GLOBALS
#Region "VARS"
    Public comport As String
    Public servAddr As String = "54.210.5.69" 'ip of the server
    Public servPort As Integer = 5000 'serv port to connect to
    Public Const bRate As Integer = 115200 'device baud rate
    Public sp As New SerialPort("COM1", bRate) ' serial port for device
    Public spdriver As New Serial_Port_Driver 'communication with device
    Public cp As New AESCrypto 'crypto provider for aes128
    Public con As SqlCeConnection = New SqlCeConnection("Data Source = DataBase1.sdf") 'connection to database
    Public cmd As SqlCeCommand 'sql command
    Public controller As New Controller(Client.Controller.engine.A_MEGA) ' controller defines with what hardware are we deal
    Public devicelist As New List(Of Device) 'list of all devices in system
    Public dm As New DeviceManager 'global device manager
    Public elui As New Elisium_UI ' global user interface
    Public tcph As New TCP_Handler ' our global tcp handler
    Public logged_in As Boolean = False
    Public pingInterval = 7000 'ping interval for ping function in tcp handler
    Public glob_triggers As New List(Of Trigger)
    Public u_id As String = "1" 'user id for logging in
    Public passwd As String = "test1" ' password for logging in
#End Region
#Region "DEVICE_SPECIFIC_GLOBALS"
    Public devicescreated As Boolean = False 'global holding are all devices created
    Public okawait As Boolean = False 'are we waiting for ok?
#End Region
#Region "SOME_FUNCTIONS"
    Public Function ConvertToRbg(ByVal HexColor As String) As Color 'function for hex to rgb support
        Dim Red As String
        Dim Green As String
        Dim Blue As String
        HexColor = Replace(HexColor, "#", "")
        Red = Val("&H" & Mid(HexColor, 1, 2))
        Green = Val("&H" & Mid(HexColor, 3, 2))
        Blue = Val("&H" & Mid(HexColor, 5, 2))
        Return Color.FromArgb(Red, Green, Blue)
    End Function
#End Region

End Module
