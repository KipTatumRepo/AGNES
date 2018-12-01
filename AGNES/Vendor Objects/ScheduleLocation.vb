Imports System.ComponentModel
Public Class ScheduleLocation
    Inherits Border

#Region "Properties"
    Public StationStack As StackPanel
    Public Property LocationName As String
    Public Property LocationBlock As TextBlock
    Public Property StationCount As Byte
    Private Property HighlightColor As Boolean = True
    Public Property CurrentWeekDay As ScheduleDay
    Private Property DropAllowed As Boolean = True
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
    Public Sub New(locname As String, sc As Byte, ByRef cwd As ScheduleDay)
        CurrentWeekDay = cwd
        StationCount = sc
        BorderBrush = Brushes.Black
        BorderThickness = New Thickness(1, 1, 1, 1)
        Margin = New Thickness(1, 1, 1, 0)
        LocationName = locname
        StationStack = New StackPanel
        Child = StationStack
        AllowDrop = True
        AddName()
        AddStations()
    End Sub

#End Region

#Region "Public Methods"

#End Region

#Region "Private Methods"

    Private Sub AddName()
        LocationBlock = New TextBlock With {.TextAlignment = TextAlignment.Center, .Text = LocationName}
        StationStack.Children.Add(LocationBlock)
    End Sub

    Private Sub AddStations()
        For x As Byte = 1 To StationCount
            Dim station As New ScheduleStation(x, CurrentWeekDay, Me)
            StationStack.Children.Add(station)
        Next
    End Sub

    Private Sub ScheduleLocation_DragEnter(sender As Object, e As DragEventArgs) Handles Me.DragEnter
        StatusBarText = VendorSched.tbSaveStatus.Text
        CheckVendorDrag(e.Data.GetData(DataFormats.Text))
    End Sub

    Private Sub ScheduleLocation_DragLeave(sender As Object, e As DragEventArgs) Handles Me.DragLeave
        VendorSched.tbSaveStatus.Text = StatusBarText
        VendorSched.sbSaveStatus.Background = StatusBarColor
    End Sub

    Private Sub ScheduleLocation_Drop(sender As Object, e As DragEventArgs) Handles Me.Drop
        If DropAllowed = False Then
            VendorSched.tbSaveStatus.Text = StatusBarText
            VendorSched.sbSaveStatus.Background = StatusBarColor
            Exit Sub
        End If

        Dim tb As New ScheduleTruckStation(VendorSched.ActiveVendor.VendorItem.Name, CurrentWeekDay, Me)
        StationStack.Children.Add(tb)
        'Dim nv As New VendorInStation With {.TextAlignment = TextAlignment.Center, .Text = e.Data.GetData(DataFormats.Text),
        '.ReferencedVendor = VendorSched.ActiveVendor, .ReferencedLocation = Me, .FontSize = 12}
        'nv.Background = Brushes.LightGray
        'VendorStack.Children.Add(nv)
        'nv.ReferencedVendor.UsedWeeklySlots += 1
        'Height += 16
        'VendorSched.tbSaveStatus.Text = "Changes Not Saved"
        'StatusBarColor = Brushes.Red
        'VendorSched.ActiveVendor = Nothing
    End Sub

    Private Sub CheckVendorDrag(vn As String)
        'Validation routines to preemptively notify about whether vendor is allowed to be scheduled
        DropAllowed = True
        VendorSched.tbSaveStatus.Text = "Okay to add"
        VendorSched.sbSaveStatus.Background = Brushes.LightGreen

        If IsVendorTypeAllowedAtBuilding() = False Then    '//     Check if vendor type (truck or brand) is allowed at building
            DropAllowed = False
            Exit Sub
        End If

        'If IsStationAvailable() = False Then          '//     Check for the presence of a vendor in the station
        '    DropAllowed = False
        '    Exit Sub
        'End If

        'If AreVendorPrereqsMet() = False Then
        '    DropAllowed = False
        '    Exit Sub
        'End If

        'If DoesVendorHaveCapacity(vn) = False Then
        '    DropAllowed = False
        '    Exit Sub
        'End If

        'If IsNoFoodTypeConflictPresent() = False Then
        '    DropAllowed = False
        '    Exit Sub
        'End If


    End Sub

    Private Function IsVendorTypeAllowedAtBuilding()
        If VendorSched.ActiveVendor.VendorItem.VendorType = 2 Then
            VendorSched.tbSaveStatus.Text = StatusBarText
            VendorSched.tbSaveStatus.Background = StatusBarColor
            Return False
        End If

        Return True
    End Function

#End Region

End Class
