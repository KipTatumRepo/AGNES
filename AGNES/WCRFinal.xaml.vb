Public Class WCRFinal
    Public Sub New()
        InitializeComponent()

    End Sub

    Private Sub btnPrintInvoices_Click(sender As Object, e As RoutedEventArgs) Handles btnPrintInvoices.Click
        MainWindow.WCR.PrintInvoices()
    End Sub
End Class
