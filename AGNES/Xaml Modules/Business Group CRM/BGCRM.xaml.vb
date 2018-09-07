Imports System.ComponentModel
Imports System.Linq
'TODO: NEED LOAD ROUTINE
Public Class BGCRM
    Dim BG As objBusinessGroup
    Dim BGC As BGCRMEntity
    Dim SD As BIEntities
    Dim curRevenue As CurrencyBox
    Dim curOffsite As CurrencyBox
    Dim numEventCount As NumberBox
    Dim num500Events As NumberBox
    Dim numCatered As NumberBox
    Dim numHeadcount As NumberBox
    Dim numPopMoving As NumberBox

    Public Sub New()
        InitializeComponent()
        BG = New objBusinessGroup
        BGC = New BGCRMEntity
        SD = New BIEntities
        PopulateOptions()
        curRevenue = New CurrencyBox(189, False, True, False, True, True, AgnesBaseInput.FontSz.Medium) With {.Margin = New Thickness(269, 28, 0, 0)}
        curOffsite = New CurrencyBox(189, False, True, False, True, True, AgnesBaseInput.FontSz.Medium) With {.Margin = New Thickness(269, 88, 0, 0)}
        numEventCount = New NumberBox(189, True, False, True, False, True, AgnesBaseInput.FontSz.Medium, 0, "0") With {.Margin = New Thickness(269, 156, 0, 0)}
        num500Events = New NumberBox(189, True, False, True, False, True, AgnesBaseInput.FontSz.Medium, 0, "0") With {.Margin = New Thickness(807, 28, 0, 0)}
        numCatered = New NumberBox(189, True, False, True, False, True, AgnesBaseInput.FontSz.Medium, 0, "0") With {.Margin = New Thickness(807, 88, 0, 0)}
        numHeadcount = New NumberBox(108, True, False, True, False, True, AgnesBaseInput.FontSz.Medium, 0, "0") With {.Margin = New Thickness(145, 149, 0, 0)}
        numPopMoving = New NumberBox(126, True, False, True, False, True, AgnesBaseInput.FontSz.Medium, 0, "0") With {.Margin = New Thickness(798, 162, 0, 0), .IsEnabled = False}
        Dim txtbx As TextBox = numPopMoving.Children(1)
        txtbx.IsTabStop = True
        txtbx.TabIndex = 5

        Dim tb As TextBox
        tb = numHeadcount.Children(1)
        tb.IsTabStop = True
        tb.TabIndex = 3

        tb = numPopMoving.Children(1)
        tb.IsTabStop = True
        tb.TabIndex = 1

        With grdGroup.Children
            .Add(numHeadcount)
        End With

        With grdFinances.Children
            .Add(curRevenue)
            .Add(curOffsite)
            .Add(numEventCount)
            .Add(num500Events)
            .Add(numCatered)
        End With

        With grdCampusRefresh.Children
            .Add(numPopMoving)
        End With
        btnSaveFinish.IsEnabled = True
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
        Dim si As ListBoxItem, tb As TextBox, v As Double
        Select Case p
            Case 0          '====== GROUP PAGE
                tb = numHeadcount.Children(1)
                v = FormatNumber(tb.Text, 0)
                BG.Headcount = v
                With BG
                    .OrgName = cboGroup.Text
                    .Overview = txtOverview.Text
                    .OnsiteRemote = cboWorkspace.SelectedIndex
                End With

                '// Populate work time
                Dim wt As String = cboWorkTimes.Text
                Dim qwt = From c In BGC.WorkTimes
                          Where c.WorkTime1 = wt
                          Select c
                For Each c In qwt
                    BG.WorkTimes = FormatNumber(c.PID, 0)
                Next

                '// Populate work locations/environment
                Dim wl As String = cboWorkspace.Text
                Dim qwl = From c In BGC.WorkLocations
                          Where c.WorkLocation1 = wl
                          Select c
                For Each c In qwl
                    BG.OnsiteRemote = FormatNumber(c.PID, 0)
                Next

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
                    Dim q = From c In SD.MasterBuildingLists
                            Where c.BuildingName Is sc
                            Select c
                    For Each c In q
                        Dim nu As Long = c.PID
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

                tb = curRevenue.Children(1)
                v = FormatNumber(tb.Text, 2)
                BG.TotalRevenue = v
                tb = curOffsite.Children(1)
                v = FormatNumber(tb.Text, 2)
                BG.OffSiteSpend = v
                tb = numEventCount.Children(1)
                v = FormatNumber(tb.Text, numEventCount.NumberOfDecimals)
                BG.TotalEvents = v
                tb = num500Events.Children(1)
                v = FormatNumber(tb.Text, num500Events.NumberOfDecimals)
                BG.Events500 = v
                tb = numCatered.Children(1)
                v = FormatNumber(tb.Text, numCatered.NumberOfDecimals)
                BG.CateredEvents = v

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
        End Select
    End Sub

    Private Sub SaveToEDM(sender As Object, e As RoutedEventArgs) Handles btnSaveFinish.Click
        ValidatePage(tabPages.SelectedIndex, 1)
        SavePageToBGObj(tabPages.SelectedIndex)
        BG.Save(BGC)
        If BG.SaveSuccessful = True Then
            BGC.SaveChanges()
            btnSaveFinish.Content = "Saved"
            btnSaveFinish.IsEnabled = False
        Else
            Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Medium, AgnesMessageBox.MsgBoxLayout.FullText, AgnesMessageBox.MsgBoxType.OkOnly, 12,, "Save failed!",, "Unable to save.  Please checks your work and try again.")
            amsg.ShowDialog()
            amsg.Close()
        End If
    End Sub

    Private Sub LoadRefreshEvents()
        lbxRefreshEvents.Items.Clear()
        Dim GroupID As Integer
        Dim GetGroupID = From businessgroups In BGC.BusinessGroups
                         Where businessgroups.BusinessGroupName Is cboGroup.SelectedValue
                         Select businessgroups
        For Each c In GetGroupID
            GroupID = FormatNumber(c.BusinessGroupID, 0)
        Next
        Dim GetRefreshEvents = From refreshevents In BGC.RefreshEvents
                               Where refreshevents.BusinessGroupId = GroupID
                               Select refreshevents
        For Each c In GetRefreshEvents
            lbxRefreshEvents.Items.Add(c.RefreshEventName)
        Next
        'TODO:  Add routine to load the events into the BGObject CREvents list property
    End Sub

    Private Sub SaveRefreshEvent(sender As Object, e As EventArgs) Handles btnSaveRefreshEvent.Click

        'TODO: '// Validate refresh event doesn't exist

        Dim NewCr As New RefreshEvent, tb As TextBox = numPopMoving.Children(1)
        With NewCr
            .RefreshEventName = txtEventName.Text
            .MoveStart = dtpStartDate.SelectedDate
            .MoveEnd = dtpEndDate.SelectedDate
            .TotalPopulation = FormatNumber(tb.Text, 0)
            .DestinationBuilding = lbxDestination.SelectedValue
        End With
        Dim lbi As ListBoxItem
        For Each lbi In lbxOriginChosen.Items
            Dim newBldg As New CRBuilding, BldgName As String = "", MovePop As Integer = 0, BldgId As Integer = 0
            Dim ParseString() As String
            ParseString = Split(lbi.Content, "--", 2)
            BldgName = ParseString(0)
            MovePop = FormatNumber(ParseString(1).Replace(" headcount", ""), 0)
            With newBldg
                .BuildingName = BldgName
                .MovePopulation = MovePop
                .BuildingId = FetchBuildingID(BldgName)
            End With
            NewCr.BuildingsMoving.Add(newBldg)
            Dim NewLbi As New ListBoxItem With {.Content = BldgName}
            lbxOriginSelect.Items.Add(NewLbi)
        Next

        BG.CREvents.Add(NewCr)
        lbxOriginChosen.Items.Clear()
        lbxOriginSelect.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))
        lbxRefreshEvents.Items.Add(NewCr.RefreshEventName)
        lbxDestination.SelectedIndex = -1
        Dim txtb As TextBox = numPopMoving.Children(1)
        txtb.Text = "0"
        txtEventName.Text = ""
        dtpStartDate.SelectedDate = Now()
        dtpEndDate.SelectedDate = Now()
    End Sub
#End Region

#Region "Field Management"
    Private Sub PopulateOptions()

        '// Populate business group names
        cboGroup.Items.Clear()
        Dim gq = From bgroup In BGC.BusinessGroups Select bgroup
        Dim ict As Byte = gq.Count, SortArray(ict - 1) As String, ct As Byte = 0

        For Each bgroup In gq
            SortArray(ct) = bgroup.BusinessGroupName
            ct += 1
        Next
        Array.Sort(SortArray)
        For Each s As String In SortArray
            cboGroup.Items.Add(New ComboBoxItem With {.Content = s})
        Next

        '// Populate work times
        cboWorkTimes.Items.Clear()
        Dim wtq = From worktime In BGC.WorkTimes Select worktime Order By worktime.WorkTime1
        For Each worktime In wtq
            Dim cbi As New ComboBoxItem
            cbi.Content = worktime.WorkTime1
            cbi.Tag = "C"
            cboWorkTimes.Items.Add(cbi)
        Next

        '// Populate workspace types - hard coded for now (7/15/18)
        cboWorkspace.Items.Clear()
        Dim wsq = From workloc In BGC.WorkLocations Select workloc Order By workloc.WorkLocation1
        For Each workloc In wsq
            Dim cbi As New ComboBoxItem
            cbi.Content = workloc.WorkLocation1
            cbi.Tag = "C"
            cboWorkspace.Items.Add(cbi)
        Next

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
        Dim loq = From bloc In SD.MasterBuildingLists Select bloc 'Order By bloc.BuildingName
        For Each bloc In loq
            Dim li As New ListBoxItem With {.IsTabStop = False}, li1 As New ListBoxItem With {.IsTabStop = False}, li2 As New ListBoxItem With {.IsTabStop = False}
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
            cboLeader.Items.Add(New ComboBoxItem With {.Content = bldr.LeaderName})
            Dim lbi As New ListBoxItem With {.Content = bldr.LeaderName, .Tag = "C"}
            lbxLeadersSelect.Items.Add(lbi)
            AddHandler lbi.MouseDoubleClick, AddressOf LeadTeamMove
        Next

        '// Populate relationship manager and frequent customers options - shared datasource
        cboRelManager.Items.Clear()
        lbxCustomerSelect.Items.Clear()
        Dim rmq = From brlm In BGC.FrequentCustomers Select brlm Order By brlm.CustomerName
        For Each brlm In rmq
            cboRelManager.Items.Add(New ComboBoxItem With {.Content = brlm.CustomerName})
            Dim lbi As New ListBoxItem With {.Content = brlm.CustomerName, .Tag = "C"}
            lbxCustomerSelect.Items.Add(lbi)
            AddHandler lbi.MouseDoubleClick, AddressOf CustomerMove
        Next

        '// Populate offsite location options
        lbxOffsiteLocsSelect.Items.Clear()
        Dim olq = From osl In BGC.OffsiteLocations Select osl Order By osl.OffsiteLocName
        For Each osl In olq
            Dim lbi As New ListBoxItem With {.Content = osl.OffsiteLocName, .Tag = "C", .IsTabStop = False}
            lbxOffsiteLocsSelect.Items.Add(lbi)
            AddHandler lbi.MouseDoubleClick, AddressOf OffsiteMove
        Next

        '// Populate notable event options
        lbxNotableSelect.Items.Clear()
        Dim neq = From nev In BGC.NotableEvents Select nev Order By nev.EventName
        For Each nev In neq
            Dim lbi As New ListBoxItem With {.Content = nev.EventName, .Tag = "C"}
            lbxNotableSelect.Items.Add(lbi)
            AddHandler lbi.MouseDoubleClick, AddressOf NotablesMove
        Next

        '// Populate top event type options
        lbxTopETypesSelect.Items.Clear()
        Dim teq = From tet In BGC.EventTypes Select tet Order By tet.EventType1
        For Each tet In teq
            Dim lbi As New ListBoxItem With {.Content = tet.EventType1, .Tag = "C"}
            lbxTopETypesSelect.Items.Add(lbi)
            AddHandler lbi.MouseDoubleClick, AddressOf TopTypeMove
        Next

        '// Populate top booked spaces options
        lbxTopSpacesSelect.Items.Clear()
        Dim tsq = From tsb In BGC.EventSpaces Select tsb Order By tsb.SpaceName
        For Each tsb In tsq
            Dim lbi As New ListBoxItem With {.Content = tsb.SpaceName, .Tag = "C"}
            lbxTopSpacesSelect.Items.Add(lbi)
            AddHandler lbi.MouseDoubleClick, AddressOf TopSpaceMove
        Next

        '// Populate eventions involvement options
        lbxInvolveSelect.Items.Clear()
        Dim tiq = From tii In BGC.Involvements Select tii Order By tii.Involvement1
        For Each tii In tiq
            Dim lbi As New ListBoxItem With {.Content = tii.Involvement1, .Tag = "C"}
            lbxInvolveSelect.Items.Add(lbi)
            AddHandler lbi.MouseDoubleClick, AddressOf InvolvementMove
        Next

        '// Populate embedded planner options
        lbxPlannersSelect.Items.Clear()
        Dim epq = From epl In BGC.Planners Select epl Order By epl.PlannerName
        For Each epl In epq
            Dim lbi As New ListBoxItem With {.Content = epl.PlannerName, .Tag = "C"}
            lbxPlannersSelect.Items.Add(lbi)
            AddHandler lbi.MouseDoubleClick, AddressOf PlannerMove
        Next

        '// Default date selectors to current date
        dtpStartDate.SelectedDate = Now().Date
        dtpStartDate.DisplayDateStart = Now().Date
        dtpStartDate.DisplayDateEnd = Now().Date.AddYears(2)
        dtpEndDate.SelectedDate = Now().Date
        dtpEndDate.DisplayDateStart = Now().Date
        dtpEndDate.DisplayDateEnd = Now().Date.AddYears(2)

    End Sub

    Private Sub GroupChosen(sender As Object, e As SelectionChangedEventArgs) Handles cboGroup.SelectionChanged
        If cboGroup.SelectedIndex = -1 Then Exit Sub
        LoadRefreshEvents()
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
                li.Content = li.Content & "--" & uni.NumVal & " headcount"
                Dim txtb As TextBox = numPopMoving.Children(1)
                Dim curval As Integer = FormatNumber(txtb.Text, 0) + uni.NumVal
                txtb.Text = curval
                uni.Close()
                lbxOriginChosen.Items.Add(li)
            Case "S"
                lbxOriginChosen.Items.Remove(li)
                li.Tag = "C"
                Dim ParseString() As String
                ParseString = Split(li.Content, "--", 2)
                li.Content = ParseString(0)
                lbxOriginSelect.Items.Add(li)
        End Select
        lbxOriginSelect.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))
        lbxOriginChosen.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))

    End Sub

    Private Sub StartDateChanged(sender As Object, e As SelectionChangedEventArgs) Handles dtpStartDate.SelectedDateChanged, dtpEndDate.SelectedDateChanged
        '// Validate end date is not before start date
        If dtpEndDate.SelectedDate < dtpStartDate.SelectedDate Then dtpEndDate.SelectedDate = dtpStartDate.SelectedDate
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
        If uni.StringVal.ToString = "" Then
            uni.Close()
            Exit Sub
        End If
        Try
            Dim check = BGC.BusinessGroups.Single(Function(p) p.BusinessGroupName = uni.StringVal.ToString)
            Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Small, AgnesMessageBox.MsgBoxLayout.FullText, AgnesMessageBox.MsgBoxType.OkOnly,
                                            12,,,, "Business group already exists")
            amsg.ShowDialog()
            amsg.Close()
        Catch ex As InvalidOperationException
            cboGroup.Items.Add(New ComboBoxItem With {.Content = uni.StringVal})
            Dim ict As Byte = cboGroup.Items.Count, SortArray(ict - 1) As String, ct As Byte = 0
            For Each i As ComboBoxItem In cboGroup.Items
                SortArray(ct) = i.Content
                ct += 1
            Next
            Array.Sort(SortArray)
            For ct = 0 To SortArray.Length - 1
                Dim cbi As ComboBoxItem = cboGroup.Items.Item(ct)
                cbi.Content = SortArray(ct)
                If SortArray(ct) = uni.StringVal Then cbi.IsSelected = True
            Next
            cboGroup.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))
        End Try
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
        If uni.StringVal.ToString = "" Then
            uni.Close()
            Exit Sub
        End If
        Try
            Dim check = BGC.Communications.Single(Function(p) p.CommType = uni.StringVal.ToString)
            Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Small, AgnesMessageBox.MsgBoxLayout.FullText, AgnesMessageBox.MsgBoxType.OkOnly,
                                            12,,,, "Comm type already exists")
            amsg.ShowDialog()
            amsg.Close()
        Catch ex As InvalidOperationException
            Dim comm As New Communication With {.CommType = uni.StringVal}
            BGC.Communications.Add(comm)
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
        If uni.StringVal.ToString = "" Then
            uni.Close()
            Exit Sub
        End If
        Try
            Dim check = BGC.GroupCultures.Single(Function(p) p.Culture = uni.StringVal.ToString)
            Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Small, AgnesMessageBox.MsgBoxLayout.FullText, AgnesMessageBox.MsgBoxType.OkOnly,
                                            12,,,, "Culture type already exists")
            amsg.ShowDialog()
            amsg.Close()
        Catch ex As InvalidOperationException
            Dim cult As New GroupCulture With {.Culture = uni.StringVal}
            BGC.GroupCultures.Add(cult)
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
        If uni.StringVal.ToString = "" Then
            uni.Close()
            Exit Sub
        End If
        Try
            Dim check = BGC.Leaders.Single(Function(p) p.LeaderName = uni.StringVal.ToString)
            Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Small, AgnesMessageBox.MsgBoxLayout.FullText, AgnesMessageBox.MsgBoxType.OkOnly,
                                            12,,,, "Team member already exists")
            amsg.ShowDialog()
            amsg.Close()
        Catch ex As InvalidOperationException
            Dim leader As New Leader With {.LeaderName = uni.StringVal}
            BGC.Leaders.Add(leader)
            Dim li As New ListBoxItem With {.Content = uni.StringVal, .Tag = "S"}
            AddHandler li.MouseDoubleClick, AddressOf LeadTeamMove
            lbxLeadersChosen.Items.Add(li)
            lbxLeadersChosen.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))
            '// Also add leader to Org Leader combobox option
            cboLeader.Items.Add(New ComboBoxItem With {.Content = uni.StringVal})
            Dim ict As Byte = cboLeader.Items.Count, SortArray(ict - 1) As String, ct As Byte = 0
            For Each i As ComboBoxItem In cboLeader.Items
                SortArray(ct) = i.Content
                ct += 1
            Next
            Array.Sort(SortArray)
            For ct = 0 To SortArray.Length - 1
                Dim cbi As ComboBoxItem = cboLeader.Items.Item(ct)
                cbi.Content = SortArray(ct)
            Next
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
        If uni.StringVal.ToString = "" Then
            uni.Close()
            Exit Sub
        End If
        Try
            Dim check = BGC.FrequentCustomers.Single(Function(p) p.CustomerName = uni.StringVal.ToString)
            Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Small, AgnesMessageBox.MsgBoxLayout.FullText, AgnesMessageBox.MsgBoxType.OkOnly,
                                            12,,,, "Customer already exists")
            amsg.ShowDialog()
            amsg.Close()
        Catch ex As InvalidOperationException
            Dim customer As New FrequentCustomer With {.CustomerName = uni.StringVal}
            BGC.FrequentCustomers.Add(customer)
            Dim li As New ListBoxItem With {.Content = uni.StringVal, .Tag = "S"}
            AddHandler li.MouseDoubleClick, AddressOf CustomerMove
            lbxCustomerChosen.Items.Add(li)
            lbxCustomerChosen.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))

            '// Also add customer to relationship manager options
            cboRelManager.Items.Add(New ComboBoxItem With {.Content = uni.StringVal})
            Dim ict As Byte = cboRelManager.Items.Count, SortArray(ict - 1) As String, ct As Byte = 0
            For Each i As ComboBoxItem In cboRelManager.Items
                SortArray(ct) = i.Content
                ct += 1
            Next
            Array.Sort(SortArray)
            For ct = 0 To SortArray.Length - 1
                Dim cbi As ComboBoxItem = cboRelManager.Items.Item(ct)
                cbi.Content = SortArray(ct)
            Next
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
        If uni.StringVal.ToString = "" Then
            uni.Close()
            Exit Sub
        End If
        Try
            Dim check = BGC.OffsiteLocations.Single(Function(p) p.OffsiteLocName = uni.StringVal.ToString)
            Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Small, AgnesMessageBox.MsgBoxLayout.FullText, AgnesMessageBox.MsgBoxType.OkOnly,
                                            12,,,, "Locations already exists")
            amsg.ShowDialog()
            amsg.Close()
        Catch ex As InvalidOperationException
            Dim Offsite As New OffsiteLocation With {.OffsiteLocName = uni.StringVal}
            BGC.OffsiteLocations.Add(Offsite)
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
        If uni.StringVal.ToString = "" Then
            uni.Close()
            Exit Sub
        End If
        Try
            Dim check = BGC.EventTypes.Single(Function(p) p.EventType1 = uni.StringVal.ToString)
            Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Small, AgnesMessageBox.MsgBoxLayout.FullText, AgnesMessageBox.MsgBoxType.OkOnly,
                                            12,,,, "Event type already exists")
            amsg.ShowDialog()
            amsg.Close()
        Catch ex As InvalidOperationException
            Dim EventType As New EventType With {.EventType1 = uni.StringVal}
            BGC.EventTypes.Add(EventType)
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
        If uni.StringVal.ToString = "" Then
            uni.Close()
            Exit Sub
        End If
        Try
            Dim check = BGC.EventSpaces.Single(Function(p) p.SpaceName = uni.StringVal.ToString)
            Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Small, AgnesMessageBox.MsgBoxLayout.FullText, AgnesMessageBox.MsgBoxType.OkOnly,
                                            12,,,, "Event space already exists")
            amsg.ShowDialog()
            amsg.Close()
        Catch ex As InvalidOperationException
            Dim EventSpc As New EventSpace With {.SpaceName = uni.StringVal}
            BGC.EventSpaces.Add(EventSpc)
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
        If uni.StringVal.ToString = "" Then
            uni.Close()
            Exit Sub
        End If
        Try
            Dim check = BGC.Involvements.Single(Function(p) p.Involvement1 = uni.StringVal.ToString)
            Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Small, AgnesMessageBox.MsgBoxLayout.FullText, AgnesMessageBox.MsgBoxType.OkOnly,
                                            12,,,, "Involvement type already exists")
            amsg.ShowDialog()
            amsg.Close()
        Catch ex As InvalidOperationException
            Dim Involve As New Involvement With {.Involvement1 = uni.StringVal}
            BGC.Involvements.Add(Involve)
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
        If uni.StringVal.ToString = "" Then
            uni.Close()
            Exit Sub
        End If
        Try
            Dim check = BGC.NotableEvents.Single(Function(p) p.EventName = uni.StringVal.ToString)
            Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Small, AgnesMessageBox.MsgBoxLayout.FullText, AgnesMessageBox.MsgBoxType.OkOnly,
                                            12,,,, "Event already exists")
            amsg.ShowDialog()
            amsg.Close()
        Catch ex As InvalidOperationException
            Dim Notable As New NotableEvent With {.EventName = uni.StringVal}
            BGC.NotableEvents.Add(Notable)
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
        If uni.StringVal.ToString = "" Then
            uni.Close()
            Exit Sub
        End If
        Try
            Dim check = BGC.Planners.Single(Function(p) p.PlannerName = uni.StringVal.ToString)
            Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Small, AgnesMessageBox.MsgBoxLayout.FullText, AgnesMessageBox.MsgBoxType.OkOnly,
                                            12,,,, "Planner already exists")
            amsg.ShowDialog()
            amsg.Close()
        Catch ex As InvalidOperationException
            Dim Plannr As New Planner With {.PlannerName = uni.StringVal}
            BGC.Planners.Add(Plannr)
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
                        Dim tb As TextBox = numHeadcount.Children(1)
                        c = FormatNumber(tb.Text, 2)
                    Catch ex As Exception
                        invalid.Add("Headcount must be a number - enter 0 if currently unknown.")
                    End Try

                Case 1  '// People page
                    If cboLeader.SelectedIndex = -1 Then invalid.Add("An organizational leader must be selected.")
                    If cboRelManager.SelectedIndex = -1 Then invalid.Add("A relationship manager must be selected.")
                Case 2  '// Financials page
                    Try
                        Dim tb As TextBox = curRevenue.Children(1)
                        c = FormatNumber(tb.Text, 2)
                    Catch ex As Exception
                        invalid.Add("Offsite spend must be a number - enter 0 if currently unknown.")
                    End Try

                    Try
                        Dim tb As TextBox = curOffsite.Children(1)
                        c = FormatNumber(tb.Text, 2)
                    Catch ex As Exception
                        invalid.Add("Offsite spend must be a number - enter 0 if currently unknown.")
                    End Try

                    Try
                        Dim tb As TextBox = numEventCount.Children(1)
                        i = FormatNumber(tb.Text, 0)
                    Catch ex As Exception
                        invalid.Add("Event count must be a number - enter 0 if currently unknown.")
                    End Try

                    Try
                        Dim tb As TextBox = num500Events.Children(1)
                        i = FormatNumber(tb.Text, 0)
                    Catch ex As Exception
                        invalid.Add("500+ event count must be a number - enter 0 if currently unknown.")
                    End Try

                    Try
                        Dim tb As TextBox = numCatered.Children(1)
                        i = FormatNumber(tb.Text, 0)
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
                Dim amsg1 = New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Medium, AgnesMessageBox.MsgBoxLayout.FullText,
                                                AgnesMessageBox.MsgBoxType.OkOnly, 14,,,, errorstring)
                amsg1.ShowDialog()
                amsg1.Close()
                Return False
            End If
        End If
        Return True
    End Function

    Private Function FetchBuildingID(bn) As Integer
        Dim retval As Integer = 0
        Dim loq = From bloc In SD.MasterBuildingLists Select bloc Where bloc.BuildingName Is bn
        For Each bloc In loq
            retval = bloc.PID
        Next
        Return retval
    End Function
#End Region

End Class
