Imports Microsoft.Win32
Imports Microsoft.Office.Interop
Public Class WCRObject
    Public Tenders As New List(Of TenderObject)
    Public Sub New()
        Dim ph As String = ""
    End Sub
    Public Sub LoadTenders(ByRef disp As WCRHello)
        Dim fd As New OpenFileDialog()
        fd.DefaultExt = ".xls"
        fd.Filter = "Excel (97-2003) documents (.xls)|*.xls"
        Dim result As Nullable(Of Boolean) = fd.ShowDialog()
        If result = True Then
            Dim ti As New TenderObject
            Tenders.Add(ti)
            Dim filename As String = fd.FileName
            Dim xlApp As New Excel.Application(), wb As Excel.Workbook = xlApp.Workbooks.Open(filename)
            Dim ws As Excel.Worksheet = wb.Sheets(1), valz As String = "", tendert As String = "", qty As Integer = 0, ttl As Double = 0, ct As Integer = 1
            Do Until Left(valz, 13) = "Selected For:"
                valz = CType(ws.Cells(ct, 1), Excel.Range).Value
                ct += 1
            Loop
            ti.VendorName = GetVendorNameFromString(valz)
            ct += 3
            Do Until valz = "Subtotal"
                tendert = CType(ws.Cells(ct, 2), Excel.Range).Value
                qty = FormatNumber(CType(ws.Cells(ct, 3), Excel.Range).Value, 0)
                ttl = FormatCurrency(CType(ws.Cells(ct, 9), Excel.Range).Value, 2)
                ti.AddItem(tendert, qty, ttl)
                ct += 1
                valz = CType(ws.Cells(ct, 1), Excel.Range).Value
            Loop

            wb.Close()
            xlApp.Quit()
            releaseObject(ws)
            releaseObject(wb)
            releaseObject(xlApp)
            disp.PrintToScreen(ti)
        End If

    End Sub
    Private Function GetVendorNameFromString(st)
        Dim vn As String = st
        Dim si As Integer = vn.IndexOf("(")
        Dim li As Integer = vn.IndexOf(")")
        Dim vnum As Integer = FormatNumber(vn.Substring(si + 1, (li - si) - 1))
        'TODO: Replace hard code with db table lookup for store num -> vendor name
        Select Case vnum
            Case 499
                vn = "Typhoon"
            Case Else
                vn = "Nada"
        End Select
        Return vn
    End Function
    Private Sub releaseObject(ByVal obj As Object)
        Try
            System.Runtime.InteropServices.Marshal.ReleaseComObject(obj)
            obj = Nothing
        Catch ex As Exception
            obj = Nothing
        End Try
    End Sub
End Class
