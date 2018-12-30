Imports System.ComponentModel
Public Class AssociateMapping

#Region "Properties"
    Dim HRISEmployeeList As New List(Of EmployeeObj)
    Dim UnmappedEmployeeList As New List(Of EmployeeObj)
    Private MappingType As Byte
#End Region

#Region "Constructor"
    Public Sub New(maptype As Byte)
        InitializeComponent()
        MappingType = maptype
        PopulateUnassigned()
        PopulateAssociates()
    End Sub

#End Region

#Region "Public Methods"

#End Region

#Region "Private Methods"
    Private Sub PopulateUnassigned()
        lbxUnmapped.Items.Clear()
        UnmappedEmployeeList.Clear()
        Dim qta = (From ta In TrainingData.TempRecords
                   Select ta).ToList()

        Dim nmlist As New List(Of String)
        For Each ta In qta
            nmlist.Add(ta.AssociateName)
        Next
        nmlist = nmlist.Distinct.ToList()

        Dim tempcount As Byte = 0
        For Each nm In nmlist
            Dim emp As New EmployeeObj With {.CompassId = 99999, .CostCenter = 99999, .FirstName = nm, .LastName = "temp"}
            UnmappedEmployeeList.Add(emp)
            Dim lbi As New ListBoxItem With {.Content = nm, .Tag = UnmappedEmployeeList.IndexOf(emp)}
            lbxUnmapped.Items.Add(lbi)
        Next
    End Sub

    Private Sub PopulateAssociates()
        HRISEmployeeList.Clear()
        lbxHRIS.Items.Clear()
        Dim qan = From anl In SharedDataGroup.EmployeeLists
                  Select anl

        For Each anl In qan
            Dim emp As New EmployeeObj With {.CompassId = anl.PersNumber, .CostCenter = anl.CostCenterNumber,
            .FirstName = anl.FirstName, .LastName = anl.LastName}
            HRISEmployeeList.Add(emp)
            Dim lbi As New ListBoxItem With {.Content = emp.Fullname & " - " & emp.CompassId, .Tag = HRISEmployeeList.IndexOf(emp)}
            lbxHRIS.Items.Add(lbi)
        Next
        lbxHRIS.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))
    End Sub

    Private Sub ShowDoMapIcon(sender As Object, e As SelectionChangedEventArgs) Handles lbxHRIS.SelectionChanged, lbxUnmapped.SelectionChanged
        tbAssocMapping.Text = ""
        If lbxUnmapped.SelectedIndex <> -1 And lbxHRIS.SelectedIndex <> -1 Then
            imgDoMap.Visibility = Visibility.Visible
        Else
            imgDoMap.Visibility = Visibility.Collapsed
        End If
    End Sub

    Private Sub MapAssociate(sender As Object, e As MouseButtonEventArgs) Handles imgDoMap.MouseLeftButtonDown
        Select Case MappingType
            Case 0  ' Training
                MapToTraining()
        End Select
        lbxHRIS.SelectedIndex = -1
        lbxUnmapped.SelectedIndex = -1
        tbAssocMapping.Text = "Saved"
        If lbxUnmapped.Items.Count = 0 Then
            lbxUnmapped.IsEnabled = False
            lbxHRIS.IsEnabled = False
            tbAssocMapping.Text = "Saved - all mapping is complete."
            sbAssocMapping.Background = Brushes.Green
        End If
    End Sub

    Private Sub MapToTraining()
        ' Map each instance of the employee in the Temp record to a new entry in the Training Record table
        Dim SelectedTempEmployee As ListBoxItem = lbxUnmapped.SelectedItem
        Dim SelectedTempEmpObj As EmployeeObj = UnmappedEmployeeList.Item(FormatNumber(SelectedTempEmployee.Tag, 0))

        Dim SelectedEmployeeName = SelectedTempEmpObj.FirstName

        Dim MappedEmployee As ListBoxItem = lbxHRIS.SelectedItem
        Dim MappedEmpObj As EmployeeObj = HRISEmployeeList.Item(FormatNumber(MappedEmployee.Tag, 0))

        Dim qtr = From tr In TrainingData.TempRecords
                  Where tr.AssociateName = SelectedEmployeeName
                  Select tr

        For Each tr In qtr
            Dim newentry As New TrainingRecord
            With newentry
                .AssociateID = MappedEmpObj.CompassId
                .Certification = tr.Certification
                .StartDate = tr.StartDate
                .EndDate = tr.EndDate
                .Score = tr.Score
                .Trainer = tr.Trainer
                .Training = tr.Training
                .TrainingRecordedBy = tr.TrainingRecordedBy
            End With
            TrainingData.TrainingRecords.Add(newentry)
            TrainingData.TempRecords.Remove(tr)
        Next
        TrainingData.SaveChanges()
        lbxUnmapped.Items.Remove(SelectedTempEmployee)
    End Sub

#End Region

End Class
