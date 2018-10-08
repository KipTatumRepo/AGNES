Imports System.ComponentModel

Public Class Flash
    'TODO:  PUSH FLASH/FORECAST UNLOCK FUNCTIONALITY TO DM FLASH STATUS UI, ALONG WITH ALERTS
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
        ConstructTemplate(FlashType, FlashUnit)
    End Sub

#Region "Toolbar Controls"
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

#End Region

#Region "Private Functions"
    Private Sub ConstructTemplate(FT, FU)
        grdFlashGroups.Children.Clear()
        '// Add period, week, and unit chooser controls 
        Dim currmsp As Byte = GetCurrentPeriod(FormatDateTime(Now(), DateFormat.ShortDate))
        Dim currwk As Byte = GetCurrentWeek(FormatDateTime(Now(), DateFormat.ShortDate))
        Wk = New WeekChooser(1, currwk, currwk)
        MSP = New PeriodChooser(Wk, 1, currmsp, currmsp)
        MSP.SelectAllEnabled = False

        Select Case FT
            Case 1      '   Commons Flash
                Title = "WCC Weekly Financial Flash - Unit " & FU
                Height = 369
                AvailableUnits = New UnitGroup With {.UnitGroupName = "WCC"}
                Dim FlashUnit As New UnitFlash With {.FlashType = "WCC", .UnitNumber = FU}
                AvailableUnits.UnitsInGroup.Add(FlashUnit)
                tlbUnits.Visibility = Visibility.Hidden
                Units = New UnitChooser(AvailableUnits)
                Units.AllowMultiSelect = True

                grdColumnLabels.Margin = New Thickness(0, 42, 0, 0)
                grdFlashGroups.Margin = New Thickness(0, 74, 0, 0)

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

            Case 2      ' Puget Sound Cafe Flash
                Title = "Cafe Weekly Financial Flash - Unit " & FU
                Height = 369
                AvailableUnits = New UnitGroup With {.UnitGroupName = "Cafes"}
                Dim FlashUnit As New UnitFlash With {.FlashType = "Cafes", .UnitNumber = FU}
                AvailableUnits.UnitsInGroup.Add(FlashUnit)
                tlbUnits.Visibility = Visibility.Hidden
                Units = New UnitChooser(AvailableUnits)
                Units.AllowMultiSelect = True

                grdColumnLabels.Margin = New Thickness(0, 42, 0, 0)
                grdFlashGroups.Margin = New Thickness(0, 74, 0, 0)

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

            Case 3      ' A/V Flash
                'TODO:  FIGURE OUT A BETTER WAY OF TYING THE OH UNITS TOGETHER VS. HARD CODING
                Title = "A/V Weekly Financial Flash"
                Height = 410
                AvailableUnits = New UnitGroup With {.UnitGroupName = "AV"}
                Dim AVUnit1 As New UnitFlash With {.FlashType = "AV", .UnitNumber = 30954}
                AvailableUnits.UnitsInGroup.Add(AVUnit1)
                Dim AVUnit2 As New UnitFlash With {.FlashType = "AV", .UnitNumber = 13797}
                AvailableUnits.UnitsInGroup.Add(AVUnit2)
                Dim AVUnit3 As New UnitFlash With {.FlashType = "AV", .UnitNumber = 13333}
                AvailableUnits.UnitsInGroup.Add(AVUnit3)
                Dim AVUnit4 As New UnitFlash With {.FlashType = "AV", .UnitNumber = 13331}
                AvailableUnits.UnitsInGroup.Add(AVUnit4)
                Dim AVUnit5 As New UnitFlash With {.FlashType = "AV", .UnitNumber = 13335}
                AvailableUnits.UnitsInGroup.Add(AVUnit5)
                Dim AVUnit6 As New UnitFlash With {.FlashType = "AV", .UnitNumber = 22443}
                AvailableUnits.UnitsInGroup.Add(AVUnit6)
                Dim AVUnit7 As New UnitFlash With {.FlashType = "AV", .UnitNumber = 23403}
                AvailableUnits.UnitsInGroup.Add(AVUnit7)
                Dim AVUnit8 As New UnitFlash With {.FlashType = "AV", .UnitNumber = 28503}
                AvailableUnits.UnitsInGroup.Add(AVUnit8)
                Dim AVUnit9 As New UnitFlash With {.FlashType = "AV", .UnitNumber = 32436}
                AvailableUnits.UnitsInGroup.Add(AVUnit9)
                Dim AVUnit10 As New UnitFlash With {.FlashType = "AV", .UnitNumber = 13332}
                AvailableUnits.UnitsInGroup.Add(AVUnit10)
                Dim AVUnit11 As New UnitFlash With {.FlashType = "AV", .UnitNumber = 30946}
                AvailableUnits.UnitsInGroup.Add(AVUnit11)
                Dim AVUnit12 As New UnitFlash With {.FlashType = "AV", .UnitNumber = 28505}
                AvailableUnits.UnitsInGroup.Add(AVUnit12)
                Dim AVUnit13 As New UnitFlash With {.FlashType = "AV", .UnitNumber = 13335}
                AvailableUnits.UnitsInGroup.Add(AVUnit13)

                tlbUnits.Visibility = Visibility.Visible
                Units = New UnitChooser(AvailableUnits)
                Units.AllowMultiSelect = True

                SalesGroup = New FlashGroup(MSP, Wk, Units, "Sales", False, 0, True, False, True, False, True)
                LaborGroup = New FlashGroup(MSP, Wk, Units, "Labor", False, 47, False, False, True, False, True)
                OpexGroup = New FlashGroup(MSP, Wk, Units, "OPEX", False, 94, True, False, True, False, True)
                FeesGroup = New FlashGroup(MSP, Wk, Units, "Fees", False, 141, False, False, True, False, True)
                SubsidyGroup = New FlashGroup(MSP, Wk, Units, "Subsidy", False, 188, True, True, True, False, False, New List(Of FlashGroup) From {SalesGroup, LaborGroup, OpexGroup, FeesGroup})
                With grdFlashGroups.Children
                    .Add(SalesGroup)
                    .Add(LaborGroup)
                    .Add(OpexGroup)
                    .Add(FeesGroup)
                    .Add(SubsidyGroup)
                End With

            Case 4      ' Field Site Flash
                Title = "Field Site Weekly Financial Flash"
                Height = 600

                AvailableUnits = New UnitGroup With {.UnitGroupName = "Cafes"}
                Dim FlashUnit As New UnitFlash With {.FlashType = "Cafes", .UnitNumber = FU}
                AvailableUnits.UnitsInGroup.Add(FlashUnit)
                'TODO:  FIGURE OUT A BETTER WAY OF TYING THE FIELD & CC/BEVERAGE UNITS TOGETHER VS. HARD CODING
                Select Case FU
                    Case 21716
                        Dim BevUnit As New UnitFlash With {.FlashType = "Cafes", .UnitNumber = 13067}
                        AvailableUnits.UnitsInGroup.Add(BevUnit)
                    Case 2612
                        Dim BevUnit As New UnitFlash With {.FlashType = "Cafes", .UnitNumber = 7150}
                        AvailableUnits.UnitsInGroup.Add(BevUnit)
                    Case 2618
                        Dim BevUnit As New UnitFlash With {.FlashType = "Cafes", .UnitNumber = 8656}
                        AvailableUnits.UnitsInGroup.Add(BevUnit)
                    Case 32444
                        Dim BevUnit As New UnitFlash With {.FlashType = "Cafes", .UnitNumber = 32445}
                        AvailableUnits.UnitsInGroup.Add(BevUnit)
                    Case 1808
                        Dim BevUnit As New UnitFlash With {.FlashType = "Cafes", .UnitNumber = 1869}
                        Dim CCUnit As New UnitFlash With {.FlashType = "Cafes", .UnitNumber = 3068}
                        AvailableUnits.UnitsInGroup.Add(BevUnit)
                        AvailableUnits.UnitsInGroup.Add(CCUnit)
                    Case 25653
                        Dim BevUnit As New UnitFlash With {.FlashType = "Cafes", .UnitNumber = 25655}
                        Dim CCUnit As New UnitFlash With {.FlashType = "Cafes", .UnitNumber = 25657}
                        AvailableUnits.UnitsInGroup.Add(BevUnit)
                        AvailableUnits.UnitsInGroup.Add(CCUnit)
                End Select

                tlbUnits.Visibility = Visibility.Visible
                Units = New UnitChooser(AvailableUnits)
                Units.AllowMultiSelect = True

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


            Case 5      ' Beverage Flash
                'TODO:  FIGURE OUT A BETTER WAY OF TYING THE OH UNITS TOGETHER VS. HARD CODING
            Case 6      ' Catering Flash
            Case 7      ' Overhead Flash
                'TODO:  FIGURE OUT A BETTER WAY OF TYING THE OH UNITS TOGETHER VS. HARD CODING
                Title = "Overhead Weekly Financial Flash"
                Height = 369
                AvailableUnits = New UnitGroup With {.UnitGroupName = "OH"}
                Dim OHUnit1 As New UnitFlash With {.FlashType = "OH", .UnitNumber = 1852}
                AvailableUnits.UnitsInGroup.Add(OHUnit1)
                Dim OHUnit2 As New UnitFlash With {.FlashType = "OH", .UnitNumber = 2734}
                AvailableUnits.UnitsInGroup.Add(OHUnit2)
                Dim OHUnit3 As New UnitFlash With {.FlashType = "OH", .UnitNumber = 31878}
                AvailableUnits.UnitsInGroup.Add(OHUnit3)
                Dim OHUnit4 As New UnitFlash With {.FlashType = "OH", .UnitNumber = 4713}
                AvailableUnits.UnitsInGroup.Add(OHUnit4)
                Dim OHUnit5 As New UnitFlash With {.FlashType = "OH", .UnitNumber = 6038}
                AvailableUnits.UnitsInGroup.Add(OHUnit5)
                Dim OHUnit6 As New UnitFlash With {.FlashType = "OH", .UnitNumber = 11414}
                AvailableUnits.UnitsInGroup.Add(OHUnit6)
                Dim OHUnit7 As New UnitFlash With {.FlashType = "OH", .UnitNumber = 11681}
                AvailableUnits.UnitsInGroup.Add(OHUnit7)
                Dim OHUnit8 As New UnitFlash With {.FlashType = "OH", .UnitNumber = 11682}
                AvailableUnits.UnitsInGroup.Add(OHUnit8)
                tlbUnits.Visibility = Visibility.Visible
                Units = New UnitChooser(AvailableUnits)
                Units.AllowMultiSelect = True

                CogsGroup = New FlashGroup(MSP, Wk, Units, "COGS", False, 0, False, False, True, False, True)
                LaborGroup = New FlashGroup(MSP, Wk, Units, "Labor", False, 47, True, False, True, False, True)
                OpexGroup = New FlashGroup(MSP, Wk, Units, "OPEX", False, 94, False, False, True, False, True)
                SubsidyGroup = New FlashGroup(MSP, Wk, Units, "Subsidy", True, 141, True, True, True, False, False, New List(Of FlashGroup) From {CogsGroup, LaborGroup, OpexGroup})
                With grdFlashGroups.Children
                    .Add(CogsGroup)
                    .Add(LaborGroup)
                    .Add(OpexGroup)
                    .Add(SubsidyGroup)
                End With


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

    Private Sub Flash_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        If SaveStatus = 0 Then
            Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Small, AgnesMessageBox.MsgBoxLayout.TopOnly, AgnesMessageBox.MsgBoxType.YesNo, 12, False, "Discard unsaved data?")
            amsg.ShowDialog()
            If amsg.ReturnResult = "No" Then e.Cancel = True
            amsg.Close()
        End If

    End Sub

#End Region

End Class
