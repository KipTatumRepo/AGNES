﻿Public Class VendorInStation
    Inherits TextBlock

#Region "Properties"
    Public Property ReferencedVendor As ScheduleVendor
    Public Property ReferencedLocation As ScheduleStation
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
    Private Sub RemoveItem()
        ReferencedLocation.DeleteItem(Me)
        ReferencedVendor.UsedWeeklySlots -= 1

    End Sub

#End Region

#Region "Private Methods"

#End Region

End Class
