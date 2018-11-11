Public Class VendorSchedule

#Region "Properties"
    Public Property MSP As PeriodChooser
    Public Property Wk As WeekChooser
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
        Dim wkSched As New ScheduleWeek
        wkSched.Update(MSP.CurrentPeriod, Wk.CurrentWeek)
        grdWeek.Children.Add(wkSched)

        PopulateVendors() '//   Any consideration of day-to-day vendor availability as to whether to show them?
    End Sub

#End Region

#Region "Public Methods"

#End Region

#Region "Private Methods"
    Private Sub PopulateVendors()
        '//TEST ONLY

        For x As Byte = 1 To 8
            Dim nv As New TextBlock With {.Text = "Vendor " & x}
            stkVendors.Children.Add(nv)
        Next
    End Sub
#End Region

#Region "Event Listeners"
    Private Sub WeekChanged()

    End Sub

#End Region
End Class
