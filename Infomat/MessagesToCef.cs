using Infomat.InfomatBrowser;
using Infomat.InfomatCardReader;
using Infomat.InfomatPrinter;
using Infomat.InfomatTools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Infomat
{
    
    class MessagesToCef : IMessagesToCef, IConfigurable
    {
        private readonly IConfig _config;
        private readonly IMessagesFromCef _fromCef;
        private readonly ControllerFactory factoryOfControllers = new ControllerFactory();

        public Browser Browser => _requestor;
        private Browser _requestor;

        public MessagesToCef(IConfig config)
        {
            Config = config;
            this.Configure();

            _config = config;



            _requestor = new Browser(_config);
            _requestor.RegisterAsyncJsObject(CallbackObject, this);

            _fromCef = new MessagesFromCef(_config, _requestor);



        }

        //-----------Implementation IMessagesToCef----------------
        public void request(string req)
        {
            var message = JsonConvert.DeserializeObject<Message>(req);
            if (message == null) { MessageError("Empty request", message); return; }

            try
            {
                int id = 0;
                if (string.IsNullOrEmpty(message.Id) ||
                    !int.TryParse(message.Id, out id)) throw new ArgumentException("Id");

            if (string.IsNullOrEmpty(message.Controller))  throw new ArgumentException("Controller"); 

            Controller contr = factoryOfControllers.CreateController(message.Controller, _fromCef, Config);

            var response = contr.ToCef(message);

            }
            catch (Exception e)
            {
                MessageError(e.Message, message);
            }


        }
        public void response(string s)
        {
            throw new NotImplementedException();
        }



        //-----------------------Supports Communicate-------------
        private void MessageError(string error, Message message)
        {
            if (message == null) message = new Message();
            message.Options = new Dictionary<string, string>();
            message.Options.Add("Status", "fail");
            message.Options.Add("Message", error);
            _fromCef.Response(message);
        }



        //-----------------Configurable

        public IConfig Config { get; }
        public string CallbackObject { get; set; } = "requestToCef";
        public IEnumerable<string> Properties { get; } = new string[]
        {
            "CallbackObject"
        };
        public string Section { get; } = "MessagesINTOCef";
    }



    public class ControllerFactory
    {
        public Controller CreateController(string str, IMessagesFromCef fromCef, IConfig config)
        {
            if (string.IsNullOrEmpty(str)) throw new ArgumentNullException("Controller");

            switch (str)
            {
                case "CardReader": return new CardReader(fromCef, config);
                case "DocPrinter": return new DocPrinter(fromCef, config);
                case "OtherOptions": return new OtherOptions(fromCef, config);
                default: throw new ArgumentException("Controller");
            }

        }
    }
}
