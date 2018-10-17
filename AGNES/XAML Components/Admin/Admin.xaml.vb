Public Class Admin
    Public Sub New()
        InitializeComponent()
        LoadAccessLevels()
        LoadModules()
        LoadUnits()
        LoadUsers()
    End Sub
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
            Dim li As New ListBoxItem With {.Content = modl.ModuleName}
            AddHandler li.MouseDoubleClick, AddressOf ModuleSelected
            lbxAvailableModules.Items.Add(li)
        Next
    End Sub

    Private Sub LoadUnits()
        'lbxAvailableUnits.Items.Clear()
        'Dim qam = From modl In AGNESShared.unit
        '          Select modl

        'For Each modl In qam
        '    Dim li As New ListBoxItem With {.Content = modl.ModuleName}
        '    AddHandler li.MouseDoubleClick, AddressOf ModuleSelected
        '    lbxAvailableUnits.Items.Add(li)
        'Next
    End Sub
    Private Sub LoadUsers()
        Dim qlu = From usr In AGNESShared.Users
                  Select usr

        For Each usr In qlu
            Dim lbi As New ListBoxItem With {.Content = usr.UserName}
            AddHandler lbi.MouseDoubleClick, AddressOf UserSelected
            lbxUsers.Items.Add(lbi)
        Next
    End Sub

    Private Sub UserSelected(sender As Object, e As MouseEventArgs)
        Dim s As ListBoxItem = sender
        PopulateUserInfo(s.Content)
    End Sub

    Private Sub ModuleSelected(sender As Object, e As MouseEventArgs)
        Dim s As ListBoxItem = sender
        Dim nli As New ListBoxItem With {.Content = s.Content}
        AddHandler nli.MouseDoubleClick, AddressOf ModuleDeselected
        lbxAccessibleModules.Items.Add(nli)
        lbxAvailableModules.Items.Remove(s)
    End Sub

    Private Sub ModuleDeselected(sender As Object, e As MouseEventArgs)
        Dim s As ListBoxItem = sender
        Dim nli As New ListBoxItem With {.Content = s.Content}
        AddHandler nli.MouseDoubleClick, AddressOf ModuleSelected
        lbxAvailableModules.Items.Add(nli)
        lbxAccessibleModules.Items.Remove(s)
    End Sub

    Private Sub UnitSelected(sender As Object, e As MouseEventArgs)
        Dim s As ListBoxItem = sender
        Dim nli As New ListBoxItem With {.Content = s.Content}
        AddHandler nli.MouseDoubleClick, AddressOf UnitDeselected
        lbxAccessibleUnits.Items.Add(nli)
        lbxAvailableUnits.Items.Remove(s)
    End Sub

    Private Sub UnitDeselected(sender As Object, e As MouseEventArgs)
        Dim s As ListBoxItem = sender
        Dim nli As New ListBoxItem With {.Content = s.Content}
        AddHandler nli.MouseDoubleClick, AddressOf UnitSelected
        lbxAvailableUnits.Items.Add(nli)
        lbxAccessibleUnits.Items.Remove(s)
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
End Class
