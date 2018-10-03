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
    Private _savestatus As Byte
    Public Property SaveStatus As Byte
        Get
            Return _savestatus
        End Get
        Set(value As Byte)
            _savestatus = value
            Select Case value
                Case 0      '   Unsaved
                    tbSaveStatus.Text = "Changes not saved"
                    barSaveStatus.Background = Brushes.Red
                Case 1      '   Draft
                    tbSaveStatus.Text = "Draft saved"
                    barSaveStatus.Background = Brushes.Yellow
                Case 2      '   Saved
                    tbSaveStatus.Text = "Flash saved"
                    barSaveStatus.Background = Brushes.LightGreen
                Case 3      '   Final
                    tbSaveStatus.Text = "Flash Locked"
                    barSaveStatus.Background = Brushes.LightGreen
            End Select
        End Set
    End Property

    Public Sub New(FlashType, FlashUnit)
        InitializeComponent()
        ConstructTemplate(FlashType)
    End Sub

    Private Sub ConstructTemplate(FT)
        grdFlashGroups.Children.Clear()
        '// Add period, week, and unit chooser controls 
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
                SalesGroup = New FlashGroup(MSP, Wk, Units, "Sales", False, 74, True, False, True, True, False) ' Increments of 47 for flashgroup spacing
                CogsGroup = New FlashGroup(MSP, Wk, Units, "COGS", True, 121, False, False, True, False, True) With {.SalesFlashGroup = SalesGroup}
                LaborGroup = New FlashGroup(MSP, Wk, Units, "Labor", True, 168, True, False, True, False, True) With {.SalesFlashGroup = SalesGroup}
                OpexGroup = New FlashGroup(MSP, Wk, Units, "OPEX", True, 215, False, False, True, False, True) With {.SalesFlashGroup = SalesGroup}
                SubsidyGroup = New FlashGroup(MSP, Wk, Units, "Subsidy", True, 262, True, True, True, False, False) With {.SalesFlashGroup = SalesGroup}
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
                lblForecast.Visibility = Visibility.Visible
                lblForecastPercentage.Visibility = Visibility.Visible
                lblForecastVariance.Visibility = Visibility.Visible
                CamGroup = New FlashGroup(MSP, Wk, Units, "CAM Revenue", False, 112, True, False, True, True, False) ' Increments of 47 for flashgroup spacing
                CogsGroup = New FlashGroup(MSP, Wk, Units, "COGS", True, 159, False, False, True, False, True) With {.SalesFlashGroup = CamGroup}
                LaborGroup = New FlashGroup(MSP, Wk, Units, "Labor", True, 206, True, False, True, False, True) With {.SalesFlashGroup = CamGroup}
                OpexGroup = New FlashGroup(MSP, Wk, Units, "OPEX", True, 253, False, False, True, False, True) With {.SalesFlashGroup = CamGroup}
                SubsidyGroup = New FlashGroup(MSP, Wk, Units, "Subsidy", True, 300, True, True, True, False, False, New List(Of FlashGroup) From {CamGroup, CogsGroup, LaborGroup, OpexGroup}) With {.SalesFlashGroup = CamGroup}
                With grdFlashGroups.Children
                    .Add(CamGroup)
                    .Add(CogsGroup)
                    .Add(LaborGroup)
                    .Add(OpexGroup)
                    .Add(SubsidyGroup)
                End With
                Height = 510 - 141 + 40
                Title = "WCC Weekly Financial Flash"
            Case "Field"
                TotalSalesGroup = New FlashGroup(MSP, Wk, Units, "Cafe Sales", False, 74, True, False, True, True, False) ' Increments of 47 for flashgroup spacing
                CateringSalesGroup = New FlashGroup(MSP, Wk, Units, "Catering Sales", True, 121, False, False, True, True, False)
                SalesGroup = New FlashGroup(MSP, Wk, Units, "Total Sales", True, 168, True, True, True, True, False)
                CogsGroup = New FlashGroup(MSP, Wk, Units, "COGS", True, 215, False, False, True, False, True) With {.SalesFlashGroup = TotalSalesGroup}
                LaborGroup = New FlashGroup(MSP, Wk, Units, "Labor", True, 262, True, False, True, False, True) With {.SalesFlashGroup = TotalSalesGroup}
                OpexGroup = New FlashGroup(MSP, Wk, Units, "OPEX", True, 309, False, False, True, False, True) With {.SalesFlashGroup = TotalSalesGroup}
                FeesGroup = New FlashGroup(MSP, Wk, Units, "Fees", True, 356, True, False, True, False, True) With {.SalesFlashGroup = TotalSalesGroup}
                TotalGroup = New FlashGroup(MSP, Wk, Units, "Total", True, 403, True, True, True, False, False) With {.SalesFlashGroup = TotalSalesGroup}
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

        For Each fg As FlashGroup In grdFlashGroups.Children
            fg.Load()
            If fg.GroupIsSubTotal = True Then fg.Update(fg)
        Next


    End Sub

End Class
