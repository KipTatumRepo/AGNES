Imports System.ComponentModel
Public Class ScheduleDay
    Inherits Border

#Region "Properties"
    Public Property DateValue As Date
    Public Property IsHoliday As Boolean
    Public LocationScrollViewer As ScrollViewer
    Public LocationStack As StackPanel
    Private Highlight As Boolean
    Private SystemCall As Boolean
#End Region

#Region "Constructor"
    Public Sub New(dt, hol)
        DateValue = dt
        IsHoliday = hol
        BorderThickness = New Thickness(1, 1, 1, 1)
        BorderBrush = Brushes.Black
        Width = 198
        SystemCall = True
        LocationScrollViewer = New ScrollViewer
        AddHandler LocationScrollViewer.ScrollChanged, AddressOf ScrollView
        LocationStack = New StackPanel With {.CanVerticallyScroll = True}
        CreateDayLabel()
        If hol = 0 Then
            LoadAndAddLocations()
        End If
        LocationScrollViewer.Content = LocationStack
        Child = LocationScrollViewer
        SystemCall = False
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

        'TODO: THIS CODE WILL BE REPLACED BY A QUERY ON THE FoodTruckSite FIELD IN THE FINAL NEW LOCATIONS TABLE
        Dim Loc92 As New ScheduleLocation("92 (Trucks Only)", 0, Me, Highlight)
        Loc92.AllowsFoodTrucks = True
        LocationStack.Children.Add(Loc92)
        Highlight = Not Highlight

        Dim LocX As New ScheduleLocation("Studio X (Trucks Only)", 0, Me, Highlight)
        LocX.AllowsFoodTrucks = True
        LocationStack.Children.Add(LocX)
        Highlight = Not Highlight

        Dim Loc32 As New ScheduleLocation("32 (Trucks Only)", 0, Me, Highlight)
        Loc32.AllowsFoodTrucks = True
        LocationStack.Children.Add(Loc32)
        Highlight = Not Highlight

        'TODO: THIS CODE WILL BE REPLACED BY A QUERY ON THE STATION COUNT FIELD IN THE FINAL NEW LOCATIONS TABLE
        Dim x As Byte
        Dim singlelocs() As String = {"4", "16", "26", "34", "37", "41", "50", "83", "112", "121",
            "CCP", "LS", "RTC", "Samm-C", "Studio H"}
        For x = 1 To singlelocs.Count
            Dim newloc As New ScheduleLocation(singlelocs(x - 1), 1, Me, Highlight)
            LocationStack.Children.Add(newloc)
            Highlight = Not Highlight
        Next

        Dim Loc43 As New ScheduleLocation("43", 1, Me, Highlight)
        Loc43.AllowsFoodTrucks = True
        LocationStack.Children.Add(Loc43)
        Highlight = Not Highlight

        Dim LocMill As New ScheduleLocation("Millennium", 1, Me, Highlight)
        LocMill.AllowsFoodTrucks = True
        LocationStack.Children.Add(LocMill)
        Highlight = Not Highlight

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

    Private Sub ScrollView(sender As ScrollViewer, e As ScrollChangedEventArgs)
        If SystemCall = True Then Exit Sub
        VendorSched.wkSched.SyncScrollViews(Me, sender.VerticalOffset)
    End Sub

#End Region

End Class
