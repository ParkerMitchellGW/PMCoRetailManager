using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRMDesktopUI.Library.Helpers
{
    public class ConfigHelper : IConfigHelper
    {
        public decimal GetTaxRate()
        {
            string rateText = ConfigurationManager.AppSettings["taxRate"];

            decimal output;
            bool isValidTaxRate = Decimal.TryParse(rateText, out output);
            if (!isValidTaxRate)
            {
                throw new ConfigurationErrorsException("The tax rate is not set up propertly.");
            }
            return output;
        }
    }
}
