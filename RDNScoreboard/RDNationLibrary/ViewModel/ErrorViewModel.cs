using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDNationLibrary.Static;
using System.Threading.Tasks;
using System.IO;
using System.Net;   

namespace RDNationLibrary.ViewModel
{
  public   class ErrorViewModel
    {

      public static void saveError(Exception e)
      {
 Task<bool>.Factory.StartNew(
               () =>
               {
                   System.Xml.Serialization.XmlSerializer writer =
                      new System.Xml.Serialization.XmlSerializer(e.GetType());
                   System.IO.StreamWriter file = new System.IO.StreamWriter( Config.SAVE_ERRORS_FOLDER);

                   writer.Serialize(file, e);
                   file.Close();

                   return true;
               }).Start();

      }
      private static void uploadErrorToServer()
      { }
      public static void checkForOldErrors()
      {
          DirectoryInfo dir = new DirectoryInfo(Config.SAVE_ERRORS_FOLDER);
         FileInfo[] files= dir.GetFiles();
         foreach (var file in files)
         {
            // WebClient client = new WebClient();
             //client.UploadFileAsync()
         }
      }
    }
}
