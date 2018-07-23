Imports System.Windows.Threading

Public Class WCRCam
    Dim dt As DispatcherTimer = New DispatcherTimer()
    Dim dt2 As DispatcherTimer = New DispatcherTimer()
    Public Sub New()
        InitializeComponent()
        ToggleEntryVisibility(0)
        Dim a As New Animation.DoubleAnimation
        a.From = 432
        a.To = 120
        a.Duration = New Duration(TimeSpan.FromSeconds(1))
        imgAGNES.BeginAnimation(Image.HeightProperty, a)
        AddHandler dt.Tick, AddressOf PauseForMinimizing
        dt.Interval = New TimeSpan(0, 0, 1)
        dt.Start()
        'TODO: Change vendor name textbox to a combobox and map to the appropriate table
    End Sub
    Public Sub PauseForMinimizing(ByVal sender As Object, ByVal e As EventArgs)
        CommandManager.InvalidateRequerySuggested()
        tbCam.Text = "Okay!  Let's move on to CAM checks.  I'll hang out down here so I'm not in your way, but I'll continue to walk you through the process." & Chr(13) & Chr(13) & "If you need additional help, just tap me on the shoulder with your cursor!"
        dt.Stop()
        AddHandler dt2.Tick, AddressOf PauseBeforeCamChecks
        dt2.Interval = New TimeSpan(0, 0, 6)
        dt2.Start()
    End Sub

    Public Sub PauseBeforeCamChecks(ByVal sender As Object, ByVal e As EventArgs)
        CommandManager.InvalidateRequerySuggested()
        tbCam.Text = "Did you have any CAM checks to enter today?"
        dt2.Stop()
        btnYesCam.Visibility = Visibility.Visible
        btnNo.Visibility = Visibility.Visible
    End Sub

    Private Sub CamComplete(sender As Object, e As RoutedEventArgs) Handles btnDone.Click, btnNo.Click
        'TODO: Confirm all objects are being released - program is staying open after this point.
        If ConfirmAndSave() = False Then
            Close()
        Else
            MsgBox("Not saved")
        End If

    End Sub

    Private Sub AddCamCheck(sender As Object, e As RoutedEventArgs) Handles btnYesCam.Click, btnMoreCam.Click
        If ConfirmAndSave() = False Then
            With dtpDepositDate
                .DisplayDateStart = Now().AddDays(-14)
                .DisplayDateEnd = Now()
                .SelectedDate = Now()
            End With
            tbVendorName.Text = ""
            tbCheckNumber.Text = ""
            tbCheckAmount.Text = ""
            tbCheckNotes.Text = ""
        End If
        ToggleEntryVisibility(1)
        btnDone.Visibility = Visibility.Visible
        btnMoreCam.Visibility = Visibility.Visible
        btnYesCam.Visibility = Visibility.Hidden
        btnNo.Visibility = Visibility.Hidden
        tbVendorName.Focus()
    End Sub

    Private Sub ToggleEntryVisibility(onoff As Boolean)
        Select Case onoff
            Case True
                txtVendorName.Visibility = Visibility.Visible
                txtCheckAmount.Visibility = Visibility.Visible
                txtCheckNotes.Visibility = Visibility.Visible
                txtCheckNumber.Visibility = Visibility.Visible
                txtDepositDate.Visibility = Visibility.Visible
                tbVendorName.Visibility = Visibility.Visible
                tbCheckNumber.Visibility = Visibility.Visible
                dtpDepositDate.Visibility = Visibility.Visible
                tbCheckAmount.Visibility = Visibility.Visible
                tbCheckNotes.Visibility = Visibility.Visible
            Case False
                txtVendorName.Visibility = Visibility.Hidden
                txtCheckAmount.Visibility = Visibility.Hidden
                txtCheckNotes.Visibility = Visibility.Hidden
                txtCheckNumber.Visibility = Visibility.Hidden
                txtDepositDate.Visibility = Visibility.Hidden
                tbVendorName.Visibility = Visibility.Hidden
                tbCheckNumber.Visibility = Visibility.Hidden
                dtpDepositDate.Visibility = Visibility.Hidden
                tbCheckAmount.Visibility = Visibility.Hidden
                tbCheckNotes.Visibility = Visibility.Hidden
        End Select

    End Sub

    Private Function ConfirmAndSave() As Boolean
        '// Check for data in each field and validate format.  If all are valid, save.
        'TODO: Add vendor name textblock check to validation routine
        Dim CheckNumValid As Boolean, CheckAmtValid As Boolean, DepDateValid As Boolean, ReturnVal As Boolean
        If tbCheckNumber.Text <> "" Then CheckNumValid = True
        If tbCheckAmount.Text <> "" Then
            Try
                Dim amtvalid As Double = FormatNumber(tbCheckAmount.Text, 2)
                CheckAmtValid = True
            Catch ex As Exception
                CheckAmtValid = False
                tbCheckAmount.SelectAll()
                tbCheckAmount.Focus()
            End Try
        End If
        If dtpDepositDate.Text <> "" Then
            Try
                Dim dtvalid As Date = FormatDateTime(dtpDepositDate.SelectedDate, DateFormat.ShortDate)
                DepDateValid = True
            Catch ex As Exception
                DepDateValid = False
                dtpDepositDate.Focus()
            End Try
        End If
        If tbCheckAmount.Text <> "" Or tbCheckNumber.Text <> "" Then
            If CheckAmtValid = True And CheckNumValid = True And DepDateValid = True Then
                ReturnVal = False
                WCRModule.WCR.AddCamCheck(tbVendorName.Text, tbCheckNumber.Text, FormatNumber(tbCheckAmount.Text, 2), dtpDepositDate.SelectedDate, tbCheckNotes.Text)
                tbCam.Text = ""
            Else
                tbCam.Text = "It looks like the check information isn't quite right.  Can you double check it and try again?"
                ReturnVal = True
            End If
        End If
        Return ReturnVal
    End Function

    Private Sub ExitWCR(sender As Object, e As MouseButtonEventArgs) Handles btnExit.MouseDown
        'TODO: ADD APPLICATION STYLE MESSAGEBOX
        Dim yn As MsgBoxResult = MsgBox("Close WCR?", vbYesNo)
        If yn = vbYes Then
            WCRModule.UserClosed = True
            Close()
        End If
    End Sub
End Class
