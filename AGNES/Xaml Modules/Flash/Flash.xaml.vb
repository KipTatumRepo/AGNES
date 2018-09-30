Public Class Flash
    Dim SalesGroup As FlashGroup
    Dim CamGroup As FlashGroup
    Dim CafeSalesGroup As FlashGroup
    Dim CateringSalesGroup As FlashGroup
    Dim TotalSalesGroup As FlashGroup
    Dim CogsGroup As FlashGroup
    Dim LaborGroup As FlashGroup
    Dim OpexGroup As FlashGroup
    Dim FeesGroup As FlashGroup
    Dim SubsidyGroup As FlashGroup
    Dim TotalGroup As FlashGroup
    Dim Wk As WeekChooser
    Dim MSP As PeriodChooser
    Dim Units As UnitChooser

    Public Sub New(FlashType, FlashUnit)
        InitializeComponent()
        ConstructTemplate(FlashType)
    End Sub

    Private Sub Image_PreviewMouseDown(sender As Object, e As MouseButtonEventArgs)
        tbSaveStatus.Text = "Draft saved"
        barSaveStatus.Background = Brushes.Yellow
    End Sub

    Private Sub Image_PreviewMouseDown_1(sender As Object, e As MouseButtonEventArgs)
        tbSaveStatus.Text = "Flash saved"
        barSaveStatus.Background = Brushes.LightGreen
    End Sub

    Private Sub ConstructTemplate(FT)
        grdFlashGroups.Children.Clear()
        '// Add period, week, and unit chooser controls 

        'TODO: "Me" represents the object that the Chooser(s) is/are in; passing it by reference
        '      will allow a writeback of the selections period And/Or week to the object.  NEED TO TIE BACK ONCE THE FLASH OBJECT IS DONE.
        Dim currmsp As Byte = GetCurrentPeriod(FormatDateTime(Now(), DateFormat.ShortDate))
        Dim currwk As Byte = GetCurrentWeek(FormatDateTime(Now(), DateFormat.ShortDate))
        Wk = New WeekChooser(1, currwk, currwk)
        MSP = New PeriodChooser(Wk, 1, currmsp, currmsp)
        MSP.SelectAllEnabled = False

        Units = New UnitChooser(AvailableUnits)
        Units.AllowMultiSelect = True

        Dim sep As New Separator
        With tlbFlash.Items
            .Add(MSP)
            .Add(sep)
            .Add(Wk)
        End With
        tlbUnits.Items.Add(Units)
        If Units.NumberOfAvailableUnits = 1 Then Units.IsEnabled = False


        Select Case FT
            Case "Cafe"
                SalesGroup = New FlashGroup(MSP, Wk, Units, "Sales", False, 74, True, False) ' Increments of 47 for flashgroup spacing
                CogsGroup = New FlashGroup(MSP, Wk, Units, "COGS", True, 121, False, False)
                LaborGroup = New FlashGroup(MSP, Wk, Units, "Labor", True, 168, True, False)
                OpexGroup = New FlashGroup(MSP, Wk, Units, "OPEX", True, 215, False, False)
                SubsidyGroup = New FlashGroup(MSP, Wk, Units, "Subsidy", True, 262, True, True)
                With grdFlashGroups.Children
                    .Add(SalesGroup)
                    .Add(CogsGroup)
                    .Add(LaborGroup)
                    .Add(OpexGroup)
                    .Add(SubsidyGroup)
                End With
                Height = 510 - 141
                Title = "Cafe Weekly Financial Flash"
            Case "WCC"
                CamGroup = New FlashGroup(MSP, Wk, Units, "CAM Revenue", False, 74, True, False) ' Increments of 47 for flashgroup spacing
                CogsGroup = New FlashGroup(MSP, Wk, Units, "COGS", True, 121, False, False)
                LaborGroup = New FlashGroup(MSP, Wk, Units, "Labor", True, 168, True, False)
                OpexGroup = New FlashGroup(MSP, Wk, Units, "OPEX", True, 215, False, False)
                SubsidyGroup = New FlashGroup(MSP, Wk, Units, "Subsidy", True, 262, True, True)
                With grdFlashGroups.Children
                    .Add(CamGroup)
                    .Add(CogsGroup)
                    .Add(LaborGroup)
                    .Add(OpexGroup)
                    .Add(SubsidyGroup)
                End With
                Height = 510 - 141
                Title = "WCC Weekly Financial Flash"
            Case "Field"
                CafeSalesGroup = New FlashGroup(MSP, Wk, Units, "Cafe Sales", False, 74, True, False) ' Increments of 47 for flashgroup spacing
                CateringSalesGroup = New FlashGroup(MSP, Wk, Units, "Catering Sales", True, 121, False, False)
                TotalSalesGroup = New FlashGroup(MSP, Wk, Units, "Total Sales", True, 168, True, True)
                CogsGroup = New FlashGroup(MSP, Wk, Units, "COGS", True, 215, False, False)
                LaborGroup = New FlashGroup(MSP, Wk, Units, "Labor", True, 262, True, False)
                OpexGroup = New FlashGroup(MSP, Wk, Units, "OPEX", True, 309, False, False)
                FeesGroup = New FlashGroup(MSP, Wk, Units, "Fees", True, 356, True, False)
                TotalGroup = New FlashGroup(MSP, Wk, Units, "Total", True, 403, True, True)
                With grdFlashGroups.Children
                    .Add(CafeSalesGroup)
                    .Add(CateringSalesGroup)
                    .Add(TotalSalesGroup)
                    .Add(CogsGroup)
                    .Add(LaborGroup)
                    .Add(OpexGroup)
                    .Add(FeesGroup)
                    .Add(TotalGroup)
                End With
                Height = 510
                Title = "Field Weekly Financial Flash"
        End Select

    End Sub



End Class
