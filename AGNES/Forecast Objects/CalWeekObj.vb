Imports System.ComponentModel
Public Class CalWeekObj
    Inherits Border

#Region "Properties"
    Private DatePanel As DockPanel
    Private WeekDates(5) As Date
    Private FiscalYr As Integer
    Private Period As Byte
    Private Unit As Long
    Private _weeknumber As Byte
    Private ParentSchedule As StaffCalendar
    Public Property WeekNumber As Byte
        Get
            Return _weeknumber
        End Get
        Set(value As Byte)
            _weeknumber = value
            SetWeekDates()
        End Set
    End Property

#End Region

#Region "Constructor"
    Public Sub New(unum As Long, fy As Integer, msp As Byte, wk As Byte, ByRef p As StaffCalendar)
        ParentSchedule = p
        FiscalYr = fy
        Period = msp
        WeekNumber = wk
        Unit = unum
        BorderBrush = Brushes.Black
        HorizontalAlignment = HorizontalAlignment.Left
        VerticalAlignment = VerticalAlignment.Top
        Height = 95
        Width = 785
        Margin = New Thickness(5, 0, 0, 0)
        DatePanel = New DockPanel
        For x As Byte = 1 To 5
            Dim NewDay As New CalDayObj(WeekDates(x), unum, ParentSchedule)
            DatePanel.Children.Add(NewDay)
            If IsHoliday(WeekDates(x)) = True Then NewDay.IsHoliday = True
        Next
        Child = DatePanel
    End Sub

#End Region

#Region "Public Methods"
    Public Sub ClearFields()
        For Each d As CalDayObj In DatePanel.Children
            d.ClearData()
        Next
    End Sub

    Public Sub SaveRecords()
        For Each d As CalDayObj In DatePanel.Children
            d.SaveData(Unit, FiscalYr, Period, _weeknumber)
            If d.SaveOkay = False Then Exit Sub
        Next
        ParentSchedule.SaveStatus = True
    End Sub

#End Region

#Region "Private Methods"
    Private Sub SetWeekDates()
        Dim qdz = From d In SharedDataGroup.Dates
                  Where d.MS_FY = FiscalYr And
                      d.MS_Period = Period And
                      d.Week = _weeknumber

        Dim x As Byte = 1
        For Each d In qdz
            If Weekday(d.Date_ID, FirstDayOfWeek.Monday) < 6 Then
                WeekDates(x) = d.Date_ID
                x += 1
            End If
        Next
    End Sub

    Private Function IsHoliday(dt As Date) As Boolean
        Dim qhd = From d In SharedDataGroup.Dates
                  Where d.Date_ID = dt

        For Each d In qhd
            If d.IS_WEEKEND_HOLIDAY = True Then Return True
        Next
        Return False
    End Function

#End Region

End Class
