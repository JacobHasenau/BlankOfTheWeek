namespace Domain.Models;

public interface IEmotionState
{
    long Id { get; }
    string Name { get; }
    string? Description { get; }
    float Strangeness { get; }
    public DateTime? LastReturned { get; }

    void StateReturned(DateTime timeReturned);
    void DescriptionFound(string description, float strangeness);
}