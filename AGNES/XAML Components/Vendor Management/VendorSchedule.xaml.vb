Imports System.ComponentModel

'CRITICAL:  VENDOR DROP MESSAGES AREN'T SHOWING PROPERLY
'CRITICAL:  WEEKLY LOCATIONS AREN'T CALCULATING PROPERLY WHEN HOLIDAYS ARE PRESENT
Public Class VendorSchedule

#Region "Properties"
    Public Property YR As YearChooser
    Public Property CAL As MonthChooser
    Public Property Wk As WeekChooser
    Public wkSched As ScheduleWeek
    Public ActiveVendor As ScheduleVendor
    Private _savestatus As Boolean
    Private CurrYear As Integer
    Private CurrMonth As Byte
    Private CurrWeek As Byte
    Private CurrentVendorView As Byte
    Public Property SaveStatus As Boolean
        Get
            Return _savestatus
        End Get
        Set(value As Boolean)
            _savestatus = value
            If value = False Then
                sbSaveStatus.Background = Brushes.Red
                tbSaveStatus.Text = "Changes Not Saved"
            Else
                sbSaveStatus.Background = Brushes.White
                tbSaveStatus.Text = ""

            End If
        End Set
    End Property


#End Region

#Region "Constructor"
    Public Sub New()
        InitializeComponent()
        SaveStatus = True
        Height = System.Windows.SystemParameters.PrimaryScreenHeight
        '// Add period and week slicers
        CurrYear = Now().Year
        CurrMonth = Now().Month
        CurrWeek = GetCurrentCalendarWeek(FormatDateTime(Now(), DateFormat.ShortDate))
        Wk = New WeekChooser(1, GetMaxCalendarWeeks(CurrMonth), CurrWeek)
        Wk.DisableSelectAllWeeks = True
        Wk.DisableHideWeeks = True
        AddHandler Wk.PropertyChanged, AddressOf WeekChanged
        CAL = New MonthChooser(Wk, 1, 12, CurrMonth)
        CAL.DisableSelectAll = False
        YR = New YearChooser(CAL, CurrYear, CurrYear + 1, CurrYear)
        Dim sep As New Separator
        With tlbVendors.Items
            .Add(YR)
            .Add(CAL)
            .Add(sep)
            .Add(Wk)
        End With

        '// Add week object, with days, locations, and data load being subfunctions
        wkSched = New ScheduleWeek
        wkSched.Update(YR.CurrentYear, CAL.CurrentMonth, Wk.CurrentWeek)
        grdWeek.Children.Add(wkSched)

        PopulateVendors(0) '//   Any consideration of day-to-day vendor availability as to whether to show them?
    End Sub

#End Region

#Region "Public Methods"
    Public Sub PopulateVendors(view)   '0=All, 1=Retail, 2=Brands, 3=Trucks
        stkVendors.Children.Clear()
        Dim qvn = From v In VendorData.VendorInfo
                  Where v.Active = True And
                      (v.VendorType = 2 Or v.VendorType = 3)

        For Each v In qvn
            Dim s As String = v.Name
            Dim nv As New ScheduleVendor(v)
            stkVendors.Children.Add(nv)
            nv.UsedWeeklySlots = 0
        Next
    End Sub

#End Region

#Region "Private Methods"
    Private Function DiscardCheck() As Boolean
        Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Small, AgnesMessageBox.MsgBoxLayout.TextAndImage, AgnesMessageBox.MsgBoxType.YesNo, 12, False,, "Discard unsaved data?",, AgnesMessageBox.ImageType.Danger)
        amsg.ShowDialog()
        If amsg.ReturnResult = "No" Then
            amsg.Close()
            Return False
        End If
        amsg.Close()
        Return True
    End Function

    Private Sub VendorSchedule_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        If SaveStatus = False Then
            If DiscardCheck() = False Then e.Cancel = True
        End If
    End Sub

    Private Sub ShowBrandsOnly(sender As Object, e As MouseButtonEventArgs) Handles imgBrands.MouseLeftButtonDown
        If CurrentVendorView = 2 Then
            imgBrands.Effect = Nothing
            imgTrucks.Effect = Nothing
            CurrentVendorView = 0
            ShowSegment(0)
            ExpandLocations()
            Exit Sub
        End If
        imgTrucks.Effect = New Effects.BlurEffect With {.KernelType = Effects.KernelType.Box, .Radius = 5,
            .RenderingBias = Effects.RenderingBias.Performance}
        imgBrands.Effect = Nothing
        CurrentVendorView = 2
        ExpandLocations()
        ShowSegment(2)
        CollapseTrucks()

    End Sub

    Private Sub ShowTrucksOnly(sender As Object, e As MouseButtonEventArgs) Handles imgTrucks.MouseLeftButtonDown
        If CurrentVendorView = 3 Then
            imgBrands.Effect = Nothing
            imgTrucks.Effect = Nothing
            CurrentVendorView = 0
            ShowSegment(0)
            ExpandLocations()
            Exit Sub
        End If
        imgBrands.Effect = New Effects.BlurEffect With {.KernelType = Effects.KernelType.Box, .Radius = 5,
            .RenderingBias = Effects.RenderingBias.Performance}
        imgTrucks.Effect = Nothing
        CurrentVendorView = 3
        ExpandLocations()
        ShowSegment(3)
        CollapseBrands()

    End Sub

    Private Sub ShowSegment(vendortype)
        For Each v In stkVendors.Children
            If TypeOf (v) Is ScheduleVendor Then
                Dim vt As ScheduleVendor = v
                If vt.VendorType <> vendortype And vendortype <> 0 Then
                    vt.Visibility = Visibility.Collapsed
                Else
                    vt.Visibility = Visibility.Visible
                End If
            End If
        Next
    End Sub

    Private Sub ExpandLocations()
        For Each sd In wkSched.Children
            If TypeOf (sd) Is ScheduleDay Then
                Dim TargetDay As ScheduleDay = sd
                For Each Location In TargetDay.LocationStack.Children
                    If TypeOf (Location) Is ScheduleLocation Then
                        Dim TargetLoc As ScheduleLocation = Location
                        TargetLoc.Visibility = Visibility.Visible
                        ExpandStations(TargetLoc)
                    End If
                Next
            End If
        Next
    End Sub

    Private Sub ExpandStations(ByRef LocObject As ScheduleLocation)
        For Each s In LocObject.StationStack.Children
            s.Visibility = Visibility.Visible
        Next
    End Sub

    Private Sub CollapseBrands()
        For Each sd In wkSched.Children
            If TypeOf (sd) Is ScheduleDay Then
                Dim TargetDay As ScheduleDay = sd
                For Each Location In TargetDay.LocationStack.Children
                    If TypeOf (Location) Is ScheduleLocation Then
                        Dim TargetLoc As ScheduleLocation = Location
                        If TargetLoc.AllowsFoodTrucks = False Then
                            TargetLoc.Visibility = Visibility.Collapsed
                        Else
                            CollapseStations(1, TargetLoc)
                        End If
                    End If
                Next
            End If
        Next
    End Sub

    Private Sub CollapseTrucks()
        For Each sd In wkSched.Children
            If TypeOf (sd) Is ScheduleDay Then
                Dim TargetDay As ScheduleDay = sd
                For Each Location In TargetDay.LocationStack.Children
                    If TypeOf (Location) Is ScheduleLocation Then
                        Dim TargetLoc As ScheduleLocation = Location
                        If TargetLoc.AllowsFoodTrucks = True And TargetLoc.StationCount = 0 Then
                            TargetLoc.Visibility = Visibility.Collapsed
                        Else
                            CollapseStations(0, TargetLoc)
                        End If
                    End If
                Next
            End If
        Next
    End Sub

    Private Sub CollapseStations(ByVal CollapseType As Byte, ByRef LocObject As ScheduleLocation)
        Select Case CollapseType
            Case 0  '   Collapse trucks
                For Each s In LocObject.StationStack.Children
                    If TypeOf (s) Is ScheduleTruckStation Then
                        Dim scollapse As ScheduleTruckStation = s
                        scollapse.Visibility = Visibility.Collapsed
                    End If
                Next
            Case 1  '   Collapse brands
                For Each s In LocObject.StationStack.Children
                    If TypeOf (s) Is ScheduleStation Then
                        Dim scollapse As ScheduleStation = s
                        scollapse.Visibility = Visibility.Collapsed
                    End If
                Next

        End Select
    End Sub

#End Region

#Region "Event Listeners"
    Private Sub WeekChanged()
        If Wk.SystemChange = True Then
            Wk.SystemChange = False
            Exit Sub
        End If
        If SaveStatus = False Then
            If DiscardCheck() = False Then
                Wk.SystemChange = True
                YR.CurrentYear = CurrYear
                CAL.CurrentMonth = CurrMonth
                Wk.CurrentWeek = CurrWeek
                Exit Sub
            End If
        End If
        CurrYear = YR.CurrentYear
        CurrMonth = CAL.CurrentMonth
        CurrWeek = Wk.CurrentWeek
        wkSched.Update(CurrYear, CurrMonth, CurrWeek)
        SaveStatus = True
    End Sub

#End Region

End Class
