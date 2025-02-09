using Microsoft.Web.WebView2.Core;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;

namespace DesktopApp;
public partial class MainWindow : Window {
	private readonly WebMessageRouter router;
	public MainWindow() {
		InitializeComponent();

		router = new WebMessageRouter();
		router.Add("generate", Generate);
	}

	protected override async void OnInitialized(EventArgs e) {
		base.OnInitialized(e);

		await Web.EnsureCoreWebView2Async();
		Web.CoreWebView2.WebMessageReceived += router.MessageRecived;
		string assetPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot");
		Web.CoreWebView2.SetVirtualHostNameToFolderMapping("app.assets", assetPath, CoreWebView2HostResourceAccessKind.Allow);
		Web.CoreWebView2.Navigate("https://app.assets/index.html");



		//await Task.Delay(2000);

		//List<string> files = [];
		//foreach(string file in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.*")) {
		//	files.Add(Path.GetFileName(file));
		//}
		//Console.WriteLine(JsonSerializer.Serialize(new { id = "outputs", content = files }));

		//Web.CoreWebView2.PostWebMessageAsJson(JsonSerializer.Serialize(new { id = "outputs", content = files }));
		//Process.Start("explorer.exe", $@"{Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory)}");
	}

	private async Task Generate(WebMessage webMessage) {
		Console.WriteLine("Got message");
		Console.WriteLine("Generating temp file");
		// Generate temp dir 
		string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp");
		if(!Directory.Exists(path)) {
			Directory.CreateDirectory(path);
		}
		path = Path.Combine(path, Guid.NewGuid().ToString());
		Directory.CreateDirectory(path);

		Console.WriteLine("Writing files...");
		GenerateMessage message = JsonSerializer.Deserialize<GenerateMessage>(webMessage.Content);

		byte[] videoData = Convert.FromBase64String(message.Content.Video.Base64Data);
		await File.WriteAllBytesAsync(Path.Combine(path, message.Content.Video.Name), videoData);
		Console.WriteLine("Video done");

		for(int i = 0; i < message.Content.Files.Count; i++) {
			FileData file = message.Content.Files[i];
			byte[] fileData = Convert.FromBase64String(file.Base64Data);
			await File.WriteAllBytesAsync(Path.Combine(path, file.Name), fileData);
			Console.WriteLine($"{i + 1}/{message.Content.Files.Count}");
		}
		Console.WriteLine("Done");

		List<string> outputs = await VideoBuilder.BuildFolder(path);

		Console.WriteLine("Cleaning up...");

		Web.CoreWebView2.PostWebMessageAsJson(JsonSerializer.Serialize(new { id = "outputs", content = outputs }));
		Process.Start("explorer.exe", $@"{path}");
	}


}

public struct GenerateMessage {
	[JsonPropertyName("id")]
	public string Id { get; set; }
	[JsonPropertyName("content")]
	public GenerateContent Content { get; set; }

}
public struct GenerateContent {
	[JsonPropertyName("files")]
	public List<FileData> Files { get; set; }
	[JsonPropertyName("video")]
	public FileData Video { get; set; }
}

public struct FileData {
	[JsonPropertyName("name")]
	public string Name { get; set; }

	[JsonPropertyName("type")]
	public string Type { get; set; }
	[JsonPropertyName("base64Data")]
	public string Base64Data { get; set; }
}
