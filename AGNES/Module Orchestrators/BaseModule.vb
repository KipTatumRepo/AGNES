Imports System.Windows.Threading
Module BaseModule

#Region "Properties"
    Public SharedDataGroup As BIEntities
    Public AGNESShared As AGNESSharedDataEntity
    Public FlashActuals As FlashActualsEntity
    Public FlashBudgets As BudgetEntity
    Public FlashForecasts As ForecastEntity
    Public TrainingData As TrainingEntities
    Public VendorData As VendorEntity
    Public ITData As LOCAL_IT_CFGEntities
    Public BGE As BGCRMEntity
    Public TimerOne As DispatcherTimer
    Public TimerTwo As DispatcherTimer
    Public CurrentFiscalYear As Integer = 2019
    Public Property FlashNotes As String
#End Region

#Region "Public Methods" '// Globally Shared Methods
    Public Sub Runmodule()
        SharedDataGroup = New BIEntities
        AGNESShared = New AGNESSharedDataEntity
        BGE = New BGCRMEntity
        FlashActuals = New FlashActualsEntity
        FlashBudgets = New BudgetEntity
        FlashForecasts = New ForecastEntity
        TrainingData = New TrainingEntities
        VendorData = New VendorEntity
        ITData = New LOCAL_IT_CFGEntities
    End Sub

    Public Function TruncateAlias(UserAlias As String) As String
        Dim ReturnAlias As String = UserAlias
        Try
            ReturnAlias = UserAlias.Remove(UserAlias.IndexOf("@microsoft.com"))
        Catch ex As Exception
            '// No domain attached
        End Try
        Return ReturnAlias
    End Function

    Public Sub PrintAnyObject(obj As Object, Desc As String)
        Dim pd As PrintDialog = New PrintDialog()
        pd.ShowDialog()
        pd.PrintVisual(obj, Desc)
    End Sub

    Public Function CheckForHoliday()
        '// Checks for a holiday on the upcoming Thursday and/or Friday, requiring the next week to be available early
        Dim dt As Date = FormatDateTime(Now(), DateFormat.ShortDate)
        Dim wkd As Byte = Weekday(dt, FirstDayOfWeek.Monday)
        If wkd > 2 And wkd < 5 Then
            dt = dt.AddDays(1)
            Dim df = From d In SharedDataGroup.Dates
                     Where d.Date_ID = dt
                     Select d
            For Each d In df
                If d.IS_WEEKEND_HOLIDAY = True Then
                    Return True
                End If
            Next
        End If
        Return False
    End Function

    Public Function GetCurrentPeriod(dt As Date) As Byte
        dt = dt.AddDays(-1)
        Dim df = From d In SharedDataGroup.Dates
                 Where d.Date_ID = dt
                 Select d
        For Each d In df
            Return (d.MS_Period)
        Next
        Return 12
    End Function

    Public Function GetLastCalendarDayOfMonth(m) As Byte
        Select Case m
            Case 1, 3, 5, 7, 8, 10, 12
                Return 31
            Case 4, 6, 9, 11
                Return 30
            Case 2
                If Date.IsLeapYear(Now.Year) = True Then
                    Return 29
                Else
                    Return 28
                End If
        End Select
        Return 1
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

    Public Function GetCurrentCalendarWeek(dt As Date) As Byte
        Dim IncrementDate As Date, MondayCount As Byte = 0
        Dim DateString As String = dt.Month & "/1/" & Now().Year
        IncrementDate = FormatDateTime(DateString, DateFormat.ShortDate)
        Do Until IncrementDate > dt
            If IncrementDate.DayOfWeek = DayOfWeek.Monday Then MondayCount += 1
            IncrementDate = IncrementDate.AddDays(1)
        Loop
        If MondayCount = 0 Then MondayCount = 1 '// Force a week 1 value when the 1st falls on Tues-Sun and module is opened on any of those gap days
        Return MondayCount
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

    Public Function GetMaxCalendarWeeks(m As Byte, y As Integer) As Byte
        Dim IncrementDate As Date, MondayCount As Byte = 0
        Dim DateString As String = m & "/1/" & y
        IncrementDate = FormatDateTime(DateString, DateFormat.ShortDate)
        Do Until IncrementDate.Month <> m
            If IncrementDate.DayOfWeek = DayOfWeek.Monday Then MondayCount += 1
            IncrementDate = IncrementDate.AddDays(1)
        Loop
        Return MondayCount
    End Function

    Public Function getweekoperatingdays(fy As Integer, p As Byte, w As Byte) As Byte
        Dim df = From d In SharedDataGroup.Dates
                 Where d.MS_FY = fy And
                     d.MS_Period = p And
                     d.Week = w And
                     d.IS_WEEKEND_HOLIDAY = 0
                 Select d

        Dim dayz As Byte = df.Count
        Return df.Count
    End Function

    Public Function getperiodoperatingdays(fy As Integer, p As Byte) As Byte
        Dim df = From d In SharedDataGroup.Dates
                 Where d.MS_FY = fy And
                     d.MS_Period = p And
                     d.IS_WEEKEND_HOLIDAY = 0
                 Select d
        Dim dayz As Byte = df.Count
        Return df.Count
    End Function

    Public Function LoadSingleUnitBudget(category As String, unit As Int64, yr As Int16, period As Byte) As Double
        Dim CurrentCycle As Integer = GetCorrectBudgetCycle(period)
        Dim bf = From b In FlashBudgets.Budgets
                 Where b.Category = category And
                     b.MSFY = yr And
                     b.MSP = period And
                     b.UnitNumber = unit And
                     b.Cycle = CurrentCycle
                 Select b
        For Each b In bf
            Return b.Budget1
        Next

        Return 0
    End Function

    Public Function LoadSingleWeekAndUnitBudget(category As String, unit As Int64, yr As Int16, period As Byte, weekoperatingdays As Byte, periodoperatingdays As Byte) As Double
        Dim CurrentCycle As Integer = GetCorrectBudgetCycle(period)
        Dim bf = From b In FlashBudgets.Budgets
                 Where b.Category = category And
                     b.MSFY = yr And
                     b.MSP = period And
                     b.UnitNumber = unit And
                     b.Cycle = CurrentCycle
                 Select b
        For Each b In bf
            Return (b.Budget1 / periodoperatingdays) * weekoperatingdays
        Next
        Return 0
    End Function

    Public Function GetCorrectBudgetCycle(p) As Integer
        Select Case p
            Case 1 To 3
                Return 1
            Case 4 To 6
                Return 2
            Case 7 To 9
                Return 3
            Case 10 To 12
                Return 4
            Case Else
                Return 1
        End Select
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

    Public Function SelectFlashForecastTypeAndUnit(Optional ignoreunits As Boolean = False) As (flashselection As Long, unitselection As Long)
        Dim fs As Long, us As Long, availableflashtypes As New List(Of Long), availableunits As New List(Of Long), usr As Integer, ulvl As Byte
        Dim LocalAGNESShared As AGNESSharedDataEntity = New AGNESSharedDataEntity
        usr = My.Settings.UserID : ulvl = My.Settings.UserLevel

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
        If ignoreunits = True Then
            Return (fs, 0)
            Exit Function
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
                us = 1851
            Case 7  ' Overhead
                us = 1852
            Case 8  ' Eventions
                us = 9890
            Case 10002 ' Urban Farming
                us = 31878
        End Select
        Return (fs, us)
    End Function

    Public Sub SessionLog(i)
        '// Ignore Owners to avoid junk data from debugging
        If My.Settings.UserLevel = 1 Then Exit Sub
        Select Case i
            Case 0      ' Initiate session
                Dim session As New ApplicationSession, sessionstart As DateTime = Format(Now(), "MM/dd/yy hh:mm:ss")

                '// Write new session data row
                With session
                    .ApplicationName = "AGNES"
                    .SessionStart = sessionstart
                    .UserId = Environment.UserName
                End With
                SharedDataGroup.ApplicationSessions.Add(session)
                SharedDataGroup.SaveChanges()

                '// Fetch newly saved row and assign session id to settings from PID
                Dim qsi = From usi In SharedDataGroup.ApplicationSessions
                          Select usi
                          Where usi.ApplicationName = "AGNES" And
                            usi.UserId = Environment.UserName And
                            usi.SessionStart = sessionstart

                For Each usi In qsi
                    My.Settings.SessionId = usi.PID
                Next
            Case 1      ' Conclude session
                If My.Settings.SessionId = 0 Then Exit Sub
                Dim sessionend As DateTime = Format(Now(), "MM/dd/yy hh:mm:ss")
                Dim qsi = From usi In SharedDataGroup.ApplicationSessions
                          Select usi
                          Where usi.PID = My.Settings.SessionId

                For Each usi In qsi
                    With usi
                        .SessionEnd = sessionend
                    End With
                Next
                SharedDataGroup.SaveChanges()
        End Select
    End Sub

    Public Function GetUnitName(i As Long) As String
        Dim qun = From f In SharedDataGroup.LOCATIONS
                  Where f.Unit_Number = i
                  Select f

        For Each f In qun
            Return f.Unit
        Next
        Return "Null"
    End Function

    Public Function GetFoodType(ft As Long) As String
        Dim qft = From t In VendorData.FoodTypes
                  Where t.PID = ft
                  Select t

        For Each t In qft
            Return t.Type
        Next
        Return ""
    End Function

    Public Function GetFoodTypeId(fn As String) As Long
        Dim qft = From t In VendorData.FoodTypes
                  Where t.Type = fn
                  Select t

        For Each t In qft
            Return t.PID
        Next
        Return 0
    End Function

    Public Function GetFoodSubType(ft As Long) As String
        Dim qft = From t In VendorData.FoodSubTypes
                  Where t.PID = ft
                  Select t

        For Each t In qft
            Return t.Subtype
        Next
        Return ""
    End Function

    Public Function GetFoodSubTypeId(fn As String) As Long
        Dim qft = From t In VendorData.FoodSubTypes
                  Where t.Subtype = fn
                  Select t

        For Each t In qft
            Return t.PID
        Next
        Return 0
    End Function

    Public Function GetVendorNameId(vn As String) As Long
        Dim qve = (From ve In VendorData.VendorInfo
                   Where ve.Name = vn
                   Select ve).ToList(0)
        If qve IsNot Nothing Then Return qve.PID
        Return 0
    End Function

    Public Function GetVendorNameId(vid As Long) As String
        Dim qve = (From ve In VendorData.VendorInfo
                   Where ve.PID = vid
                   Select ve).ToList(0)
        If qve IsNot Nothing Then Return qve.Name
        Return ""
    End Function

    Public Function GetProductClassId(pcn As String) As Long
        Dim qpc = (From pc In ITData.Product_Class_Master
                   Where pc.prod_class_name = pcn
                   Select pc).ToList(0)
        Return qpc.prod_class_id
    End Function

    Public Function GetProductClassName(pcid As Integer) As String
        Dim qpc = (From pc In ITData.Product_Class_Master
                   Where pc.prod_class_id = pcid
                   Select pc).ToList(0)
        Return qpc.prod_class_name
    End Function

#End Region


End Module
