Public Class objBusinessGroup
#Region "Properties"
    Private _orgname As String
    Private ef As BGCRMEntity
    Private sd As BIEntities
    Public AlreadyExists As Boolean
    Public Property OrgName As String
        Get
            Return _orgname
        End Get
        Set(value As String)
            _orgname = value
        End Set
    End Property
    Private _overview As String
    Public Property Overview As String
        Get
            Return _overview
        End Get
        Set(value As String)
            _overview = value
        End Set
    End Property
    Private _headcount As Integer
    Public Property Headcount As Integer
        Get
            Return _headcount
        End Get
        Set(value As Integer)
            _headcount = value
        End Set
    End Property
    Private _worktimes As Byte
    Public Property WorkTimes As Byte
        Get
            Return _worktimes
        End Get
        Set(value As Byte)
            _worktimes = value
        End Set
    End Property
    Private _onrem As Byte
    Public Property OnsiteRemote As Byte
        Get
            Return _onrem
        End Get
        Set(value As Byte)
            _onrem = value
        End Set
    End Property
    Public Property Communications As New List(Of Long)
    Public Property Culture As New List(Of Long)
    Public Property Locations As New List(Of Long)
    Private _orgleader As Long
    Public Property OrgLeader As Long
        Get
            Return _orgleader
        End Get
        Set(value As Long)
            _orgleader = value
        End Set
    End Property
    Private _relmanager As Long
    Public Property RelationshipMgr As Long
        Get
            Return _relmanager
        End Get
        Set(value As Long)
            _relmanager = value
        End Set
    End Property
    Public Property Leadership As New List(Of Long)
    Public Property FrequentCustomers As New List(Of Long)
    Private _totalrev As Double
    Public Property TotalRevenue As Double
        Get
            Return _totalrev
        End Get
        Set(value As Double)
            _totalrev = value
        End Set
    End Property
    Private _totalevents As Integer
    Public Property TotalEvents As Integer
        Get
            Return _totalevents
        End Get
        Set(value As Integer)
            _totalevents = value
        End Set
    End Property
    Private _events500 As Integer
    Public Property Events500 As Integer
        Get
            Return _events500
        End Get
        Set(value As Integer)
            _events500 = value
        End Set
    End Property
    Private _caterevents As Integer
    Public Property CateredEvents As Integer
        Get
            Return _caterevents
        End Get
        Set(value As Integer)
            _caterevents = value
        End Set
    End Property
    Private _offsitespend As Double
    Public Property OffSiteSpend As Double
        Get
            Return _offsitespend
        End Get
        Set(value As Double)
            _offsitespend = value
        End Set
    End Property
    Public Property TopOffsiteLocations As New List(Of Long)    '// Could be expanded to a list of objects with more detailed info
    Public Property EmbeddedPlanners As New List(Of Long)       '// Could be expanded to a list of objects with more detailed info
    Public Property TopBookedSpaces As New List(Of Long)        '// Could be expanded to a list of objects with more detailed info
    Public Property TopEventTypes As New List(Of Long)
    Public Property NotableEvents As New List(Of Long)          '// Could be expanded to a list of objects with more detailed info
    Public Property EventionsInvolvement As New List(Of Long)
    Public Property CREvents As New List(Of RefreshEvent)
    Public Property SaveSuccessful As Boolean
#End Region
    Public Sub New()
        sd = New BIEntities
        ef = New BGCRMEntity
    End Sub

    Public Sub Load(bgn)
        Dim OrgID As Long
        Dim GetGroupID = From businessgroups In ef.BusinessGroups
                         Where businessgroups.BusinessGroupName Is bgn
                         Select businessgroups

        If GetGroupID.Count = 0 Then
            AlreadyExists = False
            Exit Sub
        End If
        AlreadyExists = True
        For Each c In GetGroupID
            OrgName = c.BusinessGroupName
            OrgID = c.BusinessGroupID
            Overview = c.GroupOverview
            Headcount = c.Headcount
            WorkTimes = c.WorkTimes
            OnsiteRemote = c.OnsiteRemote
            OrgLeader = c.OrgLeader
            RelationshipMgr = c.RelMgr
            TotalRevenue = c.Revenue
            TotalEvents = c.Events
            Events500 = c.Events500
            CateredEvents = c.EventsCatered
            OffSiteSpend = c.OffsiteSpend
        Next
        Communications.Clear()
        Dim GetComms = From cj In ef.Comm_Join
                       Where cj.BGId = OrgID
                       Select cj
        For Each ct In GetComms
            Communications.Add(ct.CommId)
        Next
        Culture.Clear()
        Dim GetCult = From cj In ef.Culture_Join
                      Where cj.BGId = OrgID
                      Select cj
        For Each ct In GetCult
            Culture.Add(ct.CultureId)
        Next
        Locations.Clear()
        Dim GetLocs = From lj In ef.Locations_Join
                      Where lj.BGId = OrgID
                      Select lj
        For Each ct In GetLocs
            Locations.Add(ct.LocId)
        Next
        Leadership.Clear()
        Dim GetLdrs = From lj In ef.Leaders_Join
                      Where lj.BGId = OrgID
                      Select lj
        For Each ct In GetLdrs
            Leadership.Add(ct.LeaderId)
        Next
        FrequentCustomers.Clear()
        Dim GetCust = From lj In ef.FreqCust_Join
                      Where lj.BGId = OrgID
                      Select lj
        For Each ct In GetCust
            FrequentCustomers.Add(ct.CustId)
        Next
        TopOffsiteLocations.Clear()
        Dim GetOffsites = From lj In ef.Offsites_Join
                          Where lj.BGId = OrgID
                          Select lj
        For Each ct In GetOffsites
            TopOffsiteLocations.Add(ct.OffsiteId)
        Next
        NotableEvents.Clear()
        Dim GetNotables = From lj In ef.NotableEvents_Join
                          Where lj.BGId = OrgID
                          Select lj
        For Each ct In GetNotables
            NotableEvents.Add(ct.EventId)
        Next
        TopEventTypes.Clear()
        Dim GetTopEvents = From lj In ef.TopEventTypes_Join
                           Where lj.BGId = OrgID
                           Select lj
        For Each ct In GetTopEvents
            TopEventTypes.Add(ct.TypeId)
        Next
        TopBookedSpaces.Clear()
        Dim GetTopSpaces = From lj In ef.TopSpaces_Join
                           Where lj.BGId = OrgID
                           Select lj
        For Each ct In GetTopSpaces
            TopBookedSpaces.Add(ct.SpaceID)
        Next
        EventionsInvolvement.Clear()
        Dim GetInvolvements = From lj In ef.Involvement_Join
                              Where lj.BGId = OrgID
                              Select lj
        For Each ct In GetInvolvements
            EventionsInvolvement.Add(ct.InvolveId)
        Next
        EmbeddedPlanners.Clear()
        Dim GetPlanners = From lj In ef.Planners_Join
                          Where lj.BGId = OrgID
                          Select lj
        For Each ct In GetPlanners
            EmbeddedPlanners.Add(ct.PlannerId)
        Next
        CREvents.Clear()
        Dim GetRefreshEvents = From refreshevents In ef.RefreshEvents
                               Where refreshevents.BGId = OrgID
                               Select refreshevents
        For Each c In GetRefreshEvents
            Dim ncr As New RefreshEvent
            With ncr
                .RefreshEventName = c.RefreshEventName
                .GroupID = OrgID
                .MoveStart = c.MoveStartDate
                .MoveEnd = c.MoveEndDate
                .TotalPopulation = c.MovePopulation
                .DestinationBuilding = c.Destination
            End With
            Dim GetCROrigins = From refresheventorigins In ef.RefreshEventOrigins
                               Where refresheventorigins.EventId = c.EventID
                               Select refresheventorigins
            For Each d In GetCROrigins
                Dim ncrb As New CRBuilding
                With ncrb
                    .BuildingId = d.BuildingId
                    .MovePopulation = d.PopMoving
                    .BuildingName = FetchBuildingName(d.BuildingId)
                End With
                ncr.BuildingsMoving.Add(ncrb)
            Next
            CREvents.Add(ncr)
        Next
    End Sub

#Region "Save Methods"

    Public Sub Save(ByRef EnFrModel)
        ef = EnFrModel
        Try
            Dim IsNew = ef.BusinessGroups.Single(Function(p) p.BusinessGroupName = OrgName)
            UpdateExisting()
        Catch ex As InvalidOperationException
            SaveNew()
        Catch ex As Exception
            SaveSuccessful = False
        End Try

    End Sub
    Private Sub UpdateExisting()
        Try
            '// Handle all non-joined first, save, query for PID, and then handle writing to _join tables
            Dim bg = ef.BusinessGroups.Single(Function(p) p.BusinessGroupName = OrgName)
            Dim bgid As Long = bg.BusinessGroupID
            With bg
                .BusinessGroupName = OrgName
                .GroupOverview = Overview
                .Headcount = Headcount
                .WorkTimes = WorkTimes
                .OnsiteRemote = OnsiteRemote
                .OrgLeader = OrgLeader
                .RelMgr = RelationshipMgr
                .Revenue = TotalRevenue
                .Events = TotalEvents
                .Events500 = Events500
                .EventsCatered = CateredEvents
                .OffsiteSpend = OffSiteSpend
            End With
            DeleteComms(bgid)
            SaveComms(bgid)
            DeleteCultures(bgid)
            SaveCulture(bgid)
            DeleteLocations(bgid)
            SaveLocations(bgid)
            DeleteLeaders(bgid)
            SaveLeadership(bgid)
            DeleteCustomers(bgid)
            SaveCustomers(bgid)
            DeleteOffsites(bgid)
            SaveOffsites(bgid)
            DeleteNotables(bgid)
            SaveNotables(bgid)
            DeleteTopEvents(bgid)
            SaveTypes(bgid)
            DeleteSpaces(bgid)
            SaveSpaces(bgid)
            DeleteInvolvements(bgid)
            SaveInvolvements(bgid)
            DeletePlanners(bgid)
            SavePlanners(bgid)
            DeleteRefreshEvents(bgid)
            DeleteOrigins(bgid)
            'ef.SaveChanges()
            SaveRefreshEvents(bgid)
            ef.SaveChanges()
            SaveSuccessful = True
        Catch excep As Exception
            Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Medium, AgnesMessageBox.MsgBoxLayout.FullText, AgnesMessageBox.MsgBoxType.OkOnly, 14, True, "Save failed.",, "Error : " & excep.Message)
            amsg.ShowDialog()
            amsg.Close()
            SaveSuccessful = False
        End Try
    End Sub
    Private Sub SaveNew()
        Try
            '// Handle all non-joined first, save, query for next business group id, and then handle writing to _join tables
            '// Determine if the business group already exists
            Dim qwl = Aggregate c In ef.BusinessGroups
            Into Max(c.BusinessGroupID)

            Dim NextID As Long = qwl + 1
            SaveComms(NextID)
            SaveCulture(NextID)
            SaveLocations(NextID)
            SaveLeadership(NextID)
            SaveCustomers(NextID)
            SaveOffsites(NextID)
            SaveNotables(NextID)
            SaveTypes(NextID)
            SaveSpaces(NextID)
            SaveInvolvements(NextID)
            SavePlanners(NextID)
            SaveRefreshEvents(NextID)
            Dim bg As New BusinessGroup
            With bg
                .BusinessGroupID = NextID
                .BusinessGroupName = OrgName
                .GroupOverview = Overview
                .Headcount = Headcount
                .WorkTimes = WorkTimes
                .OnsiteRemote = OnsiteRemote
                .OrgLeader = OrgLeader
                .RelMgr = RelationshipMgr
                .Revenue = TotalRevenue
                .Events = TotalEvents
                .Events500 = Events500
                .EventsCatered = CateredEvents
                .OffsiteSpend = OffSiteSpend
            End With
            ef.BusinessGroups.Add(bg)
            Try
                ef.SaveChanges()
                SaveSuccessful = True
            Catch ex As Exception
                SaveSuccessful = False
            End Try
        Catch excep As Exception
            SaveSuccessful = False
        End Try
    End Sub
    Private Sub SaveComms(bgid As Long)
        For Each i As Long In Communications
            Dim cj As New Comm_Join
            With cj
                .BGId = bgid
                .CommId = i
            End With
            ef.Comm_Join.Add(cj)
        Next
    End Sub
    Private Sub SaveCulture(bgid As Long)
        For Each i As Long In Culture
            Dim cj As New Culture_Join
            With cj
                .BGId = bgid
                .CultureId = i
            End With
            ef.Culture_Join.Add(cj)
        Next
    End Sub
    Private Sub SaveLocations(bgid As Long)
        For Each i As Long In Locations
            Dim lj As New Locations_Join
            With lj
                .BGId = bgid
                .LocId = i
            End With
            ef.Locations_Join.Add(lj)
        Next
    End Sub
    Private Sub SaveLeadership(bgid As Long)
        For Each i As Long In Leadership
            Dim lj As New Leaders_Join
            With lj
                .BGId = bgid
                .LeaderId = i
            End With
            ef.Leaders_Join.Add(lj)
        Next
    End Sub
    Private Sub SaveOffsites(bgid As Long)
        For Each i As Long In TopOffsiteLocations
            Dim oj As New Offsites_Join
            With oj
                .BGId = bgid
                .OffsiteId = i
            End With
            ef.Offsites_Join.Add(oj)
        Next
    End Sub
    Private Sub SaveCustomers(bgid As Long)
        For Each i As Long In FrequentCustomers
            Dim cj As New FreqCust_Join
            With cj
                .BGId = bgid
                .CustId = i
            End With
            ef.FreqCust_Join.Add(cj)
        Next
    End Sub
    Private Sub SaveNotables(bgid As Long)
        For Each i As Long In NotableEvents
            Dim nj As New NotableEvents_Join
            With nj
                .BGId = bgid
                .EventId = i
            End With
            ef.NotableEvents_Join.Add(nj)
        Next
    End Sub
    Private Sub SaveTypes(bgid As Long)
        For Each i As Long In TopEventTypes
            Dim ej As New TopEventTypes_Join
            With ej
                .BGId = bgid
                .TypeId = i
            End With
            ef.TopEventTypes_Join.Add(ej)
        Next
    End Sub
    Private Sub SaveSpaces(bgid As Long)
        For Each i As Long In TopBookedSpaces
            Dim sj As New TopSpaces_Join
            With sj
                .BGId = bgid
                .SpaceID = i
            End With
            ef.TopSpaces_Join.Add(sj)
        Next
    End Sub
    Private Sub SaveInvolvements(bgid As Long)
        For Each i As Long In EventionsInvolvement
            Dim ij As New Involvement_Join
            With ij
                .BGId = bgid
                .InvolveId = i
            End With
            ef.Involvement_Join.Add(ij)
        Next
    End Sub
    Private Sub SavePlanners(bgid As Long)
        For Each i As Long In EmbeddedPlanners
            Dim pj As New Planners_Join
            With pj
                .BGId = bgid
                .PlannerId = i
            End With
            ef.Planners_Join.Add(pj)
        Next
    End Sub
    Private Sub SaveRefreshEvents(bgid As Long)
        Dim EID As Long
        '// Fetch next event ID
        Try
            Dim qwl = Aggregate c In ef.RefreshEvents
                Into Max(c.EventID)
            EID = qwl + 1
        Catch ex As Exception
            EID = 1
        End Try
        Dim a As Byte = CREvents.Count
        For Each cr As RefreshEvent In CREvents
            '// Write base event to database
            Dim re As New RefreshEvent
            With re
                .EventID = EID
                .RefreshEventName = cr.RefreshEventName
                .BGId = bgid
                .MoveStartDate = cr.MoveStart
                .MoveEndDate = cr.MoveEnd
                .Destination = cr.DestinationBuilding
                .MovePopulation = cr.TotalPopulation
            End With
            ef.RefreshEvents.Add(re)
            '// for each origin building, write to the Origins database
            For Each crb As CRBuilding In cr.BuildingsMoving
                Dim eob As New RefreshEventOrigin
                With eob
                    .EventId = EID
                    .BuildingId = crb.BuildingId
                    .PopMoving = crb.MovePopulation
                    .BGId = bgid
                End With
                ef.RefreshEventOrigins.Add(eob)
            Next
            EID += 1
        Next
    End Sub

#End Region

#Region "Deletion Methods"
    Public Sub DeleteFromDatabase(bgid As Long, Optional bgnm As String = "")
        DeleteGroup(bgid)
        DeleteComms(bgid)
        DeleteCultures(bgid)
        DeleteLocations(bgid)
        DeleteLeaders(bgid)
        DeleteCustomers(bgid)
        DeleteOffsites(bgid)
        DeleteNotables(bgid)
        DeleteTopEvents(bgid)
        DeleteSpaces(bgid)
        DeleteInvolvements(bgid)
        DeletePlanners(bgid)
        DeleteRefreshEvents(bgid)
        DeleteOrigins(bgid)
        ef.SaveChanges()
        Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Small, AgnesMessageBox.MsgBoxLayout.FullText, AgnesMessageBox.MsgBoxType.OkOnly,
                                        14, False, "Command Successful",, bgnm & " has been deleted.")
        amsg.ShowDialog()
        amsg.Close()
    End Sub
    Private Sub DeleteGroup(bgid As Long)
        '// Delete from BusinessGroups
        Dim bgq = From bgrp In ef.BusinessGroups Select bgrp Where bgrp.BusinessGroupID = bgid
        For Each bgrp In bgq
            bgid = bgrp.BusinessGroupID
            ef.BusinessGroups.Remove(bgrp)
        Next
    End Sub
    Private Sub DeleteComms(bgid As Long)
        '// Delete from Communications
        Try
            Dim dbq = From FoundItem In ef.Comm_Join Select FoundItem Where FoundItem.BGId = bgid
            For Each FoundItem In dbq
                ef.Comm_Join.Remove(FoundItem)
            Next
        Catch ex As Exception
            '// Nothing in table
        End Try
    End Sub
    Private Sub DeleteCultures(bgid As Long)
        '// Delete from Cultures
        Try
            Dim dbq = From FoundItem In ef.Culture_Join Select FoundItem Where FoundItem.BGId = bgid
            For Each FoundItem In dbq
                ef.Culture_Join.Remove(FoundItem)
            Next
        Catch ex As Exception
            '// Nothing in table
        End Try
    End Sub
    Private Sub DeleteLocations(bgid As Long)
        '// Delete from Locations
        Try
            Dim dbq = From FoundItem In ef.Locations_Join Select FoundItem Where FoundItem.BGId = bgid
            For Each FoundItem In dbq
                ef.Locations_Join.Remove(FoundItem)
            Next
        Catch ex As Exception
            '// Nothing in table
        End Try
    End Sub
    Private Sub DeleteLeaders(bgid As Long)
        '// Delete from Leaders
        Try
            Dim dbq = From FoundItem In ef.Leaders_Join Select FoundItem Where FoundItem.BGId = bgid
            For Each FoundItem In dbq
                ef.Leaders_Join.Remove(FoundItem)
            Next
        Catch ex As Exception
            '// Nothing in table
        End Try
    End Sub
    Private Sub DeleteCustomers(bgid As Long)
        '// Delete from Customers
        Try
            Dim dbq = From FoundItem In ef.FreqCust_Join Select FoundItem Where FoundItem.BGId = bgid
            For Each FoundItem In dbq
                ef.FreqCust_Join.Remove(FoundItem)
            Next
        Catch ex As Exception
            '// Nothing in table
        End Try
    End Sub
    Private Sub DeleteOffsites(bgid As Long)
        '// Delete from Offsites
        Try
            Dim dbq = From FoundItem In ef.Offsites_Join Select FoundItem Where FoundItem.BGId = bgid
            For Each FoundItem In dbq
                ef.Offsites_Join.Remove(FoundItem)
            Next
        Catch ex As Exception
            '// Nothing in table
        End Try
    End Sub
    Private Sub DeleteNotables(bgid As Long)
        '// Delete from Notables
        Try
            Dim dbq = From FoundItem In ef.NotableEvents_Join Select FoundItem Where FoundItem.BGId = bgid
            For Each FoundItem In dbq
                ef.NotableEvents_Join.Remove(FoundItem)
            Next
        Catch ex As Exception
            '// Nothing in table
        End Try
    End Sub
    Private Sub DeleteTopEvents(bgid As Long)
        '// Delete from Top Event Types
        Try
            Dim dbq = From FoundItem In ef.TopEventTypes_Join Select FoundItem Where FoundItem.BGId = bgid
            For Each FoundItem In dbq
                ef.TopEventTypes_Join.Remove(FoundItem)
            Next
        Catch ex As Exception
            '// Nothing in table
        End Try

    End Sub
    Private Sub DeleteSpaces(bgid As Long)
        '// Delete from Top Event Spaces
        Try
            Dim dbq = From FoundItem In ef.TopSpaces_Join Select FoundItem Where FoundItem.BGId = bgid
            For Each FoundItem In dbq
                ef.TopSpaces_Join.Remove(FoundItem)
            Next
        Catch ex As Exception
            '// Nothing in table
        End Try

    End Sub
    Private Sub DeleteInvolvements(bgid As Long)
        '// Delete from Involvements
        Try
            Dim dbq = From FoundItem In ef.Involvement_Join Select FoundItem Where FoundItem.BGId = bgid
            For Each FoundItem In dbq
                ef.Involvement_Join.Remove(FoundItem)
            Next
        Catch ex As Exception
            '// Nothing in table
        End Try
    End Sub
    Private Sub DeletePlanners(bgid As Long)
        '// Delete from Planners
        Try
            Dim dbq = From FoundItem In ef.Planners_Join Select FoundItem Where FoundItem.BGId = bgid
            For Each FoundItem In dbq
                ef.Planners_Join.Remove(FoundItem)
            Next
        Catch ex As Exception
            '// Nothing in table
        End Try
    End Sub
    Private Sub DeleteRefreshEvents(bgid As Long)
        '// Delete from Refresh Events
        Try
            Dim dbq = From FoundItem In ef.RefreshEvents Select FoundItem Where FoundItem.BGId = bgid
            For Each FoundItem In dbq
                ef.RefreshEvents.Remove(FoundItem)
            Next
        Catch ex As Exception
            '// Nothing in table
        End Try
    End Sub
    Private Sub DeleteOrigins(bgid As Long)
        '// Delete from Refresh Event Origins
        Try
            Dim dbq = From FoundItem In ef.RefreshEventOrigins Select FoundItem Where FoundItem.BGId = bgid
            For Each FoundItem In dbq
                ef.RefreshEventOrigins.Remove(FoundItem)
            Next
        Catch ex As Exception
            '// Nothing in table
        End Try
    End Sub

#End Region

#Region "Functions"
    Public Function FetchBuildingID(bn) As Integer
        Dim retval As Integer = 0
        Dim loq = From bloc In sd.MasterBuildingLists Select bloc Where bloc.BuildingName Is bn
        For Each bloc In loq
            retval = bloc.PID
        Next
        Return retval
    End Function
    Public Function FetchBuildingName(bid As Integer) As String
        Dim retval As String = ""
        Dim dbq = From bldg In sd.MasterBuildingLists Select bldg Where bldg.PID = bid
        For Each bldg In dbq
            retval = bldg.BuildingName
        Next
        Return retval
    End Function
    Public Function GetGroupID(bn) As Integer
        Try
            Dim retval As Integer = 0
            Dim grp As New BusinessGroup
            grp = ef.BusinessGroups.Find(bn)
            Return grp.BusinessGroupID
        Catch ex As Exception
            Return 0
        End Try
    End Function
#End Region

End Class
