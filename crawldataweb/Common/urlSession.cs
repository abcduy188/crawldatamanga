using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace crawldataweb.Common
{
    [Serializable]
    public class urlSession
    {
        public string url { set; get; } //các hàm properties
        public long Id { set; get; }
    }
}