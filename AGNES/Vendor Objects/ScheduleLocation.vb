Imports System.ComponentModel
Public Class ScheduleLocation
    Inherits Border

#Region "Properties"
    Public StationStack As StackPanel
    Public Property LocationName As String
    Public Property LocationBlock As TextBlock
    Public Property StationCount As Byte
    Private Property HighlightColor As Boolean = True
    Private StatusBarText As String
#End Region

#Region "Constructor"
    Public Sub New(locname, sc)
        StationCount = sc
        BorderBrush = Brushes.Black
        BorderThickness = New Thickness(1, 1, 1, 1)
        Margin = New Thickness(1, 1, 1, 0)
        LocationName = locname
        StationStack = New StackPanel
        Child = StationStack
        AddName()
        AddStations()
    End Sub

#End Region

#Region "Public Methods"

#End Region

#Region "Private Methods"
    Private Sub AddName()
        LocationBlock = New TextBlock With {.TextAlignment = TextAlignment.Center, .Text = LocationName, .Background = Brushes.LightYellow}
        StationStack.Children.Add(LocationBlock)
    End Sub

    Private Sub AddStations()
        For x As Byte = 1 To StationCount
            Dim station As New ScheduleStation(x)
            StationStack.Children.Add(station)
        Next
    End Sub

#End Region

End Class
