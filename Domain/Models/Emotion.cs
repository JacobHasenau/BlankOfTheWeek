using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models;

public class Emotion: IBlankOfTheDay
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
    public float Strangness => _state.Strangness;
    public DateTime? LastReturned => _state.LastReturned;

    public void EmotionFetched()
    {
        _state.StateReturned(DateTime.UtcNow);
    }

    private class EmotionState : IEmotionState
    {
        public long Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public float Strangness { get; set; }
        public DateTime? LastReturned { get; set; }

        public void DescriptionFound(string description, float strangness)
        {
            Description = description;
            Strangness = strangness;
        }

        public void StateReturned(DateTime timeReturned)
        {
            LastReturned = timeReturned;
        }
    }
}
