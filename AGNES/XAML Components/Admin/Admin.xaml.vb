﻿Imports System.ComponentModel
Imports System.Linq
Public Class Admin

#Region "Properties"
    Private RecordExists As Boolean
    Private UserId As Long
    Private SaveError As Boolean
#End Region

#Region "Constructor"
    Public Sub New()
        InitializeComponent()
        LoadAccessLevels()
        LoadModules()
        LoadFlashTypes()
        LoadUnits()
        LoadUsers()
    End Sub

#End Region

#Region "Private Methods"
    Private Sub LoadAccessLevels()
        Dim qal = From ual In AGNESShared.AccessLevels
                  Select ual

        For Each ual In qal
            Dim cbi As New ComboBoxItem With {.Content = ual.AccessLevel1}
            cbxAccess.Items.Add(cbi)
        Next
    End Sub

    Private Sub LoadModules()
        lbxAvailableModules.Items.Clear()
        Dim qam = From modl In AGNESShared.Modules
                  Select modl

        For Each modl In qam
            Dim li As New ModuleListItem With {.Content = modl.ModuleName, .ModuleId = modl.PID, .RequiresFlash = modl.RequiresFlashType, .RequiresUnit = modl.RequiresUnitAccess}
            AddHandler li.MouseDoubleClick, AddressOf ModuleSelected
            lbxAvailableModules.Items.Add(li)
        Next
        lbxAvailableModules.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))
    End Sub

    Private Sub LoadFlashTypes()
        Dim qft = From ft In AGNESShared.FlashTypes
                  Select ft

        For Each ft In qft
            Dim cbi As New ComboBoxItem With {.Content = ft.FlashType1}
            cbxFlashType.Items.Add(cbi)
        Next
    End Sub

    Private Sub LoadUnits()
        lbxAvailableUnits.Items.Clear()
        Dim qau = From units In SharedDataGroup.LOCATIONS
                  Where units.FlashType <> 0
                  Select units

        For Each units In qau
            Dim li As New ListBoxItem With {.Content = units.Unit_Number, .ToolTip = units.Unit & " | " & units.profit_center_name &
                " | " & units.Group}
            AddHandler li.MouseDoubleClick, AddressOf UnitSelected
            lbxAvailableUnits.Items.Add(li)
        Next
        lbxAvailableUnits.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))

    End Sub

    Private Sub LoadUsers()
        Dim qlu = From usr In AGNESShared.Users
                  Select usr

        For Each usr In qlu
            Dim lbi As New ListBoxItem With {.Content = usr.UserName}
            AddHandler lbi.MouseDoubleClick, AddressOf UserSelected
            lbxUsers.Items.Add(lbi)
        Next
        lbxUsers.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))
    End Sub

    Private Sub UserSelected(sender As Object, e As MouseEventArgs)
        'TODO:  ADD ROUTINE TO SHOW LIST OF THE SELECTED USER'S CURRENT ITEMS
        Dim s As ListBoxItem = sender
        lbxAccessibleModules.Items.Clear()
        lbxAccessibleUnits.Items.Clear()
        LoadModules()
        LoadUnits()
        PopulateUserInfo(s.Content)
        RecordExists = True
    End Sub

    Private Sub ModuleSelected(sender As Object, e As MouseEventArgs)
        Dim s As ModuleListItem = sender
        Dim nli As New ModuleListItem With {.Content = s.Content, .ModuleId = s.ModuleId, .RequiresFlash = s.RequiresFlash, .RequiresUnit = s.RequiresUnit}

        AddHandler nli.MouseDoubleClick, AddressOf ModuleDeselected
        lbxAccessibleModules.Items.Add(nli)
        lbxAvailableModules.Items.Remove(s)
        lbxAccessibleModules.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))
        cbxFlashType.IsEnabled = nli.RequiresFlash
        lbxAccessibleUnits.IsEnabled = nli.RequiresUnit
        lbxAvailableUnits.IsEnabled = nli.RequiresUnit
    End Sub

    Private Sub ModuleDeselected(sender As Object, e As MouseEventArgs)
        Dim s As ModuleListItem = sender
        Dim nli As New ModuleListItem With {.Content = s.Content, .ModuleId = s.ModuleId, .RequiresFlash = s.RequiresFlash, .RequiresUnit = s.RequiresUnit}
        AddHandler nli.MouseDoubleClick, AddressOf ModuleSelected
        lbxAvailableModules.Items.Add(nli)
        lbxAccessibleModules.Items.Remove(s)
        lbxAvailableModules.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))
        cbxFlashType.IsEnabled = nli.RequiresFlash
        lbxAccessibleUnits.IsEnabled = nli.RequiresUnit
        lbxAvailableUnits.IsEnabled = nli.RequiresUnit
    End Sub

    Private Sub UnitSelected(sender As Object, e As MouseEventArgs)
        Dim s As ListBoxItem = sender
        Dim nli As New ListBoxItem With {.Content = s.Content}
        AddHandler nli.MouseDoubleClick, AddressOf UnitDeselected
        lbxAccessibleUnits.Items.Add(nli)
        lbxAvailableUnits.Items.Remove(s)
        lbxAccessibleUnits.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))
    End Sub

    Private Sub UnitDeselected(sender As Object, e As MouseEventArgs)
        Dim s As ListBoxItem = sender
        Dim nli As New ListBoxItem With {.Content = s.Content}
        AddHandler nli.MouseDoubleClick, AddressOf UnitSelected
        lbxAvailableUnits.Items.Add(nli)
        lbxAccessibleUnits.Items.Remove(s)
        lbxAvailableUnits.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))

    End Sub

    Private Sub PopulateUserInfo(usrnm As String)
        '// Get user ID for use in pulling other data and populate info fields
        Dim tmpUserId As Long, tmpAccessLvlId As Byte, tmpAccessDesc As String = ""
        Dim qui = From usr In AGNESShared.Users
                  Where usr.UserName = usrnm
                  Select usr

        For Each usr In qui
            tmpUserId = usr.PID
            tmpAccessLvlId = usr.AccessLevelId
            txtAlias.Text = usr.UserAlias
            txtFirstName.Text = usr.FirstName
            txtLastName.Text = usr.LastName
            txtSpokenName.Text = usr.SpokenName
        Next

        Dim qal = From ual In AGNESShared.AccessLevels
                  Where ual.PID = tmpAccessLvlId
                  Select ual

        For Each ual In qal
            tmpAccessDesc = ual.AccessLevel1
        Next

        For Each i As ComboBoxItem In cbxAccess.Items
            If i.Content = tmpAccessDesc Then
                i.IsSelected = True
            Else
                i.IsSelected = False
            End If
        Next

    End Sub

    Private Sub SaveRecord(sender As Object, e As RoutedEventArgs) Handles btnSave.Click
        SaveError = False
        ValidateInfo()
        If SaveError = True Then Exit Sub
        SaveUserInfo()
        If cbxAccess.SelectedIndex = 3 Then
            SaveModuleInfo()
            SaveFlashType()
            SaveUnitInfo()
        End If
        ClearInfo()
    End Sub

    Private Sub btnClear_Click(sender As Object, e As RoutedEventArgs) Handles btnClear.Click
        ClearInfo()
    End Sub

    Private Sub ClearInfo()
        RecordExists = False
        txtAlias.Text = ""
        txtFirstName.Text = ""
        txtLastName.Text = ""
        txtSpokenName.Text = ""
        cbxAccess.SelectedIndex = -1
        UserId = 0
        lbxAccessibleModules.Items.Clear()
        lbxAccessibleUnits.Items.Clear()
        lbxAccessibleUnits.IsEnabled = False
        lbxAvailableUnits.IsEnabled = False
        cbxFlashType.IsEnabled = False
        cbxFlashType.SelectedIndex = -1
        LoadModules()
        LoadUnits()
    End Sub

    Private Sub ValidateInfo()

        '// Check for required user info
        If txtFirstName.Text = "" Or txtLastName.Text = "" Or txtSpokenName.Text = "" Or txtAlias.Text = "" Then
            Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Small, AgnesMessageBox.MsgBoxLayout.FullText, AgnesMessageBox.MsgBoxType.OkOnly, 12,, "Cannot save",, "You are missing user information.")
            amsg.ShowDialog()
            amsg.Close()
            SaveError = True
            Exit Sub
        End If

        '// Check for access level
        If cbxAccess.SelectedIndex = -1 Then
            Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Small, AgnesMessageBox.MsgBoxLayout.FullText, AgnesMessageBox.MsgBoxType.OkOnly, 12,, "Cannot save",, "You must choose an access level.")
            amsg.ShowDialog()
            amsg.Close()
            SaveError = True
            Exit Sub
        End If

        If cbxAccess.SelectedIndex = 3 Then

            '// Check for minimum number of modules (3)
            If lbxAccessibleModules.Items.Count < 3 Then
                Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Small, AgnesMessageBox.MsgBoxLayout.FullText, AgnesMessageBox.MsgBoxType.OkOnly, 12,, "Cannot save",, "Users must have a minimum of three modules available.")
                amsg.ShowDialog()
                amsg.Close()
                SaveError = True
                Exit Sub
            End If

            '// Iterate through items and check for Flash type and Unit access, if required
            For Each li As ModuleListItem In lbxAccessibleModules.Items
                If li.RequiresFlash = True Then
                    If cbxFlashType.SelectedIndex = -1 Then
                        Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Small, AgnesMessageBox.MsgBoxLayout.FullText, AgnesMessageBox.MsgBoxType.OkOnly, 12,, "Cannot save",, "At least one of the modules selected requires a flash type.")
                        amsg.ShowDialog()
                        amsg.Close()
                        SaveError = True
                        Exit Sub
                    End If
                End If
                If li.RequiresUnit = True Then
                    If lbxAccessibleUnits.Items.Count = 0 Then
                        Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Small, AgnesMessageBox.MsgBoxLayout.FullText, AgnesMessageBox.MsgBoxType.OkOnly, 12,, "Cannot save",, "At least one of the modules selected requires access to at least one unit.")
                        amsg.ShowDialog()
                        amsg.Close()
                        SaveError = True
                        Exit Sub
                    End If
                End If
            Next

        End If
    End Sub

    Private Sub SaveUserInfo()
        Select Case RecordExists
            Case True       ' Update existing record
                Dim uu = (From user In AGNESShared.Users
                          Where user.UserAlias = txtAlias.Text).ToList()(0)
                With uu
                    .UserName = txtFirstName.Text & " " & txtLastName.Text
                    .UserAlias = TruncateAlias(txtAlias.Text)
                    .FirstName = txtFirstName.Text
                    .LastName = txtLastName.Text
                    .SpokenName = txtSpokenName.Text
                    .AccessLevelId = cbxAccess.SelectedIndex + 1
                    .DateAdded = Now()
                    .SavedBy = My.Settings.UserName
                End With
                txtAlias.Text = TruncateAlias(txtAlias.Text)

            Case False      ' Create new record
                Dim nu As New User
                With nu
                    .UserName = txtFirstName.Text & " " & txtLastName.Text
                    .UserAlias = TruncateAlias(txtAlias.Text)
                    .FirstName = txtFirstName.Text
                    .LastName = txtLastName.Text
                    .SpokenName = txtSpokenName.Text
                    .AccessLevelId = cbxAccess.SelectedIndex + 1
                    .DateAdded = Now()
                    .SavedBy = My.Settings.UserName
                End With
                AGNESShared.Users.Add(nu)
                txtAlias.Text = TruncateAlias(txtAlias.Text)
        End Select
        AGNESShared.SaveChanges()

        Dim ui = (From user In AGNESShared.Users
                  Where user.UserAlias = txtAlias.Text).ToList()(0)
        UserId = ui.PID
        lbxUsers.Items.Clear()
        LoadUsers()
    End Sub

    Private Sub SaveModuleInfo()
        Dim ModuleId As Long
        For Each li As ListBoxItem In lbxAccessibleModules.Items
            '// Get module id from name in listbox
            Dim mi = (From modul In AGNESShared.Modules
                      Where modul.ModuleName = li.Content.ToString).ToList()(0)
            ModuleId = mi.PID

            '// Check to see if the module-user join already exists
            Try
                Dim jc = (From joincheck In AGNESShared.ModulesUsers_Join
                          Where joincheck.ModuleId = ModuleId And
                              joincheck.UserId = UserId).ToList()(0)

            Catch ex As Exception   ' Join does not exist
                Dim nj As New ModulesUsers_Join
                With nj
                    .UserId = UserId
                    .ModuleId = ModuleId
                End With
                AGNESShared.ModulesUsers_Join.Add(nj)
            End Try
        Next
        AGNESShared.SaveChanges()
    End Sub

    Private Sub SaveFlashType()
        Dim FlashId As Long
        If cbxFlashType.IsEnabled = True Then
            '// Get flashtype id from name in combobox
            Dim li As ComboBoxItem = cbxFlashType.SelectedItem
            Dim selectedflashtype As String = li.Content.ToString
            Dim ft = (From flashtype In AGNESShared.FlashTypes
                      Where flashtype.FlashType1 = selectedflashtype).ToList()(0)
            FlashId = ft.PID

            '// Check to see if the flashtype-user join already exists
            Try
                Dim jc = (From joincheck In AGNESShared.FlashTypesUsers_Join
                          Where joincheck.FlashId = FlashId And
                              joincheck.UserId = UserId).ToList()(0)

            Catch ex As Exception   ' Join does not exist
                Dim nj As New FlashTypesUsers_Join
                With nj
                    .UserId = UserId
                    .FlashId = FlashId
                End With
                AGNESShared.FlashTypesUsers_Join.Add(nj)
                AGNESShared.SaveChanges()
            End Try
        End If
    End Sub

    Private Sub SaveUnitInfo()
        Dim UnitId As Long
        For Each li As ListBoxItem In lbxAccessibleUnits.Items
            '// Get unit id from listbox item
            UnitId = FormatNumber(li.Content.ToString, 0)

            '// Check to see if the unit-user join already exists
            Try
                Dim jc = (From joincheck In AGNESShared.UnitsUsers_Join
                          Where joincheck.UnitNumber = UnitId And
                              joincheck.UserId = UserId).ToList()(0)

            Catch ex As Exception   ' Join does not exist
                Dim nj As New UnitsUsers_Join
                With nj
                    .UserId = UserId
                    .UnitNumber = UnitId
                    .Delegate = 0
                End With
                AGNESShared.UnitsUsers_Join.Add(nj)

            End Try
        Next
        AGNESShared.SaveChanges()
    End Sub

#End Region

End Class
