Imports System.Net.Sockets
Imports System.Net
Public Class TcpListenerEx
    Inherits TcpListener

    Public Sub New(localEP As IPEndPoint)
        MyBase.New(localEP)
    End Sub

    Public Sub New(localaddr As IPAddress, port As Integer)
        MyBase.New(localaddr, port)
    End Sub

    Public Shadows ReadOnly Property Active() As Boolean
        Get
            Return MyBase.Active
        End Get
    End Property

End Class
