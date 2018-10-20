Imports System.ComponentModel

Public Class Training

#Region "Constructor"
    Public Sub New()
        InitializeComponent()
        PopulateAssociates() '2, "32633")
        PopulateTraining()
    End Sub
#End Region

#Region "Private Methods"

    Private Sub PopulateAssociates(Optional search As Byte = 0, Optional param As String = "")
        ' Search 0 = All
        ' Search 1 = By Last Name
        ' Search 2 = By Cost Center
        ' Search 3 = By Employee Number

        cbxAssociates.Items.Clear()
        Select Case search
            Case 0
                Dim qan = From anl In SharedDataGroup.EmployeeLists
                          Select anl

                Dim x As Integer = qan.Count
                For Each anl In qan
                    Dim cbi As New ComboBoxItem With {.Content = anl.LastName & ", " & anl.FirstName}
                    cbxAssociates.Items.Add(cbi)
                Next
            Case 1
                Dim qan = From anl In SharedDataGroup.EmployeeLists
                          Where anl.LastName = param
                          Select anl

                Dim x As Integer = qan.Count
                For Each anl In qan
                    Dim cbi As New ComboBoxItem With {.Content = anl.LastName & ", " & anl.FirstName}
                    cbxAssociates.Items.Add(cbi)
                Next
            Case 2
                Dim costcenter As Long = FormatNumber(param, 0)
                Dim qan = From anl In SharedDataGroup.EmployeeLists
                          Where anl.CostCenterNumber = costcenter
                          Select anl

                Dim x As Integer = qan.Count
                For Each anl In qan
                    Dim cbi As New ComboBoxItem With {.Content = anl.LastName & ", " & anl.FirstName}
                    cbxAssociates.Items.Add(cbi)
                Next
            Case 3
                Dim empnum As Long = FormatNumber(param, 0)
                Dim qan = From anl In SharedDataGroup.EmployeeLists
                          Where anl.PersNumber = empnum
                          Select anl

                Dim x As Integer = qan.Count
                For Each anl In qan
                    Dim cbi As New ComboBoxItem With {.Content = anl.LastName & ", " & anl.FirstName}
                    cbxAssociates.Items.Add(cbi)
                Next

        End Select

        cbxAssociates.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))
    End Sub

    Private Sub PopulateTraining()
        cbxTraining.Items.Clear()
        Dim qtt = From ttl In TrainingData.TrainingTypes
                  Select ttl

        Dim x As Integer = qtt.Count
        For Each ttl In qtt
            Dim cbi As New ComboBoxItem With {.Content = ttl.TrainingName}
            cbxTraining.Items.Add(cbi)
        Next

        cbxTraining.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))
    End Sub

    Private Sub AssociateSearch(sender As Object, e As MouseButtonEventArgs) Handles imgSearch.MouseLeftButtonDown
        Dim searchparam As New AssocSearch
        searchparam.ShowDialog()
        PopulateAssociates(searchparam.ParamChoice, searchparam.ParamText)
        searchparam.Close()

    End Sub

#End Region

End Class
