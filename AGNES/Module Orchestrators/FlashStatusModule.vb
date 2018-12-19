Module FlashStatusModule
    Public FlashStatusPage As FlashStatus
    Public FlashStatusActuals As FlashActualsEntity

    Public Sub Runmodule()
        FlashActuals = New FlashActualsEntity
        Dim StatusFlashType As Long
        Dim GetSelections = SelectFlashForecastTypeAndUnit(True)
        StatusFlashType = GetSelections.flashselection
        FlashStatusPage = New FlashStatus(StatusFlashType)
        FlashStatusPage.ShowDialog()
    End Sub

End Module
