using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace WebApplication1.PDFConverter
{
    public static class SaveFile
    {
        public static string SaveFileFromBytes(byte[] httpFile, string folderName, string pdfPathToSave)
        {
            var uniqueName = Guid.NewGuid().ToString() + ".pdf";
            ///   var currentPath = "E:\\Mehmood ul Hassan" + "\\ProfileImages\\";
            //var currentPath = HttpContext.Current.Server.MapPath("~\\api\\ImageDirectory") + "\\" + folderName + "\\";
            if (!Directory.Exists(pdfPathToSave))
            {
                Directory.CreateDirectory(pdfPathToSave);
            }
            pdfPathToSave = pdfPathToSave + uniqueName;
            File.WriteAllBytes(pdfPathToSave, httpFile);
            //return "\\ImageDirectory\\" + folderName + "\\" + uniqueName;

            return pdfPathToSave;
        }
    }
}