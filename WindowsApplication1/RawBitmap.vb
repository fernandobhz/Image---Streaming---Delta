Imports System.IO
Imports System.Drawing.Imaging

Public Module RawBitmap
    'O Raw é uma formato meu... pois, pretendo manipular estes bits/bytes de forma a que não seria compatível com o padrão bitmap
    'Mas depois, vou ter que voltar do meu padrão para o bitmap para exibição
    'Pretendo trabalhar para reduzir o tamanho do bitmap, sem perder qualidade para SCREENSHOT

    Function BitmapToRaw(Bmp As Bitmap) As Byte()
        'http://en.wikipedia.org/wiki/BMP_file_format#Example_1


        Dim Bmp24bpp As Bitmap = Bitmap24bppRgb(Bmp)


        Dim BBytes As Byte() = BitmapToBuffer(Bmp24bpp)
        Dim StartPixelArrayAddress As Integer = BitConverter.ToInt32(BBytes, &HA)
        Dim RawLength As Integer = BBytes.Length - StartPixelArrayAddress


        Dim RawBytes(RawLength - 1 + 8) As Byte

        'Width
        RawBytes(0) = BBytes(&H12)
        RawBytes(1) = BBytes(&H13)
        RawBytes(2) = BBytes(&H14)
        RawBytes(3) = BBytes(&H15)

        'Height
        RawBytes(4) = BBytes(&H16)
        RawBytes(5) = BBytes(&H17)
        RawBytes(6) = BBytes(&H18)
        RawBytes(7) = BBytes(&H19)


        Buffer.BlockCopy(BBytes, StartPixelArrayAddress, RawBytes, 8, RawLength)

        Return RawBytes

    End Function

    Function RawToBitmap(Raw As Byte()) As Bitmap
        'http://en.wikipedia.org/wiki/BMP_file_format#Example_1

        Dim Width As Integer = BitConverter.ToInt32(Raw, 0)
        Dim Height As Integer = BitConverter.ToInt32(Raw, 4)


        Dim BBytes As Byte() = Bitmap24bppRgbBuffer(Width, Height)

        Dim StartPixelArrayAddress As Integer = BitConverter.ToInt32(BBytes, &HA)
        Dim RawLength As Integer = BBytes.Length - StartPixelArrayAddress

        Buffer.BlockCopy(Raw, 8, BBytes, StartPixelArrayAddress, RawLength)

        Return New Bitmap(New MemoryStream(BBytes))
    End Function




    'Function BitmapToRaw(Bmp As Bitmap) As Byte()

    '    Dim Lock As New LockBitmap(Bmp)
    '    Lock.LockBits()

    '    Dim Buff(Bmp.Width * Bmp.Height * 3 - 1 + 8) As Byte

    '    Dim WBuff As Byte() = BitConverter.GetBytes(Bmp.Width)
    '    Dim HBuff As Byte() = BitConverter.GetBytes(Bmp.Height)

    '    Buff(0) = WBuff(0)
    '    Buff(1) = WBuff(1)
    '    Buff(2) = WBuff(2)
    '    Buff(3) = WBuff(3)

    '    Buff(4) = HBuff(0)
    '    Buff(5) = HBuff(1)
    '    Buff(6) = HBuff(2)
    '    Buff(7) = HBuff(3)

    '    Dim p As Integer = 8

    '    For i As Integer = 0 To Bmp.Width - 1
    '        For j As Integer = 0 To Bmp.Height - 1

    '            Dim Color As Color = Lock.GetPixel(i, j)

    '            Buff(p) = Color.R
    '            Buff(p + 1) = Color.G
    '            Buff(p + 2) = Color.B

    '            p = p + 3
    '        Next
    '    Next

    '    Lock.UnlockBits()

    '    Return Buff

    'End Function



    'Function BitmapToRaw(Bmp As Bitmap) As Byte()

    '    Dim Lock As New LockBitmap(Bmp)
    '    Lock.LockBits()

    '    Dim MS As New MemoryStream
    '    Dim BW As New BinaryWriter(MS)

    '    BW.Write(Bmp.Width)
    '    BW.Write(Bmp.Height)

    '    For i As Integer = 0 To Bmp.Width - 1
    '        For j As Integer = 0 To Bmp.Height - 1

    '            'Esse comando é que é demorado, estudar um pouco mais o bitmap para copiar diretamente
    '            Dim Color As Color = Lock.GetPixel(i, j)

    '            Dim Pixel(2) As Byte
    '            Pixel(0) = Color.R
    '            Pixel(1) = Color.G
    '            Pixel(2) = Color.B

    '            BW.Write(Pixel)
    '        Next
    '    Next

    '    Lock.UnlockBits()

    '    Return MS.ToArray

    'End Function



    'Function RawToBitmap(Raw As Byte()) As Bitmap

    '    Dim MS As New MemoryStream(Raw)
    '    Dim BR As New BinaryReader(MS)

    '    Dim Width As Integer = BR.ReadInt32
    '    Dim Height As Integer = BR.ReadInt32


    '    Dim Bmp As New Bitmap(Width, Height)
    '    Dim Lock As New LockBitmap(Bmp)
    '    Lock.LockBits()

    '    For i As Integer = 0 To Width - 1
    '        For j As Integer = 0 To Height - 1

    '            Dim R As Byte = BR.ReadByte()
    '            Dim G As Byte = BR.ReadByte()
    '            Dim B As Byte = BR.ReadByte()

    '            Dim C As Color = Color.FromArgb(R, G, B)

    '            Lock.SetPixel(i, j, C)
    '        Next
    '    Next

    '    Lock.UnlockBits()

    '    Return Bmp

    'End Function


End Module
