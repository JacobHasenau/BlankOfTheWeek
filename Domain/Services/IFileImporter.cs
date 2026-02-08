using Domain.Models;

namespace Domain.Services;

public interface IFileImporter<T> where T : IBlankOfTheDay
{
    Task<IReadOnlyCollection<T>> ImportFile(string path);
}
