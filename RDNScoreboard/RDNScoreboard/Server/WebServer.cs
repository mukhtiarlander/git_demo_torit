using System;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using System.Threading;
using HttpServer;
using HttpServer.BodyDecoders;
using HttpServer.Logging;
using HttpServer.Modules;
using HttpServer.Resources;
using HttpServer.Routing;
using HttpListener = HttpServer.HttpListener;
using Scoreboard.Library.ViewModel;
using HttpServer.Headers;
using System.Reflection;
using System.Web;
using RDN.Utilities.Config;


namespace RDNScoreboard.Server
{
    /// <summary>
    /// http://webserver.codeplex.com/
    /// </summary>
    public class WebServer
    {
        private int triedIndex = 0;
        private int[] portsToTry = { 8085, 8086, 8087, 8088, 8089, 8090 };
        public int PortNumber { get; set; }
        public IPAddress IpAddress { get; set; }
        HttpServer.Server _server = null;

        static WebServer instance = new WebServer();

        public string IndexForShowingPages { get; set; }
        public string ControlScreen { get; set; }
        public string AnnouncersScreen { get; set; }
        public string VideoOverlayAddress4x3 { get; set; }
        public string VideoOverlayAddress16x9 { get; set; }
        public string VideoOverlay2Address4x3 { get; set; }
        public string VideoOverlay2Address16x9 { get; set; }
        public string AssistTeam1 { get; set; }
        public string AssistTeam2 { get; set; }
        public string BlockTeam1 { get; set; }
        public string BlockTeam2 { get; set; }
        public string AssistBlockTeam1 { get; set; }
        public string AssistBlockTeam2 { get; set; }
        public string PenaltyTeam1 { get; set; }
        public string PenaltyTeam2 { get; set; }
        public string PenaltyTeam1and2 { get; set; }
        public string LineUpTeam1 { get; set; }
        public string LineUpTeam2 { get; set; }
        public string ScoresTeam1 { get; set; }
        public string ScoresTeam2 { get; set; }

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static WebServer()
        { }

        public static WebServer Instance
        {
            get
            {
                return instance;
            }
        }

        //public WebServer(IPAddress address)
        //{
        //    _ipAddress = address;
        //    // same as previous example.
        //    Setup();
        //}

        public void Setup()
        {
            try
            {
                _server = new HttpServer.Server();
                var module = new FileModule();
                module.Resources.Add(new FileResources("/", ScoreboardConfig.SAVE_SERVER_FILES_FOLDER));

                _server.Add(module);

                _server.RequestReceived += OnRequest;
                _server.Add(new MultiPartDecoder());

                // use one http listener.

                _server.Add(new SimpleRouter("/", "/index.html"));
                _server.Add(new JsonRouter("/GrabOverlayUpdate"));
                _server.Add(new JsonRouter("/GrabMobileUpdate"));
                _server.Add(new JsonRouter("/GetTeam1Members"));
                _server.Add(new JsonRouter("/GetTeam2Members"));
                _server.Add(new JsonRouter("/getJamNumber"));
                _server.Add(new JsonRouter("/addAssist"));
                _server.Add(new JsonRouter("/removeAssist"));
                _server.Add(new JsonRouter("/addBlock"));
                _server.Add(new JsonRouter("/removeBlock"));
                _server.Add(new JsonRouter("/scoringLoaded"));
                _server.Add(new JsonRouter("/addScore"));
                _server.Add(new JsonRouter("/removeScore"));
                _server.Add(new JsonRouter("/setBlocker1"));
                _server.Add(new JsonRouter("/setBlocker2"));
                _server.Add(new JsonRouter("/setBlocker3"));
                _server.Add(new JsonRouter("/setBlocker4"));
                _server.Add(new JsonRouter("/setPivot"));
                _server.Add(new JsonRouter("/setJammer"));
                _server.Add(new JsonRouter("/setPBox"));
                _server.Add(new JsonRouter("/addpenalty"));
                _server.Add(new JsonRouter("/getallpenaltytypes"));
                _server.Add(new JsonRouter("/loadMainScreen"));

                PortNumber = portsToTry[triedIndex];
                _server.Add(HttpListener.Create(IpAddress, PortNumber));
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType());
            }
        }

        public void Start()
        {
            try
            {
                _server.Start(10);
                IndexForShowingPages = "http://" + IpAddress + ":" + PortNumber + "/";
                ControlScreen = "http://" + IpAddress + ":" + PortNumber + "/mobile.html";
                AnnouncersScreen = "http://" + IpAddress + ":" + PortNumber + "/announcer.html";
                VideoOverlayAddress4x3 = "http://" + IpAddress + ":" + PortNumber + "/overlay.html?d=4x3";
                VideoOverlayAddress16x9 = "http://" + IpAddress + ":" + PortNumber + "/overlay.html?d=16x9";
                VideoOverlay2Address4x3 = "http://" + IpAddress + ":" + PortNumber + "/wg.html?d=4x3";
                VideoOverlay2Address16x9 = "http://" + IpAddress + ":" + PortNumber + "/wg.html?d=16x9";
                AssistTeam1 = "http://" + IpAddress + ":" + PortNumber + "/assist.html?t=1";
                AssistTeam2 = "http://" + IpAddress + ":" + PortNumber + "/assist.html?t=2";
                BlockTeam1 = "http://" + IpAddress + ":" + PortNumber + "/block.html?t=1";
                BlockTeam2 = "http://" + IpAddress + ":" + PortNumber + "/block.html?t=2";
                AssistBlockTeam1 = "http://" + IpAddress + ":" + PortNumber + "/ab.html?t=1";
                AssistBlockTeam2 = "http://" + IpAddress + ":" + PortNumber + "/ab.html?t=2";
                PenaltyTeam1 = "http://" + IpAddress + ":" + PortNumber + "/penalty.html?t=1";
                PenaltyTeam2 = "http://" + IpAddress + ":" + PortNumber + "/penalty.html?t=2";
                PenaltyTeam1and2 = "http://" + IpAddress + ":" + PortNumber + "/penaltyboth.html";
                LineUpTeam1 = "http://" + IpAddress + ":" + PortNumber + "/lineup.html?t=1";
                LineUpTeam2 = "http://" + IpAddress + ":" + PortNumber + "/lineup.html?t=2";
                ScoresTeam1 = "http://" + IpAddress + ":" + PortNumber + "/score.html?t=1";
                ScoresTeam2 = "http://" + IpAddress + ":" + PortNumber + "/score.html?t=2";
            }
            catch
            {
                triedIndex += 1;
                Setup();
                Start();
            }
        }


        private static void OnRequest(object sender, RequestEventArgs e)
        {
            e.IsHandled = true;
        }




        public void Stop()
        {
            _server.Stop(false);
        }
    }

}
