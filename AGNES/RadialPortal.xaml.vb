Public Class RadialPortal
#Region "Properties"
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

#Region "Public Methods"
    Public Sub New()
        InitializeComponent()
        GetUserInfo()
        ConstructRadialMenu()
    End Sub

#End Region

#Region "Private Methods"

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
            Next
        End If
    End Sub

    Private Sub ConstructRadialMenu()
        '// Build the radial menu based on user access.  Placement is counterclockwise and base 0
        '// The ItemCount property requires a minimum of three items or else an overflow is triggered
        '// You can "trick" to show fewer by declaring 4 items and using positions 0 And 2 for placement of two
        '// or position 1 for placement of one item

        '// Build array of Module PIDs assigned to the user; bypass if access level is greater than user
        '// Count # of items & set ItemCount accordingly, which determines the size of the buttons and their hover event sizes

        Dim ct As Integer, UID As Integer = My.Settings.UserID, ULVL As Byte = My.Settings.UserLevel, Modules() As Long = Nothing

        '// IMPERSONATION - Change to whichever userID and Access Level needed to test
        'ULVL = 4
        'UID = 81
        '// IMPERSONATION

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
            Case "WCR"
                WCRModule.Runmodule()
            Case "Business Group CRM"
                BGCRMModule.Runmodule()
            Case "Flash"
                FlashModule.Runmodule()
        End Select
        Show()
    End Sub

#End Region

End Class