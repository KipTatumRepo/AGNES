Public Class WCRHello
    Private Property _currentstate As Integer

    Private Sub WCRHello_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        tbHello.Text = "Hi, " & MySettings.Default.UserName & "!  It's me, Agnes, and I'll be guiding you through your WCR entry today." & Chr(13) & Chr(13) & "Let's get started by choosing whether we want to look at a past WCR or create a new one!"
    End Sub

    Private Sub CreateNewWCR(sender As Object, e As RoutedEventArgs) Handles btnCreateWCR.Click
        '// Rearrange UI to prompt for Tender Load
        btnCreateWCR.Visibility = Visibility.Hidden
        btnViewWCR.Visibility = Visibility.Hidden
        btnLoadTenders.Visibility = Visibility.Visible
        Dim wkst As Date = Now().Date : wkst = wkst.AddDays(-1)
        Do Until Weekday(wkst, FirstDayOfWeek.Friday) = 1
            wkst = wkst.AddDays(-1)
        Loop
        tbHello.Text = "Sounds good, " & MySettings.Default.UserName & ".  Let's get started on the WCR for the week starting " & wkst & "." & Chr(13) & Chr(13) & "First things first, choose a vendor's Sales Tender Summary file and I'll pull in the information."
    End Sub

    Private Sub LoadTenderFile(sender As Object, e As RoutedEventArgs) Handles btnLoadTenders.Click, btnAnother.Click
        MainWindow.WCR.LoadTenders(Me)
    End Sub
    Private Sub LoadAnotherTenderFile(sender As Object, e As RoutedEventArgs) Handles btnYes.Click
        btnYes.Visibility = Visibility.Hidden
        btnNo.Visibility = Visibility.Hidden
        btnAnother.Visibility = Visibility.Visible
        btnDone.Visibility = Visibility.Visible
    End Sub

    Public Sub PrintToScreen(ByRef v As VendorObject)
        Dim t As Tender, ttl As Double
        tbHello.FontSize = 12
        For Each t In v.Tenders
            ttl += t.TenderAmt
        Next
        tbHello.Text = "It looks like " & v.VendorName & " has a total of " & FormatCurrency(ttl, 2) & ".  Is this correct?"
        btnLoadTenders.Visibility = Visibility.Hidden
        btnYes.Visibility = Visibility.Visible
        btnNo.Visibility = Visibility.Visible
    End Sub

    Private Sub btnDone_Click(sender As Object, e As RoutedEventArgs) Handles btnDone.Click
        tbHello.FontSize = 30
        btnLoadTenders.Visibility = Visibility.Hidden
        btnYes.Visibility = Visibility.Hidden
        btnNo.Visibility = Visibility.Hidden
        btnAnother.Visibility = Visibility.Hidden
        btnDone.Visibility = Visibility.Hidden
        Close()
    End Sub
End Class
