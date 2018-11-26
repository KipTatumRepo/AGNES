Public Class WCRFinal

#Region "Properties"
    Dim HoverDrop As Effects.DropShadowEffect, LeaveDrop As Effects.DropShadowEffect
    Public Property InBalance As Boolean
    Public Property CancelDueToBalanceIssue As Boolean
    Public Property PrintFailed As Boolean
#End Region

#Region "Constructor"
    Public Sub New()
        InitializeComponent()
        dtpWeekStart.SelectedDate = WCR.WeekStart
        tbFinal.Text = "Okay, we're almost done!  Let's print invoices next - click the button when you're ready."
        HoverDrop = New Effects.DropShadowEffect With {.Color = Color.FromRgb(235, 235, 235), .Direction = 200, .Opacity = 100, .ShadowDepth = 6, .BlurRadius = 2, .RenderingBias = Effects.RenderingBias.Performance}
        LeaveDrop = New Effects.DropShadowEffect With {.Color = Color.FromRgb(235, 235, 235), .Direction = 200, .Opacity = 100, .ShadowDepth = 4, .BlurRadius = 2, .RenderingBias = Effects.RenderingBias.Performance}
    End Sub

#End Region

#Region "Private Methods"

    Private Sub PrintInvoices_Click(sender As Object, e As MouseButtonEventArgs) Handles tbPrintInvoices.MouseDown
        PrintFailed = False
        WCRModule.WCR.PrintInvoices()
        If PrintFailed = True Then Exit Sub
        tbPrintInvoices.Visibility = Visibility.Hidden
        If WCRModule.WCR.InvoicesArePresent > 0 Then
            tbFinal.Text = "Invoices have been created!  Last thing - go ahead and print the WCR backup..."
        Else
            tbFinal.Text = "No invoices are available to print.  If this isn't a mistake, go ahead and print the WCR backup..."
        End If
        tbPrintWCR.Visibility = Visibility.Visible
    End Sub

    Private Sub PrintWCR_Click(sender As Object, e As MouseButtonEventArgs) Handles tbPrintWCR.MouseDown
        PrintFailed = False
        WCRModule.WCR.PrintWCR(Me)
        If PrintFailed = True Then Exit Sub
        If InBalance = True Then
            tbFinal.Text = "That's everything!  The WCR is in balance, but please make sure that you double check the numbers before you enter anything into MyFi - you're on your own from here on out!"
        Else
            If CancelDueToBalanceIssue = False Then
                tbFinal.Text = "That's everything!  The WCR was out of balance, so please make sure that you document the reason for this and double check the numbers before you enter anything into MyFi - you're on your own from here on out!"
            Else
                tbFinal.Text = "You cancelled the WCR final print job because of a balancing issue.  Please come back and see me when you resolve the issue and we try this again!"
            End If
        End If
        tbPrintWCR.Visibility = Visibility.Hidden
        tbClose.Visibility = Visibility.Visible
    End Sub

    Private Sub SoftExitWCR(sender As Object, e As MouseButtonEventArgs) Handles tbClose.MouseDown
        WCRE.SaveChanges()
        ExitModule(0)
    End Sub

    Private Sub HardExitWCR(sender As Object, e As MouseButtonEventArgs) Handles btnExit.MouseDown
        ExitModule(1)
    End Sub

    Private Sub HoverOver(sender As TextBlock, e As MouseEventArgs) Handles tbPrintInvoices.MouseEnter, tbPrintWCR.MouseEnter, tbClose.MouseEnter
        sender.Foreground = New SolidColorBrush(Colors.Blue)
        sender.Effect = HoverDrop
    End Sub

    Private Sub HoverLeave(sender As TextBlock, e As MouseEventArgs) Handles tbPrintInvoices.MouseLeave, tbPrintWCR.MouseLeave, tbClose.MouseLeave
        sender.Foreground = New SolidColorBrush(Colors.Black)
        sender.Effect = LeaveDrop
    End Sub

    Private Sub ExitModule(y)
        If y = 1 Then
            Dim msgtxt As String
            If WCR.CamChecks.Count > 0 Then
                msgtxt = "Close WCR?  Your CAM checks will not be saved."
            Else
                msgtxt = "Close WCR?"
            End If
            Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Small, AgnesMessageBox.MsgBoxLayout.BottomOnly, AgnesMessageBox.MsgBoxType.YesNo,
                                                18,,,, msgtxt)
            amsg.ShowDialog()
            If amsg.ReturnResult = "Yes" Then
                amsg.Close()
                WCRModule.UserClosed = True
                Close()
            Else
                amsg.Close()
            End If
        Else
            WCRModule.UserClosed = False
        End If
        Close()
    End Sub

    Private Sub dtpWeekStart_SelectedDateChanged(sender As Object, e As SelectionChangedEventArgs) Handles dtpWeekStart.SelectedDateChanged
        Dim tempdt As Date = dtpWeekStart.SelectedDate
        If tempdt.DayOfWeek = DayOfWeek.Friday Then WCRModule.WCR.WeekStart = dtpWeekStart.SelectedDate
    End Sub

#End Region

End Class
