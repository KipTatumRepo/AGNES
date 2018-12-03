Imports System.ComponentModel
Public Class MonthChooser
    Inherits DockPanel
    Implements INotifyPropertyChanged

#Region "Properties"
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Public SystemChange As Boolean
    Private _currentmonth As Byte
    Private Week As WeekChooser
    Public Property MinMonth As Byte
    Public Property MaxMonth As Byte
    Public Property DisableSelectAll As Boolean
    Public Property RelatedWeekObject As Object
    Public Property SelectedCount As Byte
    Public Property HeldMonth As Byte
    Public Property CurrentMonth As Byte
        Get
            Return _currentmonth
        End Get
        Set(value As Byte)
            HeldMonth = _currentmonth
            _currentmonth = value
            For Each b As Border In Children
                If b.Tag <> "Label" Then
                    Dim tb As TextBlock = b.Child
                    If FormatNumber(tb.Tag, 0) <> value Then
                        tb.Foreground = Brushes.LightGray
                        tb.FontSize = 14
                        tb.FontWeight = FontWeights.Normal
                    Else
                        tb.FontWeight = FontWeights.SemiBold
                        tb.Foreground = Brushes.Black
                        tb.FontSize = 20
                    End If
                End If
            Next
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(“Month”))
        End Set
    End Property

#End Region

#Region "Constructor"
    Public Sub New(ByRef RelatedWeekObject As WeekChooser, MinM As Byte, MaxM As Byte, CurM As Byte)
        Dim ct As Byte
        Week = RelatedWeekObject
        MinMonth = MinM
        MaxMonth = MaxM
        '// Create chooser label
        Dim BorderLabel As New Border With {.BorderBrush = Brushes.Black, .VerticalAlignment = VerticalAlignment.Center,
            .Name = "brdLabel", .Tag = "Label"}
        Dim TextLabel As New TextBlock With {.Text = "  Month: ", .TextAlignment = TextAlignment.Center,
        .HorizontalAlignment = HorizontalAlignment.Center, .FontSize = 12, .Name = "tbLabel", .Tag = "Label"}
        BorderLabel.Child = TextLabel
        Children.Add(BorderLabel)
        For ct = 1 To 12
            Dim brdMonth As New Border With {.BorderBrush = Brushes.Black, .Width = 40, .VerticalAlignment = VerticalAlignment.Center,
            .Name = "brdM" & ct}
            Dim tbMonth As New TextBlock With {.Text = MonthName(ct, True), .TextAlignment = TextAlignment.Center, .HorizontalAlignment = HorizontalAlignment.Center,
                .FontSize = 14, .Tag = ct, .Name = "tbM" & ct}
            If (ct < MinMonth) Or (ct > MaxMonth) Then brdMonth.IsEnabled = False
            AddHandler brdMonth.MouseEnter, AddressOf HoverOverMonth
            AddHandler tbMonth.MouseEnter, AddressOf HoverOverMonth
            AddHandler brdMonth.MouseLeave, AddressOf LeaveMonth
            AddHandler tbMonth.MouseLeave, AddressOf LeaveMonth
            AddHandler tbMonth.PreviewMouseDown, AddressOf ChooseMonth
            brdMonth.Child = tbMonth
            Children.Add(brdMonth)
        Next
        CurrentMonth = CurM
    End Sub

#End Region

#Region "Public Methods"
    Public Sub Reset()
        CurrentMonth = 0
        For Each brd As Border In Children
            Dim tb As TextBlock = brd.Child
            If brd.Tag <> "Label" Then
                tb.Foreground = Brushes.Black
                tb.FontSize = 14
                tb.FontWeight = FontWeights.SemiBold
            End If
        Next
    End Sub
#End Region

#Region "Private Methods"
    Private Sub HoverOverMonth(sender As Object, e As MouseEventArgs)
        Dim tb As TextBlock
        If TypeOf (sender) Is TextBlock Then
            tb = sender
        Else
            Dim brd As Border = sender
            tb = brd.Child
        End If
        tb.FontSize = 20
    End Sub

    Private Sub LeaveMonth(sender As Object, e As MouseEventArgs)
        Dim tb As TextBlock
        If TypeOf (sender) Is TextBlock Then
            tb = sender
        Else
            Dim brd As Border = sender
            tb = brd.Child
        End If
        If FormatNumber(tb.Tag, 0) <> CurrentMonth Then
            tb.FontSize = 14
        Else
            tb.FontSize = 18
        End If
    End Sub

    Private Sub ChooseMonth(sender As Object, e As MouseEventArgs)
        Dim tb As TextBlock
        If TypeOf (sender) Is TextBlock Then
            tb = sender
        Else
            Dim brd As Border = sender
            tb = brd.Child
        End If
        If (FormatNumber(tb.Tag, 0) <> CurrentMonth Or DisableSelectAll = False) Then
            CurrentMonth = FormatNumber(tb.Tag, 0)
        Else
            If CurrentMonth <> 0 And DisableSelectAll = False Then Reset()
        End If
        If CurrentMonth <> Now().Month Then
            Week.MaxWeek = GetMaxCalendarWeeks(CurrentMonth)
            Week.CurrentWeek = 1
        Else
            Week.MaxWeek = GetCurrentCalendarWeek(FormatDateTime(Now(), DateFormat.ShortDate))
            Week.CurrentWeek = GetCurrentCalendarWeek(FormatDateTime(Now(), DateFormat.ShortDate))
        End If
        Week.EnableWeeks()
    End Sub

#End Region

End Class
