Imports System.ComponentModel
'TODO: GLITCH WHEN OPENING DRAFT FLASH - UNABLE TO RESAVE (STATUSSAVE?)  ISSUE WAS WITH FARGO P5W1
'TODO: BUG ON MULTIPLE UNIT FLASHES - WHEN FLIPPING TO NEW UNIT WITH UNSAVED DATA, OPTING TO **NOT** DISCARD THE INFORMATION
'      STILL DISCARDS IT
Public Class Flash

#Region "Properties"
    Dim SalesGroup As FlashGroup
    Dim CamGroup As FlashGroup
    Dim CafeSalesGroup As FlashGroup
    Dim SalesTaxGroup As FlashGroup
    Dim CateringSalesGroup As FlashGroup
    Dim TotalSalesGroup As FlashGroup
    Dim CogsGroup As FlashGroup
    Dim LaborGroup As FlashGroup
    Dim OpexGroup As FlashGroup
    Dim FeesGroup As FlashGroup
    Dim SubsidyGroup As FlashGroup
    Dim TotalGroup As FlashGroup
    Dim Units As UnitChooser
    Public Property TypeOfFlash As Byte
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
                    imgEscalate.Visibility = Visibility.Visible
                Case 1      '   Draft
                    tbSaveStatus.Text = "Draft saved"
                    barSaveStatus.Background = Brushes.Yellow
                    imgEscalate.Visibility = Visibility.Visible
                Case 2      '   Saved
                    tbSaveStatus.Text = "Flash saved"
                    barSaveStatus.Background = Brushes.LightGreen
                    imgEscalate.Visibility = Visibility.Visible
                Case 3      '   Final
                    tbSaveStatus.Text = "Flash Locked"
                    barSaveStatus.Background = Brushes.LightGreen
                    imgEscalate.Visibility = Visibility.Collapsed
            End Select
        End Set
    End Property

#End Region

#Region "Constructor"

    Public Sub New(FlashType, FlashUnit)
        InitializeComponent()
        TypeOfFlash = FlashType
        ConstructTemplate(FlashType, FlashUnit)
    End Sub

#End Region

#Region "Public Methods"
    Public Sub ToggleAlert(onoff)
        If onoff = 0 Then
            With imgEscalate
                .Tag = "On"
                .Source = New BitmapImage(New Uri("/AGNES;component/Resources/HandWaveOn.png", UriKind.Relative))
                .ToolTip = "Deactivate alert to DM"
            End With
            AddAlertMsg()
        Else
            With imgEscalate
                .Tag = "Off"
                .Source = New BitmapImage(New Uri("/AGNES;component/Resources/HandWave.png", UriKind.Relative))
                .ToolTip = "Call out Flash to DM"
            End With
            DeleteAlertMsg()
        End If
    End Sub

#End Region

#Region "Private Methods"
    Private Sub ConstructTemplate(FT As Byte, FU As Long)
        grdFlashGroups.Children.Clear()
        '// Add period, week, and unit chooser controls 
        Dim currmsp As Byte = GetCurrentPeriod(FormatDateTime(Now(), DateFormat.ShortDate))
        Dim currwk As Byte = GetCurrentWeek(FormatDateTime(Now(), DateFormat.ShortDate))
        Wk = New WeekChooser(1, currwk, currwk)
        MSP = New PeriodChooser(Wk, 1, currmsp, currmsp)
        MSP.DisableSelectAll = False

        Select Case FT
#Region "Commons"
            Case 1      '   Commons Flash
                Title = "WCC Weekly Financial Flash - Unit " & FU
                Height = 369
                AvailableUnits = New UnitGroup With {.Summoner = 0, .UnitGroupName = "WCC"}

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
                    grdFlashGroups.Margin = New Thickness(0, 74, 0, 0)
                End If
                Units = New UnitChooser(AvailableUnits)
                If qsu.Count > 0 Then Units.AllowMultiSelect = True

                '// Add flash-specific flashgroups (categories)
                CamGroup = New FlashGroup(MSP, Wk, Units, "CAM Revenue", False, 0, True, False, True, True, False) ' Increments of 47 for flashgroup spacing 
                CogsGroup = New FlashGroup(MSP, Wk, Units, "COGS", True, 47, False, False, True, False, True) With {.SalesFlashGroup = CamGroup}
                LaborGroup = New FlashGroup(MSP, Wk, Units, "Labor", True, 94, True, False, True, False, True) With {.SalesFlashGroup = CamGroup}
                OpexGroup = New FlashGroup(MSP, Wk, Units, "OPEX", True, 141, False, False, True, False, True) With {.SalesFlashGroup = CamGroup}
                SubsidyGroup = New FlashGroup(MSP, Wk, Units, "Subsidy", True, 188, True, True, True, False, False, New List(Of FlashGroup) From {CamGroup, CogsGroup, LaborGroup, OpexGroup}) With {.SalesFlashGroup = CamGroup}
                With grdFlashGroups.Children
                    .Add(CamGroup)
                    .Add(CogsGroup)
                    .Add(LaborGroup)
                    .Add(OpexGroup)
                    .Add(SubsidyGroup)
                End With
#End Region

#Region "Cafes"
            Case 2      ' Puget Sound Cafe Flash
                Title = "Cafe Weekly Financial Flash - Unit " & FU
                Height = 369
                AvailableUnits = New UnitGroup With {.Summoner = 0, .UnitGroupName = "Cafes"}

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
                    grdFlashGroups.Margin = New Thickness(0, 74, 0, 0)
                End If
                Units = New UnitChooser(AvailableUnits)
                If qsu.Count > 0 Then Units.AllowMultiSelect = True

                '// Add flash-specific flashgroups (categories)
                SalesGroup = New FlashGroup(MSP, Wk, Units, "Sales", False, 0, True, False, True, True, False) ' Increments of 47 for flashgroup spacing
                CogsGroup = New FlashGroup(MSP, Wk, Units, "COGS", True, 47, False, False, True, False, True) With {.SalesFlashGroup = SalesGroup}
                LaborGroup = New FlashGroup(MSP, Wk, Units, "Labor", True, 94, True, False, True, False, True) With {.SalesFlashGroup = SalesGroup}
                OpexGroup = New FlashGroup(MSP, Wk, Units, "OPEX", True, 141, False, False, True, False, True) With {.SalesFlashGroup = SalesGroup}
                SubsidyGroup = New FlashGroup(MSP, Wk, Units, "Subsidy", True, 188, True, True, True, False, False, New List(Of FlashGroup) From {SalesGroup, CogsGroup, LaborGroup, OpexGroup}) With {.SalesFlashGroup = SalesGroup}
                With grdFlashGroups.Children
                    .Add(SalesGroup)
                    .Add(CogsGroup)
                    .Add(LaborGroup)
                    .Add(OpexGroup)
                    .Add(SubsidyGroup)
                End With
#End Region

#Region "A/V"
            Case 3      ' A/V Flash
                Title = "A/V Weekly Financial Flash"
                Height = 460
                AvailableUnits = New UnitGroup With {.Summoner = 0, .UnitGroupName = "AV"}

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
                    grdFlashGroups.Margin = New Thickness(0, 74, 0, 0)
                End If
                Units = New UnitChooser(AvailableUnits)
                If qsu.Count > 0 Then Units.AllowMultiSelect = True

                '// Add flash-specific flashgroups (categories)
                SalesGroup = New FlashGroup(MSP, Wk, Units, "Sales", False, 0, True, False, True, False, False) With {.SpreadByWeeks = True}
                SalesTaxGroup = New FlashGroup(MSP, Wk, Units, "Sales Tax", False, 47, False, False, True, False, False) With {.SpreadByWeeks = True}

                LaborGroup = New FlashGroup(MSP, Wk, Units, "Labor", False, 94, True, False, True, False, False) With {.SpreadByWeeks = True}
                OpexGroup = New FlashGroup(MSP, Wk, Units, "OPEX", False, 141, False, False, True, False, False) With {.SpreadByWeeks = True}
                FeesGroup = New FlashGroup(MSP, Wk, Units, "Fees", False, 188, True, False, True, False, False) With {.SpreadByWeeks = True}
                SubsidyGroup = New FlashGroup(MSP, Wk, Units, "Subsidy", False, 188 + 47, True, True, True, False, False, New List(Of FlashGroup) From {SalesGroup, SalesTaxGroup, LaborGroup, OpexGroup, FeesGroup})
                With grdFlashGroups.Children
                    .Add(SalesGroup)
                    .Add(SalesTaxGroup)
                    .Add(LaborGroup)
                    .Add(OpexGroup)
                    .Add(FeesGroup)
                    .Add(SubsidyGroup)
                End With

#End Region

#Region "Field Sites"
            Case 4      ' Field Site Flash
                Title = "Field Site Weekly Financial Flash"
                Height = 600
                AvailableUnits = New UnitGroup With {.Summoner = 0, .UnitGroupName = "Cafes"}

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
                    grdFlashGroups.Margin = New Thickness(0, 74, 0, 0)
                End If
                Units = New UnitChooser(AvailableUnits)
                If qsu.Count > 0 Then Units.AllowMultiSelect = True

                '// Add flash-specific flashgroups (categories)
                CafeSalesGroup = New FlashGroup(MSP, Wk, Units, "Sales", False, 0, True, False, True, True, False)
                CateringSalesGroup = New FlashGroup(MSP, Wk, Units, "Catering Sales", False, 47, False, False, True, True, False)
                SalesTaxGroup = New FlashGroup(MSP, Wk, Units, "Sales Tax", False, 94, True, False, True, False, True)
                TotalSalesGroup = New FlashGroup(MSP, Wk, Units, "Total Sales", False, 141, False, True, True, False, False, New List(Of FlashGroup) From {CafeSalesGroup, CateringSalesGroup, SalesTaxGroup})
                CogsGroup = New FlashGroup(MSP, Wk, Units, "COGS", True, 188, False, False, True, False, True) With {.SalesFlashGroup = TotalSalesGroup}
                LaborGroup = New FlashGroup(MSP, Wk, Units, "Labor", True, 235, True, False, True, False, True) With {.SalesFlashGroup = TotalSalesGroup}
                OpexGroup = New FlashGroup(MSP, Wk, Units, "OPEX", True, 282, False, False, True, False, True) With {.SalesFlashGroup = TotalSalesGroup}
                FeesGroup = New FlashGroup(MSP, Wk, Units, "Fees", True, 329, True, False, True, False, True) With {.SalesFlashGroup = TotalSalesGroup}
                SubsidyGroup = New FlashGroup(MSP, Wk, Units, "Subsidy", True, 376, True, True, True, False, False, New List(Of FlashGroup) From {TotalSalesGroup, CogsGroup, LaborGroup, OpexGroup, FeesGroup}) With {.SalesFlashGroup = TotalSalesGroup}

                With grdFlashGroups.Children
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
                Title = "Beverage Weekly Financial Flash"
                Height = 510

                AvailableUnits = New UnitGroup With {.Summoner = 0, .UnitGroupName = "BV"}

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
                    grdFlashGroups.Margin = New Thickness(0, 74, 0, 0)
                End If
                Units = New UnitChooser(AvailableUnits)
                If qsu.Count > 0 Then Units.AllowMultiSelect = True

                '// Add flash-specific flashgroups (categories)
                SalesGroup = New FlashGroup(MSP, Wk, Units, "Sales", False, 0, True, False, True, False, False) With {.SpreadByWeeks = True}
                SalesTaxGroup = New FlashGroup(MSP, Wk, Units, "Sales Tax", False, 47, False, False, True, False, False) With {.SpreadByWeeks = True}
                CogsGroup = New FlashGroup(MSP, Wk, Units, "COGS", True, 94, False, False, True, False, False) With {.SalesFlashGroup = TotalSalesGroup, .SpreadByWeeks = True}
                LaborGroup = New FlashGroup(MSP, Wk, Units, "Labor", True, 141, True, False, True, False, False) With {.SalesFlashGroup = TotalSalesGroup, .SpreadByWeeks = True}
                OpexGroup = New FlashGroup(MSP, Wk, Units, "OPEX", True, 188, False, False, True, False, False) With {.SalesFlashGroup = TotalSalesGroup, .SpreadByWeeks = True}
                FeesGroup = New FlashGroup(MSP, Wk, Units, "Fees", True, 235, True, False, True, False, False) With {.SalesFlashGroup = TotalSalesGroup, .SpreadByWeeks = True}
                SubsidyGroup = New FlashGroup(MSP, Wk, Units, "Subsidy", True, 282, True, True, True, False, False, New List(Of FlashGroup) From {SalesGroup, SalesTaxGroup, CogsGroup, LaborGroup, OpexGroup, FeesGroup}) With {.SalesFlashGroup = TotalSalesGroup}

                With grdFlashGroups.Children
                    .Add(SalesGroup)
                    .Add(SalesTaxGroup)
                    .Add(CogsGroup)
                    .Add(LaborGroup)
                    .Add(OpexGroup)
                    '.Add(DemoGroup)
                    .Add(FeesGroup)
                    .Add(SubsidyGroup)
                End With
#End Region

#Region "Catering"
            Case 6      ' Catering Flash
#End Region

#Region "Overhead"
            Case 7      ' Overhead Flash
                Title = "Overhead Weekly Financial Flash"
                Height = 369
                AvailableUnits = New UnitGroup With {.Summoner = 0, .UnitGroupName = "OH"}

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
                    grdFlashGroups.Margin = New Thickness(0, 74, 0, 0)
                End If
                Units = New UnitChooser(AvailableUnits)
                If qsu.Count > 0 Then Units.AllowMultiSelect = True

                '// Add flash-specific flashgroups (categories)
                CogsGroup = New FlashGroup(MSP, Wk, Units, "COGS", False, 0, False, False, True, False, False) With {.SpreadByWeeks = True}
                LaborGroup = New FlashGroup(MSP, Wk, Units, "Labor", False, 47, True, False, True, False, False) With {.SpreadByWeeks = True}
                OpexGroup = New FlashGroup(MSP, Wk, Units, "OPEX", False, 94, False, False, True, False, False) With {.SpreadByWeeks = True}
                SubsidyGroup = New FlashGroup(MSP, Wk, Units, "Subsidy", True, 141, True, True, True, False, False, New List(Of FlashGroup) From {CogsGroup, LaborGroup, OpexGroup})
                With grdFlashGroups.Children
                    .Add(CogsGroup)
                    .Add(LaborGroup)
                    .Add(OpexGroup)
                    .Add(SubsidyGroup)
                End With
#End Region

#Region "Eventions"
            Case 8
                Title = "Eventions Weekly Financial Flash"
                Height = 369
                AvailableUnits = New UnitGroup With {.Summoner = 0, .UnitGroupName = "Eventions"}

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
                    grdFlashGroups.Margin = New Thickness(0, 74, 0, 0)
                End If
                Units = New UnitChooser(AvailableUnits)
                If qsu.Count > 0 Then Units.AllowMultiSelect = True

                '// Add flash-specific flashgroups (categories)
                SalesGroup = New FlashGroup(MSP, Wk, Units, "Total Sales", False, 0, True, False, True, False, False) With {.SpreadByWeeks = True}
                LaborGroup = New FlashGroup(MSP, Wk, Units, "Labor", False, 47, False, False, True, False, False) With {.SpreadByWeeks = True}
                OpexGroup = New FlashGroup(MSP, Wk, Units, "OPEX", False, 94, True, False, True, False, False) With {.SpreadByWeeks = True}
                FeesGroup = New FlashGroup(MSP, Wk, Units, "Fees", False, 141, False, False, True, False, False) With {.SpreadByWeeks = True}
                SubsidyGroup = New FlashGroup(MSP, Wk, Units, "Subsidy", True, 188, True, True, True, False, False, New List(Of FlashGroup) From {SalesGroup, LaborGroup, OpexGroup, FeesGroup})

                With grdFlashGroups.Children
                    .Add(SalesGroup)
                    .Add(LaborGroup)
                    .Add(OpexGroup)
                    .Add(FeesGroup)
                    .Add(SubsidyGroup)
                End With
#End Region
        End Select

        For Each fg As FlashGroup In grdFlashGroups.Children
            fg.Load()
            If fg.GroupIsSubTotal = True Then fg.Update(fg)
        Next

        Dim sep As New Separator
        With tlbFlash.Items
            .Add(MSP)
            .Add(sep)
            .Add(Wk)
        End With
        tlbUnits.Items.Add(Units)
        If Units.NumberOfAvailableUnits = 1 Then Units.IsEnabled = False

    End Sub

#Region "Toolbar Methods"
    Private Sub SaveDraft(sender As Object, e As MouseButtonEventArgs) Handles imgDraft.MouseLeftButtonDown
        If SaveStatus > 0 Then Exit Sub

        For Each fg As FlashGroup In grdFlashGroups.Children
            If fg.GroupIsSubTotal = False Then
                If fg.Save("Draft") = False Then
                    SaveStatus = 0
                    Exit Sub
                End If
            End If
        Next
        SaveStatus = 1
    End Sub

    Private Sub ToggleAlertButton(sender As Object, e As MouseButtonEventArgs) Handles imgEscalate.MouseLeftButtonDown
        If imgEscalate.Tag = "On" Then
            ToggleAlert(1)
        Else
            ToggleAlert(0)
        End If
        SaveStatus = 0
    End Sub

    Private Sub SaveFinal(sender As Object, e As MouseButtonEventArgs) Handles imgSave.MouseLeftButtonDown
        If SaveStatus > 1 Then Exit Sub
        For Each fg As FlashGroup In grdFlashGroups.Children
            If fg.GroupIsSubTotal = False Then
                If fg.Save("Final") = False Then
                    SaveStatus = 0
                    Exit Sub
                End If
            End If
        Next
        SaveStatus = 3
    End Sub

    Private Sub PrintFlash(sender As Object, e As MouseButtonEventArgs) Handles imgPrint.MouseLeftButtonDown
        PrintAnyObject(grdMain, "Flash")
    End Sub

    Private Sub OpenDelegatesUI(sender As Object, e As MouseButtonEventArgs) Handles imgDelegates.MouseLeftButtonDown
        Dim DelUi As New Delegates(Units.CurrentUnit)
        DelUi.ShowDialog()
    End Sub

#End Region

    Private Sub AddAlertMsg()
        If AlertOverride = True Then Exit Sub
        Dim alertmsg As String = "", sui As New SingleUserInput With {.InputType = 0}
        sui.lblInputDirection.Text = "Please enter a short (64 characters or less) message to accompany this alert."
        sui.ShowDialog()
        alertmsg = sui.StringVal
        sui.Close()

        Dim nfa As New FlashAlert
        With nfa
            .MSFY = CurrentFiscalYear
            .MSP = MSP.CurrentPeriod
            .Week = Wk.CurrentWeek
            .UnitNumber = Units.CurrentUnit
            .AlertNote = alertmsg
            .SavedBy = My.Settings.UserShortName
        End With
        With FlashActuals
            .FlashAlerts.Add(nfa)
            .SaveChanges()
        End With
    End Sub

    Private Sub DeleteAlertMsg()
        Dim qfa = From fai In FlashActuals.FlashAlerts
                  Select fai
                  Where fai.UnitNumber = Units.CurrentUnit And
                      fai.MSFY = CurrentFiscalYear And
                      fai.MSP = MSP.CurrentPeriod And
                      fai.Week = Wk.CurrentWeek

        For Each fai In qfa
            FlashActuals.FlashAlerts.Remove(fai)
        Next
        FlashActuals.SaveChanges()
    End Sub

    Private Sub Flash_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        If SaveStatus = 0 Then
            Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Small, AgnesMessageBox.MsgBoxLayout.TextAndImage, AgnesMessageBox.MsgBoxType.YesNo, 12, False,, "Discard unsaved data?",, AgnesMessageBox.ImageType.Danger)
            amsg.ShowDialog()
            If amsg.ReturnResult = "No" Then e.Cancel = True
            amsg.Close()
        End If

    End Sub

#End Region

End Class
