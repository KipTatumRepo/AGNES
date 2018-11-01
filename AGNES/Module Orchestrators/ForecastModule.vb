﻿Module ForecastModule
#Region "Properties"
    Public AvailableFcastUnits As UnitGroup
    'Public FCastRecords As FlashActualsEntity
    Public FcastPage As Forecast
    Public InitialForecastLoadStatus As Byte = 0

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

        '// Additional follow up modules/user inputs are invoked here, after the flash is closed
        'Select Case SelectedFlashType
        '    Case 1, 2   ' Puget Sound Cafes and Commons

        '        '// Sick time and overtime, if not present already
        '        Dim qsp = From osp In FlashActuals.SickOtRecords
        '                  Select osp
        '                  Where osp.MSFY = CurrentFiscalYear And
        '                osp.MSP = FlashPage.MSP.CurrentPeriod And
        '                osp.Week = FlashPage.Wk.CurrentWeek And
        '                osp.UnitNumber = SelectedFlashUnit

        '        If qsp.Count = 0 And (FlashPage.SaveStatus = 1 Or FlashPage.SaveStatus = 3) Then ' Add new
        '            Dim SickPay As New SingleUserInput(True)
        '            With SickPay
        '                .InputType = 1
        '                .lblInputDirection.Text = "Enter your sick pay for the week."
        '                .txtUserInput.Focus()
        '                .ShowDialog()
        '            End With

        '            Dim OtPay As New SingleUserInput(True)
        '            With OtPay
        '                .InputType = 1
        '                .lblInputDirection.Text = "Enter your overtime pay for the week."
        '                .txtUserInput.Focus()
        '                .ShowDialog()
        '            End With

        '            '// Add sick overtime pay to db
        '            SaveSickOtPay(OtPay.CurrencyVal, SickPay.CurrencyVal, SelectedFlashUnit)
        '            OtPay.Close()
        '            SickPay.Close()
        '        End If

        'End Select

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
