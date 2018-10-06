Module SharedFunctions

    Public Property FlashNotes As String
    Public Function GetCurrentPeriod(dt As Date) As Byte
        dt = dt.AddDays(1)
        Dim df = From d In SharedDataGroup.Dates
                 Where d.Date_ID = dt
                 Select d
        For Each d In df
            Return (d.MS_Period)
        Next
        Return 12
    End Function

    Public Function GetCurrentWeek(dt As Date) As Byte
        dt = dt.AddDays(-1)
        Dim df = From d In SharedDataGroup.Dates
                 Where d.Date_ID = dt
                 Select d
        For Each d In df
            Return (d.Week)
        Next
        Return 5
    End Function

    Public Function GetMaxWeeks(p As Byte) As Byte
        Dim df = From d In SharedDataGroup.Dates
                 Where d.MS_FY = CurrentFiscalYear And
                     d.MS_Period = p And
                     d.Week = 5
                 Select d
        If df.Count = 0 Then
            Return 4
        End If
        Return 5
    End Function

    Public Function getweekoperatingdays(p As Byte, w As Byte) As Byte
        Dim df = From d In SharedDataGroup.Dates
                 Where d.MS_FY = CurrentFiscalYear And
                     d.MS_Period = p And
                     d.Week = w And
                     d.IS_WEEKEND_HOLIDAY = 0
                 Select d

        Dim dayz As Byte = df.Count
        Return df.Count
    End Function

    Public Function getperiodoperatingdays(p As Byte) As Byte
        Dim df = From d In SharedDataGroup.Dates
                 Where d.MS_FY = CurrentFiscalYear And
                     d.MS_Period = p And
                     d.IS_WEEKEND_HOLIDAY = 0
                 Select d
        Dim dayz As Byte = df.Count
        Return df.Count
    End Function

    Public Function LoadSingleWeekAndUnitFlash(category As String, unit As Int64, yr As Int16, period As Byte, wk As Byte) As (fv As Double, Stts As String, Notes As String)
        FlashNotes = ""
        Dim ff = From f In FlashActuals.FlashActualData
                 Where f.GLCategory = category And
                     f.MSFY = yr And
                     f.MSP = period And
                     f.Week = wk And
                     f.UnitNumber = unit
                 Select f
        For Each f In ff
            Return (f.FlashValue, f.Status, f.FlashNotes)
        Next
        Return (0, "", "")
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

    Public Function LoadSingleWeekAndUnitForecast(category As String, unit As Int64, yr As Int16, period As Byte, wk As Byte) As Double
        Dim fo = From f In FlashForecasts.Forecasts
                 Where f.GLCategory = category And
                     f.MSFY = yr And
                     f.MSP = period And
                     f.Week = wk And
                     f.UnitNumber = unit
                 Select f
        For Each f In fo
            Return (f.ForecastValue)
        Next
        Return 0
    End Function

End Module
