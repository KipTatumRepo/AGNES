Imports System.ComponentModel
Public Class CalDayObj
    Inherits Border

#Region "Properties"
    Private po As StaffCalendar
    Public SaveOkay As Boolean
    Private UnitNum As Long
    Private DayDate As Date
    Private grdDay As Grid
    Private tbDate As TextBlock
    Private txtHourly As NumberBox
    Private txtSalary As NumberBox
    Private _isholiday As Boolean
    Public Property IsHoliday As Boolean
        Get
            Return _isholiday
        End Get
        Set(value As Boolean)
            _isholiday = value
            Background = Brushes.LightGray
            IsEnabled = False
        End Set
    End Property

#End Region

#Region "Constructor"
    Public Sub New(dt As Date, un As Long, ByRef p As StaffCalendar)
        po = p
        UnitNum = un
        DayDate = dt
        BorderBrush = Brushes.Black
        BorderThickness = New Thickness(1)
        Background = Brushes.LightBlue
        HorizontalAlignment = HorizontalAlignment.Left
        Width = 155
        grdDay = New Grid
        tbDate = New TextBlock With {.Text = FormatDateTime(dt, DateFormat.ShortDate), .TextAlignment = TextAlignment.Center}
        Dim lblHourly As New TextBlock With {.Text = "Hourly", .TextAlignment = TextAlignment.Center, .Margin = New Thickness(10, 26, 0, 0),
            .HorizontalAlignment = HorizontalAlignment.Left, .Width = 65, .Height = 23, .VerticalAlignment = VerticalAlignment.Top}
        Dim lblSalary As New TextBlock With {.Text = "Salary", .TextAlignment = TextAlignment.Center, .Margin = New Thickness(80, 26, 0, 0),
            .HorizontalAlignment = HorizontalAlignment.Left, .Width = 65, .Height = 23, .VerticalAlignment = VerticalAlignment.Top}
        txtHourly = New NumberBox(FieldWidth:=65, AllowPositive:=True, AllowNegative:=False, ForcePositive:=True, ForceNegative:=False, SelectAllUponEnteringField:=True, FontSize:=AgnesBaseInput.FontSz.Smaller) With {.Margin = New Thickness(10, 49, 0, 0)}
        txtSalary = New NumberBox(FieldWidth:=65, AllowPositive:=True, AllowNegative:=False, ForcePositive:=True, ForceNegative:=False, SelectAllUponEnteringField:=True, FontSize:=AgnesBaseInput.FontSz.Smaller) With {.Margin = New Thickness(80, 49, 0, 0)}
        AddHandler txtHourly.PropertyChanged, AddressOf ScheduleChanged
        AddHandler txtSalary.PropertyChanged, AddressOf ScheduleChanged
        With grdDay.Children
            .Add(tbDate)
            .Add(lblHourly)
            .Add(lblSalary)
            .Add(txtHourly)
            .Add(txtSalary)
        End With
        LoadData()
        Child = grdDay
    End Sub

#End Region

#Region "Public Methods"
    Public Sub ClearData()
        txtHourly.SetAmount = 0
        txtSalary.SetAmount = 0
    End Sub

    Public Sub SaveData(U, FY, P, W)
        Dim qse = From s In FlashForecasts.AssociateShortages
                  Where s.Date = DayDate And
                      s.UnitNumber = UnitNum

        If qse.Count = 0 Then   '// Create new entry
            SaveOkay = SaveAsNew()
        Else                    '// Update existing entry
            SaveOkay = SaveAsUpdate()
        End If
    End Sub

#End Region

#Region "Private Methods"
    Private Sub LoadData()
        Dim qse = From s In FlashForecasts.AssociateShortages
                  Where s.Date = DayDate And
                      s.UnitNumber = UnitNum

        If qse.Count = 0 Then Exit Sub
        For Each s In qse
            txtHourly.SetAmount = s.HourlyOut
            txtSalary.SetAmount = s.SalaryOut
        Next


    End Sub

    Private Function SaveAsNew() As Boolean
        Try
            Dim ns As New AssociateShortage
            With ns
                .UnitNumber = UnitNum
                .Date = DayDate
                .HourlyOut = txtHourly.Amount
                .SalaryOut = txtSalary.Amount
            End With
            FlashForecasts.AssociateShortages.Add(ns)
            FlashForecasts.SaveChanges()
        Catch ex As Exception
            Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Medium, AgnesMessageBox.MsgBoxLayout.FullText, AgnesMessageBox.MsgBoxType.OkOnly,
                                                18, True, "Unexpected error!",, ex.Message)
            amsg.ShowDialog()
            amsg.Close()
            Return False
        End Try
        Return True
    End Function

    Private Function SaveAsUpdate() As Boolean
        Try
            Dim qsr = (From s In FlashForecasts.AssociateShortages
                       Where s.Date = DayDate And
                           s.UnitNumber = UnitNum).ToList()(0)
            With qsr
                .HourlyOut = txtHourly.Amount
                .SalaryOut = txtSalary.Amount
            End With
            FlashForecasts.SaveChanges()
        Catch ex As Exception
            Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Medium, AgnesMessageBox.MsgBoxLayout.FullText, AgnesMessageBox.MsgBoxType.OkOnly,
                                        18, True, "Unexpected error!",, ex.Message)
            amsg.ShowDialog()
            amsg.Close()
            Return False
        End Try
        Return True
    End Function

#End Region

#Region "Event Listeners"
    Private Sub ScheduleChanged()
        po.SaveStatus = False
    End Sub

#End Region

End Class
