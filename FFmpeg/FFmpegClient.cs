using System.Diagnostics;

namespace FFmpeg;

public static class FFmpegClient {

	/// <summary>
	/// Returns the duration of the input media
	/// </summary>
	/// <param name="file">The path to the file</param>
	/// <returns>The duration in seconds</returns>
	public static async Task<double> GetDuration(string file) {
		FFmpegOutput output = await RunCommand("ffprobe", $"-i {file} -show_entries format=duration -v quiet -of csv=\"p=0\"");
		return double.Parse(output.Output.Replace(".", ","));
	}

	public static async Task<int> CutVideo(string input, string output, int startTime, int endTime) {
		FFmpegOutput result = await RunCommand("ffmpeg", $"-i {input} -ss {startTime} -to {endTime} -c:v copy -c:a copy {output}");
		return result.ExitCode;
	}


	public static async Task<int> CutVideoWithAudio(string input, string audio, string output, int startTime, int endTime) {
		string corePath = Path.GetDirectoryName(output) ?? "";
		string tempVideo = Path.Combine(corePath, $"{Path.GetFileNameWithoutExtension(output)}_temp{Path.GetExtension(output)}");

		int tempResult = await CutVideo(input, tempVideo, startTime, endTime);

		FFmpegOutput result = await RunCommand("ffmpeg", $"-i {tempVideo} -i {audio} -c:v copy -c:a aac -map 0:v:0 -map 1:a:0 -shortest {output}");

		File.Delete(tempVideo);
		return result.ExitCode;
	}

	public static async Task<int> AddSubtitles(string input, string subtitlesFile, string output) {
		string arguments = $"-i {input} -vf \"subtitles={subtitlesFile}\" -c:a copy {output}";
		FFmpegOutput result = await RunCommand("ffmpeg", arguments);
		return result.ExitCode;
	}

	private static async Task<FFmpegOutput> RunCommand(string app, string args) {
		ProcessStartInfo startInfo = new(app) {
			Arguments = args,
			RedirectStandardInput = false,
			RedirectStandardOutput = true,
			RedirectStandardError = false,
			UseShellExecute = false,
		};
		Process? process = Process.Start(startInfo) ?? throw new NullReferenceException("Could not start process ffmpeg");
		await process.WaitForExitAsync();
		int exit = process.ExitCode;
		string output = await process.StandardOutput.ReadToEndAsync();
		process.Dispose();
		return new FFmpegOutput() {
			ExitCode = exit,
			Output = output
		};
	}
}

public struct FFmpegOutput {
	public int ExitCode;
	public string Output;
}
