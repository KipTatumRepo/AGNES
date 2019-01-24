Imports System.Windows.Threading
Public Class BinarySlider
    Inherits Grid

#Region "Properties"
    Public Enum SliderSize
        Small
        Medium
        Standard
        Large
        Huge
    End Enum

    Public Enum ColorSchemes
        GreenRed
        White
        WhiteYellow
    End Enum

    Public TimerActive As Boolean

    Private txtChoice1 As TextBlock
    Private txtChoice2 As TextBlock
    Private rgbPanel As RadialGradientBrush
    Private brdPanel As Border
    Private VertTextPad As Integer
    Private InternalFontSize As Integer
    Private InternalHeight As Integer
    Private InternalWidth As Integer
    Private Choice1Color As Brush = Brushes.LightGreen
    Private Choice2Color As Brush = Brushes.PaleVioletRed
    Private PanelColor1 As Color = Colors.DarkBlue
    Private PanelColor2 As Color = Colors.White
    Private _choiceval As Boolean
    Private SliderTimer As DispatcherTimer
    Private PanelXPos As Integer

    Public Property ChoiceVal As Boolean
        Get
            Return _choiceval
        End Get
        Set(value As Boolean)
            If TimerActive = True Then Exit Property
            _choiceval = value
            SliderTimer = New DispatcherTimer()
            AddHandler SliderTimer.Tick, AddressOf SlidePanel
            SliderTimer.Interval = New TimeSpan(0, 0, 0, 0, 0.25)
            TimerActive = True
            SliderTimer.Start()
        End Set
    End Property

#End Region

#Region "Constructor"
    Public Sub New(InstanceSize As SliderSize, Option1 As String, Option2 As String, Optional Extend As Byte = 1, Optional ShadowOn As Boolean = False, Optional ColorScheme As ColorSchemes = ColorSchemes.GreenRed)

        Select Case InstanceSize
            Case SliderSize.Small
                InternalHeight = 20
                InternalWidth = 40 * Extend
                InternalFontSize = 8
                VertTextPad = 4
            Case SliderSize.Medium
                InternalHeight = 30
                InternalWidth = 60 * Extend
                InternalFontSize = 12
                VertTextPad = 6
            Case SliderSize.Standard
                InternalHeight = 40
                InternalWidth = 80 * Extend
                InternalFontSize = 18
                VertTextPad = 8
            Case SliderSize.Large
                InternalHeight = 60
                InternalWidth = 120 * Extend
                InternalFontSize = 18
                VertTextPad = 16
            Case SliderSize.Huge
                InternalHeight = 80
                InternalWidth = 160 * Extend
                InternalFontSize = 24
                VertTextPad = 20

        End Select

        Select Case ColorScheme
            Case ColorSchemes.GreenRed
                Choice1Color = New BrushConverter().ConvertFrom("#FFA0FF97")
                Choice2Color = New BrushConverter().ConvertFrom("#FFFF5959")
            Case ColorSchemes.White
                Choice1Color = Brushes.White
                Choice2Color = Brushes.White
            Case ColorSchemes.WhiteYellow
                Choice1Color = Brushes.White
                Choice2Color = Brushes.Yellow
        End Select
        Height = InternalHeight
        Width = InternalWidth
        VerticalAlignment = VerticalAlignment.Top
        HorizontalAlignment = HorizontalAlignment.Left
        txtChoice1 = New TextBlock With {.Text = Option1, .FontSize = InternalFontSize, .TextAlignment = TextAlignment.Center, .HorizontalAlignment = HorizontalAlignment.Left,
            .Width = InternalWidth / 2, .Background = Choice1Color, .Height = InternalHeight, .Padding = New Thickness(0, VertTextPad, 0, 0), .Margin = New Thickness(0, 0, 0, 0)}
        txtChoice2 = New TextBlock With {.Text = Option2, .FontSize = InternalFontSize, .TextAlignment = TextAlignment.Center, .HorizontalAlignment = HorizontalAlignment.Left,
            .Width = InternalWidth / 2, .Background = Choice2Color, .Height = InternalHeight, .Padding = New Thickness(0, VertTextPad, 0, 0), .Margin = New Thickness((InternalWidth / 2), 0, 0, 0)}


        rgbPanel = New RadialGradientBrush
        Dim gs1 As New GradientStop With {.Color = PanelColor1, .Offset = 3}
        Dim gs2 As New GradientStop With {.Color = PanelColor2}
        With rgbPanel.GradientStops
            .Add(gs1)
            .Add(gs2)
        End With

        brdPanel = New Border With {.HorizontalAlignment = HorizontalAlignment.Left, .Width = InternalWidth / 2, .BorderBrush = Brushes.Gray, .BorderThickness = New Thickness(1, 1, 1, 1),
            .Background = rgbPanel}
        AddHandler brdPanel.MouseLeftButtonDown, AddressOf PanelClick

        With Children
            .Add(txtChoice1)
            .Add(txtChoice2)
            .Add(brdPanel)
        End With

        If ShadowOn = True Then Effect = New Effects.DropShadowEffect With {.BlurRadius = 10, .ShadowDepth = 5}
    End Sub

#End Region

#Region "Public Methods"

#End Region

#Region "Private Methods"
    Private Sub PanelClick(sender As Border, e As MouseEventArgs)
        If TimerActive = True Or IsEnabled = False Then Exit Sub
        ChoiceVal = Not ChoiceVal
    End Sub

    Private Sub SlidePanel()
        If ChoiceVal = True Then
            PanelXPos += 1
            If PanelXPos > (InternalWidth / 2) Then
                SliderTimer.Stop()
                SliderTimer = Nothing
                TimerActive = False
            Else
                brdPanel.Margin = New Thickness(PanelXPos, 0, 0, 0)
            End If
        Else
            PanelXPos -= 1
            If PanelXPos < 0 Then
                SliderTimer.Stop()
                SliderTimer = Nothing
                TimerActive = False
            Else
                brdPanel.Margin = New Thickness(PanelXPos, 0, 0, 0)
            End If
        End If

    End Sub

    Private Sub EnabledHandler(sender As Object, e As DependencyPropertyChangedEventArgs) Handles Me.IsEnabledChanged
        If IsEnabled = True Then
            Opacity = 1
        Else
            Opacity = 0.6
        End If
    End Sub

#End Region



End Class
