Module BaseModule
    Public SharedDataGroup As BIEntities
    Public AGNESShared As AGNESSharedDataEntity
    Public FlashBudgets As BudgetEntity
    Public FlashForecasts As ForecastEntity
    Public BGE As BGCRMEntity
    Public Sub Runmodule()
        SharedDataGroup = New BIEntities
        AGNESShared = New AGNESSharedDataEntity
        BGE = New BGCRMEntity
        FlashBudgets = New BudgetEntity
        FlashForecasts = New ForecastEntity
    End Sub
End Module
