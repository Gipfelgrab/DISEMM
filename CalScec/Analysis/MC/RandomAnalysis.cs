using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CalScec.Analysis.MC
{
    public class RandomAnalysis : ICloneable
    {
        #region Parameters

        private string _name = "";
        public string Name
        {
            get
            {
                return this._name;
            }
            set
            {
                this._name = value;
            }
        }

        #region statics

        public static string GetStiffnessConstantName(int number)
        {
            string Ret = "";
            switch (number)
            {
                case 0:
                    Ret = "C11";
                    break;
                case 1:
                    Ret = "C22";
                    break;
                case 2:
                    Ret = "C33";
                    break;
                case 3:
                    Ret = "C44";
                    break;
                case 4:
                    Ret = "C55";
                    break;
                case 5:
                    Ret = "C66";
                    break;
                case 6:
                    Ret = "C12";
                    break;
                case 7:
                    Ret = "C13";
                    break;
                case 8:
                    Ret = "C23";
                    break;
                case 9:
                    Ret = "C45";
                    break;
                case 10:
                    Ret = "C16";
                    break;
                case 11:
                    Ret = "C26";
                    break;
                case 12:
                    Ret = "C36";
                    break;
                default:
                    Ret = "";
                    break;
            }

            return Ret;
        }
        public static string GetComplianceConstantName(int number)
        {
            string Ret = "";
            switch (number)
            {
                case 0:
                    Ret = "S11";
                    break;
                case 1:
                    Ret = "S22";
                    break;
                case 2:
                    Ret = "S33";
                    break;
                case 3:
                    Ret = "S44";
                    break;
                case 4:
                    Ret = "S55";
                    break;
                case 5:
                    Ret = "S66";
                    break;
                case 6:
                    Ret = "S12";
                    break;
                case 7:
                    Ret = "S13";
                    break;
                case 8:
                    Ret = "S23";
                    break;
                case 9:
                    Ret = "S45";
                    break;
                case 10:
                    Ret = "S16";
                    break;
                case 11:
                    Ret = "S26";
                    break;
                case 12:
                    Ret = "S36";
                    break;
                default:
                    Ret = "";
                    break;
            }

            return Ret;
        }

        public static int GetConstantNumber(string name)
        {
            int Ret = -1;

            switch (name)
            {
                case "C11":
                    Ret = 0;
                    break;
                case "C22":
                    Ret = 1;
                    break;
                case "C33":
                    Ret = 2;
                    break;
                case "C44":
                    Ret = 3;
                    break;
                case "C55":
                    Ret = 4;
                    break;
                case "C66":
                    Ret = 5;
                    break;
                case "C12":
                    Ret = 6;
                    break;
                case "C13":
                    Ret = 7;
                    break;
                case "C23":
                    Ret = 8;
                    break;
                case "C45":
                    Ret = 9;
                    break;
                case "C16":
                    Ret = 10;
                    break;
                case "C26":
                    Ret = 11;
                    break;
                case "C36":
                    Ret = 12;
                    break;
                default:
                    Ret = 0;
                    break;
            }

            if(Ret == -1)
            {
                switch (name)
                {
                    case "S11":
                        Ret = 0;
                        break;
                    case "S22":
                        Ret = 1;
                        break;
                    case "S33":
                        Ret = 2;
                        break;
                    case "S44":
                        Ret = 3;
                        break;
                    case "S55":
                        Ret = 4;
                        break;
                    case "S66":
                        Ret = 5;
                        break;
                    case "S12":
                        Ret = 6;
                        break;
                    case "S13":
                        Ret = 7;
                        break;
                    case "S23":
                        Ret = 8;
                        break;
                    case "S45":
                        Ret = 9;
                        break;
                    case "S16":
                        Ret = 10;
                        break;
                    case "S26":
                        Ret = 11;
                        break;
                    case "S36":
                        Ret = 12;
                        break;
                    default:
                        Ret = 0;
                        break;
                }
            }

            return Ret;
        }

        #endregion

        public Stress.Microsopic.ElasticityTensors InvestigatedTensor;

        public List<Stress.Microsopic.ElasticityTensors> TensorResults = new List<Stress.Microsopic.ElasticityTensors>();

        /// <summary>
        /// [0]:C11
        /// [1]:C22
        /// [2]:C33
        /// [3]:C44
        /// [4]:C55
        /// [5]:C66
        /// [6]:C12
        /// [7]:C13
        /// [8]:C23
        /// [9]:C45
        /// [10]:C16
        /// [11]:C26
        /// [12]:C36
        /// </summary>
        public bool[] UsedConstants = { false, false, false, false, false, false, false, false, false, false, false, false, false };

        /// <summary>
        /// List with borders ind the same order as UsedConstants
        /// [0]: Lower border (Total value)
        /// [1]: Upper border (Total value)
        /// [2]: Stepwidth (equal distribution) or Total number of points (random distribution)
        /// </summary>
        public double[][] ConstantBorders;

        public bool ClassicREKCaluclation = true;

        public double EstimatedSimulationNumber(bool EqualDistribution)
        {
            double Ret = 0.0;
            
            for (int n = 0; n < UsedConstants.Count(); n++)
            {
                if (UsedConstants[n])
                {
                    if (EqualDistribution)
                    {
                        double Tmp = ConstantBorders[n][1] - ConstantBorders[n][0];
                        Tmp /= ConstantBorders[n][2];
                        if (Ret == 0)
                        {
                            Ret = Tmp;
                        }
                        else
                        {
                            Ret *= Tmp;
                        }
                    }
                    else
                    {
                        if (Ret == 0)
                        {
                            Ret = ConstantBorders[n][2];
                        }
                        else
                        {
                            Ret *= ConstantBorders[n][2];
                        }
                    }
                }
            }

            return Ret;
        }

        #region Borders

        #region Lower

        public string C11LBString
        {
            get
            {
                return this.ConstantBorders[0][0].ToString("e3");
            }
            set
            {
                try
                {
                    double NewC11LB = Convert.ToDouble(value);
                    this.ConstantBorders[0][0] = NewC11LB;
                }
                catch
                {

                }
            }
        }

        public string C22LBString
        {
            get
            {
                return this.ConstantBorders[1][0].ToString("e3");
            }
            set
            {
                try
                {
                    double NewC11LB = Convert.ToDouble(value);
                    this.ConstantBorders[1][0] = NewC11LB;
                }
                catch
                {

                }
            }
        }

        public string C33LBString
        {
            get
            {
                return this.ConstantBorders[2][0].ToString("e3");
            }
            set
            {
                try
                {
                    double NewC11LB = Convert.ToDouble(value);
                    this.ConstantBorders[2][0] = NewC11LB;
                }
                catch
                {

                }
            }
        }

        public string C44LBString
        {
            get
            {
                return this.ConstantBorders[3][0].ToString("e3");
            }
            set
            {
                try
                {
                    double NewC11LB = Convert.ToDouble(value);
                    this.ConstantBorders[3][0] = NewC11LB;
                }
                catch
                {

                }
            }
        }

        public string C55LBString
        {
            get
            {
                return this.ConstantBorders[4][0].ToString("e3");
            }
            set
            {
                try
                {
                    double NewC11LB = Convert.ToDouble(value);
                    this.ConstantBorders[4][0] = NewC11LB;
                }
                catch
                {

                }
            }
        }

        public string C66LBString
        {
            get
            {
                return this.ConstantBorders[5][0].ToString("e3");
            }
            set
            {
                try
                {
                    double NewC11LB = Convert.ToDouble(value);
                    this.ConstantBorders[5][0] = NewC11LB;
                }
                catch
                {

                }
            }
        }

        public string C12LBString
        {
            get
            {
                return this.ConstantBorders[6][0].ToString("e3");
            }
            set
            {
                try
                {
                    double NewC11LB = Convert.ToDouble(value);
                    this.ConstantBorders[6][0] = NewC11LB;
                }
                catch
                {

                }
            }
        }

        public string C13LBString
        {
            get
            {
                return this.ConstantBorders[7][0].ToString("e3");
            }
            set
            {
                try
                {
                    double NewC11LB = Convert.ToDouble(value);
                    this.ConstantBorders[7][0] = NewC11LB;
                }
                catch
                {

                }
            }
        }

        public string C23LBString
        {
            get
            {
                return this.ConstantBorders[8][0].ToString("e3");
            }
            set
            {
                try
                {
                    double NewC11LB = Convert.ToDouble(value);
                    this.ConstantBorders[8][0] = NewC11LB;
                }
                catch
                {

                }
            }
        }

        public string C45LBString
        {
            get
            {
                return this.ConstantBorders[9][0].ToString("e3");
            }
            set
            {
                try
                {
                    double NewC11LB = Convert.ToDouble(value);
                    this.ConstantBorders[9][0] = NewC11LB;
                }
                catch
                {

                }
            }
        }

        public string C16LBString
        {
            get
            {
                return this.ConstantBorders[10][0].ToString("e3");
            }
            set
            {
                try
                {
                    double NewC11LB = Convert.ToDouble(value);
                    this.ConstantBorders[10][0] = NewC11LB;
                }
                catch
                {

                }
            }
        }

        public string C26LBString
        {
            get
            {
                return this.ConstantBorders[11][0].ToString("e3");
            }
            set
            {
                try
                {
                    double NewC11LB = Convert.ToDouble(value);
                    this.ConstantBorders[11][0] = NewC11LB;
                }
                catch
                {

                }
            }
        }

        public string C36LBString
        {
            get
            {
                return this.ConstantBorders[12][0].ToString("e3");
            }
            set
            {
                try
                {
                    double NewC11LB = Convert.ToDouble(value);
                    this.ConstantBorders[12][0] = NewC11LB;
                }
                catch
                {

                }
            }
        }

        #endregion

        #region Upper

        public string C11UBString
        {
            get
            {
                return this.ConstantBorders[0][1].ToString("e3");
            }
            set
            {
                try
                {
                    double NewC11LB = Convert.ToDouble(value);
                    this.ConstantBorders[0][1] = NewC11LB;
                }
                catch
                {

                }
            }
        }

        public string C22UBString
        {
            get
            {
                return this.ConstantBorders[1][1].ToString("e3");
            }
            set
            {
                try
                {
                    double NewC11LB = Convert.ToDouble(value);
                    this.ConstantBorders[1][1] = NewC11LB;
                }
                catch
                {

                }
            }
        }

        public string C33UBString
        {
            get
            {
                return this.ConstantBorders[2][1].ToString("e3");
            }
            set
            {
                try
                {
                    double NewC11LB = Convert.ToDouble(value);
                    this.ConstantBorders[2][1] = NewC11LB;
                }
                catch
                {

                }
            }
        }

        public string C44UBString
        {
            get
            {
                return this.ConstantBorders[3][1].ToString("e3");
            }
            set
            {
                try
                {
                    double NewC11LB = Convert.ToDouble(value);
                    this.ConstantBorders[3][1] = NewC11LB;
                }
                catch
                {

                }
            }
        }

        public string C55UBString
        {
            get
            {
                return this.ConstantBorders[4][1].ToString("e3");
            }
            set
            {
                try
                {
                    double NewC11LB = Convert.ToDouble(value);
                    this.ConstantBorders[4][1] = NewC11LB;
                }
                catch
                {

                }
            }
        }

        public string C66UBString
        {
            get
            {
                return this.ConstantBorders[5][1].ToString("e3");
            }
            set
            {
                try
                {
                    double NewC11LB = Convert.ToDouble(value);
                    this.ConstantBorders[5][1] = NewC11LB;
                }
                catch
                {

                }
            }
        }

        public string C12UBString
        {
            get
            {
                return this.ConstantBorders[6][1].ToString("e3");
            }
            set
            {
                try
                {
                    double NewC11LB = Convert.ToDouble(value);
                    this.ConstantBorders[6][1] = NewC11LB;
                }
                catch
                {

                }
            }
        }

        public string C13UBString
        {
            get
            {
                return this.ConstantBorders[7][1].ToString("e3");
            }
            set
            {
                try
                {
                    double NewC11LB = Convert.ToDouble(value);
                    this.ConstantBorders[7][1] = NewC11LB;
                }
                catch
                {

                }
            }
        }

        public string C23UBString
        {
            get
            {
                return this.ConstantBorders[8][1].ToString("e3");
            }
            set
            {
                try
                {
                    double NewC11LB = Convert.ToDouble(value);
                    this.ConstantBorders[8][1] = NewC11LB;
                }
                catch
                {

                }
            }
        }

        public string C45UBString
        {
            get
            {
                return this.ConstantBorders[9][1].ToString("e3");
            }
            set
            {
                try
                {
                    double NewC11LB = Convert.ToDouble(value);
                    this.ConstantBorders[9][1] = NewC11LB;
                }
                catch
                {

                }
            }
        }

        public string C16UBString
        {
            get
            {
                return this.ConstantBorders[10][1].ToString("e3");
            }
            set
            {
                try
                {
                    double NewC11LB = Convert.ToDouble(value);
                    this.ConstantBorders[10][1] = NewC11LB;
                }
                catch
                {

                }
            }
        }

        public string C26UBString
        {
            get
            {
                return this.ConstantBorders[11][1].ToString("e3");
            }
            set
            {
                try
                {
                    double NewC11LB = Convert.ToDouble(value);
                    this.ConstantBorders[11][1] = NewC11LB;
                }
                catch
                {

                }
            }
        }

        public string C36UBString
        {
            get
            {
                return this.ConstantBorders[12][1].ToString("e3");
            }
            set
            {
                try
                {
                    double NewC11LB = Convert.ToDouble(value);
                    this.ConstantBorders[12][1] = NewC11LB;
                }
                catch
                {

                }
            }
        }

        #endregion

        #endregion

        #region SValue

        public string C11SValueString
        {
            get
            {
                return this.ConstantBorders[0][2].ToString("e3");
            }
            set
            {
                try
                {
                    double NewC11LB = Convert.ToDouble(value);
                    this.ConstantBorders[0][2] = NewC11LB;
                }
                catch
                {

                }
            }
        }

        public string C22SValueString
        {
            get
            {
                return this.ConstantBorders[1][2].ToString("e3");
            }
            set
            {
                try
                {
                    double NewC11LB = Convert.ToDouble(value);
                    this.ConstantBorders[1][2] = NewC11LB;
                }
                catch
                {

                }
            }
        }

        public string C33SValueString
        {
            get
            {
                return this.ConstantBorders[2][2].ToString("e3");
            }
            set
            {
                try
                {
                    double NewC11LB = Convert.ToDouble(value);
                    this.ConstantBorders[2][2] = NewC11LB;
                }
                catch
                {

                }
            }
        }

        public string C44SValueString
        {
            get
            {
                return this.ConstantBorders[3][2].ToString("e3");
            }
            set
            {
                try
                {
                    double NewC11LB = Convert.ToDouble(value);
                    this.ConstantBorders[3][2] = NewC11LB;
                }
                catch
                {

                }
            }
        }

        public string C55SValueString
        {
            get
            {
                return this.ConstantBorders[4][2].ToString("e3");
            }
            set
            {
                try
                {
                    double NewC11LB = Convert.ToDouble(value);
                    this.ConstantBorders[4][2] = NewC11LB;
                }
                catch
                {

                }
            }
        }

        public string C66SValueString
        {
            get
            {
                return this.ConstantBorders[5][2].ToString("e3");
            }
            set
            {
                try
                {
                    double NewC11LB = Convert.ToDouble(value);
                    this.ConstantBorders[5][2] = NewC11LB;
                }
                catch
                {

                }
            }
        }

        public string C12SValueString
        {
            get
            {
                return this.ConstantBorders[6][2].ToString("e3");
            }
            set
            {
                try
                {
                    double NewC11LB = Convert.ToDouble(value);
                    this.ConstantBorders[6][2] = NewC11LB;
                }
                catch
                {

                }
            }
        }

        public string C13SValueString
        {
            get
            {
                return this.ConstantBorders[7][2].ToString("e3");
            }
            set
            {
                try
                {
                    double NewC11LB = Convert.ToDouble(value);
                    this.ConstantBorders[7][2] = NewC11LB;
                }
                catch
                {

                }
            }
        }

        public string C23SValueString
        {
            get
            {
                return this.ConstantBorders[8][2].ToString("e3");
            }
            set
            {
                try
                {
                    double NewC11LB = Convert.ToDouble(value);
                    this.ConstantBorders[8][2] = NewC11LB;
                }
                catch
                {

                }
            }
        }

        public string C45SValueString
        {
            get
            {
                return this.ConstantBorders[9][2].ToString("e3");
            }
            set
            {
                try
                {
                    double NewC11LB = Convert.ToDouble(value);
                    this.ConstantBorders[9][2] = NewC11LB;
                }
                catch
                {

                }
            }
        }

        public string C16SValueString
        {
            get
            {
                return this.ConstantBorders[10][2].ToString("e3");
            }
            set
            {
                try
                {
                    double NewC11LB = Convert.ToDouble(value);
                    this.ConstantBorders[10][2] = NewC11LB;
                }
                catch
                {

                }
            }
        }

        public string C26SValueString
        {
            get
            {
                return this.ConstantBorders[11][2].ToString("e3");
            }
            set
            {
                try
                {
                    double NewC11LB = Convert.ToDouble(value);
                    this.ConstantBorders[11][2] = NewC11LB;
                }
                catch
                {

                }
            }
        }

        public string C36SValueString
        {
            get
            {
                return this.ConstantBorders[12][2].ToString("e3");
            }
            set
            {
                try
                {
                    double NewC11LB = Convert.ToDouble(value);
                    this.ConstantBorders[12][2] = NewC11LB;
                }
                catch
                {

                }
            }
        }

        #endregion

        #region Data

        #region Stiffness

        #region Total values

        public Pattern.Counts GetTotalValuesForC11(int Model)
        {
            Pattern.Counts Ret = new Pattern.Counts();

            for (int n = 0; n < this.TensorResults.Count; n++)
            {
                double[] Valuetmp = { this.TensorResults[n].C11, 0, 1 };
                switch (this.TensorResults[n].Symmetry)
                {
                    case "cubic":
                        switch (Model)
                        {
                            case 0:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicVoigtCubic(this.TensorResults[n]);
                                break;
                            case 1:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicReussCubic(this.TensorResults[n]);
                                break;
                            case 2:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicHillCubic(this.TensorResults[n]);
                                break;
                            case 3:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicKroenerCubicStiffness(this.TensorResults[n]);
                                break;
                            case 4:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicDeWittCubicStiffness(this.TensorResults[n]);
                                break;
                        }
                        break;
                    case "hexagonal":
                        switch (Model)
                        {
                            case 0:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicVoigtType1(this.TensorResults[n]);
                                break;
                            case 1:
                                break;
                            case 2:
                                break;
                            case 3:
                                break;
                            case 4:
                                break;
                        }
                        break;
                }
                if (!double.IsInfinity(Valuetmp[1]) && !double.IsNaN(Valuetmp[1]))
                {
                    Ret.Add(Valuetmp);
                }
            }

            return Ret;
        }

        public Pattern.Counts GetTotalValuesForC22(int Model)
        {
            Pattern.Counts Ret = new Pattern.Counts();

            for (int n = 0; n < this.TensorResults.Count; n++)
            {
                double[] Valuetmp = { this.TensorResults[n].C22, 0, 1 };
                switch (this.TensorResults[n].Symmetry)
                {
                    case "cubic":
                        switch (Model)
                        {
                            case 0:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicVoigtCubic(this.TensorResults[n]);
                                break;
                            case 1:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicReussCubic(this.TensorResults[n]);
                                break;
                            case 2:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicHillCubic(this.TensorResults[n]);
                                break;
                            case 3:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicKroenerCubicStiffness(this.TensorResults[n]);
                                break;
                            case 4:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicDeWittCubicStiffness(this.TensorResults[n]);
                                break;
                        }
                        break;
                    case "hexagonal":
                        switch (Model)
                        {
                            case 0:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicVoigtType1(this.TensorResults[n]);
                                break;
                            case 1:
                                break;
                            case 2:
                                break;
                            case 3:
                                break;
                            case 4:
                                break;
                        }
                        break;
                }
                if (!double.IsInfinity(Valuetmp[1]) && !double.IsNaN(Valuetmp[1]))
                {
                    Ret.Add(Valuetmp);
                }
            }

            return Ret;
        }

        public Pattern.Counts GetTotalValuesForC33(int Model)
        {
            Pattern.Counts Ret = new Pattern.Counts();

            for (int n = 0; n < this.TensorResults.Count; n++)
            {
                double[] Valuetmp = { this.TensorResults[n].C33, 0, 1 };
                switch (this.TensorResults[n].Symmetry)
                {
                    case "cubic":
                        switch (Model)
                        {
                            case 0:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicVoigtCubic(this.TensorResults[n]);
                                break;
                            case 1:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicReussCubic(this.TensorResults[n]);
                                break;
                            case 2:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicHillCubic(this.TensorResults[n]);
                                break;
                            case 3:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicKroenerCubicStiffness(this.TensorResults[n]);
                                break;
                            case 4:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicDeWittCubicStiffness(this.TensorResults[n]);
                                break;
                        }
                        break;
                    case "hexagonal":
                        switch (Model)
                        {
                            case 0:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicVoigtType1(this.TensorResults[n]);
                                break;
                            case 1:
                                break;
                            case 2:
                                break;
                            case 3:
                                break;
                            case 4:
                                break;
                        }
                        break;
                }
                if (!double.IsInfinity(Valuetmp[1]) && !double.IsNaN(Valuetmp[1]))
                {
                    Ret.Add(Valuetmp);
                }
            }

            return Ret;
        }

        public Pattern.Counts GetTotalValuesForC44(int Model)
        {
            Pattern.Counts Ret = new Pattern.Counts();

            for (int n = 0; n < this.TensorResults.Count; n++)
            {
                double[] Valuetmp = { this.TensorResults[n].C44, 0, 1 };
                switch (this.TensorResults[n].Symmetry)
                {
                    case "cubic":
                        switch (Model)
                        {
                            case 0:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicVoigtCubic(this.TensorResults[n]);
                                break;
                            case 1:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicReussCubic(this.TensorResults[n]);
                                break;
                            case 2:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicHillCubic(this.TensorResults[n]);
                                break;
                            case 3:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicKroenerCubicStiffness(this.TensorResults[n]);
                                break;
                            case 4:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicDeWittCubicStiffness(this.TensorResults[n]);
                                break;
                        }
                        break;
                    case "hexagonal":
                        switch (Model)
                        {
                            case 0:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicVoigtType1(this.TensorResults[n]);
                                break;
                            case 1:
                                break;
                            case 2:
                                break;
                            case 3:
                                break;
                            case 4:
                                break;
                        }
                        break;
                }
                if (!double.IsInfinity(Valuetmp[1]) && !double.IsNaN(Valuetmp[1]))
                {
                    Ret.Add(Valuetmp);
                }
            }

            return Ret;
        }

        public Pattern.Counts GetTotalValuesForC55(int Model)
        {
            Pattern.Counts Ret = new Pattern.Counts();

            for (int n = 0; n < this.TensorResults.Count; n++)
            {
                double[] Valuetmp = { this.TensorResults[n].C55, 0, 1 };
                switch (this.TensorResults[n].Symmetry)
                {
                    case "cubic":
                        switch (Model)
                        {
                            case 0:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicVoigtCubic(this.TensorResults[n]);
                                break;
                            case 1:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicReussCubic(this.TensorResults[n]);
                                break;
                            case 2:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicHillCubic(this.TensorResults[n]);
                                break;
                            case 3:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicKroenerCubicStiffness(this.TensorResults[n]);
                                break;
                            case 4:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicDeWittCubicStiffness(this.TensorResults[n]);
                                break;
                        }
                        break;
                    case "hexagonal":
                        switch (Model)
                        {
                            case 0:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicVoigtType1(this.TensorResults[n]);
                                break;
                            case 1:
                                break;
                            case 2:
                                break;
                            case 3:
                                break;
                            case 4:
                                break;
                        }
                        break;
                }
                if (!double.IsInfinity(Valuetmp[1]) && !double.IsNaN(Valuetmp[1]))
                {
                    Ret.Add(Valuetmp);
                }
            }

            return Ret;
        }

        public Pattern.Counts GetTotalValuesForC66(int Model)
        {
            Pattern.Counts Ret = new Pattern.Counts();

            for (int n = 0; n < this.TensorResults.Count; n++)
            {
                double[] Valuetmp = { this.TensorResults[n].C66, 0, 1 };
                switch (this.TensorResults[n].Symmetry)
                {
                    case "cubic":
                        switch (Model)
                        {
                            case 0:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicVoigtCubic(this.TensorResults[n]);
                                break;
                            case 1:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicReussCubic(this.TensorResults[n]);
                                break;
                            case 2:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicHillCubic(this.TensorResults[n]);
                                break;
                            case 3:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicKroenerCubicStiffness(this.TensorResults[n]);
                                break;
                            case 4:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicDeWittCubicStiffness(this.TensorResults[n]);
                                break;
                        }
                        break;
                    case "hexagonal":
                        switch (Model)
                        {
                            case 0:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicVoigtType1(this.TensorResults[n]);
                                break;
                            case 1:
                                break;
                            case 2:
                                break;
                            case 3:
                                break;
                            case 4:
                                break;
                        }
                        break;
                }
                if (!double.IsInfinity(Valuetmp[1]) && !double.IsNaN(Valuetmp[1]))
                {
                    Ret.Add(Valuetmp);
                }
            }

            return Ret;
        }

        public Pattern.Counts GetTotalValuesForC12(int Model)
        {
            Pattern.Counts Ret = new Pattern.Counts();

            for (int n = 0; n < this.TensorResults.Count; n++)
            {
                double[] Valuetmp = { this.TensorResults[n].C12, 0, 1 };
                switch (this.TensorResults[n].Symmetry)
                {
                    case "cubic":
                        switch (Model)
                        {
                            case 0:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicVoigtCubic(this.TensorResults[n]);
                                break;
                            case 1:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicReussCubic(this.TensorResults[n]);
                                break;
                            case 2:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicHillCubic(this.TensorResults[n]);
                                break;
                            case 3:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicKroenerCubicStiffness(this.TensorResults[n]);
                                break;
                            case 4:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicDeWittCubicStiffness(this.TensorResults[n]);
                                break;
                        }
                        break;
                    case "hexagonal":
                        switch (Model)
                        {
                            case 0:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicVoigtType1(this.TensorResults[n]);
                                break;
                            case 1:
                                break;
                            case 2:
                                break;
                            case 3:
                                break;
                            case 4:
                                break;
                        }
                        break;
                }
                if (!double.IsInfinity(Valuetmp[1]) && !double.IsNaN(Valuetmp[1]))
                {
                    Ret.Add(Valuetmp);
                }
            }

            return Ret;
        }

        public Pattern.Counts GetTotalValuesForC13(int Model)
        {
            Pattern.Counts Ret = new Pattern.Counts();

            for (int n = 0; n < this.TensorResults.Count; n++)
            {
                double[] Valuetmp = { this.TensorResults[n].C13, 0, 1 };
                switch (this.TensorResults[n].Symmetry)
                {
                    case "cubic":
                        switch (Model)
                        {
                            case 0:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicVoigtCubic(this.TensorResults[n]);
                                break;
                            case 1:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicReussCubic(this.TensorResults[n]);
                                break;
                            case 2:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicHillCubic(this.TensorResults[n]);
                                break;
                            case 3:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicKroenerCubicStiffness(this.TensorResults[n]);
                                break;
                            case 4:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicDeWittCubicStiffness(this.TensorResults[n]);
                                break;
                        }
                        break;
                    case "hexagonal":
                        switch (Model)
                        {
                            case 0:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicVoigtType1(this.TensorResults[n]);
                                break;
                            case 1:
                                break;
                            case 2:
                                break;
                            case 3:
                                break;
                            case 4:
                                break;
                        }
                        break;
                }
                if (!double.IsInfinity(Valuetmp[1]) && !double.IsNaN(Valuetmp[1]))
                {
                    Ret.Add(Valuetmp);
                }
            }

            return Ret;
        }

        public Pattern.Counts GetTotalValuesForC23(int Model)
        {
            Pattern.Counts Ret = new Pattern.Counts();

            for (int n = 0; n < this.TensorResults.Count; n++)
            {
                double[] Valuetmp = { this.TensorResults[n].C23, 0, 1 };
                switch (this.TensorResults[n].Symmetry)
                {
                    case "cubic":
                        switch (Model)
                        {
                            case 0:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicVoigtCubic(this.TensorResults[n]);
                                break;
                            case 1:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicReussCubic(this.TensorResults[n]);
                                break;
                            case 2:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicHillCubic(this.TensorResults[n]);
                                break;
                            case 3:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicKroenerCubicStiffness(this.TensorResults[n]);
                                break;
                            case 4:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicDeWittCubicStiffness(this.TensorResults[n]);
                                break;
                        }
                        break;
                    case "hexagonal":
                        switch (Model)
                        {
                            case 0:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicVoigtType1(this.TensorResults[n]);
                                break;
                            case 1:
                                break;
                            case 2:
                                break;
                            case 3:
                                break;
                            case 4:
                                break;
                        }
                        break;
                }
                if (!double.IsInfinity(Valuetmp[1]) && !double.IsNaN(Valuetmp[1]))
                {
                    Ret.Add(Valuetmp);
                }
            }

            return Ret;
        }

        public Pattern.Counts GetTotalValuesForC45(int Model)
        {
            Pattern.Counts Ret = new Pattern.Counts();

            for (int n = 0; n < this.TensorResults.Count; n++)
            {
                double[] Valuetmp = { this.TensorResults[n].C45, 0, 1 };
                switch (this.TensorResults[n].Symmetry)
                {
                    case "cubic":
                        switch (Model)
                        {
                            case 0:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicVoigtCubic(this.TensorResults[n]);
                                break;
                            case 1:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicReussCubic(this.TensorResults[n]);
                                break;
                            case 2:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicHillCubic(this.TensorResults[n]);
                                break;
                            case 3:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicKroenerCubicStiffness(this.TensorResults[n]);
                                break;
                            case 4:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicDeWittCubicStiffness(this.TensorResults[n]);
                                break;
                        }
                        break;
                    case "hexagonal":
                        switch (Model)
                        {
                            case 0:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicVoigtType1(this.TensorResults[n]);
                                break;
                            case 1:
                                break;
                            case 2:
                                break;
                            case 3:
                                break;
                            case 4:
                                break;
                        }
                        break;
                }
                if (!double.IsInfinity(Valuetmp[1]) && !double.IsNaN(Valuetmp[1]))
                {
                    Ret.Add(Valuetmp);
                }
            }

            return Ret;
        }

        public Pattern.Counts GetTotalValuesForC16(int Model)
        {
            Pattern.Counts Ret = new Pattern.Counts();

            for (int n = 0; n < this.TensorResults.Count; n++)
            {
                double[] Valuetmp = { this.TensorResults[n].C16, 0, 1 };
                switch (this.TensorResults[n].Symmetry)
                {
                    case "cubic":
                        switch (Model)
                        {
                            case 0:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicVoigtCubic(this.TensorResults[n]);
                                break;
                            case 1:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicReussCubic(this.TensorResults[n]);
                                break;
                            case 2:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicHillCubic(this.TensorResults[n]);
                                break;
                            case 3:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicKroenerCubicStiffness(this.TensorResults[n]);
                                break;
                            case 4:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicDeWittCubicStiffness(this.TensorResults[n]);
                                break;
                        }
                        break;
                    case "hexagonal":
                        switch (Model)
                        {
                            case 0:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicVoigtType1(this.TensorResults[n]);
                                break;
                            case 1:
                                break;
                            case 2:
                                break;
                            case 3:
                                break;
                            case 4:
                                break;
                        }
                        break;
                }
                if (!double.IsInfinity(Valuetmp[1]) && !double.IsNaN(Valuetmp[1]))
                {
                    Ret.Add(Valuetmp);
                }
            }

            return Ret;
        }

        public Pattern.Counts GetTotalValuesForC26(int Model)
        {
            Pattern.Counts Ret = new Pattern.Counts();

            for (int n = 0; n < this.TensorResults.Count; n++)
            {
                double[] Valuetmp = { this.TensorResults[n].C26, 0, 1 };
                switch (this.TensorResults[n].Symmetry)
                {
                    case "cubic":
                        switch (Model)
                        {
                            case 0:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicVoigtCubic(this.TensorResults[n]);
                                break;
                            case 1:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicReussCubic(this.TensorResults[n]);
                                break;
                            case 2:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicHillCubic(this.TensorResults[n]);
                                break;
                            case 3:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicKroenerCubicStiffness(this.TensorResults[n]);
                                break;
                            case 4:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicDeWittCubicStiffness(this.TensorResults[n]);
                                break;
                        }
                        break;
                    case "hexagonal":
                        switch (Model)
                        {
                            case 0:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicVoigtType1(this.TensorResults[n]);
                                break;
                            case 1:
                                break;
                            case 2:
                                break;
                            case 3:
                                break;
                            case 4:
                                break;
                        }
                        break;
                }
                if (!double.IsInfinity(Valuetmp[1]) && !double.IsNaN(Valuetmp[1]))
                {
                    Ret.Add(Valuetmp);
                }
            }

            return Ret;
        }

        public Pattern.Counts GetTotalValuesForC36(int Model)
        {
            Pattern.Counts Ret = new Pattern.Counts();

            for (int n = 0; n < this.TensorResults.Count; n++)
            {
                double[] Valuetmp = { this.TensorResults[n].C36, 0, 1 };
                switch (this.TensorResults[n].Symmetry)
                {
                    case "cubic":
                        switch (Model)
                        {
                            case 0:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicVoigtCubic(this.TensorResults[n]);
                                break;
                            case 1:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicReussCubic(this.TensorResults[n]);
                                break;
                            case 2:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicHillCubic(this.TensorResults[n]);
                                break;
                            case 3:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicKroenerCubicStiffness(this.TensorResults[n]);
                                break;
                            case 4:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicDeWittCubicStiffness(this.TensorResults[n]);
                                break;
                        }
                        break;
                    case "hexagonal":
                        switch (Model)
                        {
                            case 0:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicVoigtType1(this.TensorResults[n]);
                                break;
                            case 1:
                                break;
                            case 2:
                                break;
                            case 3:
                                break;
                            case 4:
                                break;
                        }
                        break;
                }
                if (!double.IsInfinity(Valuetmp[1]) && !double.IsNaN(Valuetmp[1]))
                {
                    Ret.Add(Valuetmp);
                }
            }

            return Ret;
        }

        #endregion

        #region Probability distributions

        public Pattern.Counts GetProbabilityDistributionForC11(int Model, double IntegrationDistance)
        {
            Pattern.Counts Ret = new Pattern.Counts();
            Pattern.Counts AbsoluteV = this.GetTotalValuesForC11(Model);

            for(double Intervall = this.ConstantBorders[0][0]; Intervall < this.ConstantBorders[0][1]; Intervall += IntegrationDistance)
            {
                double IntervallUpperLimit = Intervall + IntegrationDistance;
                double TotalRes = 0.0;
                int ResCount = 0;
                int ValCount = 0;

                for(int n = 0; n < AbsoluteV.Count; n++)
                {
                    if (AbsoluteV[n][0] > Intervall && AbsoluteV[n][0] < IntervallUpperLimit)
                    {
                        TotalRes += AbsoluteV[n][1];
                        ResCount++;
                        ValCount++;
                    }
                }

                double[] RetValueTmp = { Intervall + (IntegrationDistance / 2.0), ValCount, TotalRes / ResCount };
                Ret.Add(RetValueTmp);
            }

            return Ret;
        }

        public Pattern.Counts GetProbabilityDistributionForC22(int Model, double IntegrationDistance)
        {
            Pattern.Counts Ret = new Pattern.Counts();
            Pattern.Counts AbsoluteV = this.GetTotalValuesForC22(Model);

            for (double Intervall = this.ConstantBorders[1][0]; Intervall < this.ConstantBorders[1][1]; Intervall += IntegrationDistance)
            {
                double IntervallUpperLimit = Intervall + IntegrationDistance;
                double TotalRes = 0.0;
                int ResCount = 0;
                int ValCount = 0;

                for (int n = 0; n < AbsoluteV.Count; n++)
                {
                    if (AbsoluteV[n][0] > Intervall && AbsoluteV[n][0] < IntervallUpperLimit)
                    {
                        TotalRes += AbsoluteV[n][1];
                        ResCount++;
                        ValCount++;
                    }
                }

                double[] RetValueTmp = { Intervall + (IntegrationDistance / 2.0), ValCount, TotalRes / ResCount };
                Ret.Add(RetValueTmp);
            }

            return Ret;
        }

        public Pattern.Counts GetProbabilityDistributionForC33(int Model, double IntegrationDistance)
        {
            Pattern.Counts Ret = new Pattern.Counts();
            Pattern.Counts AbsoluteV = this.GetTotalValuesForC33(Model);

            for (double Intervall = this.ConstantBorders[2][0]; Intervall < this.ConstantBorders[2][1]; Intervall += IntegrationDistance)
            {
                double IntervallUpperLimit = Intervall + IntegrationDistance;
                double TotalRes = 0.0;
                int ResCount = 0;
                int ValCount = 0;

                for (int n = 0; n < AbsoluteV.Count; n++)
                {
                    if (AbsoluteV[n][0] > Intervall && AbsoluteV[n][0] < IntervallUpperLimit)
                    {
                        TotalRes += AbsoluteV[n][1];
                        ResCount++;
                        ValCount++;
                    }
                }

                double[] RetValueTmp = { Intervall + (IntegrationDistance / 2.0), ValCount, TotalRes / ResCount };
                Ret.Add(RetValueTmp);
            }

            return Ret;
        }

        public Pattern.Counts GetProbabilityDistributionForC44(int Model, double IntegrationDistance)
        {
            Pattern.Counts Ret = new Pattern.Counts();
            Pattern.Counts AbsoluteV = this.GetTotalValuesForC44(Model);

            for (double Intervall = this.ConstantBorders[3][0]; Intervall < this.ConstantBorders[3][1]; Intervall += IntegrationDistance)
            {
                double IntervallUpperLimit = Intervall + IntegrationDistance;
                double TotalRes = 0.0;
                int ResCount = 0;
                int ValCount = 0;

                for (int n = 0; n < AbsoluteV.Count; n++)
                {
                    if (AbsoluteV[n][0] > Intervall && AbsoluteV[n][0] < IntervallUpperLimit)
                    {
                        TotalRes += AbsoluteV[n][1];
                        ResCount++;
                        ValCount++;
                    }
                }

                double[] RetValueTmp = { Intervall + (IntegrationDistance / 2.0), ValCount, TotalRes / ResCount };
                Ret.Add(RetValueTmp);
            }

            return Ret;
        }

        public Pattern.Counts GetProbabilityDistributionForC55(int Model, double IntegrationDistance)
        {
            Pattern.Counts Ret = new Pattern.Counts();
            Pattern.Counts AbsoluteV = this.GetTotalValuesForC55(Model);

            for (double Intervall = this.ConstantBorders[4][0]; Intervall < this.ConstantBorders[4][1]; Intervall += IntegrationDistance)
            {
                double IntervallUpperLimit = Intervall + IntegrationDistance;
                double TotalRes = 0.0;
                int ResCount = 0;
                int ValCount = 0;

                for (int n = 0; n < AbsoluteV.Count; n++)
                {
                    if (AbsoluteV[n][0] > Intervall && AbsoluteV[n][0] < IntervallUpperLimit)
                    {
                        TotalRes += AbsoluteV[n][1];
                        ResCount++;
                        ValCount++;
                    }
                }

                double[] RetValueTmp = { Intervall + (IntegrationDistance / 2.0), ValCount, TotalRes / ResCount };
                Ret.Add(RetValueTmp);
            }

            return Ret;
        }

        public Pattern.Counts GetProbabilityDistributionForC66(int Model, double IntegrationDistance)
        {
            Pattern.Counts Ret = new Pattern.Counts();
            Pattern.Counts AbsoluteV = this.GetTotalValuesForC66(Model);

            for (double Intervall = this.ConstantBorders[5][0]; Intervall < this.ConstantBorders[5][1]; Intervall += IntegrationDistance)
            {
                double IntervallUpperLimit = Intervall + IntegrationDistance;
                double TotalRes = 0.0;
                int ResCount = 0;
                int ValCount = 0;

                for (int n = 0; n < AbsoluteV.Count; n++)
                {
                    if (AbsoluteV[n][0] > Intervall && AbsoluteV[n][0] < IntervallUpperLimit)
                    {
                        TotalRes += AbsoluteV[n][1];
                        ResCount++;
                        ValCount++;
                    }
                }

                double[] RetValueTmp = { Intervall + (IntegrationDistance / 2.0), ValCount, TotalRes / ResCount };
                Ret.Add(RetValueTmp);
            }

            return Ret;
        }

        public Pattern.Counts GetProbabilityDistributionForC12(int Model, double IntegrationDistance)
        {
            Pattern.Counts Ret = new Pattern.Counts();
            Pattern.Counts AbsoluteV = this.GetTotalValuesForC12(Model);

            for (double Intervall = this.ConstantBorders[6][0]; Intervall < this.ConstantBorders[6][1]; Intervall += IntegrationDistance)
            {
                double IntervallUpperLimit = Intervall + IntegrationDistance;
                double TotalRes = 0.0;
                int ResCount = 0;
                int ValCount = 0;

                for (int n = 0; n < AbsoluteV.Count; n++)
                {
                    if (AbsoluteV[n][0] > Intervall && AbsoluteV[n][0] < IntervallUpperLimit)
                    {
                        TotalRes += AbsoluteV[n][1];
                        ResCount++;
                        ValCount++;
                    }
                }

                double[] RetValueTmp = { Intervall + (IntegrationDistance / 2.0), ValCount, TotalRes / ResCount };
                Ret.Add(RetValueTmp);
            }

            return Ret;
        }

        public Pattern.Counts GetProbabilityDistributionForC13(int Model, double IntegrationDistance)
        {
            Pattern.Counts Ret = new Pattern.Counts();
            Pattern.Counts AbsoluteV = this.GetTotalValuesForC13(Model);

            for (double Intervall = this.ConstantBorders[7][0]; Intervall < this.ConstantBorders[7][1]; Intervall += IntegrationDistance)
            {
                double IntervallUpperLimit = Intervall + IntegrationDistance;
                double TotalRes = 0.0;
                int ResCount = 0;
                int ValCount = 0;

                for (int n = 0; n < AbsoluteV.Count; n++)
                {
                    if (AbsoluteV[n][0] > Intervall && AbsoluteV[n][0] < IntervallUpperLimit)
                    {
                        TotalRes += AbsoluteV[n][1];
                        ResCount++;
                        ValCount++;
                    }
                }

                double[] RetValueTmp = { Intervall + (IntegrationDistance / 2.0), ValCount, TotalRes / ResCount };
                Ret.Add(RetValueTmp);
            }

            return Ret;
        }

        public Pattern.Counts GetProbabilityDistributionForC23(int Model, double IntegrationDistance)
        {
            Pattern.Counts Ret = new Pattern.Counts();
            Pattern.Counts AbsoluteV = this.GetTotalValuesForC23(Model);

            for (double Intervall = this.ConstantBorders[8][0]; Intervall < this.ConstantBorders[8][1]; Intervall += IntegrationDistance)
            {
                double IntervallUpperLimit = Intervall + IntegrationDistance;
                double TotalRes = 0.0;
                int ResCount = 0;
                int ValCount = 0;

                for (int n = 0; n < AbsoluteV.Count; n++)
                {
                    if (AbsoluteV[n][0] > Intervall && AbsoluteV[n][0] < IntervallUpperLimit)
                    {
                        TotalRes += AbsoluteV[n][1];
                        ResCount++;
                        ValCount++;
                    }
                }

                double[] RetValueTmp = { Intervall + (IntegrationDistance / 2.0), ValCount, TotalRes / ResCount };
                Ret.Add(RetValueTmp);
            }

            return Ret;
        }

        public Pattern.Counts GetProbabilityDistributionForC45(int Model, double IntegrationDistance)
        {
            Pattern.Counts Ret = new Pattern.Counts();
            Pattern.Counts AbsoluteV = this.GetTotalValuesForC45(Model);

            for (double Intervall = this.ConstantBorders[9][0]; Intervall < this.ConstantBorders[9][1]; Intervall += IntegrationDistance)
            {
                double IntervallUpperLimit = Intervall + IntegrationDistance;
                double TotalRes = 0.0;
                int ResCount = 0;
                int ValCount = 0;

                for (int n = 0; n < AbsoluteV.Count; n++)
                {
                    if (AbsoluteV[n][0] > Intervall && AbsoluteV[n][0] < IntervallUpperLimit)
                    {
                        TotalRes += AbsoluteV[n][1];
                        ResCount++;
                        ValCount++;
                    }
                }

                double[] RetValueTmp = { Intervall + (IntegrationDistance / 2.0), ValCount, TotalRes / ResCount };
                Ret.Add(RetValueTmp);
            }

            return Ret;
        }

        public Pattern.Counts GetProbabilityDistributionForC16(int Model, double IntegrationDistance)
        {
            Pattern.Counts Ret = new Pattern.Counts();
            Pattern.Counts AbsoluteV = this.GetTotalValuesForC16(Model);

            for (double Intervall = this.ConstantBorders[10][0]; Intervall < this.ConstantBorders[10][1]; Intervall += IntegrationDistance)
            {
                double IntervallUpperLimit = Intervall + IntegrationDistance;
                double TotalRes = 0.0;
                int ResCount = 0;
                int ValCount = 0;

                for (int n = 0; n < AbsoluteV.Count; n++)
                {
                    if (AbsoluteV[n][0] > Intervall && AbsoluteV[n][0] < IntervallUpperLimit)
                    {
                        TotalRes += AbsoluteV[n][1];
                        ResCount++;
                        ValCount++;
                    }
                }

                double[] RetValueTmp = { Intervall + (IntegrationDistance / 2.0), ValCount, TotalRes / ResCount };
                Ret.Add(RetValueTmp);
            }

            return Ret;
        }

        public Pattern.Counts GetProbabilityDistributionForC26(int Model, double IntegrationDistance)
        {
            Pattern.Counts Ret = new Pattern.Counts();
            Pattern.Counts AbsoluteV = this.GetTotalValuesForC26(Model);

            for (double Intervall = this.ConstantBorders[11][0]; Intervall < this.ConstantBorders[11][1]; Intervall += IntegrationDistance)
            {
                double IntervallUpperLimit = Intervall + IntegrationDistance;
                double TotalRes = 0.0;
                int ResCount = 0;
                int ValCount = 0;

                for (int n = 0; n < AbsoluteV.Count; n++)
                {
                    if (AbsoluteV[n][0] > Intervall && AbsoluteV[n][0] < IntervallUpperLimit)
                    {
                        TotalRes += AbsoluteV[n][1];
                        ResCount++;
                        ValCount++;
                    }
                }

                double[] RetValueTmp = { Intervall + (IntegrationDistance / 2.0), ValCount, TotalRes / ResCount };
                Ret.Add(RetValueTmp);
            }

            return Ret;
        }

        public Pattern.Counts GetProbabilityDistributionForC36(int Model, double IntegrationDistance)
        {
            Pattern.Counts Ret = new Pattern.Counts();
            Pattern.Counts AbsoluteV = this.GetTotalValuesForC36(Model);

            for (double Intervall = this.ConstantBorders[12][0]; Intervall < this.ConstantBorders[12][1]; Intervall += IntegrationDistance)
            {
                double IntervallUpperLimit = Intervall + IntegrationDistance;
                double TotalRes = 0.0;
                int ResCount = 0;
                int ValCount = 0;

                for (int n = 0; n < AbsoluteV.Count; n++)
                {
                    if (AbsoluteV[n][0] > Intervall && AbsoluteV[n][0] < IntervallUpperLimit)
                    {
                        TotalRes += AbsoluteV[n][1];
                        ResCount++;
                        ValCount++;
                    }
                }

                double[] RetValueTmp = { Intervall + (IntegrationDistance / 2.0), ValCount, TotalRes / ResCount };
                Ret.Add(RetValueTmp);
            }

            return Ret;
        }

        #endregion

        #endregion

        #region Compliance

        #region Total values

        public Pattern.Counts GetTotalValuesForS11(int Model)
        {
            Pattern.Counts Ret = new Pattern.Counts();

            for (int n = 0; n < this.TensorResults.Count; n++)
            {
                double[] Valuetmp = { this.TensorResults[n].S11, 0, 1 };
                switch (this.TensorResults[n].Symmetry)
                {
                    case "Cubic":
                        switch (Model)
                        {
                            case 0:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicVoigtCubic(this.TensorResults[n]);
                                break;
                            case 1:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicReussCubic(this.TensorResults[n]);
                                break;
                            case 2:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicHillCubic(this.TensorResults[n]);
                                break;
                            case 3:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicKroenerCubicStiffness(this.TensorResults[n]);
                                break;
                            case 4:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicDeWittCubicStiffness(this.TensorResults[n]);
                                break;
                        }
                        break;
                    case "Hexagonal":
                        switch (Model)
                        {
                            case 0:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicVoigtType1(this.TensorResults[n]);
                                break;
                            case 1:
                                break;
                            case 2:
                                break;
                            case 3:
                                break;
                            case 4:
                                break;
                        }
                        break;
                }
                if (!double.IsInfinity(Valuetmp[1]) && !double.IsNaN(Valuetmp[1]))
                {
                    Ret.Add(Valuetmp);
                }
            }

            return Ret;
        }

        public Pattern.Counts GetTotalValuesForS22(int Model)
        {
            Pattern.Counts Ret = new Pattern.Counts();

            for (int n = 0; n < this.TensorResults.Count; n++)
            {
                double[] Valuetmp = { this.TensorResults[n].S22, 0, 1 };
                switch (this.TensorResults[n].Symmetry)
                {
                    case "Cubic":
                        switch (Model)
                        {
                            case 0:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicVoigtCubic(this.TensorResults[n]);
                                break;
                            case 1:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicReussCubic(this.TensorResults[n]);
                                break;
                            case 2:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicHillCubic(this.TensorResults[n]);
                                break;
                            case 3:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicKroenerCubicStiffness(this.TensorResults[n]);
                                break;
                            case 4:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicDeWittCubicStiffness(this.TensorResults[n]);
                                break;
                        }
                        break;
                    case "Hexagonal":
                        switch (Model)
                        {
                            case 0:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicVoigtType1(this.TensorResults[n]);
                                break;
                            case 1:
                                break;
                            case 2:
                                break;
                            case 3:
                                break;
                            case 4:
                                break;
                        }
                        break;
                }
                if (!double.IsInfinity(Valuetmp[1]) && !double.IsNaN(Valuetmp[1]))
                {
                    Ret.Add(Valuetmp);
                }
            }

            return Ret;
        }

        public Pattern.Counts GetTotalValuesForS33(int Model)
        {
            Pattern.Counts Ret = new Pattern.Counts();

            for (int n = 0; n < this.TensorResults.Count; n++)
            {
                double[] Valuetmp = { this.TensorResults[n].S33, 0, 1 };
                switch (this.TensorResults[n].Symmetry)
                {
                    case "Cubic":
                        switch (Model)
                        {
                            case 0:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicVoigtCubic(this.TensorResults[n]);
                                break;
                            case 1:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicReussCubic(this.TensorResults[n]);
                                break;
                            case 2:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicHillCubic(this.TensorResults[n]);
                                break;
                            case 3:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicKroenerCubicStiffness(this.TensorResults[n]);
                                break;
                            case 4:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicDeWittCubicStiffness(this.TensorResults[n]);
                                break;
                        }
                        break;
                    case "Hexagonal":
                        switch (Model)
                        {
                            case 0:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicVoigtType1(this.TensorResults[n]);
                                break;
                            case 1:
                                break;
                            case 2:
                                break;
                            case 3:
                                break;
                            case 4:
                                break;
                        }
                        break;
                }
                if (!double.IsInfinity(Valuetmp[1]) && !double.IsNaN(Valuetmp[1]))
                {
                    Ret.Add(Valuetmp);
                }
            }

            return Ret;
        }

        public Pattern.Counts GetTotalValuesForS44(int Model)
        {
            Pattern.Counts Ret = new Pattern.Counts();

            for (int n = 0; n < this.TensorResults.Count; n++)
            {
                double[] Valuetmp = { this.TensorResults[n].S44, 0, 1 };
                switch (this.TensorResults[n].Symmetry)
                {
                    case "Cubic":
                        switch (Model)
                        {
                            case 0:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicVoigtCubic(this.TensorResults[n]);
                                break;
                            case 1:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicReussCubic(this.TensorResults[n]);
                                break;
                            case 2:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicHillCubic(this.TensorResults[n]);
                                break;
                            case 3:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicKroenerCubicStiffness(this.TensorResults[n]);
                                break;
                            case 4:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicDeWittCubicStiffness(this.TensorResults[n]);
                                break;
                        }
                        break;
                    case "Hexagonal":
                        switch (Model)
                        {
                            case 0:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicVoigtType1(this.TensorResults[n]);
                                break;
                            case 1:
                                break;
                            case 2:
                                break;
                            case 3:
                                break;
                            case 4:
                                break;
                        }
                        break;
                }
                if (!double.IsInfinity(Valuetmp[1]) && !double.IsNaN(Valuetmp[1]))
                {
                    Ret.Add(Valuetmp);
                }
            }

            return Ret;
        }

        public Pattern.Counts GetTotalValuesForS55(int Model)
        {
            Pattern.Counts Ret = new Pattern.Counts();

            for (int n = 0; n < this.TensorResults.Count; n++)
            {
                double[] Valuetmp = { this.TensorResults[n].S55, 0, 1 };
                switch (this.TensorResults[n].Symmetry)
                {
                    case "Cubic":
                        switch (Model)
                        {
                            case 0:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicVoigtCubic(this.TensorResults[n]);
                                break;
                            case 1:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicReussCubic(this.TensorResults[n]);
                                break;
                            case 2:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicHillCubic(this.TensorResults[n]);
                                break;
                            case 3:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicKroenerCubicStiffness(this.TensorResults[n]);
                                break;
                            case 4:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicDeWittCubicStiffness(this.TensorResults[n]);
                                break;
                        }
                        break;
                    case "Hexagonal":
                        switch (Model)
                        {
                            case 0:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicVoigtType1(this.TensorResults[n]);
                                break;
                            case 1:
                                break;
                            case 2:
                                break;
                            case 3:
                                break;
                            case 4:
                                break;
                        }
                        break;
                }
                if (!double.IsInfinity(Valuetmp[1]) && !double.IsNaN(Valuetmp[1]))
                {
                    Ret.Add(Valuetmp);
                }
            }

            return Ret;
        }

        public Pattern.Counts GetTotalValuesForS66(int Model)
        {
            Pattern.Counts Ret = new Pattern.Counts();

            for (int n = 0; n < this.TensorResults.Count; n++)
            {
                double[] Valuetmp = { this.TensorResults[n].S66, 0, 1 };
                switch (this.TensorResults[n].Symmetry)
                {
                    case "Cubic":
                        switch (Model)
                        {
                            case 0:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicVoigtCubic(this.TensorResults[n]);
                                break;
                            case 1:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicReussCubic(this.TensorResults[n]);
                                break;
                            case 2:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicHillCubic(this.TensorResults[n]);
                                break;
                            case 3:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicKroenerCubicStiffness(this.TensorResults[n]);
                                break;
                            case 4:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicDeWittCubicStiffness(this.TensorResults[n]);
                                break;
                        }
                        break;
                    case "Hexagonal":
                        switch (Model)
                        {
                            case 0:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicVoigtType1(this.TensorResults[n]);
                                break;
                            case 1:
                                break;
                            case 2:
                                break;
                            case 3:
                                break;
                            case 4:
                                break;
                        }
                        break;
                }
                if (!double.IsInfinity(Valuetmp[1]) && !double.IsNaN(Valuetmp[1]))
                {
                    Ret.Add(Valuetmp);
                }
            }

            return Ret;
        }

        public Pattern.Counts GetTotalValuesForS12(int Model)
        {
            Pattern.Counts Ret = new Pattern.Counts();

            for (int n = 0; n < this.TensorResults.Count; n++)
            {
                double[] Valuetmp = { this.TensorResults[n].S12, 0, 1 };
                switch (this.TensorResults[n].Symmetry)
                {
                    case "Cubic":
                        switch (Model)
                        {
                            case 0:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicVoigtCubic(this.TensorResults[n]);
                                break;
                            case 1:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicReussCubic(this.TensorResults[n]);
                                break;
                            case 2:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicHillCubic(this.TensorResults[n]);
                                break;
                            case 3:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicKroenerCubicStiffness(this.TensorResults[n]);
                                break;
                            case 4:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicDeWittCubicStiffness(this.TensorResults[n]);
                                break;
                        }
                        break;
                    case "Hexagonal":
                        switch (Model)
                        {
                            case 0:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicVoigtType1(this.TensorResults[n]);
                                break;
                            case 1:
                                break;
                            case 2:
                                break;
                            case 3:
                                break;
                            case 4:
                                break;
                        }
                        break;
                }
                if (!double.IsInfinity(Valuetmp[1]) && !double.IsNaN(Valuetmp[1]))
                {
                    Ret.Add(Valuetmp);
                }
            }

            return Ret;
        }

        public Pattern.Counts GetTotalValuesForS13(int Model)
        {
            Pattern.Counts Ret = new Pattern.Counts();

            for (int n = 0; n < this.TensorResults.Count; n++)
            {
                double[] Valuetmp = { this.TensorResults[n].S13, 0, 1 };
                switch (this.TensorResults[n].Symmetry)
                {
                    case "Cubic":
                        switch (Model)
                        {
                            case 0:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicVoigtCubic(this.TensorResults[n]);
                                break;
                            case 1:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicReussCubic(this.TensorResults[n]);
                                break;
                            case 2:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicHillCubic(this.TensorResults[n]);
                                break;
                            case 3:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicKroenerCubicStiffness(this.TensorResults[n]);
                                break;
                            case 4:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicDeWittCubicStiffness(this.TensorResults[n]);
                                break;
                        }
                        break;
                    case "Hexagonal":
                        switch (Model)
                        {
                            case 0:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicVoigtType1(this.TensorResults[n]);
                                break;
                            case 1:
                                break;
                            case 2:
                                break;
                            case 3:
                                break;
                            case 4:
                                break;
                        }
                        break;
                }
                if (!double.IsInfinity(Valuetmp[1]) && !double.IsNaN(Valuetmp[1]))
                {
                    Ret.Add(Valuetmp);
                }
            }

            return Ret;
        }

        public Pattern.Counts GetTotalValuesForS23(int Model)
        {
            Pattern.Counts Ret = new Pattern.Counts();

            for (int n = 0; n < this.TensorResults.Count; n++)
            {
                double[] Valuetmp = { this.TensorResults[n].S23, 0, 1 };
                switch (this.TensorResults[n].Symmetry)
                {
                    case "Cubic":
                        switch (Model)
                        {
                            case 0:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicVoigtCubic(this.TensorResults[n]);
                                break;
                            case 1:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicReussCubic(this.TensorResults[n]);
                                break;
                            case 2:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicHillCubic(this.TensorResults[n]);
                                break;
                            case 3:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicKroenerCubicStiffness(this.TensorResults[n]);
                                break;
                            case 4:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicDeWittCubicStiffness(this.TensorResults[n]);
                                break;
                        }
                        break;
                    case "Hexagonal":
                        switch (Model)
                        {
                            case 0:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicVoigtType1(this.TensorResults[n]);
                                break;
                            case 1:
                                break;
                            case 2:
                                break;
                            case 3:
                                break;
                            case 4:
                                break;
                        }
                        break;
                }
                if (!double.IsInfinity(Valuetmp[1]) && !double.IsNaN(Valuetmp[1]))
                {
                    Ret.Add(Valuetmp);
                }
            }

            return Ret;
        }

        public Pattern.Counts GetTotalValuesForS45(int Model)
        {
            Pattern.Counts Ret = new Pattern.Counts();

            for (int n = 0; n < this.TensorResults.Count; n++)
            {
                double[] Valuetmp = { this.TensorResults[n].S45, 0, 1 };
                switch (this.TensorResults[n].Symmetry)
                {
                    case "Cubic":
                        switch (Model)
                        {
                            case 0:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicVoigtCubic(this.TensorResults[n]);
                                break;
                            case 1:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicReussCubic(this.TensorResults[n]);
                                break;
                            case 2:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicHillCubic(this.TensorResults[n]);
                                break;
                            case 3:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicKroenerCubicStiffness(this.TensorResults[n]);
                                break;
                            case 4:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicDeWittCubicStiffness(this.TensorResults[n]);
                                break;
                        }
                        break;
                    case "Hexagonal":
                        switch (Model)
                        {
                            case 0:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicVoigtType1(this.TensorResults[n]);
                                break;
                            case 1:
                                break;
                            case 2:
                                break;
                            case 3:
                                break;
                            case 4:
                                break;
                        }
                        break;
                }
                if (!double.IsInfinity(Valuetmp[1]) && !double.IsNaN(Valuetmp[1]))
                {
                    Ret.Add(Valuetmp);
                }
            }

            return Ret;
        }

        public Pattern.Counts GetTotalValuesForS16(int Model)
        {
            Pattern.Counts Ret = new Pattern.Counts();

            for (int n = 0; n < this.TensorResults.Count; n++)
            {
                double[] Valuetmp = { this.TensorResults[n].S16, 0, 1 };
                switch (this.TensorResults[n].Symmetry)
                {
                    case "Cubic":
                        switch (Model)
                        {
                            case 0:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicVoigtCubic(this.TensorResults[n]);
                                break;
                            case 1:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicReussCubic(this.TensorResults[n]);
                                break;
                            case 2:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicHillCubic(this.TensorResults[n]);
                                break;
                            case 3:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicKroenerCubicStiffness(this.TensorResults[n]);
                                break;
                            case 4:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicDeWittCubicStiffness(this.TensorResults[n]);
                                break;
                        }
                        break;
                    case "Hexagonal":
                        switch (Model)
                        {
                            case 0:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicVoigtType1(this.TensorResults[n]);
                                break;
                            case 1:
                                break;
                            case 2:
                                break;
                            case 3:
                                break;
                            case 4:
                                break;
                        }
                        break;
                }
                if (!double.IsInfinity(Valuetmp[1]) && !double.IsNaN(Valuetmp[1]))
                {
                    Ret.Add(Valuetmp);
                }
            }

            return Ret;
        }

        public Pattern.Counts GetTotalValuesForS26(int Model)
        {
            Pattern.Counts Ret = new Pattern.Counts();

            for (int n = 0; n < this.TensorResults.Count; n++)
            {
                double[] Valuetmp = { this.TensorResults[n].S26, 0, 1 };
                switch (this.TensorResults[n].Symmetry)
                {
                    case "Cubic":
                        switch (Model)
                        {
                            case 0:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicVoigtCubic(this.TensorResults[n]);
                                break;
                            case 1:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicReussCubic(this.TensorResults[n]);
                                break;
                            case 2:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicHillCubic(this.TensorResults[n]);
                                break;
                            case 3:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicKroenerCubicStiffness(this.TensorResults[n]);
                                break;
                            case 4:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicDeWittCubicStiffness(this.TensorResults[n]);
                                break;
                        }
                        break;
                    case "Hexagonal":
                        switch (Model)
                        {
                            case 0:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicVoigtType1(this.TensorResults[n]);
                                break;
                            case 1:
                                break;
                            case 2:
                                break;
                            case 3:
                                break;
                            case 4:
                                break;
                        }
                        break;
                }
                if (!double.IsInfinity(Valuetmp[1]) && !double.IsNaN(Valuetmp[1]))
                {
                    Ret.Add(Valuetmp);
                }
            }

            return Ret;
        }

        public Pattern.Counts GetTotalValuesForS36(int Model)
        {
            Pattern.Counts Ret = new Pattern.Counts();

            for (int n = 0; n < this.TensorResults.Count; n++)
            {
                double[] Valuetmp = { this.TensorResults[n].S36, 0, 1 };
                switch (this.TensorResults[n].Symmetry)
                {
                    case "Cubic":
                        switch (Model)
                        {
                            case 0:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicVoigtCubic(this.TensorResults[n]);
                                break;
                            case 1:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicReussCubic(this.TensorResults[n]);
                                break;
                            case 2:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicHillCubic(this.TensorResults[n]);
                                break;
                            case 3:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicKroenerCubicStiffness(this.TensorResults[n]);
                                break;
                            case 4:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicDeWittCubicStiffness(this.TensorResults[n]);
                                break;
                        }
                        break;
                    case "Hexagonal":
                        switch (Model)
                        {
                            case 0:
                                Valuetmp[1] = Fitting.Chi2.Chi2ClassicVoigtType1(this.TensorResults[n]);
                                break;
                            case 1:
                                break;
                            case 2:
                                break;
                            case 3:
                                break;
                            case 4:
                                break;
                        }
                        break;
                }
                if (!double.IsInfinity(Valuetmp[1]) && !double.IsNaN(Valuetmp[1]))
                {
                    Ret.Add(Valuetmp);
                }
            }

            return Ret;
        }

        #endregion

        #region Probability distributions

        public Pattern.Counts GetProbabilityDistributionForS11(int Model, double IntegrationDistance)
        {
            Pattern.Counts Ret = new Pattern.Counts();
            Pattern.Counts AbsoluteV = this.GetTotalValuesForS11(Model);

            for (double Intervall = 0.0; Intervall < 500000.0; Intervall += IntegrationDistance)
            {
                double IntervallUpperLimit = Intervall + IntegrationDistance;
                double TotalRes = 0.0;
                int ResCount = 0;
                int ValCount = 0;

                for (int n = 0; n < AbsoluteV.Count; n++)
                {
                    if (AbsoluteV[n][0] > Intervall && AbsoluteV[n][0] < IntervallUpperLimit)
                    {
                        TotalRes += AbsoluteV[n][1];
                        ResCount++;
                        ValCount++;
                    }
                }

                double[] RetValueTmp = { Intervall + (IntegrationDistance / 2.0), ValCount, TotalRes / ResCount };
                Ret.Add(RetValueTmp);
            }

            return Ret;
        }

        public Pattern.Counts GetProbabilityDistributionForS22(int Model, double IntegrationDistance)
        {
            Pattern.Counts Ret = new Pattern.Counts();
            Pattern.Counts AbsoluteV = this.GetTotalValuesForS22(Model);

            for (double Intervall = 0.0; Intervall < 500000.0; Intervall += IntegrationDistance)
            {
                double IntervallUpperLimit = Intervall + IntegrationDistance;
                double TotalRes = 0.0;
                int ResCount = 0;
                int ValCount = 0;

                for (int n = 0; n < AbsoluteV.Count; n++)
                {
                    if (AbsoluteV[n][0] > Intervall && AbsoluteV[n][0] < IntervallUpperLimit)
                    {
                        TotalRes += AbsoluteV[n][1];
                        ResCount++;
                        ValCount++;
                    }
                }

                double[] RetValueTmp = { Intervall + (IntegrationDistance / 2.0), ValCount, TotalRes / ResCount };
                Ret.Add(RetValueTmp);
            }

            return Ret;
        }

        public Pattern.Counts GetProbabilityDistributionForS33(int Model, double IntegrationDistance)
        {
            Pattern.Counts Ret = new Pattern.Counts();
            Pattern.Counts AbsoluteV = this.GetTotalValuesForS33(Model);

            for (double Intervall = 0.0; Intervall < 500000.0; Intervall += IntegrationDistance)
            {
                double IntervallUpperLimit = Intervall + IntegrationDistance;
                double TotalRes = 0.0;
                int ResCount = 0;
                int ValCount = 0;

                for (int n = 0; n < AbsoluteV.Count; n++)
                {
                    if (AbsoluteV[n][0] > Intervall && AbsoluteV[n][0] < IntervallUpperLimit)
                    {
                        TotalRes += AbsoluteV[n][1];
                        ResCount++;
                        ValCount++;
                    }
                }

                double[] RetValueTmp = { Intervall + (IntegrationDistance / 2.0), ValCount, TotalRes / ResCount };
                Ret.Add(RetValueTmp);
            }

            return Ret;
        }

        public Pattern.Counts GetProbabilityDistributionForS44(int Model, double IntegrationDistance)
        {
            Pattern.Counts Ret = new Pattern.Counts();
            Pattern.Counts AbsoluteV = this.GetTotalValuesForS44(Model);

            for (double Intervall = 0.0; Intervall < 500000.0; Intervall += IntegrationDistance)
            {
                double IntervallUpperLimit = Intervall + IntegrationDistance;
                double TotalRes = 0.0;
                int ResCount = 0;
                int ValCount = 0;

                for (int n = 0; n < AbsoluteV.Count; n++)
                {
                    if (AbsoluteV[n][0] > Intervall && AbsoluteV[n][0] < IntervallUpperLimit)
                    {
                        TotalRes += AbsoluteV[n][1];
                        ResCount++;
                        ValCount++;
                    }
                }

                double[] RetValueTmp = { Intervall + (IntegrationDistance / 2.0), ValCount, TotalRes / ResCount };
                Ret.Add(RetValueTmp);
            }

            return Ret;
        }

        public Pattern.Counts GetProbabilityDistributionForS55(int Model, double IntegrationDistance)
        {
            Pattern.Counts Ret = new Pattern.Counts();
            Pattern.Counts AbsoluteV = this.GetTotalValuesForS55(Model);

            for (double Intervall = 0.0; Intervall < 500000.0; Intervall += IntegrationDistance)
            {
                double IntervallUpperLimit = Intervall + IntegrationDistance;
                double TotalRes = 0.0;
                int ResCount = 0;
                int ValCount = 0;

                for (int n = 0; n < AbsoluteV.Count; n++)
                {
                    if (AbsoluteV[n][0] > Intervall && AbsoluteV[n][0] < IntervallUpperLimit)
                    {
                        TotalRes += AbsoluteV[n][1];
                        ResCount++;
                        ValCount++;
                    }
                }

                double[] RetValueTmp = { Intervall + (IntegrationDistance / 2.0), ValCount, TotalRes / ResCount };
                Ret.Add(RetValueTmp);
            }

            return Ret;
        }

        public Pattern.Counts GetProbabilityDistributionForS66(int Model, double IntegrationDistance)
        {
            Pattern.Counts Ret = new Pattern.Counts();
            Pattern.Counts AbsoluteV = this.GetTotalValuesForS66(Model);

            for (double Intervall = 0.0; Intervall < 500000.0; Intervall += IntegrationDistance)
            {
                double IntervallUpperLimit = Intervall + IntegrationDistance;
                double TotalRes = 0.0;
                int ResCount = 0;
                int ValCount = 0;

                for (int n = 0; n < AbsoluteV.Count; n++)
                {
                    if (AbsoluteV[n][0] > Intervall && AbsoluteV[n][0] < IntervallUpperLimit)
                    {
                        TotalRes += AbsoluteV[n][1];
                        ResCount++;
                        ValCount++;
                    }
                }

                double[] RetValueTmp = { Intervall + (IntegrationDistance / 2.0), ValCount, TotalRes / ResCount };
                Ret.Add(RetValueTmp);
            }

            return Ret;
        }

        public Pattern.Counts GetProbabilityDistributionForS12(int Model, double IntegrationDistance)
        {
            Pattern.Counts Ret = new Pattern.Counts();
            Pattern.Counts AbsoluteV = this.GetTotalValuesForS12(Model);

            for (double Intervall = 0.0; Intervall < 500000.0; Intervall += IntegrationDistance)
            {
                double IntervallUpperLimit = Intervall + IntegrationDistance;
                double TotalRes = 0.0;
                int ResCount = 0;
                int ValCount = 0;

                for (int n = 0; n < AbsoluteV.Count; n++)
                {
                    if (AbsoluteV[n][0] > Intervall && AbsoluteV[n][0] < IntervallUpperLimit)
                    {
                        TotalRes += AbsoluteV[n][1];
                        ResCount++;
                        ValCount++;
                    }
                }

                double[] RetValueTmp = { Intervall + (IntegrationDistance / 2.0), ValCount, TotalRes / ResCount };
                Ret.Add(RetValueTmp);
            }

            return Ret;
        }

        public Pattern.Counts GetProbabilityDistributionForS13(int Model, double IntegrationDistance)
        {
            Pattern.Counts Ret = new Pattern.Counts();
            Pattern.Counts AbsoluteV = this.GetTotalValuesForS13(Model);

            for (double Intervall = 0.0; Intervall < 500000.0; Intervall += IntegrationDistance)
            {
                double IntervallUpperLimit = Intervall + IntegrationDistance;
                double TotalRes = 0.0;
                int ResCount = 0;
                int ValCount = 0;

                for (int n = 0; n < AbsoluteV.Count; n++)
                {
                    if (AbsoluteV[n][0] > Intervall && AbsoluteV[n][0] < IntervallUpperLimit)
                    {
                        TotalRes += AbsoluteV[n][1];
                        ResCount++;
                        ValCount++;
                    }
                }

                double[] RetValueTmp = { Intervall + (IntegrationDistance / 2.0), ValCount, TotalRes / ResCount };
                Ret.Add(RetValueTmp);
            }

            return Ret;
        }

        public Pattern.Counts GetProbabilityDistributionForS23(int Model, double IntegrationDistance)
        {
            Pattern.Counts Ret = new Pattern.Counts();
            Pattern.Counts AbsoluteV = this.GetTotalValuesForS23(Model);

            for (double Intervall = 0.0; Intervall < 500000.0; Intervall += IntegrationDistance)
            {
                double IntervallUpperLimit = Intervall + IntegrationDistance;
                double TotalRes = 0.0;
                int ResCount = 0;
                int ValCount = 0;

                for (int n = 0; n < AbsoluteV.Count; n++)
                {
                    if (AbsoluteV[n][0] > Intervall && AbsoluteV[n][0] < IntervallUpperLimit)
                    {
                        TotalRes += AbsoluteV[n][1];
                        ResCount++;
                        ValCount++;
                    }
                }

                double[] RetValueTmp = { Intervall + (IntegrationDistance / 2.0), ValCount, TotalRes / ResCount };
                Ret.Add(RetValueTmp);
            }

            return Ret;
        }

        public Pattern.Counts GetProbabilityDistributionForS45(int Model, double IntegrationDistance)
        {
            Pattern.Counts Ret = new Pattern.Counts();
            Pattern.Counts AbsoluteV = this.GetTotalValuesForS45(Model);

            for (double Intervall = 0.0; Intervall < 500000.0; Intervall += IntegrationDistance)
            {
                double IntervallUpperLimit = Intervall + IntegrationDistance;
                double TotalRes = 0.0;
                int ResCount = 0;
                int ValCount = 0;

                for (int n = 0; n < AbsoluteV.Count; n++)
                {
                    if (AbsoluteV[n][0] > Intervall && AbsoluteV[n][0] < IntervallUpperLimit)
                    {
                        TotalRes += AbsoluteV[n][1];
                        ResCount++;
                        ValCount++;
                    }
                }

                double[] RetValueTmp = { Intervall + (IntegrationDistance / 2.0), ValCount, TotalRes / ResCount };
                Ret.Add(RetValueTmp);
            }

            return Ret;
        }

        public Pattern.Counts GetProbabilityDistributionForS16(int Model, double IntegrationDistance)
        {
            Pattern.Counts Ret = new Pattern.Counts();
            Pattern.Counts AbsoluteV = this.GetTotalValuesForS16(Model);

            for (double Intervall = 0.0; Intervall < 500000.0; Intervall += IntegrationDistance)
            {
                double IntervallUpperLimit = Intervall + IntegrationDistance;
                double TotalRes = 0.0;
                int ResCount = 0;
                int ValCount = 0;

                for (int n = 0; n < AbsoluteV.Count; n++)
                {
                    if (AbsoluteV[n][0] > Intervall && AbsoluteV[n][0] < IntervallUpperLimit)
                    {
                        TotalRes += AbsoluteV[n][1];
                        ResCount++;
                        ValCount++;
                    }
                }

                double[] RetValueTmp = { Intervall + (IntegrationDistance / 2.0), ValCount, TotalRes / ResCount };
                Ret.Add(RetValueTmp);
            }

            return Ret;
        }

        public Pattern.Counts GetProbabilityDistributionForS26(int Model, double IntegrationDistance)
        {
            Pattern.Counts Ret = new Pattern.Counts();
            Pattern.Counts AbsoluteV = this.GetTotalValuesForS26(Model);

            for (double Intervall = 0.0; Intervall < 500000.0; Intervall += IntegrationDistance)
            {
                double IntervallUpperLimit = Intervall + IntegrationDistance;
                double TotalRes = 0.0;
                int ResCount = 0;
                int ValCount = 0;

                for (int n = 0; n < AbsoluteV.Count; n++)
                {
                    if (AbsoluteV[n][0] > Intervall && AbsoluteV[n][0] < IntervallUpperLimit)
                    {
                        TotalRes += AbsoluteV[n][1];
                        ResCount++;
                        ValCount++;
                    }
                }

                double[] RetValueTmp = { Intervall + (IntegrationDistance / 2.0), ValCount, TotalRes / ResCount };
                Ret.Add(RetValueTmp);
            }

            return Ret;
        }

        public Pattern.Counts GetProbabilityDistributionForS36(int Model, double IntegrationDistance)
        {
            Pattern.Counts Ret = new Pattern.Counts();
            Pattern.Counts AbsoluteV = this.GetTotalValuesForS36(Model);

            for (double Intervall = 0.0; Intervall < 500000.0; Intervall += IntegrationDistance)
            {
                double IntervallUpperLimit = Intervall + IntegrationDistance;
                double TotalRes = 0.0;
                int ResCount = 0;
                int ValCount = 0;

                for (int n = 0; n < AbsoluteV.Count; n++)
                {
                    if (AbsoluteV[n][0] > Intervall && AbsoluteV[n][0] < IntervallUpperLimit)
                    {
                        TotalRes += AbsoluteV[n][1];
                        ResCount++;
                        ValCount++;
                    }
                }

                double[] RetValueTmp = { Intervall + (IntegrationDistance / 2.0), ValCount, TotalRes / ResCount };
                Ret.Add(RetValueTmp);
            }

            return Ret;
        }

        #endregion

        #endregion

        #endregion

        #endregion

        public RandomAnalysis()
        {
            this._name = "New analysis";
            List<double[]> ConstraintsTmp = new List<double[]>();
            for (int n = 0; n < 13; n++)
            {
                double[] BordersTmp = { 0.0, 0.0, 0.0 };
                ConstraintsTmp.Add(BordersTmp);
            }

            this.ConstantBorders = ConstraintsTmp.ToArray();
            this.InvestigatedTensor = new Stress.Microsopic.ElasticityTensors();
        }

        public RandomAnalysis(List<Stress.Microsopic.REK> investigatedREKs)
        {
            this._name = "New analysis";

            List<double[]> ConstraintsTmp = new List<double[]>();
            for (int n = 0; n < 13; n++)
            {
                double[] BordersTmp = { 0.0, 0.0, 0.0 };
                ConstraintsTmp.Add(BordersTmp);
            }

            this.ConstantBorders = ConstraintsTmp.ToArray();
            this.InvestigatedTensor = new Stress.Microsopic.ElasticityTensors();
            this.InvestigatedTensor.DiffractionConstants = investigatedREKs;
        }

        public RandomAnalysis(Stress.Microsopic.ElasticityTensors ET)
        {
            this._name = "New analysis";

            List<double[]> ConstraintsTmp = new List<double[]>();
            for (int n = 0; n < 13; n++)
            {
                double[] BordersTmp = { 0.0, 0.0, 0.0 };
                ConstraintsTmp.Add(BordersTmp);
            }

            this.ConstantBorders = ConstraintsTmp.ToArray();
            this.InvestigatedTensor = ET;
        }

        #region Calculations

        /// <summary>
        /// Fits the elastic constants with a UniformDistribution in an added parameter volume ans stepwidth
        /// </summary>
        /// <param name="Model">
        /// 0:Voigt
        /// 1:Reuss
        /// 2:Hill
        /// 3:Kroener
        /// 4:DeWitt
        /// </param>
        /// <param name="Stiffness">true if Stiffness should be fitted otherwise fompliance will be fitted. Does not apply to all models only 3 and 4</param>
        public void CalculateResultsEqualDestribution(int Model, bool Stiffness)
        {
            switch(Model)
            {
                case 0:
                    VoigtResults(Stiffness, true);
                    break;
                case 1:
                    ReussResults(Stiffness, true);
                    break;
                case 2:
                    HillResults(Stiffness, true);
                    break;
                case 3:
                    KroenerResults(Stiffness, true);
                    break;
                case 4:
                    DeWittResults(Stiffness, true);
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Fits the elastic constants with a UniformDistribution in an added parameter volume ans stepwidth
        /// </summary>
        /// <param name="Model">
        /// 0:Voigt
        /// 1:Reuss
        /// 2:Hill
        /// 3:Kroener
        /// 4:DeWitt
        /// </param>
        /// <param name="Stiffness">true if Stiffness should be fitted otherwise fompliance will be fitted. Does not apply to all models only 3 and 4</param>
        public void CalculateResultsRandomDestribution(int Model, bool Stiffness)
        {
            switch (Model)
            {
                case 0:
                    VoigtResults(Stiffness, false);
                    break;
                case 1:
                    ReussResults(Stiffness, false);
                    break;
                case 2:
                    HillResults(Stiffness, false);
                    break;
                case 3:
                    KroenerResults(Stiffness, false);
                    break;
                case 4:
                    DeWittResults(Stiffness, false);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// returns a set of starting values in the same order as Used Constants
        /// </summary>
        /// <returns></returns>
        private List<double[]> GetEqualDistribution(bool Stiffness)
        {
            List<double[]> Ret = new List<double[]>();
            List<List<double>> Values = new List<List<double>>();

            for(int n = 0; n < UsedConstants.Count(); n++)
            {
                List<double> ValueListTmp = new List<double>();
                if(UsedConstants[n])
                {
                    for(double val = ConstantBorders[n][0]; val <= ConstantBorders[n][1]; val += ConstantBorders[n][2])
                    {
                        ValueListTmp.Add(val);
                    }
                }
                else
                {
                    #region parameter setting

                    switch(n)
                    {
                        case 0:
                            if (Stiffness)
                            {
                                ValueListTmp.Add(-1);
                            }
                            else
                            {
                                ValueListTmp.Add(-1);
                            }
                            break;
                        case 1:
                            if (Stiffness)
                            {
                                ValueListTmp.Add(-1);
                            }
                            else
                            {
                                ValueListTmp.Add(-1);
                            }
                            break;
                        case 2:
                            if (Stiffness)
                            {
                                ValueListTmp.Add(-1);
                            }
                            else
                            {
                                ValueListTmp.Add(-1);
                            }
                            break;
                        case 3:
                            if (Stiffness)
                            {
                                ValueListTmp.Add(-1);
                            }
                            else
                            {
                                ValueListTmp.Add(-1);
                            }
                            break;
                        case 4:
                            if (Stiffness)
                            {
                                ValueListTmp.Add(-1);
                            }
                            else
                            {
                                ValueListTmp.Add(-1);
                            }
                            break;
                        case 5:
                            if (Stiffness)
                            {
                                ValueListTmp.Add(-1);
                            }
                            else
                            {
                                ValueListTmp.Add(-1);
                            }
                            break;
                        case 6:
                            if (Stiffness)
                            {
                                ValueListTmp.Add(-1);
                            }
                            else
                            {
                                ValueListTmp.Add(-1);
                            }
                            break;
                        case 7:
                            if (Stiffness)
                            {
                                ValueListTmp.Add(-1);
                            }
                            else
                            {
                                ValueListTmp.Add(-1);
                            }
                            break;
                        case 8:
                            if (Stiffness)
                            {
                                ValueListTmp.Add(-1);
                            }
                            else
                            {
                                ValueListTmp.Add(-1);
                            }
                            break;
                        case 9:
                            if (Stiffness)
                            {
                                ValueListTmp.Add(-1);
                            }
                            else
                            {
                                ValueListTmp.Add(-1);
                            }
                            break;
                        case 10:
                            if (Stiffness)
                            {
                                ValueListTmp.Add(-1);
                            }
                            else
                            {
                                ValueListTmp.Add(-1);
                            }
                            break;
                        case 11:
                            if (Stiffness)
                            {
                                ValueListTmp.Add(-1);
                            }
                            else
                            {
                                ValueListTmp.Add(-1);
                            }
                            break;
                        case 12:
                            if (Stiffness)
                            {
                                ValueListTmp.Add(-1);
                            }
                            else
                            {
                                ValueListTmp.Add(-1);
                            }
                            break;
                    }

                    #endregion
                }

                Values.Add(ValueListTmp);
            }

            for(int n0 = 0; n0 < Values[0].Count; n0++)
            {
                for (int n1 = 0; n1 < Values[1].Count; n1++)
                {
                    for (int n2 = 0; n2 < Values[2].Count; n2++)
                    {
                        for (int n3 = 0; n3 < Values[3].Count; n3++)
                        {
                            for (int n4 = 0; n4 < Values[4].Count; n4++)
                            {
                                for (int n5 = 0; n5 < Values[5].Count; n5++)
                                {
                                    for (int n6 = 0; n6 < Values[6].Count; n6++)
                                    {
                                        for (int n7 = 0; n7 < Values[7].Count; n7++)
                                        {
                                            for (int n8 = 0; n8 < Values[8].Count; n8++)
                                            {
                                                for (int n9 = 0; n9 < Values[9].Count; n9++)
                                                {
                                                    for (int n10 = 0; n10 < Values[10].Count; n10++)
                                                    {
                                                        for (int n11 = 0; n11 < Values[11].Count; n11++)
                                                        {
                                                            for (int n12 = 0; n12 < Values[12].Count; n12++)
                                                            {
                                                                double[] NewConstellation = { Values[0][n0], Values[1][n1], Values[2][n2], Values[3][n3], Values[4][n4], Values[5][n5], Values[6][n6], Values[7][n7], Values[8][n8], Values[9][n9], Values[10][n10], Values[11][n11], Values[12][n12] };
                                                                Ret.Add(NewConstellation);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            _TotalThreadingSimulation = Ret.Count;
            return Ret;
        }

        /// <summary>
        /// returns a set of starting values in the same order as Used Constants
        /// </summary>
        /// <returns></returns>
        private List<double[]> GetRandomDistribution(bool Stiffness)
        {
            List<double[]> Ret = new List<double[]>();
            List<List<double>> Values = new List<List<double>>();

            System.Random ValuePool = new Random();

            for (int n = 0; n < UsedConstants.Count(); n++)
            {
                List<double> ValueListTmp = new List<double>();
                if (UsedConstants[n])
                {
                    double Range = ConstantBorders[n][0] - ConstantBorders[n][1];
                    for (int i = 0; i <= ConstantBorders[n][2]; i++)
                    {
                        double NextValue = ConstantBorders[n][0] + (ValuePool.NextDouble() * Range);
                        ValueListTmp.Add(NextValue);
                    }
                }
                else
                {
                    #region parameter setting

                    switch (n)
                    {
                        case 0:
                            if (Stiffness)
                            {
                                ValueListTmp.Add(InvestigatedTensor.C11);
                            }
                            else
                            {
                                ValueListTmp.Add(InvestigatedTensor.S11);
                            }
                            break;
                        case 1:
                            if (Stiffness)
                            {
                                ValueListTmp.Add(InvestigatedTensor.C22);
                            }
                            else
                            {
                                ValueListTmp.Add(InvestigatedTensor.S22);
                            }
                            break;
                        case 2:
                            if (Stiffness)
                            {
                                ValueListTmp.Add(InvestigatedTensor.C33);
                            }
                            else
                            {
                                ValueListTmp.Add(InvestigatedTensor.S33);
                            }
                            break;
                        case 3:
                            if (Stiffness)
                            {
                                ValueListTmp.Add(InvestigatedTensor.C44);
                            }
                            else
                            {
                                ValueListTmp.Add(InvestigatedTensor.S44);
                            }
                            break;
                        case 4:
                            if (Stiffness)
                            {
                                ValueListTmp.Add(InvestigatedTensor.C55);
                            }
                            else
                            {
                                ValueListTmp.Add(InvestigatedTensor.S55);
                            }
                            break;
                        case 5:
                            if (Stiffness)
                            {
                                ValueListTmp.Add(InvestigatedTensor.C66);
                            }
                            else
                            {
                                ValueListTmp.Add(InvestigatedTensor.S66);
                            }
                            break;
                        case 6:
                            if (Stiffness)
                            {
                                ValueListTmp.Add(InvestigatedTensor.C12);
                            }
                            else
                            {
                                ValueListTmp.Add(InvestigatedTensor.S12);
                            }
                            break;
                        case 7:
                            if (Stiffness)
                            {
                                ValueListTmp.Add(InvestigatedTensor.C13);
                            }
                            else
                            {
                                ValueListTmp.Add(InvestigatedTensor.S13);
                            }
                            break;
                        case 8:
                            if (Stiffness)
                            {
                                ValueListTmp.Add(InvestigatedTensor.C23);
                            }
                            else
                            {
                                ValueListTmp.Add(InvestigatedTensor.S23);
                            }
                            break;
                        case 9:
                            if (Stiffness)
                            {
                                ValueListTmp.Add(InvestigatedTensor.C45);
                            }
                            else
                            {
                                ValueListTmp.Add(InvestigatedTensor.S45);
                            }
                            break;
                        case 10:
                            if (Stiffness)
                            {
                                ValueListTmp.Add(InvestigatedTensor.C16);
                            }
                            else
                            {
                                ValueListTmp.Add(InvestigatedTensor.S16);
                            }
                            break;
                        case 11:
                            if (Stiffness)
                            {
                                ValueListTmp.Add(InvestigatedTensor.C26);
                            }
                            else
                            {
                                ValueListTmp.Add(InvestigatedTensor.S26);
                            }
                            break;
                        case 12:
                            if (Stiffness)
                            {
                                ValueListTmp.Add(InvestigatedTensor.C36);
                            }
                            else
                            {
                                ValueListTmp.Add(InvestigatedTensor.S36);
                            }
                            break;
                    }

                    #endregion
                }

                Values.Add(ValueListTmp);
            }

            for (int n0 = 0; n0 < Values[0].Count; n0++)
            {
                for (int n1 = 0; n1 < Values[1].Count; n1++)
                {
                    for (int n2 = 0; n2 < Values[2].Count; n2++)
                    {
                        for (int n3 = 0; n3 < Values[3].Count; n3++)
                        {
                            for (int n4 = 0; n4 < Values[4].Count; n4++)
                            {
                                for (int n5 = 0; n5 < Values[5].Count; n5++)
                                {
                                    for (int n6 = 0; n6 < Values[6].Count; n6++)
                                    {
                                        for (int n7 = 0; n7 < Values[7].Count; n7++)
                                        {
                                            for (int n8 = 0; n8 < Values[8].Count; n8++)
                                            {
                                                for (int n9 = 0; n9 < Values[9].Count; n9++)
                                                {
                                                    for (int n10 = 0; n10 < Values[10].Count; n10++)
                                                    {
                                                        for (int n11 = 0; n11 < Values[11].Count; n11++)
                                                        {
                                                            for (int n12 = 0; n12 < Values[12].Count; n12++)
                                                            {
                                                                double[] NewConstellation = { Values[0][n0], Values[1][n1], Values[2][n2], Values[3][n3], Values[4][n4], Values[5][n5], Values[0][n6], Values[0][n7], Values[0][n8], Values[0][n9], Values[0][n10], Values[0][n11], Values[0][n12] };
                                                                Ret.Add(NewConstellation);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            _TotalThreadingSimulation = Ret.Count;
            return Ret;
        }


        private void VoigtResults(bool Stiffness, bool EqualDistribution)
        {
            TensorResults.Clear();

            List<double[]> StartingParamList = new List<double[]>();

            if(EqualDistribution)
            {
                StartingParamList = GetEqualDistribution(Stiffness);
            }
            else
            {
                StartingParamList = GetRandomDistribution(Stiffness);
            }

            for(int n = 0; n < StartingParamList.Count; n++)
            {
                Stress.Microsopic.ElasticityTensors ResultTmp = InvestigatedTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                for(int i = 0; i < StartingParamList[n].Count(); i++)
                {
                    #region parameter setting

                    if (StartingParamList[n][i] != -1)
                    {
                        switch (i)
                        {
                            case 0:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C11 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S11 = StartingParamList[n][i];
                                }
                                break;
                            case 1:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C22 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S22 = StartingParamList[n][i];
                                }
                                break;
                            case 2:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C33 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S33 = StartingParamList[n][i];
                                }
                                break;
                            case 3:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C44 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S44 = StartingParamList[n][i];
                                }
                                break;
                            case 4:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C55 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S55 = StartingParamList[n][i];
                                }
                                break;
                            case 5:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C66 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S66 = StartingParamList[n][i];
                                }
                                break;
                            case 6:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C12 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S12 = StartingParamList[n][i];
                                }
                                break;
                            case 7:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C13 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S13 = StartingParamList[n][i];
                                }
                                break;
                            case 8:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C23 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S23 = StartingParamList[n][i];
                                }
                                break;
                            case 9:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C45 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S45 = StartingParamList[n][i];
                                }
                                break;
                            case 10:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C16 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S16 = StartingParamList[n][i];
                                }
                                break;
                            case 11:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C26 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S26 = StartingParamList[n][i];
                                }
                                break;
                            case 12:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C36 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S36 = StartingParamList[n][i];
                                }
                                break;
                        }
                    }

                    #endregion
                }

                if(Stiffness)
                {
                    ResultTmp.CalculateCompliances();
                }
                else
                {
                    ResultTmp.CalculateStiffnesses();
                }

                ResultTmp.FitVoigt(this.ClassicREKCaluclation);
                TensorResults.Add(ResultTmp);
                OnSimulationUpdated();
            }
        }

        private void ReussResults(bool Stiffness, bool EqualDistribution)
        {
            TensorResults.Clear();

            List<double[]> StartingParamList = GetEqualDistribution(Stiffness);

            for (int n = 0; n < StartingParamList.Count; n++)
            {
                Stress.Microsopic.ElasticityTensors ResultTmp = InvestigatedTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                for (int i = 0; i < StartingParamList[n].Count(); i++)
                {
                    #region parameter setting

                    if (StartingParamList[n][i] != -1)
                    {

                        switch (i)
                        {
                            case 0:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C11 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S11 = StartingParamList[n][i];
                                }
                                break;
                            case 1:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C22 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S22 = StartingParamList[n][i];
                                }
                                break;
                            case 2:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C33 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S33 = StartingParamList[n][i];
                                }
                                break;
                            case 3:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C44 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S44 = StartingParamList[n][i];
                                }
                                break;
                            case 4:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C55 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S55 = StartingParamList[n][i];
                                }
                                break;
                            case 5:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C66 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S66 = StartingParamList[n][i];
                                }
                                break;
                            case 6:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C12 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S12 = StartingParamList[n][i];
                                }
                                break;
                            case 7:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C13 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S13 = StartingParamList[n][i];
                                }
                                break;
                            case 8:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C23 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S23 = StartingParamList[n][i];
                                }
                                break;
                            case 9:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C45 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S45 = StartingParamList[n][i];
                                }
                                break;
                            case 10:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C16 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S16 = StartingParamList[n][i];
                                }
                                break;
                            case 11:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C26 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S26 = StartingParamList[n][i];
                                }
                                break;
                            case 12:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C36 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S36 = StartingParamList[n][i];
                                }
                                break;
                        }
                    }

                    #endregion
                }

                if (Stiffness)
                {
                    ResultTmp.CalculateCompliances();
                }
                else
                {
                    ResultTmp.CalculateStiffnesses();
                }

                ResultTmp.FitReuss(this.ClassicREKCaluclation);
                TensorResults.Add(ResultTmp);
                OnSimulationUpdated();
            }
        }

        private void HillResults(bool Stiffness, bool EqualDistribution)
        {
            TensorResults.Clear();

            List<double[]> StartingParamList = GetEqualDistribution(Stiffness);

            for (int n = 0; n < StartingParamList.Count; n++)
            {
                Stress.Microsopic.ElasticityTensors ResultTmp = InvestigatedTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                for (int i = 0; i < StartingParamList[n].Count(); i++)
                {
                    #region parameter setting

                    if (StartingParamList[n][i] != -1)
                    {

                        switch (i)
                        {
                            case 0:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C11 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S11 = StartingParamList[n][i];
                                }
                                break;
                            case 1:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C22 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S22 = StartingParamList[n][i];
                                }
                                break;
                            case 2:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C33 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S33 = StartingParamList[n][i];
                                }
                                break;
                            case 3:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C44 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S44 = StartingParamList[n][i];
                                }
                                break;
                            case 4:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C55 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S55 = StartingParamList[n][i];
                                }
                                break;
                            case 5:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C66 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S66 = StartingParamList[n][i];
                                }
                                break;
                            case 6:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C12 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S12 = StartingParamList[n][i];
                                }
                                break;
                            case 7:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C13 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S13 = StartingParamList[n][i];
                                }
                                break;
                            case 8:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C23 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S23 = StartingParamList[n][i];
                                }
                                break;
                            case 9:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C45 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S45 = StartingParamList[n][i];
                                }
                                break;
                            case 10:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C16 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S16 = StartingParamList[n][i];
                                }
                                break;
                            case 11:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C26 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S26 = StartingParamList[n][i];
                                }
                                break;
                            case 12:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C36 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S36 = StartingParamList[n][i];
                                }
                                break;
                        }
                    }

                    #endregion
                }

                if (Stiffness)
                {
                    ResultTmp.CalculateCompliances();
                }
                else
                {
                    ResultTmp.CalculateStiffnesses();
                }

                ResultTmp.FitHill(this.ClassicREKCaluclation);
                TensorResults.Add(ResultTmp);
                OnSimulationUpdated();
            }
        }

        private void KroenerResults(bool Stiffness, bool EqualDistribution)
        {
            TensorResults.Clear();

            List<double[]> StartingParamList = GetEqualDistribution(Stiffness);

            for (int n = 0; n < StartingParamList.Count; n++)
            {
                Stress.Microsopic.ElasticityTensors ResultTmp = InvestigatedTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                for (int i = 0; i < StartingParamList[n].Count(); i++)
                {
                    #region parameter setting

                    if (StartingParamList[n][i] != -1)
                    {

                        switch (i)
                        {
                            case 0:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C11 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S11 = StartingParamList[n][i];
                                }
                                break;
                            case 1:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C22 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S22 = StartingParamList[n][i];
                                }
                                break;
                            case 2:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C33 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S33 = StartingParamList[n][i];
                                }
                                break;
                            case 3:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C44 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S44 = StartingParamList[n][i];
                                }
                                break;
                            case 4:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C55 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S55 = StartingParamList[n][i];
                                }
                                break;
                            case 5:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C66 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S66 = StartingParamList[n][i];
                                }
                                break;
                            case 6:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C12 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S12 = StartingParamList[n][i];
                                }
                                break;
                            case 7:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C13 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S13 = StartingParamList[n][i];
                                }
                                break;
                            case 8:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C23 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S23 = StartingParamList[n][i];
                                }
                                break;
                            case 9:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C45 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S45 = StartingParamList[n][i];
                                }
                                break;
                            case 10:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C16 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S16 = StartingParamList[n][i];
                                }
                                break;
                            case 11:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C26 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S26 = StartingParamList[n][i];
                                }
                                break;
                            case 12:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C36 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S36 = StartingParamList[n][i];
                                }
                                break;
                        }
                    }

                    #endregion
                }

                if (Stiffness)
                {
                    ResultTmp.CalculateCompliances();
                }
                else
                {
                    ResultTmp.CalculateStiffnesses();
                }

                ResultTmp.FitKroener(this.ClassicREKCaluclation, Stiffness);
                TensorResults.Add(ResultTmp);
                OnSimulationUpdated();
            }
        }

        private void DeWittResults(bool Stiffness, bool EqualDistribution)
        {
            TensorResults.Clear();

            List<double[]> StartingParamList = GetEqualDistribution(Stiffness);

            for (int n = 0; n < StartingParamList.Count; n++)
            {
                Stress.Microsopic.ElasticityTensors ResultTmp = InvestigatedTensor.Clone() as Stress.Microsopic.ElasticityTensors;

                for (int i = 0; i < StartingParamList[n].Count(); i++)
                {
                    #region parameter setting

                    if (StartingParamList[n][i] != -1)
                    {

                        switch (i)
                        {
                            case 0:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C11 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S11 = StartingParamList[n][i];
                                }
                                break;
                            case 1:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C22 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S22 = StartingParamList[n][i];
                                }
                                break;
                            case 2:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C33 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S33 = StartingParamList[n][i];
                                }
                                break;
                            case 3:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C44 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S44 = StartingParamList[n][i];
                                }
                                break;
                            case 4:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C55 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S55 = StartingParamList[n][i];
                                }
                                break;
                            case 5:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C66 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S66 = StartingParamList[n][i];
                                }
                                break;
                            case 6:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C12 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S12 = StartingParamList[n][i];
                                }
                                break;
                            case 7:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C13 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S13 = StartingParamList[n][i];
                                }
                                break;
                            case 8:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C23 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S23 = StartingParamList[n][i];
                                }
                                break;
                            case 9:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C45 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S45 = StartingParamList[n][i];
                                }
                                break;
                            case 10:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C16 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S16 = StartingParamList[n][i];
                                }
                                break;
                            case 11:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C26 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S26 = StartingParamList[n][i];
                                }
                                break;
                            case 12:
                                if (Stiffness)
                                {
                                    InvestigatedTensor.C36 = StartingParamList[n][i];
                                }
                                else
                                {
                                    InvestigatedTensor.S36 = StartingParamList[n][i];
                                }
                                break;
                        }
                    }

                    #endregion
                }

                if (Stiffness)
                {
                    ResultTmp.CalculateCompliances();
                }
                else
                {
                    ResultTmp.CalculateStiffnesses();
                }

                ResultTmp.FitKroener(this.ClassicREKCaluclation, Stiffness);
                TensorResults.Add(ResultTmp);
                OnSimulationUpdated();
            }
        }

        #endregion

        #region Multi threading

        public int _TotalThreadingSimulation = 0;
        public int _ActualThreadingSimulation = 0;

        public int _multiThreadingModel = 0;
        public bool _multiThreadingStiffness = false;
        public bool _multiThreadingEqual = false;

        public event System.ComponentModel.PropertyChangedEventHandler SimulationFinished;
        public event System.ComponentModel.PropertyChangedEventHandler SimulationStarted;
        public event System.ComponentModel.PropertyChangedEventHandler SimulationUpdated;

        protected void OnSimulationStarted()
        {
            _ActualThreadingSimulation = 0;
            System.ComponentModel.PropertyChangedEventHandler handler = SimulationStarted;
            if (handler != null)
            {
                handler(this, new System.ComponentModel.PropertyChangedEventArgs("SimulationStarted"));
            }
        }

        protected void OnSimulationFinished()
        {

            System.ComponentModel.PropertyChangedEventHandler handler = SimulationFinished;
            if (handler != null)
            {
                handler(this, new System.ComponentModel.PropertyChangedEventArgs("SimualtionFinished"));
            }
        }

        protected void OnSimulationUpdated()
        {
            _ActualThreadingSimulation++;
            System.ComponentModel.PropertyChangedEventHandler handler = SimulationUpdated;
            if (handler != null)
            {
                handler(this, new System.ComponentModel.PropertyChangedEventArgs("SimualtionUpdated"));
            }
        }

        // Wrapper method for use with thread pool. 
        public void SimulationCallback(Object threadContext)
        {
            OnSimulationStarted();

            if (this._multiThreadingEqual)
            {
                this.CalculateResultsEqualDestribution(this._multiThreadingModel, this._multiThreadingStiffness);
            }
            else
            {
                this.CalculateResultsRandomDestribution(this._multiThreadingModel, this._multiThreadingStiffness);
            }
            
            OnSimulationFinished();
        }

        # endregion

        #region Cloning

        public object Clone()
        {
            RandomAnalysis Ret = new RandomAnalysis(this.InvestigatedTensor.Clone() as Stress.Microsopic.ElasticityTensors);

            for(int n = 0; n < this.TensorResults.Count; n++)
            {
                Ret.TensorResults.Add(this.TensorResults[n].Clone() as Stress.Microsopic.ElasticityTensors);
            }

            bool[] ClonedConstants = new bool[13];
            List<double[]> ClonedBorders = new List<double[]>();

            for(int n = 0; n < 13; n++)
            {
                ClonedConstants[n] = this.UsedConstants[n];
                double[] Tmp = { this.ConstantBorders[n][0], this.ConstantBorders[n][1], this.ConstantBorders[n][2] };
                ClonedBorders.Add(Tmp);
            }

            Ret.UsedConstants = ClonedConstants;
            Ret.ConstantBorders = ClonedBorders.ToArray();

            Ret.ClassicREKCaluclation = this.ClassicREKCaluclation;

            return Ret;

        }

        #endregion
    }
}
