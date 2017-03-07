using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalScec.DataManagment.Files.SCEC
{
    [Serializable]
    public class PeakRegionInformation : List<int>
    {
        #region Parameters

        /// <summary>
        /// [0] = Sigma
        /// [1] = Angle
        /// [2] = LorentzRatio
        /// [3] = Intensity
        /// [4] = ConstantBackground
        /// [5] = CenterBackground
        /// [6] = AclivityBackground
        /// </summary>
        public bool[] FreeParameters = { true, true, true, true, true, true, true };

        /// <summary>
        /// Use as follows:
        /// false = Off
        /// true = On
        /// </summary>
        public bool backgroundSwitch;
        public bool backgroundFit;

        public Pattern.Counts FittingCounts;
        public Analysis.Peaks.Functions.BackgroundPolynomial PolynomialBackgroundFunction;
        /// <summary>
        /// If positive this lamdba is used in LMA as starting parameter
        /// </summary>
        public double startingLambda;

        public bool _fitConverged;

        #endregion

        public PeakRegionInformation(Analysis.Peaks.Functions.PeakRegionFunction PRF)
        {
            FreeParameters[0] = PRF.FreeParameters[0];
            FreeParameters[1] = PRF.FreeParameters[1];
            FreeParameters[2] = PRF.FreeParameters[2];
            FreeParameters[3] = PRF.FreeParameters[3];
            FreeParameters[4] = PRF.FreeParameters[4];
            FreeParameters[5] = PRF.FreeParameters[5];
            FreeParameters[6] = PRF.FreeParameters[6];

            this.backgroundFit = PRF.backgroundFit;
            this.backgroundSwitch = PRF.backgroundSwitch;

            this.FittingCounts = PRF.FittingCounts.Clone() as Pattern.Counts;
            this.PolynomialBackgroundFunction = new Analysis.Peaks.Functions.BackgroundPolynomial(PRF.PolynomialBackgroundFunction.Center, PRF.PolynomialBackgroundFunction.Constant, PRF.PolynomialBackgroundFunction.Aclivity);

            this.startingLambda = PRF.startingLambda;
            this._fitConverged = PRF.FitConverged;

            for(int n = 0; n < PRF.Count; n++)
            {
                this.Add(PRF[n].PeakId);
            }
        }
    }
}
