using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Utilities
{
    internal static class DataGathering
    {
        internal static int UnplacedRooms(Document doc)
        {
            int counter = 0;
            foreach (Room room in new FilteredElementCollector(doc).WherePasses(new RoomFilter()).Cast<Room>())
            {
                if (room.Location is null || room.Area == 0) { counter++; }
            }

            return counter;
        }

        internal static int UnusedTemplateCount(Document doc)
        {
            HashSet<string> applied_templates = new HashSet<string>();
            HashSet<string> all_templates = new HashSet<string>();

            foreach (View view in new FilteredElementCollector(doc).OfClass(typeof(View)))
            {
                if (view.IsTemplate)
                {
                    all_templates.Add(view.Id.ToString());
                }
                else
                {
                    applied_templates.Add(view.ViewTemplateId.ToString());
                }
            }

            int UnusedTemplates = all_templates.Count() - applied_templates.Count();

            return UnusedTemplates;
        }

        internal static int WorksetCount(Document doc)
        {
            FilteredWorksetCollector worksetlist = new FilteredWorksetCollector(doc).OfKind(WorksetKind.UserWorkset);
            return worksetlist.Count();
        }

        internal static int DwgCount(Document doc)
        {
            int counter = 0;
            Categories cats = doc.Settings.Categories;
            foreach (Category c in cats)
            {
                if (c.Name.Contains(".dwg")) { counter++; }
            }
            return counter;
        }

        internal static int ImportedLinePatCount(Document doc)
        {
            int counter = 0;
            FilteredElementCollector all_pats = new FilteredElementCollector(doc).OfClass(typeof(LinePatternElement));
            foreach (Element pattern in all_pats)
            {
                if (pattern.Name.Contains("IMPORT-")) { counter++; }
            }

            return counter;
        }

        internal static int? LocalFileSize(Document doc)
        {


            ModelPath centralpath = doc.GetWorksharingCentralModelPath();

            string user = doc.Application.LoginUserId;
            Guid modelGUID = centralpath.GetModelGUID();
            Guid projectGUID = centralpath.GetProjectGUID();

            string ver = doc.Application.SubVersionNumber;
            ver = ver.Substring(0, ver.LastIndexOf('.'));

            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            string directory = $"{appdata}\\Autodesk\\Revit\\Autodesk Revit {ver}\\CollaborationCache\\{user}\\{projectGUID}\\{modelGUID}.rvt";

            try
            {
                long length = new System.IO.FileInfo(directory).Length;
                length = length / 1000000;
                return (int)Math.Round(value: length, digits: 2);
            }

            catch (Exception ex)
            {
                return null;
            }
        }

        internal static int Warnings(Document doc)
        {
            return doc.GetWarnings().Count;
        }

        internal static int ModelGroups(Document doc)
        {
            return new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_IOSModelGroups).GetElementCount();
        }

        internal static int DetailGroups(Document doc)
        {
            return new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_IOSDetailGroups).GetElementCount();
        }

        internal static int TotalRegions(Document doc)
        {
            return new FilteredElementCollector(doc).OfClass(typeof(FilledRegionType)).GetElementCount();
        }
    }
}