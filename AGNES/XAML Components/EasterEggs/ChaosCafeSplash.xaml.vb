Imports System.Windows.Threading
Imports System.Timers
Public Class ChaosCafeSplash

#Region "Properties"
    Private ScreenWidth As Double
    Private ScreenHeight As Double

    Private StartFlashTimer As DispatcherTimer
    Private StartFlashTimerInterval As TimeSpan = New TimeSpan(0, 0, 0, 0, 500)
    Private StartFlashTimerToggle As Boolean

    Private chefx As Double = -100
    Private chefy As Double
    Private chefceiling As Double
    Private cheffloor As Double
    Private ChefTimer As DispatcherTimer
    Private ChefInterval As TimeSpan = New TimeSpan(0, 0, 0, 0, 1)
    Private ChefDirection As Boolean
    Private ChefFeets As Byte = 1
#End Region

#Region "Constructor"
    Public Sub New()
        InitializeComponent()
        ScreenWidth = System.Windows.SystemParameters.PrimaryScreenWidth
        ScreenHeight = System.Windows.SystemParameters.PrimaryScreenHeight
        chefx = -100
        chefy = (ScreenHeight / 2) - 50
        imgChef.Margin = New Thickness(chefx, chefy, 0, 0)
        chefceiling = ScreenHeight / 4
        cheffloor = ScreenHeight - chefceiling
        AwaitStart()
    End Sub

#End Region

#Region "Public Methods"

#End Region

#Region "Private Methods"
    Private Sub AwaitStart()
        StartFlashTimer = New DispatcherTimer()
        StartFlashTimer.Interval = StartFlashTimerInterval
        AddHandler StartFlashTimer.Tick, AddressOf FlashStart
        AddHandler Me.KeyDown, AddressOf SpaceBarPress

        ChefTimer = New DispatcherTimer()
        ChefTimer.Interval = ChefInterval
        AddHandler ChefTimer.Tick, AddressOf ChefMove

        StartFlashTimer.Start()
        ChefTimer.Start()
    End Sub

    Private Sub FlashStart()
        tbStart.Visibility = StartFlashTimerToggle
        StartFlashTimerToggle = Not StartFlashTimerToggle
    End Sub
    Private Sub SpaceBarPress(sender As Object, e As KeyEventArgs)
        Select Case e.Key
            Case Key.Escape
                ChefTimer = Nothing
                StartFlashTimer = Nothing
                Close()
            Case Key.Space

        End Select

    End Sub

    Private Sub ChefMove()
        Randomize()
        Dim randnum As Integer = CInt(Int((100 * Rnd()) + 1))
        If randnum > 98 Then ChefDirection = Not ChefDirection
        Dim YChanceC As Double
        Dim YChanceF As Double
        Dim Ydir As Integer
        Select Case chefy
            Case chefceiling To ScreenHeight / 3
                YChanceC = 60
                YChanceF = 90
                Ydir = 3
            Case (ScreenHeight - (ScreenHeight / 3)) To cheffloor
                YChanceC = 60
                YChanceF = 90
                Ydir = -3
            Case Else
                YChanceC = 85
                YChanceF = 95
                Ydir = 3
        End Select
        Select Case randnum
            Case YChanceC To YChanceF
                chefy += Ydir
                If chefy > cheffloor Then chefy = cheffloor
            Case YChanceF To 100
                chefy += -Ydir
                If chefy < chefceiling Then chefy = chefceiling
        End Select

        If ChefDirection = False Then     'Move right
            Select Case ChefFeets
                Case 0
                    imgChef.Source = New BitmapImage(New Uri("pack://application:,,,/Resources/chef1.png"))
                Case 1
                    imgChef.Source = New BitmapImage(New Uri("pack://application:,,,/Resources/chef2.png"))
                Case 2
                    imgChef.Source = New BitmapImage(New Uri("pack://application:,,,/Resources/chef3.png"))
            End Select
            chefx += 3
            If chefx > ScreenWidth Then chefx = 0
            imgChef.Margin = New Thickness(chefx, chefy, 0, 0)
        Else
            Select Case ChefFeets
                Case 0
                    imgChef.Source = New BitmapImage(New Uri("pack://application:,,,/Resources/chef1.png"))
                Case 1
                    imgChef.Source = New BitmapImage(New Uri("pack://application:,,,/Resources/chef2.png"))
                Case 2
                    imgChef.Source = New BitmapImage(New Uri("pack://application:,,,/Resources/chef3.png"))
            End Select
            chefx -= 3
            If chefx < -100 Then chefx = ScreenWidth
            imgChef.Margin = New Thickness(chefx, chefy, 0, 0)
        End If
        tbXcoord.Text = chefx
        ChefFeets += 1
        If ChefFeets > 2 Then ChefFeets = 0
    End Sub


#End Region
End Class
