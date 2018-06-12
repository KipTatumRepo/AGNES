Imports Microsoft.Win32
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
            MsgBox(filename)
        End If

    End Sub
End Class
