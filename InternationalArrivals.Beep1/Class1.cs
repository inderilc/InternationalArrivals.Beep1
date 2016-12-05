using Beep1.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Beep1Common.Utils;
using Beep1.MenuBase;
using Beep1.Utils;
using FishbowlSDK;
using Beep1.Objects;
using System.IO;

namespace InternationalArrivals.Beep1
{
    public class IACycle : CustomerController
    {
        private ItemCache ic;

        public IACycle(GlobalResources res) : base(res)
        {
            api = res.api;
            db = res.db;
            ic = res.ItemCache;
        }

        public override bool HasAccess()
        {
            return true;
        }

        public override bool IsCompatible()
        {
            return true;
        }

        public override MenuOptions PublicMenuOption()
        {
            return new MenuOptions("IA Cycle", "IAC", 99, this);
        }

        public override void Run()
        {
            bool cancel = false;
            while (!cancel)
            {
                C.CL();
                CycleCount(out cancel);
            }

        }
        private void CycleCount(out bool cancel)
        {
            CycleLineItem item = new CycleLineItem();
            cancel = false;
            bool finish = false;
            Location location = null;
            String partNo = null;
            int qty = 0;
            Dictionary<String, int> PartList = new Dictionary<string, int>();
            while (!cancel && !finish)
            {
                bool isReady = location != null && !String.IsNullOrEmpty(partNo) && qty > 0;
                C.CL();
                C.WL("Cycle Count");
                C.WL("Location: " + (location?.Name ?? "NONE"));
                C.WL("Part: " + (partNo ?? "None"));
                C.WL("Qty: " + ((qty > 0) ? qty : 0));

                if (location == null)
                {
                    C.WL("Location:");
                    location = LocationUtils.captureDisplayLocation(api, C.getString(out cancel));
                    if (cancel)
                        continue;
                }
                if (location != null)
                {
                    ConsoleKey? fnKey;
                    C.WL("Part #:");
                    String scan = C.getStringFinishableFunctions(out cancel, out finish, out fnKey);
                    if (cancel)
                    {
                        var yn = C.YN("Are you sure? Existing.");
                        cancel = yn;
                        continue;

                    }
                    else if (finish)
                    {
                        var yn = C.YN("Are you sure? Existing.");
                        finish = yn;
                        continue;
                    }
                    else if (fnKey.HasValue)
                    {

                    }
                    else if (!String.IsNullOrWhiteSpace(scan))
                    {
                        var part = ic.Find(scan);

                        if (part != null)
                        {
                            partNo = part.PARTNUMBER;
                        }
                        else
                        {
                            C.A("Part# Not Found");
                        }
                    }
                    else
                    {
                        C.A("Scan Error");
                    }
                }
                if (location != null && partNo != null)
                {
                    ConsoleKey? fnKey;
                    C.WL("Qty :");
                    String scan = C.getStringFinishableFunctions(out cancel, out finish, out fnKey);
                    if (cancel)
                    {
                        var yn = C.YN("Are you sure? Existing.");
                        cancel = yn;
                        continue;
                    }
                    else if (finish)
                    {
                        var yn = C.YN("Are you sure? Existing.");
                        finish = yn;
                        continue;
                    }
                    else if (fnKey.HasValue)
                    {

                    }
                    else if (!String.IsNullOrWhiteSpace(scan))
                    {
                        var QTY = scan;
                        if (QTY != null)
                        {
                            qty = Int32.Parse(QTY);
                            isReady = true;
                        }
                        else
                        {
                            C.A("Invalid Qty");
                        }
                    }
                    else
                    {
                        C.A("Entry Error");
                    }
                }
                if (isReady)
                {
                    PartGetRsType rq = api.getPart(partNo);
                    if (rq.statusCode == "1000")
                    {
                        item.PartNumber = partNo;
                        item.Location = location.Name;
                        item.Qty = Convert.ToString(qty);
                        item.PartDescription = rq.Part.Description;
                        item.UOM = rq.Part.UOM.Name;
                        SaveToCSV(item);
                        location = null;
                        partNo = null;
                        qty = 0;
                    }
                }

            }


        }

        private void SaveToCSV(CycleLineItem item)
        {
            String sw = item.PartNumber +"," + item.PartDescription + "," +item.Location + "," +item.Qty + "," + item.QtyCommitted + "," + item.UOM + "," + item.Date + "," + item.Note + "," + item.Customer + "," + item.TrackingLotNumber + "," + item.TrackingRevisionLevel + "," + item.TrackingExpirationDate;
            string fileName = DateTime.Now.ToString("ddMMyyyy") + ".csv";
            File.WriteAllText(fileName, sw);
        }
    }
}
