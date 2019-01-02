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
    Public Function ActiveLocationList() As List(Of ScheduleLocation)
        Dim ReturnList As New List(Of ScheduleLocation)
        Dim ActiveLocation As ScheduleLocation
        Dim activestation As ScheduleStation
        For Each l In LocationStack.Children
            If TypeOf (l) Is ScheduleLocation Then
                ActiveLocation = l
                For Each s In ActiveLocation.StationStack.Children
                    If TypeOf (s) Is ScheduleStation Then
                        activestation = s
                        For Each v In activestation.VendorStack.Children
                            If TypeOf (v) Is VendorInStation Then
                                If ReturnList.Count > 0 Then
                                    If ReturnList.Item(ReturnList.Count - 1) IsNot ActiveLocation Then ReturnList.Add(ActiveLocation)
                                Else
                                    ReturnList.Add(ActiveLocation)
                                End If
                                Exit For
                            End If
                        Next
                    End If
                Next
            End If
        Next
        Return ReturnList
    End Function

    Public Function ActiveLocationList(refvend As ScheduleVendor) As List(Of ScheduleLocation)
        Dim ReturnList As New List(Of ScheduleLocation)
        Dim activelocation As ScheduleLocation
        Dim activestation As ScheduleStation
        Dim activevendor As VendorInStation
        For Each l In LocationStack.Children
            If TypeOf (l) Is ScheduleLocation Then
                activelocation = l
                For Each s In activelocation.StationStack.Children
                    If TypeOf (s) Is ScheduleStation Then
                        activestation = s
                        For Each v In activestation.VendorStack.Children
                            If TypeOf (v) Is VendorInStation Then
                                activevendor = v
                                If ReturnList.Count > 0 Then
                                    If ReturnList.Item(ReturnList.Count - 1) IsNot activelocation Then
                                        If activevendor.ReferencedVendor Is refvend Then ReturnList.Add(activelocation)
                                    End If
                                Else
                                    If activevendor.ReferencedVendor Is refvend Then ReturnList.Add(activelocation)
                                End If
                                Exit For
                            End If
                        Next
                    End If
                Next
            End If
        Next
        Return ReturnList
    End Function

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
        Dim TrucksToo As Boolean, StationCount As Byte
        ' Load truck only locations first
        Dim qtl = From tl As Building In SharedDataGroup.Buildings
                  Where tl.AllowFoodTrucks = True
                  Select tl

        ' For each building that allows trucks, see if it's a cafe building with stations.  If it is, ignore it.  If not,
        ' add a truck-only location
        For Each tl In qtl
            Dim qhs = From hs As Cafe In SharedDataGroup.Cafes
                      Where hs.BldgId = tl.PID
                      Select hs

            If qhs.Count = 0 Then
                AddTruckOnlyLocation(tl.BldgName)
            Else
                For Each hs In qhs
                    If hs.BrandStations = 0 Then AddTruckOnlyLocation(tl.BldgName)
                Next
            End If
        Next

        ' Load single station locations next
        Dim qss1 = From ss As Cafe In SharedDataGroup.Cafes
                   Where ss.BrandStations = 1
                   Select ss

        ' Determine if trucks are also allowed, then add the station
        For Each ss In qss1
            TrucksToo = False
            For Each tl In qtl
                If ss.BldgId = tl.PID Then TrucksToo = True
            Next
            AddRegularLocation(GetCafeName(ss.CostCenter), 1, TrucksToo, ss.AnchorStationFoodType, ss.AnchorStationFoodSubType, ss.HasHood, StationCount)
        Next


        ' Load dual station locations next
        Dim qss2 = From ss As Cafe In SharedDataGroup.Cafes
                   Where ss.BrandStations = 2
                   Select ss

        ' Determine if trucks are also allowed, then add the station
        For Each ss In qss2
            TrucksToo = False
            For Each tl In qtl
                If ss.BldgId = tl.PID Then TrucksToo = True
            Next
            AddRegularLocation(GetCafeName(ss.CostCenter), 2, TrucksToo, ss.AnchorStationFoodType, ss.AnchorStationFoodSubType, ss.HasHood, StationCount)
        Next

        ' Load greater than two stations finally
        Dim qss3 = From ss As Cafe In SharedDataGroup.Cafes
                   Where ss.BrandStations > 2
                   Select ss

        ' Determine if trucks are also allowed, then add the station
        For Each ss In qss3
            TrucksToo = False
            For Each tl In qtl
                If ss.BldgId = tl.PID Then TrucksToo = True
            Next
            AddRegularLocation(GetCafeName(ss.CostCenter), ss.BrandStations, TrucksToo, ss.AnchorStationFoodType, ss.AnchorStationFoodSubType, ss.HasHood, StationCount)
        Next

    End Sub

    Private Sub AddRegularLocation(bldgnm, statcount, truckok, asft, asfst, hh, sc)
        Dim newloc As New ScheduleLocation(bldgnm, statcount, Me, Highlight)
        With newloc
            .AllowsFoodTrucks = truckok
            .AnchorFoodType = asft
            .AnchorFoodSubType = asfst
            .HasHood = hh
        End With

        LocationStack.Children.Add(newloc)
        Highlight = Not Highlight
    End Sub

    Private Sub AddTruckOnlyLocation(bldgnm)
        Dim TruckOnlyStation As New ScheduleLocation(bldgnm & "(Trucks Only)", 0, Me, Highlight)
        TruckOnlyStation.AllowsFoodTrucks = True
        LocationStack.Children.Add(TruckOnlyStation)
        Highlight = Not Highlight
    End Sub

    Private Sub ScrollView(sender As ScrollViewer, e As ScrollChangedEventArgs)
        If SystemCall = True Then Exit Sub
        VendorSched.wkSched.SyncScrollViews(Me, sender.VerticalOffset)
    End Sub

    Private Function GetCafeName(cc As Long) As String
        'TODO: PROBABLY NEED TO MOVE THIS TO THE BASEMODULE AND REPLACE SOMETHING THAT'S THERE ALREADY
        Try
            Dim qun = (From un As CostCenter In SharedDataGroup.CostCenters
                       Where un.CostCenter1 = cc
                       Select un).ToList(0)
            Return qun.FlashName
        Catch ex As Exception
            Return ""
        End Try
        Return ""
    End Function

#End Region

End Class
