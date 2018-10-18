Module AdminModule
    Public AdminPage As Admin

    Public Sub Runmodule()
        AdminPage = New Admin
        AdminPage.ShowDialog()
    End Sub
End Module
