using Excuses.Persistence.Shared.Models;

namespace Excuses.Persistence.InMemory.Data;

public class InMemoryDataStore
{
    public readonly List<Excuse> Excuses =
    [
        new Excuse { Id = 1, Text = "My computer exploded", Category = "work"},
        new Excuse { Id = 2, Text = "My cat hid my car keys", Category = "pets"},
        new Excuse { Id = 3, Text = "Gravity stopped working for me temporarily", Category = "general"}
    ];

    public int NextExcuseId = 3;
}