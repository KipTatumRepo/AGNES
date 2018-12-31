Module WOPRModule
    Private ActiveWOPR As WOPR
    Public GameChoice As String
    Public Sub RunModule()
        ActiveWOPR = New WOPR(My.Settings.UserShortName)
        ActiveWOPR.ShowDialog()
        If GameChoice <> "" Then
            Select Case GameChoice
                Case "Chaos Cafe"
                    Dim Chaos As New ChaosCafeSplash
                    Chaos.ShowDialog()
                    Chaos.Close()
                Case "Manager Mayhem"
                Case "Kustomer Kaos"
                Case "Corner!"
                Case "Pac-Man"
                    Dim PacGame As New PacMan
                    PacGame.ShowDialog()
                    PacGame.Close()
            End Select
        End If
    End Sub

End Module
