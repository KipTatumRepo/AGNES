Public Class FlashForecastChooser
    Private _choosertype As Byte
    Public Property ChooserType As Byte
        Get
            Return _choosertype
        End Get
        Set(value As Byte)
            _choosertype = value
            Select Case value
                Case 0
                    tbFlFoChooserText.Text = "You have access to more than one type of flash.  Please choose."
                Case 1
                    tbFlFoChooserText.Text = "You have access to more than one unit.  Please choose."
                Case 2
                    Dim ph As String = ""
            End Select
            '0 = Flash type
            '1 = Unit choice
            '2 = Flash Status
        End Set
    End Property

    Public Property UserChoice As Long

    Public Sub New()
        InitializeComponent()
    End Sub

    Public Sub Populate(l As List(Of Long))
        For Each i As Long In l
            Createbutton(i)
        Next
    End Sub

    Private Sub Createbutton(inum)
        Dim brd As New Border With {.Height = 40, .Width = 112}
        Dim tb As New TextBlock With
            {.Name = "tbChoice", .TextWrapping = TextWrapping.Wrap, .VerticalAlignment = VerticalAlignment.Center,
            .TextAlignment = TextAlignment.Center, .FontSize = 14, .Foreground = Brushes.Black}
        brd.Child = tb
        brd.Tag = inum.ToString
        AddHandler brd.MouseEnter, AddressOf MouseHover
        AddHandler brd.MouseLeave, AddressOf MouseLeft
        AddHandler brd.PreviewMouseLeftButtonDown, AddressOf ChoiceMade

        Select Case ChooserType
            Case 0

                tb.Text = GetFlashType(inum)
            Case 1
                tb.Text = GetUnitName(inum)
            Case 2
                Dim ph As String = "Placeholder"
        End Select

        wrpFlFoChooser.Children.Add(brd)
    End Sub

    Private Function GetFlashType(i As Long) As String
        Dim qft = From f In AGNESShared.FlashTypes
                  Where f.PID = i
                  Select f

        For Each f In qft
            Return f.FlashType1
        Next
        Return "Null"
    End Function

    Private Function GetUnitName(i As Long) As String
        Dim qun = From f In SharedDataGroup.LOCATIONS
                  Where f.Unit_Number = i
                  Select f

        For Each f In qun
            Return f.Unit
        Next
        Return "Null"
    End Function

    Private Sub MouseHover(s As Object, e As MouseEventArgs)
        Dim sender As Border = s, tb As TextBlock = sender.Child
        With tb
            .FontSize = 16
            .FontWeight = FontWeights.SemiBold
            .Effect = New Effects.DropShadowEffect With {.BlurRadius = 5, .ShadowDepth = 1, .Opacity = 0.5, .Direction = 270}
        End With
    End Sub

    Private Sub MouseLeft(s As Object, e As MouseEventArgs)
        Dim sender As Border = s, tb As TextBlock = sender.Child
        With tb
            .FontSize = 14
            .FontWeight = FontWeights.Normal
            .Effect = Nothing
        End With
    End Sub

    Private Sub ChoiceMade(s As Object, e As MouseEventArgs)
        Dim sender As Border = s
        UserChoice = FormatNumber(s.tag, 0)
        Hide()
    End Sub

End Class
