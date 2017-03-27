using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
//using Infomat.InfomatMachineId;
using Infomat.InfomatTools;

namespace Infomat
{

    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : IConfigurable
    {
        //private readonly InfomatBrowser.Browser _browser;

        // ReSharper disable once NotAccessedField.Local
        //private readonly InfomatCardReader.CardReader _cardReader;

        private readonly MessagesToCef _callbackObj;

        private readonly Config _config;

        private bool _cursor = true;

        public string CursorVisibility
        {
            get { return _cursor.ToString(); }
            set
            {
                if (string.IsNullOrEmpty(value)) throw new ArgumentNullException();
                try
                {
                    _cursor = bool.Parse(value);
                }
                catch{ throw new ArgumentException(); }
            }
        }
        
        public App()
        {
            //string str = AppDomain.CurrentDomain.BaseDirectory;
            //Permissions.GrantAccess(AppDomain.CurrentDomain.BaseDirectory);

            _config = new Config("settings.xml");
            //_browser = new InfomatBrowser.Browser(_config);
            //_cardReader = new InfomatCardReader.CardReader(_browser,_config);
            //_browser.RegisterAsyncJsObject("callbackObj", callbackObj);

            this.Configure();
            _callbackObj = new MessagesToCef(_config);

            if (!_cursor) Mouse.OverrideCursor = Cursors.None;

        }
            
        public IConfig Config => _config;
        public string Section { get; } = "Application";
        private void ApplicationStartup(object sender, StartupEventArgs e)
        {
            //var window = new AppWindow(_browser);
            var window = new AppWindow(_callbackObj.Browser);
            window.Show();
        }
         
        IEnumerable<string> IConfigurable.Properties { get; } = new[]
            {
                "CursorVisibility"

            };


    }
}
