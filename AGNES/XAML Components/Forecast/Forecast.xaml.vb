Imports System.ComponentModel
Public Class Forecast

#Region "Properties"
    'Dim SalesGroup As FcastGroup
    Dim CamGroup As ForecastGroup
    'Dim CafeSalesGroup As FcastGroup
    'Dim SalesTaxGroup As FcastGroup
    'Dim CateringSalesGroup As FcastGroup
    'Dim TotalSalesGroup As FcastGroup
    Dim CogsGroup As ForecastGroup
    Dim LaborGroup As ForecastGroup
    Dim OpexGroup As ForecastGroup
    'Dim FeesGroup As FcastGroup
    Dim SubsidyGroup As ForecastGroup
    'Dim TotalGroup As FcastGroup
    Dim Units As UnitChooser
    Public Property TypeOfFcast As Byte
    Public Property MSP As PeriodChooser
    Public Property Wk As WeekChooser
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
                    imgClear.Visibility = Visibility.Visible
                    imgApplyDrr.Visibility = Visibility.Visible
                    imgRefDrr.Visibility = Visibility.Visible
                Case 1      '   Draft
                    tbSaveStatus.Text = "Draft saved"
                    barSaveStatus.Background = Brushes.Yellow
                    imgClear.Visibility = Visibility.Visible
                    imgApplyDrr.Visibility = Visibility.Visible
                    imgRefDrr.Visibility = Visibility.Visible
                Case 2      '   Saved
                    tbSaveStatus.Text = "Forecast saved"
                    barSaveStatus.Background = Brushes.LightGreen
                    imgClear.Visibility = Visibility.Visible
                    imgApplyDrr.Visibility = Visibility.Visible
                    imgRefDrr.Visibility = Visibility.Visible
                Case 3      '   Final
                    tbSaveStatus.Text = "Forecast Locked"
                    barSaveStatus.Background = Brushes.LightGreen
                    imgClear.Visibility = Visibility.Collapsed
                    imgApplyDrr.Visibility = Visibility.Collapsed
                    imgRefDrr.Visibility = Visibility.Collapsed
            End Select
        End Set
    End Property

#End Region

#Region "Constructor"
    Public Sub New(FcastType, FCastUnit)
        InitializeComponent()
        TypeOfFcast = FcastType
        ConstructTemplate(FcastType, FCastUnit)
    End Sub

#End Region

#Region "Public Methods"

#End Region

#Region "Private Methods"
    Private Sub ConstructTemplate(FT As Byte, FU As Long)
        grdFcastGroups.Children.Clear()
        '// Add period, week, and unit chooser controls 
        Dim currmsp As Byte = GetCurrentPeriod(FormatDateTime(Now(), DateFormat.ShortDate))
        Dim currwk As Byte = GetCurrentWeek(FormatDateTime(Now(), DateFormat.ShortDate))
        Wk = New WeekChooser(1, currwk, currwk)
        MSP = New PeriodChooser(Wk, currmsp, 12, currmsp)
        MSP.SelectAllEnabled = False

#Region "ToComplete"
        Select Case FT
#Region "Commons"
            Case 1      '   Commons Flash
                Title = "WCC Period Financial Forecast - Unit " & FU
                Height = 369
                AvailableUnits = New UnitGroup With {.UnitGroupName = "WCC"}

                '// Add Unit and/or Subunits
                Dim qsu = From su In AGNESShared.UnitsSubunits
                          Where su.UnitNumber = FU
                          Select su

                If qsu.Count > 0 Then
                    For Each su In qsu
                        Dim subunit As New UnitFlash With {.FlashType = AvailableUnits.UnitGroupName, .UnitNumber = su.SubUnitNumber}
                        AvailableUnits.UnitsInGroup.Add(subunit)
                        tlbUnits.Visibility = Visibility.Visible
                    Next
                Else
                    Dim FlashUnit As New UnitFlash With {.FlashType = AvailableUnits.UnitGroupName, .UnitNumber = FU}
                    AvailableUnits.UnitsInGroup.Add(FlashUnit)
                    tlbUnits.Visibility = Visibility.Hidden
                    grdColumnLabels.Margin = New Thickness(0, 42, 0, 0)
                    grdFcastGroups.Margin = New Thickness(0, 74, 0, 0)
                End If
                Units = New UnitChooser(AvailableUnits)
                If qsu.Count > 0 Then Units.AllowMultiSelect = True

                '// Add forecast groups (categories)
                CamGroup = New ForecastGroup(MSP, Units, "CAM Revenue", False, 0, True, False, True, False) ' Increments of 47 for flashgroup spacing 
                CogsGroup = New ForecastGroup(MSP, Units, "COGS", True, 47, False, False, False, True) With {.SalesFcastGroup = CamGroup}
                LaborGroup = New ForecastGroup(MSP, Units, "Labor", True, 94, True, False, False, True) With {.SalesFcastGroup = CamGroup}
                OpexGroup = New ForecastGroup(MSP, Units, "OPEX", True, 141, False, False, False, True) With {.SalesFcastGroup = CamGroup}
                SubsidyGroup = New ForecastGroup(MSP, Units, "Subsidy", True, 188, True, True, False, False, New List(Of ForecastGroup) From {CamGroup, CogsGroup, LaborGroup, OpexGroup}) With {.SalesFcastGroup = CamGroup}
                With grdFcastGroups.Children
                    .Add(CamGroup)
                    .Add(CogsGroup)
                    .Add(LaborGroup)
                    .Add(OpexGroup)
                    .Add(SubsidyGroup)
                End With
#End Region

#Region "Cafes"
            Case 2      ' Puget Sound Cafe Forecast
                Title = "Cafe Period Financial Forecast - Unit " & FU
                Height = 369
                AvailableUnits = New UnitGroup With {.UnitGroupName = "Cafes"}

                '// Add Unit and/or Subunits
                Dim qsu = From su In AGNESShared.UnitsSubunits
                          Where su.UnitNumber = FU
                          Select su

                If qsu.Count > 0 Then
                    For Each su In qsu
                        Dim subunit As New UnitFlash With {.FlashType = AvailableUnits.UnitGroupName, .UnitNumber = su.SubUnitNumber}
                        AvailableUnits.UnitsInGroup.Add(subunit)
                        tlbUnits.Visibility = Visibility.Visible
                    Next
                Else
                    Dim FcastUnit As New UnitFlash With {.FlashType = AvailableUnits.UnitGroupName, .UnitNumber = FU}
                    AvailableUnits.UnitsInGroup.Add(FcastUnit)
                    tlbUnits.Visibility = Visibility.Hidden
                    grdColumnLabels.Margin = New Thickness(0, 42, 0, 0)
                    grdFcastGroups.Margin = New Thickness(0, 74, 0, 0)
                End If
                Units = New UnitChooser(AvailableUnits)
                If qsu.Count > 0 Then Units.AllowMultiSelect = True

                '// Add forecast-specific forecastgroups (categories)
                'SalesGroup = New FlashGroup(MSP, Wk, Units, "Sales", False, 0, True, False, True, True, False) ' Increments of 47 for flashgroup spacing
                'CogsGroup = New FlashGroup(MSP, Wk, Units, "COGS", True, 47, False, False, True, False, True) With {.SalesFlashGroup = SalesGroup}
                'LaborGroup = New FlashGroup(MSP, Wk, Units, "Labor", True, 94, True, False, True, False, True) With {.SalesFlashGroup = SalesGroup}
                'OpexGroup = New FlashGroup(MSP, Wk, Units, "OPEX", True, 141, False, False, True, False, True) With {.SalesFlashGroup = SalesGroup}
                'SubsidyGroup = New FlashGroup(MSP, Wk, Units, "Subsidy", True, 188, True, True, True, False, False, New List(Of FlashGroup) From {SalesGroup, CogsGroup, LaborGroup, OpexGroup}) With {.SalesFlashGroup = SalesGroup}
                'With grdFlashGroups.Children
                '    .Add(SalesGroup)
                '    .Add(CogsGroup)
                '    .Add(LaborGroup)
                '    .Add(OpexGroup)
                '    .Add(SubsidyGroup)
                'End With
#End Region

#Region "A/V"
                '            Case 3      ' A/V Flash
                '                Title = "A/V Weekly Financial Flash"
                '                Height = 460
                '                AvailableUnits = New UnitGroup With {.UnitGroupName = "AV"}

                '                '// Add Unit and/or Subunits
                '                Dim qsu = From su In AGNESShared.UnitsSubunits
                '                          Where su.UnitNumber = FU
                '                          Select su

                '                If qsu.Count > 0 Then
                '                    For Each su In qsu
                '                        Dim subunit As New UnitFlash With {.FlashType = AvailableUnits.UnitGroupName, .UnitNumber = su.SubUnitNumber}
                '                        AvailableUnits.UnitsInGroup.Add(subunit)
                '                        tlbUnits.Visibility = Visibility.Visible
                '                    Next
                '                Else
                '                    Dim FlashUnit As New UnitFlash With {.FlashType = AvailableUnits.UnitGroupName, .UnitNumber = FU}
                '                    AvailableUnits.UnitsInGroup.Add(FlashUnit)
                '                    tlbUnits.Visibility = Visibility.Hidden
                '                    grdColumnLabels.Margin = New Thickness(0, 42, 0, 0)
                '                    grdFlashGroups.Margin = New Thickness(0, 74, 0, 0)
                '                End If
                '                Units = New UnitChooser(AvailableUnits)
                '                If qsu.Count > 0 Then Units.AllowMultiSelect = True

                '                '// Add flash-specific flashgroups (categories)
                '                SalesGroup = New FlashGroup(MSP, Wk, Units, "Sales", False, 0, True, False, True, False, False) With {.SpreadByWeeks = True}
                '                SalesTaxGroup = New FlashGroup(MSP, Wk, Units, "Sales Tax", False, 47, False, False, True, False, False) With {.SpreadByWeeks = True}

                '                LaborGroup = New FlashGroup(MSP, Wk, Units, "Labor", False, 94, True, False, True, False, False) With {.SpreadByWeeks = True}
                '                OpexGroup = New FlashGroup(MSP, Wk, Units, "OPEX", False, 141, False, False, True, False, False) With {.SpreadByWeeks = True}
                '                FeesGroup = New FlashGroup(MSP, Wk, Units, "Fees", False, 188, True, False, True, False, False) With {.SpreadByWeeks = True}
                '                SubsidyGroup = New FlashGroup(MSP, Wk, Units, "Subsidy", False, 188 + 47, True, True, True, False, False, New List(Of FlashGroup) From {SalesGroup, SalesTaxGroup, LaborGroup, OpexGroup, FeesGroup})
                '                With grdFlashGroups.Children
                '                    .Add(SalesGroup)
                '                    .Add(SalesTaxGroup)
                '                    .Add(LaborGroup)
                '                    .Add(OpexGroup)
                '                    .Add(FeesGroup)
                '                    .Add(SubsidyGroup)
                '                End With

#End Region

#Region "Field Sites"
                '            Case 4      ' Field Site Flash
                '                Title = "Field Site Weekly Financial Flash"
                '                Height = 600
                '                AvailableUnits = New UnitGroup With {.UnitGroupName = "Cafes"}

                '                '// Add Unit and/or Subunits
                '                Dim qsu = From su In AGNESShared.UnitsSubunits
                '                          Where su.UnitNumber = FU
                '                          Select su

                '                If qsu.Count > 0 Then
                '                    For Each su In qsu
                '                        Dim subunit As New UnitFlash With {.FlashType = AvailableUnits.UnitGroupName, .UnitNumber = su.SubUnitNumber}
                '                        AvailableUnits.UnitsInGroup.Add(subunit)
                '                        tlbUnits.Visibility = Visibility.Visible
                '                    Next
                '                Else
                '                    Dim FlashUnit As New UnitFlash With {.FlashType = AvailableUnits.UnitGroupName, .UnitNumber = FU}
                '                    AvailableUnits.UnitsInGroup.Add(FlashUnit)
                '                    tlbUnits.Visibility = Visibility.Hidden
                '                    grdColumnLabels.Margin = New Thickness(0, 42, 0, 0)
                '                    grdFlashGroups.Margin = New Thickness(0, 74, 0, 0)
                '                End If
                '                Units = New UnitChooser(AvailableUnits)
                '                If qsu.Count > 0 Then Units.AllowMultiSelect = True

                '                '// Add flash-specific flashgroups (categories)
                '                CafeSalesGroup = New FlashGroup(MSP, Wk, Units, "Sales", False, 0, True, False, True, True, False)
                '                CateringSalesGroup = New FlashGroup(MSP, Wk, Units, "Catering Sales", False, 47, False, False, True, True, False)
                '                SalesTaxGroup = New FlashGroup(MSP, Wk, Units, "Sales Tax", False, 94, True, False, True, False, True)
                '                TotalSalesGroup = New FlashGroup(MSP, Wk, Units, "Total Sales", False, 141, False, True, True, False, False, New List(Of FlashGroup) From {CafeSalesGroup, CateringSalesGroup, SalesTaxGroup})
                '                CogsGroup = New FlashGroup(MSP, Wk, Units, "COGS", True, 188, False, False, True, False, True) With {.SalesFlashGroup = TotalSalesGroup}
                '                LaborGroup = New FlashGroup(MSP, Wk, Units, "Labor", True, 235, True, False, True, False, True) With {.SalesFlashGroup = TotalSalesGroup}
                '                OpexGroup = New FlashGroup(MSP, Wk, Units, "OPEX", True, 282, False, False, True, False, True) With {.SalesFlashGroup = TotalSalesGroup}
                '                FeesGroup = New FlashGroup(MSP, Wk, Units, "Fees", True, 329, True, False, True, False, True) With {.SalesFlashGroup = TotalSalesGroup}
                '                SubsidyGroup = New FlashGroup(MSP, Wk, Units, "Subsidy", True, 376, True, True, True, False, False, New List(Of FlashGroup) From {TotalSalesGroup, CogsGroup, LaborGroup, OpexGroup, FeesGroup}) With {.SalesFlashGroup = TotalSalesGroup}

                '                With grdFlashGroups.Children
                '                    .Add(CafeSalesGroup)
                '                    .Add(CateringSalesGroup)
                '                    .Add(SalesTaxGroup)
                '                    .Add(TotalSalesGroup)
                '                    .Add(CogsGroup)
                '                    .Add(LaborGroup)
                '                    .Add(OpexGroup)
                '                    .Add(FeesGroup)
                '                    .Add(SubsidyGroup)
                '                End With
#End Region

#Region "Beverage"
                '            Case 5      ' Beverage Flash
                '                Title = "Beverage Weekly Financial Flash"
                '                Height = 510

                '                AvailableUnits = New UnitGroup With {.UnitGroupName = "BV"}

                '                '// Add Unit and/or Subunits
                '                Dim qsu = From su In AGNESShared.UnitsSubunits
                '                          Where su.UnitNumber = FU
                '                          Select su

                '                If qsu.Count > 0 Then
                '                    For Each su In qsu
                '                        Dim subunit As New UnitFlash With {.FlashType = AvailableUnits.UnitGroupName, .UnitNumber = su.SubUnitNumber}
                '                        AvailableUnits.UnitsInGroup.Add(subunit)
                '                        tlbUnits.Visibility = Visibility.Visible
                '                    Next
                '                Else
                '                    Dim FlashUnit As New UnitFlash With {.FlashType = AvailableUnits.UnitGroupName, .UnitNumber = FU}
                '                    AvailableUnits.UnitsInGroup.Add(FlashUnit)
                '                    tlbUnits.Visibility = Visibility.Hidden
                '                    grdColumnLabels.Margin = New Thickness(0, 42, 0, 0)
                '                    grdFlashGroups.Margin = New Thickness(0, 74, 0, 0)
                '                End If
                '                Units = New UnitChooser(AvailableUnits)
                '                If qsu.Count > 0 Then Units.AllowMultiSelect = True

                '                '// Add flash-specific flashgroups (categories)
                '                SalesGroup = New FlashGroup(MSP, Wk, Units, "Sales", False, 0, True, False, True, False, False) With {.SpreadByWeeks = True}
                '                SalesTaxGroup = New FlashGroup(MSP, Wk, Units, "Sales Tax", False, 47, False, False, True, False, False) With {.SpreadByWeeks = True}
                '                CogsGroup = New FlashGroup(MSP, Wk, Units, "COGS", True, 94, False, False, True, False, False) With {.SalesFlashGroup = TotalSalesGroup, .SpreadByWeeks = True}
                '                LaborGroup = New FlashGroup(MSP, Wk, Units, "Labor", True, 141, True, False, True, False, False) With {.SalesFlashGroup = TotalSalesGroup, .SpreadByWeeks = True}
                '                OpexGroup = New FlashGroup(MSP, Wk, Units, "OPEX", True, 188, False, False, True, False, False) With {.SalesFlashGroup = TotalSalesGroup, .SpreadByWeeks = True}
                '                FeesGroup = New FlashGroup(MSP, Wk, Units, "Fees", True, 235, True, False, True, False, False) With {.SalesFlashGroup = TotalSalesGroup, .SpreadByWeeks = True}
                '                SubsidyGroup = New FlashGroup(MSP, Wk, Units, "Subsidy", True, 282, True, True, True, False, False, New List(Of FlashGroup) From {SalesGroup, SalesTaxGroup, CogsGroup, LaborGroup, OpexGroup, FeesGroup}) With {.SalesFlashGroup = TotalSalesGroup}

                '                With grdFlashGroups.Children
                '                    .Add(SalesGroup)
                '                    .Add(SalesTaxGroup)
                '                    .Add(CogsGroup)
                '                    .Add(LaborGroup)
                '                    .Add(OpexGroup)
                '                    '.Add(DemoGroup)
                '                    .Add(FeesGroup)
                '                    .Add(SubsidyGroup)
                '                End With
#End Region

#Region "Catering"
            Case 6      ' Catering Flash
#End Region

#Region "Overhead"
            Case 7      ' Overhead Flash
                '                Title = "Overhead Weekly Financial Flash"
                '                Height = 369
                '                AvailableUnits = New UnitGroup With {.UnitGroupName = "OH"}

                '                '// Add Unit and/or Subunits
                '                Dim qsu = From su In AGNESShared.UnitsSubunits
                '                          Where su.UnitNumber = FU
                '                          Select su

                '                If qsu.Count > 0 Then
                '                    For Each su In qsu
                '                        Dim subunit As New UnitFlash With {.FlashType = AvailableUnits.UnitGroupName, .UnitNumber = su.SubUnitNumber}
                '                        AvailableUnits.UnitsInGroup.Add(subunit)
                '                        tlbUnits.Visibility = Visibility.Visible
                '                    Next
                '                Else
                '                    Dim FlashUnit As New UnitFlash With {.FlashType = AvailableUnits.UnitGroupName, .UnitNumber = FU}
                '                    AvailableUnits.UnitsInGroup.Add(FlashUnit)
                '                    tlbUnits.Visibility = Visibility.Hidden
                '                    grdColumnLabels.Margin = New Thickness(0, 42, 0, 0)
                '                    grdFlashGroups.Margin = New Thickness(0, 74, 0, 0)
                '                End If
                '                Units = New UnitChooser(AvailableUnits)
                '                If qsu.Count > 0 Then Units.AllowMultiSelect = True

                '                '// Add flash-specific flashgroups (categories)
                '                CogsGroup = New FlashGroup(MSP, Wk, Units, "COGS", False, 0, False, False, True, False, False) With {.SpreadByWeeks = True}
                '                LaborGroup = New FlashGroup(MSP, Wk, Units, "Labor", False, 47, True, False, True, False, False) With {.SpreadByWeeks = True}
                '                OpexGroup = New FlashGroup(MSP, Wk, Units, "OPEX", False, 94, False, False, True, False, False) With {.SpreadByWeeks = True}
                '                SubsidyGroup = New FlashGroup(MSP, Wk, Units, "Subsidy", True, 141, True, True, True, False, False, New List(Of FlashGroup) From {CogsGroup, LaborGroup, OpexGroup})
                '                With grdFlashGroups.Children
                '                    .Add(CogsGroup)
                '                    .Add(LaborGroup)
                '                    .Add(OpexGroup)
                '                    .Add(SubsidyGroup)
                '                End With
#End Region

#Region "Eventions"
            Case 8
                'Title = "Eventions Weekly Financial Flash"
                'Height = 369
                'AvailableUnits = New UnitGroup With {.UnitGroupName = "Eventions"}

                '// Add Unit And/Or Subunits
                'Dim qsu = From su In AGNESShared.UnitsSubunits
                '          Where su.UnitNumber = FU
                '          Select su

                'If qsu.Count > 0 Then
                '    For Each su In qsu
                '        Dim subunit As New UnitFlash With {.FlashType = AvailableUnits.UnitGroupName, .UnitNumber = su.SubUnitNumber}
                '        AvailableUnits.UnitsInGroup.Add(subunit)
                '        tlbUnits.Visibility = Visibility.Visible
                '    Next
                'Else
                '    Dim FlashUnit As New UnitFlash With {.FlashType = AvailableUnits.UnitGroupName, .UnitNumber = FU}
                '    AvailableUnits.UnitsInGroup.Add(FlashUnit)
                '    tlbUnits.Visibility = Visibility.Hidden
                '    grdColumnLabels.Margin = New Thickness(0, 42, 0, 0)
                '    grdFlashGroups.Margin = New Thickness(0, 74, 0, 0)
                'End If
                'Units = New UnitChooser(AvailableUnits)
                'If qsu.Count > 0 Then Units.AllowMultiSelect = True

                '// Add flash-specific flashgroups (categories)
                'SalesGroup = New FlashGroup(MSP, Wk, Units, "Total Sales", False, 0, True, False, True, False, False) With {.SpreadByWeeks = True}
                'LaborGroup = New FlashGroup(MSP, Wk, Units, "Labor", False, 47, False, False, True, False, False) With {.SpreadByWeeks = True}
                'OpexGroup = New FlashGroup(MSP, Wk, Units, "OPEX", False, 94, True, False, True, False, False) With {.SpreadByWeeks = True}
                'FeesGroup = New FlashGroup(MSP, Wk, Units, "Fees", False, 141, False, False, True, False, False) With {.SpreadByWeeks = True}
                'SubsidyGroup = New FlashGroup(MSP, Wk, Units, "Subsidy", True, 188, True, True, True, False, False, New List(Of FlashGroup) From {SalesGroup, LaborGroup, OpexGroup, FeesGroup})

                'With grdFlashGroups.Children
                '    .Add(SalesGroup)
                '    .Add(LaborGroup)
                '    .Add(OpexGroup)
                '    .Add(FeesGroup)
                '    .Add(SubsidyGroup)
                'End With
#End Region
        End Select

        '        For Each fg As FlashGroup In grdFlashGroups.Children
        '            fg.Load()
        '            If fg.GroupIsSubTotal = True Then fg.Update(fg)
        '        Next

#End Region

        Dim sep As New Separator
        With tlbFcast.Items
            .Add(MSP)
            .Add(sep)
        End With
        tlbUnits.Items.Add(Units)
        If Units.NumberOfAvailableUnits = 1 Then Units.IsEnabled = False

    End Sub
#End Region

End Class
