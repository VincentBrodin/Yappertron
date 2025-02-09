using System.Text.RegularExpressions;

namespace AutoClips;
public class Story {
}

public static class StoryTools {
	public static string RemoveThink(string input) {
		const string pattern = @"<think>.*?</think>";
		return Regex.Replace(input, pattern, "", RegexOptions.Singleline);
	}

	public static string Clean(string input) {
		input = input.Replace("<nl>", "\n");
		const string pattern = "<[^>]*>";
		return Regex.Replace(input, pattern, "");
	}
}
