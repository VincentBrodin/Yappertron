
using System.IO;

namespace DesktopApp;
public static class Logger {
	private static bool started = false;
	private static string file = string.Empty;
	private static List<string> logs = [];

	private static string fullLog = string.Empty;

	public static void Write(string content) {
		if(!started) {
			Start();
		}
		lock(logs) {
			logs.Add($"[{DateTime.Now}] {content}");
			Console.WriteLine(content);

			if(logs.Count >= 10) {
				foreach(string log in logs) {
					fullLog += log + "\n";
				}
				logs.Clear();
				File.WriteAllText(file, fullLog);
			}
		}
	}
	private static void Start() {
		string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
		if(!Directory.Exists(path)) {
			Directory.CreateDirectory(path);
		}

		file = Path.Combine(path, $"log_{DateTime.Now.Ticks}.txt");
		AppDomain.CurrentDomain.ProcessExit += Exit;
	}

	private static void Exit(object? sender, EventArgs e) {
		lock(logs) {
			foreach(string log in logs) {
				fullLog += log + "\n";
			}
			logs.Clear();
			File.WriteAllText(file, fullLog);
		}
	}
}
