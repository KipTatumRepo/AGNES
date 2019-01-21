Public Class NotificationEditor

    '#  Group 0 = All users
    '#  Group 994 = Chefs
    '#  Group 995 = Cafe FSDs
    '#  Group 996 = POS Team
    '#  Group 997 = HR
    '#  Group 998 = Superusers
    '#  Group 999 = Admin

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
