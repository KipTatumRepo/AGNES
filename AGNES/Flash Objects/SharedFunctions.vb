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

    Public Function SelectFlashForecastTypeAndUnit() As (flashselection As Byte, unitselection As Long)
        Dim fs As Byte, us As Long, availableflashtypes As New List(Of Long), availableunits As New List(Of Long), usr As Integer, ulvl As Byte
        Dim LocalAGNESShared As AGNESSharedDataEntity = New AGNESSharedDataEntity
        usr = My.Settings.UserID : ulvl = My.Settings.UserLevel

        'TEST
        ' fs = 2 ': usr = 81 : ulvl = 4
        'TEST

        Select Case ulvl
            Case 4      '// Construct availableflashtypes wih flash types available to user
                Dim qaf = From c In LocalAGNESShared.FlashTypesUsers_Join
                          Where c.UserId = usr
                          Select c

                For Each c In qaf
                    Dim qft = From d In LocalAGNESShared.FlashTypes
                              Where d.PID = c.FlashId
                              Select d
                    For Each d In qft
                        availableflashtypes.Add(d.PID)
                    Next
                Next
            Case Else   '// Super user or above - Construct availableflashtypes with all flash types
                Dim qft = From c In LocalAGNESShared.FlashTypes
                          Select c
                For Each c In qft
                    availableflashtypes.Add(c.PID)
                Next

        End Select
        If availableflashtypes.Count > 1 Then
            '// Offer choice popup for which type the user wants; this is assigned to fs
            Dim flchs As New FlashForecastChooser With {.ChooserType = 0}
            flchs.Populate(availableflashtypes)
            flchs.ShowDialog()
            fs = flchs.UserChoice
            flchs.Close()
        Else
            fs = availableflashtypes(0)
        End If
        Select Case fs
            Case 1, 2, 4    ' Cafes, Commons, Fields
                Select Case ulvl
                    Case 4      '// Construct availableunits wih units available to user within the selected flash type
                        Dim qau = From c In LocalAGNESShared.UnitsUsers_Join
                                  Where c.UserId = usr
                                  Select c

                        For Each c In qau
                            Dim qun = From f In SharedDataGroup.LOCATIONS
                                      Where f.Unit_Number = c.UnitNumber And
                                          f.FlashType = fs
                                      Select f

                            For Each f In qun
                                availableunits.Add(f.Unit_Number)
                            Next
                        Next

                    Case Else   '// Super user or above - Construct availableunits with all units within the selected flash type
                        Dim qun = From f In SharedDataGroup.LOCATIONS
                                  Where f.FlashType = fs
                                  Select f

                        For Each f In qun
                            availableunits.Add(f.Unit_Number)
                        Next
                End Select

                If availableunits.Count > 1 Then
                    '// Offer choice popup for which unit the user wants; this is assigned to us
                    Dim flchs As New FlashForecastChooser With {.ChooserType = 1}
                    flchs.Populate(availableunits)
                    flchs.ShowDialog()
                    us = flchs.UserChoice
                    flchs.Close()
                Else
                    us = availableunits(0)
                End If
            Case 3  ' AV
                us = 30954
            Case 5  ' Beverage
                us = 2627
            Case 6  ' Catering
            Case 7  ' Overhead
                us = 1852
            Case 8  ' Eventions
                us = 9890
        End Select
        Return (fs, us)
    End Function

End Module
