Imports System.Drawing.Imaging
Imports System.IO
Imports System.Threading

Module ScreenHandleMyDiff

    Function RecordScreenMyDiff(Seconds As Integer, File As String) As Integer
        Dim tStart As DateTime = Now
        Dim tStop As DateTime = DateAdd(DateInterval.Second, Seconds, tStart)

        Dim X As New List(Of Byte())

        Dim InitialScreenShot As Bitmap = TakeScreenShot()
        Dim InitialPng As Byte() = BitmapToPngBuffer(InitialScreenShot)
        X.Add(InitialPng)

        Dim TimeA As DateTime = Now
        Dim LadoA As Byte() = BitmapToBuffer(InitialScreenShot)

        Dim framesCount As Integer = 1

        While Now < tStop
            Thread.Sleep(200 - (Now - TimeA).TotalMilliseconds) 'Max 5fps
            Dim TimeB As DateTime = Now
            Dim LadoB As Byte() = TakeScreenShotBimapBuffer()
            Dim Delta As Byte() = ByteArrayDiff(LadoA, LadoB)

            X.Add(Delta)

            framesCount = framesCount + 1
            LadoA = LadoB
            TimeA = Now
        End While

        My.Computer.FileSystem.WriteAllBytes(File, BinarySerialize(X), False)

        Return framesCount
    End Function


    Sub PlayRecordedScreenMyDiff(File As String, PictureBox As PictureBox)

        Dim X As List(Of Byte()) = BinaryDeserialize(My.Computer.FileSystem.ReadAllBytes(File))

        Dim InitialImage As Bitmap = Bitmap.FromStream(New MemoryStream(X(0)))
        PictureBox.Image = InitialImage

        Dim NewImage As Byte() = BitmapToBuffer(InitialImage)

        For i As Integer = 1 To X.Count - 1
            ByteArrayPatcheApply(NewImage, X(i))
            PictureBox.Image = Nothing
            PictureBox.Image = Bitmap.FromStream(New MemoryStream(NewImage))
            PictureBox.Refresh()
            GC.Collect()
        Next

    End Sub


End Module
