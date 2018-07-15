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
        Select Case tabPages.SelectedIndex
            Case 0 '// Group page
                ValidateGroupPage()
                SaveGroupPage()
                tabPages.SelectedIndex = 1
            Case 1 '// People page
                ValidatePeoplePage()
                SavePeoplePage()
                tabPages.SelectedIndex = 2
            Case 2 '// Finance page
                ValidateFinancePage()
                SaveFinancePage()
                tabPages.SelectedIndex = 3
            Case 3 '// Events page
                ValidateEventsPage()
                SaveEventsPage()
                tabPages.SelectedIndex = 4
            Case 4 '// CR page
                ValidateCRPage()
                SaveCRPage()
        End Select

    End Sub
    Private Sub ValidateGroupPage()
        Dim ph As String = ""
    End Sub
    Private Sub ValidatePeoplePage()
        Dim ph As String = ""
    End Sub
    Private Sub ValidateFinancePage()
        Dim ph As String = ""
    End Sub
    Private Sub ValidateEventsPage()
        Dim ph As String = ""
    End Sub
    Private Sub ValidateCRPage()
        Dim ph As String = ""
    End Sub
    Private Sub SaveGroupPage()
        'Dim bgnm As String = cboGroup.SelectedValue.ToString

        'Try
        '    Dim IsNew = BGC.BusinessGroups.Single(Function(p) p.BusinessGroupName = bgnm)

        'Catch ex As InvalidOperationException
        '    Try
        '        Dim bg As New BusinessGroup
        '        With bg
        '            .BusinessGroupName = bgnm
        '            .GroupOverview = txtOverview.Text
        '            .Headcount = FormatNumber(txtHeadcount.Text, 0)
        '            .WorkTimes = 1
        '            .OnsiteRemote = 1
        '            .OrgLeader = 1
        '            .RelMgr = 1
        '            .Revenue = 1234.56
        '            .Events = 100
        '            .Events500 = 20
        '            .EventsCatered = 10
        '            .OffsiteSpend = 123.45
        '        End With
        '        BGC.BusinessGroups.Add(bg)
        '        BGC.SaveChanges()
        '    Catch excep As Exception
        '    End Try

        'Catch ex As Exception
        'End Try

        '// SAVE DATA TO OBJECT, PASS EDM PARAMETER TO OBJECT AT END IN ORDER TO WRITE BACK TO THE DB

    End Sub

    Private Sub SavePeoplePage()
        Dim ph As String = ""
    End Sub

    Private Sub SaveFinancePage()
        Dim ph As String = ""
    End Sub

    Private Sub SaveEventsPage()
        Dim ph As String = ""
    End Sub

    Private Sub SaveCRPage()
        Dim ph As String = ""
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

        '// Populate location, Origin building, and Destination building options - shared datasource
        lbxLocationsSelect.Items.Clear()
        lbxOriginSelect.Items.Clear()
        lbxDestination.Items.Clear()
        Dim loq = From bloc In BGC.Locations Select bloc Order By bloc.BuildingName
        For Each bloc In loq
            lbxLocationsSelect.Items.Add(bloc.BuildingName)
            lbxOriginSelect.Items.Add(bloc.BuildingName)
            lbxDestination.Items.Add(bloc.BuildingName)
        Next

        '// Populate leader and leadership team options - shared datasource
        cboLeader.Items.Clear()
        lbxLeadersSelect.Items.Clear()
        Dim lq = From bldr In BGC.Leaders Select bldr Order By bldr.LeaderName
        For Each bldr In lq
            cboLeader.Items.Add(bldr.LeaderName)
            lbxLeadersSelect.Items.Add(bldr.LeaderName)
        Next

        '// Populate relationship manager and frequent customers options - shared datasource
        cboRelManager.Items.Clear()
        lbxCustomerSelect.Items.Clear()
        Dim rmq = From brlm In BGC.FrequentCustomers Select brlm Order By brlm.CustomerName
        For Each brlm In rmq
            cboRelManager.Items.Add(brlm.CustomerName)
            lbxCustomerSelect.Items.Add(brlm.CustomerName)
        Next

        '// Populate offsite location options
        lbxOffsiteLocsSelect.Items.Clear()
        Dim olq = From osl In BGC.OffsiteLocations Select osl Order By osl.OffsiteLocName
        For Each osl In olq : lbxCustomerSelect.Items.Add(osl.OffsiteLocName) : Next

        '// Populate notable event options
        lbxNotableSelect.Items.Clear()
        Dim neq = From nev In BGC.NotableEvents Select nev Order By nev.EventName
        For Each nev In neq : lbxNotableSelect.Items.Add(nev.EventName) : Next

        '// Populate top event type options
        lbxTopETypesSelect.Items.Clear()
        Dim teq = From tet In BGC.EventTypes Select tet Order By tet.TypeDescription
        For Each tet In teq : lbxTopETypesSelect.Items.Add(tet.TypeDescription) : Next

        '// Populate top booked spaces options
        lbxTopSpacesSelect.Items.Clear()
        Dim tsq = From tsb In BGC.EventSpaces Select tsb Order By tsb.SpaceName
        For Each tsb In tsq : lbxTopSpacesSelect.Items.Add(tsb.SpaceName) : Next

        '// Populate eventions involvement options
        lbxInvolveSelect.Items.Clear()
        Dim tiq = From tii In BGC.Involvements Select tii Order By tii.Involvement1
        For Each tii In tiq : lbxInvolveSelect.Items.Add(tii.Involvement1) : Next

        '// Populate embedded planner options
        lbxPlannersSelect.Items.Clear()
        Dim epq = From epl In BGC.Planners Select epl Order By epl.PlannerName
        For Each epl In epq : lbxPlannersSelect.Items.Add(epl.PlannerName) : Next



    End Sub

End Class
