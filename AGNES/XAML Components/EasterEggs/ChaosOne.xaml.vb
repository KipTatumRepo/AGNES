Imports System.Windows.Threading
Imports System.Timers

Public Class ChaosOne


#Region "Properties"
    Public ScreenWidth = System.Windows.SystemParameters.PrimaryScreenWidth
    Private StartInstructionsTimer As DispatcherTimer
    Private StartInstructionsInterval As TimeSpan = New TimeSpan(0, 0, 0, 1)
    Private InstructionOpacity As Double = 1
    Private Player As Chef
    Private GameStarted As Boolean
#End Region

#Region "Constructor"
    Public Sub New()
        InitializeComponent()
        brdInstructions.Visibility = Visibility.Visible
        Player = New Chef(grdRoundOne) With {.CanvasTop = 0, .CanvasBottom = System.Windows.SystemParameters.PrimaryScreenHeight - 50, .CanvasLeft = 50,
        .CanvasRight = ScreenWidth - 100, .Direction = 3, .Feets = 0, .Height = 150, .Width = 100, .XPos = (ScreenWidth / 2) - 50, .YPos = .CanvasBottom - 150}
        ShowInstructions()
    End Sub

#End Region

#Region "Public Methods"

#End Region

#Region "Private Methods"
    Private Sub ShowInstructions()
        '// Instantiate Timers for instructions and user input
        StartInstructionsTimer = New DispatcherTimer()
        StartInstructionsTimer.Interval = StartInstructionsInterval
        AddHandler StartInstructionsTimer.Tick, AddressOf InstructionsDecay
        AddHandler Me.KeyDown, AddressOf KeyStroke
        StartInstructionsTimer.Start()
    End Sub

    Private Sub InstructionsDecay()
        InstructionOpacity -= 0.1
        If InstructionOpacity <= 0 Then StartGame()

        Dim cntdown As Double = Math.Round(InstructionOpacity * 10, 0)
        Select Case cntdown
            Case 2
                tbTimer.Text = "Get ready..."
            Case 1
                tbTimer.Text = "Get set..."
            Case 0
                tbTimer.Text = "Begin!"
            Case Else
                tbTimer.Text = cntdown & "..."
        End Select
        brdInstructions.Opacity = InstructionOpacity
    End Sub

    Private Sub StartGame()
        StartInstructionsTimer.Stop()
        StartInstructionsTimer = Nothing
        brdInstructions.Visibility = Visibility.Hidden
        lblEmails.Visibility = Visibility.Visible
        lblScore.Visibility = Visibility.Visible
        tbEmails.Visibility = Visibility.Visible
        tbScore.Visibility = Visibility.Visible
        grdRoundOne.Children.Add(Player)
        GameStarted = True
    End Sub

    Private Sub KeyStroke(sender As Object, e As KeyEventArgs)
        Select Case e.Key
            Case Key.Escape
                StartInstructionsTimer = Nothing
                WOPRModule.MusicChannel.Stop()
                Close()
            Case Key.Space
                'TODO: Pause routine
            Case Key.Left
                If Player.XPos - 6 < Player.CanvasLeft Then Exit Sub
                Player.Direction = 7
                Player.XPos -= 6
            Case Key.Right
                If Player.XPos + 6 > Player.CanvasRight Then Exit Sub
                Player.Direction = 3
                Player.XPos += 6
        End Select

    End Sub

#End Region
End Class
