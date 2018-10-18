Module BGCRMModule
    Public UserClosed As Boolean

    Public Sub Runmodule()
        Dim BGStartPage As New BGCRM
        BGStartPage.ShowDialog()
    End Sub
End Module
