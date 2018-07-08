Public Class RefreshEvent
    Private _orgid As Integer
    Public Property OrgID As Integer
        Get
            Return _orgid
        End Get
        Set(value As Integer)
            _orgid = value
        End Set
    End Property

    Private _eventdate As Date
    Public Property MoveDate As Date
        Get
            Return _eventdate
        End Get
        Set(value As Date)
            _eventdate = value
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

    Private _destbuild As Integer
    Public Property DestinationBuilding As Integer
        Get
            Return _destbuild
        End Get
        Set(value As Integer)
            _destbuild = value
        End Set
    End Property

    Public Property BuildingsMoving As List(Of CRBuilding)

End Class
