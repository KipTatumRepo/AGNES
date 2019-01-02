Public Class AgnesBaseInput
    Inherits Grid
#Region "Properties"
    Public BaseTextBox As TextBox
    Public Enum ShowFlare
        Show
        Hide
    End Enum

    Public Enum FontSz
        Small       '8pt font
        Smaller     '10pt font
        Standard    '12pt font
        Medium      '16pt font
        Large       '18pt font
        VeryLarge   '24pt font
    End Enum
    Private _flare As Boolean

    Public Property Flare As Boolean
        Get
            Return _flare
        End Get
        Set(value As Boolean)
            _flare = value
            If value = False Then
                Me.Children(0).Visibility = Visibility.Hidden
                Me.Children(1).Opacity = 1
            Else
                Me.Children(0).Visibility = Visibility.Visible
                Me.Children(1).Opacity = 0.9
            End If

        End Set
    End Property

#End Region

#Region "Constructor"
    Public Sub New(ByVal FieldWidth As Integer, ByVal v As VerticalAlignment, ByVal h As HorizontalAlignment, FontSize As FontSz, Optional ta As TextAlignment = TextAlignment.Left, Optional ByVal s As String = "", Optional ByVal tw As TextWrapping = TextWrapping.Wrap)
        HorizontalAlignment = h
        VerticalAlignment = v
        Width = FieldWidth
        Dim errorflare As New Effects.BlurEffect With {.KernelType = Effects.KernelType.Gaussian, .Radius = 10,
            .RenderingBias = Effects.RenderingBias.Performance}
        Dim myrct As New Rectangle With {.Name = "ErrorRectangle", .Fill = Brushes.Red, .Opacity = 0.33, .StrokeThickness = 1,
            .Effect = errorflare, .Visibility = Visibility.Hidden}

        BaseTextBox = New TextBox With {.Name = "TextBox", .BorderBrush = Brushes.LightGray, .Text = s, .TextAlignment = ta,
            .TextWrapping = tw, .HorizontalAlignment = h, .VerticalAlignment = v}

        Select Case FontSize
            Case FontSz.Small
                Height = 24
                With BaseTextBox
                    .Height = 16
                    .Width = FieldWidth - 8
                    .Margin = New Thickness(4, 4, 0, 0)
                    .FontSize = 8
                End With
            Case FontSz.Smaller
                Height = 26
                With BaseTextBox
                    .Height = 18
                    .Width = FieldWidth - 8
                    .Margin = New Thickness(4, 4, 0, 0)
                    .FontSize = 10
                End With
            Case FontSz.Standard
                Height = 28
                With BaseTextBox
                    .Height = 20
                    .Width = FieldWidth - 8
                    .Margin = New Thickness(4, 4, 0, 0)
                    .FontSize = 12
                End With
            Case FontSz.Medium
                Height = 34
                With BaseTextBox
                    .Height = 26
                    .Width = FieldWidth - 8
                    .Margin = New Thickness(4, 4, 0, 0)
                    .FontSize = 16
                End With
            Case FontSz.Large
                Height = 36
                With BaseTextBox
                    .Height = 28
                    .Width = FieldWidth - 8
                    .Margin = New Thickness(4, 4, 0, 0)
                    .FontSize = 18
                End With
            Case FontSz.VeryLarge
                Height = 44
                With BaseTextBox
                    .Height = 36
                    .Width = FieldWidth - 8
                    .Margin = New Thickness(4, 4, 0, 0)
                    .FontSize = 24
                End With
        End Select

        With Children
            .Add(myrct)
            .Add(BaseTextBox)
        End With
    End Sub

#End Region

#Region "Public Methods"
    Public Sub UserFocus()
        BaseTextBox.Focus()
        BaseTextBox.SelectAll()
    End Sub

#End Region

End Class