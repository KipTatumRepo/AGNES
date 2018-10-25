Module FlashModule
    Public AvailableUnits As UnitGroup
    Public FlashActuals As FlashActualsEntity
    Public FlashPage As Flash
    Public InitialLoadStatus As Byte = 0

    Public Sub Runmodule()
        FlashActuals = New FlashActualsEntity
        Dim SelectedFlashType As Byte, SelectedFlashUnit As Long
        Dim GetSelections = SelectFlashForecastTypeAndUnit()
        SelectedFlashType = GetSelections.flashselection
        SelectedFlashUnit = GetSelections.unitselection
        FlashPage = New Flash(SelectedFlashType, SelectedFlashUnit)
        FlashPage.SaveStatus = InitialLoadStatus
        FlashPage.ShowDialog()

        '// Additional follow up modules/user inputs are invoked here, after the flash is closed
        Select Case SelectedFlashType
            Case 1, 2   ' Puget Sound Cafes and Commons
                Dim SickPay As New SingleUserInput
                With SickPay
                    .InputType = 1
                    .lblInputDirection.Text = "Enter your sick pay for the week."
                    .txtUserInput.Focus()
                    .ShowDialog()
                End With

                Dim OtPay As New SingleUserInput
                With OtPay
                    .InputType = 1
                    .lblInputDirection.Text = "Enter your overtime pay for the week."
                    .txtUserInput.Focus()
                    .ShowDialog()
                End With

                '// Add sick overtime pay to db
                SaveSickOtPay(OtPay.CurrencyVal, SickPay.CurrencyVal, SelectedFlashUnit)
                OtPay.Close()
                SickPay.Close()
        End Select

    End Sub

#Region "Private Methods"
    Private Sub SaveSickOtPay(ot As Double, sick As Double, un As Long)
        'Determine new or update and save accordingly
        Dim qsp = From osp In FlashActuals.SickOtRecords
                  Select osp
                  Where osp.MSFY = CurrentFiscalYear And
                        osp.MSP = FlashPage.MSP.CurrentPeriod And
                        osp.Week = FlashPage.Wk.CurrentWeek And
                        osp.UnitNumber = un

        If qsp.Count = 0 Then ' Add new
            Dim notsp As New SickOtRecord
            With notsp
                .UnitNumber = un
                .MSFY = CurrentFiscalYear
                .MSP = FlashPage.MSP.CurrentPeriod
                .Week = FlashPage.Wk.CurrentWeek
                .OtPay = ot
                .SickPay = sick
            End With
            FlashActuals.SickOtRecords.Add(notsp)
        Else   ' Update existing
            For Each osp In qsp
                With osp
                    .OtPay = ot
                    .SickPay = sick
                End With
            Next
        End If
        FlashActuals.SaveChanges()
    End Sub

#End Region

End Module
