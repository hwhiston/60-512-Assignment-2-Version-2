Imports System.Net.Sockets
Imports System.Threading
Imports System.IO
Imports System.Net

Public Class Form1

    Dim rand As Integer
    Dim Listener As TcpListener
    Dim Client As New TcpClient
    Dim Client2 As New TcpClient

    Dim Message As String = ""
    Dim port As Integer = 8000
    Private data As [Byte]()
    Private stream(2) As NetworkStream

    Private Sub Form1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        txtChat.Text = txtChat.Text & "Server Started" & vbNewLine
        txtChat.Text = txtChat.Text & "Attempting connections..." & vbNewLine

        Try
            Client = New TcpClient("10.71.34.1", 8000)
        Catch
            txtChat.Text = txtChat.Text & "Cannot locate host. Awaiting connections..." & vbNewLine
        End Try

        Try
            Client2 = New TcpClient("10.71.34.1", 8001)
        Catch
            txtChat.Text = txtChat.Text & "Cannot locate host. Awaiting connections..." & vbNewLine
        End Try

        If Client.Connected = True Then
            txtChat.Text = txtChat.Text & "I have connected as client1" & vbNewLine
        End If

        If Client2.Connected = True Then
            txtChat.Text = txtChat.Text & "I have connected as client2" & vbNewLine
        End If

        Dim ListThread As New Thread(New ThreadStart(AddressOf Listening))
        ListThread.IsBackground = True
        ListThread.Start()

        Dim ReceiverThread As New Thread(New ThreadStart(AddressOf Receiver))
        ReceiverThread.IsBackground = True
        ReceiverThread.Start()

    End Sub

    Private Sub Listening()
        Dim validPort As Boolean = False

        While validPort = False
            Try
                Listener = New TcpListener(IPAddress.Parse("10.71.34.1"), port)
                Listener.Start()
                validPort = True
            Catch
                port = port + 1
            End Try
        End While
    End Sub

    Private Sub Receiver()
        Dim i As Integer
        While True
            If Client.Connected Then
                stream(1) = Client.GetStream()
            End If

            If Client2.Connected Then
                stream(2) = Client2.GetStream()
            End If

            For i = 1 To 2
                If Not stream(i) Is Nothing Then
                    If stream(i).DataAvailable Then
                        ' Receive the TcpServer.response.
                        ' Buffer to store the response bytes.
                        data = New [Byte](256) {}

                        ' String to store the response ASCII representation.
                        Dim responseData As [String] = [String].Empty

                        ' Read the first batch of the TcpServer response bytes.
                        Dim bytes As Int32 = stream(i).Read(data, 0, data.Length)
                        responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes)

                        If txtChat.InvokeRequired Then
                            txtChat.Invoke(New AppendTextBoxDelegate(AddressOf AppendTextBox), New Object() {txtChat.Text & "Received: {0}" & responseData & vbNewLine})
                        Else
                            txtChat.AppendText(txtChat.Text & "Received: {0}" & responseData & vbNewLine)
                        End If

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
        txtMessage.Text = ""

        ' Translate the passed message into ASCII and store it as a Byte array.
        data = System.Text.Encoding.ASCII.GetBytes(message)

        ' Get a client stream for reading and writing.
        '  Stream stream = client.GetStream();
        stream(1) = Client.GetStream()
        ' Send the message to the connected TcpServer. 
        stream(1).Write(data, 0, data.Length)
        If Client2.Connected Then
            stream(2) = Client2.GetStream()
            stream(2).Write(data, 0, data.Length)
        End If

        txtChat.Text = txtChat.Text & "Sent: {0}" & message & vbNewLine

    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
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
