Module FlashModule
    Public AvailableUnits As UnitGroup
    Public FlashBudgets As BudgetEntity
    Public FlashActuals As FlashActualsEntity
    Public FlashForecasts As ForecastEntity
    Public SharedDataGroup As BIEntities
    Public AGNESShared As AGNESSharedDataEntity
    Public FlashPage As Flash
    Public InitialLoadStatus As Byte = 0
    Public CurrentFiscalYear As Integer = 2019
    Public Sub Runmodule()
        FlashBudgets = New BudgetEntity
        FlashActuals = New FlashActualsEntity
        FlashForecasts = New ForecastEntity
        SharedDataGroup = New BIEntities
        AGNESShared = New AGNESSharedDataEntity
        Dim SelectedFlashType As Byte, SelectedFlashUnit As Long
        Dim GetSelections = SelectFlashForecastTypeAndUnit()
        SelectedFlashType = GetSelections.flashselection
        SelectedFlashUnit = GetSelections.unitselection
        FlashPage = New Flash(SelectedFlashType, SelectedFlashUnit)
        FlashPage.SaveStatus = InitialLoadStatus
        FlashPage.ShowDialog()

        '// Additional follow up modules, such as Sick/OT, etc., are invoked here

    End Sub
End Module
