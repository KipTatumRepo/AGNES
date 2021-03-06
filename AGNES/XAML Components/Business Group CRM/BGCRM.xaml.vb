﻿Imports System.ComponentModel
Imports System.Linq
Public Class BGCRM
#Region "Properties"
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
    Dim AdminSelect As Boolean
#End Region

#Region "Constructor"
    Public Sub New()
        InitializeComponent()
        BG = New objBusinessGroup
        BGC = New BGCRMEntity
        SD = New BIEntities
        PopulateOptions()
        curRevenue = New CurrencyBox(189, True, AgnesBaseInput.FontSz.Medium) With {.Margin = New Thickness(269, 28, 0, 0)}
        curOffsite = New CurrencyBox(189, True, AgnesBaseInput.FontSz.Medium) With {.Margin = New Thickness(269, 88, 0, 0)}
        numEventCount = New NumberBox(189, True, False, True, False, True, AgnesBaseInput.FontSz.Medium, 0, "0") With {.Margin = New Thickness(269, 156, 0, 0)}
        num500Events = New NumberBox(189, True, False, True, False, True, AgnesBaseInput.FontSz.Medium, 0, "0") With {.Margin = New Thickness(807, 28, 0, 0)}
        numCatered = New NumberBox(189, True, False, True, False, True, AgnesBaseInput.FontSz.Medium, 0, "0") With {.Margin = New Thickness(807, 88, 0, 0)}
        numHeadcount = New NumberBox(108, True, False, True, False, True, AgnesBaseInput.FontSz.Medium, 0, "0") With {.Margin = New Thickness(886, 156, 0, 0)}
        numPopMoving = New NumberBox(126, True, False, True, False, True, AgnesBaseInput.FontSz.Medium, 0, "0") With {.Margin = New Thickness(798, 162, 0, 0), .IsEnabled = False}
        Dim txtbx As TextBox = numPopMoving.Children(1)

        Dim tb As TextBox
        tb = numHeadcount.Children(1)
        tb.IsTabStop = True
        tb.TabIndex = 4

        tb = numPopMoving.Children(1)
        tb.IsTabStop = False

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
#End Region

#Region "Private Methods"
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

#Region "Navigation Methods"
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

#Region "Data Handling Methods"
    Private Sub SavePageToBGObj(p)
        Dim si As ListBoxItem
        Select Case p
            Case 0          '====== GROUP PAGE
                With BG
                    .Headcount = FormatNumber(numHeadcount.Amount, 0)
                    .OrgName = cboGroup.Text
                    .Overview = txtOverview.Text
                    .OnsiteRemote = cboWorkspace.SelectedIndex
                End With

                '// Populate work time
                Dim wt As String = cboWorkTimes.Text
                Dim qwt = From c In BGC.WorkTimes
                          Where c.Time = wt
                          Select c
                For Each c In qwt
                    BG.WorkTimes = FormatNumber(c.PID, 0)
                Next

                '// Populate work locations/environment
                Dim wl As String = cboWorkspace.Text
                Dim qwl = From c In BGC.WorkLocations
                          Where c.Location = wl
                          Select c
                For Each c In qwl
                    BG.OnsiteRemote = FormatNumber(c.PID, 0)
                Next

                '// Populate chosen communications into array
                BG.Communications.Clear()
                For Each si In lbxCommsChosen.Items
                    Dim sc As String = si.Content
                    Dim query = From comms In BGC.Communications
                                Where comms.Communication.ToString = sc
                                Select comms
                    For Each comm In query
                        BG.Communications.Add(FormatNumber(comm.PID, 0))
                    Next
                Next

                '// Populate chosen culture into array
                BG.Culture.Clear()
                For Each si In lbxCultureChosen.Items
                    Dim sc As String = si.Content
                    Dim q = From c In BGC.Cultures
                            Where c.Culture = sc
                            Select c
                    For Each c In q
                        BG.Culture.Add(FormatNumber(c.PID, 0))
                    Next
                Next

                '// Populate chosen locations into array
                BG.Locations.Clear()

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
                         Where c.Leader = sl
                         Select c
                For Each c In ql
                    BG.OrgLeader = FormatNumber(c.PID, 0)
                Next

                '// Populate relationship manager
                Dim orm As String = cboRelManager.Text
                Dim qr = From c In BGC.Customers
                         Where c.Customer = orm
                         Select c
                For Each c In qr
                    BG.RelationshipMgr = FormatNumber(c.PID, 0)
                Next

                '// Populate chosen leaders into array
                BG.Leadership.Clear()
                For Each si In lbxLeadersChosen.Items
                    Dim slt As String = si.Content
                    Dim q = From c In BGC.Leaders
                            Where c.Leader = slt
                            Select c
                    For Each c In q
                        BG.Leadership.Add(FormatNumber(c.PID, 0))
                    Next
                Next

                '// Populate chosen customers into array
                BG.FrequentCustomers.Clear()
                For Each si In lbxCustomerChosen.Items
                    Dim sfc As String = si.Content
                    Dim q = From c In BGC.Customers
                            Where c.Customer = sfc
                            Select c
                    For Each c In q
                        BG.FrequentCustomers.Add(FormatNumber(c.PID, 0))
                    Next
                Next

            Case 2          '======FINANCIAL PAGE

                With BG
                    .TotalRevenue = FormatNumber(curRevenue.SetAmount, 2)
                    .OffSiteSpend = FormatNumber(curOffsite.SetAmount, 2)
                    .TotalEvents = FormatNumber(numEventCount.Amount, numEventCount.NumberOfDecimals)
                    .Events500 = FormatNumber(num500Events.Amount, num500Events.NumberOfDecimals)
                    .CateredEvents = FormatNumber(numCatered.Amount, numCatered.NumberOfDecimals)
                End With

                '// Populate top offsite locations into array
                BG.TopOffsiteLocations.Clear()
                For Each si In lbxOffsiteLocsChosen.Items
                    Dim slt As String = si.Content
                    Dim q = From c In BGC.OffsiteLocations
                            Where c.Location = slt
                            Select c
                    For Each c In q
                        BG.TopOffsiteLocations.Add(FormatNumber(c.PID, 0))
                    Next
                Next

            Case 3          '======EVENTS PAGE
                '// Populate notable events into array
                BG.NotableEvents.Clear()
                For Each si In lbxNotableChosen.Items
                    Dim slt As String = si.Content
                    Dim q = From c In BGC.NotableEvents
                            Where c.Event = slt
                            Select c
                    For Each c In q
                        BG.NotableEvents.Add(FormatNumber(c.PID, 0))
                    Next
                Next

                '// Populate top event types into array
                BG.TopEventTypes.Clear()
                For Each si In lbxTopETypesChosen.Items
                    Dim slt As String = si.Content
                    Dim q = From c In BGC.Events
                            Where c.Event = slt
                            Select c
                    For Each c In q
                        BG.TopEventTypes.Add(FormatNumber(c.PID, 0))
                    Next
                Next

                '// Populate top event spaces into array
                BG.TopBookedSpaces.Clear()
                For Each si In lbxTopSpacesChosen.Items
                    Dim slt As String = si.Content
                    Dim q = From c In BGC.Spaces
                            Where c.Space = slt
                            Select c
                    For Each c In q
                        BG.TopBookedSpaces.Add(FormatNumber(c.PID, 0))
                    Next
                Next

                '// Populate top eventions involvements into array
                BG.EventionsInvolvement.Clear()
                For Each si In lbxInvolveChosen.Items
                    Dim slt As String = si.Content
                    Dim q = From c In BGC.Involvements
                            Where c.Involvement = slt
                            Select c
                    For Each c In q
                        BG.EventionsInvolvement.Add(FormatNumber(c.PID, 0))
                    Next
                Next

                '// Populate embedded planners into array
                BG.EmbeddedPlanners.Clear()
                For Each si In lbxPlannersChosen.Items
                    Dim slt As String = si.Content
                    Dim q = From c In BGC.Planners
                            Where c.Planner = slt
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
            btnSaveFinish.Content = "Saved"
            btnSaveFinish.IsEnabled = False
        Else
            Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Medium, AgnesMessageBox.MsgBoxLayout.FullText, AgnesMessageBox.MsgBoxType.OkOnly, 12,, "Save failed!",, "Unable to save.  Please checks your work and try again.")
            amsg.ShowDialog()
            amsg.Close()
        End If
    End Sub

    Private Sub LoadExisting()
        Dim tempbizgroup = cboGroup.SelectedValue
        PopulateOptions()
        AdminSelect = True
        cboGroup.SelectedValue = tempbizgroup
        AdminSelect = False
        BG.Load(cboGroup.SelectedValue)
        LoadToUI()
    End Sub

    Private Sub LoadToUI()
        Dim lbc As Integer, WorkTime As String = "", workspace As String = ""
        txtOverview.Text = BG.Overview
        cboWorkspace.SelectedIndex = BG.OnsiteRemote
        numHeadcount.SetAmount = FormatNumber(BG.Headcount, 0)
        Dim GetWorkTime = From wt In BGC.WorkTimes
                          Where wt.PID = BG.WorkTimes
                          Select wt
        For Each c In GetWorkTime
            WorkTime = Trim(c.Time)
        Next
        For ct As Byte = 0 To cboWorkTimes.Items.Count - 1
            Dim twp As ComboBoxItem = cboWorkTimes.Items(ct)
            If twp.Content = WorkTime Then cboWorkTimes.SelectedIndex = ct
        Next

        Dim GetWorkSpace = From ws In BGC.WorkLocations
                           Where ws.PID = BG.OnsiteRemote
                           Select ws
        For Each c In GetWorkSpace
            workspace = Trim(c.Location)
        Next
        For ct As Byte = 0 To cboWorkspace.Items.Count - 1
            Dim twp As ComboBoxItem = cboWorkspace.Items(ct)
            If twp.Content = workspace Then cboWorkspace.SelectedIndex = ct
        Next

        For Each comm As Long In BG.Communications
            Dim GetCommName = From comms In BGC.Communications
                              Where comms.PID = comm
                              Select comms
            For Each c In GetCommName
                Dim lbi As New ListBoxItem With {.Content = c.Communication, .Tag = "S"}
                AddHandler lbi.MouseDoubleClick, AddressOf CommItemMove
                lbxCommsChosen.Items.Add(lbi)
                For lbc = (lbxCommSelect.Items.Count - 1) To 0 Step -1
                    If lbxCommSelect.Items(lbc).Content = lbi.Content Then
                        lbxCommSelect.Items.RemoveAt(lbc)
                    End If
                Next
            Next
        Next
        For Each cult As Long In BG.Culture
            Dim GetCultName = From cults In BGC.Cultures
                              Where cults.PID = cult
                              Select cults
            For Each c In GetCultName
                Dim lbi As New ListBoxItem With {.Content = c.Culture, .Tag = "S"}
                AddHandler lbi.MouseDoubleClick, AddressOf CultureItemMove
                lbxCultureChosen.Items.Add(lbi)
                For lbc = (lbxCultureSelect.Items.Count - 1) To 0 Step -1
                    If lbxCultureSelect.Items(lbc).Content = lbi.Content Then
                        lbxCultureSelect.Items.RemoveAt(lbc)
                    End If
                Next
            Next
        Next
        For Each loc As Long In BG.Locations
            Dim GetLocName = From locs In SD.MasterBuildingLists
                             Where locs.PID = loc
                             Select locs
            For Each c In GetLocName
                Dim lbi As New ListBoxItem With {.Content = c.BuildingName, .Tag = "S"}
                AddHandler lbi.MouseDoubleClick, AddressOf LocationItemMove
                lbxLocationsChosen.Items.Add(lbi)
                For lbc = (lbxLocationsSelect.Items.Count - 1) To 0 Step -1
                    If lbxLocationsSelect.Items(lbc).Content = lbi.Content Then
                        lbxLocationsSelect.Items.RemoveAt(lbc)
                    End If
                Next
            Next
        Next

        Dim GetOrgLdr = From orgl In BGC.Leaders
                        Where orgl.PID = BG.OrgLeader
                        Select orgl
        For Each c In GetOrgLdr
            cboLeader.SelectedValue = c.Leader
        Next

        Dim GetRelMgr = From rlm In BGC.Customers
                        Where rlm.PID = BG.RelationshipMgr
                        Select rlm
        For Each c In GetRelMgr
            cboRelManager.SelectedValue = c.Customer
        Next

        For Each ldr As Long In BG.Leadership
            Dim GetLdrName = From lds In BGC.Leaders
                             Where lds.PID = ldr
                             Select lds
            For Each c In GetLdrName
                Dim lbi As New ListBoxItem With {.Content = c.Leader, .Tag = "S"}
                AddHandler lbi.MouseDoubleClick, AddressOf LeadTeamMove
                lbxLeadersChosen.Items.Add(lbi)
                For lbc = (lbxLeadersSelect.Items.Count - 1) To 0 Step -1
                    If lbxLeadersSelect.Items(lbc).Content = lbi.Content Then
                        lbxLeadersSelect.Items.RemoveAt(lbc)
                    End If
                Next
            Next
        Next

        For Each cust As Long In BG.FrequentCustomers
            Dim GetCustName = From cst In BGC.Customers
                              Where cst.PID = cust
                              Select cst
            For Each c In GetCustName
                Dim lbi As New ListBoxItem With {.Content = c.Customer, .Tag = "S"}
                AddHandler lbi.MouseDoubleClick, AddressOf CustomerMove
                lbxCustomerChosen.Items.Add(lbi)
                For lbc = (lbxCustomerSelect.Items.Count - 1) To 0 Step -1
                    If lbxCustomerSelect.Items(lbc).Content = lbi.Content Then
                        lbxCustomerSelect.Items.RemoveAt(lbc)
                    End If
                Next
            Next
        Next


        curRevenue.SetAmount = FormatNumber(BG.TotalRevenue, 2)
        curOffsite.SetAmount = FormatNumber(BG.OffSiteSpend, 2)
        curRevenue.SetAmount = FormatNumber(BG.TotalRevenue, 2)
        numEventCount.SetAmount = FormatNumber(BG.TotalEvents, 0)
        num500Events.SetAmount = FormatNumber(BG.Events500, 0)
        numCatered.SetAmount = FormatNumber(BG.CateredEvents, 0)

        For Each osl As Long In BG.TopOffsiteLocations
            Dim GetOSLName = From osn In BGC.OffsiteLocations
                             Where osn.PID = osl
                             Select osn
            For Each c In GetOSLName
                Dim lbi As New ListBoxItem With {.Content = c.Location, .Tag = "S"}
                AddHandler lbi.MouseDoubleClick, AddressOf OffsiteMove
                lbxOffsiteLocsChosen.Items.Add(lbi)
                For lbc = (lbxOffsiteLocsSelect.Items.Count - 1) To 0 Step -1
                    If lbxOffsiteLocsSelect.Items(lbc).Content = lbi.Content Then
                        lbxOffsiteLocsSelect.Items.RemoveAt(lbc)
                    End If
                Next
            Next
        Next

        For Each noe As Long In BG.NotableEvents
            Dim GetEventName = From ntb In BGC.NotableEvents
                               Where ntb.PID = noe
                               Select ntb
            For Each c In GetEventName
                Dim lbi As New ListBoxItem With {.Content = c.Event, .Tag = "S"}
                AddHandler lbi.MouseDoubleClick, AddressOf NotablesMove
                lbxNotableChosen.Items.Add(lbi)
                For lbc = (lbxNotableSelect.Items.Count - 1) To 0 Step -1
                    If lbxNotableSelect.Items(lbc).Content = lbi.Content Then
                        lbxNotableSelect.Items.RemoveAt(lbc)
                    End If
                Next
            Next
        Next

        For Each tet As Long In BG.TopEventTypes
            Dim GetEventType = From et In BGC.Events
                               Where et.PID = tet
                               Select et
            For Each c In GetEventType
                Dim lbi As New ListBoxItem With {.Content = c.Event, .Tag = "S"}
                AddHandler lbi.MouseDoubleClick, AddressOf TopTypeMove
                lbxTopETypesChosen.Items.Add(lbi)
                For lbc = (lbxTopETypesSelect.Items.Count - 1) To 0 Step -1
                    If lbxTopETypesSelect.Items(lbc).Content = lbi.Content Then
                        lbxTopETypesSelect.Items.RemoveAt(lbc)
                    End If
                Next
            Next
        Next

        For Each tbs As Long In BG.TopBookedSpaces
            Dim GetBookedSpace = From gbs In BGC.Spaces
                                 Where gbs.PID = tbs
                                 Select gbs
            For Each c In GetBookedSpace
                Dim lbi As New ListBoxItem With {.Content = c.Space, .Tag = "S"}
                AddHandler lbi.MouseDoubleClick, AddressOf TopSpaceMove
                lbxTopSpacesChosen.Items.Add(lbi)
                For lbc = (lbxTopSpacesSelect.Items.Count - 1) To 0 Step -1
                    If lbxTopSpacesSelect.Items(lbc).Content = lbi.Content Then
                        lbxTopSpacesSelect.Items.RemoveAt(lbc)
                    End If
                Next
            Next
        Next

        For Each tei As Long In BG.EventionsInvolvement
            Dim GetInvolvement = From gei In BGC.Involvements
                                 Where gei.PID = tei
                                 Select gei
            For Each c In GetInvolvement
                Dim lbi As New ListBoxItem With {.Content = c.Involvement, .Tag = "S"}
                AddHandler lbi.MouseDoubleClick, AddressOf InvolvementMove
                lbxInvolveChosen.Items.Add(lbi)
                For lbc = (lbxInvolveSelect.Items.Count - 1) To 0 Step -1
                    If lbxInvolveSelect.Items(lbc).Content = lbi.Content Then
                        lbxInvolveSelect.Items.RemoveAt(lbc)
                    End If
                Next
            Next
        Next

        For Each pln As Long In BG.EmbeddedPlanners
            Dim GetPlanner = From plnr In BGC.Planners
                             Where plnr.PID = pln
                             Select plnr
            For Each c In GetPlanner
                Dim lbi As New ListBoxItem With {.Content = c.Planner, .Tag = "S"}
                AddHandler lbi.MouseDoubleClick, AddressOf PlannerMove
                lbxPlannersChosen.Items.Add(lbi)
                For lbc = (lbxPlannersSelect.Items.Count - 1) To 0 Step -1
                    If lbxPlannersSelect.Items(lbc).Content = lbi.Content Then
                        lbxPlannersSelect.Items.RemoveAt(lbc)
                    End If
                Next
            Next
        Next

        lbxRefreshEvents.Items.Clear()
        For Each cr As RefreshEvents In BGC.RefreshEvents
            Dim lbi As New ListBoxItem With {.Content = cr.Event}
            AddHandler lbi.MouseDoubleClick, AddressOf PopulateRefreshEvent
            lbxRefreshEvents.Items.Add(lbi)
        Next

    End Sub

    Private Sub SaveRefreshEvent(sender As Object, e As EventArgs) Handles btnSaveRefreshEvent.Click
        'TODO: ADD ADDITIONAL VALIDATION PRIOR TO SAVING FOR ALL FIELDS/LISTS!  
        For Each cr As RefreshEvent In BG.CREvents
            If cr.EventName = txtEventName.Text Then
                Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Small, AgnesMessageBox.MsgBoxLayout.FullText, AgnesMessageBox.MsgBoxType.OkOnly, 12,, "Save failed.",, "An event with this name already exists.")
                amsg.ShowDialog()
                amsg.Close()
                Exit Sub
            End If
        Next
        Dim NewCr As New RefreshEvent, tb As TextBox = numPopMoving.Children(1)
        With NewCr
            .EventName = txtEventName.Text
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
                .BuildingId = BG.FetchBuildingID(BldgName)
            End With
            NewCr.BuildingsMoving.Add(newBldg)
            Dim NewLbi As New ListBoxItem With {.Content = BldgName}
            lbxOriginSelect.Items.Add(NewLbi)
        Next

        BG.CREvents.Add(NewCr)
        lbxOriginChosen.Items.Clear()
        lbxOriginSelect.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))
        Dim mainlbi As New ListBoxItem With {.Content = NewCr.EventName}
        lbxRefreshEvents.Items.Add(mainlbi)
        lbxDestination.SelectedIndex = -1
        numPopMoving.SetAmount = 0
        txtEventName.Text = ""
        dtpStartDate.SelectedDate = Now()
        dtpEndDate.SelectedDate = Now()
    End Sub

#End Region

#Region "Field Management Methods"
    Private Sub PopulateOptions()

        '// Populate business group names
        cboGroup.Items.Clear()
        Dim gq = From bgroup In BGC.Groups Select bgroup
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
        Dim wtq = From worktime In BGC.WorkTimes Select worktime Order By worktime.Time
        For Each worktime In wtq
            Dim cbi As New ComboBoxItem
            cbi.Content = worktime.Time
            cbi.Tag = "C"
            cboWorkTimes.Items.Add(cbi)
        Next

        '// Populate workspace types - hard coded for now (7/15/18)
        cboWorkspace.Items.Clear()
        Dim wsq = From workloc In BGC.WorkLocations Select workloc Order By workloc.Location
        For Each workloc In wsq
            Dim cbi As New ComboBoxItem
            cbi.Content = workloc.Location
            cbi.Tag = "C"
            cboWorkspace.Items.Add(cbi)
        Next

        '// Populate communication options
        lbxCommSelect.Items.Clear()
        lbxCommsChosen.Items.Clear()
        Dim cq = From bcomm In BGC.Communications Select bcomm Order By bcomm.Communication
        For Each bcomm In cq
            Dim li As New ListBoxItem
            li.Content = bcomm.Communication
            li.Tag = "C"
            AddHandler li.MouseDoubleClick, AddressOf CommItemMove
            lbxCommSelect.Items.Add(li)
        Next

        '// Populate culture options
        lbxCultureSelect.Items.Clear()
        lbxCultureChosen.Items.Clear()
        Dim cuq = From bcult In BGC.Cultures Select bcult Order By bcult.Culture
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
            'li2.Content = bloc.BuildingName
            'li2.Tag = "C"
            lbxDestination.Items.Add(bloc.BuildingName)
            'li2.Width = lbxDestination.Width - 40
        Next

        '// Populate leader and leadership team options - shared datasource
        cboLeader.Items.Clear()
        lbxLeadersSelect.Items.Clear()
        lbxLeadersChosen.Items.Clear()
        Dim lq = From bldr In BGC.Leaders Select bldr Order By bldr.Leader
        For Each bldr In lq
            cboLeader.Items.Add(New ComboBoxItem With {.Content = bldr.Leader})
            Dim lbi As New ListBoxItem With {.Content = bldr.Leader, .Tag = "C"}
            lbxLeadersSelect.Items.Add(lbi)
            AddHandler lbi.MouseDoubleClick, AddressOf LeadTeamMove
        Next

        '// Populate relationship manager and frequent customers options - shared datasource
        cboRelManager.Items.Clear()
        lbxCustomerSelect.Items.Clear()
        lbxCustomerChosen.Items.Clear()
        Dim rmq = From brlm In BGC.Customers Select brlm Order By brlm.Customer
        For Each brlm In rmq
            cboRelManager.Items.Add(New ComboBoxItem With {.Content = brlm.Customer})
            Dim lbi As New ListBoxItem With {.Content = brlm.Customer, .Tag = "C"}
            lbxCustomerSelect.Items.Add(lbi)
            AddHandler lbi.MouseDoubleClick, AddressOf CustomerMove
        Next

        '// Populate offsite location options
        lbxOffsiteLocsSelect.Items.Clear()
        lbxOffsiteLocsChosen.Items.Clear()
        Dim olq = From osl In BGC.OffsiteLocations Select osl Order By osl.Location
        For Each osl In olq
            Dim lbi As New ListBoxItem With {.Content = osl.Location, .Tag = "C", .IsTabStop = False}
            lbxOffsiteLocsSelect.Items.Add(lbi)
            AddHandler lbi.MouseDoubleClick, AddressOf OffsiteMove
        Next

        '// Populate notable event options
        lbxNotableSelect.Items.Clear()
        lbxNotableChosen.Items.Clear()
        Dim neq = From nev In BGC.NotableEvents Select nev Order By nev.Event
        For Each nev In neq
            Dim lbi As New ListBoxItem With {.Content = nev.Event, .Tag = "C"}
            lbxNotableSelect.Items.Add(lbi)
            AddHandler lbi.MouseDoubleClick, AddressOf NotablesMove
        Next

        '// Populate top event type options
        lbxTopETypesSelect.Items.Clear()
        lbxTopETypesChosen.Items.Clear()
        Dim teq = From tet In BGC.Events Select tet Order By tet.Event
        For Each tet In teq
            Dim lbi As New ListBoxItem With {.Content = tet.Event, .Tag = "C"}
            lbxTopETypesSelect.Items.Add(lbi)
            AddHandler lbi.MouseDoubleClick, AddressOf TopTypeMove
        Next

        '// Populate top booked spaces options
        lbxTopSpacesSelect.Items.Clear()
        lbxTopSpacesChosen.Items.Clear()
        Dim tsq = From tsb In BGC.Spaces Select tsb Order By tsb.Space
        For Each tsb In tsq
            Dim lbi As New ListBoxItem With {.Content = tsb.Space, .Tag = "C"}
            lbxTopSpacesSelect.Items.Add(lbi)
            AddHandler lbi.MouseDoubleClick, AddressOf TopSpaceMove
        Next

        '// Populate eventions involvement options
        lbxInvolveSelect.Items.Clear()
        lbxInvolveChosen.Items.Clear()
        Dim tiq = From tii In BGC.Involvements Select tii Order By tii.Involvement
        For Each tii In tiq
            Dim lbi As New ListBoxItem With {.Content = tii.Involvement, .Tag = "C"}
            lbxInvolveSelect.Items.Add(lbi)
            AddHandler lbi.MouseDoubleClick, AddressOf InvolvementMove
        Next

        '// Populate embedded planner options
        lbxPlannersSelect.Items.Clear()
        lbxPlannersChosen.Items.Clear()
        Dim epq = From epl In BGC.Planners Select epl Order By epl.Planner
        For Each epl In epq
            Dim lbi As New ListBoxItem With {.Content = epl.Planner, .Tag = "C"}
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
        If (cboGroup.SelectedIndex = -1) Or (AdminSelect = True) Then Exit Sub
        LoadExisting()
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
                Dim curval As Integer = numPopMoving.Amount + uni.NumVal
                numPopMoving.SetAmount = curval
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

    Private Sub PopulateRefreshEvent(sender, EventArgs) Handles lbxRefreshEvents.SelectionChanged
        'TODO: REFRESH SELECTABLE LISTBOX AND CLEAR CHOSEN LISTBOX PRIOR TO POPULATING
        Dim RefEvent As String, EventID As Long, lbi As ListBoxItem = lbxRefreshEvents.SelectedItem
        RefEvent = lbi.Content
        Dim GetEventDetails = From evnt In BGC.RefreshEvents
                              Where evnt.Event = RefEvent
                              Select evnt
        For Each c In GetEventDetails
            EventID = c.EventID
            txtEventName.Text = RefEvent
            dtpStartDate.DisplayDate = c.MoveStartDate
            numPopMoving.SetAmount = FormatNumber(c.PopulationMoving, 0)
            dtpEndDate.DisplayDate = c.MoveEndDate
            For ct = 0 To lbxDestination.Items.Count - 1
                If lbxDestination.Items(ct).ToString = c.Destination Then lbxDestination.SelectedIndex = ct
            Next
        Next

        Dim GetEventBuildings = From ebldgs In BGC.RefreshEventOrigins
                                Where ebldgs.EventId = EventID
                                Select ebldgs

        For Each c In GetEventBuildings
            Dim bldgnm As String = BG.FetchBuildingName(FormatNumber(c.BuildingId, 0))
            Dim lb As ListBoxItem
            For Each lb In lbxOriginSelect.Items
                If lb.Content = bldgnm Then
                    lbxOriginSelect.Items.Remove(lb)
                    lb.Content = lb.Content & "--" & c.PopulationMoving & " headcount"
                    lb.Tag = "S"
                    lbxOriginChosen.Items.Add(lb)
                    Exit For
                End If
            Next
        Next
    End Sub

    Private Sub StartDateChanged(sender As Object, e As SelectionChangedEventArgs) Handles dtpStartDate.SelectedDateChanged, dtpEndDate.SelectedDateChanged
        '// Validate end date is not before start date
        If dtpEndDate.SelectedDate < dtpStartDate.SelectedDate Then dtpEndDate.SelectedDate = dtpStartDate.SelectedDate
    End Sub

#End Region

#Region "Context Menu Methods"
    Private Sub BusinessGroupContextMenu(sender As Object, e As ContextMenuEventArgs) Handles cboGroup.ContextMenuOpening
        If cboGroup.SelectedIndex = -1 Then
            cbiDeleteBG.IsEnabled = False
        Else
            cbiDeleteBG.IsEnabled = True
        End If

    End Sub

    'TODO: ADD ADDITIONAL CONTEXT_MENU_OPENING ROUTINES TO DISABLE DELETE OPTION; ADD WHEN BUILDING OUT DELETE OPTIONS FOR THE REMAINING CHOICES
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
            Dim check = BGC.Groups.Single(Function(p) p.BusinessGroupName = uni.StringVal.ToString)
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
            Dim a As Byte = cboGroup.Items.Count
            For ct = 0 To SortArray.Length - 1
                Dim cbi As ComboBoxItem = cboGroup.Items.Item(ct)
                cbi.Content = SortArray(ct)
                If SortArray(ct) = uni.StringVal Then cbi.IsSelected = True
            Next
            cboGroup.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))
        End Try
        uni.Close()
    End Sub

    Private Sub DeleteBusinessGroup(sender As Object, e As MouseButtonEventArgs)
        If cboGroup.SelectedIndex = -1 Then Exit Sub
        Dim bgnm As String = cboGroup.SelectedValue
        Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Medium, AgnesMessageBox.MsgBoxLayout.FullText, AgnesMessageBox.MsgBoxType.YesNo,
                                        18, False, "Confirm Delete",, "This will delete EVERYTHING related to " & bgnm & ", including refresh events.  Continue?")
        amsg.ShowDialog()
        If amsg.ReturnResult = "No" Then
            amsg.Close()
        Else
            Dim cbi As ComboBoxItem = cboGroup.SelectedItem
            cboGroup.Items.Remove(cbi)
            cboGroup.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))
            BG.DeleteFromDatabase(BG.GetGroupID(bgnm), bgnm)
        End If
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
            Dim check = BGC.Communications.Single(Function(p) p.Communication = uni.StringVal.ToString)
            Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Small, AgnesMessageBox.MsgBoxLayout.FullText, AgnesMessageBox.MsgBoxType.OkOnly,
                                            12,,,, "Comm type already exists")
            amsg.ShowDialog()
            amsg.Close()
        Catch ex As InvalidOperationException
            Dim comm As New Communications With {.Communication = uni.StringVal}
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
            Dim check = BGC.Cultures.Single(Function(p) p.Culture = uni.StringVal.ToString)
            Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Small, AgnesMessageBox.MsgBoxLayout.FullText, AgnesMessageBox.MsgBoxType.OkOnly,
                                            12,,,, "Culture type already exists")
            amsg.ShowDialog()
            amsg.Close()
        Catch ex As InvalidOperationException
            Dim cult As New Cultures With {.Culture = uni.StringVal}
            BGC.Cultures.Add(cult)
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
        Dim TempBGC As New BGCRMEntity
        Try
            Dim check = TempBGC.Leaders.Single(Function(p) p.Leader = uni.StringVal.ToString)
            Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Small, AgnesMessageBox.MsgBoxLayout.FullText, AgnesMessageBox.MsgBoxType.OkOnly,
                                            12,,,, "Team member already exists")
            amsg.ShowDialog()
            amsg.Close()
        Catch ex As InvalidOperationException
            Dim leader As New Leaders With {.Leader = uni.StringVal}
            TempBGC.Leaders.Add(leader)
            TempBGC.SaveChanges()
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
        Dim TempBGC As New BGCRMEntity
        Try
            Dim check = TempBGC.Customers.Single(Function(p) p.Customer = uni.StringVal.ToString)
            Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Small, AgnesMessageBox.MsgBoxLayout.FullText, AgnesMessageBox.MsgBoxType.OkOnly,
                                            12,,,, "Customer already exists")
            amsg.ShowDialog()
            amsg.Close()
        Catch ex As InvalidOperationException
            Dim customer As New Customers With {.Customer = uni.StringVal}
            TempBGC.Customers.Add(customer)
            TempBGC.SaveChanges()
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
        Dim TempBGC As New BGCRMEntity
        Try
            Dim check = TempBGC.OffsiteLocations.Single(Function(p) p.Location = uni.StringVal.ToString)
            Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Small, AgnesMessageBox.MsgBoxLayout.FullText, AgnesMessageBox.MsgBoxType.OkOnly,
                                            12,,,, "Locations already exists")
            amsg.ShowDialog()
            amsg.Close()
        Catch ex As InvalidOperationException
            Dim Offsite As New OffsiteLocations With {.Location = uni.StringVal}
            TempBGC.OffsiteLocations.Add(Offsite)
            TempBGC.SaveChanges()
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
        Dim TempBGC As New BGCRMEntity
        Try
            Dim check = TempBGC.Events.Single(Function(p) p.Event = uni.StringVal.ToString)
            Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Small, AgnesMessageBox.MsgBoxLayout.FullText, AgnesMessageBox.MsgBoxType.OkOnly,
                                            12,,,, "Event type already exists")
            amsg.ShowDialog()
            amsg.Close()
        Catch ex As InvalidOperationException
            Dim EventType As New Events With {.Event = uni.StringVal}
            TempBGC.Events.Add(EventType)
            TempBGC.SaveChanges()
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
        Dim TempBGC As New BGCRMEntity
        Try
            Dim check = TempBGC.Spaces.Single(Function(p) p.Space = uni.StringVal.ToString)
            Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Small, AgnesMessageBox.MsgBoxLayout.FullText, AgnesMessageBox.MsgBoxType.OkOnly,
                                            12,,,, "Event space already exists")
            amsg.ShowDialog()
            amsg.Close()
        Catch ex As InvalidOperationException
            Dim EventSpc As New Spaces With {.Space = uni.StringVal}
            TempBGC.Spaces.Add(EventSpc)
            TempBGC.SaveChanges()
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
        Dim TempBGC As New BGCRMEntity
        Try
            Dim check = TempBGC.Involvements.Single(Function(p) p.Involvement = uni.StringVal.ToString)
            Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Small, AgnesMessageBox.MsgBoxLayout.FullText, AgnesMessageBox.MsgBoxType.OkOnly,
                                            12,,,, "Involvement type already exists")
            amsg.ShowDialog()
            amsg.Close()
        Catch ex As InvalidOperationException
            Dim Involve As New Involvements With {.Involvement = uni.StringVal}
            TempBGC.Involvements.Add(Involve)
            TempBGC.SaveChanges()
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
        Dim TempBGC As New BGCRMEntity
        Try
            Dim check = TempBGC.NotableEvents.Single(Function(p) p.Event = uni.StringVal.ToString)
            Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Small, AgnesMessageBox.MsgBoxLayout.FullText, AgnesMessageBox.MsgBoxType.OkOnly,
                                            12,,,, "Event already exists")
            amsg.ShowDialog()
            amsg.Close()
        Catch ex As InvalidOperationException
            Dim Notable As New NotableEvents With {.Event = uni.StringVal}
            TempBGC.NotableEvents.Add(Notable)
            TempBGC.SaveChanges()
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
        Dim TempBGC As New BGCRMEntity
        Try
            Dim check = TempBGC.Planners.Single(Function(p) p.Planner = uni.StringVal.ToString)
            Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Small, AgnesMessageBox.MsgBoxLayout.FullText, AgnesMessageBox.MsgBoxType.OkOnly,
                                            12,,,, "Planner already exists")
            amsg.ShowDialog()
            amsg.Close()
        Catch ex As InvalidOperationException
            Dim Plannr As New Planners With {.Planner = uni.StringVal}
            TempBGC.Planners.Add(Plannr)
            TempBGC.SaveChanges()
            Dim li As New ListBoxItem With {.Content = uni.StringVal, .Tag = "S"}
            AddHandler li.MouseDoubleClick, AddressOf PlannerMove
            lbxPlannersChosen.Items.Add(li)
            lbxPlannersChosen.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))
        End Try
        uni.Close()
    End Sub

#End Region

#End Region

End Class
