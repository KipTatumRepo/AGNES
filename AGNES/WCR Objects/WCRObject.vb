Imports Microsoft.Win32
Imports Microsoft.Office.Interop
Public Class WCRObject
    Public Vendors As New List(Of VendorObject)
    Public CamChecks As New List(Of CamCheck)
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
            Dim v As New VendorObject With {.VendorName = vn}
            Vendors.Add(v)
            ct += 3
            Do Until valz = "Subtotal"
                '// Check for Suspend and Dept Charges
                Select Case CType(ws.Cells(ct, 1), Excel.Range).Value
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
                    Case Else
                        v.AddTender(CType(ws.Cells(ct, 1), Excel.Range).Value, CType(ws.Cells(ct, 2), Excel.Range).Value,
                            FormatNumber(CType(ws.Cells(ct, 3), Excel.Range).Value, 0), FormatNumber(CType(ws.Cells(ct, 9), Excel.Range).Value, 2))
                End Select
                ct += 1
                valz = CType(ws.Cells(ct, 1), Excel.Range).Value
            Loop
            wb.Close()
            xlApp.Quit()
            releaseObject(ws)
            releaseObject(wb)
            releaseObject(xlApp)
            If BadFile = False Then disp.PrintToScreen(v)
        End If
    End Sub

    Public Sub AddCamCheck(Num As String, Amt As Double, Dte As Date, Nts As String)
        Dim c As New CamCheck With {.CheckNumber = Num, .CheckAmt = Amt, .DepositDate = Dte, .Notes = Nts}
        CamChecks.Add(c)
    End Sub

    Public Sub PrintWCR()
        Dim ph As String = ""
        'TODO: Create print WCR routine
    End Sub

    Private Function GetVendorNameFromString(st)
        Dim vn As String = st
        Dim si As Integer = vn.IndexOf("(")
        Dim li As Integer = vn.IndexOf(")")
        Dim vnum As Integer = FormatNumber(vn.Substring(si + 1, (li - si) - 1))
        'TODO: Replace hard code with db table lookup for store num -> vendor name
        Select Case vnum
            Case 49
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
