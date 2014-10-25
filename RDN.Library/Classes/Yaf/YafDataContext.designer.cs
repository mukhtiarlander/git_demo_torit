﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RDN.Library.Classes.Yaf
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="Sports")]
	public partial class YafDataContextDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void Insertyaf_User(yaf_User instance);
    partial void Updateyaf_User(yaf_User instance);
    partial void Deleteyaf_User(yaf_User instance);
    #endregion
		
		public YafDataContextDataContext() : 
				base(global::RDN.Library.Properties.Settings.Default.SportsConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public YafDataContextDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public YafDataContextDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public YafDataContextDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public YafDataContextDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<yaf_User> yaf_Users
		{
			get
			{
				return this.GetTable<yaf_User>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.yaf_User")]
	public partial class yaf_User : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _UserID;
		
		private int _BoardID;
		
		private string _ProviderUserKey;
		
		private string _Name;
		
		private string _DisplayName;
		
		private string _Password;
		
		private string _Email;
		
		private System.DateTime _Joined;
		
		private System.DateTime _LastVisit;
		
		private string _IP;
		
		private int _NumPosts;
		
		private int _TimeZone;
		
		private string _Avatar;
		
		private string _Signature;
		
		private System.Data.Linq.Binary _AvatarImage;
		
		private string _AvatarImageType;
		
		private int _RankID;
		
		private System.Nullable<System.DateTime> _Suspended;
		
		private string _LanguageFile;
		
		private string _ThemeFile;
		
		private bool _UseSingleSignOn;
		
		private string _TextEditor;
		
		private bool _OverrideDefaultThemes;
		
		private bool _PMNotification;
		
		private bool _AutoWatchTopics;
		
		private bool _DailyDigest;
		
		private System.Nullable<int> _NotificationType;
		
		private int _Flags;
		
		private int _Points;
		
		private System.Nullable<bool> _IsApproved;
		
		private System.Nullable<bool> _IsGuest;
		
		private System.Nullable<bool> _IsCaptchaExcluded;
		
		private System.Nullable<bool> _IsActiveExcluded;
		
		private System.Nullable<bool> _IsDST;
		
		private System.Nullable<bool> _IsDirty;
		
		private string _Culture;
		
		private bool _IsFacebookUser;
		
		private bool _IsTwitterUser;
		
		private string _UserStyle;
		
		private int _StyleFlags;
		
		private System.Nullable<bool> _IsUserStyle;
		
		private System.Nullable<bool> _IsGroupStyle;
		
		private System.Nullable<bool> _IsRankStyle;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnUserIDChanging(int value);
    partial void OnUserIDChanged();
    partial void OnBoardIDChanging(int value);
    partial void OnBoardIDChanged();
    partial void OnProviderUserKeyChanging(string value);
    partial void OnProviderUserKeyChanged();
    partial void OnNameChanging(string value);
    partial void OnNameChanged();
    partial void OnDisplayNameChanging(string value);
    partial void OnDisplayNameChanged();
    partial void OnPasswordChanging(string value);
    partial void OnPasswordChanged();
    partial void OnEmailChanging(string value);
    partial void OnEmailChanged();
    partial void OnJoinedChanging(System.DateTime value);
    partial void OnJoinedChanged();
    partial void OnLastVisitChanging(System.DateTime value);
    partial void OnLastVisitChanged();
    partial void OnIPChanging(string value);
    partial void OnIPChanged();
    partial void OnNumPostsChanging(int value);
    partial void OnNumPostsChanged();
    partial void OnTimeZoneChanging(int value);
    partial void OnTimeZoneChanged();
    partial void OnAvatarChanging(string value);
    partial void OnAvatarChanged();
    partial void OnSignatureChanging(string value);
    partial void OnSignatureChanged();
    partial void OnAvatarImageChanging(System.Data.Linq.Binary value);
    partial void OnAvatarImageChanged();
    partial void OnAvatarImageTypeChanging(string value);
    partial void OnAvatarImageTypeChanged();
    partial void OnRankIDChanging(int value);
    partial void OnRankIDChanged();
    partial void OnSuspendedChanging(System.Nullable<System.DateTime> value);
    partial void OnSuspendedChanged();
    partial void OnLanguageFileChanging(string value);
    partial void OnLanguageFileChanged();
    partial void OnThemeFileChanging(string value);
    partial void OnThemeFileChanged();
    partial void OnUseSingleSignOnChanging(bool value);
    partial void OnUseSingleSignOnChanged();
    partial void OnTextEditorChanging(string value);
    partial void OnTextEditorChanged();
    partial void OnOverrideDefaultThemesChanging(bool value);
    partial void OnOverrideDefaultThemesChanged();
    partial void OnPMNotificationChanging(bool value);
    partial void OnPMNotificationChanged();
    partial void OnAutoWatchTopicsChanging(bool value);
    partial void OnAutoWatchTopicsChanged();
    partial void OnDailyDigestChanging(bool value);
    partial void OnDailyDigestChanged();
    partial void OnNotificationTypeChanging(System.Nullable<int> value);
    partial void OnNotificationTypeChanged();
    partial void OnFlagsChanging(int value);
    partial void OnFlagsChanged();
    partial void OnPointsChanging(int value);
    partial void OnPointsChanged();
    partial void OnIsApprovedChanging(System.Nullable<bool> value);
    partial void OnIsApprovedChanged();
    partial void OnIsGuestChanging(System.Nullable<bool> value);
    partial void OnIsGuestChanged();
    partial void OnIsCaptchaExcludedChanging(System.Nullable<bool> value);
    partial void OnIsCaptchaExcludedChanged();
    partial void OnIsActiveExcludedChanging(System.Nullable<bool> value);
    partial void OnIsActiveExcludedChanged();
    partial void OnIsDSTChanging(System.Nullable<bool> value);
    partial void OnIsDSTChanged();
    partial void OnIsDirtyChanging(System.Nullable<bool> value);
    partial void OnIsDirtyChanged();
    partial void OnCultureChanging(string value);
    partial void OnCultureChanged();
    partial void OnIsFacebookUserChanging(bool value);
    partial void OnIsFacebookUserChanged();
    partial void OnIsTwitterUserChanging(bool value);
    partial void OnIsTwitterUserChanged();
    partial void OnUserStyleChanging(string value);
    partial void OnUserStyleChanged();
    partial void OnStyleFlagsChanging(int value);
    partial void OnStyleFlagsChanged();
    partial void OnIsUserStyleChanging(System.Nullable<bool> value);
    partial void OnIsUserStyleChanged();
    partial void OnIsGroupStyleChanging(System.Nullable<bool> value);
    partial void OnIsGroupStyleChanged();
    partial void OnIsRankStyleChanging(System.Nullable<bool> value);
    partial void OnIsRankStyleChanged();
    #endregion
		
		public yaf_User()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_UserID", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int UserID
		{
			get
			{
				return this._UserID;
			}
			set
			{
				if ((this._UserID != value))
				{
					this.OnUserIDChanging(value);
					this.SendPropertyChanging();
					this._UserID = value;
					this.SendPropertyChanged("UserID");
					this.OnUserIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_BoardID", DbType="Int NOT NULL")]
		public int BoardID
		{
			get
			{
				return this._BoardID;
			}
			set
			{
				if ((this._BoardID != value))
				{
					this.OnBoardIDChanging(value);
					this.SendPropertyChanging();
					this._BoardID = value;
					this.SendPropertyChanged("BoardID");
					this.OnBoardIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ProviderUserKey", DbType="NVarChar(64)")]
		public string ProviderUserKey
		{
			get
			{
				return this._ProviderUserKey;
			}
			set
			{
				if ((this._ProviderUserKey != value))
				{
					this.OnProviderUserKeyChanging(value);
					this.SendPropertyChanging();
					this._ProviderUserKey = value;
					this.SendPropertyChanged("ProviderUserKey");
					this.OnProviderUserKeyChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Name", DbType="NVarChar(255) NOT NULL", CanBeNull=false)]
		public string Name
		{
			get
			{
				return this._Name;
			}
			set
			{
				if ((this._Name != value))
				{
					this.OnNameChanging(value);
					this.SendPropertyChanging();
					this._Name = value;
					this.SendPropertyChanged("Name");
					this.OnNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_DisplayName", DbType="NVarChar(255) NOT NULL", CanBeNull=false)]
		public string DisplayName
		{
			get
			{
				return this._DisplayName;
			}
			set
			{
				if ((this._DisplayName != value))
				{
					this.OnDisplayNameChanging(value);
					this.SendPropertyChanging();
					this._DisplayName = value;
					this.SendPropertyChanged("DisplayName");
					this.OnDisplayNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Password", DbType="NVarChar(32) NOT NULL", CanBeNull=false)]
		public string Password
		{
			get
			{
				return this._Password;
			}
			set
			{
				if ((this._Password != value))
				{
					this.OnPasswordChanging(value);
					this.SendPropertyChanging();
					this._Password = value;
					this.SendPropertyChanged("Password");
					this.OnPasswordChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Email", DbType="NVarChar(255)")]
		public string Email
		{
			get
			{
				return this._Email;
			}
			set
			{
				if ((this._Email != value))
				{
					this.OnEmailChanging(value);
					this.SendPropertyChanging();
					this._Email = value;
					this.SendPropertyChanged("Email");
					this.OnEmailChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Joined", DbType="DateTime NOT NULL")]
		public System.DateTime Joined
		{
			get
			{
				return this._Joined;
			}
			set
			{
				if ((this._Joined != value))
				{
					this.OnJoinedChanging(value);
					this.SendPropertyChanging();
					this._Joined = value;
					this.SendPropertyChanged("Joined");
					this.OnJoinedChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_LastVisit", DbType="DateTime NOT NULL")]
		public System.DateTime LastVisit
		{
			get
			{
				return this._LastVisit;
			}
			set
			{
				if ((this._LastVisit != value))
				{
					this.OnLastVisitChanging(value);
					this.SendPropertyChanging();
					this._LastVisit = value;
					this.SendPropertyChanged("LastVisit");
					this.OnLastVisitChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IP", DbType="VarChar(39)")]
		public string IP
		{
			get
			{
				return this._IP;
			}
			set
			{
				if ((this._IP != value))
				{
					this.OnIPChanging(value);
					this.SendPropertyChanging();
					this._IP = value;
					this.SendPropertyChanged("IP");
					this.OnIPChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_NumPosts", DbType="Int NOT NULL")]
		public int NumPosts
		{
			get
			{
				return this._NumPosts;
			}
			set
			{
				if ((this._NumPosts != value))
				{
					this.OnNumPostsChanging(value);
					this.SendPropertyChanging();
					this._NumPosts = value;
					this.SendPropertyChanged("NumPosts");
					this.OnNumPostsChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_TimeZone", DbType="Int NOT NULL")]
		public int TimeZone
		{
			get
			{
				return this._TimeZone;
			}
			set
			{
				if ((this._TimeZone != value))
				{
					this.OnTimeZoneChanging(value);
					this.SendPropertyChanging();
					this._TimeZone = value;
					this.SendPropertyChanged("TimeZone");
					this.OnTimeZoneChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Avatar", DbType="NVarChar(255)")]
		public string Avatar
		{
			get
			{
				return this._Avatar;
			}
			set
			{
				if ((this._Avatar != value))
				{
					this.OnAvatarChanging(value);
					this.SendPropertyChanging();
					this._Avatar = value;
					this.SendPropertyChanged("Avatar");
					this.OnAvatarChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Signature", DbType="NText", UpdateCheck=UpdateCheck.Never)]
		public string Signature
		{
			get
			{
				return this._Signature;
			}
			set
			{
				if ((this._Signature != value))
				{
					this.OnSignatureChanging(value);
					this.SendPropertyChanging();
					this._Signature = value;
					this.SendPropertyChanged("Signature");
					this.OnSignatureChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_AvatarImage", DbType="Image", CanBeNull=true, UpdateCheck=UpdateCheck.Never)]
		public System.Data.Linq.Binary AvatarImage
		{
			get
			{
				return this._AvatarImage;
			}
			set
			{
				if ((this._AvatarImage != value))
				{
					this.OnAvatarImageChanging(value);
					this.SendPropertyChanging();
					this._AvatarImage = value;
					this.SendPropertyChanged("AvatarImage");
					this.OnAvatarImageChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_AvatarImageType", DbType="NVarChar(50)")]
		public string AvatarImageType
		{
			get
			{
				return this._AvatarImageType;
			}
			set
			{
				if ((this._AvatarImageType != value))
				{
					this.OnAvatarImageTypeChanging(value);
					this.SendPropertyChanging();
					this._AvatarImageType = value;
					this.SendPropertyChanged("AvatarImageType");
					this.OnAvatarImageTypeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_RankID", DbType="Int NOT NULL")]
		public int RankID
		{
			get
			{
				return this._RankID;
			}
			set
			{
				if ((this._RankID != value))
				{
					this.OnRankIDChanging(value);
					this.SendPropertyChanging();
					this._RankID = value;
					this.SendPropertyChanged("RankID");
					this.OnRankIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Suspended", DbType="DateTime")]
		public System.Nullable<System.DateTime> Suspended
		{
			get
			{
				return this._Suspended;
			}
			set
			{
				if ((this._Suspended != value))
				{
					this.OnSuspendedChanging(value);
					this.SendPropertyChanging();
					this._Suspended = value;
					this.SendPropertyChanged("Suspended");
					this.OnSuspendedChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_LanguageFile", DbType="NVarChar(50)")]
		public string LanguageFile
		{
			get
			{
				return this._LanguageFile;
			}
			set
			{
				if ((this._LanguageFile != value))
				{
					this.OnLanguageFileChanging(value);
					this.SendPropertyChanging();
					this._LanguageFile = value;
					this.SendPropertyChanged("LanguageFile");
					this.OnLanguageFileChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ThemeFile", DbType="NVarChar(50)")]
		public string ThemeFile
		{
			get
			{
				return this._ThemeFile;
			}
			set
			{
				if ((this._ThemeFile != value))
				{
					this.OnThemeFileChanging(value);
					this.SendPropertyChanging();
					this._ThemeFile = value;
					this.SendPropertyChanged("ThemeFile");
					this.OnThemeFileChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_UseSingleSignOn", DbType="Bit NOT NULL")]
		public bool UseSingleSignOn
		{
			get
			{
				return this._UseSingleSignOn;
			}
			set
			{
				if ((this._UseSingleSignOn != value))
				{
					this.OnUseSingleSignOnChanging(value);
					this.SendPropertyChanging();
					this._UseSingleSignOn = value;
					this.SendPropertyChanged("UseSingleSignOn");
					this.OnUseSingleSignOnChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_TextEditor", DbType="NVarChar(50)")]
		public string TextEditor
		{
			get
			{
				return this._TextEditor;
			}
			set
			{
				if ((this._TextEditor != value))
				{
					this.OnTextEditorChanging(value);
					this.SendPropertyChanging();
					this._TextEditor = value;
					this.SendPropertyChanged("TextEditor");
					this.OnTextEditorChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_OverrideDefaultThemes", DbType="Bit NOT NULL")]
		public bool OverrideDefaultThemes
		{
			get
			{
				return this._OverrideDefaultThemes;
			}
			set
			{
				if ((this._OverrideDefaultThemes != value))
				{
					this.OnOverrideDefaultThemesChanging(value);
					this.SendPropertyChanging();
					this._OverrideDefaultThemes = value;
					this.SendPropertyChanged("OverrideDefaultThemes");
					this.OnOverrideDefaultThemesChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_PMNotification", DbType="Bit NOT NULL")]
		public bool PMNotification
		{
			get
			{
				return this._PMNotification;
			}
			set
			{
				if ((this._PMNotification != value))
				{
					this.OnPMNotificationChanging(value);
					this.SendPropertyChanging();
					this._PMNotification = value;
					this.SendPropertyChanged("PMNotification");
					this.OnPMNotificationChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_AutoWatchTopics", DbType="Bit NOT NULL")]
		public bool AutoWatchTopics
		{
			get
			{
				return this._AutoWatchTopics;
			}
			set
			{
				if ((this._AutoWatchTopics != value))
				{
					this.OnAutoWatchTopicsChanging(value);
					this.SendPropertyChanging();
					this._AutoWatchTopics = value;
					this.SendPropertyChanged("AutoWatchTopics");
					this.OnAutoWatchTopicsChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_DailyDigest", DbType="Bit NOT NULL")]
		public bool DailyDigest
		{
			get
			{
				return this._DailyDigest;
			}
			set
			{
				if ((this._DailyDigest != value))
				{
					this.OnDailyDigestChanging(value);
					this.SendPropertyChanging();
					this._DailyDigest = value;
					this.SendPropertyChanged("DailyDigest");
					this.OnDailyDigestChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_NotificationType", DbType="Int")]
		public System.Nullable<int> NotificationType
		{
			get
			{
				return this._NotificationType;
			}
			set
			{
				if ((this._NotificationType != value))
				{
					this.OnNotificationTypeChanging(value);
					this.SendPropertyChanging();
					this._NotificationType = value;
					this.SendPropertyChanged("NotificationType");
					this.OnNotificationTypeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Flags", DbType="Int NOT NULL")]
		public int Flags
		{
			get
			{
				return this._Flags;
			}
			set
			{
				if ((this._Flags != value))
				{
					this.OnFlagsChanging(value);
					this.SendPropertyChanging();
					this._Flags = value;
					this.SendPropertyChanged("Flags");
					this.OnFlagsChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Points", DbType="Int NOT NULL")]
		public int Points
		{
			get
			{
				return this._Points;
			}
			set
			{
				if ((this._Points != value))
				{
					this.OnPointsChanging(value);
					this.SendPropertyChanging();
					this._Points = value;
					this.SendPropertyChanged("Points");
					this.OnPointsChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IsApproved", AutoSync=AutoSync.Always, DbType="Bit", IsDbGenerated=true, UpdateCheck=UpdateCheck.Never)]
		public System.Nullable<bool> IsApproved
		{
			get
			{
				return this._IsApproved;
			}
			set
			{
				if ((this._IsApproved != value))
				{
					this.OnIsApprovedChanging(value);
					this.SendPropertyChanging();
					this._IsApproved = value;
					this.SendPropertyChanged("IsApproved");
					this.OnIsApprovedChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IsGuest", AutoSync=AutoSync.Always, DbType="Bit", IsDbGenerated=true, UpdateCheck=UpdateCheck.Never)]
		public System.Nullable<bool> IsGuest
		{
			get
			{
				return this._IsGuest;
			}
			set
			{
				if ((this._IsGuest != value))
				{
					this.OnIsGuestChanging(value);
					this.SendPropertyChanging();
					this._IsGuest = value;
					this.SendPropertyChanged("IsGuest");
					this.OnIsGuestChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IsCaptchaExcluded", AutoSync=AutoSync.Always, DbType="Bit", IsDbGenerated=true, UpdateCheck=UpdateCheck.Never)]
		public System.Nullable<bool> IsCaptchaExcluded
		{
			get
			{
				return this._IsCaptchaExcluded;
			}
			set
			{
				if ((this._IsCaptchaExcluded != value))
				{
					this.OnIsCaptchaExcludedChanging(value);
					this.SendPropertyChanging();
					this._IsCaptchaExcluded = value;
					this.SendPropertyChanged("IsCaptchaExcluded");
					this.OnIsCaptchaExcludedChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IsActiveExcluded", AutoSync=AutoSync.Always, DbType="Bit", IsDbGenerated=true, UpdateCheck=UpdateCheck.Never)]
		public System.Nullable<bool> IsActiveExcluded
		{
			get
			{
				return this._IsActiveExcluded;
			}
			set
			{
				if ((this._IsActiveExcluded != value))
				{
					this.OnIsActiveExcludedChanging(value);
					this.SendPropertyChanging();
					this._IsActiveExcluded = value;
					this.SendPropertyChanged("IsActiveExcluded");
					this.OnIsActiveExcludedChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IsDST", AutoSync=AutoSync.Always, DbType="Bit", IsDbGenerated=true, UpdateCheck=UpdateCheck.Never)]
		public System.Nullable<bool> IsDST
		{
			get
			{
				return this._IsDST;
			}
			set
			{
				if ((this._IsDST != value))
				{
					this.OnIsDSTChanging(value);
					this.SendPropertyChanging();
					this._IsDST = value;
					this.SendPropertyChanged("IsDST");
					this.OnIsDSTChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IsDirty", AutoSync=AutoSync.Always, DbType="Bit", IsDbGenerated=true, UpdateCheck=UpdateCheck.Never)]
		public System.Nullable<bool> IsDirty
		{
			get
			{
				return this._IsDirty;
			}
			set
			{
				if ((this._IsDirty != value))
				{
					this.OnIsDirtyChanging(value);
					this.SendPropertyChanging();
					this._IsDirty = value;
					this.SendPropertyChanged("IsDirty");
					this.OnIsDirtyChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Culture", DbType="VarChar(10)")]
		public string Culture
		{
			get
			{
				return this._Culture;
			}
			set
			{
				if ((this._Culture != value))
				{
					this.OnCultureChanging(value);
					this.SendPropertyChanging();
					this._Culture = value;
					this.SendPropertyChanged("Culture");
					this.OnCultureChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IsFacebookUser", DbType="Bit NOT NULL")]
		public bool IsFacebookUser
		{
			get
			{
				return this._IsFacebookUser;
			}
			set
			{
				if ((this._IsFacebookUser != value))
				{
					this.OnIsFacebookUserChanging(value);
					this.SendPropertyChanging();
					this._IsFacebookUser = value;
					this.SendPropertyChanged("IsFacebookUser");
					this.OnIsFacebookUserChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IsTwitterUser", DbType="Bit NOT NULL")]
		public bool IsTwitterUser
		{
			get
			{
				return this._IsTwitterUser;
			}
			set
			{
				if ((this._IsTwitterUser != value))
				{
					this.OnIsTwitterUserChanging(value);
					this.SendPropertyChanging();
					this._IsTwitterUser = value;
					this.SendPropertyChanged("IsTwitterUser");
					this.OnIsTwitterUserChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_UserStyle", DbType="VarChar(510)")]
		public string UserStyle
		{
			get
			{
				return this._UserStyle;
			}
			set
			{
				if ((this._UserStyle != value))
				{
					this.OnUserStyleChanging(value);
					this.SendPropertyChanging();
					this._UserStyle = value;
					this.SendPropertyChanged("UserStyle");
					this.OnUserStyleChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_StyleFlags", DbType="Int NOT NULL")]
		public int StyleFlags
		{
			get
			{
				return this._StyleFlags;
			}
			set
			{
				if ((this._StyleFlags != value))
				{
					this.OnStyleFlagsChanging(value);
					this.SendPropertyChanging();
					this._StyleFlags = value;
					this.SendPropertyChanged("StyleFlags");
					this.OnStyleFlagsChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IsUserStyle", AutoSync=AutoSync.Always, DbType="Bit", IsDbGenerated=true, UpdateCheck=UpdateCheck.Never)]
		public System.Nullable<bool> IsUserStyle
		{
			get
			{
				return this._IsUserStyle;
			}
			set
			{
				if ((this._IsUserStyle != value))
				{
					this.OnIsUserStyleChanging(value);
					this.SendPropertyChanging();
					this._IsUserStyle = value;
					this.SendPropertyChanged("IsUserStyle");
					this.OnIsUserStyleChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IsGroupStyle", AutoSync=AutoSync.Always, DbType="Bit", IsDbGenerated=true, UpdateCheck=UpdateCheck.Never)]
		public System.Nullable<bool> IsGroupStyle
		{
			get
			{
				return this._IsGroupStyle;
			}
			set
			{
				if ((this._IsGroupStyle != value))
				{
					this.OnIsGroupStyleChanging(value);
					this.SendPropertyChanging();
					this._IsGroupStyle = value;
					this.SendPropertyChanged("IsGroupStyle");
					this.OnIsGroupStyleChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IsRankStyle", AutoSync=AutoSync.Always, DbType="Bit", IsDbGenerated=true, UpdateCheck=UpdateCheck.Never)]
		public System.Nullable<bool> IsRankStyle
		{
			get
			{
				return this._IsRankStyle;
			}
			set
			{
				if ((this._IsRankStyle != value))
				{
					this.OnIsRankStyleChanging(value);
					this.SendPropertyChanging();
					this._IsRankStyle = value;
					this.SendPropertyChanged("IsRankStyle");
					this.OnIsRankStyleChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
#pragma warning restore 1591