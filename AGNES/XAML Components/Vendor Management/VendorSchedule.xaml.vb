Public Class VendorSchedule

#Region "Properties"
    Public Property MSP As PeriodChooser
    Public Property Wk As WeekChooser
    Public wkSched As ScheduleWeek
#End Region

#Region "Constructor"
    Public Sub New()
        InitializeComponent()
        '// Add period and week slicers
        Dim currmsp As Byte = GetCurrentPeriod(FormatDateTime(Now(), DateFormat.ShortDate))
        Dim currwk As Byte = GetCurrentWeek(FormatDateTime(Now(), DateFormat.ShortDate))
        Wk = New WeekChooser(1, GetMaxWeeks(currmsp), currwk)
        AddHandler Wk.PropertyChanged, AddressOf WeekChanged
        MSP = New PeriodChooser(Wk, 1, 12, currmsp)
        MSP.DisableSelectAll = False
        Dim sep As New Separator
        With tlbVendors.Items
            .Add(MSP)
            .Add(sep)
            .Add(Wk)
        End With

        '// Add week object, with days, locations, and data load being subfunctions
        wkSched = New ScheduleWeek
        wkSched.Update(MSP.CurrentPeriod, Wk.CurrentWeek)
        grdWeek.Children.Add(wkSched)

        PopulateVendors(0) '//   Any consideration of day-to-day vendor availability as to whether to show them?
    End Sub

#End Region

#Region "Public Methods"

#End Region

#Region "Private Methods"
    Private Sub PopulateVendors(view)   '0=All, 1=Brands, 2=Trucks
        Dim qvn = From v In VendorData.VendorInfo
                  Where v.VendorType = 2 Or
                      v.VendorType = 3 And
                      v.Active = True

        For Each v In qvn
            Dim nv As New ScheduleVendor(v.Name)
            stkVendors.Children.Add(nv)
        Next
    End Sub

#End Region

#Region "Event Listeners"
    Private Sub WeekChanged()
        wkSched.Update(MSP.CurrentPeriod, Wk.CurrentWeek)
    End Sub

#End Region

End Class
