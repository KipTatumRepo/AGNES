Imports System.Windows.Threading
Imports System.Timers

Public Class WOPR

#Region "Properties"
    Private HelloString As String
    Private MsgTimer As DispatcherTimer
    Private TimerInterval As TimeSpan
    Private ActiveTextBlock As TextBlock
    Private _textindex As Integer
    Private Property TextIndex As Integer
        Get
            Return _textindex
        End Get
        Set(value As Integer)
            If value > TextLength Then
                MsgTimer.Stop()
                Complete = True
            End If
            _textindex = value
        End Set
    End Property
    Private TextLength As Integer
    Private _texttodisplay As String
    Private Property TextToDisplay As String
        Get
            Return _texttodisplay
        End Get
        Set(value As String)
            _texttodisplay = value
            TextLength = value.Length
        End Set
    End Property
    Private Username As String
    Private Complete As Boolean
    Private testcomplete As Boolean
    Private Gameslist() As String = {"Manager Mayhem", "Kustomer Kaos", "Corner!", "Pac-man"}
#End Region

#Region "Constructor"
    Public Sub New(uname)
        InitializeComponent()
        Username = uname
        TimerInterval = New TimeSpan(0, 0, 0, 0, 20)
        RunHello()
    End Sub

#End Region

#Region "Public Methods"

#End Region

#Region "Private Methods"

    Private Sub RunHello()
        Complete = False
        ActiveTextBlock = tblkHello
        TextToDisplay = "Hello, " & Username & ".  How about a nice game of chess? (Y/N)"
        TextIndex = 0
        MsgTimer = New DispatcherTimer()
        MsgTimer.Interval = TimerInterval
        AddHandler MsgTimer.Tick, AddressOf DisplayHelloandChessText
        AddHandler Me.KeyDown, AddressOf HelloKeyPress
        MsgTimer.Start()
    End Sub

    Private Sub HelloKeyPress(sender As Object, e As KeyEventArgs)
        If Complete = False Then Exit Sub
        MsgTimer.Stop()
        Select Case e.Key
            Case Key.Y
                MsgTimer = Nothing
                ChessOwn()
            Case Key.N
                MsgTimer = Nothing
                WOPRModule.GameChoice = "Chaos Cafe"
                Close()
                'DisplayGameList()
        End Select
    End Sub

    Private Sub ChessOwn()
        Complete = False
        ActiveTextBlock.Width = Width
        TextToDisplay = "I honestly don't think that you would be much of a match for me.  How about something more your speed? (Y/N)"
        TextIndex = 0
        MsgTimer = New DispatcherTimer()
        MsgTimer.Interval = TimerInterval
        RemoveHandler Me.KeyDown, AddressOf HelloKeyPress
        AddHandler MsgTimer.Tick, AddressOf DisplayHelloandChessText
        AddHandler Me.KeyDown, AddressOf MoreGamesPress
        MsgTimer.Start()
    End Sub

    Private Sub MoreGamesPress(sender As Object, e As KeyEventArgs)
        If Complete = False Then Exit Sub
        MsgTimer.Stop()
        Select Case e.Key
            Case Key.Y
                MsgTimer = Nothing
                RemoveHandler Me.KeyDown, AddressOf MoreGamesPress
                ' DisplayGameList()
                WOPRModule.GameChoice = "Chaos Cafe"
                Close()
            Case Key.N
                MsgTimer = Nothing
                Close()
        End Select
    End Sub

    Private Sub DisplayHelloandChessText()
        ActiveTextBlock.Text = TextToDisplay.Substring(0, TextIndex)
        TextIndex += 1
    End Sub

    Private Sub DisplayGameList()
        Complete = False
        ActiveTextBlock = tblkGamesList
        TextToDisplay = ""
        Dim ct As Byte = Gameslist.Length
        For ct = 1 To Gameslist.Length
            TextToDisplay = TextToDisplay & ct & ") " & Gameslist(ct - 1) & Chr(13)
        Next
        TextIndex = 0
        MsgTimer = New DispatcherTimer()
        MsgTimer.Interval = TimerInterval
        AddHandler MsgTimer.Tick, AddressOf DisplayGamesListText
        MsgTimer.Start()
    End Sub

    Private Sub DisplayGamesListText()
        ActiveTextBlock.Text = TextToDisplay.Substring(0, TextIndex)
        TextIndex += 1
        If TextIndex > TextLength Then
            MsgTimer.Stop()
            MsgTimer = Nothing
            DisplaySelectGameOption()
        End If
    End Sub

    Private Sub DisplaySelectGameOption()
        Complete = False
        ActiveTextBlock = tblkChooseGame
        TextToDisplay = "Choose the number of the game that you would like to play."
        TextIndex = 0
        MsgTimer = New DispatcherTimer()
        MsgTimer.Interval = TimerInterval
        AddHandler MsgTimer.Tick, AddressOf DisplaySelectGameText
        AddHandler Me.KeyDown, AddressOf GameSelect
        MsgTimer.Start()
    End Sub

    Private Sub DisplaySelectGameText()
        ActiveTextBlock.Text = TextToDisplay.Substring(0, TextIndex)
        TextIndex += 1
    End Sub

    Private Sub GameSelect(sender As Object, e As KeyEventArgs)
        If Complete = False Then Exit Sub
        MsgTimer.Stop()
        Select Case e.Key
            Case Key.D1, Key.NumPad1
                WOPRModule.GameChoice = "Manager Mayhem"
                Close()
            Case Key.D2, Key.NumPad2
                WOPRModule.GameChoice = "Kustomer Kaos"
                Close()
            Case Key.D3, Key.NumPad3
                WOPRModule.GameChoice = "Corner!"
                Close()
            Case Key.D4, Key.NumPad4
                WOPRModule.GameChoice = "Pac-Man"
                Close()
        End Select
    End Sub

#End Region

End Class
