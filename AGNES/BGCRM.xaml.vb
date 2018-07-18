Imports System.ComponentModel

Public Class BGCRM
    Dim BG As objBusinessGroup
    Dim BGC As BGCRMEntity
    Public Sub New()
        InitializeComponent()
        BG = New objBusinessGroup
        BGC = New BGCRMEntity
        PopulateOptions()
        btnSaveFinish.IsEnabled = True
        'TODO: ADD COMPREHENSIVE TRIGGER FOR ENABLING SAVE
        cboGroup.Focus()
    End Sub

#Region "Navigation"
    Private Sub LastPage(sender As Object, e As RoutedEventArgs) Handles btnBack1.Click, btnBack2.Click, btnBack3.Click, btnBack4.Click
        ValidatePage(tabPages.SelectedIndex, 0)
        SavePageToBGObj(tabPages.SelectedIndex)
        tabPages.SelectedIndex -= 1
    End Sub

    Private Sub NextPage(sender As Object, e As RoutedEventArgs) Handles btnFwd1.Click, btnFwd2.Click, btnFwd3.Click, btnFwd4.Click
        If ValidatePage(tabPages.SelectedIndex, 1) = False Then Exit Sub
        SavePageToBGObj(tabPages.SelectedIndex)
        tabPages.SelectedIndex += 1
    End Sub
#End Region

#Region "Data Handling"
    Private Sub SavePageToBGObj(p)
        Dim si As ListBoxItem
        Select Case p
            Case 0          '====== GROUP PAGE
                With BG
                    .OrgName = cboGroup.Text
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
                Dim sl As String = cboLeader.Text
                Dim ql = From c In BGC.Leaders
                         Where c.LeaderName = sl
                         Select c
                For Each c In ql
                    BG.OrgLeader = FormatNumber(c.PID, 0)
                Next

                '// Populate relationship manager
                Dim orm As String = cboRelManager.Text
                Dim qr = From c In BGC.FrequentCustomers
                         Where c.CustomerName = orm
                         Select c
                For Each c In qr
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
        ValidatePage(tabPages.SelectedIndex, 1)
        SavePageToBGObj(tabPages.SelectedIndex)
        BG.Save(BGC)
        BGC.SaveChanges()
        'TODO: Add verification of save
        btnSaveFinish.Content = "Saved"
        btnSaveFinish.IsEnabled = False

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
            Dim li As New ListBoxItem, li1 As New ListBoxItem, li2 As New ListBoxItem
            li.Content = bloc.BuildingName
            li.Tag = "C"
            AddHandler li.MouseDoubleClick, AddressOf LocationItemMove
            lbxLocationsSelect.Items.Add(li)

            li1.Content = bloc.BuildingName
            li1.Tag = "C"
            AddHandler li1.MouseDoubleClick, AddressOf OriginMove
            lbxOriginSelect.Items.Add(li1)

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

    Private Sub LeadTeamMove(sender, eventargs)
        Dim li As New ListBoxItem
        li = sender
        Select Case li.Tag
            Case "C"
                lbxLeadersSelect.Items.Remove(li)
                li.Tag = "S"
                lbxLeadersChosen.Items.Add(li)
            Case "S"
                lbxLeadersChosen.Items.Remove(li)
                li.Tag = "C"
                lbxLeadersSelect.Items.Add(li)
        End Select
        lbxLeadersSelect.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))
        lbxLeadersChosen.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))
    End Sub

    Private Sub CustomerMove(sender, eventargs)
        Dim li As New ListBoxItem
        li = sender
        Select Case li.Tag
            Case "C"
                lbxCustomerSelect.Items.Remove(li)
                li.Tag = "S"
                lbxCustomerChosen.Items.Add(li)
            Case "S"
                lbxCustomerChosen.Items.Remove(li)
                li.Tag = "C"
                lbxCustomerSelect.Items.Add(li)
        End Select
        lbxCustomerSelect.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))
        lbxCustomerChosen.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))
    End Sub

    Private Sub OffsiteMove(sender, eventargs)
        Dim li As New ListBoxItem
        li = sender
        Select Case li.Tag
            Case "C"
                lbxOffsiteLocsSelect.Items.Remove(li)
                li.Tag = "S"
                lbxOffsiteLocsChosen.Items.Add(li)
            Case "S"
                lbxOffsiteLocsChosen.Items.Remove(li)
                li.Tag = "C"
                lbxOffsiteLocsSelect.Items.Add(li)
        End Select
        lbxOffsiteLocsSelect.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))
        lbxOffsiteLocsChosen.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))
    End Sub

    Private Sub NotablesMove(sender, eventargs)
        Dim li As New ListBoxItem
        li = sender
        Select Case li.Tag
            Case "C"
                lbxNotableSelect.Items.Remove(li)
                li.Tag = "S"
                lbxNotableChosen.Items.Add(li)
            Case "S"
                lbxNotableChosen.Items.Remove(li)
                li.Tag = "C"
                lbxNotableSelect.Items.Add(li)
        End Select
        lbxNotableSelect.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))
        lbxNotableChosen.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))
    End Sub

    Private Sub TopTypeMove(sender, eventargs)
        Dim li As New ListBoxItem
        li = sender
        Select Case li.Tag
            Case "C"
                lbxTopETypesSelect.Items.Remove(li)
                li.Tag = "S"
                lbxTopETypesChosen.Items.Add(li)
            Case "S"
                lbxTopETypesChosen.Items.Remove(li)
                li.Tag = "C"
                lbxTopETypesSelect.Items.Add(li)
        End Select
        lbxTopETypesSelect.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))
        lbxTopETypesChosen.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))
    End Sub

    Private Sub TopSpaceMove(sender, eventargs)
        Dim li As New ListBoxItem
        li = sender
        Select Case li.Tag
            Case "C"
                lbxTopSpacesSelect.Items.Remove(li)
                li.Tag = "S"
                lbxTopSpacesChosen.Items.Add(li)
            Case "S"
                lbxTopSpacesChosen.Items.Remove(li)
                li.Tag = "C"
                lbxTopSpacesSelect.Items.Add(li)
        End Select
        lbxTopSpacesSelect.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))
        lbxTopSpacesChosen.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))
    End Sub

    Private Sub InvolvementMove(sender, eventargs)
        Dim li As New ListBoxItem
        li = sender
        Select Case li.Tag
            Case "C"
                lbxInvolveSelect.Items.Remove(li)
                li.Tag = "S"
                lbxInvolveChosen.Items.Add(li)
            Case "S"
                lbxInvolveChosen.Items.Remove(li)
                li.Tag = "C"
                lbxInvolveSelect.Items.Add(li)
        End Select
        lbxInvolveSelect.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))
        lbxInvolveChosen.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))
    End Sub

    Private Sub PlannerMove(sender, eventargs)
        Dim li As New ListBoxItem
        li = sender
        Select Case li.Tag
            Case "C"
                lbxPlannersSelect.Items.Remove(li)
                li.Tag = "S"
                lbxPlannersChosen.Items.Add(li)
            Case "S"
                lbxPlannersChosen.Items.Remove(li)
                li.Tag = "C"
                lbxPlannersSelect.Items.Add(li)
        End Select
        lbxPlannersSelect.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))
        lbxPlannersChosen.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))
    End Sub

    Private Sub OriginMove(sender, eventargs)
        Dim li As New ListBoxItem
        li = sender
        Select Case li.Tag
            Case "C"
                lbxOriginSelect.Items.Remove(li)
                li.Tag = "S"
                Dim uni As New SingleUserInput
                With uni
                    .InputType = 2
                    .lblInputDirection.Text = "Enter the population being moved from this building"
                    .txtUserInput.Focus()
                    .ShowDialog()
                End With
                li.Content = li.Content & "- " & uni.NumVal & " headcount"
                uni.Close()
                lbxOriginChosen.Items.Add(li)
            Case "S"
                lbxOriginChosen.Items.Remove(li)
                li.Tag = "C"
                Dim str As String = li.Content.ToString.Remove(li.Content.ToString.IndexOf("-"))
                li.Content = str
                lbxOriginSelect.Items.Add(li)
        End Select
        lbxOriginSelect.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))
        lbxOriginChosen.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))

    End Sub

#End Region

#Region "Context Menu Actions"
    Private Sub AddBusinessGroup(sender As Object, e As MouseButtonEventArgs)
        Dim uni As New SingleUserInput
        With uni
            .InputType = 0
            .lblInputDirection.Text = "Enter the business group name!"
            .txtUserInput.Focus()
            .ShowDialog()
        End With
        cboGroup.Items.Add(New ComboBoxItem With {.Content = uni.StringVal})
        cboGroup.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))
        uni.Close()

    End Sub

    Private Sub AddCommunicationType(sender As Object, e As MouseButtonEventArgs)
        Dim uni As New SingleUserInput
        With uni
            .InputType = 0
            .lblInputDirection.Text = "Enter the new communication type!"
            .txtUserInput.Focus()
            .ShowDialog()
        End With
        Try
            Dim check = BGC.Communications.Single(Function(p) p.CommType = uni.StringVal.ToString)
            MsgBox("Comm type already exists")  'TODO: EXPAND COMM TYPE EXISTS ALERT
        Catch ex As InvalidOperationException
            Dim comm As New Communication With {.CommType = uni.StringVal}
            BGC.Communications.Add(comm)
            BGC.SaveChanges()
            Dim li As New ListBoxItem With {.Content = uni.StringVal, .Tag = "S"}
            AddHandler li.MouseDoubleClick, AddressOf CommItemMove
            lbxCommsChosen.Items.Add(li)
            lbxCommsChosen.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))
        End Try
        uni.Close()
    End Sub

    Private Sub AddCulture(sender As Object, e As MouseButtonEventArgs)
        Dim uni As New SingleUserInput
        With uni
            .InputType = 0
            .lblInputDirection.Text = "Enter the new culture type!"
            .txtUserInput.Focus()
            .ShowDialog()
        End With
        Try
            Dim check = BGC.GroupCultures.Single(Function(p) p.Culture = uni.StringVal.ToString)
            MsgBox("Culture type already exists")  'TODO: EXPAND CULTURE TYPE EXISTS ALERT
        Catch ex As InvalidOperationException
            Dim cult As New GroupCulture With {.Culture = uni.StringVal}
            BGC.GroupCultures.Add(cult)
            BGC.SaveChanges()
            Dim li As New ListBoxItem With {.Content = uni.StringVal, .Tag = "S"}
            AddHandler li.MouseDoubleClick, AddressOf CultureItemMove
            lbxCultureChosen.Items.Add(li)
            lbxCultureChosen.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))
        End Try
        uni.Close()
    End Sub

    Private Sub AddLeadership(sender As Object, e As MouseButtonEventArgs)
        Dim uni As New SingleUserInput
        With uni
            .InputType = 0
            .lblInputDirection.Text = "Enter the new team member!"
            .txtUserInput.Focus()
            .ShowDialog()
        End With
        Try
            Dim check = BGC.Leaders.Single(Function(p) p.LeaderName = uni.StringVal.ToString)
            MsgBox("Team member already exists")  'TODO: EXPAND LEADERSHIP TEAM MEMBER EXISTS ALERT
        Catch ex As InvalidOperationException
            Dim leader As New Leader With {.LeaderName = uni.StringVal}
            BGC.Leaders.Add(leader)
            BGC.SaveChanges()
            Dim li As New ListBoxItem With {.Content = uni.StringVal, .Tag = "S"}
            AddHandler li.MouseDoubleClick, AddressOf LeadTeamMove
            lbxLeadersChosen.Items.Add(li)
            lbxLeadersChosen.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))
            '// Also add leader to Org Leader combobox option
            cboLeader.Items.Add(uni.StringVal)
            cboLeader.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))
        End Try
        uni.Close()
    End Sub

    Private Sub AddNewCustomer(sender As Object, e As MouseButtonEventArgs)
        Dim uni As New SingleUserInput
        With uni
            .InputType = 0
            .lblInputDirection.Text = "Enter the new customer!"
            .txtUserInput.Focus()
            .ShowDialog()
        End With
        Try
            Dim check = BGC.FrequentCustomers.Single(Function(p) p.CustomerName = uni.StringVal.ToString)
            MsgBox("Customer already exists")  'TODO: EXPAND CUSTOMER EXISTS ALERT
        Catch ex As InvalidOperationException
            Dim customer As New FrequentCustomer With {.CustomerName = uni.StringVal}
            BGC.FrequentCustomers.Add(customer)
            BGC.SaveChanges()
            Dim li As New ListBoxItem With {.Content = uni.StringVal, .Tag = "S"}
            AddHandler li.MouseDoubleClick, AddressOf CustomerMove
            lbxCustomerChosen.Items.Add(li)
            lbxCustomerChosen.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))

            '// Also add customer to relationship manager options
            cboRelManager.Items.Add(uni.StringVal)
            cboRelManager.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))
        End Try
        uni.Close()
    End Sub

    Private Sub AddNewOffsite(sender As Object, e As MouseButtonEventArgs) Handles lbiNewOffsite.PreviewMouseDoubleClick
        Dim uni As New SingleUserInput
        With uni
            .InputType = 0
            .lblInputDirection.Text = "Enter the new offsite location (temporary interface)."
            .txtUserInput.Focus()
            .ShowDialog()
        End With
        Try
            Dim check = BGC.OffsiteLocations.Single(Function(p) p.OffsiteLocName = uni.StringVal.ToString)
            MsgBox("Location already exists")  'TODO: EXPAND OFFSITE LOCATION EXISTS ALERT
        Catch ex As InvalidOperationException
            Dim Offsite As New OffsiteLocation With {.OffsiteLocName = uni.StringVal}
            BGC.OffsiteLocations.Add(Offsite)
            BGC.SaveChanges()
            Dim li As New ListBoxItem With {.Content = uni.StringVal, .Tag = "S"}
            AddHandler li.MouseDoubleClick, AddressOf OffsiteMove
            lbxOffsiteLocsChosen.Items.Add(li)
            lbxOffsiteLocsChosen.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))
        End Try
        uni.Close()
    End Sub

    Private Sub AddNewEventType(sender As Object, e As MouseButtonEventArgs)
        Dim uni As New SingleUserInput
        With uni
            .InputType = 0
            .lblInputDirection.Text = "Enter the new event type (temporary interface)."
            .txtUserInput.Focus()
            .ShowDialog()
        End With
        Try
            Dim check = BGC.EventTypes.Single(Function(p) p.EventType1 = uni.StringVal.ToString)
            MsgBox("Event type already exists")  'TODO: EXPAND EVENT TYPE EXISTS ALERT
        Catch ex As InvalidOperationException
            Dim EventType As New EventType With {.EventType1 = uni.StringVal}
            BGC.EventTypes.Add(EventType)
            BGC.SaveChanges()
            Dim li As New ListBoxItem With {.Content = uni.StringVal, .Tag = "S"}
            AddHandler li.MouseDoubleClick, AddressOf TopTypeMove
            lbxTopETypesChosen.Items.Add(li)
            lbxTopETypesChosen.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))
        End Try
        uni.Close()
    End Sub

    Private Sub AddNewEventSpace(sender As Object, e As MouseButtonEventArgs)
        Dim uni As New SingleUserInput
        With uni
            .InputType = 0
            .lblInputDirection.Text = "Enter the new event space (temporary interface)."
            .txtUserInput.Focus()
            .ShowDialog()
        End With
        Try
            Dim check = BGC.EventSpaces.Single(Function(p) p.SpaceName = uni.StringVal.ToString)
            MsgBox("Event space already exists")  'TODO: EXPAND EVENT SPACE EXISTS ALERT
        Catch ex As InvalidOperationException
            Dim EventSpc As New EventSpace With {.SpaceName = uni.StringVal}
            BGC.EventSpaces.Add(EventSpc)
            BGC.SaveChanges()
            Dim li As New ListBoxItem With {.Content = uni.StringVal, .Tag = "S"}
            AddHandler li.MouseDoubleClick, AddressOf TopSpaceMove
            lbxTopSpacesChosen.Items.Add(li)
            lbxTopSpacesChosen.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))
        End Try
        uni.Close()
    End Sub

    Private Sub AddNewInvolvement(sender As Object, e As MouseButtonEventArgs)
        Dim uni As New SingleUserInput
        With uni
            .InputType = 0
            .lblInputDirection.Text = "Enter the new involvement type."
            .txtUserInput.Focus()
            .ShowDialog()
        End With
        Try
            Dim check = BGC.Involvements.Single(Function(p) p.Involvement1 = uni.StringVal.ToString)
            MsgBox("Event space already exists")  'TODO: EXPAND EVENT SPACE EXISTS ALERT
        Catch ex As InvalidOperationException
            Dim Involve As New Involvement With {.Involvement1 = uni.StringVal}
            BGC.Involvements.Add(Involve)
            BGC.SaveChanges()
            Dim li As New ListBoxItem With {.Content = uni.StringVal, .Tag = "S"}
            AddHandler li.MouseDoubleClick, AddressOf InvolvementMove
            lbxInvolveChosen.Items.Add(li)
            lbxInvolveChosen.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))
        End Try
        uni.Close()
    End Sub

    Private Sub AddNewNotableEvent(sender As Object, e As MouseButtonEventArgs)
        Dim uni As New SingleUserInput
        With uni
            .InputType = 0
            .lblInputDirection.Text = "Enter the new notable event (temporary UI)."
            .txtUserInput.Focus()
            .ShowDialog()
        End With
        Try
            Dim check = BGC.NotableEvents.Single(Function(p) p.EventName = uni.StringVal.ToString)
            MsgBox("Event already exists")  'TODO: EXPAND NOTABLE EVENT EXISTS ALERT
        Catch ex As InvalidOperationException
            Dim Notable As New NotableEvent With {.EventName = uni.StringVal}
            BGC.NotableEvents.Add(Notable)
            BGC.SaveChanges()
            Dim li As New ListBoxItem With {.Content = uni.StringVal, .Tag = "S"}
            AddHandler li.MouseDoubleClick, AddressOf NotablesMove
            lbxNotableChosen.Items.Add(li)
            lbxNotableChosen.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))
        End Try
        uni.Close()
    End Sub

    Private Sub AddNewPlanner(sender As Object, e As MouseButtonEventArgs)
        Dim uni As New SingleUserInput
        With uni
            .InputType = 0
            .lblInputDirection.Text = "Enter the new planner name (temporary UI)."
            .txtUserInput.Focus()
            .ShowDialog()
        End With
        Try
            Dim check = BGC.Planners.Single(Function(p) p.PlannerName = uni.StringVal.ToString)
            MsgBox("Planner already exists")  'TODO: EXPAND PLANNER EXISTS ALERT
        Catch ex As InvalidOperationException
            Dim Plannr As New Planner With {.PlannerName = uni.StringVal}
            BGC.Planners.Add(Plannr)
            BGC.SaveChanges()
            Dim li As New ListBoxItem With {.Content = uni.StringVal, .Tag = "S"}
            AddHandler li.MouseDoubleClick, AddressOf PlannerMove
            lbxPlannersChosen.Items.Add(li)
            lbxPlannersChosen.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))
        End Try
        uni.Close()
    End Sub

#End Region

#Region "Functions"
    Private Function ValidatePage(p, dir) As Boolean
        If dir = 1 Then '// Direction 1 = forward, triggering validation
            Dim invalid As New List(Of String), i As Integer, c As Double
            invalid.Clear()
            Select Case p
                Case 0  '// Group page
                    If cboGroup.SelectedIndex = -1 Then invalid.Add("A business group must be selected.")
                    If cboWorkTimes.SelectedIndex = -1 Then invalid.Add("A work times option must be selected.")
                    If cboWorkspace.SelectedIndex = -1 Then invalid.Add("A workspace option must be selected.")
                    Try
                        i = FormatNumber(txtHeadcount.Text, 0)
                    Catch ex As Exception
                        invalid.Add("Headcount must be a number - enter 0 if currently unknown.")
                    End Try

                Case 1  '// People page
                    If cboLeader.SelectedIndex = -1 Then invalid.Add("An organizational leader must be selected.")
                    If cboRelManager.SelectedIndex = -1 Then invalid.Add("A relationship manager must be selected.")
                Case 2  '// Financials page
                    Try
                        c = FormatNumber(txtRevenue.Text, 2)
                    Catch ex As Exception
                        invalid.Add("Revenue must be a number - enter 0 if currently unknown.")
                    End Try

                    Try
                        c = FormatNumber(txtOffsiteSpend.Text, 2)
                    Catch ex As Exception
                        invalid.Add("Offsite spend must be a number - enter 0 if currently unknown.")
                    End Try

                    Try
                        i = FormatNumber(txtEventCount.Text, 0)
                    Catch ex As Exception
                        invalid.Add("Event count must be a number - enter 0 if currently unknown.")
                    End Try

                    Try
                        i = FormatNumber(txt500EventCount.Text, 0)
                    Catch ex As Exception
                        invalid.Add("500+ event count must be a number - enter 0 if currently unknown.")
                    End Try

                    Try
                        i = FormatNumber(txtCateredEventCount.Text, 0)
                    Catch ex As Exception
                        invalid.Add("Catered event count must be a number - enter 0 if currently unknown.")
                    End Try

                Case 3  '// Events Page
                Case 4  '// CR page
            End Select

            If invalid.Count > 0 Then
                Dim errorstring As String = "", ct As Integer
                For ct = 0 To invalid.Count - 1
                    errorstring = errorstring & invalid(ct) & Chr(13) & Chr(13)
                Next
                MsgBox(errorstring, MsgBoxStyle.OkOnly, "Validation failed")
                Return False
            End If
        End If
        Return True
    End Function

#End Region

End Class
