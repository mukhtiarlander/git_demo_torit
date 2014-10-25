namespace App_Code
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Script.Services;
    using System.Web.Services;
    using BlogEngine.Core;
    using BlogEngine.Core.Data.Models;
    using BlogEngine.Core.Web.Extensions;

    /// <summary>
    /// The comments.
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ScriptService]
    public class Comments : WebService
    {
        #region Constants and Fields

        /// <summary>
        ///     JSON object that will be return back to client
        /// </summary>
        private readonly JsonResponse response;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref = "Comments" /> class.
        /// </summary>
        static Comments()
        {
            CurrentPage = 1;
            LastPage = 1;
            CommCnt = 1;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref = "Comments" /> class.
        /// </summary>
        public Comments()
        {
            this.response = new JsonResponse();
        }

        #endregion

        /// <summary>
        ///     Gets or sets the comm CNT.
        /// </summary>
        /// <value>The comm CNT.</value>
        protected static int CommCnt { get; set; }

        /// <summary>
        ///     Gets or sets the last page.
        /// </summary>
        /// <value>The last page.</value>
        protected static int LastPage { get; set; }

        /// <summary>
        ///     Gets or sets the current page.
        /// </summary>
        /// <value>The current page.</value>
        protected static int CurrentPage { get; set; }

    }
}