Public Class HRAudit
    Private Sub HRAudit_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Dim testme As New CurrencyBox(204, True, True, False, False, True, AgnesBaseInput.FontSz.VeryLarge)
        grdMain.Children.Add(testme)
    End Sub

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        For Each a As Object In grdMain.Children
            If TypeOf (a) Is CurrencyBox Then
                Dim b As CurrencyBox = a
                b.Flare = Not b.Flare
            End If
        Next
    End Sub
End Class
