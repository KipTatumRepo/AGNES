Public Class ScheduleStation
    Inherits Border

    'TODO: UPDATE VENDOR/LOCATION PREREQS (HOOD, ETC.)
#Region "Properties"
    Private BC As New BrushConverter
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
        Dim bc As New BrushConverter()
        Background = bc.ConvertFrom("#FFFBF1C6")
        Margin = New Thickness(1, 1, 1, 0)
        StationNumber = sn
        StationName = "Station " & StationNumber
        VendorStack = New StackPanel
        Child = VendorStack
        AddName()
    End Sub

#End Region

#Region "Public Methods"
    Public Sub Save()
        If VendorStack.Children.Count = 1 Then Exit Sub
        Dim vndr As VendorInStation = VendorStack.Children(1)
        Try
            Dim qee = (From e In VendorData.Schedules
                       Where e.ScheduleDate = CurrentWeekDay.DateValue And
                          e.Location = CurrentLocation.LocationName
                       Select e).ToList(0)

            With qee
                .ScheduleDate = CurrentWeekDay.DateValue
                .Location = CurrentLocation.LocationName
                .Station = StationName
                .VendorId = vndr.ReferencedVendor.VendorItem.PID
                .SavedBy = My.Settings.UserName
            End With
        Catch ex As Exception
            '// Save as new entry
            Dim ns As New Schedule
            With ns
                .ScheduleDate = CurrentWeekDay.DateValue
                .Location = CurrentLocation.LocationName
                .Station = StationName
                .VendorId = vndr.ReferencedVendor.VendorItem.PID
                .SavedBy = My.Settings.UserName
            End With
            VendorData.Schedules.Add(ns)
        End Try
    End Sub

    Public Sub DropVendorIntoStation(ByVal VendorName As String, ByRef RV As ScheduleVendor)
        Dim nv As New VendorInStation With {.TextAlignment = TextAlignment.Center, .Text = VendorName,
            .ReferencedVendor = RV, .ReferencedStation = Me, .FontSize = 12}
        nv.IsBrand = True
        nv.Background = Brushes.WhiteSmoke
        VendorStack.Children.Add(nv)
        nv.ReferencedVendor.UsedWeeklySlots += 1
        Height += 16
        VendorSched.SaveStatus = 0
        VendorSched.ActiveVendor = Nothing
        CurrentLocation.DraggingIntoStation = False
    End Sub

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
        CheckVendorDrag(e.Data.GetData(DataFormats.Text))
    End Sub

    Private Sub ScheduleStation_DragLeave(sender As Object, e As DragEventArgs) Handles Me.DragLeave
        CurrentLocation.DraggingIntoStation = False
        VendorSched.SaveStatus = VendorSched.SaveStatus
        VendorSched.tbSaveStatus.Text = StatusBarText
        VendorSched.tbSaveStatus.Background = StatusBarColor
        CurrentLocation.Background = BC.ConvertFrom("#FFBFE8F7")
    End Sub

    Private Sub ScheduleStation_Drop(sender As Object, e As DragEventArgs) Handles Me.Drop
        If DropAllowed = False Then
            VendorSched.SaveStatus = VendorSched.SaveStatus
            Exit Sub
        End If
        DropVendorIntoStation(e.Data.GetData(DataFormats.Text), VendorSched.ActiveVendor)
        CurrentLocation.Background = BC.ConvertFrom("#FFBFE8F7")
    End Sub

    Private Sub CheckVendorDrag(vn As String)
        '//Validation routines to preemptively notify about whether vendor is allowed to be scheduled
        CurrentLocation.DraggingIntoStation = True

        '// Check if vendor type is a brand (and allowed at a station)
        If IsVendorTypeAllowedAtStation() = False Then Exit Sub

        '// Check for the presence of a vendor in the station
        If IsStationAvailable() = False Then Exit Sub

        '// Check to ensure vendor prereqs are met at this station/location
        If AreVendorPrereqsMet() = False Then Exit Sub

        '// Check to make sure vendor placement does not exceed their max daily placements
        If DoesVendorHaveCapacity(vn) = False Then Exit Sub

        '// Check to see if the food type conflicts with the location's anchor station - Warning only
        If IsNoAnchorConflict() = False Then Exit Sub

        '// Check for the presence of the vendor in another station at the location - Warning only
        If IsVendorNotAlreadyAtLocation() = False Then Exit Sub

        '// Check to see if the food type is already present at the location on the same day - Warning only
        If IsNoSameDayFoodTypeConflictPresent() = False Then Exit Sub

        '// Check to see if the food type is present at the location on adjacent days - Warning only
        If IsNoAdjacentDayFoodTypeConflictPresent() = False Then Exit Sub


        DropAllowed = True
        VendorSched.tbSaveStatus.Text = "Okay to add"
        VendorSched.sbSaveStatus.Background = Brushes.LightGreen
        CurrentLocation.Background = Brushes.LightGreen
    End Sub

    Private Function IsVendorTypeAllowedAtStation()
        If VendorSched.ActiveVendor.VendorItem.VendorType = 3 Then  ' Food truck...not allowed in station
            VendorSched.tbSaveStatus.Text = "Food trucks cannot be added to stations"
            VendorSched.sbSaveStatus.Background = Brushes.PaleVioletRed
            DropAllowed = False
            Return False
        End If
        Return True
    End Function

    Private Function IsStationAvailable()
        If VendorStack.Children.Count > 1 Then
            VendorSched.tbSaveStatus.Text = "Only one vendor can be added to a station."
            VendorSched.sbSaveStatus.Background = Brushes.PaleVioletRed
            DropAllowed = False
            Return False
        End If
        Return True
    End Function

    Private Function AreVendorPrereqsMet()
        '// Conflicts such as the requirement for a hood at a unit that does not have one available
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
            DropAllowed = False
            Return False
        End If
        Return True
    End Function

    Private Function IsNoAnchorConflict()
        If (CurrentLocation.AnchorFoodSubType <> 0 And CurrentLocation.AnchorFoodSubType <> 10007) And
            (VendorSched.ActiveVendor.VendorItem.FoodSubType IsNot Nothing And
            VendorSched.ActiveVendor.VendorItem.FoodSubType <> 10007) Then
            If VendorSched.ActiveVendor.VendorItem.FoodSubType = CurrentLocation.AnchorFoodSubType Then
                With VendorSched
                    .tbSaveStatus.Text = "This vendor's food subtype conflicts with the anchor station at this location."
                    .sbSaveStatus.Background = Brushes.LightYellow
                End With
                DropAllowed = True
                Return False
            End If
        Else
            If VendorSched.ActiveVendor.FoodType = CurrentLocation.AnchorFoodType Then
                With VendorSched
                    .tbSaveStatus.Text = "This vendor's food type conflicts with the anchor station at this location."
                    .sbSaveStatus.Background = Brushes.LightYellow
                End With
                DropAllowed = True
                Return False
            End If
        End If
        Return True
    End Function

    Private Function IsVendorNotAlreadyAtLocation()
        For Each obj In CurrentLocation.StationStack.Children
            If TypeOf (obj) Is ScheduleStation Then
                Dim station As ScheduleStation = obj
                If station.VendorStack.Children.Count > 0 Then
                    Dim vndor As VendorInStation
                    For Each ooobj In station.VendorStack.Children
                        If TypeOf (ooobj) Is VendorInStation Then
                            vndor = ooobj
                            If vndor.ReferencedVendor.VendorItem.Name = VendorSched.ActiveVendor.VendorItem.Name Then
                                With VendorSched
                                    .tbSaveStatus.Text = "This vendor is already present at this location on this day."
                                    .sbSaveStatus.Background = Brushes.LightYellow
                                End With
                                DropAllowed = True
                                Return False
                            End If
                        End If
                    Next
                End If
            End If
        Next
        Return True
    End Function

    Private Function IsNoSameDayFoodTypeConflictPresent()
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
                            If vndor.ReferencedVendor.VendorItem.FoodSubType IsNot Nothing And VendorSched.ActiveVendor.VendorItem.FoodSubType IsNot Nothing Then

                                If vndor.ReferencedVendor.VendorItem.FoodSubType = VendorSched.ActiveVendor.VendorItem.FoodSubType Then
                                    With VendorSched
                                        .tbSaveStatus.Text = "This food subtype conflicts with another vendor present on the same day"
                                        .sbSaveStatus.Background = Brushes.LightYellow
                                    End With
                                    DropAllowed = True
                                    Return False
                                End If
                            Else
                                If vndor.ReferencedVendor.FoodType = VendorSched.ActiveVendor.FoodType Then
                                    With VendorSched
                                        .tbSaveStatus.Text = "This food type conflicts with another vendor present on the same day"
                                        .sbSaveStatus.Background = Brushes.LightYellow
                                    End With
                                    DropAllowed = True
                                    Return False
                                End If
                            End If
                        End If
                    Next
                End If
            End If
        Next


        Return True
    End Function

    Private Function IsNoAdjacentDayFoodTypeConflictPresent() As Boolean
        Dim CurrLocName As String = CurrentLocation.LocationName
        Dim CurrentDayIndex As Byte = VendorSched.wkSched.Children.IndexOf(CurrentWeekDay)
        Dim AdjacentDayOkay As Boolean = True

        If CurrentDayIndex > 0 Then AdjacentDayOkay = CheckAdjacentDay(CurrLocName, CurrentDayIndex - 1)
        If AdjacentDayOkay = False Then
            With VendorSched
                .tbSaveStatus.Text = "This food type or subtype is present at this location on the previous day"
                .sbSaveStatus.Background = Brushes.LightYellow
            End With
            DropAllowed = True
            Return False
        End If

        If CurrentDayIndex < 4 Then AdjacentDayOkay = CheckAdjacentDay(CurrLocName, CurrentDayIndex + 1)
        If AdjacentDayOkay = False Then
            With VendorSched
                .tbSaveStatus.Text = "This food type or subtype is present at this location the next day"
                .sbSaveStatus.Background = Brushes.LightYellow
            End With
            DropAllowed = True
            Return False
        End If

        Return True
    End Function

    Private Function CheckAdjacentDay(locationname, dayindex) As Boolean
        Dim AdjDay As ScheduleDay = VendorSched.wkSched.Children(dayindex)

        For Each locobj In AdjDay.LocationStack.Children
            If TypeOf (locobj) Is ScheduleLocation Then
                Dim loc As ScheduleLocation = locobj
                If loc.LocationName = locationname Then
                    For Each stationobj In loc.StationStack.Children
                        If TypeOf (stationobj) Is ScheduleStation Then
                            Dim station As ScheduleStation = stationobj
                            If station.VendorStack.Children.Count > 1 Then
                                Dim vndor As VendorInStation
                                For Each ooobj In station.VendorStack.Children
                                    If TypeOf (ooobj) Is VendorInStation Then
                                        vndor = ooobj
                                        If vndor.ReferencedVendor.VendorItem.FoodSubType IsNot Nothing And VendorSched.ActiveVendor.VendorItem.FoodSubType IsNot Nothing Then
                                            If vndor.ReferencedVendor.VendorItem.FoodSubType = VendorSched.ActiveVendor.VendorItem.FoodSubType Then Return False
                                        Else
                                            If vndor.ReferencedVendor.FoodType = VendorSched.ActiveVendor.FoodType Then Return False
                                        End If
                                    End If
                                Next
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
