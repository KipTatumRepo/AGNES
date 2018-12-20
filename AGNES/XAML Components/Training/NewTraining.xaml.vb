Public Class NewTraining
#Region "Properties"
    Private Changesmade As Boolean

    Public Sub New()
        InitializeComponent()
        txtTrainingName.Focus()
        txtTrainingName.SelectAll()
    End Sub

    Private Sub CheckForNameEnter(sender As Object, e As KeyEventArgs) Handles txtTrainingName.KeyDown
        If e.Key = Key.Return Then
            e.Handled = True
            If ValidateName(txtTrainingName.Text) = True Then GetDesc()
        End If
    End Sub

    Private Sub GetDesc()
        txtTrainingName.Visibility = Visibility.Collapsed
        tbDirections.Text = "Please provide a short description of the training"
        txtDescription.Visibility = Visibility.Visible
        With txtDescription
            .Focus()
            .SelectAll()
        End With

    End Sub

    Private Function ValidateName(fieldval As String) As Boolean
        Return True
    End Function

#End Region

#Region "Constructor"

#End Region

#Region "Public Methods"

#End Region

#Region "Private Methods"

#End Region



End Class
