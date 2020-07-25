Imports System.Drawing.Imaging
Imports System.IO
Imports System.Threading

Module ScreenHandleRaw256Colors

    Function RecordScreenRaw256Colors(File As String, ByRef CancelFlag As Boolean) As Integer
        Dim X As New List(Of Byte())

        Dim InitialRaw As Byte() = TakeScreenShotRaw256Colors()
        X.Add(Deflate(InitialRaw))


        Dim TimeA As DateTime = Now
        Dim LadoA As Byte() = InitialRaw

        Dim framesCount As Integer = 1

        While Not CancelFlag
            'Thread.Sleep(200 - (Now - TimeA).TotalMilliseconds) 'Max 5fps
            Dim TimeB As DateTime = Now
            Dim LadoB As Byte() = TakeScreenShotRaw256Colors()
            Dim Delta As Byte() = ByteArrayDiff(LadoA, LadoB)

            X.Add(Delta)

            framesCount = framesCount + 1
            LadoA = LadoB
            TimeA = Now
            Application.DoEvents()
            Thread.Sleep(700)
        End While

        My.Computer.FileSystem.WriteAllBytes(File, BinarySerialize(X), False)

        Return framesCount
    End Function

    Async Function RecordScreenRaw256Colors(Seconds As Integer, File As String) As Task(Of Integer)
        Dim tStart As DateTime = Now
        Dim tStop As DateTime = DateAdd(DateInterval.Second, Seconds, tStart)

        Dim CancelFlag As Boolean

        RecordScreenRaw256Colors(File, CancelFlag)

        Await Task.Delay(Seconds * 1000)

        CancelFlag = True

    End Function


    Sub PlayRecordedScreenRaw256Colors(File As String, PictureBox As PictureBox)

        Dim X As List(Of Byte()) = BinaryDeserialize(My.Computer.FileSystem.ReadAllBytes(File))

        Dim InitialRaw As Byte() = Inflate(X(0))

        Dim InitialImage As Bitmap = RawToBitmap256Colors(InitialRaw)
        PictureBox.Image = InitialImage
        PictureBox.Refresh()

        Dim NewRaw As Byte() = InitialRaw

        For i As Integer = 1 To X.Count - 1
            ByteArrayPatcheApply(NewRaw, X(i))

            Dim NewRawBitmap As Bitmap = RawToBitmap256Colors(NewRaw)

            PictureBox.Image.Dispose()
            GC.Collect()

            'Using gr As Graphics = Graphics.FromImage(PictureBox.Image)
            'gr.DrawImage(NewRawBitmap, 0, 0, InitialImage.Width, InitialImage.Height)
            'End Using

            PictureBox.Image = NewRawBitmap
            PictureBox.Refresh()
            Application.DoEvents()

            'PictureBox.Image = RawToBitmap256Colors(NewRaw)

        Next

    End Sub


End Module
