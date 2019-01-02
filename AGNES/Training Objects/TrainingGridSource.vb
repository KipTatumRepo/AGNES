Imports System.Collections.ObjectModel
Public Class TrainingRecordItem
    Public Property Training As String
    Public Property Start As Date
    Public Property Complete As Date
    Public Property Trainer As String
    Public Property Certification As Boolean
    Public Property Score As Double

End Class

Public Class Trainings
    Inherits ObservableCollection(Of TrainingRecordItem)
End Class

