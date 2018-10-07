Imports System.ComponentModel

Public Class Flash
    'TODO:  PUSH FLASH/FORECAST UNLOCK FUNCTIONALITY TO DM FLASH STATUS UI, ALONG WITH ALERTS
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

            Case 4      ' Field Site Flash
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

            Case 5      ' Beverage Flash
            Case 6      ' Catering Flash
            Case 7      ' Overhead Flash


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
