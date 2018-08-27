Imports System.Windows.Threading
Public Class AgnesMessageBox

    Public Enum MsgBoxSize
        Small
        Medium
        Large
    End Enum

    Public Enum MsgBoxLayout
        FullText
        TextAndImage
        TopOnly
        BottomOnly
    End Enum

    Public Enum MsgBoxType
        OkayCancel
        YesNo
        YesNoCancel
        OkOnly
    End Enum

    Public Enum ImageType
        Danger
        Alert
    End Enum
    Public Property FntSz As Byte
    Private dt As DispatcherTimer = New DispatcherTimer()
    Private _topsectiontext As String
    Private CopiedText As String
    Public Property TopSectionText As String
        Get
            Return _topsectiontext
        End Get
        Set(value As String)
            _topsectiontext = value
            tbTopSection.Text = value
        End Set
    End Property

    Private _offsettext As String
    Public Property OffsetSectionText As String
        Get
            Return _offsettext
        End Get
        Set(value As String)
            _offsettext = value
            tbTopOffsetSection.Text = value
        End Set
    End Property

    Private _bottomtext As String
    Public Property BottomSectionText As String
        Get
            Return _bottomtext
        End Get
        Set(value As String)
            _bottomtext = value
            tbBottomSection.Text = value
        End Set
    End Property

    Private _imgsrc As ImageType
    Public Property ImageSource As ImageType
        Get
            Return _imgsrc
        End Get
        Set(value As ImageType)
            _imgsrc = value
            Select Case value
                Case ImageType.Alert
                    Dim img As New Image With {.Source = New BitmapImage(New Uri("Resources/BusinessGroup.png", UriKind.Relative)),
                        .Height = imgAlert.Height, .Width = imgAlert.Width, .Stretch = Stretch.Fill}
                    img.Margin = New Thickness(imgAlert.Margin.Left, imgAlert.Margin.Top, imgAlert.Margin.Right, imgAlert.Margin.Bottom)
                    imgAlert = img
                    'imgAlert.Source = New BitmapImage(New Uri("Resources/BusinessGroup.png", UriKind.Relative))
                Case ImageType.Danger
                    Dim img As New Image With {.Source = New BitmapImage(New Uri("Resources/danger.png", UriKind.Relative)),
                        .Height = imgAlert.Height, .Width = imgAlert.Width, .Stretch = Stretch.Fill}
                    img.Margin = New Thickness(imgAlert.Margin.Left, imgAlert.Margin.Top, imgAlert.Margin.Right, imgAlert.Margin.Bottom)
                    imgAlert = img
            End Select
            'TODO: FIGURE OUT THE RUNTIME IMGSOURCE METHOD
        End Set
    End Property
    Public Property ReturnResult As String

    Private _msgsize As MsgBoxSize
    Public Property MsgSize As MsgBoxSize
        Get
            Return _msgsize
        End Get
        Set(value As MsgBoxSize)
            _msgsize = value
            Select Case value
                Case MsgBoxSize.Large
                    ScaleMessagebox(400, 800, 100, 100, 71, 31, 0, 0, 100, 667, 71, 31, 0, 0, 100, 545, 193, 31, 0, 0, 170, 667, 71, 137, 0, 0,
                                 40, 112, 115, 322, 569, 37, 350, 322, 334, 37, 626, 322, 62, 37, 214, 322, 474, 37, 492, 322, 196, 37, 14, 6, 24)
                Case MsgBoxSize.Medium
                    ScaleMessagebox(264, 528, 66, 66, 43, 22, 0, 0, 66, 440, 43, 22, 0, 0, 66, 360, 123, 22, 0, 0, 112, 440, 43, 93, 0, 0,
                                    26, 74, 43, 210, 411, 28, 228, 210, 226, 28, 409, 210, 45, 28, 134, 210, 320, 28, 320, 210, 134, 28, 9, 4, 18)
                Case MsgBoxSize.Small
                    ScaleMessagebox(174, 348, 44, 44, 33, 16, 0, 0, 44, 290, 33, 16, 0, 0, 44, 238, 77, 16, 0, 0, 74, 290, 33, 60, 0, 0,
                                    17, 49, 33, 139, 266, 18, 157, 139, 142, 18, 274, 139, 25, 18, 95, 139, 204, 18, 216, 139, 83, 18, 6, 3, 10)
            End Select
        End Set
    End Property

    Private _msgtype As MsgBoxType
    Public Property MsgType As MsgBoxType
        Get
            Return _msgtype
        End Get
        Set(value As MsgBoxType)
            _msgtype = value
            Select Case value
                Case MsgBoxType.OkayCancel
                    OkCancelFormat()
                Case MsgBoxType.YesNo
                    YesNoFormat()
                Case MsgBoxType.YesNoCancel
                    YesNoCancelFormat()
                Case MsgBoxType.OkOnly
                    OkOnlyFormat()
            End Select
        End Set
    End Property

    Private _textstyle As MsgBoxLayout
    Public Property TextStyle As MsgBoxLayout
        Get
            Return _textstyle
        End Get
        Set(value As MsgBoxLayout)
            _textstyle = value
            Select Case value
                Case MsgBoxLayout.FullText            '// Full text sections
                    ShowFullText()
                Case MsgBoxLayout.TextAndImage        '// Image and offset top text section
                    ShowTextandImage()
                Case MsgBoxLayout.TopOnly             '// Top section only
                    ShowTopOnly()
                Case MsgBoxLayout.BottomOnly          '// Bottom section only
                    ShowBottomOnly()
            End Select
        End Set
    End Property

    Private _allowcopy As Boolean

    Public Property AllowCopy As Boolean
        Get
            Return _allowcopy
        End Get
        Set(value As Boolean)
            _allowcopy = value
        End Set
    End Property

    Private Sub ScaleMessagebox(win_h, win_w, img_h, img_w, img_ml, img_mt, img_mr, img_mb, ts_h, ts_w, ts_ml, ts_mt, ts_mr, ts_mb, tos_h,
                             tos_w, tos_ml, tos_mt, tos_mr, tos_mb, bs_h, bs_w, bs_ml, bs_mt, bs_mr, bs_mb, but_h, but_w, b1_ml, b1_mt, b1_mr, b1_mb,
                             b2_ml, b2_mt, b2_mr, b2_mb, b3_ml, b3_mt, b3_mr, b3_mb, b4_ml, b4_mt, b4_mr, b4_mb, b5_ml, b5_mt, b5_mr, b5_mb, dsr, dsd, bfs)
        Height = win_h
        Width = win_w
        With imgAlert
            .Height = img_h
            .Width = img_w
            .Margin = New Thickness(img_ml, img_mt, img_mr, img_mb)
        End With
        With tbTopSection
            .Height = ts_h
            .Width = ts_w
            .Margin = New Thickness(ts_ml, ts_mt, ts_mr, ts_mb)
            .FontSize = FntSz
        End With
        With tbTopOffsetSection
            .Height = tos_h
            .Width = tos_w
            .Margin = New Thickness(tos_ml, tos_mt, tos_mr, tos_mb)
            .FontSize = FntSz
        End With
        With tbBottomSection
            .Height = bs_h
            .Width = bs_w
            .Margin = New Thickness(bs_ml, bs_mt, bs_mr, bs_mb)
            .FontSize = FntSz
        End With
        With brdButtonOne
            .Height = but_h
            .Width = but_w
            .Margin = New Thickness(b1_ml, b1_mt, b1_mr, b1_mb)
        End With
        tbButtonOneText.FontSize = bfs
        With brdButtonTwo
            .Height = but_h
            .Width = but_w
            .Margin = New Thickness(b2_ml, b2_mt, b2_mr, b2_mb)
        End With
        tbButtonTwoText.FontSize = bfs
        With brdButtonThree
            .Height = but_h
            .Width = but_w
            .Margin = New Thickness(b3_ml, b3_mt, b3_mr, b3_mb)
        End With
        tbButtonThreeText.FontSize = FntSz
        With brdButtonFour
            .Height = but_h
            .Width = but_w
            .Margin = New Thickness(b4_ml, b4_mt, b4_mr, b4_mb)
        End With
        tbButtonFourText.FontSize = bfs
        With brdButtonFive
            .Height = but_h
            .Width = but_w
            .Margin = New Thickness(b5_ml, b5_mt, b5_mr, b5_mb)
        End With
        tbButtonFiveText.FontSize = bfs
        Effect = New Effects.DropShadowEffect With {.Direction = 240, .Opacity = 0.7, .BlurRadius = dsr, .ShadowDepth = dsd}
    End Sub

    Private Sub OkOnlyFormat()
        Dim tbTwo As TextBlock = brdButtonTwo.Child
        brdButtonOne.Visibility = Visibility.Hidden
        brdButtonThree.Visibility = Visibility.Hidden
        brdButtonFour.Visibility = Visibility.Hidden
        brdButtonFive.Visibility = Visibility.Hidden
        tbTwo.Text = "Okay"
    End Sub

    Private Sub OkCancelFormat()
        Dim tbFour As TextBlock = brdButtonFour.Child, tbFive As TextBlock = brdButtonFive.Child
        brdButtonOne.Visibility = Visibility.Hidden
        brdButtonTwo.Visibility = Visibility.Hidden
        brdButtonThree.Visibility = Visibility.Hidden
        brdButtonFour.Visibility = Visibility.Visible
        brdButtonFive.Visibility = Visibility.Visible
        tbFour.Text = "Okay"
        tbFive.Text = "Cancel"
    End Sub

    Private Sub YesNoFormat()
        Dim tbFour As TextBlock = brdButtonFour.Child, tbFive As TextBlock = brdButtonFive.Child
        brdButtonOne.Visibility = Visibility.Hidden
        brdButtonTwo.Visibility = Visibility.Hidden
        brdButtonThree.Visibility = Visibility.Hidden
        brdButtonFour.Visibility = Visibility.Visible
        brdButtonFive.Visibility = Visibility.Visible
        tbFour.Text = "Yes"
        tbFive.Text = "No"
    End Sub

    Private Sub YesNoCancelFormat()
        Dim tbOne As TextBlock = brdButtonOne.Child, tbTwo As TextBlock = brdButtonTwo.Child, tbThree As TextBlock = brdButtonThree.Child
        tbOne.Text = "Yes"
        tbTwo.Text = "No"
        tbThree.Text = "Cancel"
        brdButtonOne.Visibility = Visibility.Visible
        brdButtonTwo.Visibility = Visibility.Visible
        brdButtonThree.Visibility = Visibility.Visible
        brdButtonFour.Visibility = Visibility.Hidden
        brdButtonFive.Visibility = Visibility.Hidden
    End Sub

    Private Sub ShowFullText()
        tbTopOffsetSection.Visibility = Visibility.Hidden
        imgAlert.Visibility = Visibility.Hidden
        tbTopSection.Visibility = Visibility.Visible
        tbBottomSection.Visibility = Visibility.Visible
    End Sub

    Private Sub ShowTextandImage()
        tbTopOffsetSection.Visibility = Visibility.Visible
        imgAlert.Visibility = Visibility.Visible
        tbTopSection.Visibility = Visibility.Hidden
        tbBottomSection.Visibility = Visibility.Visible
    End Sub

    Private Sub ShowTopOnly()
        tbTopOffsetSection.Visibility = Visibility.Hidden
        imgAlert.Visibility = Visibility.Hidden
        tbTopSection.Visibility = Visibility.Visible
        tbBottomSection.Visibility = Visibility.Hidden
    End Sub

    Private Sub ShowBottomOnly()
        tbTopOffsetSection.Visibility = Visibility.Hidden
        imgAlert.Visibility = Visibility.Hidden
        tbTopSection.Visibility = Visibility.Hidden
        tbBottomSection.Visibility = Visibility.Visible
    End Sub

    Private Sub HoverOver(sender As Object, e As MouseEventArgs) Handles brdButtonOne.MouseEnter, brdButtonTwo.MouseEnter, brdButtonThree.MouseEnter, brdButtonFour.MouseEnter, brdButtonFive.MouseEnter
        Dim b As Border = sender, t As TextBlock = b.Child
        Dim mdse As New Effects.DropShadowEffect With {.BlurRadius = 8, .Direction = 270, .ShadowDepth = 6, .Opacity = 0.75}
        b.Effect = mdse
        t.Foreground = New SolidColorBrush(Colors.Yellow)
    End Sub

    Private Sub HoverLeave(sender As Object, e As MouseEventArgs) Handles brdButtonOne.MouseLeave, brdButtonTwo.MouseLeave, brdButtonThree.MouseLeave, brdButtonFour.MouseLeave, brdButtonFive.MouseLeave
        Dim b As Border = sender, t As TextBlock = b.Child
        Dim mdse As New Effects.DropShadowEffect With {.BlurRadius = 6, .Direction = 270, .ShadowDepth = 4, .Opacity = 0.5}
        b.Effect = mdse
        t.Foreground = New SolidColorBrush(Colors.White)
    End Sub

    Private Sub ClickButton(sender As Object, e As MouseButtonEventArgs) Handles tbButtonOneText.PreviewMouseLeftButtonDown, tbButtonTwoText.PreviewMouseLeftButtonDown, tbButtonThreeText.PreviewMouseLeftButtonDown, tbButtonFourText.PreviewMouseLeftButtonDown, tbButtonFiveText.PreviewMouseLeftButtonDown
        Dim b As New TextBlock
        b = sender
        ReturnResult = b.Text
        Hide()
    End Sub

    Private Sub CopyText(sender As Object, e As MouseButtonEventArgs) Handles tbBottomSection.PreviewMouseRightButtonDown
        If _allowcopy = True Then
            CopiedText = tbBottomSection.Text
            Clipboard.SetText(CopiedText)
            tbBottomSection.Text = "Text copied"
            AddHandler dt.Tick, AddressOf PauseForCopyNotify
            dt.Interval = New TimeSpan(0, 0, 1)
            dt.Start()
        End If
    End Sub

    Public Sub PauseForCopyNotify(ByVal sender As Object, ByVal e As EventArgs)
        CommandManager.InvalidateRequerySuggested()
        dt.Stop()
        tbBottomSection.Text = CopiedText
    End Sub

End Class