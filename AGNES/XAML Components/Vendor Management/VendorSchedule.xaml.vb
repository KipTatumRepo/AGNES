Imports System.ComponentModel
'CRITICAL: ERROR BEING THROWN AFTER VENDOR IS DELETED AND ANOTHER VENDOR IS ACQUIRED (CURRENT VENDOR NOT UPDATING?)
'           APPEARS TO BE FOOD TRUCKS
'TODO: Changing weeks resets filters in practice, but does not reset filter flags or buttons
Public Class VendorSchedule

#Region "Properties"
    Public Property YR As YearChooser
    Public Property CAL As MonthChooser
    Public Property Wk As WeekChooser
    Public wkSched As ScheduleWeek
    Public ActiveVendor As ScheduleVendor
    Public VendorFilterOn As Boolean
    Private _savestatus As Byte
    Private CurrYear As Integer
    Private CurrMonth As Byte
    Private CurrWeek As Byte
    Private CurrentVendorView As Byte
    Public Property SaveStatus As Byte
        Get
            Return _savestatus
        End Get
        Set(value As Byte)
            _savestatus = value
            Select Case value
                Case 0
                    UpdateStatusBar("NotSaved")
                Case 1
                    UpdateStatusBar("Default")
                Case 2
                    UpdateStatusBar("Saved")
            End Select
        End Set
    End Property

#End Region

#Region "Constructor"
    Public Sub New()
        InitializeComponent()
        SaveStatus = 1
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
        UpdateStatusBar("Loading")
    End Sub

    Private Sub InitialScheduleLoad(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        LoadSchedule(0)
        SaveStatus = 1
        UpdateStatusBar("Default")
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

    Public Sub UpdateStatusBar(status)
        Select Case status
            Case "Default"
                sbSaveStatus.Background = Brushes.White
                tbSaveStatus.Text = ""
            Case "NotSaved"
                sbSaveStatus.Background = Brushes.Red
                tbSaveStatus.Text = "Changes Not Saved"
            Case "Saved"
                sbSaveStatus.Background = Brushes.LightGreen
                tbSaveStatus.Text = "Changes Saved"
            Case "Loading"
                sbSaveStatus.Background = Brushes.Yellow
                tbSaveStatus.Text = "Loading..."
            Case "Saving"
                sbSaveStatus.Background = Brushes.Yellow
                tbSaveStatus.Text = "Saving..."
        End Select

    End Sub

    Public Sub ResetVendorFilters()
        tglBrands.IsChecked = False
        tglTrucks.IsChecked = False
        CurrentVendorView = 0
        ShowSegment(0)
        ExpandLocations()
        VendorFilterOn = False
    End Sub

#End Region

#Region "Private Methods"

#Region "Toolbar"
    Private Sub ImportPreviousWeek(sender As Object, e As MouseButtonEventArgs) Handles imgImport.MouseLeftButtonDown
        Dim daysback As Integer = -7

        If My.Computer.Keyboard.CtrlKeyDown Then daysback = -14
        If My.Computer.Keyboard.CtrlKeyDown And My.Computer.Keyboard.ShiftKeyDown Then daysback = -21
        If SaveStatus = 0 Then
            If DiscardCheck() = False Then Exit Sub
        End If
        LoadSchedule(daysback)
    End Sub

    Private Sub SaveSchedule(sender As Object, e As MouseButtonEventArgs) Handles imgSave.MouseLeftButtonDown
        If SaveStatus > 0 Then Exit Sub

        'Loop through days
        'Loop through locations
        'Purge DB of current entries for the day
        'Loop through stations and truck entries and save data

        Try
            For Each wd As ScheduleDay In wkSched.Children
                If TypeOf (wd) Is ScheduleDay Then
                    Dim wday As ScheduleDay = wd
                    For Each loc As Object In wday.LocationStack.Children
                        If TypeOf (loc) Is ScheduleLocation Then
                            Dim locitem As ScheduleLocation = loc
                            locitem.PurgeDatabase()
                            For Each sat In locitem.StationStack.Children
                                If TypeOf (sat) Is ScheduleStation Then
                                    Dim s As ScheduleStation = sat
                                    s.Save()
                                End If
                                If TypeOf (sat) Is ScheduleTruckStation Then
                                    Dim s As ScheduleTruckStation = sat
                                    s.Save()
                                End If
                            Next
                        End If
                    Next
                End If
            Next
            VendorData.SaveChanges()
            SaveStatus = 2
        Catch ex As Exception
            Dim amsg = New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Medium, AgnesMessageBox.MsgBoxLayout.FullText,
                                AgnesMessageBox.MsgBoxType.OkOnly, 18,, "Unable to save",, "AGNES encountered " & ex.Message & ".  Please review and try again.  If the error continues, contact the BI team.")
            amsg.ShowDialog()
            amsg.Close()
        End Try
    End Sub

    Private Sub BrandsFilterClicked(sender As Object, e As RoutedEventArgs) Handles tglBrands.Click
        If tglBrands.IsChecked = False Then
            ResetVendorFilters()
            Exit Sub
        End If
        tglTrucks.IsChecked = False
        CurrentVendorView = 2
        ExpandLocations()
        ShowSegment(2)
        CollapseTrucks()
    End Sub

    Private Sub TrucksFilterClicked(sender As Object, e As RoutedEventArgs) Handles tglTrucks.Click
        If tglTrucks.IsChecked = False Then
            ResetVendorFilters()
            Exit Sub
        End If
        tglBrands.IsChecked = False
        CurrentVendorView = 3
        ExpandLocations()
        ShowSegment(3)
        CollapseBrands()
    End Sub

#End Region

    Private Sub LoadSchedule(LoadType As Integer)
        ' Loadtype = number of days back to retrieve (0 for current week, -7 for previous, -14 for two weeks, -21 for three weeks)
        Try
            For Each wd As ScheduleDay In wkSched.Children
                If TypeOf (wd) Is ScheduleDay Then
                    Dim wday As ScheduleDay = wd
                    Dim targetdate As Date = wd.DateValue.AddDays(LoadType)
                    For Each loc As Object In wday.LocationStack.Children
                        If TypeOf (loc) Is ScheduleLocation Then
                            Dim locitem As ScheduleLocation = loc
                            locitem.Load(targetdate, LoadType)
                        End If
                    Next
                End If
            Next
        Catch ex As Exception
            Dim amsg = New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Medium, AgnesMessageBox.MsgBoxLayout.FullText,
                    AgnesMessageBox.MsgBoxType.OkOnly, 18,, "Unhandled Error",, "AGNES encountered " & ex.Message & ".")
            amsg.ShowDialog()
            amsg.Close()
        End Try
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

    Private Sub VendorSchedule_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        If SaveStatus = 0 Then
            If DiscardCheck() = False Then e.Cancel = True
        End If
    End Sub

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

#End Region

#Region "Event Listeners"
    Private Sub WeekChanged()
        If Wk.SystemChange = True Then
            Wk.SystemChange = False
            Exit Sub
        End If
        If SaveStatus = 0 Then
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
        ResetVendorFilters()
        wkSched.Update(CurrYear, CurrMonth, CurrWeek)
        PopulateVendors(CurrentVendorView)
        LoadSchedule(0)
        SaveStatus = 1
    End Sub





#End Region

End Class
