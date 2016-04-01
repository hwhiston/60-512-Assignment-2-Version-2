Imports System.Net.Sockets
Imports System.Threading
Imports System.IO
Imports System.Net

Public Class frmCentralServer

    Private myHost As String = Dns.GetHostName()
    Private myIP As String = Dns.GetHostEntry(myHost).AddressList(1).ToString()
    Dim port As Integer = 8080
    Dim client1 As TcpClient
    Dim client2 As TcpClient
    Dim Listener As TcpListenerEx
    Private data As [Byte]()
    Dim stream As NetworkStream
    Dim hasToken As Boolean

    Private Sub frmCentralServer_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        txtChat.Text = txtChat.Text & "Central Server Started" & vbNewLine
        txtChat.Text = txtChat.Text & "Awaiting connections..." & vbNewLine
        GenerateToken()

        'Create a listener thread for other clients to connect to
        Dim ListThread As New Thread(New ThreadStart(AddressOf Listening))
        ListThread.IsBackground = True
        ListThread.Start()

        'Create a receiver thread for receiving messages
        Dim ReceiverThread As New Thread(New ThreadStart(AddressOf Receiver))
        ReceiverThread.IsBackground = True
        ReceiverThread.Start()
    End Sub

    Private Sub GenerateToken()
        hasToken = True
    End Sub

    Private Sub Listening()

        Listener = New TcpListenerEx(IPAddress.Parse(myIP), port)
        Listener.Start()

    End Sub

    Private Sub Receiver()
        While True
            ReceiveFromClient(client1)
            ReceiveFromClient(client2)


        End While
        'Dim helper As New SocketHelper()
        'helper.processMsg(client1, stream, data)
    End Sub

    Sub ReceiveFromClient(client As TcpClient)
        Dim messageString As String
        data = New [Byte](256) {}
        Dim receivedData As String = ""

        ' Read the data stream from the client.
        If Not client Is Nothing Then
            If client.Connected Then
                stream = client.GetStream()
                If stream.DataAvailable Then

                    Dim bytes As Int32 = stream.Read(data, 0, data.Length)
                    receivedData = System.Text.Encoding.ASCII.GetString(data, 0, bytes)
                    If receivedData = "Requesting token" Then
                        If hasToken Then
                            messageString = "Token request received, sending token"
                        Else
                            messageString = "Token request received, cannot send token as it was sent to another process"
                        End If


                        If txtChat.InvokeRequired Then
                            txtChat.Invoke(New AppendTextBoxDelegate(AddressOf AppendTextBox), New Object() {txtChat.Text & messageString & vbNewLine})
                        Else
                            txtChat.AppendText(txtChat.Text & messageString & vbNewLine)
                        End If

                        data = System.Text.Encoding.ASCII.GetBytes(hasToken.ToString)

                        'Send token
                        If client.Connected Then
                            ' Get a client stream for reading and writing.
                            '  Stream stream = client.GetStream();
                            If Not stream Is Nothing Then
                                stream = client.GetStream()
                                ' Send the message to the connected TcpServer. 
                                hasToken = False
                                stream.Write(data, 0, data.Length)
                            End If
                        End If
                    ElseIf receivedData = "Returning token" Then
                        messageString = "Token returned succesfully"
                        hasToken = True

                        If txtChat.InvokeRequired Then
                            txtChat.Invoke(New AppendTextBoxDelegate(AddressOf AppendTextBox), New Object() {txtChat.Text & messageString & vbNewLine})
                        Else
                            txtChat.AppendText(txtChat.Text & messageString & vbNewLine)
                        End If

                        data = System.Text.Encoding.ASCII.GetBytes("[ACK]")

                        'Send token
                        If client.Connected Then
                            ' Get a client stream for reading and writing.
                            '  Stream stream = client.GetStream();
                            If Not stream Is Nothing Then
                                stream = client.GetStream()
                                ' Send the message to the connected TcpServer. 

                                stream.Write(data, 0, data.Length)
                            End If
                        End If
                    End If
                End If
            End If
        End If
    End Sub

    Private Delegate Sub AppendTextBoxDelegate(ByVal txt As String)

    Private Sub AppendTextBox(ByVal txt As String)
        txtChat.Text = txt
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        If Not Listener Is Nothing Then
            Try
                If Listener.Pending = False Then
                    Exit Sub
                End If
            Catch ex As InvalidOperationException
                Exit Sub
                'txtChat.Text = txtChat.Text & "Listener not initialized" & vbNewLine
            End Try
            txtChat.Text = txtChat.Text & "Client connection request pending" & vbNewLine

            If client1 Is Nothing Then
                client1 = Listener.AcceptTcpClient()
                Listener.Start()
            End If

            If client2 Is Nothing Then
                client2 = Listener.AcceptTcpClient()
            End If
        End If

    End Sub
End Class
