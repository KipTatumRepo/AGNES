Public Class RefreshEvent
    Private _groupid As Integer
    Public Property GroupID As Integer
        Get
            Return _groupid
        End Get
        Set(value As Integer)
            _groupid = value
        End Set
    End Property

    Private _startdate As Date
    Public Property MoveStart As Date
        Get
            Return _startdate
        End Get
        Set(value As Date)
            _startdate = value
        End Set
    End Property

    Private _enddate As Date
    Public Property MoveEnd As Date
        Get
            Return _enddate
        End Get
        Set(value As Date)
            _enddate = value
        End Set
    End Property

    Private _totalpopulation As Integer

    Public Property TotalPopulation As Integer
        Get
            Return _totalpopulation
        End Get
        Set(value As Integer)
            _totalpopulation = value
        End Set
    End Property

    Public Property BuildingsMoving As New List(Of CRBuilding)

    Private _destbuild As String
    Public Property DestinationBuilding As String
        Get
            Return _destbuild
        End Get
        Set(value As String)
            _destbuild = value
        End Set
    End Property



End Class
