namespace Domain.Models;

public class Emotion : IBlankOfTheDay
{
    private readonly IEmotionState _state;

    public Emotion(IEmotionState state)
    {
        _state = state;
    }

    public Emotion(string name, string? description)
    {
        _state = new EmotionState { Name = name, Description = description };
    }

    public long Id => _state.Id;
    public string Name => _state.Name;
    public string? Description => _state.Description;
    public float Strangeness => _state.Strangeness;
    public DateTime? LastReturned => _state.LastReturned;

    public void NewDescriptionFound(DefinedWord definition)
    {
        var newDescription = definition?.Meanings?.FirstOrDefault()?.Definition;
        if (newDescription is not null)
        {
            _state.DescriptionFound(newDescription, 0);
        }
    }

    public void EmotionFetched()
    {
        _state.StateReturned(DateTime.UtcNow);
    }

    private class EmotionState : IEmotionState
    {
        public long Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public float Strangeness { get; set; }
        public DateTime? LastReturned { get; set; }

        public void DescriptionFound(string description, float strangeness)
        {
            Description = description;
            Strangeness = strangeness;
        }

        public void StateReturned(DateTime timeReturned)
        {
            LastReturned = timeReturned;
        }
    }
}
