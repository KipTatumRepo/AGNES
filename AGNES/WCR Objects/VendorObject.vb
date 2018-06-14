Public Class VendorObject
    Public Property VendorName As String
    Public Tenders As New List(Of Tender)
    Private _grosssales As Double
    Public Property GrossSales As Double
        Get
            Return _grosssales
        End Get
        Set(value As Double)
            _grosssales = value
            Dim st As Double = My.Settings.WASalesTax
            NetSales = _grosssales / (1 + st)
            SalesTax = value - NetSales
            If CAM > 0 Then CAMAmt = NetSales * CAM
            If KPI > 0 Then KPIAmt = NetSales * KPI
        End Set
    End Property
    Public Property SalesTax As Double
    Public Property NetSales As Double
    Public Property CAMAmt As Double
    Public Property KPIAmt As Double
    Public Property CAM As Double
    Public Property KPI As Double

    Public Sub New()
        'TODO: Add function to populate CAM and KPI values for the vendor from a table.  Hard coding for development use only
        CAM = 0.075
        KPI = 0.075
    End Sub
    Public Sub AddTender(id, nm, qty, amt)
        Dim t As New Tender With {.TenderID = id, .TenderName = nm, .TenderQty = qty, .TenderAmt = amt}
        Tenders.Add(t)
        Recalculate()
    End Sub
    Private Sub Recalculate()
        '// Calculate Gross Sales, Tax, and Net Sales
        Dim gs As Double = 0
        For Each t As Tender In Tenders
            gs += t.TenderAmt
        Next
        GrossSales = gs
    End Sub
End Class
