﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading;
using Microsoft.AspNetCore.Http.Connections;
using Newtonsoft.Json;

namespace server
{
    internal class NotificationsServer
    {
        public NotificationsServer()
        {
            try
            {
                Console.WriteLine("[NotificationsServer.cs] has started.");
                new Thread(new ThreadStart(this.StartListen)).Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An Exception Occurred while Listening :" + ex.ToString());
            }
        }
        private void StartListen()
        {
            this.listener.Prefixes.Add("https://localhost:44305/");
            for (; ; )
            {
                this.listener.Start();
                Console.WriteLine("{NotificationsServer.cs] is listening.");
                HttpListenerContext context = this.listener.GetContext();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;
                string rawUrl = request.RawUrl;
                string text = "";
                string text2;
                using (StreamReader streamReader = new StreamReader(request.InputStream, request.ContentEncoding))
                {
                    text2 = streamReader.ReadToEnd();
                }
                if (!(text2 == ""))
                {
                    Console.WriteLine("Notifications Requested: " + text2);
                }
                else
                {
                    Console.WriteLine("Notifications Requested (rawUrl): " + rawUrl);
                }
               
                if (rawUrl.Contains("negotiate"))
                {
                    text = JsonConvert.SerializeObject(new
                    {
                        ConnectionId = "skull",
                        negotiateVersion = 0,
                        SupportedTransports = new List<string>(),
                        url = new Uri("https://localhost:44306/"),
                        availableTransports = "[]",
                    });
                                   
                }
                if (rawUrl.StartsWith("versioncheck"))
                {
                    text = APIServer.VersionCheckResponse;
                }
                Console.WriteLine("Notifications Data: " + text2);
                Console.WriteLine("Notifications Response: " + text);
                byte[] bytes = Encoding.UTF8.GetBytes(text);
                response.ContentLength64 = (long)bytes.Length;
                response.OutputStream.Write(bytes, 0, bytes.Length);
                Thread.Sleep(1);
                this.listener.Stop();
            }
        }
        public static string VersionCheckResponse = "{\"ValidVersion\":true}";
        public static string BlankResponse = "";

        private HttpListener listener = new HttpListener();
    }
}
