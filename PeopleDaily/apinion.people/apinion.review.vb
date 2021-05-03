Imports PeopleDaily.Zero

Public Class Apinion : Inherits People

	' 派生类字段、属性
	''' <summary>
	''' 时评网的路径：'http://opinion.people.com.cn/GB/'
	''' <para>示例：'http://opinion.people.com.cn/GB/8213/353915/353916/index.html'</para>
	''' </summary>
	''' <returns><seealso cref="String"/></returns>
	Public Overrides ReadOnly Property URL As String
		Get
			Return Host + "GB/"
		End Get
	End Property
	''' <summary>
	''' 从人民网评页面抓获当前的时评链接
	''' </summary>
	''' <returns>抓获的链接字符串：<see cref="String"/></returns>
	Public ReadOnly Property Apinion_Review_Url As String
		Get
			Const apinionUrl As String = "http://opinion.people.com.cn/GB/"
			Dim page As String = GetPageContent(apinionUrl)
			Dim blank As String = TextParser.RegParseOne(page, "人民论坛(.+?)人民时评")
			Return TextParser.ExtractOne(blank, "href=""", """")
		End Get
	End Property


	' 派生类方法、函数
	''' <summary>
	''' 从页面代码中提取章节信息
	''' </summary>
	''' <param name="page">页面源代码 text/html</param>
	''' <returns><see cref="List(Of Chapter)"/></returns>
	Public Overrides Function ExtractChapterList(page As String) As List(Of Chapter)
		Dim ret As New List(Of Chapter)
		Dim chapter As Chapter
		Dim block As String = TextParser.ExtractOne(TextParser.TrimNewlineChars(page), "<td class=""t11""", "</body>")
		For Each item As String In TextParser.Extract(block, "<a ", "<br>")
			chapter = New Chapter()
			ret.Add(chapter.ParseTitleAndLink(item, Host))
		Next
		Return ret
	End Function
	''' <summary>
	''' 迭代获取目标url的页数及之后所有页数对应的页面
	''' </summary>
	''' <param name="url"></param>
	''' <returns><see cref="List(Of Chapter)"/></returns>
	Public Overloads Function GetChapterList(url As String) As List(Of Chapter)
		Dim ret As New List(Of Chapter)
		Dim page As String
		Dim block As String
		Dim nextPageLink As String = url
		Dim lastSeperaterPosition = url.LastIndexOf("/")
		Dim pagePath As String = url.Substring(0, lastSeperaterPosition + 1) ' 这一步是为了获取页面的父级文件夹
		Do
			page = GetPageContent(nextPageLink)
			block = TextParser.ExtractOne(page, "<td align='right' nowrap", "</td>")
			ret.AddRange(ExtractChapterList(page))
			nextPageLink = pagePath + TextParser.Extract(block, "<a href='", "'").Last
		Loop Until Not block.Contains("下一页")
		ret.Reverse()
		Return ret
	End Function
	''' <summary>
	''' 下载列表中的所有文章
	''' </summary>
	''' <param name="chapterList">文章对象的列表</param>
	''' <param name="path">保存路径</param>
	Public Overrides Sub DownloadChapters(chapterList As List(Of Chapter), path As String)
		path += "\人民日报时评\"
		If Not FileManager.FileExists(path) Then IO.Directory.CreateDirectory(path)
		For Each chapter In chapterList
			chapter = DownloadChapter(chapter, path)
			Downloaded += 1
		Next
	End Sub
	Public Overloads Sub DownloadChaptersInSingleFile(chapterList As List(Of Chapter), path As String)
		If Not FileManager.FileExists(path) Then IO.Directory.CreateDirectory(path)
		Dim baseString As New Text.StringBuilder
		Dim clist As New List(Of Chapter)
		For Each chapter As Chapter In chapterList
			Console.Write($"正在下载{chapter.Title} - {chapter.Author}...")
			clist.Add(GetChapterContent(chapter))
			baseString.Append(clist.Last.Content + vbNewLine)
			Downloaded += 1
			Console.WriteLine("成功！")
		Next
		path += "\人民日报时评.txt" ' + clist(0).Time + "-" + clist.Last.Time + ".txt"
		path = path.Replace("-", "_")
		IO.File.CreateText(path).Write(baseString.ToString)
	End Sub

	Public Overloads Sub AutoGetInSingleFile(path As String)
		Console.Write("正在获取文章信息...")
		Dim chapterList = GetChapterList(Apinion_Review_Url)
		Console.WriteLine("完毕！")
		Console.WriteLine($"共{chapterList.Count}篇文章。开始下载！")
		Console.WriteLine()
		DownloadChaptersInSingleFile(chapterList, path)
		Console.WriteLine($"下载完毕！共 [{Downloaded}] 篇文章。")
	End Sub
	' 整合到一个方法
	''' <summary>
	''' 自动下载时评文章
	''' </summary>
	Public Overrides Sub AutoGet(path As String)
		Console.Write("正在获取文章信息...")
		Dim chapterList = GetChapterList(Apinion_Review_Url)
		Console.WriteLine("完毕！")
		Console.WriteLine($"共{chapterList.Count}篇文章。开始下载！")
		Console.WriteLine()
		DownloadChapters(chapterList, path)
		Console.WriteLine($"下载完毕！共 [{Downloaded}] 篇文章。")
	End Sub

End Class

