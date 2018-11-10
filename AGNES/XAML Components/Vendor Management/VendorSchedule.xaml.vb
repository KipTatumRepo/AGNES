Public Class VendorSchedule

#Region "Properties"
    Public Property MSP As PeriodChooser
    Public Property Wk As WeekChooser
#End Region

#Region "Constructor"
    Public Sub New()
        InitializeComponent()
        Dim currmsp As Byte = GetCurrentPeriod(FormatDateTime(Now(), DateFormat.ShortDate))
        Dim currwk As Byte = GetCurrentWeek(FormatDateTime(Now(), DateFormat.ShortDate))
        Wk = New WeekChooser(1, GetMaxWeeks(currmsp), currwk)
        MSP = New PeriodChooser(Wk, 1, 12, currmsp)
        MSP.DisableSelectAll = False
        Dim sep As New Separator
        With tlbVendors.Items
            .Add(MSP)
            .Add(sep)
            .Add(Wk)
        End With
    End Sub
#End Region

#Region "Public Methods"

#End Region

#Region "Private Methods"

#End Region
End Class
