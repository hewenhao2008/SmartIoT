Imports System.Net, System.Net.Sockets, System.Text
Imports System.Timers
Imports Client.GLOBALS
Imports Client.Message_Processor

Public Class TCP_Handler
    Dim clientSocket As Socket '' private socket for connection
    Dim byteData(1023) As Byte '' initialize byteData array for receiving data
    Dim ping As Timer '' ping timer that will handle ping to server , it's useless data just to keep connection open

    Public Sub Connect(Optional address As String = Nothing, Optional port As Integer = Nothing) 'function connect is connecting to server
        'if parameters are given change globals to paramters else use globals
        If address = Nothing Then
            address = servAddr
        Else
            servAddr = address
        End If
        If port = Nothing Then
            port = servPort
        Else
            servPort = port
        End If
        'try to connect to server 
        Try
            clientSocket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            Dim ipAddress As IPAddress = ipAddress.Parse(address)
            Dim ipEndPoint As IPEndPoint = New IPEndPoint(ipAddress, port)
            clientSocket.BeginConnect(ipEndPoint, New AsyncCallback(AddressOf OnConnect), Nothing)
        Catch ex As SocketException
            'couldn't connect to server abort the program
            elui.Log("Maybe server down or you're not on internet, check your connection and restart application")
            'destroy client socket
            If Not clientSocket Is Nothing Then
                clientSocket.Disconnect(False)
            End If
        End Try

    End Sub
    Private Sub OnConnect(ByVal ar As IAsyncResult)
        Try
            ' we're connected, end connect
            clientSocket.EndConnect(ar)
            'Log successful connect
            elui.Log("Connection successful, logging in ..")
            login(u_id, passwd) ' call login, if we don't login server bans us
            clientSocket.BeginReceive(byteData, 0, byteData.Length, SocketFlags.None, _
                                      New AsyncCallback(AddressOf OnRecieve), clientSocket) 'initialize data receive 
            ping = New Timer '' init system timer 
            ping.Interval = pingInterval '' set it to ping interval
            ping.AutoReset = True 'timer will run forever
            AddHandler ping.Elapsed, AddressOf pingServer 'when tick, pingServer()
            ping.Start() 'start timer
        Catch e As Exception
            clientSocket.Dispose()
            If MessageBox.Show("Error on connection, try again?", "Error", MessageBoxButtons.OKCancel) = DialogResult.OK Then
                elui.Log("Allowed exception while connection") 'log 
                Connect() ' connect again
            End If
        End Try

    End Sub
    Private Sub OnRecieve(ByVal ar As IAsyncResult)
        Try
            If Not clientSocket Is Nothing Then
                clientSocket.EndReceive(ar)
            End If

            'swap bytes into a new array
            Dim bytesRec As Byte() = byteData
            'get the string representation
            Dim message As String = System.Text.ASCIIEncoding.ASCII.GetString(bytesRec)
            'send message to read
            Read(message)
            'check for disconnection
            If message.Length = 1 Then
                If Asc(message) = 0 Then 'if we get message that contains ASCII 0 we can suppose we're disconnected
                    If Not clientSocket Is Nothing Then
                        clientSocket.Disconnect(False)
                    End If
                    Throw New SocketException("Disconnected from server!")
                End If
            End If
            ReDim byteData(0) ' get bytedata to 0 again
            clientSocket.BeginReceive(byteData, 0, byteData.Length, SocketFlags.None, New AsyncCallback(AddressOf OnRecieve), clientSocket) 'recieve again
        Catch e As SocketException
            elui.Log("Error : " & e.Message) 'log error
        End Try

    End Sub
    Private Sub Read(ByVal msg As String)
        Try
            msg = msg.Replace(Chr(0), "") 'we'll relace char ASCII = 0
            ProcessNetMesage(msg) '' message processor is main handler for our reading messages process
        Catch e As Exception
            If Not clientSocket Is Nothing Then
                clientSocket.Disconnect(False)
            End If
            elui.Log("Disconnected while reading from server!") 'log error on disconnect
        End Try

    End Sub
    Public Sub Send(message As String)
        Try
            If Not clientSocket Is Nothing And clientSocket.Connected Then
                message = cp.Encrypt(message) 'encrypt message using aes
                clientSocket.Send(Encoding.ASCII.GetBytes(message & vbNewLine)) 'send it and new line
            End If
        Catch e As Exception
            elui.Log("Unsuccessful sending of data, disconnecting!")
            If Not clientSocket Is Nothing Then
                clientSocket.Disconnect(False)
                clientSocket.Dispose()
                clientSocket = Nothing
            End If

        End Try


    End Sub

    Public Sub Destroy(reuse As Boolean) 'destroy the client socket
        If Not clientSocket Is Nothing Then
            clientSocket.Disconnect(reuse)
            If Not reuse Then
                clientSocket.Dispose()
            End If
        End If
    End Sub

    Private Sub pingServer(sender As Object, e As EventArgs)
        If Not clientSocket Is Nothing And clientSocket.Connected Then
            Me.Send("PING")
        End If


    End Sub
    Private Sub login(uname As String, passwd As String)
        Me.Send(String.Join(":", New String() {uname, passwd, "[]"}))
        logged_in = True
        elui.Log("Logovan sa parametrima : " & String.Join(":", New String() {uname, passwd}))
        dm.Server_Update_Device_List()

    End Sub
    Public Function isConnected()
        If clientSocket.Connected And Not clientSocket Is Nothing Then
            Return True
        End If
        Return False
    End Function
End Class
