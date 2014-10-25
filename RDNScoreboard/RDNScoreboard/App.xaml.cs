using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.IO;
using System.Threading.Tasks;
using RDN.Utilities.Error;
using Scoreboard.Library.ViewModel;
using System.Reflection;
using System;
using System.Windows.Media.Imaging;
using System.Net.NetworkInformation;
using System.Net;

using RDN.Utilities.Version;
using RDN.Utilities.Config;
using System.Diagnostics;
using RDNScoreboard.Code;
using Scoreboard.Library.Network;
using RDN.Utilities.Util;

namespace RDNScoreboard
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
        }
        /// <summary>
        /// Copies the contents of input to output. Doesn't close either stream.
        /// </summary>
        private static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[8 * 1024];
            int len;
            while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, len);
            }
        }
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Task<bool>.Factory.StartNew(
                          () =>
                          {
                              try
                              {
                                  DirectoryInfo dir = new DirectoryInfo(ScoreboardConfig.SAVE_APPLICATION_FOLDER);
                                  if (!dir.Exists)
                                      dir.Create();
                                  //must create the log folder second.
                                  dir = new DirectoryInfo(ScoreboardConfig.LOG_SCOREBOARD_FOLDER);
                                  if (!dir.Exists)
                                      dir.Create();
                                  //must create the error folder thrid.
                                  dir = new DirectoryInfo(ScoreboardConfig.SAVE_ERRORS_FOLDER);
                                  if (!dir.Exists)
                                      dir.Create();

                                  dir = new DirectoryInfo(ScoreboardConfig.SAVE_BACKGROUND_IMAGES_FOLDER);
                                  if (!dir.Exists)
                                      dir.Create();

                                  dir = new DirectoryInfo(ScoreboardConfig.SAVE_FILES_FOLDER);
                                  if (!dir.Exists)
                                      dir.Create();

                                  dir = new DirectoryInfo(ScoreboardConfig.SAVE_LOGOS_FOLDER);
                                  if (!dir.Exists)
                                      dir.Create();

                                  dir = new DirectoryInfo(ScoreboardConfig.SAVE_ADVERTS_FOLDER);
                                  if (!dir.Exists)
                                      dir.Create();

                                  dir = new DirectoryInfo(ScoreboardConfig.SAVE_FEEDBACK_FOLDER);
                                  if (!dir.Exists)
                                      dir.Create();

                                  dir = new DirectoryInfo(ScoreboardConfig.LOG_SCOREBOARD_FOLDER);
                                  if (!dir.Exists)
                                      dir.Create();

                                  dir = new DirectoryInfo(ScoreboardConfig.SAVE_SLIDESHOW_FOLDER);
                                  if (!dir.Exists)
                                      dir.Create();

                                  dir = new DirectoryInfo(ScoreboardConfig.SAVE_SKATERS_PICTURE_FOLDER);
                                  if (!dir.Exists)
                                      dir.Create();

                                  dir = new DirectoryInfo(ScoreboardConfig.SAVE_INTRODUCTIONS_SLIDESHOW_FOLDER);
                                  if (!dir.Exists)
                                      dir.Create();

                                  dir = new DirectoryInfo(ScoreboardConfig.SAVE_SERVER_FILES_FOLDER);
                                  if (!dir.Exists)
                                      dir.Create();
                                  dir = new DirectoryInfo(ScoreboardConfig.SAVE_SERVER_FILES_FOLDER_CSS);
                                  if (!dir.Exists)
                                      dir.Create();
                                  dir = new DirectoryInfo(ScoreboardConfig.SAVE_SERVER_FILES_FOLDER_JS);
                                  if (!dir.Exists)
                                      dir.Create();
                                  dir = new DirectoryInfo(ScoreboardConfig.SAVE_SERVER_FILES_FOLDER_IMG);
                                  if (!dir.Exists)
                                      dir.Create();
                                  dir = new DirectoryInfo(ScoreboardConfig.SAVE_SERVER_FILES_FOLDER_IMG_WFT);
                                  if (!dir.Exists)
                                      dir.Create();
                                  //dir = new DirectoryInfo(ScoreboardConfig.SAVE_SERVER_FILES_FOLDER_JSEXT);
                                  //if (!dir.Exists)
                                  //    dir.Create();
                                  //dir = new DirectoryInfo(ScoreboardConfig.SAVE_SERVER_FILES_FOLDER_JSEXT_IMG);
                                  //if (!dir.Exists)
                                  //    dir.Create();



                                  loadNationalFlagsRDNation();
                                  loadAdvertisementForRDNation();
                                  loadServerFilesForRDNation();
                                  Logger.Instance.logMessage("Directories Created", LoggerEnum.message);



                              }
                              catch (Exception exception)
                              {
                                  ErrorViewModel.Save(exception, exception.GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
                              }
                              return true;
                          });
            Task<bool>.Factory.StartNew(
                        () =>
                        {
                            try
                            {
                                ScoreboardViewModel.sendActiveInstallIdToServer();
                                LogoViewModel.Instance.LoadLogos(true);
                                ErrorViewModel.checkForOldErrors();
                            }
                            catch (Exception exception)
                            {
                                ErrorViewModel.Save(exception, exception.GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
                            }
                            return true;
                        });

            Logger.Instance.logMessage("Application Opened", LoggerEnum.message);
            Logger.Instance.logMessage("Mac Address:" + Client.GetMacAddress(), LoggerEnum.message);
            try
            {
                Microsoft.VisualBasic.Devices.ComputerInfo comp = new Microsoft.VisualBasic.Devices.ComputerInfo();
                Logger.Instance.logMessage("TotalPhysicalMemory:" + RDN.Utilities.Strings.StringExt.FormatKiloBytes(Convert.ToInt64(comp.TotalPhysicalMemory)), LoggerEnum.message);
                Logger.Instance.logMessage("AvailablePhysicalMemory:" + RDN.Utilities.Strings.StringExt.FormatKiloBytes(Convert.ToInt64(comp.AvailablePhysicalMemory)), LoggerEnum.message);
                Logger.Instance.logMessage("OSFullName:" + comp.OSFullName, LoggerEnum.message);
                Logger.Instance.logMessage("OSPlatform:" + comp.OSPlatform, LoggerEnum.message);
                Logger.Instance.logMessage("OSVersion:" + comp.OSVersion, LoggerEnum.message);


            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }

            // Global exception handling
            Application.Current.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(AppDispatcherUnhandledException);


        }
        /// <summary>
        /// loads the flags for the slideshow
        /// </summary>
        private static void loadNationalFlagsRDNation()
        {
            try
            {
                var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Images/AmericanFlag.jpg");
                var path = Application.GetResourceStream(ur);
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_SLIDESHOW_FOLDER, "00000000000000000000000000000002.jpg");

                if (!File.Exists(destinationFilename))
                {
                    using (Stream file = File.OpenWrite(destinationFilename))
                    {
                        CopyStream(path.Stream, file);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
            try
            {
                var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Images/UKFlag.jpg");
                var path = Application.GetResourceStream(ur);
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_SLIDESHOW_FOLDER, "00000000000000000000000000000003.jpg");

                if (!File.Exists(destinationFilename))
                {
                    using (Stream file = File.OpenWrite(destinationFilename))
                    {
                        CopyStream(path.Stream, file);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
            try
            {
                var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Images/CanadianFlag.jpg");
                var path = Application.GetResourceStream(ur);
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_SLIDESHOW_FOLDER, "00000000000000000000000000000004.jpg");

                if (!File.Exists(destinationFilename))
                {
                    using (Stream file = File.OpenWrite(destinationFilename))
                    {
                        CopyStream(path.Stream, file);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
            try
            {
                var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Images/UKflaganim.gif");
                var path = Application.GetResourceStream(ur);
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_SLIDESHOW_FOLDER, "00000000000000000000000000000005.gif");

                if (!File.Exists(destinationFilename))
                {
                    using (Stream file = File.OpenWrite(destinationFilename))
                    {
                        CopyStream(path.Stream, file);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
            try
            {
                var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Images/UnitedStatesFlagAnim.gif");
                var path = Application.GetResourceStream(ur);
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_SLIDESHOW_FOLDER, "00000000000000000000000000000006.gif");

                if (!File.Exists(destinationFilename))
                {
                    using (Stream file = File.OpenWrite(destinationFilename))
                    {
                        CopyStream(path.Stream, file);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
            try
            {
                var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Images/Canadaflaganim.gif");
                var path = Application.GetResourceStream(ur);
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_SLIDESHOW_FOLDER, "00000000000000000000000000000007.gif");

                if (!File.Exists(destinationFilename))
                {
                    using (Stream file = File.OpenWrite(destinationFilename))
                    {
                        CopyStream(path.Stream, file);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
            try
            {
                var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Images/FranceFlag.jpg");
                var path = Application.GetResourceStream(ur);
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_SLIDESHOW_FOLDER, "00000000000000000000000000000008.jpg");

                if (!File.Exists(destinationFilename))
                {
                    using (Stream file = File.OpenWrite(destinationFilename))
                    {
                        CopyStream(path.Stream, file);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
            try
            {
                var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Images/PortugalFlag.jpg");
                var path = Application.GetResourceStream(ur);
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_SLIDESHOW_FOLDER, "00000000000000000000000000000009.jpg");

                if (!File.Exists(destinationFilename))
                {
                    using (Stream file = File.OpenWrite(destinationFilename))
                    {
                        CopyStream(path.Stream, file);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
            try
            {
                var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Images/SpainFlag.jpg");
                var path = Application.GetResourceStream(ur);
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_SLIDESHOW_FOLDER, "00000000000000000000000000000010.jpg");

                if (!File.Exists(destinationFilename))
                {
                    using (Stream file = File.OpenWrite(destinationFilename))
                    {
                        CopyStream(path.Stream, file);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
        }

        private static void loadServerFilesForRDNation()
        {
            try
            {
                var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Server/ServerFiles/announcer.html");
                var path = Application.GetResourceStream(ur);
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_SERVER_FILES_FOLDER, "announcer.html");
                if (File.Exists(destinationFilename))
                    File.Delete(destinationFilename);
                using (Stream file = File.OpenWrite(destinationFilename))
                { CopyStream(path.Stream, file); }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
            try
            {
                var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Server/ServerFiles/index.html");
                var path = Application.GetResourceStream(ur);
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_SERVER_FILES_FOLDER, "index.html");
                if (File.Exists(destinationFilename))
                    File.Delete(destinationFilename);
                using (Stream file = File.OpenWrite(destinationFilename))
                { CopyStream(path.Stream, file); }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
            try
            {
                var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Server/ServerFiles/mobile.html");
                var path = Application.GetResourceStream(ur);
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_SERVER_FILES_FOLDER, "mobile.html");
                if (File.Exists(destinationFilename))
                    File.Delete(destinationFilename);
                using (Stream file = File.OpenWrite(destinationFilename))
                { CopyStream(path.Stream, file); }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
            try
            {
                var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Server/ServerFiles/control.html");
                var path = Application.GetResourceStream(ur);
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_SERVER_FILES_FOLDER, "control.html");
                if (File.Exists(destinationFilename))
                    File.Delete(destinationFilename);
                using (Stream file = File.OpenWrite(destinationFilename))
                { CopyStream(path.Stream, file); }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
            try
            {
                var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Server/ServerFiles/penaltyboth.html");
                var path = Application.GetResourceStream(ur);
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_SERVER_FILES_FOLDER, "penaltyboth.html");
                if (File.Exists(destinationFilename))
                    File.Delete(destinationFilename);
                using (Stream file = File.OpenWrite(destinationFilename))
                { CopyStream(path.Stream, file); }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
            try
            {
                var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Server/ServerFiles/ab.html");
                var path = Application.GetResourceStream(ur);
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_SERVER_FILES_FOLDER, "ab.html");
                if (File.Exists(destinationFilename))
                    File.Delete(destinationFilename);
                using (Stream file = File.OpenWrite(destinationFilename))
                { CopyStream(path.Stream, file); }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
            try
            {
                var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Server/ServerFiles/overlay.html");
                var path = Application.GetResourceStream(ur);
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_SERVER_FILES_FOLDER, "overlay.html");
                if (File.Exists(destinationFilename))
                    File.Delete(destinationFilename);
                using (Stream file = File.OpenWrite(destinationFilename))
                { CopyStream(path.Stream, file); }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }


            try
            {
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_SERVER_FILES_FOLDER, "favicon.ico");
                if (!File.Exists(destinationFilename))
                {
                    var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Server/ServerFiles/favicon.ico");
                    var path = Application.GetResourceStream(ur);

                    using (Stream file = File.OpenWrite(destinationFilename))
                    { CopyStream(path.Stream, file); }
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
            try
            {
                var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Server/ServerFiles/css/main.css");
                var path = Application.GetResourceStream(ur);
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_SERVER_FILES_FOLDER_CSS, "main.css");
                if (File.Exists(destinationFilename))
                    File.Delete(destinationFilename);
                using (Stream file = File.OpenWrite(destinationFilename))
                { CopyStream(path.Stream, file); }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
            try
            { //LOGO for overlay
                var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Server/ServerFiles/css/logo.css");
                var path = Application.GetResourceStream(ur);
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_SERVER_FILES_FOLDER_CSS, "logo.css");
                if (File.Exists(destinationFilename))
                    File.Delete(destinationFilename);
                using (Stream file = File.OpenWrite(destinationFilename))
                { CopyStream(path.Stream, file); }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }

            try
            {
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_SERVER_FILES_FOLDER_IMG, "Rollerball_pink_s100.png");
                if (!File.Exists(destinationFilename))
                {
                    var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Images/Rollerball_pink_s100.png");
                    var path = Application.GetResourceStream(ur);
                    using (Stream file = File.OpenWrite(destinationFilename))
                    { CopyStream(path.Stream, file); }
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
            try
            {
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_SERVER_FILES_FOLDER_IMG, "ajax-loader.gif");
                if (!File.Exists(destinationFilename))
                {
                    var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Server/ServerFiles/img/ajax-loader.gif");
                    var path = Application.GetResourceStream(ur);
                    using (Stream file = File.OpenWrite(destinationFilename))
                    { CopyStream(path.Stream, file); }
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
            try
            {
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_SERVER_FILES_FOLDER_IMG, "icons-18-black.png");
                if (!File.Exists(destinationFilename))
                {
                    var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Server/ServerFiles/img/icons-18-black.png");
                    var path = Application.GetResourceStream(ur);
                    using (Stream file = File.OpenWrite(destinationFilename))
                    { CopyStream(path.Stream, file); }
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            } try
            {
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_SERVER_FILES_FOLDER_IMG, "icons-18-white.png");
                if (!File.Exists(destinationFilename))
                {
                    var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Server/ServerFiles/img/icons-18-white.png");
                    var path = Application.GetResourceStream(ur);
                    using (Stream file = File.OpenWrite(destinationFilename))
                    { CopyStream(path.Stream, file); }
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            } try
            {
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_SERVER_FILES_FOLDER_IMG, "icons-36-black.png");
                if (!File.Exists(destinationFilename))
                {
                    var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Server/ServerFiles/img/icons-36-black.png");
                    var path = Application.GetResourceStream(ur);
                    using (Stream file = File.OpenWrite(destinationFilename))
                    { CopyStream(path.Stream, file); }
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            } try
            {
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_SERVER_FILES_FOLDER_IMG, "icons-36-white.png");
                if (!File.Exists(destinationFilename))
                {
                    var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Server/ServerFiles/img/icons-36-white.png");
                    var path = Application.GetResourceStream(ur);
                    using (Stream file = File.OpenWrite(destinationFilename))
                    { CopyStream(path.Stream, file); }
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
            try
            {
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_SERVER_FILES_FOLDER_JS, "jquery-1.7.2.min.js");
                if (!File.Exists(destinationFilename))
                {
                    var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Server/ServerFiles/js/jquery-1.7.2.min.js");
                    var path = Application.GetResourceStream(ur);
                    using (Stream file = File.OpenWrite(destinationFilename))
                    { CopyStream(path.Stream, file); }
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
            try
            {
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_SERVER_FILES_FOLDER_JS, "jquery.mobile-1.3.2.min.js");
                if (!File.Exists(destinationFilename))
                {
                    var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Server/ServerFiles/js/jquery.mobile-1.3.2.min.js");
                    var path = Application.GetResourceStream(ur);
                    using (Stream file = File.OpenWrite(destinationFilename))
                    { CopyStream(path.Stream, file); }
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }

            try
            {
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_SERVER_FILES_FOLDER_JS, "jquery.easydate.js");
                if (!File.Exists(destinationFilename))
                {
                    var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Server/ServerFiles/js/jquery.easydate.js");
                    var path = Application.GetResourceStream(ur);
                    using (Stream file = File.OpenWrite(destinationFilename))
                    { CopyStream(path.Stream, file); }
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
            try
            {
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_SERVER_FILES_FOLDER_JS, "jquery.dataTables.naturalSort.js");
                if (!File.Exists(destinationFilename))
                {
                    var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Server/ServerFiles/js/jquery.dataTables.naturalSort.js");
                    var path = Application.GetResourceStream(ur);
                    using (Stream file = File.OpenWrite(destinationFilename))
                    { CopyStream(path.Stream, file); }
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
            try
            {
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_SERVER_FILES_FOLDER_JS, "jquery.dataTables.min.js");
                if (!File.Exists(destinationFilename))
                {
                    var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Server/ServerFiles/js/jquery.dataTables.min.js");
                    var path = Application.GetResourceStream(ur);
                    using (Stream file = File.OpenWrite(destinationFilename))
                    { CopyStream(path.Stream, file); }
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
            try
            {
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_SERVER_FILES_FOLDER_JS, "jquery.timer.js");
                if (!File.Exists(destinationFilename))
                {
                    var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Server/ServerFiles/js/jquery.timer.js");
                    var path = Application.GetResourceStream(ur);
                    using (Stream file = File.OpenWrite(destinationFilename))
                    { CopyStream(path.Stream, file); }
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
            try
            {
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_SERVER_FILES_FOLDER_JS, "knockout-2.0.0.js");
                if (!File.Exists(destinationFilename))
                {
                    var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Server/ServerFiles/js/knockout-2.0.0.js");
                    var path = Application.GetResourceStream(ur);
                    using (Stream file = File.OpenWrite(destinationFilename))
                    { CopyStream(path.Stream, file); }
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
            try
            {
                var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Server/ServerFiles/js/main.js");
                var path = Application.GetResourceStream(ur);
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_SERVER_FILES_FOLDER_JS, "main.js");
                if (File.Exists(destinationFilename))
                    File.Delete(destinationFilename);
                using (Stream file = File.OpenWrite(destinationFilename))
                { CopyStream(path.Stream, file); }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
            try
            {
                var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Server/ServerFiles/js/overlay.js");
                var path = Application.GetResourceStream(ur);
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_SERVER_FILES_FOLDER_JS, "overlay.js");
                if (File.Exists(destinationFilename))
                    File.Delete(destinationFilename);
                using (Stream file = File.OpenWrite(destinationFilename))
                { CopyStream(path.Stream, file); }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }

            try
            {
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_SERVER_FILES_FOLDER_JS, "windowfunctions.js");
                if (!File.Exists(destinationFilename))
                {
                    var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Server/ServerFiles/js/windowfunctions.js");
                    var path = Application.GetResourceStream(ur);
                    using (Stream file = File.OpenWrite(destinationFilename))
                    { CopyStream(path.Stream, file); }
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
            try
            {
                var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Server/ServerFiles/js/inputStats.js");
                var path = Application.GetResourceStream(ur);
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_SERVER_FILES_FOLDER_JS, "inputStats.js");
                if (File.Exists(destinationFilename))
                    File.Delete(destinationFilename);
                using (Stream file = File.OpenWrite(destinationFilename))
                { CopyStream(path.Stream, file); }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }

            try
            {
                var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Server/ServerFiles/assist.html");
                var path = Application.GetResourceStream(ur);
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_SERVER_FILES_FOLDER, "assist.html");
                if (File.Exists(destinationFilename))
                    File.Delete(destinationFilename);
                using (Stream file = File.OpenWrite(destinationFilename))
                { CopyStream(path.Stream, file); }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
            try
            {
                var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Server/ServerFiles/block.html");
                var path = Application.GetResourceStream(ur);
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_SERVER_FILES_FOLDER, "block.html");
                if (File.Exists(destinationFilename))
                    File.Delete(destinationFilename);
                using (Stream file = File.OpenWrite(destinationFilename))
                { CopyStream(path.Stream, file); }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
            try
            {
                var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Server/ServerFiles/lineup.html");
                var path = Application.GetResourceStream(ur);
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_SERVER_FILES_FOLDER, "lineup.html");
                if (File.Exists(destinationFilename))
                    File.Delete(destinationFilename);
                using (Stream file = File.OpenWrite(destinationFilename))
                { CopyStream(path.Stream, file); }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
            try
            {
                var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Server/ServerFiles/score.html");
                var path = Application.GetResourceStream(ur);
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_SERVER_FILES_FOLDER, "score.html");
                if (File.Exists(destinationFilename))
                    File.Delete(destinationFilename);
                using (Stream file = File.OpenWrite(destinationFilename))
                { CopyStream(path.Stream, file); }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
            try
            {
                var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Server/ServerFiles/penalty.html");
                var path = Application.GetResourceStream(ur);
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_SERVER_FILES_FOLDER, "penalty.html");
                if (File.Exists(destinationFilename))
                    File.Delete(destinationFilename);
                using (Stream file = File.OpenWrite(destinationFilename))
                { CopyStream(path.Stream, file); }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
            //Test
            try
            {
                var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Server/ServerFiles/wg.html");
                var path = Application.GetResourceStream(ur);
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_SERVER_FILES_FOLDER, "wg.html");
                if (File.Exists(destinationFilename))
                    File.Delete(destinationFilename);
                using (Stream file = File.OpenWrite(destinationFilename))
                { CopyStream(path.Stream, file); }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
            try
            {
                var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Server/ServerFiles/img/wftdaoverlay/wftda_overlay_main.png");
                var path = Application.GetResourceStream(ur);
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_SERVER_FILES_FOLDER_IMG_WFT, "wftda_overlay_main.png");
                if (File.Exists(destinationFilename))
                    File.Delete(destinationFilename);
                using (Stream file = File.OpenWrite(destinationFilename))
                { CopyStream(path.Stream, file); }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
            try
            {
                var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Server/ServerFiles/img/wftdaoverlay/wftda_overlay_t1_lead.png");
                var path = Application.GetResourceStream(ur);
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_SERVER_FILES_FOLDER_IMG_WFT, "wftda_overlay_t1_lead.png");
                if (File.Exists(destinationFilename))
                    File.Delete(destinationFilename);
                using (Stream file = File.OpenWrite(destinationFilename))
                { CopyStream(path.Stream, file); }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
            try
            {
                var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Server/ServerFiles/img/wftdaoverlay/wftda_overlay_t1_or.png");
                var path = Application.GetResourceStream(ur);
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_SERVER_FILES_FOLDER_IMG_WFT, "wftda_overlay_t1_or.png");
                if (File.Exists(destinationFilename))
                    File.Delete(destinationFilename);
                using (Stream file = File.OpenWrite(destinationFilename))
                { CopyStream(path.Stream, file); }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
            try
            {
                var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Server/ServerFiles/img/wftdaoverlay/wftda_overlay_t1_to1.png");
                var path = Application.GetResourceStream(ur);
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_SERVER_FILES_FOLDER_IMG_WFT, "wftda_overlay_t1_to1.png");
                if (File.Exists(destinationFilename))
                    File.Delete(destinationFilename);
                using (Stream file = File.OpenWrite(destinationFilename))
                { CopyStream(path.Stream, file); }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
            try
            {
                var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Server/ServerFiles/img/wftdaoverlay/wftda_overlay_t1_to2.png");
                var path = Application.GetResourceStream(ur);
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_SERVER_FILES_FOLDER_IMG_WFT, "wftda_overlay_t1_to2.png");
                if (File.Exists(destinationFilename))
                    File.Delete(destinationFilename);
                using (Stream file = File.OpenWrite(destinationFilename))
                { CopyStream(path.Stream, file); }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
            try
            {
                var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Server/ServerFiles/img/wftdaoverlay/wftda_overlay_t1_to3.png");
                var path = Application.GetResourceStream(ur);
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_SERVER_FILES_FOLDER_IMG_WFT, "wftda_overlay_t1_to3.png");
                if (File.Exists(destinationFilename))
                    File.Delete(destinationFilename);
                using (Stream file = File.OpenWrite(destinationFilename))
                { CopyStream(path.Stream, file); }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
            try
            {
                var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Server/ServerFiles/img/wftdaoverlay/wftda_overlay_t2_lead.png");
                var path = Application.GetResourceStream(ur);
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_SERVER_FILES_FOLDER_IMG_WFT, "wftda_overlay_t2_lead.png");
                if (File.Exists(destinationFilename))
                    File.Delete(destinationFilename);
                using (Stream file = File.OpenWrite(destinationFilename))
                { CopyStream(path.Stream, file); }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
            try
            {
                var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Server/ServerFiles/img/wftdaoverlay/wftda_overlay_t2_or.png");
                var path = Application.GetResourceStream(ur);
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_SERVER_FILES_FOLDER_IMG_WFT, "wftda_overlay_t2_or.png");
                if (File.Exists(destinationFilename))
                    File.Delete(destinationFilename);
                using (Stream file = File.OpenWrite(destinationFilename))
                { CopyStream(path.Stream, file); }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
            try
            {
                var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Server/ServerFiles/img/wftdaoverlay/wftda_overlay_t2_to1.png");
                var path = Application.GetResourceStream(ur);
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_SERVER_FILES_FOLDER_IMG_WFT, "wftda_overlay_t2_to1.png");
                if (File.Exists(destinationFilename))
                    File.Delete(destinationFilename);
                using (Stream file = File.OpenWrite(destinationFilename))
                { CopyStream(path.Stream, file); }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
            try
            {
                var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Server/ServerFiles/img/wftdaoverlay/wftda_overlay_t2_to2.png");
                var path = Application.GetResourceStream(ur);
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_SERVER_FILES_FOLDER_IMG_WFT, "wftda_overlay_t2_to2.png");
                if (File.Exists(destinationFilename))
                    File.Delete(destinationFilename);
                using (Stream file = File.OpenWrite(destinationFilename))
                { CopyStream(path.Stream, file); }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
            try
            {
                var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Server/ServerFiles/img/wftdaoverlay/wftda_overlay_t2_to3.png");
                var path = Application.GetResourceStream(ur);
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_SERVER_FILES_FOLDER_IMG_WFT, "wftda_overlay_t2_to3.png");
                if (File.Exists(destinationFilename))
                    File.Delete(destinationFilename);
                using (Stream file = File.OpenWrite(destinationFilename))
                { CopyStream(path.Stream, file); }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
            try
            {
                var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Server/ServerFiles/css/wg.css");
                var path = Application.GetResourceStream(ur);
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_SERVER_FILES_FOLDER_CSS, "wg.css");
                if (File.Exists(destinationFilename))
                    File.Delete(destinationFilename);
                using (Stream file = File.OpenWrite(destinationFilename))
                { CopyStream(path.Stream, file); }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }

            try
            {
                var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Server/ServerFiles/js/scoreboard.js");
                var path = Application.GetResourceStream(ur);
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_SERVER_FILES_FOLDER_JS, "scoreboard.js");
                if (File.Exists(destinationFilename))
                    File.Delete(destinationFilename);
                using (Stream file = File.OpenWrite(destinationFilename))
                { CopyStream(path.Stream, file); }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }



            //

            //deletes an old CSS file we don't use anymore.
            if (File.Exists(ScoreboardConfig.SAVE_SERVER_FILES_FOLDER_CSS + "overlay.css"))
                File.Delete(ScoreboardConfig.SAVE_SERVER_FILES_FOLDER_CSS + "overlay.css");

        }

        /// <summary>
        /// loads the advert for RDNation.
        /// </summary>
        private static void loadAdvertisementForRDNation()
        {
            try
            {
                var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Images/RDNation.png");
                var path = Application.GetResourceStream(ur);
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_ADVERTS_FOLDER, "00000000000000000000000000000000.png");

                //if (!File.Exists(destinationFilename))
                //{
                using (Stream file = File.OpenWrite(destinationFilename))
                {
                    CopyStream(path.Stream, file);
                }
                //}
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
            try
            {
                var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Images/RollinNews.png");
                var path = Application.GetResourceStream(ur);
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_ADVERTS_FOLDER, "00000000000000000000000000000001.png");

                if (!File.Exists(destinationFilename))
                {
                    using (Stream file = File.OpenWrite(destinationFilename))
                    {
                        CopyStream(path.Stream, file);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }

            try
            {
                var ur = new Uri("pack://application:,,,/RDNScoreboard;component/Images/RDNationLarge.png");
                var path = Application.GetResourceStream(ur);
                string destinationFilename = System.IO.Path.Combine(ScoreboardConfig.SAVE_SLIDESHOW_FOLDER, "00000000000000000000000000000001.png");
                if (File.Exists(destinationFilename))
                    File.Delete(destinationFilename);
                if (!File.Exists(destinationFilename))
                {
                    using (Stream file = File.OpenWrite(destinationFilename))
                    {
                        CopyStream(path.Stream, file);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
        }

        void AppDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // In debug mode do not custom-handle the exception, let Visual Studio handle it e.Handled = false;
            //want to save and send the error out before the project crashes.
            ErrorViewModel.Save(e.Exception, e.GetType(), ErrorGroupEnum.Threading, additionalInformation: Logger.Instance.getLoggedMessages());
            ErrorViewModel.checkForOldErrors();
            ShowUnhandeledException(e);
        }

        void ShowUnhandeledException(DispatcherUnhandledExceptionEventArgs e)
        {
            try { 
            e.Handled = true;
            string errorMessage = string.Empty;
            if (e.Exception.Message.Contains("System.Windows.Markup.StaticExtension"))
                errorMessage = "Please Contact info@rdnation.com if the problem persists.  \n\n\nAn application error occurred.\n\n\n We have seen this error before.  If running WinXP, please choose the DEFAULT of CLASSIC theme set.  That should fix this error.  \n\n\nDo you want to continue?\n(if you click Yes you will continue with your work, if you click No the application will close)";
            else if (e.Exception.Message.Contains("Cannot have nested BeginInit calls on the same instance"))
                errorMessage = "Please Contact info@rdnation.com if the problem persists.  \n\n\nAn application error occurred.\n\n\n We have seen this error before.  Please run application in full admin mode.  There seems to be a slight problem with permissions.  \n\n\nDo you want to continue?\n(if you click Yes you will continue with your work, if you click No the application will close)";
            else
                errorMessage = string.Format("Please Contact info@rdnation.com if the problem persists.  \n\n\nAn application error occurred.\n\n\nError:{0}\n\nDo you want to continue?\n(if you click Yes you will continue with your work, if you click No the application will close)",
                 e.Exception.Message + (e.Exception.InnerException != null ? "\n" +
                 e.Exception.InnerException.Message : null));

            if (MessageBox.Show(errorMessage, "Application Error", MessageBoxButton.YesNoCancel, MessageBoxImage.Error) == MessageBoxResult.No)
            {
                if (MessageBox.Show("WARNING: The application will close. Any changes will not be saved!\nDo you really want to close it?", "Close the application!", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    Application.Current.Shutdown();
                }
            }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
        }
    }
}
