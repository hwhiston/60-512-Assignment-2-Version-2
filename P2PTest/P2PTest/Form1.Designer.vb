<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.txtChat = New System.Windows.Forms.TextBox()
        Me.btnSend = New System.Windows.Forms.Button()
        Me.lblPort = New System.Windows.Forms.Label()
        Me.txtMessage = New System.Windows.Forms.TextBox()
        Me.SuspendLayout()
        '
        'Timer1
        '
        Me.Timer1.Enabled = True
        '
        'txtChat
        '
        Me.txtChat.Location = New System.Drawing.Point(12, 12)
        Me.txtChat.Multiline = True
        Me.txtChat.Name = "txtChat"
        Me.txtChat.ReadOnly = True
        Me.txtChat.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtChat.Size = New System.Drawing.Size(243, 161)
        Me.txtChat.TabIndex = 0
        '
        'btnSend
        '
        Me.btnSend.Location = New System.Drawing.Point(180, 179)
        Me.btnSend.Name = "btnSend"
        Me.btnSend.Size = New System.Drawing.Size(75, 23)
        Me.btnSend.TabIndex = 1
        Me.btnSend.Text = "Button1"
        Me.btnSend.UseVisualStyleBackColor = True
        '
        'lblPort
        '
        Me.lblPort.Location = New System.Drawing.Point(180, 205)
        Me.lblPort.Name = "lblPort"
        Me.lblPort.Size = New System.Drawing.Size(75, 23)
        Me.lblPort.TabIndex = 2
        '
        'txtMessage
        '
        Me.txtMessage.Location = New System.Drawing.Point(13, 181)
        Me.txtMessage.Name = "txtMessage"
        Me.txtMessage.Size = New System.Drawing.Size(161, 20)
        Me.txtMessage.TabIndex = 3
        '
        'Form1
        '
        Me.AcceptButton = Me.btnSend
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(267, 237)
        Me.Controls.Add(Me.txtMessage)
        Me.Controls.Add(Me.lblPort)
        Me.Controls.Add(Me.btnSend)
        Me.Controls.Add(Me.txtChat)
        Me.Name = "Form1"
        Me.Text = "Form1"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Timer1 As Timer
    Friend WithEvents txtChat As TextBox
    Friend WithEvents btnSend As Button
    Friend WithEvents lblPort As Label
    Friend WithEvents txtMessage As TextBox
End Class
