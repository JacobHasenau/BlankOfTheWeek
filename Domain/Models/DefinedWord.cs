namespace Domain.Models;

public record DefinedWord(string Word, Phonetic Phonetic, IReadOnlyCollection<Meaning> Meanings);