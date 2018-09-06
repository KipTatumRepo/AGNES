Public Class objBusinessGroup
    Private _orgname As String
    Private ef As BGCRMEntity
    Private sd As BIEntities
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
    Public Sub New()
        Dim ph As String = ""
    End Sub

    Public Sub Load()
        Dim ph As String = ""
    End Sub

    Public Sub Save(ByRef EnFrModel)
        ef = EnFrModel
        Try
            Dim IsNew = ef.BusinessGroups.Single(Function(p) p.BusinessGroupName = OrgName)
            UpdateExisting()
        Catch ex As InvalidOperationException
            SaveNew()
        Catch ex As Exception
        End Try

    End Sub

    Private Sub UpdateExisting()
        Try
            '// Handle all non-joined first, save, query for PID, and then handle writing to _join tables
            Dim bg = ef.BusinessGroups.Single(Function(p) p.BusinessGroupName = OrgName)
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

            ef.SaveChanges()

            SaveComms(bg.PID, 1)
            SaveCulture(bg.PID, 1)
            SaveLocations(bg.PID, 1)
            SaveLeadership(bg.PID, 1)
            SaveCustomers(bg.PID, 1)
            SaveOffsites(bg.PID, 1)
            SaveNotables(bg.PID, 1)
            SaveTypes(bg.PID, 1)
            SaveSpaces(bg.PID, 1)
            SaveInvolvements(bg.PID, 1)
            SavePlanners(bg.PID, 1)

        Catch excep As Exception
        End Try
    End Sub

    Private Sub SaveNew()
        Try
            '// Handle all non-joined first, save, query for next business group id, and then handle writing to _join tables
            '// Determine if the business group already exists
            Dim qwl = Aggregate c In ef.BusinessGroups
            Into Max(c.BusinessGroupID)

            Dim NextID As Long = qwl + 1
            SaveComms(NextID, 0)
            SaveCulture(NextID, 0)
            SaveLocations(NextID, 0)
            SaveLeadership(NextID, 0)
            SaveCustomers(NextID, 0)
            SaveOffsites(NextID, 0)
            SaveNotables(NextID, 0)
            SaveTypes(NextID, 0)
            SaveSpaces(NextID, 0)
            SaveInvolvements(NextID, 0)
            SavePlanners(NextID, 0)
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
    Private Sub SaveComms(bgid, isnew)
        If isnew = 0 Then
            For Each i As Long In Communications
                Dim cj As New Comm_Join
                With cj
                    .BGId = bgid
                    .CommId = i
                End With
                ef.Comm_Join.Add(cj)
            Next
        Else
            Dim ph1 As String = ""
        End If

    End Sub
    Private Sub SaveCulture(bgid, isnew)
        If isnew = 0 Then
            For Each i As Long In Culture
                Dim cj As New Culture_Join
                With cj
                    .BGId = bgid
                    .CultureId = i
                End With
                ef.Culture_Join.Add(cj)
            Next
        Else
            Dim ph1 As String = ""
        End If
    End Sub
    Private Sub SaveLocations(bgid, isnew)
        If isnew = 0 Then
            For Each i As Long In Locations
                Dim lj As New Locations_Join
                With lj
                    .BGId = bgid
                    .LocId = i
                End With
                ef.Locations_Join.Add(lj)
            Next
        Else
            Dim ph1 As String = ""
        End If

    End Sub
    Private Sub SaveLeadership(bgid, isnew)
        If isnew = 0 Then
            For Each i As Long In Leadership
                Dim lj As New Leaders_Join
                With lj
                    .BGId = bgid
                    .LeaderId = i
                End With
                ef.Leaders_Join.Add(lj)
            Next
        Else
            Dim ph1 As String = ""
        End If
    End Sub
    Private Sub SaveOffsites(bgid, isnew)
        If isnew = 0 Then
            For Each i As Long In TopOffsiteLocations
                Dim oj As New Offsites_Join
                With oj
                    .BGId = bgid
                    .OffsiteId = i
                End With
                ef.Offsites_Join.Add(oj)
            Next
        Else
            Dim ph1 As String = ""
        End If
    End Sub
    Private Sub SaveCustomers(bgid, isnew)
        If isnew = 0 Then
            For Each i As Long In FrequentCustomers
                Dim cj As New FreqCust_Join
                With cj
                    .BGId = bgid
                    .CustId = i
                End With
                ef.FreqCust_Join.Add(cj)
            Next
        Else
            Dim ph1 As String = ""
        End If
    End Sub
    Private Sub SaveNotables(bgid, isnew)
        If isnew = 0 Then
            For Each i As Long In NotableEvents
                Dim nj As New NotableEvents_Join
                With nj
                    .BGId = bgid
                    .EventId = i
                End With
                ef.NotableEvents_Join.Add(nj)
            Next
        Else
            Dim ph1 As String = ""
        End If
    End Sub
    Private Sub SaveTypes(bgid, isnew)
        If isnew = 0 Then
            For Each i As Long In TopEventTypes
                Dim ej As New TopEventTypes_Join
                With ej
                    .BGGroup = bgid
                    .TypeId = i
                End With
                ef.TopEventTypes_Join.Add(ej)
            Next
        Else
            Dim ph1 As String = ""
        End If
    End Sub
    Private Sub SaveSpaces(bgid, isnew)
        If isnew = 0 Then
            For Each i As Long In TopBookedSpaces
                Dim sj As New TopSpaces_Join
                With sj
                    .BGId = bgid
                    .SpaceID = i
                End With
                ef.TopSpaces_Join.Add(sj)
            Next
        Else
            Dim ph1 As String = ""
        End If
    End Sub
    Private Sub SaveInvolvements(bgid, isnew)
        If isnew = 0 Then
            For Each i As Long In EventionsInvolvement
                Dim ij As New Involvement_Join
                With ij
                    .BGId = bgid
                    .InvolveId = i
                End With
                ef.Involvement_Join.Add(ij)
            Next
        Else
            Dim ph1 As String = ""
        End If
    End Sub
    Private Sub SavePlanners(bgid, isnew)
        If isnew = 0 Then
            For Each i As Long In EmbeddedPlanners
                Dim pj As New Planners_Join
                With pj
                    .BGId = bgid
                    .PlannerId = i
                End With
                ef.Planners_Join.Add(pj)
            Next
        Else
            Dim ph1 As String = ""
        End If
    End Sub
    Public Sub Delete()
        Dim ph As String = ""
    End Sub
End Class
