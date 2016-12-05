using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternationalArrivals.Beep1
{
    class CycleLineItem
    {
        public String PartNumber { get; set; }
        public String PartDescription { get; set; }
        public String Location { get; set; }
        public String Qty { get; set; }
        public String QtyCommitted { get; set; }
        public String UOM { get; set; }
        public String Date { get; set; }
        public String Note { get; set; }
        public String Customer { get; set; }
        public String TrackingLotNumber { get; set; }
        public String TrackingRevisionLevel { get; set; }
        public String TrackingExpirationDate { get; set; }
    }
}
