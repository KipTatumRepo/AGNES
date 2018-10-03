Module FlashModule
    Public UserClosed As Boolean
    Public AvailableUnits As UnitGroup
    Public FlashBudgets As BudgetEntity
    Public FlashActuals As FlashActualsEntity
    Public FlashForecasts As ForecastEntity
    Public SharedDataGroup As BIEntities
    Public FlashPage As Flash
    Public InitialLoadStatus As Byte = 0
    Public CurrentFiscalYear As Integer = 2019
    Public Sub Runmodule()
        FlashBudgets = New BudgetEntity
        FlashActuals = New FlashActualsEntity
        FlashForecasts = New ForecastEntity
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

        FlashPage = New Flash(FlashType, FlashUnit)
        FlashPage.SaveStatus = InitialLoadStatus
        FlashPage.ShowDialog()
        If UserClosed = True Then Exit Sub
    End Sub
End Module
