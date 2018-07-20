Public Class RadialPortal
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Private Sub WCRGo(sender As Object, e As MouseButtonEventArgs) Handles btnRadial1.PreviewMouseLeftButtonDown
        WCRModule.Runmodule()
    End Sub

    Private Sub BGCRMGo(sender As Object, e As MouseButtonEventArgs) Handles btnRadial2.PreviewMouseLeftButtonDown
        Dim bgcrm As New BGCRM
        bgcrm.ShowDialog()
    End Sub

    Private Sub DragViaLeftMouse(sender As Object, e As MouseButtonEventArgs)
        DragMove()
    End Sub

    Private Sub CloseAGNES(sender As Object, e As MouseButtonEventArgs)
        Dim ask As MsgBoxResult = MsgBox("Close AGNES?", MsgBoxStyle.YesNo)
        If ask = MsgBoxResult.Yes Then
            Close()
        End If
    End Sub

    Private Sub ButtonHover(sender As Object, e As MouseEventArgs) Handles btnRadial1.MouseEnter, btnRadial2.MouseEnter, btnRadial3.MouseEnter, btnRadial4.MouseEnter
        Dim s As Image = sender
        s.Height = 105
        s.Width = 105
        Dim l As Integer = s.Margin.Left - 15
        Dim t As Integer = s.Margin.Top - 15
        s.Margin = New Thickness(l, t, 0, 0)

    End Sub
    Private Sub ButtonLeave(sender As Object, e As MouseEventArgs) Handles btnRadial1.MouseLeave, btnRadial2.MouseLeave, btnRadial3.MouseLeave, btnRadial4.MouseLeave
        Dim s As Image = sender
        s.Height = 75
        s.Width = 75
        Dim l As Integer = s.Margin.Left + 15
        Dim t As Integer = s.Margin.Top + 15
        s.Margin = New Thickness(l, t, 0, 0)
    End Sub
End Class
