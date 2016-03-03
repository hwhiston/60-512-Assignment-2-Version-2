Imports System.Threading
Imports System.Runtime.InteropServices

''' <summary>
''' Class represents a user process to be generated
''' </summary>
Public Class UserProcess

    Private id As Integer                       'id of the process
    Private userName As String                  'username of the process
    Private timeStamp As Integer                'timestamp for vectorclock
    Private rand As Integer                     'random integer for sleep timer of listener

    ''' <summary>
    ''' Generate a new user process
    ''' </summary>
    ''' <param name="id">id to assign to process</param>
    Public Sub New(id As Integer)
        Me.id = id
        Me.userName = "P" & id
        Me.timeStamp = 0
    End Sub

#Region "Getters/Setters"
    Property User() As String
        Get
            Return Me.userName
        End Get
        Set(ByVal Value As String)
            Me.userName = Value
        End Set
    End Property

    Property PID() As Integer
        Get
            Return Me.id
        End Get
        Set(ByVal Value As Integer)
            Me.id = Value
        End Set
    End Property

    Property CurrentTimeStamp() As Integer
        Get
            Return Me.timeStamp
        End Get
        Set(ByVal Value As Integer)
            Me.timeStamp = Value
        End Set
    End Property

    Property SleepInterval() As Integer
        Get
            Return Me.rand
        End Get
        Set(ByVal Value As Integer)
            Me.rand = Value
        End Set
    End Property
#End Region

End Class
