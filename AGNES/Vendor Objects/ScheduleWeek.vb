Imports System.ComponentModel
Public Class ScheduleWeek
    Inherits DockPanel
#Region "Properties"

#End Region

#Region "Constructor"

#End Region

#Region "Public Methods"
    Public Sub Update(p As Byte, w As Byte)
        Children.Clear()
        Dim qwd = From f As Dates In SharedDataGroup.Dates
                  Where f.MS_FY = CurrentFiscalYear And
                      f.MS_Period = p And
                      f.Week = w

        For Each f In qwd
            If Weekday(f.Date_ID, FirstDayOfWeek.Monday) < 6 Then CreateWeekDay(f.Date_ID, f.IS_WEEKEND_HOLIDAY)
        Next

    End Sub

#End Region

#Region "Private Methods"
    Private Sub CreateWeekDay(dt, hol)
        Dim newday As New ScheduleDay(dt, hol)
        If hol = 1 Then newday.IsEnabled = False
        Children.Add(newday)
    End Sub

#End Region

End Class
