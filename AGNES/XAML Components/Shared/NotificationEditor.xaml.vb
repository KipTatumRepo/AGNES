Public Class NotificationEditor

#Region "Properties"

#End Region

#Region "Constructor"
    Public Sub New()
        InitializeComponent()
        dtpStart.DisplayDateStart = Now()
        dtpEnd.DisplayDateStart = Now().AddDays(1)
        PopulateModules()
    End Sub
#End Region

#Region "Public Methods"

#End Region

#Region "Private Methods"
    Private Sub PopulateModules()
        cbxActionTgt.Items.Clear()
        Dim qgm = From gm In AGNESShared.Modules
                  Select gm
                  Order By gm.ModuleName

        For Each gm In qgm
            cbxActionTgt.Items.Add(gm.ModuleName)
        Next

    End Sub
#End Region
End Class
