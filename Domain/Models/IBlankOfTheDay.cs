namespace Domain.Models;

public interface IBlankOfTheDay
{
    long Id { get; }
    string Name { get; }
    float Strangeness { get; }
    DateTime? LastReturned { get; }
}