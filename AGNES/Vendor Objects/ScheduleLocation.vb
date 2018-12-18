Imports System.ComponentModel
Public Class ScheduleLocation
    Inherits Border

#Region "Properties"
    Private BC As New BrushConverter
    Public StationStack As StackPanel
    Public Property LocationName As String
    Public Property LocationBlock As TextBlock
    Public Property AnchorFoodType As Long
    Public Property AnchorFoodSubType As Long
    Public Property StationCount As Byte
    Public Property AllowsFoodTrucks As Boolean
    Private Property HighlightColor As Boolean = True
    Public Property CurrentWeekDay As ScheduleDay
    Public Property DraggingIntoStation As Boolean
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
    Public Sub New(locname As String, sc As Byte, ByRef cwd As ScheduleDay, Highlight As Boolean)
        CurrentWeekDay = cwd
        StationCount = sc
        BorderBrush = Brushes.Black
        Dim bc As New BrushConverter
        Background = bc.ConvertFrom("#FFBFE8F7")
        BorderThickness = New Thickness(1, 1, 1, 1)
        Margin = New Thickness(1, 1, 1, 0)
        LocationName = locname
        StationStack = New StackPanel
        Child = StationStack
        AllowDrop = True
        AddName()
        If sc > 0 Then AddStations()
    End Sub

#End Region

#Region "Public Methods"
    Public Sub DeleteItem(ByRef v As VendorInStation)
        StationStack.Children.Remove(v.ReferencedTruckStation)
        Height -= 32
    End Sub

    Public Sub DropTruckIntoLocation(ByVal TruckName As String, ByRef RV As ScheduleVendor)
        Dim tb As New ScheduleTruckStation(TruckName, CurrentWeekDay, Me)
        StationStack.Children.Add(tb)
        Dim nv As New VendorInStation With {.TextAlignment = TextAlignment.Center, .Text = TruckName,
            .ReferencedVendor = RV, .ReferencedLoc = Me, .FontSize = 12, .ReferencedTruckStation = tb}
        nv.Background = Brushes.WhiteSmoke
        tb.TruckStack.Children.Add(nv)
        nv.ReferencedVendor.UsedWeeklySlots += 1
        Height += 32
        VendorSched.SaveStatus = 0
        VendorSched.ActiveVendor = Nothing
        Background = BC.ConvertFrom("#FFBFE8F7")
    End Sub

    Public Sub PurgeDatabase()
        Dim qdr = From dr In VendorData.Schedules
                  Where dr.ScheduleDate = CurrentWeekDay.DateValue And
                      dr.Location = LocationName
                  Select dr

        For Each dr In qdr
            VendorData.Schedules.Remove(dr)
        Next
        VendorData.SaveChanges()
    End Sub

    Public Sub Load(loaddate As Date, lt As Integer)
        ClearExistingData()
        Dim vn As String
        Dim sv As ScheduleVendor = Nothing
        Dim qsi = From si In VendorData.Schedules
                  Where si.Location = LocationName And
                      si.ScheduleDate = loaddate

        For Each si In qsi
            ' Get vendor name from database & increase used vendor slots, if they're present in the vendor panel
            Dim qvi = (From vi In VendorData.VendorInfo
                       Where vi.PID = si.VendorId).ToList(0)
            vn = qvi.Name
            For Each v In VendorSched.stkVendors.Children
                If TypeOf (v) Is ScheduleVendor Then
                    sv = v
                    If sv.VendorItem.PID = si.VendorId Then Exit For
                End If
            Next

            If si.Station <> "Truck" Then
                ' Locate station and add vendor, if it's present in the vendor panel (assumes vendor is therefore active)
                For Each s In StationStack.Children
                    If TypeOf (s) Is ScheduleStation Then
                        Dim st As ScheduleStation = s
                        If st.StationName = si.Station And sv IsNot Nothing Then st.DropVendorIntoStation(vn, sv)
                    End If
                Next
            Else
                ' Create a truck station and add vendor, if it's present in the vendor panel (assumes vendor is therefore active)
                ' Per Rachel Kanner 12/12/18, do not import food trucks from previous weeks
                If sv IsNot Nothing And lt = 0 Then DropTruckIntoLocation(vn, sv)
            End If
        Next
    End Sub

#End Region

#Region "Private Methods"

    Private Sub AddName()
        LocationBlock = New TextBlock With {.TextAlignment = TextAlignment.Center, .Text = LocationName, .FontSize = 14, .FontWeight = FontWeights.SemiBold}
        StationStack.Children.Add(LocationBlock)
    End Sub

    Private Sub AddStations()
        For x As Byte = 1 To StationCount
            Dim station As New ScheduleStation(x, CurrentWeekDay, Me)
            StationStack.Children.Add(station)
        Next
    End Sub

    Private Sub ScheduleLocation_DragEnter(sender As Object, e As DragEventArgs) Handles Me.DragEnter
        If DraggingIntoStation = True Then Exit Sub
        CheckVendorDrag(e.Data.GetData(DataFormats.Text))
    End Sub

    Private Sub ScheduleLocation_DragLeave(sender As Object, e As DragEventArgs) Handles Me.DragLeave
        VendorSched.SaveStatus = VendorSched.SaveStatus
        Background = BC.ConvertFrom("#FFBFE8F7")
    End Sub

    Private Sub ScheduleLocation_Drop(sender As Object, e As DragEventArgs) Handles Me.Drop
        If DropAllowed = False Or DraggingIntoStation = True Then
            VendorSched.SaveStatus = VendorSched.SaveStatus
            Exit Sub
        End If
        DropTruckIntoLocation(e.Data.GetData(DataFormats.Text), VendorSched.ActiveVendor)
    End Sub

    Private Sub CheckVendorDrag(vn As String)
        'Validation routines to preemptively notify about whether vendor is allowed to be scheduled; this is food trucks at the location level
        DropAllowed = True

        If IsVendorTypeAllowedAtBuilding() = False Then    '//     Check if vendor type (truck or brand) is allowed at building
            DropAllowed = False
            Exit Sub
        End If

        VendorSched.tbSaveStatus.Text = "Okay to add"
        VendorSched.sbSaveStatus.Background = Brushes.LightGreen
        Background = Brushes.LightGreen
    End Sub

    Private Sub ClearExistingData()
        Dim truckremovallist As New List(Of ScheduleTruckStation)
        For Each s In StationStack.Children
            If TypeOf (s) Is ScheduleStation Then
                Dim ss As ScheduleStation = s
                For Each v In ss.VendorStack.Children
                    If TypeOf (v) Is VendorInStation Then
                        ss.VendorStack.Children.Remove(v)
                        Height -= 16
                    End If
                Next
            End If

            If TypeOf (s) Is ScheduleTruckStation Then
                Dim ts As ScheduleTruckStation = s
                truckremovallist.Add(ts)
                Height -= 32
            End If
        Next

        ' Remove trucks now, so the collection enumeration is not disrupted
        If truckremovallist.Count > 0 Then
            Dim x As Integer
            For x = (truckremovallist.Count - 1) To 0 Step -1
                StationStack.Children.Remove(truckremovallist(x))
            Next
        End If
    End Sub

    Private Function IsVendorTypeAllowedAtBuilding()
        If VendorSched.ActiveVendor.VendorItem.VendorType = 2 Then
            VendorSched.tbSaveStatus.Text = "Add brands to specific stations."
            VendorSched.sbSaveStatus.Background = Brushes.PaleVioletRed
            Return False
        End If
        If AllowsFoodTrucks = False Then
            VendorSched.tbSaveStatus.Text = "This location does not support food trucks."
            VendorSched.sbSaveStatus.Background = Brushes.PaleVioletRed
            Return False
        End If
        Return True
    End Function

#End Region

End Class
