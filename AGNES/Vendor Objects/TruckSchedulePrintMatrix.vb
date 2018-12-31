Public Class TruckSchedulePrintMatrix
    Public Mon As String
    Public Tue As String
    Public Wed As String
    Public Thu As String
    Public Fri As String
    Private TotalCount As Byte
    Public LocName As String
    Private WkSchedRef As ScheduleWeek

    Public Sub New(wksched As ScheduleWeek, locnm As String)
        WkSchedRef = wksched
        LocName = locnm
    End Sub

    Public Sub GetData()
        Mon = GetDayInfo(0)
        Tue = GetDayInfo(1)
        Wed = GetDayInfo(2)
        Thu = GetDayInfo(3)
        Fri = GetDayInfo(4)
    End Sub

    Private Function GetDayInfo(ind) As String
        Dim ReturnString As String = "No Trucks"
        Dim BuilderString As String = ""
        Dim workingday As ScheduleDay = WkSchedRef.Children(ind)

        For Each l In workingday.LocationStack.Children
            If TypeOf (l) Is ScheduleLocation Then
                Dim workingloc As ScheduleLocation = l
                If workingloc.LocationName = LocName Then
                    For Each s In workingloc.StationStack.Children
                        If TypeOf (s) Is ScheduleTruckStation Then
                            Dim workingstat As ScheduleTruckStation = s
                            If BuilderString = "" Then
                                BuilderString = workingstat.TruckName
                            Else
                                BuilderString = BuilderString & Chr(13) & workingstat.TruckName
                            End If
                        End If
                    Next
                End If
            End If
        Next
        If BuilderString <> "" Then
            Return BuilderString
            TotalCount += 1
        Else
            Return ReturnString
        End If

    End Function

End Class
