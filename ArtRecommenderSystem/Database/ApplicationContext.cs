using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ArtRecommenderSystem.Models;

namespace ArtRecommenderSystem.Database
{
    public class ApplicationContext : DbContext
    {
        private static ApplicationContext _applicationContext;
        private static readonly object SyncRoot = new object();

        public DbSet<User> Users { get; set; }
        public DbSet<Preference> Preferences { get; set; }

        public User CurrentUser { get; private set; }

        private ApplicationContext() : base("DefaultConnection")
        {
        }

        public static ApplicationContext GetApplicationContext()
        {
            if (_applicationContext == null)
            {
                lock (SyncRoot)
                {
                    _applicationContext = new ApplicationContext();
                    _applicationContext.SetUser("architect");
                    _applicationContext.Preferences.Load();
                }
            }

            return _applicationContext;
        }

        public bool SetUser(string nickname)
        {
            CurrentUser =
                _applicationContext.Users.FirstOrDefault(user =>
                    user.Nickname == nickname);
            return CurrentUser != null;
        }

        public List<Preference> GetUserPreferences()
        {
            if (CurrentUser == null) return new List<Preference>();
            return Preferences.Where(pref => pref.UserId == CurrentUser.Id)
                .ToList();
        }

        public void UpdatePreference(int artId, bool isLiked)
        {
            if (CurrentUser == null) return;

            var preference =
                Preferences.FirstOrDefault(pref =>
                    pref.UserId == CurrentUser.Id && pref.ArtId == artId);
            if (preference != null)
            {
                preference.Like = isLiked ? 1 : 0;
                Entry(preference).State = EntityState.Modified;
                SaveChanges();
            }
            else
            {
                var newPreference =
                    new Preference(CurrentUser.Id, artId, isLiked ? 1 : 0);
                Preferences.Add(newPreference);
                SaveChanges();
            }
        }

        public void RemovePreference(int artId)
        {
            if (CurrentUser == null) return;

            var preference =
                Preferences.FirstOrDefault(pref =>
                    pref.UserId == CurrentUser.Id && pref.ArtId == artId);
            if (preference != null)
            {
                Preferences.Remove(preference);
                SaveChanges();
            }
        }
    }
}
