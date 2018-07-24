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

    Private Sub GetGLCode(tid As Integer)
        Dim q = From c In WCRE.Tender_GL_Mapping
                Where c.Tender_ID = tid
                Select c
        GL = 999
        Dim ct As Integer = q.Count
        For Each c In q
            GL = c.GL_Account
        Next
    End Sub

End Class
