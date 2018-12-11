Public Class ScheduleTruckStation
    Inherits Border

#Region "Properties"
    Public TruckStack As StackPanel
    Public Property TruckName As String
    'Public Property StationNumber As Byte
    Public Property TruckBlock As TextBlock
    Private Property DropAllowed As Boolean = False
    Public Property CurrentWeekDay As ScheduleDay
    Public Property CurrentLocation As ScheduleLocation
    Private StatusBarText As String
    Private _statusbarcolor As SolidColorBrush
    Private Property StatusBarColor As SolidColorBrush
        Get
            Return _statusbarcolor
        End Get
        Set(value As SolidColorBrush)
            _statusbarcolor = value
            VendorSched.sbSaveStatus.Background = value
        End Set
    End Property

#End Region

#Region "Constructor"
    Public Sub New(tnm As String, ByRef cwd As ScheduleDay, ByRef cloc As ScheduleLocation)
        CurrentWeekDay = cwd
        CurrentLocation = cloc
        AllowDrop = True
        Height = 32
        BorderBrush = Brushes.Black
        BorderThickness = New Thickness(1, 1, 1, 1)
        Margin = New Thickness(1, 1, 1, 0)
        'StationNumber = sn
        TruckName = tnm
        TruckStack = New StackPanel
        Child = TruckStack
        AddName()
    End Sub

#End Region

#Region "Public Methods"
    Public Sub Save()
        If TruckStack.Children.Count = 1 Then Exit Sub
        Dim vndr As VendorInStation = TruckStack.Children(1)
        Dim ns As New Schedule
        With ns
            .ScheduleDate = CurrentWeekDay.DateValue
            .Location = CurrentLocation.LocationName
            .Station = "Truck"
            .VendorId = vndr.ReferencedVendor.VendorItem.PID
            .SavedBy = My.Settings.UserName
        End With
        VendorData.Schedules.Add(ns)

    End Sub

#End Region

#Region "Private Methods"
    Private Sub AddName()
        TruckBlock = New TextBlock With {.TextAlignment = TextAlignment.Center, .Text = "Food Truck", .FontSize = 10}
        TruckStack.Children.Add(TruckBlock)
    End Sub

#End Region

End Class
