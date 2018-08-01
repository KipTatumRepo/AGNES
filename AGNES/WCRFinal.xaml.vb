Public Class WCRFinal
    Public Sub New()
        InitializeComponent()
        tbFinal.Text = "Okay, we're almost done!  Let's print invoices next - click the button when you're ready."
    End Sub

    Private Sub btnPrintInvoices_Click(sender As Object, e As RoutedEventArgs) Handles btnPrintInvoices.Click
        WCRModule.WCR.PrintInvoices()
        btnPrintInvoices.Visibility = Visibility.Hidden
        If WCRModule.WCR.InvoicesArePresent > 0 Then
            tbFinal.Text = "Invoices have been created!  Last thing - go ahead and print the WCR backup..."
        Else
            tbFinal.Text = "No invoices are available to print.  If this isn't a mistake, go ahead and print the WCR backup..."
        End If
        btnPrintWCR.Visibility = Visibility.Visible
    End Sub

    Private Sub btnPrintWCR_Click(sender As Object, e As RoutedEventArgs) Handles btnPrintWCR.Click
        WCRModule.WCR.PrintWCR()
        tbFinal.Text = "That's everything!  Make sure that you double check the numbers before you enter anything into MyFi - you're on your own from here on out!"
        btnPrintWCR.Visibility = Visibility.Hidden
        btnClose.Visibility = Visibility.Visible
    End Sub

    Private Sub SoftExitWCR(sender As Object, e As RoutedEventArgs) Handles btnClose.Click
        ExitModule(0)
    End Sub

    Private Sub HardExitWCR(sender As Object, e As MouseButtonEventArgs) Handles btnExit.MouseDown
        ExitModule(1)
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
