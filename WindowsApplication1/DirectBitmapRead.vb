Imports System.IO

Public Class DirectBitmapRead
    Implements IDisposable

    Private _Bitmap As Bitmap
    Public _BBytes As Byte()

    Private _StartPixelArrayAddress As Integer
    Private _RawLength As Integer
    Private _HeaderSize As Integer

    Sub New(ByRef Bitmap As Bitmap)
        Bitmap = Bitmap24bppRgb(Bitmap)

        _Bitmap = Bitmap
        _BBytes = BitmapToBuffer(_Bitmap)

        _RawLength = _Bitmap.Width * _Bitmap.Height * 3
        _StartPixelArrayAddress = _BBytes.Length - _RawLength
        _HeaderSize = _StartPixelArrayAddress 'Is the same number, but, for code readability, let's duplicate them :)
    End Sub

    Function getBitmap() As Bitmap
        Return New Bitmap(New MemoryStream(_BBytes))
    End Function

    Public Function GetPixel(x As Integer, y As Integer) As Color
        'performance issues, one calc is better than two
        'Dim rowsBeforeMe As Integer = _Bitmap.Width * 3 * y
        'Dim address As Integer = rowsBeforeMe + (x * 3) + _HeaderSize
        'Dim address As Integer = (_Bitmap.Width * 3 * y) + (x * 3) + _HeaderSize
        Dim address As Integer = ((y * _Bitmap.Width) + x) * 3 + _HeaderSize

        Dim Blue As Byte = _BBytes(address)
        Dim Green As Byte = _BBytes(Address + 1)
        Dim Red As Byte = _BBytes(Address + 2)

        Return Color.FromArgb(Red, Green, Blue)

    End Function

    Public Function GetPixelRGB(x As Integer, y As Integer) As Byte()
        Dim address As Integer = ((y * _Bitmap.Width) + x) * 3 + _HeaderSize

        Dim Ret(2) As Byte
        Ret(0) = _BBytes(address)
        Ret(1) = _BBytes(address + 1)
        Ret(2) = _BBytes(address + 2)
        Return Ret
    End Function

    Public Sub SetPixel(x As Integer, y As Integer, color As Color)
        Dim address As Integer = (_Bitmap.Width * 3 * y) + (x * 3) + _HeaderSize

        _BBytes(Address) = color.B
        _BBytes(Address + 1) = color.G
        _BBytes(Address + 2) = color.R
    End Sub

    Public Sub SetPixelRGB(x As Integer, y As Integer, Red As Byte, Green As Byte, Blue As Byte)
        Dim address As Integer = (_Bitmap.Width * 3 * y) + (x * 3) + _HeaderSize

        _BBytes(address) = Red
        _BBytes(address + 1) = Green
        _BBytes(address + 2) = Blue
    End Sub

    Public Sub SetPixelRGB(x As Integer, y As Integer, RGB As Byte())
        Dim address As Integer = (_Bitmap.Width * 3 * y) + (x * 3) + _HeaderSize

        _BBytes(address) = RGB(0)
        _BBytes(address + 1) = RGB(1)
        _BBytes(address + 2) = RGB(2)
    End Sub


#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
            End If

            _BBytes = Nothing
            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        Me.disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
