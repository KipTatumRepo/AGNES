﻿Imports System.ComponentModel
Public Class Forecast

#Region "Properties"
    Dim SalesGroup As ForecastGroup
    Dim CamGroup As ForecastGroup
    Dim CafeSalesGroup As ForecastGroup
    Dim SalesTaxGroup As ForecastGroup
    Dim CateringSalesGroup As ForecastGroup
    Dim TotalSalesGroup As ForecastGroup
    Dim CogsGroup As ForecastGroup
    Dim LaborGroup As ForecastGroup
    Dim OpexGroup As ForecastGroup
    Dim FeesGroup As ForecastGroup
    Dim SubsidyGroup As ForecastGroup
    Dim TotalGroup As ForecastGroup
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
                Case 1      '   Saved
                    tbSaveStatus.Text = "Forecast saved"
                    barSaveStatus.Background = Brushes.LightGreen
                    imgClear.Visibility = Visibility.Visible
                    imgApplyDrr.Visibility = Visibility.Visible
                    imgRefDrr.Visibility = Visibility.Visible
            End Select
        End Set
    End Property

#End Region

#Region "Constructor"
    Public Sub New(FcastType, FCastUnit)
        InitializeComponent()
        TypeOfFcast = FcastType
        ConstructTemplate(FcastType, FCastUnit)
        Dim adminunlock As Byte = My.Settings.UserLevel
        If adminunlock < 3 Then
            imgUnlock.Visibility = Visibility.Visible
            sepUnlock.Visibility = Visibility.Visible
        Else
            imgUnlock.Visibility = Visibility.Collapsed
            sepUnlock.Visibility = Visibility.Hidden
        End If
    End Sub

#End Region

#Region "Public Methods"

#End Region

#Region "Private Methods"
    Private Sub ConstructTemplate(FT As Byte, FU As Long)

        'TODO: FINISH TEMPLATING CATERING, AND EVENTIONS

        grdFcastGroups.Children.Clear()
        '// Add period, week, and unit chooser controls 
        Dim currmsp As Byte = GetCurrentPeriod(FormatDateTime(Now(), DateFormat.ShortDate))
        Dim currwk As Byte = GetCurrentWeek(FormatDateTime(Now(), DateFormat.ShortDate))
        Wk = New WeekChooser(1, currwk, currwk)
        MSP = New PeriodChooser(Wk, currmsp, 12, currmsp)
        MSP.DisableSelectAll = True

        Select Case FT
#Region "Commons"
            Case 1      '   Commons Forecast
                Title = "WCC Period Financial Forecast"
                Height = 369
                AvailableUnits = New UnitGroup With {.Summoner = 1, .UnitGroupName = "WCC"}

                '// Add Unit and/or Subunits
                Dim qsu = From su In AGNESShared.UnitsSubunits
                          Where su.UnitNumber = FU
                          Select su

                If qsu.Count > 0 Then
                    For Each su In qsu
                        Dim subunit As New UnitFcast With {.FcastType = AvailableUnits.UnitGroupName, .UnitNumber = su.SubUnitNumber}
                        AvailableUnits.FcastUnitsInGroup.Add(subunit)
                        tlbUnits.Visibility = Visibility.Visible
                    Next
                Else
                    Dim FCastUnit As New UnitFcast With {.FcastType = AvailableUnits.UnitGroupName, .UnitNumber = FU}
                    AvailableUnits.FcastUnitsInGroup.Add(FCastUnit)
                    tlbUnits.Visibility = Visibility.Hidden
                    grdColumnLabels.Margin = New Thickness(0, 42, 0, 0)
                    grdFcastGroups.Margin = New Thickness(0, 74, 0, 0)
                End If
                Units = New UnitChooser(AvailableUnits)
                If qsu.Count > 0 Then Units.AllowMultiSelect = True

                '// Add forecast groups (categories)
                CamGroup = New ForecastGroup(PC:=MSP, UC:=Units, GroupName:="CAM Revenue", ShowPercentages:=False, Top:=0, Highlight:=True,
                                             Subtotal:=False, CreditOnly:=True, DebitOnly:=False) ' Increments of 47 for flashgroup spacing 
                CogsGroup = New ForecastGroup(PC:=MSP, UC:=Units, GroupName:="COGS", ShowPercentages:=True, Top:=47, Highlight:=False,
                                             Subtotal:=False, CreditOnly:=False, DebitOnly:=True) With {.SalesFcastGroup = CamGroup}
                LaborGroup = New ForecastGroup(PC:=MSP, UC:=Units, GroupName:="Labor", ShowPercentages:=True, Top:=94, Highlight:=True,
                                             Subtotal:=False, CreditOnly:=False, DebitOnly:=True) With {.SalesFcastGroup = CamGroup}
                OpexGroup = New ForecastGroup(PC:=MSP, UC:=Units, GroupName:="OPEX", ShowPercentages:=True, Top:=141, Highlight:=False,
                                             Subtotal:=False, CreditOnly:=False, DebitOnly:=True) With {.SalesFcastGroup = CamGroup}
                SubsidyGroup = New ForecastGroup(PC:=MSP, UC:=Units, GroupName:="Subsidy", ShowPercentages:=True, Top:=188, Highlight:=True,
                                             Subtotal:=True, CreditOnly:=False, DebitOnly:=False, SubtotalGroupList:=New List(Of ForecastGroup) From
                                             {CamGroup, CogsGroup, LaborGroup, OpexGroup}) With {.SalesFcastGroup = CamGroup}

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
                AvailableUnits = New UnitGroup With {.Summoner = 1, .UnitGroupName = "Cafes"}

                '// Add Unit and/or Subunits
                Dim qsu = From su In AGNESShared.UnitsSubunits
                          Where su.UnitNumber = FU
                          Select su

                If qsu.Count > 0 Then
                    For Each su In qsu
                        Dim subunit As New UnitFcast With {.FcastType = AvailableUnits.UnitGroupName, .UnitNumber = su.SubUnitNumber}
                        AvailableUnits.FcastUnitsInGroup.Add(subunit)
                        tlbUnits.Visibility = Visibility.Visible
                    Next
                Else
                    Dim FCastUnit As New UnitFcast With {.FcastType = AvailableUnits.UnitGroupName, .UnitNumber = FU}
                    AvailableUnits.FcastUnitsInGroup.Add(FCastUnit)
                    tlbUnits.Visibility = Visibility.Hidden
                    grdColumnLabels.Margin = New Thickness(0, 42, 0, 0)
                    grdFcastGroups.Margin = New Thickness(0, 74, 0, 0)
                End If
                Units = New UnitChooser(AvailableUnits)
                If qsu.Count > 0 Then Units.AllowMultiSelect = True

                '// Add forecast groups (categories)
                SalesGroup = New ForecastGroup(PC:=MSP, UC:=Units, GroupName:="Sales", ShowPercentages:=False, Top:=0, Highlight:=True,
                                             Subtotal:=False, CreditOnly:=True, DebitOnly:=False) ' Increments of 47 for flashgroup spacing 
                CogsGroup = New ForecastGroup(PC:=MSP, UC:=Units, GroupName:="COGS", ShowPercentages:=True, Top:=47, Highlight:=False,
                                             Subtotal:=False, CreditOnly:=False, DebitOnly:=True) With {.SalesFcastGroup = SalesGroup}
                LaborGroup = New ForecastGroup(PC:=MSP, UC:=Units, GroupName:="Labor", ShowPercentages:=True, Top:=94, Highlight:=True,
                                             Subtotal:=False, CreditOnly:=False, DebitOnly:=True) With {.SalesFcastGroup = SalesGroup}
                OpexGroup = New ForecastGroup(PC:=MSP, UC:=Units, GroupName:="OPEX", ShowPercentages:=True, Top:=141, Highlight:=False,
                                             Subtotal:=False, CreditOnly:=False, DebitOnly:=True) With {.SalesFcastGroup = SalesGroup}
                SubsidyGroup = New ForecastGroup(PC:=MSP, UC:=Units, GroupName:="Subsidy", ShowPercentages:=True, Top:=188, Highlight:=True,
                                             Subtotal:=True, CreditOnly:=False, DebitOnly:=False, SubtotalGroupList:=New List(Of ForecastGroup) From
                                             {SalesGroup, CogsGroup, LaborGroup, OpexGroup}) With {.SalesFcastGroup = SalesGroup}

                With grdFcastGroups.Children
                    .Add(SalesGroup)
                    .Add(CogsGroup)
                    .Add(LaborGroup)
                    .Add(OpexGroup)
                    .Add(SubsidyGroup)
                End With
#End Region

#Region "A/V"
            Case 3      ' A/V Forecast
                Title = "A/V Period Financial Forecast"
                Height = 510
                AvailableUnits = New UnitGroup With {.Summoner = 1, .UnitGroupName = "AV"}

                '// Add Unit and/or Subunits
                Dim qsu = From su In AGNESShared.UnitsSubunits
                          Where su.UnitNumber = FU
                          Select su

                If qsu.Count > 0 Then
                    For Each su In qsu
                        Dim subunit As New UnitFcast With {.FcastType = AvailableUnits.UnitGroupName, .UnitNumber = su.SubUnitNumber}
                        AvailableUnits.FcastUnitsInGroup.Add(subunit)
                        tlbUnits.Visibility = Visibility.Visible
                    Next
                Else
                    Dim FCastUnit As New UnitFcast With {.FcastType = AvailableUnits.UnitGroupName, .UnitNumber = FU}
                    AvailableUnits.FcastUnitsInGroup.Add(FCastUnit)
                    tlbUnits.Visibility = Visibility.Hidden
                    grdColumnLabels.Margin = New Thickness(0, 42, 0, 0)
                    grdFcastGroups.Margin = New Thickness(0, 74, 0, 0)
                End If
                Units = New UnitChooser(AvailableUnits)
                If qsu.Count > 0 Then Units.AllowMultiSelect = True

                '// Add forecast groups (categories)
                SalesGroup = New ForecastGroup(PC:=MSP, UC:=Units, GroupName:="Sales", ShowPercentages:=False, Top:=0, Highlight:=True,
                                             Subtotal:=False, CreditOnly:=True, DebitOnly:=False) ' Increments of 47 for flashgroup spacing 
                SalesTaxGroup = New ForecastGroup(PC:=MSP, UC:=Units, GroupName:="Sales Tax", ShowPercentages:=False, Top:=47, Highlight:=False,
                                             Subtotal:=False, CreditOnly:=False, DebitOnly:=True)
                TotalSalesGroup = New ForecastGroup(PC:=MSP, UC:=Units, GroupName:="Total Sales", ShowPercentages:=False, Top:=94, Highlight:=True,
                                             Subtotal:=True, CreditOnly:=False, DebitOnly:=False, SubtotalGroupList:=New List(Of ForecastGroup) From
                                             {SalesGroup, SalesTaxGroup})
                LaborGroup = New ForecastGroup(PC:=MSP, UC:=Units, GroupName:="Labor", ShowPercentages:=True, Top:=141, Highlight:=True,
                                             Subtotal:=False, CreditOnly:=False, DebitOnly:=True) With {.SalesFcastGroup = TotalSalesGroup}
                OpexGroup = New ForecastGroup(PC:=MSP, UC:=Units, GroupName:="OPEX", ShowPercentages:=True, Top:=188, Highlight:=False,
                                             Subtotal:=False, CreditOnly:=False, DebitOnly:=True) With {.SalesFcastGroup = TotalSalesGroup}
                FeesGroup = New ForecastGroup(PC:=MSP, UC:=Units, GroupName:="Fees", ShowPercentages:=True, Top:=235, Highlight:=True,
                                             Subtotal:=False, CreditOnly:=False, DebitOnly:=True) With {.SalesFcastGroup = TotalSalesGroup}
                SubsidyGroup = New ForecastGroup(PC:=MSP, UC:=Units, GroupName:="Subsidy", ShowPercentages:=True, Top:=282, Highlight:=True,
                                             Subtotal:=True, CreditOnly:=False, DebitOnly:=False, SubtotalGroupList:=New List(Of ForecastGroup) From
                                             {TotalSalesGroup, LaborGroup, FeesGroup, OpexGroup}) With {.SalesFcastGroup = TotalSalesGroup}
                With grdFcastGroups.Children
                    .Add(SalesGroup)
                    .Add(SalesTaxGroup)
                    .Add(TotalSalesGroup)
                    .Add(LaborGroup)
                    .Add(OpexGroup)
                    .Add(FeesGroup)
                    .Add(SubsidyGroup)
                End With

#End Region

#Region "Field Sites"
            Case 4      ' Field Site Flash
                Title = "Field Period Financial Forecast"
                Height = 600
                AvailableUnits = New UnitGroup With {.Summoner = 1, .UnitGroupName = "Field"}

                '// Add Unit and/or Subunits
                Dim qsu = From su In AGNESShared.UnitsSubunits
                          Where su.UnitNumber = FU
                          Select su

                If qsu.Count > 0 Then
                    For Each su In qsu
                        Dim subunit As New UnitFcast With {.FcastType = AvailableUnits.UnitGroupName, .UnitNumber = su.SubUnitNumber}
                        AvailableUnits.FcastUnitsInGroup.Add(subunit)
                        tlbUnits.Visibility = Visibility.Visible
                    Next
                Else
                    Dim FCastUnit As New UnitFcast With {.FcastType = AvailableUnits.UnitGroupName, .UnitNumber = FU}
                    AvailableUnits.FcastUnitsInGroup.Add(FCastUnit)
                    tlbUnits.Visibility = Visibility.Hidden
                    grdColumnLabels.Margin = New Thickness(0, 42, 0, 0)
                    grdFcastGroups.Margin = New Thickness(0, 74, 0, 0)
                End If
                Units = New UnitChooser(AvailableUnits)
                If qsu.Count > 0 Then Units.AllowMultiSelect = True

                '// Add forecast groups (categories)
                CafeSalesGroup = New ForecastGroup(PC:=MSP, UC:=Units, GroupName:="Cafe Sales", ShowPercentages:=False, Top:=0, Highlight:=True,
                                             Subtotal:=False, CreditOnly:=True, DebitOnly:=False) ' Increments of 47 for flashgroup spacing 
                CateringSalesGroup = New ForecastGroup(PC:=MSP, UC:=Units, GroupName:="Catering Sales", ShowPercentages:=False, Top:=47, Highlight:=False,
                                             Subtotal:=False, CreditOnly:=True, DebitOnly:=False) ' Increments of 47 for flashgroup spacing 
                SalesTaxGroup = New ForecastGroup(PC:=MSP, UC:=Units, GroupName:="Sales Tax", ShowPercentages:=False, Top:=94, Highlight:=True,
                                             Subtotal:=False, CreditOnly:=False, DebitOnly:=True) With {.SalesFcastGroup = SalesGroup}
                TotalSalesGroup = New ForecastGroup(PC:=MSP, UC:=Units, GroupName:="Total Sales", ShowPercentages:=False, Top:=141, Highlight:=True,
                                             Subtotal:=True, CreditOnly:=False, DebitOnly:=False, SubtotalGroupList:=New List(Of ForecastGroup) From
                                             {CafeSalesGroup, CateringSalesGroup, SalesTaxGroup})
                CogsGroup = New ForecastGroup(PC:=MSP, UC:=Units, GroupName:="COGS", ShowPercentages:=True, Top:=188, Highlight:=False,
                                             Subtotal:=False, CreditOnly:=False, DebitOnly:=True) With {.SalesFcastGroup = TotalSalesGroup}
                LaborGroup = New ForecastGroup(PC:=MSP, UC:=Units, GroupName:="Labor", ShowPercentages:=True, Top:=235, Highlight:=True,
                                             Subtotal:=False, CreditOnly:=False, DebitOnly:=True) With {.SalesFcastGroup = TotalSalesGroup}
                OpexGroup = New ForecastGroup(PC:=MSP, UC:=Units, GroupName:="OPEX", ShowPercentages:=True, Top:=282, Highlight:=False,
                                             Subtotal:=False, CreditOnly:=False, DebitOnly:=True) With {.SalesFcastGroup = TotalSalesGroup}
                FeesGroup = New ForecastGroup(PC:=MSP, UC:=Units, GroupName:="Fees", ShowPercentages:=True, Top:=329, Highlight:=True,
                                             Subtotal:=False, CreditOnly:=False, DebitOnly:=True) With {.SalesFcastGroup = TotalSalesGroup}
                SubsidyGroup = New ForecastGroup(PC:=MSP, UC:=Units, GroupName:="Subsidy", ShowPercentages:=True, Top:=376, Highlight:=True,
                                             Subtotal:=True, CreditOnly:=False, DebitOnly:=False, SubtotalGroupList:=New List(Of ForecastGroup) From
                                             {TotalSalesGroup, CogsGroup, LaborGroup, FeesGroup, OpexGroup}) With {.SalesFcastGroup = TotalSalesGroup}
                With grdFcastGroups.Children
                    .Add(CafeSalesGroup)
                    .Add(CateringSalesGroup)
                    .Add(SalesTaxGroup)
                    .Add(TotalSalesGroup)
                    .Add(CogsGroup)
                    .Add(LaborGroup)
                    .Add(OpexGroup)
                    .Add(FeesGroup)
                    .Add(SubsidyGroup)
                End With
#End Region

#Region "Beverage"
            Case 5      ' Beverage Flash
                Title = "Beverage, Vending, and Markets Period Financial Forecast"
                Height = 560
                AvailableUnits = New UnitGroup With {.Summoner = 1, .UnitGroupName = "Field"}

                '// Add Unit and/or Subunits
                Dim qsu = From su In AGNESShared.UnitsSubunits
                          Where su.UnitNumber = FU
                          Select su

                If qsu.Count > 0 Then
                    For Each su In qsu
                        Dim subunit As New UnitFcast With {.FcastType = AvailableUnits.UnitGroupName, .UnitNumber = su.SubUnitNumber}
                        AvailableUnits.FcastUnitsInGroup.Add(subunit)
                        tlbUnits.Visibility = Visibility.Visible
                    Next
                Else
                    Dim FCastUnit As New UnitFcast With {.FcastType = AvailableUnits.UnitGroupName, .UnitNumber = FU}
                    AvailableUnits.FcastUnitsInGroup.Add(FCastUnit)
                    tlbUnits.Visibility = Visibility.Hidden
                    grdColumnLabels.Margin = New Thickness(0, 42, 0, 0)
                    grdFcastGroups.Margin = New Thickness(0, 74, 0, 0)
                End If
                Units = New UnitChooser(AvailableUnits)
                If qsu.Count > 0 Then Units.AllowMultiSelect = True

                '// Add forecast groups (categories)

                SalesGroup = New ForecastGroup(PC:=MSP, UC:=Units, GroupName:="Sales", ShowPercentages:=False, Top:=0, Highlight:=True,
                                             Subtotal:=False, CreditOnly:=False, DebitOnly:=False) ' Increments of 47 for flashgroup spacing 
                SalesTaxGroup = New ForecastGroup(PC:=MSP, UC:=Units, GroupName:="Sales Tax", ShowPercentages:=False, Top:=47, Highlight:=False,
                                             Subtotal:=False, CreditOnly:=False, DebitOnly:=False)
                TotalSalesGroup = New ForecastGroup(PC:=MSP, UC:=Units, GroupName:="Total Sales", ShowPercentages:=False, Top:=94, Highlight:=True,
                                             Subtotal:=True, CreditOnly:=False, DebitOnly:=False, SubtotalGroupList:=New List(Of ForecastGroup) From
                                             {SalesGroup, SalesTaxGroup})
                CogsGroup = New ForecastGroup(PC:=MSP, UC:=Units, GroupName:="COGS", ShowPercentages:=True, Top:=141, Highlight:=False,
                                             Subtotal:=False, CreditOnly:=False, DebitOnly:=False) With {.SalesFcastGroup = TotalSalesGroup}
                LaborGroup = New ForecastGroup(PC:=MSP, UC:=Units, GroupName:="Labor", ShowPercentages:=True, Top:=188, Highlight:=True,
                                             Subtotal:=False, CreditOnly:=False, DebitOnly:=False) With {.SalesFcastGroup = TotalSalesGroup}
                OpexGroup = New ForecastGroup(PC:=MSP, UC:=Units, GroupName:="OPEX", ShowPercentages:=True, Top:=235, Highlight:=False,
                                             Subtotal:=False, CreditOnly:=False, DebitOnly:=False) With {.SalesFcastGroup = TotalSalesGroup}
                FeesGroup = New ForecastGroup(PC:=MSP, UC:=Units, GroupName:="Fees", ShowPercentages:=True, Top:=282, Highlight:=True,
                                             Subtotal:=False, CreditOnly:=False, DebitOnly:=False) With {.SalesFcastGroup = TotalSalesGroup}
                SubsidyGroup = New ForecastGroup(PC:=MSP, UC:=Units, GroupName:="Subsidy", ShowPercentages:=True, Top:=329, Highlight:=True,
                                             Subtotal:=True, CreditOnly:=False, DebitOnly:=False, SubtotalGroupList:=New List(Of ForecastGroup) From
                                             {TotalSalesGroup, CogsGroup, LaborGroup, FeesGroup, OpexGroup}) With {.SalesFcastGroup = TotalSalesGroup}
                With grdFcastGroups.Children
                    .Add(SalesGroup)
                    .Add(SalesTaxGroup)
                    .Add(TotalSalesGroup)
                    .Add(CogsGroup)
                    .Add(LaborGroup)
                    .Add(OpexGroup)
                    .Add(FeesGroup)
                    .Add(SubsidyGroup)
                End With
#End Region

#Region "Catering"
            Case 6      ' Catering Flash
#End Region

#Region "Overhead"
            Case 7      ' Overhead Flash
                Title = "Overhead Period Financial Forecast"
                Height = 369
                AvailableUnits = New UnitGroup With {.Summoner = 1, .UnitGroupName = "Field"}

                '// Add Unit and/or Subunits
                Dim qsu = From su In AGNESShared.UnitsSubunits
                          Where su.UnitNumber = FU
                          Select su

                If qsu.Count > 0 Then
                    For Each su In qsu
                        Dim subunit As New UnitFcast With {.FcastType = AvailableUnits.UnitGroupName, .UnitNumber = su.SubUnitNumber}
                        AvailableUnits.FcastUnitsInGroup.Add(subunit)
                        tlbUnits.Visibility = Visibility.Visible
                    Next
                Else
                    Dim FCastUnit As New UnitFcast With {.FcastType = AvailableUnits.UnitGroupName, .UnitNumber = FU}
                    AvailableUnits.FcastUnitsInGroup.Add(FCastUnit)
                    tlbUnits.Visibility = Visibility.Hidden
                    grdColumnLabels.Margin = New Thickness(0, 42, 0, 0)
                    grdFcastGroups.Margin = New Thickness(0, 74, 0, 0)
                End If
                Units = New UnitChooser(AvailableUnits)
                If qsu.Count > 0 Then Units.AllowMultiSelect = True

                '// Add forecast groups (categories)
                CogsGroup = New ForecastGroup(PC:=MSP, UC:=Units, GroupName:="COGS", ShowPercentages:=False, Top:=0, Highlight:=False,
                                             Subtotal:=False, CreditOnly:=False, DebitOnly:=False)
                LaborGroup = New ForecastGroup(PC:=MSP, UC:=Units, GroupName:="Labor", ShowPercentages:=False, Top:=47, Highlight:=True,
                                             Subtotal:=False, CreditOnly:=False, DebitOnly:=False) With {.SalesFcastGroup = CamGroup}
                OpexGroup = New ForecastGroup(PC:=MSP, UC:=Units, GroupName:="OPEX", ShowPercentages:=False, Top:=94, Highlight:=False,
                                             Subtotal:=False, CreditOnly:=False, DebitOnly:=False) With {.SalesFcastGroup = CamGroup}
                SubsidyGroup = New ForecastGroup(PC:=MSP, UC:=Units, GroupName:="Subsidy", ShowPercentages:=False, Top:=141, Highlight:=True,
                                             Subtotal:=True, CreditOnly:=False, DebitOnly:=False, SubtotalGroupList:=New List(Of ForecastGroup) From
                                             {CogsGroup, LaborGroup, OpexGroup})
                With grdFcastGroups.Children
                    .Add(CogsGroup)
                    .Add(LaborGroup)
                    .Add(OpexGroup)
                    .Add(SubsidyGroup)
                End With
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

        For Each fg As ForecastGroup In grdFcastGroups.Children
            fg.Load()
            If fg.GroupIsSubTotal = True Then fg.Update(fg)
        Next

        tlbFcast.Items.Add(MSP)
        tlbUnits.Items.Add(Units)
        If Units.NumberOfAvailableUnits = 1 Then Units.IsEnabled = False

    End Sub

    Private Sub Forecast_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        If SaveStatus = 0 Then
            Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Small, AgnesMessageBox.MsgBoxLayout.TextAndImage, AgnesMessageBox.MsgBoxType.YesNo, 12, False,, "Discard unsaved data?",, AgnesMessageBox.ImageType.Danger)
            amsg.ShowDialog()
            If amsg.ReturnResult = "No" Then e.Cancel = True
            amsg.Close()
        End If
    End Sub

#Region "Toolbar"
    Private Sub SaveForecast(sender As Object, e As MouseButtonEventArgs) Handles imgSave.MouseLeftButtonDown
        If SaveStatus = 1 Then Exit Sub
        For Each fg As ForecastGroup In grdFcastGroups.Children
            If fg.GroupIsSubTotal = False Then
                If fg.Save("Final") = False Then
                    SaveStatus = 0
                    Exit Sub
                End If
            End If
        Next
        SaveStatus = 1
    End Sub

    Private Sub PrintForecast(sender As Object, e As MouseButtonEventArgs) Handles imgPrint.MouseLeftButtonDown
        PrintAnyObject(grdMain, "Flash")
    End Sub

    Private Sub ClearForecast(sender As Object, e As MouseButtonEventArgs) Handles imgClear.MouseLeftButtonDown
        For Each fg As ForecastGroup In grdFcastGroups.Children
            If fg.GroupIsSubTotal = False Then fg.ClearForecast()
            fg.Update(fg)
        Next
        SaveStatus = 0
    End Sub

    Private Sub RefreshRunRate(sender As Object, e As MouseButtonEventArgs) Handles imgRefDrr.MouseLeftButtonDown
        For Each fg As ForecastGroup In grdFcastGroups.Children
            If fg.GroupIsSubTotal = False Then fg.CalculateRunRate()
            fg.Update(fg)
        Next
    End Sub

    Private Sub ApplyRunRate(sender As Object, e As MouseButtonEventArgs) Handles imgApplyDrr.MouseLeftButtonDown
        For Each fg As ForecastGroup In grdFcastGroups.Children
            If fg.GroupIsSubTotal = False Then fg.ApplyRunRate()
            fg.Update(fg)
        Next
        SaveStatus = 0
    End Sub

    Private Sub RecordStaffingShorts(sender As Object, e As MouseButtonEventArgs) Handles imgStaffing.MouseLeftButtonDown
        Dim staff As New StaffCalendar(CurrentFiscalYear, MSP.CurrentPeriod, Units.CurrentUnit) With {.Title = "Staffing Shortages - Unit " & Units.CurrentUnit & " - Period " & MSP.CurrentPeriod}
        staff.ShowDialog()
    End Sub

    Private Sub UnlockWeeks(sender As Object, e As MouseButtonEventArgs) Handles imgUnlock.MouseLeftButtonDown
        For Each fg As ForecastGroup In grdFcastGroups.Children
            If fg.GroupIsSubTotal = False Then fg.Unlock()
        Next
    End Sub

#End Region

#End Region

End Class
