using Domain.Models;

namespace Domain.Services;

public interface IEmotionRepoistory
{
    Emotion Add(Emotion emotion);
    void Delete(Emotion emotion);
    Task SaveChanges();
    Task<IReadOnlyCollection<Emotion>> GetAll(int take = 100, int skip = 0);
    Task<Emotion?> GetById(long Id);
    Task<Emotion?> GetByName(string name);
    Task<long> GetTotalCount();
}
