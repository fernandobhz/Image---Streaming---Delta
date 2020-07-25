Imports System.IO

Public Module RawBitmap256Colors

    Function BitmapToRaw256Colors(Bmp As Bitmap) As Byte()

        'Dim t1 As DateTime = Now

        Bmp = Bitmap32bppArgb(Bmp)
        Dim BBytes As Byte() = BitmapToBuffer(Bmp)

        Dim MS As New MemoryStream
        Dim BW As New BinaryWriter(MS)

        BW.Write(Bmp.Width)
        BW.Write(Bmp.Height)
        BW.Write(Date.Now.ToBinary)

        For i As Integer = 54 To BBytes.Length - 1 Step 4

            Dim Red As Byte = BBytes(i)
            Dim Green As Byte = BBytes(i + 1)
            Dim Blue As Byte = BBytes(i + 2)

            Dim RSmall As Byte = IIf(Red < 240, (Red / 32), 7)
            Dim GSmall As Byte = IIf(Green < 240, (Green / 32), 7)
            Dim BSmall As Byte = IIf(Blue < 224, (Blue / 64), 3)

            Dim RShifted As Byte = RSmall
            Dim GShifted As Byte = GSmall << 3
            Dim BShifted As Byte = BSmall << 6

            Dim ByteColor As Byte = RShifted Or GShifted Or BShifted

            BW.Write(ByteColor)
        Next

        'Debug.Print(String.Format("BitmapToRaw256Colors in {0} seconds", (Now - t1).TotalSeconds))

        Return MS.ToArray
    End Function

    Function RawToBitmap256Colors(Raw As Byte(), Optional TimeStampOverlay As Boolean = True) As Bitmap

        Dim TimeStamp As Date
        Dim BBytes As Byte() = RawToBitmap256ColorsTranslateOnly(Raw, TimeStamp)

        Dim Ret As Bitmap = New Bitmap(New MemoryStream(BBytes))

        If TimeStampOverlay Then
            Using gr As Graphics = Graphics.FromImage(Ret)
                gr.DrawString(TimeStamp.ToString, New Font("Arial", 32), New SolidBrush(Color.Black), 0, 0)
            End Using
        End If

        Return Ret

    End Function

    Function RawToBitmap256ColorsTranslateOnly(Raw As Byte(), ByRef TimeStamp As Date) As Byte()

        'Dim t1 As DateTime = Now

        Dim MS As New MemoryStream(Raw)
        Dim BR As New BinaryReader(MS)

        Dim Width As Integer = BR.ReadInt32
        Dim Height As Integer = BR.ReadInt32
        TimeStamp = Date.FromBinary(BR.ReadInt64)

        Dim BBytes As Byte() = Bitmap32bppArgbBuffer(Width, Height)

        Dim j As Integer = 54

        For i As Integer = 16 To Raw.Length - 1

            Dim ByteColor As Byte = Raw(i)

            Dim __BShifted As Byte = ByteColor
            __BShifted = __BShifted >> 6 << 6

            Dim __GShifted As Byte = ByteColor
            __GShifted = __GShifted << 2
            __GShifted = __GShifted >> 2
            __GShifted = __GShifted >> 3
            __GShifted = __GShifted << 3

            Dim __RShifted As Byte = ByteColor
            __RShifted = __RShifted << 5
            __RShifted = __RShifted >> 5



            Dim __RSmall As Byte = __RShifted

            Dim __GSmall As Byte = __GShifted
            __GSmall = __GSmall >> 3

            Dim __BSmall As Byte = __BShifted
            __BSmall = __BSmall >> 6



            Dim __Red As Byte
            Dim __Green As Byte
            Dim __Blue As Byte

            If __RSmall = 7 And __GSmall = 7 And __BSmall = 3 Then
                __Red = 255
                __Green = 255
                __Blue = 255
            Else
                __Red = __RSmall * 32
                __Green = __GSmall * 32
                __Blue = __BSmall * 64
            End If



            BBytes(j) = __Blue
            BBytes(j + 1) = __Green
            BBytes(j + 2) = __Red
            'BBytes(j + 3) = &H0

            j = j + 4
        Next

        'Debug.Print(String.Format("RawToBitmap256Colors {0} seconds", (Now - t1).TotalSeconds))

        Return BBytes

    End Function

End Module

