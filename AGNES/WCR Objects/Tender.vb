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
        'TODO: Connect to mapping table and look up associated GL; the below is hard coding for development only
        Select Case tid
            Case 1              ' Cash
                GL = 105200
            Case 9              ' Meal card
                GL = 219301
            Case 11            ' ECash
                GL = 219927
            Case 123            ' Meal card credit
                GL = 219301
            Case 124            ' Scratch coupon
                GL = 219927
            Case 45            ' Expired card
                GL = 681020
            Case 126            ' Charges to Dept (IO dept)
                GL = 112295
            Case 127            ' Suspend
            Case 128            ' Employee meals
            Case 129            ' IOU charge
            Case 83, 91, 92, 94 ' Freedom Pay & credit cards
                GL = 112265
        End Select
        GL = 999
    End Sub

End Class
