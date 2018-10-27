Module SharedFunctions

#Region "Properties"
    Public Property FlashNotes As String

#End Region

#Region "Public Methods"
    Public Function LoadSingleWeekAndUnitFlash(category As String, unit As Int64, yr As Int16, period As Byte, wk As Byte) As (fv As Double, Stts As String, Notes As String, alert As Boolean)
        FlashNotes = ""
        Dim ff = From f In FlashActuals.FlashActualData
                 Where f.GLCategory = category And
                     f.MSFY = yr And
                     f.MSP = period And
                     f.Week = wk And
                     f.UnitNumber = unit
                 Select f
        For Each f In ff
            Return (f.FlashValue, f.Status, f.FlashNotes, f.Alert)
        Next
        Return (0, "", "", False)
    End Function

#End Region

End Module
