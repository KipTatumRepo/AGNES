Imports System.Windows.Threading
Imports System.ComponentModel
Public Class LocationEditor

#Region "Properties"
    Private _truckyesno As Boolean
    Public Property TruckYesNo As Boolean
        Get
            Return _truckyesno
        End Get
        Set(value As Boolean)
            _truckyesno = value
            TruckPanelTimer = New DispatcherTimer()
            AddHandler TruckPanelTimer.Tick, AddressOf SlideTruckPanel
            TruckPanelTimer.Interval = New TimeSpan(0, 0, 0, 0, 0.25)
            TruckPanelTimer.Start()
        End Set
    End Property

    Private _hoodyesno As Boolean
    Public Property HoodYesNo As Boolean
        Get
            Return _hoodyesno
        End Get
        Set(value As Boolean)
            _hoodyesno = value
            HoodPanelTimer = New DispatcherTimer()
            AddHandler HoodPanelTimer.Tick, AddressOf SlideHoodPanel
            HoodPanelTimer.Interval = New TimeSpan(0, 0, 0, 0, 0.25)
            HoodPanelTimer.Start()
        End Set
    End Property

    Private _cansave As Boolean
    Private Property CanSave As Boolean
        Get
            Return _cansave
        End Get
        Set(value As Boolean)
            _cansave = value
            imgSave.IsEnabled = value
            If value = True Then
                imgSave.Opacity = 1
            Else
                imgSave.Opacity = 0.6
            End If
        End Set
    End Property

    Public numStationCount As NumberBox
    Private TruckPanelTimer As DispatcherTimer
    Private HoodPanelTimer As DispatcherTimer
    Private TruckPanelXPos As Integer
    Private HoodPanelXPos As Integer

#End Region

#Region "Constructor"
    Public Sub New()
        InitializeComponent()
        PopulateCafes()
        PopulateBuildings()
        PopulateFoodTypes()
        PopulateFoodSubtypes()
        ConstructNewFields()

    End Sub

#End Region

#Region "Public Methods"

#End Region

#Region "Private Methods"
    Private Sub PopulateCafes()
        lbxCafes.Items.Clear()
        Dim qci = From ci In SharedDataGroup.Cafes
                  Select ci

        For Each ci In qci
            lbxCafes.Items.Add(GetCafeName(ci.CostCenter))
        Next
        lbxCafes.Items.SortDescriptions.Add(New SortDescription("", ListSortDirection.Ascending))
    End Sub

    Private Sub PopulateBuildings()
        lbxBuilding.Items.Clear()
        Dim qci = From ci In SharedDataGroup.Buildings
                  Select ci

        For Each ci In qci
            If IsBldgACafe(ci.PID) = False Then lbxBuilding.Items.Add(ci.BldgName)
        Next
        lbxCafes.Items.SortDescriptions.Add(New SortDescription("", ListSortDirection.Ascending))
    End Sub

    Private Sub PopulateFoodTypes()
        cbxFoodType.Items.Clear()
        Dim qft = From ft In VendorData.FoodTypes
                  Select ft
                  Order By ft.Type

        For Each ft In qft
            cbxFoodType.Items.Add(ft.Type)
        Next

    End Sub

    Private Sub PopulateFoodSubtypes()
        cbxFoodSubType.Items.Clear()
        Dim qft = From ft In VendorData.FoodSubTypes
                  Select ft
                  Order By ft.Subtype

        For Each ft In qft
            cbxFoodSubType.Items.Add(ft.Subtype)
        Next
    End Sub

    Private Sub ConstructNewFields()
        numStationCount = New NumberBox(118, True, False, True, False, True, AgnesBaseInput.FontSz.Medium) With {.Margin = New Thickness(10, 258, 0, 0), .IsEnabled = False}
        numStationCount.BaseTextBox.TabIndex = 2
        grdMain.Children.Add(numStationCount)
    End Sub

    Private Sub SlideTruckYesNo(sender As Object, e As MouseButtonEventArgs) Handles synTruck.MouseLeftButtonDown
        If TruckPanelTimer IsNot Nothing Then Exit Sub
        TruckYesNo = Not TruckYesNo

    End Sub

    Private Sub SlideTruckPanel()
        If TruckYesNo = True Then
            TruckPanelXPos += 1
            If TruckPanelXPos > 40 Then
                TruckPanelTimer.Stop()
                TruckPanelTimer = Nothing
            Else
                synTruck.brdSlider.Margin = New Thickness(TruckPanelXPos, 0, 0, 0)
            End If
        Else
            TruckPanelXPos -= 1
            If TruckPanelXPos < 0 Then
                TruckPanelTimer.Stop()
                TruckPanelTimer = Nothing
            Else
                synTruck.brdSlider.Margin = New Thickness(TruckPanelXPos, 0, 0, 0)
            End If
        End If


    End Sub

    Private Sub SlideHoodYesNo(sender As Object, e As MouseButtonEventArgs) Handles synHood.MouseLeftButtonDown
        If HoodPanelTimer IsNot Nothing Then Exit Sub
        HoodYesNo = Not HoodYesNo

    End Sub

    Private Sub SlideHoodPanel()
        If HoodYesNo = True Then
            HoodPanelXPos += 1
            If HoodPanelXPos > 40 Then
                HoodPanelTimer.Stop()
                HoodPanelTimer = Nothing
            Else
                synHood.brdSlider.Margin = New Thickness(HoodPanelXPos, 0, 0, 0)
            End If
        Else
            HoodPanelXPos -= 1
            If HoodPanelXPos < 0 Then
                HoodPanelTimer.Stop()
                HoodPanelTimer = Nothing
            Else
                synHood.brdSlider.Margin = New Thickness(HoodPanelXPos, 0, 0, 0)
            End If
        End If
    End Sub

    Private Sub lbxCafes_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles lbxCafes.SelectionChanged
        If lbxCafes.SelectedIndex = -1 Then
            CanSave = False
            synHood.Opacity = 0.6
            synHood.IsEnabled = False
            synTruck.Opacity = 0.6
            synTruck.IsEnabled = False
            cbxFoodType.SelectedIndex = -1
            cbxFoodType.IsEnabled = False
            cbxFoodSubType.SelectedIndex = -1
            cbxFoodSubType.IsEnabled = False
            numStationCount.IsEnabled = False
            numStationCount.SetAmount = 0
            Exit Sub
        End If
        lbxBuilding.SelectedIndex = -1
        lbxBuilding.SelectedValue = ""
        synHood.Opacity = 1
        synHood.IsEnabled = True
        synTruck.Opacity = 1
        synTruck.IsEnabled = True
        cbxFoodType.IsEnabled = True
        cbxFoodSubType.IsEnabled = True
        LoadCafeInfo(lbxCafes.SelectedValue)
        numStationCount.IsEnabled = True
        CanSave = True
        numStationCount.UserFocus()
    End Sub

    Private Sub lbxBuilding_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles lbxBuilding.SelectionChanged
        If lbxBuilding.SelectedIndex = -1 Then
            CanSave = False
            Exit Sub
        End If
        lbxCafes.SelectedIndex = -1
        lbxCafes.SelectedValue = ""
        LoadBuildingInfo(lbxBuilding.SelectedValue.ToString)
        CanSave = True
    End Sub

    Private Sub LoadCafeInfo(CafeName As String)
        Dim cc As Long = GetCostCenter(CafeName)
        Dim qgc = (From gc In SharedDataGroup.Cafes
                   Where gc.CostCenter = cc
                   Select gc).ToList(0)

        numStationCount.SetAmount = qgc.BrandStations
        HoodYesNo = qgc.HasHood
        TruckYesNo = CheckForTrucksAtCafe(qgc.BldgId)
        cbxFoodType.Text = GetFoodType(qgc.AnchorStationFoodType)
        cbxFoodSubType.Text = GetFoodSubType(qgc.AnchorStationFoodSubType)

    End Sub

    Private Sub LoadBuildingInfo(BldgNm As String)
        Dim qgb = (From gb In SharedDataGroup.Buildings
                   Where gb.BldgName = BldgNm
                   Select gb).ToList(0)

        Try
            TruckYesNo = qgb.AllowFoodTrucks
        Catch
            TruckYesNo = False
        End Try
        HoodYesNo = False
        synTruck.IsEnabled = True
        synTruck.Opacity = 1

    End Sub

    Private Sub UserSaving(sender As Object, e As MouseButtonEventArgs) Handles imgSave.PreviewMouseLeftButtonDown
        If lbxCafes.SelectedIndex <> -1 Then
            SaveCafeData()
        Else
            SaveBldgData()
        End If
        SharedDataGroup.SaveChanges()
    End Sub

    Private Sub SaveCafeData()
        Dim cc As Long = GetCostCenter(lbxCafes.SelectedValue)
        Dim qgc = (From gc In SharedDataGroup.Cafes
                   Where gc.CostCenter = cc
                   Select gc).ToList(0)

        With qgc
            .BrandStations = numStationCount.Amount
            .HasHood = HoodYesNo
        End With

        If cbxFoodType.SelectedIndex > -1 Then qgc.AnchorStationFoodType = GetFoodTypeId(cbxFoodType.SelectedValue)
        If cbxFoodSubType.SelectedIndex > -1 Then qgc.AnchorStationFoodSubType = GetFoodSubTypeId(cbxFoodSubType.SelectedValue)

        Dim bid As Long = qgc.BldgId
        Dim qct = (From ct In SharedDataGroup.Buildings
                   Where ct.PID = Bid
                   Select ct).ToList(0)

        qct.AllowFoodTrucks = TruckYesNo

        lbxCafes.SelectedIndex = -1
        lbxCafes.SelectedValue = ""
        TruckYesNo = False
        synTruck.Opacity = 0.6
        synTruck.IsEnabled = False
        HoodYesNo = False
        synHood.Opacity = 0.6
        synHood.IsEnabled = False
        CanSave = False
    End Sub

    Private Sub SaveBldgData()
        Dim BldgNm As String = lbxBuilding.SelectedValue
        Dim qgb = (From gb In SharedDataGroup.Buildings
                   Where gb.BldgName = BldgNm
                   Select gb).ToList(0)

        qgb.AllowFoodTrucks = TruckYesNo
        lbxBuilding.SelectedIndex = -1
        lbxBuilding.SelectedValue = ""
        TruckYesNo = False
        synTruck.Opacity = 0.6
        synTruck.IsEnabled = False
        CanSave = False
    End Sub

    Private Function GetCafeName(cc As Long) As String
        Dim qgc = (From gc In SharedDataGroup.CostCenters
                   Where gc.CostCenter1 = cc
                   Select gc).ToList(0)

        Return qgc.UnitName
    End Function

    Private Function GetCostCenter(cn As String) As Long
        Dim qgc = (From gc In SharedDataGroup.CostCenters
                   Where gc.UnitName = cn
                   Select gc).ToList(0)

        Return qgc.CostCenter1
    End Function

    Private Function IsBldgACafe(bid As Long) As Boolean
        Dim qbc = (From bc In SharedDataGroup.Cafes
                   Where bc.BldgId = bid).ToList().AsEnumerable

        If qbc.Count > 0 Then Return True

        Return False
    End Function

    Private Function CheckForTrucksAtCafe(bid As Long) As Boolean
        Dim qct = (From ct In SharedDataGroup.Buildings
                   Where ct.PID = bid
                   Select ct).ToList(0)

        If qct.AllowFoodTrucks = True Then Return True
        Return False
    End Function

#End Region

End Class
