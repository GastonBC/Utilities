using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Utilities
{
    public static partial class Utils
    {
        public static void SimpleDialog(string header, string content)
        {
            TaskDialog mainDialog = new TaskDialog("Gas Tools");
            mainDialog.TitleAutoPrefix = false;
            mainDialog.MainInstruction = header;
            mainDialog.MainContent = content;
            mainDialog.Show();
        }

        public static void SimpleDialog(string content)
        {
            TaskDialog mainDialog = new TaskDialog("Gas Tools");
            mainDialog.TitleAutoPrefix = false;
            mainDialog.MainContent = content;
            mainDialog.Show();
        }

        public static bool ConfirmDialog(string header, string content)
        {
            TaskDialog mainDialog = new TaskDialog("Gas Tools")
            {
                TitleAutoPrefix = false,
                MainInstruction = header,
                MainContent = content,
                CommonButtons = TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No
            };

            TaskDialogResult res = mainDialog.Show();

            switch (res)
            {
                case TaskDialogResult.Yes:
                    return true;
                default:
                    return false;
            }
        }

        public static string GetExeConfigPath(string DllName)
        {
            string ThisDllPath = Assembly.GetExecutingAssembly().Location;
            Assembly ThisAssembly = Assembly.GetExecutingAssembly();

            // Assembly that contains the invoke method
            return Path.GetDirectoryName(ThisDllPath) + "\\" + DllName;
        }

        /// <summary>
        /// Create a panel in the default Addins tab
        /// </summary>
        public static RibbonPanel GetRevitPanel(UIControlledApplication uiApp, string PanelName)
        {
            RibbonPanel DefaultPanel = null;


            // Create the panel in the addins tab
            try
            {
                DefaultPanel = uiApp.CreateRibbonPanel(PanelName);
            }

            catch (Autodesk.Revit.Exceptions.ArgumentException)
            {
                DefaultPanel = uiApp.GetRibbonPanels().FirstOrDefault(n => n.Name.Equals(PanelName, StringComparison.InvariantCulture));
            }

            return DefaultPanel;
        }

        /// <summary>
        /// Create a panel in a specific existing or new tab
        /// </summary>
        public static RibbonPanel GetRevitPanel(UIControlledApplication uiApp, string PanelName, string TabName)
        {
            RibbonPanel DefaultPanel = null;
            GetRevitTab(uiApp, TabName);


            // Create the panel in the addins tab
            try
            {
                DefaultPanel = uiApp.CreateRibbonPanel(TabName, PanelName);
            }

            catch (Autodesk.Revit.Exceptions.ArgumentException)
            {
                DefaultPanel = uiApp.GetRibbonPanels(TabName).FirstOrDefault(n => n.Name.Equals(PanelName, StringComparison.InvariantCulture));
            }

            return DefaultPanel;
        }


        /// <summary>
        /// Create or get a Tab
        /// </summary>
        public static bool GetRevitTab(UIControlledApplication uiApp, string TabName)
        {
            // First try to create the tab
            try
            {
                uiApp.CreateRibbonTab(TabName);
                return true;
            }
            catch (Autodesk.Revit.Exceptions.ArgumentException)
            {
                // Tab is already created
                return true;
            }

            catch (Exception ex)
            {
                CatchDialog(ex);
                return false;
            }
            
        }

public static void CatchDialog(Exception ex)
        {
            string head = ex.Source + " - " + ex.GetType().ToString();
            string moreText = ex.Message + "\n\n" + ex.StackTrace + "\n\n" + ex.Data;

            Utils.SimpleDialog(head, moreText);
        }


        internal static void CatchDialog(Exception ex, string TitlePrefix)
        {
            string head = TitlePrefix + " " + ex.Source + " - " + ex.GetType().ToString();
            string moreText = ex.Message + "\n\n" + ex.StackTrace;

            Utils.SimpleDialog(head, moreText);
        }


        /// <summary>
        /// Convert embedded ico, png, jpg, etc to something usable by Revit
        /// </summary>
        /// <param name="imagePath">Path to the embedded resource</param>
        /// <returns>ImageSource</returns>
        public static ImageSource RetriveImage(string imagePath, Assembly assembly)
        {

            Stream manifestResourceStream = assembly.GetManifestResourceStream(imagePath);
            string str = imagePath.Substring(imagePath.Length - 3);

            if (str == "jpg")
                return (ImageSource)new JpegBitmapDecoder(bitmapStream: manifestResourceStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default).Frames[0];
            else if (str == "bmp")
                return (ImageSource)new BmpBitmapDecoder(bitmapStream: manifestResourceStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default).Frames[0];
            else if (str == "png")
                return (ImageSource)new PngBitmapDecoder(bitmapStream: manifestResourceStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default).Frames[0];
            else if (str == "ico")
                return (ImageSource)new IconBitmapDecoder(bitmapStream: manifestResourceStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default).Frames[0];
            else
                return (ImageSource)null;
        }

        public static List<ViewSheet> GetAllSheets(Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc).OfClass(typeof(ViewSheet));
            List<ViewSheet> SheetList = new List<ViewSheet>();
            foreach (Element elem in collector)
            {
                ViewSheet Sheet = elem as ViewSheet;
                SheetList.Add(Sheet);
            }
            return SheetList;
        }
    }
}
