using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalScec.Analysis.Peaks
{
    public class DiffractionPeak : ICloneable
    {
        #region Parameters

        #region Display

        public string AngleView
        {
            get
            {
                return this.Angle.ToString("F3");
            }
        }
        public string LatticeDistanceView
        {
            get
            {
                return this.LatticeDistance.ToString("F3");
            }
        }
        public string LatticeDistanceErrorView
        {
            get
            {
                return this.LatticeDistanceError.ToString("F3");
            }
        }

        #endregion

        #region Detection parameters

        private string _associatedPatternName;
        public string AssociatedPatternName
        {
            get
            {
                return this._associatedPatternName;
            }
            set
            {
                this._associatedPatternName = value;
            }
        }

        public int PeakId
        {
            get
            {
                return this.PFunction.PeakId;
            }
        }

        private int _detectedChannel;
        public int DetectedChannel
        {
            get
            {
                return this._detectedChannel;
            }
        }

        private double _detectedAngle;
        public double DetectedAngle
        {
            get
            {
                return this._detectedAngle;
            }
        }

        private double _detectedBackground;
        public double DetectedBackground
        {
            get
            {
                return this._detectedBackground;
            }
        }

        private double _detectedHeight;
        public double DetectedHeight
        {
            get
            {
                return this._detectedHeight;
            }
        }

        public double LatticeDistanceError
        {
            get
            {
                //double ErrorTmp = this.PFunction.Sigma / Math.Sqrt(Math.Log(this.PFunction.Intensity));
                double ErrorTmp = this.PFunction.Sigma / Math.Log(this.PFunction.Intensity);
                ErrorTmp *= LatticeDistance;
                ErrorTmp /= Math.Tan((Math.PI * this.PFunction.Angle) / 360);
                return ErrorTmp;
            }
        }
        public double LatticeDistance
        {
            get
            {
                double ret = CalScec.Properties.Settings.Default.UsedWaveLength;
                ret /= 2 * Math.Sin((Math.PI * this.PFunction.Angle) / 360);
                return ret;
            }
        }

        #endregion

        #region HKL Stuff

        private bool _toHKLAssociated = false;
        public bool ToHKLAssociated
        {
            get
            {
                return this._toHKLAssociated;
            }
        }
        public string HKLAssociation
        {
            get
            {
                if(_toHKLAssociated)
                {
                    string ret = AssociatedCrystalData.SymmetryGroup + " ( " + AssociatedHKLReflex.H + ", " + AssociatedHKLReflex.K + ", " + AssociatedHKLReflex.L + " )";
                    return ret;
                }
                else
                {
                    return "Not associated";
                }
            }
        }

        public DataManagment.CrystalData.CODData AssociatedCrystalData;
        public DataManagment.CrystalData.HKLReflex AssociatedHKLReflex;

        public void AddHKLAssociation(DataManagment.CrystalData.HKLReflex forAssociationHKL, DataManagment.CrystalData.CODData forAssociationCD)
        {
            this._toHKLAssociated = true;
            this.AssociatedHKLReflex = forAssociationHKL;
            this.AssociatedCrystalData = forAssociationCD;
        }

        public void RemoveHKLAssociation()
        {
            this._toHKLAssociated = false;
            this.AssociatedHKLReflex = new DataManagment.CrystalData.HKLReflex();
            this.AssociatedCrystalData = new DataManagment.CrystalData.CODData();
        }

        #endregion

        public Functions.PeakFunction PFunction;

        public double Angle
        {
            get
            {
                return PFunction.Angle;
            }
        }

        #endregion

        public DiffractionPeak(int dChannel, double dAngle, double dHeight, double dBackground)
        {
            this._detectedChannel = dChannel;
            this._detectedAngle = dAngle;
            this._detectedHeight = dHeight;
            this._detectedBackground = dBackground;
        }

        public DiffractionPeak(int dChannel, double dAngle, double dHeight, double dBackground, Pattern.Counts fittingData)
        {
            this._detectedChannel = dChannel;
            this._detectedAngle = dAngle;
            this._detectedHeight = dHeight;
            this._detectedBackground = dBackground;

            this.PFunction = new Functions.PeakFunction(this._detectedAngle, this._detectedHeight, this._detectedBackground, fittingData);
        }

        public DiffractionPeak(int dChannel, double dAngle, double dHeight, double dBackground, double estimatedFWHM, Pattern.Counts fittingData)
        {
            this._detectedChannel = dChannel;
            this._detectedAngle = dAngle;
            this._detectedHeight = dHeight;
            this._detectedBackground = dBackground;

            this.PFunction = new Functions.PeakFunction(this._detectedAngle, this._detectedHeight, this._detectedBackground, estimatedFWHM, fittingData);
        }

        public DiffractionPeak()
        {

        }

        public DiffractionPeak(DataManagment.Files.SCEC.PeakFunctionInformation PFI)
        {
            this._detectedAngle = PFI._detectedAngle;
            this._detectedBackground = PFI._detectedBackground;
            this._detectedChannel = PFI._detectedChannel;
            this._detectedHeight = PFI._detectedHeight;

            this._toHKLAssociated = PFI._toHKLAssociated;
            this.AssociatedCrystalData = PFI.AssociatedCrystalData;
            this.AssociatedHKLReflex = PFI.AssociatedHKLReflex;

            this.PFunction = new Functions.PeakFunction(PFI);
        }
    

        #region IClonable

        public object Clone()
        {
            DiffractionPeak ret = new DiffractionPeak();

            ret._detectedChannel = this._detectedChannel;
            ret._detectedAngle = this._detectedAngle;
            ret._detectedHeight = this._detectedHeight;
            ret._detectedBackground = this._detectedBackground;

            ret.PFunction = this.PFunction.Clone() as Functions.PeakFunction;

            ret._toHKLAssociated = this._toHKLAssociated;
            if (this._toHKLAssociated)
            {
                ret.AssociatedCrystalData = new DataManagment.CrystalData.CODData(this.AssociatedCrystalData);
                ret.AssociatedHKLReflex = new DataManagment.CrystalData.HKLReflex(this.AssociatedHKLReflex.H, this.AssociatedHKLReflex.K, this.AssociatedHKLReflex.L, this.AssociatedHKLReflex.Distance);
            }

            return ret;
        }

        #endregion
    }
}
