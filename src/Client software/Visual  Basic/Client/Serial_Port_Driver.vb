Imports Client.GLOBALS
Imports Client.Message_Processor

Public Class Serial_Port_Driver
    Public Event dataRecived(data As String)
    Public Function Connect(Optional comport_name As String = "COM1", Optional baudRate As Integer = 1000000)
        sp.PortName = comport_name 'port name
        sp.BaudRate = baudRate 'port baudrate
        Try
            If Not sp.IsOpen Then 'connect to device
                sp.Open()
                sp.DiscardInBuffer()
                AddHandler sp.DataReceived, AddressOf DataRecv ''add handler for data recieve
                Return True 'return true, because successful connection
            End If
        Catch ex As Exception
            Return False
        End Try
        Return False
    End Function


    Private Sub DataRecv(sender As Object, e As IO.Ports.SerialDataReceivedEventArgs)
        Dim data As String = sp.ReadLine 'recieve lines
        ProcesSPMessage(data) 'process serial port messages
    End Sub
    Public Sub Send(msg As String)
        sp.WriteLine(msg) 'writeline to serial port 
    End Sub
    Public Sub clearbuffer()
        sp.DiscardInBuffer() 'clear in buffer 
    End Sub
End Class
