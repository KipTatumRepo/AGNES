﻿Public Class RadialPortal
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Private Sub WCRGo(sender As Object, e As MouseButtonEventArgs) Handles btnRadial1.PreviewMouseLeftButtonDown
        Hide()
        WCRModule.Runmodule()
        Show()
    End Sub

    Private Sub BGCRMGo(sender As Object, e As MouseButtonEventArgs) Handles btnRadial2.PreviewMouseLeftButtonDown
        Hide()
        Dim bgcrm As New BGCRM
        bgcrm.ShowDialog()
        Show()

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

    Private Sub ButtonHover(sender As Object, e As MouseEventArgs) Handles btnRadial1.MouseEnter, btnRadial2.MouseEnter, btnRadial3.MouseEnter, btnRadial4.MouseEnter,
            btnRadial5.MouseEnter, btnRadial6.MouseEnter, btnRadial7.MouseEnter, btnRadial8.MouseEnter
        Dim s As Image = sender
        s.Height = 85
        s.Width = 85
        Dim l As Integer = s.Margin.Left - 5
        Dim t As Integer = s.Margin.Top - 5
        s.Margin = New Thickness(l, t, 0, 0)
    End Sub
    Private Sub ButtonLeave(sender As Object, e As MouseEventArgs) Handles btnRadial1.MouseLeave, btnRadial2.MouseLeave, btnRadial3.MouseLeave, btnRadial4.MouseLeave,
            btnRadial5.MouseLeave, btnRadial6.MouseLeave, btnRadial7.MouseLeave, btnRadial8.MouseLeave
        Dim s As Image = sender
        s.Height = 75
        s.Width = 75
        Dim l As Integer = s.Margin.Left + 5
        Dim t As Integer = s.Margin.Top + 5
        s.Margin = New Thickness(l, t, 0, 0)
    End Sub
End Class