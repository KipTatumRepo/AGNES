Imports System.ComponentModel

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
    Private NewVendor As Boolean
    Private StartVendor As String
    Private StartVendorIndex As Integer

#End Region

#Region "Constructor"
    Public Sub New(Optional VendorShow As String = "")
        InitializeComponent()
        If VendorShow <> "" Then StartVendor = VendorShow
        AddInitialCustomFields()
        PopulateVendors()
        PopulateProductClasses()
        PopulateFoodTypes()
        PopulateFoodSubTypes()
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
        numSupplierCode.BaseTextBox.TabIndex = 15
        AddHandler numSupplierCode.BaseTextBox.TextChanged, AddressOf FlagChanges
        grdSupplierInfo.Children.Add(numSupplierCode)

        '// Add numbox for maximum number of daily cafes
        numDailyCafes = New NumberBox(94, True, False, True, False, True, AgnesBaseInput.FontSz.Standard) With {.Margin = New Thickness(10, 31, 0, 0)}
        numDailyCafes.BaseTextBox.TabIndex = 18
        AddHandler numDailyCafes.BaseTextBox.TextChanged, AddressOf FlagChanges
        grdBrandDetail.Children.Add(numDailyCafes)

        '// Add CAM amount currency box
        curCam = New CurrencyBox(82, True, AgnesBaseInput.FontSz.Standard,, True, False) With {.Margin = New Thickness(282, 31, 0, 0), .Visibility = Visibility.Collapsed}
        AddHandler curCam.BaseTextBox.TextChanged, AddressOf FlagChanges
        curCam.tb.TabIndex = 7
        grdCamKpi.Children.Add(curCam)

        '// Add KPI amount currency box
        curKpi = New CurrencyBox(82, True, AgnesBaseInput.FontSz.Standard,, True, False) With {.Margin = New Thickness(282, 77, 0, 0), .Visibility = Visibility.Collapsed}
        curKpi.tb.TabIndex = 12
        AddHandler curKpi.BaseTextBox.TextChanged, AddressOf FlagChanges
        grdCamKpi.Children.Add(curKpi)

        '// Add CAM amount percentage box
        percCam = New PercentBox(82, True, AgnesBaseInput.FontSz.Standard, 1, True, False) With {.Margin = New Thickness(282, 31, 0, 0), .Visibility = Visibility.Collapsed}
        percCam.BaseTextBox.TabIndex = 8
        AddHandler percCam.BaseTextBox.TextChanged, AddressOf FlagChanges
        grdCamKpi.Children.Add(percCam)

        '// Add KPI amount percentage box
        percKpi = New PercentBox(82, True, AgnesBaseInput.FontSz.Standard, 1, True, False) With {.Margin = New Thickness(282, 77, 0, 0), .Visibility = Visibility.Collapsed}
        percKpi.BaseTextBox.TabIndex = 13
        AddHandler percKpi.BaseTextBox.TextChanged, AddressOf FlagChanges
        grdCamKpi.Children.Add(percKpi)

        '// Add day options to CAM due (1st-25th)
        cbxCamDue.Items.Clear()
        For x As Byte = 1 To 25
            cbxCamDue.Items.Add(x)
        Next
    End Sub

    Private Sub PopulateVendors()
        cbxVendorName.Items.Clear()
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
            If av.Name = StartVendor Then StartVendor = DisplayVendName
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

        Dim qft = From ft In VendorData.FoodTypes
                  Select ft
                  Order By ft.Type

        For Each ft In qft
            cbxFoodType.Items.Add(ft.Type)
        Next
    End Sub

    Private Sub PopulateFoodSubTypes()
        cbxFoodSubType.Items.Clear()
        Dim qfs = From fs In VendorData.FoodSubTypes
                  Select fs
                  Order By fs.Subtype

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
                If NewVendor = True Then Exit Sub
                If ActiveVendor IsNot Nothing Then CollapseForm(0)
                ActiveVendor = Nothing
                NewVendor = True
                AddNewVendor()
                txtInvoiceName.IsEnabled = True
                numSupplierCode.IsEnabled = True
            Case Else ' Existing vendor selected
                If ActiveVendor IsNot Nothing Then
                    If ChangesMade = True Then
                        If VerifyDiscardChanges() = False Then Exit Sub
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
        gbxBrandsTrucks.Visibility = Visibility.Collapsed
        gbxCommonsFood.Visibility = Visibility.Collapsed
        gbxCommonsGeneral.Visibility = Visibility.Collapsed
        gbxNonRetail.Visibility = Visibility.Collapsed
        imgSave.Visibility = Visibility.Collapsed
    End Sub

    Private Sub DisplayForm()
        Dim VendType As Byte
        If NewVendor = False Then
            VendType = ActiveVendor.VendorType
        Else
            VendType = cbxVendorType.SelectedIndex
            cbxCamType.IsEnabled = True
            dtpCamStart.IsEnabled = True
            curCam.IsEnabled = True
            percCam.IsEnabled = True
            cbxCamDue.IsEditable = True
            cbxKpiType.IsEnabled = True
            dtpKpiStart.IsEnabled = True
            curKpi.IsEnabled = True
            percKpi.IsEnabled = True
        End If
        Select Case VendType
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
                gbxBrandsTrucks.Visibility = Visibility.Visible
                lblProdClass.Visibility = Visibility.Visible
                lblHood.Visibility = Visibility.Visible
                chkHood.Visibility = Visibility.Visible
                cbxCommonsProductClass.Visibility = Visibility.Visible
            Case 3  ' Food Truck
                Height = 325
                gbxNonRetail.Visibility = Visibility.Visible
                gbxBrandsTrucks.Visibility = Visibility.Visible
                lblProdClass.Visibility = Visibility.Collapsed
                lblHood.Visibility = Visibility.Collapsed
                chkHood.Visibility = Visibility.Collapsed
                cbxCommonsProductClass.Visibility = Visibility.Collapsed

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
                        dtpCamStart.Visibility = Visibility.Visible
                        dtpCamStart.SelectedDate = ActiveVendor.CAMStart
                        dtpCamStart.DisplayDate = ActiveVendor.CAMStart

                        lblCamAmt.Visibility = Visibility.Visible
                        percCam.Visibility = Visibility.Visible
                        percCam.SetAmount = ActiveVendor.CAMAmount

                        lblCamDue.Visibility = Visibility.Visible
                        cbxCamDue.Visibility = Visibility.Visible
                        cbxCamDue.Text = ActiveVendor.CamDue.ToString

                    Case 2  ' Flat amount
                        lblCamStart.Visibility = Visibility.Visible
                        dtpCamStart.Visibility = Visibility.Visible
                        dtpCamStart.SelectedDate = ActiveVendor.CAMStart
                        dtpCamStart.DisplayDate = ActiveVendor.CAMStart

                        lblCamAmt.Visibility = Visibility.Visible
                        curCam.Visibility = Visibility.Visible
                        curCam.SetAmount = ActiveVendor.CAMAmount

                        lblCamDue.Visibility = Visibility.Visible
                        cbxCamDue.Visibility = Visibility.Visible
                        cbxCamDue.Text = ActiveVendor.CamDue

                End Select

                cbxKpiType.SelectedIndex = ActiveVendor.KPIType - 1
                Select Case ActiveVendor.KPIType - 1
                    Case 0  ' None

                    Case 1  ' Percentage
                        lblKpiStart.Visibility = Visibility.Visible
                        dtpKpiStart.Visibility = Visibility.Visible
                        dtpKpiStart.SelectedDate = ActiveVendor.KPIStart
                        dtpKpiStart.DisplayDate = ActiveVendor.KPIStart

                        lblKpiAmt.Visibility = Visibility.Visible
                        percKpi.Visibility = Visibility.Visible
                        percKpi.SetAmount = ActiveVendor.KPIAmount
                    Case 2  ' Flat amount
                        lblKpiStart.Visibility = Visibility.Visible
                        dtpKpiStart.Visibility = Visibility.Visible
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
                dtpCamStart.Visibility = Visibility.Collapsed
                dtpCamStart.SelectedDate = Nothing
                dtpCamStart.DisplayDate = Now()

                lblKpiStart.Visibility = Visibility.Collapsed
                dtpKpiStart.Visibility = Visibility.Collapsed
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

                lblCamDue.Visibility = Visibility.Collapsed
                cbxCamDue.Visibility = Visibility.Collapsed
                cbxCamDue.SelectedIndex = -1
                cbxCamDue.Text = ""

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
                If ActiveVendor.VendorType = 2 Then
                    cbxCommonsProductClass.Text = GetProductClassName(ActiveVendor.ProductClassId)
                    chkHood.IsChecked = ActiveVendor.RequiresHood
                End If
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
        txtInvoiceName.IsEnabled = False
        numSupplierCode.IsEnabled = False
        cbxVendorType.IsEnabled = False
        cbxCamType.IsEnabled = False
        dtpCamStart.IsEnabled = False
        curCam.IsEnabled = False
        percCam.IsEnabled = False
        cbxCamDue.IsEditable = False
        cbxKpiType.IsEnabled = False
        dtpKpiStart.IsEnabled = False
        curKpi.IsEnabled = False
        percKpi.IsEnabled = False
    End Sub

    Private Sub FlagChanges() Handles cbxStatus.SelectionChanged, dtpContract.SelectedDateChanged, dtpInsurance.SelectedDateChanged,
            dtpCamStart.SelectedDateChanged, dtpKpiStart.SelectedDateChanged,
            txtInvoiceName.TextChanged, cbxFoodType.SelectionChanged, cbxFoodSubType.SelectionChanged, cbxCommonsProductClass.SelectionChanged,
            chkHood.Unchecked, chkHood.Checked
        If SystemLoad = False Then ChangesMade = True

    End Sub

    Private Sub CamSelected(sender As Object, e As SelectionChangedEventArgs) Handles cbxCamType.SelectionChanged
        If SystemLoad = False Then ChangesMade = True
        Select Case cbxCamType.SelectedIndex
            Case 1  ' Percentage
                lblCamStart.Visibility = Visibility.Visible
                dtpCamStart.Visibility = Visibility.Visible
                dtpCamStart.SelectedDate = Nothing
                dtpCamStart.DisplayDate = Now()
                lblCamAmt.Visibility = Visibility.Visible
                percCam.Visibility = Visibility.Visible
                percCam.SetAmount = 0
                curCam.Visibility = Visibility.Collapsed
                lblCamDue.Visibility = Visibility.Visible
                cbxCamDue.Visibility = Visibility.Visible
                curCam.SetAmount = 0
            Case 2  ' Flat
                lblCamStart.Visibility = Visibility.Visible
                dtpCamStart.Visibility = Visibility.Visible
                dtpCamStart.SelectedDate = Nothing
                dtpCamStart.DisplayDate = Now()
                lblCamAmt.Visibility = Visibility.Visible
                curCam.Visibility = Visibility.Visible
                curCam.SetAmount = 0
                percCam.Visibility = Visibility.Collapsed
                lblCamDue.Visibility = Visibility.Visible
                cbxCamDue.Visibility = Visibility.Visible
                percCam.SetAmount = 0
            Case Else
                lblCamStart.Visibility = Visibility.Collapsed
                dtpCamStart.Visibility = Visibility.Collapsed
                dtpCamStart.SelectedDate = Nothing
                dtpCamStart.DisplayDate = Now()

                lblCamAmt.Visibility = Visibility.Collapsed
                curCam.Visibility = Visibility.Collapsed
                curCam.SetAmount = 0
                percCam.Visibility = Visibility.Collapsed
                percCam.SetAmount = 0
        End Select
    End Sub

    Private Sub KPISelected(sender As Object, e As SelectionChangedEventArgs) Handles cbxKpiType.SelectionChanged
        If SystemLoad = False Then ChangesMade = True
        Select Case cbxKpiType.SelectedIndex
            Case 1  ' Percentage
                lblKpiStart.Visibility = Visibility.Visible
                dtpKpiStart.Visibility = Visibility.Visible
                dtpKpiStart.SelectedDate = Nothing
                dtpKpiStart.DisplayDate = Now()
                lblKpiAmt.Visibility = Visibility.Visible
                percKpi.Visibility = Visibility.Visible
                percKpi.SetAmount = 0
                curKpi.Visibility = Visibility.Collapsed
                curKpi.SetAmount = 0
            Case 2  ' Flat
                lblKpiStart.Visibility = Visibility.Visible
                dtpKpiStart.Visibility = Visibility.Visible
                dtpKpiStart.SelectedDate = Nothing
                dtpKpiStart.DisplayDate = Now()
                lblKpiAmt.Visibility = Visibility.Visible
                curKpi.Visibility = Visibility.Visible
                curKpi.SetAmount = 0
                percKpi.Visibility = Visibility.Collapsed
                percKpi.SetAmount = 0
            Case Else
                lblKpiStart.Visibility = Visibility.Collapsed
                dtpKpiStart.Visibility = Visibility.Collapsed
                dtpKpiStart.SelectedDate = Nothing
                dtpKpiStart.DisplayDate = Now()

                lblKpiAmt.Visibility = Visibility.Collapsed
                curKpi.Visibility = Visibility.Collapsed
                curKpi.SetAmount = 0
                percKpi.Visibility = Visibility.Collapsed
                percKpi.SetAmount = 0
        End Select
    End Sub

    Private Sub AddNewVendor()

        '// Get new vendor name
        Dim newname As New SingleUserInput(EnterOnly:=True) With {.InputType = 0, .DisplayText = "Add new vendor name"}
        newname.ShowDialog()
        If newname.StringVal = "" Then Exit Sub

        NewVendor = True
        ChangesMade = True
        Dim NewVendorName As String = newname.StringVal
        newname.Close()

        ChangeOverride = True
        cbxVendorType.SelectedIndex = -1
        cbxStatus.SelectedIndex = -1
        dtpInsurance.SelectedDate = Nothing
        dtpInsurance.DisplayDate = Now()
        dtpContract.SelectedDate = Nothing
        dtpContract.DisplayDate = Now()

        '// Add new vendor to combobox temporarily and suppress Add New Vendor option
        cbxVendorName.Items.Insert(0, NewVendorName)
        cbxVendorName.SelectedIndex = 0
        cbxVendorName.Items.RemoveAt(1)

        '// Open up the vendor type combobox
        cbxVendorType.IsEnabled = True

        ChangeOverride = False

    End Sub

    Private Sub VendorTypeSelected(sender As Object, e As SelectionChangedEventArgs) Handles cbxVendorType.SelectionChanged
        If NewVendor = False Or ChangeOverride = True Then Exit Sub
        CollapseForm(1)
        cbxStatus.SelectedIndex = 0
        DisplayForm()
        cbxStatus.IsEnabled = False
        dtpInsurance.Focus()
    End Sub

    Private Sub VendorEditor_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        If ChangesMade = True Then
            If VerifyDiscardChanges() = False Then
                e.Cancel = True
                Exit Sub
            End If
        End If
    End Sub

    Private Sub AddNewFoodType(sender As Object, e As MouseButtonEventArgs) Handles imgAddFoodType.MouseLeftButtonDown
        '// Get new food type name
        Dim newfood As New SingleUserInput(EnterOnly:=False) With {.InputType = 0, .DisplayText = "Enter new food type name"}
        newfood.ShowDialog()
        Dim FoodName As String = newfood.StringVal
        newfood.Close()
        If FoodName = "" Then Exit Sub

        '// Check if food type already exists
        Dim qft = From ft In VendorData.FoodTypes
                  Where UCase(ft.Type) = UCase(FoodName)
                  Select ft

        If qft.Count > 0 Then
            Dim amsg = New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Small, AgnesMessageBox.MsgBoxLayout.TextAndImage, AgnesMessageBox.MsgBoxType.OkOnly, 12,,, "Cannot Add", "Food type already exists", AgnesMessageBox.ImageType.Danger)
            amsg.ShowDialog()
            amsg.Close()
            Exit Sub
        End If

        '// Add to database
        Dim nft As New FoodType
        nft.Type = FoodName
        VendorData.FoodTypes.Add(nft)
        VendorData.SaveChanges()

        '// Refresh combobox list and select new food type
        PopulateFoodTypes()
        cbxFoodType.Text = FoodName
    End Sub

    Private Sub AddNewFoodSubType(sender As Object, e As MouseButtonEventArgs) Handles imgAddFoodSubType.MouseLeftButtonDown
        '// Get new food subtype name
        Dim newfood As New SingleUserInput(EnterOnly:=False) With {.InputType = 0, .DisplayText = "Enter new food subtype name"}
        newfood.ShowDialog()
        Dim FoodName As String = newfood.StringVal
        newfood.Close()
        If FoodName = "" Then Exit Sub

        '// Check if food type already exists
        Dim qft = From ft In VendorData.FoodSubTypes
                  Where UCase(ft.Subtype) = UCase(FoodName)
                  Select ft

        If qft.Count > 0 Then
            Dim amsg = New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Small, AgnesMessageBox.MsgBoxLayout.TextAndImage, AgnesMessageBox.MsgBoxType.OkOnly, 12,,, "Cannot Add", "Food subtype already exists", AgnesMessageBox.ImageType.Danger)
            amsg.ShowDialog()
            amsg.Close()
            Exit Sub
        End If

        '// Add to database
        Dim nft As New FoodSubType
        nft.Subtype = FoodName
        VendorData.FoodSubTypes.Add(nft)
        VendorData.SaveChanges()

        '// Refresh combobox list and select new food type
        PopulateFoodSubTypes()
        cbxFoodSubType.Text = FoodName
    End Sub

    Private Sub SaveButtonClicked(sender As Object, e As MouseButtonEventArgs) Handles imgSave.MouseLeftButtonDown
        If NewVendor = True Then
            SaveNewVendor()
        Else
            UpdateVendor()
        End If
    End Sub

    Private Sub SaveNewVendor()
        If ValidateEntry(cbxVendorType.SelectedIndex) = False Then Exit Sub
        Dim nv As New VendorInfo, NotifyAboutStore As Boolean = False
        With nv
            .Name = cbxVendorName.Text
            .VendorType = cbxVendorType.SelectedIndex
            .Active = True
            .InsuranceExpiration = dtpInsurance.SelectedDate
            .ContractExpiration = dtpContract.SelectedDate
        End With

        Select Case cbxVendorType.SelectedIndex
            Case 0  ' Commons Food
                With nv
                    .CAMType = cbxCamType.SelectedIndex + 1
                    .CAMStart = dtpCamStart.SelectedDate
                    .CAMAmount = curCam.SetAmount
                    .CAMAmount = percCam.SetAmount
                    .KPIType = cbxKpiType.SelectedIndex + 1
                    .KPIStart = dtpKpiStart.SelectedDate
                    .KPIAmount = curKpi.SetAmount
                    .KPIAmount = percKpi.SetAmount
                    .FoodType = GetFoodTypeId(cbxFoodType.Text)
                    .FoodSubType = GetFoodSubTypeId(cbxFoodSubType.Text)
                    .Invoice = txtInvoiceName.Text
                    .Supplier = numSupplierCode.SetAmount
                End With
                NotifyAboutStore = True
            Case 1  ' Commons Retail
                With nv
                    .CAMType = cbxCamType.SelectedIndex + 1
                    .CAMStart = dtpCamStart.SelectedDate
                    .CAMAmount = curCam.SetAmount
                    .CAMAmount = percCam.SetAmount

                    .KPIType = cbxKpiType.SelectedIndex + 1
                    .KPIStart = dtpKpiStart.SelectedDate
                    .KPIAmount = curKpi.SetAmount
                    .KPIAmount = percKpi.SetAmount
                End With

            Case 2  ' Local Brand
                With nv
                    .Invoice = cbxVendorName.Text
                    .Supplier = 99999
                    .StoreId = 99999
                    .RequiresHood = chkHood.IsChecked
                    .MaximumDailyCafes = numDailyCafes.Amount
                    .FoodType = GetFoodTypeId(cbxFoodType.Text)
                    .FoodSubType = GetFoodSubTypeId(cbxFoodSubType.Text)
                    .ProductClassId = GetProductClassId(cbxCommonsProductClass.Text)
                End With
                NotifyAboutStore = True

            Case 3  ' Food Truck
                With nv
                    .Invoice = cbxVendorName.Text
                    .Supplier = 99998
                    .StoreId = 99998
                    .FoodType = GetFoodTypeId(cbxFoodType.Text)
                    .FoodSubType = GetFoodSubTypeId(cbxFoodSubType.Text)
                    .MaximumDailyCafes = numDailyCafes.Amount
                End With

        End Select

        VendorData.VendorInfo.Add(nv)

        Try
            VendorData.VendorInfo.Add(nv)
            VendorData.SaveChanges()
            If NotifyAboutStore = True Then AddStoreNotification(nv.Name)
        Catch ex As Exception
            MsgBox("Error: " & ex.Message)
        End Try

        NewVendor = False
        PopulateVendors()
        ChangesMade = False
        VendorSched.PopulateVendors(0)
        Close()
    End Sub

    Private Sub UpdateVendor()
        If ValidateEntry(ActiveVendor.VendorType) = False Then Exit Sub
        With ActiveVendor
            .Active = (1 - cbxStatus.SelectedIndex)
            .InsuranceExpiration = dtpInsurance.SelectedDate
            .ContractExpiration = dtpContract.SelectedDate
        End With

        Select Case cbxVendorType.SelectedIndex
            Case 0  ' Commons Food
                With ActiveVendor
                    .FoodType = GetFoodTypeId(cbxFoodType.Text)
                    .FoodSubType = GetFoodSubTypeId(cbxFoodSubType.Text)
                    .Invoice = txtInvoiceName.Text
                End With

            Case 2  ' Local Brand
                With ActiveVendor
                    .RequiresHood = chkHood.IsChecked
                    .MaximumDailyCafes = numDailyCafes.Amount
                    .FoodType = GetFoodTypeId(cbxFoodType.Text)
                    .FoodSubType = GetFoodSubTypeId(cbxFoodSubType.Text)
                End With

            Case 3  ' Food Truck
                With ActiveVendor
                    .FoodType = GetFoodTypeId(cbxFoodType.Text)
                    .FoodSubType = GetFoodSubTypeId(cbxFoodSubType.Text)
                    .MaximumDailyCafes = numDailyCafes.Amount
                End With

        End Select

        Try
            VendorData.SaveChanges()
        Catch ex As Exception
            MsgBox("Error: " & ex.Message)
        End Try

        PopulateVendors()
        ChangesMade = False
        VendorSched.PopulateVendors(0)
        Close()
    End Sub

    Private Function ValidateEntry(VendType As Byte) As Boolean
        Dim ph As String = ""
        Dim errorlist As New List(Of String)
        '// Validate basics, which should never be in error except through deliberate stupidity
        If cbxVendorName.SelectedIndex = -1 Or cbxVendorName.Text = "" Then
            errorlist.Add("Vendor name missing")
        End If
        If cbxVendorType.SelectedIndex = -1 Or cbxVendorType.Text = "" Then
            errorlist.Add("Vendor type missing")
        End If
        If cbxStatus.SelectedIndex = -1 Or cbxStatus.Text = "" Then
            errorlist.Add("Vendor status missing")
        End If
        If dtpInsurance.SelectedDate Is Nothing Then
            errorlist.Add("Insurance start date missing")
        End If
        If dtpContract.SelectedDate Is Nothing Then
            errorlist.Add("Contract start date missing")
        End If

        Select Case VendType
            Case 0  ' Commons Food
                If cbxCamType.SelectedIndex = -1 Or cbxCamType.Text = "" Then
                    errorlist.Add("CAM type missing")
                End If
                Select Case cbxCamType.SelectedIndex
                    Case 0  ' None
                    Case 1  ' Percentage
                        If dtpCamStart.SelectedDate Is Nothing Then
                            errorlist.Add("CAM start date missing")
                        End If
                        If percCam.SetAmount = 0 Then
                            errorlist.Add("CAM percentage missing")
                        End If
                    Case 2  ' Flat
                        If dtpCamStart.SelectedDate Is Nothing Then
                            errorlist.Add("CAM start date missing")
                        End If
                        If curCam.SetAmount = 0 Then
                            errorlist.Add("CAM amount missing")
                        End If
                End Select

                If cbxKpiType.SelectedIndex = -1 Or cbxKpiType.Text = "" Then
                    errorlist.Add("KPI type missing")
                End If
                Select Case cbxKpiType.SelectedIndex
                    Case 0  ' None
                    Case 1  ' Percentage
                        If dtpKpiStart.SelectedDate Is Nothing Then
                            errorlist.Add("KPI start date missing")
                        End If
                        If percKpi.SetAmount = 0 Then
                            errorlist.Add("KPI percentage missing")
                        End If
                    Case 2  ' Flat
                        If dtpKpiStart.SelectedDate Is Nothing Then
                            errorlist.Add("KPI start date missing")
                        End If
                        If curKpi.SetAmount = 0 Then
                            errorlist.Add("KPI amount missing")
                        End If
                End Select

                If txtInvoiceName.Text = "" Then
                    errorlist.Add("Invoice name missing")
                End If

                If numSupplierCode.Amount = 0 Then
                    errorlist.Add("Supplier missing")
                End If

                If cbxFoodType.SelectedIndex = -1 Or cbxFoodType.Text = "" Then
                    errorlist.Add("Food type missing")
                End If

                If cbxFoodSubType.SelectedIndex = -1 Or cbxFoodSubType.Text = "" Then
                    errorlist.Add("Food subtype missing")
                End If

            Case 1  ' Commons Retail
                If cbxCamType.SelectedIndex = -1 Or cbxCamType.Text = "" Then
                    errorlist.Add("CAM type missing")
                End If
                Select Case cbxCamType.SelectedIndex
                    Case 0  ' None
                    Case 1  ' Percentage
                        If dtpCamStart.SelectedDate Is Nothing Then
                            errorlist.Add("CAM start date missing")
                        End If
                        If percCam.SetAmount = 0 Then
                            errorlist.Add("CAM percentage missing")
                        End If
                    Case 2  ' Flat
                        If dtpCamStart.SelectedDate Is Nothing Then
                            errorlist.Add("CAM start date missing")
                        End If
                        If curCam.SetAmount = 0 Then
                            errorlist.Add("CAM amount missing")
                        End If
                End Select

                If cbxKpiType.SelectedIndex = -1 Or cbxKpiType.Text = "" Then
                    errorlist.Add("KPI type missing")
                End If
                Select Case cbxKpiType.SelectedIndex
                    Case 0  ' None
                    Case 1  ' Percentage
                        If dtpKpiStart.SelectedDate Is Nothing Then
                            errorlist.Add("KPI start date missing")
                        End If
                        If percKpi.SetAmount = 0 Then
                            errorlist.Add("KPI percentage missing")
                        End If
                    Case 2  ' Flat
                        If dtpKpiStart.SelectedDate Is Nothing Then
                            errorlist.Add("KPI start date missing")
                        End If
                        If curKpi.SetAmount = 0 Then
                            errorlist.Add("KPI amount missing")
                        End If
                End Select

                If txtInvoiceName.Text = "" Then
                    errorlist.Add("Invoice name missing")
                End If

                If numSupplierCode.Amount = 0 Then
                    errorlist.Add("Supplier missing")
                End If

            Case 2  ' Local Brand
                If cbxFoodType.SelectedIndex = -1 Or cbxFoodType.Text = "" Then
                    errorlist.Add("Food type missing")
                End If

                If cbxFoodSubType.SelectedIndex = -1 Or cbxFoodSubType.Text = "" Then
                    errorlist.Add("Food subtype missing")
                End If

                If numDailyCafes.Amount = 0 Then
                    errorlist.Add("Max daily cafes missing")
                End If

                If cbxCommonsProductClass.SelectedIndex = -1 Or cbxCommonsProductClass.Text = "" Then
                    errorlist.Add("Product class selection missing")
                End If

            Case 3  ' Food Truck
                If cbxFoodType.SelectedIndex = -1 Or cbxFoodType.Text = "" Then
                    errorlist.Add("Food type missing")
                End If

                If cbxFoodSubType.SelectedIndex = -1 Or cbxFoodSubType.Text = "" Then
                    errorlist.Add("Food subtype missing")
                End If

                If numDailyCafes.Amount = 0 Then
                    errorlist.Add("Max daily locations missing")
                End If

        End Select

        If errorlist.Count > 0 Then
            Dim errmsg As String = ""
            For Each li In errorlist
                errmsg = errmsg & li & Chr(13)
            Next
            Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Medium, AgnesMessageBox.MsgBoxLayout.TextAndImage, AgnesMessageBox.MsgBoxType.OkOnly, 12,,, "Unable to save", errmsg, AgnesMessageBox.ImageType.Danger)
            amsg.ShowDialog()
            amsg.Close()
            Return False
        End If

        Return True
    End Function

    Private Sub OnceLoaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        If StartVendor <> "" Then
            StartVendorIndex = cbxVendorName.Items.IndexOf(StartVendor)
            cbxVendorName.SelectedIndex = StartVendorIndex
        End If

    End Sub

    Private Sub AddStoreNotification(vendorname)
        Dim NewNote As New Notification
        With NewNote
            .StartDate = Now()
            .EndDate = Now.AddDays(365)
            .Audience = 999
            .Message = "A new Vendor -" & vendorname & "- has been created.  A store ID required."
            .Creator = 0
            .OneOffNotification = False
            .Snooze = True
            .RequireConfirm = False
            .Dismissable = True
        End With
        Try
            AGNESShared.Notifications.Add(NewNote)
            AGNESShared.SaveChanges()
        Catch ex As Exception

        End Try
    End Sub

    Private Function VerifyDiscardChanges() As Boolean
        Dim amsg = New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Small, AgnesMessageBox.MsgBoxLayout.TextAndImage, AgnesMessageBox.MsgBoxType.YesNo, 12,,, "Discard changes?", "You have unsaved changes.  Continue and discard?", AgnesMessageBox.ImageType.Alert)
        amsg.ShowDialog()
        If amsg.ReturnResult = "No" Then
            ChangeOverride = True
            cbxVendorName.SelectedIndex = VendorIndex
            ChangeOverride = False
            amsg.Close()
            Return False
        Else
            amsg.Close()
        End If
        Return True
    End Function


#End Region

End Class
