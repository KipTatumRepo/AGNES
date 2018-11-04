Module ForecastModule
#Region "Properties"
    Public AvailableFcastUnits As UnitGroup
    Public FcastPage As Forecast
    Public InitialForecastLoadStatus As Byte = 1

#End Region

#Region "Public Methods"
    Public Sub Runmodule()
        FlashActuals = New FlashActualsEntity
        Dim SelectedFcastType As Byte, SelectedFcastUnit As Long
        Dim GetSelections = SelectFlashForecastTypeAndUnit()
        SelectedFcastType = GetSelections.flashselection
        SelectedFcastUnit = GetSelections.unitselection
        FcastPage = New Forecast(SelectedFcastType, SelectedFcastUnit)
        FcastPage.SaveStatus = InitialForecastLoadStatus
        FcastPage.ShowDialog()

        '// Additional follow up modules/user inputs are invoked here, after the forecast is closed

    End Sub

#End Region

#Region "Private Methods"
    'Private Sub SaveSickOtPay(ot As Double, sick As Double, un As Long)
    '    Dim qsp = From osp In FlashActuals.SickOtRecords
    '              Select osp
    '              Where osp.MSFY = CurrentFiscalYear And
    '                    osp.MSP = FlashPage.MSP.CurrentPeriod And
    '                    osp.Week = FlashPage.Wk.CurrentWeek And
    '                    osp.UnitNumber = un

    '    Dim notsp As New SickOtRecord
    '    With notsp
    '        .UnitNumber = un
    '        .MSFY = CurrentFiscalYear
    '        .MSP = FlashPage.MSP.CurrentPeriod
    '        .Week = FlashPage.Wk.CurrentWeek
    '        .OtPay = ot
    '        .SickPay = sick
    '    End With
    '    FlashActuals.SickOtRecords.Add(notsp)
    '    FlashActuals.SaveChanges()
    'End Sub

#End Region

End Module
