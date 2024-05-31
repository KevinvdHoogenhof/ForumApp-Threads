using System.Diagnostics.CodeAnalysis;
using ThreadService.API.Models;

namespace ThreadService.API.SeedData
{
    [ExcludeFromCodeCoverage]
    public class SeedData
    {
        public static IEnumerable<Models.Thread> GetThreads()
        {
            return
            [
                new()
                {
                    Id = "6622190fe21ab10bc3650d6b",
                    Name = "General",
                    Description = "For general discussions",
                    Posts = 2
                },
                new()
                {
                    Id = "6622190fe21ab10bc3650d6c",
                    Name = "Guides",
                    Description = "Guides on how to accomplish something",
                    Posts = 0
                },
                new()
                {
                    Id = "6622190fe21ab10bc3650d6d",
                    Name = "Games",
                    Description = "For all discussions related to video games",
                    Posts = 0
                },
                new()
                {
                    Id = "6622184887798bcbf9d615d8",
                    Name = "Music",
                    Description = "Talk about your favorite music",
                    Posts = 0
                },
                new()
                {
                    Id = "6622190fe21ab10bc3650d6f",
                    Name = "Off-Topic",
                    Description = "For all sorts of discussions",
                    Posts = 0
                },
                new()
                {
                    Id = "6622190fe21ab10bc3650d70",
                    Name = "Nederlands",
                    Description = "Lekker Nederlands praten",
                    Posts = 0
                },
                new()
                {
                    Id = "6622190fe21ab10bc3650d71",
                    Name = "Art",
                    Description = "Everything about art",
                    Posts = 0
                },
                new()
                {
                    Id = "6622190fe21ab10bc3650d72",
                    Name = "Sport",
                    Description = "For discussions about sport",
                    Posts = 0
                },
                new()
                {
                    Id = "6622190fe21ab10bc3650d73",
                    Name = "Technology",
                    Description = "For discussing technology",
                    Posts = 0
                },
                new()
                {
                    Id = "6622190fe21ab10bc3650d74",
                    Name = "International",
                    Description = "Communicate with people from all over the world",
                    Posts = 0
                }
            ];
        }
    }
}
