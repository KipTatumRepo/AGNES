Imports System.ComponentModel
Public Class ScheduleVendor
    Inherits Border

#Region "Properties"
    Public VendorItem As VendorInfo
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
    Private cmiFilter As MenuItem
    Private cmiKillFilters As MenuItem
    Private _usedweeklyslots As Byte
    Private stkVendorInfo As StackPanel
    Private DisableDrag As Boolean


    Public Property UsedWeeklySlots As Byte
        Get
            Return _usedweeklyslots
        End Get

        Set(value As Byte)
            _usedweeklyslots = value
            ActiveVendor(1)
            SlotsText.Text = "Weekly Slots:" & (MaxWeeklySlots - _usedweeklyslots) & "/" & MaxWeeklySlots
            If value = MaxWeeklySlots Then ActiveVendor(0)
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
    Private Sub ActiveVendor(tf)
        If tf = 0 Then
            DisableDrag = True
            NameText.Foreground = Brushes.LightGray
            TypeText.Foreground = Brushes.LightGray
            SlotsText.Foreground = Brushes.LightGray
        Else
            DisableDrag = False
            NameText.Foreground = Brushes.Black
            TypeText.Foreground = Brushes.Black
            SlotsText.Foreground = Brushes.Black
        End If
    End Sub

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
        If DisableDrag = True Then Exit Sub
        VendorSched.ActiveVendor = Me
        DragDrop.DoDragDrop(Me, NameText.Text, DragDropEffects.Copy)
    End Sub


#Region "Context Menu Items"
    Private Sub CreateContextMenu()
        VendorContextMenu = New ContextMenu
        AddHandler VendorContextMenu.Loaded, AddressOf FilterEnable
        cmiEdit = New MenuItem With {.Header = "Edit Vendor"}
        AddHandler cmiEdit.Click, AddressOf EditVendor
        cmiDeactivate = New MenuItem With {.Header = "Deactivate Vendor"}
        AddHandler cmiDeactivate.Click, AddressOf DeactivateVendor
        cmiFilter = New MenuItem With {.Header = "Filter on Vendor"}
        AddHandler cmiFilter.Click, AddressOf VendorFilter
        cmiKillFilters = New MenuItem With {.Header = "Remove Vendor Filters"}
        AddHandler cmiKillFilters.Click, AddressOf RemoveFilter
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

    Private Sub FilterEnable()
        If VendorSched.VendorFilterOn = True Then
            cmiFilter.IsEnabled = False
            cmiKillFilters.IsEnabled = True
        Else
            cmiFilter.IsEnabled = True
            cmiKillFilters.IsEnabled = False
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
                                If TargetStation.VendorStack.Children.Count < 2 Then
                                    TargetStation.Visibility = Visibility.Collapsed
                                Else
                                    For Each v In TargetStation.VendorStack.Children
                                        If TypeOf (v) Is VendorInStation Then
                                            Dim targetvendor As VendorInStation = v
                                            If targetvendor.ReferencedVendor IsNot Me Then TargetStation.Visibility = Visibility.Collapsed
                                        End If
                                    Next
                                End If
                            End If
                            If TypeOf (s) Is ScheduleTruckStation Then
                                Dim TargetTruck As ScheduleTruckStation = s
                                If TargetTruck.TruckName <> Me.VendorItem.Name Then TargetTruck.Visibility = Visibility.Collapsed
                            End If
                        Next
                    End If
                Next
            End If
        Next
        VendorSched.VendorFilterOn = True
    End Sub

    Private Sub RemoveFilter()
        VendorSched.ResetVendorFilters()
    End Sub

#End Region

#End Region

End Class
