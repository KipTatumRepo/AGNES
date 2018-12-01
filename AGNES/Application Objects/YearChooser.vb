Imports System.ComponentModel
Public Class YearChooser
    Inherits DockPanel
    Implements INotifyPropertyChanged

#Region "Properties"
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Public SystemChange As Boolean
    Private _currentyear As Integer
    Private PeriodC As PeriodChooser
    Private MonthC As MonthChooser
    Public Property MinYear As Integer
    Public Property MaxYear As Integer
    Public Property DisableSelectAll As Boolean
    'Public Property RelatedPeriodObject As Object
    Public Property SelectedCount As Integer
    Public Property HeldYear As Integer
    Public Property CurrentYear As Integer
        Get
            Return _currentyear
        End Get
        Set(value As Integer)
            HeldYear = _currentyear
            _currentyear = value
            For Each b As Border In Children
                If b.Tag <> "Label" Then
                    Dim tb As TextBlock = b.Child
                    If FormatNumber(tb.Tag, 0) <> value Then
                        tb.Foreground = Brushes.LightGray
                        tb.FontSize = 12
                        tb.FontWeight = FontWeights.Normal
                    Else
                        tb.FontWeight = FontWeights.SemiBold
                        tb.Foreground = Brushes.Black
                        tb.FontSize = 18
                    End If
                End If
            Next
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(“Year”))
        End Set
    End Property

#End Region

#Region "Constructor"
    Public Sub New(ByRef RMPO As Object, MinY As Integer, MaxY As Integer, CurY As Integer)
        Dim ct As Integer, lbltext As String
        If TypeOf (RMPO) Is PeriodChooser Then
            PeriodC = RMPO
            lbltext = "  FY: "
        Else
            MonthC = RMPO
            lbltext = "  Year: "
        End If
        MinYear = MinY
        MaxYear = MaxY
        '// Create chooser label
        Dim BorderLabel As New Border With {.BorderBrush = Brushes.Black, .VerticalAlignment = VerticalAlignment.Center,
            .Name = "brdLabel", .Tag = "Label"}
        Dim TextLabel As New TextBlock With {.Text = lbltext, .TextAlignment = TextAlignment.Center,
        .HorizontalAlignment = HorizontalAlignment.Center, .FontSize = 12, .Name = "tbLabel", .Tag = "Label"}
        BorderLabel.Child = TextLabel
            Children.Add(BorderLabel)
        For ct = MinY To MaxY
            Dim brdYear As New Border With {.BorderBrush = Brushes.Black, .Width = 48, .VerticalAlignment = VerticalAlignment.Center,
            .Name = "brdY" & ct}
            Dim tbYear As New TextBlock With {.Text = ct, .TextAlignment = TextAlignment.Center, .HorizontalAlignment = HorizontalAlignment.Center,
                .FontSize = 12, .Tag = ct, .Name = "tbY" & ct}
            AddHandler brdYear.MouseEnter, AddressOf HoverOverYear
            AddHandler tbYear.MouseEnter, AddressOf HoverOverYear
            AddHandler brdYear.MouseLeave, AddressOf LeaveYear
            AddHandler tbYear.MouseLeave, AddressOf LeaveYear
            AddHandler tbYear.PreviewMouseDown, AddressOf ChooseYear
            brdYear.Child = tbYear
            Children.Add(brdYear)
        Next
        CurrentYear = CurY

    End Sub

#End Region

#Region "Public Methods"
    Public Sub Reset()
        CurrentYear = 0
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
    Private Sub HoverOverYear(sender As Object, e As MouseEventArgs)
        Dim tb As TextBlock
        If TypeOf (sender) Is TextBlock Then
            tb = sender
        Else
            Dim brd As Border = sender
            tb = brd.Child
        End If
        tb.FontSize = 20
    End Sub

    Private Sub LeaveYear(sender As Object, e As MouseEventArgs)
        Dim tb As TextBlock
        If TypeOf (sender) Is TextBlock Then
            tb = sender
        Else
            Dim brd As Border = sender
            tb = brd.Child
        End If
        If FormatNumber(tb.Tag, 0) <> CurrentYear Then
            tb.FontSize = 12
        Else
            tb.FontSize = 18
        End If
    End Sub

    Private Sub ChooseYear(sender As Object, e As MouseEventArgs)
        Dim tb As TextBlock
        If TypeOf (sender) Is TextBlock Then
            tb = sender
        Else
            Dim brd As Border = sender
            tb = brd.Child
        End If
        If (FormatNumber(tb.Tag, 0) <> CurrentYear Or DisableSelectAll = False) Then
            CurrentYear = FormatNumber(tb.Tag, 0)
        Else
            If CurrentYear <> 0 And DisableSelectAll = False Then Reset()
        End If
        If CurrentYear <> Now().Year Then
            If PeriodC Is Nothing Then
                MonthC.CurrentMonth = 1
                VendorSched.wkSched.Update(CurrentYear, MonthC.CurrentMonth, 1)
            Else
                PeriodC.CurrentPeriod = 1
                VendorSched.wkSched.Update(CurrentYear, 1, 1)
            End If
        Else
            If PeriodC Is Nothing Then
                MonthC.CurrentMonth = Now().Month
                VendorSched.Wk.CurrentWeek = GetCurrentCalendarWeek(Now())
                VendorSched.wkSched.Update(CurrentYear, MonthC.CurrentMonth, VendorSched.Wk.CurrentWeek)
            Else
                PeriodC.CurrentPeriod = GetCurrentPeriod(Now())
                VendorSched.Wk.CurrentWeek = GetCurrentWeek(Now())
                VendorSched.wkSched.Update(CurrentYear, PeriodC.CurrentPeriod, VendorSched.Wk.CurrentWeek)
            End If
        End If
    End Sub

#End Region


End Class
