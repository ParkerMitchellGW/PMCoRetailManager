using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRMDesktopUI.Library
{
    internal static class SecureStringExtensions
    {
        // convert a secure string into a normal plain text string
        public static string ToPlainString(this System.Security.SecureString secureStr)
        {
            string plainStr = new System.Net.NetworkCredential(string.Empty,
                              secureStr).Password;
            return plainStr;
        }
    }
}
