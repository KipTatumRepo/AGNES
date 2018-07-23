Public Class WCRFinal
    Public Sub New()
        InitializeComponent()

    End Sub

    Private Sub btnPrintInvoices_Click(sender As Object, e As RoutedEventArgs) Handles btnPrintInvoices.Click
        WCRModule.WCR.PrintInvoices()
    End Sub

    Private Sub ExitWCR(sender As Object, e As MouseButtonEventArgs) Handles btnExit.MouseDown
        'TODO: ADD APPLICATION STYLE MESSAGEBOX
        Dim yn As MsgBoxResult = MsgBox("Close WCR?", vbYesNo)
        If yn = vbYes Then
            WCRModule.UserClosed = True
            Close()
        End If
    End Sub
End Class
