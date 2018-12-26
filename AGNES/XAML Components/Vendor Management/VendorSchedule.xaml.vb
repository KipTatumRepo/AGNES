﻿Imports System.ComponentModel
Imports Microsoft.Win32
Imports Microsoft.Office.Interop
Imports System.Printing
Imports System.Windows.Xps
'CRITICAL: ERROR BEING THROWN AFTER VENDOR IS DELETED AND ANOTHER VENDOR IS ACQUIRED (CURRENT VENDOR NOT UPDATING?)
'           APPEARS TO BE FOOD TRUCKS
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
    Private PrintFailed As Boolean
    Private CurrentVendorView As Byte
    Private pd As PrintDialog
    Private fd As FlowDocument
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
        If view = 0 Or view = 3 Then
            Dim BrandLabel As New TextBlock With {.Text = "Brands", .TextAlignment = TextAlignment.Center, .Background = Brushes.LightBlue,
                    .Foreground = Brushes.White, .FontSize = 12, .FontWeight = FontWeights.Bold, .Padding = New Thickness(0, 5, 0, 0)}
            stkVendors.Children.Add(BrandLabel)
            Dim qbv = From bv In VendorData.VendorInfo
                      Where bv.Active = True And
                              bv.VendorType = 2
                      Order By bv.Name
                      Select bv

            For Each bv In qbv
                Dim s As String = bv.Name
                Dim nbv As New ScheduleVendor(bv)
                stkVendors.Children.Add(nbv)
                nbv.UsedWeeklySlots = 0
            Next
        End If
        If view = 0 Or view = 2 Then
            Dim Trucklabel As New TextBlock With {.Text = "Trucks", .TextAlignment = TextAlignment.Center, .Background = Brushes.LightBlue,
                    .Foreground = Brushes.White, .FontSize = 12, .FontWeight = FontWeights.Bold, .Padding = New Thickness(0, 5, 0, 0)}
            stkVendors.Children.Add(Trucklabel)
            Dim qtv = From tv In VendorData.VendorInfo
                      Where tv.Active = True And
                          tv.VendorType = 3
                      Order By tv.Name
                      Select tv

            For Each tv In qtv
                Dim s As String = tv.Name
                Dim ntv As New ScheduleVendor(tv)
                stkVendors.Children.Add(ntv)
                ntv.UsedWeeklySlots = 0
            Next
        End If

        'Dim qvn = From v In VendorData.VendorInfo
        '          Where v.Active = True And
        '              (v.VendorType = 2 Or v.VendorType = 3)
        '          Order By v.Name
        '          Select v

        'For Each v In qvn
        '    Dim s As String = v.Name
        '    Dim nv As New ScheduleVendor(v)
        '    stkVendors.Children.Add(nv)
        '    nv.UsedWeeklySlots = 0
        'Next
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
        VendorSched.tbPBStatus.Text = "Saving"
        VendorSched.stkProgBar.Visibility = Visibility.Visible
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
        Finally
            VendorSched.stkProgBar.Visibility = Visibility.Collapsed
        End Try
    End Sub

    Private Sub PrintSchedule(sender As Object, e As MouseButtonEventArgs) Handles imgPrint.MouseLeftButtonDown
        Try
            pd = New PrintDialog
            If pd.ShowDialog() <> True Then
                PrintFailed = True
                Exit Sub
            End If
        Catch
            Exit Sub
        End Try
        Select Case CurrentVendorView
            Case 0  ' Print all three
                PrintBrandsbyCafe()
                PrintCafesbyBrand()
                PrintTrucks()
            Case 2  ' Print Brands
                'PrintBrandsbyCafe()
                PrintCafesbyBrand()
            Case 3  ' Print Trucks
                PrintTrucks()
        End Select

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
            If TypeOf (v) Is TextBlock Then
                Dim tb As TextBlock = v
                If (tb.Text = "Trucks" And vendortype = 2) Or (tb.Text = "Brands" And vendortype = 3) Then
                    tb.Visibility = Visibility.Collapsed
                Else
                    tb.Visibility = Visibility.Visible
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

    Private Sub PrintBrandsbyCafe()
        Dim BrandsByCafeArray(40, 6) As String
        fd = New FlowDocument With {.ColumnGap = 0, .ColumnWidth = pd.PrintableAreaWidth}

#Region "Build Header and Table"
        '// Header
        Dim p As New Paragraph(New Run("Brand Rotation by Cafe for the week of " & GetWeekStart().ToShortDateString)) With
            {.FontSize = 14, .TextAlignment = TextAlignment.Center, .FontWeight = FontWeights.Bold, .FontFamily = New FontFamily("Segoe UI")}

        '// Build table
        Dim t As New Table() With {.CellSpacing = 0, .Background = Brushes.LemonChiffon}
        t.Columns.Add(New TableColumn() With {.Background = Brushes.LightBlue, .Width = New GridLength(100)})
        t.Columns.Add(New TableColumn() With {.Background = Brushes.White, .Width = New GridLength(130)})
        t.Columns.Add(New TableColumn() With {.Background = Brushes.White, .Width = New GridLength(130)})
        t.Columns.Add(New TableColumn() With {.Background = Brushes.White, .Width = New GridLength(130)})
        t.Columns.Add(New TableColumn() With {.Background = Brushes.White, .Width = New GridLength(130)})
        t.Columns.Add(New TableColumn() With {.Background = Brushes.White, .Width = New GridLength(130)})
        t.RowGroups.Add(New TableRowGroup())
#End Region

#Region "Gather Information Array"
        'Loop through locations.  Count the number of non-truck stations and add that many instances of the location to the array
        Dim activeday As ScheduleDay
        Dim activeloc As ScheduleLocation
        Dim activestat As ScheduleStation
        Dim activevend As VendorInStation
        Dim RowCount As Integer
        Dim d As Integer = 0
        For Each dayobj As Object In wkSched.Children
            If TypeOf (dayobj) Is ScheduleDay Then
                activeday = dayobj
                RowCount = 0
                For Each loc As Object In activeday.LocationStack.Children
                    If TypeOf (loc) Is ScheduleLocation Then
                        activeloc = loc
                        For Each stat As Object In activeloc.StationStack.Children
                            If TypeOf (stat) Is ScheduleStation Then
                                activestat = stat
                                BrandsByCafeArray(RowCount, 0) = activeloc.LocationName
                                For Each vis As Object In activestat.VendorStack.Children
                                    If TypeOf (vis) Is VendorInStation Then
                                        activevend = vis
                                        BrandsByCafeArray(RowCount, d + 1) = vis.ReferencedVendor.VendorItem.Name
                                    End If
                                Next
                                RowCount += 1
                            End If
                        Next
                    End If
                Next
                d += 1
            End If
        Next
#End Region

#Region "Build Column Rows and Headers"
        Dim cr As New TableRow With {.FontSize = 8, .FontWeight = FontWeights.Normal, .FontFamily = New FontFamily("Segoe UI")}

        For rb As Integer = 1 To RowCount + 1
            t.RowGroups(0).Rows.Add(New TableRow() With {.FontSize = 8, .FontWeight = FontWeights.Normal, .FontFamily = New FontFamily("Segoe UI")})
        Next rb
        '// Alias the current working row for easy reference.
        cr = t.RowGroups(0).Rows(0)

        '// Add column headers
        cr = t.RowGroups(0).Rows(0)
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Cafes")) With {.Background = Brushes.LightBlue, .TextAlignment = TextAlignment.Center, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .FontWeight = FontWeights.Bold, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 1, 0, 1)}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Mon")) With {.Background = Brushes.LightBlue, .TextAlignment = TextAlignment.Center, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .FontWeight = FontWeights.Bold, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 1, 0, 1)}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Tue")) With {.Background = Brushes.LightBlue, .TextAlignment = TextAlignment.Center, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .FontWeight = FontWeights.Bold, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 1, 0, 1)}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Wed")) With {.Background = Brushes.LightBlue, .TextAlignment = TextAlignment.Center, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .FontWeight = FontWeights.Bold, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 1, 0, 1)}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Thu")) With {.Background = Brushes.LightBlue, .TextAlignment = TextAlignment.Center, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .FontWeight = FontWeights.Bold, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 1, 0, 1)}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Fri")) With {.Background = Brushes.LightBlue, .TextAlignment = TextAlignment.Center, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .FontWeight = FontWeights.Bold, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 1, 1, 1)}))

#End Region

#Region "Populate the Table Rows from the Array"
        Dim rc As Integer
        For rc = 1 To RowCount - 1
            cr = t.RowGroups(0).Rows(rc)
            Dim ln As String = BrandsByCafeArray(rc, 0)
            cr.Cells.Add(New TableCell(New Paragraph(New Run(ln)) With {.Background = Brushes.LightBlue, .TextAlignment = TextAlignment.Center, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .FontWeight = FontWeights.Bold, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 1, 1, 1)}))
            For cc = 1 To 5
                Dim vn As String = BrandsByCafeArray(rc, cc)
                If vn = "" Then
                    cr.Cells.Add(New TableCell(New Paragraph(New Run("No Brands")) With {.TextAlignment = TextAlignment.Center, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .FontWeight = FontWeights.Light, .FontStyle = FontStyles.Italic, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 1, 1, 1)}))
                Else
                    cr.Cells.Add(New TableCell(New Paragraph(New Run(vn)) With {.TextAlignment = TextAlignment.Center, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .FontWeight = FontWeights.Normal, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 1, 1, 1)}))
                End If
            Next
        Next

#End Region

#Region "Compose and Print"
        With fd.Blocks
            .Add(p)
            .Add(t)
        End With

        Dim xps_writer As XpsDocumentWriter = PrintQueue.CreateXpsDocumentWriter(pd.PrintQueue)
        Dim idps As IDocumentPaginatorSource = CType(fd, IDocumentPaginatorSource)
        Try
            xps_writer.Write(idps.DocumentPaginator)
        Catch ex As System.Runtime.CompilerServices.RuntimeWrappedException
            Dim notifymsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Small, AgnesMessageBox.MsgBoxLayout.FullText, AgnesMessageBox.MsgBoxType.OkOnly,
                                     18,, "Unable to print!",, "This error usually occurs if you have the PDF file you're trying to overwrite open.  Close the file and try again!")
            notifymsg.ShowDialog()
            notifymsg.Close()
            PrintFailed = True
        Catch ex As Exception
            Dim notifymsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Small, AgnesMessageBox.MsgBoxLayout.FullText, AgnesMessageBox.MsgBoxType.OkOnly,
                                     18,, "Operation failed!",, "Error: " & ex.Message)
            notifymsg.ShowDialog()
            notifymsg.Close()
            PrintFailed = True
        End Try
#End Region

    End Sub

    Private Sub PrintCafesbyBrand()

#Region "Build Header"
        Dim p As New Paragraph(New Run("Brand Rotation by Cafe for the week of " & GetWeekStart().ToShortDateString)) With
            {.FontSize = 14, .TextAlignment = TextAlignment.Center, .FontWeight = FontWeights.Bold, .FontFamily = New FontFamily("Segoe UI")}

#End Region

        Dim activevndr As ScheduleVendor, activeday As ScheduleDay, activeloc As ScheduleLocation, activestation As ScheduleStation,
            activeVIS As VendorInStation, vp As Paragraph
        Dim dc As Byte, ar As Byte, rg As Byte
        fd = New FlowDocument With {.ColumnGap = 0, .ColumnWidth = pd.PrintableAreaWidth}
        fd.Blocks.Add(p)
        For Each v In stkVendors.Children
            Dim activevendorarray(12, 5) As String
            If TypeOf (v) Is ScheduleVendor Then
                activevndr = v
                dc = 1
                ' Search through each day
                For Each d In VendorSched.wkSched.Children
                    If TypeOf (d) Is ScheduleDay Then
                        activeday = d
                        ar = 1
                        For Each l In activeday.LocationStack.Children
                            If TypeOf (l) Is ScheduleLocation Then
                                activeloc = l
                                For Each s In activeloc.StationStack.Children
                                    If TypeOf (s) Is ScheduleStation Then
                                        activestation = s
                                        For Each vis In activestation.VendorStack.Children
                                            If TypeOf (vis) Is VendorInStation Then
                                                activeVIS = vis
                                                If activeVIS.ReferencedVendor Is v Then
                                                    activevendorarray(ar, dc) = activeloc.LocationName
                                                    ar += 1
                                                End If
                                            End If
                                        Next
                                    End If
                                Next
                            End If
                        Next
                    End If
                    dc += 1
                Next
            End If
            ' If activevendorarry.count > 0 then pass to subroutine to add as a printgroup
            If activevendorarray(1, 1) <> "" Then

                '// Build table
                Dim t As New Table() With {.CellSpacing = 0, .Background = Brushes.LemonChiffon}
                t.Columns.Add(New TableColumn() With {.Background = Brushes.White, .Width = New GridLength(130)})
                t.Columns.Add(New TableColumn() With {.Background = Brushes.White, .Width = New GridLength(130)})
                t.Columns.Add(New TableColumn() With {.Background = Brushes.White, .Width = New GridLength(130)})
                t.Columns.Add(New TableColumn() With {.Background = Brushes.White, .Width = New GridLength(130)})
                t.Columns.Add(New TableColumn() With {.Background = Brushes.White, .Width = New GridLength(130)})
                t.RowGroups.Add(New TableRowGroup())

#Region "Build Column Rows and Headers into new rowgroup"
                vp = New Paragraph(New Run(activevndr.VendorItem.Name)) With
            {.FontSize = 12, .TextAlignment = TextAlignment.Center, .FontWeight = FontWeights.SemiBold, .FontFamily = New FontFamily("Segoe UI")}

                Dim cr As New TableRow With {.FontSize = 8, .FontWeight = FontWeights.Normal, .FontFamily = New FontFamily("Segoe UI")}
                t.RowGroups(0).Rows.Add(New TableRow() With {.FontSize = 8, .FontWeight = FontWeights.Normal, .FontFamily = New FontFamily("Segoe UI")})
                cr = t.RowGroups(0).Rows(0)
                '// Add column headers
                cr = t.RowGroups(0).Rows(0)
                cr.Cells.Add(New TableCell(New Paragraph(New Run("Mon")) With {.Background = Brushes.LightBlue, .TextAlignment = TextAlignment.Center, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .FontWeight = FontWeights.Bold, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 1, 0, 1)}))
                cr.Cells.Add(New TableCell(New Paragraph(New Run("Tue")) With {.Background = Brushes.LightBlue, .TextAlignment = TextAlignment.Center, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .FontWeight = FontWeights.Bold, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 1, 0, 1)}))
                cr.Cells.Add(New TableCell(New Paragraph(New Run("Wed")) With {.Background = Brushes.LightBlue, .TextAlignment = TextAlignment.Center, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .FontWeight = FontWeights.Bold, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 1, 0, 1)}))
                cr.Cells.Add(New TableCell(New Paragraph(New Run("Thu")) With {.Background = Brushes.LightBlue, .TextAlignment = TextAlignment.Center, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .FontWeight = FontWeights.Bold, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 1, 0, 1)}))
                cr.Cells.Add(New TableCell(New Paragraph(New Run("Fri")) With {.Background = Brushes.LightBlue, .TextAlignment = TextAlignment.Center, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .FontWeight = FontWeights.Bold, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 1, 1, 1)}))
#End Region

#Region "Populate the Table Rows from the Array"
                Dim rc As Integer
                For rc = 1 To ar - 1
                    t.RowGroups(0).Rows.Add(New TableRow() With {.FontSize = 8, .FontWeight = FontWeights.Normal, .FontFamily = New FontFamily("Segoe UI")})
                    cr = t.RowGroups(0).Rows(rc)
                    For cc = 1 To 5
                        Dim vl As String = activevendorarray(rc, cc)
                        If vl = "" Then
                            cr.Cells.Add(New TableCell(New Paragraph(New Run("")) With {.TextAlignment = TextAlignment.Center, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .FontWeight = FontWeights.Light, .FontStyle = FontStyles.Italic, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 1, 1, 1)}))
                        Else
                            cr.Cells.Add(New TableCell(New Paragraph(New Run(vl)) With {.TextAlignment = TextAlignment.Center, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .FontWeight = FontWeights.Normal, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 1, 1, 1)}))
                        End If
                    Next
                Next
                With fd.Blocks
                    .Add(vp)
                    .Add(t)
                End With
#End Region
                rg += 1
            End If
        Next
#Region "Compose and Print"

        Dim xps_writer As XpsDocumentWriter = PrintQueue.CreateXpsDocumentWriter(pd.PrintQueue)
        Dim idps As IDocumentPaginatorSource = CType(fd, IDocumentPaginatorSource)
        Try
            xps_writer.Write(idps.DocumentPaginator)
        Catch ex As System.Runtime.CompilerServices.RuntimeWrappedException
            Dim notifymsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Small, AgnesMessageBox.MsgBoxLayout.FullText, AgnesMessageBox.MsgBoxType.OkOnly,
                                     18,, "Unable to print!",, "This error usually occurs if you have the PDF file you're trying to overwrite open.  Close the file and try again!")
            notifymsg.ShowDialog()
            notifymsg.Close()
            PrintFailed = True
        Catch ex As Exception
            Dim notifymsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Small, AgnesMessageBox.MsgBoxLayout.FullText, AgnesMessageBox.MsgBoxType.OkOnly,
                                     18,, "Operation failed!",, "Error: " & ex.Message)
            notifymsg.ShowDialog()
            notifymsg.Close()
            PrintFailed = True
        End Try
#End Region

    End Sub

    Private Sub PrintTrucks()
        Dim ph As String = ""
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

    Private Function GetWeekStart() As DateTime
        Dim dayobj As ScheduleDay
        dayobj = wkSched.Children(0)
        Return dayobj.DateValue
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
