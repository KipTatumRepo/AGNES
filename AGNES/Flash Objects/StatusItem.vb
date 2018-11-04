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
        If am <> "" Then AlertMsg = am
        AddHandler MouseEnter, AddressOf MouseHover
        AddHandler MouseLeave, AddressOf MouseEndHover

        '// Add frame and grid using border object
        Dim brd As New Border With {.Height = 40, .Width = 250, .BorderBrush = Media.Brushes.Black, .BorderThickness = New Thickness(1)}
        AddHandler brd.MouseEnter, AddressOf MouseHover
        AddHandler brd.MouseLeave, AddressOf MouseEndHover

        Dim dp As New DockPanel
        AddHandler dp.MouseEnter, AddressOf MouseHover
        AddHandler dp.MouseLeave, AddressOf MouseEndHover
        brd.Child = dp
        Children.Add(brd)

        '// Add Unit name and number
        Dim UnitNameLabel As New TextBlock With {.Width = 120, .VerticalAlignment = VerticalAlignment.Center, .HorizontalAlignment = HorizontalAlignment.Left,
            .Margin = New Thickness(5, 0, 0, 0)}
        UnitNameLabel.Text = tl
        dp.Children.Add(UnitNameLabel)
        AddHandler UnitNameLabel.MouseEnter, AddressOf MouseHover
        AddHandler UnitNameLabel.MouseLeave, AddressOf MouseEndHover

        Dim IconDp As New DockPanel
        IconDp.HorizontalAlignment = HorizontalAlignment.Right
        Dim DisplayedImage As BitmapImage

        '// Add alert icon, if present, and hoverover alert message
        If al = True Then
            DisplayedImage = New BitmapImage(New Uri("pack://application:,,,/Resources/HandWave.png"))
            Dim AlertImg As New Image With {.Name = "imgAlertStatus", .Height = 32, .Width = 32, .Stretch = Stretch.UniformToFill,
                .ToolTip = am, .Margin = New Thickness(0, 0, 5, 0)}
            AlertImg.Source = DisplayedImage
            AddHandler AlertImg.MouseEnter, AddressOf MouseHover
            AddHandler AlertImg.MouseLeave, AddressOf MouseEndHover
            IconDp.Children.Add(AlertImg)
        End If

        '// Add status icon (unlocked for draft, locked for final)
        '// Create eye/view icon (show if in draft or final)
        Dim StatusImg As New Image With {.Name = "imgFlashStatus", .Height = 32, .Width = 32, .Stretch = Stretch.UniformToFill,
            .Margin = New Thickness(0, 0, 5, 0)}
        AddHandler StatusImg.MouseEnter, AddressOf MouseHover
        AddHandler StatusImg.MouseLeave, AddressOf MouseEndHover
        Dim EyeImg As New Image With {.Name = "imgViewFlash", .Height = 32, .Width = 32, .Stretch = Stretch.UniformToFill,
            .ToolTip = "View Flash", .Margin = New Thickness(0, 0, 5, 0)}
        AddHandler EyeImg.PreviewMouseLeftButtonDown, AddressOf ViewFlash
        AddHandler EyeImg.MouseEnter, AddressOf MouseHover
        AddHandler EyeImg.MouseLeave, AddressOf MouseEndHover


        Select Case st
            Case 1
                DefaultBrush = Brushes.LightYellow
                DisplayedImage = New BitmapImage(New Uri("pack://application:,,,/Resources/eye.png"))
                EyeImg.Source = DisplayedImage
                DisplayedImage = New BitmapImage(New Uri("pack://application:,,,/Resources/unlocked.png"))
                StatusImg.Source = DisplayedImage
                IconDp.Children.Add(EyeImg)
                IconDp.Children.Add(StatusImg)

            Case 2
                DefaultBrush = Brushes.White
                DisplayedImage = New BitmapImage(New Uri("pack://application:,,,/Resources/eye.png"))
                EyeImg.Source = DisplayedImage
                DisplayedImage = New BitmapImage(New Uri("pack://application:,,,/Resources/locked.png"))
                StatusImg.Source = DisplayedImage
                IconDp.Children.Add(EyeImg)
                StatusImg.ToolTip = "Click to unlock Flash for edits"
                AddHandler StatusImg.PreviewMouseLeftButtonDown, AddressOf UnlockUnit
                IconDp.Children.Add(StatusImg)
            Case Else
                DefaultBrush = Brushes.LightGray
        End Select

        Background = DefaultBrush



            dp.Children.Add(IconDp)

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

    Private Sub ViewFlash()
        FlashPage = New Flash(FlashStatusPage.TypeofFlash, UnitNumber)
        FlashPage.SaveStatus = InitialLoadStatus
        FlashPage.ShowDialog()
    End Sub

#End Region

End Class
