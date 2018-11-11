Imports System.ComponentModel
Public Class ScheduleVendor
    Inherits Border

#Region "Properties"
    Public Property VendorName As String
    Public NameText As TextBlock

#End Region

#Region "Constructor"
    Public Sub New(vn)
        VendorName = vn
        Height = 25
        Background = Brushes.LightBlue
        BorderBrush = Brushes.Black
        BorderThickness = New Thickness(1, 1, 1, 1)
        Margin = New Thickness(1, 1, 2, 0)
        AddName()
    End Sub
#End Region

#Region "Public Methods"

#End Region

#Region "Private Methods"
    Private Sub AddName()
        NameText = New TextBlock With {.TextAlignment = TextAlignment.Center, .Text = VendorName}
        Child = NameText
    End Sub

    Private Sub ScheduleVendor_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles Me.MouseDown
        DragDrop.DoDragDrop(Me, NameText.Text, DragDropEffects.Copy)
    End Sub

#End Region
End Class
