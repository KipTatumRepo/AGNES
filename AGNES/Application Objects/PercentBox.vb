Imports System.ComponentModel
Public Class PercentBox
    Inherits AgnesBaseInput
    Implements INotifyPropertyChanged
#Region "Properties"
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Private _negonly As Boolean
    Private _posonly As Boolean
    Public Highlight As Boolean
    Private SystemChange As Boolean
    Private HeldValue As String
    Private NumofDec As Byte
    Private _pos As Boolean
    Public Property Pos As Boolean
        Get
            Return _pos
        End Get
        Set(value As Boolean)
            _pos = value
        End Set
    End Property

    Private _neg As Boolean
    Public Property Neg As Boolean
        Get
            Return _neg
        End Get
        Set(value As Boolean)
            _neg = value
        End Set
    End Property

    Private _setamount As Double
    Public Property SetAmount As Double
        Get
            Return _setamount
        End Get
        Set(value As Double)
            _setamount = value
            Dim tb As TextBox = Children(1)
            tb.Text = FormatPercent(_setamount, NumofDec)
        End Set
    End Property

    Private _fontsize As Integer
    Public Property FontSize As Integer
        Get
            Return _fontsize
        End Get
        Set(value As Integer)
            _fontsize = value
            Dim t As TextBox = Children(1)
            t.FontSize = value
        End Set
    End Property

#End Region

#Region "Constructor"
    Public Sub New(FieldWidth As Integer, SelectAllUponEnteringField As Boolean, FontSize As AgnesBaseInput.FontSz, Optional DecimalCount As Byte = 0, Optional ByVal DefaultText As String = "0%", Optional ForcePos As Boolean = False, Optional ForceNeg As Boolean = False)
        MyBase.New(FieldWidth, VerticalAlignment.Top, HorizontalAlignment.Left, FontSize, TextAlignment.Right, DefaultText, TextWrapping.NoWrap)
        Pos = Not ForceNeg
        Neg = Not ForcePos
        _negonly = ForceNeg
        _posonly = ForcePos
        NumofDec = DecimalCount
        Highlight = SelectAllUponEnteringField
        Dim t As TextBox = Children(1)
        AddHandler t.GotFocus, AddressOf EnterField
        AddHandler t.LostFocus, AddressOf ExitField
        AddHandler t.TextChanged, AddressOf ValidateText
        ' Field width for 8pt = 80
        '             for 12pt= 112
        '             for 16pt= 140
        '             for 18pt= 160
        '             for 24pt= 204
    End Sub
#End Region

#Region "Public Methods"
#End Region

#Region "Private Methods"
    Private Sub ValidateText(sender As Object, e As EventArgs)
        Dim t As TextBox = sender
        If SystemChange = True Then Exit Sub
        Try
            Dim temptext As String = t.Text.Replace("%", "")
            Dim cval As Double = FormatNumber(temptext, 6)
        Catch ex As Exception
            Flare = True
            Exit Sub
        End Try
        Flare = False
    End Sub

    Private Sub EnterField(sender As Object, e As EventArgs)
        Dim t As TextBox = sender
        HeldValue = t.Text
        SystemChange = True
        '// Remove currency symbol, if present  
        t.Text = t.Text.Replace("%", "")

        '// Remove negative parentheses, if present
        If InStr(1, t.Text, "(") > 0 Then
            t.Text = t.Text.Replace("(", "")
            t.Text = t.Text.Replace(")", "")
            t.Text = "-" & t.Text
        End If

        If Highlight = True Then
            t.SelectAll()
        Else
            t.CaretIndex = t.Text.Length
        End If
        SystemChange = False

    End Sub

    Private Sub ExitField(sender As Object, e As EventArgs)
        Dim t As TextBox = sender

        Try
            Dim cval As Double = FormatNumber(t.Text.Replace("%", ""), 6)
            If (Neg = False And cval > 0) Or (Pos = False And cval < 0) Then
                SystemChange = True
                Flare = True
            Else
                Flare = False
            End If
            t.Text = FormatPercent(cval, NumofDec)
            SetAmount = cval
            SystemChange = False
        Catch ex As Exception
            Flare = True
            Me.Focus()
        End Try

        Try
            Dim cval As Double = FormatNumber(t.Text.Replace("%", ""), 6)
            If (_negonly = True And cval < 0) Or (_posonly = True And cval > 0) Then
                SystemChange = True
                Flare = False
                t.Text = FormatPercent(-cval, NumofDec)
                SetAmount = -cval
                SystemChange = False
            End If
        Catch ex As Exception
            Flare = True
            Me.Focus()
        End Try
        If t.Text = HeldValue Then Exit Sub
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(“Amountchanged”))
    End Sub

#End Region

End Class
