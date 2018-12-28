Imports System.ComponentModel
Public Class Trainers

#Region "Properties"

#End Region

#Region "Constructor"
    Public Sub New()
        InitializeComponent()
        PopulateTraining()
    End Sub
#End Region

#Region "Public Methods"

#End Region

#Region "Private Methods"

    Private Sub PopulateTraining()
        cbxTrainings.Items.Clear()
        Dim qtt = From tt In TrainingData.TrainingTypes
                  Select tt

        For Each tt In qtt
            cbxTrainings.Items.Add(tt.TrainingName)
        Next

    End Sub

    Private Sub TrainingSelected(sender As Object, e As SelectionChangedEventArgs) Handles cbxTrainings.SelectionChanged
        If cbxTrainings.SelectedIndex = -1 Then Exit Sub
        PopulateAvailableAndAssignedTrainers()
    End Sub

    Private Sub PopulateAvailableAndAssignedTrainers()
        Dim tid As Long = TrainingModule.TrainMod.GetTrainingId(cbxTrainings.SelectedItem.ToString)
        Dim AssignedDict As New Dictionary(Of Long, String)
        Dim SuppAssigned As Boolean

        lbxAvailable.Items.Clear()
        lbxAssigned.Items.Clear()

        Dim qta = From ta In TrainingData.TrainerTraining_Join.AsEnumerable()
                  Where ta.TrainingId = tid
                  Select ta


        For Each ta In qta
            If ta.TrainerId = 0 Then
                lbxAssigned.Items.Add("Support Teams")
                SuppAssigned = True
            Else
                AssignedDict.Add(ta.TrainerId, ta.TrainerId.ToString())
            End If
        Next
        Dim x As Integer = qta.Count
        Dim y As Boolean = AssignedDict.ContainsKey(0)
        If qta.Count = 0 Or (qta.Count > 0 And SuppAssigned = False) Then lbxAvailable.Items.Add("Support Teams")

        For Each emp In TrainingModule.EmpDict
            If AssignedDict.ContainsKey(emp.Key) Then
                lbxAssigned.Items.Add(emp.Value)
            Else
                lbxAvailable.Items.Add(emp.Value)
            End If
        Next
    End Sub

    Private Sub AddTrainer(sender As Object, e As MouseButtonEventArgs) Handles imgAddTrainer.MouseLeftButtonDown
        If lbxAvailable.SelectedIndex = -1 Then Exit Sub
        lbxAssigned.Items.Add(lbxAvailable.SelectedItem)
        lbxAvailable.Items.RemoveAt(lbxAvailable.SelectedIndex)
    End Sub

    Private Sub RemoveTrainer(sender As Object, e As MouseButtonEventArgs) Handles imgRemoveTrainer.MouseLeftButtonDown
        If lbxAssigned.SelectedIndex = -1 Then Exit Sub
        lbxAvailable.Items.Add(lbxAssigned.SelectedItem)
        lbxAssigned.Items.RemoveAt(lbxAssigned.SelectedIndex)
    End Sub

    Private Sub SaveTrainers(sender As Object, e As MouseButtonEventArgs) Handles imgSave.MouseLeftButtonDown
        '// Clear list of all trainers assigned to this training first
        Dim tid As Long = TrainingModule.TrainMod.GetTrainingId(cbxTrainings.SelectedItem)

        For Each t As TrainerTraining_Join In TrainingData.TrainerTraining_Join
            If t.TrainingId = tid Then TrainingData.TrainerTraining_Join.Remove(t)
        Next
        TrainingData.SaveChanges()

        For Each i In lbxAssigned.Items
            Dim ntj As New TrainerTraining_Join
            If i = "Support Teams" Then
                ntj.TrainerId = 0
            Else
                ntj.TrainerId = GetEmpId(i)
            End If
            ntj.TrainingId = tid
            TrainingData.TrainerTraining_Join.Add(ntj)
        Next
        TrainingData.SaveChanges()
    End Sub

    Private Function GetEmpId(i) As Long
        Dim nameparts As String() = i.Split(New Char() {","c})
        Dim lname As String = nameparts(0)
        Dim fname As String = nameparts(1).TrimStart
        Dim qei = (From ei In SharedDataGroup.EmployeeLists
                   Where ei.LastName = lname And
                           ei.FirstName = fname
                   Select ei).ToList(0)

        Return qei.PID
    End Function
#End Region

End Class
