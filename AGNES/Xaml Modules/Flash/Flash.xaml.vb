Public Class Flash
    Private Sub Image_PreviewMouseDown(sender As Object, e As MouseButtonEventArgs)
        tbSaveStatus.Text = "Draft saved"
        barSaveStatus.Background = Brushes.Yellow
    End Sub

    Private Sub Image_PreviewMouseDown_1(sender As Object, e As MouseButtonEventArgs)
        tbSaveStatus.Text = "Flash saved"
        barSaveStatus.Background = Brushes.LightGreen
    End Sub

    Private Sub Image_PreviewMouseDown_2(sender As Object, e As MouseButtonEventArgs)
        grdFlashGroups.Children.Clear()
        Dim CafeSales As New FlashGroup("Cafe Sales", False, 74, True, False) ' Increments of 47 for flashgroup spacing
        Dim CateringSales As New FlashGroup("Catering Sales", True, 121, False, False)
        Dim TotalSales As New FlashGroup("Total Sales", True, 168, True, True)
        Dim Cogs As New FlashGroup("COGS", True, 215, False, False)
        Dim Labor As New FlashGroup("Labor", True, 262, True, False)
        Dim Opex As New FlashGroup("OPEX", True, 309, False, False)
        Dim Fees As New FlashGroup("Fees", True, 356, True, False)
        Dim Total As New FlashGroup("Total", True, 403, True, True)
        'TODO: ADD PROCEDURE AND LIST FOR RELATED SUBTOTALS

        With grdFlashGroups.Children
            .Add(CafeSales)
            .Add(CateringSales)
            .Add(TotalSales)
            .Add(Cogs)
            .Add(Labor)
            .Add(Opex)
            .Add(Fees)
            .Add(Total)
        End With
        Height = 510
    End Sub

    Private Sub Image_PreviewMouseDown_3(sender As Object, e As MouseButtonEventArgs)
        grdFlashGroups.Children.Clear()
        Dim Sales As New FlashGroup("Sales", False, 74, True, False) ' Increments of 47 for flashgroup spacing
        Dim Cogs As New FlashGroup("COGS", True, 121, False, False)
        Dim Labor As New FlashGroup("Labor", True, 168, True, False)
        Dim Opex As New FlashGroup("OPEX", True, 215, False, False)
        Dim Subsidy As New FlashGroup("Subsidy", True, 262, True, True)

        'TODO: ADD PROCEDURE AND LIST FOR RELATED SUBTOTALS

        With grdFlashGroups.Children
            .Add(Sales)
            .Add(Cogs)
            .Add(Labor)
            .Add(Opex)
            .Add(Subsidy)
        End With
        Height = 510 - 141
    End Sub

    Private Sub Image_PreviewMouseDown_4(sender As Object, e As MouseButtonEventArgs)
        grdFlashGroups.Children.Clear()
        Dim Cam As New FlashGroup("CAM Revenue", False, 74, True, False) ' Increments of 47 for flashgroup spacing
        Dim Cogs As New FlashGroup("COGS", True, 121, False, False)
        Dim Labor As New FlashGroup("Labor", True, 168, True, False)
        Dim Opex As New FlashGroup("OPEX", True, 215, False, False)
        Dim Subsidy As New FlashGroup("Subsidy", True, 262, True, True)

        'TODO: ADD PROCEDURE AND LIST FOR RELATED SUBTOTALS

        With grdFlashGroups.Children
            .Add(Cam)
            .Add(Cogs)
            .Add(Labor)
            .Add(Opex)
            .Add(Subsidy)
        End With
        Height = 510 - 141
    End Sub
End Class
