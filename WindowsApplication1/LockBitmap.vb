Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices

Public Class LockBitmap
    Private source As Bitmap = Nothing
    Private Iptr As IntPtr = IntPtr.Zero
    Private bitmapData As BitmapData = Nothing
    Public Property Pixels() As Byte()
    Public Property Depth() As Integer
    Public Property Width() As Integer
    Public Property Height() As Integer

    Public Sub New(source As Bitmap)
        Me.source = source
    End Sub


    Public Sub LockBits()
        Try
            ' Get width and height of bitmap
            Width = source.Width
            Height = source.Height

            ' get total locked pixels count
            Dim PixelCount As Integer = Width * Height

            ' Create rectangle to lock
            Dim rect As New Rectangle(0, 0, Width, Height)

            ' get source bitmap pixel format size
            Depth = System.Drawing.Bitmap.GetPixelFormatSize(source.PixelFormat)

            ' Check if bpp (Bits Per Pixel) is 8, 24, or 32
            If Depth <> 8 AndAlso Depth <> 24 AndAlso Depth <> 32 Then
                Throw New ArgumentException("Only 8, 24 and 32 bpp images are supported.")
            End If

            ' Lock bitmap and return bitmap data
            bitmapData = source.LockBits(rect, ImageLockMode.ReadWrite, source.PixelFormat)

            ' create byte array to copy pixel values
            Dim [step] As Integer = Depth / 8
            Pixels = New Byte(PixelCount * [step] - 1) {}
            Iptr = bitmapData.Scan0

            ' Copy data from pointer to array
            Marshal.Copy(Iptr, Pixels, 0, Pixels.Length)
        Catch ex As Exception
            Throw ex
        End Try
    End Sub


    Public Sub UnlockBits()
        Try
            ' Copy data from byte array to pointer
            Marshal.Copy(Pixels, 0, Iptr, Pixels.Length)

            ' Unlock bitmap data
            source.UnlockBits(bitmapData)
        Catch ex As Exception
            Throw ex
        End Try
    End Sub


    Public Function GetPixel(x As Integer, y As Integer) As Color
        Dim clr As Color = Color.Empty

        ' Get color components count
        Dim cCount As Integer = Depth / 8

        ' Get start index of the specified pixel
        Dim i As Integer = ((y * Width) + x) * cCount

        If i > Pixels.Length - cCount Then
            Throw New IndexOutOfRangeException()
        End If

        If Depth = 32 Then
            ' For 32 bpp get Red, Green, Blue and Alpha
            Dim b As Byte = Pixels(i)
            Dim g As Byte = Pixels(i + 1)
            Dim r As Byte = Pixels(i + 2)
            Dim a As Byte = Pixels(i + 3)
            ' a
            clr = Color.FromArgb(a, r, g, b)
        End If
        If Depth = 24 Then
            ' For 24 bpp get Red, Green and Blue
            Dim b As Byte = Pixels(i)
            Dim g As Byte = Pixels(i + 1)
            Dim r As Byte = Pixels(i + 2)
            clr = Color.FromArgb(r, g, b)
        End If
        If Depth = 8 Then
            ' For 8 bpp get color value (Red, Green and Blue values are the same)
            Dim c As Byte = Pixels(i)
            clr = Color.FromArgb(c, c, c)
        End If
        Return clr
    End Function


    Public Sub SetPixel(x As Integer, y As Integer, color As Color)
        ' Get color components count
        Dim cCount As Integer = Depth / 8

        ' Get start index of the specified pixel
        Dim i As Integer = ((y * Width) + x) * cCount

        If Depth = 32 Then
            ' For 32 bpp set Red, Green, Blue and Alpha
            Pixels(i) = color.B
            Pixels(i + 1) = color.G
            Pixels(i + 2) = color.R
            Pixels(i + 3) = color.A
        End If
        If Depth = 24 Then
            ' For 24 bpp set Red, Green and Blue
            Pixels(i) = color.B
            Pixels(i + 1) = color.G
            Pixels(i + 2) = color.R
        End If
        If Depth = 8 Then
            ' For 8 bpp set color value (Red, Green and Blue values are the same)
            Pixels(i) = color.B
        End If
    End Sub
End Class
