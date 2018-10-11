Module BGCRMModule
    Public UserClosed As Boolean
    Public BGE As BGCRMEntity

    Public Sub Runmodule()
        BGE = New BGCRMEntity
        Dim BGStartPage As New BGCRM
        BGStartPage.ShowDialog()
    End Sub
End Module
