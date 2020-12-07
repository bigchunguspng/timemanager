using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using TimeManager.Model.Events;
using TimeManager.Model.Regular;
using TimeManager.Model.Tasks;
using TimeManager.Properties;
using TimeManager.Utilities;
using TimeManager.ViewModel;

namespace TimeManager.Model
{
    public static class Storage
    {
        private static readonly FileIO CategoriesIO = new FileIO($@"{Path}\Categories.json");
        private static readonly FileIO ActivitiesIO = new FileIO($@"{Path}\Activities.json");
        private static readonly FileIO TopicsIO = new FileIO($@"{Path}\Events.json");

        public static string Path => @"D:\Documents\TimeManager";

        public static ObservableCollection<Category> Categories { get; private set; }
        public static ObservableCollection<RegularActivity> Activities { get; private set; }
        public static ObservableCollection<Topic> Topics { get; private set; }
        
        public static List<Category> RecycleBin { get; } = new List<Category>();
        public static Category SelectedCategory { get; set; }


        public static void LoadData()
        {
            Directory.CreateDirectory(Path);
            Categories = CategoriesIO.LoadData<Category>();
            foreach (var category in Categories) category.LoadTaskLists();
            Activities = ActivitiesIO.LoadData<RegularActivity>();
            Topics = TopicsIO.LoadData<Topic>();
        }

        public static void SaveAll()
        {
            Directory.CreateDirectory(Path);
            CategoriesIO.SaveData(Categories);
            foreach (var category in Categories)
            {
                category.SaveTaskLists();
                category.UpdateDeadlinesIndicator();
            }

            ActivitiesIO.SaveData(Activities);
            TopicsIO.SaveData(Topics);

            Settings.Default.Save();
            foreach (var category in RecycleBin) category.ClearFile();
            RecycleBin.Clear();
            MainWindowViewModel.ShowInStatusBar($"Saved at {DateTime.Now.TimeOfDay:%h\\:mm\\:ss}");
        }
    }
}