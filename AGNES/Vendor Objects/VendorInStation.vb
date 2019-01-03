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

        Dim vnd As String = ReferencedVendor.VendorItem.Name
        Dim dte As Date = ReferencedLoc.CurrentWeekDay.DateValue
        Dim qve = From ve In VendorData.Receipts
                  Where ve.ReceiptDate = dte And
                      ve.VendorId = ReferencedVendor.VendorItem.PID And
                      ve.Location = ReferencedLoc.LocationName
                  Select ve

        If qve.Count > 0 Then
            Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Small, AgnesMessageBox.MsgBoxLayout.TextAndImage, AgnesMessageBox.MsgBoxType.OkOnly, 10,,, "Quick Entry Prohibited", "An entry already exists.  Use the full receipt editor to modify entries.", AgnesMessageBox.ImageType.Alert)
            amsg.ShowDialog()
            amsg.Close()
            amsg = Nothing
            Exit Sub
        End If
        Dim QSE As New SingleUserInput(False)
        With QSE
            .InputType = 1
            .lblInputDirection.Text = "Enter the sales for " & vnd & " on " & dte
            .txtUserInput.Focus()
            .ShowDialog()
        End With
        QSE.Hide()

        Dim QRE As New SingleUserInput(False)
        With QRE
            .InputType = 1
            .lblInputDirection.Text = "Enter the transactions for " & vnd & " on " & dte
            .txtUserInput.Focus()
            .ShowDialog()
        End With

        Dim newvendorreceipt As New Receipt
        With newvendorreceipt
            .ReceiptDate = dte
            .VendorId = ReferencedVendor.VendorItem.PID
            .VendorType = ReferencedVendor.VendorItem.VendorType
            .Location = ReferencedLoc.LocationName
            .Sales = FormatNumber(QSE.txtUserInput.Text, 2)
            .Transactions = FormatNumber(QRE.txtUserInput.Text, 0)
            .RecordSaveDate = Now()
            .RecordSavedBy = My.Settings.UserID
        End With
        VendorData.Receipts.Add(newvendorreceipt)
        VendorData.SaveChanges()
        QRE.Close()
        QSE.Close()
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
