Public Class HRAudit
    Private Sub HRAudit_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Dim testme As New AgnesBaseInput(VerticalAlignment.Top, HorizontalAlignment.Left, "Hey") With {.Name = "TestFlare"}
        grdMain.Children.Add(testme)

    End Sub

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        For Each a As Object In grdMain.Children
            If TypeOf (a) Is AgnesBaseInput Then
                Dim b As AgnesBaseInput = a
                b.Flare = Not b.Flare
            End If
        Next
    End Sub
End Class
