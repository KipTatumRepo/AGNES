Module WCRModule
    Public Sub Runmodule()
        Dim WCRStartPage As New WCRHello
        Dim WCRCamPage As New WCRCam
        Dim WCRFinalPage As New WCRFinal
        WCRStartPage.ShowDialog()
        WCRCamPage.ShowDialog()
        WCRFinalPage.ShowDialog()
    End Sub
End Module
