using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Threading;

// ReSharper disable once CheckNamespace
namespace Infomat.InfomatTools
{
    public interface Controller
    {
        void ToCef(Message message);
        void FromCef(Message message);

    }

    public class Message
    {

        public string Id { get; set; }
        public string Controller { get; set; }
        public string MessageType { get; set; }
        public string Method { get; set; }
        public IDictionary<string, string> Options { get; set; }

        public Message()
        {

        }
        public Message(string id, string messageType, string controller, IDictionary<string, string> options)
        {
            Id = id;
            MessageType = messageType;
            Controller = controller;
            Options = options;
        }
        public bool ValidateMessageType()
        {
            if (string.IsNullOrEmpty(MessageType) ||
                MessageType != "request" ||
                MessageType != "response") return false;
            else return true;

        }
        public bool ValidateMethod()
        {
            if (string.IsNullOrEmpty(Method)) return false;
            else return true;
        }

       
    }
        


    public interface IMessagesToCef
    {
        void request(string s);
        void response(string s);
    }

    public interface IMessagesFromCef
    {

        void Request(Message message);
        void Response(Message message);
    }

    
   

    public interface IExecuteScript
    {
        void ExecuteScript(string script);
    }

    public interface IConfigurable
    {
        IEnumerable<string> Properties { get; }

        string Section { get; }

        IConfig Config { get; }

       

    }

    public static class ConfigurableExtension
    {
        public static void Configure(this IConfigurable leftHand)
        {
            if (leftHand == null) return;
            if (leftHand.Properties == null) return;
            foreach (var property in leftHand.Properties)
            {
                System.Reflection.PropertyInfo prop = null;
                try
                {
                    prop = leftHand.GetType().GetProperty(property);//can be null if property not exists, can throw exceptions
                    var value = leftHand.Config.GetValue(leftHand.Section, prop.Name);//can throw exceptions 
                    prop.SetValue(leftHand, value);
                }
                catch
                {
                    if (prop != null)
                    {
                        leftHand.Config.SetValue(leftHand.Section, prop.Name, (string)prop.GetValue(leftHand));
                    }
                }
            }
        }
    }

    public interface IConfig
    {
        string GetValue(string section, string key);

        void SetValue(string section, string key, string value);
    }

   

    public class IdleChecker
    {
        public IdleChecker(int interval, int idleTimer, Action action)
        {
            _timer = new DispatcherTimer();
            Interval = interval;
            IdleTimer = idleTimer;
            _action = action;
            _timer.Interval = new TimeSpan(0, 0, 0,Interval);
            _timer.Tick += IdleTick;
            _timer.Start();
        }

        
        private readonly DispatcherTimer _timer;
        public int Interval { get; }
        public int IdleTimer { get; }
        private readonly Action _action;

        private void IdleTick(object sender, EventArgs e)
        {
            var idle = GetIdle();
            if (idle.TotalSeconds >= IdleTimer)
            {
                _action();
            }
        }

        TimeSpan GetIdle()
        {
            var lastInputInfo = new LASTINPUTINFO();
            lastInputInfo.cbSize = (uint)Marshal.SizeOf(lastInputInfo);

            GetLastInputInfo(ref lastInputInfo);

            var lastInput = DateTime.Now.AddMilliseconds(
                -(Environment.TickCount - lastInputInfo.dwTime));
            return DateTime.Now - lastInput;
        }

        [DllImport("User32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        [StructLayout(LayoutKind.Sequential)]
        // ReSharper disable once InconsistentNaming
        internal struct LASTINPUTINFO
        {
            public uint cbSize;
            public uint dwTime;
        }
    }

}
