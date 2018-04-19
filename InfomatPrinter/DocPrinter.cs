using System.Collections.Generic;
using System.Drawing.Printing;
using Spire.Doc;
using Infomat.InfomatTools;
using System;

namespace Infomat.InfomatPrinter
{
    public class DocPrinterConfigurator : IConfigurable
    {
        private readonly DocPrinter _docPrinter;
        private readonly IConfig _config;
        public IConfig Config { get { return _config; } }



        public DocPrinterConfigurator(DocPrinter docPrinter, IConfig config)
        {
            _docPrinter = docPrinter;
            _config = config;
            this.Configure();
        }



        IEnumerable<string> IConfigurable.Properties { get; } = null;//new[]{};

        public string Section { get; } = "DocPrinter";
    }
    public class DocPrinter : Controller
    {
       

        private readonly IMessagesFromCef _fromCef;
        private readonly IConfigurable _configurator;

        public DocPrinter(IMessagesFromCef fromCef, IConfig config)
        {
            
            _configurator = new DocPrinterConfigurator(this, config);
            _fromCef = fromCef;
        }





        //------------Controller override-------------------
        
        
        public void FromCef(Message message)
        {
            _fromCef.Response(message);
        }
        public void ToCef(Message message)
        { 
            if (message.ValidateMethod()) { throw new ArgumentException("Method"); }

            if (message.Method == _printMethodValue)
            {
                PrintRequest(message);
            }
        }



        // ------------------Configurable --------------------
        
        //-------   {"method":"print"}
        private string _nameOfActionKey = "method";
        public string NameOfActionKey
        {
            get { return _nameOfActionKey; }
            set
            {
                if (string.IsNullOrEmpty(value)) throw new ArgumentNullException();
                _nameOfActionKey = value;
            }
        }

        private string _printMethodValue = "print";
        public string PrintMethodValue
        {
            get { return _printMethodValue; }
            set
            {
                if (string.IsNullOrEmpty(value)) throw new ArgumentNullException();
                _printMethodValue = value;
            }
        }

        //--------{"PathToDocument":"path.."}
        private string _documentPathKey = "PathToDocument";
        public string DocumentPathKey
        {
            get { return _documentPathKey; }
            set
            {
                if (string.IsNullOrEmpty(value)) throw new ArgumentNullException();
                _documentPathKey = value;
            }
        }

        





        //--------------Available Actions-----------------
        public void PrintRequest(Message message)
        {
            var parametres = message.Options;
           parametres.Remove(_nameOfActionKey);
            try {

                if (!parametres.ContainsKey(_documentPathKey))  throw new ArgumentException(_documentPathKey); 
                var path = parametres[_documentPathKey];
                Document doc = GetDocument(path);
                doc = Fill(doc, parametres);
                Print(doc);
                message.Options = new Dictionary<string, string>();
                message.Options.Add("status", "ok");
                FromCef(message);
            }
            catch (Exception)
            {
                throw;
            }

            
        }
        



        //----------------Supports--------------------------
        private Document GetDocument(string path)
        {
            try {
                Document doc = new Document();
                doc.LoadFromFile(path);
                return doc;
            }
            catch (Exception)
            {
                throw new Exception(_documentPathKey);
            }
        }
        private Document Fill(Document doc, IDictionary<string, string> parametres)
        {
            
            parametres.Remove(_documentPathKey);
            foreach(var left in parametres.Keys)
            {
                doc.Replace(left, parametres[left], false, true);
            }
            return doc;
        }
        private void Print(Document doc)
        {
            var printDocument = doc.PrintDocument;
            //Вызывает диалоговое окно, если нет принтера
            printDocument.PrintController = new StandardPrintController();
            printDocument.Print();
            //doc.SaveToFile("result.docx");
        }




        //----------------Non-actual-------------------

        public void PrintEx(Dictionary<string, string> userData)
        {
            //добавление шрифта

            //создание документа для печати и считывание содержимого из шаблона
            //сам шаблон не изменяется
            Document document = new Document();
            document.LoadFromFile("print.docx");

            //замена ключевых слов на соответсвующие им значения, полученные в виде json-объекта
            document.Replace("Number", userData["number"], false, true);
            document.Replace("FirstName", userData["firstname"], false, true);
            document.Replace("LastName", userData["lastname"], false, true);
            document.Replace("Patronymic", userData["patronymic"], false, true);
            document.Replace("ValidUntil", userData["validUntil"], false, true);
            document.Replace("Code", userData["number"], false, true);

            //печать документа
            //StandardPrintController не вызывает диалоговых окон
            var printDocument = document.PrintDocument;
            printDocument.PrintController = new StandardPrintController();
            printDocument.Print();
        }
    }
}