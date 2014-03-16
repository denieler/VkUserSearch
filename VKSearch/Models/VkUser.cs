using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VKSearch.Models
{
    public class VkUser
    {
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string screen_name { get; set; }
        public string bdate { get; set; }
        public int hasMobile { get; set; }

        public long id { get; set; }
        public string photo_100 { get; set; }
        public string photo_200_orig { get; set; }

        public string status { get; set; }
    }
}