Module FlashModule
    Public UserClosed As Boolean
    Public Sub Runmodule()
        Dim FlashType As String = "WCC" 'TODO: REMOVE PLACEHOLDER FLASH TYPE
        '// Determine which flash, or flashes, user has access to.  If multiple, give them a choice.  If not, move forward
        Dim FlashUnit As Long = 19837   'TODO: REMOVE PLACEHOLDER UNIT
        '// Determine which unit, or units, user has access to.  If multiple, give them a choice.  If not, move forward


        Dim FlashPage As New Flash(FlashType, FlashUnit)
        FlashPage.ShowDialog()
        If UserClosed = True Then Exit Sub
    End Sub
End Module
