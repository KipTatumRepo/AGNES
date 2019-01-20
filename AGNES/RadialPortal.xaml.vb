Imports System.ComponentModel
Imports System.Reflection
Imports System.Windows.Threading

Public Class RadialPortal

#Region "Properties"
    Private dt As DispatcherTimer
    Private dt1 As DispatcherTimer
    Private dt2 As DispatcherTimer
    Private dt3 As DispatcherTimer
    Private ScaleAmt As Double
    Private RadialFull As Boolean
    Private bday As Boolean
    Private _buttonrest As Byte
    Private _buttonhover As Byte
    Private _itemcount As Byte
    Private Property ItemCount As Byte
        Get
            Return _itemcount
        End Get
        Set(value As Byte)
            _itemcount = value

            '// Determine button sizes for both base and mouseover states
            _buttonrest = 75 * (8 / ItemCount)
            If _buttonrest > 75 Then _buttonrest = 75
            _buttonhover = 85 * (8 / ItemCount)
            If _buttonhover > 85 Then _buttonhover = 85
        End Set
    End Property

#End Region

#Region "Constructor"
    Public Sub New()
        InitializeComponent()
        GetVersion()
        GetUserInfo()
        BaseModule.Runmodule()
        SessionLog(0)
        ConstructRadialMenu()
        PortalEasterEggs()
        CheckForNotifications()
        AddHandler imgAGNES.MouseEnter, AddressOf EnteredImg
        AddHandler imgAGNES.MouseLeave, AddressOf LeftImg
        AddHandler grdPortal.MouseLeave, AddressOf LeftWindow
    End Sub

#End Region

#Region "Private Methods"
    Private Sub EnteredImg()
        If dt IsNot Nothing Then Exit Sub
        dt = New DispatcherTimer()
        AddHandler dt.Tick, AddressOf ExpandRadial
        dt.Interval = New TimeSpan(0, 0, 0, 0, 300)
        dt.Start()
    End Sub

    Private Sub LeftImg()
        If dt IsNot Nothing Then
            dt.Stop()
            dt = Nothing
        End If
    End Sub

    Private Sub LeftWindow()
        If dt2 IsNot Nothing Then
            dt2.Stop()
            dt2 = Nothing
        End If
        If RadialFull = True Then
            dt2 = New DispatcherTimer()
            AddHandler dt2.Tick, AddressOf ContractRadial
            dt2.Interval = New TimeSpan(0, 0, 0, 0, 750)
            dt2.Start()
        End If
    End Sub

    Private Sub ExpandRadial()
        dt.Stop()
        dt = Nothing
        If (imgAGNES.IsMouseOver = True Or cnvRadialMenu.IsMouseOver = True) And Mouse.LeftButton <> MouseButtonState.Pressed And RadialFull = False Then
            ScaleAmt = 0.25
            dt1 = New DispatcherTimer()
            AddHandler dt1.Tick, AddressOf ScaleUp
            dt1.Interval = New TimeSpan(0, 0, 0, 0, 5)
            dt1.Start()
        End If

    End Sub

    Private Sub ContractRadial()
        dt2.Stop()
        dt2 = Nothing
        If (imgAGNES.IsMouseOver = False And cnvRadialMenu.IsMouseOver = False) And Mouse.LeftButton <> MouseButtonState.Pressed And RadialFull = True Then
            ScaleAmt = 1
            dt3 = New DispatcherTimer()
            AddHandler dt3.Tick, AddressOf ScaleDown
            dt3.Interval = New TimeSpan(0, 0, 0, 0, 5)
            dt3.Start()
        End If

    End Sub

    Private Sub ScaleUp()
        Dim st As ScaleTransform = New ScaleTransform(ScaleAmt, ScaleAmt)
        cnvRadialMenu.RenderTransform = st
        ScaleAmt += 0.05
        If ScaleAmt >= 1 Then
            dt1.Stop()
            dt1 = Nothing
            RadialFull = True
        End If
    End Sub

    Private Sub ScaleDown()
        Dim st As ScaleTransform = New ScaleTransform(ScaleAmt, ScaleAmt)
        cnvRadialMenu.RenderTransform = st
        ScaleAmt -= 0.05
        If ScaleAmt <= 0.25 Then
            dt3.Stop()
            dt3 = Nothing
            RadialFull = False
        End If
    End Sub

    Private Sub PortalEasterEggs()
        Dim m As Byte = Now.Month
        Dim d As Byte = Now.Day

        'Skynet day
        If m = 8 And d = 29 Then
            imgAGNES.Source = New BitmapImage(New Uri("Resources/AGNES800.png", UriKind.Relative))
            txtVersion.Text = "Model T-800"
        End If

        ' Mrs. Claus
        If m = 12 And (d > 14 And d < 25) Then
            imgAGNES.Source = New BitmapImage(New Uri("Resources/AGNESXmas.png", UriKind.Relative))
            txtVersion.Text = "Happy Holidays!"
        End If


    End Sub

    Private Sub CheckForNotifications()
        Dim MyGroup As Byte = 0
        Notifications.Clear()
        imgNotifications.Visibility = Visibility.Collapsed
        '// Determine group
        'TEST
        MyGroup = 0

        '// Capture active notifications
        Dim qng = From ng In AGNESShared.Notifications
                  Where ng.StartDate <= Today And
                      ng.EndDate > Today And
                      ng.Audience = MyGroup
                  Select ng

        For Each ng In qng
            '// Check if user has already seen notification, if it's a one-off
            If ng.OneOffNotification = True Then
                If CheckIfNotificationSeen(ng.PID) = False Then
                    Notifications.Add(ng.PID)
                End If
            Else
                Notifications.Add(ng.PID)
            End If

        Next

        If Notifications.Count = 0 Then Exit Sub
        imgNotifications.Visibility = Visibility.Visible

    End Sub

    Private Sub DisplayNotifications(sender As Object, e As MouseButtonEventArgs) Handles imgNotifications.PreviewMouseLeftButtonDown
        imgNotifications.Visibility = Visibility.Collapsed
        Dim ShowNotifications As New NotificationWindow
        ShowNotifications.ShowDialog()
        ShowNotifications.Close()
    End Sub

    Private Sub GetVersion()
        Dim v As Version = Assembly.GetExecutingAssembly().GetName().Version
        txtVersion.Text = "Version " & v.Major & "." & v.Minor
    End Sub

    Private Sub GetUserInfo()
        Dim ef As New AGNESSharedDataEntity

        '// Determine user based on network id and search for their access in the AGNES.Users database table
        '// If not found, throw an alert message notifying them of their lack of access, invoke garbage collection, and shut down
        '// If present, assign their information to custome application settings for global use

        Dim usr As String = Environment.UserName

        Dim qwl = From c In ef.Users
                  Where c.UserAlias = usr
                  Select c

        If qwl.Count = 0 Then
            Dim amsg = New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Medium, AgnesMessageBox.MsgBoxLayout.FullText,
                                            AgnesMessageBox.MsgBoxType.OkOnly, 18,, "Access denied",, "It appears that you don't have any access.  Please let your manager know that you need to be added.")
            amsg.ShowDialog()
            amsg.Close()
            GC.Collect()
            Close()
        Else
            For Each c In qwl
                With My.Settings
                    .UserName = c.UserName
                    .UserShortName = c.FirstName
                    .UserID = c.PID
                    .UserLevel = c.AccessLevelId
                End With
                Try
                    If Month(Now()) = Month(c.DOB) And Day(Now()) = Day(c.DOB) Then bday = True
                Catch ex As Exception

                End Try
            Next

            '// You say it's your birthday....it's my birthday, too yeaaaaaah
            If bday = True Then
                Dim bdaypg As Birthday = New Birthday
                bdaypg.ShowDialog()
            End If

#Region "Impersonation for testing"
            '// IMPERSONATION - Change to whichever userID and Access Level needed to test
            'With My.Settings
            '    .UserName = "Shawn Roland"
            '    .UserShortName = "Shawn"
            '    .UserID = 10095
            '    .UserLevel = 4
            'End With

            'With My.Settings
            '    .UserName = "Lexi"
            '    .UserShortName = "Aguilar"
            '    .UserID = 10162
            '    .UserLevel = 4
            'End With

            'With My.Settings
            '    .UserName = "Ian"
            '    .UserShortName = "Ian"
            '    .UserID = 10132
            '    .UserLevel = 4
            'End With

            'With My.Settings
            '    .UserName = "Bryan"
            '    .UserShortName = "Robinson"
            '    .UserID = 10154
            '    .UserLevel = 4
            'End With

            'With My.Settings
            '    .UserName = "Mike Shea"
            '    .UserShortName = "Mike"
            '    .UserID = 10086
            '    .UserLevel = 4
            'End With

            'With My.Settings
            '    .UserName = "Nicholas Pagel"
            '    .UserShortName = "Nick"
            '    .UserID = 81
            '    .UserLevel = 4
            'End With

            'With My.Settings
            '    .UserName = "James Normandin"
            '    .UserShortName = "Jim"
            '    .UserID = 10096
            '    .UserLevel = 4
            'End With

            'With My.Settings
            '    .UserName = "Aloka Bhatt"
            '    .UserShortName = "Aloka"
            '    .UserID = 10109
            '    .UserLevel = 4
            'End With

            'With My.Settings
            '    .UserName = "Tracy Ducken"
            '    .UserShortName = "Tracy"
            '    .UserID = 10117
            '    .UserLevel = 4
            'End With

            'With My.Settings
            '    .UserName = "Brian Unruh"
            '    .UserShortName = "Brian"
            '    .UserID = 10124
            '    .UserLevel = 4
            'End With

            'With My.Settings
            '    .UserName = "Julie Ogawa"
            '    .UserShortName = "Julie"
            '    .UserID = 10135
            '    .UserLevel = 4
            'End With

            'With My.Settings
            '    .UserName = "Sue Pettengill"
            '    .UserShortName = "Sue"
            '    .UserID = 10141
            '    .UserLevel = 4
            'End With
#End Region

        End If

    End Sub

    Private Sub ConstructRadialMenu()
        '// Build the radial menu based on user access.  Placement is counterclockwise and base 0
        '// The ItemCount property requires a minimum of three items or else an overflow is triggered

        '// Build array of Module PIDs assigned to the user; bypass if access level is greater than user
        '// Count # of items & set ItemCount accordingly, which determines the size of the buttons and their hover event sizes

        Dim ct As Integer, UID As Integer = My.Settings.UserID, ULVL As Byte = My.Settings.UserLevel, Modules() As Long = Nothing



        Dim ef As New AGNESSharedDataEntity
        Select Case ULVL
            '// Owner level - Full control over all elements of AGNES
            Case 1
                Dim qwl = From c In ef.Modules Select c
                ItemCount = qwl.Count
                ReDim Modules(ItemCount - 1)
                ct = 0
                For Each c In qwl
                    Modules(ct) = c.PID
                    ct += 1
                Next

            '// Admin level - Control over most elements of AGNES - can add/delete/modify users
            Case 2
                Dim qwl = From c In ef.Modules Select c
                ItemCount = qwl.Count
                ReDim Modules(ItemCount - 1)
                ct = 0
                For Each c In qwl
                    Modules(ct) = c.PID
                    ct += 1
                Next

            '// Superuser level - Access to all modules in AGNES - no user control
            Case 3
                Dim qwl = From c In ef.Modules Where c.PID > 1 Select c
                ItemCount = qwl.Count
                ReDim Modules(ItemCount - 1)
                ct = 0
                For Each c In qwl
                    Modules(ct) = c.PID
                    ct += 1
                Next

            '// User level - Access to individually defined modules and units in AGNES
            Case 4
                Dim qwl = From c In ef.ModulesUsers_Join Where c.UserId = UID Select c
                ItemCount = qwl.Count
                ReDim Modules(ItemCount - 1)
                ct = 0
                For Each c In qwl
                    Modules(ct) = c.ModuleId
                    ct += 1
                Next
        End Select

        '// Create buttons from array
        For ct = 0 To ItemCount - 1
            Dim m As Long = Modules(ct)
            Dim modul = From c In ef.Modules Where c.PID = m Select c

            Dim y As Byte = modul.Count
            For Each c In modul
                PlaceMenuItem(ct, c.ModuleName, c.ModuleName, "Resources/" & c.ImgResource & ".png")
            Next
        Next
    End Sub

    Private Sub PlaceMenuItem(item, associatedmodule, tooltip, moduleimage)

        '// Determine position of the button on the dial radius
        Dim ang As Double = item * ((2 * Math.PI) / ItemCount)
        Dim rad As Integer = cnvRadialMenu.Height / 2
        Dim x As Integer = ((Math.Cos(ang) * rad) + rad)
        Dim y As Integer = (rad - (Math.Sin(ang) * rad))

        '// Create button (image element) object and add event handlers
        Dim img As New Image With {.Source = New BitmapImage(New Uri(moduleimage, UriKind.Relative)), .Name = "btnRadial" & item,
            .Height = _buttonrest, .Width = _buttonrest, .Stretch = Stretch.UniformToFill, .ToolTip = tooltip, .Tag = associatedmodule}
        x -= img.Width / 2 : y -= img.Height / 2
        img.Margin = New Thickness(x, y, 0, 0)
        cnvRadialMenu.Children.Add(img)
        AddHandler img.MouseEnter, AddressOf ModuleMouseHover
        AddHandler img.MouseLeave, AddressOf ModuleMouseLeave
        AddHandler img.MouseLeftButtonDown, AddressOf ModuleSelect
    End Sub

    Private Sub DragViaLeftMouse(sender As Object, e As MouseButtonEventArgs)
        DragMove()
    End Sub

    Private Sub CloseAGNES(sender As Object, e As MouseButtonEventArgs)
        Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Medium, AgnesMessageBox.MsgBoxLayout.BottomOnly,
                                        AgnesMessageBox.MsgBoxType.YesNo, 18,,,, "Close AGNES?")
        amsg.ShowDialog()
        If amsg.ReturnResult = "Yes" Then
            amsg.Close()
            GC.Collect()
            Close()
        Else
            amsg.Close()
        End If
    End Sub

    Private Sub ModuleMouseHover(sender As Object, e As MouseEventArgs)
        Dim s As Image = sender
        s.Height = _buttonhover
        s.Width = _buttonhover
        Dim l As Integer = s.Margin.Left - 5
        Dim t As Integer = s.Margin.Top - 5
        s.Margin = New Thickness(l, t, 0, 0)
    End Sub

    Private Sub ModuleMouseLeave(sender As Object, e As MouseEventArgs)
        Dim s As Image = sender
        s.Height = _buttonrest
        s.Width = _buttonrest
        Dim l As Integer = s.Margin.Left + 5
        Dim t As Integer = s.Margin.Top + 5
        s.Margin = New Thickness(l, t, 0, 0)
    End Sub

    Private Sub ModuleSelect(sender As Object, e As MouseEventArgs)
        Dim s As Image = sender
        Hide()
        Select Case s.Tag
            Case "Power BI"
                Process.Start("http://www.powerbi.com")
            Case "WCR"
                WCRModule.Runmodule()
            Case "Business Group CRM"
                BGCRMModule.Runmodule()
            Case "Flash"
                FlashModule.Runmodule(-1)
            Case "Flash Status"
                FlashStatusModule.Runmodule()
            Case "Forecast"
                ForecastModule.Runmodule()
            Case "Admin"
                AdminModule.Runmodule()
            Case "Training"
                TrainingModule.Runmodule()
            Case "Vendor Manager"
                VendorModule.Runmodule()
            Case "Event Journal"
                ''//TEST
                ' If My.Computer.Keyboard.CtrlKeyDown Then WOPRModule.RunModule()
                'EventJournal.Runmodule()
                ''//TEST
            Case "HR Manager"
                CSharpModules.RunHRMgrModule()
        End Select
        Show()
    End Sub

    Private Sub ClosingEvents(sender As Object, e As CancelEventArgs) Handles Me.Closing
        SessionLog(1)
    End Sub

    Private Sub txtVersion_PreviewMouseLeftButtonDown(sender As Object, e As MouseButtonEventArgs) Handles txtVersion.PreviewMouseLeftButtonDown
        If e.ClickCount = 2 Then
            OpenMicrosoftWord("\\compasspowerbi\compassbiapplications\agnes\AGNESDocumentation.docx")
        End If
    End Sub

    Private Sub OpenMicrosoftWord(ByVal f As String)
        Dim startInfo As New ProcessStartInfo With {.FileName = "WINWORD.EXE", .Arguments = f}
        Process.Start(startInfo)
    End Sub

    Private Function CheckIfNotificationSeen(NotificationId As Long) As Boolean
        Dim nscount As Integer = (From ns In AGNESShared.NotificationConfirms
                                  Where ns.Notification = NotificationId And
                                  ns.UserId = My.Settings.UserID
                                  Select ns).Count()

        If nscount = 0 Then Return False
        Return True

    End Function

#End Region

End Class