﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using RDN.Library.Classes.Error;
using RDN.Library.DataModels.Context;
using RDN.Portable.Classes.Colors;

namespace RDN.Library.Classes.Colors
{
    public class ColorDisplayFactory
    {
        
        public static bool AddColor(string nameOfColor, string colorHex)
        {
            try
            {
                var dc = new ManagementContext();
                Color color = ColorTranslator.FromHtml(colorHex);
                int arb = color.ToArgb();
                var colorDb = dc.Colors.Where(x => x.ColorIdCSharp == arb).FirstOrDefault();

                if (colorDb == null)
                {
                    DataModels.Color.Color co = new DataModels.Color.Color();
                    co.ColorName = nameOfColor;
                    co.ColorIdCSharp = arb;
                    dc.Colors.Add(co);
                    int c = dc.SaveChanges();
                    return c > 0;
                }
                return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: nameOfColor + " " + colorHex);
            }
            return false;
        }
        public static bool AddOwnerColor(string nameOfColor, string colorHex, Guid ownerId)
        {
            try
            {
                var dc = new ManagementContext();
                Color color = ColorTranslator.FromHtml(colorHex);
                int arb = color.ToArgb();
                var colorDb = dc.Colors.FirstOrDefault(x => x.ColorIdCSharp == arb && x.OwnerGuid == ownerId);

                //add color if doesn't exist
                if (colorDb == null)
                {
                    colorDb = new DataModels.Color.Color();
                    colorDb.ColorName = nameOfColor;
                    colorDb.ColorIdCSharp = arb;
                    colorDb.OwnerGuid = ownerId;
                    dc.Colors.Add(colorDb);

                    int c = dc.SaveChanges();
                    return c > 0;
                }
                return true; 
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: nameOfColor + " " + colorHex);
            }
            return false;
        }
        
        public static List<ColorDisplay> GetColors()
        {
            try
            {
                var dc = new ManagementContext();

                var colors = (from xx in dc.Colors
                              select new ColorDisplay
                              {
                                  ColorId = xx.ColorId,
                                  CSharpColor = xx.ColorIdCSharp,
                                  NameOfColor = xx.ColorName
                              }).ToList();
                foreach (var color in colors)
                {
                    Color c = System.Drawing.Color.FromArgb(color.CSharpColor);
                    color.HexColor = ColorTranslator.ToHtml(c);

                }
                return colors.OrderBy(x => x.HexColor).ToList();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new List<ColorDisplay>();
        }
        public static List<ColorDisplay> GetOwnerColors(Guid ownerId)
        {
            try
            {
                var dc = new ManagementContext();

                var colors = (from xx in dc.Colors
                              where xx.OwnerGuid == ownerId
                              select new ColorDisplay
                              {
                                  ColorId = xx.ColorId,
                                  CSharpColor = xx.ColorIdCSharp,
                                  NameOfColor = xx.ColorName
                              }).OrderBy(x => x.CSharpColor).ToList();
                foreach (var color in colors)
                {
                    Color c = System.Drawing.Color.FromArgb(color.CSharpColor);
                    color.HexColor = ColorTranslator.ToHtml(c);

                }
                return colors;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new List<ColorDisplay>();
        }       
    }
}
