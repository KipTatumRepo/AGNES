Public Class StatusItem
    Inherits DockPanel

#Region "Properties"
    Dim AlertMsg As String
    Dim UnitNumber As Long
    Public Property DefaultBrush As Brush
    Public Property MSP As Byte
    Public Property Wk As Byte
#End Region

#Region "Constructor"
    Public Sub New(tl As String, st As Byte, al As Boolean, am As String, un As Long)
        UnitNumber = un
        Dim NextMarginLeft As Integer = 170
        If am <> "" Then AlertMsg = am
        AddHandler MouseEnter, AddressOf MouseHover
        AddHandler MouseLeave, AddressOf MouseEndHover

        '// Add frame and grid using border object
        Dim brd As New Border With {.Height = 40, .Width = 250, .BorderBrush = Media.Brushes.Black, .BorderThickness = New Thickness(1)}
        AddHandler brd.MouseEnter, AddressOf MouseHover
        AddHandler brd.MouseLeave, AddressOf MouseEndHover

        Dim grd As New Grid
        AddHandler grd.MouseEnter, AddressOf MouseHover
        AddHandler grd.MouseLeave, AddressOf MouseEndHover
        brd.Child = grd
        Children.Add(brd)

        '// Add Unit name and number
        Dim UnitNameLabel As New TextBlock With {.Width = 166, .VerticalAlignment = VerticalAlignment.Center, .HorizontalAlignment = HorizontalAlignment.Left,
            .Margin = New Thickness(5, 0, 0, 0)}
        UnitNameLabel.Text = tl
        grd.Children.Add(UnitNameLabel)
        AddHandler UnitNameLabel.MouseEnter, AddressOf MouseHover
        AddHandler UnitNameLabel.MouseLeave, AddressOf MouseEndHover

        '// Add status icon (unlocked for draft, locked for final)
        Dim DisplayedImage As BitmapImage
        Dim StatusImg As New Image With {.Name = "imgFlashStatus", .Height = 32, .Width = 32, .Stretch = Stretch.UniformToFill,
            .HorizontalAlignment = HorizontalAlignment.Right}
        If al = True Then StatusImg.Margin = New Thickness(0, 0, 64, 0)
        AddHandler StatusImg.MouseEnter, AddressOf MouseHover
        AddHandler StatusImg.MouseLeave, AddressOf MouseEndHover

        Select Case st
            Case 1
                DisplayedImage = New BitmapImage(New Uri("pack://application:,,,/Resources/unlocked.png"))
                DefaultBrush = Brushes.LightYellow
                StatusImg.Source = DisplayedImage
                grd.Children.Add(StatusImg)
            Case 2
                DisplayedImage = New BitmapImage(New Uri("pack://application:,,,/Resources/locked.png"))
                DefaultBrush = Brushes.White
                StatusImg.Source = DisplayedImage
                StatusImg.ToolTip = "Click to unlock Flash for edits"
                AddHandler StatusImg.PreviewMouseLeftButtonDown, AddressOf UnlockUnit
                grd.Children.Add(StatusImg)
            Case Else
                DefaultBrush = Brushes.LightGray
        End Select

        Background = DefaultBrush
        '// Add alert icon, if present, and hoverover alert message
        If al = True Then
            DisplayedImage = New BitmapImage(New Uri("pack://application:,,,/Resources/HandWave.png"))
            Dim AlertImg As New Image With {.Name = "imgAlertStatus", .Height = 32, .Width = 32, .Stretch = Stretch.UniformToFill,
                .ToolTip = am, .HorizontalAlignment = HorizontalAlignment.Right}
            AlertImg.Source = DisplayedImage
            AddHandler AlertImg.MouseEnter, AddressOf MouseHover
            AddHandler AlertImg.MouseLeave, AddressOf MouseEndHover
            grd.Children.Add(AlertImg)
        End If

    End Sub

#End Region

#Region "Private Methods"
    Private Sub UnlockUnit()
        Dim qul = From uul In FlashActuals.FlashActualData
                  Select uul Where uul.UnitNumber = UnitNumber And
                                 uul.MSFY = CurrentFiscalYear And
                                 uul.MSP = MSP And
                                 uul.Week = Wk

        For Each uul In qul
            uul.Status = "Draft"
        Next

        FlashActuals.SaveChanges()
        FlashStatusPage.PopulateUnits()
    End Sub

    Private Sub MouseHover()
        If IsEnabled = False Then Exit Sub
        Background = Brushes.LightBlue
    End Sub

    Private Sub MouseEndHover()
        If IsEnabled = False Then Exit Sub
        Background = DefaultBrush
    End Sub
#End Region

End Class
