using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using CefSharp;

// ReSharper disable once CheckNamespace
namespace Infomat.InfomatBrowser
{
    public sealed class CustomRequestHandler : IRequestHandler
    {
        public CustomRequestHandler()
        {
            _whitelist = null;
        }

        public CustomRequestHandler(string whitelist)
        {
            try
            {
                var doc = XDocument.Load(whitelist);
                if (doc.Root != null)
                    _whitelist = new HashSet<string>(doc.Root.Elements("Host").Select(e => e.Value));
            }
            catch
            {
                // ignored
            }
        }

        private readonly HashSet<string> _whitelist;

        public static readonly string VersionNumberString =
            $"Chromium: {Cef.ChromiumVersion}, CEF: {Cef.CefVersion}, CefSharp: {Cef.CefSharpVersion}";

        bool IRequestHandler.OnBeforeBrowse(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, bool isRedirect)
        {
            var host = new Uri(request.Url).Host;

            if (_whitelist == null)
                return false;

            return !_whitelist.Contains(host);
        }

        bool IRequestHandler.OnOpenUrlFromTab(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl, WindowOpenDisposition targetDisposition, bool userGesture)
        {
            return false;
        }

        bool IRequestHandler.OnCertificateError(IWebBrowser browserControl, IBrowser browser, CefErrorCode errorCode, string requestUrl, ISslInfo sslInfo, IRequestCallback callback)
        {
            //NOTE: If you do not wish to implement this method returning false is the default behaviour
            // We also suggest you explicitly Dispose of the callback as it wraps an unmanaged resource.
            //callback.Dispose();
            //return false;

            //NOTE: When executing the callback in an async fashion need to check to see if it's disposed
            if (!callback.IsDisposed)
            {
                //using (callback)
                //{
                //    //To allow certificate
                //    //callback.Continue(true);
                //    //return true;
                //}
                callback.Dispose();
            }

            return false;
        }

        void IRequestHandler.OnPluginCrashed(IWebBrowser browserControl, IBrowser browser, string pluginPath)
        {
            // TODO: Add your own code here for handling scenarios where a plugin crashed, for one reason or another.
        }

        CefReturnValue IRequestHandler.OnBeforeResourceLoad(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IRequestCallback callback)
        {
            //NOTE: If you do not wish to implement this method returning false is the default behaviour
            // We also suggest you explicitly Dispose of the callback as it wraps an unmanaged resource.

            if(!callback.IsDisposed)
                callback.Dispose();
           // return false;

            //NOTE: When executing the callback in an async fashion need to check to see if it's disposed
            //if (!callback.IsDisposed)
            //{
            //    using (callback)
            //    {
            //        if (request.Method == "POST")
            //        {
            //            using (var postData = request.PostData)
            //            {
            //                var elements = postData.Elements;

            //                var charSet = request.GetCharSet();

            //                foreach (var element in elements)
            //                {
            //                    if (element.Type == PostDataElementType.Bytes)
            //                    {
            //                        var body = element.GetBody(charSet);
            //                    }
            //                }
            //            }
            //        }

            //        //Note to Redirect simply set the request Url
            //        //if (request.Url.StartsWith("http://92", StringComparison.OrdinalIgnoreCase))
            //        //{
            //        //    request.Url = "https://gianty.github.io/test";
            //        //}

            //        //Callback in async fashion
            //        //callback.Continue(true);
            //        //return CefReturnValue.ContinueAsync;
            //    }
            //}

            return CefReturnValue.Continue;
        }

        bool IRequestHandler.GetAuthCredentials(IWebBrowser browserControl, IBrowser browser, IFrame frame, bool isProxy, string host, int port, string realm, string scheme, IAuthCallback callback)
        {
            //NOTE: If you do not wish to implement this method returning false is the default behaviour
            // We also suggest you explicitly Dispose of the callback as it wraps an unmanaged resource.

            callback.Dispose();
            return false;
        }

        void IRequestHandler.OnRenderProcessTerminated(IWebBrowser browserControl, IBrowser browser, CefTerminationStatus status)
        {
            // TODO: Add your own code here for handling scenarios where the Render Process terminated for one reason or another.
            //browserControl.Load(CefExample.RenderProcessCrashedUrl);
            
        }

        bool IRequestHandler.OnQuotaRequest(IWebBrowser browserControl, IBrowser browser, string originUrl, long newSize, IRequestCallback callback)
        {
            //NOTE: If you do not wish to implement this method returning false is the default behaviour
            // We also suggest you explicitly Dispose of the callback as it wraps an unmanaged resource.
            //callback.Dispose();
            //return false;

            //NOTE: When executing the callback in an async fashion need to check to see if it's disposed
            if (!callback.IsDisposed)
            {
                //using (callback)
                //{
                //    //Accept Request to raise Quota
                //    //callback.Continue(true);
                //    //return true;
                //}
                callback.Dispose();
            }

            return false;
        }

        void IRequestHandler.OnResourceRedirect(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, ref string newUrl)
        {
            //Example of how to redirect - need to check `newUrl` in the second pass
            //if (string.Equals(frame.GetUrl(), "https://www.google.com/", StringComparison.OrdinalIgnoreCase) && !newUrl.Contains("github"))
            //{
            //	newUrl = "https://github.com";
            //}
        }

        bool IRequestHandler.OnProtocolExecution(IWebBrowser browserControl, IBrowser browser, string url)
        {
            return url.StartsWith("mailto");
        }

        void IRequestHandler.OnRenderViewReady(IWebBrowser browserControl, IBrowser browser)
        {

        }

        public bool  OnResourceResponse(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response)
        {
            return false;
            
        }

        public IResponseFilter GetResourceResponseFilter(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request,
            IResponse response)
        {
            return null;
        }

        public void OnResourceLoadComplete(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request,
            IResponse response, UrlRequestStatus status, long receivedContentLength)
        {
            
        }
    }
}
