using FFmpeg;
using System.IO;
using System.Text;
using TextToSpeech;

namespace DesktopApp;

public static class VideoBuilder {
	public static async Task<List<string>> BuildFolder(string path) {
		string videoPath = Directory.GetFiles(path, "*.mp4")[0];
		string[] files = Directory.GetFiles(path, "*.txt");

		List<string> outputs = [];

		for(int i = 0; i < files.Length; i++) {
			Console.WriteLine($"Starting with {i + 1}/{files.Length}");
			string file = files[i];

			string outputPath = Path.Combine(path, $"{Path.GetFileNameWithoutExtension(file)}.mp4");

			string audioPath = Path.Combine(path, "audio.wav");
			await SpeechClient.GenerateSpeech(await File.ReadAllTextAsync(file), audioPath);

			double audioDuration = await FFmpegClient.GetDuration(audioPath) + 1;
			double videoDuration = await FFmpegClient.GetDuration(videoPath);

			Random rnd = new();
			int videoEnd = rnd.Next((int)audioDuration, (int)videoDuration);
			int videoStart = videoEnd - (int)audioDuration;

			await FFmpegClient.CutVideoWithAudio(videoPath, audioPath, outputPath, videoStart, videoEnd);

			Console.WriteLine($"Done with {i + 1}/{files.Length}");

			outputs.Add(Path.GetFileName(outputPath));

			File.Delete(audioPath);
		}

		File.Delete(videoPath);
		foreach(string file in files) {
			File.Delete(file);
		}
		return outputs;
	}

}

