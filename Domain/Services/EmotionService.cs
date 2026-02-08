using Domain.Models;
using System.Xml.Linq;

namespace Domain.Services;

public class EmotionService(IEmotionRepoistory repo, IDefiner definer) : IEmotionService
{
    private readonly IEmotionRepoistory _repository = repo;
    private readonly IDefiner _definer = definer;

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

        if (description is null) 
        {
            var definition = await _definer.DefineWord(name);
            description = definition?.Meanings?.FirstOrDefault()?.Definition;
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
        
        if (emotion?.Description is null)
        {
            var definition = await _definer.DefineWord(emotion.Name);
            if (definition is not null)
            {
                emotion.NewDescriptionFound(definition);
                await _repository.SaveChanges();
            }
        }

        return emotion;
    }
}
