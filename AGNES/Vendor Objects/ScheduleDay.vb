Imports System.ComponentModel
Public Class ScheduleDay
    Inherits Border

#Region "Properties"
    Public Property DateValue As Date
    Public Property IsHoliday As Boolean
    Public LocationScrollViewer As ScrollViewer
    Public LocationStack As StackPanel
    Private Highlight As Boolean
#End Region

#Region "Constructor"
    Public Sub New(dt, hol)
        DateValue = dt
        IsHoliday = hol
        BorderThickness = New Thickness(1, 1, 1, 1)
        BorderBrush = Brushes.Black
        Width = 198
        LocationScrollViewer = New ScrollViewer
        LocationStack = New StackPanel With {.CanVerticallyScroll = True}
        CreateDayLabel()
        If hol = 0 Then
            LoadAndAddLocations()
        End If
        LocationScrollViewer.Content = LocationStack
        Child = LocationScrollViewer

    End Sub

#End Region

#Region "Public Methods"

#End Region

#Region "Private Methods"
    Private Sub CreateDayLabel()
        Dim brd As New Border With {.Background = Brushes.Black, .Width = 178, .Height = 50, .HorizontalAlignment = HorizontalAlignment.Left,
            .VerticalAlignment = VerticalAlignment.Top}
        Dim tblk As New TextBlock With {.TextWrapping = TextWrapping.Wrap, .FontSize = 14, .TextAlignment = TextAlignment.Center,
            .Foreground = Brushes.White, .Text = FormatDateTime(DateValue, DateFormat.LongDate)}
        If IsHoliday = True Then
            tblk.Background = Brushes.DarkGray
        Else
            tblk.Background = Brushes.Black
        End If
        brd.Child = tblk
        LocationStack.Children.Add(brd)
    End Sub

    Private Sub LoadAndAddLocations()
        Dim x As Byte
        Dim singlelocs() As String = {"4", "16", "26", "34", "37", "41", "43", "50", "83", "112", "121",
            "CCP", "LS", "Millennium", "RTC", "Samm-C", "Studio H"}
        For x = 1 To singlelocs.Count
            Dim newloc As New ScheduleLocation(singlelocs(x - 1), 1, Me, Highlight)
            LocationStack.Children.Add(newloc)
            Highlight = Not Highlight
        Next

        Dim doublelocs() As String = {"86", "Redwest", "31"}
        For x = 1 To doublelocs.Count
            Dim newloc As New ScheduleLocation(doublelocs(x - 1), 2, Me, Highlight)
            LocationStack.Children.Add(newloc)
            Highlight = Not Highlight
        Next

        Dim triplelocs() As String = {"Advanta"}
        For x = 1 To triplelocs.Count
            Dim newloc As New ScheduleLocation(triplelocs(x - 1), 3, Me, Highlight)
            LocationStack.Children.Add(newloc)
            Highlight = Not Highlight
        Next

    End Sub

#End Region

End Class
