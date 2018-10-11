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

        'TODO: ADDITIONAL FOLLOW UP MODULES/USER INPUTS, SUCH AS SICK/OT, ETC., ARE INVOKED HERE AFTER THE FLASH IS CLOSED

    End Sub
End Module
