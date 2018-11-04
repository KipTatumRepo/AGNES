Imports System.ComponentModel
Public Class UnitChooser
    'TODO:  ADD MULTISELECT FUNCTIONALITY TO UNIT CHOOSER
    Inherits DockPanel
    Implements INotifyPropertyChanged

#Region "Properties"
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Private Week As WeekChooser

    Public Property HeldUnit As Long
    Public Property NumberOfAvailableUnits As Byte
    Public Property AllowMultiSelect As Boolean
    Public Property SelectedCount As Byte

    Private _currentunit As Long
    Public Property CurrentUnit As Long
        Get
            Return _currentunit
        End Get
        Set(value As Long)
            HeldUnit = _currentunit
            _currentunit = value
            If value > 0 Then
                SelectedCount = 0
                For Each b As Border In Children
                    If b.Tag <> "Label" Then
                        Dim tb As TextBlock = b.Child
                        If FormatNumber(tb.Text, 0) <> value Then
                            tb.Foreground = Brushes.LightGray
                            tb.FontSize = 12
                            tb.FontWeight = FontWeights.Normal
                        Else
                            tb.FontWeight = FontWeights.SemiBold
                            tb.Foreground = Brushes.Black
                            tb.FontSize = 16
                            If b.IsEnabled = True Then SelectedCount += 1
                        End If
                    End If
                Next
            End If
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(“Unit”))
        End Set
    End Property

#End Region

#Region "Constructor"
    Public Sub New(ByRef ListOfUnits As UnitGroup)

        Dim ct As Byte
        '// Create chooser label
        Dim BorderLabel As New Border With {.BorderBrush = Brushes.Black, .VerticalAlignment = VerticalAlignment.Center,
            .Name = "brdLabel", .Tag = "Label"}
        Dim TextLabel As New TextBlock With {.Text = "  Unit: ", .TextAlignment = TextAlignment.Center,
        .HorizontalAlignment = HorizontalAlignment.Center, .FontSize = 14, .Name = "tbLabel", .Tag = "Label"}
        BorderLabel.Child = TextLabel
        Children.Add(BorderLabel)
        Select Case ListOfUnits.Summoner
            Case 0
                NumberOfAvailableUnits = ListOfUnits.UnitsInGroup.Count
            Case 1
                NumberOfAvailableUnits = ListOfUnits.FcastUnitsInGroup.Count
        End Select

        For ct = 0 To NumberOfAvailableUnits - 1
            Dim unitnumber As Long
            Select Case ListOfUnits.Summoner
                Case 0
                    unitnumber = ListOfUnits.UnitsInGroup(ct).UnitNumber
                Case 1
                    unitnumber = ListOfUnits.FcastUnitsInGroup(ct).UnitNumber
            End Select

            Dim brdUnit As New Border With {.BorderBrush = Brushes.Black, .Width = 64, .VerticalAlignment = VerticalAlignment.Center,
            .Name = "brd" & unitnumber}
            Dim tbUnit As New TextBlock With {.Text = unitnumber, .TextAlignment = TextAlignment.Center, .HorizontalAlignment = HorizontalAlignment.Center,
                .FontSize = 12, .Tag = unitnumber, .Name = "tbP" & unitnumber}
            AddHandler brdUnit.MouseEnter, AddressOf HoverOverUnit
            AddHandler tbUnit.MouseEnter, AddressOf HoverOverUnit
            AddHandler brdUnit.MouseLeave, AddressOf LeaveUnit
            AddHandler tbUnit.MouseLeave, AddressOf LeaveUnit
            AddHandler tbUnit.PreviewMouseDown, AddressOf ChooseUnit
            brdUnit.Child = tbUnit
            Children.Add(brdUnit)
        Next
        Select Case ListOfUnits.Summoner
            Case 0
                CurrentUnit = ListOfUnits.UnitsInGroup(0).UnitNumber
            Case 1
                CurrentUnit = ListOfUnits.FcastUnitsInGroup(0).UnitNumber
        End Select


    End Sub

#End Region

#Region "Public Methods"
    Public Sub Reset()
        For Each brd As Border In Children
            Dim tb As TextBlock = brd.Child
            If brd.Tag <> "Label" Then
                tb.Foreground = Brushes.Black
                tb.FontSize = 14
                tb.FontWeight = FontWeights.SemiBold
                If brd.IsEnabled = True Then SelectedCount += 1
            End If
        Next
        CurrentUnit = 0
    End Sub

#End Region

#Region "Private Methods"
    Private Sub HoverOverUnit(sender As Object, e As MouseEventArgs)
        Dim tb As TextBlock
        If TypeOf (sender) Is TextBlock Then
            tb = sender
        Else
            Dim brd As Border = sender
            tb = brd.Child
        End If
        tb.FontSize = 18
    End Sub

    Private Sub LeaveUnit(sender As Object, e As MouseEventArgs)
        Dim tb As TextBlock
        If TypeOf (sender) Is TextBlock Then
            tb = sender
        Else
            Dim brd As Border = sender
            tb = brd.Child
        End If
        If FormatNumber(tb.Tag, 0) <> CurrentUnit Then
            tb.FontSize = 12
        Else
            tb.FontSize = 16
        End If
    End Sub

    Private Sub ChooseUnit(sender As Object, e As MouseEventArgs)
        SelectedCount = 0
        Dim tb As TextBlock
        If TypeOf (sender) Is TextBlock Then
            tb = sender
        Else
            Dim brd As Border = sender
            tb = brd.Child
        End If
        If FormatNumber(tb.Tag, 0) <> CurrentUnit Then
            CurrentUnit = FormatNumber(tb.Tag, 0)
        Else
            If CurrentUnit <> 0 Then Reset()
        End If
    End Sub

#End Region

End Class
