using Infomat.InfomatTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infomat
{
    class OtherOptionsConfigurator : IConfigurable
    {
        private readonly OtherOptions _otherOptions;
        private readonly IConfig _config;
        public IConfig Config { get { return _config; } }

        public OtherOptionsConfigurator(OtherOptions otheroptions, IConfig config)
        {
            _otherOptions = otheroptions;
            _config = config;
            this.Configure();
        }

        IEnumerable<string> IConfigurable.Properties { get; } = new[]
            {
                "Location"
            };

        public string Section { get; } = "OtherOptions";
        public string Location
        {
            get { return _otherOptions.Location; }
            set { _otherOptions.Location = value; }
        }
    }

    class OtherOptions : Controller
    {

        private readonly IMessagesFromCef _fromCef;
        private readonly OtherOptionsConfigurator _configurator;

        public OtherOptions(IMessagesFromCef fromCef, IConfig config)
        {
            _configurator = new OtherOptionsConfigurator(this, config);
            _fromCef = fromCef; 
        }



        //----------------------Supports of Communication---------------
        private void Error()
        {

        }



        //------------Controller override-------------------
        private readonly string _method = "method";
        public void ToCef(Message message)
        {
            
            if (!options.ContainsKey(_method)) { Error(); return; }
            switch (options[_method])
            {
                case "Location":
                    GetLocation();
                    break;

            }
        }
        public void FromCef(Message message)
        {
            _fromCef.Response(message);
        }



        // ------------------Configurable --------------------
        private string _location = "defaultLocation";
        public string Location
        {
            get { return _location; }
            set
            {
                if (string.IsNullOrEmpty(value)) throw new ArgumentNullException();
                try
                {
                    _location = value;
                }
                catch { throw new ArgumentException(); }
            }
        }



        //--------------Available Actions-----------------
        public void GetLocation()
        {
            FromCef(new Dictionary<string, string>()
            {
                {"Location", Location }
            });
        }



        //----------------Work with machine-------------------


    }
}
