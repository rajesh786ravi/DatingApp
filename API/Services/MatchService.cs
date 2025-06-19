namespace API.Services
{
    public class MatchService
    {
        public async Task HandleMatchAsync(int userId)
        {
            var userProfile = await GetUserProfileAsync(userId);
            var matches = await FindPotentialMatchesAsync(userProfile);
            var bestMatch = await EvaluateBestMatchAsync(matches);

            if (bestMatch != null)
            {
                await NotifyMatchAsync(userProfile, bestMatch);
            }
        }

        private async Task<UserProfile> GetUserProfileAsync(int userId)
        {
            await Task.Delay(300);
            return new UserProfile { Id = userId, Name = "Alice", Age = 25, Interests = new[] { "Music", "Hiking" } };
        }

        private async Task<List<UserProfile>> FindPotentialMatchesAsync(UserProfile user)
        {
            await Task.Delay(500);
            return new List<UserProfile>
            {
                new UserProfile { Id = 2, Name = "Bob", Age = 26, Interests = new[] { "Music", "Movies" } },
                new UserProfile { Id = 3, Name = "Clara", Age = 24, Interests = new[] { "Hiking", "Travel" } }
            };
        }

        private async Task<UserProfile?> EvaluateBestMatchAsync(List<UserProfile> candidates)
        {
            await Task.Delay(200);
            return candidates.FirstOrDefault();
        }

        private async Task NotifyMatchAsync(UserProfile user, UserProfile match)
        {
            await Task.Delay(200);
            Console.WriteLine($"Notified {user.Name} and {match.Name} of a match!");
        }
    }

    public class UserProfile
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int Age { get; set; }
        public required string[] Interests { get; set; }
    }
}
