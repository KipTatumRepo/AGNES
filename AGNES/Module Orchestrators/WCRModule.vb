Module WCRModule
    Public WCR As New WCRObject
    Public WCRE As WCREntities
    Public UserClosed As Boolean
    'TODO: REFACTOR WCR MODULE TO REFLECT CLEARER ENTITY MODELING

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
