Public Class WCRHello
    'TODO: ADJUST WCR TENDER PULL ROUTINE TO INCLUDE PARAMETER FOR VENDOR TYPE (AS COMMONS VENDOR ONLY - TYPE 0); CONFLICT WITH VENDORS WHO ARE ALSO BRANDS
#Region "Properties"
    Private Property _currentstate As Integer
    Private ActiveWCR As WCRObject = WCRModule.WCR
    Dim HoverDrop As Effects.DropShadowEffect, LeaveDrop As Effects.DropShadowEffect
#End Region

#Region "Public Methods"
    Public Sub TenderLoadComplete(totalfiles, badfiles)
        If badfiles = 0 Then
            tbHello.Text = "All selected tenders have been loaded"
        Else
            tbHello.Text = (totalfiles - badfiles) & " of the " & totalfiles & " tenders you selected have been loaded."
        End If

        tbLoadTenders.Visibility = Visibility.Hidden
        tbAnother.Visibility = Visibility.Visible
        tbDone.Visibility = Visibility.Visible
    End Sub
#End Region

#Region "Private Methods"
    Private Sub WCRHello_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        tbHello.Text = "Hi, " & My.Settings.UserShortName & "!  It's me, Agnes, and I'll be guiding you through your WCR entry today." & Chr(13) & Chr(13) & "Let's get started by choosing whether we want to look at a past WCR or create a new one!"
        tbCreateWCR.Visibility = Visibility.Visible
        HoverDrop = New Effects.DropShadowEffect With {.Color = Color.FromRgb(235, 235, 235), .Direction = 200, .Opacity = 100, .ShadowDepth = 6, .BlurRadius = 2, .RenderingBias = Effects.RenderingBias.Performance}
        LeaveDrop = New Effects.DropShadowEffect With {.Color = Color.FromRgb(235, 235, 235), .Direction = 200, .Opacity = 100, .ShadowDepth = 4, .BlurRadius = 2, .RenderingBias = Effects.RenderingBias.Performance}
    End Sub

    Private Sub CreateNewWCR(sender As Object, e As MouseEventArgs) Handles tbCreateWCR.MouseDown
        tbCreateWCR.Visibility = Visibility.Hidden
        tbLoadTenders.Visibility = Visibility.Visible
        Dim wkst As Date = Now().Date : wkst = wkst.AddDays(-1)
        Do Until Weekday(wkst, FirstDayOfWeek.Friday) = 1
            wkst = wkst.AddDays(-1)
        Loop
        tbHello.Text = "Sounds good, " & My.Settings.UserShortName & ".  Let's get started on the WCR for the week starting " & wkst & "." & Chr(13) & Chr(13) & "First things first, choose a vendor's Sales Tender Summary file and I'll pull in the information."
        ActiveWCR.WeekStart = wkst
    End Sub

    Private Sub LoadTenderFile(sender As Object, e As MouseEventArgs) Handles tbLoadTenders.MouseDown, tbAnother.MouseDown
        WCRModule.WCR.LoadTenders(Me, tbHello)
    End Sub

    Private Sub btnDone_Click(sender As Object, e As MouseEventArgs) Handles tbDone.MouseDown
        tbHello.FontSize = 30
        tbLoadTenders.Visibility = Visibility.Hidden
        tbAnother.Visibility = Visibility.Hidden
        tbDone.Visibility = Visibility.Hidden
        Close()
    End Sub

    Private Sub HoverOver(sender As TextBlock, e As MouseEventArgs) Handles tbCreateWCR.MouseEnter, tbAnother.MouseEnter, tbDone.MouseEnter, tbLoadTenders.MouseEnter
        sender.Foreground = New SolidColorBrush(Colors.Blue)
        sender.Effect = HoverDrop
    End Sub

    Private Sub HoverLeave(sender As TextBlock, e As MouseEventArgs) Handles tbCreateWCR.MouseLeave, tbAnother.MouseLeave, tbDone.MouseLeave, tbLoadTenders.MouseLeave
        sender.Foreground = New SolidColorBrush(Colors.Black)
        sender.Effect = LeaveDrop
    End Sub

    Private Sub ExitWCR(sender As Object, e As MouseButtonEventArgs) Handles btnExit.MouseDown
        Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Small, AgnesMessageBox.MsgBoxLayout.BottomOnly, AgnesMessageBox.MsgBoxType.YesNo,
                                                18,,,, "Close WCR?")
        amsg.ShowDialog()
        If amsg.ReturnResult = "Yes" Then
            amsg.Close()
            WCRModule.UserClosed = True
            Close()
        Else
            amsg.Close()
        End If
    End Sub

#End Region

End Class