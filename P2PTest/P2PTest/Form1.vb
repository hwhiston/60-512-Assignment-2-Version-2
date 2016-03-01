Imports System.Net.Sockets
Imports System.Threading
Imports System.IO
Imports System.Net

Public Class Form1

    Dim rand As Integer
    Dim Listener As TcpListener
    Dim Client As New TcpClient
    Dim Message As String = ""
    Dim port As Integer = 8000
    Private data As [Byte]()
    Private stream As NetworkStream

    Private Sub Form1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        txtChat.Text = txtChat.Text & "Server Started" & vbNewLine
        txtChat.Text = txtChat.Text & "Attempting connections..." & vbNewLine

        Try
            Client = New TcpClient("10.71.34.1", port)
        Catch
            txtChat.Text = txtChat.Text & "Cannot locate host. Awaiting connections..." & vbNewLine
        End Try

        If Client.Connected = True Then
            txtChat.Text = txtChat.Text & "I have connected" & vbNewLine
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
                Client = New TcpClient("10.71.34.1", port)
                validPort = True
            Catch
                port = port + 1
            End Try
        End While
    End Sub

    Private Sub Receiver()
        While True
            If Client.Connected Then
                stream = Client.GetStream()
            End If

            'Do
            'If stream.DataAvailable Then
            '
            'Dim r As New BinaryReader(stream)
            '
            'responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes)
            'Console.WriteLine("Received: {0}", r.ReadString())
            'End If
            'Loop
            If Not stream Is Nothing Then

                ' Receive the TcpServer.response.
                ' Buffer to store the response bytes.
                data = New [Byte](256) {}

                ' String to store the response ASCII representation.
                Dim responseData As [String] = [String].Empty

                ' Read the first batch of the TcpServer response bytes.
                Dim bytes As Int32 = stream.Read(data, 0, data.Length)
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes)

                If txtChat.InvokeRequired Then
                    txtChat.Invoke(New AppendTextBoxDelegate(AddressOf AppendTextBox), New Object() {txtChat, txtChat.Text & "Received: {0}" & responseData & vbNewLine})
                Else
                    txtChat.AppendText(txtChat.Text & "Received: {0}" & responseData & vbNewLine)
                End If

            End If
        End While
    End Sub

    Private Delegate Sub AppendTextBoxDelegate(ByVal TB As TextBox, ByVal txt As String)

    Private Sub AppendTextBox(ByVal TB As TextBox, ByVal txt As String)
        txtChat.Text = txt
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSend.Click
        Dim message As String = txtMessage.Text
        txtMessage.Text = ""

        ' Translate the passed message into ASCII and store it as a Byte array.
        data = System.Text.Encoding.ASCII.GetBytes(message)

        ' Get a client stream for reading and writing.
        '  Stream stream = client.GetStream();
        stream = Client.GetStream()

        ' Send the message to the connected TcpServer. 
        stream.Write(data, 0, data.Length)

        txtChat.Text = txtChat.Text & "Sent: {0}" & message & vbNewLine

        'Dim Writer As New StreamWriter(Client.GetStream())
        'Writer.Write(txtChat.Text)
        'Writer.Flush()
    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        lblPort.Text = port
        If Not Listener Is Nothing Then
            Try
                If Listener.Pending = True Then
                    Message = ""

                    txtChat.Text = txtChat.Text & "Connection request pending" & vbNewLine
                    Client = Listener.AcceptTcpClient()

                    'Dim Reader As New StreamReader(Client.GetStream())
                    'While Reader.Peek > -1
                    'Message = Message + Convert.ToChar(Reader.Read()).ToString
                    'End While

                    'MsgBox(Message, MsgBoxStyle.OkOnly)
                End If
            Catch ex As InvalidOperationException
                'txtChat.Text = txtChat.Text & "Listener not initialized" & vbNewLine
            End Try
        End If
    End Sub

    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If Not Listener Is Nothing Then Listener.Stop()
    End Sub

End Class
