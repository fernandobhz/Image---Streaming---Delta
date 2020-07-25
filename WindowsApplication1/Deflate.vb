Imports System.IO
Imports System.IO.Compression

Public Module Defalte

    Function Deflate(inBuffer As Byte()) As Byte()
        Using outDeflated As New MemoryStream
            Using DStream As New DeflateStream(outDeflated, CompressionLevel.Fastest)
                DStream.Write(inBuffer, 0, inBuffer.Length)
            End Using
            Return outDeflated.ToArray
        End Using
    End Function

    Function DeflateMax(inBuffer As Byte()) As Byte()
        Using outDeflated As New MemoryStream
            Using DStream As New DeflateStream(outDeflated, CompressionLevel.Optimal)
                DStream.Write(inBuffer, 0, inBuffer.Length)
            End Using
            Return outDeflated.ToArray
        End Using
    End Function

    Function Inflate(inBuffer As Byte()) As Byte()
        Dim inflatedList As New List(Of Byte)
        Using outInflated As New MemoryStream
            Using IStream As New DeflateStream(New MemoryStream(inBuffer), CompressionMode.Decompress)

                Dim iReadBytes As Integer
                Dim iBuff(0 To 16 * 1024) As Byte

                Do
                    iReadBytes = IStream.Read(iBuff, 0, iBuff.Length)
                    inflatedList.AddRange(iBuff.Take(iReadBytes))
                Loop Until iReadBytes = 0

            End Using
        End Using
        Return inflatedList.ToArray
    End Function

End Module
