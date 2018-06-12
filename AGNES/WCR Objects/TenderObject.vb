Public Class TenderObject
    Public Property VendorName As String
    Public TenderNames As New List(Of String)
    Public TenderQty As New List(Of Integer)
    Public TenderAmt As New List(Of Double)
    Public Sub New()
        Dim ph As String = ""
    End Sub
    Public Sub AddItem(n, q, a)
        TenderNames.Add(n)
        TenderQty.Add(q)
        TenderAmt.Add(a)
    End Sub
End Class
