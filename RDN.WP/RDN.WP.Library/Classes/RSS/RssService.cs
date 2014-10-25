using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace RDN.WP.Library.Classes.RSS
{
    public class RssService
    {
        public static Task<List<SyndicationItemModel>> Execute(string link)
        {

            WebClient wc = new WebClient();
            TaskCompletionSource<List<SyndicationItemModel>> tcs = new TaskCompletionSource<List<SyndicationItemModel>>();

            wc.DownloadStringCompleted += (s, e) =>
            {
                try
                {
                    if (e.Error == null)
                    {
                        StringReader stringReader = new StringReader(e.Result);
                        XmlReader xmlReader = XmlReader.Create(stringReader);
                        SyndicationFeed feed = SyndicationFeed.Load(xmlReader);
                        List<SyndicationItemModel> list = new List<SyndicationItemModel>();

                        foreach (var item in feed.Items)
                        {
                            SyndicationItemModel l = new SyndicationItemModel();
                            l.Title = item.Title.Text;
                            l.Url = item.Links.FirstOrDefault().Uri.ToString();
                            foreach (SyndicationElementExtension extension in item.ElementExtensions)
                            {

                                if (extension.OuterName == "frontimage")
                                {
                                    XElement ele = extension.GetObject<XElement>();
                                    l.InitialImage = ele.Value;
                                }
                                else if (extension.OuterName == "articleimage")
                                {
                                    XElement ele = extension.GetObject<XElement>();
                                    l.MainImage = ele.Value;
                                }
                            }
                            list.Add(l);
                        }

                        tcs.SetResult(list);
                    }
                    else
                    {
                        tcs.SetResult(new List<SyndicationItemModel>());
                    }
                }
                catch
                {

                }
            };
            wc.DownloadStringAsync(new Uri(link, UriKind.Absolute));
            return tcs.Task;
        }
    }
}
