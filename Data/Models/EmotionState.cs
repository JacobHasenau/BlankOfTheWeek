using Domain.Models;
using System.Diagnostics.CodeAnalysis;

namespace Data.Models;

public class EmotionState : IEmotionState
{
    public EmotionState() { }
    
    [SetsRequiredMembers] //Hm, don't know if I like this just to have requried fields... 
    public EmotionState(Emotion emotion)
    {
        Id = emotion.Id;
        Name = emotion.Name;
        Description = emotion.Description;
        Strangness = emotion.Strangness;
        LastReturned = emotion.LastReturned;
    }

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
