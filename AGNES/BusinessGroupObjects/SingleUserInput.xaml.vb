Public Class SingleUserInput

#Region "Properties"
    Public Property StringVal As String
    Public Property CurrencyVal As Double
    Public Property NumVal As Long
    Public Property InputType As Byte
#End Region

#Region "Private Methods"
    Private Sub btnOkay_Click(sender As Object, e As RoutedEventArgs) Handles btnOkay.Click
        Select Case InputType
            Case 0  '// String input
                StringVal = txtUserInput.Text
                Hide()
            Case 1  '// Currency
                Try
                    CurrencyVal = FormatNumber(txtUserInput.Text, 2)
                Catch ex As Exception
                    '// Add error routine
                End Try
            Case 2  '// Whole number
                Try
                    NumVal = FormatNumber(txtUserInput.Text, 0)
                    Hide()
                Catch ex As Exception
                    '// Add error routine
                End Try
        End Select
    End Sub

#End Region

End Class
