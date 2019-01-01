Imports System.Windows.Threading
Imports System.Windows.Media

Imports System.Timers
Imports System.Media

Public Class ChaosCafeSplash

#Region "Properties"
    Private ScreenWidth As Double
    Private ScreenHeight As Double

    Dim MusicPlayer As SoundPlayer

    Dim mplayer As MediaPlayer

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
        chefy = (ScreenHeight / 2) - 75
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
        '// Start Fire
        Dim image As New BitmapImage()
        image.BeginInit()
        image.UriSource = New Uri("pack://application:,,,/Resources/fire2.gif")
        image.EndInit()
        imgfire.Height = ScreenHeight / 3
        ImageBehavior.SetAnimatedSource(imgfire, image)

        '// Get Top Ten
        Dim t As String = "BLF" & vbTab & "10,000" & Chr(13)
        t = t & "DKT" & vbTab & "9,000" & Chr(13)
        t = t & "WCW" & vbTab & "8,000" & Chr(13)
        t = t & "EJG" & vbTab & "7,000" & Chr(13)
        t = t & "ABC" & vbTab & "6,000" & Chr(13)
        t = t & "DEF" & vbTab & "5,000" & Chr(13)
        t = t & "GHI" & vbTab & "4,000" & Chr(13)
        t = t & "JKL" & vbTab & "3,000" & Chr(13)
        t = t & "MNO" & vbTab & "2,000" & Chr(13)
        t = t & "PQR" & vbTab & "1,000"

        tbTopTen.Text = t

        'Mplayer = New MediaPlayer
        'Mplayer.Open(
        'Mplayer.Play()
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
                mplayer.Stop()
                mplayer = Nothing
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
                If chefy + 75 > cheffloor Then chefy = cheffloor
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
        ChefFeets += 1
        If ChefFeets > 2 Then ChefFeets = 0
    End Sub

    Private Sub SplashLoaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        '// Kickin' tunes
        'MusicPlayer = New SoundPlayer(My.Resources.CCMain)
        'MusicPlayer.PlayLooping()
        'MusicPlayer.Play()

        mplayer = New MediaPlayer
        AddHandler mplayer.MediaEnded, AddressOf LoopTunes
        mplayer.Open(New Uri("Resources/Sound Assets/jump.mp3", UriKind.Relative))
        mplayer.Play()

    End Sub

    Private Sub LoopTunes(sender As Object, e As EventArgs)
        mplayer.Position = TimeSpan.Zero
        mplayer.Play()
    End Sub


#End Region
End Class
