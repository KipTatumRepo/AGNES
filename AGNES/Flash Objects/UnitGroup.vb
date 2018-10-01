Public Class UnitGroup
    '// Container class for UnitFlash class objects, used to construct the Flash interface itself, specifically informing the 
    '   UnitChooser control

    Public Property UnitGroupName As String
    Public Property UnitsInGroup As New List(Of UnitFlash)
End Class
