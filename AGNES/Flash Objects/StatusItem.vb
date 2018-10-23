Public Class StatusItem
    Inherits DockPanel

#Region "Properties"
    Dim AlertMsg As String
    Dim UnitNumber As Long
#End Region
    Public Sub New(tl As String, st As Byte, al As Boolean, am As String, un As Long)
        UnitNumber = un
        Dim NextMarginLeft As Integer = 170
        If am <> "" Then AlertMsg = am

        '// Add frame and grid using border object
        Dim brd As New Border With {.Height = 40, .Width = 250, .BorderBrush = Media.Brushes.Black, .BorderThickness = New Thickness(1)}
        Dim grd As New Grid
        brd.Child = grd
        Children.Add(brd)

        '// Add Unit name and number
        Dim UnitNameLabel As New TextBlock With {.Width = 166, .VerticalAlignment = VerticalAlignment.Center, .HorizontalAlignment = HorizontalAlignment.Left,
            .Margin = New Thickness(5, 0, 0, 0)}
        UnitNameLabel.Text = tl
        grd.Children.Add(UnitNameLabel)

        '// Add status icon (unlocked for draft, locked for final)
        Dim DisplayedImage As BitmapImage
        Dim StatusImg As New Image With {.Name = "imgFlashStatus", .Height = 32, .Width = 32, .Stretch = Stretch.UniformToFill,
            .HorizontalAlignment = HorizontalAlignment.Right}
        If al = True Then StatusImg.Margin = New Thickness(0, 0, 64, 0)

        Select Case st
            Case 1
                DisplayedImage = New BitmapImage(New Uri("pack://application:,,,/Resources/unlocked.png"))
                StatusImg.Source = DisplayedImage
                grd.Children.Add(StatusImg)
            Case 2
                DisplayedImage = New BitmapImage(New Uri("pack://application:,,,/Resources/locked.png"))
                StatusImg.Source = DisplayedImage
                StatusImg.ToolTip = "Click to unlock Flash for edits"
                AddHandler StatusImg.PreviewMouseLeftButtonDown, AddressOf UnlockUnit
                grd.Children.Add(StatusImg)
        End Select

        '// Add alert icon, if present, and hoverover alert message
        If al = True Then
            DisplayedImage = New BitmapImage(New Uri("pack://application:,,,/Resources/HandWave.png"))
            Dim AlertImg As New Image With {.Name = "imgAlertStatus", .Height = 32, .Width = 32, .Stretch = Stretch.UniformToFill,
                .ToolTip = am, .HorizontalAlignment = HorizontalAlignment.Right}
            AlertImg.Source = DisplayedImage

            grd.Children.Add(AlertImg)
        End If

    End Sub

    Private Sub UnlockUnit()
        Dim ph As String = ""
    End Sub
End Class
