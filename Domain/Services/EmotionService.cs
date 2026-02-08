using Domain.Models;

namespace Domain.Services;

public class EmotionService(IEmotionRepoistory repo) : IEmotionService
{
    private readonly IEmotionRepoistory _repository = repo;

    public async Task<EmotionAdditionResult> AddEmotion(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return EmotionAdditionResult.InvalidName;
        }

        var existingEmotion = await _repository.GetByName(name);
        if(existingEmotion is not null)
        {
            return EmotionAdditionResult.AlreadyExists;
        }

        var emotion = new Emotion(name, description);
        emotion = _repository.Add(emotion);
        await _repository.SaveChanges();
        return EmotionAdditionResult.Success(emotion);
    }

    public async Task<Emotion?> GetRandomEmotion()
    {
        var emotions = await _repository.GetAll();
        var emotion = emotions.OrderBy(e => Guid.NewGuid()).FirstOrDefault();
        return emotion;
    }
}
