Imports Microsoft.Win32
Imports Microsoft.Office.Interop
Public Class WCRObject
    Public Sub New()
        Dim Tender As New TenderObject
    End Sub
    Public Sub LoadTenders()
        Dim fd As New OpenFileDialog()
        fd.DefaultExt = ".xls"
        fd.Filter = "Excel (97-2003) documents (.xls)|*.xls"

        ' Display OpenFileDialog by calling ShowDialog method
        Dim result As Nullable(Of Boolean) = fd.ShowDialog()

        ' Get the selected file name and display in a TextBox
        If result = True Then
            ' Open document
            Dim filename As String = fd.FileName
            Dim xlApp As New Excel.Application(), wb As Excel.Workbook = xlApp.Workbooks.Open(filename)
            Dim ws As Excel.Worksheet = wb.Sheets(1), valz As String = "", tender As String = "", qty As Integer = 0, ttl As Double = 0, ct As Integer = 1
            Do Until valz = "Tenders"
                valz = CType(ws.Cells(ct, 2), Excel.Range).Value
                ct += 1
            Loop
            Do Until valz = "Subtotal"
                tender = CType(ws.Cells(ct, 2), Excel.Range).Value
                qty = FormatNumber(CType(ws.Cells(ct, 3), Excel.Range).Value, 0)
                ttl = FormatCurrency(CType(ws.Cells(ct, 9), Excel.Range).Value, 2)
                MsgBox(tender & " has a quantity of " & qty & " and a net tender of " & ttl & ".")
                ct += 1
                valz = CType(ws.Cells(ct, 1), Excel.Range).Value
            Loop
        End If

    End Sub
End Class
