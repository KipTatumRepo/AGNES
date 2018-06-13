Public Class VendorObject
    Public Property VendorName As String
    Public Tenders As New List(Of Tender)
    Public CamChecks As New List(Of CamCheck)

    Public Sub New()
        Dim ph As String = ""
    End Sub
    Public Sub AddTender(id, nm, qty, amt)
        Dim t As New Tender With {.TenderID = id, .TenderName = nm, .TenderQty = qty, .TenderAmt = amt}
        Tenders.Add(t)
    End Sub
    Public Sub AddCamCheck(Num, Amt, Dte, Nts)
        Dim c As New CamCheck With {.CheckNumber = Num, .CheckAmt = Amt, .DepositDate = Dte, .Notes = Nts}
        CamChecks.Add(c)
    End Sub
End Class
