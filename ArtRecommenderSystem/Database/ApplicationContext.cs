using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using ArtRecommenderSystem.Models;

using Newtonsoft.Json;

namespace ArtRecommenderSystem.Database
{
    public class ApplicationContext : DbContext
    {
        private static ApplicationContext _instance;
        private static readonly object SyncRoot = new object();

        public DbSet<User> Users { get; set; }
        public DbSet<Preference> Preferences { get; set; }

        public List<ArtLeaf> ArtLeaves { get; }

        public User CurrentUser { get; private set; }

        public DateTime FavoritesChangedTime { get; private set; }
        public DateTime BlacklistChangedTime { get; private set; }

        private ApplicationContext() : base("DefaultConnection")
        {
            var tree = File.ReadAllText("art.json");
            var reader = new JsonTextReader(new StringReader(tree));
            var root = JsonSerializer.CreateDefault()
                .Deserialize<ArtNode>(reader);
            //root.InitParents(new[] { root.Name });
            root.InitParents(new string[0]);

            ArtLeaves = new List<ArtLeaf>(root.GetAllLeaves());

            FavoritesChangedTime = DateTime.Now;
            BlacklistChangedTime = DateTime.Now;
        }

        public static ApplicationContext GetInstance()
        {
            if (_instance == null)
            {
                lock (SyncRoot)
                {
                    _instance = new ApplicationContext();
                    _instance.SetUser("user");
                    _instance.Preferences.Load();
                }
            }

            return _instance;
        }

        public bool SetUser(string nickname)
        {
            CurrentUser =
                Users.FirstOrDefault(user => user.Nickname == nickname);
            return CurrentUser != null;
        }

        public List<Preference> GetUserPreferences()
        {
            if (CurrentUser == null) return new List<Preference>();
            return Preferences.Where(pref => pref.UserId == CurrentUser.Id)
                .ToList();
        }

        public List<Preference> GetUserPreferences(bool isFavorite)
        {
            if (CurrentUser == null) return new List<Preference>();
            return Preferences.Where(pref =>
                    pref.UserId == CurrentUser.Id &&
                    pref.Like == (isFavorite ? 1 : 0))
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

                FavoritesChangedTime = DateTime.Now;
                BlacklistChangedTime = DateTime.Now;
            }
            else
            {
                var newPreference =
                    new Preference(CurrentUser.Id, artId, isLiked ? 1 : 0);
                Preferences.Add(newPreference);
                SaveChanges();

                if (isLiked) FavoritesChangedTime = DateTime.Now;
                else BlacklistChangedTime = DateTime.Now;
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
                if (preference.Like == 1) FavoritesChangedTime = DateTime.Now;
                else BlacklistChangedTime = DateTime.Now;
            }
        }
    }
}
