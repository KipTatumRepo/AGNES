Imports System.Windows.Threading
Public Class WCRCam

#Region "Properties"

    Dim dt As DispatcherTimer = New DispatcherTimer()
    Dim dt2 As DispatcherTimer = New DispatcherTimer()

    Dim HoverDrop As Effects.DropShadowEffect, LeaveDrop As Effects.DropShadowEffect
#End Region

#Region "Constructor"
    Public Sub New()
        InitializeComponent()
        ToggleEntryVisibility(0)
        tbYesCam.Visibility = Visibility.Hidden
        tbSave.Visibility = Visibility.Hidden
        tbNo.Visibility = Visibility.Hidden
        Dim a As New Animation.DoubleAnimation
        a.From = 432
        a.To = 120
        a.Duration = New Duration(TimeSpan.FromSeconds(1))
        imgAGNES.BeginAnimation(Image.HeightProperty, a)
        AddHandler dt.Tick, AddressOf PauseForMinimizing
        dt.Interval = New TimeSpan(0, 0, 1)
        dt.Start()
        Dim q = From c In VendorData.VendorInfo
                Where c.VendorType = 1 And c.Active = True
                Select c
        Dim ct As Integer = q.Count
        For Each c In q
            cboVendor.Items.Add(Trim(c.Name))
        Next
        cboVendor.SelectedValuePath = Content.ToString
        HoverDrop = New Effects.DropShadowEffect With {.Color = Color.FromRgb(235, 235, 235), .Direction = 200, .Opacity = 100, .ShadowDepth = 6, .BlurRadius = 2, .RenderingBias = Effects.RenderingBias.Performance}
        LeaveDrop = New Effects.DropShadowEffect With {.Color = Color.FromRgb(235, 235, 235), .Direction = 200, .Opacity = 100, .ShadowDepth = 4, .BlurRadius = 2, .RenderingBias = Effects.RenderingBias.Performance}

    End Sub

#End Region

#Region "Public Methods"
    Public Sub PauseForMinimizing(ByVal sender As Object, ByVal e As EventArgs)
        CommandManager.InvalidateRequerySuggested()
        tbCam.Text = "Okay!  Let's move on to CAM checks.  I'll hang out down here so I'm not in your way, but I'll continue to walk you through the process."
        dt.Stop()
        AddHandler dt2.Tick, AddressOf PauseBeforeCamChecks
        dt2.Interval = New TimeSpan(0, 0, 4)
        dt2.Start()
    End Sub

    Public Sub PauseBeforeCamChecks(ByVal sender As Object, ByVal e As EventArgs)
        CommandManager.InvalidateRequerySuggested()
        tbCam.Text = "Did you have any CAM checks to enter today?"
        dt2.Stop()
        tbYesCam.Visibility = Visibility.Visible
        tbNo.Visibility = Visibility.Visible
    End Sub
#End Region

#Region "Private Methods"
    Private Sub AddCamCheck(sender As Object, e As MouseButtonEventArgs) Handles tbYesCam.MouseDown
        With dtpDepositDate
            .DisplayDateStart = WCR.WeekStart
            .DisplayDateEnd = Now()
            .SelectedDate = Now()
        End With
        cboVendor.SelectedIndex = -1
        cboVendor.Text = ""
        tbCheckNumber.Text = ""
        tbCheckAmount.Text = ""
        tbCheckNotes.Text = ""
        ToggleEntryVisibility(1)
        tbCam.Text = "Please enter the CAM check information above - once you save, you can add more checks."
        tbSave.Visibility = Visibility.Visible
        tbYesCam.Visibility = Visibility.Hidden
        tbNo.Visibility = Visibility.Hidden
        cboVendor.Focus()
    End Sub

    Private Sub SaveCheck(sender As Object, e As MouseButtonEventArgs) Handles tbSave.MouseDown
        If ConfirmAndSave() = True Then
            Dim vid As Long = WCR.GetVendorID(cboVendor.Text)
            If WCR.CheckDoesNotExist(tbCheckNumber.Text, vid) = True And WCR.DoesNotExistInTempCheckList(tbCheckNumber.Text, vid) = True Then
                Dim dow As Byte = Weekday(dtpDepositDate.SelectedDate, FirstDayOfWeek.Friday)
                WCR.AddCamCheck(vid, cboVendor.Text, tbCheckNumber.Text, FormatNumber(tbCheckAmount.Text, 2), dtpDepositDate.SelectedDate, dow, tbCheckNotes.Text)
                tbCam.Text = "CAM check saved - did you want to add another?"
            Else
                Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Medium, AgnesMessageBox.MsgBoxLayout.FullText, AgnesMessageBox.MsgBoxType.OkOnly,
                                                18,, "Save cancelled.",, "A check for this vendor already exists.")
                amsg.ShowDialog()
                amsg.Close()
                tbCam.Text = ""
            End If
            With dtpDepositDate
                .DisplayDateStart = Now().AddDays(-14)
                .DisplayDateEnd = Now()
                .SelectedDate = Now()
            End With
            cboVendor.SelectedIndex = -1
            cboVendor.Text = ""
            tbCheckNumber.Text = ""
            tbCheckAmount.Text = ""
            tbCheckNotes.Text = ""
            ToggleEntryVisibility(1)
            tbSave.Visibility = Visibility.Hidden
            tbYesCam.Visibility = Visibility.Visible
            tbNo.Visibility = Visibility.Visible
            tbYesCam.Text = "Add Another"
            tbNo.Text = "I'm done!"
            ToggleEntryVisibility(0)
        End If
    End Sub

    Private Sub CamComplete(sender As Object, e As MouseButtonEventArgs) Handles tbNo.MouseDown
        Close()
    End Sub

    Private Sub HoverOver(sender As TextBlock, e As MouseEventArgs) Handles tbYesCam.MouseEnter, tbNo.MouseEnter, tbSave.MouseEnter
        sender.Foreground = New SolidColorBrush(Colors.Blue)
        sender.Effect = HoverDrop
    End Sub

    Private Sub HoverLeave(sender As TextBlock, e As MouseEventArgs) Handles tbYesCam.MouseLeave, tbNo.MouseLeave, tbSave.MouseLeave
        sender.Foreground = New SolidColorBrush(Colors.Black)
        sender.Effect = LeaveDrop
    End Sub

    Private Sub ToggleEntryVisibility(onoff As Boolean)
        Select Case onoff
            Case True
                txtVendorName.Visibility = Visibility.Visible
                txtCheckAmount.Visibility = Visibility.Visible
                txtCheckNotes.Visibility = Visibility.Visible
                txtCheckNumber.Visibility = Visibility.Visible
                txtDepositDate.Visibility = Visibility.Visible
                cboVendor.Visibility = Visibility.Visible
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
                cboVendor.Visibility = Visibility.Hidden
                tbCheckNumber.Visibility = Visibility.Hidden
                dtpDepositDate.Visibility = Visibility.Hidden
                tbCheckAmount.Visibility = Visibility.Hidden
                tbCheckNotes.Visibility = Visibility.Hidden
        End Select

    End Sub

    Private Function ConfirmAndSave() As Boolean
        '// Check for data in each field and validate format.  If all are valid, save via returning a TRUE value.  False indicates something is wrong.
        Dim VendorNameisValid As Boolean, CheckNumIsValid As Boolean, CheckAmtIsValid As Boolean, DepDateIsValid As Boolean, ReturnVal As Boolean
        If cboVendor.SelectedIndex > -1 Then
            VendorNameisValid = True
            rctVendorNm.Visibility = Visibility.Hidden
        Else
            rctVendorNm.Visibility = Visibility.Visible
        End If

        If tbCheckNumber.Text <> "" Then
            CheckNumIsValid = True
            rctCheckNumber.Visibility = Visibility.Hidden
        Else
            rctCheckNumber.Visibility = Visibility.Visible
        End If

        If tbCheckAmount.Text <> "" Then
            Try
                Dim amtvalid As Double = FormatNumber(tbCheckAmount.Text, 2)
                CheckAmtIsValid = True
                rctCheckAmt.Visibility = Visibility.Hidden
            Catch ex As Exception
                tbCheckAmount.SelectAll()
                tbCheckAmount.Focus()
                rctCheckAmt.Visibility = Visibility.Visible
            End Try
        Else
            rctCheckAmt.Visibility = Visibility.Visible
        End If
        If dtpDepositDate.Text <> "" Then
            Try
                Dim dtvalid As Date = FormatDateTime(dtpDepositDate.SelectedDate, DateFormat.ShortDate)
                DepDateIsValid = True
                rctDepositDt.Visibility = Visibility.Hidden
            Catch ex As Exception
                dtpDepositDate.Focus()
                rctDepositDt.Visibility = Visibility.Visible
            End Try
        Else
            rctDepositDt.Visibility = Visibility.Visible
        End If

        If tbCheckAmount.Text <> "" Or tbCheckNumber.Text <> "" Then
            If VendorNameisValid = True And CheckNumIsValid = True And CheckAmtIsValid = True And DepDateIsValid = True Then
                ReturnVal = True
                tbCam.Text = ""
            Else
                tbCam.Text = "It looks like the check information isn't quite right.  Please double check it and try again."
                ReturnVal = False
            End If
        End If
        Return ReturnVal
    End Function

    Private Sub ExitWCR(sender As Object, e As MouseButtonEventArgs) Handles btnExit.MouseDown
        Dim msgtxt As String
        If WCR.CamChecks.Count > 0 Then
            msgtxt = "Close WCR?  Your CAM checks will not be saved."
        Else
            msgtxt = "Close WCR?"
        End If
        Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Small, AgnesMessageBox.MsgBoxLayout.BottomOnly, AgnesMessageBox.MsgBoxType.YesNo,
                                                18, , "Out of balance!",, msgtxt)
        amsg.ShowDialog()
        If amsg.ReturnResult = "Yes" Then
            amsg.Close()
            WCRModule.UserClosed = True
            Close()
        Else
            amsg.Close()
        End If
    End Sub

#End Region

End Class
