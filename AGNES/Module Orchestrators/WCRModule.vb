Module WCRModule
    Public WCR As New WCRObject
    Public UserClosed As Boolean
    Public WCRE As WCREntities
    Public Sub Runmodule()
        WCRE = New WCREntities
        Dim WCRStartPage As New WCRHello
        WCRStartPage.ShowDialog()
        If UserClosed = True Then
            WCR = Nothing
            Exit Sub
        End If
        Dim WCRCamPage As New WCRCam
        WCRCamPage.ShowDialog()
        If UserClosed = True Then
            WCR = Nothing
            Exit Sub
        End If
        Dim WCRFinalPage As New WCRFinal
        WCRFinalPage.ShowDialog()
        WCR = Nothing
    End Sub
End Module
