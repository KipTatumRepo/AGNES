Public Class RadialPortal
    Private _buttonrest As Byte
    Private _buttonhover As Byte
    Private _itemcount As Byte
    Private Property ItemCount As Byte
        Get
            Return _itemcount
        End Get
        Set(value As Byte)
            _itemcount = value
            _buttonrest = 75 * (8 / ItemCount)
            If _buttonrest > 75 Then _buttonrest = 75
            _buttonhover = 85 * (8 / ItemCount)
            If _buttonhover > 85 Then _buttonhover = 85

        End Set
    End Property

    Public Sub New()
        InitializeComponent()
        GetUserInfo()
        ConstructRadialMenu()
    End Sub

    Private Sub GetUserInfo()
        Dim ef As New AGNESSharedData
        Dim usr As String = Environment.UserName
        Dim qwl = From c In ef.UserLists
                  Where c.UserAlias = usr
                  Select c
        If qwl.Count = 0 Then
            Dim amsg1 = New AgnesMessageBox With
                {.FntSz = 18, .MsgSize = AgnesMessageBox.MsgBoxSize.Medium, .MsgType = AgnesMessageBox.MsgBoxType.OkOnly,
                .TextStyle = AgnesMessageBox.MsgBoxLayout.FullText, .TopSectionText = "Access denied",
                .BottomSectionText = "It appears that you don't have any access.  Please let your manager know that you need to be added."}
            amsg1.ShowDialog()
            amsg1.Close()
            GC.Collect()
            Close()
        Else
            For Each c In qwl
                With My.Settings
                    .UserName = Trim(c.UserName)
                    .UserShortName = Trim(c.FirstName)
                    .UserID = c.PID
                    .UserLevel = c.AccessLevelId
                End With
            Next
        End If
    End Sub

    Private Sub ConstructRadialMenu()
        '// Placement is counterclockwise and base 0
        '// The ItemCount property requires a minimum of three items or else an overflow is triggered
        '// You can "trick" to show fewer by declaring 4 items and using positions 0 And 2 for placement of two
        '// or position 1 for placement of one item

        '// Build array of Module PIDs assigned to the user; bypass if access level is greater than user
        '// Count # of items & set ItemCount accordingly
        '// Create buttons from array
        Dim ct As Integer, UID As Integer = My.Settings.UserID, ULVL As Byte = My.Settings.UserLevel, Modules() As Long

        '// IMPERSONATION - Change to whichever userID and Access Level needed to test
        ' ULVL = 3
        ' UID = 80
        '// IMPERSONATION

        Dim ef As New AGNESSharedData
        Select Case ULVL
            Case 1
                Dim qwl = From c In ef.ModuleLists
                          Select c
                ItemCount = qwl.Count
                ReDim Modules(ItemCount - 1)
                ct = 0
                For Each c In qwl
                    Modules(ct) = c.PID
                    ct += 1
                Next
            Case 2
                Dim qwl = From c In ef.ModuleLists
                          Select c
                ItemCount = qwl.Count
                ReDim Modules(ItemCount - 1)
                ct = 0
                For Each c In qwl
                    Modules(ct) = c.PID
                    ct += 1
                Next
            Case 3
                Dim qwl = From c In ef.ModuleLists
                          Where c.PID > 1
                          Select c
                ItemCount = qwl.Count
                ReDim Modules(ItemCount - 1)
                ct = 0
                For Each c In qwl
                    Modules(ct) = c.PID
                    ct += 1
                Next
            Case Else
                Dim qwl = From c In ef.ModuleUser_Join
                          Where c.UserId = UID
                          Select c
                ItemCount = qwl.Count
                ReDim Modules(ItemCount - 1)
                ct = 0
                For Each c In qwl
                    Modules(ct) = c.ModuleId
                    ct += 1
                Next
        End Select

        For ct = 0 To ItemCount - 1
            Dim m As Long = Modules(ct)
            Dim modul = From c In ef.ModuleLists
                        Where c.PID = m
                        Select c

            Dim y As Byte = modul.Count
            For Each c In modul
                PlaceMenuItem(ct, c.ModuleName, c.ModuleName, "Resources/" & c.ImgResource & ".png")
            Next
        Next

    End Sub

    Private Sub DragViaLeftMouse(sender As Object, e As MouseButtonEventArgs)
        DragMove()
    End Sub

    Private Sub CloseAGNES(sender As Object, e As MouseButtonEventArgs)
        Dim amsg As New AgnesMessageBox With
            {.FntSz = 18, .MsgSize = AgnesMessageBox.MsgBoxSize.Medium, .MsgType = AgnesMessageBox.MsgBoxType.YesNo,
            .TextStyle = AgnesMessageBox.MsgBoxLayout.FullText, .BottomSectionText = "Close AGNES?", .AllowCopy = True}
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

    Private Sub PlaceMenuItem(item, associatedmodule, tooltip, moduleimage)
        Dim ang As Double = item * ((2 * Math.PI) / ItemCount)
        Dim rad As Integer = cnvRadialMenu.Height / 2
        Dim x As Integer = ((Math.Cos(ang) * rad) + rad)
        Dim y As Integer = (rad - (Math.Sin(ang) * rad))
        Dim img As New Image With {.Source = New BitmapImage(New Uri(moduleimage, UriKind.Relative)), .Name = "btnRadial" & item,
            .Height = _buttonrest, .Width = _buttonrest, .Stretch = Stretch.UniformToFill, .ToolTip = tooltip, .Tag = associatedmodule}
        x -= img.Width / 2 : y -= img.Height / 2
        img.Margin = New Thickness(x, y, 0, 0)
        cnvRadialMenu.Children.Add(img)
        AddHandler img.MouseEnter, AddressOf ModuleMouseHover
        AddHandler img.MouseLeave, AddressOf ModuleMouseLeave
        AddHandler img.MouseLeftButtonDown, AddressOf ModuleSelect
    End Sub

End Class