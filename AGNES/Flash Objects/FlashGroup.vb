Public Class FlashGroup
    Inherits DockPanel
    'REFRESH: REPLACE TEXTBOXES WITH PERCENTAGE BOXES
    'REFRESH: REPLACE REFERENCED WEEK, PERIOD, AND UNIT CHOOSERS WITH ACTUALS BOUND TO XAML PAGES

#Region "Properties"
    Public GroupCategory As String
    Public FlashVal As CurrencyBox
    Public FlashPercent As TextBox
    Public BudgetVal As CurrencyBox
    Public BudgetPercent As TextBox
    Public BudgetVariance As CurrencyBox
    Public ForecastVal As CurrencyBox
    Public ForecastPercent As TextBox
    Public ForecastVariance As CurrencyBox
    Public Notes As Expander
    Private NotesText As TextBox
    Private _notestate As Boolean
    Private Property NoteState As Boolean
        Get
            Return _notestate
        End Get
        Set(value As Boolean)
            _notestate = value
            If value = True Then
                Notes.Background = Brushes.Transparent
            Else
                If NoteContent <> "" Then Notes.Background = Brushes.Red
            End If
        End Set
    End Property
    Private _notescontent As String
    Public Property NoteContent As String
        Get
            Return _notescontent
        End Get
        Set(value As String)
            _notescontent = value
            If value <> "" Then
                Notes.Background = Brushes.Red
            Else
                Notes.Background = Brushes.Transparent
            End If
        End Set
    End Property
    Public WeekChooseObject As WeekChooser
    Public PeriodChooseObject As PeriodChooser
    Public UnitChooseObject As UnitChooser
    Public SalesFlashGroup As FlashGroup
    Private _flashcontent As Double
    Private _heldflashcontent As Double
    Private _budgetcontent As Double
    Private _forecastcontent As Double
    Public Property FlashContent As Double
        Get
            Return _flashcontent
        End Get
        Set(value As Double)
            _flashcontent = value
            FlashVal.SetAmount = value
            Update(Me)
        End Set
    End Property
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
    Public Property ForecastContent As Double
        Get
            Return _forecastcontent
        End Get
        Set(value As Double)
            _forecastcontent = value
            ForecastVal.SetAmount = value
            Update(Me)
        End Set
    End Property
    Private _subtotal As Boolean
    Public Property GroupIsSubTotal As Boolean
        Get
            Return _subtotal
        End Get
        Set(value As Boolean)
            _subtotal = value
            If value = True Then Background = Brushes.LightGray
        End Set
    End Property
    Public Property SubtotalGroups As New List(Of FlashGroup)
    Private Property _largepercentage As Boolean
    Public Property LargePercentage As Boolean
        Get
            Return _largepercentage
        End Get

        Set(value As Boolean)
            _largepercentage = value
            If value = True Then
                FlashPercent.FontSize = 8
                BudgetPercent.FontSize = 8
                ForecastPercent.FontSize = 8
            Else
                FlashPercent.FontSize = 12
                BudgetPercent.FontSize = 12
                ForecastPercent.FontSize = 12
            End If
        End Set
    End Property
    Private Property GroupHasForecast As Boolean
    Private Property GroupHasPercentages As Boolean
    Public Property SpreadByWeeks As Boolean

#End Region

#Region "Constructor"
    Public Sub New(PC As PeriodChooser, WC As WeekChooser, UC As UnitChooser, GroupName As String, ShowPercentages As Boolean, Top As Integer, Highlight As Boolean, Subtotal As Boolean, HasForecast As Boolean, CreditOnly As Boolean, DebitOnly As Boolean, Optional SubtotalGroupList As List(Of FlashGroup) = Nothing)
        GroupCategory = GroupName
        GroupHasForecast = HasForecast
        GroupHasPercentages = ShowPercentages
        HorizontalAlignment = HorizontalAlignment.Left
        VerticalAlignment = VerticalAlignment.Top
        Height = 42
        Width = 962
        LastChildFill = False
        Margin = New Thickness(10, Top, 0, 0)
        If Highlight = True Then Background = Brushes.WhiteSmoke
        GroupIsSubTotal = Subtotal
        '// Create Flash group header label
        Dim GroupLabel As New Border
        Dim tb As New TextBlock With {.Text = GroupName, .LineHeight = 16, .TextAlignment = TextAlignment.Center,
            .Margin = New Thickness(0, -2, 0, 0), .Width = 80, .VerticalAlignment = VerticalAlignment.Center,
            .FontSize = 16, .FontWeight = FontWeights.SemiBold, .TextWrapping = TextWrapping.Wrap}
        GroupLabel.Child = tb

        '// Create Flash value input field
        FlashVal = New CurrencyBox(140, True, AgnesBaseInput.FontSz.Medium,, CreditOnly, DebitOnly) With
            {.Margin = New Thickness(4, 4, 0, 0)}

        If GroupIsSubTotal = True Then IsEnabled = False

        '// Create expander for notes
        Notes = New Expander With {.Height = 24, .ExpandDirection = ExpandDirection.Right, .IsExpanded = False, .ToolTip = "Add Notes"}
        NotesText = New TextBox With {.MaxLength = 130, .Width = 715, .Background = Brushes.White, .Opacity = 1}
        AddHandler NotesText.GotFocus, AddressOf EnterNoteField
        AddHandler NotesText.LostFocus, AddressOf LeaveNoteField
        AddHandler NotesText.TextChanged, AddressOf NotesChanged
        AddHandler Notes.Expanded, AddressOf NotesExpanderChanged
        AddHandler Notes.Collapsed, AddressOf NotesExpanderChanged
        AddHandler Notes.PreviewKeyDown, AddressOf EnterKeyCheck
        Notes.Content = NotesText

        '// Create flash percentage textbox.  Hide if it doesn't belong with this group (preserving spacing)
        FlashPercent = New TextBox With
            {.FontSize = 12, .VerticalAlignment = VerticalAlignment.Center,
            .HorizontalAlignment = Windows.HorizontalAlignment.Center, .BorderBrush = Brushes.LightGray, .IsEnabled = False,
            .Height = 26, .Width = 40, .Margin = New Thickness(4, 0, 0, 0)}
        If GroupHasPercentages = False Then FlashPercent.Visibility = Visibility.Hidden

        '// Create budget value field
        BudgetVal = New CurrencyBox(140, True, AgnesBaseInput.FontSz.Medium,, CreditOnly, DebitOnly) With
            {.Margin = New Thickness(4, 4, 0, 0), .IsEnabled = False}

        '// Create budget percentage textbox.  Hide if it doesn't belong with this group (preserving spacing)
        BudgetPercent = New TextBox With
            {.FontSize = 12, .VerticalAlignment = VerticalAlignment.Center,
            .HorizontalAlignment = Windows.HorizontalAlignment.Center, .BorderBrush = Brushes.LightGray, .IsEnabled = False,
            .Height = 26, .Width = 40, .Margin = New Thickness(4, 0, 0, 0)}

        If GroupHasPercentages = False Then BudgetPercent.Visibility = Visibility.Hidden

        '// Create variance value field
        BudgetVariance = New CurrencyBox(140, True, AgnesBaseInput.FontSz.Medium) With
            {.Margin = New Thickness(4, 4, 0, 0), .IsEnabled = False}


        '// Create forecast value field
        ForecastVal = New CurrencyBox(140, True, AgnesBaseInput.FontSz.Medium,, CreditOnly, DebitOnly) With
            {.Margin = New Thickness(4, 4, 0, 0), .IsEnabled = False}

        '// Create forecast percentage textbox.  Hide if it doesn't belong with this group (preserving spacing)
        ForecastPercent = New TextBox With
            {.FontSize = 12, .VerticalAlignment = VerticalAlignment.Center,
            .HorizontalAlignment = Windows.HorizontalAlignment.Center, .BorderBrush = Brushes.LightGray, .IsEnabled = False,
            .Height = 26, .Width = 40, .Margin = New Thickness(4, 0, 0, 0)}

        If GroupHasPercentages = False Then ForecastPercent.Visibility = Visibility.Hidden

        '// Create forecast variance value field
        ForecastVariance = New CurrencyBox(140, True, AgnesBaseInput.FontSz.Medium) With
            {.Margin = New Thickness(4, 4, 0, 0), .IsEnabled = False}

        With Children
            .Add(GroupLabel)
            .Add(FlashVal)
            .Add(Notes)
            .Add(FlashPercent)
            .Add(BudgetVal)
            .Add(BudgetPercent)
            .Add(BudgetVariance)
        End With

        If GroupHasForecast = True Then
            With Children
                .Add(ForecastVal)
                .Add(ForecastPercent)
                .Add(ForecastVariance)
            End With
        End If

        If GroupIsSubTotal = True Then
            Notes.Visibility = Visibility.Hidden
            For Each fg As FlashGroup In SubtotalGroupList
                SubtotalGroups.Add(fg)
            Next
        End If

        WeekChooseObject = WC
        PeriodChooseObject = PC
        UnitChooseObject = UC

        AddHandler FlashVal.PropertyChanged, AddressOf FlashChanged
    End Sub

#End Region

#Region "Public Methods"
    Public Sub Load()
        AlertOverride = True
        LoadFlash()
        LoadBudget()
        LoadForecast()
        AlertOverride = False
    End Sub

    Public Function Save(SaveType) As Boolean
        Dim SaveOkay As Boolean
        '// SaveType only influences the value saved to the status field; the status bar is updated via the Flashpage that calls this routine

        '// Check to see if textbox is in edit mode 
        If FlashVal.FieldInEditMode = True Then
            '// Compare value with saved value; if a match, ignore and proceed.  If not, prompt.
            Dim cval As Double
            Try
                cval = FormatNumber(FlashVal.tb.Text, 2)
            Catch ex As Exception
                cval = FlashVal.SetAmount
            End Try

            If cval <> FlashVal.SetAmount Then
                Dim msg As String = "It looks like you may still be editing " & GroupCategory & ".  Do you want " &
                    FormatCurrency(FlashVal.tb.Text, 2) & " to be the amount saved?  Selecting No aborts your save."
                Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Medium, AgnesMessageBox.MsgBoxLayout.FullText,
                                        AgnesMessageBox.MsgBoxType.YesNo, 18,, "Uncommitted data",, msg, AgnesMessageBox.ImageType.Danger)
                amsg.ShowDialog()
                If amsg.ReturnResult = "No" Then
                    amsg.Close()
                    Return False
                    Exit Function
                Else
                    amsg.Close()
                    FlashVal.tb.Text = FormatCurrency(cval, 2)
                    FlashVal.SetAmount = FormatNumber(cval, 2)
                End If
            End If
        End If


        '// Check to see if multiple units, weeks, or periods are selected. If so, kill routine 
        If CheckIfMultipleAreSelected() > 3 Then
            Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Small, AgnesMessageBox.MsgBoxLayout.FullText, AgnesMessageBox.MsgBoxType.OkOnly, 8, True, "Cannot save with multiple periods/weeks/units selected")
            amsg.ShowDialog()
            Return False
            Exit Function
        End If

        '// Perform check to see if the record already exists; this defines which save function (new or update) is called
        Dim ff = From f In FlashActuals.FlashActualData
                 Where f.MSFY = CurrentFiscalYear And
                     f.MSP = PeriodChooseObject.CurrentPeriod And
                     f.Week = WeekChooseObject.CurrentWeek And
                     f.UnitNumber = UnitChooseObject.CurrentUnit And
                     f.GLCategory = GroupCategory

        Try
            If ff.Count = 0 Then
                SaveOkay = SaveNewFlash(SaveType)
            Else
                SaveOkay = UpdateFlash(SaveType)
            End If
        Catch ex As Exception
            Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Small, AgnesMessageBox.MsgBoxLayout.FullText, AgnesMessageBox.MsgBoxType.OkOnly, 8, True, "Unable to save!",, "Error: " & ex.Message)
            amsg.ShowDialog()
            Return False
            Exit Function
        End Try
        Return True
    End Function

    Public Sub Update(TargetFlashGroup As FlashGroup)
        '//     Recalculate subtotals, if applicable
        If TargetFlashGroup.GroupIsSubTotal = True Then
            Dim flashsub As Double, budgetsub As Double, forecastsub As Double
            For Each fg As FlashGroup In TargetFlashGroup.SubtotalGroups
                flashsub += fg.FlashVal.SetAmount
                budgetsub += fg.BudgetVal.SetAmount
                If TargetFlashGroup.GroupHasForecast Then forecastsub += fg.ForecastVal.SetAmount
            Next
            TargetFlashGroup.FlashVal.SetAmount = flashsub
            TargetFlashGroup.BudgetVal.SetAmount = budgetsub
            If TargetFlashGroup.GroupHasForecast = True Then TargetFlashGroup.ForecastVal.SetAmount = forecastsub
            TargetFlashGroup.IsEnabled = False
        End If

        '//     Recalculate variances
        TargetFlashGroup.BudgetVariance.SetAmount = (TargetFlashGroup.BudgetVal.SetAmount - TargetFlashGroup.FlashVal.SetAmount)
        If TargetFlashGroup.GroupHasForecast = True Then TargetFlashGroup.ForecastVariance.SetAmount = (TargetFlashGroup.ForecastVal.SetAmount - TargetFlashGroup.FlashVal.SetAmount)

        '//     Recalculate percentages, if applicable
        If TargetFlashGroup.GroupHasPercentages = True Then
            Dim flashperc As Double, budgetperc As Double, forecastperc As Double, salesamount As Double
            Try
                salesamount = Math.Abs(TargetFlashGroup.SalesFlashGroup.FlashVal.SetAmount)
                flashperc = (TargetFlashGroup.FlashVal.SetAmount / salesamount)
            Catch ex As Exception
                flashperc = 0
            End Try
            TargetFlashGroup.FlashPercent.Text = FormatPercent(flashperc, 1)

            Try
                salesamount = Math.Abs(TargetFlashGroup.SalesFlashGroup.BudgetVal.SetAmount)
                budgetperc = (TargetFlashGroup.BudgetVal.SetAmount / salesamount)
            Catch ex As Exception
                budgetperc = 0
            End Try
            TargetFlashGroup.BudgetPercent.Text = FormatPercent(budgetperc, 1)

            If TargetFlashGroup.GroupHasForecast = True Then
                Try
                    salesamount = Math.Abs(TargetFlashGroup.SalesFlashGroup.ForecastVal.SetAmount)
                    forecastperc = (TargetFlashGroup.ForecastVal.SetAmount / salesamount)
                Catch ex As Exception
                    forecastperc = 0
                End Try
                TargetFlashGroup.ForecastPercent.Text = FormatPercent(forecastperc, 1)
            End If

            '//     Reduce font size if percentage is 1000% or greater
            If (flashperc >= 1) Or (budgetperc >= 1) Or (forecastperc >= 1) Then
                TargetFlashGroup.LargePercentage = True
            Else
                TargetFlashGroup.LargePercentage = False
            End If
        End If
    End Sub

    Public Sub SetFocus()
        FlashVal.tb.Focus()
    End Sub

#End Region

#Region "Private Methods"
    Private Sub LoadBudget()
        Dim unitbrd As Border, weekbrd As Border, unittb As TextBlock, weektb As TextBlock
        Dim CalculateBudget As Double = 0, WeekCount As Byte, UnitCount As Byte
        If GroupIsSubTotal = True Then Exit Sub
        For Each unitbrd In UnitChooseObject.Children
            If unitbrd.Tag <> "Label" Then
                unittb = unitbrd.Child
                If unittb.FontWeight = FontWeights.SemiBold Then
                    UnitCount += 1
                    For Each weekbrd In WeekChooseObject.Children
                        If weekbrd.Tag <> "Label" Then
                            weektb = weekbrd.Child
                            If weektb.FontWeight = FontWeights.SemiBold And FormatNumber(weektb.Tag, 0) <= WeekChooseObject.MaxWeek Then
                                WeekCount += 1
                                If SpreadByWeeks = False Then
                                    CalculateBudget += LoadSingleWeekAndUnitBudget(GroupCategory, FormatNumber(unittb.Tag, 0), CurrentFiscalYear, PeriodChooseObject.CurrentPeriod,
                                                                          getweekoperatingdays(CurrentFiscalYear, PeriodChooseObject.CurrentPeriod, FormatNumber(weektb.Tag, 0)),
                                                                          getperiodoperatingdays(CurrentFiscalYear, PeriodChooseObject.CurrentPeriod))
                                Else
                                    Dim tempopdays = 4
                                    If getperiodoperatingdays(CurrentFiscalYear, PeriodChooseObject.CurrentPeriod) > 20 Then tempopdays = 5
                                    CalculateBudget += LoadSingleWeekAndUnitBudget(GroupCategory, FormatNumber(unittb.Tag, 0), CurrentFiscalYear,
                                                                                   PeriodChooseObject.CurrentPeriod, 1, tempopdays)
                                End If

                            End If
                        End If
                    Next
                End If
            End If
        Next
        BudgetContent = CalculateBudget
    End Sub

    Private Sub LoadFlash()
        'REFRESH: REFACTOR FLASH LOAD ROUTINES
        IsEnabled = True
        Dim CurrVal As Double = 0, WeekCount As Byte, UnitCount As Byte
        Dim unitbrd As Border, weekbrd As Border, unittb As TextBlock, weektb As TextBlock, tmpsavestatus As Byte, notestb As TextBox = Notes.Content
        notestb.Text = ""
        Dim CalculateFlash As Double = 0
        If GroupIsSubTotal = True Then Exit Sub
        For Each unitbrd In UnitChooseObject.Children
            If unitbrd.Tag <> "Label" Then
                unittb = unitbrd.Child
                If unittb.FontWeight = FontWeights.SemiBold Then
                    UnitCount += 1
                    For Each weekbrd In WeekChooseObject.Children
                        If weekbrd.Tag <> "Label" Then
                            weektb = weekbrd.Child
                            If weektb.FontWeight = FontWeights.SemiBold And FormatNumber(weektb.Tag, 0) <= WeekChooseObject.MaxWeek Then
                                WeekCount += 1
                                Dim AddValue = LoadSingleWeekAndUnitFlash(GroupCategory, FormatNumber(unittb.Tag, 0), CurrentFiscalYear, PeriodChooseObject.CurrentPeriod, FormatNumber(weektb.Tag, 0))
                                NoteContent = AddValue.Notes
                                notestb.Text = NoteContent
                                Select Case AddValue.Stts
                                    Case "Final"
                                        FlashVal.IsEnabled = False
                                        notestb.IsEnabled = False
                                        tmpsavestatus = 3
                                    Case "Draft"
                                        FlashVal.IsEnabled = True
                                        notestb.IsEnabled = True
                                        tmpsavestatus = 1
                                    Case Else
                                        FlashVal.IsEnabled = True
                                        notestb.IsEnabled = True
                                        tmpsavestatus = 2
                                End Select

                                '// Lock flash fields during PTD or Multiple Unit views, regardless of individual save statuses
                                If CheckIfMultipleAreSelected() > 3 Then
                                    FlashVal.IsEnabled = False
                                    notestb.IsEnabled = False
                                End If

                                Try
                                    If AddValue.alert = True Then
                                        FlashPage.ToggleAlert(0)
                                    Else
                                        FlashPage.ToggleAlert(1)
                                    End If
                                Catch ex As Exception
                                    ' Initial load or legitimate error
                                End Try

                                Try
                                    FlashPage.SaveStatus = tmpsavestatus
                                Catch ex As Exception
                                        InitialLoadStatus = tmpsavestatus
                                    End Try
                                    If AddValue.fv = 999999.99 Then AddValue.fv = 0
                                    CalculateFlash += AddValue.fv
                                End If
                            End If
                    Next
                End If
            End If
        Next
        CalculateFlash += CurrVal
        FlashContent = CalculateFlash
        Dim tb As TextBox = Notes.Content
        If tb.Text = "" Then tb.Text = BaseModule.FlashNotes
    End Sub

    Private Sub LoadForecast()
        Dim ph As String = "FlashForecasts"
        Dim unitbrd As Border, weekbrd As Border, unittb As TextBlock, weektb As TextBlock
        Dim CalculateForecast As Double = 0
        If GroupIsSubTotal = True Then Exit Sub
        For Each unitbrd In UnitChooseObject.Children
            If unitbrd.Tag <> "Label" Then
                unittb = unitbrd.Child
                If unittb.FontWeight = FontWeights.SemiBold Then
                    For Each weekbrd In WeekChooseObject.Children
                        If weekbrd.Tag <> "Label" Then
                            weektb = weekbrd.Child
                            If weektb.FontWeight = FontWeights.SemiBold And FormatNumber(weektb.Tag, 0) <= WeekChooseObject.MaxWeek Then
                                CalculateForecast += LoadSingleWeekAndUnitForecast(GroupCategory, FormatNumber(unittb.Tag, 0), CurrentFiscalYear, PeriodChooseObject.CurrentPeriod, FormatNumber(weektb.Tag, 0))
                            End If
                        End If
                    Next
                End If
            End If
        Next
        ForecastContent = CalculateForecast
    End Sub

    Private Sub UpdateSubtotals(sender As FlashGroup)
        Dim grd As Grid = Parent
        For Each fg As FlashGroup In grd.Children
            If fg.GroupIsSubTotal = True Then Update(fg)
            If fg.SalesFlashGroup Is sender Then Update(fg)
        Next
    End Sub

    Private Function SaveNewFlash(SaveType) As Boolean
        Dim tb As TextBox = Notes.Content
        Try
            Dim nf As New FlashActualData
            With nf
                .MSFY = CurrentFiscalYear
                .MSP = PeriodChooseObject.CurrentPeriod
                .Week = WeekChooseObject.CurrentWeek
                .UnitNumber = UnitChooseObject.CurrentUnit
                .Status = SaveType
                .GL = 0
                .GLCategory = GroupCategory
                .FlashValue = FlashVal.SetAmount
                .FlashNotes = tb.Text
                .SavedBy = My.Settings.UserName
            End With
            If SpreadByWeeks = False Then
                nf.OpDaysWeek = getweekoperatingdays(CurrentFiscalYear, PeriodChooseObject.CurrentPeriod, WeekChooseObject.CurrentWeek)
                nf.OpDaysPeriod = getperiodoperatingdays(CurrentFiscalYear, PeriodChooseObject.CurrentPeriod)
            Else
                Dim tempopdays = 4
                If getperiodoperatingdays(CurrentFiscalYear, PeriodChooseObject.CurrentPeriod) > 20 Then tempopdays = 5
                nf.OpDaysWeek = 1
                nf.OpDaysPeriod = tempopdays
            End If

            If FlashPage.imgEscalate.Tag = "On" Then
                nf.Alert = True
            Else
                nf.Alert = False
            End If
            FlashActuals.FlashActualData.Add(nf)
            FlashActuals.SaveChanges()
            If SaveType = "Final" Then
                FlashVal.IsEnabled = False
                tb.IsEnabled = False
            End If
            Return True
        Catch ex As Exception
            Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Medium, AgnesMessageBox.MsgBoxLayout.FullText, AgnesMessageBox.MsgBoxType.OkOnly,
                                                    18, True, "Unexpected error!",, ex.Message)
            amsg.ShowDialog()
            amsg.Close()
            Return False
        End Try
    End Function

    Private Function UpdateFlash(SaveType) As Boolean
        Dim tb As TextBox = Notes.Content
        Try

            Dim uf = (From cust In FlashActuals.FlashActualData
                      Where cust.UnitNumber = UnitChooseObject.CurrentUnit And
                                            cust.MSFY = CurrentFiscalYear And
                                            cust.MSP = PeriodChooseObject.CurrentPeriod And
                                            cust.Week = WeekChooseObject.CurrentWeek And
                                            cust.GLCategory = GroupCategory).ToList()(0)
            With uf
                .Status = SaveType
                .FlashValue = FlashVal.SetAmount
                .FlashNotes = tb.Text
                .SavedBy = My.Settings.UserName
            End With

            If SpreadByWeeks = False Then
                uf.OpDaysWeek = getweekoperatingdays(CurrentFiscalYear, PeriodChooseObject.CurrentPeriod, WeekChooseObject.CurrentWeek)
                uf.OpDaysPeriod = getperiodoperatingdays(CurrentFiscalYear, PeriodChooseObject.CurrentPeriod)
            Else
                Dim tempopdays = 4
                If getperiodoperatingdays(CurrentFiscalYear, PeriodChooseObject.CurrentPeriod) > 20 Then tempopdays = 5
                uf.OpDaysWeek = 1
                uf.OpDaysPeriod = tempopdays
            End If


            If FlashPage.imgEscalate.Tag = "On" Then
                uf.Alert = True
            Else
                uf.Alert = False
            End If

            FlashActuals.SaveChanges()

            If SaveType = "Final" Then
                FlashVal.IsEnabled = False
                tb.IsEnabled = False
            End If
            Return True
        Catch ex As Exception
            Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Medium, AgnesMessageBox.MsgBoxLayout.FullText, AgnesMessageBox.MsgBoxType.OkOnly,
                                        18, True, "Unexpected error!",, ex.Message)
            amsg.ShowDialog()
            amsg.Close()
            Return False
        End Try
    End Function

    Private Function CheckIfMultipleAreSelected() As Byte
        Dim unitbrd As Border, periodbrd As Border, weekbrd As Border, unittb As TextBlock, periodtb As TextBlock, weektb As TextBlock, InternalCounter As Byte = 0
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

        For Each weekbrd In WeekChooseObject.Children
            If weekbrd.Tag <> "Label" Then
                weektb = weekbrd.Child
                If weektb.FontWeight = FontWeights.SemiBold Then InternalCounter += 1
            End If
        Next
        Return InternalCounter
    End Function

    Private Sub NotesExpanderChanged()
        NoteState = Notes.IsExpanded
    End Sub

    Private Sub LeaveNoteField()
        Notes.IsExpanded = False
        NoteContent = NotesText.Text
    End Sub

    Private Sub EnterNoteField()
        NoteContent = NotesText.Text
    End Sub

    Private Sub NotesChanged()
        If NotesText.Text <> NoteContent Then FlashPage.SaveStatus = 0
    End Sub

    Private Sub EnterKeyCheck(sender As Object, e As KeyEventArgs)
        If e.Key = Key.Enter Then
            Dim ct As Byte = 0, target As Byte, fgcount As Byte
            For Each fg As FlashGroup In FlashPage.grdFlashGroups.Children
                fgcount += 1
            Next

            For Each fg As FlashGroup In FlashPage.grdFlashGroups.Children
                If fg Is Me Then
                    target = ct
                    Exit For
                End If
                ct += 1
            Next
            ct = 0
            For Each fg As FlashGroup In FlashPage.grdFlashGroups.Children
                If ct >= target + 1 Then
                    If fg.IsEnabled = True Then
                        fg.SetFocus()
                        e.Handled = True
                        Exit Sub
                    End If
                End If
                ct += 1
                If ct >= fgcount Then
                    For Each fg2 As FlashGroup In FlashPage.grdFlashGroups.Children
                        fg2.SetFocus()
                        e.Handled = True
                        Exit Sub
                    Next
                End If
            Next
        End If
    End Sub

#End Region

#Region "Event Listeners"
    Private Sub FlashChanged()
        Update(Me)
        UpdateSubtotals(Me)
        FlashPage.SaveStatus = 0
    End Sub
#End Region

End Class
