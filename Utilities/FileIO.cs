using System.Collections.ObjectModel;
using System.IO;
using Newtonsoft.Json;

namespace TimeManager.Utilities
{
    public class FileIO
    {
        private readonly string _path;

        public FileIO(string path) => _path = path;

        public ObservableCollection<T> LoadData<T>()
        {
            if (!File.Exists(_path))
            {
                File.CreateText(_path).Dispose();
                return new ObservableCollection<T>();
            }
            using (StreamReader reader = File.OpenText(_path))
                return JsonConvert.DeserializeObject<ObservableCollection<T>>(reader.ReadToEnd());
        }

        public void SaveData(object list)
        {
            using (StreamWriter writer = File.CreateText(_path))
                writer.Write(JsonConvert.SerializeObject(list, new JsonSerializerSettings
                {
                    DefaultValueHandling = DefaultValueHandling.Ignore
                }));
        }
    }
}