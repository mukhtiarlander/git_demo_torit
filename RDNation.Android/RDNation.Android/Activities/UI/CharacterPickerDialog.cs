using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Text.Method;
using Android.Views;
using Android.Widget;
using RDNation.Droid.Classes;
using RDN.Portable.Config.Enums;

namespace RDNation.Droid.Activities.UI
{
    public delegate void CharacterPickerDialogEventHandler(
          object sender, MyCharacterPickerDialog.CharacterPickerDialogEventArgs args);

    public class MyCharacterPickerDialog : CharacterPickerDialog
    {
        public class CharacterPickerDialogEventArgs : EventArgs
        {
            public string Text { get; set; }
        }

        public event CharacterPickerDialogEventHandler Clicked;

        protected MyCharacterPickerDialog(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public MyCharacterPickerDialog(Context context, View view, IEditable text, string options, bool insert)
            : base(context, view, text, options, insert)
        {
        }

        public override void OnClick(View v)
        {
            try
            {
                if (Clicked != null)
                    Clicked(this, new CharacterPickerDialogEventArgs
                    {
                        Text = ((TextView)v).Text
                    });
                base.OnClick(v);
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.Android, Context);

            }
        }
    }
}