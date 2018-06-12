Class MainWindow
    Public Sub New()
        InitializeComponent()
        MySettings.Default.UserName = Environment.UserName
    End Sub

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Dim WCRStartPage As New WCRHello
        Hide()
        WCRStartPage.ShowDialog()
    End Sub
End Class
