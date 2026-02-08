namespace Domain.Models;

public interface IBlankOfTheDay
{
    long Id { get; }
    string Name { get; }
    float Strangness { get; }
    DateTime? LastReturned { get; }
}