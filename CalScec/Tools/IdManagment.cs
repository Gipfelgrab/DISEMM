using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalScec.Tools
{
    public static class IdManagment
    {
        public static int ActualCODId
        {
            get
            {
                CalScec.Properties.Settings.Default.CODId++;

                return CalScec.Properties.Settings.Default.CODId;
            }

        }

        public static int ActualPeakId
        {
            get
            {
                CalScec.Properties.Settings.Default.PeakId++;

                return CalScec.Properties.Settings.Default.PeakId;
            }

        }
    }
}
