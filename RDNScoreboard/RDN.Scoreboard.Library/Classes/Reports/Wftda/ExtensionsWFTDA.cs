using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Scoreboard.Library.Static.Enums;
using Scoreboard.Library.ViewModel;

namespace Scoreboard.Library.Classes.Reports.Excel
{
    public static class ExtensionsWFTDA
    {
        public static ExcelRange SetPenaltySummaryHeaderAttribute(this ExcelRange cell)
        {
            cell.SetBackgroundColor(Color.Black).SetCenterBottom().SetFontSize(8);
            cell.Style.TextRotation = 90;
            cell.Style.WrapText = true;
            cell.SetFontColor(Color.White);
            return cell;
        }
        public static ExcelRange SetBoutSummaryFooterValues(this ExcelRange cell, Color background)
        {
            cell.SetBackgroundColor(background).SetBorder(ExcelBorderStyle.Thin).SetFontSize(11);
            return cell;
        }
        public static ExcelRange SetBoutSummaryHeader(this ExcelRange cell)
        {
            cell.Merge().SetBackgroundColor(Color.Black).SetFontColor(Color.White).SetCenterAlign();
            return cell;
        }
        public static ExcelRange SetBoutSummaryAttributesHeader3(this ExcelRange cell)
        {
            cell.SetCenterAlign().SetBorder(ExcelBorderStyle.Thin).SetBackgroundColor(Color.Black).SetFontColor(Color.White);
            cell.Style.TextRotation = 90;
            cell.Style.Font.Size = 9;
            cell.Style.WrapText = true;
            return cell;
        }
        public static ExcelRange SetBoutSummaryAttributesHeader2(this ExcelRange cell)
        {
            cell.SetCenterAlign().SetBorder(ExcelBorderStyle.Thin);
            cell.Style.Font.Size = 9;
            cell.Style.TextRotation = 90;
            cell.Style.WrapText = true;
            return cell;
        }

        public static ExcelRange SetPenaltyAbbrieviation(this ExcelRange cell, PenaltiesWFTDAEnum penalty)
        {
            cell.SetCenterAlign().SetFontBold().SetValue(PenaltyViewModel.ToAbbreviation(penalty));
            return cell;
        }
        public static ExcelRange SetPenaltyName(this ExcelRange cell, PenaltiesWFTDAEnum penalty)
        {
            cell.SetCenterAlign().SetFontSize(8).SetValue(RDN.Utilities.Enums.EnumExt.ToFreindlyName(penalty));
            return cell;
        }
        public static ExcelRange SetPageTitle(this ExcelRange cell)
        {
            cell.Merge = true;
            cell.Style.Font.Bold = true;
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            return cell;
        }
        public static ExcelRange SetTitleFont(this ExcelRange cell)
        {
            cell.Merge = true;
            cell.Style.Font.Bold = true;
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cell.Style.Font.Color.SetColor(Color.Pink);
            cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
            cell.Style.Fill.BackgroundColor.SetColor(Color.Black);
            return cell;
        }
        public static ExcelRange SetBackgroundColor(this ExcelRange cell, Color color)
        {
            cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
            cell.Style.Fill.BackgroundColor.SetColor(color);
            return cell;
        }

        public static ExcelRange SetFontSize(this ExcelRange cell, int size)
        {
            cell.Style.Font.Size = size;
            return cell;
        }
        public static ExcelRange SetBorderBottom(this ExcelRange cell, ExcelBorderStyle style)
        {
            cell.Style.Border.Bottom.Style = style;
            return cell;
        }
        public static ExcelRange Merge(this ExcelRange cell)
        {
            cell.Merge = true;
            return cell;
        }
        public static ExcelRange SetTitleBreak(this ExcelRange cell)
        {
            cell.Merge = true;
            cell.Style.Font.Bold = true;
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cell.Style.Font.Color.SetColor(Color.White);
            cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
            cell.Style.Fill.BackgroundColor.SetColor(Color.Black);
            return cell;
        }

        /// <summary>
        /// this is the light pink header.
        /// </summary>
        /// <param name="cell"></param>
        public static ExcelRange SetHeader2(this ExcelRange cell)
        {
            cell.Merge = true;
            cell.Style.Font.Bold = true;
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cell.Style.Font.Color.SetColor(Color.Black);
            cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
            cell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 232, 255));
            return cell;
        }

        public static ExcelRange SetWhiteSpace(this ExcelRange cell)
        {
            cell.Merge = true;
            return cell;
        }
        public static ExcelRange SetLavenderSpace(this ExcelRange cell)
        {
            cell.Merge = true;
            cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
            cell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 232, 255));
            return cell;
        }
        /// <summary>
        /// without the bold effect
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public static ExcelRange SetHeader3(this ExcelRange cell)
        {
            cell.Merge = true;
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cell.Style.Font.Color.SetColor(Color.Black);
            cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
            cell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 232, 255));
            return cell;
        }
        public static ExcelRange SetBorder(this ExcelRange cell, ExcelBorderStyle style)
        {
            cell.Style.Border.BorderAround(style);
            return cell;
        }
        public static ExcelRange SetFontColor(this ExcelRange cell, Color color)
        {
            cell.Style.Font.Color.SetColor(color);
            return cell;
        }
        public static ExcelRange SetFontBold(this ExcelRange cell)
        {
            cell.Style.Font.Bold = true;
            return cell;
        }
        public static ExcelRange SetFontItalic(this ExcelRange cell)
        {
            cell.Style.Font.Italic = true;
            return cell;
        }
        public static ExcelRange WrapText(this ExcelRange cell)
        {
            cell.Style.WrapText = true;
            return cell;
        }
        public static ExcelRange SetTitleRowScoresPage(this ExcelRange cell)
        {
            cell.SetBackgroundColor(Color.Black).SetBorder(ExcelBorderStyle.Thin).SetFontColor(Color.White).SetFontBold().SetCenterAlign();
            return cell;
        }
        public static ExcelRange SetTitleRowLineupPage(this ExcelRange cell)
        {
            cell.SetBackgroundColor(Color.Black).Merge().SetBorder(ExcelBorderStyle.Thin).SetFontColor(Color.White).SetCenterAlign();
            return cell;
        }
        public static ExcelRange SetTitleRowFlippedScoresPage(this ExcelRange cell)
        {
            cell.SetBackgroundColor(Color.Black).SetFontColor(Color.White).SetFontBold().SetCenterAlign();
            cell.Style.TextRotation = 90;
            cell.Style.Font.Size = 9;
            return cell;
        }
        public static ExcelRange SetCenterAlign(this ExcelRange cell)
        {
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            return cell;
        }
        public static ExcelRange SetCenterBottom(this ExcelRange cell)
        {
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cell.Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
            return cell;
        }
        public static ExcelRange SetRightAlignment(this ExcelRange cell)
        {
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            return cell;
        }
        public static ExcelRange SetValue(this ExcelRange cell, string value)
        {
            cell.Value = value;
            return cell;
        }
        public static ExcelRange SetValue(this ExcelRange cell, int value)
        {
            cell.Value = value;
            return cell;
        }
        public static ExcelRange SetFormula(this ExcelRange cell, string formula)
        {
            cell.Formula = formula;
            return cell;
        }
        /// <summary>
        /// this is the dark pink header.
        /// </summary>
        /// <param name="cell"></param>
        public static ExcelRange SetHeader1(this ExcelRange cell)
        {
            cell.Merge = true;
            cell.Style.Font.Bold = true;
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cell.Style.Font.Color.SetColor(Color.Black);
            cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
            cell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 208, 255));
            cell.Style.Font.Size = 9;
            return cell;
        }
    }
}
