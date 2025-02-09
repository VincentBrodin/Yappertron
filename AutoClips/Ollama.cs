using System.Diagnostics;
using System.Text.Json;

namespace AutoClips;
public class Ollama {
	public bool Running { get; private set; }
	private string address = string.Empty;
	private Process? process;
	private HttpClient httpClient;

	public Ollama() {
		httpClient = new HttpClient();

	}

	public void Run(int port) {
		address = $"http://127.0.0.1:{port}";
		ProcessStartInfo startInfo = new() {
			FileName = "ollama",
			Arguments = "serve",
			RedirectStandardOutput = true,
			UseShellExecute = false,
			CreateNoWindow = true
		};
		startInfo.EnvironmentVariables.Add("OLLAMA_HOST", address);

		process = Process.Start(startInfo);

		if(process == null) {
			throw new Exception("Could not start ollama");
		}

		Running = true;
		Console.WriteLine("Ollama running");
	}

	public void Hook(int port) {
		address = $"http://127.0.0.1:{port}";
		Running = true;
	}



	public async Task<Message> SendAsync(Chat chat) {
		StringContent json = new(JsonSerializer.Serialize(chat));
		HttpResponseMessage response = await httpClient.PostAsync($"{address}/api/chat", json);
		string responseJson = await response.Content.ReadAsStringAsync();

		ResponseChat? responseChat = JsonSerializer.Deserialize<ResponseChat>(responseJson);
		if(responseChat == null || responseChat.Message == null) {
			throw new Exception("Could not convert response to chat");
		}
		return new Message(responseChat.Message);
	}

	public void Stop() {
		process?.Kill();
		process?.Dispose();
	}
}
