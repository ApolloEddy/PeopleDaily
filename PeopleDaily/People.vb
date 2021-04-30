Imports PeopleDaily.Zero

Public MustInherit Class People

	' 公有字段、属性、方法
	''' <summary>
	''' 人民日报服务主机域名
	''' </summary>
	Public Const Host As String = "http://opinion.people.com.cn/"
	''' <summary>
	''' 派生类的目标主页面链接
	''' </summary>
	''' <returns><see cref="String"/></returns>
	Public MustOverride ReadOnly Property URL As String
	''' <summary>
	''' 已下载的个数
	''' </summary>
	Protected Downloaded As Integer = 0
	''' <summary>
	''' 在不同页面用来提取文章信息
	''' </summary>
	''' <param name="page">页面源代码</param>
	''' <returns><see cref="List(Of Chapter)"/></returns>
	Public MustOverride Function ExtractChapterList(page As String) As List(Of Chapter)
	''' <summary>
	''' 按照文章对象<see cref="Chapter"/>下载单篇文章
	''' </summary>
	''' <param name="chapter">文章对象</param>
	''' <param name="path">存储的文件名</param>
	''' <returns><see cref="Chapter"/></returns>
	Public Overloads Function DownloadChapter(chapter As Chapter, path As String) As Chapter
		'If Not FileManager.FileExists(path) Then Throw New ArgumentException($"路径 ""{path}"" 不存在！")
		Console.Write($"正在下载文章 ""{chapter.Title}"" ...")
		For i = 1 To 6
			Try
				Dim page As String = TextParser.TrimNewlineChars(GetPageContent(chapter.Link))
				' 获取正文
				Dim block As String = TextParser.ExtractOne(page, "<div class=""layout rm_txt cf"">", "<em class=""section-common-share-wrap"">")
				Dim content As New Text.StringBuilder(chapter.Title + vbNewLine)
				For Each paragraph As String In TextParser.Extract(block, "<p>", "</p>")
					' 新的占位符：'　'，一个占位符的占位等于一个汉字的占位，两个该字符可做首行缩进
					content.Append(paragraph.Replace("&nbsp;", "").Replace(vbTab, "") + vbNewLine)
				Next
				content.Replace("<span id=""paper_num"">", "").Replace("</span>", "")
				' 获取文章更多信息
				chapter = chapter.ParseTimeAuthorSource(block)
				path += "\" + chapter.GenerateFileName("{tit}_{aut}") + ".txt"
				' 保存文件
				FileManager.SaveText(path, content.ToString)
				Console.WriteLine("成功！")
				Return chapter
			Catch ex As Exception
				If i = 6 Then
					PutsError(ex.Message)
					If Not Downloaded = -1 Then Downloaded -= 1
					Return chapter
				End If
				If i = 1 Then Console.WriteLine()
				PutsError(ex.Message + $"　正在进行第 [{i + 1}] 次下载...")
			End Try
		Next

		Return chapter
	End Function
	''' <summary>
	''' 下载列表中的所有文章
	''' </summary>
	''' <param name="chapterList">文章对象的列表</param>
	''' <param name="path">保存路径</param>
	Public MustOverride Overloads Sub DownloadChapters(chapterList As List(Of Chapter), path As String)
	''' <summary>
	''' 自动下载文章
	''' </summary>
	Public MustOverride Overloads Sub AutoGet(path As String)
	''' <summary>
	''' 适配于人民日报网站的页面请求方式，用于请求其他页面的源代码
	''' </summary>
	''' <returns>页面源代码 text/html :<see cref="String"/></returns>
	Public Overloads Shared Function GetPageContent(url As String) As String
		Dim web As New WebProtocol(url, True)
		web.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9"
		Dim contentBytes As Byte() = web.GetContentBytes()
		Return Text.Encoding.Default.GetString(contentBytes) ' 采用字符串默认编码以免无法识别中文
	End Function

	' 私有、保护的字段、属性、方法
	Private _page As String = ""
	''' <summary>
	''' 动态获取人民日报首页源码
	''' </summary>
	''' <returns>text/html：<see cref="String"/></returns>
	Protected ReadOnly Property IndexPage As String
		Get
			If _page = "" Then Return GetPageContent(Host)
			Return _page
		End Get
	End Property

	''' <summary>
	''' 适配于人民日报网站的页面请求方式,用于请求派生类的目标页面的源码
	''' </summary>
	''' <returns>页面源代码 text/html :<see cref="String"/></returns>
	Protected Overloads Function GetPageContent() As String
		Return GetPageContent(URL)
	End Function
	''' <summary>
	''' 输出报错信息
	''' </summary>
	''' <param name="message">错误信息</param>
	Protected Shared Sub PutsError(message As String)
		Console.ResetColor()
		Console.ForegroundColor = ConsoleColor.Red
		Console.WriteLine($"[Error] {message}")
		Console.ResetColor()
	End Sub

End Class

