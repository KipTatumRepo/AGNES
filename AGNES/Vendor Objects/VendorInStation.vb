Public Class VendorInStation
    Inherits TextBlock

#Region "Properties"
    Public Property ReferencedVendor As ScheduleVendor
    Public Property ReferencedStation As ScheduleStation
    Public Property ReferencedLoc As ScheduleLocation
    Public Property ReferencedTruckStation As ScheduleTruckStation
    Public Property IsBrand As Boolean
    Private VendorInStationContext As ContextMenu
#End Region

#Region "Constructor"
    Public Sub New()
        VendorInStationContext = New ContextMenu
        Dim DeleteCmi As New MenuItem With {.Header = "Remove from schedule"}
        AddHandler DeleteCmi.Click, AddressOf RemoveItem
        VendorInStationContext.Items.Add(DeleteCmi)
        ContextMenu = VendorInStationContext
    End Sub

    Public Sub New(truck As Integer)
        VendorInStationContext = New ContextMenu
        Dim DeleteCmi As New MenuItem With {.Header = "Remove from schedule"}
        Dim ReceiptsCmi As New MenuItem With {.Header = "Quick Receipt Entry"}
        AddHandler DeleteCmi.Click, AddressOf RemoveItem
        AddHandler ReceiptsCmi.Click, AddressOf QuickReceiptEntry
        VendorInStationContext.Items.Add(DeleteCmi)
        VendorInStationContext.Items.Add(ReceiptsCmi)
        ContextMenu = VendorInStationContext
    End Sub
#End Region

#Region "Public Methods"

    Private Sub QuickReceiptEntry()
        Dim QRE As New SingleUserInput(False)
        Dim vnd As String = ReferencedVendor.VendorItem.Name
        Dim dte As Date = ReferencedLoc.CurrentWeekDay.DateValue
        With QRE
            .InputType = 1
            .lblInputDirection.Text = "Enter the sales for " & vnd & " on " & dte
            .txtUserInput.Focus()
            .ShowDialog()
        End With

        'CRITICAL: ADD SAVE ROUTINE TO QUICK ENTRY

        QRE.Close()
    End Sub
    Private Sub RemoveItemFromStation()
        ReferencedStation.DeleteItem(Me)
        ReferencedVendor.UsedWeeklySlots -= 1
        VendorSched.SaveStatus = False
    End Sub

    Private Sub RemoveItem()
        Select Case IsBrand
            Case True
                ReferencedStation.DeleteItem(Me)
            Case False
                ReferencedLoc.DeleteItem(Me)
        End Select
        ReferencedVendor.UsedWeeklySlots -= 1
        VendorSched.SaveStatus = False
    End Sub

#End Region

#Region "Private Methods"

#End Region

End Class
