Imports PeopleDaily.Zero

Public Structure Chapter
	Dim Title As String
	Dim Link As String
	Dim Time As String
	Dim Author As String
	Dim Source As String
	Dim Content As String
	Sub New(title As String, link As String)
		Me.Title = title
		Me.Link = link
	End Sub
	''' <summary>
	''' 返回一个分析了页面源代码的章节对象
	''' </summary>
	''' <param name="page">文章源代码text/html</param>
	''' <param name="link">文章链接</param>
	''' <returns><see cref="Chapter"/></returns>
	Overloads Shared Function Create(page As String, link As String) As Chapter
		Dim chp As New Chapter()
		page = TextParser.TrimNewlineChars(page)
		Dim block As String = TextParser.ExtractOne(page, "<div class=""layout rm_txt cf"">", "<em class=""section-common-share-wrap"">")
		chp.Link = link
		Dim tp As New TextParser(block)
		chp.Title = tp.ExtractOne("<h1>", "</h1>")
		chp.Author = tp.ExtractOne("<div class=""author cf"">", "</div>").Replace("&nbsp;", "")
		chp.Time = tp.ExtractOne("<div class=""col-1-1 fl"">", " | ")
		chp.Source = tp.ExtractOne("target=""_blank"">", "</a>")
		Return chp
	End Function
	''' <summary>
	''' 先请求页面源代码，再返回一个分析了页面源代码的章节对象
	''' </summary> 
	''' <param name="link">文章链接</param>
	''' <returns><see cref="Chapter"/></returns>
	Overloads Shared Function Create(link As String) As Chapter
		Dim page As String = Apinion.GetPageContent(link)
		page = TextParser.TrimNewlineChars(page)
		Dim chp = Create(page, link)
		Return chp
	End Function
	Sub AddMessage(time As String, author As String, source As String)
		Me.Time = time
		Me.Author = author.Replace("&nbsp;", "")
		Me.Source = source
	End Sub
	''' <summary>
	''' 将一个 '_blank' 标签块的链接和标题
	''' </summary>
	''' <param name="input">输入的 '_blank' 标签块</param>
	''' <param name="host">服务主机域名</param>
	''' <returns>新的文章结构体对象<see cref="Chapter"/></returns>
	Function ParseTitleAndLink(input As String, host As String) As Chapter
		Link = host + TextParser.ExtractOne(input, "href='", "'")
		Link = Link.Replace(".cn//n", ".cn/n")
		Title = TextParser.ExtractOne(input, "target=_blank>", "</a>")
		Return Me
	End Function
	''' <summary>
	''' 从标签块代码中获取作者、时间、来源的值
	''' </summary>
	''' <param name="block">标签块代码</param>
	''' <returns>新的实例<see cref="Chapter"/></returns>
	Function ParseTimeAuthorSource(block As String) As Chapter
		Dim tp As New TextParser(block)
		Author = tp.ExtractOne("<div class=""author cf"">", "</div>").Replace("&nbsp;", "")
		Time = tp.ExtractOne("<div class=""col-1-1 fl"">", " | ")
		Source = tp.ExtractOne("target=""_blank"">", "</a>")
		Return Me
	End Function
	''' <summary>
	''' 根据给定格式生成文件名
	''' <list type="bullet">说明：</list>
	''' <list type="number">
	''' <item>　　用 '{tit}' 表示标题</item>
	''' <item>　　用 '{aut}' 表示作者</item>
	''' <item>　　用 '{tim}' 表示时间</item>
	''' <item>　　用 '{lnk}' 表示链接</item>
	''' <item>　　用 '{soc}' 表示来源</item>
	''' </list>
	''' </summary>
	''' <inheritdoc/>
	''' <param name="format">带格式的字符串</param>
	''' <returns>符合格式的文件名<see cref="String"/></returns>
	Function GenerateFileName(format As String) As String

		'
		' 说明：
		' 　　用 '{tit}' 表示标题
		' 　　用 '{aut}' 表示作者
		' 　　用 '{tim}' 表示时间
		' 　　用 '{lnk}' 表示链接
		' 　　用 '{soc}' 表示来源
		'

		format = format.Replace("{tit}", Title)
		format = format.Replace("{aut}", Author)
		format = format.Replace("{tim}", Time)
		format = format.Replace("{lnk}", Link)
		format = format.Replace("{soc}", Source)

		Return format
	End Function
End Structure
