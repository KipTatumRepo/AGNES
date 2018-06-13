Public Class VendorObject
    Public Property VendorName As String
    Public Tenders As New List(Of Tender)


    Public Sub New()
        Dim ph As String = ""
    End Sub
    Public Sub AddTender(id, nm, qty, amt)
        Dim t As New Tender With {.TenderID = id, .TenderName = nm, .TenderQty = qty, .TenderAmt = amt}
        Tenders.Add(t)
    End Sub

End Class
