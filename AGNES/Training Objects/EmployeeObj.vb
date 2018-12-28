Public Class EmployeeObj
    Public Property FirstName As String
    Public Property LastName As String
    Public Property CostCenter As Long
    Public Property CompassId As Long
    Public ReadOnly Property Fullname As String
        Get
            'Return FirstName & " " & LastName
            Return LastName & ", " & FirstName

        End Get
    End Property

End Class

