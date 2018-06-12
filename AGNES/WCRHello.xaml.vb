Public Class WCRHello
    Shared WCR As New WCRObject
    Private Sub WCRHello_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        tbHello.Text = "Hi, " & MySettings.Default.UserName & "!  It's me, Agnes, and I'll be guiding you through your WCR entry today." & Chr(13) & Chr(13) & "Let's get started by choosing whether we want to look at a past WCR or create a new one!"
    End Sub

    Private Sub CreateNewWCR(sender As Object, e As RoutedEventArgs) Handles btnCreateWCR.Click
        '// Rearrange UI to prompt for Tender Load
        btnCreateWCR.Visibility = Visibility.Hidden
        btnViewWCR.Visibility = Visibility.Hidden
        btnLoadTenders.Visibility = Visibility.Visible
        tbHello.Text = "Sounds good, " & MySettings.Default.UserName & ".  First things first, choose your Sales Tender Summary file and I'll pull in the information."
    End Sub

    Private Sub btnLoadTenders_Click(sender As Object, e As RoutedEventArgs) Handles btnLoadTenders.Click
        WCR.LoadTenders()
    End Sub
End Class
