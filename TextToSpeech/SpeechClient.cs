using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System.Text;
using System.Text.Json.Serialization;

namespace TextToSpeech;

public static class SpeechClient {
	private static string? SPEECH_KEY;
	private static string? SPEECH_REGION;
	private static SpeechConfig? speechConfig;

	public static async Task GenerateSpeech(string input, string outputAudio, string outputSubtitles) {
		if(speechConfig == null) {
			CreateSpeechConfig();
		}

		using SpeechSynthesizer speechSynthesizer = new(speechConfig, null);
		SpeechSynthesisResult ttsResult = await speechSynthesizer.SpeakTextAsync(input);
		await File.WriteAllBytesAsync(outputAudio, ttsResult.AudioData);

		using AudioConfig audioConfig = AudioConfig.FromWavFileInput(outputAudio);
		using SpeechRecognizer recognizer = new(speechConfig, audioConfig);

		//SpeechRecognitionResult recognizeResult = await recognizer.RecognizeOnceAsync();
		//string json = recognizeResult.Properties.GetProperty(PropertyId.SpeechServiceResponse_JsonResult);

		//Root? root = JsonSerializer.Deserialize<Root>(json);
		//if(root == null) {
		//return;
		//}

		//string assContent = GenerateAss(root.NBest);
		//await File.WriteAllTextAsync(outputSubtitles, assContent);
	}

	public static async Task GenerateSpeech(string input, string outputAudio) {
		if(speechConfig == null) {
			CreateSpeechConfig();
		}

		using SpeechSynthesizer speechSynthesizer = new(speechConfig, null);
		SpeechSynthesisResult ttsResult = await speechSynthesizer.SpeakTextAsync(input);
		await File.WriteAllBytesAsync(outputAudio, ttsResult.AudioData);

		using AudioConfig audioConfig = AudioConfig.FromWavFileInput(outputAudio);
		using SpeechRecognizer recognizer = new(speechConfig, audioConfig);
	}


	private static string GenerateAss(List<NBest> nBests) {
		StringBuilder sb = new();
		sb.AppendLine("[Script Info]");
		sb.AppendLine("; Subtitles generated with Azure Speech to Text");
		sb.AppendLine("Title: Generated Subtitles");
		sb.AppendLine("ScriptType: v4.00+");
		sb.AppendLine("Collisions: Normal");
		sb.AppendLine("PlayResX: 1280");
		sb.AppendLine("PlayResY: 720");
		sb.AppendLine("Timer: 100.0000");
		sb.AppendLine();
		sb.AppendLine("[V4+ Styles]");
		sb.AppendLine("Format: Name,Fontname,Fontsize,PrimaryColour,SecondaryColour,OutlineColour,BackColour,Bold,Italic,Underline,StrikeOut,ScaleX,ScaleY,Spacing,Angle,BorderStyle,Outline,Shadow,Alignment,MarginL,MarginR,MarginV,Encoding");
		sb.AppendLine("Style: Default,Arial,36,&H00FFFFFF,&H000000FF,&H00000000,&H80000000,0,0,0,0,100,100,0,0,3,2,0,2,10,10,20,1");
		sb.AppendLine();
		sb.AppendLine("[Events]");
		sb.AppendLine("Format: Layer, Start, End, Style, Name, MarginL, MarginR, MarginV, Effect, Text");

		foreach(NBest nBest in nBests) {
			foreach(WordTTS word in nBest.Words) {
				string startTime = FormatTime(word.Offset);
				string endTime = FormatTime(word.Offset + word.Duration);
				sb.AppendLine($"Dialogue: 0,{startTime},{endTime},Default,,0,0,0,,{word.Word}");
			}
		}

		return sb.ToString();
	}

	private static string FormatTime(int? milliseconds) {
		if(milliseconds == null) {
			return "0:00:00.00";
		}

		TimeSpan ts = TimeSpan.FromMilliseconds(milliseconds.Value);
		return ts.ToString(@"h\:mm\:ss\.ff");
	}
	private static void CreateSpeechConfig() {
		SPEECH_KEY = Environment.GetEnvironmentVariable("SPEECH_KEY");
		SPEECH_REGION = Environment.GetEnvironmentVariable("SPEECH_REGION");
		speechConfig = SpeechConfig.FromSubscription(SPEECH_KEY, SPEECH_REGION);
		speechConfig.SetProfanity(ProfanityOption.Raw);
		speechConfig.SpeechRecognitionLanguage = "en-US";
		speechConfig.SpeechSynthesisVoiceName = "en-US-AndrewMultilingualNeural";
		speechConfig.OutputFormat = OutputFormat.Detailed;
		speechConfig.RequestWordLevelTimestamps();
	}


	public class NBest {
		[JsonPropertyName("Words")]
		public List<WordTTS> Words { get; set; }
	}

	public class Root {
		[JsonPropertyName("NBest")]
		public List<NBest> NBest { get; set; }
	}

	public class WordTTS {
		[JsonPropertyName("Word")]
		public string Word { get; set; }

		[JsonPropertyName("Offset")]
		public int? Offset { get; set; }

		[JsonPropertyName("Duration")]
		public int? Duration { get; set; }
	}


}
