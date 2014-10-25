using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Utilities.Documents
{
    public class ExcelExt
    {
        /// <summary>
        /// https://epplus.codeplex.com/workitem/12457
        /// </summary>
        /// <param name="dblWidth"></param>
        /// <returns></returns>
        public static double GetTrueColumnWidth(double dblWidth)
        {
            //DEDUCE WHAT THE COLUMN WIDTH WOULD REALLY GET SET TO
            double z = 1d;
            if (dblWidth >= (1 + 2 / 3))
                z = Math.Round((Math.Round(7 * (dblWidth - 1 / 256), 0) - 5) / 7, 2);
            else
                z = Math.Round((Math.Round(12 * (dblWidth - 1 / 256), 0) - Math.Round(5 * dblWidth, 0)) / 12, 2);

            //HOW FAR OFF? (WILL BE LESS THAN 1)
            double errorAmt = dblWidth - z;

            //CALCULATE WHAT AMOUNT TO TACK ONTO THE ORIGINAL AMOUNT TO RESULT IN THE CLOSEST POSSIBLE SETTING 
            double adjAmt = 0d;
            if (dblWidth >= (1 + 2 / 3))
                adjAmt = (Math.Round(7 * errorAmt - 7 / 256, 0)) / 7;
            else
                adjAmt = ((Math.Round(12 * errorAmt - 12 / 256, 0)) / 12) + (2 / 12);

            //RETURN A SCALED-VALUE THAT SHOULD RESULT IN THE NEAREST POSSIBLE VALUE TO THE TRUE DESIRED SETTING
            if (z > 0)
                return dblWidth + adjAmt;
            return 0d;

        }
    }
}
