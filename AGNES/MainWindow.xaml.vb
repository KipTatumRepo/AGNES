Public Class MainWindow
    Public Shared WCR As New WCRObject
    Public Sub New()
        InitializeComponent()
        MySettings.Default.UserName = Environment.UserName
    End Sub

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Dim WCRStartPage As New WCRHello
        Dim WCRCamPage As New WCRCam
        Dim WCRFinalPage As New WCRFinal
        Hide()
        WCRStartPage.ShowDialog()
        WCRCamPage.ShowDialog()
        WCRFinalPage.ShowDialog()
    End Sub
End Class
