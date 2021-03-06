/* Yet Another Forum.NET
 * Copyright (C) 2006-2012 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */
namespace YAF.Providers.Membership
{
  #region Using

  using System;
  using System.Data;
  using System.Data.SqlClient;
  using System.Web.Security;

  using YAF.Classes;
  using YAF.Classes.Pattern;
  using YAF.Core; using YAF.Types.Interfaces; using YAF.Types.Constants;
  using YAF.Classes.Data;
  using YAF.Types;

  #endregion

  /// <summary>
  /// The yaf membership db conn manager.
  /// </summary>
  public class MsSqlMembershipDbConnectionManager : MsSqlDbConnectionManager
  {
    #region Properties

    /// <summary>
    ///   Gets ConnectionString.
    /// </summary>
    public override string ConnectionString
    {
      get
      {
        if (YafContext.Application[YafMembershipProvider.ConnStrAppKeyName] != null)
        {
          return YafContext.Application[YafMembershipProvider.ConnStrAppKeyName] as string;
        }

        return Config.ConnectionString;
      }
    }

    #endregion
  }

  /// <summary>
  /// The db.
  /// </summary>
  public class DB
  {
    #region Constants and Fields

    /// <summary>
    ///   The _db access.
    /// </summary>
    private readonly MsSqlDbAccess _msSqlDbAccess = new MsSqlDbAccess();

    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref = "DB" /> class.
    /// </summary>
    public DB()
    {
      this._msSqlDbAccess.SetConnectionManagerAdapter<MsSqlMembershipDbConnectionManager>();
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets Current.
    /// </summary>
    public static DB Current
    {
      get
      {
        return PageSingleton<DB>.Instance;
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// The change password.
    /// </summary>
    /// <param name="appName">
    /// The app name.
    /// </param>
    /// <param name="username">
    /// The username.
    /// </param>
    /// <param name="newPassword">
    /// The new password.
    /// </param>
    /// <param name="newSalt">
    /// The new salt.
    /// </param>
    /// <param name="passwordFormat">
    /// The password format.
    /// </param>
    /// <param name="newPasswordAnswer">
    /// The new password answer.
    /// </param>
    public void ChangePassword([NotNull] string appName, [NotNull] string username, [NotNull] string newPassword, [NotNull] string newSalt, int passwordFormat, [NotNull] string newPasswordAnswer)
    {
      using (var cmd = new SqlCommand(MsSqlDbAccess.GetObjectName("prov_changepassword")))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@ApplicationName", appName);

        // Nonstandard args
        cmd.Parameters.AddWithValue("@Username", username);
        cmd.Parameters.AddWithValue("@Password", newPassword);
        cmd.Parameters.AddWithValue("@PasswordSalt", newSalt);
        cmd.Parameters.AddWithValue("@PasswordFormat", passwordFormat);
        cmd.Parameters.AddWithValue("@PasswordAnswer", newPasswordAnswer);

        this._msSqlDbAccess.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// The change password question and answer.
    /// </summary>
    /// <param name="appName">
    /// The app name.
    /// </param>
    /// <param name="username">
    /// The username.
    /// </param>
    /// <param name="passwordQuestion">
    /// The password question.
    /// </param>
    /// <param name="passwordAnswer">
    /// The password answer.
    /// </param>
    public void ChangePasswordQuestionAndAnswer([NotNull] string appName, [NotNull] string username, [NotNull] string passwordQuestion, [NotNull] string passwordAnswer)
    {
      using (var cmd = new SqlCommand(MsSqlDbAccess.GetObjectName("prov_changepasswordquestionandanswer")))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@ApplicationName", appName);

        // Nonstandard args
        cmd.Parameters.AddWithValue("@Username", username);
        cmd.Parameters.AddWithValue("@PasswordQuestion", passwordQuestion);
        cmd.Parameters.AddWithValue("@PasswordAnswer", passwordAnswer);
        this._msSqlDbAccess.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// The create user.
    /// </summary>
    /// <param name="appName">
    /// The app name.
    /// </param>
    /// <param name="username">
    /// The username.
    /// </param>
    /// <param name="password">
    /// The password.
    /// </param>
    /// <param name="passwordSalt">
    /// The password salt.
    /// </param>
    /// <param name="passwordFormat">
    /// The password format.
    /// </param>
    /// <param name="email">
    /// The email.
    /// </param>
    /// <param name="passwordQuestion">
    /// The password question.
    /// </param>
    /// <param name="passwordAnswer">
    /// The password answer.
    /// </param>
    /// <param name="isApproved">
    /// The is approved.
    /// </param>
    /// <param name="providerUserKey">
    /// The provider user key.
    /// </param>
    public void CreateUser([NotNull] string appName, [NotNull] string username, [NotNull] string password, [NotNull] string passwordSalt, 
      int passwordFormat, [NotNull] string email, [NotNull] string passwordQuestion, [NotNull] string passwordAnswer, 
      bool isApproved, [NotNull] object providerUserKey)
    {
      using (var cmd = new SqlCommand(MsSqlDbAccess.GetObjectName("prov_createuser")))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@ApplicationName", appName);

        // Input Parameters
        cmd.Parameters.AddWithValue("Username", username);
        cmd.Parameters.AddWithValue("Password", password);
        cmd.Parameters.AddWithValue("PasswordSalt", passwordSalt);
        cmd.Parameters.AddWithValue("PasswordFormat", passwordFormat);
        cmd.Parameters.AddWithValue("Email", email);
        cmd.Parameters.AddWithValue("PasswordQuestion", passwordQuestion);
        cmd.Parameters.AddWithValue("PasswordAnswer", passwordAnswer);
        cmd.Parameters.AddWithValue("IsApproved", isApproved);
        cmd.Parameters.AddWithValue("@UTCTIMESTAMP", DateTime.UtcNow);

        // Input Output Parameters
        var paramUserKey = new SqlParameter("UserKey", SqlDbType.UniqueIdentifier);
        paramUserKey.Direction = ParameterDirection.InputOutput;
        paramUserKey.Value = providerUserKey;
        cmd.Parameters.Add(paramUserKey);

        // Execute
        this._msSqlDbAccess.ExecuteNonQuery(cmd);

        // Retrieve Output Parameters
        providerUserKey = paramUserKey.Value;
      }
    }

    /// <summary>
    /// The delete user.
    /// </summary>
    /// <param name="appName">
    /// The app name.
    /// </param>
    /// <param name="username">
    /// The username.
    /// </param>
    /// <param name="deleteAllRelatedData">
    /// The delete all related data.
    /// </param>
    public void DeleteUser([NotNull] string appName, [NotNull] string username, bool deleteAllRelatedData)
    {
      using (var cmd = new SqlCommand(MsSqlDbAccess.GetObjectName("prov_deleteuser")))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@ApplicationName", appName);

        // Nonstandard args
        cmd.Parameters.AddWithValue("@Username", username);
        cmd.Parameters.AddWithValue("@DeleteAllRelated", deleteAllRelatedData);
        this._msSqlDbAccess.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// The find users by email.
    /// </summary>
    /// <param name="appName">
    /// The app name.
    /// </param>
    /// <param name="emailToMatch">
    /// The email to match.
    /// </param>
    /// <param name="pageIndex">
    /// The page index.
    /// </param>
    /// <param name="pageSize">
    /// The page size.
    /// </param>
    /// <returns>
    /// </returns>
    public DataTable FindUsersByEmail([NotNull] string appName, [NotNull] string emailToMatch, int pageIndex, int pageSize)
    {
      using (var cmd = new SqlCommand(MsSqlDbAccess.GetObjectName("prov_findusersbyemail")))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@ApplicationName", appName);

        // Nonstandard args
        cmd.Parameters.AddWithValue("@EmailAddress", emailToMatch);
        cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
        cmd.Parameters.AddWithValue("@PageSize", pageSize);
        return this._msSqlDbAccess.GetData(cmd);
      }
    }

    /// <summary>
    /// The find users by name.
    /// </summary>
    /// <param name="appName">
    /// The app name.
    /// </param>
    /// <param name="usernameToMatch">
    /// The username to match.
    /// </param>
    /// <param name="pageIndex">
    /// The page index.
    /// </param>
    /// <param name="pageSize">
    /// The page size.
    /// </param>
    /// <returns>
    /// </returns>
    public DataTable FindUsersByName([NotNull] string appName, [NotNull] string usernameToMatch, int pageIndex, int pageSize)
    {
      using (var cmd = new SqlCommand(MsSqlDbAccess.GetObjectName("prov_findusersbyname")))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@ApplicationName", appName);

        // Nonstandard args
        cmd.Parameters.AddWithValue("@Username", usernameToMatch);
        cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
        cmd.Parameters.AddWithValue("@PageSize", pageSize);
        return this._msSqlDbAccess.GetData(cmd);
      }
    }

    /// <summary>
    /// The get all users.
    /// </summary>
    /// <param name="appName">
    /// The app name.
    /// </param>
    /// <param name="pageIndex">
    /// The page index.
    /// </param>
    /// <param name="pageSize">
    /// The page size.
    /// </param>
    /// <returns>
    /// </returns>
    public DataTable GetAllUsers([NotNull] string appName, int pageIndex, int pageSize)
    {
      using (var cmd = new SqlCommand(MsSqlDbAccess.GetObjectName("prov_getallusers")))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@ApplicationName", appName);

        // Nonstandard args
        cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
        cmd.Parameters.AddWithValue("@PageSize", pageSize);
        return this._msSqlDbAccess.GetData(cmd);
      }
    }

    /// <summary>
    /// The get number of users online.
    /// </summary>
    /// <param name="appName">
    /// The app name.
    /// </param>
    /// <param name="timeWindow">
    /// The time window.
    /// </param>
    /// <returns>
    /// The get number of users online.
    /// </returns>
    public int GetNumberOfUsersOnline([NotNull] string appName, int timeWindow)
    {
      using (var cmd = new SqlCommand(MsSqlDbAccess.GetObjectName("prov_getnumberofusersonline")))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@ApplicationName", appName);

        // Nonstandard args
        cmd.Parameters.AddWithValue("@TimeWindow", timeWindow);
        cmd.Parameters.AddWithValue("@CurrentTimeUtc", DateTime.UtcNow);
        var p = new SqlParameter("ReturnValue", SqlDbType.Int);
        p.Direction = ParameterDirection.ReturnValue;
        cmd.Parameters.Add(p);
        this._msSqlDbAccess.ExecuteNonQuery(cmd);
        return Convert.ToInt32(cmd.Parameters["ReturnValue"].Value);
      }
    }

    /// <summary>
    /// The get user.
    /// </summary>
    /// <param name="appName">
    /// The app name.
    /// </param>
    /// <param name="providerUserKey">
    /// The provider user key.
    /// </param>
    /// <param name="userName">
    /// The user name.
    /// </param>
    /// <param name="userIsOnline">
    /// The user is online.
    /// </param>
    /// <returns>
    /// </returns>
    public DataRow GetUser([NotNull] string appName, [NotNull] object providerUserKey, [NotNull] string userName, bool userIsOnline)
    {
      using (var cmd = new SqlCommand(MsSqlDbAccess.GetObjectName("prov_getuser")))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@ApplicationName", appName);

        // Nonstandard args
        cmd.Parameters.AddWithValue("@UserName", userName);
        cmd.Parameters.AddWithValue("@UserKey", providerUserKey);
        cmd.Parameters.AddWithValue("@UserIsOnline", userIsOnline);
        cmd.Parameters.AddWithValue("@UTCTIMESTAMP", DateTime.UtcNow);
        using (DataTable dt = this._msSqlDbAccess.GetData(cmd))
        {
          if (dt.Rows.Count > 0)
          {
            return dt.Rows[0];
          }
          else
          {
            return null;
          }
        }
      }
    }

    /// <summary>
    /// The get user name by email.
    /// </summary>
    /// <param name="appName">
    /// The app name.
    /// </param>
    /// <param name="email">
    /// The email.
    /// </param>
    /// <returns>
    /// </returns>
    public DataTable GetUserNameByEmail([NotNull] string appName, [NotNull] string email)
    {
      using (var cmd = new SqlCommand(MsSqlDbAccess.GetObjectName("prov_getusernamebyemail")))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@ApplicationName", appName);

        // Nonstandard args
        cmd.Parameters.AddWithValue("@Email", email);
        return this._msSqlDbAccess.GetData(cmd);
      }
    }

    /// <summary>
    /// The get user password info.
    /// </summary>
    /// <param name="appName">
    /// The app name.
    /// </param>
    /// <param name="username">
    /// The username.
    /// </param>
    /// <param name="updateUser">
    /// The update user.
    /// </param>
    /// <returns>
    /// </returns>
    public DataTable GetUserPasswordInfo([NotNull] string appName, [NotNull] string username, bool updateUser)
    {
      using (var cmd = new SqlCommand(MsSqlDbAccess.GetObjectName("prov_getuser")))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@ApplicationName", appName);

        // Nonstandard args
        cmd.Parameters.AddWithValue("@Username", username);
        cmd.Parameters.AddWithValue("@UserIsOnline", updateUser);
        cmd.Parameters.AddWithValue("@UTCTIMESTAMP", DateTime.UtcNow);
        return this._msSqlDbAccess.GetData(cmd);
      }
    }

    /// <summary>
    /// The reset password.
    /// </summary>
    /// <param name="appName">
    /// The app name.
    /// </param>
    /// <param name="userName">
    /// The user name.
    /// </param>
    /// <param name="password">
    /// The password.
    /// </param>
    /// <param name="passwordSalt">
    /// The password salt.
    /// </param>
    /// <param name="passwordFormat">
    /// The password format.
    /// </param>
    /// <param name="maxInvalidPasswordAttempts">
    /// The max invalid password attempts.
    /// </param>
    /// <param name="passwordAttemptWindow">
    /// The password attempt window.
    /// </param>
    public void ResetPassword([NotNull] string appName, [NotNull] string userName, [NotNull] string password, [NotNull] string passwordSalt, 
      int passwordFormat, 
      int maxInvalidPasswordAttempts, 
      int passwordAttemptWindow)
    {
      using (var cmd = new SqlCommand(MsSqlDbAccess.GetObjectName("prov_resetpassword")))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@ApplicationName", appName);

        // Nonstandard args
        cmd.Parameters.AddWithValue("@UserName", userName);
        cmd.Parameters.AddWithValue("@Password", password);
        cmd.Parameters.AddWithValue("@PasswordSalt", passwordSalt);
        cmd.Parameters.AddWithValue("@PasswordFormat", passwordFormat);
        cmd.Parameters.AddWithValue("@MaxInvalidAttempts", maxInvalidPasswordAttempts);
        cmd.Parameters.AddWithValue("@PasswordAttemptWindow", passwordAttemptWindow);
        cmd.Parameters.AddWithValue("@CurrentTimeUtc", DateTime.UtcNow);

        this._msSqlDbAccess.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// The unlock user.
    /// </summary>
    /// <param name="appName">
    /// The app name.
    /// </param>
    /// <param name="userName">
    /// The user name.
    /// </param>
    public void UnlockUser([NotNull] string appName, [NotNull] string userName)
    {
      using (var cmd = new SqlCommand(MsSqlDbAccess.GetObjectName("prov_unlockuser")))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@ApplicationName", appName);

        // Nonstandard args
        cmd.Parameters.AddWithValue("@UserName", userName);
        this._msSqlDbAccess.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// The update user.
    /// </summary>
    /// <param name="appName">
    /// The app name.
    /// </param>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <param name="requiresUniqueEmail">
    /// The requires unique email.
    /// </param>
    /// <returns>
    /// The update user.
    /// </returns>
    public int UpdateUser([NotNull] object appName, [NotNull] MembershipUser user, bool requiresUniqueEmail)
    {
      using (var cmd = new SqlCommand(MsSqlDbAccess.GetObjectName("prov_updateuser")))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("ApplicationName", appName);

        // Nonstandard args
        cmd.Parameters.AddWithValue("UserKey", user.ProviderUserKey);
        cmd.Parameters.AddWithValue("UserName", user.UserName);
        cmd.Parameters.AddWithValue("Email", user.Email);
        cmd.Parameters.AddWithValue("Comment", user.Comment);
        cmd.Parameters.AddWithValue("IsApproved", user.IsApproved);
        cmd.Parameters.AddWithValue("LastLogin", user.LastLoginDate);
        cmd.Parameters.AddWithValue("LastActivity", user.LastActivityDate.ToUniversalTime());
        cmd.Parameters.AddWithValue("UniqueEmail", requiresUniqueEmail);

        // Add Return Value
        var p = new SqlParameter("ReturnValue", SqlDbType.Int);
        p.Direction = ParameterDirection.ReturnValue;
        cmd.Parameters.Add(p);

        this._msSqlDbAccess.ExecuteNonQuery(cmd); // Execute Non SQL Query
        return Convert.ToInt32(p.Value); // Return
      }
    }

    /// <summary>
    /// The upgrade membership.
    /// </summary>
    /// <param name="previousVersion">
    /// The previous version.
    /// </param>
    /// <param name="newVersion">
    /// The new version.
    /// </param>
    public void UpgradeMembership(int previousVersion, int newVersion)
    {
      using (var cmd = new SqlCommand(MsSqlDbAccess.GetObjectName("prov_upgrade")))
      {
        cmd.CommandType = CommandType.StoredProcedure;

        // Nonstandard args
        cmd.Parameters.AddWithValue("@PreviousVersion", previousVersion);
        cmd.Parameters.AddWithValue("@NewVersion", newVersion);
        cmd.Parameters.AddWithValue("@UTCTIMESTAMP", DateTime.UtcNow);
        this._msSqlDbAccess.ExecuteNonQuery(cmd);
      }
    }

    #endregion
  }
}