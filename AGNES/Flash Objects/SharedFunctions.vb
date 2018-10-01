Module SharedFunctions

    Public Property FlashNotes As String
    Public Function GetCurrentPeriod(dt As Date) As Byte
        dt = dt.AddDays(1)
        Dim df = From d In SharedDataGroup.Dates
                 Where d.Date_ID = dt
                 Select d
        For Each d In df
            Return (d.MS_Period)
            Exit Function
        Next
        Return 12
    End Function

    Public Function GetCurrentWeek(dt As Date) As Byte
        dt = dt.AddDays(1)
        Dim df = From d In SharedDataGroup.Dates
                 Where d.Date_ID = dt
                 Select d
        For Each d In df
            Return (d.Week)
            Exit Function
        Next
        Return 5
    End Function

    Public Function GetMaxWeeks(p As Byte) As Byte
        Dim df = From d In SharedDataGroup.Dates
                 Where d.MS_FY = 2019 And
                     d.MS_Period = p And
                     d.Week = 5
                 Select d
        If df.Count = 0 Then
            Return 4
            Exit Function
        End If
        Return 5
    End Function

    Public Function getweekoperatingdays(p, w) As Byte
        Return 5    'TODO: TEST ONLY
    End Function

    Public Function getperiodoperatingdays(p, w) As Byte
        Return 25   'TODO: TEST ONLY
    End Function

    Public Function LoadSingleWeekAndUnitFlash(category As String, unit As Int64, yr As Int16, period As Byte, wk As Byte) As Double
        FlashNotes = ""
        Dim ff = From f In FlashActuals.FlashActualData
                 Where f.GLCategory = category And
                     f.MSFY = yr And
                     f.MSP = period And
                     f.Week = wk And
                     f.UnitNumber = unit
                 Select f
        For Each f In ff
            FlashNotes = f.FlashNotes
            Return (f.FlashValue)
        Next
        Return 0
    End Function

    Public Function LoadSingleWeekAndUnitBudget(category As String, unit As Int64, yr As Int16, period As Byte, weekoperatingdays As Byte, periodoperatingdays As Byte) As Double
        Dim bf = From b In FlashBudgets.Budgets
                 Where b.Category = category And
                     b.MSFY = yr And
                     b.MSP = period And
                     b.UnitNumber = unit
                 Select b
        For Each b In bf
            Return (b.Budget1 / periodoperatingdays) * weekoperatingdays
        Next
        Return 0
    End Function

End Module
