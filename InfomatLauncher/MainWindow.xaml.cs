using System.IO;
using System.Windows;
using System.Xml.Linq;
using System.Windows.Controls;
using System.Diagnostics;

namespace InfomatLauncher
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public partial class MainWindow : Window
    {
        private readonly XDocument _document;
        private string _path = "settings.xml";

        
        public MainWindow()
        {
            InitializeComponent();
            //Permissions.GrantAccess(AppDomain.CurrentDomain.BaseDirectory);

            if (!File.Exists(_path)) File.Create(_path).Close();
            //Permissions.GrantAccess(_path);
            try
            {
                _document = XDocument.Load(_path);
            }
            catch
            {
                new XDocument(new XElement("Settings")).Save(_path);
                _document = XDocument.Load(_path);
            }
            //document = XDocument.Load(_path);
            try
            {
                SetContent();
            }
            catch
            {
                // ignored
            }

            //SetContent();
        }

        private void SetContent()
        {
            int i = 0;
            var xElements = _document.Root?.Elements();
            if (xElements == null) return;
            foreach (var elem in xElements)
            {
                
                Grid contForFields = new Grid();

                contForFields.ColumnDefinitions.Add(new ColumnDefinition());
                contForFields.ColumnDefinitions.Add(new ColumnDefinition());

                int j = 0;
                foreach (var set in elem.Elements())
                {
                    TextBox value = new TextBox() { Text = set.Value, MaxHeight = 30 , Name = set.Name.ToString()};
                    Label key = new Label { Content = set.Name.ToString(), MaxHeight = 30 };
                    contForFields.RowDefinitions.Add(new RowDefinition());
                    contForFields.Children.Add(value);
                    contForFields.Children.Add(key);

                    Grid.SetRow(key, j);
                    Grid.SetColumn(key, 0);

                    Grid.SetRow(value, j);
                    Grid.SetColumn(value, 1);
                    j++;
                }
                GroupBox box = new GroupBox { Header = elem.Name, Content = contForFields};

                Launch.RowDefinitions.Add(new RowDefinition());
                Launch.Children.Add(box);

                Grid.SetRow(box, i);
                i++;
            }
        }

        private void ChangeCfg()
        {
            var xElements = _document.Root?.Elements().Elements();
            if (xElements != null)
                foreach(var item in xElements)
                {
                    item.Value = LauncherTools.FindChild<TextBox>(Launch, item.Name.ToString()).Text;
                }
            _document.Save(_path);
        }

        private void Configure_Click(object sender, RoutedEventArgs e)
        {
            ChangeCfg();

            if (OpenApp.IsChecked == false)
            {
                Close();
                return;
            }
            var p = new Process();
            p.StartInfo.FileName = "Infomat.exe";
            p.Start();
            Close();

        }
    }
}
