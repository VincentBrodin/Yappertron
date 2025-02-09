using Microsoft.Web.WebView2.Core;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DesktopApp;


public delegate Task WebMessageHandler(WebMessage webMessage);
public class WebMessageRouter {
	private readonly Dictionary<string, WebMessageHandler> handlers = [];

	public void Add(string key, WebMessageHandler handler) {
		handlers.Add(key, handler);
	}

	public async void MessageRecived(object? sender, CoreWebView2WebMessageReceivedEventArgs e) {
		WebMessage webMessage = JsonSerializer.Deserialize<WebMessage>(e.WebMessageAsJson);
		webMessage.Content = e.WebMessageAsJson;
		if(handlers.TryGetValue(webMessage.Id, out WebMessageHandler? messageHandler) && messageHandler != null) {
			await messageHandler(webMessage);
		}
		else {
			Console.WriteLine($"No message handler for {webMessage.Id}");
		}
	}
}

public struct WebMessage {
	[JsonPropertyName("id")]
	public string Id { get; set; } = string.Empty;
	[JsonIgnore]
	public string Content { get; set; } = string.Empty;

	public WebMessage() { }

	public WebMessage(string id, string content) {
		Id = id;
		Content = content;
	}
}
