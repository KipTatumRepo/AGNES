Public Class VendorObject
    Private _vendorname As String
    Public Property VendorName As String
        Get
            Return _vendorname
        End Get
        Set(value As String)
            _vendorname = value
            Dim q = From c In WCRE.VendorInfoes
                    Where c.VendorName = value
                    Select c
            For Each c In q
                InvoiceName = Trim(c.InvoiceName)
                VendorNumber = Trim(c.SupplierCode)
                '// CAM and KPI >>>
                CAM = FormatNumber(c.CAMAmount, 4)
                KPI = FormatNumber(c.KPIAmount, 4)
                Dim q1 = From c1 In WCRE.CAMWithholdingTypes
                         Where c1.PID = c.CAMType
                         Select c1
                For Each c1 In q1
                    CAMType = Trim(c1.WithholdingType)
                Next
                If Now() < c.CAMStart Then
                    CAMType = "None" : CAM = 0
                End If
                Dim q2 = From c2 In WCRE.KPIWithholdingTypes
                         Where c2.PID = c.KPIType
                         Select c2
                For Each c2 In q2
                    KPIType = Trim(c2.WithholdingType)
                Next
                If Now() < c.KPIStart Then
                    KPIType = "None" : KPI = 0
                End If
                '<<< CAM and KPI //
            Next
        End Set
    End Property
    Public Property InvoiceName As String
    Public Property VendorNumber As Long
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
            CAMAmt = 0 : KPIAmt = 0
            Select Case CAMType
                Case "Percentage"
                    If CAM > 0 Then CAMAmt = NetSales * CAM
                Case "Flat"
                    If CAM > 0 Then CAMAmt = CAM
            End Select

            Select Case KPIType
                Case "Percentage"
                    If KPI > 0 Then KPIAmt = NetSales * KPI
                Case "Flat"
                    If KPI > 0 Then KPIAmt = KPI
            End Select
        End Set
    End Property
    Public Property SalesTax As Double
    Public Property NetSales As Double
    Public Property CAMType As String
    Public Property CAMAmt As Double
    Public Property KPIType As String
    Public Property KPIAmt As Double
    Public Property CAM As Double
    Public Property KPI As Double
    Public Property Cash As Double
    Public Property CreditCards As Double
    Public Property VisaMastercard As Double
    Public Property FreedomPay As Double
    Public Property AMEX As Double
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
    Public Tenders As New List(Of Tender)

    Public Sub New()
        Dim ph As String = ""
    End Sub

    Public Sub AddTender(id, nm, qty, amt)
        Dim t As New Tender With {.TenderId = id, .TenderName = nm, .TenderQty = qty, .TenderAmt = amt}
        Tenders.Add(t)
        Recalculate()
    End Sub

    Public Sub PrintInvoice(ByRef pd As PrintDialog, ByRef fd As FlowDocument)
        Dim ph As String = ""
        '// Create Commons logo object
        Dim bimg As New BitmapImage
        bimg.BeginInit()
        bimg.UriSource = (New Uri("pack://application:,,,/Resources/Commons.jpg"))
        bimg.EndInit()
        Dim img As New Image With {.Source = bimg, .Stretch = Stretch.None}
        Dim buic As New BlockUIContainer(img)

        '// Header, vendor, invoice #, and date
        Dim p As New Paragraph(New Run("West Campus Commons Vendor Sales Report and Invoice")) With
            {.FontSize = 24, .TextAlignment = TextAlignment.Center, .FontWeight = FontWeights.Bold, .FontFamily = New FontFamily("Segoe UI")}

        '// Create the Table...
        Dim t As New Table() With {.CellSpacing = 0, .Background = Brushes.LemonChiffon}
        t.Columns.Add(New TableColumn() With {.Background = Brushes.White, .Width = New GridLength(180)})
        t.Columns.Add(New TableColumn() With {.Background = Brushes.White, .Width = New GridLength(200)})
        t.Columns.Add(New TableColumn() With {.Background = Brushes.White, .Width = New GridLength(100)})
        t.Columns.Add(New TableColumn() With {.Background = Brushes.White, .Width = New GridLength(280)})
        t.RowGroups.Add(New TableRowGroup())

        '// Alias the current working row for easy reference.
        Dim cr As New TableRow With {.FontSize = 8, .FontWeight = FontWeights.Normal, .FontFamily = New FontFamily("Segoe UI")} '= t.RowGroups(0).Rows(0) 'cr.Background = Brushes.Silver

        '// Add the invoice and date rows
        Dim rc As Integer
        For rc = 1 To 32
            t.RowGroups(0).Rows.Add(New TableRow())
        Next rc
        cr = t.RowGroups(0).Rows(0)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(VendorName)) With {.TextAlignment = TextAlignment.Center, .FontFamily = New FontFamily("Segoe UI"), .FontWeight = FontWeights.Bold, .FontSize = 20}))
        cr.Cells(0).ColumnSpan = 4

        cr = t.RowGroups(0).Rows(1)
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Invoice Number: ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("ABCD1234")) With {.TextAlignment = TextAlignment.Left, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Week Start Date: 6/22/2018")) With {.TextAlignment = TextAlignment.Left, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr = t.RowGroups(0).Rows(2)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr = t.RowGroups(0).Rows(3)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Net Sales:")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(NetSales, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr = t.RowGroups(0).Rows(4)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Sales Tax Due from Vendor to State:")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(SalesTax, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr = t.RowGroups(0).Rows(5)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Total Sales:")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontWeight = FontWeights.Bold, .FontSize = 12, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(0, 1, 0, 0)}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(GrossSales, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontWeight = FontWeights.Bold, .FontSize = 12, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(0, 1, 0, 0)}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr = t.RowGroups(0).Rows(6)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr = t.RowGroups(0).Rows(7)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Cash:")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(Cash, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr = t.RowGroups(0).Rows(8)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Meal Card:")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(MealCard, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr = t.RowGroups(0).Rows(9)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Meal Card Credit:")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(MealCardCredit, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr = t.RowGroups(0).Rows(10)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("eCoupons:")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(ECoupons, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr = t.RowGroups(0).Rows(11)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("eCash:")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(ECash, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr = t.RowGroups(0).Rows(12)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Credit Cards:")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(CreditCards, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr = t.RowGroups(0).Rows(13)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Scratch Coupons:")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(ScratchCoupons, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr = t.RowGroups(0).Rows(14)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Expired Cards:")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(ExpiredCard, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr = t.RowGroups(0).Rows(15)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Department Charges:")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(IOCharges, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr = t.RowGroups(0).Rows(16)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Total Tender:")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontWeight = FontWeights.Bold, .FontSize = 12, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(0, 1, 0, 0)}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(GrossSales, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontWeight = FontWeights.Bold, .FontSize = 12, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(0, 1, 0, 0)}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr = t.RowGroups(0).Rows(17)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr = t.RowGroups(0).Rows(18)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("% of Sales to Remit to Compass:")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontWeight = FontWeights.Normal, .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency((CAMAmt + KPIAmt), 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontWeight = FontWeights.Normal, .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr = t.RowGroups(0).Rows(19)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Potential KPI Earnback:")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontWeight = FontWeights.SemiBold, .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(KPIAmt, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontWeight = FontWeights.SemiBold, .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr = t.RowGroups(0).Rows(20)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr = t.RowGroups(0).Rows(21)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Meal Card Payments:")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontWeight = FontWeights.Normal, .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(MealCard, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontWeight = FontWeights.Normal, .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr = t.RowGroups(0).Rows(22)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Cash Added to Meal Cards:")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontWeight = FontWeights.Normal, .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(MealCardCredit, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontWeight = FontWeights.Normal, .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr = t.RowGroups(0).Rows(23)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("eCoupons:")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontWeight = FontWeights.Normal, .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(ECoupons, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontWeight = FontWeights.Normal, .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr = t.RowGroups(0).Rows(24)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("eCash:")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontWeight = FontWeights.Normal, .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(ECash, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontWeight = FontWeights.Normal, .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr = t.RowGroups(0).Rows(25)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Scratch Coupons:")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontWeight = FontWeights.Normal, .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(ScratchCoupons, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontWeight = FontWeights.Normal, .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr = t.RowGroups(0).Rows(26)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Expired WCC Cards:")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontWeight = FontWeights.Normal, .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(ExpiredCard, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontWeight = FontWeights.Normal, .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr = t.RowGroups(0).Rows(27)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Department Charges:")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontWeight = FontWeights.Normal, .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(IOCharges, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontWeight = FontWeights.Normal, .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr = t.RowGroups(0).Rows(28)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr = t.RowGroups(0).Rows(29)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Total Due from Compass:")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontWeight = FontWeights.SemiBold, .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(CompassPayment, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontWeight = FontWeights.SemiBold, .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr = t.RowGroups(0).Rows(30)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Total Due from " & VendorName & ":")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontWeight = FontWeights.SemiBold, .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(VendorPayment, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontWeight = FontWeights.SemiBold, .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr = t.RowGroups(0).Rows(31)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(" ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Invoice Total:")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontWeight = FontWeights.Bold, .FontSize = 12, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(0, 1, 0, 0)}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(DueFromVendor, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontWeight = FontWeights.Bold, .FontSize = 12, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(0, 1, 0, 0)}))

        With fd.Blocks
            .Add(buic)
            .Add(p)
            .Add(t)
        End With

    End Sub

    Private Sub Recalculate()
        '// Calculate Gross Sales, Tax, and Net Sales
        Dim gs As Double = 0
        CreditCards = 0
        For Each t As Tender In Tenders
            gs += t.TenderAmt
            GrossSales = gs
            'TODO: Handle suspend and all other tender-specific properties and Compass Owes/Vendor Owes/Total Owed
            'TODO: Map property association to Tender ID in table.  Hard coding for development use only
            Select Case t.TenderId
                Case 1
                    Cash = t.TenderAmt
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
                Case 2, 3, 83, 91, 92, 93, 94
                    CreditCards += t.TenderAmt
            End Select
            CompassPayment = MealCard + ECoupons + ECash + ScratchCoupons + ExpiredCard + IOCharges
            VendorPayment = MealCardCredit + CAMAmt + KPIAmt
            DueFromVendor = CompassPayment - VendorPayment
        Next
    End Sub

End Class
