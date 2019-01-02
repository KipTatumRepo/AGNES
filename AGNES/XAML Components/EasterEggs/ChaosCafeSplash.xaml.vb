Imports System.Windows.Threading
Imports System.Timers

Public Class ChaosCafeSplash

#Region "Properties"
    Private ScreenWidth As Double
    Private ScreenHeight As Double

    Private StartFlashTimer As DispatcherTimer
    Private StartFlashTimerInterval As TimeSpan = New TimeSpan(0, 0, 0, 0, 500)
    Private StartFlashTimerToggle As Boolean

    Private SplashChef As Chef
    Private chefceiling As Double
    Private cheffloor As Double
    Private ChefTimer As DispatcherTimer
    Private ChefInterval As TimeSpan = New TimeSpan(0, 0, 0, 0, 0.8)

#End Region

#Region "Constructor"
    Public Sub New()
        InitializeComponent()
        ScreenWidth = System.Windows.SystemParameters.PrimaryScreenWidth
        ScreenHeight = System.Windows.SystemParameters.PrimaryScreenHeight
        chefceiling = ScreenHeight / 4
        cheffloor = ScreenHeight - chefceiling
        AwaitStart()
    End Sub

#End Region

#Region "Public Methods"

#End Region

#Region "Private Methods"
    Private Sub AwaitStart()
        '// Instantiate Chef
        SplashChef = New Chef(grdSplash) With {.XPos = -100, .YPos = (ScreenHeight / 2) - 75, .Direction = 3, .Feets = 0, .Height = 150, .Width = 100, .CanvasTop = chefceiling,
            .CanvasBottom = cheffloor, .CanvasLeft = 0, .CanvasRight = ScreenWidth}
        grdSplash.Children.Add(SplashChef)


        '// Instantiate Fire
        Dim image As New BitmapImage()
        image.BeginInit()
        image.UriSource = New Uri("pack://application:,,,/Resources/fire2.gif")
        image.EndInit()
        imgfire.Height = ScreenHeight / 3
        ImageBehavior.SetAnimatedSource(imgfire, image)

        '// Instantiate Top Ten - HARD CODED FOR THE MOMENT
        Dim t As String = "BLF" & vbTab & "10,000" & Chr(13)
        t = t & "DTT" & vbTab & "9,000" & Chr(13)
        t = t & "WPW" & vbTab & "8,000" & Chr(13)
        t = t & "EJG" & vbTab & "7,000" & Chr(13)
        t = t & "ABC" & vbTab & "6,000" & Chr(13)
        t = t & "DEF" & vbTab & "5,000" & Chr(13)
        t = t & "GHI" & vbTab & "4,000" & Chr(13)
        t = t & "JKL" & vbTab & "3,000" & Chr(13)
        t = t & "MNO" & vbTab & "2,000" & Chr(13)
        t = t & "PQR" & vbTab & "1,000"
        tbTopTen.Text = t

        '// Kick out the jams
        WOPRModule.MusicLoop = True
        WOPRModule.MusicChoice("Resources/Sound Assets/jump.mp3")

        '// Instantiate Timers for Chef movement and user input
        StartFlashTimer = New DispatcherTimer()
        StartFlashTimer.Interval = StartFlashTimerInterval
        AddHandler StartFlashTimer.Tick, AddressOf FlashStart
        AddHandler Me.KeyDown, AddressOf KeyStroke

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

    Private Sub KeyStroke(sender As Object, e As KeyEventArgs)
        Select Case e.Key
            Case Key.Escape
                ChefTimer = Nothing
                StartFlashTimer = Nothing
                WOPRModule.MusicChannel.Stop()
                Close()
            Case Key.Space
                ChefTimer = Nothing
                StartFlashTimer = Nothing
                WOPRModule.MusicChannel.Stop()
                WOPRModule.StartContinueGame = True
                Close()
        End Select

    End Sub

    Private Sub ChefMove()
        Randomize()
        Dim randnum As Integer = CInt(Int((100 * Rnd()) + 1))
        If randnum > 98 Then
            If SplashChef.Direction = 3 Then
                SplashChef.Direction = 7
            Else
                SplashChef.Direction = 3
            End If
        End If
        Dim YChanceC As Double
        Dim YChanceF As Double
        Dim Ydir As Integer

        Select Case SplashChef.Direction
            Case 3
                If SplashChef.XPos + 3 > SplashChef.CanvasRight - 50 Then
                    SplashChef.XPos = SplashChef.CanvasLeft
                Else
                    SplashChef.XPos += 3
                End If

            Case 7
                If SplashChef.XPos - 3 < SplashChef.CanvasLeft - 100 Then
                    SplashChef.XPos = SplashChef.CanvasRight + 50
                Else
                    SplashChef.XPos -= 3
                End If

        End Select

        Select Case SplashChef.YPos
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
                If SplashChef.YPos + 75 > cheffloor Then
                    SplashChef.YPos = cheffloor
                Else
                    SplashChef.YPos += Ydir
                End If

            Case YChanceF To 100
                If SplashChef.YPos < chefceiling Then
                    SplashChef.YPos = chefceiling
                Else
                    SplashChef.YPos += -Ydir
                End If
        End Select

    End Sub

#End Region

End Class
