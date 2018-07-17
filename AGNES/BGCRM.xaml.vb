Imports System.ComponentModel

Public Class BGCRM
    Dim BG As objBusinessGroup
    Dim BGC As BGCRMEntity
    Public Sub New()
        InitializeComponent()
        BG = New objBusinessGroup
        BGC = New BGCRMEntity
        PopulateOptions()

        '//TEST
        Dim cbi As New ComboBoxItem
        cbi.Content = "Core Engineering"
        cboGroup.Items.Add(cbi)
        '//TEST

        cboGroup.Focus()
    End Sub
#Region "Navigation"
    Private Sub LastPage(sender As Object, e As RoutedEventArgs) Handles btnBack1.Click, btnBack2.Click, btnBack3.Click, btnBack4.Click
        ValidatePage(tabPages.SelectedIndex)
        SavePageToBGObj(tabPages.SelectedIndex)
        tabPages.SelectedIndex -= 1
    End Sub
    Private Sub NextPage(sender As Object, e As RoutedEventArgs) Handles btnFwd1.Click, btnFwd2.Click, btnFwd3.Click, btnFwd4.Click
        ValidatePage(tabPages.SelectedIndex)
        SavePageToBGObj(tabPages.SelectedIndex)
        tabPages.SelectedIndex += 1
    End Sub
#End Region

#Region "Data Handling"
    Private Sub ValidatePage(p)
        MsgBox("Validatation routine pending construction")
    End Sub

    Private Sub SavePageToBGObj(p)
        Dim si As ListBoxItem
        Select Case p
            Case 0          '====== GROUP PAGE
                With BG
                    .OrgName = cboGroup.SelectedValue
                    .Overview = txtOverview.Text
                    .Headcount = FormatNumber(txtHeadcount.Text, 0)
                    .WorkTimes = cboWorkTimes.SelectedIndex
                    .OnsiteRemote = cboWorkspace.SelectedIndex
                End With

                '// Populate chosen communications into array

                For Each si In lbxCommsChosen.Items
                    Dim sc As String = si.Content
                    Dim query = From comms In BGC.Communications
                                Where comms.CommType.ToString = sc
                                Select comms
                    For Each comm In query
                        BG.Communications.Add(FormatNumber(comm.PID, 0))
                    Next
                Next

                '// Populate chosen culture into array
                For Each si In lbxCultureChosen.Items
                    Dim sc As String = si.Content
                    Dim q = From c In BGC.GroupCultures
                            Where c.Culture = sc
                            Select c
                    For Each c In q
                        BG.Culture.Add(FormatNumber(c.PID, 0))
                    Next
                Next

                '// Populate chosen locations into array
                For Each si In lbxLocationsChosen.Items
                    Dim sc As String = si.Content
                    Dim q = From c In BGC.Locations
                            Where c.BuildingName = sc
                            Select c
                    For Each c In q
                        BG.Locations.Add(FormatNumber(c.PID, 0))
                    Next
                Next

            Case 1      '======PEOPLE PAGE
                '// Populate Org leader
                Dim sl As String = cboLeader.SelectedValue
                Dim ql = From c In BGC.Leaders
                         Where c.LeaderName = sl
                         Select c
                For Each c In ql
                    BG.OrgLeader = FormatNumber(c.PID, 0)
                Next

                '// Populate relationship manager
                Dim orm As String = cboRelManager.SelectedValue
                Dim qr = From c In BGC.FrequentCustomers
                         Where c.CustomerName = orm
                         Select c
                For Each c In ql
                    BG.RelationshipMgr = FormatNumber(c.PID, 0)
                Next

                '// Populate chosen leaders into array
                For Each si In lbxLeadersChosen.Items
                    Dim slt As String = si.Content
                    Dim q = From c In BGC.Leaders
                            Where c.LeaderName = slt
                            Select c
                    For Each c In q
                        BG.Leadership.Add(FormatNumber(c.PID, 0))
                    Next
                Next

                '// Populate chosen customers into array
                For Each si In lbxCustomerChosen.Items
                    Dim sfc As String = si.Content
                    Dim q = From c In BGC.FrequentCustomers
                            Where c.CustomerName = sfc
                            Select c
                    For Each c In q
                        BG.FrequentCustomers.Add(FormatNumber(c.PID, 0))
                    Next
                Next

            Case 2          '======FINANCIAL PAGE
                With BG
                    .TotalRevenue = FormatNumber(txtRevenue.Text, 2)
                    .OffSiteSpend = FormatNumber(txtOffsiteSpend.Text, 2)
                    .TotalEvents = FormatNumber(txtEventCount.Text, 0)
                    .Events500 = FormatNumber(txt500EventCount.Text, 0)
                    .CateredEvents = FormatNumber(txtCateredEventCount.Text, 0)
                End With

                '// Populate top offsite locations into array
                For Each si In lbxOffsiteLocsChosen.Items
                    Dim slt As String = si.Content
                    Dim q = From c In BGC.OffsiteLocations
                            Where c.OffsiteLocName = slt
                            Select c
                    For Each c In q
                        BG.TopOffsiteLocations.Add(FormatNumber(c.PID, 0))
                    Next
                Next

            Case 3          '======EVENTS PAGE
                '// Populate notable events into array
                For Each si In lbxNotableChosen.Items
                    Dim slt As String = si.Content
                    Dim q = From c In BGC.NotableEvents
                            Where c.EventName = slt
                            Select c
                    For Each c In q
                        BG.NotableEvents.Add(FormatNumber(c.PID, 0))
                    Next
                Next

                '// Populate top event types into array
                For Each si In lbxTopETypesChosen.Items
                    Dim slt As String = si.Content
                    Dim q = From c In BGC.EventTypes
                            Where c.EventType1 = slt
                            Select c
                    For Each c In q
                        BG.TopEventTypes.Add(FormatNumber(c.PID, 0))
                    Next
                Next

                '// Populate top event spaces into array
                For Each si In lbxTopSpacesChosen.Items
                    Dim slt As String = si.Content
                    Dim q = From c In BGC.EventSpaces
                            Where c.SpaceName = slt
                            Select c
                    For Each c In q
                        BG.TopBookedSpaces.Add(FormatNumber(c.PID, 0))
                    Next
                Next

                '// Populate top eventions involvements into array
                For Each si In lbxInvolveChosen.Items
                    Dim slt As String = si.Content
                    Dim q = From c In BGC.Involvements
                            Where c.Involvement1 = slt
                            Select c
                    For Each c In q
                        BG.EventionsInvolvement.Add(FormatNumber(c.PID, 0))
                    Next
                Next

                '// Populate embedded planners into array
                For Each si In lbxPlannersChosen.Items
                    Dim slt As String = si.Content
                    Dim q = From c In BGC.Planners
                            Where c.PlannerName = slt
                            Select c
                    For Each c In q
                        BG.EmbeddedPlanners.Add(FormatNumber(c.PID, 0))
                    Next
                Next

            Case 4          '======CAMPUS REFRESH PAGE
                'TODO: Build out CR objects and populate into BG object

        End Select
    End Sub

    Private Sub SaveToEDM(sender As Object, e As RoutedEventArgs) Handles btnSaveFinish.Click
        ValidatePage(tabPages.SelectedIndex)
        SavePageToBGObj(tabPages.SelectedIndex)
        BG.Save(BGC)
        BGC.SaveChanges()
    End Sub

#End Region

#Region "Field Management"
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
        For Each bcomm In cq
            Dim li As New ListBoxItem
            li.Content = bcomm.CommType
            li.Tag = "C"
            AddHandler li.MouseDoubleClick, AddressOf CommItemMove
            lbxCommSelect.Items.Add(li)
        Next

        '// Populate culture options
        lbxCultureSelect.Items.Clear()
        Dim cuq = From bcult In BGC.GroupCultures Select bcult Order By bcult.Culture
        For Each bcult In cuq
            Dim li As New ListBoxItem
            li.Content = bcult.Culture
            li.Tag = "C"
            AddHandler li.MouseDoubleClick, AddressOf CultureItemMove
            lbxCultureSelect.Items.Add(li)
        Next

        '// Populate location, Origin building, and Destination building options - shared datasource
        lbxLocationsSelect.Items.Clear()
        lbxOriginSelect.Items.Clear()
        lbxDestination.Items.Clear()
        Dim loq = From bloc In BGC.Locations Select bloc Order By bloc.BuildingName
        For Each bloc In loq
            Dim li As New ListBoxItem
            li.Content = bloc.BuildingName
            li.Tag = "C"
            AddHandler li.MouseDoubleClick, AddressOf LocationItemMove
            lbxLocationsSelect.Items.Add(li)

            'TODO: ADD HANDLERS FOR ORIGIN AND DESTINATION LOCATION BLOCKS
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

    Private Sub CommItemMove(sender, eventargs)
        Dim li As New ListBoxItem
        li = sender
        Select Case li.Tag
            Case "C"
                lbxCommSelect.Items.Remove(li)
                li.Tag = "S"
                lbxCommsChosen.Items.Add(li)
            Case "S"
                lbxCommsChosen.Items.Remove(li)
                li.Tag = "C"
                lbxCommSelect.Items.Add(li)
        End Select
        lbxCommSelect.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))
        lbxCommsChosen.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))

    End Sub

    Private Sub CultureItemMove(sender, eventargs)
        Dim li As New ListBoxItem
        li = sender
        Select Case li.Tag
            Case "C"
                lbxCultureSelect.Items.Remove(li)
                li.Tag = "S"
                lbxCultureChosen.Items.Add(li)
            Case "S"
                lbxCultureChosen.Items.Remove(li)
                li.Tag = "C"
                lbxCultureSelect.Items.Add(li)
        End Select
        lbxCultureSelect.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))
        lbxCultureChosen.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))

    End Sub

    Private Sub LocationItemMove(sender, eventargs)
        Dim li As New ListBoxItem
        li = sender
        Select Case li.Tag
            Case "C"
                lbxLocationsSelect.Items.Remove(li)
                li.Tag = "S"
                lbxLocationsChosen.Items.Add(li)
            Case "S"
                lbxLocationsChosen.Items.Remove(li)
                li.Tag = "C"
                lbxLocationsSelect.Items.Add(li)
        End Select
        lbxLocationsSelect.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))
        lbxLocationsChosen.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))

    End Sub

#End Region

#Region "Context Menu Actions"
    Private Sub AddBusinessGroup(sender As Object, e As MouseButtonEventArgs) Handles cbiAddBG.PreviewMouseLeftButtonDown
        Dim uni As New SingleUserInput
        With uni
            .InputType = 0
            .lblInputDirection.Text = "Enter the business group name!"
            .txtUserInput.Focus()
            .ShowDialog()
        End With
        cboGroup.Items.Add(New ComboBoxItem With {.Content = uni.StringVal})
        uni.Close()

    End Sub
#End Region
End Class
