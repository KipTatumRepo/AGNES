Imports System.ComponentModel
Public Class ScheduleDay
    Inherits Border

#Region "Properties"
    Public Property DateValue As Date
    Public Property IsHoliday As Boolean
    Public LocationStack As StackPanel
#End Region

#Region "Constructor"
    Public Sub New(dt, hol)
        DateValue = dt
        IsHoliday = hol
        BorderThickness = New Thickness(1, 1, 1, 1)
        BorderBrush = Brushes.Black
        Height = 658
        Width = 202
        LocationStack = New StackPanel
        Child = LocationStack
        CreateDayLabel()
        If hol = 0 Then LoadAndAddLocations()
    End Sub

#End Region

#Region "Public Methods"

#End Region

#Region "Private Methods"
    Private Sub CreateDayLabel()
        Dim brd As New Border With {.Background = Brushes.Black, .Width = 200, .Height = 50, .HorizontalAlignment = HorizontalAlignment.Left,
            .VerticalAlignment = VerticalAlignment.Top}
        Dim tblk As New TextBlock With {.TextWrapping = TextWrapping.Wrap, .FontSize = 18, .TextAlignment = TextAlignment.Center,
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
        'Dim locs(20) As String = {"4", "9", "16", "25", "26", "31", "34", "36"}
        Dim newloc As New ScheduleLocation("Building 92", 1)
        LocationStack.Children.Add(newloc)
        Dim newloc2 As New ScheduleLocation("Cafe 16", 2)
        LocationStack.Children.Add(newloc2)
    End Sub

#End Region

End Class
