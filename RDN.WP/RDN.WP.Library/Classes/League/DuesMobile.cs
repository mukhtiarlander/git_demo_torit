using RDN.Portable.Classes.Account.Classes;
using RDN.Portable.Classes.Controls.Dues;
using RDN.Portable.Classes.Payment.Classes;
using RDN.Portable.Classes.Utilities;
using RDN.Portable.Classes.Voting;
using RDN.Portable.Config;
using RDN.Portable.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDN.WP.Library.Classes.League
{
    public class DuesMobile
    {
        public static DuesPortableModel GetDuesManagement(Guid memId, Guid uid)
        {
            Random r = new Random();
            var response = Network.SendPackage(Network.ConvertObjectToStream(new DuesSendParams() { CurrentMemberId = memId, UserId = uid }), MobileConfig.LEAGUE_DUES_MANAGE_URL+"?r="+ r.Next());
            var stream = response.GetResponseStream();
            StreamReader read = new StreamReader(stream);
            string json = read.ReadToEnd();
            return Json.DeserializeObject<DuesPortableModel>(json);
        }
        public static DuesPortableModel GetDuesItem(Guid memId, Guid uid, Guid duesId, long duesItemId)
        {
            Random r = new Random();
            var response = Network.SendPackage(Network.ConvertObjectToStream(new DuesSendParams() { DuesId = duesId, DuesItemId = duesItemId, CurrentMemberId = memId, UserId = uid }), MobileConfig.LEAGUE_DUES_ITEM_URL + "?r=" + r.Next());
            var stream = response.GetResponseStream();
            StreamReader read = new StreamReader(stream);
            string json = read.ReadToEnd();
            return Json.DeserializeObject<DuesPortableModel>(json);
        }
        public static DuesSendParams RemoveDuesPayment(Guid memId, Guid uid, Guid duesId, long duesItemId, long duesCollectedId, Guid toMemberId)
        {
            var response = Network.SendPackage(Network.ConvertObjectToStream(new DuesSendParams() {MemberId = toMemberId, DuesCollectedId = duesCollectedId, DuesId = duesId, DuesItemId = duesItemId, CurrentMemberId = memId, UserId = uid }), MobileConfig.LEAGUE_DUES_ITEM_REMOVE_URL);
            var stream = response.GetResponseStream();
            StreamReader read = new StreamReader(stream);
            string json = read.ReadToEnd();
            return Json.DeserializeObject<DuesSendParams>(json);
        }
        public static DuesSendParams SendEmailReminderWithstanding(Guid memId, Guid uid, Guid duesId, Guid toMemberId)
        {
            var response = Network.SendPackage(Network.ConvertObjectToStream(new DuesSendParams() { DuesId = duesId, MemberId = toMemberId, CurrentMemberId = memId, UserId = uid }), MobileConfig.LEAGUE_DUES_SEND_EMAIL_REMINDER_WITHSTANDING_URL);
            var stream = response.GetResponseStream();
            StreamReader read = new StreamReader(stream);
            string json = read.ReadToEnd();
            return Json.DeserializeObject<DuesSendParams>(json);
        }
        public static DuesSendParams SendEmailReminder(Guid memId, Guid uid, Guid duesId, long duesItemId, Guid toMemberId)
        {
            var response = Network.SendPackage(Network.ConvertObjectToStream(new DuesSendParams() { MemberId = toMemberId, DuesId = duesId, DuesItemId = duesItemId, CurrentMemberId = memId, UserId = uid }), MobileConfig.LEAGUE_DUES_SEND_EMAIL_REMINDER_URL);
            var stream = response.GetResponseStream();
            StreamReader read = new StreamReader(stream);
            string json = read.ReadToEnd();
            return Json.DeserializeObject<DuesSendParams>(json);
        }
        public static DuesSendParams SendEmailReminderAll(Guid memId, Guid uid, Guid duesId, long duesItemId)
        {
            var response = Network.SendPackage(Network.ConvertObjectToStream(new DuesSendParams() { DuesId = duesId, DuesItemId = duesItemId, CurrentMemberId = memId, UserId = uid }), MobileConfig.LEAGUE_DUES_SEND_EMAIL_REMINDER_ALL_URL);
            var stream = response.GetResponseStream();
            StreamReader read = new StreamReader(stream);
            string json = read.ReadToEnd();
            return Json.DeserializeObject<DuesSendParams>(json);
        }
        public static DuesSendParams PayDuesAmount(Guid memId, Guid uid, Guid duesId, long duesItemId, double amount, string note, Guid toMemberId)
        {
            var response = Network.SendPackage(Network.ConvertObjectToStream(new DuesSendParams() { Amount = amount, Note = note, MemberId = toMemberId, DuesId = duesId, DuesItemId = duesItemId, CurrentMemberId = memId, UserId = uid }), MobileConfig.LEAGUE_DUES_PAY_URL);
            var stream = response.GetResponseStream();
            StreamReader read = new StreamReader(stream);
            string json = read.ReadToEnd();
            return Json.DeserializeObject<DuesSendParams>(json);
        }
        public static DuesSendParams SetDuesAmount(Guid memId, Guid uid, Guid duesId, long duesItemId, double amount, Guid toMemberId)
        {
            var response = Network.SendPackage(Network.ConvertObjectToStream(new DuesSendParams() { Amount = amount, MemberId = toMemberId, DuesId = duesId, DuesItemId = duesItemId, CurrentMemberId = memId, UserId = uid }), MobileConfig.LEAGUE_DUES_SET_AMOUNT_URL);
            var stream = response.GetResponseStream();
            StreamReader read = new StreamReader(stream);
            string json = read.ReadToEnd();
            return Json.DeserializeObject<DuesSendParams>(json);
        }
        public static DuesPortableModel SaveDuesSettings(DuesPortableModel model)
        {
            var response = Network.SendPackage(Network.ConvertObjectToStream(model), MobileConfig.LEAGUE_DUES_SAVE_SETTINGS_URL);
            var stream = response.GetResponseStream();
            StreamReader read = new StreamReader(stream);
            string json = read.ReadToEnd();
            return Json.DeserializeObject<DuesPortableModel>(json);
        }
        public static DuesSendParams WaiveDuesAmount(Guid memId, Guid uid, Guid duesId, long duesItemId, Guid toMemberId, string note)
        {
            var response = Network.SendPackage(Network.ConvertObjectToStream(new DuesSendParams() { Note = note, MemberId = toMemberId, DuesId = duesId, DuesItemId = duesItemId, CurrentMemberId = memId, UserId = uid }), MobileConfig.LEAGUE_DUES_WAIVE_URL);
            var stream = response.GetResponseStream();
            StreamReader read = new StreamReader(stream);
            string json = read.ReadToEnd();
            return Json.DeserializeObject<DuesSendParams>(json);
        }
        public static DuesSendParams WaiveRemoveDuesAmount(Guid memId, Guid uid, Guid duesId, long duesItemId, Guid toMemberId)
        {
            var response = Network.SendPackage(Network.ConvertObjectToStream(new DuesSendParams() { MemberId = toMemberId, DuesId = duesId, DuesItemId = duesItemId, CurrentMemberId = memId, UserId = uid }), MobileConfig.LEAGUE_DUES_REMOVE_WAIVE_URL);
            var stream = response.GetResponseStream();
            StreamReader read = new StreamReader(stream);
            string json = read.ReadToEnd();
            return Json.DeserializeObject<DuesSendParams>(json);
        }
        public static DuesSendParams EditDuesItem(Guid memId, Guid uid, Guid duesId, long duesItemId, double amount, DateTime payBy)
        {
            var response = Network.SendPackage(Network.ConvertObjectToStream(new DuesSendParams() { Amount = amount, PayBy = payBy, DuesId = duesId, DuesItemId = duesItemId, CurrentMemberId = memId, UserId = uid }), MobileConfig.LEAGUE_DUES_EDIT_ITEM_URL);
            var stream = response.GetResponseStream();
            StreamReader read = new StreamReader(stream);
            string json = read.ReadToEnd();
            return Json.DeserializeObject<DuesSendParams>(json);
        }
        public static DuesSendParams DeleteDuesItem(Guid memId, Guid uid, Guid duesId, long duesItemId)
        {
            var response = Network.SendPackage(Network.ConvertObjectToStream(new DuesSendParams() { DuesId = duesId, DuesItemId = duesItemId, CurrentMemberId = memId, UserId = uid }), MobileConfig.LEAGUE_DUES_DELETE_ITEM_URL);
            var stream = response.GetResponseStream();
            StreamReader read = new StreamReader(stream);
            string json = read.ReadToEnd();
            return Json.DeserializeObject<DuesSendParams>(json);
        }
        public static DuesPortableModel DuesForMember(Guid memId, Guid uid, Guid toMemberId)
        {
            var response = Network.SendPackage(Network.ConvertObjectToStream(new DuesSendParams() { MemberId = toMemberId, CurrentMemberId = memId, UserId = uid }), MobileConfig.LEAGUE_DUES_FOR_MEMBER_URL);
            var stream = response.GetResponseStream();
            StreamReader read = new StreamReader(stream);
            string json = read.ReadToEnd();
            return Json.DeserializeObject<DuesPortableModel>(json);
        }
        public static DuesPortableModel GetDuesSettings(Guid memId, Guid uid, Guid duesId)
        {
            var response = Network.SendPackage(Network.ConvertObjectToStream(new DuesSendParams() { DuesId = duesId, CurrentMemberId = memId, UserId = uid }), MobileConfig.LEAGUE_DUES_GET_SETTINGS_URL);
            var stream = response.GetResponseStream();
            StreamReader read = new StreamReader(stream);
            string json = read.ReadToEnd();
            return Json.DeserializeObject<DuesPortableModel>(json);
        }
        public static DuesReceipt GetReceipt(Guid memId, Guid uid, Guid invoiceId)
        {
            var response = Network.SendPackage(Network.ConvertObjectToStream(new DuesSendParams() { DuesId = invoiceId, CurrentMemberId = memId, UserId = uid }), MobileConfig.LEAGUE_DUES_RECEIPT_URL);
            var stream = response.GetResponseStream();
            StreamReader read = new StreamReader(stream);
            string json = read.ReadToEnd();
            return Json.DeserializeObject<DuesReceipt>(json);
        }
        public static DuesSendParams GenerateNewDuesItem(Guid memId, Guid uid, Guid duesId)
        {
            var response = Network.SendPackage(Network.ConvertObjectToStream(new DuesSendParams() { DuesId = duesId, CurrentMemberId = memId, UserId = uid }), MobileConfig.LEAGUE_DUES_GENERATE_NEW_URL);
            var stream = response.GetResponseStream();
            StreamReader read = new StreamReader(stream);
            string json = read.ReadToEnd();
            return Json.DeserializeObject<DuesSendParams>(json);
        }
        public static CreateInvoiceReturn PayDuesPersonally(Guid memId, Guid uid, Guid duesId, long duesItemId)
        {
            var response = Network.SendPackage(Network.ConvertObjectToStream(new DuesSendParams() { DuesItemId = duesItemId, DuesId = duesId, CurrentMemberId = memId, UserId = uid }), MobileConfig.LEAGUE_DUES_PAY_DUES_PERSONALLY_URL);
            var stream = response.GetResponseStream();
            StreamReader read = new StreamReader(stream);
            string json = read.ReadToEnd();
            return Json.DeserializeObject<CreateInvoiceReturn>(json);
        }
        public static DuesMemberItem EditMemberDues(Guid memId, Guid uid, Guid duesId, long duesItemId, Guid memberId)
        {
            var response = Network.SendPackage(Network.ConvertObjectToStream(new DuesSendParams() { DuesItemId = duesItemId, DuesId = duesId, CurrentMemberId = memId, UserId = uid, MemberId = memberId }), MobileConfig.LEAGUE_DUES_EDIT_MEMBER_DUES_URL);
            var stream = response.GetResponseStream();
            StreamReader read = new StreamReader(stream);
            string json = read.ReadToEnd();
            return Json.DeserializeObject<DuesMemberItem>(json);
        }
    }
}
