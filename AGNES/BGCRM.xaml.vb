Public Class BGCRM
    Dim BG As objBusinessGroup
    Dim BGC As BGCRMEntity
    Public Sub New()
        InitializeComponent()
        BG = New objBusinessGroup
        BGC = New BGCRMEntity
        PopulateOptions()
        cboGroup.Focus()
    End Sub

    Private Sub SaveAndNext(sender As Object, e As RoutedEventArgs) Handles btnSaveNextGroup.Click, btnSaveNextPeople.Click, btnSaveFinish.Click, btnSaveNextEvents.Click, btnSaveNextFinances.Click
        tabPages.SelectedIndex += 1

    End Sub

    Private Sub PopulateOptions()

        '// Populate business group names
        cboGroup.Items.Clear()
        Dim gq = From bgroup In BGC.BusinessGroups Select bgroup
        For Each bgroup In gq : cboGroup.Items.Add(bgroup.BusinessGroupName) : Next

        '// Populate work times
        With cboWorkTimes.Items
            .Clear()
            .Add("Banker hours")
            .Add("Early birds")
            .Add("Late arrival")
        End With

        '// Populate workspace types - hard coded for now (7/15/18)
        With cboWorkspace.Items
            .Clear()
            .Add("Onsite")
            .Add("Remote")
        End With

        '// Populate communication options
        lbxCommSelect.Items.Clear()
        Dim cq = From bcomm In BGC.Communications Select bcomm Order By bcomm.CommType
        For Each bcomm In cq : lbxCommSelect.Items.Add(bcomm.CommType) : Next

        '// Populate culture options
        lbxCultureSelect.Items.Clear()
        Dim cuq = From bcult In BGC.GroupCultures Select bcult Order By bcult.Culture
        For Each bcult In cuq : lbxCultureSelect.Items.Add(bcult.Culture) : Next

        '// Populate location options
        lbxLocationsSelect.Items.Clear()
        Dim loq = From bloc In BGC.Locations Select bloc Order By bloc.BuildingName
        For Each bloc In loq : lbxLocationsSelect.Items.Add(bloc.BuildingName) : Next
    End Sub
End Class
