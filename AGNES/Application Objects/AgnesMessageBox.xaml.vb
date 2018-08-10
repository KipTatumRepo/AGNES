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

    Private Sub OkCancelFormat()
        brdButtonOne.Visibility = Visibility.Visible
        brdButtonTwo.Visibility = Visibility.Hidden
        brdButtonThree.Visibility = Visibility.Visible
        tbButtonOneText.Text = "Okay"
        tbButtonTwoText.Text = ""
        tbButtonThreeText.Text = "Cancel"
        AddHandler brdButtonOne.MouseDown, AddressOf ClickOne
        AddHandler brdButtonThree.MouseDown, AddressOf ClickThree
    End Sub

    Private Sub YesNoFormat()
        Dim ph As String = ""
    End Sub

    Private Sub YesNoCancelFormat()
        Dim ph As String = ""
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
        Dim s As Border = sender

    End Sub
    Private Sub ClickOne()
        Dim ph As String = ""
    End Sub

    Private Sub ClickTwo()
        Dim ph As String = ""
    End Sub

    Private Sub ClickThree()
        Dim ph As String = ""
    End Sub

End Class
