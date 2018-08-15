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
                    LargeVersion()
                Case 1                  '// Medium
                    MediumVersion()
                Case 2                  '// Small
                    SmallVersion()
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

    Private Sub LargeVersion()
        Height = 400
        Width = 800
    End Sub

    Private Sub MediumVersion()
        Height = 200
        Width = 400
    End Sub

    Private Sub SmallVersion()
        Height = 100
        Width = 200
    End Sub

    Private Sub OkOnlyFormat()
        Dim tbTwo As TextBlock = brdButtonTwo.Child
        With brdButtonTwo
            .Visibility = Visibility.Visible
            .Margin = New Thickness With {.Left = 350, .Top = 322, .Right = 334, .Bottom = 37}
        End With
        brdButtonOne.Visibility = Visibility.Hidden
        brdButtonThree.Visibility = Visibility.Hidden
        tbTwo.Text = "Okay"
    End Sub

    Private Sub OkCancelFormat()
        Dim tbOne As TextBlock = brdButtonOne.Child, tbTwo As TextBlock = brdButtonTwo.Child
        With brdButtonOne
            .Visibility = Visibility.Visible
            .Margin = New Thickness With {.Left = 235, .Top = 322, .Right = 453, .Bottom = 37}
        End With
        With brdButtonTwo
            .Visibility = Visibility.Visible
            .Margin = New Thickness With {.Left = 469, .Top = 322, .Right = 219, .Bottom = 37}
        End With
        brdButtonThree.Visibility = Visibility.Hidden
        tbOne.Text = "Okay"
        tbTwo.Text = "Cancel"
    End Sub

    Private Sub YesNoFormat()
        Dim tbOne As TextBlock = brdButtonOne.Child, tbTwo As TextBlock = brdButtonTwo.Child
        With brdButtonOne
            .Visibility = Visibility.Visible
            .Margin = New Thickness With {.Left = 235, .Top = 322, .Right = 453, .Bottom = 37}
        End With
        With brdButtonTwo
            .Visibility = Visibility.Visible
            .Margin = New Thickness With {.Left = 469, .Top = 322, .Right = 219, .Bottom = 37}
        End With
        brdButtonThree.Visibility = Visibility.Hidden
        tbOne.Text = "Yes"
        tbTwo.Text = "No"
    End Sub

    Private Sub YesNoCancelFormat()
        Dim tbOne As TextBlock = brdButtonOne.Child, tbTwo As TextBlock = brdButtonTwo.Child, tbThree As TextBlock = brdButtonThree.Child
        With brdButtonOne
            .Visibility = Visibility.Visible
            .Margin = New Thickness With {.Left = 115, .Top = 322, .Right = 569, .Bottom = 37}
        End With
        With brdButtonTwo
            .Visibility = Visibility.Visible
            .Margin = New Thickness With {.Left = 350, .Top = 322, .Right = 334, .Bottom = 37}
        End With
        With brdButtonThree
            .Visibility = Visibility.Visible
            .Margin = New Thickness With {.Left = 581, .Top = 322, .Right = 103, .Bottom = 37}
        End With
        tbOne.Text = "Yes"
        tbTwo.Text = "No"
        tbThree.Text = "Cancel"
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

    Private Sub HoverOver(sender As Object, e As MouseEventArgs) Handles brdButtonOne.MouseEnter, brdButtonTwo.MouseEnter, brdButtonThree.MouseEnter
        Dim b As Border = sender, t As TextBlock = b.Child
        Dim mdse As New Effects.DropShadowEffect With {.BlurRadius = 8, .Direction = 270, .ShadowDepth = 6, .Opacity = 0.75}
        b.Effect = mdse
        t.Foreground = New SolidColorBrush(Colors.Yellow)
    End Sub

    Private Sub HoverLeave(sender As Object, e As MouseEventArgs) Handles brdButtonOne.MouseLeave, brdButtonTwo.MouseLeave, brdButtonThree.MouseLeave
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

End Class
