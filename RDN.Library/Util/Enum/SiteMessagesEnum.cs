using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Util.Enum
{
    public enum SiteMessagesEnum
    {
        //internal message sent.
        ms = 0,
        //internal message not sent.
        mns = 1,
        //you do not have access to that page.
        na = 3,
        //something went wrong, a error message was saved.
        sww = 4,
        //successful in adding a location
        sal = 5,
        //successfully added new league class
        sac = 6,
        //successfully added group
        sag = 7,
        //successfully removed group
        srg = 8,
        //canceled the stripe connection in store settings.
        sced = 9,
        //Stripe key added successfully
        sca = 10,
        //tells the sign up page that the user is NOT connected to Roller Derby
        f = 11,
        /// <summary>
        /// just a success of something.
        /// </summary>
        s = 12,
        //wrong beta code for store creation
        wbc = 13,
        //something was deleted
        de = 14,
        //paypal dues email isn't confirmed by paypal.
        ppldnc = 15,
        //used for users who should pay subscription to access the feature.
        pp = 16,
        /// <summary>
        /// reocurring event updated
        /// </summary>
        re = 17,
        /// <summary>
        /// calendar event wasn't deleted 
        /// </summary>
        evwd = 18,
        /// <summary>
        /// event type updated
        /// </summary>
        et = 19,
        /// <summary>
        /// doesn't exist
        /// </summary>
        dex = 20,
        /// <summary>
        /// changed the league succesfully.
        /// </summary>
        cls = 21,
        /// <summary>
        /// changed leagues unsuccessfully.
        /// </summary>
        clus = 22,
        /// <summary>
        /// successfully updated
        /// </summary>
        su = 23,
        /// <summary>
        /// successfully updated settings.
        /// </summary>
        sus = 24,
        /// <summary>
        /// succwessvully uploaded picture.
        /// </summary>
        sup = 25,
        /// <summary>
        /// refunded order
        /// </summary>
        ro = 26,
        /// <summary>
        /// succesffulyl voted.
        /// </summary>
        sv = 27,
        /// <summary>
        /// Poll was closed
        /// </summary>
        pc = 28,
        /// <summary>
        /// Closed
        /// </summary>
        cl= 29,
        /// <summary>
        /// subscription was canceled
        /// </summary>
        sc = 30
    }
}
