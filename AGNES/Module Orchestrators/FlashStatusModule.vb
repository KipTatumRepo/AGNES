Module FlashStatusModule
    Public FlashStatusPage As FlashStatus
    Public FlashStatusActuals As FlashActualsEntity
    'TODO: FLASH STATUS NOT FUNCTIONING FOR NON-CAFE (SPECIFICALLY, OH)
    Public Sub RunModule()
        FlashActuals = New FlashActualsEntity
        Dim StatusFlashType As Byte
        Dim GetSelections = SelectFlashForecastTypeAndUnit(True)
        StatusFlashType = GetSelections.flashselection
        FlashStatusPage = New FlashStatus(StatusFlashType)
        FlashStatusPage.ShowDialog()
    End Sub

End Module
