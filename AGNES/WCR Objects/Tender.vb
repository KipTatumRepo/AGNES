Imports System.Data
Public Class Tender
    Private _tenderid As Integer
    Public Property TenderId As Integer
        Get
            Return _tenderid
        End Get
        Set(value As Integer)
            GetGLCode(value)
            _tenderid = value
        End Set
    End Property
    Public Property TenderName As String
    Public Property TenderQty As Integer
    Public Property TenderAmt As Double
    Public Property GL As Long

    Public Sub New()
        Dim ph As String = ""
    End Sub

    Private Sub GetGLCode(tid)
        DataSets.TenderGLAdapt.Fill(DataSets.TenderGLTable)
        Dim dr() As DataRow = DataSets.TenderGLTable.Select("Tender_ID = '" & tid & "'")
        If dr.Count > 0 Then
            GL = dr(0)("GL_Account")
        Else
            GL = 999
        End If
    End Sub

End Class
