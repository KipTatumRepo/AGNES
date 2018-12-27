Public Class NewTraining

#Region "Properties"
    Private Changesmade As Boolean
    Private nbxTrainingHours As NumberBox
    Private pbxScore As PercentBox
    Private TrainingName As String
    Private Description As String
    Private GroupList As New List(Of String)
    Private Hours As Integer
    Private Skore As Double
    Private Cert As Boolean
    Private Score As Boolean
#End Region

#Region "Constructor"
    Public Sub New()
        InitializeComponent()
        txtTrainingName.Focus()
        txtTrainingName.SelectAll()
    End Sub
#End Region

#Region "Public Methods"

#End Region

#Region "Private Methods"
    Private Sub CheckForNameEnter(sender As Object, e As KeyEventArgs) Handles txtTrainingName.KeyDown
        If e.Key = Key.Return Then
            e.Handled = True
            If ValidateName(txtTrainingName.Text) = True Then GetDesc()
        End If
    End Sub

    Private Sub GetDesc()
        imgIcon.Source = New BitmapImage(New Uri("pack://application:,,,/Resources/Book.png"))
        txtTrainingName.Visibility = Visibility.Collapsed
        tbDirections.Text = "Please provide a short description"
        ' "What do you want to name the new training?"
        txtDescription.Visibility = Visibility.Visible
        With txtDescription
            .Focus()
            .SelectAll()
        End With
    End Sub

    Private Sub CheckForDescEnter(sender As Object, e As KeyEventArgs) Handles txtDescription.KeyDown
        If e.Key = Key.Return Then
            e.Handled = True
            If ValidateDesc(txtDescription.Text) = True Then GetGroups()
        End If
    End Sub

    Private Sub GetGroups()
        imgIcon.Source = New BitmapImage(New Uri("pack://application:,,,/Resources/crowd.png"))
        txtDescription.Visibility = Visibility.Collapsed
        tbDirections.Text = "Please select business groups"
        lbxBusinessGroups.Visibility = Visibility.Visible
        imgCheckMark.Visibility = Visibility.Visible
        PopulateBusinessGroups()
    End Sub

    Private Sub BusinessGroupsSelected(sender As Object, e As MouseButtonEventArgs) Handles imgCheckMark.MouseLeftButtonDown
        If ValidateGroups() = True Then GetHours()
    End Sub

    Private Sub GetHours()
        imgIcon.Source = New BitmapImage(New Uri("pack://application:,,,/Resources/clock.png"))
        lbxBusinessGroups.Visibility = Visibility.Collapsed
        imgCheckMark.Visibility = Visibility.Collapsed
        tbDirections.Text = "Enter the number of training hours."
        nbxTrainingHours = New NumberBox(120, True, False, True, False, True, 10, 1) With
            {.Margin = New Thickness(271, 47, 0, 0)}
        grdMain.Children.Add(nbxTrainingHours)
        nbxTrainingHours.UserFocus()
        AddHandler nbxTrainingHours.KeyDown, AddressOf CheckForHoursEnter
    End Sub

    Private Sub CheckForHoursEnter(sender As Object, e As KeyEventArgs)
        If e.Key = Key.Return Then
            e.Handled = True
            If ValidateHours() = True Then GetScore()
        End If

    End Sub

    Private Sub GetScore()
        pbxScore = New PercentBox(60, True, 10, 0, "100%", True, False)
        imgIcon.Source = New BitmapImage(New Uri("pack://application:,,,/Resources/reportcard.png"))
        nbxTrainingHours.Visibility = Visibility.Collapsed
        tbDirections.Text = "Is the training scored?"
        imgScoreYes.Visibility = Visibility.Visible
        AddHandler imgScoreYes.MouseLeftButtonDown, AddressOf ScoreYes
        imgScoreNo.Visibility = Visibility.Visible
        AddHandler imgScoreNo.MouseLeftButtonDown, AddressOf ScoreNo

    End Sub

    Private Sub ScoreYes()
        Score = True
        tbDirections.Text = "What is the passing percentage?"
        imgScoreYes.Visibility = Visibility.Collapsed
        imgScoreNo.Visibility = Visibility.Collapsed
        pbxScore = New PercentBox(60, True, 10, 0, "100%", True, False)
        pbxScore.Margin = New Thickness(271, 47, 0, 0)
        grdMain.Children.Add(pbxScore)
        pbxScore.UserFocus()
        AddHandler pbxScore.KeyDown, AddressOf CheckForScoreEnter
    End Sub

    Private Sub CheckForScoreEnter(sender As Object, e As KeyEventArgs)
        If e.Key = Key.Return Then
            e.Handled = True
            If ValidateScore() = True Then GetCert()
        End If

    End Sub

    Private Sub ScoreNo()
        imgScoreYes.Visibility = Visibility.Collapsed
        imgScoreNo.Visibility = Visibility.Collapsed
        GetCert()
    End Sub

    Private Sub GetCert()
        imgIcon.Source = New BitmapImage(New Uri("pack://application:,,,/Resources/medal.png"))
        pbxScore.Visibility = Visibility.Collapsed
        imgCertYes.Visibility = Visibility.Visible
        AddHandler imgCertYes.MouseLeftButtonDown, AddressOf CertYes
        imgCertNo.Visibility = Visibility.Visible
        AddHandler imgCertNo.MouseLeftButtonDown, AddressOf CertNo
        tbDirections.Text = "Does the training have certification?"

    End Sub

    Private Sub CertYes()
        Cert = True
        Inspect()
    End Sub

    Private Sub CertNo()
        Cert = False
        Inspect()
    End Sub

    Private Sub Inspect()
        tbDirections.Text = "Please confirm the information to save it."
        imgCertYes.Visibility = Visibility.Collapsed
        imgCertNo.Visibility = Visibility.Collapsed
        imgIcon.Source = New BitmapImage(New Uri("pack://application:,,,/Resources/inspect.png"))
        PopulateConfirmInfo()
        tbConfirm.Visibility = Visibility.Visible
        imgConfirmYes.Visibility = Visibility.Visible
        AddHandler imgConfirmYes.MouseLeftButtonDown, AddressOf ConfirmYes
        imgConfirmNo.Visibility = Visibility.Visible
        AddHandler imgConfirmYes.MouseLeftButtonDown, AddressOf ConfirmNo
    End Sub

    Private Sub PopulateConfirmInfo()
        Dim ConfirmString As String = "You wish to add the training " & txtTrainingName.Text & ", which takes " &
        Hours & " hours.  "
        If lbxBusinessGroups.SelectedItems.Count = 1 Then
            If lbxBusinessGroups.SelectedItem = "All" Then
                ConfirmString = ConfirmString & "It is available to everyone"
            Else
                ConfirmString = ConfirmString & "It is available to " & lbxBusinessGroups.SelectedItem
            End If
        Else
            ConfirmString = ConfirmString & "It is available to multiple groups"
        End If
        If Score = True Then
            ConfirmString = ConfirmString & ", and requires a score of " & FormatPercent(Skore, 1) & " to pass"
            If Cert = True Then
                ConfirmString = ConfirmString & " and receive certification.  "
            Else
                ConfirmString = ConfirmString & ".  "
            End If
        Else
            ConfirmString = ConfirmString & ", and has no scoring"
            If Cert = True Then
                ConfirmString = ConfirmString & ", but does have certification.  "
            Else
                ConfirmString = ConfirmString & " or certification."
            End If
        End If
        tbConfirm.Text = ConfirmString
    End Sub

    Private Sub ConfirmYes()
        SaveTraining()
    End Sub

    Private Sub ConfirmNo()
        Close()
    End Sub

    Private Sub PopulateBusinessGroups()
        Dim qbg = From bg In SharedDataGroup.BusinessGroups
                  Select bg
                  Order By bg.BusinessGroup1

        lbxBusinessGroups.Items.Clear()
        lbxBusinessGroups.Items.Add("All")
        For Each bg In qbg
            lbxBusinessGroups.Items.Add(bg.BusinessGroup1)
        Next

    End Sub

    Private Sub SaveTraining()
        Dim ntt As New TrainingType
        With ntt
            .TrainingName = TrainingName
            .TrainingDescription = Description
            .Hours = Hours
            .Certification = Cert
            .Scored = Score
            .PassCertScore = Skore
        End With
        TrainingData.TrainingTypes.Add(ntt)
        TrainingData.SaveChanges()

        '// Retrieve newly saved PID
        Dim qnt = (From e In TrainingData.TrainingTypes
                   Where e.TrainingName = TrainingName
                   Select e).ToList(0)
        For Each i In GroupList
            Dim ntg As New BusinessGroupTraining_Join

            If i = "All" Then
                ntg.BusinessGroupId = 0
                ntg.TrainingId = qnt.PID
                TrainingData.BusinessGroupTraining_Join.Add(ntg)
                Exit For
            Else
                ntg.BusinessGroupId = GetBizGroup(i)
                ntg.TrainingId = qnt.PID
                TrainingData.BusinessGroupTraining_Join.Add(ntg)
            End If
        Next
        TrainingData.SaveChanges()
        Close()
    End Sub

    Private Function GetBizGroup(i) As Long
        Dim bg As String = i.ToString
        Dim qbg = (From g In SharedDataGroup.BusinessGroups
                   Where g.BusinessGroup1 = bg
                   Select g).ToList(0)
        Return qbg.PID
    End Function

    Private Function ValidateName(fieldval As String) As Boolean
        If txtTrainingName.Text = "" Then Return False
        TrainingName = txtTrainingName.Text
        Return True
    End Function

    Private Function ValidateDesc(fieldval As String) As Boolean
        If txtDescription.Text = "Add description (255 characters)" Then txtDescription.Text = ""
        Description = txtDescription.Text
        Return True
    End Function

    Private Function ValidateGroups() As Boolean
        GroupList.Clear()
        If lbxBusinessGroups.SelectedItems.Count = 0 Then Return False
        For Each i In lbxBusinessGroups.SelectedItems
            GroupList.Add(i)
            If i = "All" Then Exit For
        Next
        Return True
    End Function

    Private Function ValidateHours() As Boolean
        Try
            Hours = FormatNumber(nbxTrainingHours.BaseTextBox.Text, 1)
        Catch ex As Exception
            Return False
        End Try
        If Hours = 0 Then Return False
        Return True
    End Function

    Private Function ValidateScore() As Boolean
        Try
            Skore = FormatNumber(pbxScore.BaseTextBox.Text, 4)
        Catch ex As Exception
            Return False
        End Try
        If Skore = 0 Then Return False
        If Skore > 1 Then Skore = Skore / 100
        Return True
    End Function

#End Region

End Class
