using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoUpdate
{
    class UpdateModel
    {
        public string version { get; set; }
        public string product { get; set; }
        public string url { get; set; }
        public DateTime date { get; set; }
        public string log { get; set; }

        public List<FileInfo> files { get; set; }

        [JsonIgnore]
        public long ver
        {
            get
            {
                long data = 0;
                var temp = version.Replace(".", "");
                long.TryParse(temp, out data);
                return data;
            }
        }

    }

    class FileInfo
    {
        public string file { get; set; }
    }
}
