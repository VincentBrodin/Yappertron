using FFmpeg;
using TextToSpeech;

namespace AutoClips;
public static class Program {
	private static async Task Main(string[] args) {
		string projectName = GetString("Enter name of project: ");
		Console.WriteLine("Creating project files...");
		if(!Directory.Exists("projects")) {
			Directory.CreateDirectory("projects");
		}
		string projectPath = "projects/" + projectName;
		Directory.CreateDirectory(projectPath);
		Console.WriteLine("Project files created");

		string inputPath = GetString("Enter path to story: ");
		string inputContent = await File.ReadAllTextAsync(inputPath);

		Console.WriteLine("Creating audio...");
		string audioPath = $"{projectPath}/audio.wav";
		string subtitlePath = $"{projectPath}/subtitles.ass";
		await SpeechClient.GenerateSpeech(inputContent, audioPath, subtitlePath);

		//string tempPath = $"{projectPath}/video_temp.mp4";
		string outputPath = $"{projectPath}/video.mp4";

		double audioDuration = await FFmpegClient.GetDuration(audioPath) + 0.5;
		double videoDuration = await FFmpegClient.GetDuration("vid.mp4");

		// Gets a random start and end time based on the input audios length.
		Random rnd = new();
		int videoEnd = rnd.Next((int)audioDuration, (int)videoDuration);
		int videoStart = videoEnd - (int)audioDuration;

		await FFmpegClient.CutVideoWithAudio("vid.mp4", audioPath, outputPath, videoStart, videoEnd);
		//await FFmpegClient.CutVideoWithAudio("vid.mp4", audioPath, tempPath, videoStart, videoEnd);
		//await FFmpegClient.AddSubtitles(tempPath, subtitlePath, outputPath);
		//File.Delete(tempPath);
	}

	private static string GetString(string prompt) {
		while(true) {
			Console.Write(prompt);
			string? output = Console.ReadLine();
			if(!string.IsNullOrEmpty(output)) {
				return output;
			}
		}
	}

	static async void SplitStories(string path) {
		string stories_text = await File.ReadAllTextAsync(path);
		string[] stories = stories_text.Split("<eos>");
		Console.WriteLine(stories.Length);

		List<Task> tasks = [];
		for(int i = 0; i < stories.Length; i++) {
			string story = StoryTools.Clean(stories[i]);
			tasks.Add(File.WriteAllTextAsync($"stories/{i}.txt", story));
		}

		Task.WaitAll(tasks);
	}
}
