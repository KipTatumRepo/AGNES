Public Class WCRFinal
    Public Sub New()
        InitializeComponent()
        tbFinal.Text = "Okay, we're almost done!  Let's print invoices next - click the button when you're ready."
    End Sub

    Private Sub btnPrintInvoices_Click(sender As Object, e As RoutedEventArgs) Handles btnPrintInvoices.Click
        WCRModule.WCR.PrintInvoices()
        btnPrintInvoices.Visibility = Visibility.Hidden
        tbFinal.Text = "Last thing - go ahead and print the WCR backup!"
        btnPrintWCR.Visibility = Visibility.Visible
    End Sub
    Private Sub btnPrintWCR_Click(sender As Object, e As RoutedEventArgs) Handles btnPrintWCR.Click
        WCRModule.WCR.PrintWCR()
        tbFinal.Text = "That's everything!  Make sure that you double check the numbers before you enter anything into MyFi - you're on your own from here on out!"
        ExitModule()
    End Sub

    Private Sub ExitWCR(sender As Object, e As MouseButtonEventArgs) Handles btnExit.MouseDown
        ExitModule()
    End Sub

    Private Sub ExitModule()
        'TODO: ADD APPLICATION STYLE MESSAGEBOX
        Dim yn As MsgBoxResult = MsgBox("Close WCR?", vbYesNo)
        If yn = vbYes Then
            WCRModule.UserClosed = True
            Close()
        End If
    End Sub
End Class
