using System.Collections.Generic;
using System.Threading;
using SLSLibLib;
using System;
using Infomat.InfomatTools;

// ReSharper disable once CheckNamespace
namespace Infomat.InfomatCardReader
{

    public class CardReaderConfigurator : IConfigurable
    {
        private readonly CardReader _cardReader;
        private readonly IConfig _config;
        public IConfig Config { get { return _config; } }

        public CardReaderConfigurator(CardReader cardReader, IConfig config)
        {
            _cardReader = cardReader;
            _config = config;
            this.Configure();
        }

        IEnumerable<string> IConfigurable.Properties { get; } = new[]
            {
                "Timeout"
            };

        public string Section { get; } = "CardReader";

        public string Timeout
        {
            get { return _cardReader.Timeout; }
            set { _cardReader.Timeout = value; }
        }
    }
    public class CardReader : Controller
    {

        private readonly IMessagesFromCef _fromCef;
        private readonly CardReaderConfigurator _configurator;

        public CardReader(IMessagesFromCef fromCef, IConfig config)
        {
          
            _configurator = new CardReaderConfigurator(this, config);
            _fromCef = fromCef;
            

            _term = new EMVTerminal();
            _utils = new Utils();
            _tlv = new TLV();
            _term.InitTerminal("14", "0643");
        }



        //----------------------Supports of Communication---------------
        private Message Message { get; set; }
        private void AssembleControllerError(string error)
        {
            Message.Options = new Dictionary<string, dynamic>();
            Message.Options.Add("Status", "fail");
            Message.Options.Add("Message", error);
            FromCef(Message);
        }
        private void AssembleControllerResponse(string response)
        {
            Message.Options = new Dictionary<string, dynamic>();
            Message.Options.Add("Status", "Ok");
            if (Message.Controller == CardRequestMethod)
                Message.Options.Add("AsnUid", response);
            FromCef(Message);
        }

        //------------Controller override-------------------
        public void ToCef(Message message)
        {
            //if (options == null) { Error(); return; }
            //if (!options.ContainsKey(_method)) { Error(); return; }
            if (string.IsNullOrEmpty(message.Method)) throw new ArgumentException("Method");
            Message = message;
            if (Message.Method == CardRequestMethod) CardRequest();
            else throw new ArgumentException("Method");
            
        }
        public void FromCef(Message message)
        {
            _fromCef.Response(message);
        }
        // ------------------Configurable --------------------
        private int timeout = 30;
        public string Timeout
        {
            get { return timeout.ToString(); }
            set
            {
                if (string.IsNullOrEmpty(value)) throw new ArgumentNullException();
                try
                {
                    timeout = Int32.Parse(value);
                }
                catch { throw new ArgumentException(); }
            }
        }

        public string CardRequestMethod { get; set; } = "CardRequest";
        
        
        
        
        //--------------Available Actions-----------------
        private void CardRequest()
        {
            Thread = new Thread(ReadCard);
            Thread.Start();
        }
        private void CancelRequest()
        {
            if (!Thread.IsAlive) return;
            try
            {
                Thread.Abort();
            }
            catch
            {
                // ignored
            }
            finally
            {
                
            }
        }


        
        //----------------Work with machine-------------------
        private readonly EMVTerminal _term;
        private readonly Utils _utils;
        private readonly TLV _tlv;
        private const string EMV_APP_AID = "A0 00 00 04 87 03 07 07";
        private Thread _thread;
        public Thread Thread
        {
            get
            {
                return _thread;
            }

            set
            {
                _thread = value;
            }
        }
        public void ReadCard()
        {
            int connect;
            try
            {
                _term.ConnectToPCSCReader(_term.GetPCSCReaderName(2));
                connect = _term.WaitCardPresent(timeout);
            }
            catch
            {
                //Потеря соединения с ридером во время работы
                return;
            }

            if (connect != 0)
            {
                string fci;
                try
                {
                    _term.PowerOn();
                    fci = _term.SelectEMVApplication(EMV_APP_AID);
                    _term.PowerOff();
                }
                catch
                {
                    string error = "cardtime";

                    //!!!Вынести в настройки имя метода cardError!!!
                    //_requestor.Response("Card Reader", $"{ErrorMethod}('{error}')");
                    
                    FromCef(Message);
                    AssembleControllerError(error);

                    //Карта не успела считаться —- 
                    return;
                }

                if (fci != null)
                {
                    var asnUid = GetUid(fci);

                    //!!!Вынести в настройки имя метода idResponse!!!
                    //_requestor.ExecuteScript($"{ResponseMethod}('{asnUid}')");
                    
                    AssembleControllerResponse(asnUid);
                    //FromCef(Message);
                    //нужно вернуть asn_uid в js 

                }
            }
            else
            {
                string error = "timeout";

                //!!!Вынести в настройки имя метода cardError!!!
                //_requestor.ExecuteScript($"{ErrorMethod}('{error}')");
                AssembleControllerError(error);
            }
        }
        private string GetUid(string fci)
        {
            var df27 = _tlv.FindValue(fci, "6F/A5/BF0C/DF27");
            var asn = _utils.HexSlice(df27, 10, 8);//		asn	"0001000101000300" asn_uid "0001010003"

            return asn;//.Substring(4, 10);
        }
    }
}

