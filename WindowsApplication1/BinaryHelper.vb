Imports System.IO

Public Module BinaryHelper

    Public Function ByteArrayCompare(A As Byte(), B As Byte())

        If A.Length <> B.Length Then Return False

        For i As Integer = 0 To A.Length - 1
            If A(i) <> B(i) Then Return False
        Next

        Return True
    End Function

    Public Function ByteArrayXor(A As Byte(), B As Byte()) As Byte()

        If A.Length <> B.Length Then Throw New ArrayNotSameLenghtException

        Dim C(A.Length - 1) As Byte

        For i As Integer = 0 To A.Length - 1
            C(i) = A(i) Xor B(i)
        Next

        Return C

    End Function

    Public Function ByteArrayRemoveZeros(C As Byte()) As Byte()

        Dim MS As New MemoryStream
        Dim BW As New BinaryWriter(MS)

        For i As Integer = 0 To C.Length - 1
            If C(i) <> 0 Then
                BW.Write(C(i))
            End If
        Next

        Return MS.ToArray

    End Function

    Public Class ArrayNotSameLenghtException
        Inherits Exception
    End Class

    Public Function UnUsedBytes(B As Byte()) As List(Of Byte)

        Dim UsedBytes As New List(Of Byte)

        For i As Integer = 0 To B.Length - 1

            If UsedBytes.Count = 256 Then Return New List(Of Byte)

            Dim CurrentByte As Byte = B(i)

            If UsedBytes.Where(Function(x) x = CurrentByte).Count = 0 Then
                UsedBytes.Add(B(i))
            End If

        Next



        Dim Ret As New List(Of Byte)

        For i As Integer = 0 To 255

            Dim CurrentByte As Byte = i

            If UsedBytes.Where(Function(x) x = CurrentByte).Count = 0 Then
                Ret.Add(i)
            End If

        Next

        Return Ret

    End Function

    Public Function ByteArrayDiff(A As Byte(), B As Byte()) As Byte()

        If A.Length <> B.Length Then Throw New ArrayNotSameLenghtException

        Dim MS As New MemoryStream
        Dim BW As New BinaryWriter(MS)

        Dim StopByte As Byte = 0

        For i As Integer = 0 To A.Length - 1

            If (A(i) Xor B(i)) <> 0 Then ' Loop until find the first different byte

                BW.Write(i)

                Do
                    BW.Write(A(i) Xor B(i))

                    If i = A.Length - 1 Then
                        Exit Do
                    End If

                    i = i + 1
                Loop Until (A(i) Xor B(i)) = 0

                BW.Write(StopByte)
            End If

        Next

        Return Deflate(MS.ToArray)

    End Function

    Public Sub ByteArrayPatcheApply(Original As Byte(), Diff As Byte())

        Diff = Inflate(Diff)

        For iDiff As Integer = 0 To Diff.Length - 1

            Dim Address As Integer = BitConverter.ToInt32(Diff, iDiff)
            iDiff = iDiff + 4

            Do
                Original(Address) = Original(Address) Xor Diff(iDiff)
                iDiff = iDiff + 1
                Address = Address + 1
            Loop Until Diff(iDiff) = 0

        Next

    End Sub

    Public Function BinarySerialize(Graph As Object) As Byte()
        Dim Bin As New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
        Dim MS As New MemoryStream
        Bin.Serialize(MS, Graph)
        Return MS.ToArray
    End Function

    Public Function BinaryDeserialize(Buffer As Byte()) As Object
        Dim Bin As New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
        Dim MS As New MemoryStream(Buffer)
        Return Bin.Deserialize(MS)
    End Function

End Module
