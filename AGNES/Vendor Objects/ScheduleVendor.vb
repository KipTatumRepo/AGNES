Imports System.ComponentModel
'INFRASTRUCTURE: BUILD MAX VENDOR SLOTS INTO DATABASE
Public Class ScheduleVendor
    Inherits Border

#Region "Properties"
    Public VendorItem As VendorInfo
    Private stkVendorInfo As StackPanel
    Public NameText As TextBlock
    Public VendorType As Byte
    Public TypeText As TextBlock
    Public FoodType As Integer
    Public SubType As Integer
    Public SlotsText As TextBlock
    Public MaxDailySlots As Byte = 0
    Public MaxWeeklySlots As Byte = 0
    Public VendorContextMenu As ContextMenu
    Private cmiEdit As MenuItem
    Private cmiDeactivate As MenuItem
    Private cmiReceipts As MenuItem
    Private _usedweeklyslots As Byte

    Public Property UsedWeeklySlots As Byte
        Get
            Return _usedweeklyslots
        End Get

        Set(value As Byte)
            _usedweeklyslots = value
            Visibility = Visibility.Visible
            SlotsText.Text = "Weekly Slots:" & (MaxWeeklySlots - _usedweeklyslots) & "/" & MaxWeeklySlots
            If value = MaxWeeklySlots Then Visibility = Visibility.Collapsed
        End Set
    End Property
#End Region

#Region "Constructor"
    Public Sub New(ByRef vndr As VendorInfo)
        VendorItem = vndr
        stkVendorInfo = New StackPanel
        Height = 60
        Background = Brushes.WhiteSmoke
        BorderBrush = Brushes.Black
        BorderThickness = New Thickness(1, 1, 1, 1)
        Margin = New Thickness(1, 1, 2, 0)
        CreateContextMenu()
        MaxDailySlots = vndr.MaximumDailyCafes
        MaxWeeklySlots = MaxDailySlots * VendorModule.NumberOfDaysInWeek
        VendorType = vndr.VendorType
        AddName()
        AddSlots()
        AddFoodType()
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
        SlotsText = New TextBlock With {.TextAlignment = TextAlignment.Center, .Text = "0/0"}
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
