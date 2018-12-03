Module VendorModule

#Region "Properties"
    Public VendorSched As VendorSchedule
    Public NumberOfDaysInWeek As Byte
#End Region

#Region "Public Methods"
    Public Sub Runmodule()
        VendorSched = New VendorSchedule
        VendorSched.ShowDialog()
    End Sub

#End Region

#Region "Private Methods"

#End Region

End Module
