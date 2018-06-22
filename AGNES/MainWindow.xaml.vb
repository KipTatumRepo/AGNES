Public Class MainWindow
    Public Shared WCR As New WCRObject
    Public Sub New()
        InitializeComponent()
        MySettings.Default.UserName = Environment.UserName
    End Sub

    Private Sub RunWCR(sender As Object, e As RoutedEventArgs) Handles btnWCR.Click
        Hide()
        WCRModule.Runmodule()
        Show()
    End Sub
End Class
