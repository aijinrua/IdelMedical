using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdelMedical.User.Kr.Models
{
    public class Home_LandingRequestList_Response
    {
        public int TotalItemCount { get; internal set; }
        public int Page { get; internal set; }
        public Item[] Items { get; internal set; }
        public string Search { get; internal set; }

        public class Item
        {
            public DateTime CreateTime { get; internal set; }
            public int Id { get; internal set; }
            public string Memo { get; internal set; }
            public string Phone { get; internal set; }
            public string Question { get; internal set; }
            public string UserName { get; internal set; }
        }
    }
}
