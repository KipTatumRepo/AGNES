Public Class objBusinessGroup
    Private _orgname As String
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
    Public Property Communications As List(Of Byte)
    Public Property Culture As List(Of Byte)
    Public Property Locations As List(Of CRBuilding)
    Private _orgleader As Byte
    Public Property OrgLeader As Byte
        Get
            Return _orgleader
        End Get
        Set(value As Byte)
            _orgleader = value
        End Set
    End Property
    Private _relmanager As Byte
    Public Property RelationshipMgr As Byte
        Get
            Return _relmanager
        End Get
        Set(value As Byte)
            _relmanager = value
        End Set
    End Property
    Public Property Leadership As List(Of Byte)
    Public Property FrequentCustomers As List(Of Byte)
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
    Public Property TopOffsiteLocations As List(Of Byte)    '// Could be expanded to a list of objects with more detailed info
    Public Property EmbeddedPlanners As List(Of Byte)       '// Could be expanded to a list of objects with more detailed info
    Public Property TopBookedSpaces As List(Of Byte)        '// Could be expanded to a list of objects with more detailed info
    Public Property TopEventTypes As List(Of Byte)
    Public Property NotableEvents As List(Of Byte)          '// Could be expanded to a list of objects with more detailed info
    Public Property EventionsInvolvement As List(Of Byte)
    Public Property CREvents As List(Of RefreshEvent)

    Public Sub New()
        Dim ph As String = ""
    End Sub

    Public Sub Load()
        Dim ph As String = ""
    End Sub

    Public Sub Save()
        Dim ph As String = ""
    End Sub

    Public Sub Delete()
        Dim ph As String = ""
    End Sub
End Class
