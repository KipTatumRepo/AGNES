Public Class FlashGroup
    Inherits DockPanel
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

#End Region

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
        Notes = New Expander With {.Height = 32, .ExpandDirection = ExpandDirection.Right, .IsExpanded = False, .ToolTip = "Add Notes"}
        Notes.Content = New TextBox With {.MaxLength = 130, .Width = 715}

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
        AddHandler WeekChooseObject.PropertyChanged, AddressOf WeekChanged

        PeriodChooseObject = PC
        AddHandler PeriodChooseObject.PropertyChanged, AddressOf PeriodChanged

        UnitChooseObject = UC
        AddHandler UnitChooseObject.PropertyChanged, AddressOf UnitChanged

        AddHandler FlashVal.PropertyChanged, AddressOf FlashChanged
    End Sub

#Region "Event Listeners"
    Private Sub PeriodChanged()
        Load()
        Update(Me)
    End Sub

    Private Sub WeekChanged()
        'MsgBox(WeekChooseObject.SelectedCount)
        Load()
        Update(Me)
    End Sub

    Private Sub UnitChanged()
        ' MsgBox(UnitChooseObject.SelectedCount)
        Load()
        Update(Me)
    End Sub

    Private Sub FlashChanged()
        Update(Me)
        UpdateSubtotals()
        FlashPage.SaveStatus = 0
    End Sub
#End Region

#Region "Public Methods"
    Public Sub Load()
        LoadFlash()
        LoadBudget()
        LoadForecast()
    End Sub
    Public Sub Save()
        Dim ph As String = ""
    End Sub
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

#End Region

#Region "Private Methods"
    Private Sub LoadBudget()
        Dim unitbrd As Border, weekbrd As Border, unittb As TextBlock, weektb As TextBlock
        Dim CalculateBudget As Double = 0
        If GroupIsSubTotal = True Then Exit Sub
        For Each unitbrd In UnitChooseObject.Children
            If unitbrd.Tag <> "Label" Then
                unittb = unitbrd.Child
                If unittb.FontWeight = FontWeights.SemiBold Then
                    For Each weekbrd In WeekChooseObject.Children
                        If weekbrd.Tag <> "Label" Then
                            weektb = weekbrd.Child
                            If weektb.FontWeight = FontWeights.SemiBold And FormatNumber(weektb.Tag, 0) <= WeekChooseObject.MaxWeek Then
                                CalculateBudget += LoadSingleWeekAndUnitBudget(GroupCategory, FormatNumber(unittb.Tag, 0), 2019, PeriodChooseObject.CurrentPeriod,
                                                                      getweekoperatingdays(PeriodChooseObject.CurrentPeriod, FormatNumber(weektb.Tag, 0)),
                                                                      getperiodoperatingdays(PeriodChooseObject.CurrentPeriod))
                            End If
                        End If
                    Next
                End If
            End If
        Next
        BudgetContent = CalculateBudget
    End Sub

    Private Sub LoadFlash()
        'TODO:  CAPTURE INSTANCE WHERE USER IS RETURNING TO CURRENT WEEK FROM PTD VIEW
        FlashVal.IsEnabled = True
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
                                Dim AddValue = LoadSingleWeekAndUnitFlash(GroupCategory, FormatNumber(unittb.Tag, 0), 2019, PeriodChooseObject.CurrentPeriod, FormatNumber(weektb.Tag, 0))
                                '// Lock flash fields during PTD or Multiple Unit views, regardless of individual save statuses
                                If (WeekCount > 1) Or (UnitCount > 1) Or (AddValue.fv <> 999999.99) Then FlashVal.IsEnabled = False
                                Select Case AddValue.Stts
                                    Case "Flash"
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

        'TODO: PROOF AGAINST OVERWRITING OF UNSAVED FLASH NOTES
        If tb.Text = "" Then tb.Text = SharedFunctions.FlashNotes
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
                                CalculateForecast += LoadSingleWeekAndUnitForecast(GroupCategory, FormatNumber(unittb.Tag, 0), 2019, PeriodChooseObject.CurrentPeriod, FormatNumber(weektb.Tag, 0))
                            End If
                        End If
                    Next
                End If
            End If
        Next
        ForecastContent = CalculateForecast
    End Sub

    Private Sub UpdateSubtotals()
        Dim grd As Grid = Parent
        For Each fg As FlashGroup In grd.Children
            If fg.GroupIsSubTotal = True Then Update(fg)
        Next
    End Sub
#End Region

End Class
