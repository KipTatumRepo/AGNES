Imports System.Windows.Threading
Imports System.Timers

Public Class PacMan
#Region "Properties"
    Public GameTimer As DispatcherTimer
    Public TimerInterval As TimeSpan
    Private MoveDirection As Byte = 2
    Private GhostDirection As Byte = 3
    Private px As Double = 292
    Private py As Double = 352
    Private gx As Double = 292
    Private gy As Double = 128
    Private xdistance As Double
    Private ydistance As Double

#End Region

#Region "Constructor"
    Public Sub New()
        InitializeComponent()
        TimerInterval = New TimeSpan(0, 0, 0, 0, 125)
        StartGame()
    End Sub

#End Region

#Region "Public Methods"

#End Region

#Region "Private Methods"
    Private Sub StartGame()
        GameTimer = New DispatcherTimer()
        GameTimer.Interval = TimerInterval

        AddHandler GameTimer.Tick, AddressOf GameRoutines
        AddHandler Me.KeyDown, AddressOf KeyPressed

        GameTimer.Start()

    End Sub

    Private Sub GameRoutines()
        MovePacMan()
        DisplayCoords()
        MoveGhosts()
        CheckForCollision()
        CheckForBonus()

    End Sub

    Private Sub MovePacMan()
        Select Case MoveDirection

            Case 1  ' Up
                For py = py To py - 16 Step -1
                    If py < 1 Then
                        py = 703
                        imgPacMan.Margin = New Thickness(px, py, 0, 0)
                        Exit For
                    End If
                    imgPacMan.Margin = New Thickness(px, py, 0, 0)
                Next
            Case 2  ' Right
                For px = px To px + 16
                    If px > 540 Then
                        px = 1
                        imgPacMan.Margin = New Thickness(px, py, 0, 0)
                        Exit For
                    End If
                    imgPacMan.Margin = New Thickness(px, py, 0, 0)
                Next
            Case 3  ' Down
                For py = py To py + 16
                    If py > 703 Then
                        py = 1
                        imgPacMan.Margin = New Thickness(px, py, 0, 0)
                        Exit For
                    End If
                    imgPacMan.Margin = New Thickness(px, py, 0, 0)
                Next
            Case 4  ' Left
                For px = px To px - 16 Step -1
                    If px < 1 Then
                        px = 540
                        imgPacMan.Margin = New Thickness(px, py, 0, 0)
                        Exit For
                    End If
                    imgPacMan.Margin = New Thickness(px, py, 0, 0)
                Next
        End Select
    End Sub

    Private Sub DisplayCoords()
        'txtCoords.Text = "P" & px & ", " & py & "G:" & gx & "," & gy
    End Sub

    Private Sub MoveGhosts()
        ChooseDirection()
        Select Case GhostDirection

            Case 1  ' Up
                For gy = gy To gy - 16 Step -1
                    If gy < 1 Then
                        gy = 703
                        imgGhost.Margin = New Thickness(gx, gy, 0, 0)
                        Exit For
                    End If
                    imgGhost.Margin = New Thickness(gx, gy, 0, 0)
                Next
            Case 2  ' Right
                For gx = gx To gx + 16
                    If gx > 540 Then
                        gx = 1
                        imgGhost.Margin = New Thickness(gx, gy, 0, 0)
                        Exit For
                    End If
                    imgGhost.Margin = New Thickness(gx, gy, 0, 0)
                Next
            Case 3  ' Down
                For gy = gy To gy + 16
                    If gy > 703 Then
                        gy = 1
                        imgGhost.Margin = New Thickness(gx, gy, 0, 0)
                        Exit For
                    End If
                    imgGhost.Margin = New Thickness(gx, gy, 0, 0)
                Next
            Case 4  ' Left
                For gx = gx To gx - 16 Step -1
                    If gx < 1 Then
                        gx = 540
                        imgGhost.Margin = New Thickness(gx, gy, 0, 0)
                        Exit For
                    End If
                    imgGhost.Margin = New Thickness(gx, gy, 0, 0)
                Next
        End Select
    End Sub

    Private Sub ChooseDirection()
        xdistance = px - gx
        ydistance = py - gx
        If Math.Abs(xdistance) >= Math.Abs(ydistance) Then
            If gx < px Then
                GhostDirection = 2
            Else
                GhostDirection = 4
            End If
        Else
            If gy < py Then
                GhostDirection = 3
            Else
                GhostDirection = 1
            End If
        End If

    End Sub

    Private Sub CheckForBonus()
        Dim ph As String = ""
    End Sub

    Private Sub CheckForCollision()
        Dim gr As Rect = New Rect()
        gr.X = gx + 8
        gr.Y = gy + 8
        gr.Width = 16
        gr.Height = 16
        Dim grg As RectangleGeometry = New RectangleGeometry()
        grg.Rect = gr

        Dim pr As Rect = New Rect()
        pr.X = px + 8
        pr.Y = py + 8
        pr.Width = 16
        pr.Height = 16
        Dim prg As RectangleGeometry = New RectangleGeometry()
        prg.Rect = pr

        If gr.IntersectsWith(pr) Then
            MsgBox("Dead!")
            StartOver()
        End If

    End Sub

    Private Sub StartOver()
        px = 292 : py = 352
        gx = 292 : gy = 128
        imgPacMan.Margin = New Thickness(px, py, 0, 0)
        imgGhost.Margin = New Thickness(gx, gy, 0, 0)

    End Sub

    Private Sub KeyPressed(sender As Object, e As KeyEventArgs)
        GameTimer.Stop()

        Select Case e.Key
            Case Key.Up
                imgPacMan.Source = New BitmapImage(New Uri("pack://application:,,,/Resources/pacmanup.png"))
                MoveDirection = 1

            Case Key.Right
                imgPacMan.Source = New BitmapImage(New Uri("pack://application:,,,/Resources/pacmanright.png"))
                MoveDirection = 2

            Case Key.Down
                imgPacMan.Source = New BitmapImage(New Uri("pack://application:,,,/Resources/pacmandown.png"))
                MoveDirection = 3

            Case Key.Left
                imgPacMan.Source = New BitmapImage(New Uri("pack://application:,,,/Resources/pacmanleft.png"))
                MoveDirection = 4

            Case Key.Space, Key.Escape
                PauseGame()
        End Select
        GameTimer.Start()
    End Sub

    Private Sub PauseGame()
        Dim x As MsgBoxResult = MsgBox("Continue?", vbYesNo, "Game paused")
        If x = vbNo Then
            Close()
            Exit Sub
        End If
    End Sub

#End Region

End Class
