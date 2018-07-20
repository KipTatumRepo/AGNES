Module WCRModule
    Public WCR As New WCRObject
    Public Sub Runmodule()
        Dim WCRStartPage As New WCRHello
        WCRStartPage.ShowDialog()
        Dim WCRCamPage As New WCRCam
        WCRCamPage.ShowDialog()
        Dim WCRFinalPage As New WCRFinal
        WCRFinalPage.ShowDialog()
    End Sub
End Module
