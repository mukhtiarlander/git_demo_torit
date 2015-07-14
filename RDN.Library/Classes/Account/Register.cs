using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Classes.Account.Enums;

namespace RDN.Library.Classes.Account
{
    public class Register
    {
        public static List<string> RegisterErrors(IEnumerable<NewUserEnum> list)
        {
            var result = new List<string>();

            foreach (var item in list)
            {
                switch (item)
                {

                    case NewUserEnum.User_AlreadyAttachedToAccount:
                        result.Add("This Profile Has Already Been Claimed");
                        break;
                    case NewUserEnum.Email_EmailNotRightIncorrect:
                        result.Add("The email address you gave does not match our records");
                        break;
                    case NewUserEnum.Email_EmailRepeatIncorrect:
                        result.Add("The email address and confirmation email address do not match");
                        break;
                    case NewUserEnum.Email_IllegalCharacters:
                        result.Add("The email contains illegal characters");
                        break;
                    case NewUserEnum.Email_InUse:
                        result.Add("The email supplied is already in use");
                        break;
                    case NewUserEnum.Email_IsEmpty:
                        result.Add("The email field is empty");
                        break;
                    case NewUserEnum.Error_Save:
                        result.Add("An error occured when saving your data, try again in a little while");
                        break;
                    case NewUserEnum.Firstname_IllegalCharacters:
                        result.Add("The firstname contains illegal characters");
                        break;
                    case NewUserEnum.Firstname_DoesntMatchVerifiedFirstName:
                        result.Add("The firstname you gave does not match our records");
                        break;
                    case NewUserEnum.Firstname_IsEmpty:
                        result.Add("The firstname field is empty");
                        break;
                    case NewUserEnum.Firstname_TooShort:
                        result.Add("The firstname is too short");
                        break;
                    case NewUserEnum.Password_IsEmpty:
                        result.Add("The password field is empty");
                        break;
                    case NewUserEnum.Password_PasswordRepeatIncorrect:
                        result.Add("The password and confirmation password do not match.");
                        break;
                    case NewUserEnum.Password_RuleViolated:
                        result.Add("The number of special caracters/case letters and numbers is not fulfilled");
                        break;
                    case NewUserEnum.Password_TooShort:
                        result.Add("The password is too short");
                        break;
                    case NewUserEnum.Gender_IsntSelected:
                        result.Add("The gender currently isn't selected");
                        break;
                    case NewUserEnum.Nickname_IsEmpty:
                        result.Add("There is no Nick Name Given");
                        break;
                    case NewUserEnum.Position_IsntSelected:
                        result.Add("Position is not selected");
                        break;
                    case NewUserEnum.League_LeagueNotFound:
                        result.Add("The supplied league does not exist");
                        break;
                    case NewUserEnum.UserName_IllegalCharacters:
                        result.Add("The username contains illegal characters");
                        break;
                    case NewUserEnum.UserName_InUse:
                        result.Add("The username is already in use");
                        break;
                    case NewUserEnum.UserName_IsEmpty:
                        result.Add("The username field is empty");
                        break;
                    case NewUserEnum.UserName_TooShort:
                        result.Add("The username is too short");
                        break;
                }
            }

            return result;
        }

    }
}
