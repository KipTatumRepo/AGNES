Imports System.ComponentModel

Public Class PeriodChooser
    Inherits DockPanel
    Implements INotifyPropertyChanged
    Private _currentperiod As Byte
    Private Week As WeekChooser
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Public Property HeldPeriod As Byte
    Public Property CurrentPeriod As Byte
        Get
            Return _currentperiod
        End Get
        Set(value As Byte)
            HeldPeriod = _currentperiod
            _currentperiod = value
            For Each b As Border In Children
                If b.Tag <> "Label" Then
                    Dim tb As TextBlock = b.Child
                    If FormatNumber(tb.Text, 0) <> value Then
                        tb.Foreground = Brushes.LightGray
                        tb.FontSize = 16
                        tb.FontWeight = FontWeights.Normal
                    Else
                        tb.FontWeight = FontWeights.SemiBold
                        tb.Foreground = Brushes.Black
                        tb.FontSize = 24
                    End If
                End If
            Next
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(“Period”))
        End Set
    End Property
    Public Property MinPeriod As Byte
    Public Property MaxPeriod As Byte
    Public SelectAllEnabled As Boolean
    Public RelatedWeekObject As Object
    Public Property SelectedCount As Byte
    Public Sub New(ByRef RelatedWeekObject As WeekChooser, MinP As Byte, MaxP As Byte, CurP As Byte)
        Dim ct As Byte
        Week = RelatedWeekObject
        MinPeriod = MinP
        MaxPeriod = MaxP
        '// Create chooser label
        Dim BorderLabel As New Border With {.BorderBrush = Brushes.Black, .VerticalAlignment = VerticalAlignment.Center,
            .Name = "brdLabel", .Tag = "Label"}
        Dim TextLabel As New TextBlock With {.Text = "  MS Period: ", .TextAlignment = TextAlignment.Center,
        .HorizontalAlignment = HorizontalAlignment.Center, .FontSize = 16, .Name = "tbLabel", .Tag = "Label"}
        BorderLabel.Child = TextLabel
        Children.Add(BorderLabel)
        For ct = 1 To 12
            Dim brdPeriod As New Border With {.BorderBrush = Brushes.Black, .Width = 32, .VerticalAlignment = VerticalAlignment.Center,
            .Name = "brdP" & ct}
            Dim tbPeriod As New TextBlock With {.Text = ct, .TextAlignment = TextAlignment.Center, .HorizontalAlignment = HorizontalAlignment.Center,
                .FontSize = 16, .Tag = ct, .Name = "tbP" & ct}
            If (ct < MinPeriod) Or (ct > MaxPeriod) Then brdPeriod.IsEnabled = False
            AddHandler brdPeriod.MouseEnter, AddressOf HoverOverPeriod
            AddHandler tbPeriod.MouseEnter, AddressOf HoverOverPeriod
            AddHandler brdPeriod.MouseLeave, AddressOf LeavePeriod
            AddHandler tbPeriod.MouseLeave, AddressOf LeavePeriod
            AddHandler tbPeriod.PreviewMouseDown, AddressOf ChoosePeriod
            brdPeriod.Child = tbPeriod
            Children.Add(brdPeriod)
        Next
        CurrentPeriod = CurP
    End Sub

    Private Sub HoverOverPeriod(sender As Object, e As MouseEventArgs)
        Dim tb As TextBlock
        If TypeOf (sender) Is TextBlock Then
            tb = sender
        Else
            Dim brd As Border = sender
            tb = brd.Child
        End If
        tb.FontSize = 30
    End Sub

    Private Sub LeavePeriod(sender As Object, e As MouseEventArgs)
        Dim tb As TextBlock
        If TypeOf (sender) Is TextBlock Then
            tb = sender
        Else
            Dim brd As Border = sender
            tb = brd.Child
        End If
        If FormatNumber(tb.Tag, 0) <> CurrentPeriod Then
            tb.FontSize = 16
        Else
            tb.FontSize = 24
        End If
    End Sub

    Private Sub ChoosePeriod(sender As Object, e As MouseEventArgs)
        Dim tb As TextBlock
        If TypeOf (sender) Is TextBlock Then
            tb = sender
        Else
            Dim brd As Border = sender
            tb = brd.Child
        End If
        If (FormatNumber(tb.Tag, 0) <> CurrentPeriod Or SelectAllEnabled = False) Then
            CurrentPeriod = FormatNumber(tb.Tag, 0)
        Else
            If CurrentPeriod <> 0 Then Reset()
        End If
        If CurrentPeriod < MaxPeriod Then
            Week.MaxWeek = GetMaxWeeks(CurrentPeriod)
        Else
            Week.MaxWeek = GetCurrentWeek(FormatDateTime(Now(), DateFormat.ShortDate))
        End If
        Week.EnableWeeks()
        Week.CurrentWeek = 1
    End Sub

    Public Sub Reset()
        CurrentPeriod = 0
        For Each brd As Border In Children
            Dim tb As TextBlock = brd.Child
            If brd.Tag <> "Label" Then
                tb.Foreground = Brushes.Black
                tb.FontSize = 16
                tb.FontWeight = FontWeights.SemiBold
            End If
        Next
    End Sub

End Class
