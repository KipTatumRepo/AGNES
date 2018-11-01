Imports System.ComponentModel

Public Class Training

#Region "Properties"
    Dim EmployeeList As New List(Of EmployeeObj)
    Private WithEvents source As Trainings
    Public ScoreBox As NumberBox
    Public ScoreBoxText As TextBox

#End Region

#Region "Constructor"
    Public Sub New()
        InitializeComponent()
        CreateScoreBox()
        PopulateAssociates()
        PopulateTrainingTypes()
        ResetFields()
    End Sub

#End Region

#Region "Private Methods"

    Private Sub CreateScoreBox()
        ScoreBox = New NumberBox(112, True, False, True, False, True, AgnesBaseInput.FontSz.Standard, 1, "")
        With ScoreBox
            .Name = "txtScore"
            .Margin = New Thickness(392, 165, 0, 0)
        End With
        ScoreBoxText = ScoreBox.Children(1)
        grdEditor.Children.Add(ScoreBox)
    End Sub

    Private Sub ResetFields()
        cbxTraining.SelectedIndex = -1
        cbxTraining.Text = ""
        cbxTraining.IsEnabled = False
        cbxTrainer.SelectedIndex = -1
        cbxTrainer.Text = ""
        cbxTrainer.IsEnabled = False
        ScoreBoxText.Text = ""
        cbxTraining.IsEnabled = False
        dtpStartDt.DisplayDateStart = Now().AddDays(-60)
        dtpStartDt.DisplayDateEnd = Now()
        dtpStartDt.IsEnabled = False
        dtpEndDt.DisplayDateStart = Now().AddDays(-60)
        dtpEndDt.DisplayDateEnd = Now()
        dtpStartDt.IsEnabled = False
        ScoreBox.IsEnabled = False
        btnSave.IsEnabled = False
    End Sub

    Private Sub PopulateAssociates(Optional search As Byte = 0, Optional param As String = "")
        ' Search 0 = All
        ' Search 1 = By Last Name
        ' Search 2 = By Cost Center
        ' Search 3 = By Employee Number
        EmployeeList.Clear()
        cbxAssociates.Items.Clear()
        cbxTraining.IsEnabled = False
        cbxTrainer.IsEnabled = False
        dtpStartDt.IsEnabled = False
        dtpEndDt.IsEnabled = False
        Select Case search
            Case 0
                Dim qan = From anl In SharedDataGroup.EmployeeLists
                          Select anl

                Dim x As Integer = qan.Count
                For Each anl In qan
                    Dim emp As New EmployeeObj With {.CompassId = anl.PersNumber, .CostCenter = anl.CostCenterNumber,
                    .FirstName = anl.FirstName, .LastName = anl.LastName}
                    EmployeeList.Add(emp)
                    Dim cbi As New ComboBoxItem With {.Content = emp.Fullname, .Tag = EmployeeList.IndexOf(emp)}
                    cbxAssociates.Items.Add(cbi)
                    AddHandler cbi.Selected, AddressOf AssociateSelected
                Next
            Case 1
                Dim qan = From anl In SharedDataGroup.EmployeeLists
                          Where anl.LastName = param
                          Select anl

                Dim x As Integer = qan.Count
                For Each anl In qan
                    Dim emp As New EmployeeObj With {.CompassId = anl.PersNumber, .CostCenter = anl.CostCenterNumber,
                    .FirstName = anl.FirstName, .LastName = anl.LastName}
                    EmployeeList.Add(emp)
                    Dim cbi As New ComboBoxItem With {.Content = emp.Fullname, .Tag = EmployeeList.IndexOf(emp)}
                    cbxAssociates.Items.Add(cbi)
                    AddHandler cbi.Selected, AddressOf AssociateSelected
                Next
            Case 2
                Dim costcenter As Long = FormatNumber(param, 0)
                Dim qan = From anl In SharedDataGroup.EmployeeLists
                          Where anl.CostCenterNumber = costcenter
                          Select anl

                Dim x As Integer = qan.Count
                For Each anl In qan
                    Dim emp As New EmployeeObj With {.CompassId = anl.PersNumber, .CostCenter = anl.CostCenterNumber,
                    .FirstName = anl.FirstName, .LastName = anl.LastName}
                    EmployeeList.Add(emp)
                    Dim cbi As New ComboBoxItem With {.Content = emp.Fullname, .Tag = EmployeeList.IndexOf(emp)}
                    cbxAssociates.Items.Add(cbi)
                    AddHandler cbi.Selected, AddressOf AssociateSelected
                Next
            Case 3
                Dim empnum As Long = FormatNumber(param, 0)
                Dim qan = From anl In SharedDataGroup.EmployeeLists
                          Where anl.PersNumber = empnum
                          Select anl

                Dim x As Integer = qan.Count
                For Each anl In qan
                    Dim emp As New EmployeeObj With {.CompassId = anl.PersNumber, .CostCenter = anl.CostCenterNumber,
                    .FirstName = anl.FirstName, .LastName = anl.LastName}
                    EmployeeList.Add(emp)
                    Dim cbi As New ComboBoxItem With {.Content = emp.Fullname, .Tag = EmployeeList.IndexOf(emp)}
                    cbxAssociates.Items.Add(cbi)
                    AddHandler cbi.Selected, AddressOf AssociateSelected
                Next

        End Select

        cbxAssociates.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))
    End Sub

    Private Sub PopulateTrainingTypes()
        cbxTraining.Items.Clear()
        Dim qtt = From ttl In TrainingData.TrainingTypes
                  Select ttl

        Dim x As Integer = qtt.Count
        For Each ttl In qtt
            Dim cbi As New ComboBoxItem With {.Content = ttl.TrainingName, .Tag = ttl.PID}
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

    Private Sub AssociateSelected(sender As ComboBoxItem, e As RoutedEventArgs)
        Dim selindex As Integer = cbxAssociates.Items.IndexOf(sender)
        PopulateTrainingRecords(EmployeeList(Long.Parse(sender.Tag)).CompassId)
        cbxTraining.IsEnabled = True
        cbxTraining.SelectedIndex = -1
        cbxTrainer.SelectedIndex = -1
        cbxTrainer.IsEnabled = False
        dtpStartDt.IsEnabled = False
        dtpEndDt.IsEnabled = False
        ScoreBox.IsEnabled = False
        btnSave.IsEnabled = False
    End Sub

    Private Sub PopulateTrainingRecords(eid As Long)
        dgTrainingHistory.ItemsSource = Nothing
        dgTrainingHistory.DataContext = Nothing

        source = New Trainings

        Dim qtr = From atr In TrainingData.TrainingRecords
                  Where atr.AssociateID = eid
                  Select atr

        For Each atr In qtr
            Dim tr As New TrainingRecordItem
            With tr
                .Training = GetTrainingType(atr.Training)
                .Start = FormatDateTime(atr.StartDate, DateFormat.ShortDate)
                .Complete = Format(atr.EndDate, "MM/dd/yy")
                .Trainer = GetTrainer(atr.Trainer)
                .Score = atr.Score
            End With

            'TODO:  ADD ROUTINE AND DATA TO DETERMINE IF CERTIFICATION IS NEEDED AND, IF SO, WHAT SCORE QUALIFIES
            'If HasCert(atr.Training) = True Then
            '    tr.Certification = CertAchieved(atr.Training, atr.Score)
            'End If

            source.Add(tr)
            dgTrainingHistory.DataContext = source
            dgTrainingHistory.ItemsSource = source
        Next

        'Dim col As DataGridColumn = dgTrainingHistory.Columns(1)

        'col.Width = 20


    End Sub

    Private Sub TrainingSelected(sender As Object, e As SelectionChangedEventArgs) Handles cbxTraining.SelectionChanged
        If cbxTraining.SelectedIndex = -1 Then
            cbxTrainer.Items.Clear()
            Exit Sub
        End If
        Dim cbi As ComboBoxItem = cbxTraining.SelectedItem
        PopulateTrainers(Integer.Parse(cbi.Tag))

    End Sub

    Private Sub PopulateTrainers(tid As Integer)
        cbxTrainer.Items.Clear()
        cbxTrainer.IsEnabled = False
        Dim qtn = From ttn In TrainingData.Trainers
                  Select ttn
                  Where ttn.TrainingId = tid

        For Each ttn In qtn
            Dim cbi As New ComboBoxItem With {.Content = ttn.TrainerName, .Tag = ttn.PID}
            cbxTrainer.Items.Add(cbi)
        Next

        If qtn.Count > 0 Then cbxTrainer.IsEnabled = True
        If qtn.Count = 1 Then cbxTrainer.SelectedIndex = 0

    End Sub

    Private Sub TrainerSelected(sender As Object, e As SelectionChangedEventArgs) Handles cbxTrainer.SelectionChanged
        dtpStartDt.IsEnabled = True
        dtpStartDt.Text = Now()
        dtpEndDt.IsEnabled = True
        dtpEndDt.Text = Now()
        ScoreBox.IsEnabled = True
        btnSave.IsEnabled = True
    End Sub

    Private Sub SaveRecord(sender As Object, e As RoutedEventArgs) Handles btnSave.Click
        'TODO: ADD VALIDATION ROUTINE
        Dim empid As Long = 0

        For Each emp As EmployeeObj In EmployeeList
            If emp.Fullname = cbxAssociates.Text Then
                empid = emp.CompassId
                Exit For
            End If
        Next

        Dim newentry As New TrainingRecord
        With newentry
            .AssociateID = empid
            'TODO: DETERMINE IF CERTIFICATION NEEDS TO BE SAVED
            '.Certification = 0
            .StartDate = dtpStartDt.DisplayDate
            .EndDate = dtpEndDt.DisplayDate
            .Score = FormatNumber(ScoreBoxText.Text, 1)
            .Trainer = GetTrainerId(cbxTrainer.Text)
            .Training = GetTrainingId(cbxTraining.Text)
        End With

        TrainingData.TrainingRecords.Add(newentry)
        TrainingData.SaveChanges()
        PopulateTrainingRecords(empid)
    End Sub

    Private Function GetTrainingType(tid As Long) As String
        Dim qtt = From ttl In TrainingData.TrainingTypes
                  Select ttl
                  Where ttl.PID = tid

        For Each ttl In qtt
            Return ttl.TrainingName
        Next
        Return ""
    End Function

    Private Function GetTrainingId(tname As String) As Integer
        Dim qtt = From ttl In TrainingData.TrainingTypes
                  Select ttl
                  Where ttl.TrainingName = tname

        For Each ttl In qtt
            Return ttl.PID
        Next
        Return 0
    End Function

    Private Function GetTrainer(tid As Long) As String
        Dim qtn = From ttn In TrainingData.Trainers
                  Select ttn
                  Where ttn.PID = tid

        For Each ttn In qtn
            Return ttn.TrainerName
        Next
        Return ""
    End Function

    Private Function GetTrainerId(tname As String) As Integer
        Dim qtn = From ttn In TrainingData.Trainers
                  Select ttn
                  Where ttn.TrainerName = tname

        For Each ttn In qtn
            Return ttn.PID
        Next
        Return 0
    End Function

#End Region


End Class
