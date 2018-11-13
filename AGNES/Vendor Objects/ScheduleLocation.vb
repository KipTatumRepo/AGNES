Imports System.ComponentModel
Public Class ScheduleLocation
    Inherits Border

#Region "Properties"
    Public VendorStack As StackPanel
    Public Property LocationName As String
    Public Property LocationBlock As TextBlock
    Private Property HighlightColor As Boolean = True
    Private Property DropAllowed As Boolean = True
    Private StatusBarText As String
#End Region

#Region "Constructor"
    Public Sub New(locname)
        AllowDrop = True
        Height = 25
        BorderBrush = Brushes.Black
        BorderThickness = New Thickness(1, 1, 1, 1)
        Margin = New Thickness(1, 1, 1, 0)
        LocationName = locname
        VendorStack = New StackPanel
        Child = VendorStack
        AddName()
    End Sub

#End Region

#Region "Public Methods"
    Public Sub DeleteItem(ByRef v As VendorInLoc)
        v.ReferencedVendor.UsedSlots -= 1
        VendorStack.Children.Remove(v)
        Height -= 16
    End Sub

#End Region

#Region "Private Methods"
    Private Sub AddName()
        LocationBlock = New TextBlock With {.TextAlignment = TextAlignment.Center, .Text = LocationName, .Background = Brushes.LightYellow}
        VendorStack.Children.Add(LocationBlock)
    End Sub

    Private Sub ScheduleLocation_DragEnter(sender As Object, e As DragEventArgs) Handles Me.DragEnter
        StatusBarText = VendorSched.tbSaveStatus.Text
        CheckVendorDrag()

    End Sub

    Private Sub ScheduleLocation_Drop(sender As Object, e As DragEventArgs) Handles Me.Drop
        If DropAllowed = False Then
            VendorSched.tbSaveStatus.Text = StatusBarText
            Exit Sub
        End If

        Dim nv As New VendorInLoc With {.TextAlignment = TextAlignment.Center, .Text = e.Data.GetData(DataFormats.Text),
        .ReferencedVendor = VendorSched.ActiveVendor, .ReferencedLocation = Me}
        If HighlightColor = True Then nv.Background = Brushes.LightGray
        HighlightColor = Not HighlightColor
        VendorStack.Children.Add(nv)
        Height += 16
        VendorSched.tbSaveStatus.Text = StatusBarText
        VendorSched.sbSaveStatus.Background = Brushes.LightGreen
        VendorSched.ActiveVendor.UsedSlots += 1
        VendorSched.ActiveVendor = Nothing
    End Sub

    Private Sub ScheduleLocation_DragLeave(sender As Object, e As DragEventArgs) Handles Me.DragLeave
        VendorSched.tbSaveStatus.Text = StatusBarText
        VendorSched.sbSaveStatus.Background = Brushes.LightGreen
    End Sub

    Private Sub CheckVendorDrag()
        'Validation routines to preemptively notify about whether vendor is allowed to be scheduled
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

    Private Function CheckVendorTypeAllowed()
        '// Is the vendor type (truck or brand) allowed at the building?

        '//TEST//
        If LocationName = "Building 92" Then
            VendorSched.tbSaveStatus.Text = "The vendor type is not allowed at this building"
            VendorSched.sbSaveStatus.Background = Brushes.PaleVioletRed
            Return False
        End If
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
        Dim ft As Byte = VendorSched.ActiveVendor.FoodType
        For Each vil As Object In VendorStack.Children
            Try
                Dim vtmp As VendorInLoc
                vtmp = CType(vil, VendorInLoc)
                If vtmp.ReferencedVendor.FoodType = ft Then
                    If vtmp.ReferencedVendor.VendorItem.Name = VendorSched.ActiveVendor.VendorItem.Name Then
                        VendorSched.tbSaveStatus.Text = "This vendor is already present at this location on this day"
                    Else
                        VendorSched.tbSaveStatus.Text = "This food type conflicts with another vendor present on the same day"
                    End If
                    VendorSched.sbSaveStatus.Background = Brushes.LightYellow
                    Return False
                End If
            Catch ex As Exception

            End Try
        Next
        Return True
    End Function

#End Region

End Class
