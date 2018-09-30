Public Class FlashGroup
    Inherits DockPanel
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
            Update()
        End Set
    End Property
    Public Property BudgetContent As Double
        Get
            Return _budgetcontent
        End Get
        Set(value As Double)
            _budgetcontent = value
            BudgetVal.SetAmount = value
            Update()
        End Set
    End Property
    Public Property ForecastContent As Double
        Get
            Return _forecastcontent
        End Get
        Set(value As Double)
            _forecastcontent = value
            ForecastVal.SetAmount = value
            Update()
        End Set
    End Property
    Private _subtotal As Boolean
    Private Property IsSubTotal As Boolean
        Get
            Return _subtotal
        End Get
        Set(value As Boolean)
            _subtotal = value
            If value = True Then Background = Brushes.LightGray
        End Set
    End Property
    Public Property SubtotalGroups As List(Of FlashGroup)
    Public Sub New(PC As PeriodChooser, WC As WeekChooser, UC As UnitChooser, GroupName As String, ShowPercentages As Boolean, Top As Integer, Highlight As Boolean, Subtotal As Boolean)
        GroupCategory = GroupName
        HorizontalAlignment = HorizontalAlignment.Left
        VerticalAlignment = VerticalAlignment.Top
        Height = 42
        Width = 962
        LastChildFill = False
        Margin = New Thickness(10, Top, 0, 0)
        If Highlight = True Then Background = Brushes.WhiteSmoke
        IsSubTotal = Subtotal
        '// Create Flash group header label
        Dim GroupLabel As New Border
        Dim tb As New TextBlock With {.Text = GroupName, .LineHeight = 16, .TextAlignment = TextAlignment.Center,
            .Margin = New Thickness(0, -2, 0, 0), .Width = 80, .VerticalAlignment = VerticalAlignment.Center,
            .FontSize = 16, .FontWeight = FontWeights.SemiBold, .TextWrapping = TextWrapping.Wrap}
        GroupLabel.Child = tb

        '// Create Flash value input field
        FlashVal = New CurrencyBox(140, False, True, False, False, True, AgnesBaseInput.FontSz.Medium) With
            {.Margin = New Thickness(4, 4, 0, 0)}

        '// Create expander for notes
        Notes = New Expander With {.Height = 32, .ExpandDirection = ExpandDirection.Right, .ToolTip = "Add Notes"}
        Notes.Content = New TextBox With {.MaxLength = 130, .Width = 700}

        '// Create flash percentage textbox.  Hide if it doesn't belong with this group (preserving spacing)
        FlashPercent = New TextBox With
            {.Text = "95%", .FontSize = 16, .VerticalAlignment = VerticalAlignment.Center,
            .HorizontalAlignment = Windows.HorizontalAlignment.Center, .BorderBrush = Brushes.LightGray, .IsEnabled = False,
            .Height = 26, .Margin = New Thickness(4, 0, 0, 0)}
        If ShowPercentages = False Then FlashPercent.Visibility = Visibility.Hidden

        '// Create budget value field
        BudgetVal = New CurrencyBox(140, True, False, True, False, True, AgnesBaseInput.FontSz.Medium) With
            {.Margin = New Thickness(4, 4, 0, 0), .IsEnabled = False}

        '// Create budget percentage textbox.  Hide if it doesn't belong with this group (preserving spacing)
        BudgetPercent = New TextBox With
            {.Text = "85%", .FontSize = 16, .VerticalAlignment = VerticalAlignment.Center,
            .HorizontalAlignment = Windows.HorizontalAlignment.Center, .BorderBrush = Brushes.LightGray, .IsEnabled = False,
            .Height = 26, .Margin = New Thickness(4, 0, 0, 0)}

        If ShowPercentages = False Then BudgetPercent.Visibility = Visibility.Hidden

        '// Create variance value field
        BudgetVariance = New CurrencyBox(140, True, False, True, False, True, AgnesBaseInput.FontSz.Medium) With
            {.Margin = New Thickness(4, 4, 0, 0), .IsEnabled = False}

        '// Create forecast value field
        ForecastVal = New CurrencyBox(140, True, False, True, False, True, AgnesBaseInput.FontSz.Medium) With
            {.Margin = New Thickness(4, 4, 0, 0), .IsEnabled = False}

        '// Create forecast percentage textbox.  Hide if it doesn't belong with this group (preserving spacing)
        ForecastPercent = New TextBox With
            {.Text = "75%", .FontSize = 16, .VerticalAlignment = VerticalAlignment.Center,
            .HorizontalAlignment = Windows.HorizontalAlignment.Center, .BorderBrush = Brushes.LightGray, .IsEnabled = False,
            .Height = 26, .Margin = New Thickness(4, 0, 0, 0)}

        If ShowPercentages = False Then ForecastPercent.Visibility = Visibility.Hidden

        '// Create forecast variance value field
        ForecastVariance = New CurrencyBox(140, True, False, True, False, True, AgnesBaseInput.FontSz.Medium) With
            {.Margin = New Thickness(4, 4, 0, 0), .IsEnabled = False}

        With Children
            .Add(GroupLabel)
            .Add(FlashVal)
            .Add(Notes)
            .Add(FlashPercent)
            .Add(BudgetVal)
            .Add(BudgetPercent)
            .Add(BudgetVariance)
            .Add(ForecastVal)
            .Add(ForecastPercent)
            .Add(ForecastVariance)
        End With
        If IsSubTotal = True Then Notes.Visibility = Visibility.Hidden

        WeekChooseObject = WC
        AddHandler WeekChooseObject.PropertyChanged, AddressOf WeekChanged

        PeriodChooseObject = PC
        AddHandler PeriodChooseObject.PropertyChanged, AddressOf PeriodChanged

        UnitChooseObject = UC
        AddHandler UnitChooseObject.PropertyChanged, AddressOf UnitChanged

    End Sub
#Region "Private Event Listeners"
    Private Sub PeriodChanged()
        Load()
    End Sub

    Private Sub WeekChanged()
        Load()
    End Sub

    Private Sub UnitChanged()
        Load()
    End Sub
#End Region

#Region "Public Methods"
    Public Sub Load()
        Dim unitbrd As Border, weekbrd As Border, unittb As TextBlock, weektb As TextBlock
        Dim CalculateBudget As Double = 0
        If IsSubTotal = True Then Exit Sub
        For Each unitbrd In UnitChooseObject.Children
            If unitbrd.Tag <> "Label" Then
                unittb = unitbrd.Child
                If unittb.FontWeight = FontWeights.SemiBold Then
                    For Each weekbrd In WeekChooseObject.Children
                        If weekbrd.Tag <> "Label" Then
                            weektb = weekbrd.Child
                            If weektb.FontWeight = FontWeights.SemiBold And FormatNumber(weektb.Tag, 0) <= WeekChooseObject.MaxWeek Then
                                CalculateBudget += LoadSingleWeekAndUnitBudget(FormatNumber(unittb.Tag, 0), 2019, PeriodChooseObject.CurrentPeriod,
                                                                      getweekoperatingdays(PeriodChooseObject.CurrentPeriod, FormatNumber(weektb.Tag, 0)),
                                                                      getperiodoperatingdays(PeriodChooseObject.CurrentPeriod, FormatNumber(weektb.Tag, 0)))
                            End If
                        End If
                    Next
                End If
            End If
        Next
        BudgetContent = CalculateBudget
        ' LoadSingleWeekAndUnitBudget(UnitChooseObject.CurrentUnit, 2019, PeriodChooseObject.CurrentPeriod, 5, 19)
    End Sub
    Public Sub Save()
        Dim ph As String = ""
    End Sub
    Public Sub Update()
        Dim ph As String = ""
    End Sub
#End Region

#Region "Private Methods"
    Private Function getweekoperatingdays(p, w) As Byte
        Return 5    'TODO: TEST ONLY
    End Function
    Private Function getperiodoperatingdays(p, w) As Byte
        Return 25   'TODO: TEST ONLY
    End Function
    Private Function LoadSingleWeekandUnitFlash(category, unit, year, period, week) As Double
        Return 0
    End Function

    Private Function LoadSingleWeekAndUnitBudget(unit As Int64, yr As Int16, period As Byte, weekoperatingdays As Byte, periodoperatingdays As Byte) As Double
        Dim bf = From b In FlashBudgets.Budgets
                 Where b.Category = GroupCategory And
                     b.MSFY = yr And
                     b.MSP = period And
                     b.UnitNumber = unit
                 Select b
        For Each b In bf
            Return (b.Budget1 / periodoperatingdays) * weekoperatingdays
            Exit Function
        Next
        Return 0
    End Function
#End Region

End Class
