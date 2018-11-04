Imports System.ComponentModel

Public Class StaffCalendar

#Region "Properties"
    Public WeekOne As CalWeekObj
    Public WeekTwo As CalWeekObj
    Public WeekThree As CalWeekObj
    Public WeekFour As CalWeekObj
    Public WeekFive As CalWeekObj
    Private _savestatus As Boolean
    Public Property SaveStatus As Boolean
        Get
            Return _savestatus
        End Get
        Set(value As Boolean)
            _savestatus = value
            If _savestatus = True Then
                tbSaveStatus.Text = "Shortages saved"
                barSaveStatus.Background = Brushes.LightGreen
            Else
                tbSaveStatus.Text = "Changes not saved"
                barSaveStatus.Background = Brushes.Red
            End If
        End Set
    End Property

#End Region

#Region "Constructor"
    Public Sub New(fy, msp, unum)
        InitializeComponent()
        WeekOne = New CalWeekObj(unum, fy, msp, 1, Me)
        WeekTwo = New CalWeekObj(unum, fy, msp, 2, Me)
        WeekThree = New CalWeekObj(unum, fy, msp, 3, Me)
        WeekFour = New CalWeekObj(unum, fy, msp, 4, Me)

        If getperiodoperatingdays(fy, msp) > 20 Then WeekFive = New CalWeekObj(unum, fy, msp, 5, Me)

        With stkPeriod.Children
            .Add(WeekOne)
            .Add(WeekTwo)
            .Add(WeekThree)
            .Add(WeekFour)
        End With
        If getperiodoperatingdays(fy, msp) > 20 Then stkPeriod.Children.Add(WeekFive)

    End Sub

#End Region

#Region "Public Methods"

#End Region

#Region "Private Methods"
#Region "Toolbar"
    Private Sub SaveSchedule(sender As Object, e As MouseButtonEventArgs) Handles imgSave.MouseLeftButtonDown
        For Each wi As CalWeekObj In stkPeriod.Children
            wi.SaveRecords()
        Next
    End Sub

    Private Sub PrintSchedule(sender As Object, e As MouseButtonEventArgs) Handles imgPrint.MouseLeftButtonDown
        PrintAnyObject(stkPeriod, "Shortages")
    End Sub

    Private Sub ClearFields(sender As Object, e As MouseButtonEventArgs) Handles imgClear.MouseLeftButtonDown
        For Each wi As CalWeekObj In stkPeriod.Children
            wi.ClearFields()
        Next
    End Sub

#End Region
    Private Sub StaffCalendar_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        If SaveStatus = False Then
            Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Small, AgnesMessageBox.MsgBoxLayout.TextAndImage, AgnesMessageBox.MsgBoxType.YesNo, 12, False,, "Discard unsaved data?",, AgnesMessageBox.ImageType.Danger)
            amsg.ShowDialog()
            If amsg.ReturnResult = "No" Then e.Cancel = True
            amsg.Close()
        End If
    End Sub
#End Region

End Class
