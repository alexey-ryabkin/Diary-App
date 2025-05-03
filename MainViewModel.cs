using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Diary_App.Properties;

namespace Diary_App
{
    internal struct UIRow : INotifyPropertyChanged
    {
        private bool _isCurrent;
        private string _folderName;
        private bool _folderCreated;
        internal DateTime _month;
        internal int _index;
        public UIRow(bool _isCurrent, string _folderName, bool _folderCreated, DateTime _month, int _index)
        {
            this._isCurrent = _isCurrent;
            this._folderName = _folderName;
            this._folderCreated = _folderCreated;
            this._month = _month;
            this._index = _index;
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public bool IsCurrent
        {
            get { return _isCurrent; }
            set
            {
                _isCurrent = value;
                OnPropertyChanged(nameof(IsCurrent));
            }
        }
        public string FolderName
        {
            get { return _folderName; }
            set
            {
                _folderName = value;
                OnPropertyChanged(nameof(FolderName));
            }
        }
        public bool FolderCreated
        {
            get { return _folderCreated; }
            set
            {
                _folderCreated = value;
                OnPropertyChanged(nameof(FolderCreated));
            }
        }
    }
    internal class MainViewModel : INotifyPropertyChanged
    {
        private string path;
        private DateTime[] months;
        const int _rows = 7;
        const int offset = 3;
        public ObservableCollection<UIRow> Rows { get; set; }
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public string Path
        {
            get { return path; }
            set
            {
                path = value;
                OnPropertyChanged(nameof(Path));
                Properties.Settings.Default.Save();
            }
        }

        public MainViewModel()
        {
            path = Properties.Settings.Default.DiaryPath;
            if (path == null || path == "")
            {
                path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\MyDiary";
                Properties.Settings.Default.DiaryPath = path;
                Properties.Settings.Default.Save();
            }

            months = new DateTime[_rows];
            for (int i = 0; i < _rows; i++)
            {
                int year = DateTime.Now.Year;
                int month = DateTime.Now.Month + i - offset;
                if (month < 1)
                {
                    year--;
                    month += 12;
                }
                else if (month > 12)
                {
                    year++;
                    month -= 12;
                }
                months[i] = new DateTime(year, month, 1);
            }

            Rows = new ObservableCollection<UIRow>();

            for (int i = 0; i < _rows; i++)
            {
                bool iscurrent = (i == offset);
                string folderName = MainModel.GetFolderName(months[i]);
                bool folderCreated = MainModel.IsFolderCreated(path, months[i]).Created;
                DateTime month = months[i];

                Rows.Add(new UIRow(iscurrent, folderName, folderCreated, month, i));

                Debug.WriteLine("Настроена папка " + folderName);
                Debug.WriteLine(iscurrent ? "Это папка текущего месяца" : "Это папка прошлого или будущего месяца");
                Debug.WriteLine("Папка " + folderName + " " + (folderCreated ? "существует" : "не существует"));
            }
        }
        public void UpdateButton(int i)
        {
            bool iscurrent = Rows[i].IsCurrent;
            string folderName = Rows[i].FolderName;
            DateTime month = Rows[i]._month;
            bool folderCreated = MainModel.IsFolderCreated(path, month).Created;

            Rows[i] = new UIRow(iscurrent, folderName, folderCreated, month, i);

            Debug.WriteLine("Настроена папка " + folderName);
            Debug.WriteLine(iscurrent ? "Это папка текущего месяца" : "Это папка прошлого или будущего месяца");
            Debug.WriteLine("Папка " + folderName + " " + (folderCreated ? "существует" : "не существует"));
        }
        public void CreateOrOpen(UIRow button)
        {
            if (button.FolderCreated)
            {
                MainModel.OpenFolder(Path, button._month);
            }
            else
            {
                MainModel.CreateFolder(Path, button._month);
            }
        }
    }
    public class ButtonActionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool isCreated)
            {
                return isCreated ? "Открыть" : "Создать";
            }
            // Недостижимый код
            throw new NotSupportedException("Вы пытаетесь перевести не bool в надпись на кнопке.");
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Недостижимый код
            throw new NotSupportedException("Вы пытаетесь перевести надпись на кнопке в bool.");
        }
    }
}
