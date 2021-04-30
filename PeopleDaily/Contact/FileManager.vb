Imports System.IO
''' <summary>
''' 用来处理文件
''' </summary>
Public MustInherit Class FileManager
	Public Overloads Shared Function FileExists(path As String) As Boolean
		Return File.Exists(path)
	End Function
	Public Overloads Shared Sub SaveText(path As String, value As String)
		File.WriteAllText(path, value)
	End Sub
	Public Overloads Shared Sub SaveText(path As String, value As String, encoding As String)
		File.WriteAllText(path, value, Text.Encoding.GetEncoding(encoding))
	End Sub
	Public Overloads Shared Sub SaveImage(path As String, img As Byte())
		File.WriteAllBytes(path, img)
	End Sub
	Public Overloads Shared Sub SaveImage(path As String, img As Stream)
		Dim ms As MemoryStream = img
		Dim bytes As Byte() = ms.ToArray()
		File.WriteAllBytes(path, bytes)
	End Sub
	Public Overloads Shared Function ReadText(path As String) As String
		Return File.ReadAllText(path)
	End Function
	Public Overloads Shared Function ReadText(path As String, encoding As String)
		Return File.ReadAllText(path, Text.Encoding.GetEncoding(encoding))
	End Function

	Public Shared Function HasIllegalChars(input As String) As Boolean
		For Each illegalChar As Char In IO.Path.GetInvalidFileNameChars()
			If input.Contains(illegalChar) Then Return True
		Next
		Return False
	End Function
	Public Overloads Shared Sub DeletFile(path As String)
		File.Delete(path)
	End Sub
End Class
