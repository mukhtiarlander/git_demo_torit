using RDN.Library.Cache;
using RDN.Library.Classes.Error;
using RDN.Portable.Classes.Account.Classes;
using RDN.Portable.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RDN.Api.Controllers.Member
{
    public class MemberController : Controller
    {
        public ActionResult SaveMember()
        {
            try
            {
                if (HttpContext.Request.InputStream != null)
                {
                    Stream stream = HttpContext.Request.InputStream;
                    var ob = Network.LoadObject<MemberPassParams>(ref stream);
                    var edit = MemberCache.GetMemberDisplay(ob.Mid);
                    if (ob.Uid == edit.UserId)
                    {


                        if (!String.IsNullOrEmpty(ob.Member.DerbyName))
                            edit.DerbyName = ob.Member.DerbyName.Trim();
                        if (ob.Member.DOB != null && ob.Member.DOB > DateTime.Now.AddYears(-100))
                            edit.DOB = ob.Member.DOB;
                        else if (ob.Member.DOB == new DateTime())
                            edit.DOB = new DateTime();
                        edit.Email = ob.Member.Email;
                        if (!String.IsNullOrEmpty(ob.Member.Email))
                            edit.Email = ob.Member.Email.Trim();
                        edit.DayJob = ob.Member.DayJob;
                        if (ob.Member.StartedSkating != null && ob.Member.StartedSkating > DateTime.Now.AddYears(-100))
                            edit.StartedSkating = ob.Member.StartedSkating;
                        if (ob.Member.StoppedSkating != null && ob.Member.StoppedSkating > DateTime.Now.AddYears(-100))
                            edit.StoppedSkating = ob.Member.StoppedSkating;

                        edit.InsuranceNumbers = ob.Member.InsuranceNumbers;

                        edit.IsProfileRemovedFromPublicView = ob.Member.IsProfileRemovedFromPublicView;
                        if (!String.IsNullOrEmpty(ob.Member.Firstname))
                            edit.Firstname = ob.Member.Firstname.Trim();
                        else
                            edit.Firstname = ob.Member.Firstname;
                        edit.Gender = ob.Member.Gender;
                        edit.HeightFeet = ob.Member.HeightFeet;
                        edit.HeightInches = ob.Member.HeightInches;
                        edit.LastName = ob.Member.LastName;

                        edit.MemberId = ob.Member.MemberId;
                        edit.PhoneNumber = ob.Member.PhoneNumber;

                        edit.PlayerNumber = ob.Member.PlayerNumber;
                        edit.WeightLbs = ob.Member.WeightLbs;
                        edit.IsRetired = ob.Member.IsRetired;

                        edit.Address = ob.Member.Address;
                        edit.Address2 = ob.Member.Address2;
                        edit.City = ob.Member.City;
                        edit.State = ob.Member.State;
                        edit.ZipCode = ob.Member.ZipCode;
                        edit.CountryId = ob.Member.Country;
                        RDN.Library.Classes.Account.User.UpdateMemberDisplayForMemberMobile(edit);
                        ob.Member.IsSuccessful = true;

                        MemberCache.ClearWebSitesCache(ob.Member.MemberId);
                        MemberCache.Clear(ob.Member.MemberId);
                        return Json(ob.Member, JsonRequestBehavior.AllowGet);

                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Json(new MemberDisplayEdit() { IsSuccessful = false }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult EditMember()
        {
            try
            {
                if (HttpContext.Request.InputStream != null)
                {
                    Stream stream = HttpContext.Request.InputStream;
                    var ob = Network.LoadObject<MemberPassParams>(ref stream);
                    var mem = MemberCache.GetMemberDisplay(ob.Mid);
                    if (ob.Uid == mem.UserId)
                    {

                        MemberDisplayEdit m = new MemberDisplayEdit();
                        m.IsSuccessful = true;
                        m.Countries = ApiCache.GetCountriesInfo();
                        if (mem.ContactCard.Addresses.Count > 0)
                        {
                            foreach (var add in mem.ContactCard.Addresses)
                            {
                                m.Address = add.Address1;
                                m.Address2 = add.Address2;
                                m.City = add.CityRaw;
                                m.State = add.StateRaw;
                                m.ZipCode = add.Zip;
                                if (m.Countries.Where(x => x.Code == add.Country).FirstOrDefault() != null)
                                    m.Country = m.Countries.Where(x => x.Code == add.Country).FirstOrDefault().CountryId;
                            }
                        }

                        m.DayJob = mem.DayJob;
                        m.DerbyName = mem.DerbyName;
                        m.DerbyNameUrl = mem.DerbyNameUrl;
                        m.DOB = mem.DOB;
                        m.Email = mem.Email;
                        m.Firstname = mem.Firstname;
                        m.Gender = mem.Gender;
                        m.HeightFeet = mem.HeightFeet;
                        m.HeightInches = mem.HeightInches;
                        m.InsuranceNumbers = mem.InsuranceNumbers;
                        m.IsProfileRemovedFromPublicView = mem.IsProfileRemovedFromPublicView;
                        m.IsRetired = mem.IsRetired;
                        m.LastName = mem.LastName;
                        m.MemberId = mem.MemberId;
                        m.PhoneNumber = mem.PhoneNumber;
                        m.PlayerNumber = mem.PlayerNumber;
                        m.StartedSkating = mem.StartedSkating;
                        m.StoppedSkating = mem.StoppedSkating;
                        if (mem.Photos.FirstOrDefault(x => x.IsPrimaryPhoto) != null)
                            m.ThumbUrl = mem.Photos.FirstOrDefault(x => x.IsPrimaryPhoto).ImageThumbUrl;
                        m.WeightLbs = mem.WeightLbs;
                        return Json(m, JsonRequestBehavior.AllowGet);

                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
            return Json(new MemberDisplayEdit() { IsSuccessful = false }, JsonRequestBehavior.AllowGet);
        }


    }
}
