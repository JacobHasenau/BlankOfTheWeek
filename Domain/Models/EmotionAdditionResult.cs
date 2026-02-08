namespace Domain.Models;

public record EmotionAdditionResult(bool Added, string Reason, Emotion? Emotion)
{
    public static EmotionAdditionResult Success(Emotion emotion) => new(true, string.Empty, emotion);
    public static EmotionAdditionResult AlreadyExists => new(false, "Emotion already existed", null);
    public static EmotionAdditionResult InvalidName => new(false, "Name provided cannot be an emotion name.", null);
}