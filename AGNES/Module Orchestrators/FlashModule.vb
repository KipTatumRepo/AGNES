Module FlashModule
    Public AvailableUnits As UnitGroup
    Public FlashActuals As FlashActualsEntity
    Public FlashPage As Flash
    Public InitialLoadStatus As Byte = 0
    Public CurrentFiscalYear As Integer = 2019
    Public Sub Runmodule()
        FlashActuals = New FlashActualsEntity
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
