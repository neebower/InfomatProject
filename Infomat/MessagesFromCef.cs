using Infomat.InfomatTools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infomat
{
    class MessagesFromCef : IMessagesFromCef, IConfigurable
    {
        private readonly IExecuteScript _requestor;

        public MessagesFromCef(IConfig config, IExecuteScript requestor)
        {
            _requestor = requestor;

            Config = config;
            this.Configure();
        }



        
        //----------------Implementation IMessagesFromCef-------------------
        public void Request(Message message)
        {
            throw new NotImplementedException();
        }

        public void Response(Message message)
        {
            string json = JsonConvert.SerializeObject(message);
            _requestor.ExecuteScript(JsMethodToResponse + "('" + json + "')");
        }



        



        //-------------------------Configurable------------------------
        public string JsMethodToResponse { get; set; } = "responseFromCef";
        public IConfig Config { get; }

        public IEnumerable<string> Properties { get; } = new string[]
        {
            "JsMethodToResponse"
        };

        public string Section { get; } = "MessagesFROMCef";

        

    }
}

