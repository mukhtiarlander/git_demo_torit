namespace RDN.Library.Classes.Account.Enums
{
    public enum NewUserEnum
    {
        Email_IsEmpty,
        Email_IllegalCharacters,
        Email_InUse,
        Email_EmailRepeatIncorrect,
        Email_EmailNotRightIncorrect,
        UserName_IsEmpty,
        UserName_IllegalCharacters,
        UserName_TooShort,
        UserName_InUse,
        Password_IsEmpty,
        Password_TooShort,
        Gender_IsntSelected,
        Position_IsntSelected,
        /// <summary>
        /// This could mean missing a special character like ! % etc or it could be that there is no big character or no number or similar
        /// </summary>
        Password_RuleViolated,
        Password_PasswordRepeatIncorrect,
        Firstname_IsEmpty,
        Firstname_IllegalCharacters,
        Firstname_TooShort,
        Firstname_DoesntMatchVerifiedFirstName,
        Nickname_IsEmpty,
        League_LeagueNotFound,
        Error_Save,

        User_AlreadyAttachedToAccount
    }
}
