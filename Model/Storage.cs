using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using TimeManager.Model.Regular;
using TimeManager.Model.Tasks;
using TimeManager.Properties;
using TimeManager.Utilities;
using TimeManager.ViewModel;

namespace TimeManager.Model
{
    public static class Storage
    {
        public static readonly string Path = @"D:\Documents\TimeManager";
        private static readonly FileIO CategoriesIO = new FileIO($@"{Path}\Categories.json");
        private static readonly FileIO ActivitiesIO = new FileIO($@"{Path}\Activities.json");


        public static ObservableCollection<Category> Categories { get; private set; }
        public static ObservableCollection<RegularActivity> Activities { get; private set; }
        
        public static List<Category> RecycleBin { get; } = new List<Category>();
        public static Category SelectedCategory { get; set; }


        public static void LoadData()
        {
            Directory.CreateDirectory(Path);
            Categories = CategoriesIO.LoadData<Category>();
            Activities = ActivitiesIO.LoadData<RegularActivity>();
            foreach (var category in Categories) category.LoadTaskLists();
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

            Settings.Default.Save();
            foreach (var category in RecycleBin) category.Clear();
            RecycleBin.Clear();
            MainWindowViewModel.ShowInStatusBar($"Saved at {DateTime.Now.TimeOfDay:%h\\:mm\\:ss}");
        }
    }
}