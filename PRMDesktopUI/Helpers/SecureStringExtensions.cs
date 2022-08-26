using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRMDesktopUI.Helpers
{
    internal static class SecureStringExtensions
    {
        // convert a secure string into a normal plain text string
        public static String ToPlainString(this System.Security.SecureString secureStr)
        {
            String plainStr = new System.Net.NetworkCredential(string.Empty,
                              secureStr).Password;
            return plainStr;
        }
    }
}
