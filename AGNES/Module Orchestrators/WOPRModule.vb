Module WOPRModule
    Private ActiveWOPR As WOPR
    Public GameChoice As String
    Public MusicChannel As MediaPlayer
    Public MusicLoop As Boolean
    Public SoundChannelOne As MediaPlayer
    Public SoundChannelTwo As MediaPlayer
    Public PlayerScore As Long
    Public StartContinueGame As Boolean

    Public ChefDirection As Byte
    Public ChefFeets As Byte = 1

    Public Sub RunModule()
        ActiveWOPR = New WOPR(My.Settings.UserShortName)
        ActiveWOPR.ShowDialog()
        If GameChoice <> "" Then
            Select Case GameChoice
                Case "Chaos Cafe"
                    Dim ChaosSplash As New ChaosCafeSplash
                    ChaosSplash.ShowDialog()
                    ChaosSplash.Close()
                    If StartContinueGame = False Then Exit Sub
                    StartContinueGame = False

                    '//Round One
                    Dim ChaosRoundOne As New ChaosOne
                    ChaosRoundOne.ShowDialog()
                    ChaosRoundOne.Close()
                Case "Pac-Man"
                    Dim PacGame As New PacMan
                    PacGame.ShowDialog()
                    PacGame.Close()
            End Select
        End If
    End Sub

    Public Sub MusicChoice(FileLoc As String)
        MusicChannel = New MediaPlayer
        AddHandler MusicChannel.MediaEnded, AddressOf LoopMusic
        MusicChannel.Open(New Uri(FileLoc, UriKind.Relative))
        MusicChannel.Play()
    End Sub

    Public Sub SoundOneChoice(FileLoc As String)
        SoundChannelOne = New MediaPlayer
        SoundChannelOne.Open(New Uri(FileLoc, UriKind.Relative))
        SoundChannelOne.Play()
    End Sub

    Public Sub SoundTwoChoice(FileLoc As String)
        SoundChannelTwo = New MediaPlayer
        SoundChannelTwo.Open(New Uri(FileLoc, UriKind.Relative))
        SoundChannelTwo.Play()
    End Sub

    Private Sub LoopMusic(sender As Object, e As EventArgs)
        If MusicLoop = False Then
            MusicChannel.Stop()
        Else
            MusicChannel.Position = TimeSpan.Zero
            MusicChannel.Play()
        End If
    End Sub

End Module

