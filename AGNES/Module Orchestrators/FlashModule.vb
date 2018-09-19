Module FlashModule
    Public UserClosed As Boolean
    Public AvailableUnits As UnitGroup
    Public Sub Runmodule()
        '///TEST
        Dim FlashType As String = "WCC" 'TODO: REMOVE PLACEHOLDER FLASH TYPE
        '// Determine which flash, or flashes, user has access to.  If multiple, give them a choice.  If not, move forward
        Dim FlashUnit As Long = 19837   'TODO: REMOVE PLACEHOLDER UNIT
        '// Determine which unit, or units, user has access to.  If multiple, give them a choice.  If not, move forward
        '// Populate FlashGroup with applicable units
        'TODO: REMOVE PLACEHOLDER FLASH TYPE
        AvailableUnits = New UnitGroup With {.UnitGroupName = "WCC"}
        Dim testFlash As New UnitFlash With {.FlashType = FlashType, .UnitNumber = FlashUnit}
        AvailableUnits.UnitsInGroup.Add(testFlash)
        '///TEST

        Dim FlashPage As New Flash(FlashType, FlashUnit)
        FlashPage.ShowDialog()
        If UserClosed = True Then Exit Sub
    End Sub
End Module
