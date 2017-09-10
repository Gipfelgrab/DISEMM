using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalScec.DataManagment.Files.SCEC
{
    [Serializable]
    public class ODFDataInformation
    {
        /// <summary>
        /// [0] phi1
        /// [1] phi
        /// [2] phi2
        /// [3] value
        /// </summary>
        public List<double[]> TDData = new List<double[]>();

        public DataManagment.CrystalData.CODData CrystalData = new DataManagment.CrystalData.CODData();
    }
}
