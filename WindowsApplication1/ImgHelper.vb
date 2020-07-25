Imports System.Drawing.Imaging
Imports System.IO

Module ImgHelper

    Function TakeScreenShot() As Bitmap
        Dim Bounds As Rectangle = Screen.PrimaryScreen.Bounds
        Dim ScreenShot As New Bitmap(Bounds.Width, Bounds.Height)
        Graphics.FromImage(ScreenShot).CopyFromScreen(Bounds.X, Bounds.Y, 0, 0, Bounds.Size, CopyPixelOperation.SourceCopy)
        Return ScreenShot
    End Function

    Function BitmapToByteArray(Bitmap As Bitmap, ImageFormat As ImageFormat) As Byte()
        Dim MS As New MemoryStream
        Bitmap.Save(MS, ImageFormat)
        Return MS.ToArray
    End Function

    Function TakeScreenShotThumb() As Bitmap
        Dim ScreenShot As Bitmap = TakeScreenShot()
        Dim Image As Image = ScreenShot
        Return Image.GetThumbnailImage(ScreenShot.Width / 2, ScreenShot.Height / 2, Nothing, IntPtr.Zero)
    End Function

    Function BitmapToPngBuffer(Bitmap As Bitmap) As Byte()
        Return BitmapToByteArray(Bitmap, ImageFormat.Png)
    End Function

    Function BitmapToBuffer(Bitmap As Bitmap) As Byte()
        Return BitmapToByteArray(Bitmap, ImageFormat.Bmp)
    End Function

    Function TakeScreenShotBimapBuffer() As Byte()
        Return BitmapToBuffer(TakeScreenShot())
    End Function

    Function TakeScreenShotPngBuffer() As Byte()
        Return BitmapToPngBuffer(TakeScreenShot())
    End Function

    Function TakeScreenShotRaw() As Byte()
        Return BitmapToRaw(TakeScreenShot())
    End Function

    Function TakeScreenShotRaw256Colors() As Byte()
        Return BitmapToRaw256Colors(TakeScreenShot())
    End Function

    Function TakeScreenShotBmpThumb() As Byte()
        Return BitmapToBuffer(TakeScreenShotThumb())
    End Function

    Function Bitmap24bppRgb(Bmp As Bitmap) As Bitmap

        If Bmp.PixelFormat = PixelFormat.Format24bppRgb Then
            Return Bmp
        Else
            Dim Bmp24bpp As Bitmap = New Bitmap(Bmp.Width, Bmp.Height, PixelFormat.Format24bppRgb)

            Using gr As Graphics = Graphics.FromImage(Bmp24bpp)
                gr.DrawImage(Bmp, 0, 0, Bmp.Width, Bmp.Height)
            End Using

            Return Bmp24bpp
        End If

    End Function

    Function Bitmap32bppArgb(Bmp As Bitmap) As Bitmap

        If Bmp.PixelFormat = PixelFormat.Format32bppArgb Then
            Return Bmp
        Else
            Dim Bmp32bpp As Bitmap = New Bitmap(Bmp.Width, Bmp.Height, PixelFormat.Format32bppArgb)

            Using gr As Graphics = Graphics.FromImage(Bmp32bpp)
                gr.DrawImage(Bmp, 0, 0, Bmp.Width, Bmp.Height)
            End Using

            Return Bmp32bpp
        End If

    End Function


    'Function Bitmap16bppArgb1555(Bmp As Bitmap) As Bitmap

    '    If Bmp.PixelFormat = PixelFormat.Format16bppArgb1555 Then
    '        Return Bmp
    '    Else
    '        Dim Bmp24bpp As Bitmap = New Bitmap(Bmp.Width, Bmp.Height, PixelFormat.Format16bppArgb1555)

    '        Using gr As Graphics = Graphics.FromImage(Bmp24bpp)
    '            gr.DrawImage(Bmp, 0, 0, Bmp.Width, Bmp.Height)
    '        End Using

    '        Return Bmp24bpp
    '    End If

    'End Function

    Function Bitmap24bppRgbBuffer(Width As Integer, Height As Integer) As Byte()
        Dim Header As Byte() = Bitmap24bppRgbHeader(Width, Height)
        ReDim Preserve Header(BitConverter.ToInt32(Header, &H2) - 1)
        Return Header
    End Function

    Function Bitmap24bppRgbHeader(Width As Integer, Height As Integer) As Byte()
        '54 bytes
        Dim Buff As Byte() = {&H42, &H4D, &H7E, &H45, &H25, &H0, &H0, &H0, &H0, &H0, &H36, &H0, &H0, &H0, &H28, &H0, &H0, &H0, &H3D, &H5, &H0, &H0, &H5F, &H2, &H0, &H0, &H1, &H0, &H18, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &HC4, &HE, &H0, &H0, &HC4, &HE, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0}



        Dim PaddingBytesCount As Integer = Width Mod 4.0 ' Must be multiple of 4



        Dim RawSize As Integer = Width * Height * 3 + (PaddingBytesCount * Height)
        Dim BMPSize As Integer = RawSize + Buff.Length



        Dim TempBuff As Byte()



        TempBuff = BitConverter.GetBytes(BMPSize)
        Buff(&H2) = TempBuff(0)
        Buff(&H3) = TempBuff(1)
        Buff(&H4) = TempBuff(2)
        Buff(&H5) = TempBuff(3)


        TempBuff = BitConverter.GetBytes(RawSize) ' O própio .net não faz isso, então não vou fazer também... para não confundir o código, lugar vai ler do header e outro lugar não.
        Buff(&H22) = 0 'TempBuff(0)
        Buff(&H23) = 0 'TempBuff(1)
        Buff(&H24) = 0 'TempBuff(2)
        Buff(&H25) = 0 'TempBuff(3)


        TempBuff = BitConverter.GetBytes(Width)
        Buff(&H12) = TempBuff(0)
        Buff(&H13) = TempBuff(1)
        Buff(&H14) = TempBuff(2)
        Buff(&H15) = TempBuff(3)


        TempBuff = BitConverter.GetBytes(Height)
        Buff(&H16) = TempBuff(0)
        Buff(&H17) = TempBuff(1)
        Buff(&H18) = TempBuff(2)
        Buff(&H19) = TempBuff(3)

        Return Buff
    End Function



    Function Bitmap32bppArgbBuffer(Width As Integer, Height As Integer) As Byte()
        Dim Header As Byte() = Bitmap32bppArgbHeader(Width, Height)
        ReDim Preserve Header(BitConverter.ToInt32(Header, &H2) - 1)
        Return Header
    End Function

    Function Bitmap32bppArgbHeader(Width As Integer, Height As Integer) As Byte()
        '54 bytes
        Dim Buff As Byte() = {&H42, &H4D, &H7E, &H45, &H25, &H0, &H0, &H0, &H0, &H0, &H36, &H0, &H0, &H0, &H28, &H0, &H0, &H0, &H3D, &H5, &H0, &H0, &H5F, &H2, &H0, &H0, &H1, &H0, &H20, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &HC4, &HE, &H0, &H0, &HC4, &HE, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0}



        Dim PaddingBytesCount As Integer = Width Mod 4.0 ' Must be multiple of 4



        Dim RawSize As Integer = Width * Height * 4 + (PaddingBytesCount * Height)
        Dim BMPSize As Integer = RawSize + Buff.Length



        Dim TempBuff As Byte()



        TempBuff = BitConverter.GetBytes(BMPSize)
        Buff(&H2) = TempBuff(0)
        Buff(&H3) = TempBuff(1)
        Buff(&H4) = TempBuff(2)
        Buff(&H5) = TempBuff(3)


        TempBuff = BitConverter.GetBytes(RawSize) ' O própio .net não faz isso, então não vou fazer também... para não confundir o código, lugar vai ler do header e outro lugar não.
        Buff(&H22) = 0 'TempBuff(0)
        Buff(&H23) = 0 'TempBuff(1)
        Buff(&H24) = 0 'TempBuff(2)
        Buff(&H25) = 0 'TempBuff(3)


        TempBuff = BitConverter.GetBytes(Width)
        Buff(&H12) = TempBuff(0)
        Buff(&H13) = TempBuff(1)
        Buff(&H14) = TempBuff(2)
        Buff(&H15) = TempBuff(3)


        TempBuff = BitConverter.GetBytes(Height)
        Buff(&H16) = TempBuff(0)
        Buff(&H17) = TempBuff(1)
        Buff(&H18) = TempBuff(2)
        Buff(&H19) = TempBuff(3)

        Return Buff
    End Function


End Module
