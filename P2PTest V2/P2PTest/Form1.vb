Imports System.Net.Sockets
Imports System.Threading
Imports System.IO
Imports System.Net

Public Class Form1

    Private myHost As String = Dns.GetHostName()
    Private myIP As String = Dns.GetHostEntry(myHost).AddressList(1).ToString()
    Private process As UserProcess
    Private holdBackQueue As ArrayList
    Private vectorClock As ArrayList
    Dim rand As Integer
    Dim Listener As TcpListenerEx
    Dim Client As New TcpClient
    Dim Client2 As New TcpClient
    Dim serverClient As New TcpClient
    Dim Message As String = ""
    Dim port As Integer = 8000
    Private data As [Byte]()
    Private stream(2) As NetworkStream
    Private serverStream As NetworkStream
    Private hasToken As Boolean

    Private Sub Form1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        txtChat.Text = txtChat.Text & "Client Started" & vbNewLine
        txtChat.Text = txtChat.Text & "Attempting connections..." & vbNewLine
        hasToken = False


        'Create vectorclock
        vectorClock = New ArrayList
        holdBackQueue = New ArrayList
        vectorClock.Add(0)
        vectorClock.Add(0)
        vectorClock.Add(0)

        Try
            serverClient = New TcpClient(myIP, 8080)
        Catch
            txtChat.Text = txtChat.Text & "Cannot locate central server." & vbNewLine
        End Try

        'Initialize clients and ports using same ip address
        Try
            Client = New TcpClient(myIP, 8000)
        Catch
            txtChat.Text = txtChat.Text & "Cannot locate host. Awaiting connections..." & vbNewLine
        End Try

        Try
            Client2 = New TcpClient(myIP, 8001)
        Catch
            txtChat.Text = txtChat.Text & "Cannot locate host. Awaiting connections..." & vbNewLine
        End Try

        If serverClient.Connected = True Then
            txtChat.Text = txtChat.Text & "I have connected to central server" & vbNewLine
        End If

        If Client.Connected = True Then
            txtChat.Text = txtChat.Text & "I have connected as client1" & vbNewLine
        End If

        If Client2.Connected = True Then
            txtChat.Text = txtChat.Text & "I have connected as client2" & vbNewLine
        End If

        'Create a listener thread for other clients to connect to
        Dim ListThread As New Thread(New ThreadStart(AddressOf Listening))
        ListThread.IsBackground = True
        ListThread.Start()

        'Create a receiver thread for receiving messages
        Dim ReceiverThread As New Thread(New ThreadStart(AddressOf Receiver))
        ReceiverThread.IsBackground = True
        ReceiverThread.Start()

    End Sub

    Private Sub Listening()
        Dim validPort As Boolean = False

        'Listen for connecting clients and assign a valid port
        While validPort = False
            Try
                Listener = New TcpListenerEx(IPAddress.Parse(myIP), port)
                Listener.Start()
                validPort = True
            Catch
                port = port + 1
            End Try
        End While

        If process Is Nothing Then
            process = New UserProcess(port Mod 10)
        End If

    End Sub

    Private Sub EnterCS()
        'Enter CS for 10 seconds before releasing resource
        Thread.Sleep(10000)
        hasToken = False

        If txtChat.InvokeRequired Then
            txtChat.Invoke(New AppendTextBoxDelegate(AddressOf AppendTextBox), New Object() {txtChat.Text & "[Token released]" & vbNewLine})
        Else
            txtChat.AppendText(txtChat.Text & "[Token released]" & vbNewLine)
        End If

        data = System.Text.Encoding.ASCII.GetBytes("Returning token")
        Dim messageToSend As UserMessage

        messageToSend = New UserMessage(process.User, data, vectorClock)

        'Try catch for returning token if server crashes before getting token
        Try
            If serverClient.Connected = True Then
                If Not serverStream Is Nothing Then
                    serverStream = serverClient.GetStream()
                    ' Send the message to the connected TcpServer. 
                    serverStream.Write(data, 0, data.Length)
                End If
            End If
        Catch
            If txtChat.InvokeRequired Then
                txtChat.Invoke(New AppendTextBoxDelegate(AddressOf AppendTextBox), New Object() {txtChat.Text & "[Could not detect server, token still released]" & vbNewLine})
            Else
                txtChat.AppendText(txtChat.Text & "[Could not detect server,  token still released]" & vbNewLine)
            End If
        End Try

    End Sub

    Private Sub Receiver()
        Dim i As Integer
        Dim j As Integer
        Dim k As Integer
        Dim messageString As String

        While True
            If Client.Connected Then
                stream(1) = Client.GetStream()
            End If

            If Client2.Connected Then
                stream(2) = Client2.GetStream()
            End If

            If serverClient.Connected = True Then
                serverStream = serverClient.GetStream()
            End If

            If Not serverStream Is Nothing Then
                If serverStream.DataAvailable Then
                    'Variable to store bytes received
                    data = New [Byte](256) {}

                    'Variable to store string representation
                    Dim receivedData As [String] = [String].Empty

                    'Read in the received bytes
                    Dim bytes As Int32 = serverStream.Read(data, 0, data.Length)
                    receivedData = System.Text.Encoding.ASCII.GetString(data, 0, bytes)
                    If receivedData = "True" Then
                        messageString = "[Token received, may enter CS]"
                        hasToken = True
                        'Create a listener thread for other clients to connect to
                        Dim CSThread As New Thread(New ThreadStart(AddressOf EnterCS))
                        CSThread.IsBackground = True
                        CSThread.Start()
                    ElseIf receivedData = "[ACK]" Then
                        messageString = "[Token returned successfully]"
                        hasToken = False
                    Else
                        messageString = "[Token cannot be received]"
                    End If
                    If txtChat.InvokeRequired Then
                        txtChat.Invoke(New AppendTextBoxDelegate(AddressOf AppendTextBox), New Object() {txtChat.Text & messageString & vbNewLine})
                    Else
                        txtChat.AppendText(txtChat.Text & messageString & vbNewLine)
                    End If

                    If serverStream.DataAvailable Then
                        Dim Buffer As [Byte]()
                        Buffer = New [Byte](256) {}
                        While serverStream.DataAvailable
                            serverStream.Read(Buffer, 0, Buffer.Length)
                        End While
                    End If
                End If

            End If

            For i = 1 To 2
                If Not stream(i) Is Nothing Then
                    If stream(i).DataAvailable Then
                        'Variable to store bytes received
                        data = New [Byte](256) {}

                        'Variable to store string representation
                        Dim receivedData As [String] = [String].Empty

                        'Read in the received bytes
                        Dim bytes As Int32 = stream(i).Read(data, 0, data.Length)
                        receivedData = System.Text.Encoding.ASCII.GetString(data, 0, bytes)
                        'Causal ordering
                        For j = 0 To vectorClock.Count - 1
                            'Check if the vectorclock was incremented
                            If receivedData.Substring(j, 1) = (vectorClock(j) + 1).ToString Then
                                For k = 0 To vectorClock.Count - 1
                                    'If the vectorclock of other processes in incremented, we know that there was a change
                                    If k <> j And vectorClock(k) >= receivedData.Substring(k, 1) Then
                                        If txtChat.InvokeRequired Then
                                            txtChat.Invoke(New AppendTextBoxDelegate(AddressOf AppendTextBox), New Object() {txtChat.Text & "[Received:] " & receivedData & vbNewLine})
                                        Else
                                            txtChat.AppendText(txtChat.Text & "[Received:] " & receivedData & vbNewLine)
                                        End If
                                        GoTo EndOfFor
                                    Else
                                        holdBackQueue.Add(receivedData & vbNewLine)
                                    End If
                                Next
EndOfFor:
                                'Set vectorclock to max of current vectorclock or vectorclock received
                                If vectorClock(j) < receivedData.Substring(j, 1) Then
                                    vectorClock(j) = receivedData.Substring(j, 1)
                                End If
                            End If
                        Next

                        'stream.Flush()

                        If stream(i).DataAvailable Then
                            Dim Buffer As [Byte]()
                            Buffer = New [Byte](256) {}
                            While stream(i).DataAvailable
                                stream(i).Read(Buffer, 0, Buffer.Length)
                            End While
                        End If
                    End If
                End If
            Next

        End While
    End Sub

    Private Delegate Sub AppendTextBoxDelegate(ByVal txt As String)

    Private Sub AppendTextBox(ByVal txt As String)
        txtChat.Text = txt
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSend.Click
        Dim message As String = txtMessage.Text
        Dim messageToSend As UserMessage
        txtMessage.Text = ""

        If message = "" Then
            Exit Sub
        End If

        ' Translate the passed message into ASCII and store it as a Byte array.
        vectorClock(process.PID) = vectorClock(process.PID) + 1
        data = System.Text.Encoding.ASCII.GetBytes(vectorClock(0) & vectorClock(1) & vectorClock(2) & " " & process.User & " says " & message)
        messageToSend = New UserMessage(process.User, data, vectorClock)

        If Client.Connected Then
            ' Get a client stream for reading and writing.
            '  Stream stream = client.GetStream();
            If Not stream(1) Is Nothing Then
                stream(1) = Client.GetStream()
                ' Send the message to the connected TcpServer. 
                stream(1).Write(data, 0, data.Length)
            End If
        End If
        If Client2.Connected Then
            If Not stream(2) Is Nothing Then
                stream(2) = Client2.GetStream()
                stream(2).Write(data, 0, data.Length)
            End If
        End If

        txtChat.Text = txtChat.Text & "[Sent:] " & message & vbNewLine

    End Sub

    Private Sub btnReqTok_Click(sender As Object, e As EventArgs) Handles btnReqTok.Click

        If hasToken Then
            txtChat.Text = txtChat.Text & "[Token already received] " & vbNewLine
            Return
        End If

        data = System.Text.Encoding.ASCII.GetBytes("Requesting token")
        Dim messageToSend As UserMessage

        messageToSend = New UserMessage(process.User, data, vectorClock)

        If serverClient.Connected = True Then
            If Not serverStream Is Nothing Then
                serverStream = serverClient.GetStream()
                ' Send the message to the connected TcpServer. 
                serverStream.Write(data, 0, data.Length)
            End If
        End If

        txtChat.Text = txtChat.Text & "[Token request sent] " & vbNewLine
    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        If process Is Nothing Then
            lblUsername.Text = "Connecting..."
            txtMessage.Enabled = False
        Else
            lblUsername.Text = process.User
            txtMessage.Enabled = True
        End If

        lblPort.Text = port

        If Not Listener Is Nothing Then
            Try
                If Listener.Pending = False Then
                    Exit Sub
                End If
            Catch ex As InvalidOperationException
                Exit Sub
                'txtChat.Text = txtChat.Text & "Listener not initialized" & vbNewLine
            End Try
            txtChat.Text = txtChat.Text & "Connection request pending" & vbNewLine

            If serverClient.Connected = False Then
                serverClient = Listener.AcceptTcpClient()
            End If

            Try
                If Client.Connected Then
                    If Not Client2 Is Nothing Then
                        Client2 = Listener.AcceptTcpClient()
                    End If
                End If
            Catch

            End Try
            If Client.Connected = False Then
                Client = Listener.AcceptTcpClient()
            End If

        End If
    End Sub

    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        'Stop the listeners and streams
        If Not Listener Is Nothing Then
            Listener.Stop()
            stream(1).Close()
            Client.Close()
            If Client2.Connected Then
                stream(2).Close()
                Client2.Close()
            End If
        End If
    End Sub

End Class
