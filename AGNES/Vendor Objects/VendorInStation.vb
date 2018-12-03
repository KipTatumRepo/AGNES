Public Class VendorInStation
    Inherits TextBlock

#Region "Properties"
    Public Property ReferencedVendor As ScheduleVendor
    Public Property ReferencedStation As ScheduleStation
    Public Property ReferencedLoc As ScheduleLocation
    Public Property ReferencedTruckStation As ScheduleTruckStation
    Public Property IsBrand As Boolean
    Private DeleteContextMenu As ContextMenu
#End Region

#Region "Constructor"
    Public Sub New()
        DeleteContextMenu = New ContextMenu
        Dim cmi As New MenuItem With {.Header = "Remove from schedule"}
        AddHandler cmi.Click, AddressOf RemoveItem
        DeleteContextMenu.Items.Add(cmi)
        ContextMenu = DeleteContextMenu
    End Sub

#End Region

#Region "Public Methods"
    Private Sub RemoveItemFromStation()
        ReferencedStation.DeleteItem(Me)
        ReferencedVendor.UsedWeeklySlots -= 1

    End Sub

    Private Sub RemoveItem()
        Select Case IsBrand
            Case True
                ReferencedStation.DeleteItem(Me)
            Case False
                ReferencedLoc.DeleteItem(Me)
        End Select
        ReferencedVendor.UsedWeeklySlots -= 1

    End Sub

#End Region

#Region "Private Methods"

#End Region

End Class
