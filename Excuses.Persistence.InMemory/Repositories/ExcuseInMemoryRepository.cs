using Excuses.Persistence.InMemory.Data;
using Excuses.Persistence.Shared.Models;
using Excuses.Persistence.Shared.Repositories;

namespace Excuses.Persistence.InMemory.Repositories;

public class ExcuseInMemoryRepository : IExcuseRepository
{
    private readonly InMemoryDataStore _dataStore;

    public ExcuseInMemoryRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<List<Excuse>> GetAllExcuses()
    {
        return Task.FromResult(_dataStore.Excuses);
    }

    public Task<Excuse?> GetExcuseById(int id)
    {
        var excuse = _dataStore.Excuses.FirstOrDefault(e => e.Id == id);
        return Task.FromResult(excuse);
    }

    public Task<Excuse> AddExcuse(ExcuseInputDto excuse)
    {
        _dataStore.NextExcuseId += 1;
        var newExcuse = new Excuse { Id = _dataStore.NextExcuseId, Text = excuse.Text, Category = excuse.Category };
        _dataStore.Excuses.Add(newExcuse);
        return Task.FromResult(newExcuse);
    }

    public Task<Excuse?> UpdateExcuseById(int id, ExcuseInputDto excuse)
    {
        var existingExcuse = _dataStore.Excuses.FirstOrDefault(e => e.Id == id);
        if (existingExcuse != null)
        {
            existingExcuse.Text = excuse.Text;
            existingExcuse.Category = excuse.Category;
        }
        return Task.FromResult(existingExcuse);
    }

    public Task<bool> DeleteExcuseById(int id)
    {
        var existingExcuse = _dataStore.Excuses.FirstOrDefault(e => e.Id == id);
        if (existingExcuse == null)
        {
            return Task.FromResult(false);
        }
        _dataStore.Excuses.Remove(existingExcuse);
        return Task.FromResult(true);
    }
}
