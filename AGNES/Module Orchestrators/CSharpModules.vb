Module CSharpModules
    Public Sub RunHRMgrModule()
        'CRITICAL: ERROR TRAP TEMPORARILY ADDED
        Dim UserAccess As Long
        Try
            If My.Settings.UserLevel <> 4 Then
                UserAccess = 0
            Else
                UserAccess = My.Settings.UserID
            End If
            MsgBox("First initializing action okay for userID: " & UserAccess)
        Catch ex As Exception
            MsgBox("First initializing action failed - error was " & ex.Message)
        End Try



        Try
            Dim HRMgr As New AGNESCSharp.MainWindow(UserAccess)
            HRMgr.Close()
        Catch ex As Exception
            MsgBox("Second initializing action failed - error was " & ex.Message)
        End Try


    End Sub

End Module
