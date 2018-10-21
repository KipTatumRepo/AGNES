Public Class AssocSearch

#Region "Properties"
    Public Property ParamChoice As Byte = 0
    Public Property ParamText As String = ""
#End Region

#Region "Private Methods"
    Private Sub LastName(sender As Object, e As RoutedEventArgs) Handles rdbLastName.Click
        ParamChoice = 1
    End Sub

    Private Sub CostCenter(sender As Object, e As RoutedEventArgs) Handles rdbCostCenter.Click
        ParamChoice = 2
    End Sub

    Private Sub EmployeeNumber(sender As Object, e As RoutedEventArgs) Handles rdbNumber.Click
        ParamChoice = 3
    End Sub

    Private Sub btnSearch_Click(sender As Object, e As RoutedEventArgs) Handles btnSearch.Click
        ParamText = txtSearch.Text
        If txtSearch.Text = "" Then ParamChoice = 0
        Hide()
    End Sub

#End Region

End Class
