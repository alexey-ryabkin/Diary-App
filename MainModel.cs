using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Diary_App
{
    internal struct FolderStatus
    {
        public bool Created;
        public int CreatedCount, OutOfCount;
        public FolderStatus(bool created, int createdCount, int outOfCount)
        {
            Created = created;
            CreatedCount = createdCount;
            OutOfCount = outOfCount;
        }
        public override string ToString()
        {
            return $"Создано: {CreatedCount} из {OutOfCount}, поэтому папка {(Created ? "существует" : "не существует")}.";
        }
    }
    internal class MainModel
    {
        internal static void CreateFolder(string path, DateTime date)
        {
            DateTime FirstOfTheMonth;
            int daysInMonth;
            string filePath;
            bool keepTrying = true;

            FirstOfTheMonth = new DateTime(date.Year, date.Month, 1);
            daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);
            while (keepTrying)
            {
                try
                {
                    Directory.CreateDirectory(path + "\\" + GetFolderName(date));
                    for (int i = 0; i < daysInMonth; i++)
                    {
                        filePath = _GetFilePath(path, FirstOfTheMonth.AddDays(i));
                        FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate);
                        fs.Close();
                    }
                    keepTrying = false;
                }
                catch
                {
                    MessageBoxResult result = System.Windows.MessageBox.Show(
                        "У меня нет прав, прими, пожалуйста, исключение для контролируемого доступа к папкам.",
                        "Ошибка доступа",
                        System.Windows.MessageBoxButton.OKCancel,
                        MessageBoxImage.Error,
                        MessageBoxResult.OK);
                    
                    if (result == MessageBoxResult.Cancel)
                    {
                        keepTrying = false;
                    }
                }
            }
        }
        internal static FolderStatus IsFolderCreated(string path, DateTime date)
        {
            // Создать список названий файлов
            // Проверить, существует ли каждый из них
            DateTime FirstOfTheMonth;
            int daysInMonth, existingFiles;
            string filePath;
            FolderStatus result;

            FirstOfTheMonth = new DateTime(date.Year, date.Month, 1);
            daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);
            existingFiles = 0;
            for (int i = 0; i < daysInMonth; i++)
            {
                filePath = _GetFilePath(path, FirstOfTheMonth.AddDays(i));
                if (File.Exists(filePath))
                {
                    existingFiles++;
                }
            }
            result = new FolderStatus(existingFiles == daysInMonth, existingFiles, daysInMonth);
            return result;
        }
        internal static void OpenFolder(string path, DateTime date)
        {
            // Добавление к пути окружающих кавычек, потому что это аргумент для explorer.exe
            // Добавление к пути к папке имени папки, которая будет открыта
            // Добавление к пути слэшей до и после имени папки
            path = $"\"{path}\\{GetFolderName(date)}\\\"";
            System.Diagnostics.Process.Start("explorer.exe", path);
            //Console.WriteLine("Открыта папка\n{0}", path);
        }
        internal static string GetFolderName(DateTime date)
        {
            string result = date.ToString("yy.MM, MMMM");
            // Делает первую букву месяца заглавной
            result = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(result);
            return result;
        }
        private static string _GetFileName(DateTime date)
        {
            string result = date.ToString("yyyy.MM.dd, dddd");
            return result;
        }
        private static string _GetFilePath(string diaryPath, DateTime date)
        {
            // Добавление к пути окружающих кавычек, потому что это аргумент для explorer.exe
            // Добавление к пути к папке имени папки, которая будет открыта
            // Добавление к пути слэшей до и после имени папки
            string path = $"{diaryPath}\\{GetFolderName(date)}\\{_GetFileName(date)}.txt";
            return path;
        }
    }
}
