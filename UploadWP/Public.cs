using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UploadWP
{
    sealed class Public
    {
        public static List<string> Flags = new List<string>();
        public static readonly AQHelpers.Helpers Helpers = new AQHelpers.Helpers(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
        public static readonly AQFiles.Files Files = new AQFiles.Files();
        public static PersistData PersistData = new PersistData();
    }
    public class PersistData
    {
        public DateTime last_run = new DateTime();
        public DateTime last_purged_orphaned = new DateTime();
        public Dictionary<string, Dates> pages = new Dictionary<string,Dates>();
        public Dictionary<string, Dates> files = new Dictionary<string, Dates>();
    }
    public class Dates
    {
        public DateTime created = new DateTime();
        public DateTime updated = new DateTime();
        public DateTime uploaded { get; set; }
    }
    public class FileData
    {
        public StringBuilder text = new StringBuilder();
        public string path = String.Empty;
        public string filename = String.Empty;
    }
}
