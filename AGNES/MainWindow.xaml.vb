Public Class MainWindow
    Public Shared WCR As New WCRObject
    Public Sub New()
        InitializeComponent()
        MySettings.Default.UserName = Environment.UserName
    End Sub

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Dim WCRStartPage As New WCRHello
        Hide()
        WCRStartPage.ShowDialog()
        Dim WCRCamPage As New WCRCam
        WCRCamPage.ShowDialog()
    End Sub
End Class
