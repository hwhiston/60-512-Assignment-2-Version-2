Public Class UserMessage

    Private vectorClock As ArrayList
    Private message As [Byte]()
    Private sentBy As String
    Private time As DateTime

    ''' <summary>
    ''' Create new user message
    ''' </summary>
    ''' <param name="user">user who created message</param>
    ''' <param name="message">message contents</param>
    Public Sub New(user As String, message As [Byte](), vectorClock As ArrayList)
        Me.sentBy = user
        Me.message = message
        Me.vectorClock = vectorClock
        Me.time = DateTime.Now
    End Sub

    Public Sub New(user As String, message As [Byte]())
        Me.sentBy = user
        Me.message = message
        Me.time = DateTime.Now
    End Sub

#Region "Getters/Setters"
    Property SentMessage() As [Byte]()
        Get
            Return Me.message
        End Get
        Set(ByVal Value As [Byte]())
            Me.message = Value
        End Set
    End Property

    Property SentByUser() As String
        Get
            Return Me.sentBy
        End Get
        Set(ByVal Value As String)
            Me.sentBy = Value
        End Set
    End Property

    Property SentAtTime() As String
        Get
            Return Me.time.ToString("H:mm:ss")
        End Get
        Set(ByVal Value As String)
            Me.time = Value
        End Set
    End Property

    Property VClock() As ArrayList
        Get
            Return Me.vectorClock
        End Get
        Set(ByVal Value As ArrayList)
            Me.vectorClock = Value
        End Set
    End Property
#End Region

End Class
