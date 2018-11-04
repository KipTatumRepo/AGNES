Public Class ForecastGroup
    Inherits DockPanel

#Region "Properties"
    Public GroupCategory As String
    Public SalesFcastGroup As ForecastGroup
    Public DRR As CurrencyBox
    Public W1Val As CurrencyBox
    Public W2Val As CurrencyBox
    Public W3Val As CurrencyBox
    Public W4Val As CurrencyBox
    Public W5Val As CurrencyBox
    Public PeriodTotalVal As CurrencyBox
    Public TotalPercent As PercentBox
    Public BudgetVal As CurrencyBox
    Public BudgetPercent As PercentBox
    Public VarianceVal As CurrencyBox

    Public PeriodChooseObject As PeriodChooser
    Public UnitChooseObject As UnitChooser

    Private _subtotal As Boolean
    Private Property _largepercentage As Boolean
    Public Property LargePercentage As Boolean
        Get
            Return _largepercentage
        End Get
        Set(value As Boolean)
            _largepercentage = value
            If value = True Then
                TotalPercent.FontSize = 8
                BudgetPercent.FontSize = 8
            Else
                TotalPercent.FontSize = 10
                BudgetPercent.FontSize = 10
            End If
        End Set
    End Property
    Private _budgetcontent As Double
    Public Property BudgetContent As Double
        Get
            Return _budgetcontent
        End Get
        Set(value As Double)
            _budgetcontent = value
            BudgetVal.SetAmount = value
            Update(Me)
        End Set
    End Property

    Public Property GroupIsSubTotal As Boolean
        Get
            Return _subtotal
        End Get
        Set(value As Boolean)
            _subtotal = value
            If value = True Then Background = Brushes.LightGray
        End Set
    End Property
    Public Property SubtotalGroups As New List(Of ForecastGroup)
    Private Property GroupHasPercentages As Boolean

#End Region

#Region "Constructor"
    Public Sub New(PC As PeriodChooser, UC As UnitChooser, GroupName As String, ShowPercentages As Boolean, Top As Integer, Highlight As Boolean, Subtotal As Boolean, CreditOnly As Boolean, DebitOnly As Boolean, Optional SubtotalGroupList As List(Of ForecastGroup) = Nothing)
        GroupCategory = GroupName
        GroupHasPercentages = ShowPercentages
        HorizontalAlignment = HorizontalAlignment.Left
        VerticalAlignment = VerticalAlignment.Top
        Height = 42
        Width = 970
        LastChildFill = False
        Margin = New Thickness(10, Top, 0, 0)
        If Highlight = True Then Background = Brushes.WhiteSmoke
        GroupIsSubTotal = Subtotal
        '// Create Flash group header label
        Dim GroupLabel As New Border
        Dim tb As New TextBlock With {.Text = GroupName, .Width = 80, .LineHeight = 16, .TextAlignment = TextAlignment.Center,
            .Margin = New Thickness(0, -2, 0, 0), .VerticalAlignment = VerticalAlignment.Center,
            .FontSize = 12, .FontWeight = FontWeights.SemiBold, .TextWrapping = TextWrapping.Wrap}
        GroupLabel.Child = tb
        '// Create daily run rate field
        DRR = New CurrencyBox(80, True, AgnesBaseInput.FontSz.Smaller,, CreditOnly, DebitOnly) With
            {.Margin = New Thickness(8, 6, 0, 0)}

        '// Create Week value input fields
        W1Val = New CurrencyBox(80, True, AgnesBaseInput.FontSz.Smaller,, CreditOnly, DebitOnly) With
            {.Margin = New Thickness(8, 6, 0, 0)}
        W2Val = New CurrencyBox(80, True, AgnesBaseInput.FontSz.Smaller,, CreditOnly, DebitOnly) With
            {.Margin = New Thickness(6, 6, 0, 0)}
        W3Val = New CurrencyBox(80, True, AgnesBaseInput.FontSz.Smaller,, CreditOnly, DebitOnly) With
            {.Margin = New Thickness(6, 6, 0, 0)}
        W4Val = New CurrencyBox(80, True, AgnesBaseInput.FontSz.Smaller,, CreditOnly, DebitOnly) With
            {.Margin = New Thickness(6, 6, 0, 0)}
        W5Val = New CurrencyBox(80, True, AgnesBaseInput.FontSz.Smaller,, CreditOnly, DebitOnly) With
            {.Margin = New Thickness(6, 6, 0, 0)}

        '// Create calculated fields (total, budget, variance, and percentages)

        PeriodTotalVal = New CurrencyBox(80, True, AgnesBaseInput.FontSz.Smaller,, CreditOnly, DebitOnly) With
            {.Margin = New Thickness(6, 6, 0, 0), .IsEnabled = False}

        TotalPercent = New PercentBox(60, True, AgnesBaseInput.FontSz.Smaller, 1) With
            {.VerticalAlignment = VerticalAlignment.Center, .Margin = New Thickness(1, 0, 0, 0), .IsEnabled = False}

        BudgetVal = New CurrencyBox(80, True, AgnesBaseInput.FontSz.Smaller,, CreditOnly, DebitOnly) With
            {.Margin = New Thickness(1, 6, 0, 0), .IsEnabled = False}

        BudgetPercent = New PercentBox(60, True, AgnesBaseInput.FontSz.Smaller, 1) With
            {.VerticalAlignment = VerticalAlignment.Center, .Margin = New Thickness(1, 0, 0, 0), .IsEnabled = False}

        VarianceVal = New CurrencyBox(80, True, AgnesBaseInput.FontSz.Smaller,, False, False) With
            {.Margin = New Thickness(1, 6, 0, 0), .IsEnabled = False}



        If GroupHasPercentages = False Then
            TotalPercent.Visibility = Visibility.Hidden
            BudgetPercent.Visibility = Visibility.Hidden
        End If

        If GroupIsSubTotal = True Then IsEnabled = False

        With Children
            .Add(GroupLabel)
            .Add(DRR)
            .Add(W1Val)
            .Add(W2Val)
            .Add(W3Val)
            .Add(W4Val)
            .Add(W5Val)
            .Add(PeriodTotalVal)
            .Add(TotalPercent)
            .Add(BudgetVal)
            .Add(BudgetPercent)
            .Add(VarianceVal)
        End With

        If GroupIsSubTotal = True Then
            For Each fg As ForecastGroup In SubtotalGroupList
                SubtotalGroups.Add(fg)
            Next
        End If

        PeriodChooseObject = PC
        AddHandler PeriodChooseObject.PropertyChanged, AddressOf PeriodChanged

        UnitChooseObject = UC
        AddHandler UnitChooseObject.PropertyChanged, AddressOf UnitChanged

        AddHandler DRR.PropertyChanged, AddressOf ForecastChanged
        AddHandler W1Val.PropertyChanged, AddressOf ForecastChanged
        AddHandler W2Val.PropertyChanged, AddressOf ForecastChanged
        AddHandler W3Val.PropertyChanged, AddressOf ForecastChanged
        AddHandler W4Val.PropertyChanged, AddressOf ForecastChanged
        AddHandler W5Val.PropertyChanged, AddressOf ForecastChanged

    End Sub

#End Region

#Region "Public Methods"
    Public Sub Load()
        CheckWeekFive()
        LockPreviousWeeks()
        LoadForecast()
        LoadPeriodBudget()
        CalculateRunRate()
    End Sub

    Public Sub Update(TargetFcastGroup As ForecastGroup)

        '//     Recalculate subtotals, if applicable
        If TargetFcastGroup.GroupIsSubTotal = True Then
            Dim DRRsub As Double, W1sub As Double, W2sub As Double, W3sub As Double, W4sub As Double, W5sub As Double, budgetsub As Double
            For Each fg As ForecastGroup In TargetFcastGroup.SubtotalGroups
                DRRsub += fg.DRR.SetAmount
                W1sub += fg.W1Val.SetAmount
                W2sub += fg.W2Val.SetAmount
                W3sub += fg.W3Val.SetAmount
                W4sub += fg.W4Val.SetAmount
                W5sub += fg.W5Val.SetAmount
                budgetsub += fg.BudgetVal.SetAmount
            Next
            TargetFcastGroup.DRR.SetAmount = DRRsub
            TargetFcastGroup.W1Val.SetAmount = W1sub
            TargetFcastGroup.W2Val.SetAmount = W2sub
            TargetFcastGroup.W3Val.SetAmount = W3sub
            TargetFcastGroup.W4Val.SetAmount = W4sub
            TargetFcastGroup.W5Val.SetAmount = W5sub
            Dim targetperiodtotal As Double = 0
            targetperiodtotal += TargetFcastGroup.W1Val.SetAmount
            targetperiodtotal += TargetFcastGroup.W2Val.SetAmount
            targetperiodtotal += TargetFcastGroup.W3Val.SetAmount
            targetperiodtotal += TargetFcastGroup.W4Val.SetAmount
            targetperiodtotal += TargetFcastGroup.W5Val.SetAmount
            TargetFcastGroup.PeriodTotalVal.SetAmount = targetperiodtotal
            TargetFcastGroup.BudgetVal.SetAmount = budgetsub
            TargetFcastGroup.IsEnabled = False
        End If

        '// Recalculate period total
        Dim periodtotal As Double = 0
        periodtotal += W1Val.SetAmount
        periodtotal += W2Val.SetAmount
        periodtotal += W3Val.SetAmount
        periodtotal += W4Val.SetAmount
        periodtotal += W5Val.SetAmount
        PeriodTotalVal.SetAmount = periodtotal

        '//     Recalculate variance
        TargetFcastGroup.VarianceVal.SetAmount = (TargetFcastGroup.BudgetVal.SetAmount - TargetFcastGroup.PeriodTotalVal.SetAmount)

        '//     Recalculate percentages, if applicable
        If TargetFcastGroup.GroupHasPercentages = True Then
            Dim fcastperc As Double, budgetperc As Double, salesamount As Double
            '//     Populate the forecast percentage
            Try
                salesamount = Math.Abs(TargetFcastGroup.SalesFcastGroup.PeriodTotalVal.SetAmount)
                If salesamount <> 0 Then
                    fcastperc = (TargetFcastGroup.PeriodTotalVal.SetAmount / salesamount)
                Else
                    fcastperc = 0
                End If
            Catch ex As Exception
                fcastperc = 0
            End Try

            TargetFcastGroup.TotalPercent.SetAmount = fcastperc

            '//     Populate the budget percent
            Try
                salesamount = Math.Abs(TargetFcastGroup.SalesFcastGroup.BudgetVal.SetAmount)
                If salesamount <> 0 Then
                    budgetperc = (TargetFcastGroup.BudgetVal.SetAmount / salesamount)
                Else
                    fcastperc = 0
                End If

            Catch ex As Exception
                budgetperc = 0
            End Try
            TargetFcastGroup.BudgetPercent.SetAmount = budgetperc

            '//     Reduce font size if percentage Is 10000% Or greater
            If (fcastperc >= 10) Or (budgetperc >= 10) Then
                TargetFcastGroup.LargePercentage = True
            Else
                TargetFcastGroup.LargePercentage = False
            End If
        End If

    End Sub

    Public Sub CalculateRunRate()
        Dim fetchweek As Byte, fetchperiod As Byte = PeriodChooseObject.CurrentPeriod, fetchyear As Integer = CurrentFiscalYear,
            pod As Byte, w As Byte, sumval As Double
        ' Determine current open week
        '   If it's the first week, set fetchperiod to the previous period (if it's P1, set fetchyear to the previous year and 
        '   period to 12).  Set weeks to all.
        '   If not, then set fetchweek to the previous week
        If GetCurrentWeek(FormatDateTime(Now(), DateFormat.ShortDate)) > 1 Then
            fetchweek = GetCurrentWeek(FormatDateTime(Now(), DateFormat.ShortDate)) - 1
        Else
            If PeriodChooseObject.CurrentPeriod = 1 Then
                fetchperiod = 12
                fetchyear = CurrentFiscalYear - 1
            Else
                fetchperiod -= 1
                If getperiodoperatingdays(fetchyear, fetchperiod) < 21 Then
                    fetchweek = 4
                Else
                    fetchweek = 5
                End If
            End If
        End If

        ' Fetch # of operating days preceding current week, or total in previous period
        For w = 1 To fetchweek
            pod += getweekoperatingdays(fetchyear, fetchperiod, w)
        Next

        ' Fetch and sum all previous flashes in the fetchperiod up to the fetchweek
        Dim qpf = From pfa In FlashActuals.FlashActualData
                  Where pfa.MSFY = fetchyear And
                      pfa.MSP = fetchperiod And
                      pfa.Week <= fetchweek And
                      pfa.UnitNumber = UnitChooseObject.CurrentUnit And
                      pfa.GLCategory = GroupCategory

        For Each pfa In qpf
            sumval += pfa.FlashValue
        Next

        ' Divide the sum by the number of operating days
        DRR.SetAmount = sumval / pod
    End Sub

    Public Sub ClearForecast()
        If W1Val.IsEnabled Then W1Val.SetAmount = 0
        If W2Val.IsEnabled Then W2Val.SetAmount = 0
        If W3Val.IsEnabled Then W3Val.SetAmount = 0
        If W4Val.IsEnabled Then W4Val.SetAmount = 0
        If W5Val.IsEnabled And W5Val.Visibility = Visibility.Visible Then W5Val.SetAmount = 0
    End Sub

    Public Sub ApplyRunRate()
        If W1Val.IsEnabled = True Then W1Val.SetAmount = (getweekoperatingdays(CurrentFiscalYear, PeriodChooseObject.CurrentPeriod, 1) * DRR.SetAmount)
        If W2Val.IsEnabled = True Then W2Val.SetAmount = (getweekoperatingdays(CurrentFiscalYear, PeriodChooseObject.CurrentPeriod, 2) * DRR.SetAmount)
        If W3Val.IsEnabled = True Then W3Val.SetAmount = (getweekoperatingdays(CurrentFiscalYear, PeriodChooseObject.CurrentPeriod, 3) * DRR.SetAmount)
        If W4Val.IsEnabled = True Then W4Val.SetAmount = (getweekoperatingdays(CurrentFiscalYear, PeriodChooseObject.CurrentPeriod, 4) * DRR.SetAmount)
        If (W5Val.IsEnabled = True And W5Val.Visibility = Visibility.Visible) Then W5Val.SetAmount = (getweekoperatingdays(CurrentFiscalYear, PeriodChooseObject.CurrentPeriod, 5) * DRR.SetAmount)
    End Sub

    Public Sub Unlock()
        W1Val.IsEnabled = True
        W2Val.IsEnabled = True
        W3Val.IsEnabled = True
        W4Val.IsEnabled = True
        If W5Val.Visibility = Visibility.Visible Then W5Val.IsEnabled = True
    End Sub

    Public Function Save(SaveType) As Boolean
        Dim SaveOkay As Boolean
        '// SaveType only influences the value saved to the status field; the status bar is updated via the Fcastpage that calls this routine

        '// Check to see if multiple units are selected. If so, kill routine 
        If CheckIfMultipleAreSelected() > 2 Then
            Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Small, AgnesMessageBox.MsgBoxLayout.FullText, AgnesMessageBox.MsgBoxType.OkOnly, 8, True, "Cannot save with multiple units selected")
            amsg.ShowDialog()
            Return False
            Exit Function
        End If

        '// Perform check to see if the record already exists; this defines which save function (new or update) is called
        Dim ff = From f In FlashForecasts.Forecasts
                 Where f.MSFY = CurrentFiscalYear And
                     f.MSP = PeriodChooseObject.CurrentPeriod And
                     f.UnitNumber = UnitChooseObject.CurrentUnit And
                     f.GLCategory = GroupCategory
        Try
            If ff.Count = 0 Then
                If W1Val.IsEnabled = True Then SaveOkay = SaveNewForecast(SaveType, W1Val, 1)
                If W2Val.IsEnabled = True Then SaveOkay = SaveNewForecast(SaveType, W2Val, 2)
                If W3Val.IsEnabled = True Then SaveOkay = SaveNewForecast(SaveType, W3Val, 3)
                If W4Val.IsEnabled = True Then SaveOkay = SaveNewForecast(SaveType, W4Val, 4)
                If W5Val.IsEnabled = True And W4Val.Visibility = Visibility.Visible Then SaveOkay = SaveNewForecast(SaveType, W5Val, 5)

            Else
                If W1Val.IsEnabled = True Then SaveOkay = UpdateForecast(SaveType, W1Val, 1)
                If W2Val.IsEnabled = True Then SaveOkay = UpdateForecast(SaveType, W2Val, 2)
                If W3Val.IsEnabled = True Then SaveOkay = UpdateForecast(SaveType, W3Val, 3)
                If W4Val.IsEnabled = True Then SaveOkay = UpdateForecast(SaveType, W4Val, 4)
                If W5Val.IsEnabled = True And W4Val.Visibility = Visibility.Visible Then SaveOkay = UpdateForecast(SaveType, W5Val, 5)
            End If
        Catch ex As Exception
            Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Small, AgnesMessageBox.MsgBoxLayout.FullText, AgnesMessageBox.MsgBoxType.OkOnly, 8, True, "Unable to save!",, "Error: " & ex.Message)
            amsg.ShowDialog()
            Return False
            Exit Function
        End Try
        Return True
    End Function

#End Region

#Region "Private Methods"
    Private Sub LoadPeriodBudget()
        Dim unitbrd As Border, unittb As TextBlock
        Dim CalculateBudget As Double = 0, UnitCount As Byte
        If GroupIsSubTotal = True Then Exit Sub
        For Each unitbrd In UnitChooseObject.Children
            If unitbrd.Tag <> "Label" Then
                unittb = unitbrd.Child
                If unittb.FontWeight = FontWeights.SemiBold Then
                    UnitCount += 1
                    CalculateBudget += LoadSingleUnitBudget(GroupCategory, FormatNumber(unittb.Tag, 0), CurrentFiscalYear, PeriodChooseObject.CurrentPeriod)
                End If
            End If
        Next
        BudgetContent = CalculateBudget
    End Sub

    Private Sub LoadForecast()
        W1Val.SetAmount = LoadSingleWeekAndUnitForecast(GroupCategory, UnitChooseObject.CurrentUnit, CurrentFiscalYear, PeriodChooseObject.CurrentPeriod, 1)
        W2Val.SetAmount = LoadSingleWeekAndUnitForecast(GroupCategory, UnitChooseObject.CurrentUnit, CurrentFiscalYear, PeriodChooseObject.CurrentPeriod, 2)
        W3Val.SetAmount = LoadSingleWeekAndUnitForecast(GroupCategory, UnitChooseObject.CurrentUnit, CurrentFiscalYear, PeriodChooseObject.CurrentPeriod, 3)
        W4Val.SetAmount = LoadSingleWeekAndUnitForecast(GroupCategory, UnitChooseObject.CurrentUnit, CurrentFiscalYear, PeriodChooseObject.CurrentPeriod, 4)
        If W5Val.Visibility = Visibility.Visible Then W5Val.SetAmount = LoadSingleWeekAndUnitForecast(GroupCategory, UnitChooseObject.CurrentUnit, CurrentFiscalYear, PeriodChooseObject.CurrentPeriod, 5)
    End Sub

    Private Sub UpdateSubtotals()
        Dim grd As Grid = Parent
        For Each fg As ForecastGroup In grd.Children
            If fg.GroupIsSubTotal = True Then Update(fg)
        Next
    End Sub

    Private Function CheckIfMultipleAreSelected() As Byte
        Dim unitbrd As Border, periodbrd As Border, unittb As TextBlock, periodtb As TextBlock, InternalCounter As Byte = 0
        For Each unitbrd In UnitChooseObject.Children
            If unitbrd.Tag <> "Label" Then
                unittb = unitbrd.Child
                If unittb.FontWeight = FontWeights.SemiBold Then InternalCounter += 1
            End If
        Next

        For Each periodbrd In PeriodChooseObject.Children
            If periodbrd.Tag <> "Label" Then
                periodtb = periodbrd.Child
                If periodtb.FontWeight = FontWeights.SemiBold Then InternalCounter += 1
            End If
        Next

        Return InternalCounter
    End Function

    Private Function SaveNewForecast(SaveType As String, WkObj As CurrencyBox, wk As Byte) As Boolean
        Try
            Dim nf As New Forecasts
            With nf
                .MSFY = CurrentFiscalYear
                .MSP = PeriodChooseObject.CurrentPeriod
                .Week = wk
                .UnitNumber = UnitChooseObject.CurrentUnit
                .GL = 0
                .GLCategory = GroupCategory
                .ForecastValue = WkObj.SetAmount
            End With
            FlashForecasts.Forecasts.Add(nf)
            FlashForecasts.SaveChanges()

            '// Get PID for audit trail
            Dim ff = From f In FlashForecasts.Forecasts
                     Where f.MSFY = CurrentFiscalYear And
                     f.MSP = PeriodChooseObject.CurrentPeriod And
                     f.Week = wk And
                     f.UnitNumber = UnitChooseObject.CurrentUnit And
                     f.GLCategory = GroupCategory

            For Each f In ff
                SaveAuditTrail(f.PID, 0)
            Next
            FlashForecasts.SaveChanges()
            Return True
        Catch ex As Exception
            Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Medium, AgnesMessageBox.MsgBoxLayout.FullText, AgnesMessageBox.MsgBoxType.OkOnly,
                                                    18, True, "Unexpected error!",, ex.Message)
            amsg.ShowDialog()
            amsg.Close()
            Return False
        End Try
    End Function

    Private Function UpdateForecast(SaveType As String, wkobj As CurrencyBox, wk As Byte) As Boolean
        Try
            Dim prevval As Double
            Dim quf = (From uf In FlashForecasts.Forecasts
                       Where uf.UnitNumber = UnitChooseObject.CurrentUnit And
                                            uf.MSFY = CurrentFiscalYear And
                                            uf.MSP = PeriodChooseObject.CurrentPeriod And
                                            uf.Week = wk And
                                            uf.GLCategory = GroupCategory).ToList()(0)
            prevval = quf.ForecastValue
            With quf
                .ForecastValue = wkobj.SetAmount
            End With
            FlashForecasts.SaveChanges()
            SaveAuditTrail(quf.PID, prevval)
            Return True
        Catch ex As Exception
            Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Medium, AgnesMessageBox.MsgBoxLayout.FullText, AgnesMessageBox.MsgBoxType.OkOnly,
                                        18, True, "Unexpected error!",, ex.Message)
            amsg.ShowDialog()
            amsg.Close()
            Return False
        End Try
    End Function

    Private Sub SaveAuditTrail(eid As Long, prevval As Double)
        Dim na As New ForecastAudits
        With na
            .ForecastId = eid
            .SavedBy = My.Settings.UserID
            .Date = Now()
            .PreviousValue = prevval
        End With
        FlashForecasts.ForecastAudits.Add(na)
    End Sub

    Private Sub CheckWeekFive()
        If GetMaxWeeks(PeriodChooseObject.CurrentPeriod) = 4 Then
            W5Val.Visibility = Visibility.Hidden
            W5Val.IsEnabled = False
            FcastPage.lblWeek5.Foreground = Brushes.LightGray
        Else
            W5Val.Visibility = Visibility.Visible
            W5Val.IsEnabled = True
        End If
    End Sub

    Private Sub LockPreviousWeeks()
        Dim openwk As Byte
        Dim testwk As Byte = GetCurrentWeek(FormatDateTime(Now(), DateFormat.ShortDate))
        Dim testperiod As Byte = GetCurrentPeriod(FormatDateTime(Now(), DateFormat.ShortDate))

        If PeriodChooseObject.CurrentPeriod > testperiod Then openwk = 0
        If PeriodChooseObject.CurrentPeriod = testperiod Then openwk = testwk

        Select Case openwk
            Case 0
                W1Val.IsEnabled = True
                W2Val.IsEnabled = True
                W3Val.IsEnabled = True
                W4Val.IsEnabled = True
                If W5Val.Visibility = Visibility.Visible Then W5Val.IsEnabled = True
            Case 1
                W1Val.IsEnabled = False
                W2Val.IsEnabled = True
                W3Val.IsEnabled = True
                W4Val.IsEnabled = True
                If W5Val.Visibility = Visibility.Visible Then W5Val.IsEnabled = True
            Case 2
                W1Val.IsEnabled = False
                W2Val.IsEnabled = False
                W3Val.IsEnabled = True
                W4Val.IsEnabled = True
                If W5Val.Visibility = Visibility.Visible Then W5Val.IsEnabled = True
            Case 3
                W1Val.IsEnabled = False
                W2Val.IsEnabled = False
                W3Val.IsEnabled = False
                W4Val.IsEnabled = True
                If W5Val.Visibility = Visibility.Visible Then W5Val.IsEnabled = True
            Case 4
                W1Val.IsEnabled = False
                W2Val.IsEnabled = False
                W3Val.IsEnabled = False
                W4Val.IsEnabled = False
                If W5Val.Visibility = Visibility.Visible Then W5Val.IsEnabled = True
            Case 5
                W1Val.IsEnabled = False
                W2Val.IsEnabled = False
                W3Val.IsEnabled = False
                W4Val.IsEnabled = False
                W5Val.IsEnabled = False
        End Select
    End Sub

#End Region

#Region "Event Listeners"
    Private Sub PeriodChanged()
        If FcastPage.SaveStatus = 0 Then
            Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Medium, AgnesMessageBox.MsgBoxLayout.BottomOnly, AgnesMessageBox.MsgBoxType.YesNo,
                                                18,,,, "Discard unsaved changes?")
            amsg.ShowDialog()
            If amsg.ReturnResult = "No" Then
                FcastPage.SaveStatus = 1
                PeriodChooseObject.CurrentPeriod = PeriodChooseObject.HeldPeriod
                amsg.Close()
                Exit Sub
            Else
                amsg.Close()
            End If
        End If
        Load()
        Update(Me)
    End Sub

    Private Sub UnitChanged()
        If FcastPage.SaveStatus = 0 Then
            Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Medium, AgnesMessageBox.MsgBoxLayout.BottomOnly, AgnesMessageBox.MsgBoxType.YesNo,
                                                18,,,, "Discard unsaved changes?")
            amsg.ShowDialog()
            If amsg.ReturnResult = "No" Then
                FcastPage.SaveStatus = 1
                UnitChooseObject.CurrentUnit = UnitChooseObject.HeldUnit
                amsg.Close()
                Exit Sub
            Else
                amsg.Close()
            End If
        End If
        Load()
        Update(Me)
    End Sub

    Private Sub ForecastChanged()
        Update(Me)
        UpdateSubtotals()
        FcastPage.SaveStatus = 0
    End Sub

#End Region

End Class
