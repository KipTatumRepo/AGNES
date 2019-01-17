Public Class NotificationWindow

#Region "Properties"
    Private NotificationCount As Byte
    Private _currentnotification As Byte
    Private Property CurrentNotification As Byte
        Get
            Return _currentnotification
        End Get
        Set(value As Byte)
            _currentnotification = value
            ShowNotification(value)
            tbTitle.Text = "Notifications (" & value + 1 & "/" & NotificationCount & ")"
            If value < NotificationCount - 1 Then
                imgNextNotification.Visibility = Visibility.Visible
                imgRightCheck.Visibility = Visibility.Collapsed
            Else
                imgNextNotification.Visibility = Visibility.Collapsed
                imgRightCheck.Visibility = Visibility.Visible
            End If
        End Set
    End Property

#End Region

#Region "Constructor"
    Public Sub New()
        InitializeComponent()
        NotificationCount = Notifications.Count()
        CurrentNotification = 0
    End Sub
#End Region

#Region "Public Methods"

#End Region

#Region "Private Methods"
    Private Sub ShowNotification(notificationnumber)
        flwNotificationText.Blocks.Clear()
        Dim notenum As Long = Notifications(notificationnumber)
        Dim qgn = (From gn In AGNESShared.Notifications
                   Where gn.PID = notenum
                   Select gn).ToList(0)

        Dim sections As String() = qgn.Message.Split(New Char() {"~"c})
        Dim section As String
        For Each section In sections
            Dim p As New Paragraph(New Run(section)) With {.TextAlignment = TextAlignment.Left, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 16}
            flwNotificationText.Blocks.Add(p)
        Next
    End Sub

    Private Sub Snoozle(sender As Object, e As MouseButtonEventArgs) Handles imgSnooze.MouseDown
        If CurrentNotification < NotificationCount - 1 Then
            CurrentNotification += 1
        Else
            Close()
        End If
    End Sub

    Private Sub Acknowledged(sender As Object, e As MouseButtonEventArgs) Handles imgRightCheck.MouseDown
        NotificationConfirmation()
        Close()
    End Sub

    Private Sub NotificationConfirmation()
        '// Write acknowledgement to database
        Dim SawNote As New NotificationConfirm
        With SawNote
            .Notification = Notifications(CurrentNotification)
            .UserId = My.Settings.UserID
            .ConfirmDate = Now()
        End With
        AGNESShared.NotificationConfirms.Add(SawNote)
        AGNESShared.SaveChanges()
    End Sub

    Private Sub imgNextNotification_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles imgNextNotification.MouseDown
        NotificationConfirmation()
        CurrentNotification += 1
    End Sub

#End Region

End Class
