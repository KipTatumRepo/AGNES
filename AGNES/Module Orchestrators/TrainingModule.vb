Imports System.ComponentModel
Module TrainingModule
    Public TrainMod As Training
    Public EmpDict As Dictionary(Of Long, String)
    Public Sub Runmodule()
        PopulateAssociateDict()
        TrainMod = New Training
        TrainMod.ShowDialog()
    End Sub

    Private Sub PopulateAssociateDict()
        EmpDict = New Dictionary(Of Long, String)
        Dim qan = From anl In SharedDataGroup.EmployeeLists
                  Select anl
                  Order By anl.LastName, anl.FirstName


        For Each anl In qan
            Dim fname As String = anl.LastName & ", " & anl.FirstName
            EmpDict.Add(anl.PID, fname)
        Next

    End Sub
End Module
