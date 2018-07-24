Module WCRModule
    Public WCR As New WCRObject
    Public UserClosed As Boolean
    Public WCRE As WCREntities
    Public Sub Runmodule()
        WCRE = New WCREntities
        Dim WCRStartPage As New WCRHello
        WCRStartPage.ShowDialog()
        If UserClosed = True Then Exit Sub
        Dim WCRCamPage As New WCRCam
        WCRCamPage.ShowDialog()
        If UserClosed = True Then Exit Sub
        Dim WCRFinalPage As New WCRFinal
        WCRFinalPage.ShowDialog()
    End Sub
End Module
