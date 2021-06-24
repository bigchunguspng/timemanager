using System;
using System.IO;

namespace TimeManager.Utilities
{
    public static class HashGenerator
    {
        /// <summary> Повертає рядок з малих латинських літер та цифр, що не збігається з ім'ям жодного файлу в теці </summary>
        public static string UniqueHash(string folderPath)
        {
            const string source = "wertuopasdfghkzxcvbnm1234567890";

            int length = 0;
            string result;
            var random = new Random();
            do
            {
                length++;
                result = "";
                for (var i = 0; i < length; i++)
                    result += source[random.Next(source.Length)];
            } while (File.Exists($@"{folderPath}\{result}.json")); //повторити якщо в папці є файл з таким ім'ям

            return result;
        }
    }
}