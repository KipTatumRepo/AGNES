Public Class RadialPortal
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()
        ConstructRadialMenu()

    End Sub
    Private Sub ConstructRadialMenu()
        'TODO: REPLACE TEST MENU CONSTRUCTION WITH DATABASE-DRIVEN FINAL PRODUCT
        Dim ModuleName As String, ModuleToolTip As String, moduleimage As String

        ModuleName = "WCR" : ModuleToolTip = "Commons WCR" : moduleimage = "Resources/WCR.png"
        PlaceMenuItem(1, 8, ModuleName, ModuleToolTip, moduleimage)

        ModuleName = "BGCRM" : ModuleToolTip = "Business Group CRM" : moduleimage = "Resources/BusinessGroup.png"
        PlaceMenuItem(2, 8, ModuleName, ModuleToolTip, moduleimage)

        ModuleName = "CafeFlash" : ModuleToolTip = "Cafe Weekly Flash" : moduleimage = "Resources/Flash.png"
        PlaceMenuItem(3, 8, ModuleName, ModuleToolTip, moduleimage)

        ModuleName = "CafeForecast" : ModuleToolTip = "Cafe Period Forecast" : moduleimage = "Resources/ForecastButton.png"
        PlaceMenuItem(4, 8, ModuleName, ModuleToolTip, moduleimage)

        ModuleName = "HRAudit" : ModuleToolTip = "HR Audit" : moduleimage = "Resources/Audit.png"
        PlaceMenuItem(5, 8, ModuleName, ModuleToolTip, moduleimage)

        ModuleName = "AvFlash" : ModuleToolTip = "A/V Weekly Flash" : moduleimage = "Resources/AVFlash.png"
        PlaceMenuItem(6, 8, ModuleName, ModuleToolTip, moduleimage)

        ModuleName = "Admin" : ModuleToolTip = "Admin Tools" : moduleimage = "Resources/AdminTools.png"
        PlaceMenuItem(7, 8, ModuleName, ModuleToolTip, moduleimage)

        ModuleName = "More" : ModuleToolTip = "More Modules" : moduleimage = "Resources/More.png"
        PlaceMenuItem(8, 8, ModuleName, ModuleToolTip, moduleimage)
    End Sub

    Private Sub DragViaLeftMouse(sender As Object, e As MouseButtonEventArgs)
        DragMove()
    End Sub

    Private Sub CloseAGNES(sender As Object, e As MouseButtonEventArgs)
        Dim ask As MsgBoxResult = MsgBox("Close AGNES?", MsgBoxStyle.YesNo)
        If ask = MsgBoxResult.Yes Then
            GC.Collect()
            Close()
        End If
    End Sub

    Private Sub ModuleMouseHover(sender As Object, e As MouseEventArgs)
        Dim s As Image = sender
        s.Height = 85
        s.Width = 85
        Dim l As Integer = s.Margin.Left - 5
        Dim t As Integer = s.Margin.Top - 5
        s.Margin = New Thickness(l, t, 0, 0)
    End Sub

    Private Sub ModuleMouseLeave(sender As Object, e As MouseEventArgs)
        Dim s As Image = sender
        s.Height = 75
        s.Width = 75
        Dim l As Integer = s.Margin.Left + 5
        Dim t As Integer = s.Margin.Top + 5
        s.Margin = New Thickness(l, t, 0, 0)
    End Sub

    Private Sub ModuleSelect(sender As Object, e As MouseEventArgs)
        Dim s As Image = sender
        'TODO: FIGURE OUT IMPROVED METHOD FOR CALLING MODULES FROM MENU ITEM CLICK
        Hide()
        Select Case s.Tag
            Case "WCR"
                WCRModule.Runmodule()
            Case "BGCRM"
                BGCRMModule.Runmodule()
        End Select
        Show()
    End Sub

    Private Sub PlaceMenuItem(item, itemcount, associatedmodule, tooltip, moduleimage)
        Dim ang As Double = item * ((2 * Math.PI) / itemcount)
        Dim rad As Integer = cnvRadialMenu.Height / 2
        Dim x As Integer = ((Math.Cos(ang) * rad) + rad)
        Dim y As Integer = (rad - (Math.Sin(ang) * rad))
        Dim img As New Image With {.Source = New BitmapImage(New Uri(moduleimage, UriKind.Relative)), .Name = "btnRadial" & item,
            .Height = 75, .Width = 75, .Stretch = Stretch.UniformToFill, .ToolTip = tooltip, .Tag = associatedmodule}
        x -= img.Width / 2 : y -= img.Height / 2
        img.Margin = New Thickness(x, y, 0, 0)
        cnvRadialMenu.Children.Add(img)
        AddHandler img.MouseEnter, AddressOf ModuleMouseHover
        AddHandler img.MouseLeave, AddressOf ModuleMouseLeave
        AddHandler img.MouseLeftButtonDown, AddressOf ModuleSelect
    End Sub
End Class
