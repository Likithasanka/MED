using System.Text.Json;
using MED.Models;
using Microsoft.AspNetCore.Identity;

namespace MED.Services;

// Small JSON-backed store for university demonstration;
// replace with EF Core for deployment.

public class MedStore
{
    private readonly string path;

    private readonly object gate = new();

    private MedDatabase data;

    public MedStore(IWebHostEnvironment env)
    {
        path = Path.Combine(
            env.ContentRootPath,
            "App_Data",
            "med-data.json"
        );

        Directory.CreateDirectory(
            Path.GetDirectoryName(path)!
        );

        data = File.Exists(path)
            ? JsonSerializer.Deserialize<MedDatabase>(
                File.ReadAllText(path)
              ) ?? new()
            : new();

        if (data.Users.Count == 0)
        {
            var h = new PasswordHasher<MedUser>();

            foreach (var u in new[]
            {
                new MedUser
                {
                    Id = 1,
                    Name = "Demo Employee",
                    Email = "employee@med.local",
                    Role = "Employee"
                },

                new MedUser
                {
                    Id = 2,
                    Name = "Demo Customer",
                    Email = "customer@med.local",
                    Role = "Customer"
                }
            })
            {
                u.PasswordHash = h.HashPassword(
                    u,
                    "Demo123!"
                );

                data.Users.Add(u);
            }
        }

        if (data.Notices.Count == 0)
        {
            data.Notices.AddRange(new[]
            {
                new Notice
                {
                    Id = 1,
                    Title = "Sample infusion pump safety alert",
                    Device = "Infusion Pump",
                    Type = "Safety",
                    Description = "Example notice for university testing only; not a real recall.",
                    Date = new DateTime(2026, 9, 12)
                },

                new Notice
                {
                    Id = 2,
                    Title = "Sample monitor recall notice",
                    Device = "Patient Monitor",
                    Type = "Recall",
                    Description = "Example notice for university testing only; not a real recall.",
                    Date = new DateTime(2026, 10, 4)
                },

                new Notice
                {
                    Id = 3,
                    Title = "Sample syringe device alert",
                    Device = "Syringe Device",
                    Type = "Safety",
                    Description = "Example notice for university testing only; not a real recall.",
                    Date = new DateTime(2026, 10, 22)
                }
            });
        }

        Save();
    }

    private void Save()
    {
        File.WriteAllText(
            path,
            JsonSerializer.Serialize(
                data,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                }
            )
        );
    }

    public T Read<T>(Func<MedDatabase, T> action)
    {
        lock (gate)
        {
            return action(data);
        }
    }

    public T Change<T>(Func<MedDatabase, T> action)
    {
        lock (gate)
        {
            var result = action(data);

            Save();

            return result;
        }
    }

    public static int Next<T>(
        IEnumerable<T> items,
        Func<T, int> id)
    {
        return items
            .Select(id)
            .DefaultIfEmpty()
            .Max() + 1;
    }
}