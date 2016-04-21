<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DeviceManager
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
        Me.Devices = New System.Windows.Forms.FlowLayoutPanel()
        Me.SuspendLayout()
        '
        'Devices
        '
        Me.Devices.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.Devices.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.Devices.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Devices.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Devices.FlowDirection = System.Windows.Forms.FlowDirection.TopDown
        Me.Devices.Location = New System.Drawing.Point(0, 0)
        Me.Devices.Name = "Devices"
        Me.Devices.Size = New System.Drawing.Size(698, 479)
        Me.Devices.TabIndex = 0
        '
        'DeviceManager
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(698, 479)
        Me.Controls.Add(Me.Devices)
        Me.Name = "DeviceManager"
        Me.Text = "DeviceManager"
        Me.ResumeLayout(False)

    End Sub
    Public WithEvents Devices As System.Windows.Forms.FlowLayoutPanel
End Class
