using Domain.Models;

namespace Domain.Services;

public interface IEmotionService
{
    Task<EmotionAdditionResult> AddEmotion(string name, string? description = null);
    Task<Emotion?> GetRandomEmotion();
}
