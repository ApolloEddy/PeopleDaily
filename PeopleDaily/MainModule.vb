Imports PeopleDaily.Zero

Module MainModule

	Sub Main()

		' 以下为载入程序时输出的信息
		PutsProgramInfo()

		' 以下是测试单元内容
		'Test()

		' 以下是程序最终运行内容
		Dim p_ap As New Apinion()
		p_ap.AutoGet(My.Computer.FileSystem.CurrentDirectory)

		' 以下为控制台窗体滞留措施
		Console.WriteLine("finish!")
		Console.ReadKey()

	End Sub

	Sub Test()
		Dim p_ap As New Apinion()
		p_ap.DownloadChapter(Chapter.Create("http://opinion.people.com.cn/n1/2021/0429/c1003-32091154.html"), "D:\VS2019\VBDemo\PeopleDaily\PeopleDaily")
		Console.WriteLine()
	End Sub
	Sub PutsProgramInfo()
		Dim info = My.Application.Info
		Dim InfoString As String =
$"欢迎使用 [{info.AssemblyName} v{info.Version}]，{info.Description}
By [{info.CompanyName}]		 {info.Copyright} - {Year(Now)}
作者[洛尘] QQ：2573993130		QQ交流群：1045839144	
GitHub项目地址：https://github.com/ApolloEddy/VIPMusicDownloader/
"
		Console.WriteLine(InfoString)
	End Sub
End Module
