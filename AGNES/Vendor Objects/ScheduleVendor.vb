Imports System.ComponentModel
'INFRASTRUCTURE: BUILD MAX VENDOR SLOTS INTO DATABASE
Public Class ScheduleVendor
    Inherits Border

#Region "Properties"
    Public VendorItem As VendorInfo
    Private stkVendorInfo As StackPanel
    Public NameText As TextBlock
    Public TypeText As TextBlock
    Public FoodType As Integer
    Public SubType As Integer
    Public SlotsText As TextBlock
    Private MaxSlots As Byte = 3     '//TEST
    Public VendorContextMenu As ContextMenu
    Private cmiEdit As MenuItem
    Private cmiDeactivate As MenuItem
    Private cmiReceipts As MenuItem
    Private _usedslots As Byte
    Public Property UsedSlots As Byte
        Get
            Return _usedslots
        End Get
        Set(value As Byte)
            _usedslots = value
            Visibility = Visibility.Visible
            SlotsText.Text = "Max schedule: " & (3 - UsedSlots) & "/" & MaxSlots
            If value = MaxSlots Then Visibility = Visibility.Collapsed
        End Set
    End Property


#End Region

#Region "Constructor"
    Public Sub New(ByRef vndr As VendorInfo)
        VendorItem = vndr
        stkVendorInfo = New StackPanel
        Height = 60
        Background = Brushes.LightBlue
        BorderBrush = Brushes.Black
        BorderThickness = New Thickness(1, 1, 1, 1)
        Margin = New Thickness(1, 1, 2, 0)
        CreateContextMenu()

        AddName()
        AddFoodType()
        AddSlots()
        UsedSlots = 0

        Child = stkVendorInfo
    End Sub

#End Region

#Region "Public Methods"

#End Region

#Region "Private Methods"

    Private Sub AddName()
        NameText = New TextBlock With {.TextAlignment = TextAlignment.Center, .Text = VendorItem.Name, .ContextMenu = VendorContextMenu}
        stkVendorInfo.Children.Add(NameText)
    End Sub

    Private Sub AddFoodType()
        Dim textstr As String = ""
        TypeText = New TextBlock With {.TextAlignment = TextAlignment.Center}
        If VendorItem.FoodType IsNot Nothing Then
            textstr = GetFoodType(VendorItem.FoodType)
            FoodType = VendorItem.FoodType
        End If
        If VendorItem.FoodSubType IsNot Nothing Then
            textstr = textstr & "(" & GetFoodSubType(VendorItem.FoodSubType) & ")"
            SubType = VendorItem.FoodSubType
        End If
        TypeText.Text = textstr
        stkVendorInfo.Children.Add(TypeText)
    End Sub

    Private Sub AddSlots()
        SlotsText = New TextBlock With {.TextAlignment = TextAlignment.Center}
        stkVendorInfo.Children.Add(SlotsText)
    End Sub

    Private Sub ScheduleVendor_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles Me.MouseLeftButtonDown
        VendorSched.ActiveVendor = Me
        DragDrop.DoDragDrop(Me, NameText.Text, DragDropEffects.Copy)
    End Sub


#Region "Context Menu Items"
    Private Sub CreateContextMenu()
        VendorContextMenu = New ContextMenu
        Dim cmiEdit As New MenuItem With {.Header = "Edit Vendor"}
        AddHandler cmiEdit.Click, AddressOf EditVendor
        Dim cmiDeactivate As New MenuItem With {.Header = "Deactive Vendor"}
        AddHandler cmiDeactivate.Click, AddressOf DeactivateVendor
        With VendorContextMenu.Items
            .Add(cmiEdit)
            .Add(cmiDeactivate)
        End With
        If VendorItem.VendorType = 3 Then
            Dim cmiReceipts As New MenuItem With {.Header = "Record Receipts"}
            AddHandler cmiReceipts.Click, AddressOf EnterReceipts
            VendorContextMenu.Items.Add(cmiReceipts)
        End If
    End Sub

    Private Sub EditVendor()
        Dim ph As String = ""
    End Sub

    Private Sub DeactivateVendor()
        Dim ph As String = ""
    End Sub

    Private Sub EnterReceipts()
        Dim ph As String = ""
    End Sub

#End Region

#End Region

End Class
