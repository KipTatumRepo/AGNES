Public Class TruckSchedulePrintMatrix

#Region "Properties"

    Public Mon As String
    Public MonCount As Byte
    Public Tue As String
    Public TueCount As Byte
    Public Wed As String
    Public WedCount As Byte
    Public Thu As String
    Public ThuCount As Byte
    Public Fri As String
    Public FriCount As Byte
    Public MaxRowCount As Byte
    Public TotalCount As Byte
    Public MaxTruckCount As Byte
    Public LocName As String
    Public HdrLocName As String
    Private WkSchedRef As ScheduleWeek
    Private WorkingName As String
    Private DayTruckCount As Byte
    Private _truckcount As Byte
    Private Property TruckCount As Byte
        Get
            Return _truckcount
        End Get
        Set(value As Byte)
            _truckcount = value
            If value > MaxTruckCount Then MaxTruckCount = value
        End Set
    End Property


#End Region

#Region "Constructor"

    Public Sub New(wksched As ScheduleWeek, locnm As String)
        WkSchedRef = wksched
        WorkingName = locnm
        GetPrintName(locnm)
    End Sub

#End Region

#Region "Public Methods"

    Public Sub GetData()
        Mon = GetDayInfo(0)
        MonCount = DayTruckCount
        Tue = GetDayInfo(1)
        TueCount = DayTruckCount
        Wed = GetDayInfo(2)
        WedCount = DayTruckCount
        Thu = GetDayInfo(3)
        ThuCount = DayTruckCount
        Fri = GetDayInfo(4)
        FriCount = DayTruckCount
        If VendorSched.MaxTruckRowCount < MaxRowCount Then VendorSched.MaxTruckRowCount = MaxRowCount
    End Sub

#End Region

#Region "Private Methods"
    Private Function GetDayInfo(ind) As String
        DayTruckCount = 0
        Dim ReturnString As String = "  No Trucks"
        Dim BuilderString As String = ""
        Dim workingday As ScheduleDay = WkSchedRef.Children(ind)

        For Each l In workingday.LocationStack.Children
            If TypeOf (l) Is ScheduleLocation Then
                Dim workingloc As ScheduleLocation = l
                If workingloc.LocationName = WorkingName Then
                    For Each s In workingloc.StationStack.Children
                        If TypeOf (s) Is ScheduleTruckStation Then
                            Dim workingstat As ScheduleTruckStation = s
                            Dim truckname As String = "  " & workingstat.TruckName
                            truckname = truckname.Replace(" (Truck)", "")
                            If BuilderString = "" Then
                                BuilderString = truckname
                            Else
                                BuilderString = BuilderString & Chr(13) & truckname
                                TruckCount += 1

                            End If
                            DayTruckCount += 1
                            If DayTruckCount > MaxRowCount Then MaxRowCount = DayTruckCount
                        End If
                    Next
                End If
            End If
        Next
        If BuilderString <> "" Then
            TotalCount += 1
            Return BuilderString
        Else
            Return ReturnString
        End If

    End Function

    Private Sub GetPrintName(ln)
        'CRITICAL: ADD COLUMN TO LOCATIONS TABLE WITH TRANSLATIONS (OR TRUCK LOCATION EDITOR SOURCE, AS IT DEVELOPS)
        Select Case ln
            Case "32(Trucks Only)"
                LocName = "  Bldg 32"
                HdrLocName = "32"
            Case "92(Trucks Only)"
                LocName = "  Bldg 92"
                HdrLocName = "92"
            Case "STUDIO X(Trucks Only)"
                LocName = "  Studio X"
                HdrLocName = "Studio X"
            Case "Café 43"
                LocName = "  Bldg 43"
                HdrLocName = "43"
            Case Else
                LocName = "  " & ln
                HdrLocName = ln
        End Select
    End Sub

#End Region

End Class
