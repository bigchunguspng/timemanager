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
            {
                string fileText = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<ObservableCollection<T>>(fileText);
            }
        }

        public void SaveData(object list)
        {
            using (StreamWriter writer = File.CreateText(_path))
            {
                string data = JsonConvert.SerializeObject(list);
                writer.Write(data);
            }
        }
    }
}