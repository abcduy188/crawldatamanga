using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace crawldataweb.Common
{
    [Serializable]
    public class pageSession
    {
        public long page { set; get; } //các hàm properties
        public long IDCate { set; get; }
    }
}