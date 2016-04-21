<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Elisium_UI
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.txtLog = New System.Windows.Forms.RichTextBox()
        Me.showDevMgr = New System.Windows.Forms.Button()
        Me.btnSendToDevice = New System.Windows.Forms.Button()
        Me.txtCmd = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'txtLog
        '
        Me.txtLog.Location = New System.Drawing.Point(31, 98)
        Me.txtLog.Name = "txtLog"
        Me.txtLog.Size = New System.Drawing.Size(721, 322)
        Me.txtLog.TabIndex = 1
        Me.txtLog.Text = ""
        '
        'showDevMgr
        '
        Me.showDevMgr.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.showDevMgr.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.showDevMgr.Location = New System.Drawing.Point(31, 12)
        Me.showDevMgr.Name = "showDevMgr"
        Me.showDevMgr.Size = New System.Drawing.Size(287, 72)
        Me.showDevMgr.TabIndex = 4
        Me.showDevMgr.Text = "Show device manager"
        Me.showDevMgr.UseVisualStyleBackColor = True
        '
        'btnSendToDevice
        '
        Me.btnSendToDevice.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnSendToDevice.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSendToDevice.Location = New System.Drawing.Point(564, 443)
        Me.btnSendToDevice.Name = "btnSendToDevice"
        Me.btnSendToDevice.Size = New System.Drawing.Size(204, 57)
        Me.btnSendToDevice.TabIndex = 5
        Me.btnSendToDevice.Text = "Send to device"
        Me.btnSendToDevice.UseVisualStyleBackColor = True
        '
        'txtCmd
        '
        Me.txtCmd.Location = New System.Drawing.Point(239, 470)
        Me.txtCmd.Name = "txtCmd"
        Me.txtCmd.Size = New System.Drawing.Size(301, 20)
        Me.txtCmd.TabIndex = 6
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(26, 465)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(208, 25)
        Me.Label1.TabIndex = 7
        Me.Label1.Text = "Command to device:"
        '
        'Elisium_UI
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(780, 512)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.txtCmd)
        Me.Controls.Add(Me.btnSendToDevice)
        Me.Controls.Add(Me.showDevMgr)
        Me.Controls.Add(Me.txtLog)
        Me.Name = "Elisium_UI"
        Me.Text = "Form1"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents txtLog As System.Windows.Forms.RichTextBox
    Friend WithEvents showDevMgr As System.Windows.Forms.Button
    Friend WithEvents btnSendToDevice As System.Windows.Forms.Button
    Friend WithEvents txtCmd As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label

End Class
