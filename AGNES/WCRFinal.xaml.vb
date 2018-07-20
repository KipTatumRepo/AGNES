Public Class WCRFinal
    Public Sub New()
        InitializeComponent()

    End Sub

    Private Sub btnPrintInvoices_Click(sender As Object, e As RoutedEventArgs) Handles btnPrintInvoices.Click
        WCRModule.WCR.PrintInvoices()
    End Sub
End Class
