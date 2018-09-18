Module FlashModule
    Public UserClosed As Boolean
    Public Sub Runmodule()
        ' BGE = New BGCRMEntity
        Dim FlashPage As New Flash
        FlashPage.ShowDialog()
        If UserClosed = True Then Exit Sub
    End Sub
End Module
