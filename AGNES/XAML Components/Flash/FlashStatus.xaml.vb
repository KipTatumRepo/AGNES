Imports System.ComponentModel
Imports System.Windows.Threading
Public Class FlashStatus
#Region "Properties"
    Public TypeofFlash As Byte
    Public StatusWk As WeekChooser
    Public StatusMsp As PeriodChooser

#End Region

#Region "Constructor"
    Public Sub New(ft As Byte)
        InitializeComponent()
        TypeofFlash = ft
        Dim currwk As Byte = GetCurrentWeek(FormatDateTime(Now(), DateFormat.ShortDate))
        Dim currmsp As Byte = GetCurrentPeriod(FormatDateTime(Now(), DateFormat.ShortDate))
        StatusWk = New WeekChooser(1, currwk, currwk)
        StatusMsp = New PeriodChooser(StatusWk, 1, currmsp, currmsp)
        AddHandler StatusWk.PropertyChanged, AddressOf PopulateUnits
        Dim sep As New Separator
        With tlbTimePeriods.Items
            .Add(StatusMsp)
            .Add(sep)
            .Add(StatusWk)
        End With
        PopulateUnits()
    End Sub

#End Region

#Region "Public Methods"
    Public Sub PopulateUnits()
        TimerOne = Nothing
        wrpFlashes.Children.Clear()

        '// Acquire all units belonging to the selected flash group
        Dim qgu = From uig In SharedDataGroup.LOCATIONS
                  Select uig
                  Where uig.FlashType = TypeofFlash

        '// Move into a dictionary and add any subunits that might be associated with the primary unit
        Dim UnitList As New Dictionary(Of String, Long)

        For Each uig In qgu
            UnitList.Add(uig.Unit, uig.Unit_Number)

            Dim qsu = From su In AGNESShared.UnitsSubunits
                      Where su.UnitNumber = uig.Unit_Number And
                          su.SubUnitNumber <> uig.Unit_Number
                      Select su

            For Each sug In qsu
                UnitList.Add(sug.Description, sug.SubUnitNumber)
            Next
        Next


        '// Build items into wrappanel
        For Each i In UnitList
            Dim CurrentFlashStatus As String = GetFlashstatus(i.Value)
            Select Case CurrentFlashStatus
                Case "None"
                    ' Flash pending - add item, but disable
                    Dim NewStatusItem As New StatusItem(i.Value & "-" & i.Key, 0, False, "", i.Value)
                    NewStatusItem.IsEnabled = False
                    NewStatusItem.MSP = StatusMsp.CurrentPeriod
                    NewStatusItem.Wk = StatusWk.CurrentWeek
                    wrpFlashes.Children.Add(NewStatusItem)
                Case "Draft"
                    ' Draft - check alerts & add with unlocked icon, disabled
                    Dim als As Boolean = GetAlertStatus(i.Value)
                    Dim alm As String = GetAlertMessage(als, i.Value)
                    Dim NewStatusItem As New StatusItem(i.Value & "-" & i.Key, 1, als, alm, i.Value)
                    NewStatusItem.IsEnabled = True
                    NewStatusItem.MSP = StatusMsp.CurrentPeriod
                    NewStatusItem.Wk = StatusWk.CurrentWeek
                    wrpFlashes.Children.Add(NewStatusItem)

                Case "Final"
                    ' Final - check alerts & add item with locked icon
                    Dim als As Boolean = GetAlertStatus(i.Value)
                    Dim alm As String = GetAlertMessage(als, i.Value)
                    Dim NewStatusItem As New StatusItem(i.Value & "-" & i.Key, 2, als, alm, i.Value)
                    NewStatusItem.IsEnabled = True
                    NewStatusItem.MSP = StatusMsp.CurrentPeriod
                    NewStatusItem.Wk = StatusWk.CurrentWeek
                    wrpFlashes.Children.Add(NewStatusItem)
            End Select
        Next

        '// Set timer to 1:00 for refreshing
        tbRefresh.Text = "Last refresh: " & Now.ToShortTimeString
        TimerOne = New DispatcherTimer()
        AddHandler TimerOne.Tick, AddressOf RefreshStatus
        TimerOne.Interval = New TimeSpan(0, 5, 0)
        TimerOne.Start()
    End Sub

#End Region

#Region "Private Methods"
    Private Sub RefreshStatus()
        TimerOne.Stop()
        PopulateUnits()
    End Sub

    Private Function GetFlashstatus(unum As Long) As String
        Dim p As Byte = StatusMsp.CurrentPeriod, w As Byte = StatusWk.CurrentWeek
        Dim qfs = From ufs In FlashActuals.FlashActualData
                  Select ufs
                  Where ufs.UnitNumber = unum And
                      ufs.MSFY = CurrentFiscalYear And
                      ufs.MSP = p And
                      ufs.Week = w And
                      ufs.Status <> ""

        If qfs.Count = 0 Then
            Return "None"
        Else
            For Each ufs In qfs
                Return ufs.Status
            Next
        End If
        Return "None"
    End Function

    Private Function GetAlertStatus(unum As Long) As Boolean
        Dim p As Byte = StatusMsp.CurrentPeriod, w As Byte = StatusWk.CurrentWeek
        Dim qas = From fas In FlashActuals.FlashActualData
                  Select fas
                  Where fas.UnitNumber = unum And
                      fas.MSFY = CurrentFiscalYear And
                      fas.MSP = p And
                      fas.Week = w

        For Each fas In qas
            Return fas.Alert
        Next
        Return False
    End Function

    Private Function GetAlertMessage(yn As Boolean, unum As Long) As String
        If yn = True Then
            Dim p As Byte = StatusMsp.CurrentPeriod, w As Byte = StatusWk.CurrentWeek
            Dim qam = From fam In FlashActuals.FlashAlerts
                      Select fam
                      Where fam.UnitNumber = unum And
                          fam.MSFY = CurrentFiscalYear And
                          fam.MSP = p And
                          fam.Week = w

            For Each fam In qam
                Return fam.AlertNote
            Next
        End If
        Return ""
    End Function

    Private Sub FlashStatus_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        TimerOne = Nothing
    End Sub

#End Region

End Class
