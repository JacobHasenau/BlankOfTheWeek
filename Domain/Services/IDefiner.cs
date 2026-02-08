using Domain.Models;

namespace Domain.Services;

public interface IDefiner
{
    Task<DefinedWord?> DefineWord(string word);
}
