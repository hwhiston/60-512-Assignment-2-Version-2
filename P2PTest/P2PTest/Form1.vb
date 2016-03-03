Imports System.Net.Sockets
Imports System.Threading
Imports System.IO
Imports System.Net

Public Class Form1

    Private process As UserProcess
    Private holdBackQueue As ArrayList
    Private vectorClock As ArrayList
    Dim rand As Integer
    Dim Listener As TcpListenerEx
    Dim Client As New TcpClient
    Dim Client2 As New TcpClient

    Dim Message As String = ""
    Dim port As Integer = 8000
    Private data As [Byte]()
    Private stream(2) As NetworkStream

    Private Sub Form1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        txtChat.Text = txtChat.Text & "Server Started" & vbNewLine
        txtChat.Text = txtChat.Text & "Attempting connections..." & vbNewLine

        vectorClock = New ArrayList
        holdBackQueue = New ArrayList
        vectorClock.Add(0)
        vectorClock.Add(0)
        vectorClock.Add(0)

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
                Listener = New TcpListenerEx(IPAddress.Parse("10.71.34.1"), port)
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

    Private Sub Receiver()
        Dim i As Integer
        Dim j As Integer
        Dim k As Integer

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
                        For j = 0 To vectorClock.Count - 1
                            If responseData.Substring(j, 1) = (vectorClock(j) + 1).ToString Then
                                For k = 0 To vectorClock.Count - 1
                                    If k <> j And vectorClock(k) >= responseData.Substring(k, 1) Then
                                        If txtChat.InvokeRequired Then
                                            txtChat.Invoke(New AppendTextBoxDelegate(AddressOf AppendTextBox), New Object() {txtChat.Text & "[Received:] " & responseData & vbNewLine})
                                        Else
                                            txtChat.AppendText(txtChat.Text & "[Received:] " & responseData & vbNewLine)
                                        End If
                                        GoTo EndOfFor
                                    End If
                                Next
EndOfFor:
                                If vectorClock(j) < responseData.Substring(j, 1) Then
                                    vectorClock(j) = responseData.Substring(j, 1)
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

    Private Function MaxArrayList(vector1 As ArrayList, vector2 As ArrayList) As ArrayList
        Dim i As Integer

        For Each i In vector1
            If vector1(i) > vector2(i) Then
                MaxArrayList = vector1
                Exit Function
            End If
        Next

        MaxArrayList = vector2

    End Function

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        If Not process Is Nothing Then
            lblUsername.Text = process.User
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
