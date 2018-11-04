﻿Public Class SingleUserInput

#Region "Properties"
    Public Property StringVal As String
    Public Property CurrencyVal As Double
    Public Property DoubleVal As Double
    Public Property NumVal As Long
    Public Property InputType As Byte
#End Region

#Region "Constructor"
    Public Sub New(Optional EnterOnly As Boolean = False)
        InitializeComponent()
        If EnterOnly = True Then btnOkay.Visibility = Visibility.Hidden
    End Sub
#End Region

#Region "Private Methods"
    Private Sub btnOkay_Click(sender As Object, e As RoutedEventArgs) Handles btnOkay.Click
        ExitAndSave()
    End Sub

    Private Sub txtUserInput_PreviewKeyUp(sender As Object, e As KeyEventArgs) Handles txtUserInput.PreviewKeyUp
        If e.Key = Key.Enter Then
            ExitAndSave()
        End If
    End Sub

    Private Sub ExitAndSave()
        Select Case InputType
            Case 0  '// String input
                StringVal = txtUserInput.Text
                Hide()
            Case 1  '// Currency
                Try
                    CurrencyVal = FormatNumber(txtUserInput.Text, 2)
                    Hide()
                Catch ex As Exception
                    'TODO:  ADD ERROR ROUTINE TO SINGLE INPUT UI
                End Try
            Case 2  '// Whole number
                Try
                    NumVal = FormatNumber(txtUserInput.Text, 0)
                    Hide()
                Catch ex As Exception
                    '// Add error routine
                End Try
            Case 3  '// Decimal, non-currency, number
                Try
                    DoubleVal = Double.Parse(txtUserInput.Text)
                    Hide()
                Catch ex As Exception
                    '// Add error routine
                End Try

        End Select
    End Sub

#End Region

End Class