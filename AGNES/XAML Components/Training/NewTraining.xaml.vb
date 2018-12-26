Public Class NewTraining
#Region "Properties"
    Private Changesmade As Boolean
    Private nbxTrainingHours As NumberBox
    Private pbxScore As PercentBox
    Private Cert As Boolean
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
        imgIcon.Source = New BitmapImage(New Uri("pack://application:,,,/Resources/reportcard.png"))
        nbxTrainingHours.Visibility = Visibility.Collapsed
        tbDirections.Text = "Is the training scored?"
        imgScoreYes.Visibility = Visibility.Visible
        AddHandler imgScoreYes.MouseLeftButtonDown, AddressOf ScoreYes
        imgScoreNo.Visibility = Visibility.Visible
        AddHandler imgScoreNo.MouseLeftButtonDown, AddressOf ScoreNo

    End Sub

    Private Sub ScoreYes()
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
        tbDirections.Text = "Confirm?"
        imgCertYes.Visibility = Visibility.Collapsed
        imgCertNo.Visibility = Visibility.Collapsed
        imgIcon.Source = New BitmapImage(New Uri("pack://application:,,,/Resources/inspect.png"))
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

    Private Function ValidateName(fieldval As String) As Boolean
        Return True
    End Function

    Private Function ValidateDesc(fieldval As String) As Boolean
        Return True
    End Function

    Private Function ValidateGroups() As Boolean
        Return True
    End Function

    Private Function ValidateHours() As Boolean
        Return True
    End Function

    Private Function ValidateScore() As Boolean
        Return True
    End Function
#End Region



End Class
