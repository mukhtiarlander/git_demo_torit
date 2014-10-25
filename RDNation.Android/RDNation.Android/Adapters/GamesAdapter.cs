using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using RDN.Utilities.Util;
using RDNation.Droid.Classes;
using RDNation.Droid.Classes.Images;
using RDN.Portable.Models.Json.Games;
using RDN.Portable.Config.Enums;

namespace RDNation.Droid.Adapters
{
    public class GamesAdapter : BaseAdapter<CurrentGameJson>
    {

        private class ViewHolder : Java.Lang.Object
        {
            public TextView publicGameRowHeaderPeriodJam;
            public TextView publicGameRowTeamOneName;
            public TextView publicGameRowTeamOneScore;
            public TextView publicGameRowTeamTwoName;
            public TextView publicGameRowTeamTwoScore;
            public ImageView publicGameRowTeamOneLogo;
            public ImageView publicGameRowTeamTwoLogo;
            public ImageView publicGameRowTeamOneWinner;
            public ImageView publicGameRowTeamTwoWinner;
        }
        List<CurrentGameJson> items;
        Activity context;
        List<Image> n_bitmapCache;
        Action MoreCallback;
        public GamesAdapter(Activity context, List<CurrentGameJson> items, Action getMoreCallback)
            : base()
        {
            this.context = context;
            this.items = items;
            this.MoreCallback = getMoreCallback;
            n_bitmapCache = new List<Image>();
            LoggerMobile.Instance.logMessage("Opening GamesAdapter", LoggerEnum.message);
        }


        public void AddItems(List<CurrentGameJson> items)
        {
            this.items.AddRange(items);
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override CurrentGameJson this[int position]
        {
            get { return items[position]; }
        }
        public override int Count
        {
            get { return items.Count(); }
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            //loads more skaters when 10 are left to view.
            if (position == (Count - 5) && MoreCallback != null)
                this.MoreCallback();

            var item = items[position];

            ViewHolder holder = null;
            try
            {
                if (convertView == null) // no view to re-use, create new
                {
                    convertView = context.LayoutInflater.Inflate(Resource.Layout.PublicGameRow, null);
                    holder = new ViewHolder();
                    holder.publicGameRowHeaderPeriodJam = convertView.FindViewById<TextView>(Resource.Id.publicGameRowHeaderPeriodJam);
                    holder.publicGameRowTeamOneName = convertView.FindViewById<TextView>(Resource.Id.publicGameRowTeamOneName);
                    holder.publicGameRowTeamOneScore = convertView.FindViewById<TextView>(Resource.Id.publicGameRowTeamOneScore);
                    holder.publicGameRowTeamTwoName = convertView.FindViewById<TextView>(Resource.Id.publicGameRowTeamTwoName);
                    holder.publicGameRowTeamTwoScore = convertView.FindViewById<TextView>(Resource.Id.publicGameRowTeamTwoScore);
                    holder.publicGameRowTeamOneLogo = convertView.FindViewById<ImageView>(Resource.Id.publicGameRowTeamOneLogo);
                    holder.publicGameRowTeamTwoLogo = convertView.FindViewById<ImageView>(Resource.Id.publicGameRowTeamTwoLogo);
                    holder.publicGameRowTeamOneWinner = convertView.FindViewById<ImageView>(Resource.Id.publicGameRowTeamOneWinner);
                    holder.publicGameRowTeamTwoWinner = convertView.FindViewById<ImageView>(Resource.Id.publicGameRowTeamTwoWinner);
                    convertView.Tag = holder;
                }
                if (holder == null)
                    holder = (ViewHolder)convertView.Tag;
                if (item.HasGameEnded)
                    holder.publicGameRowHeaderPeriodJam.Text = item.StartTime.Day + "/" + item.StartTime.Month + ": Final";
                else
                {
                    string time = string.Empty;
                    var tsPeriod = TimeSpan.FromMilliseconds(item.PeriodTimeLeft);
                    string answerPeriod = string.Format("{0:D2}:{1:D2}", tsPeriod.Minutes, tsPeriod.Seconds);
                    var tsJam = TimeSpan.FromMilliseconds(item.JamTimeLeft);
                    string answerJam = string.Format("{0:D2}:{1:D2}", tsJam.Minutes, tsJam.Seconds);
                    holder.publicGameRowHeaderPeriodJam.Text = "P" + item.PeriodNumber + " " + answerPeriod + ": J" + item.JamNumber + " " + answerJam;
                }

                holder.publicGameRowTeamOneName.Text = item.Team1Name;
                holder.publicGameRowTeamOneScore.Text = item.Team1Score.ToString();
                holder.publicGameRowTeamTwoName.Text = item.Team2Name;
                holder.publicGameRowTeamTwoScore.Text = item.Team2Score.ToString();

                if (item.Team1Score > item.Team2Score)
                {
                    holder.publicGameRowTeamOneWinner.SetImageResource(Resource.Drawable.ic_action_left);
                    holder.publicGameRowTeamTwoWinner.SetImageBitmap(null);
                }
                else if (item.Team2Score > item.Team1Score)
                {
                    holder.publicGameRowTeamOneWinner.SetImageBitmap(null);
                    holder.publicGameRowTeamTwoWinner.SetImageResource(Resource.Drawable.ic_action_left);
                }
                holder.publicGameRowTeamOneLogo.SetImageBitmap(null);
                holder.publicGameRowTeamTwoLogo.SetImageBitmap(null);

                if (!String.IsNullOrEmpty(item.Team1LogoUrl))
                {
                    var bit = n_bitmapCache.Where(x => x.Id == item.Team1LogoUrl).FirstOrDefault();
                    if (bit == null)
                    {
                        Task<bool>.Factory.StartNew(
                                       () =>
                                       {
                                           try
                                           {
                                               LoggerMobile.Instance.logMessage("downloading1: " + item.Team1LogoUrl, LoggerEnum.message);
                                               var i = Image.GetImageBitmapFromUrl(item.Team1LogoUrl);
                                               Image img = new Image();
                                               img.Id = item.Team1LogoUrl;
                                               img.Img = i;
                                               if (i != null)
                                               {
                                                   
                                                   context.RunOnUiThread(() =>
                                                   {
                                                       holder.publicGameRowTeamOneLogo.SetImageBitmap(i);
                                                   });
                                                   n_bitmapCache.Add(img);
                                               }
                                           }
                                           catch (Exception exception)
                                           {
                                               ErrorHandler.Save(exception, MobileTypeEnum.Android, context);
                                           }
                                           return true;
                                       });
                    }
                    else
                        holder.publicGameRowTeamOneLogo.SetImageBitmap(bit.Img);
                }
                if (!String.IsNullOrEmpty(item.Team2LogoUrl))
                {
                    var bit = n_bitmapCache.Where(x => x.Id == item.Team2LogoUrl).FirstOrDefault();
                    if (bit == null)
                    {
                        Task<bool>.Factory.StartNew(
                                       () =>
                                       {
                                           try
                                           {
                                               LoggerMobile.Instance.logMessage("downloading2: " + item.Team2LogoUrl, LoggerEnum.message);
                                               var i = Image.GetImageBitmapFromUrl(item.Team2LogoUrl);

                                               Image img = new Image();
                                               img.Id = item.Team2LogoUrl;
                                               img.Img = i;
                                               if (i != null)
                                               {
                                                   
                                                   context.RunOnUiThread(() =>
                                                   {
                                                       holder.publicGameRowTeamTwoLogo.SetImageBitmap(i);
                                                   });
                                                   n_bitmapCache.Add(img);
                                               }
                                           }
                                           catch (Exception exception)
                                           {
                                               ErrorHandler.Save(exception, MobileTypeEnum.Android, context);
                                           }
                                           return true;
                                       });
                    }
                    else
                        holder.publicGameRowTeamTwoLogo.SetImageBitmap(bit.Img);
                }



            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.Android, context);
            }
            return convertView;
        }
    }
}