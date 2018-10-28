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
    Public BudgetVal As CurrencyBox
    Public VarianceVal As CurrencyBox
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
        Width = 962
        LastChildFill = False
        Margin = New Thickness(10, Top, 0, 0)
        If Highlight = True Then Background = Brushes.WhiteSmoke
        GroupIsSubTotal = Subtotal
        '// Create Flash group header label
        Dim GroupLabel As New Border
        Dim tb As New TextBlock With {.Text = GroupName, .Height = 42, .Width = 80, .LineHeight = 16, .TextAlignment = TextAlignment.Center,
            .Margin = New Thickness(0, -2, 0, 0), .VerticalAlignment = VerticalAlignment.Center,
            .FontSize = 12, .FontWeight = FontWeights.SemiBold, .TextWrapping = TextWrapping.Wrap}
        GroupLabel.Child = tb
        '// Create daily run rate field
        DRR = New CurrencyBox(80, True, AgnesBaseInput.FontSz.Medium,, CreditOnly, DebitOnly) With
            {.Margin = New Thickness(8, 6, 0, 0)}

        '// Create Week value input fields
        W1Val = New CurrencyBox(80, True, AgnesBaseInput.FontSz.Medium,, CreditOnly, DebitOnly) With
            {.Margin = New Thickness(8, 6, 0, 0)}
        W2Val = New CurrencyBox(80, True, AgnesBaseInput.FontSz.Medium,, CreditOnly, DebitOnly) With
            {.Margin = New Thickness(6, 6, 0, 0)}
        W3Val = New CurrencyBox(80, True, AgnesBaseInput.FontSz.Medium,, CreditOnly, DebitOnly) With
            {.Margin = New Thickness(6, 6, 0, 0)}
        W4Val = New CurrencyBox(80, True, AgnesBaseInput.FontSz.Medium,, CreditOnly, DebitOnly) With
            {.Margin = New Thickness(6, 6, 0, 0)}
        W5Val = New CurrencyBox(80, True, AgnesBaseInput.FontSz.Medium,, CreditOnly, DebitOnly) With
            {.Margin = New Thickness(6, 6, 0, 0)}

        '// Create calculated fields (total, budget, variance, and percentages)
        'W1Val = New CurrencyBox(80, True, AgnesBaseInput.FontSz.Medium,, CreditOnly, DebitOnly) With
        '    {.Margin = New Thickness(8, 6, 0, 0)}


        If GroupIsSubTotal = True Then IsEnabled = False


        '    <Margin="8,6,0,0"/>
        '     Margin="6,6,0,0"/>
        '    Margin="6,6,0,0"/>
        '    Margin="6,6,0,0"/>
        '     Margin="6,6,0,0"/>
        '     Margin="6,6,0,0"/>
        '    Margin="6,6,0,0"/>
        '     Width="50" Margin="6,6,0,0"/>
        '     Width="80" Margin="6,6,0,0"/>
        '     Width="50" Margin="6,6,0,0"/>
        '     Width="70" Margin="6,6,0,0"/>
        '</DockPanel>

        With Children
            .Add(GroupLabel)
            .Add(DRR)
            .Add(W1Val)
            .Add(W2Val)
            .Add(W3Val)
            .Add(W4Val)
            .Add(W5Val)
        End With


        If GroupIsSubTotal = True Then
            For Each fg As ForecastGroup In SubtotalGroupList
                SubtotalGroups.Add(fg)
            Next
        End If

    End Sub
#End Region
End Class
