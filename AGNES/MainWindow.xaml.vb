Public Class MainWindow
    Public Shared WCR As New WCRObject
    Public Sub New()
        InitializeComponent()
        MySettings.Default.UserName = Environment.UserName
    End Sub

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Hide()
        Dim WCRStartPage As New WCRHello
        WCRStartPage.ShowDialog()
        Dim WCRCamPage As New WCRCam
        WCRCamPage.ShowDialog()
        Dim WCRFinalPage As New WCRFinal
        WCRFinalPage.ShowDialog()
    End Sub
End Class
