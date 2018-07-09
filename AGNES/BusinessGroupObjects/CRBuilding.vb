Public Class CRBuilding
    Private _buildingid As Integer
    Public Property BuildingId As Integer
        Get
            Return _buildingid
        End Get
        Set(value As Integer)
            _buildingid = value
        End Set
    End Property

    Private _movepop As Integer
    Public Property MovePopulation As Integer
        Get
            Return _movepop
        End Get
        Set(value As Integer)
            _movepop = value
        End Set
    End Property

End Class
