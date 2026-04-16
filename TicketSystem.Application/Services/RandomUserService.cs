using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TicketSystem.Domain.Models;

namespace TicketSystem.Application.Services
{
    public class RandomUserService : IRandomUserService
    {
        private readonly HttpClient _httpClient;
        private readonly Random _random;

        public RandomUserService()
        {
            _httpClient = new HttpClient();
            _random = new Random();
        }

        public async Task<ApplicationUser> GenerateRandomUserAsync()
        {
            try
            {
                var response = await _httpClient.GetStringAsync("https://randomuser.me/api/");
                var json = JObject.Parse(response);
                var result = json["results"][0];

                var firstName = result["name"]["first"].ToString();
                var lastName = result["name"]["last"].ToString();
                var email = result["email"].ToString();
                var picture = result["picture"]["thumbnail"].ToString();

                return new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    Name = $"{firstName} {lastName}",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow,
                    DepartmentId = _random.Next(1, 11) // Zufällige Abteilung 1-10
                };
            }
            catch
            {
                // Fallback wenn API nicht erreichbar
                return new ApplicationUser
                {
                    UserName = $"user{_random.Next(1000, 9999)}@demo.de",
                    Email = $"user{_random.Next(1000, 9999)}@demo.de",
                    Name = $"Demo User {_random.Next(1, 100)}",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                };
            }
        }

        public async Task<List<ApplicationUser>> GenerateRandomUsersAsync(int count)
        {
            var users = new List<ApplicationUser>();
            for (int i = 0; i < count; i++)
            {
                users.Add(await GenerateRandomUserAsync());
            }
            return users;
        }

        public string GetAvatarUrl(string email)
        {
            // Gravatar (einfach, keine API nötig)
            var hash = GetMd5Hash(email.Trim().ToLower());
            return $"https://www.gravatar.com/avatar/{hash}?d=identicon&s=80";
        }

        private string GetMd5Hash(string input)
        {
            using var md5 = System.Security.Cryptography.MD5.Create();
            var bytes = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }
}