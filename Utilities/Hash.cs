using System;
using System.IO;

namespace TimeManager.Utilities
{
    public static class Hash
    {
        public static string UniqueHash(string folderPath)
        {
            const string source = "wertuopasdfghkzxcvbnm1234567890";

            int length = 0;
            string hash;
            var random = new Random();
            do
            {
                length++;
                hash = "";
                for (int i = 0; i < length; i++)
                    hash += source[random.Next(source.Length)];
            } while (File.Exists($@"{folderPath}\{hash}.json")); //повторити якщо в папці є файл з таким ім'ям

            return hash;
        }
    }
}