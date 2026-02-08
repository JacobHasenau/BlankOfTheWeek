using Data.Models;
using Domain.Models;
using Domain.Services;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Data.Services;

public class EmotionRepository(BlankOfTheDayContext context) : IEmotionRepoistory
{
    public Emotion Add(Emotion emotion)
    {
        var emotionState = new EmotionState(emotion);
        context.Emotions.Add(emotionState);
        var newEmotion = new Emotion(emotionState);
        return newEmotion;
    }

    public void Delete(Emotion emotion)
    {
        context.Emotions.Where(e => e.Id == emotion.Id);
    }

    public async Task SaveChanges()
    {
        await context.SaveChangesAsync();
    }

    public async Task<IReadOnlyCollection<Emotion>> GetAll(int take = 100, int skip = 0)
    {
        var emotionStates = await context.Emotions
            .Take(take)
            .Skip(skip)
            .OrderBy(e => e.Id)
            .ToListAsync();

        return emotionStates.Select(es => new Emotion(es)).ToList();
    }

    public async Task<Emotion?> GetById(long Id)
    {
        var state = await context.Emotions
            .Where(e => e.Id == Id)
            .SingleOrDefaultAsync();

        if (state is null)
        {
            return null;
        }

        return new Emotion(state);
    }

    public async Task<Emotion?> GetByName(string name)
    {
        var state = await context.Emotions
            .Where(e => e.Name == name)
            .SingleOrDefaultAsync();

        if (state is null)
        {
            return null;
        }

        return new Emotion(state);
    }

    public Task<long> GetTotalCount()
    {
        throw new NotImplementedException();
    }
}
