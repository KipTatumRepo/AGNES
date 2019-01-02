Module CSharpModules
    Public Sub RunHRMgrModule()
        Dim UserAccess As Long
        If My.Settings.UserLevel <> 4 Then
            UserAccess = 0
        Else
            UserAccess = My.Settings.UserID
        End If
        Dim HRMgr As New AGNESCSharp.MainWindow
        HRMgr.Close()
    End Sub

End Module
