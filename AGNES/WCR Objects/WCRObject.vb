Imports Microsoft.Win32
Imports Microsoft.Office.Interop
Imports System.Printing
Imports System.Windows.Xps

Public Class WCRObject
    Public WeekStart As Date
    Public Author As String
    Public ShortName As String
    Public Vendors As New List(Of VendorObject)
    Public CamChecks As New List(Of CamCheck)
    Dim GrossSales As Double, SalesTax As Double, NetSales As Double, CamToCompass As Double, PotentialKpi As Double,
            MealCardPayments As Double, MealCardCredits As Double, Ecoupons As Double, Ecash As Double, ScratchCoupons As Double,
            ExpiredCards As Double, IoCharges As Double, CompassPayment As Double, VendorPayment As Double, DueFromVendors As Double,
            FreedomPay As Double, Amex As Double, VisaMcDisc As Double

    Public Sub New()
        Dim ph As String = ""
    End Sub

    Public Sub LoadTenders(ByRef disp As WCRHello)
        Dim vn As String, fd As New OpenFileDialog()
        fd.DefaultExt = ".xls"
        fd.Filter = "Excel (97-2003) documents (.xls)|*.xls"
        Dim result As Nullable(Of Boolean) = fd.ShowDialog()
        If result = True Then
            Dim filename As String = fd.FileName, BadFile As Boolean
            Dim xlApp As New Excel.Application(), wb As Excel.Workbook = xlApp.Workbooks.Open(filename)
            Dim ws As Excel.Worksheet = wb.Sheets(1), valz As String = "", ct As Integer = 1
            Do Until Left(valz, 13) = "Selected For:"
                valz = CType(ws.Cells(ct, 1), Excel.Range).Value
                ct += 1
            Loop
            vn = GetVendorNameFromString(valz)
            Dim tn As Integer, v As New VendorObject With {.VendorName = vn}
            ct += 3
            Try
                Do Until valz = "Subtotal"
                    '// Check for Suspend and Dept Charges

                    tn = CType(ws.Cells(ct, 1), Excel.Range).Value
                    Select Case tn
                        Case 15         '/ Dept Charges
                            If MsgBox("IO Charges are present in this tender.  Do you confirm that the required documentation has been received?", MsgBoxStyle.YesNo, "This tender type requires validation!") = MessageBoxResult.Yes Then
                                v.AddTender(CType(ws.Cells(ct, 1), Excel.Range).Value, CType(ws.Cells(ct, 2), Excel.Range).Value,
                            FormatNumber(CType(ws.Cells(ct, 3), Excel.Range).Value, 0), FormatNumber(CType(ws.Cells(ct, 9), Excel.Range).Value, 2))
                            Else
                                v.Tenders.Clear()
                                disp.tbHello.Text = "I've terminated the tender import for " & v.VendorName & ".  Please edit the file, if needed, and reload."
                                BadFile = True
                                Exit Do
                            End If
                        Case 20, 36, 51 '/ IOU charges, IOU credit, IOU FS
                            MsgBox("Sorry, " & MySettings.Default.UserName & ", but IOU charges and credits are no longer allowed.", MsgBoxStyle.OkOnly, "Invalid tender type found!")
                            v.Tenders.Clear()
                            disp.tbHello.Text = "I've terminated the tender import for " & v.VendorName & ".  Please edit the file, if needed, and reload."
                            BadFile = True
                            Exit Do
                        Case 37         '/ Suspend
                            MsgBox("Sorry, " & MySettings.Default.UserName & ", but Suspend charges are no longer allowed.", MsgBoxStyle.OkOnly, "Invalid tender type found!")
                            v.Tenders.Clear()
                            disp.tbHello.Text = "I've terminated the tender import for " & v.VendorName & ".  Please edit the file, if needed, and reload."
                            BadFile = True
                            Exit Do
                        Case 2, 3, 91, 93, 94       '// Visa/Mastercard/Discover
                            v.AddTender(CType(ws.Cells(ct, 1), Excel.Range).Value, "VisaMastercard", FormatNumber(CType(ws.Cells(ct, 3), Excel.Range).Value, 0), FormatNumber(CType(ws.Cells(ct, 9), Excel.Range).Value, 2))
                        Case 57                     '// Coupons (used by Lunchbox for their internal promotions)
                            MsgBox("FYI, " & MySettings.Default.UserName & ", I'm omitting the Coupon tender for " & vn & " in the amount of " & FormatCurrency(CType(ws.Cells(ct, 9), Excel.Range).Value, 2))
                        Case 83                     '// Freedompay [pass-through]
                            v.AddTender(CType(ws.Cells(ct, 1), Excel.Range).Value, "FreedomPay", FormatNumber(CType(ws.Cells(ct, 3), Excel.Range).Value, 0), FormatNumber(CType(ws.Cells(ct, 9), Excel.Range).Value, 2))
                        Case 92                     '// AMEX
                            v.AddTender(CType(ws.Cells(ct, 1), Excel.Range).Value, "AMEX", FormatNumber(CType(ws.Cells(ct, 3), Excel.Range).Value, 0), FormatNumber(CType(ws.Cells(ct, 9), Excel.Range).Value, 2))
                        Case Else
                            v.AddTender(CType(ws.Cells(ct, 1), Excel.Range).Value, CType(ws.Cells(ct, 2), Excel.Range).Value, FormatNumber(CType(ws.Cells(ct, 3), Excel.Range).Value, 0), FormatNumber(CType(ws.Cells(ct, 9), Excel.Range).Value, 2))
                    End Select
                    ct += 1
                    valz = CType(ws.Cells(ct, 1), Excel.Range).Value
                Loop
                v.Recalculate()
                Vendors.Add(v)
            Catch ex As InvalidCastException
                BadFile = True
            Catch OtherEx As Exception
                MsgBox("Encountered error " & OtherEx.Message)
                'TODO: ADD OTHER TENDER-RELATED ERROR CATCHES
            Finally
                wb.Close()
                xlApp.Quit()
                ReleaseObject(ws)
                ReleaseObject(wb)
                ReleaseObject(xlApp)
                disp.PrintVendorTotalTendersToScreen(v, BadFile)
            End Try

        End If
    End Sub

    Public Sub AddCamCheck(Vnm As String, Num As String, Amt As Double, Dte As Date, Nts As String)
        Dim c As New CamCheck With {.VendorName = Vnm, .CheckNumber = Num, .CheckAmt = Amt, .DepositDate = Dte, .Notes = Nts}
        CamChecks.Add(c)
    End Sub

    Public Sub PrintWCR()
        Dim pd As New PrintDialog
        pd.ShowDialog()
        'TODO: Add error trap for dialog box

        Dim fd As New FlowDocument With {.ColumnGap = 0, .ColumnWidth = pd.PrintableAreaWidth}

        '// Totals Sheet
        For Each v As VendorObject In Vendors
            GrossSales += v.GrossSales
            SalesTax += v.SalesTax
            NetSales += v.NetSales
            CamToCompass += v.CAMAmt
            PotentialKpi += v.KPIAmt
            MealCardPayments += v.MealCard
            MealCardCredits += v.MealCardCredit
            Ecoupons += v.ECoupons
            Ecash += v.ECash
            ScratchCoupons += v.ScratchCoupons
            ExpiredCards += v.ExpiredCard
            IoCharges += v.IOCharges
            CompassPayment += v.CompassPayment
            VendorPayment += v.VendorPayment
            DueFromVendors += v.DueFromVendor
            FreedomPay += v.FreedomPay
            Amex += v.AMEX
            VisaMcDisc += v.VisaMastercard
        Next

        CreateTotalsSection(pd, fd)
        CreateInvoiceSection(pd, fd)
        CreateSummarySection(pd, fd)

        Dim xps_writer As XpsDocumentWriter = PrintQueue.CreateXpsDocumentWriter(pd.PrintQueue)
        Dim idps As IDocumentPaginatorSource = CType(fd, IDocumentPaginatorSource)
        xps_writer.Write(idps.DocumentPaginator)

    End Sub

    Private Sub CreateTotalsSection(ByRef pd As PrintDialog, ByRef fd As FlowDocument)
        '// Header, vendor, invoice #, and date
        Dim p As New Paragraph(New Run("Totals for Week Starting " & WeekStart)) With
            {.FontSize = 24, .TextAlignment = TextAlignment.Center, .FontWeight = FontWeights.Bold, .FontFamily = New FontFamily("Segoe UI")}


        '// Create the Table...
        Dim t As New Table() With {.CellSpacing = 0, .Background = Brushes.LemonChiffon}
        t.Columns.Add(New TableColumn() With {.Background = Brushes.White, .Width = New GridLength(200)})
        t.Columns.Add(New TableColumn() With {.Background = Brushes.White, .Width = New GridLength(200)})
        t.Columns.Add(New TableColumn() With {.Background = Brushes.White, .Width = New GridLength(100)})
        t.RowGroups.Add(New TableRowGroup())

        '// Alias the current working row for easy reference.
        Dim cr As New TableRow With {.FontSize = 8, .FontWeight = FontWeights.Normal, .FontFamily = New FontFamily("Segoe UI")}

        '// Add the invoice and date rows
        Dim rc As Integer
        For rc = 1 To 17
            t.RowGroups(0).Rows.Add(New TableRow())
        Next rc

        cr = t.RowGroups(0).Rows(0)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(""))))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("         Item Detail")) With {.TextAlignment = TextAlignment.Center, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("     Total")) With {.TextAlignment = TextAlignment.Center, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))

        cr = t.RowGroups(0).Rows(1)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(""))))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))

        cr = t.RowGroups(0).Rows(2)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(""))))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Gross Sales: ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(GrossSales, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))

        cr = t.RowGroups(0).Rows(3)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(""))))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Sales Tax: ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(SalesTax, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))

        cr = t.RowGroups(0).Rows(4)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(""))))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Net Sales: ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(NetSales, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))

        cr = t.RowGroups(0).Rows(5)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(""))))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("CAM to Compass: ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(CamToCompass, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))

        cr = t.RowGroups(0).Rows(6)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(""))))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Potential KPI: ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(PotentialKpi, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))

        cr = t.RowGroups(0).Rows(7)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(""))))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Meal Card Payments: ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(MealCardPayments, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))

        cr = t.RowGroups(0).Rows(8)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(""))))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Mead Card Credits: ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(MealCardCredits, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))

        cr = t.RowGroups(0).Rows(9)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(""))))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("eCoupons: ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(Ecoupons, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))

        cr = t.RowGroups(0).Rows(10)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(""))))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("eCash: ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(Ecash, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))

        cr = t.RowGroups(0).Rows(11)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(""))))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Scratch Coupons: ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(ScratchCoupons, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))

        cr = t.RowGroups(0).Rows(12)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(""))))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Expired Cards: ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(ExpiredCards, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))

        cr = t.RowGroups(0).Rows(13)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(""))))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("IO Charges: ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(IoCharges, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))

        cr = t.RowGroups(0).Rows(14)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(""))))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Compass Payment: ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(CompassPayment, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))

        cr = t.RowGroups(0).Rows(15)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(""))))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Vendor Payment: ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(VendorPayment, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))

        '// Determine language based on who owes whom
        Dim paylang As String = ""
        If DueFromVendors < 0 Then
            paylang = "Due to Compass from Vendors: "
            DueFromVendors = -DueFromVendors
        Else
            paylang = "Due to Vendors from Compass: "
        End If
        cr = t.RowGroups(0).Rows(16)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(""))))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(paylang)) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(DueFromVendors, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))


        With fd.Blocks
            .Add(p)
            .Add(t)
            .Add(New Section())
        End With
    End Sub

    Private Sub CreateInvoiceSection(ByRef pd As PrintDialog, ByRef fd As FlowDocument)
        '// Header, vendor, invoice #, and date
        Dim p As New Paragraph(New Run("Invoices for Week Starting " & WeekStart)) With
            {.FontSize = 24, .TextAlignment = TextAlignment.Center, .FontWeight = FontWeights.Bold, .FontFamily = New FontFamily("Segoe UI")}


        '// Create the Table...
        Dim t As New Table() With {.CellSpacing = 0, .Background = Brushes.LemonChiffon}
        t.Columns.Add(New TableColumn() With {.Background = Brushes.White, .Width = New GridLength(160)})
        t.Columns.Add(New TableColumn() With {.Background = Brushes.White, .Width = New GridLength(120)})
        t.Columns.Add(New TableColumn() With {.Background = Brushes.White, .Width = New GridLength(120)})
        t.Columns.Add(New TableColumn() With {.Background = Brushes.White, .Width = New GridLength(120)})
        t.Columns.Add(New TableColumn() With {.Background = Brushes.White, .Width = New GridLength(100)})
        t.RowGroups.Add(New TableRowGroup())

        '// Alias the current working row for easy reference.
        Dim cr As New TableRow With {.FontSize = 8, .FontWeight = FontWeights.Normal, .FontFamily = New FontFamily("Segoe UI")}

        '// Add the invoice and date rows
        Dim rc As Integer
        For rc = 1 To Vendors.Count + 2
            t.RowGroups(0).Rows.Add(New TableRow())
        Next rc

        cr = t.RowGroups(0).Rows(0)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(""))))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Vendor")) With {.TextAlignment = TextAlignment.Center, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(0, 0, 0, 1)}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Vendor Code")) With {.TextAlignment = TextAlignment.Center, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(0, 0, 0, 1)}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Invoice Number")) With {.TextAlignment = TextAlignment.Center, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(0, 0, 0, 1)}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Invoice Amount")) With {.TextAlignment = TextAlignment.Center, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(0, 0, 0, 1)}))

        Dim v As VendorObject, ct As Integer = 1, invtotal As Double
        For Each v In Vendors
            cr = t.RowGroups(0).Rows(ct)
            cr.Cells.Add(New TableCell(New Paragraph(New Run(""))))
            cr.Cells.Add(New TableCell(New Paragraph(New Run(v.InvoiceName)) With {.TextAlignment = TextAlignment.Left, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(0, 0, 0, 1)}))
            cr.Cells.Add(New TableCell(New Paragraph(New Run(v.VendorNumber)) With {.TextAlignment = TextAlignment.Center, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(0, 0, 0, 1)}))
            cr.Cells.Add(New TableCell(New Paragraph(New Run(v.InvoiceNumber)) With {.TextAlignment = TextAlignment.Center, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(0, 0, 0, 1)}))
            cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(v.DueFromVendor, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(0, 0, 0, 1)}))
            invtotal += v.DueFromVendor
            ct += 1
        Next
        cr = t.RowGroups(0).Rows(ct)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(""))))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(""))))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(""))))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Total:")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(invtotal, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))

        With fd.Blocks
            .Add(p)
            .Add(t)
            .Add(New Section() With {.BreakPageBefore = True})
        End With

    End Sub

    Private Sub CreateSummarySection(ByRef pd As PrintDialog, ByRef fd As FlowDocument)
        Dim ph As String = ""
    End Sub
    Public Sub PrintInvoices()
        Dim pd As New PrintDialog
        pd.ShowDialog()


        'TODO: Add error trap for dialog box
        Dim fd As New FlowDocument With {.ColumnGap = 0, .ColumnWidth = pd.PrintableAreaWidth}
        Dim v As VendorObject, ct As Integer = Vendors.Count

        For Each v In Vendors
            ct -= 1
            v.PrintInvoice(pd, fd)
            Dim s As New Section() With {.BreakPageBefore = True}
            If ct > 0 Then fd.Blocks.Add(s)
        Next

        Dim xps_writer As XpsDocumentWriter = PrintQueue.CreateXpsDocumentWriter(pd.PrintQueue)
        Dim idps As IDocumentPaginatorSource = CType(fd, IDocumentPaginatorSource)
        xps_writer.Write(idps.DocumentPaginator)

    End Sub

    Private Function GetVendorNameFromString(st)
        Dim vn As String = st
        Dim si As Integer = vn.IndexOf("(")
        Dim li As Integer = vn.IndexOf(")")
        Dim vnum As Integer = FormatNumber(vn.Substring(si + 1, (li - si) - 1))
        Dim q = From c In WCRE.VendorInfoes
                Where c.StoreId = vnum
                Select c
        For Each c In q
            vn = Trim(c.VendorName)
        Next
        Return vn
    End Function

    Private Sub ReleaseObject(ByVal obj As Object)
        Try
            System.Runtime.InteropServices.Marshal.ReleaseComObject(obj)
            obj = Nothing
        Catch ex As Exception
            obj = Nothing
        End Try
    End Sub

End Class
