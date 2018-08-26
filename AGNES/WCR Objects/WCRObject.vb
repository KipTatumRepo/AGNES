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
    Public CreditArray(8, 8) As String
    Public DebitArray(8, 9) As String
    Public DepositArray(8, 3) As String
    Public InvoicesArePresent As Integer
    Dim GrossSales As Double, SalesTax As Double, NetSales As Double, CamToCompass As Double, PotentialKpi As Double,
            MealCardPayments As Double, MealCardCredits As Double, Ecoupons As Double, Ecash As Double, ScratchCoupons As Double,
            ExpiredCards As Double, IoCharges As Double, CompassPayment As Double, VendorPayment As Double, DueFromVendors As Double,
            FreedomPay As Double, Amex As Double, VisaMcDisc As Double, CreditCards As Double, InvoiceTotal As Double, ConciergeCash As Double,
            CCClear As Double, AmexClear As Double, CamCheckTotal As Double, FriCam As Double, MonCam As Double, TueCam As Double, WedCam As Double,
            ThuCam As Double, ConciergeTenderLoaded As Boolean


    Public Sub New()
        'TODO: Fetch user information
        Dim ph As String = ""
    End Sub

    Public Sub LoadTenders(ByRef disp As WCRHello, ByRef disptext As TextBlock)
        Dim vn As String, fd As New OpenFileDialog() With {.Multiselect = True}
        fd.DefaultExt = ".xls"
        fd.Filter = "Excel (97-2003) documents (.xls)|*.xls"
        Dim result As Nullable(Of Boolean) = fd.ShowDialog()
        If result = True Then
            Dim SelectedFile As String, BadFile As Byte = 0, filecount As Byte = fd.FileNames.Count, xlApp As New Excel.Application()
            For Each SelectedFile In fd.FileNames
                Dim wb As Excel.Workbook = xlApp.Workbooks.Open(SelectedFile)
                Dim ws As Excel.Worksheet = wb.Sheets(1), valz As String = "", ct As Integer = 1, badvname As Boolean = False
                Try
                    Do Until Left(valz, 13) = "Selected For:"
                        valz = CType(ws.Cells(ct, 1), Excel.Range).Value
                        ct += 1
                    Loop
                Catch ex As Exception
                    MsgBox("Error finding the vendor name in " & SelectedFile & ": " & ex.Message & ".  Operation canceled.")
                    badvname = True
                    BadFile += 1
                End Try
                If badvname = False Then
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
                                        BadFile += 1
                                        Exit Do
                                    End If
                                Case 20, 36, 51 '/ IOU charges, IOU credit, IOU FS
                                    MsgBox("Sorry, " & MySettings.Default.UserName & ", but IOU charges and credits are no longer allowed.", MsgBoxStyle.OkOnly, "Invalid tender type found!")
                                    v.Tenders.Clear()
                                    disp.tbHello.Text = "I've terminated the tender import for " & v.VendorName & ".  Please edit the file, if needed, and reload."
                                    BadFile += 1
                                    Exit Do
                                Case 37         '/ Suspend
                                    MsgBox("Sorry, " & MySettings.Default.UserName & ", but Suspend charges are no longer allowed.", MsgBoxStyle.OkOnly, "Invalid tender type found!")
                                    v.Tenders.Clear()
                                    disp.tbHello.Text = "I've terminated the tender import for " & v.VendorName & ".  Please edit the file, if needed, and reload."
                                    BadFile += 1
                                    Exit Do
                                Case 2, 3, 5, 91, 93, 94       '// Visa/Mastercard/Discover
                                    If v.VendorName <> "Concierge" Then
                                        v.AddTender(CType(ws.Cells(ct, 1), Excel.Range).Value, "VisaMastercard", FormatNumber(CType(ws.Cells(ct, 3), Excel.Range).Value, 0), FormatNumber(CType(ws.Cells(ct, 9), Excel.Range).Value, 2))
                                    Else
                                        v.AddTender(CType(ws.Cells(ct, 1), Excel.Range).Value, "CCClearing", FormatNumber(CType(ws.Cells(ct, 3), Excel.Range).Value, 0), FormatNumber(CType(ws.Cells(ct, 9), Excel.Range).Value, 2))
                                    End If
                                Case 57                     '// Coupons (used by Lunchbox for their internal promotions)
                                    MsgBox("FYI, " & MySettings.Default.UserName & ", I'm omitting the Coupon tender for " & vn & " in the amount of " & FormatCurrency(CType(ws.Cells(ct, 9), Excel.Range).Value, 2))
                                Case 83                     '// Freedompay [pass-through]
                                    v.AddTender(CType(ws.Cells(ct, 1), Excel.Range).Value, "FreedomPay", FormatNumber(CType(ws.Cells(ct, 3), Excel.Range).Value, 0), FormatNumber(CType(ws.Cells(ct, 9), Excel.Range).Value, 2))
                                Case 4, 92                     '// AMEX
                                    If v.VendorName <> "Concierge" Then
                                        v.AddTender(CType(ws.Cells(ct, 1), Excel.Range).Value, "AMEX", FormatNumber(CType(ws.Cells(ct, 3), Excel.Range).Value, 0), FormatNumber(CType(ws.Cells(ct, 9), Excel.Range).Value, 2))
                                    Else
                                        v.AddTender(CType(ws.Cells(ct, 1), Excel.Range).Value, "AMEXClearing", FormatNumber(CType(ws.Cells(ct, 3), Excel.Range).Value, 0), FormatNumber(CType(ws.Cells(ct, 9), Excel.Range).Value, 2))
                                    End If
                                Case Else
                                    v.AddTender(CType(ws.Cells(ct, 1), Excel.Range).Value, CType(ws.Cells(ct, 2), Excel.Range).Value, FormatNumber(CType(ws.Cells(ct, 3), Excel.Range).Value, 0), FormatNumber(CType(ws.Cells(ct, 9), Excel.Range).Value, 2))
                            End Select
                            ct += 1
                            valz = CType(ws.Cells(ct, 1), Excel.Range).Value
                        Loop
                        v.Recalculate()
                        Dim ttl As Double = 0
                        For Each t In v.Tenders
                            ttl += t.TenderAmt
                        Next
                        Dim amsg As New AgnesMessageBox With {.MsgSize = 2, .MsgType = 1, .TextStyle = 0}
                        'TODO: CRITICAL - msgbox object not releasing; memory leak results
                        With amsg
                            .tbTopSection.Text = "Validation!"
                            .tbBottomSection.Text = "It looks like " & v.VendorName & "" & " has a total of " & FormatCurrency(ttl, 2) & ".  Is this correct?"
                        End With
                        amsg.ShowDialog()
                        If amsg.ReturnResult = "Yes" Then

                            'Dim ask As MsgBoxResult = MsgBox("Close AGNES?", MsgBoxStyle.YesNo)
                            'If ask = MsgBoxResult.Yes Then

                            'Dim am As MsgBoxResult = MsgBox("It looks like " & v.VendorName & "" & " has a total of " & FormatCurrency(ttl, 2) & ".  Is this correct?", MsgBoxStyle.YesNo, "Validation!")
                            'If am = MessageBoxResult.Yes Then
                            Vendors.Add(v)
                        Else
                            Dim amsg1 = New AgnesMessageBox With {.MsgSize = 0, .MsgType = 3, .TextStyle = 0}
                            With amsg1
                                .tbTopSection.Text = "Total incorrect"
                                .tbBottomSection.Text = "Vendor not added.  Please try to add again after resolving discrepancy."
                            End With
                            amsg1.ShowDialog()
                            amsg1.Close()
                            BadFile += 1
                        End If
                        amsg.Close()
                    Catch ex As InvalidCastException
                        BadFile += 1
                    Catch OtherEx As Exception
                        Dim amsg = New AgnesMessageBox With {.MsgSize = 0, .MsgType = 3, .TextStyle = 0}
                        With amsg
                            .tbTopSection.Text = "Error encountered"
                            .tbBottomSection.Text = OtherEx.Message
                            .AllowCopy = True
                        End With
                        amsg.ShowDialog()
                        amsg.Close()
                        BadFile += 1
                        'TODO: ADD OTHER TENDER-RELATED ERROR CATCHES
                    End Try
                End If
                Try
                    wb.Close()
                    ReleaseObject(ws)
                    ReleaseObject(wb)
                Catch ex As Exception

                End Try
            Next
            xlApp.Quit()
            ReleaseObject(xlApp)
            GC.Collect()
            disp.TenderLoadComplete(filecount, BadFile)
        End If
    End Sub

    Public Sub AddCamCheck(Vnm As String, Num As String, Amt As Double, Dte As Date, Dow As Byte, Nts As String)
        Dim c As New CamCheck With {.VendorName = Vnm, .CheckNumber = Num, .CheckAmt = Amt, .DepositDate = Dte, .DayofWeek = Dow, .Notes = Nts}
        CamChecks.Add(c)
    End Sub

    Public Sub PrintWCR(ByRef disp As WCRFinal)
        Dim pd As New PrintDialog
        pd.ShowDialog()
        'TODO: Add error trap for dialog box

        Dim fd As New FlowDocument With {.ColumnGap = 0, .ColumnWidth = pd.PrintableAreaWidth}

        '// Totals Sheet
        For Each v As VendorObject In Vendors
            GrossSales += v.GrossSales
            SalesTax += v.SalesTax
            NetSales += v.NetSales
            If v.VendorName = "Concierge" Then ConciergeCash += v.Cash
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
            CCClear += v.CCClear
            AmexClear += v.AmexClear
        Next
        CreditCards = FreedomPay + Amex + VisaMcDisc

        CreateTotalsSection(pd, fd)
        CreateInvoiceSection(pd, fd)
        CreateCreditSummarySection(pd, fd)
        CreateDebitSummarySection(pd, fd)
        CreateDepositSummarySection(pd, fd)

        Dim balanced As Double = WCRInBalance()
        If balanced = 0 Then
            disp.InBalance = True
        Else
            'TODO: ADD APPLICATION STYLE MESSAGEBOX
            Dim yn As MsgBoxResult = MsgBox("The WCR is out of balance in the amount of " & FormatCurrency(balanced, 2) & ".  Do you wish to continue?", vbYesNo)
            If yn = vbNo Then
                disp.CancelDueToBalanceIssue = True
                disp.InBalance = False
                Exit Sub
            End If
        End If

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
        For rc = 1 To 20
            t.RowGroups(0).Rows.Add(New TableRow())
        Next rc

        cr = t.RowGroups(0).Rows(0)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(""))))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("         Item Detail")) With {.TextAlignment = TextAlignment.Center, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .FontWeight = FontWeights.Bold, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(0, 0, 0, 1)}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("     Total")) With {.TextAlignment = TextAlignment.Center, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .FontWeight = FontWeights.Bold, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(0, 0, 0, 1)}))

        cr = t.RowGroups(0).Rows(1)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(""))))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Gross Sales: ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(GrossSales, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))

        cr = t.RowGroups(0).Rows(2)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(""))))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Sales Tax: ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(SalesTax, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))

        cr = t.RowGroups(0).Rows(3)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(""))))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Net Sales: ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(NetSales, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))

        cr = t.RowGroups(0).Rows(4)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(""))))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("CAM to Compass: ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(CamToCompass, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))

        cr = t.RowGroups(0).Rows(5)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(""))))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Potential KPI: ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(PotentialKpi, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))

        cr = t.RowGroups(0).Rows(6)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(""))))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Meal Card Payments: ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(MealCardPayments, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))

        cr = t.RowGroups(0).Rows(7)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(""))))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Mead Card Credits: ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(MealCardCredits, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))

        cr = t.RowGroups(0).Rows(8)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(""))))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("eCoupons: ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(Ecoupons, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))

        cr = t.RowGroups(0).Rows(9)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(""))))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("eCash: ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(Ecash, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))

        cr = t.RowGroups(0).Rows(10)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(""))))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Scratch Coupons: ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(ScratchCoupons, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))

        cr = t.RowGroups(0).Rows(11)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(""))))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Expired Cards: ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(ExpiredCards, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))

        cr = t.RowGroups(0).Rows(12)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(""))))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("IO Charges: ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(IoCharges, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))

        cr = t.RowGroups(0).Rows(13)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(""))))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Compass Payment: ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(CompassPayment, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))

        cr = t.RowGroups(0).Rows(14)
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
        cr = t.RowGroups(0).Rows(15)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(""))))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(paylang)) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(DueFromVendors, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))

        cr = t.RowGroups(0).Rows(16)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(""))))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Credit Cards: ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(CreditCards, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))

        cr = t.RowGroups(0).Rows(17)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(""))))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Freedom Pay: ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(FreedomPay, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))

        cr = t.RowGroups(0).Rows(18)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(""))))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Visa/MC/Disc: ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency((VisaMcDisc + CCClear), 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))

        cr = t.RowGroups(0).Rows(19)
        cr.Cells.Add(New TableCell(New Paragraph(New Run(""))))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("AMEX: ")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency((Amex + AmexClear), 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))


        With fd.Blocks
            .Add(p)
            .Add(t)
            .Add(New Section())
        End With
    End Sub

    Private Sub CreateInvoiceSection(ByRef pd As PrintDialog, ByRef fd As FlowDocument)
        If InvoicesArePresent > 0 Then

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
            Dim rc As Integer, invcnt As Byte
            invcnt = Vendors.Count + 2
            If ConciergeTenderLoaded = True Then invcnt -= 1

            For rc = 1 To invcnt
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
                If v.VendorName <> "Concierge" Then
                    cr = t.RowGroups(0).Rows(ct)
                    cr.Cells.Add(New TableCell(New Paragraph(New Run(""))))
                    cr.Cells.Add(New TableCell(New Paragraph(New Run(v.InvoiceName)) With {.TextAlignment = TextAlignment.Left, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(0, 0, 0, 1)}))
                    cr.Cells.Add(New TableCell(New Paragraph(New Run(v.VendorNumber)) With {.TextAlignment = TextAlignment.Center, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(0, 0, 0, 1)}))
                    cr.Cells.Add(New TableCell(New Paragraph(New Run(v.InvoiceNumber)) With {.TextAlignment = TextAlignment.Center, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(0, 0, 0, 1)}))
                    cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(v.DueFromVendor, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(0, 0, 0, 1)}))
                    invtotal += v.DueFromVendor
                    ct += 1
                End If
            Next
            cr = t.RowGroups(0).Rows(ct)
            cr.Cells.Add(New TableCell(New Paragraph(New Run(""))))
            cr.Cells.Add(New TableCell(New Paragraph(New Run(""))))
            cr.Cells.Add(New TableCell(New Paragraph(New Run(""))))
            cr.Cells.Add(New TableCell(New Paragraph(New Run("Total:")) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
            cr.Cells.Add(New TableCell(New Paragraph(New Run(FormatCurrency(invtotal, 2))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12}))
            InvoiceTotal = invtotal
            With fd.Blocks
                .Add(p)
                .Add(t)
            End With
        End If
        fd.Blocks.Add(New Section() With {.BreakPageBefore = True})

    End Sub

    Private Sub CreateCreditSummarySection(ByRef pd As PrintDialog, ByRef fd As FlowDocument)
        '// Header
        Dim p As New Paragraph(New Run("WCR Summary for Week Starting " & WeekStart)) With
            {.FontSize = 24, .TextAlignment = TextAlignment.Center, .FontWeight = FontWeights.Bold, .FontFamily = New FontFamily("Segoe UI")}

        '// Create the Table...
        Dim t As New Table() With {.CellSpacing = 0, .Background = Brushes.LemonChiffon}
        t.Columns.Add(New TableColumn() With {.Background = Brushes.White, .Width = New GridLength(80)})
        t.Columns.Add(New TableColumn() With {.Background = Brushes.White, .Width = New GridLength(140)})
        t.Columns.Add(New TableColumn() With {.Background = Brushes.White, .Width = New GridLength(80)})
        t.Columns.Add(New TableColumn() With {.Background = Brushes.White, .Width = New GridLength(80)})
        t.Columns.Add(New TableColumn() With {.Background = Brushes.White, .Width = New GridLength(80)})
        t.Columns.Add(New TableColumn() With {.Background = Brushes.White, .Width = New GridLength(80)})
        t.Columns.Add(New TableColumn() With {.Background = Brushes.White, .Width = New GridLength(80)})
        t.Columns.Add(New TableColumn() With {.Background = Brushes.White, .Width = New GridLength(100)})
        t.RowGroups.Add(New TableRowGroup())

        '// Alias the current working row for easy reference.
        Dim cr As New TableRow With {.FontSize = 8, .FontWeight = FontWeights.Normal, .FontFamily = New FontFamily("Segoe UI")}

        '// Add the credit rows and column headers
        Dim rc As Integer
        For rc = 1 To 5
            t.RowGroups(0).Rows.Add(New TableRow())
        Next rc

        cr = t.RowGroups(0).Rows(0)
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Account")) With {.TextAlignment = TextAlignment.Center, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .FontWeight = FontWeights.Bold, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(0, 0, 0, 1)}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Credit Description")) With {.TextAlignment = TextAlignment.Center, .FontFamily = New FontFamily("Segoe UI"), .FontWeight = FontWeights.Bold, .FontSize = 12, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 0, 0, 1)}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Fri")) With {.TextAlignment = TextAlignment.Center, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .FontWeight = FontWeights.Bold, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 0, 0, 1)}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Mon")) With {.TextAlignment = TextAlignment.Center, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .FontWeight = FontWeights.Bold, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 0, 0, 1)}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Tue")) With {.TextAlignment = TextAlignment.Center, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .FontWeight = FontWeights.Bold, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 0, 0, 1)}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Wed")) With {.TextAlignment = TextAlignment.Center, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .FontWeight = FontWeights.Bold, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 0, 0, 1)}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Thu")) With {.TextAlignment = TextAlignment.Center, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .FontWeight = FontWeights.Bold, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 0, 0, 1)}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Total")) With {.TextAlignment = TextAlignment.Center, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .FontWeight = FontWeights.Bold, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 0, 1, 1)}))

        PopulateCreditArray()
        For rc = 0 To 3
            cr = t.RowGroups(0).Rows(rc + 1)
            cr.Cells.Add(New TableCell(New Paragraph(New Run(CreditArray(0, rc))) With {.TextAlignment = TextAlignment.Center, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(0, 0, 0, 1)}))
            cr.Cells.Add(New TableCell(New Paragraph(New Run(CreditArray(1, rc))) With {.TextAlignment = TextAlignment.Left, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 0, 0, 1)}))
            cr.Cells.Add(New TableCell(New Paragraph(New Run(CreditArray(2, rc))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 0, 0, 1)}))
            cr.Cells.Add(New TableCell(New Paragraph(New Run(CreditArray(3, rc))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 0, 0, 1)}))
            cr.Cells.Add(New TableCell(New Paragraph(New Run(CreditArray(4, rc))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 0, 0, 1)}))
            cr.Cells.Add(New TableCell(New Paragraph(New Run(CreditArray(5, rc))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 0, 0, 1)}))
            cr.Cells.Add(New TableCell(New Paragraph(New Run(CreditArray(6, rc))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 0, 0, 1)}))
            cr.Cells.Add(New TableCell(New Paragraph(New Run(CreditArray(7, rc))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 0, 1, 1)}))
        Next

        With fd.Blocks
            .Add(p)
            .Add(t)
            .Add(New Section())
        End With

    End Sub

    Private Sub CreateDebitSummarySection(ByRef pd As PrintDialog, ByRef fd As FlowDocument)
        '// Header
        Dim p As New Paragraph(New Run(""))

        '// Create the Table...
        Dim t As New Table() With {.CellSpacing = 0, .Background = Brushes.LemonChiffon}
        t.Columns.Add(New TableColumn() With {.Background = Brushes.White, .Width = New GridLength(80)})
        t.Columns.Add(New TableColumn() With {.Background = Brushes.White, .Width = New GridLength(140)})
        t.Columns.Add(New TableColumn() With {.Background = Brushes.White, .Width = New GridLength(80)})
        t.Columns.Add(New TableColumn() With {.Background = Brushes.White, .Width = New GridLength(80)})
        t.Columns.Add(New TableColumn() With {.Background = Brushes.White, .Width = New GridLength(80)})
        t.Columns.Add(New TableColumn() With {.Background = Brushes.White, .Width = New GridLength(80)})
        t.Columns.Add(New TableColumn() With {.Background = Brushes.White, .Width = New GridLength(80)})
        t.Columns.Add(New TableColumn() With {.Background = Brushes.White, .Width = New GridLength(100)})
        t.RowGroups.Add(New TableRowGroup())

        '// Alias the current working row for easy reference.
        Dim cr As New TableRow With {.FontSize = 8, .FontWeight = FontWeights.Normal, .FontFamily = New FontFamily("Segoe UI")}

        '// Add the credit rows and column headers
        Dim rc As Integer
        For rc = 1 To 9
            t.RowGroups(0).Rows.Add(New TableRow())
        Next rc

        cr = t.RowGroups(0).Rows(0)
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Account")) With {.TextAlignment = TextAlignment.Center, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .FontWeight = FontWeights.Bold, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(0, 0, 0, 1)}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Debit Description")) With {.TextAlignment = TextAlignment.Center, .FontFamily = New FontFamily("Segoe UI"), .FontWeight = FontWeights.Bold, .FontSize = 12, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 0, 0, 1)}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Fri")) With {.TextAlignment = TextAlignment.Center, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .FontWeight = FontWeights.Bold, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 0, 0, 1)}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Mon")) With {.TextAlignment = TextAlignment.Center, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .FontWeight = FontWeights.Bold, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 0, 0, 1)}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Tue")) With {.TextAlignment = TextAlignment.Center, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .FontWeight = FontWeights.Bold, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 0, 0, 1)}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Wed")) With {.TextAlignment = TextAlignment.Center, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .FontWeight = FontWeights.Bold, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 0, 0, 1)}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Thu")) With {.TextAlignment = TextAlignment.Center, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .FontWeight = FontWeights.Bold, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 0, 0, 1)}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Total")) With {.TextAlignment = TextAlignment.Center, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .FontWeight = FontWeights.Bold, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 0, 1, 1)}))

        PopulateDebitArray()
        For rc = 0 To 7
            cr = t.RowGroups(0).Rows(rc + 1)
            cr.Cells.Add(New TableCell(New Paragraph(New Run(DebitArray(0, rc))) With {.TextAlignment = TextAlignment.Center, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(0, 0, 0, 1)}))
            cr.Cells.Add(New TableCell(New Paragraph(New Run(DebitArray(1, rc))) With {.TextAlignment = TextAlignment.Left, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 0, 0, 1)}))
            cr.Cells.Add(New TableCell(New Paragraph(New Run(DebitArray(2, rc))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 0, 0, 1)}))
            cr.Cells.Add(New TableCell(New Paragraph(New Run(DebitArray(3, rc))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 0, 0, 1)}))
            cr.Cells.Add(New TableCell(New Paragraph(New Run(DebitArray(4, rc))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 0, 0, 1)}))
            cr.Cells.Add(New TableCell(New Paragraph(New Run(DebitArray(5, rc))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 0, 0, 1)}))
            cr.Cells.Add(New TableCell(New Paragraph(New Run(DebitArray(6, rc))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 0, 0, 1)}))
            cr.Cells.Add(New TableCell(New Paragraph(New Run(DebitArray(7, rc))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 0, 1, 1)}))
        Next

        With fd.Blocks
            .Add(p)
            .Add(t)
            .Add(New Section())
        End With
    End Sub

    Private Sub CreateDepositSummarySection(ByRef pd As PrintDialog, ByRef fd As FlowDocument)
        '// Header
        Dim p As New Paragraph(New Run(""))

        '// Create the Table...
        Dim t As New Table() With {.CellSpacing = 0, .Background = Brushes.LemonChiffon}
        t.Columns.Add(New TableColumn() With {.Background = Brushes.White, .Width = New GridLength(80)})
        t.Columns.Add(New TableColumn() With {.Background = Brushes.White, .Width = New GridLength(140)})
        t.Columns.Add(New TableColumn() With {.Background = Brushes.White, .Width = New GridLength(80)})
        t.Columns.Add(New TableColumn() With {.Background = Brushes.White, .Width = New GridLength(80)})
        t.Columns.Add(New TableColumn() With {.Background = Brushes.White, .Width = New GridLength(80)})
        t.Columns.Add(New TableColumn() With {.Background = Brushes.White, .Width = New GridLength(80)})
        t.Columns.Add(New TableColumn() With {.Background = Brushes.White, .Width = New GridLength(80)})
        t.Columns.Add(New TableColumn() With {.Background = Brushes.White, .Width = New GridLength(100)})
        t.RowGroups.Add(New TableRowGroup())

        '// Alias the current working row for easy reference.
        Dim cr As New TableRow With {.FontSize = 8, .FontWeight = FontWeights.Normal, .FontFamily = New FontFamily("Segoe UI")}

        '// Add the deposit rows and column headers
        Dim rc As Integer
        For rc = 1 To 4
            t.RowGroups(0).Rows.Add(New TableRow())
        Next rc

        cr = t.RowGroups(0).Rows(0)
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Account")) With {.TextAlignment = TextAlignment.Center, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .FontWeight = FontWeights.Bold, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(0, 0, 0, 1)}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Deposit Description")) With {.TextAlignment = TextAlignment.Center, .FontFamily = New FontFamily("Segoe UI"), .FontWeight = FontWeights.Bold, .FontSize = 12, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 0, 0, 1)}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Fri")) With {.TextAlignment = TextAlignment.Center, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .FontWeight = FontWeights.Bold, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 0, 0, 1)}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Mon")) With {.TextAlignment = TextAlignment.Center, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .FontWeight = FontWeights.Bold, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 0, 0, 1)}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Tue")) With {.TextAlignment = TextAlignment.Center, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .FontWeight = FontWeights.Bold, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 0, 0, 1)}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Wed")) With {.TextAlignment = TextAlignment.Center, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .FontWeight = FontWeights.Bold, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 0, 0, 1)}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Thu")) With {.TextAlignment = TextAlignment.Center, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .FontWeight = FontWeights.Bold, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 0, 0, 1)}))
        cr.Cells.Add(New TableCell(New Paragraph(New Run("Total")) With {.TextAlignment = TextAlignment.Center, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .FontWeight = FontWeights.Bold, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 0, 1, 1)}))

        PopulateDepositArray()
        For rc = 0 To 2
            cr = t.RowGroups(0).Rows(rc + 1)
            cr.Cells.Add(New TableCell(New Paragraph(New Run(DepositArray(0, rc))) With {.TextAlignment = TextAlignment.Center, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(0, 0, 0, 1)}))
            cr.Cells.Add(New TableCell(New Paragraph(New Run(DepositArray(1, rc))) With {.TextAlignment = TextAlignment.Left, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 0, 0, 1)}))
            cr.Cells.Add(New TableCell(New Paragraph(New Run(DepositArray(2, rc))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 0, 0, 1)}))
            cr.Cells.Add(New TableCell(New Paragraph(New Run(DepositArray(3, rc))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 0, 0, 1)}))
            cr.Cells.Add(New TableCell(New Paragraph(New Run(DepositArray(4, rc))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 0, 0, 1)}))
            cr.Cells.Add(New TableCell(New Paragraph(New Run(DepositArray(5, rc))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 0, 0, 1)}))
            cr.Cells.Add(New TableCell(New Paragraph(New Run(DepositArray(6, rc))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 0, 0, 1)}))
            cr.Cells.Add(New TableCell(New Paragraph(New Run(DepositArray(7, rc))) With {.TextAlignment = TextAlignment.Right, .FontFamily = New FontFamily("Segoe UI"), .FontSize = 12, .BorderBrush = Brushes.Black, .BorderThickness = New Thickness(1, 0, 1, 1)}))
        Next

        With fd.Blocks
            .Add(p)
            .Add(t)
            .Add(New Section())
            'TODO: CHANGE AUTHORSHIP TO FULL SYSTEM NAME LATER
            .Add(New Paragraph(New Run("Prepared " & Now() & " by " & Environment.UserName)))
        End With
    End Sub

    Private Sub PopulateCreditArray()
        '// Loop through CAM checks, add to specific weekday total, and tally sum total of checks
        For Each cc As CamCheck In CamChecks
            Select Case cc.DayofWeek
                Case 1  'Friday
                    FriCam = cc.CheckAmt
                    CamCheckTotal += cc.CheckAmt
                Case 4  'Monday
                    MonCam = cc.CheckAmt
                    CamCheckTotal += cc.CheckAmt
                Case 5  'Tuesday
                    TueCam = cc.CheckAmt
                    CamCheckTotal += cc.CheckAmt
                Case 6  'Wednesday
                    WedCam = cc.CheckAmt
                    CamCheckTotal += cc.CheckAmt
                Case 7  'Thursday
                    ThuCam = cc.CheckAmt
                    CamCheckTotal += cc.CheckAmt
            End Select
        Next

        Dim CamTotal As Double = 0
        CreditArray(0, 0) = "353008"
        CreditArray(1, 0) = "CAM Revenue"
        CreditArray(2, 0) = FormatCurrency(FriCam, 2)
        CreditArray(3, 0) = FormatCurrency(MonCam, 2)
        CreditArray(4, 0) = FormatCurrency(TueCam, 2)
        CreditArray(5, 0) = FormatCurrency(WedCam, 2)
        CreditArray(6, 0) = FormatCurrency(CamToCompass + PotentialKpi + ThuCam, 2)
        CamTotal = (CamToCompass + PotentialKpi + CamCheckTotal)
        CreditArray(7, 0) = FormatCurrency(CamTotal, 2)

        CreditArray(0, 1) = "212910"
        CreditArray(1, 1) = "KPI Hold"
        CreditArray(2, 1) = FormatCurrency(0, 2)
        CreditArray(3, 1) = FormatCurrency(0, 2)
        CreditArray(4, 1) = FormatCurrency(0, 2)
        CreditArray(5, 1) = FormatCurrency(0, 2)
        CreditArray(6, 1) = FormatCurrency(PotentialKpi, 2)
        CreditArray(7, 1) = FormatCurrency(PotentialKpi, 2)

        Dim owed As Double = 0
        If InvoiceTotal > 0 Then owed = InvoiceTotal
        CreditArray(0, 2) = "411007"
        CreditArray(1, 2) = "Net Owed to Vendor"
        CreditArray(2, 2) = FormatCurrency(0, 2)
        CreditArray(3, 2) = FormatCurrency(0, 2)
        CreditArray(4, 2) = FormatCurrency(0, 2)
        CreditArray(5, 2) = FormatCurrency(0, 2)
        CreditArray(6, 2) = FormatCurrency(owed, 2)
        CreditArray(7, 2) = FormatCurrency(owed, 2)

        CreditArray(0, 3) = "219301"
        CreditArray(1, 3) = "Mealcard Deposit"
        CreditArray(2, 3) = FormatCurrency(0, 2)
        CreditArray(3, 3) = FormatCurrency(0, 2)
        CreditArray(4, 3) = FormatCurrency(0, 2)
        CreditArray(5, 3) = FormatCurrency(0, 2)
        CreditArray(6, 3) = FormatCurrency(-MealCardCredits, 2)
        CreditArray(7, 3) = FormatCurrency(-MealCardCredits, 2)

    End Sub

    Private Sub PopulateDebitArray()
        DebitArray(0, 0) = "219301"
        DebitArray(1, 0) = "Mealcard Usage"
        DebitArray(2, 0) = FormatCurrency(0, 2)
        DebitArray(3, 0) = FormatCurrency(0, 2)
        DebitArray(4, 0) = FormatCurrency(0, 2)
        DebitArray(5, 0) = FormatCurrency(0, 2)
        DebitArray(6, 0) = FormatCurrency(MealCardPayments, 2)
        DebitArray(7, 0) = FormatCurrency(MealCardPayments, 2)

        DebitArray(0, 1) = "693100"
        DebitArray(1, 1) = "KPI Hold"
        DebitArray(2, 1) = FormatCurrency(0, 2)
        DebitArray(3, 1) = FormatCurrency(0, 2)
        DebitArray(4, 1) = FormatCurrency(0, 2)
        DebitArray(5, 1) = FormatCurrency(0, 2)
        DebitArray(6, 1) = FormatCurrency(PotentialKpi, 2)
        DebitArray(7, 1) = FormatCurrency(PotentialKpi, 2)

        Dim Owed As Double = 0
        If InvoiceTotal < 0 Then Owed = -InvoiceTotal
        DebitArray(0, 2) = "411007"
        DebitArray(1, 2) = "Net Owed to Compass"
        DebitArray(2, 2) = FormatCurrency(0, 2)
        DebitArray(3, 2) = FormatCurrency(0, 2)
        DebitArray(4, 2) = FormatCurrency(0, 2)
        DebitArray(5, 2) = FormatCurrency(0, 2)
        DebitArray(6, 2) = FormatCurrency(Owed, 2)
        DebitArray(7, 2) = FormatCurrency(Owed, 2)

        DebitArray(0, 3) = "219927"
        DebitArray(1, 3) = "eCash"
        DebitArray(2, 3) = FormatCurrency(0, 2)
        DebitArray(3, 3) = FormatCurrency(0, 2)
        DebitArray(4, 3) = FormatCurrency(0, 2)
        DebitArray(5, 3) = FormatCurrency(0, 2)
        DebitArray(6, 3) = FormatCurrency(Ecash, 2)
        DebitArray(7, 3) = FormatCurrency(Ecash, 2)

        DebitArray(0, 4) = "219927"
        DebitArray(1, 4) = "eCoupons"
        DebitArray(2, 4) = FormatCurrency(0, 2)
        DebitArray(3, 4) = FormatCurrency(0, 2)
        DebitArray(4, 4) = FormatCurrency(0, 2)
        DebitArray(5, 4) = FormatCurrency(0, 2)
        DebitArray(6, 4) = FormatCurrency(Ecoupons, 2)
        DebitArray(7, 4) = FormatCurrency(Ecoupons, 2)

        DebitArray(0, 5) = "11295"
        DebitArray(1, 5) = "IO Billing"
        DebitArray(2, 5) = FormatCurrency(0, 2)
        DebitArray(3, 5) = FormatCurrency(0, 2)
        DebitArray(4, 5) = FormatCurrency(0, 2)
        DebitArray(5, 5) = FormatCurrency(0, 2)
        DebitArray(6, 5) = FormatCurrency(IoCharges, 2)
        DebitArray(7, 5) = FormatCurrency(IoCharges, 2)

        DebitArray(0, 6) = "621000"
        DebitArray(1, 6) = "Marketing Discounts"
        DebitArray(2, 6) = FormatCurrency(0, 2)
        DebitArray(3, 6) = FormatCurrency(0, 2)
        DebitArray(4, 6) = FormatCurrency(0, 2)
        DebitArray(5, 6) = FormatCurrency(0, 2)
        DebitArray(6, 6) = FormatCurrency(ScratchCoupons, 2)
        DebitArray(7, 6) = FormatCurrency(ScratchCoupons, 2)

        DebitArray(0, 7) = "681020"
        DebitArray(1, 7) = "Expired Cards"
        DebitArray(2, 7) = FormatCurrency(0, 2)
        DebitArray(3, 7) = FormatCurrency(0, 2)
        DebitArray(4, 7) = FormatCurrency(0, 2)
        DebitArray(5, 7) = FormatCurrency(0, 2)
        DebitArray(6, 7) = FormatCurrency(ExpiredCards, 2)
        DebitArray(7, 7) = FormatCurrency(ExpiredCards, 2)


    End Sub

    Private Sub PopulateDepositArray()

        DepositArray(0, 0) = "105200"
        DepositArray(1, 0) = "Depository Cash"
        DepositArray(2, 0) = FormatCurrency(FriCam, 2)
        DepositArray(3, 0) = FormatCurrency(MonCam, 2)
        DepositArray(4, 0) = FormatCurrency(TueCam, 2)
        DepositArray(5, 0) = FormatCurrency(WedCam, 2)
        DepositArray(6, 0) = FormatCurrency(ConciergeCash + ThuCam, 2)
        DepositArray(7, 0) = FormatCurrency(ConciergeCash + CamCheckTotal, 2)

        DepositArray(0, 1) = "112265"
        DepositArray(1, 1) = "Credit Card Clearing"
        DepositArray(2, 1) = FormatCurrency(0, 2)
        DepositArray(3, 1) = FormatCurrency(0, 2)
        DepositArray(4, 1) = FormatCurrency(0, 2)
        DepositArray(5, 1) = FormatCurrency(0, 2)
        DepositArray(6, 1) = FormatCurrency(CCClear, 2)
        DepositArray(7, 1) = FormatCurrency(CCClear, 2)

        DepositArray(0, 2) = "112266"
        DepositArray(1, 2) = "AMEX Clearing"
        DepositArray(2, 2) = FormatCurrency(0, 2)
        DepositArray(3, 2) = FormatCurrency(0, 2)
        DepositArray(4, 2) = FormatCurrency(0, 2)
        DepositArray(5, 2) = FormatCurrency(0, 2)
        DepositArray(6, 2) = FormatCurrency(AmexClear, 2)
        DepositArray(7, 2) = FormatCurrency(AmexClear, 2)
    End Sub

    Public Sub PrintInvoices()
        Dim pd As New PrintDialog
        pd.ShowDialog()

        'TODO: Add error trap for dialog box
        Dim fd As New FlowDocument With {.ColumnGap = 0, .ColumnWidth = pd.PrintableAreaWidth}
        Dim v As VendorObject, ct As Integer = Vendors.Count
        InvoicesArePresent = 0
        For Each v In Vendors
            Dim s As New Section() With {.BreakPageBefore = True}
            ct -= 1
            If v.VendorName <> "Concierge" Then
                v.PrintInvoice(pd, fd)
                InvoicesArePresent += 1
            End If
            If ct > 0 Then fd.Blocks.Add(s)
        Next

        If InvoicesArePresent > 0 Then
            Dim xps_writer As XpsDocumentWriter = PrintQueue.CreateXpsDocumentWriter(pd.PrintQueue)
            Dim idps As IDocumentPaginatorSource = CType(fd, IDocumentPaginatorSource)
            xps_writer.Write(idps.DocumentPaginator)
        End If
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

    Private Function WCRInBalance() As Double
        Dim creditsection As Double, debitanddepositsection As Double
        creditsection = CamToCompass + PotentialKpi + PotentialKpi - MealCardCredits + CamCheckTotal
        If InvoiceTotal > 0 Then creditsection += InvoiceTotal
        debitanddepositsection = MealCardPayments + PotentialKpi + Ecoupons + Ecash + IoCharges + ScratchCoupons + ExpiredCards + ConciergeCash + CCClear + AmexClear + CamCheckTotal
        If InvoiceTotal < 0 Then debitanddepositsection -= InvoiceTotal
        Return Math.Round(creditsection, 2) - Math.Round(debitanddepositsection, 2)

    End Function

End Class
