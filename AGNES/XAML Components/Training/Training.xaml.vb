Imports System.ComponentModel

Public Class Training

#Region "Properties"
    Dim EmployeeList As New List(Of EmployeeObj)
    Private WithEvents source As Trainings
    Public ScoreBox As NumberBox
    Public ScoreBoxText As TextBox
    Private HasCert As Boolean
    Private PassingScore As Double
    Private NewAssociate As Boolean
    Private newassociatecbi As ComboBoxItem
    Private InitialWarning As Boolean = True

#End Region

#Region "Constructor"
    Public Sub New()
        InitializeComponent()
        CreateScoreBox()
        PopulateAssociates()
        ResetFields()
    End Sub

#End Region

#Region "Public Methods"

    Public Function GetTrainingId(tname As String) As Integer
        Try
            Dim qtt = (From ttl In TrainingData.TrainingTypes
                       Select ttl
                       Where ttl.TrainingName = tname).ToList(0)

            Return qtt.PID
        Catch
        End Try
        Return 0
    End Function

    Public Function GetTrainingType(tid As Long) As String

        Try
            Dim qtt = (From ttl In TrainingData.TrainingTypes
                       Select ttl
                       Where ttl.PID = tid).ToList(0)

            Return qtt.TrainingName
        Catch
        End Try

        Return ""
    End Function

    Public Function GetTrainer(tid As Long) As String
        Try
            Dim qtn = (From ttn In TrainingData.Trainers
                       Select ttn
                       Where ttn.PID = tid).ToList(0)
            Return qtn.TrainerName
        Catch
        End Try
        Return ""
    End Function

    Public Function GetTrainerId(tname As String) As Integer
        Try
            Dim qtn = (From ttn In TrainingData.Trainers
                       Select ttn
                       Where ttn.TrainerName = tname).ToList(0)
            Return qtn.PID
        Catch
        End Try
        Return 0
    End Function

#End Region

#Region "Private Methods"

#Region "Toolbar"
    Private Sub AddTraining(sender As Object, e As MouseButtonEventArgs) Handles imgAddTraining.MouseLeftButtonDown
        Dim NewTrainingUI As New NewTraining
        NewTrainingUI.ShowDialog()
        If cbxAssociates.SelectedIndex = -1 Then Exit Sub
        Dim eid As Long = EmployeeList(Long.Parse(cbxAssociates.SelectedItem.Tag)).CompassId
        PopulateTrainingTypes(GetBusinessGroup(eid))
    End Sub

    Private Sub AddTrainer(sender As Object, e As MouseButtonEventArgs) Handles imgAddTrainer.MouseLeftButtonDown
        Dim NewTrainerUI As New Trainers
        NewTrainerUI.ShowDialog()
    End Sub

    Private Sub MapAssociates(sender As Object, e As MouseButtonEventArgs) Handles imgAssocMap.MouseLeftButtonDown
        Dim NewMapUI As New AssociateMapping(0)
        NewMapUI.ShowDialog()
        NewMapUI.Close()
        PopulateAssociates(0)
    End Sub

#End Region
    Private Sub CreateScoreBox()
        ScoreBox = New NumberBox(112, True, False, True, False, True, AgnesBaseInput.FontSz.Standard, 1, "")
        With ScoreBox
            .Name = "txtScore"
            .Margin = New Thickness(392, 165, 0, 0)
        End With
        ScoreBoxText = ScoreBox.Children(1)
        ScoreBoxText.TabIndex = 5
        AddHandler ScoreBox.LostFocus, AddressOf EvaluateScore
        grdEditor.Children.Add(ScoreBox)
    End Sub

    Private Sub EvaluateScore()
        ScoreBox.Flare = False
        Dim ScoreVal As Double = FormatNumber(ScoreBoxText.Text, 3)
        If ScoreVal > 1 Then ScoreVal = ScoreVal / 100
        If ScoreVal > 1 Then
            ScoreVal = 1
            ScoreBox.SetAmount = 100
        End If
        If ScoreVal < PassingScore Then ScoreBox.Flare = True

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
        dtpStartDt.DisplayDateStart = Now().AddDays(-120)
        dtpStartDt.DisplayDateEnd = Now()
        dtpStartDt.IsEnabled = False
        dtpEndDt.DisplayDateStart = Now().AddDays(-120)
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
        imgAssocMap.Visibility = Visibility.Collapsed
        EmployeeList.Clear()
        cbxAssociates.Items.Clear()
        cbxTraining.IsEnabled = False
        cbxTrainer.IsEnabled = False
        dtpStartDt.IsEnabled = False
        dtpEndDt.IsEnabled = False
        NewAssociate = False
        Select Case search
            Case 0
                Dim qan = From anl In SharedDataGroup.EmployeeLists
                          Select anl

                For Each anl In qan
                    Dim emp As New EmployeeObj With {.CompassId = anl.PersNumber, .CostCenter = anl.CostCenterNumber,
                    .FirstName = anl.FirstName, .LastName = anl.LastName}
                    EmployeeList.Add(emp)
                    Dim cbi As New ComboBoxItem With {.Content = emp.Fullname, .Tag = EmployeeList.IndexOf(emp)}
                    cbxAssociates.Items.Add(cbi)
                    AddHandler cbi.Selected, AddressOf AssociateSelected
                Next

                '// Add associates saved to the temporary table to the list
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
                    EmployeeList.Add(emp)
                    Dim cbi As New ComboBoxItem With {.Content = "**" & nm, .Tag = EmployeeList.IndexOf(emp)}
                    cbxAssociates.Items.Add(cbi)
                    AddHandler cbi.Selected, AddressOf AssociateSelected
                    tempcount += 1
                Next

                If tempcount > 0 Then
                    imgAssocMap.Visibility = Visibility.Visible
                    If InitialWarning = True Then
                        Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Small, AgnesMessageBox.MsgBoxLayout.TextAndImage, AgnesMessageBox.MsgBoxType.OkOnly, 12,,, "Unassigned associates are present", "These are indicated with ** before the name", AgnesMessageBox.ImageType.Alert)
                        amsg.ShowDialog()
                        amsg.Close()
                        InitialWarning = False
                    End If
                End If

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
        newassociatecbi = New ComboBoxItem With {.Content = "New Associate (not in list)"}
        AddHandler newassociatecbi.Selected, AddressOf NewAssociateSelected
        cbxAssociates.Items.Insert(0, newassociatecbi)

    End Sub

    Private Sub PopulateTrainingTypes(bgid)
        cbxTraining.Items.Clear()
        Dim qtt = From ttl In TrainingData.TrainingTypes
                  Select ttl

        If bgid <> 0 Then
            For Each ttl In qtt
                Dim qtg = From tg In TrainingData.BusinessGroupTraining_Join
                          Where tg.TrainingId = ttl.PID
                          Select tg

                For Each tg In qtg
                    If tg.BusinessGroupId = bgid Or tg.BusinessGroupId = 0 Then
                        Dim cbi As New ComboBoxItem With {.Content = ttl.TrainingName, .Tag = ttl.PID}
                        cbxTraining.Items.Add(cbi)
                    End If
                Next
            Next
        Else
            For Each ttl In qtt
                Dim cbi As New ComboBoxItem With {.Content = ttl.TrainingName, .Tag = ttl.PID}
                cbxTraining.Items.Add(cbi)
            Next
        End If
        If cbxTraining.Items.Count > 0 Then cbxTraining.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))
    End Sub

    Private Sub AssociateSearch(sender As Object, e As MouseButtonEventArgs) Handles imgSearch.MouseLeftButtonDown
        Dim searchparam As New AssocSearch
        searchparam.ShowDialog()
        PopulateAssociates(searchparam.ParamChoice, searchparam.ParamText)
        searchparam.Close()

    End Sub

    Private Sub AssociateSelected(sender As ComboBoxItem, e As RoutedEventArgs)
        Dim selindex As Integer = cbxAssociates.Items.IndexOf(sender)
        Dim eid As Long = EmployeeList(Long.Parse(sender.Tag)).CompassId

        If eid = 99999 Then
            NewAssociate = True
            PopulateTrainingTypes(0)
            PopulateTrainingRecords(eid, sender.Content)
        Else
            PopulateTrainingTypes(GetBusinessGroup(eid))
            PopulateTrainingRecords(eid)
        End If


        If cbxTraining.Items.Count = 0 Then Exit Sub
        cbxTraining.IsEnabled = True
        cbxTraining.SelectedIndex = -1
        cbxTrainer.SelectedIndex = -1
        cbxTrainer.IsEnabled = False
        dtpStartDt.IsEnabled = False
        dtpEndDt.IsEnabled = False
        With ScoreBox
            .IsEnabled = False
            .Flare = False
            .SystemChange = True
            .BaseTextBox.Text = ""
            .SystemChange = False
        End With
        btnSave.IsEnabled = False
    End Sub

    Private Sub NewAssociateSelected(sender As ComboBoxItem, e As RoutedEventArgs)
        Dim ibx As String = InputBox("Enter name", "Add New Associate", "")
        If ibx = "" Then Exit Sub
        newassociatecbi.Content = ibx
        NewAssociate = True
        PopulateTrainingTypes(0)
        cbxTraining.IsEnabled = True
        cbxTraining.SelectedIndex = -1
        cbxTrainer.SelectedIndex = -1
        cbxTrainer.IsEnabled = False
        dtpStartDt.IsEnabled = False
        dtpEndDt.IsEnabled = False
        With ScoreBox
            .IsEnabled = False
            .Flare = False
            .SystemChange = True
            .BaseTextBox.Text = ""
            .SystemChange = False
        End With
        btnSave.IsEnabled = False
    End Sub

    Private Sub PopulateTrainingRecords(eid As Long, Optional nm As String = "")
        dgTrainingHistory.ItemsSource = Nothing
        dgTrainingHistory.DataContext = Nothing
        source = New Trainings

        If eid = 99999 Then
            If Mid(nm, 1, 2) = "**" Then nm = Mid(nm, 3, Len(nm) - 2)
            Dim qtr = From atr In TrainingData.TempRecords
                      Where atr.AssociateName = nm
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

                source.Add(tr)
            Next
        Else
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

                source.Add(tr)
            Next
        End If
        dgTrainingHistory.DataContext = source
        dgTrainingHistory.ItemsSource = source

    End Sub

    Private Sub TrainingSelected(sender As Object, e As SelectionChangedEventArgs) Handles cbxTraining.SelectionChanged
        If cbxTraining.SelectedIndex = -1 Then
            cbxTrainer.Items.Clear()
            Exit Sub
        End If
        Dim cbi As ComboBoxItem = cbxTraining.SelectedItem
        Dim tid As Integer = Integer.Parse(cbi.Tag)
        PopulateTrainers(tid)

        '// Assign minimum passing score and whether certification is required
        Dim qtd = From tdi In TrainingData.TrainingTypes
                  Select tdi
                  Where tdi.PID = tid

        For Each tdi In qtd
            HasCert = tdi.Certification
            PassingScore = tdi.PassCertScore
            If PassingScore <> 0 Then
                ScoreBox.IsEnabled = True
                tbScore.Text = "Score (passing is " & FormatPercent(PassingScore, 1) & "):"
                With ScoreBox
                    .Flare = False
                    .SystemChange = True
                    .BaseTextBox.Text = ""
                    .SystemChange = False
                End With
            Else
                tbScore.Text = "Score:"
                With ScoreBox
                    .IsEnabled = False
                    .Flare = False
                    .SystemChange = True
                    .BaseTextBox.Text = ""
                    .SystemChange = False
                End With
            End If
        Next

    End Sub

    Private Sub PopulateTrainers(tid As Integer)
        cbxTrainer.Items.Clear()
        cbxTrainer.IsEnabled = False
        Dim qti = From tti In TrainingData.TrainerTraining_Join
                  Select tti
                  Where tti.TrainingId = tid

        For Each tti In qti
            Dim qtn = From ttn In TrainingData.Trainers
                      Select ttn
                      Where ttn.EmpId = tti.TrainerId

            For Each ttn In qtn
                Dim cbi As New ComboBoxItem With {.Content = ttn.TrainerName, .Tag = ttn.EmpId}
                cbxTrainer.Items.Add(cbi)
            Next

            If qtn.Count > 0 Then cbxTrainer.IsEnabled = True
            If qtn.Count = 1 Then cbxTrainer.SelectedIndex = 0

        Next

    End Sub

    Private Sub TrainerSelected(sender As Object, e As SelectionChangedEventArgs) Handles cbxTrainer.SelectionChanged
        dtpStartDt.IsEnabled = True
        dtpStartDt.Text = Now()
        dtpEndDt.IsEnabled = True
        dtpEndDt.Text = Now()
        btnSave.IsEnabled = True
    End Sub

    Private Sub SaveRecord(sender As Object, e As RoutedEventArgs) Handles btnSave.Click

        '// Field validation
        Dim CertVal As Boolean = HasCert
        Dim ScoreVal As Double
        If ScoreBoxText.IsEnabled = True Then
            If ScoreBoxText.Text = "" Then
                Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Small, AgnesMessageBox.MsgBoxLayout.TextAndImage, AgnesMessageBox.MsgBoxType.OkOnly, 12,,,, "Score required.", AgnesMessageBox.ImageType.Danger)
                amsg.ShowDialog()
                amsg.Close()
                Exit Sub
            Else
                ScoreVal = FormatNumber(ScoreBoxText.Text, 3)
                If ScoreVal > 1 Then ScoreVal = ScoreVal / 100

                '// Notify if score is below passing/cert not achieved
                If ScoreVal < PassingScore Then
                    If CertVal = True Then
                        Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Small, AgnesMessageBox.MsgBoxLayout.TextAndImage, AgnesMessageBox.MsgBoxType.YesNo, 12,,, "Score is below passing/certification requirement.", "Do you still wish to save?", AgnesMessageBox.ImageType.Alert)
                        amsg.ShowDialog()
                        If amsg.ReturnResult = "No" Then
                            amsg.Close()
                            Exit Sub
                        End If
                        CertVal = False
                    Else
                        Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Small, AgnesMessageBox.MsgBoxLayout.TextAndImage, AgnesMessageBox.MsgBoxType.YesNo, 12,,, "Score is below passing.", "Do you still wish to save?", AgnesMessageBox.ImageType.Alert)
                        amsg.ShowDialog()
                        If amsg.ReturnResult = "No" Then
                            amsg.Close()
                            Exit Sub
                        End If
                    End If
                End If
            End If

        End If

        If NewAssociate = False Then
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
                .Certification = CertVal
                .StartDate = dtpStartDt.DisplayDate
                .EndDate = dtpEndDt.DisplayDate
                .Score = ScoreVal
                .Trainer = GetTrainerId(cbxTrainer.Text)
                .Training = GetTrainingId(cbxTraining.Text)
                .TrainingRecordedBy = My.Settings.UserID
            End With
            TrainingData.TrainingRecords.Add(newentry)
            TrainingData.SaveChanges()
            PopulateTrainingRecords(empid)
        Else

            Dim newentry As New TempRecord
            If cbxAssociates.SelectedIndex = 0 Then
                newentry.AssociateName = newassociatecbi.Content
            Else
                Dim tmpnm As String = cbxAssociates.Text
                If Mid(tmpnm, 1, 2) = "**" Then tmpnm = Mid(tmpnm, 3, Len(tmpnm) - 2)
                newentry.AssociateName = tmpnm
            End If

            With newentry
                .Certification = CertVal
                .StartDate = dtpStartDt.DisplayDate
                .EndDate = dtpEndDt.DisplayDate
                .Score = ScoreVal
                .Trainer = GetTrainerId(cbxTrainer.Text)
                .Training = GetTrainingId(cbxTraining.Text)
                .TrainingRecordedBy = My.Settings.UserID
            End With
            TrainingData.TempRecords.Add(newentry)
            TrainingData.SaveChanges()

        End If
        ClearFields()
        PopulateAssociates()
    End Sub

    Private Sub ClearFields()
        cbxAssociates.SelectedIndex = -1
        With cbxTraining
            .Text = ""
            .SelectedIndex = -1
            .IsEnabled = False
        End With

        With cbxTrainer
            .Text = ""
            .Items.Clear()
            .IsEnabled = False
        End With

        With dtpStartDt
            .SelectedDate = Nothing
            .DisplayDate = DateTime.Today
            .IsEnabled = False
        End With

        With dtpEndDt
            .SelectedDate = Nothing
            .DisplayDate = DateTime.Today
            .IsEnabled = False
        End With

        tbScore.Text = "Score:"
        ScoreBoxText.Text = ""
        ScoreBox.Flare = False
        ScoreBox.IsEnabled = False

        dgTrainingHistory.ItemsSource = Nothing
        NewAssociate = False
    End Sub

    Private Function GetBusinessGroup(eid As Long) As Long
        Dim cc As Long
        Dim qec = (From ec In SharedDataGroup.EmployeeLists
                   Where ec.PersNumber = eid
                   Select ec).ToList(0)

        cc = qec.CostCenterNumber
        Dim ebg = (From bg In SharedDataGroup.CostCenters
                   Where bg.CostCenter1 = cc
                   Select bg).ToList(0)

        Return ebg.BusinessGroup
    End Function

#End Region

End Class
