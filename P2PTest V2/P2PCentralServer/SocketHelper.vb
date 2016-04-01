Imports System.Net.Sockets
Imports System.Threading
Imports System.IO
Imports System.Net
Imports System.Text

Class SocketHelper
    Private mscClient As TcpClient
    Private mstrMessage As String
    Private mstrResponse As String
    Private bytesSent() As Byte

    Public Sub processMsg(ByVal client As TcpClient, ByVal stream As NetworkStream, ByVal bytesReceived() As Byte)
        ' Handle the message received and  
        ' send a response back to the client.
        mstrMessage = Encoding.ASCII.GetString(bytesReceived, 0, bytesReceived.Length)
        mscClient = client
        mstrMessage = mstrMessage.Substring(0, 5)
        bytesSent = Encoding.ASCII.GetBytes(mstrResponse)
        stream.Write(bytesSent, 0, bytesSent.Length)

    End Sub
End Class