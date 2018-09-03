Public Class FlashGroup
    Inherits DockPanel

    Public Sub New(GroupName As String, ShowPercentages As Boolean, Top As Integer, Highlight As Boolean, Subtotal As Boolean)
        HorizontalAlignment = HorizontalAlignment.Left
        VerticalAlignment = VerticalAlignment.Top
        Height = 42
        Width = 962
        LastChildFill = False
        Margin = New Thickness(10, Top, 0, 0)
        If Highlight = True Then Background = Brushes.WhiteSmoke
        If Subtotal = True Then Background = Brushes.LightGray
        '// Create Flash group header label
        Dim GroupLabel As New Border
        Dim tb As New TextBlock With {.Text = GroupName, .LineHeight = 16, .TextAlignment = TextAlignment.Center,
            .Margin = New Thickness(0, -2, 0, 0), .Width = 80, .VerticalAlignment = VerticalAlignment.Center,
            .FontSize = 16, .FontWeight = FontWeights.SemiBold, .TextWrapping = TextWrapping.Wrap}
        GroupLabel.Child = tb

        '// Create Flash value input field
        Dim FlashVal As New CurrencyBox(140, True, False, True, False, True, AgnesBaseInput.FontSz.Medium) With {.Margin = New Thickness(4, 4, 0, 0)}

        '// Create image for notes
        Dim NoteImage As New Image With {.Height = 32, .Width = 32, .Stretch = Stretch.UniformToFill, .HorizontalAlignment = Windows.HorizontalAlignment.Center,
            .VerticalAlignment = VerticalAlignment.Center, .Opacity = 0.33, .ToolTip = "Add Notes", .Margin = New Thickness(0, 1, 0, 0),
            .Source = New BitmapImage(New Uri("pack://application:,,,/Resources/Notes-icon.png"))}
        AddHandler NoteImage.PreviewMouseDown, AddressOf EnterNotes

        '// Create flash percentage textbox.  Hide if it doesn't belong with this group (preserving spacing)
        Dim FlashPercent As New TextBox With {.Text = "95%", .FontSize = 16, .VerticalAlignment = VerticalAlignment.Center, .HorizontalAlignment = Windows.HorizontalAlignment.Center,
            .BorderBrush = Brushes.LightGray, .IsEnabled = False, .Height = 26, .Margin = New Thickness(4, 0, 0, 0)}
        If ShowPercentages = False Then FlashPercent.Visibility = Visibility.Hidden

        '// Create budget value field
        Dim BudgetVal As New CurrencyBox(140, True, False, True, False, True, AgnesBaseInput.FontSz.Medium) With {.Margin = New Thickness(4, 4, 0, 0), .IsEnabled = False}

        '// Create budget percentage textbox.  Hide if it doesn't belong with this group (preserving spacing)
        Dim BudgetPercent As New TextBox With {.Text = "85%", .FontSize = 16, .VerticalAlignment = VerticalAlignment.Center, .HorizontalAlignment = Windows.HorizontalAlignment.Center,
            .BorderBrush = Brushes.LightGray, .IsEnabled = False, .Height = 26, .Margin = New Thickness(4, 0, 0, 0)}
        If ShowPercentages = False Then BudgetPercent.Visibility = Visibility.Hidden

        '// Create variance value field
        Dim BudgetVariance As New CurrencyBox(140, True, False, True, False, True, AgnesBaseInput.FontSz.Medium) With {.Margin = New Thickness(4, 4, 0, 0), .IsEnabled = False}

        '// Create forecast value field
        Dim ForecastVal As New CurrencyBox(140, True, False, True, False, True, AgnesBaseInput.FontSz.Medium) With {.Margin = New Thickness(4, 4, 0, 0), .IsEnabled = False}

        '// Create forecast percentage textbox.  Hide if it doesn't belong with this group (preserving spacing)
        Dim ForecastPercent As New TextBox With {.Text = "75%", .FontSize = 16, .VerticalAlignment = VerticalAlignment.Center, .HorizontalAlignment = Windows.HorizontalAlignment.Center,
            .BorderBrush = Brushes.LightGray, .IsEnabled = False, .Height = 26, .Margin = New Thickness(4, 0, 0, 0)}
        If ShowPercentages = False Then ForecastPercent.Visibility = Visibility.Hidden

        '// Create forecast variance value field
        Dim ForecastVariance As New CurrencyBox(140, True, False, True, False, True, AgnesBaseInput.FontSz.Medium) With {.Margin = New Thickness(4, 4, 0, 0), .IsEnabled = False}

        With Children
            .Add(GroupLabel)
            .Add(FlashVal)
            .Add(NoteImage)
            .Add(FlashPercent)
            .Add(BudgetVal)
            .Add(BudgetPercent)
            .Add(BudgetVariance)
            .Add(ForecastVal)
            .Add(ForecastPercent)
            .Add(ForecastVariance)
        End With


    End Sub

    Private Sub EnterNotes()
        MsgBox("Enter notes")
    End Sub



    '        <TextBox Height="26" Margin="4,0,0,0" />
    '       
    '<Grid HorizontalAlignment = "Left" Height="34" Margin="4,4,0,0" VerticalAlignment="Top" Width="140 " IsEnabled="False">
    '            <Rectangle Fill = "#FFFD092A" Opacity="0.33" RadiusX="5" RadiusY="5" Visibility="Hidden">
    '                <Rectangle.Effect>
    '<BlurEffect Radius = "10    " />
    '                </Rectangle.Effect>
    '                </Rectangle>
    '            <TextBox HorizontalAlignment = "Left" TextAlignment="Right" Height="26" Margin="4,4,0,0" TextWrapping="Wrap" Text="($999,999,999.99)" VerticalAlignment="Top" Width="132" BorderBrush="{DynamicResource {x:Static SystemColors.ActiveBorderBrushKey}}" Opacity="0.75" FontSize="16"/>
    '        </Grid>
    '        <TextBox Text = "85%" FontSize="16" VerticalAlignment="Center" HorizontalAlignment="Center" BorderBrush="#FFD6D8DE" IsEnabled="False" Height="26" Margin="4,0,0,0" />
    '        <Grid HorizontalAlignment = "Left" Height="34" Margin="4,4,0,0" VerticalAlignment="Top" Width="140 " IsEnabled="False">
    '            <Rectangle Fill = "#FFFD092A" Opacity="0.33" RadiusX="5" RadiusY="5" Visibility="Hidden">
    '                <Rectangle.Effect>
    '<BlurEffect Radius = "10    " />
    '                </Rectangle.Effect>
    '                </Rectangle>
    '            <TextBox HorizontalAlignment = "Left" TextAlignment="Right" Height="26" Margin="4,4,0,0" TextWrapping="Wrap" Text="($999,999,999.99)" VerticalAlignment="Top" Width="132" BorderBrush="{DynamicResource {x:Static SystemColors.ActiveBorderBrushKey}}" Opacity="0.75" FontSize="16"/>
    '        </Grid>
    '        <Grid HorizontalAlignment = "Left" Height="34" Margin="4,4,0,0" VerticalAlignment="Top" Width="140 " IsEnabled="False">
    '            <Rectangle Fill = "#FFFD092A" Opacity="0.33" RadiusX="5" RadiusY="5" Visibility="Hidden">
    '                <Rectangle.Effect>
    '<BlurEffect Radius = "10    " />
    '                </Rectangle.Effect>
    '                </Rectangle>
    '            <TextBox HorizontalAlignment = "Left" TextAlignment="Right" Height="26" Margin="4,4,0,0" TextWrapping="Wrap" Text="($999,999,999.99)" VerticalAlignment="Top" Width="132" BorderBrush="{DynamicResource {x:Static SystemColors.ActiveBorderBrushKey}}" Opacity="0.75" FontSize="16"/>
    '        </Grid>
    '        <TextBox Text = "85%" FontSize="16" VerticalAlignment="Center" HorizontalAlignment="Center" BorderBrush="#FFD6D8DE" IsEnabled="False" Height="26" Margin="4,0,0,0" />

    '        <Grid HorizontalAlignment = "Left" Height="34" Margin="4,4,0,0" VerticalAlignment="Top" Width="140 " IsEnabled="False">
    '            <Rectangle Fill = "#FFFD092A" Opacity="0.33" RadiusX="5" RadiusY="5" Visibility="Hidden">
    '                <Rectangle.Effect>
    '<BlurEffect Radius = "10    " />
    '                </Rectangle.Effect>
    '                </Rectangle>
    '            <TextBox HorizontalAlignment = "Left" TextAlignment="Right" Height="26" Margin="4,4,0,0" TextWrapping="Wrap" Text="($999,999,999.99)" VerticalAlignment="Top" Width="132" BorderBrush="{DynamicResource {x:Static SystemColors.ActiveBorderBrushKey}}" Opacity="0.75" FontSize="16"/>
    '        </Grid>
    '    </DockPanel>





End Class
