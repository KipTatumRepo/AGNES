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
End Class
