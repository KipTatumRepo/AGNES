Module WCRModule
    Public WCRStartPage As WCRHello
    Public WCR As WCRObject
    Public WCRE As WCREntities
    Public WCRCamPage As WCRCam
    Public WCRFinalPage As WCRFinal
    Public UserClosed As Boolean
    'REFRESH: REFACTOR WCR MODULE TO REFLECT CLEARER ENTITY MODELING

    Public Sub Runmodule()
        WCRStartPage = New WCRHello
        WCR = New WCRObject
        WCRE = New WCREntities
        WCRStartPage.ShowDialog()
        If UserClosed = True Then
            TimerOne = Nothing
            TimerTwo = Nothing
            WCR = Nothing
            Exit Sub
        End If
        WCRCamPage = New WCRCam
        WCRCamPage.ShowDialog()
        If UserClosed = True Then
            TimerOne = Nothing
            TimerTwo = Nothing
            WCR = Nothing
            Exit Sub
        End If
        WCRFinalPage = New WCRFinal
        WCRFinalPage.ShowDialog()
        WCR = Nothing
        TimerOne = Nothing
        TimerTwo = Nothing
    End Sub
End Module
