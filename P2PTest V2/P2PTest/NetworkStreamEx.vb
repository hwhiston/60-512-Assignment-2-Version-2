Imports System.Net.Sockets
Imports System.Net

Public Class NetworkStreamEx
    Inherits NetworkStream

    Private username As String
    Private vector As ArrayList
    Private receive As Boolean
    Private myStream As NetworkStream

    Public Sub New(socket As Socket)
        MyBase.New(socket)
    End Sub

    Public Sub SetVector(user As String, vectorClock As ArrayList)
        Me.username = user
        Me.vector = vectorClock
        Me.receive = False
    End Sub

    Property Stream() As NetworkStream
        Get
            Return Me.myStream
        End Get
        Set(ByVal Value As NetworkStream)
            Me.myStream = Value
        End Set
    End Property
    Property User() As String
        Get
            Return Me.username
        End Get
        Set(ByVal Value As String)
            Me.username = Value
        End Set
    End Property
    Property VectorClock() As ArrayList
        Get
            Return Me.vector
        End Get
        Set(ByVal Value As ArrayList)
            Me.vector = Value
        End Set
    End Property

    Property MayReceive() As Boolean
        Get
            Return Me.receive
        End Get
        Set(ByVal Value As Boolean)
            Me.receive = Value
        End Set
    End Property

End Class
