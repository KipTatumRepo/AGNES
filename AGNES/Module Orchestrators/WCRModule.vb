Module WCRModule
    Public WCR As New WCRObject
    Public WCRE As WCREntities
    Public UserClosed As Boolean
    'TODO: ADD VALIDATION TO PREVENT DUPLICATE CAM CHECKS FROM BEING ENTERED
    Public Sub Runmodule()
        Dim WCRStartPage As New WCRHello
        WCRE = New WCREntities
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
