Imports System.ComponentModel
Public Class ScheduleWeek
    Inherits DockPanel
#Region "Properties"
    Public Property SaveStatus As Boolean
#End Region

#Region "Constructor"

#End Region

#Region "Public Methods"
    Public Sub Update(m As Byte, w As Byte)
        Children.Clear()
        VendorModule.NumberOfDaysInWeek = 0

        Dim IncrementDate As Date, WeekEndDate As Date, MondayCount As Byte = 0
        Dim DateString As String = m & "/1/" & Now().Year
        IncrementDate = FormatDateTime(DateString, DateFormat.ShortDate)
        Do Until MondayCount = w
            If IncrementDate.DayOfWeek = DayOfWeek.Monday Then MondayCount += 1
            IncrementDate = IncrementDate.AddDays(1)
        Loop
        IncrementDate = IncrementDate.AddDays(-1)
        WeekEndDate = IncrementDate.AddDays(4)

        Dim qwd = From f As Dates In SharedDataGroup.Dates
                  Where f.Date_ID >= IncrementDate And
                      f.Date_ID <= WeekEndDate

        For Each f In qwd
            CreateWeekDay(f.Date_ID, f.IS_WEEKEND_HOLIDAY)
        Next



    End Sub

#End Region

#Region "Private Methods"
    Private Sub CreateWeekDay(dt, hol)
        Dim newday As New ScheduleDay(dt, hol)
        If hol = True Then
            newday.IsEnabled = False
        Else
            VendorModule.NumberOfDaysInWeek += 1
        End If
        Children.Add(newday)
    End Sub

#End Region

End Class
