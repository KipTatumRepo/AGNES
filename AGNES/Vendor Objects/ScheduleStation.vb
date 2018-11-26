Public Class ScheduleStation
    Inherits Border

#Region "Properties"
    Public VendorStack As StackPanel
    Public Property StationName As String
    Public Property StationNumber As Byte
    Public Property StationBlock As TextBlock
    Private Property DropAllowed As Boolean = True
    Private StatusBarText As String
#End Region

#Region "Constructor"
    Public Sub New(sn)
        AllowDrop = True
        Height = 16
        BorderBrush = Brushes.Black
        BorderThickness = New Thickness(1, 1, 1, 1)
        Margin = New Thickness(1, 1, 1, 0)
        StationNumber = sn
        StationName = "Station " & StationNumber
        VendorStack = New StackPanel
        Child = VendorStack
        AddName()
    End Sub

#End Region

#Region "Public Methods"
    Public Sub DeleteItem(ByRef v As VendorInStation)
        v.ReferencedVendor.UsedSlots -= 1
        VendorStack.Children.Remove(v)
        Height -= 16
    End Sub

#End Region

#Region "Private Methods"
    Private Sub AddName()
        StationBlock = New TextBlock With {.TextAlignment = TextAlignment.Center, .Text = StationName, .FontSize = 10}
        VendorStack.Children.Add(StationBlock)
    End Sub

    Private Sub ScheduleStation_DragEnter(sender As Object, e As DragEventArgs) Handles Me.DragEnter
        StatusBarText = VendorSched.tbSaveStatus.Text
        CheckVendorDrag()

    End Sub

    Private Sub ScheduleStation_Drop(sender As Object, e As DragEventArgs) Handles Me.Drop
        If DropAllowed = False Then
            VendorSched.tbSaveStatus.Text = StatusBarText
            Exit Sub
        End If

        Dim nv As New VendorInStation With {.TextAlignment = TextAlignment.Center, .Text = e.Data.GetData(DataFormats.Text),
        .ReferencedVendor = VendorSched.ActiveVendor, .ReferencedLocation = Me, .FontSize = 12}
        nv.Background = Brushes.LightGray
        VendorStack.Children.Add(nv)
        Height += 16
        VendorSched.tbSaveStatus.Text = StatusBarText
        VendorSched.sbSaveStatus.Background = Brushes.LightGreen
        VendorSched.ActiveVendor.UsedSlots += 1
        VendorSched.ActiveVendor = Nothing
    End Sub

    Private Sub ScheduleStation_DragLeave(sender As Object, e As DragEventArgs) Handles Me.DragLeave
        VendorSched.tbSaveStatus.Text = StatusBarText
        VendorSched.sbSaveStatus.Background = Brushes.LightGreen
    End Sub

    Private Sub CheckVendorDrag()
        'Validation routines to preemptively notify about whether vendor is allowed to be scheduled
        If StationAvailable() = False Then
            DropAllowed = False
            Exit Sub
        End If
        If CheckVendorTypeAllowed() = False Then
            DropAllowed = False
            Exit Sub
        End If
        If CheckMaxVendors() = False Then
            DropAllowed = False
            Exit Sub
        End If
        If CheckVendorPrereqs() = False Then
            DropAllowed = False
            Exit Sub
        End If
        If CheckFoodTypeConflicts() = False Then
            DropAllowed = True
            Exit Sub
        End If
        VendorSched.tbSaveStatus.Text = "Okay to add"
    End Sub

    Private Function StationAvailable()
        If VendorStack.Children.Count > 1 Then
            VendorSched.tbSaveStatus.Text = "Only one vendor can be added to a station."
            VendorSched.sbSaveStatus.Background = Brushes.PaleVioletRed
            Return False
        End If
        Return True
    End Function

    Private Function CheckVendorTypeAllowed()
        '// Is the vendor type (truck or brand) allowed at the building?

        '//TEST//
        'If LocationName = "Building 92" Then
        '    VendorSched.tbSaveStatus.Text = "The vendor type is not allowed at this building"
        '    VendorSched.sbSaveStatus.Background = Brushes.PaleVioletRed
        '    Return False
        'End If
        '//TEST//

        Return True
    End Function

    Private Function CheckMaxVendors()
        '// Would adding the vendor exceed the max number of trucks or local brands allowed at the unit?
        '// Possibly allow replacing what's already there?
        Return True
    End Function

    Private Function CheckVendorPrereqs()
        '// Conflicts such as the requirement for a hood at a unit that does not have one available
        Return True
    End Function

    Private Function CheckFoodTypeConflicts()
        '// Cautionary alert if food type conflicts with an anchor station, another vendor present at the same time, or a food
        '// type scheduled on adjacent days (the last one should be fun to code. :| )
        'Dim ft As Byte = VendorSched.ActiveVendor.FoodType
        'For Each vil As Object In VendorStack.Children
        '    Try
        '        Dim vtmp As VendorInStation
        '        vtmp = CType(vil, VendorInStation)
        '        If vtmp.ReferencedVendor.FoodType = ft Then
        '            If vtmp.ReferencedVendor.VendorItem.Name = VendorSched.ActiveVendor.VendorItem.Name Then
        '                VendorSched.tbSaveStatus.Text = "This vendor is already present at this location on this day"
        '            Else
        '                VendorSched.tbSaveStatus.Text = "This food type conflicts with another vendor present on the same day"
        '            End If
        '            VendorSched.sbSaveStatus.Background = Brushes.LightYellow
        '            Return False
        '        End If
        '    Catch ex As Exception

        '    End Try
        'Next
        Return True
    End Function

#End Region

End Class
