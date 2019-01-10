Public Class VendorEditor

#Region "Properties"

    Public numSupplierCode As NumberBox
    Public numDailyCafes As NumberBox
    Public curKpi As CurrencyBox
    Public curCam As CurrencyBox
    Public ChangesMade As Boolean

    Private ActiveVendor As VendorInfo

#End Region

#Region "Constructor"
    Public Sub New()
        InitializeComponent()
        AddInitialCustomFields()
        PopulateVendors()
        CollapseForm(0)
        Height = 100
    End Sub
#End Region

#Region "Public Methods"

#End Region

#Region "Private Methods"
    Private Sub AddInitialCustomFields()

        '// Add numbox for supplier code
        numSupplierCode = New NumberBox(125, True, False, True, False, True, AgnesBaseInput.FontSz.Standard) With {.Margin = New Thickness(227, 26, 0, 0)}
        grdSupplierInfo.Children.Add(numSupplierCode)

        '// Add numbox for maximum number of daily cafes
        numDailyCafes = New NumberBox(90, True, False, True, False, True, AgnesBaseInput.FontSz.Standard) With {.Margin = New Thickness(258, 27, 0, 0)}
        grdBrandDetail.Children.Add(numDailyCafes)

        '// Add CAM amount currency box
        curCam = New CurrencyBox(82, True, AgnesBaseInput.FontSz.Standard,, True, False) With {.Margin = New Thickness(361, 31, 0, 0)}
        grdCamKpi.Children.Add(curCam)

        '// Add KPI amount currency box
        curKpi = New CurrencyBox(82, True, AgnesBaseInput.FontSz.Standard,, True, False) With {.Margin = New Thickness(361, 77, 0, 0)}
        grdCamKpi.Children.Add(curKpi)

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

    Private Sub VendorSelected(sender As Object, e As SelectionChangedEventArgs) Handles cbxVendorName.SelectionChanged
        Select Case cbxVendorName.SelectedIndex
            Case -1 ' Deselected - clear and disable everything
                CollapseForm(0)
            Case 0  ' New vendor entry

            Case Else ' Existing vendor selected
                Dim vndnm As String = cbxVendorName.SelectedValue
                vndnm = Mid(vndnm, 1, vndnm.IndexOf("[") - 1)

                Dim qav = (From av In VendorData.VendorInfo
                           Where av.Name = vndnm
                           Select av).ToList(0)
                CollapseForm(1)
                DisplayForm(qav)

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

    Private Sub DisplayForm(qav As VendorInfo)
        cbxVendorType.SelectedIndex = qav.VendorType
        Select Case qav.VendorType
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

#End Region

End Class
