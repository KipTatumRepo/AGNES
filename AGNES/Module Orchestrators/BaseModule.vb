Module BaseModule
    Public SharedDataGroup As BIEntities
    Public AGNESShared As AGNESSharedDataEntity
    Public FlashBudgets As BudgetEntity
    Public FlashForecasts As ForecastEntity
    Public TrainingData As TrainingEntities
    Public BGE As BGCRMEntity
    Public Sub Runmodule()
        SharedDataGroup = New BIEntities
        AGNESShared = New AGNESSharedDataEntity
        BGE = New BGCRMEntity
        FlashBudgets = New BudgetEntity
        FlashForecasts = New ForecastEntity
        TrainingData = New TrainingEntities
    End Sub
    Public Function TruncateAlias(UserAlias As String) As String
        Dim ReturnAlias As String = UserAlias
        Try
            ReturnAlias = UserAlias.Remove(UserAlias.IndexOf("@microsoft.com"))
        Catch ex As Exception
            '// No domain attached
        End Try
        Return ReturnAlias
    End Function
End Module
