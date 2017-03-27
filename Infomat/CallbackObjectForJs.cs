//using System;
//using System.Collections.Generic;
//using Infomat.InfomatCardReader;
//using Infomat.InfomatMachineId;
//using Infomat.InfomatPrinter;
//using Infomat.InfomatTools;
//using Infomat.InfomatBrowser;

//namespace Infomat
//{
//    public sealed class CallbackObjectForJs : IConfigurable
//    {
//        public CallbackObjectForJs(IConfig config)
//        {
//            Config = config;
//            this.Configure();

//            _machineIdGenerator = MachineIdGenerator.Instance;
//            _docPrinter = new DocPrinter();
//            _config = config;

//            _requestor = new Browser(_config);

//            //callbackObj в настройки
//            _requestor.RegisterAsyncJsObject(CallBackObjName, this);
//        }

//        public Browser Browser => _requestor;

//        private readonly DocPrinter _docPrinter;

//        private readonly MachineIdGenerator _machineIdGenerator;

//        private readonly Browser _requestor;

//        private CardReader _cardReader;

//        private readonly IConfig _config;

//        //запуск потока для ожидания и обработки карточки
//        // ReSharper disable once InconsistentNaming
//        public void cardRequest()
//        {
//            _cardReader = new CardReader(_requestor, _config);
//            _cardReader.CardRequest();
//        }

//        private string _processMachineIdMethod = "processMachineId";
//        public string ProcessMachineIdMethod
//        {
//            get { return _processMachineIdMethod; }
//            set
//            {
//                if (string.IsNullOrEmpty(value)) throw new ArgumentNullException(nameof(value));
//                _processMachineIdMethod = value;
//            }
//        }

//        private string _callBackObjName = "callbackObj";
//        public string CallBackObjName
//        {
//            get { return _callBackObjName; }
//            set
//            {
//                if (string.IsNullOrEmpty(value)) throw new ArgumentNullException(nameof(value));
//                _callBackObjName = value;
//            }
//        }


//        //реквест на получение id и респонс(возвращение id через return не работает)
//        // ReSharper disable once InconsistentNaming
//        public void requestMachineId()
//        {
//            //processMachineId в настройки
//            _requestor.ExecuteScript($"{ProcessMachineIdMethod}('{_machineIdGenerator.MachineId}');");
//        }

//        //получение данных о пользователе для печати читательского билета в формате json
//        // ReSharper disable once InconsistentNaming
//        public void sendJSON(string jsonobj)
//        {
//            //преобразование json-строки в словарь
//            Dictionary<string, string> userData = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonobj);

//            _docPrinter.FillAndPrint(userData);
//        }

//        //закрытие потока карточки при отмене операции пользователем
//        // ReSharper disable once InconsistentNaming
//        public void cancelRequest()
//        {
//            _cardReader.CancelRequest();   
//        }

//        IEnumerable<string> IConfigurable.Properties { get; } = new []
//            {
//                "CallBackObjName",
//               "ProcessMachineIdMethod"
//            };

//        public string Section { get; } = "CallBackObject";
//        public IConfig Config { get; }
//    }
//}