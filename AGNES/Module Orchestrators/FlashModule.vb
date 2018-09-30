Module FlashModule
    Public UserClosed As Boolean
    Public AvailableUnits As UnitGroup
    Public FlashBudgets As BudgetEntity
    Public SharedDataGroup As BIEntities
    Public Sub Runmodule()
        FlashBudgets = New BudgetEntity
        SharedDataGroup = New BIEntities
        '///TEST
        Dim FlashType As String = "WCC" 'TODO: REMOVE PLACEHOLDER FLASH TYPE
        '// Determine which flash, or flashes, user has access to.  If multiple, give them a choice.  If not, move forward
        Dim FlashUnit As Long = 19837   'TODO: REMOVE PLACEHOLDER UNIT
        '// Determine which unit, or units, user has access to.  If multiple, give them a choice.  If not, move forward
        '// Populate FlashGroup with applicable units
        'TODO: REMOVE PLACEHOLDER FLASH TYPE
        AvailableUnits = New UnitGroup With {.UnitGroupName = "WCC"}
        Dim testFlash As New UnitFlash With {.FlashType = FlashType, .UnitNumber = FlashUnit}
        Dim testFlash1 As New UnitFlash With {.FlashType = FlashType, .UnitNumber = 19838}
        Dim testFlash2 As New UnitFlash With {.FlashType = FlashType, .UnitNumber = 19839}
        Dim testFlash3 As New UnitFlash With {.FlashType = FlashType, .UnitNumber = 19840}
        Dim testFlash4 As New UnitFlash With {.FlashType = FlashType, .UnitNumber = 19841}
        Dim testFlash5 As New UnitFlash With {.FlashType = FlashType, .UnitNumber = 19842}
        Dim testFlash6 As New UnitFlash With {.FlashType = FlashType, .UnitNumber = 19843}
        Dim testFlash7 As New UnitFlash With {.FlashType = FlashType, .UnitNumber = 19844}
        Dim testFlash8 As New UnitFlash With {.FlashType = FlashType, .UnitNumber = 19855}
        Dim testFlash9 As New UnitFlash With {.FlashType = FlashType, .UnitNumber = 19856}
        Dim testFlash10 As New UnitFlash With {.FlashType = FlashType, .UnitNumber = 19857}
        Dim testFlash11 As New UnitFlash With {.FlashType = FlashType, .UnitNumber = 19858}
        Dim testFlash12 As New UnitFlash With {.FlashType = FlashType, .UnitNumber = 19857}
        Dim testFlash13 As New UnitFlash With {.FlashType = FlashType, .UnitNumber = 19858}
        With AvailableUnits.UnitsInGroup
            .Add(testFlash)
            .Add(testFlash1)
            .Add(testFlash2)
            .Add(testFlash3)
            .Add(testFlash4)
            .Add(testFlash5)
            .Add(testFlash6)
            .Add(testFlash7)
            .Add(testFlash8)
            .Add(testFlash9)
            .Add(testFlash10)
            .Add(testFlash11)
            .Add(testFlash12)
            .Add(testFlash13)
        End With
        '///TEST

        Dim FlashPage As New Flash(FlashType, FlashUnit)
        FlashPage.ShowDialog()
        If UserClosed = True Then Exit Sub
    End Sub
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
End Module
