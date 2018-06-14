Public Class Tender
    Private _tenderid As Integer
    Public Property TenderID As Integer
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
    Private Property GL As Long

    Private Sub GetGLCode(tid)
        'TODO: Connect to mapping table and look up associated GL
        GL = 999
    End Sub

End Class
