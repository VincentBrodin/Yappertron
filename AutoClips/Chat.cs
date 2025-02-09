using System.Text.Json.Serialization;

namespace AutoClips;
public class Chat {
	[JsonPropertyName("model")]
	public string Model { get; }
	[JsonPropertyName("messages")]
	public List<Message> Messages { get; } = [];

	[JsonInclude]
	[JsonPropertyName("stream")]
	private bool stream = false;

	public Chat(string model) {
		Model = model;
	}
}

public enum Roles {
	User,
	Assistant,
	System
}

public class Message {
	[JsonIgnore]
	public Roles Role { get; }

	[JsonInclude]
	[JsonPropertyName("role")]
	private string roleString => Role.ToString().ToLower();
	[JsonPropertyName("content")]
	public string Content { get; }

	public Message(Roles role, string content) {
		Role = role;
		Content = content;
	}

	public Message(ResponseMessage message) {
		char[] role = message.Role.ToCharArray();
		role[0] = role[0].ToString().ToUpper()[0];
		Role = Enum.Parse<Roles>(new string(role));
		Content = message.Content;
	}
}

public class ResponseChat {

	[JsonPropertyName("message")]
	public ResponseMessage? Message { get; set; }
}

public class ResponseMessage {
	[JsonPropertyName("role")]
	public string Role { get; set; } = string.Empty;
	[JsonPropertyName("content")]
	public string Content { get; set; } = string.Empty;
}

