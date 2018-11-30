Public Class ScheduleStation
    Inherits Border

#Region "Properties"
    Public VendorStack As StackPanel
    Public Property StationName As String
    Public Property StationNumber As Byte
    Public Property StationBlock As TextBlock
    Private Property DropAllowed As Boolean = True
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
    Public Sub New(sn As Byte, ByRef cwd As ScheduleDay, ByRef cloc As ScheduleLocation)
        CurrentWeekDay = cwd
        CurrentLocation = cloc
        AllowDrop = True
        Height = 16
        BorderBrush = Brushes.Black
        BorderThickness = New Thickness(1, 1, 1, 1)
        Margin = New Thickness(1, 1, 1, 0)
        StationNumber = sn
        StationName = "Station " & StationNumber
        VendorStack = New StackPanel
        Child = VendorStack
        AddName()
    End Sub

#End Region

#Region "Public Methods"
    Public Sub DeleteItem(ByRef v As VendorInStation)
        VendorStack.Children.Remove(v)
        Height -= 16
    End Sub

#End Region

#Region "Private Methods"
    Private Sub AddName()
        StationBlock = New TextBlock With {.TextAlignment = TextAlignment.Center, .Text = StationName, .FontSize = 10}
        VendorStack.Children.Add(StationBlock)
    End Sub

    Private Sub ScheduleStation_DragEnter(sender As Object, e As DragEventArgs) Handles Me.DragEnter
        StatusBarText = VendorSched.tbSaveStatus.Text
        CheckVendorDrag(e.Data.GetData(DataFormats.Text))
    End Sub

    Private Sub ScheduleStation_DragLeave(sender As Object, e As DragEventArgs) Handles Me.DragLeave
        VendorSched.tbSaveStatus.Text = StatusBarText
        VendorSched.sbSaveStatus.Background = StatusBarColor
    End Sub

    Private Sub ScheduleStation_Drop(sender As Object, e As DragEventArgs) Handles Me.Drop
        If DropAllowed = False Then
            VendorSched.tbSaveStatus.Text = StatusBarText
            VendorSched.sbSaveStatus.Background = StatusBarColor
            Exit Sub
        End If

        Dim nv As New VendorInStation With {.TextAlignment = TextAlignment.Center, .Text = e.Data.GetData(DataFormats.Text),
        .ReferencedVendor = VendorSched.ActiveVendor, .ReferencedLocation = Me, .FontSize = 12}
        nv.Background = Brushes.LightGray
        VendorStack.Children.Add(nv)
        nv.ReferencedVendor.UsedWeeklySlots += 1
        Height += 16
        VendorSched.tbSaveStatus.Text = "Changes Not Saved"
        StatusBarColor = Brushes.Red
        VendorSched.ActiveVendor = Nothing
    End Sub

    Private Sub CheckVendorDrag(vn As String)
        'Validation routines to preemptively notify about whether vendor is allowed to be scheduled
        VendorSched.tbSaveStatus.Text = "Okay to add"
        VendorSched.sbSaveStatus.Background = Brushes.LightGreen
        If IsStationAvailable() = False Then          '//     Check for the presence of a vendor in the station
            DropAllowed = False
            Exit Sub
        End If

        If IsVendorTypeAllowedAtStation() = False Then    '//     Check if vendor type (truck or brand) is allowed at station
            DropAllowed = False
            Exit Sub
        End If

        If IsVendorTypeAllowedAtBuilding() = False Then    '//     Check if vendor type (truck or brand) is allowed at building
            DropAllowed = False
            Exit Sub
        End If

        If AreVendorPrereqsMet() = False Then
            DropAllowed = False
            Exit Sub
        End If

        If DoesVendorHaveCapacity(vn) = False Then
            DropAllowed = False
            Exit Sub
        End If

        If IsNoFoodTypeConflictPresent() = False Then
            DropAllowed = False
            Exit Sub
        End If


    End Sub

    Private Function IsStationAvailable()
        If VendorStack.Children.Count > 1 Then
            VendorSched.tbSaveStatus.Text = "Only one vendor can be added to a station."
            VendorSched.sbSaveStatus.Background = Brushes.PaleVioletRed
            Return False
        End If
        Return True
    End Function

    Private Function IsVendorTypeAllowedAtStation()
        '// Is the vendor type (truck or brand) allowed at the station?
        '// Check locations table; if vendor is a brand, confirm the station field has a value greater than zero
        '// No additional logic identified yet (11/27)

        Return True
    End Function

    Private Function IsVendorTypeAllowedAtBuilding()
        '// Is the vendor type (truck or brand) allowed at the building?

        '// Previous check (Allowed@Station) will rule out brands disallowed at building
        '// For trucks, check locations table for a TRUE value in the FoodTrucks field

        Return True
    End Function

    Private Function DoesVendorHaveCapacity(vn As String)
        '// Would adding the vendor exceed the max number of daily locations the vendor can support?
        Dim CountCurrentVendorDeployments As Byte = 1
        Dim AssessLocation As ScheduleLocation
        For Each obj In CurrentWeekDay.LocationStack.Children
            If TypeOf (obj) Is ScheduleLocation Then
                AssessLocation = obj
                Dim station As ScheduleStation
                For Each oobj In AssessLocation.StationStack.Children
                    If TypeOf (oobj) Is ScheduleStation Then
                        station = oobj
                        If station.VendorStack.Children.Count > 0 Then
                            Dim vndor As VendorInStation
                            For Each ooobj In station.VendorStack.Children
                                If TypeOf (ooobj) Is VendorInStation Then
                                    vndor = ooobj
                                    If vndor.Text = vn Then CountCurrentVendorDeployments += 1
                                End If
                            Next
                        End If
                    End If

                Next
            End If
        Next
        If CountCurrentVendorDeployments > VendorSched.ActiveVendor.MaxDailySlots Then
            VendorSched.tbSaveStatus.Text = "Vendor has reached the maximum number of cafes per day."
            VendorSched.sbSaveStatus.Background = Brushes.PaleVioletRed
            Return False
        End If
        Return True
    End Function

    Private Function AreVendorPrereqsMet()
        '// Conflicts such as the requirement for a hood at a unit that does not have one available
        Return True
    End Function

    Private Function IsNoFoodTypeConflictPresent()
        '// Cautionary alert if food type conflicts with an anchor station, another vendor present at the same time, or a food
        '// type scheduled on adjacent days (the last one should be fun to code. :| )

        For Each obj In CurrentLocation.StationStack.Children
            If TypeOf (obj) Is ScheduleStation Then
                Dim station As ScheduleStation = obj
                If station.VendorStack.Children.Count > 0 Then
                    Dim vndor As VendorInStation
                    For Each ooobj In station.VendorStack.Children
                        If TypeOf (ooobj) Is VendorInStation Then
                            vndor = ooobj
                            If vndor.ReferencedVendor.FoodType = VendorSched.ActiveVendor.FoodType Then
                                With VendorSched
                                    .tbSaveStatus.Text = "This food type conflicts with another vendor present on the same day"
                                    .sbSaveStatus.Background = Brushes.LightYellow
                                End With
                            End If
                        End If
                    Next
                End If
            End If
        Next
        Return True
    End Function

#End Region

End Class
