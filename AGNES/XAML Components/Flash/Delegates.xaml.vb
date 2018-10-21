Imports System.ComponentModel

Public Class Delegates

#Region "Properties"
    Public Property UnitNumber As Long
#End Region

#Region "Constructor"
    Public Sub New(un As Long)
        InitializeComponent()
        txtInfo.Text = "Delegates have full access to the current unit (they must be assigned unit by unit).  You may add and remove them at your convenience by double clicking any user to transfer them between the list boxes.  " &
        "If someone you wish to add does not appear on the list, they either have no AGNES access or have full access to all " &
        "units already (DMs, for example)"
        UnitNumber = un
        LoadAvailableandAssignedUsers()
    End Sub

#End Region

#Region "Private Methods"
    Private Sub LoadAvailableandAssignedUsers()
        Dim qau = From uau In AGNESShared.Users
                  Select uau
                  Where uau.AccessLevelId = 4 And uau.PID <> My.Settings.UserID

        For Each uau In qau
            Dim aha = From uha In AGNESShared.UnitsUsers_Join
                      Select uha
                      Where uha.UnitNumber = UnitNumber And uha.UserId = uau.PID And uha.Delegate = True

            Dim cbi As New ListBoxItem With {.Content = uau.UserName, .Tag = uau.PID}

            If aha.Count = 0 Then
                lbxAvailable.Items.Add(cbi)
                AddHandler cbi.MouseDoubleClick, AddressOf AddDelegate
            Else
                lbxDelegates.Items.Add(cbi)
                AddHandler cbi.MouseDoubleClick, AddressOf RemoveDelegate
            End If
        Next
        lbxAvailable.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))
        If lbxDelegates.Items.Count > 0 Then lbxDelegates.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))
    End Sub

    Private Sub AddDelegate(sender As ListBoxItem, e As MouseEventArgs)
        Dim lbi As New ListBoxItem With {.Content = sender.Content, .Tag = sender.Tag}
        lbxDelegates.Items.Add(lbi)
        AddHandler lbi.MouseDoubleClick, AddressOf RemoveDelegate
        lbxAvailable.Items.Remove(sender)
        lbxDelegates.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))
    End Sub

    Private Sub RemoveDelegate(sender As ListBoxItem, e As MouseEventArgs)
        Dim lbi As New ListBoxItem With {.Content = sender.Content, .Tag = sender.Tag}
        lbxAvailable.Items.Add(lbi)
        AddHandler lbi.MouseDoubleClick, AddressOf AddDelegate
        lbxDelegates.Items.Remove(sender)
        lbxAvailable.Items.SortDescriptions.Add(New SortDescription("Content", ListSortDirection.Ascending))
    End Sub

    Private Sub SaveDelegates(sender As Object, e As RoutedEventArgs) Handles btnSave.Click
        Try
            RemoveAllDelegates()
            AddSelectedDelegates()
            AGNESShared.SaveChanges()
            Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Small, AgnesMessageBox.MsgBoxLayout.BottomOnly, AgnesMessageBox.MsgBoxType.OkOnly, 12,,,, "Save Successful")
            amsg.ShowDialog()
            amsg.Close()
            GC.Collect()
            Close()
        Catch ex As Exception
            Dim amsg As New AgnesMessageBox(AgnesMessageBox.MsgBoxSize.Small, AgnesMessageBox.MsgBoxLayout.FullText, AgnesMessageBox.MsgBoxType.OkOnly, 12,, "Save Unsuccessful",, "Error: " & ex.Message)
            amsg.ShowDialog()
            amsg.Close()
            GC.Collect()
        End Try
    End Sub

    Private Sub RemoveAllDelegates()
        Try
            Dim qdu = From Del In AGNESShared.UnitsUsers_Join
                      Select Del
                      Where Del.UnitNumber = UnitNumber And Del.Delegate = True

            For Each del In qdu
                AGNESShared.UnitsUsers_Join.Remove(del)
            Next

        Catch ex As Exception
            '// Nothing in table
        End Try


    End Sub

    Private Sub AddSelectedDelegates()
        For Each lbi As ListBoxItem In lbxDelegates.Items
            Dim uuj As New UnitsUsers_Join, uid As Long = Long.Parse(lbi.Tag)
            With uuj
                .UnitNumber = UnitNumber
                .UserId = uid
                .Delegate = True
                .DelegateAddedBy = My.Settings.UserID
            End With
            AGNESShared.UnitsUsers_Join.Add(uuj)
        Next
    End Sub
#End Region

End Class
