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
    Public Property MealCard As Double
    Public Property MealCardCredit As Double
    Public Property ECoupons As Double
    Public Property ECash As Double
    Public Property ScratchCoupons As Double
    Public Property ExpiredCard As Double
    Public Property IOCharges As Double
    Public Property Suspend As Double
    Public Property CompassPayment As Double
    Public Property VendorPayment As Double
    Public Property DueFromVendor As Double


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
            GrossSales = gs
            'TODO: Handle suspend and all other tender-specific properties and Compass Owes/Vendor Owes/Total Owed
            'TODO: Map property association to Tender ID in table.  Hard coding for development use only
            Select Case t.TenderID
                Case 9
                    MealCard = t.TenderAmt
                Case 10
                    MealCardCredit = t.TenderAmt
                Case 11
                    ECash = t.TenderAmt
                Case 12
                    ECoupons = t.TenderAmt
                Case 35, 55, 81
                    IOCharges = t.TenderAmt
                Case 37
                    'TODO: Deal with Suspend items
                    Suspend = t.TenderAmt
                Case 45
                    ExpiredCard = t.TenderAmt
                Case 52
                    ScratchCoupons = t.TenderAmt
            End Select
            CompassPayment = MealCard + ECoupons + ECash + ScratchCoupons + ExpiredCard + IOCharges
            VendorPayment = MealCardCredit + CAMAmt + KPIAmt
            DueFromVendor = CompassPayment - VendorPayment
        Next
    End Sub

End Class
