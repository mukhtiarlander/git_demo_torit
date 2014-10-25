using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace RDNScoreboard.Code
{
  public   class StringUtil
    {
        [DllImport("user32.dll")]
        static extern short VkKeyScan(char ch);

        static public Key ResolveKey(char charToResolve)
        {
            return KeyInterop.KeyFromVirtualKey(VkKeyScan(charToResolve));
        }
    }
}
