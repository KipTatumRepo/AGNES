Public Class AgnesMessageBox
    Private _msgsize As Byte
    Public Property MsgSize As Byte
        Get
            Return _msgsize
        End Get
        Set(value As Byte)
            _msgsize = value
            Select Case value
                Case 0                  '// Large
                    ScaleMessagebox(400, 800, 100, 100, 71, 31, 0, 0, 100, 667, 71, 31, 0, 0, 100, 545, 193, 31, 0, 0, 170, 667, 71, 137, 0, 0,
                                 40, 112, 115, 322, 569, 37, 350, 322, 334, 37, 626, 322, 62, 37, 214, 322, 474, 37, 492, 322, 196, 37, 24)
                Case 1                  '// Medium
                    ScaleMessagebox(264, 528, 66, 66, 43, 22, 0, 0, 66, 440, 43, 22, 0, 0, 66, 360, 123, 22, 0, 0, 112, 440, 43, 93, 0, 0,
                                    26, 74, 43, 210, 411, 28, 228, 210, 226, 28, 409, 210, 45, 28, 134, 210, 320, 28, 320, 210, 134, 28, 18)
                Case 2                  '// Small
                    ScaleMessagebox(174, 348, 44, 44, 33, 16, 0, 0, 44, 290, 33, 16, 0, 0, 44, 238, 77, 16, 0, 0, 74, 290, 33, 60, 0, 0,
                                    17, 49, 33, 139, 266, 18, 157, 139, 142, 18, 274, 139, 25, 18, 95, 139, 204, 18, 216, 139, 83, 18, 18)
            End Select
        End Set
    End Property

    Private _msgtype As Byte
    Public Property MsgType As Byte
        Get
            Return _msgtype
        End Get
        Set(value As Byte)
            _msgtype = value
            Select Case value
                Case 0                  '// Okay/Cancel
                    OkCancelFormat()
                Case 1                  '// Yes/No
                    YesNoFormat()
                Case 2                  '// Yes/No/Cancel
                    YesNoCancelFormat()
                Case 3
                    OkOnlyFormat()      '// Just OK option
            End Select
        End Set
    End Property

    Private _textstyle As Byte
    Public Property TextStyle As Byte
        Get
            Return _textstyle
        End Get
        Set(value As Byte)
            _textstyle = value
            Select Case value
                Case 0      '// Full text sections
                    ShowFullText()
                Case 1      '// Image and offset top text section
                    ShowTextandImage()
                Case 2      '// Top section only
                    ShowTopOnly()
                Case 3      '// Bottom section only
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
            'TODO: Copy feature enable on messagebox
        End Set
    End Property

    Public Property ReturnResult As String

    Public Property Caller As Window
    Private Sub ScaleMessagebox(win_h, win_w, img_h, img_w, img_ml, img_mt, img_mr, img_mb, ts_h, ts_w, ts_ml, ts_mt, ts_mr, ts_mb, tos_h,
                             tos_w, tos_ml, tos_mt, tos_mr, tos_mb, bs_h, bs_w, bs_ml, bs_mt, bs_mr, bs_mb, but_h, but_w, b1_ml, b1_mt, b1_mr, b1_mb,
                             b2_ml, b2_mt, b2_mr, b2_mb, b3_ml, b3_mt, b3_mr, b3_mb, b4_ml, b4_mt, b4_mr, b4_mb, b5_ml, b5_mt, b5_mr, b5_mb, fs)
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
            .FontSize = fs
        End With
        With tbTopOffsetSection
            .Height = tos_h
            .Width = tos_w
            .Margin = New Thickness(tos_ml, tos_mt, tos_mr, tos_mb)
            .FontSize = fs
        End With
        With tbBottomSection
            .Height = bs_h
            .Width = bs_w
            .Margin = New Thickness(bs_ml, bs_mt, bs_mr, bs_mb)
            .FontSize = fs
        End With
        With brdButtonOne
            .Height = but_h
            .Width = but_w
            .Margin = New Thickness(b1_ml, b1_mt, b1_mr, b1_mb)
        End With
        tbButtonOneText.FontSize = fs
        With brdButtonTwo
            .Height = but_h
            .Width = but_w
            .Margin = New Thickness(b2_ml, b2_mt, b2_mr, b2_mb)
        End With
        tbButtonTwoText.FontSize = fs
        With brdButtonThree
            .Height = but_h
            .Width = but_w
            .Margin = New Thickness(b3_ml, b3_mt, b3_mr, b3_mb)
        End With
        tbButtonThreeText.FontSize = fs
        With brdButtonFour
            .Height = but_h
            .Width = but_w
            .Margin = New Thickness(b4_ml, b4_mt, b4_mr, b4_mb)
        End With
        tbButtonFourText.FontSize = fs
        With brdButtonFive
            .Height = but_h
            .Width = but_w
            .Margin = New Thickness(b5_ml, b5_mt, b5_mr, b5_mb)
        End With
        tbButtonFiveText.FontSize = fs
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
        tbTopOffsetSection.Visibility = Visibility.Hidden
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

    Private Sub ClickOne(sender As Object, e As MouseButtonEventArgs) Handles tbButtonOneText.PreviewMouseDown
        ReturnResult = tbButtonOneText.Text
        Hide()
    End Sub

    Private Sub ClickTwo(sender As Object, e As MouseButtonEventArgs) Handles tbButtonTwoText.PreviewMouseDown
        ReturnResult = tbButtonTwoText.Text
        Hide()
    End Sub

    Private Sub ClickThree(sender As Object, e As MouseButtonEventArgs) Handles tbButtonThreeText.PreviewMouseDown
        ReturnResult = tbButtonThreeText.Text
        Hide()
    End Sub

    Private Sub ClickFour(sender As Object, e As MouseButtonEventArgs) Handles tbButtonFourText.PreviewMouseDown
        ReturnResult = tbButtonFourText.Text
        Hide()
    End Sub

    Private Sub ClickFive(sender As Object, e As MouseButtonEventArgs) Handles tbButtonFiveText.PreviewMouseDown
        ReturnResult = tbButtonFiveText.Text
        Hide()
    End Sub
End Class
