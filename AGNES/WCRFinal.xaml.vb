Public Class WCRFinal
    Public Sub New()
        InitializeComponent()
        tbFinal.Text = "Okay, we're almost done!  Let's print invoices next - click the button when you're ready."
    End Sub

    Private Sub PrintInvoices_Click(sender As Object, e As MouseButtonEventArgs) Handles tbPrintInvoices.MouseDown
        WCRModule.WCR.PrintInvoices()
        tbPrintInvoices.Visibility = Visibility.Hidden
        If WCRModule.WCR.InvoicesArePresent > 0 Then
            tbFinal.Text = "Invoices have been created!  Last thing - go ahead and print the WCR backup..."
        Else
            tbFinal.Text = "No invoices are available to print.  If this isn't a mistake, go ahead and print the WCR backup..."
        End If
        tbPrintWCR.Visibility = Visibility.Visible
    End Sub

    Private Sub PrintWCR_Click(sender As Object, e As MouseButtonEventArgs) Handles tbPrintWCR.MouseDown
        WCRModule.WCR.PrintWCR()
        tbFinal.Text = "That's everything!  Make sure that you double check the numbers before you enter anything into MyFi - you're on your own from here on out!"
        tbPrintWCR.Visibility = Visibility.Hidden
        tbClose.Visibility = Visibility.Visible
    End Sub

    Private Sub SoftExitWCR(sender As Object, e As MouseButtonEventArgs) Handles tbClose.MouseDown
        ExitModule(0)
    End Sub

    Private Sub HardExitWCR(sender As Object, e As MouseButtonEventArgs) Handles btnExit.MouseDown
        ExitModule(1)
    End Sub

    Private Sub HoverOver(sender As TextBlock, e As MouseEventArgs) Handles tbPrintInvoices.MouseEnter, tbPrintWCR.MouseEnter, tbClose.MouseEnter
        sender.Foreground = New SolidColorBrush(Colors.Blue)
    End Sub
    Private Sub HoverLeave(sender As TextBlock, e As MouseEventArgs) Handles tbPrintInvoices.MouseLeave, tbPrintWCR.MouseLeave, tbClose.MouseLeave
        sender.Foreground = New SolidColorBrush(Colors.Black)
    End Sub
    Private Sub ExitModule(y)
        'TODO: ADD APPLICATION STYLE MESSAGEBOX
        If y = 1 Then
            Dim yn As MsgBoxResult = MsgBox("Close WCR?", vbYesNo)
            If yn = vbNo Then Exit Sub
        End If
        WCRModule.UserClosed = True
        Close()
    End Sub

End Class
