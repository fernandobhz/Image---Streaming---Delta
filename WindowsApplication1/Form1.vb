Imports System.IO
Imports System.IO.Compression
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Threading

Public Class Form1

    Private DoDeltaResults As String

    Sub DoDelta()
        Dim A As Byte() = My.Computer.FileSystem.ReadAllBytes("g:\c.bmp")
        Dim B As Byte() = My.Computer.FileSystem.ReadAllBytes("g:\x.bmp")

        ''                               X   X   X
        'Dim A As Byte() = {10, 20, 30, 40, 50, 60, 70, 80, 90, 100}
        'Dim B As Byte() = {10, 20, 30, 44, 55, 60, 70, 88, 99, 100}


        'Dim t1 As DateTime = Now
        Dim Delta As Byte() = BinaryHelper.ByteArrayDiff(A, B)
        'Dim t2 As DateTime = Now

        'MsgBox(BitConverter.ToString(Delta))

        'If ByteArrayCompare(A, B) Then
        '    MsgBox("equals")
        'Else
        '    MsgBox("different")
        'End If

        Dim t1 As DateTime = Now
        ByteArrayPatcheApply(A, Delta)
        Dim t2 As DateTime = Now

        'If ByteArrayCompare(A, B) Then
        '    MsgBox("equals")
        'Else
        '    MsgBox("different")
        'End If


        DoDeltaResults = _
            String.Concat _
            ( _
                String.Format("Original size: {0:f2} MB", A.Length / 1024 / 1024), vbCrLf, vbCrLf, _
                String.Format("Delta size: {0:f2} MB", Delta.Length / 1024 / 1024), vbCrLf, _
                String.Format("Delta is {0:p} of original", Delta.Length / A.Length), vbCrLf, vbCrLf, _
                String.Format("Time to build delta: {0} ms", (t2 - t1).TotalMilliseconds), vbCrLf _
            )

    End Sub


    Private DoNewResults As String

    Sub DoNew()

        Dim B As Byte() = My.Computer.FileSystem.ReadAllBytes("g:\x.bmp")


        Dim t1 As DateTime = Now
        Dim BX As Byte() = DeflateMax(B)
        Dim t2 As DateTime = Now

        DoNewResults = _
            String.Concat _
            ( _
                String.Format("Deflate size  {0:f2} KB", BX.Length / 1024), vbCrLf, _
                String.Format("Time to compress: {0} ms", (t2 - t1).TotalMilliseconds) _
            )
    End Sub


    Private FileName As String
    Private CancelFlag As Boolean

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If String.IsNullOrEmpty(FileName) Then
            MsgBox("Não há videos gravados", MsgBoxStyle.Critical)
            Exit Sub
        End If

        PlayRecordedScreenRaw256Colors(FileName, PictureBox1)
        MsgBox("OK")
    End Sub

    Private Sub btnStartStop_Click(sender As Object, e As EventArgs) Handles btnStartStop.Click

        If Me.btnStartStop.Text = "Start" Then
            Me.btnStartStop.Text = "Stop"
            CancelFlag = False

            Dim Desktop As String = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)

            FileName = String.Format("{0}\{1:yyyyMMddhhmmss}.asc256", Desktop, Now)

            Dim t1 As Date = Now

            Dim fc As Integer = RecordScreenRaw256Colors(FileName, CancelFlag)

            Dim t2 As Date = Now

            MsgBox(String.Format("Recorded {0} frames in {1:f2} seconds at {2:f2} fps in {3}", fc, (t2 - t1).TotalSeconds, fc / (t2 - t1).TotalSeconds, FileName))

            Me.btnStartStop.Text = "Start"
        Else
            CancelFlag = True
        End If

    End Sub


    Private Sub btnPlayFromFile_Click(sender As Object, e As EventArgs) Handles btnPlayFromFile.Click

        If Not Me.OpenFileDialog1.ShowDialog() = Windows.Forms.DialogResult.OK Then
            Exit Sub
        End If

        PlayRecordedScreenRaw256Colors(OpenFileDialog1.FileName, PictureBox1)

    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load




        Me.btnStartStop.Text = "Start"


        'Dim screen As Bitmap = TakeScreenShot()

        'Dim du As New Bitmap(screen.Width, screen.Height, PixelFormat.Format16bppGrayScale)

        'Using gr As Graphics = Graphics.FromImage(du)
        '    gr.DrawImage(screen, 0, 0, screen.Width, screen.Height)
        'End Using

        'Me.PictureBox1.Image = du


        'MsgBox(RecordScreenRaw256Colors(600, "G:\my_3_seconds.aggmovie"))

        'My.Computer.FileSystem.WriteAllBytes("g:\xxxx.bmp", TakeScreenShotBimapBuffer, False)

        'Dim Bitmap As Bitmap = TakeScreenShot()

        'Dim t1 As DateTime = Now

        'Dim LadoBR As Byte() = BitmapToRaw256Colors(Bitmap)

        'Dim t2 As DateTime = Now
        'Dim md2 As TimeSpan = t2 - t1
        'MsgBox(String.Format("Total time {0} milliseconds, {1} seconds", md2.TotalMilliseconds, md2.TotalSeconds))

        'MsgBox(RecordScreenMyDiffThumb(60, "G:\my_3_seconds.aggmovie"))
        'Me.Close()
        'Exit Sub

        'My.Computer.FileSystem.WriteAllBytes("g:\abcxyzxpto.bmp", TakeScreenShotBmp, False)

        'Me.Close()
        'Exit Sub



        'Dim dd As New Thread(New ThreadStart(AddressOf DoDelta))
        'dd.Start()

        'Dim dn As New Thread(New ThreadStart(AddressOf DoNew))
        'dn.Start()

        'dd.Join()
        'dn.Join()

        'MsgBox(DoDeltaResults)
        'MsgBox(DoNewResults)

        'Me.Close()
        'Exit Sub



        'Dim bounds As Rectangle
        'Dim screenshot As System.Drawing.Bitmap
        'Dim graph As Graphics
        'bounds = Screen.PrimaryScreen.Bounds
        'screenshot = New System.Drawing.Bitmap(bounds.Width, bounds.Height)

        'graph = Graphics.FromImage(screenshot)
        'graph.CopyFromScreen(bounds.X, bounds.Y, 0, 0, bounds.Size, CopyPixelOperation.SourceCopy)

        'Dim MS As MemoryStream





        ''Bmp: Takes about 13ms
        'MS = New MemoryStream
        'screenshot.Save(MS, Drawing.Imaging.ImageFormat.Bmp)
        'Dim Bmp As Byte() = MS.ToArray


        'Dim XXXX As Byte() = BitmapToByteArray(TakeScreenShot(), ImageFormat.Bmp)


        'MsgBox(ByteArrayCompare(Bmp, XXXX))
        'Me.Close()
        'Exit Sub


        ''Bmp > Png. Takes about 33ms
        'MS = New MemoryStream
        'screenshot.Save(MS, Drawing.Imaging.ImageFormat.Png)
        'Dim Png As Byte() = MS.ToArray



        ''Png2Bmp: Takes about 24ms
        'MS = New MemoryStream
        'Dim Png2Bmp As Byte()
        'Using X As New Bitmap(New MemoryStream(Png))
        '    X.Save(MS, ImageFormat.Bmp)
        '    Png2Bmp = MS.ToArray
        'End Using










        '###################################################################################
        '       RawToBitmap and BitmapToRaw works? YES, It's work very well.
        '###################################################################################
        ''Takes about 40ms
        'Dim Raw As Byte() = BitmapToRaw(screenshot)
        ''Takes about 40ms
        'Dim BitmapFromRaw As Bitmap = RawToBitmap(Raw)

        'MS = New MemoryStream
        'BitmapFromRaw.Save(MS, Drawing.Imaging.ImageFormat.Bmp)
        'Dim BitmapFromRawArray As Byte() = MS.ToArray

        'If ByteArrayCompare(Bmp, BitmapFromRawArray) Then
        '    MsgBox("OK equals")
        'Else
        '    MsgBox("NOK equals")
        'End If











        '###################################################################################
        '       Is Png2Bmp equals Bmp? Answers: Almost equals, only headers change 
        '###################################################################################
        '
        'If ByteArrayCompare(Bmp, Png2Bmp) Then
        '    MsgBox("OK")
        'Else

        '    Dim C As Byte() = ByteArrayXor(Bmp, Png2Bmp)
        '    Dim D As Byte() = ByteArrayRemoveZeros(C)

        '    Clipboard.SetText(BitConverter.ToString(D))
        '    MsgBox("NOK")
        'End If










        '###################################################################################
        '       Is Raw(Png2Bmp) equals Raw(Bmp)? Answers: Almost equals, only headers change 
        '###################################################################################
        '
        'If ByteArrayCompare(Bmp, Png2Bmp) Then
        '    MsgBox("OK")
        'Else

        '    Dim C As Byte() = ByteArrayXor(Bmp, Png2Bmp)
        '    Dim D As Byte() = ByteArrayRemoveZeros(C)

        '    Clipboard.SetText(BitConverter.ToString(D))
        '    MsgBox("NOK")
        'End If










        'Dim C As Byte() = ByteArrayXor(Bmp, Png2Bmp)
        'Dim D As Byte() = DeflateMax(C)

        'Clipboard.SetText(BitConverter.ToString(D))
        'MsgBox("OK")









        ''Compress BMP: Takes about 40ms
        'MS = New MemoryStream
        'Using DeflatingStream As New DeflateStream(MS, CompressionLevel.Fastest)
        '    DeflatingStream.Write(Bmp, 0, Bmp.Length)
        'End Using
        'Dim BmpDeflated As Byte() = MS.ToArray



        ''Decompress BMP: Takes about 39ms
        'MS = New MemoryStream(BmpDeflated)
        'Dim BmpInflated As Byte()
        'Using InflatingStream As New DeflateStream(MS, CompressionMode.Decompress)
        '    Using X As New MemoryStream
        '        InflatingStream.CopyTo(X)
        '        BmpInflated = X.ToArray
        '    End Using
        'End Using





        'Dim t2 As DateTime = Now
        'Dim md2 As TimeSpan = t2 - t1
        'MsgBox(String.Format("Total time {0} milliseconds, {1} seconds", md2.TotalMilliseconds, md2.TotalSeconds))


        'If ByteArrayCompare(Bmp, BmpInflated) Then
        '    MsgBox("OK")
        'Else
        '    MsgBox("NOK")
        'End If


        'MsgBox(Bmp.Length)
        'MsgBox(BmpInflated.Length)

        'Me.Close()
    End Sub

    Private Sub Form1_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        On Error Resume Next
        If AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData IsNot Nothing Then
            PlayRecordedScreenRaw256Colors(AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData(0), PictureBox1)
        End If

    End Sub
End Class

