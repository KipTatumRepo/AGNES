﻿Imports System.ComponentModel

Public Class VendorEditor

#Region "Properties"

    Public numSupplierCode As NumberBox
    Public numDailyCafes As NumberBox
    Public curKpi As CurrencyBox
    Public curCam As CurrencyBox
    Public percCam As PercentBox
    Public percKpi As PercentBox
    Private _changesmade As Boolean
    Public Property ChangesMade As Boolean
        Get
            Return _changesmade
        End Get
        Set(value As Boolean)
            _changesmade = value
            If value = True Then
                imgSave.IsEnabled = True
                imgSave.Opacity = 1
            Else
                imgSave.IsEnabled = False
                imgSave.Opacity = 0.5
            End If
        End Set
    End Property

    Private SystemLoad As Boolean
    Private ChangeOverride As Boolean
    Private ActiveVendor As VendorInfo
    Private VendorIndex As Byte

#End Region

#Region "Constructor"
    Public Sub New()
        InitializeComponent()
        AddInitialCustomFields()
        PopulateVendors()
        PopulateProductClasses()
        PopulateFoodTypes()
        CollapseForm(0)
        Height = 100
    End Sub
#End Region

#Region "Public Methods"

#End Region

#Region "Private Methods"
    Private Sub AddInitialCustomFields()

        '// Add numbox for supplier code
        numSupplierCode = New NumberBox(125, True, False, True, False, True, AgnesBaseInput.FontSz.Standard,,, True) With {.Margin = New Thickness(227, 26, 0, 0)}
        AddHandler numSupplierCode.BaseTextBox.TextChanged, AddressOf FlagChanges
        grdSupplierInfo.Children.Add(numSupplierCode)

        '// Add numbox for maximum number of daily cafes
        numDailyCafes = New NumberBox(90, True, False, True, False, True, AgnesBaseInput.FontSz.Standard) With {.Margin = New Thickness(346, 31, 0, 0)}
        AddHandler numDailyCafes.BaseTextBox.TextChanged, AddressOf FlagChanges
        grdBrandDetail.Children.Add(numDailyCafes)

        '// Add CAM amount currency box
        curCam = New CurrencyBox(82, True, AgnesBaseInput.FontSz.Standard,, True, False) With {.Margin = New Thickness(361, 31, 0, 0), .Visibility = Visibility.Collapsed}
        AddHandler curCam.BaseTextBox.TextChanged, AddressOf FlagChanges
        grdCamKpi.Children.Add(curCam)

        '// Add KPI amount currency box
        curKpi = New CurrencyBox(82, True, AgnesBaseInput.FontSz.Standard,, True, False) With {.Margin = New Thickness(361, 77, 0, 0), .Visibility = Visibility.Collapsed}
        AddHandler curKpi.BaseTextBox.TextChanged, AddressOf FlagChanges
        grdCamKpi.Children.Add(curKpi)

        '// Add CAM amount percentage box
        percCam = New PercentBox(82, True, AgnesBaseInput.FontSz.Standard, 1, True, False) With {.Margin = New Thickness(361, 31, 0, 0), .Visibility = Visibility.Collapsed}
        AddHandler percCam.BaseTextBox.TextChanged, AddressOf FlagChanges
        grdCamKpi.Children.Add(percCam)

        '// Add KPI amount percentage box
        percKpi = New PercentBox(82, True, AgnesBaseInput.FontSz.Standard, 1, True, False) With {.Margin = New Thickness(361, 77, 0, 0), .Visibility = Visibility.Collapsed}
        AddHandler percKpi.BaseTextBox.TextChanged, AddressOf FlagChanges
        grdCamKpi.Children.Add(percKpi)

    End Sub

    Private Sub PopulateVendors()
        Dim qav = From av In VendorData.VendorInfo
                  Where av.Active = True
                  Order By av.Name
                  Select av
        cbxVendorName.Items.Add(New ComboBoxItem With {.Content = "Add New Vendor", .FontSize = 10, .FontWeight = FontWeights.Bold})
        For Each av In qav
            Dim DisplayVendName As String = av.Name & " ["
            Select Case av.VendorType
                Case 0  ' Commons Food
                    DisplayVendName &= "Commons Food]"
                Case 1  ' Commons Retail
                    DisplayVendName &= "Commons Retail]"
                Case 2  ' Local Brand
                    DisplayVendName &= "Local Brand]"
                Case 3  ' Food Truck
                    DisplayVendName &= "Food Truck]"

            End Select
            cbxVendorName.Items.Add(DisplayVendName)
        Next
    End Sub

    Private Sub PopulateProductClasses()
        Dim qpc = (From pc In ITData.Product_Class_Master
                   Select pc.prod_class_name).ToArray()
        Array.Sort(qpc)
        For Each pc In qpc
            cbxCommonsProductClass.Items.Add(pc)
        Next
    End Sub

    Private Sub PopulateFoodTypes()
        cbxFoodType.Items.Clear()
        cbxFoodSubType.Items.Clear()
        Dim qft = From ft In VendorData.FoodTypes
                  Select ft

        For Each ft In qft
            cbxFoodType.Items.Add(ft.Type)
        Next

        Dim qfs = From fs In VendorData.FoodSubTypes
                  Select fs

        For Each fs In qfs
            cbxFoodSubType.Items.Add(fs.Subtype)
        Next

    End Sub

    Private Sub VendorSelected(sender As Object, e As SelectionChangedEventArgs) Handles cbxVendorName.SelectionChanged
        If ChangeOverride = True Then
            ChangeOverride = False
            Exit Sub
        End If
        Select Case cbxVendorName.SelectedIndex

            Case -1 ' Deselected - clear and disable everything
                CollapseForm(0)
            Case 0  ' New vendor entry
                ActiveVendor = Nothing
                'CRITICAL: ADD NOTIFICATION HANDLER FOR STORE ID
            Case Else ' Existing vendor selected
                If ActiveVendor IsNot Nothing Then
                    If ChangesMade = True Then
                        Dim amsg = New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Small, AgnesMessageBox.MsgBoxLayout.TextAndImage, AgnesMessageBox.MsgBoxType.YesNo, 12,,, "Discard changes?", "You have unsaved changes.  Continue and discard?", AgnesMessageBox.ImageType.Alert)
                        amsg.ShowDialog()
                        If amsg.ReturnResult = "No" Then
                            ChangeOverride = True
                            cbxVendorName.SelectedIndex = VendorIndex
                            ChangeOverride = False
                            amsg.Close()
                            Exit Sub
                        Else
                            amsg.Close()
                        End If
                    End If
                End If

                SystemLoad = True
                Dim vndnm As String = cbxVendorName.SelectedValue
                vndnm = Mid(vndnm, 1, vndnm.IndexOf("[") - 1)

                Dim qav = (From av In VendorData.VendorInfo
                           Where av.Name = vndnm
                           Select av).ToList(0)
                ActiveVendor = qav
                CollapseForm(1)

                cbxVendorType.SelectedIndex = ActiveVendor.VendorType
                cbxStatus.SelectedIndex = 0
                dtpContract.SelectedDate = ActiveVendor.ContractExpiration
                dtpInsurance.SelectedDate = ActiveVendor.InsuranceExpiration

                DisplayForm()
                LockImmutables()
                SystemLoad = False
                ChangesMade = False
                VendorIndex = cbxVendorName.SelectedIndex
        End Select
    End Sub

    Private Sub CollapseForm(i)
        If i = 0 Then
            Height = 100
            txtIns.Visibility = Visibility.Collapsed
            txtCon.Visibility = Visibility.Collapsed
            cbxStatus.IsEnabled = False
        Else
            Height = 145
            txtIns.Visibility = Visibility.Visible
            txtCon.Visibility = Visibility.Visible
        End If
        cbxVendorType.IsEnabled = False
        gbxBrands.Visibility = Visibility.Collapsed
        gbxCommonsFood.Visibility = Visibility.Collapsed
        gbxCommonsGeneral.Visibility = Visibility.Collapsed
        gbxNonRetail.Visibility = Visibility.Collapsed
        imgSave.Visibility = Visibility.Collapsed
    End Sub

    Private Sub DisplayForm()

        Select Case ActiveVendor.VendorType
            Case 0  ' Commons Food
                Height = 490
                gbxCommonsGeneral.Visibility = Visibility.Visible
                gbxNonRetail.Visibility = Visibility.Visible
                gbxCommonsFood.Visibility = Visibility.Visible

            Case 1  ' Commons Retail
                Height = 320
                gbxCommonsGeneral.Visibility = Visibility.Visible
            Case 2  ' Local Brand
                Height = 325
                gbxNonRetail.Visibility = Visibility.Visible
                gbxBrands.Visibility = Visibility.Visible
            Case 3  ' Food Truck
                Height = 270
                gbxNonRetail.Visibility = Visibility.Visible

        End Select
        cbxStatus.IsEnabled = True
        dtpContract.IsEnabled = True
        dtpInsurance.IsEnabled = True
        imgSave.Visibility = Visibility.Visible
    End Sub

    Private Sub PopulateCommonsGeneralDetails(sender As Object, e As DependencyPropertyChangedEventArgs) Handles gbxCommonsGeneral.IsVisibleChanged
        Select Case e.NewValue
            Case True   ' Visible
                If ActiveVendor Is Nothing Then Exit Sub
                cbxCamType.SelectedIndex = ActiveVendor.CAMType - 1
                Select Case ActiveVendor.CAMType - 1
                    Case 0  ' None

                    Case 1  ' Percentage
                        lblCamStart.Visibility = Visibility.Visible
                        dtpCamStart.SelectedDate = ActiveVendor.CAMStart
                        dtpCamStart.DisplayDate = ActiveVendor.CAMStart

                        lblCamAmt.Visibility = Visibility.Visible
                        percCam.Visibility = Visibility.Visible
                        percCam.SetAmount = ActiveVendor.CAMAmount
                    Case 2  ' Flat amount
                        lblCamStart.Visibility = Visibility.Visible
                        dtpCamStart.SelectedDate = ActiveVendor.CAMStart
                        dtpCamStart.DisplayDate = ActiveVendor.CAMStart

                        lblCamAmt.Visibility = Visibility.Visible
                        curCam.Visibility = Visibility.Visible
                        curCam.SetAmount = ActiveVendor.CAMAmount
                End Select

                cbxKpiType.SelectedIndex = ActiveVendor.KPIType - 1
                Select Case ActiveVendor.KPIType - 1
                    Case 0  ' None

                    Case 1  ' Percentage
                        lblKpiStart.Visibility = Visibility.Visible
                        dtpKpiStart.SelectedDate = ActiveVendor.KPIStart
                        dtpKpiStart.DisplayDate = ActiveVendor.KPIStart

                        lblKpiAmt.Visibility = Visibility.Visible
                        percKpi.Visibility = Visibility.Visible
                        percKpi.SetAmount = ActiveVendor.KPIAmount
                    Case 2  ' Flat amount
                        lblKpiStart.Visibility = Visibility.Visible
                        dtpKpiStart.SelectedDate = ActiveVendor.KPIStart
                        dtpKpiStart.DisplayDate = ActiveVendor.KPIStart

                        lblKpiAmt.Visibility = Visibility.Visible
                        curKpi.Visibility = Visibility.Visible
                        curKpi.SetAmount = ActiveVendor.KPIAmount
                End Select

            Case False  ' Collapsed/Hidden
                cbxCamType.SelectedIndex = -1
                cbxCamType.Text = ""

                cbxKpiType.SelectedIndex = -1
                cbxKpiType.Text = ""

                lblCamStart.Visibility = Visibility.Collapsed
                dtpCamStart.SelectedDate = Nothing
                dtpCamStart.DisplayDate = Now()

                lblKpiStart.Visibility = Visibility.Collapsed
                dtpKpiStart.SelectedDate = Nothing
                dtpKpiStart.DisplayDate = Now()

                lblCamAmt.Visibility = Visibility.Collapsed
                curCam.Visibility = Visibility.Collapsed
                curCam.SetAmount = 0
                percCam.Visibility = Visibility.Collapsed
                percCam.SetAmount = 0

                lblKpiAmt.Visibility = Visibility.Collapsed
                curKpi.Visibility = Visibility.Collapsed
                curKpi.SetAmount = 0
                percKpi.Visibility = Visibility.Collapsed
                percKpi.SetAmount = 0
        End Select
    End Sub

    Private Sub PopulateCommonsFoodDetails(sender As Object, e As DependencyPropertyChangedEventArgs) Handles gbxCommonsFood.IsVisibleChanged
        Select Case e.NewValue
            Case True   ' Visible
                If ActiveVendor Is Nothing Then Exit Sub
                txtInvoiceName.Text = ActiveVendor.Invoice
                numSupplierCode.SetAmount = ActiveVendor.Supplier
            Case False  ' Collapsed/hidden
                txtInvoiceName.Text = ""
                numSupplierCode.SetAmount = 0
        End Select
    End Sub

    Private Sub PopulateNonRetailDetails(sender As Object, e As DependencyPropertyChangedEventArgs) Handles gbxNonRetail.IsVisibleChanged
        Select Case e.NewValue
            Case True   ' Visible
                If ActiveVendor Is Nothing Then Exit Sub
                cbxFoodType.Text = GetFoodType(ActiveVendor.FoodType)
                cbxFoodSubType.Text = GetFoodSubType(ActiveVendor.FoodSubType)
            Case False  ' Collapse/hidden
                cbxFoodType.SelectedIndex = -1
                cbxFoodType.Text = ""
                cbxFoodSubType.SelectedIndex = -1
                cbxFoodSubType.Text = ""
        End Select

    End Sub

    Private Sub PopulateBrandDetails(sender As Object, e As DependencyPropertyChangedEventArgs) Handles grdBrandDetail.IsVisibleChanged
        Select Case e.NewValue
            Case True   ' Visible
                If ActiveVendor Is Nothing Then Exit Sub
                cbxCommonsProductClass.Text = GetProductClassName(ActiveVendor.ProductClassId)
                chkHood.IsChecked = ActiveVendor.RequiresHood
                numDailyCafes.SetAmount = ActiveVendor.MaximumDailyCafes
            Case False  ' Collapse/hidden
                cbxCommonsProductClass.SelectedIndex = -1
                cbxCommonsProductClass.Text = ""
                chkHood.IsChecked = False
                numDailyCafes.SetAmount = 0
        End Select
    End Sub

    Private Sub LockImmutables()
        cbxCommonsProductClass.IsEnabled = False
        numSupplierCode.IsEnabled = False
        cbxVendorType.IsEnabled = False
    End Sub

    Private Sub FlagChanges() Handles cbxStatus.SelectionChanged, dtpContract.SelectedDateChanged, dtpInsurance.SelectedDateChanged,
            cbxCamType.SelectionChanged, dtpCamStart.SelectedDateChanged, cbxKpiType.SelectionChanged, dtpKpiStart.SelectedDateChanged,
            txtInvoiceName.TextChanged, cbxFoodType.SelectionChanged, cbxFoodSubType.SelectionChanged, cbxCommonsProductClass.SelectionChanged,
            chkHood.Unchecked, chkHood.Checked
        If SystemLoad = False Then ChangesMade = True
    End Sub

#End Region

End Class
