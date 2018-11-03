Public Class UnitGroup
    '// Container class for UnitFlash or UnitFcast class objects, used to construct the Flash and Forecast interfaces, specifically informing the 
    '   UnitChooser control

    Public Property Summoner As Byte ' 0 = Flash, 1 = Forecast
    Public Property UnitGroupName As String
    Public Property UnitsInGroup As New List(Of UnitFlash)
    Public Property FcastUnitsInGroup As New List(Of UnitFcast)
End Class
