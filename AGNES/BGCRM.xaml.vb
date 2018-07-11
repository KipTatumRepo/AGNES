Public Class BGCRM
    Private Sub SaveAndNext(sender As Object, e As RoutedEventArgs) Handles btnSaveNextGroup.Click, btnSaveNextPeople.Click, btnSaveFinish.Click, btnSaveNextEvents.Click, btnSaveNextFinances.Click
        tabPages.SelectedIndex += 1


    End Sub

End Class
