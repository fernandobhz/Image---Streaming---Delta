Imports System.Drawing.Imaging
Imports System.IO
Imports System.Threading

Module ScreenHandleRaw

    Function RecordScreenRaw(Seconds As Integer, File As String) As Integer
        Dim tStart As DateTime = Now
        Dim tStop As DateTime = DateAdd(DateInterval.Second, Seconds, tStart)

        Dim X As New List(Of Byte())

        Dim InitialRaw As Byte() = TakeScreenShotRaw()
        X.Add(Deflate(InitialRaw))


        Dim TimeA As DateTime = Now
        Dim LadoA As Byte() = InitialRaw

        Dim framesCount As Integer = 1

        While Now < tStop
            'Thread.Sleep(200 - (Now - TimeA).TotalMilliseconds) 'Max 5fps
            Dim TimeB As DateTime = Now
            Dim LadoB As Byte() = TakeScreenShotRaw()
            Dim Delta As Byte() = ByteArrayDiff(LadoA, LadoB)

            X.Add(Delta)

            framesCount = framesCount + 1
            LadoA = LadoB
            TimeA = Now
        End While

        My.Computer.FileSystem.WriteAllBytes(File, BinarySerialize(X), False)

        Return framesCount
    End Function


    Sub PlayRecordedScreenRaw(File As String, PictureBox As PictureBox)

        Dim X As List(Of Byte()) = BinaryDeserialize(My.Computer.FileSystem.ReadAllBytes(File))

        Dim InitialRaw As Byte() = Inflate(X(0))

        Dim InitialImage As Bitmap = RawToBitmap(InitialRaw)
        PictureBox.Image = InitialImage

        Dim NewRaw As Byte() = InitialRaw

        For i As Integer = 1 To X.Count - 1
            ByteArrayPatcheApply(NewRaw, X(i))
            PictureBox.Image = Nothing
            PictureBox.Image = RawToBitmap(NewRaw)
            PictureBox.Refresh()
            GC.Collect()
        Next

    End Sub


End Module
