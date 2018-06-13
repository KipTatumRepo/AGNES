Imports System.Windows.Threading

Public Class WCRCam
    Dim dt As DispatcherTimer = New DispatcherTimer()
    Dim dt2 As DispatcherTimer = New DispatcherTimer()
    Public Sub New()
        InitializeComponent()
        Dim a As New Animation.DoubleAnimation
        a.From = 432
        a.To = 120
        a.Duration = New Duration(TimeSpan.FromSeconds(1))
        imgAGNES.BeginAnimation(Image.HeightProperty, a)
        AddHandler dt.Tick, AddressOf PauseForMinimizing
        dt.Interval = New TimeSpan(0, 0, 2)
        dt.Start()
    End Sub
    Public Sub PauseForMinimizing(ByVal sender As Object, ByVal e As EventArgs)
        CommandManager.InvalidateRequerySuggested()
        tbCam.Text = "Okay!  Let's move on to CAM checks.  I'll hang out down here so I'm not in your way, but I'll continue to walk you through the process." & Chr(13) & Chr(13) & "If you need additional help, just tap me on the shoulder with your cursor!"
        dt.Stop()
        AddHandler dt2.Tick, AddressOf PauseBeforeCamChecks
        dt2.Interval = New TimeSpan(0, 0, 6)
        dt2.Start()
    End Sub
    Public Sub PauseBeforeCamChecks(ByVal sender As Object, ByVal e As EventArgs)
        CommandManager.InvalidateRequerySuggested()
        tbCam.Text = "Did you have any CAM checks to enter today?"
        dt2.Stop()
        btnYesCam.Visibility = Visibility.Visible
        btnNo.Visibility = Visibility.Visible
    End Sub

    Private Sub CamComplete(sender As Object, e As RoutedEventArgs) Handles btnDone.Click, btnNo.Click
        Close()
        MsgBox("All done")
    End Sub
End Class
