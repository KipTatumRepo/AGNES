Public Class FlashGroup
    Inherits DockPanel
    Public FlashVal As CurrencyBox
    Public FlashPercent As TextBox
    Public BudgetVal As CurrencyBox
    Public BudgetPercent As TextBox
    Public BudgetVariance As CurrencyBox
    Public ForecastVal As CurrencyBox
    Public ForecastPercent As TextBox
    Public ForecastVariance As CurrencyBox
    Public Notes As Expander
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
    Public Sub New(GroupName As String, ShowPercentages As Boolean, Top As Integer, Highlight As Boolean, Subtotal As Boolean)
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
    End Sub

    Private Sub EnterNotes()
        MsgBox("Enter notes")
    End Sub

    Public Sub Update()
        Dim ph As String = ""
    End Sub
End Class
