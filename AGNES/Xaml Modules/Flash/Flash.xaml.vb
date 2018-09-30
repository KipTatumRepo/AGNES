Public Class Flash
    Dim SalesGroup As FlashGroup
    Dim CamGroup As FlashGroup
    Dim CafeSalesGroup As FlashGroup
    Dim CateringSalesGroup As FlashGroup
    Dim TotalSalesGroup As FlashGroup
    Dim CogsGroup As FlashGroup
    Dim LaborGroup As FlashGroup
    Dim OpexGroup As FlashGroup
    Dim FeesGroup As FlashGroup
    Dim SubsidyGroup As FlashGroup
    Dim TotalGroup As FlashGroup
    Dim Wk As WeekChooser
    Dim MSP As PeriodChooser
    Dim Units As UnitChooser

    Public Sub New(FlashType, FlashUnit)
        InitializeComponent()
        ConstructTemplate(FlashType)
    End Sub

    Private Sub Image_PreviewMouseDown(sender As Object, e As MouseButtonEventArgs)
        tbSaveStatus.Text = "Draft saved"
        barSaveStatus.Background = Brushes.Yellow
    End Sub

    Private Sub Image_PreviewMouseDown_1(sender As Object, e As MouseButtonEventArgs)
        tbSaveStatus.Text = "Flash saved"
        barSaveStatus.Background = Brushes.LightGreen
    End Sub

    Private Sub ConstructTemplate(FT)
        grdFlashGroups.Children.Clear()
        Select Case FT
            Case "Cafe"
                SalesGroup = New FlashGroup("Sales", False, 74, True, False) ' Increments of 47 for flashgroup spacing
                CogsGroup = New FlashGroup("COGS", True, 121, False, False)
                LaborGroup = New FlashGroup("Labor", True, 168, True, False)
                OpexGroup = New FlashGroup("OPEX", True, 215, False, False)
                SubsidyGroup = New FlashGroup("Subsidy", True, 262, True, True)
                With grdFlashGroups.Children
                    .Add(SalesGroup)
                    .Add(CogsGroup)
                    .Add(LaborGroup)
                    .Add(OpexGroup)
                    .Add(SubsidyGroup)
                End With
                Height = 510 - 141
                Title = "Cafe Weekly Financial Flash"
            Case "WCC"
                CamGroup = New FlashGroup("CAM Revenue", False, 74, True, False) ' Increments of 47 for flashgroup spacing
                CogsGroup = New FlashGroup("COGS", True, 121, False, False)
                LaborGroup = New FlashGroup("Labor", True, 168, True, False)
                OpexGroup = New FlashGroup("OPEX", True, 215, False, False)
                SubsidyGroup = New FlashGroup("Subsidy", True, 262, True, True)
                With grdFlashGroups.Children
                    .Add(CamGroup)
                    .Add(CogsGroup)
                    .Add(LaborGroup)
                    .Add(OpexGroup)
                    .Add(SubsidyGroup)
                End With
                Height = 510 - 141
                Title = "WCC Weekly Financial Flash"
            Case "Field"
                CafeSalesGroup = New FlashGroup("Cafe Sales", False, 74, True, False) ' Increments of 47 for flashgroup spacing
                CateringSalesGroup = New FlashGroup("Catering Sales", True, 121, False, False)
                TotalSalesGroup = New FlashGroup("Total Sales", True, 168, True, True)
                CogsGroup = New FlashGroup("COGS", True, 215, False, False)
                LaborGroup = New FlashGroup("Labor", True, 262, True, False)
                OpexGroup = New FlashGroup("OPEX", True, 309, False, False)
                FeesGroup = New FlashGroup("Fees", True, 356, True, False)
                TotalGroup = New FlashGroup("Total", True, 403, True, True)
                With grdFlashGroups.Children
                    .Add(CafeSalesGroup)
                    .Add(CateringSalesGroup)
                    .Add(TotalSalesGroup)
                    .Add(CogsGroup)
                    .Add(LaborGroup)
                    .Add(OpexGroup)
                    .Add(FeesGroup)
                    .Add(TotalGroup)
                End With
                Height = 510
                Title = "Field Weekly Financial Flash"
        End Select

        'TODO: "Me" represents the object that the Chooser(s) is/are in; passing it by reference
        '      will allow a writeback of the selections period And/Or week to the object.  NEED TO TIE BACK ONCE THE FLASH OBJECT IS DONE.

        Wk = New WeekChooser(Me, 1, 5, 1)
        MSP = New PeriodChooser(Me, Wk, 1, 12, 3)
        Units = New UnitChooser(AvailableUnits)
        Dim sep As New Separator
        With tlbFlash.Items
            .Add(MSP)
            .Add(sep)
            .Add(Wk)
        End With
        tlbUnits.Items.Add(Units)
        If Units.NumberOfAvailableUnits = 1 Then Units.IsEnabled = False

    End Sub
End Class
