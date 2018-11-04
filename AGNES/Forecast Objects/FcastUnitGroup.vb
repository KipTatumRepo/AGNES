Public Class FcastUnitGroup
    '// Container class for UnitFcast class objects, used to construct the Forecast interface itself, specifically informing the 
    '   UnitChooser control

    Public Property UnitGroupName As String
    Public Property UnitsInGroup As New List(Of UnitFcast)
End Class
