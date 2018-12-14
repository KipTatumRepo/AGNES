﻿Imports System.ComponentModel
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
        Height = 44
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
        NameText = New TextBlock With {.TextAlignment = TextAlignment.Center, .Text = VendorItem.Name, .ContextMenu = VendorContextMenu, .FontSize = 10}
        stkVendorInfo.Children.Add(NameText)
    End Sub

    Private Sub AddFoodType()
        Dim textstr As String = ""
        TypeText = New TextBlock With {.TextAlignment = TextAlignment.Center, .FontSize = 10}
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
        SlotsText = New TextBlock With {.TextAlignment = TextAlignment.Center, .Text = "0/0", .FontSize = 10}
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
        Dim cmiFilter As New MenuItem With {.Header = "Filter on Vendor"}
        AddHandler cmiFilter.Click, AddressOf VendorFilter
        Dim cmiKillFilters As New MenuItem With {.Header = "Remove Vendor Filters"}
        AddHandler cmiKillFilters.Click, AddressOf VendorSched.ResetVendorFilters
        With VendorContextMenu.Items
            .Add(cmiFilter)
            .Add(cmiKillFilters)
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

    Private Sub VendorFilter()
        For Each sd In VendorSched.wkSched.Children
            If TypeOf (sd) Is ScheduleDay Then
                Dim TargetDay As ScheduleDay = sd
                For Each Location In TargetDay.LocationStack.Children
                    If TypeOf (Location) Is ScheduleLocation Then
                        Dim TargetLoc As ScheduleLocation = Location
                        For Each s In TargetLoc.StationStack.Children
                            If TypeOf (s) Is ScheduleStation Then
                                Dim TargetStation As ScheduleStation = s
                                For Each v In TargetStation.VendorStack.Children
                                    If TypeOf (v) Is VendorInStation Then
                                        Dim targetvendor As VendorInStation = v
                                        If targetvendor.ReferencedVendor IsNot Me Then TargetStation.Visibility = Visibility.Collapsed
                                    End If
                                Next
                            End If
                        Next
                    End If
                Next
            End If
        Next
    End Sub

#End Region

#End Region

End Class
