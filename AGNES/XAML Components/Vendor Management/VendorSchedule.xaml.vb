Public Class VendorSchedule

#Region "Properties"
    Public Property CAL As MonthChooser
    Public Property Wk As WeekChooser
    Public wkSched As ScheduleWeek
    Public ActiveVendor As ScheduleVendor
#End Region

#Region "Constructor"
    Public Sub New()
        InitializeComponent()
        Height = System.Windows.SystemParameters.PrimaryScreenHeight
        '// Add period and week slicers
        Dim CurrMonth As Byte = Now().Month
        Dim CurrWk As Byte = GetCurrentCalendarWeek(FormatDateTime(Now(), DateFormat.ShortDate))
        Wk = New WeekChooser(1, GetMaxCalendarWeeks(CurrMonth), CurrWk)
        Wk.DisableSelectAllWeeks = True
        AddHandler Wk.PropertyChanged, AddressOf WeekChanged
        CAL = New MonthChooser(Wk, 1, 12, 11)
        CAL.DisableSelectAll = False
        Dim sep As New Separator
        With tlbVendors.Items
            .Add(CAL)
            .Add(sep)
            .Add(Wk)
        End With

        '// Add week object, with days, locations, and data load being subfunctions
        wkSched = New ScheduleWeek
        wkSched.Update(CAL.CurrentMonth, Wk.CurrentWeek)
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


#End Region

#Region "Event Listeners"
    Private Sub WeekChanged()
        wkSched.Update(CAL.CurrentMonth, Wk.CurrentWeek)
    End Sub

#End Region

End Class
