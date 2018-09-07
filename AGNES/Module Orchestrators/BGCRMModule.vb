Module BGCRMModule
    Public UserClosed As Boolean
    Public BGE As BGCRMEntity

    Public Sub Runmodule()
        BGE = New BGCRMEntity
        Dim BGStartPage As New BGCRM
        BGStartPage.ShowDialog()
        If UserClosed = True Then Exit Sub
    End Sub
End Module
