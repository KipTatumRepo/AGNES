Module VendorModule

#Region "Properties"
    Public VendorSched As VendorSchedule
    Public VendorEdit As VendorEditor
    Public NumberOfDaysInWeek As Byte
#End Region

#Region "Public Methods"
    Public Sub Runmodule()
        VendorSched = New VendorSchedule
        VendorSched.ShowDialog()
    End Sub

    Public Function GetAlternateStationName(bn, sn) As String
        'CRITICAL: ALTERNATE STATION NAMES HARD CODED FOR TESTING - WILL NEED TO CONVERT TO DB TABLE FOR EDITOR/MAINTENANCE
        Select Case bn
            Case "Advanta"
                If sn = 3 Then Return "NP"
            Case "Café 31"
                If sn = 2 Then Return "POD"
        End Select
        Return "Station " & sn
    End Function

#End Region

#Region "Private Methods"

#End Region

End Module
