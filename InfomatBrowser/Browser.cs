using System;
using System.Collections.Generic;
using System.Windows.Controls;
using CefSharp.Wpf;
using CefSharp;
using Infomat.InfomatBrowser.Handlers;
using Infomat.InfomatTools;


// ReSharper disable once CheckNamespace
namespace Infomat.InfomatBrowser
{
    public sealed class BrowserConfigurator : IConfigurable
    {
        private readonly Browser _browser;
        private readonly IConfig _config;
        public IConfig Config { get { return _config; } }

        public BrowserConfigurator(Browser browser, IConfig config)
        {
            _browser = browser;
            _config = config;
            this.Configure();
        }
        IEnumerable<string> IConfigurable.Properties { get; } = new[]
        {
            "Address",
            "OpenNewTabs",
            "HideContextMenu",
            "EnableApplicationCache",
            "FirstTimer",
            "SecondTimer"
        };
        public string Section { get; } = "Browser";

        public string Address
        {
            get { return _browser.Address; }
            set { _browser.Address = value; }
        }
        public string OpenNewTabs
        {
            get { return _browser.OpenNewTabs; }
            set { _browser.OpenNewTabs = value; }
        }
        public string HideContextMenu
        {
            get { return _browser.HideContextMenu; }
            set { _browser.HideContextMenu = value; }
        }
        public string EnableApplicationCache
        {
            get { return _browser.EnableApplicationCache; }
            set { _browser.EnableApplicationCache = value; }
        }
        public string FirstTimer {
            get { return _browser.FirstTimer; }
            set { _browser.FirstTimer = value; }
        }

        public string SecondTimer
        {
            get { return _browser.FirstTimer; }
            set { _browser.FirstTimer = value; }
        }


    }
    public sealed class Browser : ContentControl, IExecuteScript
    {
        //-------------------Configgurable-----------------------
        public string Address
        {
            get { return _browser?.Address; }
            set
            {
                if (_browser == null)
                    throw new NullReferenceException(nameof(_browser));
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException(nameof(value));
                //!!!!!!Validation!!!!!!!!!!

                _browser.Address = value;
            }
        }

        private bool _openNewTabs = false;
        public string OpenNewTabs
        {
            get { return _openNewTabs.ToString(); }
            set
            {
                if (string.IsNullOrEmpty(value)) throw new ArgumentNullException();
                try
                {
                    _openNewTabs = bool.Parse(value);
                }
                catch { throw new ArgumentException(); }
            }
        }

        private bool _enableApplicationCache;
        public string EnableApplicationCache
        {
            get { return _enableApplicationCache.ToString(); }
            set
            {
                if (string.IsNullOrEmpty(value)) throw new ArgumentNullException();
                try
                {
                    _enableApplicationCache = bool.Parse(value);
                }
                catch { throw new ArgumentException(); }
            }
        }

        private bool _hideContextMenu = true;
        public string HideContextMenu
        {
            get { return _hideContextMenu.ToString(); }
            set
            {
                if (string.IsNullOrEmpty(value)) throw new ArgumentNullException();
                try
                {
                    _hideContextMenu = bool.Parse(value);
                }
                catch { throw new ArgumentException(); }
            }
        }

        private int _firstTimer = 180;
        private int _secondTimer = 120;
        public string FirstTimer
        {
            get { return _firstTimer.ToString(); }
            set
            {
                if (string.IsNullOrEmpty(value)) throw new ArgumentNullException();
                try
                {
                    _firstTimer = int.Parse(value);
                }
                catch { throw new ArgumentException(); }
            }
        }
        public string SecondTimer
        {
            get { return _secondTimer.ToString(); }
            set
            {
                if (string.IsNullOrEmpty(value)) throw new ArgumentNullException();
                try
                {
                    _secondTimer = int.Parse(value);
                }
                catch { throw new ArgumentException(); }
            }
        }
        //------------------------------------------


        private readonly ChromiumWebBrowser _browser;
        private readonly BrowserConfigurator _configurator;

        public Browser(IConfig config)
        {
            InitCef();

            _browser = new ChromiumWebBrowser()
            {
                //Address = "http://92.53.101.102"
                Address = "http://neebower.github.io"
               // Address = "http://google.com"
            };

            _browser.BrowserSettings.ApplicationCache = _enableApplicationCache ? CefState.Enabled : CefState.Disabled;
            

            _browser.RequestHandler = new CustomRequestHandler("HostWhitelist.xml");

            _configurator = new BrowserConfigurator(this, config);

            _defaultAddress = Address;

            if (!_openNewTabs)
                _browser.LifeSpanHandler = new CustomLifeSpanHandler();
            if (_hideContextMenu)
                _browser.MenuHandler = new CustomContextMenuHandler();
            Content = _browser;


            _idleChecker = new IdleChecker(180, 120, () => _browser.Load(_defaultAddress));
        }

        private readonly string _defaultAddress;

        private IdleChecker _idleChecker;


        public void RegisterAsyncJsObject(string name, object jsObject)
        {
            _browser.RegisterAsyncJsObject(name, jsObject);
        }



        private static void InitCef()
        {
            //Perform dependency check to make sure all relevant resources are in our output directory.
            var settings = new CefSettings();
            settings.CachePath = "cache";
            //settings.EnableInternalPdfViewerOffScreen();
            // Disable GPU in WPF and Offscreen examples until #1634 has been resolved
            settings.CefCommandLineArgs.Add("disable-gpu", "1");

            Cef.Initialize(settings, shutdownOnProcessExit: true, performDependencyCheck: true);

        }

        public void ExecuteScript(string script)
        {
            _browser?.ExecuteScriptAsync(script);
        }
    }
}