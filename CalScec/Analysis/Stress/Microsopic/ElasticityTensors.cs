using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalScec.Analysis.Stress.Microsopic
{
    public class ElasticityTensors : ICloneable
    {
        #region Parameters

        public MathNet.Numerics.LinearAlgebra.Matrix<double> _stiffnessTensor = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(6, 6, 0.0);
        public MathNet.Numerics.LinearAlgebra.Matrix<double> _stiffnessTensorError = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(6, 6, 0.0);

        public MathNet.Numerics.LinearAlgebra.Matrix<double> _complianceTensor = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(6, 6, 0.0);
        public MathNet.Numerics.LinearAlgebra.Matrix<double> _complianceTensorError = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(6, 6, 0.0);

        private int _symmetry;
        public string Symmetry
        {
            get
            {
                switch(this._symmetry)
                {
                    case 1:
                        return "cubic";
                    case 2:
                        return "hexagonal";
                    case 3:
                        return "tetragonal type 1";
                    case 4:
                        return "tetragonal type 2";
                    case 5:
                        return "trigonal type 1";
                    case 6:
                        return "trigonal type 2";
                    case 7:
                        return "rhombic";
                    case 8:
                        return "monoclinic";
                    case 9:
                        return "triclinic";
                    default:
                        return "triclinic";
                }
            }
            set
            {
                switch(value)
                {
                    case "cubic":
                        this._symmetry = 1;
                        break;
                    case "hexagonal":
                        this._symmetry = 2;
                        break;
                    case "tetragonal type 1":
                        this._symmetry = 3;
                        break;
                    case "tetragonal type 2":
                        this._symmetry = 4;
                        break;
                    case "trigonal type 1":
                        this._symmetry = 5;
                        break;
                    case "trigonal type 2":
                        this._symmetry = 6;
                        break;
                    case "rhombic":
                        this._symmetry = 7;
                        break;
                    case "monoclinic":
                        this._symmetry = 8;
                        break;
                    case "triclinic":
                        this._symmetry = 9;
                        break;
                    default:
                        this._symmetry = 9;
                        break;
                }
            }
        }

        public DataManagment.CrystalData.CODData GetPhaseInformation
        {
            get
            {
                if(this.DiffractionConstants.Count > 0)
                {
                    return DiffractionConstants[0].PhaseInformation;
                }
                else
                {
                    return new DataManagment.CrystalData.CODData();
                }
            }
        }

        public Texture.OrientationDistributionFunction ODF;

        public List<REK> DiffractionConstants = new List<REK>();
        /// <summary>
        /// Calculates the diffraction elastic constants from the single crystaline ones
        /// </summary>
        /// <param name="Model">
        /// Ist the used model:
        /// 0: Voigt
        /// 1: Reuss
        /// 2: Hill
        /// 3: Kroener
        /// 4: DeWitt
        /// 5: Geometric Hill
        /// </param>
        /// <returns></returns>
        public List<REK> GetCalculatedDiffractionConstants(int Model, DataManagment.CrystalData.CODData crystalData, int CoSt)
        {
            switch(Model)
            {
                case 0:
                    return this.GetCalculatedDiffractionConstantsVoigt(crystalData);
                case 1:
                    return this.GetCalculatedDiffractionConstantsReuss(crystalData);
                case 2:
                    return this.GetCalculatedDiffractionConstantsHill(crystalData);
                case 3:
                    if (CoSt == 0)
                    {
                        return this.GetCalculatedDiffractionConstantsKroenerCompliance(crystalData);
                    }
                    else
                    {
                        return this.GetCalculatedDiffractionConstantsKroenerStiffness(crystalData);
                    }
                case 4:
                    if (CoSt == 0)
                    {
                        return this.GetCalculatedDiffractionConstantsDeWittCompliance(crystalData);
                    }
                    else
                    {
                        return this.GetCalculatedDiffractionConstantsDeWittStiffness(crystalData);
                    }
                case 5:
                    return this.GetCalculatedDiffractionConstantsGeometricHill(crystalData);
                default:
                    return new List<REK>();
            }
        }

        public List<REK> GetCalculatedDiffractionConstantsVoigt(DataManagment.CrystalData.CODData crystalData)
        {
            List<REK> Ret = new List<REK>();
            DataManagment.CrystalData.HKLReflex NewReflex = new DataManagment.CrystalData.HKLReflex(0, 0, 0, 0);
            REK Tmp = new REK(crystalData, NewReflex);

            switch (this.Symmetry)
            {
                case "cubic":
                    if (this.IsIsotropic)
                    {
                        Tmp.ClassicFittingFunction.Constant = this.S1VoigtCubicIsotrope();
                        Tmp.ClassicFittingFunction.Aclivity = this.HS2VoigtCubicIsotrope();
                    }
                    else
                    {
                        Tmp.ClassicFittingFunction.Constant = this.S1VoigtCubic();
                        Tmp.ClassicFittingFunction.Aclivity = this.HS2VoigtCubic();
                    }
                    break;
                case "hexagonal":
                    Tmp.ClassicFittingFunction.Constant = this.S1VoigtType1();
                    Tmp.ClassicFittingFunction.Aclivity = this.HS2VoigtType1();
                    break;
                case "tetragonal type 1":
                    Tmp.ClassicFittingFunction.Constant = this.S1VoigtType2();
                    Tmp.ClassicFittingFunction.Aclivity = this.HS2VoigtType2();
                    break;
                case "tetragonal type 2":
                    Tmp.ClassicFittingFunction.Constant = this.S1VoigtType2();
                    Tmp.ClassicFittingFunction.Aclivity = this.HS2VoigtType2();
                    break;
                case "trigonal type 1":
                    Tmp.ClassicFittingFunction.Constant = this.S1VoigtType1();
                    Tmp.ClassicFittingFunction.Aclivity = this.HS2VoigtType1();
                    break;
                case "trigonal type 2":
                    Tmp.ClassicFittingFunction.Constant = this.S1VoigtType1();
                    Tmp.ClassicFittingFunction.Aclivity = this.HS2VoigtType1();
                    break;
                case "rhombic":
                    Tmp.ClassicFittingFunction.Constant = this.S1VoigtType3();
                    Tmp.ClassicFittingFunction.Aclivity = this.HS2VoigtType3();
                    break;
                case "monoclinic":
                    Tmp.ClassicFittingFunction.Constant = this.S1VoigtType3();
                    Tmp.ClassicFittingFunction.Aclivity = this.HS2VoigtType3();
                    break;
                case "triclinic":
                    Tmp.ClassicFittingFunction.Constant = this.S1VoigtType3();
                    Tmp.ClassicFittingFunction.Aclivity = this.HS2VoigtType3();
                    break;
                default:
                    Tmp.ClassicFittingFunction.Constant = this.S1VoigtType3();
                    Tmp.ClassicFittingFunction.Aclivity = this.HS2VoigtType3();
                    break;
            }

            Ret.Add(Tmp);
            
            return Ret;
        }

        public List<REK> GetCalculatedDiffractionConstantsReuss(DataManagment.CrystalData.CODData crystalData)
        {
            List<REK> Ret = new List<REK>();

            switch (this.Symmetry)
            {
                case "cubic":
                    for (int n = 0; n < crystalData.HKLList.Count; n++)
                    {
                        REK Tmp = new REK(crystalData, crystalData.HKLList[n]);

                        Tmp.ClassicFittingFunction.Constant = this.S1ReussCubic(crystalData.HKLList[n]);
                        Tmp.ClassicFittingFunction.Aclivity = this.HS2ReussCubic(crystalData.HKLList[n]);

                        Ret.Add(Tmp);
                    }
                    break;
                case "hexagonal":
                    for (int n = 0; n < crystalData.HKLList.Count; n++)
                    {
                        REK Tmp = new REK(crystalData, crystalData.HKLList[n]);

                        Tmp.ClassicFittingFunction.Constant = this.S1ReussHexagonal(crystalData.HKLList[n]);
                        Tmp.ClassicFittingFunction.Aclivity = this.HS2ReussHexagonal(crystalData.HKLList[n]);

                        Ret.Add(Tmp);
                    }
                    break;
                case "tetragonal type 1":

                    break;
                case "tetragonal type 2":

                    break;
                case "trigonal type 1":

                    break;
                case "trigonal type 2":

                    break;
                case "rhombic":

                    break;
                case "monoclinic":

                    break;
                case "triclinic":

                    break;
                default:

                    break;
            }

            return Ret;
        }

        public List<REK> GetCalculatedDiffractionConstantsHill(DataManagment.CrystalData.CODData crystalData)
        {
            List<REK> Ret = new List<REK>();

            switch (this.Symmetry)
            {
                case "cubic":
                    for (int n = 0; n < crystalData.HKLList.Count; n++)
                    {
                        REK Tmp = new REK(crystalData, crystalData.HKLList[n]);

                        Tmp.ClassicFittingFunction.Constant = this.S1HillCubic(crystalData.HKLList[n]);
                        Tmp.ClassicFittingFunction.Aclivity = this.HS2HillCubic(crystalData.HKLList[n]);

                        Ret.Add(Tmp);
                    }
                    break;
                case "hexagonal":
                    for (int n = 0; n < crystalData.HKLList.Count; n++)
                    {
                        REK Tmp = new REK(crystalData, crystalData.HKLList[n]);

                        Tmp.ClassicFittingFunction.Constant = this.S1HillHexagonal(crystalData.HKLList[n]);
                        Tmp.ClassicFittingFunction.Aclivity = this.HS2HillHexagonal(crystalData.HKLList[n]);

                        Ret.Add(Tmp);
                    }
                    break;
                case "tetragonal type 1":

                    break;
                case "tetragonal type 2":

                    break;
                case "trigonal type 1":

                    break;
                case "trigonal type 2":

                    break;
                case "rhombic":

                    break;
                case "monoclinic":

                    break;
                case "triclinic":

                    break;
                default:

                    break;
            }

            return Ret;
        }

        public List<REK> GetCalculatedDiffractionConstantsKroenerCompliance(DataManagment.CrystalData.CODData crystalData)
        {
            List<REK> Ret = new List<REK>();

            switch (this.Symmetry)
            {
                case "cubic":
                    for (int n = 0; n < crystalData.HKLList.Count; n++)
                    {
                        REK Tmp = new REK(crystalData, crystalData.HKLList[n]);
                        double S1Tmp = this.S1KroenerCubicCompliance();
                        double HS2Tmp = this.S1KroenerCubicCompliance();
                        Tmp.ClassicFittingFunction.Constant = S1Tmp;
                        Tmp.ClassicFittingFunction.Aclivity = HS2Tmp;

                        Ret.Add(Tmp);
                    }
                    break;
                case "hexagonal":
                    break;
                case "tetragonal type 1":

                    break;
                case "tetragonal type 2":

                    break;
                case "trigonal type 1":

                    break;
                case "trigonal type 2":

                    break;
                case "rhombic":

                    break;
                case "monoclinic":

                    break;
                case "triclinic":

                    break;
                default:

                    break;
            }

            return Ret;
        }

        public List<REK> GetCalculatedDiffractionConstantsDeWittCompliance(DataManagment.CrystalData.CODData crystalData)
        {
            List<REK> Ret = new List<REK>();

            switch (this.Symmetry)
            {
                case "cubic":
                    for (int n = 0; n < crystalData.HKLList.Count; n++)
                    {
                        REK Tmp = new REK(crystalData, crystalData.HKLList[n]);

                        Tmp.ClassicFittingFunction.Constant = this.S1DeWittCubicCompliance(crystalData.HKLList[n]);
                        Tmp.ClassicFittingFunction.Aclivity = this.HS2DeWittCubicCompliance(crystalData.HKLList[n]);

                        Ret.Add(Tmp);
                    }
                    break;
                case "hexagonal":
                    break;
                case "tetragonal type 1":

                    break;
                case "tetragonal type 2":

                    break;
                case "trigonal type 1":

                    break;
                case "trigonal type 2":

                    break;
                case "rhombic":

                    break;
                case "monoclinic":

                    break;
                case "triclinic":

                    break;
                default:

                    break;
            }

            return Ret;
        }

        public List<REK> GetCalculatedDiffractionConstantsKroenerStiffness(DataManagment.CrystalData.CODData crystalData)
        {
            List<REK> Ret = new List<REK>();

            switch (this.Symmetry)
            {
                case "cubic":
                    for (int n = 0; n < crystalData.HKLList.Count; n++)
                    {
                        REK Tmp = new REK(crystalData, crystalData.HKLList[n]);

                        Tmp.ClassicFittingFunction.Constant = this.S1KroenerCubicStiffness();
                        Tmp.ClassicFittingFunction.Aclivity = this.HS2KroenerCubicStiffness();

                        Ret.Add(Tmp);
                    }
                    break;
                case "hexagonal":
                    break;
                case "tetragonal type 1":

                    break;
                case "tetragonal type 2":

                    break;
                case "trigonal type 1":

                    break;
                case "trigonal type 2":

                    break;
                case "rhombic":

                    break;
                case "monoclinic":

                    break;
                case "triclinic":

                    break;
                default:

                    break;
            }

            return Ret;
        }

        public List<REK> GetCalculatedDiffractionConstantsDeWittStiffness(DataManagment.CrystalData.CODData crystalData)
        {
            List<REK> Ret = new List<REK>();

            switch (this.Symmetry)
            {
                case "cubic":
                    for (int n = 0; n < crystalData.HKLList.Count; n++)
                    {
                        REK Tmp = new REK(crystalData, crystalData.HKLList[n]);

                        Tmp.ClassicFittingFunction.Constant = this.S1DeWittCubicStiffness(crystalData.HKLList[n]);
                        Tmp.ClassicFittingFunction.Aclivity = this.HS2DeWittCubicStiffness(crystalData.HKLList[n]);

                        Ret.Add(Tmp);
                    }
                    break;
                case "hexagonal":
                    break;
                case "tetragonal type 1":

                    break;
                case "tetragonal type 2":

                    break;
                case "trigonal type 1":

                    break;
                case "trigonal type 2":

                    break;
                case "rhombic":

                    break;
                case "monoclinic":

                    break;
                case "triclinic":

                    break;
                default:

                    break;
            }

            return Ret;
        }

        public List<REK> GetCalculatedDiffractionConstantsGeometricHill(DataManagment.CrystalData.CODData crystalData)
        {
            List<REK> Ret = new List<REK>();

            switch (this.Symmetry)
            {
                case "cubic":
                    for (int n = 0; n < crystalData.HKLList.Count; n++)
                    {
                        REK Tmp = new REK(crystalData, crystalData.HKLList[n]);

                        Tmp.ClassicFittingFunction.Constant = this.S1GeometricHillCubic(crystalData.HKLList[n]);
                        Tmp.ClassicFittingFunction.Aclivity = this.HS2GeometricHillCubic(crystalData.HKLList[n]);

                        Ret.Add(Tmp);
                    }
                    break;
                case "hexagonal":
                    for (int n = 0; n < crystalData.HKLList.Count; n++)
                    {
                        REK Tmp = new REK(crystalData, crystalData.HKLList[n]);

                        Tmp.ClassicFittingFunction.Constant = this.S1GeometricHillHexagonal(crystalData.HKLList[n]);
                        Tmp.ClassicFittingFunction.Aclivity = this.HS2GeometricHillHexagonal(crystalData.HKLList[n]);

                        Ret.Add(Tmp);
                    }
                    break;
                case "tetragonal type 1":

                    break;
                case "tetragonal type 2":

                    break;
                case "trigonal type 1":

                    break;
                case "trigonal type 2":

                    break;
                case "rhombic":

                    break;
                case "monoclinic":

                    break;
                case "triclinic":

                    break;
                default:

                    break;
            }

            return Ret;
        }

        public bool IsIsotropic = false;
        public bool FitConverged = false;

        #region averaged parameters

        private double _averagedEModul;
        public double AveragedEModul
        {
            get
            {
                return this._averagedEModul;
            }
        }

        private double _averagedNu;
        public double AveragedNu
        {
            get
            {
                return this._averagedNu;
            }
        }

        private double _averagedShearModul;
        public double AveragedSchearModul
        {
            get
            {
                return this._averagedShearModul;
            }
        }

        private double _averagedBulkModul;
        public double AveragedBulkModul
        {
            get
            {
                return this._averagedBulkModul;
            }
        }

        private double _averagedEModulFit;
        public double AveragedEModulFit
        {
            get
            {
                return this._averagedEModulFit;
            }
        }

        private double _averagedNuFit;
        public double AveragedNuFit
        {
            get
            {
                return this._averagedNuFit;
            }
        }

        private double _averagedShearModulFit;
        public double AveragedSchearModulFit
        {
            get
            {
                return this._averagedShearModulFit;
            }
        }

        private double _averagedBulkModulFit;
        public double AveragedBulkModulFit
        {
            get
            {
                return this._averagedBulkModulFit;
            }
        }

        public void SetAverageParameters(List<REK> CalculatedREK)
        {
            this._averagedEModul = 0;
            this._averagedNu = 0;
            this._averagedShearModul = 0;
            this._averagedBulkModul = 0;

            for(int n = 0; n < CalculatedREK.Count; n++)
            {
                this._averagedEModul += CalculatedREK[n].ClassicEModulus;
                this._averagedNu += CalculatedREK[n].ClassicTransverseContraction;
                this._averagedShearModul += CalculatedREK[n].ClassicShearModulus;
                this._averagedBulkModul += CalculatedREK[n].ClassicBulkModulus;
            }

            this._averagedEModul /= CalculatedREK.Count;
            this._averagedNu /= CalculatedREK.Count;
            this._averagedShearModul /= CalculatedREK.Count;
            this._averagedBulkModul /= CalculatedREK.Count;
        }

        public void SetAverageParametersFit()
        {
            this._averagedEModulFit = 0;
            this._averagedNuFit = 0;
            this._averagedShearModulFit = 0;
            this._averagedBulkModulFit = 0;

            for (int n = 0; n < DiffractionConstants.Count; n++)
            {
                this._averagedEModulFit += DiffractionConstants[n].ClassicEModulus;
                this._averagedNuFit += DiffractionConstants[n].ClassicTransverseContraction;
                this._averagedShearModulFit += DiffractionConstants[n].ClassicShearModulus;
                this._averagedBulkModulFit += DiffractionConstants[n].ClassicBulkModulus;
            }

            this._averagedEModulFit /= DiffractionConstants.Count;
            this._averagedNuFit /= DiffractionConstants.Count;
            this._averagedShearModulFit /= DiffractionConstants.Count;
            this._averagedBulkModulFit /= DiffractionConstants.Count;
        }

        public double GetFittingChi2Cubic(int Model, bool Compliance)
        {
            double Ret = 0;

            switch (Model)
            {
                case 0:
                    Ret = Fitting.Chi2.Chi2ClassicVoigtCubic(this);
                    break;
                case 1:
                    Ret = Fitting.Chi2.Chi2ClassicReussCubic(this);
                    break;
                case 2:
                    Ret = Fitting.Chi2.Chi2ClassicHillCubic(this);
                    break;
                case 3:
                    if (Compliance)
                    {
                        Ret = Fitting.Chi2.Chi2ClassicKroenerCubicCompliance(this);
                    }
                    else
                    {
                        Ret = Fitting.Chi2.Chi2ClassicKroenerCubicStiffness(this);
                    }
                    break;
                case 4:
                    if (Compliance)
                    {
                        Ret = Fitting.Chi2.Chi2ClassicDeWittCubicCompliance(this);
                    }
                    else
                    {
                        Ret = Fitting.Chi2.Chi2ClassicDeWittCubicStiffness(this);
                    }
                    break;
                case 5:
                    Ret = Fitting.Chi2.Chi2ClassicGeometricHillCubic(this);
                    break;
            }

            return Ret;
        }

        public double GetFittingChi2Hexagonal(int Model, bool Compliance)
        {
            double Ret = 0;

            switch (Model)
            {
                case 0:
                    Ret = Fitting.Chi2.Chi2ClassicVoigtType1(this);
                    break;
                case 1:
                    Ret = Fitting.Chi2.Chi2ClassicReussHexagonal(this);
                    break;
                case 2:
                    Ret = Fitting.Chi2.Chi2ClassicHillHexagonal(this);
                    break;
                case 3:
                    if (Compliance)
                    {
                        Ret = Fitting.Chi2.Chi2ClassicKroenerCubicCompliance(this);
                    }
                    else
                    {
                        Ret = Fitting.Chi2.Chi2ClassicKroenerCubicStiffness(this);
                    }
                    break;
                case 4:
                    if (Compliance)
                    {
                        Ret = Fitting.Chi2.Chi2ClassicDeWittCubicCompliance(this);
                    }
                    else
                    {
                        Ret = Fitting.Chi2.Chi2ClassicDeWittCubicStiffness(this);
                    }
                    break;
                case 5:
                    Ret = Fitting.Chi2.Chi2ClassicGeometricHillHexagonal(this);
                    break;
            }

            return Ret;
        }
        #endregion

        #region Kappa

        #region Cubic

        public double KappaCubicCompliance
        {
            get
            {
                double Ret = 1.0 / 3.0;

                Ret /= this.S11 + (2.0 * this.S12);

                return Ret;
            }
        }

        public double KappaCubicStiffness
        {
            get
            {
                double Ret = 1.0 / 3.0;

                Ret *= this.C11 + (2.0 * this.C12);

                return Ret;
            }
        }

        #region First Derivatives

        public double FirstDerivativeC11KappaCubic()
        {
            double Ret = 1;
            Ret *= 0.5;

            return Ret;
        }

        public double FirstDerivativeC12KappaCubic()
        {
            double Ret = -1.0;
            Ret *= 0.5;

            return Ret;
        }

        public double FirstDerivativeC44KappaCubic()
        {
            return 0.0;
        }

        public double FirstDerivativeS11KappaCubic()
        {
            double Ret = 1;
            double N = 3.0 * Math.Pow(this.S11 + (2.0 * this.S12), 2);
            Ret /= N;

            return Ret;
        }

        public double FirstDerivativeS12KappaCubic()
        {
            double Ret = 2;
            double N = 3.0 * Math.Pow(this.S11 + (2.0 * this.S12), 2);
            Ret /= N;

            return Ret;
        }

        public double FirstDerivativeS44KappaCubic()
        {
            return 0.0;
        }

        #endregion

        #endregion

        #endregion

        #region ShearModulus

        #region Kroener

        public double[] _kroenerShearModulus = { 0.0, 0.0, 0.0 };
        public double KroenerShearModulus
        {
            get
            {
                return this._kroenerShearModulus[0];
            }
            set
            {
                this._kroenerShearModulus[0] = value;
            }
        }

        #endregion

        #region DeWitt

        public double[] _deWittShearModulus = { 0.0, 0.0, 0.0 };
        public double DeWittShearModulus
        {
            get
            {
                return this._deWittShearModulus[0];
            }
            set
            {
                this._deWittShearModulus[0] = value;
            }
        }

        #endregion

        /// <summary>
        /// Finds all possible solutions for G with the method of Deiters und Macias-Salinas
        /// </summary>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <param name="gamma"></param>
        private double[] CalculateShearModulus(double alpha, double beta, double gamma)
        {
            double[] Ret = { 0, 0, 0 };
            double Xinfl = beta / 3.0;

            double FirstZeroCheck = Analysis.Fitting.ShearModulusPolynome.Y(alpha, beta, gamma, Xinfl);
            bool[] SetSolutions = { false, false, false };

            if (Math.Abs(FirstZeroCheck) < 0.001)
            {
                if (Xinfl <= 0)
                {
                    Ret[2] = Xinfl;
                    SetSolutions[2] = true;
                }
                else
                {
                    Ret[0] = Xinfl;
                    SetSolutions[0] = true;
                }
            }
            double D = Math.Pow(alpha, 2);
            D += 3.0 * beta;

            if (Math.Abs(D) < 0.001)
            {
                double SolTmp = Xinfl - Math.Pow(Analysis.Fitting.ShearModulusPolynome.Y(alpha, beta, gamma, Xinfl), 1.0 / 3.0);

                if (SolTmp <= 0)
                {
                    if (SetSolutions[2])
                    {
                        Ret[1] = SolTmp;
                        SetSolutions[1] = true;
                    }
                    else
                    {
                        Ret[2] = SolTmp;
                        SetSolutions[2] = true;
                    }
                }
                else
                {
                    if (SetSolutions[0])
                    {
                        Ret[1] = SolTmp;
                        SetSolutions[1] = true;
                    }
                    else
                    {
                        Ret[0] = SolTmp;
                        SetSolutions[0] = true;
                    }
                }
            }

            if (!SetSolutions[0] && !SetSolutions[1] && !SetSolutions[2])
            {
                int MaxIterations = 500;
                double StepValue = 0.0;
                if (D > 0)
                {
                    if (Analysis.Fitting.ShearModulusPolynome.Y(alpha, beta, gamma, Xinfl) > 0)
                    {
                        StepValue = Xinfl + Math.Sqrt(D);
                    }
                    else
                    {
                        StepValue = Xinfl - Math.Sqrt(D);
                    }
                }
                else
                {
                    StepValue = Xinfl;
                }

                bool Found = false;
                int IterationCount = 0;

                double LastYValue = Analysis.Fitting.ShearModulusPolynome.Y(alpha, beta, gamma, StepValue);
                while (!Found)
                {
                    StepValue = Analysis.Fitting.ShearModulusPolynome.HalleyIteration(StepValue, alpha, beta, gamma);
                    if (Math.Abs(Analysis.Fitting.ShearModulusPolynome.Y(alpha, beta, gamma, StepValue)) < 0.01)
                    {
                        if (StepValue <= 0)
                        {
                            Ret[2] = StepValue;
                            SetSolutions[2] = true;
                        }
                        else
                        {
                            Ret[0] = StepValue;
                            SetSolutions[0] = true;
                        }

                        Found = true;
                    }

                    if (IterationCount > MaxIterations)
                    {
                        Found = true;
                    }
                    if(Math.Abs(LastYValue - Analysis.Fitting.ShearModulusPolynome.Y(alpha, beta, gamma, StepValue)) < 0.0001)
                    {
                        if (StepValue <= 0)
                        {
                            Ret[2] = StepValue;
                            SetSolutions[2] = true;
                        }
                        else
                        {
                            Ret[0] = StepValue;
                            SetSolutions[0] = true;
                        }

                        Found = true;
                        break;
                    }
                    LastYValue = Analysis.Fitting.ShearModulusPolynome.Y(alpha, beta, gamma, StepValue);
                    IterationCount++;
                }
            }

            if (SetSolutions[0])
            {
                double[] LastTwoZeros = Analysis.Fitting.ShearModulusPolynome.GetLastZerosPolyDevision(alpha, beta, Ret[0]);

                if (LastTwoZeros[0] > LastTwoZeros[1])
                {
                    Ret[1] = LastTwoZeros[0];
                    Ret[2] = LastTwoZeros[1];
                }
                else
                {
                    Ret[1] = LastTwoZeros[1];
                    Ret[2] = LastTwoZeros[0];
                }
            }
            else if (SetSolutions[1])
            {
                double[] LastTwoZeros = Analysis.Fitting.ShearModulusPolynome.GetLastZerosPolyDevision(alpha, beta, Ret[1]);

                if (LastTwoZeros[0] > LastTwoZeros[1])
                {
                    Ret[0] = LastTwoZeros[0];
                    Ret[2] = LastTwoZeros[1];
                }
                else
                {
                    Ret[0] = LastTwoZeros[1];
                    Ret[2] = LastTwoZeros[0];
                }
            }
            else if (SetSolutions[2])
            {
                double[] LastTwoZeros = Analysis.Fitting.ShearModulusPolynome.GetLastZerosPolyDevision(alpha, beta, Ret[2]);
                if (LastTwoZeros[0] > LastTwoZeros[1])
                {
                    Ret[0] = LastTwoZeros[0];
                    Ret[1] = LastTwoZeros[1];
                }
                else
                {
                    Ret[0] = LastTwoZeros[1];
                    Ret[1] = LastTwoZeros[0];
                }
            }

            return Ret;
        }

        #region First Derivatives

        /// <summary>
        /// Caluclates the first derivative of the Shear modulus. 
        /// Caution depending on the parameter ranges it may happen, that the inversion fails
        /// </summary>
        /// <param name="alpha">alpha parameter</param>
        /// <param name="beta">beta parameter</param>
        /// <param name="gamma">gamma parameter</param>
        /// <param name="alphaFD">First derivative of the alpha parameter</param>
        /// <param name="betaFD">First derivative of the beta parameter</param>
        /// <param name="gammaFD">First derivative of the gamma parameter</param>
        /// <returns></returns>
        private double FirstDerivativeShearModulusCubic(double alpha, double beta, double gamma, double alphaFD, double betaFD, double gammaFD)
        {
            double[] ShearModulusTmp = this.CalculateShearModulus(alpha, beta, gamma);

            double Z = alphaFD * Math.Pow(ShearModulusTmp[0], 2);
            Z += betaFD * ShearModulusTmp[0];
            Z += gammaFD;

            double N = 3.0 * ShearModulusTmp[0];
            N -= 2.0 * alpha * ShearModulusTmp[0];
            N -= beta;

            double Ret = Z / N;

            return Ret;
        }

        #endregion

        #endregion

        #region Voigt Matrix components

        #region Stiffness matrix components

        public double C11
        {
            get
            {
                return this._stiffnessTensor[0, 0];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._stiffnessTensor[0, 0] = value;
                        this._stiffnessTensor[1, 1] = value;
                        this._stiffnessTensor[2, 2] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 0.5 * (this._stiffnessTensor[0, 0] - this._stiffnessTensor[0, 1]);
                            this._stiffnessTensor[3, 3] = IsoValue;
                            this._stiffnessTensor[4, 4] = IsoValue;
                            this._stiffnessTensor[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        double IsoValue1 = 0.5 * (value - this._stiffnessTensor[0, 1]);
                        this._stiffnessTensor[5, 5] = IsoValue1;
                        goto case 3;
                    case 3:
                        this._stiffnessTensor[0, 0] = value;
                        this._stiffnessTensor[1, 1] = value;
                        break;
                    case 4:
                        goto case 3;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._stiffnessTensor[0, 0] = value;
                        break;
                }
            }
        }
        public double C11Error
        {
            get
            {
                return this._stiffnessTensorError[0, 0];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._stiffnessTensorError[0, 0] = value;
                        this._stiffnessTensorError[1, 1] = value;
                        this._stiffnessTensorError[2, 2] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 0.5 * (this._stiffnessTensorError[0, 0] - this._stiffnessTensorError[0, 1]);
                            this._stiffnessTensorError[3, 3] = IsoValue;
                            this._stiffnessTensorError[4, 4] = IsoValue;
                            this._stiffnessTensorError[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        double IsoValue1 = 0.5 * (value - this._stiffnessTensorError[0, 1]);
                        this._stiffnessTensorError[5, 5] = IsoValue1;
                        goto case 3;
                    case 3:
                        this._stiffnessTensorError[0, 0] = value;
                        this._stiffnessTensorError[1, 1] = value;
                        break;
                    case 4:
                        goto case 3;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._stiffnessTensorError[0, 0] = value;
                        break;
                }
            }
        }

        public double C12
        {
            get
            {
                return this._stiffnessTensor[0, 1];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._stiffnessTensor[0, 1] = value;
                        this._stiffnessTensor[0, 2] = value;
                        this._stiffnessTensor[1, 2] = value;
                        this._stiffnessTensor[1, 0] = value;
                        this._stiffnessTensor[2, 0] = value;
                        this._stiffnessTensor[2, 1] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 0.5 * (this._stiffnessTensor[0, 0] - this._stiffnessTensor[0, 1]);
                            this._stiffnessTensor[3, 3] = IsoValue;
                            this._stiffnessTensor[4, 4] = IsoValue;
                            this._stiffnessTensor[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        double IsoValue1 = 0.5 * (this._stiffnessTensor[0, 0] - value);
                        this._stiffnessTensor[5, 5] = IsoValue1;
                        goto default;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._stiffnessTensor[0, 1] = value;
                        this._stiffnessTensor[1, 0] = value;
                        break;
                }
            }
        }
        public double C12Error
        {
            get
            {
                return this._stiffnessTensorError[0, 1];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._stiffnessTensorError[0, 1] = value;
                        this._stiffnessTensorError[0, 2] = value;
                        this._stiffnessTensorError[1, 2] = value;
                        this._stiffnessTensorError[1, 0] = value;
                        this._stiffnessTensorError[2, 0] = value;
                        this._stiffnessTensorError[2, 1] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 0.5 * (this._stiffnessTensorError[0, 0] - this._stiffnessTensorError[0, 1]);
                            this._stiffnessTensorError[3, 3] = IsoValue;
                            this._stiffnessTensorError[4, 4] = IsoValue;
                            this._stiffnessTensorError[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        double IsoValue1 = 0.5 * (this._stiffnessTensorError[0, 0] - value);
                        this._stiffnessTensorError[5, 5] = IsoValue1;
                        goto default;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._stiffnessTensorError[0, 1] = value;
                        this._stiffnessTensorError[1, 0] = value;
                        break;
                }
            }
        }

        public double C13
        {
            get
            {
                return this._stiffnessTensor[0, 2];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._stiffnessTensor[0, 1] = value;
                        this._stiffnessTensor[0, 2] = value;
                        this._stiffnessTensor[1, 2] = value;
                        this._stiffnessTensor[1, 0] = value;
                        this._stiffnessTensor[2, 0] = value;
                        this._stiffnessTensor[2, 1] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 0.5 * (this._stiffnessTensor[0, 0] - this._stiffnessTensor[0, 1]);
                            this._stiffnessTensor[3, 3] = IsoValue;
                            this._stiffnessTensor[4, 4] = IsoValue;
                            this._stiffnessTensor[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        this._stiffnessTensor[0, 2] = value;
                        this._stiffnessTensor[1, 2] = value;
                        this._stiffnessTensor[2, 0] = value;
                        this._stiffnessTensor[2, 1] = value;
                        break;
                    case 3:
                        goto case 2;
                    case 4:
                        goto case 2;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._stiffnessTensor[0, 2] = value;
                        this._stiffnessTensor[2, 0] = value;
                        break;
                }
            }
        }
        public double C13Error
        {
            get
            {
                return this._stiffnessTensorError[0, 2];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._stiffnessTensorError[0, 1] = value;
                        this._stiffnessTensorError[0, 2] = value;
                        this._stiffnessTensorError[1, 2] = value;
                        this._stiffnessTensorError[1, 0] = value;
                        this._stiffnessTensorError[2, 0] = value;
                        this._stiffnessTensorError[2, 1] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 0.5 * (this._stiffnessTensorError[0, 0] - this._stiffnessTensorError[0, 1]);
                            this._stiffnessTensorError[3, 3] = IsoValue;
                            this._stiffnessTensorError[4, 4] = IsoValue;
                            this._stiffnessTensorError[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        this._stiffnessTensorError[0, 2] = value;
                        this._stiffnessTensorError[1, 2] = value;
                        this._stiffnessTensorError[2, 0] = value;
                        this._stiffnessTensorError[2, 1] = value;
                        break;
                    case 3:
                        goto case 2;
                    case 4:
                        goto case 2;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._stiffnessTensorError[0, 2] = value;
                        this._stiffnessTensorError[2, 0] = value;
                        break;
                }
            }
        }

        public double C14
        {
            get
            {
                return this._stiffnessTensor[0, 3];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 5:
                        this._stiffnessTensor[0, 3] = value;
                        this._stiffnessTensor[1, 3] = -1 * value;
                        this._stiffnessTensor[4, 5] = value;
                        this._stiffnessTensor[3, 0] = value;
                        this._stiffnessTensor[3, 1] = -1 * value;
                        this._stiffnessTensor[5, 4] = value;
                        break;
                    case 6:
                        goto case 5;
                    case 9:
                        this._stiffnessTensor[0, 3] = value;
                        this._stiffnessTensor[3, 0] = value;
                        break;
                    default:
                        this._stiffnessTensor[0, 3] = 0;
                        this._stiffnessTensor[3, 0] = 0;
                        break;
                }
            }
        }
        public double C14Error
        {
            get
            {
                return this._stiffnessTensorError[0, 3];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 5:
                        this._stiffnessTensorError[0, 3] = value;
                        this._stiffnessTensorError[1, 3] = -1 * value;
                        this._stiffnessTensorError[4, 5] = value;
                        this._stiffnessTensorError[3, 0] = value;
                        this._stiffnessTensorError[3, 1] = -1 * value;
                        this._stiffnessTensorError[5, 4] = value;
                        break;
                    case 6:
                        goto case 5;
                    case 9:
                        this._stiffnessTensorError[0, 3] = value;
                        this._stiffnessTensorError[3, 0] = value;
                        break;
                    default:
                        this._stiffnessTensorError[0, 3] = 0;
                        this._stiffnessTensorError[3, 0] = 0;
                        break;
                }
            }
        }

        public double C15
        {
            get
            {
                return this._stiffnessTensor[0, 4];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 6:
                        this._stiffnessTensor[0, 4] = value;
                        this._stiffnessTensor[1, 4] = -1 * value;
                        this._stiffnessTensor[3, 5] = -1 * value;
                        this._stiffnessTensor[4, 0] = value;
                        this._stiffnessTensor[4, 1] = -1 * value;
                        this._stiffnessTensor[5, 3] = -1 * value;
                        break;
                    case 9:
                        this._stiffnessTensor[0, 4] = value;
                        this._stiffnessTensor[4, 0] = value;
                        break;
                    default:
                        this._stiffnessTensor[0, 4] = 0;
                        this._stiffnessTensor[4, 0] = 0;
                        break;
                }
            }
        }
        public double C15Error
        {
            get
            {
                return this._stiffnessTensorError[0, 4];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 6:
                        this._stiffnessTensorError[0, 4] = value;
                        this._stiffnessTensorError[1, 4] = -1 * value;
                        this._stiffnessTensorError[3, 5] = -1 * value;
                        this._stiffnessTensorError[4, 0] = value;
                        this._stiffnessTensorError[4, 1] = -1 * value;
                        this._stiffnessTensorError[5, 3] = -1 * value;
                        break;
                    case 9:
                        this._stiffnessTensorError[0, 4] = value;
                        this._stiffnessTensorError[4, 0] = value;
                        break;
                    default:
                        this._stiffnessTensorError[0, 4] = 0;
                        this._stiffnessTensorError[4, 0] = 0;
                        break;
                }
            }
        }

        public double C16
        {
            get
            {
                return this._stiffnessTensor[0, 5];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 4:
                        this._stiffnessTensor[0, 5] = value;
                        this._stiffnessTensor[1, 5] = -1 * value;
                        this._stiffnessTensor[5, 0] = value;
                        this._stiffnessTensor[5, 1] = -1 * value;
                        break;
                    case 8:
                        this._stiffnessTensor[0, 5] = value;
                        this._stiffnessTensor[5, 0] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._stiffnessTensor[0, 5] = 0;
                        this._stiffnessTensor[5, 0] = 0;
                        break;
                }
            }
        }
        public double C16Error
        {
            get
            {
                return this._stiffnessTensorError[0, 5];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 4:
                        this._stiffnessTensorError[0, 5] = value;
                        this._stiffnessTensorError[1, 5] = -1 * value;
                        this._stiffnessTensorError[5, 0] = value;
                        this._stiffnessTensorError[5, 1] = -1 * value;
                        break;
                    case 8:
                        this._stiffnessTensorError[0, 5] = value;
                        this._stiffnessTensorError[5, 0] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._stiffnessTensorError[0, 5] = 0;
                        this._stiffnessTensorError[5, 0] = 0;
                        break;
                }
            }
        }

        public double C21
        {
            get
            {
                return this._stiffnessTensor[1, 0];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._stiffnessTensor[0, 1] = value;
                        this._stiffnessTensor[0, 2] = value;
                        this._stiffnessTensor[1, 2] = value;
                        this._stiffnessTensor[1, 0] = value;
                        this._stiffnessTensor[2, 0] = value;
                        this._stiffnessTensor[2, 1] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 0.5 * (this._stiffnessTensor[0, 0] - this._stiffnessTensor[0, 1]);
                            this._stiffnessTensor[3, 3] = IsoValue;
                            this._stiffnessTensor[4, 4] = IsoValue;
                            this._stiffnessTensor[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        double IsoValue1 = 0.5 * (this._stiffnessTensor[0, 0] - value);
                        this._stiffnessTensor[5, 5] = IsoValue1;
                        goto default;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._stiffnessTensor[0, 1] = value;
                        this._stiffnessTensor[1, 0] = value;
                        break;
                }
            }
        }
        public double C21Error
        {
            get
            {
                return this._stiffnessTensorError[1, 0];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._stiffnessTensorError[0, 1] = value;
                        this._stiffnessTensorError[0, 2] = value;
                        this._stiffnessTensorError[1, 2] = value;
                        this._stiffnessTensorError[1, 0] = value;
                        this._stiffnessTensorError[2, 0] = value;
                        this._stiffnessTensorError[2, 1] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 0.5 * (this._stiffnessTensorError[0, 0] - this._stiffnessTensorError[0, 1]);
                            this._stiffnessTensorError[3, 3] = IsoValue;
                            this._stiffnessTensorError[4, 4] = IsoValue;
                            this._stiffnessTensorError[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        double IsoValue1 = 0.5 * (this._stiffnessTensorError[0, 0] - value);
                        this._stiffnessTensorError[5, 5] = IsoValue1;
                        goto default;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._stiffnessTensorError[0, 1] = value;
                        this._stiffnessTensorError[1, 0] = value;
                        break;
                }
            }
        }

        public double C22
        {
            get
            {
                return this._stiffnessTensor[1, 1];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._stiffnessTensor[0, 0] = value;
                        this._stiffnessTensor[1, 1] = value;
                        this._stiffnessTensor[2, 2] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 0.5 * (this._stiffnessTensor[0, 0] - this._stiffnessTensor[0, 1]);
                            this._stiffnessTensor[3, 3] = IsoValue;
                            this._stiffnessTensor[4, 4] = IsoValue;
                            this._stiffnessTensor[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        double IsoValue1 = 0.5 * (value - this._stiffnessTensor[0, 1]);
                        this._stiffnessTensor[5, 5] = IsoValue1;
                        goto case 3;
                    case 3:
                        this._stiffnessTensor[0, 0] = value;
                        this._stiffnessTensor[1, 1] = value;
                        break;
                    case 4:
                        goto case 3;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._stiffnessTensor[1, 1] = value;
                        break;
                }
            }
        }
        public double C22Error
        {
            get
            {
                return this._stiffnessTensorError[1, 1];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._stiffnessTensorError[0, 0] = value;
                        this._stiffnessTensorError[1, 1] = value;
                        this._stiffnessTensorError[2, 2] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 0.5 * (this._stiffnessTensorError[0, 0] - this._stiffnessTensorError[0, 1]);
                            this._stiffnessTensorError[3, 3] = IsoValue;
                            this._stiffnessTensorError[4, 4] = IsoValue;
                            this._stiffnessTensorError[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        double IsoValue1 = 0.5 * (value - this._stiffnessTensorError[0, 1]);
                        this._stiffnessTensorError[5, 5] = IsoValue1;
                        goto case 3;
                    case 3:
                        this._stiffnessTensorError[0, 0] = value;
                        this._stiffnessTensorError[1, 1] = value;
                        break;
                    case 4:
                        goto case 3;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._stiffnessTensorError[1, 1] = value;
                        break;
                }
            }
        }

        public double C23
        {
            get
            {
                return this._stiffnessTensor[1, 2];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._stiffnessTensor[0, 1] = value;
                        this._stiffnessTensor[0, 2] = value;
                        this._stiffnessTensor[1, 2] = value;
                        this._stiffnessTensor[1, 0] = value;
                        this._stiffnessTensor[2, 0] = value;
                        this._stiffnessTensor[2, 1] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 0.5 * (this._stiffnessTensor[0, 0] - this._stiffnessTensor[0, 1]);
                            this._stiffnessTensor[3, 3] = IsoValue;
                            this._stiffnessTensor[4, 4] = IsoValue;
                            this._stiffnessTensor[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        this._stiffnessTensor[0, 2] = value;
                        this._stiffnessTensor[1, 2] = value;
                        this._stiffnessTensor[2, 0] = value;
                        this._stiffnessTensor[2, 1] = value;
                        break;
                    case 3:
                        goto case 2;
                    case 4:
                        goto case 2;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._stiffnessTensor[1, 2] = value;
                        this._stiffnessTensor[2, 1] = value;
                        break;
                }
            }
        }
        public double C23Error
        {
            get
            {
                return this._stiffnessTensorError[1, 2];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._stiffnessTensorError[0, 1] = value;
                        this._stiffnessTensorError[0, 2] = value;
                        this._stiffnessTensorError[1, 2] = value;
                        this._stiffnessTensorError[1, 0] = value;
                        this._stiffnessTensorError[2, 0] = value;
                        this._stiffnessTensorError[2, 1] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 0.5 * (this._stiffnessTensorError[0, 0] - this._stiffnessTensorError[0, 1]);
                            this._stiffnessTensorError[3, 3] = IsoValue;
                            this._stiffnessTensorError[4, 4] = IsoValue;
                            this._stiffnessTensorError[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        this._stiffnessTensorError[0, 2] = value;
                        this._stiffnessTensorError[1, 2] = value;
                        this._stiffnessTensorError[2, 0] = value;
                        this._stiffnessTensorError[2, 1] = value;
                        break;
                    case 3:
                        goto case 2;
                    case 4:
                        goto case 2;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._stiffnessTensorError[1, 2] = value;
                        this._stiffnessTensorError[2, 1] = value;
                        break;
                }
            }
        }

        public double C24
        {
            get
            {
                return this._stiffnessTensor[1, 3];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 5:
                        this._stiffnessTensor[0, 3] = -1 * value;
                        this._stiffnessTensor[1, 3] = value;
                        this._stiffnessTensor[4, 5] = -1 * value;
                        this._stiffnessTensor[3, 0] = -1 * value;
                        this._stiffnessTensor[3, 1] = value;
                        this._stiffnessTensor[5, 4] = -1 * value;
                        break;
                    case 6:
                        goto case 5;
                    case 9:
                        this._stiffnessTensor[1, 3] = value;
                        this._stiffnessTensor[3, 1] = value;
                        break;
                    default:
                        this._stiffnessTensor[1, 3] = 0;
                        this._stiffnessTensor[3, 1] = 0;
                        break;
                }
            }
        }
        public double C24Error
        {
            get
            {
                return this._stiffnessTensorError[1, 3];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 5:
                        this._stiffnessTensorError[0, 3] = -1 * value;
                        this._stiffnessTensorError[1, 3] = value;
                        this._stiffnessTensorError[4, 5] = -1 * value;
                        this._stiffnessTensorError[3, 0] = -1 * value;
                        this._stiffnessTensorError[3, 1] = value;
                        this._stiffnessTensorError[5, 4] = -1 * value;
                        break;
                    case 6:
                        goto case 5;
                    case 9:
                        this._stiffnessTensorError[1, 3] = value;
                        this._stiffnessTensorError[3, 1] = value;
                        break;
                    default:
                        this._stiffnessTensorError[1, 3] = 0;
                        this._stiffnessTensorError[3, 1] = 0;
                        break;
                }
            }
        }

        public double C25
        {
            get
            {
                return this._stiffnessTensor[1, 4];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 6:
                        this._stiffnessTensor[0, 4] = -1 * value;
                        this._stiffnessTensor[1, 4] = value;
                        this._stiffnessTensor[3, 5] = value;
                        this._stiffnessTensor[4, 0] = -1 * value;
                        this._stiffnessTensor[4, 1] = value;
                        this._stiffnessTensor[5, 3] = value;
                        break;
                    case 9:
                        this._stiffnessTensor[1, 4] = value;
                        this._stiffnessTensor[4, 1] = value;
                        break;
                    default:
                        this._stiffnessTensor[1, 4] = 0;
                        this._stiffnessTensor[4, 1] = 0;
                        break;
                }
            }
        }
        public double C25Error
        {
            get
            {
                return this._stiffnessTensorError[1, 4];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 6:
                        this._stiffnessTensorError[0, 4] = -1 * value;
                        this._stiffnessTensorError[1, 4] = value;
                        this._stiffnessTensorError[3, 5] = value;
                        this._stiffnessTensorError[4, 0] = -1 * value;
                        this._stiffnessTensorError[4, 1] = value;
                        this._stiffnessTensorError[5, 3] = value;
                        break;
                    case 9:
                        this._stiffnessTensorError[1, 4] = value;
                        this._stiffnessTensorError[4, 1] = value;
                        break;
                    default:
                        this._stiffnessTensorError[1, 4] = 0;
                        this._stiffnessTensorError[4, 1] = 0;
                        break;
                }
            }
        }

        public double C26
        {
            get
            {
                return this._stiffnessTensor[1, 5];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 4:
                        this._stiffnessTensor[0, 5] = -1 * value;
                        this._stiffnessTensor[1, 5] = value;
                        this._stiffnessTensor[5, 0] = -1 * value;
                        this._stiffnessTensor[5, 1] = value;
                        break;
                    case 8:
                        this._stiffnessTensor[1, 5] = value;
                        this._stiffnessTensor[5, 1] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._stiffnessTensor[1, 5] = 0;
                        this._stiffnessTensor[5, 1] = 0;
                        break;
                }
            }
        }
        public double C26Error
        {
            get
            {
                return this._stiffnessTensorError[1, 5];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 4:
                        this._stiffnessTensorError[0, 5] = -1 * value;
                        this._stiffnessTensorError[1, 5] = value;
                        this._stiffnessTensorError[5, 0] = -1 * value;
                        this._stiffnessTensorError[5, 1] = value;
                        break;
                    case 8:
                        this._stiffnessTensorError[1, 5] = value;
                        this._stiffnessTensorError[5, 1] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._stiffnessTensorError[1, 5] = 0;
                        this._stiffnessTensorError[5, 1] = 0;
                        break;
                }
            }
        }

        public double C31
        {
            get
            {
                return this._stiffnessTensor[2, 0];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._stiffnessTensor[0, 1] = value;
                        this._stiffnessTensor[0, 2] = value;
                        this._stiffnessTensor[1, 2] = value;
                        this._stiffnessTensor[1, 0] = value;
                        this._stiffnessTensor[2, 0] = value;
                        this._stiffnessTensor[2, 1] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 0.5 * (this._stiffnessTensor[0, 0] - this._stiffnessTensor[0, 1]);
                            this._stiffnessTensor[3, 3] = IsoValue;
                            this._stiffnessTensor[4, 4] = IsoValue;
                            this._stiffnessTensor[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        this._stiffnessTensor[0, 2] = value;
                        this._stiffnessTensor[1, 2] = value;
                        this._stiffnessTensor[2, 0] = value;
                        this._stiffnessTensor[2, 1] = value;
                        break;
                    case 3:
                        goto case 2;
                    case 4:
                        goto case 2;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._stiffnessTensor[0, 2] = value;
                        this._stiffnessTensor[2, 0] = value;
                        break;
                }
            }
        }
        public double C31Error
        {
            get
            {
                return this._stiffnessTensorError[2, 0];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._stiffnessTensorError[0, 1] = value;
                        this._stiffnessTensorError[0, 2] = value;
                        this._stiffnessTensorError[1, 2] = value;
                        this._stiffnessTensorError[1, 0] = value;
                        this._stiffnessTensorError[2, 0] = value;
                        this._stiffnessTensorError[2, 1] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 0.5 * (this._stiffnessTensorError[0, 0] - this._stiffnessTensorError[0, 1]);
                            this._stiffnessTensorError[3, 3] = IsoValue;
                            this._stiffnessTensorError[4, 4] = IsoValue;
                            this._stiffnessTensorError[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        this._stiffnessTensorError[0, 2] = value;
                        this._stiffnessTensorError[1, 2] = value;
                        this._stiffnessTensorError[2, 0] = value;
                        this._stiffnessTensorError[2, 1] = value;
                        break;
                    case 3:
                        goto case 2;
                    case 4:
                        goto case 2;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._stiffnessTensorError[0, 2] = value;
                        this._stiffnessTensorError[2, 0] = value;
                        break;
                }
            }
        }

        public double C32
        {
            get
            {
                return this._stiffnessTensor[2, 1];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._stiffnessTensor[0, 1] = value;
                        this._stiffnessTensor[0, 2] = value;
                        this._stiffnessTensor[1, 2] = value;
                        this._stiffnessTensor[1, 0] = value;
                        this._stiffnessTensor[2, 0] = value;
                        this._stiffnessTensor[2, 1] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 0.5 * (this._stiffnessTensor[0, 0] - this._stiffnessTensor[0, 1]);
                            this._stiffnessTensor[3, 3] = IsoValue;
                            this._stiffnessTensor[4, 4] = IsoValue;
                            this._stiffnessTensor[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        this._stiffnessTensor[0, 2] = value;
                        this._stiffnessTensor[1, 2] = value;
                        this._stiffnessTensor[2, 0] = value;
                        this._stiffnessTensor[2, 1] = value;
                        break;
                    case 3:
                        goto case 2;
                    case 4:
                        goto case 2;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._stiffnessTensor[1, 2] = value;
                        this._stiffnessTensor[2, 1] = value;
                        break;
                }
            }
        }
        public double C32Error
        {
            get
            {
                return this._stiffnessTensorError[2, 1];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._stiffnessTensorError[0, 1] = value;
                        this._stiffnessTensorError[0, 2] = value;
                        this._stiffnessTensorError[1, 2] = value;
                        this._stiffnessTensorError[1, 0] = value;
                        this._stiffnessTensorError[2, 0] = value;
                        this._stiffnessTensorError[2, 1] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 0.5 * (this._stiffnessTensorError[0, 0] - this._stiffnessTensorError[0, 1]);
                            this._stiffnessTensorError[3, 3] = IsoValue;
                            this._stiffnessTensorError[4, 4] = IsoValue;
                            this._stiffnessTensorError[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        this._stiffnessTensorError[0, 2] = value;
                        this._stiffnessTensorError[1, 2] = value;
                        this._stiffnessTensorError[2, 0] = value;
                        this._stiffnessTensorError[2, 1] = value;
                        break;
                    case 3:
                        goto case 2;
                    case 4:
                        goto case 2;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._stiffnessTensorError[1, 2] = value;
                        this._stiffnessTensorError[2, 1] = value;
                        break;
                }
            }
        }

        public double C33
        {
            get
            {
                return this._stiffnessTensor[2, 2];
            }
            set
            {
                switch(this._symmetry)
                {
                    case 1:
                        this._stiffnessTensor[0, 0] = value;
                        this._stiffnessTensor[1, 1] = value;
                        this._stiffnessTensor[2, 2] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 0.5 * (this._stiffnessTensor[0, 0] - this._stiffnessTensor[0, 1]);
                            this._stiffnessTensor[3, 3] = IsoValue;
                            this._stiffnessTensor[4, 4] = IsoValue;
                            this._stiffnessTensor[5, 5] = IsoValue;
                        }
                        break;
                    default:
                        this._stiffnessTensor[2, 2] = value;
                        break;
                }
            }
        }
        public double C33Error
        {
            get
            {
                return this._stiffnessTensorError[2, 2];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._stiffnessTensorError[0, 0] = value;
                        this._stiffnessTensorError[1, 1] = value;
                        this._stiffnessTensorError[2, 2] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 0.5 * (this._stiffnessTensorError[0, 0] - this._stiffnessTensorError[0, 1]);
                            this._stiffnessTensorError[3, 3] = IsoValue;
                            this._stiffnessTensorError[4, 4] = IsoValue;
                            this._stiffnessTensorError[5, 5] = IsoValue;
                        }
                        break;
                    default:
                        this._stiffnessTensorError[2, 2] = value;
                        break;
                }
            }
        }

        public double C34
        {
            get
            {
                return this._stiffnessTensor[2, 3];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 9:
                        this._stiffnessTensor[2, 3] = value;
                        this._stiffnessTensor[3, 2] = value;
                        break;
                    default:
                        this._stiffnessTensor[2, 3] = 0;
                        this._stiffnessTensor[3, 2] = 0;
                        break;
                }
            }
        }
        public double C34Error
        {
            get
            {
                return this._stiffnessTensorError[2, 3];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 9:
                        this._stiffnessTensorError[2, 3] = value;
                        this._stiffnessTensorError[3, 2] = value;
                        break;
                    default:
                        this._stiffnessTensorError[2, 3] = 0;
                        this._stiffnessTensorError[3, 2] = 0;
                        break;
                }
            }
        }

        public double C35
        {
            get
            {
                return this._stiffnessTensor[2, 4];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 9:
                        this._stiffnessTensor[2, 4] = value;
                        this._stiffnessTensor[4, 2] = value;
                        break;
                    default:
                        this._stiffnessTensor[2, 4] = 0;
                        this._stiffnessTensor[4, 2] = 0;
                        break;
                }
            }
        }
        public double C35Error
        {
            get
            {
                return this._stiffnessTensorError[2, 4];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 9:
                        this._stiffnessTensorError[2, 4] = value;
                        this._stiffnessTensorError[4, 2] = value;
                        break;
                    default:
                        this._stiffnessTensorError[2, 4] = 0;
                        this._stiffnessTensorError[4, 2] = 0;
                        break;
                }
            }
        }

        public double C36
        {
            get
            {
                return this._stiffnessTensor[2, 5];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 8:
                        this._stiffnessTensor[2, 5] = value;
                        this._stiffnessTensor[5, 2] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._stiffnessTensor[2, 5] = 0;
                        this._stiffnessTensor[5, 2] = 0;
                        break;
                }
            }
        }
        public double C36Error
        {
            get
            {
                return this._stiffnessTensorError[2, 5];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 8:
                        this._stiffnessTensorError[2, 5] = value;
                        this._stiffnessTensorError[5, 2] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._stiffnessTensorError[2, 5] = 0;
                        this._stiffnessTensorError[5, 2] = 0;
                        break;
                }
            }
        }

        public double C41
        {
            get
            {
                return this._stiffnessTensor[3, 0];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 5:
                        this._stiffnessTensor[0, 3] = value;
                        this._stiffnessTensor[1, 3] = -1 * value;
                        this._stiffnessTensor[4, 5] = value;
                        this._stiffnessTensor[3, 0] = value;
                        this._stiffnessTensor[3, 1] = -1 * value;
                        this._stiffnessTensor[5, 4] = value;
                        break;
                    case 6:
                        goto case 5;
                    case 9:
                        this._stiffnessTensor[0, 3] = value;
                        this._stiffnessTensor[3, 0] = value;
                        break;
                    default:
                        this._stiffnessTensor[0, 3] = 0;
                        this._stiffnessTensor[3, 0] = 0;
                        break;
                }
            }
        }
        public double C41Error
        {
            get
            {
                return this._stiffnessTensorError[3, 0];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 5:
                        this._stiffnessTensorError[0, 3] = value;
                        this._stiffnessTensorError[1, 3] = -1 * value;
                        this._stiffnessTensorError[4, 5] = value;
                        this._stiffnessTensorError[3, 0] = value;
                        this._stiffnessTensorError[3, 1] = -1 * value;
                        this._stiffnessTensorError[5, 4] = value;
                        break;
                    case 6:
                        goto case 5;
                    case 9:
                        this._stiffnessTensorError[0, 3] = value;
                        this._stiffnessTensorError[3, 0] = value;
                        break;
                    default:
                        this._stiffnessTensorError[0, 3] = 0;
                        this._stiffnessTensorError[3, 0] = 0;
                        break;
                }
            }
        }

        public double C42
        {
            get
            {
                return this._stiffnessTensor[3, 1];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 5:
                        this._stiffnessTensor[0, 3] = -1 * value;
                        this._stiffnessTensor[1, 3] = value;
                        this._stiffnessTensor[4, 5] = -1 * value;
                        this._stiffnessTensor[3, 0] = -1 * value;
                        this._stiffnessTensor[3, 1] = value;
                        this._stiffnessTensor[5, 4] = -1 * value;
                        break;
                    case 6:
                        goto case 5;
                    case 9:
                        this._stiffnessTensor[1, 3] = value;
                        this._stiffnessTensor[3, 1] = value;
                        break;
                    default:
                        this._stiffnessTensor[1, 3] = 0;
                        this._stiffnessTensor[3, 1] = 0;
                        break;
                }
            }
        }
        public double C42Error
        {
            get
            {
                return this._stiffnessTensorError[3, 1];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 5:
                        this._stiffnessTensorError[0, 3] = -1 * value;
                        this._stiffnessTensorError[1, 3] = value;
                        this._stiffnessTensorError[4, 5] = -1 * value;
                        this._stiffnessTensorError[3, 0] = -1 * value;
                        this._stiffnessTensorError[3, 1] = value;
                        this._stiffnessTensorError[5, 4] = -1 * value;
                        break;
                    case 6:
                        goto case 5;
                    case 9:
                        this._stiffnessTensorError[1, 3] = value;
                        this._stiffnessTensorError[3, 1] = value;
                        break;
                    default:
                        this._stiffnessTensorError[1, 3] = 0;
                        this._stiffnessTensorError[3, 1] = 0;
                        break;
                }
            }
        }

        public double C43
        {
            get
            {
                return this._stiffnessTensor[3, 2];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 9:
                        this._stiffnessTensor[2, 3] = value;
                        this._stiffnessTensor[3, 2] = value;
                        break;
                    default:
                        this._stiffnessTensor[2, 3] = 0;
                        this._stiffnessTensor[3, 2] = 0;
                        break;
                }
            }
        }
        public double C43Error
        {
            get
            {
                return this._stiffnessTensorError[3, 2];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 9:
                        this._stiffnessTensorError[2, 3] = value;
                        this._stiffnessTensorError[3, 2] = value;
                        break;
                    default:
                        this._stiffnessTensorError[2, 3] = 0;
                        this._stiffnessTensorError[3, 2] = 0;
                        break;
                }
            }
        }

        public double C44
        {
            get
            {
                return this._stiffnessTensor[3, 3];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        if(!this.IsIsotropic)
                        {
                            this._stiffnessTensor[3, 3] = value;
                            this._stiffnessTensor[4, 4] = value;
                            this._stiffnessTensor[5, 5] = value;
                        }
                        break;
                    case 2:
                        this._stiffnessTensor[3, 3] = value;
                        this._stiffnessTensor[4, 4] = value;
                        break;
                    case 3:
                        goto case 2;
                    case 4:
                        goto case 2;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._stiffnessTensor[3, 3] = value;
                        break;
                }
            }
        }
        public double C44Error
        {
            get
            {
                return this._stiffnessTensorError[3, 3];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        if (!this.IsIsotropic)
                        {
                            this._stiffnessTensorError[3, 3] = value;
                            this._stiffnessTensorError[4, 4] = value;
                            this._stiffnessTensorError[5, 5] = value;
                        }
                        break;
                    case 2:
                        this._stiffnessTensorError[3, 3] = value;
                        this._stiffnessTensorError[4, 4] = value;
                        break;
                    case 3:
                        goto case 2;
                    case 4:
                        goto case 2;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._stiffnessTensorError[3, 3] = value;
                        break;
                }
            }
        }

        public double C45
        {
            get
            {
                return this._stiffnessTensor[3, 4];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 8:
                        this._stiffnessTensor[3, 4] = value;
                        this._stiffnessTensor[4, 3] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._stiffnessTensor[3, 4] = 0;
                        this._stiffnessTensor[4, 3] = 0;
                        break;
                }
            }
        }
        public double C45Error
        {
            get
            {
                return this._stiffnessTensorError[3, 4];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 8:
                        this._stiffnessTensorError[3, 4] = value;
                        this._stiffnessTensorError[4, 3] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._stiffnessTensorError[3, 4] = 0;
                        this._stiffnessTensorError[4, 3] = 0;
                        break;
                }
            }
        }

        public double C46
        {
            get
            {
                return this._stiffnessTensor[3, 5];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 6:
                        this._stiffnessTensor[0, 4] = -1 * value;
                        this._stiffnessTensor[1, 4] = value;
                        this._stiffnessTensor[3, 5] = value;
                        this._stiffnessTensor[4, 0] = -1 * value;
                        this._stiffnessTensor[4, 1] = value;
                        this._stiffnessTensor[5, 3] = value;
                        break;
                    case 9:
                        this._stiffnessTensor[3, 5] = value;
                        this._stiffnessTensor[5, 3] = value;
                        break;
                    default:
                        this._stiffnessTensor[3, 5] = 0;
                        this._stiffnessTensor[5, 3] = 0;
                        break;
                }
            }
        }
        public double C46Error
        {
            get
            {
                return this._stiffnessTensorError[3, 5];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 6:
                        this._stiffnessTensorError[0, 4] = -1 * value;
                        this._stiffnessTensorError[1, 4] = value;
                        this._stiffnessTensorError[3, 5] = value;
                        this._stiffnessTensorError[4, 0] = -1 * value;
                        this._stiffnessTensorError[4, 1] = value;
                        this._stiffnessTensorError[5, 3] = value;
                        break;
                    case 9:
                        this._stiffnessTensorError[3, 5] = value;
                        this._stiffnessTensorError[5, 3] = value;
                        break;
                    default:
                        this._stiffnessTensorError[3, 5] = 0;
                        this._stiffnessTensorError[5, 3] = 0;
                        break;
                }
            }
        }

        public double C51
        {
            get
            {
                return this._stiffnessTensor[4, 0];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 6:
                        this._stiffnessTensor[0, 4] = value;
                        this._stiffnessTensor[1, 4] = -1 * value;
                        this._stiffnessTensor[3, 5] = -1 * value;
                        this._stiffnessTensor[4, 0] = value;
                        this._stiffnessTensor[4, 1] = -1 * value;
                        this._stiffnessTensor[5, 3] = -1 * value;
                        break;
                    case 9:
                        this._stiffnessTensor[0, 4] = value;
                        this._stiffnessTensor[4, 0] = value;
                        break;
                    default:
                        this._stiffnessTensor[0, 4] = 0;
                        this._stiffnessTensor[4, 0] = 0;
                        break;
                }
            }
        }
        public double C51Error
        {
            get
            {
                return this._stiffnessTensorError[4, 0];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 6:
                        this._stiffnessTensorError[0, 4] = value;
                        this._stiffnessTensorError[1, 4] = -1 * value;
                        this._stiffnessTensorError[3, 5] = -1 * value;
                        this._stiffnessTensorError[4, 0] = value;
                        this._stiffnessTensorError[4, 1] = -1 * value;
                        this._stiffnessTensorError[5, 3] = -1 * value;
                        break;
                    case 9:
                        this._stiffnessTensorError[0, 4] = value;
                        this._stiffnessTensorError[4, 0] = value;
                        break;
                    default:
                        this._stiffnessTensorError[0, 4] = 0;
                        this._stiffnessTensorError[4, 0] = 0;
                        break;
                }
            }
        }

        public double C52
        {
            get
            {
                return this._stiffnessTensor[4, 1];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 6:
                        this._stiffnessTensor[0, 4] = -1 * value;
                        this._stiffnessTensor[1, 4] = value;
                        this._stiffnessTensor[3, 5] = value;
                        this._stiffnessTensor[4, 0] = -1 * value;
                        this._stiffnessTensor[4, 1] = value;
                        this._stiffnessTensor[5, 3] = value;
                        break;
                    case 9:
                        this._stiffnessTensor[1, 4] = value;
                        this._stiffnessTensor[4, 1] = value;
                        break;
                    default:
                        this._stiffnessTensor[1, 4] = 0;
                        this._stiffnessTensor[4, 1] = 0;
                        break;
                }
            }
        }
        public double C52Error
        {
            get
            {
                return this._stiffnessTensorError[4, 1];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 6:
                        this._stiffnessTensorError[0, 4] = -1 * value;
                        this._stiffnessTensorError[1, 4] = value;
                        this._stiffnessTensorError[3, 5] = value;
                        this._stiffnessTensorError[4, 0] = -1 * value;
                        this._stiffnessTensorError[4, 1] = value;
                        this._stiffnessTensorError[5, 3] = value;
                        break;
                    case 9:
                        this._stiffnessTensorError[1, 4] = value;
                        this._stiffnessTensorError[4, 1] = value;
                        break;
                    default:
                        this._stiffnessTensorError[1, 4] = 0;
                        this._stiffnessTensorError[4, 1] = 0;
                        break;
                }
            }
        }

        public double C53
        {
            get
            {
                return this._stiffnessTensor[4, 2];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 9:
                        this._stiffnessTensor[2, 4] = value;
                        this._stiffnessTensor[4, 2] = value;
                        break;
                    default:
                        this._stiffnessTensor[2, 4] = 0;
                        this._stiffnessTensor[4, 2] = 0;
                        break;
                }
            }
        }
        public double C53Error
        {
            get
            {
                return this._stiffnessTensorError[4, 2];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 9:
                        this._stiffnessTensorError[2, 4] = value;
                        this._stiffnessTensorError[4, 2] = value;
                        break;
                    default:
                        this._stiffnessTensorError[2, 4] = 0;
                        this._stiffnessTensorError[4, 2] = 0;
                        break;
                }
            }
        }

        public double C54
        {
            get
            {
                return this._stiffnessTensor[4, 3];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 8:
                        this._stiffnessTensor[3, 4] = value;
                        this._stiffnessTensor[4, 3] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._stiffnessTensor[3, 4] = 0;
                        this._stiffnessTensor[4, 3] = 0;
                        break;
                }
            }
        }
        public double C54Error
        {
            get
            {
                return this._stiffnessTensorError[4, 3];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 8:
                        this._stiffnessTensorError[3, 4] = value;
                        this._stiffnessTensorError[4, 3] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._stiffnessTensorError[3, 4] = 0;
                        this._stiffnessTensorError[4, 3] = 0;
                        break;
                }
            }
        }

        public double C55
        {
            get
            {
                return this._stiffnessTensor[4, 4];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        if (!this.IsIsotropic)
                        {
                            this._stiffnessTensor[3, 3] = value;
                            this._stiffnessTensor[4, 4] = value;
                            this._stiffnessTensor[5, 5] = value;
                        }
                        break;
                    case 2:
                        this._stiffnessTensor[3, 3] = value;
                        this._stiffnessTensor[4, 4] = value;
                        break;
                    case 3:
                        goto case 2;
                    case 4:
                        goto case 2;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._stiffnessTensor[4, 4] = value;
                        break;
                }
            }
        }
        public double C55Error
        {
            get
            {
                return this._stiffnessTensorError[4, 4];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        if (!this.IsIsotropic)
                        {
                            this._stiffnessTensorError[3, 3] = value;
                            this._stiffnessTensorError[4, 4] = value;
                            this._stiffnessTensorError[5, 5] = value;
                        }
                        break;
                    case 2:
                        this._stiffnessTensorError[3, 3] = value;
                        this._stiffnessTensorError[4, 4] = value;
                        break;
                    case 3:
                        goto case 2;
                    case 4:
                        goto case 2;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._stiffnessTensorError[4, 4] = value;
                        break;
                }
            }
        }

        public double C56
        {
            get
            {
                return this._stiffnessTensor[4, 5];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 5:
                        this._stiffnessTensor[0, 3] = value;
                        this._stiffnessTensor[1, 3] = -1 * value;
                        this._stiffnessTensor[4, 5] = value;
                        this._stiffnessTensor[3, 0] = value;
                        this._stiffnessTensor[3, 1] = -1 * value;
                        this._stiffnessTensor[5, 4] = value;
                        break;
                    case 6:
                        goto case 5;
                    case 9:
                        this._stiffnessTensor[4, 5] = value;
                        this._stiffnessTensor[5, 4] = value;
                        break;
                    default:
                        this._stiffnessTensor[4, 5] = 0;
                        this._stiffnessTensor[5, 4] = 0;
                        break;
                }
            }
        }
        public double C56Error
        {
            get
            {
                return this._stiffnessTensorError[4, 5];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 5:
                        this._stiffnessTensorError[0, 3] = value;
                        this._stiffnessTensorError[1, 3] = -1 * value;
                        this._stiffnessTensorError[4, 5] = value;
                        this._stiffnessTensorError[3, 0] = value;
                        this._stiffnessTensorError[3, 1] = -1 * value;
                        this._stiffnessTensorError[5, 4] = value;
                        break;
                    case 6:
                        goto case 5;
                    case 9:
                        this._stiffnessTensorError[4, 5] = value;
                        this._stiffnessTensorError[5, 4] = value;
                        break;
                    default:
                        this._stiffnessTensorError[4, 5] = 0;
                        this._stiffnessTensorError[5, 4] = 0;
                        break;
                }
            }
        }

        public double C61
        {
            get
            {
                return this._stiffnessTensor[5, 0];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 4:
                        this._stiffnessTensor[0, 5] = value;
                        this._stiffnessTensor[1, 5] = -1 * value;
                        this._stiffnessTensor[5, 0] = value;
                        this._stiffnessTensor[5, 1] = -1 * value;
                        break;
                    case 8:
                        this._stiffnessTensor[0, 5] = value;
                        this._stiffnessTensor[5, 0] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._stiffnessTensor[0, 5] = 0;
                        this._stiffnessTensor[5, 0] = 0;
                        break;
                }
            }
        }
        public double C61Error
        {
            get
            {
                return this._stiffnessTensorError[5, 0];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 4:
                        this._stiffnessTensorError[0, 5] = value;
                        this._stiffnessTensorError[1, 5] = -1 * value;
                        this._stiffnessTensorError[5, 0] = value;
                        this._stiffnessTensorError[5, 1] = -1 * value;
                        break;
                    case 8:
                        this._stiffnessTensorError[0, 5] = value;
                        this._stiffnessTensorError[5, 0] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._stiffnessTensorError[0, 5] = 0;
                        this._stiffnessTensorError[5, 0] = 0;
                        break;
                }
            }
        }

        public double C62
        {
            get
            {
                return this._stiffnessTensor[5, 1];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 4:
                        this._stiffnessTensor[0, 5] = -1 * value;
                        this._stiffnessTensor[1, 5] = value;
                        this._stiffnessTensor[5, 0] = -1 * value;
                        this._stiffnessTensor[5, 1] = value;
                        break;
                    case 8:
                        this._stiffnessTensor[1, 5] = value;
                        this._stiffnessTensor[5, 1] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._stiffnessTensor[1, 5] = 0;
                        this._stiffnessTensor[5, 1] = 0;
                        break;
                }
            }
        }
        public double C62Error
        {
            get
            {
                return this._stiffnessTensorError[5, 1];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 4:
                        this._stiffnessTensorError[0, 5] = -1 * value;
                        this._stiffnessTensorError[1, 5] = value;
                        this._stiffnessTensorError[5, 0] = -1 * value;
                        this._stiffnessTensorError[5, 1] = value;
                        break;
                    case 8:
                        this._stiffnessTensorError[1, 5] = value;
                        this._stiffnessTensorError[5, 1] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._stiffnessTensorError[1, 5] = 0;
                        this._stiffnessTensorError[5, 1] = 0;
                        break;
                }
            }
        }

        public double C63
        {
            get
            {
                return this._stiffnessTensor[5, 2];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 8:
                        this._stiffnessTensor[2, 5] = value;
                        this._stiffnessTensor[5, 2] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._stiffnessTensor[2, 5] = 0;
                        this._stiffnessTensor[5, 2] = 0;
                        break;
                }
            }
        }
        public double C63Error
        {
            get
            {
                return this._stiffnessTensorError[5, 2];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 8:
                        this._stiffnessTensorError[2, 5] = value;
                        this._stiffnessTensorError[5, 2] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._stiffnessTensorError[2, 5] = 0;
                        this._stiffnessTensorError[5, 2] = 0;
                        break;
                }
            }
        }

        public double C64
        {
            get
            {
                return this._stiffnessTensor[5, 3];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 6:
                        this._stiffnessTensor[0, 4] = -1 * value;
                        this._stiffnessTensor[1, 4] = value;
                        this._stiffnessTensor[3, 5] = value;
                        this._stiffnessTensor[4, 0] = -1 * value;
                        this._stiffnessTensor[4, 1] = value;
                        this._stiffnessTensor[5, 3] = value;
                        break;
                    case 9:
                        this._stiffnessTensor[3, 5] = value;
                        this._stiffnessTensor[5, 3] = value;
                        break;
                    default:
                        this._stiffnessTensor[3, 5] = 0;
                        this._stiffnessTensor[5, 3] = 0;
                        break;
                }
            }
        }
        public double C64Error
        {
            get
            {
                return this._stiffnessTensorError[5, 3];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 6:
                        this._stiffnessTensorError[0, 4] = -1 * value;
                        this._stiffnessTensorError[1, 4] = value;
                        this._stiffnessTensorError[3, 5] = value;
                        this._stiffnessTensorError[4, 0] = -1 * value;
                        this._stiffnessTensorError[4, 1] = value;
                        this._stiffnessTensorError[5, 3] = value;
                        break;
                    case 9:
                        this._stiffnessTensorError[3, 5] = value;
                        this._stiffnessTensorError[5, 3] = value;
                        break;
                    default:
                        this._stiffnessTensorError[3, 5] = 0;
                        this._stiffnessTensorError[5, 3] = 0;
                        break;
                }
            }
        }

        public double C65
        {
            get
            {
                return this._stiffnessTensor[5, 4];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 5:
                        this._stiffnessTensor[0, 3] = value;
                        this._stiffnessTensor[1, 3] = -1 * value;
                        this._stiffnessTensor[4, 5] = value;
                        this._stiffnessTensor[3, 0] = value;
                        this._stiffnessTensor[3, 1] = -1 * value;
                        this._stiffnessTensor[5, 4] = value;
                        break;
                    case 6:
                        goto case 5;
                    case 9:
                        this._stiffnessTensor[4, 5] = value;
                        this._stiffnessTensor[5, 4] = value;
                        break;
                    default:
                        this._stiffnessTensor[4, 5] = 0;
                        this._stiffnessTensor[5, 4] = 0;
                        break;
                }
            }
        }
        public double C65Error
        {
            get
            {
                return this._stiffnessTensorError[5, 4];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 5:
                        this._stiffnessTensorError[0, 3] = value;
                        this._stiffnessTensorError[1, 3] = -1 * value;
                        this._stiffnessTensorError[4, 5] = value;
                        this._stiffnessTensorError[3, 0] = value;
                        this._stiffnessTensorError[3, 1] = -1 * value;
                        this._stiffnessTensorError[5, 4] = value;
                        break;
                    case 6:
                        goto case 5;
                    case 9:
                        this._stiffnessTensorError[4, 5] = value;
                        this._stiffnessTensorError[5, 4] = value;
                        break;
                    default:
                        this._stiffnessTensorError[4, 5] = 0;
                        this._stiffnessTensorError[5, 4] = 0;
                        break;
                }
            }
        }

        public double C66
        {
            get
            {
                return this._stiffnessTensor[5, 5];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        if (!this.IsIsotropic)
                        {
                            this._stiffnessTensor[3, 3] = value;
                            this._stiffnessTensor[4, 4] = value;
                            this._stiffnessTensor[5, 5] = value;
                        }
                        break;
                    case 2:
                        break;
                    case 5:
                        break;
                    case 6:
                        break;
                    default:
                        this._stiffnessTensor[5, 5] = value;
                        break;
                }
            }
        }
        public double C66Error
        {
            get
            {
                return this._stiffnessTensorError[5, 5];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        if (!this.IsIsotropic)
                        {
                            this._stiffnessTensorError[3, 3] = value;
                            this._stiffnessTensorError[4, 4] = value;
                            this._stiffnessTensorError[5, 5] = value;
                        }
                        break;
                    case 2:
                        break;
                    case 5:
                        break;
                    case 6:
                        break;
                    default:
                        this._stiffnessTensorError[5, 5] = value;
                        break;
                }
            }
        }

        #endregion

        #region Compliance matrix components

        public double S11
        {
            get
            {
                return this._complianceTensor[0, 0];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._complianceTensor[0, 0] = value;
                        this._complianceTensor[1, 1] = value;
                        this._complianceTensor[2, 2] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 2 * (this._complianceTensor[0, 0] - this._complianceTensor[0, 1]);
                            this._complianceTensor[3, 3] = IsoValue;
                            this._complianceTensor[4, 4] = IsoValue;
                            this._complianceTensor[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        double IsoValue1 = 2 * (value - this._complianceTensor[0, 1]);
                        this._complianceTensor[5, 5] = IsoValue1;
                        goto case 3;
                    case 3:
                        this._complianceTensor[0, 0] = value;
                        this._complianceTensor[1, 1] = value;
                        break;
                    case 4:
                        goto case 3;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._complianceTensor[0, 0] = value;
                        break;
                }
            }
        }
        public double S11Error
        {
            get
            {
                return this._complianceTensorError[0, 0];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._complianceTensorError[0, 0] = value;
                        this._complianceTensorError[1, 1] = value;
                        this._complianceTensorError[2, 2] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 2 * (this._complianceTensorError[0, 0] - this._complianceTensorError[0, 1]);
                            this._complianceTensorError[3, 3] = IsoValue;
                            this._complianceTensorError[4, 4] = IsoValue;
                            this._complianceTensorError[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        double IsoValue1 = 2 * (value - this._complianceTensorError[0, 1]);
                        this._complianceTensorError[5, 5] = IsoValue1;
                        goto case 3;
                    case 3:
                        this._complianceTensorError[0, 0] = value;
                        this._complianceTensorError[1, 1] = value;
                        break;
                    case 4:
                        goto case 3;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._complianceTensorError[0, 0] = value;
                        break;
                }
            }
        }

        public double S12
        {
            get
            {
                return this._complianceTensor[0, 1];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._complianceTensor[0, 1] = value;
                        this._complianceTensor[0, 2] = value;
                        this._complianceTensor[1, 2] = value;
                        this._complianceTensor[1, 0] = value;
                        this._complianceTensor[2, 0] = value;
                        this._complianceTensor[2, 1] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 2 * (this._complianceTensor[0, 0] - this._complianceTensor[0, 1]);
                            this._complianceTensor[3, 3] = IsoValue;
                            this._complianceTensor[4, 4] = IsoValue;
                            this._complianceTensor[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        double IsoValue1 = 2 * (this._complianceTensor[0, 0] - value);
                        this._complianceTensor[5, 5] = IsoValue1;
                        goto default;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._complianceTensor[0, 1] = value;
                        this._complianceTensor[1, 0] = value;
                        break;
                }
            }
        }
        public double S12Error
        {
            get
            {
                return this._complianceTensorError[0, 1];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._complianceTensorError[0, 1] = value;
                        this._complianceTensorError[0, 2] = value;
                        this._complianceTensorError[1, 2] = value;
                        this._complianceTensorError[1, 0] = value;
                        this._complianceTensorError[2, 0] = value;
                        this._complianceTensorError[2, 1] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 2 * (this._complianceTensorError[0, 0] - this._complianceTensorError[0, 1]);
                            this._complianceTensorError[3, 3] = IsoValue;
                            this._complianceTensorError[4, 4] = IsoValue;
                            this._complianceTensorError[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        double IsoValue1 = 2 * (this._complianceTensorError[0, 0] - value);
                        this._complianceTensorError[5, 5] = IsoValue1;
                        goto default;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._complianceTensorError[0, 1] = value;
                        this._complianceTensorError[1, 0] = value;
                        break;
                }
            }
        }

        public double S13
        {
            get
            {
                return this._complianceTensor[0, 2];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._complianceTensor[0, 1] = value;
                        this._complianceTensor[0, 2] = value;
                        this._complianceTensor[1, 2] = value;
                        this._complianceTensor[1, 0] = value;
                        this._complianceTensor[2, 0] = value;
                        this._complianceTensor[2, 1] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 2 * (this._complianceTensor[0, 0] - this._complianceTensor[0, 1]);
                            this._complianceTensor[3, 3] = IsoValue;
                            this._complianceTensor[4, 4] = IsoValue;
                            this._complianceTensor[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        this._complianceTensor[0, 2] = value;
                        this._complianceTensor[1, 2] = value;
                        this._complianceTensor[2, 0] = value;
                        this._complianceTensor[2, 1] = value;
                        break;
                    case 3:
                        goto case 2;
                    case 4:
                        goto case 2;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._complianceTensor[0, 2] = value;
                        this._complianceTensor[2, 0] = value;
                        break;
                }
            }
        }
        public double S13Error
        {
            get
            {
                return this._complianceTensorError[0, 2];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._complianceTensorError[0, 1] = value;
                        this._complianceTensorError[0, 2] = value;
                        this._complianceTensorError[1, 2] = value;
                        this._complianceTensorError[1, 0] = value;
                        this._complianceTensorError[2, 0] = value;
                        this._complianceTensorError[2, 1] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 2 * (this._complianceTensorError[0, 0] - this._complianceTensorError[0, 1]);
                            this._complianceTensorError[3, 3] = IsoValue;
                            this._complianceTensorError[4, 4] = IsoValue;
                            this._complianceTensorError[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        this._complianceTensorError[0, 2] = value;
                        this._complianceTensorError[1, 2] = value;
                        this._complianceTensorError[2, 0] = value;
                        this._complianceTensorError[2, 1] = value;
                        break;
                    case 3:
                        goto case 2;
                    case 4:
                        goto case 2;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._complianceTensorError[0, 2] = value;
                        this._complianceTensorError[2, 0] = value;
                        break;
                }
            }
        }

        public double S14
        {
            get
            {
                return this._complianceTensor[0, 3];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 5:
                        this._complianceTensor[0, 3] = value;
                        this._complianceTensor[1, 3] = -1 * value;
                        this._complianceTensor[4, 5] = 0.5 * value;
                        this._complianceTensor[3, 0] = value;
                        this._complianceTensor[3, 1] = -1 * value;
                        this._complianceTensor[5, 4] = 0.5 * value;
                        break;
                    case 6:
                        goto case 5;
                    case 9:
                        this._complianceTensor[0, 3] = value;
                        this._complianceTensor[3, 0] = value;
                        break;
                    default:
                        this._complianceTensor[0, 3] = 0;
                        this._complianceTensor[3, 0] = 0;
                        break;
                }
            }
        }
        public double S14Error
        {
            get
            {
                return this._complianceTensorError[0, 3];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 5:
                        this._complianceTensorError[0, 3] = value;
                        this._complianceTensorError[1, 3] = -1 * value;
                        this._complianceTensorError[4, 5] = 0.5 * value;
                        this._complianceTensorError[3, 0] = value;
                        this._complianceTensorError[3, 1] = -1 * value;
                        this._complianceTensorError[5, 4] = 0.5 * value;
                        break;
                    case 6:
                        goto case 5;
                    case 9:
                        this._complianceTensorError[0, 3] = value;
                        this._complianceTensorError[3, 0] = value;
                        break;
                    default:
                        this._complianceTensorError[0, 3] = 0;
                        this._complianceTensorError[3, 0] = 0;
                        break;
                }
            }
        }

        public double S15
        {
            get
            {
                return this._complianceTensor[0, 4];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 6:
                        this._complianceTensor[0, 4] = value;
                        this._complianceTensor[1, 4] = -1 * value;
                        this._complianceTensor[3, 5] = -0.5 * value;
                        this._complianceTensor[4, 0] = value;
                        this._complianceTensor[4, 1] = -1 * value;
                        this._complianceTensor[5, 3] = -0.5 * value;
                        break;
                    case 9:
                        this._complianceTensor[0, 4] = value;
                        this._complianceTensor[4, 0] = value;
                        break;
                    default:
                        this._complianceTensor[0, 4] = 0;
                        this._complianceTensor[4, 0] = 0;
                        break;
                }
            }
        }
        public double S15Error
        {
            get
            {
                return this._complianceTensorError[0, 4];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 6:
                        this._complianceTensorError[0, 4] = value;
                        this._complianceTensorError[1, 4] = -1 * value;
                        this._complianceTensorError[3, 5] = -0.5 * value;
                        this._complianceTensorError[4, 0] = value;
                        this._complianceTensorError[4, 1] = -1 * value;
                        this._complianceTensorError[5, 3] = -0.5 * value;
                        break;
                    case 9:
                        this._complianceTensorError[0, 4] = value;
                        this._complianceTensorError[4, 0] = value;
                        break;
                    default:
                        this._complianceTensorError[0, 4] = 0;
                        this._complianceTensorError[4, 0] = 0;
                        break;
                }
            }
        }

        public double S16
        {
            get
            {
                return this._complianceTensor[0, 5];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 4:
                        this._complianceTensor[0, 5] = value;
                        this._complianceTensor[1, 5] = -1 * value;
                        this._complianceTensor[5, 0] = value;
                        this._complianceTensor[5, 1] = -1 * value;
                        break;
                    case 8:
                        this._complianceTensor[0, 5] = value;
                        this._complianceTensor[5, 0] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._complianceTensor[0, 5] = 0;
                        this._complianceTensor[5, 0] = 0;
                        break;
                }
            }
        }
        public double S16Error
        {
            get
            {
                return this._complianceTensorError[0, 5];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 4:
                        this._complianceTensorError[0, 5] = value;
                        this._complianceTensorError[1, 5] = -1 * value;
                        this._complianceTensorError[5, 0] = value;
                        this._complianceTensorError[5, 1] = -1 * value;
                        break;
                    case 8:
                        this._complianceTensorError[0, 5] = value;
                        this._complianceTensorError[5, 0] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._complianceTensorError[0, 5] = 0;
                        this._complianceTensorError[5, 0] = 0;
                        break;
                }
            }
        }

        public double S21
        {
            get
            {
                return this._complianceTensor[1, 0];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._complianceTensor[0, 1] = value;
                        this._complianceTensor[0, 2] = value;
                        this._complianceTensor[1, 2] = value;
                        this._complianceTensor[1, 0] = value;
                        this._complianceTensor[2, 0] = value;
                        this._complianceTensor[2, 1] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 2 * (this._complianceTensor[0, 0] - this._complianceTensor[0, 1]);
                            this._complianceTensor[3, 3] = IsoValue;
                            this._complianceTensor[4, 4] = IsoValue;
                            this._complianceTensor[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        double IsoValue1 = 2 * (this._complianceTensor[0, 0] - value);
                        this._complianceTensor[5, 5] = IsoValue1;
                        goto default;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._complianceTensor[0, 1] = value;
                        this._complianceTensor[1, 0] = value;
                        break;
                }
            }
        }
        public double S21Error
        {
            get
            {
                return this._complianceTensorError[1, 0];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._complianceTensorError[0, 1] = value;
                        this._complianceTensorError[0, 2] = value;
                        this._complianceTensorError[1, 2] = value;
                        this._complianceTensorError[1, 0] = value;
                        this._complianceTensorError[2, 0] = value;
                        this._complianceTensorError[2, 1] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 2 * (this._complianceTensorError[0, 0] - this._complianceTensorError[0, 1]);
                            this._complianceTensorError[3, 3] = IsoValue;
                            this._complianceTensorError[4, 4] = IsoValue;
                            this._complianceTensorError[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        double IsoValue1 = 2 * (this._complianceTensorError[0, 0] - value);
                        this._complianceTensorError[5, 5] = IsoValue1;
                        goto default;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._complianceTensorError[0, 1] = value;
                        this._complianceTensorError[1, 0] = value;
                        break;
                }
            }
        }

        public double S22
        {
            get
            {
                return this._complianceTensor[1, 1];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._complianceTensor[0, 0] = value;
                        this._complianceTensor[1, 1] = value;
                        this._complianceTensor[2, 2] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 2 * (this._complianceTensor[0, 0] - this._complianceTensor[0, 1]);
                            this._complianceTensor[3, 3] = IsoValue;
                            this._complianceTensor[4, 4] = IsoValue;
                            this._complianceTensor[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        double IsoValue1 = 2 * (value - this._complianceTensor[0, 1]);
                        this._complianceTensor[5, 5] = IsoValue1;
                        goto case 3;
                    case 3:
                        this._complianceTensor[0, 0] = value;
                        this._complianceTensor[1, 1] = value;
                        break;
                    case 4:
                        goto case 3;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._complianceTensor[1, 1] = value;
                        break;
                }
            }
        }
        public double S22Error
        {
            get
            {
                return this._complianceTensorError[1, 1];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._complianceTensorError[0, 0] = value;
                        this._complianceTensorError[1, 1] = value;
                        this._complianceTensorError[2, 2] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 2 * (this._complianceTensorError[0, 0] - this._complianceTensorError[0, 1]);
                            this._complianceTensorError[3, 3] = IsoValue;
                            this._complianceTensorError[4, 4] = IsoValue;
                            this._complianceTensorError[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        double IsoValue1 = 2 * (value - this._complianceTensorError[0, 1]);
                        this._complianceTensorError[5, 5] = IsoValue1;
                        goto case 3;
                    case 3:
                        this._complianceTensorError[0, 0] = value;
                        this._complianceTensorError[1, 1] = value;
                        break;
                    case 4:
                        goto case 3;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._complianceTensorError[1, 1] = value;
                        break;
                }
            }
        }

        public double S23
        {
            get
            {
                return this._complianceTensor[1, 2];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._complianceTensor[0, 1] = value;
                        this._complianceTensor[0, 2] = value;
                        this._complianceTensor[1, 2] = value;
                        this._complianceTensor[1, 0] = value;
                        this._complianceTensor[2, 0] = value;
                        this._complianceTensor[2, 1] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 2 * (this._complianceTensor[0, 0] - this._complianceTensor[0, 1]);
                            this._complianceTensor[3, 3] = IsoValue;
                            this._complianceTensor[4, 4] = IsoValue;
                            this._complianceTensor[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        this._complianceTensor[0, 2] = value;
                        this._complianceTensor[1, 2] = value;
                        this._complianceTensor[2, 0] = value;
                        this._complianceTensor[2, 1] = value;
                        break;
                    case 3:
                        goto case 2;
                    case 4:
                        goto case 2;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._complianceTensor[1, 2] = value;
                        this._complianceTensor[2, 1] = value;
                        break;
                }
            }
        }
        public double S23Error
        {
            get
            {
                return this._complianceTensorError[1, 2];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._complianceTensorError[0, 1] = value;
                        this._complianceTensorError[0, 2] = value;
                        this._complianceTensorError[1, 2] = value;
                        this._complianceTensorError[1, 0] = value;
                        this._complianceTensorError[2, 0] = value;
                        this._complianceTensorError[2, 1] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 2 * (this._complianceTensorError[0, 0] - this._complianceTensorError[0, 1]);
                            this._complianceTensorError[3, 3] = IsoValue;
                            this._complianceTensorError[4, 4] = IsoValue;
                            this._complianceTensorError[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        this._complianceTensorError[0, 2] = value;
                        this._complianceTensorError[1, 2] = value;
                        this._complianceTensorError[2, 0] = value;
                        this._complianceTensorError[2, 1] = value;
                        break;
                    case 3:
                        goto case 2;
                    case 4:
                        goto case 2;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._complianceTensorError[1, 2] = value;
                        this._complianceTensorError[2, 1] = value;
                        break;
                }
            }
        }

        public double S24
        {
            get
            {
                return this._complianceTensor[1, 3];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 5:
                        this._complianceTensor[0, 3] = -1 * value;
                        this._complianceTensor[1, 3] = value;
                        this._complianceTensor[4, 5] = -0.5 * value;
                        this._complianceTensor[3, 0] = -1 * value;
                        this._complianceTensor[3, 1] = value;
                        this._complianceTensor[5, 4] = -0.5 * value;
                        break;
                    case 6:
                        goto case 5;
                    case 9:
                        this._complianceTensor[1, 3] = value;
                        this._complianceTensor[3, 1] = value;
                        break;
                    default:
                        this._complianceTensor[1, 3] = 0;
                        this._complianceTensor[3, 1] = 0;
                        break;
                }
            }
        }
        public double S24Error
        {
            get
            {
                return this._complianceTensorError[1, 3];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 5:
                        this._complianceTensorError[0, 3] = -1 * value;
                        this._complianceTensorError[1, 3] = value;
                        this._complianceTensorError[4, 5] = -0.5 * value;
                        this._complianceTensorError[3, 0] = -1 * value;
                        this._complianceTensorError[3, 1] = value;
                        this._complianceTensorError[5, 4] = -0.5 * value;
                        break;
                    case 6:
                        goto case 5;
                    case 9:
                        this._complianceTensorError[1, 3] = value;
                        this._complianceTensorError[3, 1] = value;
                        break;
                    default:
                        this._complianceTensorError[1, 3] = 0;
                        this._complianceTensorError[3, 1] = 0;
                        break;
                }
            }
        }

        public double S25
        {
            get
            {
                return this._complianceTensor[1, 4];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 6:
                        this._complianceTensor[0, 4] = -1 * value;
                        this._complianceTensor[1, 4] = value;
                        this._complianceTensor[3, 5] = 0.5 * value;
                        this._complianceTensor[4, 0] = -1 * value;
                        this._complianceTensor[4, 1] = value;
                        this._complianceTensor[5, 3] = 0.5 * value;
                        break;
                    case 9:
                        this._complianceTensor[1, 4] = value;
                        this._complianceTensor[4, 1] = value;
                        break;
                    default:
                        this._complianceTensor[1, 4] = 0;
                        this._complianceTensor[4, 1] = 0;
                        break;
                }
            }
        }
        public double S25Error
        {
            get
            {
                return this._complianceTensorError[1, 4];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 6:
                        this._complianceTensorError[0, 4] = -1 * value;
                        this._complianceTensorError[1, 4] = value;
                        this._complianceTensorError[3, 5] = 0.5 * value;
                        this._complianceTensorError[4, 0] = -1 * value;
                        this._complianceTensorError[4, 1] = value;
                        this._complianceTensorError[5, 3] = 0.5 * value;
                        break;
                    case 9:
                        this._complianceTensorError[1, 4] = value;
                        this._complianceTensorError[4, 1] = value;
                        break;
                    default:
                        this._complianceTensorError[1, 4] = 0;
                        this._complianceTensorError[4, 1] = 0;
                        break;
                }
            }
        }

        public double S26
        {
            get
            {
                return this._complianceTensor[1, 5];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 4:
                        this._complianceTensor[0, 5] = -1 * value;
                        this._complianceTensor[1, 5] = value;
                        this._complianceTensor[5, 0] = -1 * value;
                        this._complianceTensor[5, 1] = value;
                        break;
                    case 8:
                        this._complianceTensor[1, 5] = value;
                        this._complianceTensor[5, 1] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._complianceTensor[1, 5] = 0;
                        this._complianceTensor[5, 1] = 0;
                        break;
                }
            }
        }
        public double S26Error
        {
            get
            {
                return this._complianceTensorError[1, 5];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 4:
                        this._complianceTensorError[0, 5] = -1 * value;
                        this._complianceTensorError[1, 5] = value;
                        this._complianceTensorError[5, 0] = -1 * value;
                        this._complianceTensorError[5, 1] = value;
                        break;
                    case 8:
                        this._complianceTensorError[1, 5] = value;
                        this._complianceTensorError[5, 1] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._complianceTensorError[1, 5] = 0;
                        this._complianceTensorError[5, 1] = 0;
                        break;
                }
            }
        }

        public double S31
        {
            get
            {
                return this._complianceTensor[2, 0];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._complianceTensor[0, 1] = value;
                        this._complianceTensor[0, 2] = value;
                        this._complianceTensor[1, 2] = value;
                        this._complianceTensor[1, 0] = value;
                        this._complianceTensor[2, 0] = value;
                        this._complianceTensor[2, 1] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 2 * (this._complianceTensor[0, 0] - this._complianceTensor[0, 1]);
                            this._complianceTensor[3, 3] = IsoValue;
                            this._complianceTensor[4, 4] = IsoValue;
                            this._complianceTensor[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        this._complianceTensor[0, 2] = value;
                        this._complianceTensor[1, 2] = value;
                        this._complianceTensor[2, 0] = value;
                        this._complianceTensor[2, 1] = value;
                        break;
                    case 3:
                        goto case 2;
                    case 4:
                        goto case 2;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._complianceTensor[0, 2] = value;
                        this._complianceTensor[2, 0] = value;
                        break;
                }
            }
        }
        public double S31Error
        {
            get
            {
                return this._complianceTensorError[2, 0];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._complianceTensorError[0, 1] = value;
                        this._complianceTensorError[0, 2] = value;
                        this._complianceTensorError[1, 2] = value;
                        this._complianceTensorError[1, 0] = value;
                        this._complianceTensorError[2, 0] = value;
                        this._complianceTensorError[2, 1] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 2 * (this._complianceTensorError[0, 0] - this._complianceTensorError[0, 1]);
                            this._complianceTensorError[3, 3] = IsoValue;
                            this._complianceTensorError[4, 4] = IsoValue;
                            this._complianceTensorError[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        this._complianceTensorError[0, 2] = value;
                        this._complianceTensorError[1, 2] = value;
                        this._complianceTensorError[2, 0] = value;
                        this._complianceTensorError[2, 1] = value;
                        break;
                    case 3:
                        goto case 2;
                    case 4:
                        goto case 2;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._complianceTensorError[0, 2] = value;
                        this._complianceTensorError[2, 0] = value;
                        break;
                }
            }
        }

        public double S32
        {
            get
            {
                return this._complianceTensor[2, 1];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._complianceTensor[0, 1] = value;
                        this._complianceTensor[0, 2] = value;
                        this._complianceTensor[1, 2] = value;
                        this._complianceTensor[1, 0] = value;
                        this._complianceTensor[2, 0] = value;
                        this._complianceTensor[2, 1] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 2 * (this._complianceTensor[0, 0] - this._complianceTensor[0, 1]);
                            this._complianceTensor[3, 3] = IsoValue;
                            this._complianceTensor[4, 4] = IsoValue;
                            this._complianceTensor[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        this._complianceTensor[0, 2] = value;
                        this._complianceTensor[1, 2] = value;
                        this._complianceTensor[2, 0] = value;
                        this._complianceTensor[2, 1] = value;
                        break;
                    case 3:
                        goto case 2;
                    case 4:
                        goto case 2;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._complianceTensor[1, 2] = value;
                        this._complianceTensor[2, 1] = value;
                        break;
                }
            }
        }
        public double S32Error
        {
            get
            {
                return this._complianceTensorError[2, 1];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._complianceTensorError[0, 1] = value;
                        this._complianceTensorError[0, 2] = value;
                        this._complianceTensorError[1, 2] = value;
                        this._complianceTensorError[1, 0] = value;
                        this._complianceTensorError[2, 0] = value;
                        this._complianceTensorError[2, 1] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 2 * (this._complianceTensorError[0, 0] - this._complianceTensorError[0, 1]);
                            this._complianceTensorError[3, 3] = IsoValue;
                            this._complianceTensorError[4, 4] = IsoValue;
                            this._complianceTensorError[5, 5] = IsoValue;
                        }
                        break;
                    case 2:
                        this._complianceTensorError[0, 2] = value;
                        this._complianceTensorError[1, 2] = value;
                        this._complianceTensorError[2, 0] = value;
                        this._complianceTensorError[2, 1] = value;
                        break;
                    case 3:
                        goto case 2;
                    case 4:
                        goto case 2;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._complianceTensorError[1, 2] = value;
                        this._complianceTensorError[2, 1] = value;
                        break;
                }
            }
        }

        public double S33
        {
            get
            {
                return this._complianceTensor[2, 2];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._complianceTensor[0, 0] = value;
                        this._complianceTensor[1, 1] = value;
                        this._complianceTensor[2, 2] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 2 * (this._complianceTensor[0, 0] - this._complianceTensor[0, 1]);
                            this._complianceTensor[3, 3] = IsoValue;
                            this._complianceTensor[4, 4] = IsoValue;
                            this._complianceTensor[5, 5] = IsoValue;
                        }
                        break;
                    default:
                        this._complianceTensor[2, 2] = value;
                        break;
                }
            }
        }
        public double S33Error
        {
            get
            {
                return this._complianceTensorError[2, 2];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        this._complianceTensorError[0, 0] = value;
                        this._complianceTensorError[1, 1] = value;
                        this._complianceTensorError[2, 2] = value;
                        if (this.IsIsotropic)
                        {
                            double IsoValue = 2 * (this._complianceTensorError[0, 0] - this._complianceTensorError[0, 1]);
                            this._complianceTensorError[3, 3] = IsoValue;
                            this._complianceTensorError[4, 4] = IsoValue;
                            this._complianceTensorError[5, 5] = IsoValue;
                        }
                        break;
                    default:
                        this._complianceTensorError[2, 2] = value;
                        break;
                }
            }
        }

        public double S34
        {
            get
            {
                return this._complianceTensor[2, 3];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 9:
                        this._complianceTensor[2, 3] = value;
                        this._complianceTensor[3, 2] = value;
                        break;
                    default:
                        this._complianceTensor[2, 3] = 0;
                        this._complianceTensor[3, 2] = 0;
                        break;
                }
            }
        }
        public double S34Error
        {
            get
            {
                return this._complianceTensorError[2, 3];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 9:
                        this._complianceTensorError[2, 3] = value;
                        this._complianceTensorError[3, 2] = value;
                        break;
                    default:
                        this._complianceTensorError[2, 3] = 0;
                        this._complianceTensorError[3, 2] = 0;
                        break;
                }
            }
        }

        public double S35
        {
            get
            {
                return this._complianceTensor[2, 4];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 9:
                        this._complianceTensor[2, 4] = value;
                        this._complianceTensor[4, 2] = value;
                        break;
                    default:
                        this._complianceTensor[2, 4] = 0;
                        this._complianceTensor[4, 2] = 0;
                        break;
                }
            }
        }
        public double S35Error
        {
            get
            {
                return this._complianceTensorError[2, 4];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 9:
                        this._complianceTensorError[2, 4] = value;
                        this._complianceTensorError[4, 2] = value;
                        break;
                    default:
                        this._complianceTensorError[2, 4] = 0;
                        this._complianceTensorError[4, 2] = 0;
                        break;
                }
            }
        }

        public double S36
        {
            get
            {
                return this._complianceTensor[2, 5];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 8:
                        this._complianceTensor[2, 5] = value;
                        this._complianceTensor[5, 2] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._complianceTensor[2, 5] = 0;
                        this._complianceTensor[5, 2] = 0;
                        break;
                }
            }
        }
        public double S36Error
        {
            get
            {
                return this._complianceTensorError[2, 5];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 8:
                        this._complianceTensorError[2, 5] = value;
                        this._complianceTensorError[5, 2] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._complianceTensorError[2, 5] = 0;
                        this._complianceTensorError[5, 2] = 0;
                        break;
                }
            }
        }

        public double S41
        {
            get
            {
                return this._complianceTensor[3, 0];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 5:
                        this._complianceTensor[0, 3] = value;
                        this._complianceTensor[1, 3] = -1 * value;
                        this._complianceTensor[4, 5] = 0.5 * value;
                        this._complianceTensor[3, 0] = value;
                        this._complianceTensor[3, 1] = -1 * value;
                        this._complianceTensor[5, 4] = 0.5 * value;
                        break;
                    case 6:
                        goto case 5;
                    case 9:
                        this._complianceTensor[0, 3] = value;
                        this._complianceTensor[3, 0] = value;
                        break;
                    default:
                        this._complianceTensor[0, 3] = 0;
                        this._complianceTensor[3, 0] = 0;
                        break;
                }
            }
        }
        public double S41Error
        {
            get
            {
                return this._complianceTensorError[3, 0];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 5:
                        this._complianceTensorError[0, 3] = value;
                        this._complianceTensorError[1, 3] = -1 * value;
                        this._complianceTensorError[4, 5] = 0.5 * value;
                        this._complianceTensorError[3, 0] = value;
                        this._complianceTensorError[3, 1] = -1 * value;
                        this._complianceTensorError[5, 4] = 0.5 * value;
                        break;
                    case 6:
                        goto case 5;
                    case 9:
                        this._complianceTensorError[0, 3] = value;
                        this._complianceTensorError[3, 0] = value;
                        break;
                    default:
                        this._complianceTensorError[0, 3] = 0;
                        this._complianceTensorError[3, 0] = 0;
                        break;
                }
            }
        }

        public double S42
        {
            get
            {
                return this._complianceTensor[3, 1];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 5:
                        this._complianceTensor[0, 3] = -1 * value;
                        this._complianceTensor[1, 3] = value;
                        this._complianceTensor[4, 5] = -0.5 * value;
                        this._complianceTensor[3, 0] = -1 * value;
                        this._complianceTensor[3, 1] = value;
                        this._complianceTensor[5, 4] = -0.5 * value;
                        break;
                    case 6:
                        goto case 5;
                    case 9:
                        this._complianceTensor[1, 3] = value;
                        this._complianceTensor[3, 1] = value;
                        break;
                    default:
                        this._complianceTensor[1, 3] = 0;
                        this._complianceTensor[3, 1] = 0;
                        break;
                }
            }
        }
        public double S42Error
        {
            get
            {
                return this._complianceTensorError[3, 1];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 5:
                        this._complianceTensorError[0, 3] = -1 * value;
                        this._complianceTensorError[1, 3] = value;
                        this._complianceTensorError[4, 5] = -0.5 * value;
                        this._complianceTensorError[3, 0] = -1 * value;
                        this._complianceTensorError[3, 1] = value;
                        this._complianceTensorError[5, 4] = -0.5 * value;
                        break;
                    case 6:
                        goto case 5;
                    case 9:
                        this._complianceTensorError[1, 3] = value;
                        this._complianceTensorError[3, 1] = value;
                        break;
                    default:
                        this._complianceTensorError[1, 3] = 0;
                        this._complianceTensorError[3, 1] = 0;
                        break;
                }
            }
        }

        public double S43
        {
            get
            {
                return this._complianceTensor[3, 2];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 9:
                        this._complianceTensor[2, 3] = value;
                        this._complianceTensor[3, 2] = value;
                        break;
                    default:
                        this._complianceTensor[2, 3] = 0;
                        this._complianceTensor[3, 2] = 0;
                        break;
                }
            }
        }
        public double S43Error
        {
            get
            {
                return this._complianceTensorError[3, 2];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 9:
                        this._complianceTensorError[2, 3] = value;
                        this._complianceTensorError[3, 2] = value;
                        break;
                    default:
                        this._complianceTensorError[2, 3] = 0;
                        this._complianceTensorError[3, 2] = 0;
                        break;
                }
            }
        }

        public double S44
        {
            get
            {
                return this._complianceTensor[3, 3];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        if (!this.IsIsotropic)
                        {
                            this._complianceTensor[3, 3] = value;
                            this._complianceTensor[4, 4] = value;
                            this._complianceTensor[5, 5] = value;
                        }
                        break;
                    case 2:
                        this._complianceTensor[3, 3] = value;
                        this._complianceTensor[4, 4] = value;
                        break;
                    case 3:
                        goto case 2;
                    case 4:
                        goto case 2;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._complianceTensor[3, 3] = value;
                        break;
                }
            }
        }
        public double S44Error
        {
            get
            {
                return this._complianceTensorError[3, 3];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        if (!this.IsIsotropic)
                        {
                            this._complianceTensorError[3, 3] = value;
                            this._complianceTensorError[4, 4] = value;
                            this._complianceTensorError[5, 5] = value;
                        }
                        break;
                    case 2:
                        this._complianceTensorError[3, 3] = value;
                        this._complianceTensorError[4, 4] = value;
                        break;
                    case 3:
                        goto case 2;
                    case 4:
                        goto case 2;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._complianceTensorError[3, 3] = value;
                        break;
                }
            }
        }

        public double S45
        {
            get
            {
                return this._complianceTensor[3, 4];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 8:
                        this._complianceTensor[3, 4] = value;
                        this._complianceTensor[4, 3] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._complianceTensor[3, 4] = 0;
                        this._complianceTensor[4, 3] = 0;
                        break;
                }
            }
        }
        public double S45Error
        {
            get
            {
                return this._complianceTensorError[3, 4];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 8:
                        this._complianceTensorError[3, 4] = value;
                        this._complianceTensorError[4, 3] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._complianceTensorError[3, 4] = 0;
                        this._complianceTensorError[4, 3] = 0;
                        break;
                }
            }
        }

        public double S46
        {
            get
            {
                return this._complianceTensor[3, 5];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 6:
                        this._complianceTensor[0, 4] = -2 * value;
                        this._complianceTensor[1, 4] = 2 * value;
                        this._complianceTensor[3, 5] = value;
                        this._complianceTensor[4, 0] = -2 * value;
                        this._complianceTensor[4, 1] = 2 * value;
                        this._complianceTensor[5, 3] = value;
                        break;
                    case 9:
                        this._complianceTensor[3, 5] = value;
                        this._complianceTensor[5, 3] = value;
                        break;
                    default:
                        this._complianceTensor[3, 5] = 0;
                        this._complianceTensor[5, 3] = 0;
                        break;
                }
            }
        }
        public double S46Error
        {
            get
            {
                return this._complianceTensorError[3, 5];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 6:
                        this._complianceTensorError[0, 4] = -2 * value;
                        this._complianceTensorError[1, 4] = 2 * value;
                        this._complianceTensorError[3, 5] = value;
                        this._complianceTensorError[4, 0] = -2 * value;
                        this._complianceTensorError[4, 1] = 2 * value;
                        this._complianceTensorError[5, 3] = value;
                        break;
                    case 9:
                        this._complianceTensorError[3, 5] = value;
                        this._complianceTensorError[5, 3] = value;
                        break;
                    default:
                        this._complianceTensorError[3, 5] = 0;
                        this._complianceTensorError[5, 3] = 0;
                        break;
                }
            }
        }

        public double S51
        {
            get
            {
                return this._complianceTensor[4, 0];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 6:
                        this._complianceTensor[0, 4] = value;
                        this._complianceTensor[1, 4] = -1 * value;
                        this._complianceTensor[3, 5] = -0.5 * value;
                        this._complianceTensor[4, 0] = value;
                        this._complianceTensor[4, 1] = -1 * value;
                        this._complianceTensor[5, 3] = -0.5 * value;
                        break;
                    case 9:
                        this._complianceTensor[0, 4] = value;
                        this._complianceTensor[4, 0] = value;
                        break;
                    default:
                        this._complianceTensor[0, 4] = 0;
                        this._complianceTensor[4, 0] = 0;
                        break;
                }
            }
        }
        public double S51Error
        {
            get
            {
                return this._complianceTensorError[4, 0];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 6:
                        this._complianceTensorError[0, 4] = value;
                        this._complianceTensorError[1, 4] = -1 * value;
                        this._complianceTensorError[3, 5] = -0.5 * value;
                        this._complianceTensorError[4, 0] = value;
                        this._complianceTensorError[4, 1] = -1 * value;
                        this._complianceTensorError[5, 3] = -0.5 * value;
                        break;
                    case 9:
                        this._complianceTensorError[0, 4] = value;
                        this._complianceTensorError[4, 0] = value;
                        break;
                    default:
                        this._complianceTensorError[0, 4] = 0;
                        this._complianceTensorError[4, 0] = 0;
                        break;
                }
            }
        }

        public double S52
        {
            get
            {
                return this._complianceTensor[4, 1];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 6:
                        this._complianceTensor[0, 4] = -1 * value;
                        this._complianceTensor[1, 4] = value;
                        this._complianceTensor[3, 5] = 0.5 * value;
                        this._complianceTensor[4, 0] = -1 * value;
                        this._complianceTensor[4, 1] = value;
                        this._complianceTensor[5, 3] = 0.5 * value;
                        break;
                    case 9:
                        this._complianceTensor[1, 4] = value;
                        this._complianceTensor[4, 1] = value;
                        break;
                    default:
                        this._complianceTensor[1, 4] = 0;
                        this._complianceTensor[4, 1] = 0;
                        break;
                }
            }
        }
        public double S52Error
        {
            get
            {
                return this._complianceTensorError[4, 1];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 6:
                        this._complianceTensorError[0, 4] = -1 * value;
                        this._complianceTensorError[1, 4] = value;
                        this._complianceTensorError[3, 5] = 0.5 * value;
                        this._complianceTensorError[4, 0] = -1 * value;
                        this._complianceTensorError[4, 1] = value;
                        this._complianceTensorError[5, 3] = 0.5 * value;
                        break;
                    case 9:
                        this._complianceTensorError[1, 4] = value;
                        this._complianceTensorError[4, 1] = value;
                        break;
                    default:
                        this._complianceTensorError[1, 4] = 0;
                        this._complianceTensorError[4, 1] = 0;
                        break;
                }
            }
        }

        public double S53
        {
            get
            {
                return this._complianceTensor[4, 2];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 9:
                        this._complianceTensor[2, 4] = value;
                        this._complianceTensor[4, 2] = value;
                        break;
                    default:
                        this._complianceTensor[2, 4] = 0;
                        this._complianceTensor[4, 2] = 0;
                        break;
                }
            }
        }
        public double S53Error
        {
            get
            {
                return this._complianceTensorError[4, 2];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 9:
                        this._complianceTensorError[2, 4] = value;
                        this._complianceTensorError[4, 2] = value;
                        break;
                    default:
                        this._complianceTensorError[2, 4] = 0;
                        this._complianceTensorError[4, 2] = 0;
                        break;
                }
            }
        }

        public double S54
        {
            get
            {
                return this._complianceTensor[4, 3];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 8:
                        this._complianceTensor[3, 4] = value;
                        this._complianceTensor[4, 3] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._complianceTensor[3, 4] = 0;
                        this._complianceTensor[4, 3] = 0;
                        break;
                }
            }
        }
        public double S54Error
        {
            get
            {
                return this._complianceTensorError[4, 3];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 8:
                        this._complianceTensorError[3, 4] = value;
                        this._complianceTensorError[4, 3] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._complianceTensorError[3, 4] = 0;
                        this._complianceTensorError[4, 3] = 0;
                        break;
                }
            }
        }

        public double S55
        {
            get
            {
                return this._complianceTensor[4, 4];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        if (!this.IsIsotropic)
                        {
                            this._complianceTensor[3, 3] = value;
                            this._complianceTensor[4, 4] = value;
                            this._complianceTensor[5, 5] = value;
                        }
                        break;
                    case 2:
                        this._complianceTensor[3, 3] = value;
                        this._complianceTensor[4, 4] = value;
                        break;
                    case 3:
                        goto case 2;
                    case 4:
                        goto case 2;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._complianceTensor[4, 4] = value;
                        break;
                }
            }
        }
        public double S55Error
        {
            get
            {
                return this._complianceTensorError[4, 4];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        if (!this.IsIsotropic)
                        {
                            this._complianceTensorError[3, 3] = value;
                            this._complianceTensorError[4, 4] = value;
                            this._complianceTensorError[5, 5] = value;
                        }
                        break;
                    case 2:
                        this._complianceTensorError[3, 3] = value;
                        this._complianceTensorError[4, 4] = value;
                        break;
                    case 3:
                        goto case 2;
                    case 4:
                        goto case 2;
                    case 5:
                        goto case 2;
                    case 6:
                        goto case 2;
                    default:
                        this._complianceTensorError[4, 4] = value;
                        break;
                }
            }
        }

        public double S56
        {
            get
            {
                return this._complianceTensor[4, 5];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 5:
                        this._complianceTensor[0, 3] = 2 * value;
                        this._complianceTensor[1, 3] = -2 * value;
                        this._complianceTensor[4, 5] = value;
                        this._complianceTensor[3, 0] = 2 * value;
                        this._complianceTensor[3, 1] = -2 * value;
                        this._complianceTensor[5, 4] = value;
                        break;
                    case 6:
                        goto case 5;
                    case 9:
                        this._complianceTensor[4, 5] = value;
                        this._complianceTensor[5, 4] = value;
                        break;
                    default:
                        this._complianceTensor[4, 5] = 0;
                        this._complianceTensor[5, 4] = 0;
                        break;
                }
            }
        }
        public double S56Error
        {
            get
            {
                return this._complianceTensorError[4, 5];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 5:
                        this._complianceTensorError[0, 3] = 2 * value;
                        this._complianceTensorError[1, 3] = -2 * value;
                        this._complianceTensorError[4, 5] = value;
                        this._complianceTensorError[3, 0] = 2 * value;
                        this._complianceTensorError[3, 1] = -2 * value;
                        this._complianceTensorError[5, 4] = value;
                        break;
                    case 6:
                        goto case 5;
                    case 9:
                        this._complianceTensorError[4, 5] = value;
                        this._complianceTensorError[5, 4] = value;
                        break;
                    default:
                        this._complianceTensorError[4, 5] = 0;
                        this._complianceTensorError[5, 4] = 0;
                        break;
                }
            }
        }

        public double S61
        {
            get
            {
                return this._complianceTensor[5, 0];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 4:
                        this._complianceTensor[0, 5] = value;
                        this._complianceTensor[1, 5] = -1 * value;
                        this._complianceTensor[5, 0] = value;
                        this._complianceTensor[5, 1] = -1 * value;
                        break;
                    case 8:
                        this._complianceTensor[0, 5] = value;
                        this._complianceTensor[5, 0] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._complianceTensor[0, 5] = 0;
                        this._complianceTensor[5, 0] = 0;
                        break;
                }
            }
        }
        public double S61Error
        {
            get
            {
                return this._complianceTensorError[5, 0];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 4:
                        this._complianceTensorError[0, 5] = value;
                        this._complianceTensorError[1, 5] = -1 * value;
                        this._complianceTensorError[5, 0] = value;
                        this._complianceTensorError[5, 1] = -1 * value;
                        break;
                    case 8:
                        this._complianceTensorError[0, 5] = value;
                        this._complianceTensorError[5, 0] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._complianceTensorError[0, 5] = 0;
                        this._complianceTensorError[5, 0] = 0;
                        break;
                }
            }
        }

        public double S62
        {
            get
            {
                return this._complianceTensor[5, 1];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 4:
                        this._complianceTensor[0, 5] = -1 * value;
                        this._complianceTensor[1, 5] = value;
                        this._complianceTensor[5, 0] = -1 * value;
                        this._complianceTensor[5, 1] = value;
                        break;
                    case 8:
                        this._complianceTensor[1, 5] = value;
                        this._complianceTensor[5, 1] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._complianceTensor[1, 5] = 0;
                        this._complianceTensor[5, 1] = 0;
                        break;
                }
            }
        }
        public double S62Error
        {
            get
            {
                return this._complianceTensorError[5, 1];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 4:
                        this._complianceTensorError[0, 5] = -1 * value;
                        this._complianceTensorError[1, 5] = value;
                        this._complianceTensorError[5, 0] = -1 * value;
                        this._complianceTensorError[5, 1] = value;
                        break;
                    case 8:
                        this._complianceTensorError[1, 5] = value;
                        this._complianceTensorError[5, 1] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._complianceTensorError[1, 5] = 0;
                        this._complianceTensorError[5, 1] = 0;
                        break;
                }
            }
        }

        public double S63
        {
            get
            {
                return this._complianceTensor[5, 2];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 8:
                        this._complianceTensor[2, 5] = value;
                        this._complianceTensor[5, 2] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._complianceTensor[2, 5] = 0;
                        this._complianceTensor[5, 2] = 0;
                        break;
                }
            }
        }
        public double S63Error
        {
            get
            {
                return this._complianceTensorError[5, 2];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 8:
                        this._complianceTensorError[2, 5] = value;
                        this._complianceTensorError[5, 2] = value;
                        break;
                    case 9:
                        goto case 8;
                    default:
                        this._complianceTensorError[2, 5] = 0;
                        this._complianceTensorError[5, 2] = 0;
                        break;
                }
            }
        }

        public double S64
        {
            get
            {
                return this._complianceTensor[5, 3];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 6:
                        this._complianceTensor[0, 4] = -2 * value;
                        this._complianceTensor[1, 4] = 2 * value;
                        this._complianceTensor[3, 5] = value;
                        this._complianceTensor[4, 0] = -2 * value;
                        this._complianceTensor[4, 1] = 2 * value;
                        this._complianceTensor[5, 3] = value;
                        break;
                    case 9:
                        this._complianceTensor[3, 5] = value;
                        this._complianceTensor[5, 3] = value;
                        break;
                    default:
                        this._complianceTensor[3, 5] = 0;
                        this._complianceTensor[5, 3] = 0;
                        break;
                }
            }
        }
        public double S64Error
        {
            get
            {
                return this._complianceTensorError[5, 3];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 6:
                        this._complianceTensorError[0, 4] = -2 * value;
                        this._complianceTensorError[1, 4] = 2 * value;
                        this._complianceTensorError[3, 5] = value;
                        this._complianceTensorError[4, 0] = -2 * value;
                        this._complianceTensorError[4, 1] = 2 * value;
                        this._complianceTensorError[5, 3] = value;
                        break;
                    case 9:
                        this._complianceTensorError[3, 5] = value;
                        this._complianceTensorError[5, 3] = value;
                        break;
                    default:
                        this._complianceTensorError[3, 5] = 0;
                        this._complianceTensorError[5, 3] = 0;
                        break;
                }
            }
        }

        public double S65
        {
            get
            {
                return this._complianceTensor[5, 4];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 5:
                        this._complianceTensor[0, 3] = 2 * value;
                        this._complianceTensor[1, 3] = -2 * value;
                        this._complianceTensor[4, 5] = value;
                        this._complianceTensor[3, 0] = 2 * value;
                        this._complianceTensor[3, 1] = -2 * value;
                        this._complianceTensor[5, 4] = value;
                        break;
                    case 6:
                        goto case 5;
                    case 9:
                        this._complianceTensor[4, 5] = value;
                        this._complianceTensor[5, 4] = value;
                        break;
                    default:
                        this._complianceTensor[4, 5] = 0;
                        this._complianceTensor[5, 4] = 0;
                        break;
                }
            }
        }
        public double S65Error
        {
            get
            {
                return this._complianceTensorError[5, 4];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 5:
                        this._complianceTensorError[0, 3] = 2 * value;
                        this._complianceTensorError[1, 3] = -2 * value;
                        this._complianceTensorError[4, 5] = value;
                        this._complianceTensorError[3, 0] = 2 * value;
                        this._complianceTensorError[3, 1] = -2 * value;
                        this._complianceTensorError[5, 4] = value;
                        break;
                    case 6:
                        goto case 5;
                    case 9:
                        this._complianceTensorError[4, 5] = value;
                        this._complianceTensorError[5, 4] = value;
                        break;
                    default:
                        this._complianceTensorError[4, 5] = 0;
                        this._complianceTensorError[5, 4] = 0;
                        break;
                }
            }
        }

        public double S66
        {
            get
            {
                return this._complianceTensor[5, 5];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        if (!this.IsIsotropic)
                        {
                            this._complianceTensor[3, 3] = value;
                            this._complianceTensor[4, 4] = value;
                            this._complianceTensor[5, 5] = value;
                        }
                        break;
                    case 2:
                        break;
                    case 5:
                        break;
                    case 6:
                        break;
                    default:
                        this._complianceTensor[5, 5] = value;
                        break;
                }
            }
        }
        public double S66Error
        {
            get
            {
                return this._complianceTensorError[5, 5];
            }
            set
            {
                switch (this._symmetry)
                {
                    case 1:
                        if (!this.IsIsotropic)
                        {
                            this._complianceTensorError[3, 3] = value;
                            this._complianceTensorError[4, 4] = value;
                            this._complianceTensorError[5, 5] = value;
                        }
                        break;
                    case 2:
                        break;
                    case 5:
                        break;
                    case 6:
                        break;
                    default:
                        this._complianceTensorError[5, 5] = value;
                        break;
                }
            }
        }

        #endregion

        #endregion

        #endregion

        #region Calculation

        public void CalculateCompliances()
        {
            MathNet.Numerics.LinearAlgebra.Matrix<double> Inverted = this._stiffnessTensor.Inverse();

            #region uncorrected

            switch (this.Symmetry)
            {
                case "cubic":
                    if (IsIsotropic)
                    {
                        this.S11 = Inverted[0, 0];
                        this.S12 = Inverted[0, 1];
                    }
                    else
                    {
                        this.S11 = Inverted[0, 0];
                        this.S12 = Inverted[0, 1];
                        this.S44 = Inverted[3, 3];
                    }
                    break;
                case "hexagonal":
                    this.S11 = Inverted[0, 0];
                    this.S33 = Inverted[2, 2];
                    this.S12 = Inverted[0, 1];
                    this.S13 = Inverted[0, 2];
                    this.S44 = Inverted[3, 3];
                    break;
                case "tetragonal type 1":
                    this.S11 = Inverted[0, 0];
                    this.S33 = Inverted[2, 2];
                    this.S12 = Inverted[0, 1];
                    this.S13 = Inverted[0, 2];
                    this.S44 = Inverted[3, 3];
                    this.S66 = Inverted[5, 5];
                    break;
                case "tetragonal type 2":
                    this.S11 = Inverted[0, 0];
                    this.S33 = Inverted[2, 2];
                    this.S12 = Inverted[0, 1];
                    this.S13 = Inverted[0, 2];
                    this.S16 = Inverted[0, 5];
                    this.S44 = Inverted[3, 3];
                    this.S66 = Inverted[5, 5];
                    break;
                case "trigonal type 1":
                    this.S11 = Inverted[0, 0];
                    this.S33 = Inverted[2, 2];
                    this.S12 = Inverted[0, 1];
                    this.S13 = Inverted[0, 2];
                    this.S14 = Inverted[0, 3];
                    this.S44 = Inverted[3, 3];
                    break;
                case "trigonal type 2":
                    this.S11 = Inverted[0, 0];
                    this.S33 = Inverted[2, 2];
                    this.S12 = Inverted[0, 1];
                    this.S13 = Inverted[0, 2];
                    this.S14 = Inverted[0, 3];
                    this.S15 = Inverted[0, 4];
                    this.S44 = Inverted[3, 3];
                    break;
                case "rhombic":
                    this.S11 = Inverted[0, 0];
                    this.S22 = Inverted[1, 1];
                    this.S33 = Inverted[2, 2];
                    this.S12 = Inverted[0, 1];
                    this.S13 = Inverted[0, 2];
                    this.S23 = Inverted[1, 2];
                    this.S44 = Inverted[3, 3];
                    this.S55 = Inverted[4, 4];
                    this.S66 = Inverted[5, 5];
                    break;
                case "monoclinic":
                    this.S11 = Inverted[0, 0];
                    this.S22 = Inverted[1, 1];
                    this.S33 = Inverted[2, 2];
                    this.S12 = Inverted[0, 1];
                    this.S13 = Inverted[0, 2];
                    this.S16 = Inverted[0, 5];
                    this.S23 = Inverted[1, 2];
                    this.S26 = Inverted[1, 5];
                    this.S36 = Inverted[2, 5];
                    this.S44 = Inverted[3, 3];
                    this.S45 = Inverted[3, 4];
                    this.S55 = Inverted[4, 4];
                    this.S66 = Inverted[5, 5];
                    break;
                case "triclinic":
                    this.S11 = Inverted[0, 0];
                    this.S12 = Inverted[0, 1];
                    this.S13 = Inverted[0, 2];
                    this.S14 = Inverted[0, 3];
                    this.S15 = Inverted[0, 4];
                    this.S16 = Inverted[0, 5];
                    this.S22 = Inverted[1, 1];
                    this.S23 = Inverted[1, 2];
                    this.S24 = Inverted[1, 3];
                    this.S25 = Inverted[1, 4];
                    this.S26 = Inverted[1, 5];
                    this.S33 = Inverted[2, 2];
                    this.S34 = Inverted[2, 3];
                    this.S35 = Inverted[2, 4];
                    this.S36 = Inverted[2, 5];
                    this.S44 = Inverted[3, 3];
                    this.S45 = Inverted[3, 4];
                    this.S46 = Inverted[3, 5];
                    this.S55 = Inverted[4, 4];
                    this.S56 = Inverted[4, 5];
                    this.S66 = Inverted[5, 5];
                    break;
                default:
                    this.S11 = Inverted[0, 0];
                    this.S12 = Inverted[0, 1];
                    this.S13 = Inverted[0, 2];
                    this.S14 = Inverted[0, 3];
                    this.S15 = Inverted[0, 4];
                    this.S16 = Inverted[0, 5];
                    this.S22 = Inverted[1, 1];
                    this.S23 = Inverted[1, 2];
                    this.S24 = Inverted[1, 3];
                    this.S25 = Inverted[1, 4];
                    this.S26 = Inverted[1, 5];
                    this.S33 = Inverted[2, 2];
                    this.S34 = Inverted[2, 3];
                    this.S35 = Inverted[2, 4];
                    this.S36 = Inverted[2, 5];
                    this.S44 = Inverted[3, 3];
                    this.S45 = Inverted[3, 4];
                    this.S46 = Inverted[3, 5];
                    this.S55 = Inverted[4, 4];
                    this.S56 = Inverted[4, 5];
                    this.S66 = Inverted[5, 5];
                    break;
            }

            #endregion

            #region Corrected

            //switch (this.Symmetry)
            //{
            //    case "cubic":
            //        if (IsIsotropic)
            //        {
            //            this.S11 = Inverted[0, 0];
            //            this.S12 = Inverted[0, 1];
            //        }
            //        else
            //        {
            //            this.S11 = Inverted[0, 0];
            //            this.S12 = Inverted[0, 1];
            //            this.S44 = 4 * Inverted[3, 3];
            //        }
            //        break;
            //    case "hexagonal":
            //        this.S11 = Inverted[0, 0];
            //        this.S33 = Inverted[2, 2];
            //        this.S12 = Inverted[0, 1];
            //        this.S13 = Inverted[0, 2];
            //        this.S44 = 4 * Inverted[3, 3];
            //        break;
            //    case "tetragonal type 1":
            //        this.S11 = Inverted[0, 0];
            //        this.S33 = Inverted[2, 2];
            //        this.S12 = Inverted[0, 1];
            //        this.S13 = Inverted[0, 2];
            //        this.S44 = 4 * Inverted[3, 3];
            //        this.S66 = 4 * Inverted[5, 5];
            //        break;
            //    case "tetragonal type 2":
            //        this.S11 = Inverted[0, 0];
            //        this.S33 = Inverted[2, 2];
            //        this.S12 = Inverted[0, 1];
            //        this.S13 = Inverted[0, 2];
            //        this.S16 = 2 * Inverted[0, 5];
            //        this.S44 = 4 * Inverted[3, 3];
            //        this.S66 = 4 * Inverted[5, 5];
            //        break;
            //    case "trigonal type 1":
            //        this.S11 = Inverted[0, 0];
            //        this.S33 = Inverted[2, 2];
            //        this.S12 = Inverted[0, 1];
            //        this.S13 = Inverted[0, 2];
            //        this.S14 = 2 * Inverted[0, 3];
            //        this.S44 = 4 * Inverted[3, 3];
            //        break;
            //    case "trigonal type 2":
            //        this.S11 = Inverted[0, 0];
            //        this.S33 = Inverted[2, 2];
            //        this.S12 = Inverted[0, 1];
            //        this.S13 = Inverted[0, 2];
            //        this.S14 = 2 * Inverted[0, 3];
            //        this.S15 = 2 * Inverted[0, 4];
            //        this.S44 = 4 * Inverted[3, 3];
            //        break;
            //    case "rhombic":
            //        this.S11 = Inverted[0, 0];
            //        this.S22 = Inverted[1, 1];
            //        this.S33 = Inverted[2, 2];
            //        this.S12 = Inverted[0, 1];
            //        this.S13 = Inverted[0, 2];
            //        this.S23 = Inverted[1, 2];
            //        this.S44 = 4 * Inverted[3, 3];
            //        this.S55 = 4 * Inverted[4, 4];
            //        this.S66 = 4 * Inverted[5, 5];
            //        break;
            //    case "monoclinic":
            //        this.S11 = Inverted[0, 0];
            //        this.S22 = Inverted[1, 1];
            //        this.S33 = Inverted[2, 2];
            //        this.S12 = Inverted[0, 1];
            //        this.S13 = Inverted[0, 2];
            //        this.S16 = 2 * Inverted[0, 5];
            //        this.S23 = Inverted[1, 2];
            //        this.S26 = 2 * Inverted[1, 5];
            //        this.S36 = 2 * Inverted[2, 5];
            //        this.S44 = 4 * Inverted[3, 3];
            //        this.S45 = 4 * Inverted[3, 4];
            //        this.S55 = 4 * Inverted[4, 4];
            //        this.S66 = 4 * Inverted[5, 5];
            //        break;
            //    case "triclinic":
            //        this.S11 = Inverted[0, 0];
            //        this.S12 = Inverted[0, 1];
            //        this.S13 = Inverted[0, 2];
            //        this.S14 = 2 * Inverted[0, 3];
            //        this.S15 = 2 * Inverted[0, 4];
            //        this.S16 = 2 * Inverted[0, 5];
            //        this.S22 = Inverted[1, 1];
            //        this.S23 = Inverted[1, 2];
            //        this.S24 = 2 * Inverted[1, 3];
            //        this.S25 = 2 * Inverted[1, 4];
            //        this.S26 = 2 * Inverted[1, 5];
            //        this.S33 = Inverted[2, 2];
            //        this.S34 = 2 * Inverted[2, 3];
            //        this.S35 = 2 * Inverted[2, 4];
            //        this.S36 = 2 * Inverted[2, 5];
            //        this.S44 = 4 * Inverted[3, 3];
            //        this.S45 = 4 * Inverted[3, 4];
            //        this.S46 = 4 * Inverted[3, 5];
            //        this.S55 = 4 * Inverted[4, 4];
            //        this.S56 = 4 * Inverted[4, 5];
            //        this.S66 = 4 * Inverted[5, 5];
            //        break;
            //    default:
            //        this.S11 = Inverted[0, 0];
            //        this.S12 = Inverted[0, 1];
            //        this.S13 = Inverted[0, 2];
            //        this.S14 = 2 * Inverted[0, 3];
            //        this.S15 = 2 * Inverted[0, 4];
            //        this.S16 = 2 * Inverted[0, 5];
            //        this.S22 = Inverted[1, 1];
            //        this.S23 = Inverted[1, 2];
            //        this.S24 = 2 * Inverted[1, 3];
            //        this.S25 = 2 * Inverted[1, 4];
            //        this.S26 = 2 * Inverted[1, 5];
            //        this.S33 = Inverted[2, 2];
            //        this.S34 = 2 * Inverted[2, 3];
            //        this.S35 = 2 * Inverted[2, 4];
            //        this.S36 = 2 * Inverted[2, 5];
            //        this.S44 = 4 * Inverted[3, 3];
            //        this.S45 = 4 * Inverted[3, 4];
            //        this.S46 = 4 * Inverted[3, 5];
            //        this.S55 = 4 * Inverted[4, 4];
            //        this.S56 = 4 * Inverted[4, 5];
            //        this.S66 = 4 * Inverted[5, 5];
            //        break;
            //}

            #endregion
        }

        public void CalculateStiffnesses()
        {
            #region correction

            //MathNet.Numerics.LinearAlgebra.Matrix<double> ForInversion = createStiffnessMatrix();


            //for(int n = 0; n < 6; n++)
            //{
            //    for(int i = 0; i < 6; i++)
            //    {
            //        if(n < 3)
            //        {
            //            if(i > 2)
            //            {
            //                ForInversion[n, i] *= 2;
            //            }
            //        }
            //        else
            //        {
            //            if(i < 3)
            //            {
            //                ForInversion[n, i] *= 2;
            //            }
            //            else
            //            {
            //                ForInversion[n, i] *= 4;
            //            }
            //        }
            //    }
            //}

            //MathNet.Numerics.LinearAlgebra.Matrix<double> Inverted = ForInversion.Inverse();

            #endregion

            MathNet.Numerics.LinearAlgebra.Matrix<double> Inverted = this._complianceTensor.Inverse();

            switch (this.Symmetry)
            {
                case "cubic":
                    if (IsIsotropic)
                    {
                        this.C11 = Inverted[0, 0];
                        this.C12 = Inverted[0, 1];
                    }
                    else
                    {
                        this.C11 = Inverted[0, 0];
                        this.C12 = Inverted[0, 1];
                        this.C44 = Inverted[3, 3];
                    }
                    break;
                case "hexagonal":
                    this.C11 = Inverted[0, 0];
                    this.C33 = Inverted[2, 2];
                    this.C12 = Inverted[0, 1];
                    this.C13 = Inverted[0, 2];
                    this.C44 = Inverted[3, 3];
                    break;
                case "tetragonal type 1":
                    this.C11 = Inverted[0, 0];
                    this.C33 = Inverted[2, 2];
                    this.C12 = Inverted[0, 1];
                    this.C13 = Inverted[0, 2];
                    this.C44 = Inverted[3, 3];
                    this.C66 = Inverted[5, 5];
                    break;
                case "tetragonal type 2":
                    this.C11 = Inverted[0, 0];
                    this.C33 = Inverted[2, 2];
                    this.C12 = Inverted[0, 1];
                    this.C13 = Inverted[0, 2];
                    this.C16 = Inverted[0, 5];
                    this.C44 = Inverted[3, 3];
                    this.C66 = Inverted[5, 5];
                    break;
                case "trigonal type 1":
                    this.C11 = Inverted[0, 0];
                    this.C33 = Inverted[2, 2];
                    this.C12 = Inverted[0, 1];
                    this.C13 = Inverted[0, 2];
                    this.C14 = Inverted[0, 3];
                    this.C44 = Inverted[3, 3];
                    break;
                case "trigonal type 2":
                    this.C11 = Inverted[0, 0];
                    this.C33 = Inverted[2, 2];
                    this.C12 = Inverted[0, 1];
                    this.C13 = Inverted[0, 2];
                    this.C14 = Inverted[0, 3];
                    this.C15 = Inverted[0, 4];
                    this.C44 = Inverted[3, 3];
                    break;
                case "rhombic":
                    this.C11 = Inverted[0, 0];
                    this.C22 = Inverted[1, 1];
                    this.C33 = Inverted[2, 2];
                    this.C12 = Inverted[0, 1];
                    this.C13 = Inverted[0, 2];
                    this.C23 = Inverted[1, 2];
                    this.C44 = Inverted[3, 3];
                    this.C55 = Inverted[4, 4];
                    this.C66 = Inverted[5, 5];
                    break;
                case "monoclinic":
                    this.C11 = Inverted[0, 0];
                    this.C22 = Inverted[1, 1];
                    this.C33 = Inverted[2, 2];
                    this.C12 = Inverted[0, 1];
                    this.C13 = Inverted[0, 2];
                    this.C16 = Inverted[0, 5];
                    this.C23 = Inverted[1, 2];
                    this.C26 = Inverted[1, 5];
                    this.C36 = Inverted[2, 5];
                    this.C44 = Inverted[3, 3];
                    this.C45 = Inverted[3, 4];
                    this.C55 = Inverted[4, 4];
                    this.C66 = Inverted[5, 5];
                    break;
                case "triclinic":
                    this.C11 = Inverted[0, 0];
                    this.C12 = Inverted[0, 1];
                    this.C13 = Inverted[0, 2];
                    this.C14 = Inverted[0, 3];
                    this.C15 = Inverted[0, 4];
                    this.C16 = Inverted[0, 5];
                    this.C22 = Inverted[1, 1];
                    this.C23 = Inverted[1, 2];
                    this.C24 = Inverted[1, 3];
                    this.C25 = Inverted[1, 4];
                    this.C26 = Inverted[1, 5];
                    this.C33 = Inverted[2, 2];
                    this.C34 = Inverted[2, 3];
                    this.C35 = Inverted[2, 4];
                    this.C36 = Inverted[2, 5];
                    this.C44 = Inverted[3, 3];
                    this.C45 = Inverted[3, 4];
                    this.C46 = Inverted[3, 5];
                    this.C55 = Inverted[4, 4];
                    this.C56 = Inverted[4, 5];
                    this.C66 = Inverted[5, 5];
                    break;
                default:
                    this.C11 = Inverted[0, 0];
                    this.C12 = Inverted[0, 1];
                    this.C13 = Inverted[0, 2];
                    this.C14 = Inverted[0, 3];
                    this.C15 = Inverted[0, 4];
                    this.C16 = Inverted[0, 5];
                    this.C22 = Inverted[1, 1];
                    this.C23 = Inverted[1, 2];
                    this.C24 = Inverted[1, 3];
                    this.C25 = Inverted[1, 4];
                    this.C26 = Inverted[1, 5];
                    this.C33 = Inverted[2, 2];
                    this.C34 = Inverted[2, 3];
                    this.C35 = Inverted[2, 4];
                    this.C36 = Inverted[2, 5];
                    this.C44 = Inverted[3, 3];
                    this.C45 = Inverted[3, 4];
                    this.C46 = Inverted[3, 5];
                    this.C55 = Inverted[4, 4];
                    this.C56 = Inverted[4, 5];
                    this.C66 = Inverted[5, 5];
                    break;
            }
        }

        //public MathNet.Numerics.LinearAlgebra.Matrix<double> createStiffnessMatrix()
        //{
        //    MathNet.Numerics.LinearAlgebra.Matrix<double> Ret = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(6, 6, 0.0);

        //    Ret[0, 0] = this.S11;
        //    Ret[0, 1] = this.S12;
        //    Ret[0, 2] = this.S13;
        //    Ret[0, 3] = 0.5 * this.S14;
        //    Ret[0, 4] = 0.5 * this.S15;
        //    Ret[0, 5] = 0.5 * this.S16;

        //    Ret[1, 0] = this.S21;
        //    Ret[1, 1] = this.S22;
        //    Ret[1, 2] = this.S23;
        //    Ret[1, 3] = 0.5 * this.S24;
        //    Ret[1, 4] = 0.5 * this.S25;
        //    Ret[1, 5] = 0.5 * this.S26;

        //    Ret[2, 0] = this.S31;
        //    Ret[2, 1] = this.S32;
        //    Ret[2, 2] = this.S33;
        //    Ret[2, 3] = 0.5 * this.S34;
        //    Ret[2, 4] = 0.5 * this.S35;
        //    Ret[2, 5] = 0.5 * this.S36;

        //    Ret[3, 0] = 0.5 * this.S41;
        //    Ret[3, 1] = 0.5 * this.S42;
        //    Ret[3, 2] = 0.5 * this.S43;
        //    Ret[3, 3] = 0.25 * this.S44;
        //    Ret[3, 4] = 0.25 * this.S45;
        //    Ret[3, 5] = 0.25 * this.S46;

        //    Ret[4, 0] = 0.5 * this.S51;
        //    Ret[4, 1] = 0.5 * this.S52;
        //    Ret[4, 2] = 0.5 * this.S53;
        //    Ret[4, 3] = 0.25 * this.S54;
        //    Ret[4, 4] = 0.25 * this.S55;
        //    Ret[4, 5] = 0.25 * this.S56;

        //    Ret[5, 0] = 0.5 * this.S61;
        //    Ret[5, 1] = 0.5 * this.S62;
        //    Ret[5, 2] = 0.5 * this.S63;
        //    Ret[5, 3] = 0.25 * this.S64;
        //    Ret[5, 4] = 0.25 * this.S65;
        //    Ret[5, 5] = 0.25 * this.S66;

        //    return Ret;
        //}

        public void SetErrors(double percentage)
        {
            for(int n = 0; n < 6; n++)
            {
                for(int i = 0; i < 6; i++)
                {
                    if(this._stiffnessTensorError[n, i] == 0.0)
                    {
                        this._stiffnessTensorError[n, i] = percentage * this._stiffnessTensor[n, i];
                    }
                    if (this._complianceTensorError[n, i] == 0.0)
                    {
                        this._complianceTensorError[n, i] = percentage * this._complianceTensor[n, i];
                    }
                }
            }
        }

        #endregion

        #region Fitting

        public void FitVoigt(bool classicCalculation)
        {
            switch (this.Symmetry)
            {
                case "cubic":
                    if(this.IsIsotropic)
                    {
                        this.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorVoigtCubicIsotrope(this, classicCalculation);
                    }
                    else
                    {
                        this.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorVoigtCubic(this, classicCalculation);
                    }
                    break;
                case "hexagonal":
                    this.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorVoigtType1(this, classicCalculation);
                    break;
                case "tetragonal type 1":
                    this.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorVoigtType2(this, classicCalculation);
                    break;
                case "tetragonal type 2":
                    this.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorVoigtType2(this, classicCalculation);
                    break;
                case "trigonal type 1":
                    this.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorVoigtType1(this, classicCalculation);
                    break;
                case "trigonal type 2":
                    this.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorVoigtType1(this, classicCalculation);
                    break;
                case "rhombic":
                    this.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorVoigtType3(this, classicCalculation);
                    break;
                case "monoclinic":
                    this.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorVoigtType3(this, classicCalculation);
                    break;
                case "triclinic":
                    this.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorVoigtType3(this, classicCalculation);
                    break;
                default:
                    this.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorVoigtType3(this, classicCalculation);
                    break;
            }

            this.CalculateCompliances();
        }

        public void FitReuss(bool classicCalculation)
        {
            switch (this.Symmetry)
            {
                case "cubic":
                    this.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorReussCubic(this, classicCalculation);
                    break;
                case "hexagonal":
                    this.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorReussHexagonal(this, classicCalculation);
                    break;
                case "tetragonal type 1":
                    
                    break;
                case "tetragonal type 2":
                    
                    break;
                case "trigonal type 1":
                    
                    break;
                case "trigonal type 2":
                    
                    break;
                case "rhombic":
                    
                    break;
                case "monoclinic":
                    
                    break;
                case "triclinic":
                    
                    break;
                default:
                    
                    break;
            }

            this.CalculateStiffnesses();
            SetFittingErrorsReuss(classicCalculation);
        }

        public void FitHill(bool classicCalculation)
        {
            switch (this.Symmetry)
            {
                case "cubic":
                    this.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorHillCubic(this, classicCalculation);
                    break;
                case "hexagonal":
                    this.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorHillHexagonal(this, classicCalculation);
                    break;
                case "tetragonal type 1":

                    break;
                case "tetragonal type 2":

                    break;
                case "trigonal type 1":

                    break;
                case "trigonal type 2":

                    break;
                case "rhombic":

                    break;
                case "monoclinic":

                    break;
                case "triclinic":

                    break;
                default:

                    break;
            }

            this.CalculateStiffnesses();
            SetFittingErrorsHill(classicCalculation);
        }

        public void FitKroener(bool classicCalculation, bool stiffnessCalc)
        {
            if (stiffnessCalc)
            {
                switch (this.Symmetry)
                {
                    case "cubic":
                        this.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorKroenerCubicStiffness(this, classicCalculation);
                        break;
                    case "hexagonal":
                        break;
                    case "tetragonal type 1":

                        break;
                    case "tetragonal type 2":

                        break;
                    case "trigonal type 1":

                        break;
                    case "trigonal type 2":

                        break;
                    case "rhombic":

                        break;
                    case "monoclinic":

                        break;
                    case "triclinic":

                        break;
                    default:

                        break;
                }

                this.CalculateCompliances();
                SetFittingErrorsKroener(classicCalculation);
            }
            else
            {
                switch (this.Symmetry)
                {
                    case "cubic":
                        this.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorKroenerCubicCompliance(this, classicCalculation);
                        break;
                    case "hexagonal":
                        break;
                    case "tetragonal type 1":

                        break;
                    case "tetragonal type 2":

                        break;
                    case "trigonal type 1":

                        break;
                    case "trigonal type 2":

                        break;
                    case "rhombic":

                        break;
                    case "monoclinic":

                        break;
                    case "triclinic":

                        break;
                    default:

                        break;
                }

                this.CalculateStiffnesses();
                SetFittingErrorsKroener(classicCalculation);
            }
        }

        public void FitDeWitt(bool classicCalculation, bool stiffnessCalc)
        {
            if (stiffnessCalc)
            {
                switch (this.Symmetry)
                {
                    case "cubic":
                        this.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorDeWittCubicStiffness(this, classicCalculation);
                        break;
                    case "hexagonal":
                        break;
                    case "tetragonal type 1":

                        break;
                    case "tetragonal type 2":

                        break;
                    case "trigonal type 1":

                        break;
                    case "trigonal type 2":

                        break;
                    case "rhombic":

                        break;
                    case "monoclinic":

                        break;
                    case "triclinic":

                        break;
                    default:

                        break;
                }

                this.CalculateCompliances();
                SetFittingErrorsDeWitt(classicCalculation);
            }
            else
            {
                switch (this.Symmetry)
                {
                    case "cubic":
                        this.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorDeWittCubicCompliance(this, classicCalculation);
                        break;
                    case "hexagonal":
                        break;
                    case "tetragonal type 1":

                        break;
                    case "tetragonal type 2":

                        break;
                    case "trigonal type 1":

                        break;
                    case "trigonal type 2":

                        break;
                    case "rhombic":

                        break;
                    case "monoclinic":

                        break;
                    case "triclinic":

                        break;
                    default:

                        break;
                }

                this.CalculateStiffnesses();
                SetFittingErrorsDeWitt(classicCalculation);
            }
        }

        public void FitGeometricHill(bool classicCalculation)
        {
            switch (this.Symmetry)
            {
                case "cubic":
                    this.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorGeometricHillCubic(this, classicCalculation);
                    break;
                case "hexagonal":
                    this.FitConverged = Analysis.Fitting.LMA.FitElasticityTensorGeometricHillHexagonal(this, classicCalculation);
                    break;
                case "tetragonal type 1":

                    break;
                case "tetragonal type 2":

                    break;
                case "trigonal type 1":

                    break;
                case "trigonal type 2":

                    break;
                case "rhombic":

                    break;
                case "monoclinic":

                    break;
                case "triclinic":

                    break;
                default:

                    break;
            }

            this.CalculateStiffnesses();
            SetFittingErrorsHill(classicCalculation);
        }

        #region Error calculation

        public void SetFittingErrorsDeWitt(bool classicCalculation)
        {
            switch (this.Symmetry)
            {
                case "cubic":
                    SetErrorDeWittCubic();
                    break;
                case "hexagonal":
                    break;
                case "tetragonal type 1":

                    break;
                case "tetragonal type 2":

                    break;
                case "trigonal type 1":

                    break;
                case "trigonal type 2":

                    break;
                case "rhombic":

                    break;
                case "monoclinic":

                    break;
                case "triclinic":

                    break;
                default:

                    break;
            }
        }

        public void SetFittingErrorsKroener(bool classicCalculation)
        {
            switch (this.Symmetry)
            {
                case "cubic":
                    SetErrorKroenerCubic();
                    break;
                case "hexagonal":
                    break;
                case "tetragonal type 1":

                    break;
                case "tetragonal type 2":

                    break;
                case "trigonal type 1":

                    break;
                case "trigonal type 2":

                    break;
                case "rhombic":

                    break;
                case "monoclinic":

                    break;
                case "triclinic":

                    break;
                default:

                    break;
            }
        }

        public void SetFittingErrorsHill(bool classicCalculation)
        {
            switch (this.Symmetry)
            {
                case "cubic":
                    SetErrorHillCubic();
                    break;
                case "hexagonal":
                    SetErrorHillHexagonal();
                    break;
                case "tetragonal type 1":

                    break;
                case "tetragonal type 2":

                    break;
                case "trigonal type 1":

                    break;
                case "trigonal type 2":

                    break;
                case "rhombic":

                    break;
                case "monoclinic":

                    break;
                case "triclinic":

                    break;
                default:

                    break;
            }
        }

        public void SetFittingErrorsReuss(bool classicCalculation)
        {
            switch (this.Symmetry)
            {
                case "cubic":
                    SetErrorReussCubic();
                    break;
                case "hexagonal":
                    SetErrorReussHexagonal();
                    break;
                case "tetragonal type 1":

                    break;
                case "tetragonal type 2":

                    break;
                case "trigonal type 1":

                    break;
                case "trigonal type 2":

                    break;
                case "rhombic":

                    break;
                case "monoclinic":

                    break;
                case "triclinic":

                    break;
                default:

                    break;
            }
        }

        private void SetErrorDeWittCubic()
        {
            //Stiffness Cubic
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrixStiffness = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

            for (int n = 0; n < this.DiffractionConstants.Count; n++)
            {
                #region Matrix Build

                HessianMatrixStiffness[0, 0] += (this.FirstDerivativeC11S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeC11S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixStiffness[0, 0] += (this.FirstDerivativeC11HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeC11HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrixStiffness[1, 1] += (this.FirstDerivativeC12S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeC12S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixStiffness[1, 1] += (this.FirstDerivativeC12HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeC12HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixStiffness[0, 1] += (this.FirstDerivativeC12S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeC11S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixStiffness[0, 1] += (this.FirstDerivativeC12HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeC11HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixStiffness[1, 0] += (this.FirstDerivativeC12S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeC11S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixStiffness[1, 0] += (this.FirstDerivativeC12HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeC11HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrixStiffness[2, 2] += (this.FirstDerivativeC44S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeC44S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixStiffness[2, 2] += (this.FirstDerivativeC44HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeC44HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixStiffness[0, 2] += (this.FirstDerivativeC44S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeC11S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixStiffness[0, 2] += (this.FirstDerivativeC44HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeC11HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixStiffness[2, 0] += (this.FirstDerivativeC44S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeC11S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixStiffness[2, 0] += (this.FirstDerivativeC44HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeC11HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixStiffness[1, 2] += (this.FirstDerivativeC44S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeC12S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixStiffness[1, 2] += (this.FirstDerivativeC44HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeC12HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixStiffness[2, 1] += (this.FirstDerivativeC44S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeC12S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixStiffness[2, 1] += (this.FirstDerivativeC44HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeC12HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                #endregion
            }

            //Compliance
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrixCompliance = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

            for (int n = 0; n < this.DiffractionConstants.Count; n++)
            {
                #region Matrix Build

                HessianMatrixCompliance[0, 0] += (this.FirstDerivativeS11S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[0, 0] += (this.FirstDerivativeS11HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrixCompliance[1, 1] += (this.FirstDerivativeS12S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[1, 1] += (this.FirstDerivativeS12HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[0, 1] += (this.FirstDerivativeS12S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[0, 1] += (this.FirstDerivativeS12HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[1, 0] += (this.FirstDerivativeS12S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[1, 0] += (this.FirstDerivativeS12HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrixCompliance[2, 2] += (this.FirstDerivativeS44S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS44S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[2, 2] += (this.FirstDerivativeS44HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS44HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[0, 2] += (this.FirstDerivativeS44S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[0, 2] += (this.FirstDerivativeS44HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[2, 0] += (this.FirstDerivativeS44S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[2, 0] += (this.FirstDerivativeS44HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[1, 2] += (this.FirstDerivativeS44S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[1, 2] += (this.FirstDerivativeS44HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[2, 1] += (this.FirstDerivativeS44S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[2, 1] += (this.FirstDerivativeS44HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                #endregion
            }

            MathNet.Numerics.LinearAlgebra.Matrix<double> CoVMatrixStiffness = HessianMatrixStiffness.Inverse();
            MathNet.Numerics.LinearAlgebra.Matrix<double> CoVMatrixCompliance = HessianMatrixCompliance.Inverse();

            this.S11Error = Math.Sqrt(CoVMatrixCompliance[0, 0]);
            this.S12Error = Math.Sqrt(CoVMatrixCompliance[1, 1]);
            this.S44Error = Math.Sqrt(CoVMatrixCompliance[2, 2]);

            this.C11Error = Math.Sqrt(CoVMatrixStiffness[0, 0]);
            this.C12Error = Math.Sqrt(CoVMatrixStiffness[1, 1]);
            this.C44Error = Math.Sqrt(CoVMatrixStiffness[2, 2]);

        }

        private void SetErrorKroenerCubic()
        {
            //Stiffness Cubic
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrixStiffness = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

            for (int n = 0; n < this.DiffractionConstants.Count; n++)
            {
                #region Matrix Build

                HessianMatrixStiffness[0, 0] += (this.FirstDerivativeC11S1KroenerCubic() * this.FirstDerivativeC11S1KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixStiffness[0, 0] += (this.FirstDerivativeC11HS2KroenerCubic() * this.FirstDerivativeC11HS2KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrixStiffness[1, 1] += (this.FirstDerivativeC12S1KroenerCubic() * this.FirstDerivativeC12S1KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixStiffness[1, 1] += (this.FirstDerivativeC12HS2KroenerCubic() * this.FirstDerivativeC12HS2KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixStiffness[0, 1] += (this.FirstDerivativeC12S1KroenerCubic() * this.FirstDerivativeC11S1KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixStiffness[0, 1] += (this.FirstDerivativeC12HS2KroenerCubic() * this.FirstDerivativeC11HS2KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixStiffness[1, 0] += (this.FirstDerivativeC12S1KroenerCubic() * this.FirstDerivativeC11S1KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixStiffness[1, 0] += (this.FirstDerivativeC12HS2KroenerCubic() * this.FirstDerivativeC11HS2KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrixStiffness[2, 2] += (this.FirstDerivativeC44S1KroenerCubic() * this.FirstDerivativeC44S1KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixStiffness[2, 2] += (this.FirstDerivativeC44HS2KroenerCubic() * this.FirstDerivativeC44HS2KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixStiffness[0, 2] += (this.FirstDerivativeC44S1KroenerCubic() * this.FirstDerivativeC11S1KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixStiffness[0, 2] += (this.FirstDerivativeC44HS2KroenerCubic() * this.FirstDerivativeC11HS2KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixStiffness[2, 0] += (this.FirstDerivativeC44S1KroenerCubic() * this.FirstDerivativeC11S1KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixStiffness[2, 0] += (this.FirstDerivativeC44HS2KroenerCubic() * this.FirstDerivativeC11HS2KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixStiffness[1, 2] += (this.FirstDerivativeC44S1KroenerCubic() * this.FirstDerivativeC12S1KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixStiffness[1, 2] += (this.FirstDerivativeC44HS2KroenerCubic() * this.FirstDerivativeC12HS2KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixStiffness[2, 1] += (this.FirstDerivativeC44S1KroenerCubic() * this.FirstDerivativeC12S1KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixStiffness[2, 1] += (this.FirstDerivativeC44HS2KroenerCubic() * this.FirstDerivativeC12HS2KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                #endregion
            }

            //Compliance
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrixCompliance = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

            for (int n = 0; n < this.DiffractionConstants.Count; n++)
            {
                #region Matrix Build

                HessianMatrixCompliance[0, 0] += (this.FirstDerivativeS11S1KroenerCubic() * this.FirstDerivativeS11S1KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[0, 0] += (this.FirstDerivativeS11HS2KroenerCubic() * this.FirstDerivativeS11HS2KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrixCompliance[1, 1] += (this.FirstDerivativeS12S1KroenerCubic() * this.FirstDerivativeS12S1KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[1, 1] += (this.FirstDerivativeS12HS2KroenerCubic() * this.FirstDerivativeS12HS2KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[0, 1] += (this.FirstDerivativeS12S1KroenerCubic() * this.FirstDerivativeS11S1KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[0, 1] += (this.FirstDerivativeS12HS2KroenerCubic() * this.FirstDerivativeS11HS2KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[1, 0] += (this.FirstDerivativeS12S1KroenerCubic() * this.FirstDerivativeS11S1KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[1, 0] += (this.FirstDerivativeS12HS2KroenerCubic() * this.FirstDerivativeS11HS2KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrixCompliance[2, 2] += (this.FirstDerivativeS44S1KroenerCubic() * this.FirstDerivativeS44S1KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[2, 2] += (this.FirstDerivativeS44HS2KroenerCubic() * this.FirstDerivativeS44HS2KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[0, 2] += (this.FirstDerivativeS44S1KroenerCubic() * this.FirstDerivativeS11S1KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[0, 2] += (this.FirstDerivativeS44HS2KroenerCubic() * this.FirstDerivativeS11HS2KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[2, 0] += (this.FirstDerivativeS44S1KroenerCubic() * this.FirstDerivativeS11S1KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[2, 0] += (this.FirstDerivativeS44HS2KroenerCubic() * this.FirstDerivativeS11HS2KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[1, 2] += (this.FirstDerivativeS44S1KroenerCubic() * this.FirstDerivativeS12S1KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[1, 2] += (this.FirstDerivativeS44HS2KroenerCubic() * this.FirstDerivativeS12HS2KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[2, 1] += (this.FirstDerivativeS44S1KroenerCubic() * this.FirstDerivativeS12S1KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[2, 1] += (this.FirstDerivativeS44HS2KroenerCubic() * this.FirstDerivativeS12HS2KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                #endregion
            }

            MathNet.Numerics.LinearAlgebra.Matrix<double> CoVMatrixStiffness = HessianMatrixStiffness.Inverse();
            MathNet.Numerics.LinearAlgebra.Matrix<double> CoVMatrixCompliance = HessianMatrixCompliance.Inverse();

            this.S11Error = Math.Sqrt(CoVMatrixCompliance[0, 0]);
            this.S12Error = Math.Sqrt(CoVMatrixCompliance[1, 1]);
            this.S44Error = Math.Sqrt(CoVMatrixCompliance[2, 2]);

            this.C11Error = Math.Sqrt(CoVMatrixStiffness[0, 0]);
            this.C12Error = Math.Sqrt(CoVMatrixStiffness[1, 1]);
            this.C44Error = Math.Sqrt(CoVMatrixStiffness[2, 2]);

        }

        private void SetErrorHillCubic()
        {
            //Compliance
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrixCompliance = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

            for (int n = 0; n < this.DiffractionConstants.Count; n++)
            {
                #region Matrix Build

                HessianMatrixCompliance[0, 0] += (this.FirstDerivativeS11S1HillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1HillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[0, 0] += (this.FirstDerivativeS11HS2HillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2HillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrixCompliance[1, 1] += (this.FirstDerivativeS12S1HillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1HillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[1, 1] += (this.FirstDerivativeS12HS2HillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2HillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[0, 1] += (this.FirstDerivativeS12S1HillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1HillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[0, 1] += (this.FirstDerivativeS12HS2HillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2HillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[1, 0] += (this.FirstDerivativeS12S1HillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1HillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[1, 0] += (this.FirstDerivativeS12HS2HillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2HillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrixCompliance[2, 2] += (this.FirstDerivativeS44S1HillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS44S1HillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[2, 2] += (this.FirstDerivativeS44HS2HillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS44HS2HillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[0, 2] += (this.FirstDerivativeS44S1HillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1HillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[0, 2] += (this.FirstDerivativeS44HS2HillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2HillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[2, 0] += (this.FirstDerivativeS44S1HillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1HillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[2, 0] += (this.FirstDerivativeS44HS2HillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2HillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[1, 2] += (this.FirstDerivativeS44S1HillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1HillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[1, 2] += (this.FirstDerivativeS44HS2HillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2HillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[2, 1] += (this.FirstDerivativeS44S1HillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1HillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[2, 1] += (this.FirstDerivativeS44HS2HillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2HillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                
                #endregion
                
            }

            MathNet.Numerics.LinearAlgebra.Matrix<double> CoVMatrixCompliance = HessianMatrixCompliance.Inverse();

            this.S11Error = Math.Sqrt(CoVMatrixCompliance[0, 0]);
            this.S12Error = Math.Sqrt(CoVMatrixCompliance[1, 1]);
            this.S44Error = Math.Sqrt(CoVMatrixCompliance[2, 2]);

            MathNet.Numerics.LinearAlgebra.Matrix<double> ErrorMatrixStiffness =this._complianceTensorError.Inverse();

            this.C11Error = ErrorMatrixStiffness[0, 0];
            this.C12Error = ErrorMatrixStiffness[1, 1];
            this.C44Error = ErrorMatrixStiffness[2, 2];

        }

        private void SetErrorHillHexagonal()
        {
            //Compliance
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrixCompliance = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(5, 5, 0.0);

            for (int n = 0; n < this.DiffractionConstants.Count; n++)
            {
                #region Matrix Build

                HessianMatrixCompliance[0, 0] += (this.FirstDerivativeS11S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[0, 0] += (this.FirstDerivativeS11HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrixCompliance[1, 1] += (this.FirstDerivativeS33S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[1, 1] += (this.FirstDerivativeS33HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[0, 1] += (this.FirstDerivativeS33S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[0, 1] += (this.FirstDerivativeS33HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[1, 0] += (this.FirstDerivativeS33S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[1, 0] += (this.FirstDerivativeS33HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrixCompliance[2, 2] += (this.FirstDerivativeS12S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[2, 2] += (this.FirstDerivativeS12HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[0, 2] += (this.FirstDerivativeS12S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[0, 2] += (this.FirstDerivativeS12HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[2, 0] += (this.FirstDerivativeS12S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[2, 0] += (this.FirstDerivativeS12HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[1, 2] += (this.FirstDerivativeS12S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[1, 2] += (this.FirstDerivativeS12HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[2, 1] += (this.FirstDerivativeS12S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[2, 1] += (this.FirstDerivativeS12HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrixCompliance[3, 3] += (this.FirstDerivativeS13S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[3, 3] += (this.FirstDerivativeS13HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[0, 3] += (this.FirstDerivativeS13S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[0, 3] += (this.FirstDerivativeS13HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[3, 0] += (this.FirstDerivativeS13S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[3, 0] += (this.FirstDerivativeS13HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[1, 3] += (this.FirstDerivativeS13S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[1, 3] += (this.FirstDerivativeS13HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[3, 1] += (this.FirstDerivativeS13S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[3, 1] += (this.FirstDerivativeS13HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[2, 3] += (this.FirstDerivativeS13S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[2, 3] += (this.FirstDerivativeS13HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[3, 2] += (this.FirstDerivativeS13S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[3, 2] += (this.FirstDerivativeS13HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrixCompliance[4, 4] += (this.FirstDerivativeS44S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS44S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[4, 4] += (this.FirstDerivativeS44HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS44HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[0, 4] += (this.FirstDerivativeS44S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[0, 4] += (this.FirstDerivativeS44HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[4, 0] += (this.FirstDerivativeS44S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[4, 0] += (this.FirstDerivativeS44HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[1, 4] += (this.FirstDerivativeS44S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[1, 4] += (this.FirstDerivativeS44HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[4, 1] += (this.FirstDerivativeS44S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[4, 1] += (this.FirstDerivativeS44HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[2, 4] += (this.FirstDerivativeS44S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[2, 4] += (this.FirstDerivativeS44HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[4, 2] += (this.FirstDerivativeS44S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[4, 2] += (this.FirstDerivativeS44HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[3, 4] += (this.FirstDerivativeS44S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[3, 4] += (this.FirstDerivativeS44HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[4, 3] += (this.FirstDerivativeS44S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[4, 3] += (this.FirstDerivativeS44HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                #endregion
            }

            MathNet.Numerics.LinearAlgebra.Matrix<double> CoVMatrixCompliance = HessianMatrixCompliance.Inverse();

            this.S11Error = Math.Sqrt(CoVMatrixCompliance[0, 0]);
            this.S33Error = Math.Sqrt(CoVMatrixCompliance[1, 1]);
            this.S12Error = Math.Sqrt(CoVMatrixCompliance[2, 2]);
            this.S13Error = Math.Sqrt(CoVMatrixCompliance[3, 3]);
            this.S44Error = Math.Sqrt(CoVMatrixCompliance[4, 4]);

            MathNet.Numerics.LinearAlgebra.Matrix<double> ErrorMatrixStiffness = this._complianceTensorError.Inverse();

            this.C11Error = ErrorMatrixStiffness[0, 0];
            this.C33Error = ErrorMatrixStiffness[1, 1];
            this.C12Error = ErrorMatrixStiffness[2, 2];
            this.C13Error = ErrorMatrixStiffness[3, 3];
            this.C44Error = ErrorMatrixStiffness[4, 4];

        }

        private void SetErrorReussCubic()
        {
            //Compliance
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrixCompliance = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

            for (int n = 0; n < this.DiffractionConstants.Count; n++)
            {
                #region Matrix Build

                HessianMatrixCompliance[0, 0] += (this.FirstDerivativeS11S1ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[0, 0] += (this.FirstDerivativeS11HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrixCompliance[1, 1] += (this.FirstDerivativeS12S1ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[1, 1] += (this.FirstDerivativeS12HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[0, 1] += (this.FirstDerivativeS12S1ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[0, 1] += (this.FirstDerivativeS12HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[1, 0] += (this.FirstDerivativeS12S1ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[1, 0] += (this.FirstDerivativeS12HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrixCompliance[2, 2] += (this.FirstDerivativeS44S1ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS44S1ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[2, 2] += (this.FirstDerivativeS44HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS44HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[0, 2] += (this.FirstDerivativeS44S1ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[0, 2] += (this.FirstDerivativeS44HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[2, 0] += (this.FirstDerivativeS44S1ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[2, 0] += (this.FirstDerivativeS44HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[1, 2] += (this.FirstDerivativeS44S1ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[1, 2] += (this.FirstDerivativeS44HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[2, 1] += (this.FirstDerivativeS44S1ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[2, 1] += (this.FirstDerivativeS44HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                
                #endregion
            }

            MathNet.Numerics.LinearAlgebra.Matrix<double> CoVMatrixCompliance = HessianMatrixCompliance.Inverse();

            this.S11Error = Math.Sqrt(CoVMatrixCompliance[0, 0]);
            this.S12Error = Math.Sqrt(CoVMatrixCompliance[1, 1]);
            this.S44Error = Math.Sqrt(CoVMatrixCompliance[2, 2]);

            MathNet.Numerics.LinearAlgebra.Matrix<double> ErrorMatrixStiffness = this._complianceTensorError.Inverse();

            this.C11Error = ErrorMatrixStiffness[0, 0];
            this.C12Error = ErrorMatrixStiffness[1, 1];
            this.C44Error = ErrorMatrixStiffness[2, 2];

        }

        private void SetErrorReussHexagonal()
        {
            //Compliance
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrixCompliance = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(5, 5, 0.0);

            for (int n = 0; n < this.DiffractionConstants.Count; n++)
            {
                #region Matrix Build

                HessianMatrixCompliance[0, 0] += (this.FirstDerivativeS11S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[0, 0] += (this.FirstDerivativeS11HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrixCompliance[1, 1] += (this.FirstDerivativeS33S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[1, 1] += (this.FirstDerivativeS33HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[0, 1] += (this.FirstDerivativeS33S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[0, 1] += (this.FirstDerivativeS33HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[1, 0] += (this.FirstDerivativeS33S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[1, 0] += (this.FirstDerivativeS33HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrixCompliance[2, 2] += (this.FirstDerivativeS12S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[2, 2] += (this.FirstDerivativeS12HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[0, 2] += (this.FirstDerivativeS12S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[0, 2] += (this.FirstDerivativeS12HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[2, 0] += (this.FirstDerivativeS12S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[2, 0] += (this.FirstDerivativeS12HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[1, 2] += (this.FirstDerivativeS12S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[1, 2] += (this.FirstDerivativeS12HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[2, 1] += (this.FirstDerivativeS12S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[2, 1] += (this.FirstDerivativeS12HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrixCompliance[3, 3] += (this.FirstDerivativeS13S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[3, 3] += (this.FirstDerivativeS13HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[0, 3] += (this.FirstDerivativeS13S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[0, 3] += (this.FirstDerivativeS13HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[3, 0] += (this.FirstDerivativeS13S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[3, 0] += (this.FirstDerivativeS13HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[1, 3] += (this.FirstDerivativeS13S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[1, 3] += (this.FirstDerivativeS13HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[3, 1] += (this.FirstDerivativeS13S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[3, 1] += (this.FirstDerivativeS13HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[2, 3] += (this.FirstDerivativeS13S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[2, 3] += (this.FirstDerivativeS13HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[3, 2] += (this.FirstDerivativeS13S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[3, 2] += (this.FirstDerivativeS13HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrixCompliance[4, 4] += (this.FirstDerivativeS44S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS44S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[4, 4] += (this.FirstDerivativeS44HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS44HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[0, 4] += (this.FirstDerivativeS44S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[0, 4] += (this.FirstDerivativeS44HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[4, 0] += (this.FirstDerivativeS44S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[4, 0] += (this.FirstDerivativeS44HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[1, 4] += (this.FirstDerivativeS44S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[1, 4] += (this.FirstDerivativeS44HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[4, 1] += (this.FirstDerivativeS44S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[4, 1] += (this.FirstDerivativeS44HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[2, 4] += (this.FirstDerivativeS44S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[2, 4] += (this.FirstDerivativeS44HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[4, 2] += (this.FirstDerivativeS44S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[4, 2] += (this.FirstDerivativeS44HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[3, 4] += (this.FirstDerivativeS44S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[3, 4] += (this.FirstDerivativeS44HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrixCompliance[4, 3] += (this.FirstDerivativeS44S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrixCompliance[4, 3] += (this.FirstDerivativeS44HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                #endregion
            }

            MathNet.Numerics.LinearAlgebra.Matrix<double> CoVMatrixCompliance = HessianMatrixCompliance.Inverse();

            this.S11Error = Math.Sqrt(CoVMatrixCompliance[0, 0]);
            this.S33Error = Math.Sqrt(CoVMatrixCompliance[1, 1]);
            this.S12Error = Math.Sqrt(CoVMatrixCompliance[2, 2]);
            this.S13Error = Math.Sqrt(CoVMatrixCompliance[3, 3]);
            this.S44Error = Math.Sqrt(CoVMatrixCompliance[4, 4]);

            MathNet.Numerics.LinearAlgebra.Matrix<double> ErrorMatrixStiffness = this._complianceTensorError.Inverse();

            this.C11Error = ErrorMatrixStiffness[0, 0];
            this.C33Error = ErrorMatrixStiffness[1, 1];
            this.C12Error = ErrorMatrixStiffness[2, 2];
            this.C13Error = ErrorMatrixStiffness[3, 3];
            this.C44Error = ErrorMatrixStiffness[4, 4];

        }

        #endregion

        #endregion

        #region REK Calculation

        #region Voigt

        #region Cubic Isotrope

        public double S1VoigtCubicIsotrope()
        {
            double Ret = -5 * this.C12;

            double N = 3 * Math.Pow(this.C11, 2);
            N += this.C11 * this.C12;
            N -= 10 * Math.Pow(this.C12, 2);

            Ret /= N;

            return Ret;
        }

        public double HS2VoigtCubicIsotrope()
        {
            double Ret = 5 / 4;

            double Sum1 = this.C11 - this.C12;

            Ret /= Sum1;

            return Ret;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] C11
        ///[1] C12
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorVoigtCubicIsotropeClassic(double Lambda)
        {
            //[0][0] Aclivity
            //[1][1] Constant
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(2, 2, 0.0);

            //[0] Aclivity
            //[1] Constant
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(2);

            for (int n = 0; n < this.DiffractionConstants.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeC11S1VoigtCubicIsotrope() * this.FirstDerivativeC11S1VoigtCubicIsotrope() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 0] += (this.FirstDerivativeC11HS2VoigtCubicIsotrope() * this.FirstDerivativeC11HS2VoigtCubicIsotrope() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeC12S1VoigtCubicIsotrope() * this.FirstDerivativeC12S1VoigtCubicIsotrope() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 1] += (this.FirstDerivativeC12HS2VoigtCubicIsotrope() * this.FirstDerivativeC12HS2VoigtCubicIsotrope() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeC12S1VoigtCubicIsotrope() * this.FirstDerivativeC11S1VoigtCubicIsotrope() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeC12HS2VoigtCubicIsotrope() * this.FirstDerivativeC11HS2VoigtCubicIsotrope() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeC12S1VoigtCubicIsotrope() * this.FirstDerivativeC11S1VoigtCubicIsotrope() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeC12HS2VoigtCubicIsotrope() * this.FirstDerivativeC11HS2VoigtCubicIsotrope() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                #endregion

                #region Vector build

                SolutionVector[0] += ((this.DiffractionConstants[n].ClassicS1 - this.S1VoigtCubicIsotrope()) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeC11S1VoigtCubicIsotrope();
                SolutionVector[0] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2VoigtCubicIsotrope()) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeC11HS2VoigtCubicIsotrope();
                SolutionVector[1] += ((this.DiffractionConstants[n].ClassicS1 - this.S1VoigtCubicIsotrope()) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeC12S1VoigtCubicIsotrope();
                SolutionVector[1] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2VoigtCubicIsotrope()) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeC12HS2VoigtCubicIsotrope();

                #endregion
            }

            for (int n = 0; n < 2; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(2);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] C11
        ///[1] C12
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorVoigtCubicIsotropeMacroscopic(double Lambda)
        {
            //[0][0] Aclivity
            //[1][1] Constant
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(2, 2, 0.0);

            //[0] Aclivity
            //[1] Constant
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(2);

            for (int n = 0; n < this.DiffractionConstants.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeC11S1VoigtCubicIsotrope() * this.FirstDerivativeC11S1VoigtCubicIsotrope() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 0] += (this.FirstDerivativeC11HS2VoigtCubicIsotrope() * this.FirstDerivativeC11HS2VoigtCubicIsotrope() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeC12S1VoigtCubicIsotrope() * this.FirstDerivativeC12S1VoigtCubicIsotrope() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 1] += (this.FirstDerivativeC12HS2VoigtCubicIsotrope() * this.FirstDerivativeC12HS2VoigtCubicIsotrope() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeC12S1VoigtCubicIsotrope() * this.FirstDerivativeC11S1VoigtCubicIsotrope() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeC12HS2VoigtCubicIsotrope() * this.FirstDerivativeC11HS2VoigtCubicIsotrope() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeC12S1VoigtCubicIsotrope() * this.FirstDerivativeC11S1VoigtCubicIsotrope() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeC12HS2VoigtCubicIsotrope() * this.FirstDerivativeC11HS2VoigtCubicIsotrope() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                #endregion

                #region Vector build

                SolutionVector[0] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1VoigtCubicIsotrope()) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeC11S1VoigtCubicIsotrope();
                SolutionVector[0] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2VoigtCubicIsotrope()) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeC11HS2VoigtCubicIsotrope();
                SolutionVector[1] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1VoigtCubicIsotrope()) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeC12S1VoigtCubicIsotrope();
                SolutionVector[1] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2VoigtCubicIsotrope()) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeC12HS2VoigtCubicIsotrope();

                #endregion
            }

            for (int n = 0; n < 2; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(2);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        #region First derivative

        public double FirstDerivativeC11S1VoigtCubicIsotrope()
        {
            double Ret = 5 * this.C12;
            Ret *= (6 * this.C11) + this.C12;

            double Sum = 3 * Math.Pow(this.C12, 2);
            Sum -= 10 * Math.Pow(this.C12, 2);
            Sum += this.C12 * this.C11;

            Ret /= Math.Pow(Sum, 2);

            return Ret;
        }

        public double FirstDerivativeC12S1VoigtCubicIsotrope()
        {
            double Ret = (3 * Math.Pow(this.C11, 2)) + (10 * Math.Pow(this.C12, 2));
            Ret *= -5;

            double Sum = 3 * Math.Pow(this.C12, 2);
            Sum -= 10 * Math.Pow(this.C12, 2);
            Sum += this.C12 * this.C11;

            Ret /= Math.Pow(Sum, 2);

            return Ret;
        }

        public double FirstDerivativeC11HS2VoigtCubicIsotrope()
        {
            double Ret = -5;

            double Sum = 4 * Math.Pow(this.C11 - this.C12, 2);

            Ret /= Sum;

            return Ret;
        }

        public double FirstDerivativeC12HS2VoigtCubicIsotrope()
        {
            double Ret = 5;

            double Sum = 4 * Math.Pow(this.C11 - this.C12, 2);

            Ret /= Sum;

            return Ret;
        }

        #endregion

        #endregion

        #region Cubic
        
        #region Stiffnes components

        public double S1VoigtCubic()
        {
            double Ret = this.C11;
            Ret += 4 * this.C12;
            Ret -= 2 * this.C44;
            Ret *= -0.5;

            double N = this.C11 - this.C12;
            N += 3 * this.C44;

            N *= ((2 * this.C12) + this.C11);

            Ret /= N;

            return Ret;
        }

        public double HS2VoigtCubic()
        {
            double Ret = 5 / 2;

            double Sum = this.C11 - this.C12;
            Sum += 3 * this.C44;

            Ret /= Sum;

            return Ret;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] C11
        ///[1] C12
        ///[2] C44
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorVoigtCubicClassic(double Lambda)
        {
            //[0][0] C11
            //[1][1] C12
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

            //[0] Aclivity
            //[1] Constant
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);

            for (int n = 0; n < this.DiffractionConstants.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeC11S1VoigtCubic() * this.FirstDerivativeC11S1VoigtCubic() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 0] += (this.FirstDerivativeC11HS2VoigtCubic() * this.FirstDerivativeC11HS2VoigtCubic() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeC12S1VoigtCubic() * this.FirstDerivativeC12S1VoigtCubic() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 1] += (this.FirstDerivativeC12HS2VoigtCubic() * this.FirstDerivativeC12HS2VoigtCubic() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeC12S1VoigtCubic() * this.FirstDerivativeC11S1VoigtCubic() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeC12HS2VoigtCubic() * this.FirstDerivativeC11HS2VoigtCubic() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeC12S1VoigtCubic() * this.FirstDerivativeC11S1VoigtCubic() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeC12HS2VoigtCubic() * this.FirstDerivativeC11HS2VoigtCubic() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeC44S1VoigtCubic() * this.FirstDerivativeC44S1VoigtCubic() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 2] += (this.FirstDerivativeC44HS2VoigtCubic() * this.FirstDerivativeC44HS2VoigtCubic() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeC44S1VoigtCubic() * this.FirstDerivativeC11S1VoigtCubic() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeC44HS2VoigtCubic() * this.FirstDerivativeC11HS2VoigtCubic() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeC44S1VoigtCubic() * this.FirstDerivativeC11S1VoigtCubic() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeC44HS2VoigtCubic() * this.FirstDerivativeC11HS2VoigtCubic() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeC44S1VoigtCubic() * this.FirstDerivativeC12S1VoigtCubic() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeC44HS2VoigtCubic() * this.FirstDerivativeC12HS2VoigtCubic() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeC44S1VoigtCubic() * this.FirstDerivativeC12S1VoigtCubic() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeC44HS2VoigtCubic() * this.FirstDerivativeC12HS2VoigtCubic() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                #endregion

                #region Vector build

                SolutionVector[0] += ((this.DiffractionConstants[n].ClassicS1 - this.S1VoigtCubic()) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeC11S1VoigtCubic();
                SolutionVector[0] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2VoigtCubic()) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeC11HS2VoigtCubic();
                SolutionVector[1] += ((this.DiffractionConstants[n].ClassicS1 - this.S1VoigtCubic()) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeC12S1VoigtCubic();
                SolutionVector[1] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2VoigtCubic()) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeC12HS2VoigtCubic();
                SolutionVector[2] += ((this.DiffractionConstants[n].ClassicS1 - this.S1VoigtCubic()) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeC44S1VoigtCubic();
                SolutionVector[2] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2VoigtCubic()) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeC44HS2VoigtCubic();

                #endregion
            }

            for (int n = 0; n < 3; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] C11
        ///[1] C12
        ///[2] C44
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorVoigtCubicMacroscopic(double Lambda)
        {
            //[0][0] C11
            //[1][1] C12
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

            //[0] Aclivity
            //[1] Constant
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);

            for (int n = 0; n < this.DiffractionConstants.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeC11S1VoigtCubic() * this.FirstDerivativeC11S1VoigtCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 0] += (this.FirstDerivativeC11HS2VoigtCubic() * this.FirstDerivativeC11HS2VoigtCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeC12S1VoigtCubic() * this.FirstDerivativeC12S1VoigtCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 1] += (this.FirstDerivativeC12HS2VoigtCubic() * this.FirstDerivativeC12HS2VoigtCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeC12S1VoigtCubic() * this.FirstDerivativeC11S1VoigtCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeC12HS2VoigtCubic() * this.FirstDerivativeC11HS2VoigtCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeC12S1VoigtCubic() * this.FirstDerivativeC11S1VoigtCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeC12HS2VoigtCubic() * this.FirstDerivativeC11HS2VoigtCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeC44S1VoigtCubic() * this.FirstDerivativeC44S1VoigtCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 2] += (this.FirstDerivativeC44HS2VoigtCubic() * this.FirstDerivativeC44HS2VoigtCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeC44S1VoigtCubic() * this.FirstDerivativeC11S1VoigtCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeC44HS2VoigtCubic() * this.FirstDerivativeC11HS2VoigtCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeC44S1VoigtCubic() * this.FirstDerivativeC11S1VoigtCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeC44HS2VoigtCubic() * this.FirstDerivativeC11HS2VoigtCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeC44S1VoigtCubic() * this.FirstDerivativeC12S1VoigtCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeC44HS2VoigtCubic() * this.FirstDerivativeC12HS2VoigtCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeC44S1VoigtCubic() * this.FirstDerivativeC12S1VoigtCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeC44HS2VoigtCubic() * this.FirstDerivativeC12HS2VoigtCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                #endregion

                #region Vector build

                SolutionVector[0] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1VoigtCubic()) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeC11S1VoigtCubic();
                SolutionVector[0] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2VoigtCubic()) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeC11HS2VoigtCubic();
                SolutionVector[1] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1VoigtCubic()) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeC12S1VoigtCubic();
                SolutionVector[1] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2VoigtCubic()) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeC12HS2VoigtCubic();
                SolutionVector[2] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1VoigtCubic()) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeC44S1VoigtCubic();
                SolutionVector[2] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2VoigtCubic()) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeC44HS2VoigtCubic();

                #endregion
            }

            for (int n = 0; n < 3; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        #region Parameter setting

        /// <summary>
        /// Set C11, C12, C44
        /// The deltas should be 
        /// [0] C11
        /// [1] C12
        /// [2] C44
        /// </summary>
        /// <param name="DeltaValues"></param>
        /// <returns></returns>
        public bool SetCubicVoigtParameters(double[] DeltaValues)
        {
            bool ret = true;

            double C11New = this.C11 + DeltaValues[0];
            double C12New = this.C12 + DeltaValues[1];
            double C44New = this.C44 + DeltaValues[2];

            return ret;
        }

        #endregion
        

        #region First derivative

        public double FirstDerivativeC11S1VoigtCubic()
        {
            double Ret = Math.Pow(this.C11, 2);
            Ret += 6 * Math.Pow(this.C12, 2);
            Ret -= 6 * Math.Pow(this.C44, 2);
            Ret += 8 * this.C11 * this.C12;
            Ret -= 4 * this.C11 * this.C44;
            Ret += 4 * this.C12 * this.C44;

            double N = Math.Pow(this.C11 + (2 * this.C12), 2) * Math.Pow(this.C11 - this.C12 + (3 * this.C44), 2);

            Ret /= 2 * N;

            return Ret;
        }

        public double FirstDerivativeC12S1VoigtCubic()
        {
            double Ret = 3.0 * Math.Pow(this.C11, 2);
            Ret += 8 * Math.Pow(this.C12, 2);
            Ret += 12 * Math.Pow(this.C44, 2);
            Ret += 4 * this.C11 * this.C12;
            Ret += 8 * this.C11 * this.C44;
            Ret -= 8 * this.C12 * this.C44;

            double N = Math.Pow(this.C11 + (2 * this.C12), 2) * Math.Pow(this.C11 - this.C12 + (3 * this.C44), 2);

            Ret /= -2 * N;

            return Ret;
        }

        public double FirstDerivativeC44S1VoigtCubic()
        {
            double Ret = 5 / 2;
            Ret /= Math.Pow(this.C11 - this.C12 + (3.0 * this.C44), 2);

            return Ret;
        }

        public double FirstDerivativeC11HS2VoigtCubic()
        {
            double Ret = -5;

            double Sum = this.C11 - this.C12 + (3 * this.C44);

            Ret /= 2 * Math.Pow(Sum, 2);

            return Ret;
        }

        public double FirstDerivativeC12HS2VoigtCubic()
        {
            double Ret = 5;

            double Sum = this.C11 - this.C12 + (3 * this.C44);

            Ret /= 2 * Math.Pow(Sum, 2);

            return Ret;
        }

        public double FirstDerivativeC44HS2VoigtCubic()
        {
            double Ret = -15;

            double Sum = this.C11 - this.C12 + (3 * this.C44);

            Ret /= 2 * Math.Pow(Sum, 2);

            return Ret;
        }

        #endregion

        #endregion

        #region Compliance components

        public double S1VoigtCubicCompliance()
        {
            double Ret = this.S11 / 5.0;
            Ret += (4.0 / 5.0) * this.S12;
            Ret -= this.S44 / 10.0;

            return Ret;
        }

        public double HS2VoigtCubicCompliance()
        {
            double Ret = this.S11 * (2.0 / 5.0);
            Ret -= (2.0 / 5.0) * this.S12;
            Ret += this.S44 * (3.0 / 10.0);

            return Ret;
        }

        #region First derivative

        public double FirstDerivativeS11S1VoigtCubic()
        {
            double Ret = 1.0 / 5.0;

            return Ret;
        }

        public double FirstDerivativeS12S1VoigtCubic()
        {
            double Ret = (4.0 / 5.0);

            return Ret;
        }

        public double FirstDerivativeS44S1VoigtCubic()
        {
            double Ret = -1.0 / 10.0;

            return Ret;
        }

        public double FirstDerivativeS11HS2VoigtCubic()
        {
            double Ret = (2.0 / 5.0);

            return Ret;
        }

        public double FirstDerivativeS12HS2VoigtCubic()
        {
            double Ret = -2.0 / 5.0;

            return Ret;
        }

        public double FirstDerivativeS44HS2VoigtCubic()
        {
            double Ret = (3.0 / 10.0);

            return Ret;
        }

        #endregion

        #endregion

        #endregion

        #region Type1 (Hexagonal, Trigonal 1 and 2)

        #region Stiffnes components

        public double S1VoigtType1()
        {
            double Ret = this.C11 + this.C33;
            Ret += 5 * this.C12;
            Ret += 8 * this.C13;
            Ret -= 4 * this.C44;
            Ret *= -3 / 2;

            double Sum = (7 / 2) * this.C11;
            Sum -= (5 / 2) * this.C12;
            Sum -= 2 * this.C13;
            Sum += this.C33;
            Sum += 6 * this.C44;

            double Sum1 = 2 * this.C11;
            Sum1 += this.C33;
            Sum1 += 2 * this.C12;
            Sum1 += 4 * this.C13;

            Sum *= Sum1;

            Ret /= Sum;

            return Ret;
        }

        public double HS2VoigtType1()
        {
            double Ret = 15;

            double Sum = 7 * this.C11;
            Sum += 2 * this.C33;
            Sum -= 5 * this.C12;
            Sum -= 4 * this.C13;
            Sum += 12 * this.C44;

            Ret /= Sum;

            return Ret;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] C11
        ///[1] C33
        ///[2] C12
        ///[3] C13
        ///[4] C44
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorVoigtType1Classic(double Lambda)
        {
            //[0][0] C11
            //[1][1] C12
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(5, 5, 0.0);

            //[0] Aclivity
            //[1] Constant
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(5);

            for (int n = 0; n < this.DiffractionConstants.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeC11S1VoigtType1() * this.FirstDerivativeC11S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 0] += (this.FirstDerivativeC11HS2VoigtType1() * this.FirstDerivativeC11HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeC33S1VoigtType1() * this.FirstDerivativeC33S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 1] += (this.FirstDerivativeC33HS2VoigtType1() * this.FirstDerivativeC33HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeC33S1VoigtType1() * this.FirstDerivativeC11S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeC33HS2VoigtType1() * this.FirstDerivativeC11HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeC33S1VoigtType1() * this.FirstDerivativeC11S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeC33HS2VoigtType1() * this.FirstDerivativeC11HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeC12S1VoigtType1() * this.FirstDerivativeC12S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 2] += (this.FirstDerivativeC12HS2VoigtType1() * this.FirstDerivativeC12HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeC12S1VoigtType1() * this.FirstDerivativeC11S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeC12HS2VoigtType1() * this.FirstDerivativeC11HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeC12S1VoigtType1() * this.FirstDerivativeC11S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeC12HS2VoigtType1() * this.FirstDerivativeC11HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeC12S1VoigtType1() * this.FirstDerivativeC33S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeC12HS2VoigtType1() * this.FirstDerivativeC33HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeC12S1VoigtType1() * this.FirstDerivativeC33S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeC12HS2VoigtType1() * this.FirstDerivativeC33HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[3, 3] += (this.FirstDerivativeC13S1VoigtType1() * this.FirstDerivativeC13S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 3] += (this.FirstDerivativeC13HS2VoigtType1() * this.FirstDerivativeC13HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 3] += (this.FirstDerivativeC13S1VoigtType1() * this.FirstDerivativeC11S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 3] += (this.FirstDerivativeC13HS2VoigtType1() * this.FirstDerivativeC11HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[3, 0] += (this.FirstDerivativeC13S1VoigtType1() * this.FirstDerivativeC11S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 0] += (this.FirstDerivativeC13HS2VoigtType1() * this.FirstDerivativeC11HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 3] += (this.FirstDerivativeC13S1VoigtType1() * this.FirstDerivativeC33S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 3] += (this.FirstDerivativeC13HS2VoigtType1() * this.FirstDerivativeC33HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[3, 1] += (this.FirstDerivativeC13S1VoigtType1() * this.FirstDerivativeC33S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 1] += (this.FirstDerivativeC13HS2VoigtType1() * this.FirstDerivativeC33HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 3] += (this.FirstDerivativeC13S1VoigtType1() * this.FirstDerivativeC12S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 3] += (this.FirstDerivativeC13HS2VoigtType1() * this.FirstDerivativeC12HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[3, 2] += (this.FirstDerivativeC13S1VoigtType1() * this.FirstDerivativeC12S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 2] += (this.FirstDerivativeC13HS2VoigtType1() * this.FirstDerivativeC12HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[4, 4] += (this.FirstDerivativeC44S1VoigtType1() * this.FirstDerivativeC44S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 4] += (this.FirstDerivativeC44HS2VoigtType1() * this.FirstDerivativeC44HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 4] += (this.FirstDerivativeC44S1VoigtType1() * this.FirstDerivativeC11S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 4] += (this.FirstDerivativeC44HS2VoigtType1() * this.FirstDerivativeC11HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[4, 0] += (this.FirstDerivativeC44S1VoigtType1() * this.FirstDerivativeC11S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 0] += (this.FirstDerivativeC44HS2VoigtType1() * this.FirstDerivativeC11HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 4] += (this.FirstDerivativeC44S1VoigtType1() * this.FirstDerivativeC33S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 4] += (this.FirstDerivativeC44HS2VoigtType1() * this.FirstDerivativeC33HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[4, 1] += (this.FirstDerivativeC44S1VoigtType1() * this.FirstDerivativeC33S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 1] += (this.FirstDerivativeC44HS2VoigtType1() * this.FirstDerivativeC33HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 4] += (this.FirstDerivativeC44S1VoigtType1() * this.FirstDerivativeC12S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 4] += (this.FirstDerivativeC44HS2VoigtType1() * this.FirstDerivativeC12HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[4, 2] += (this.FirstDerivativeC44S1VoigtType1() * this.FirstDerivativeC12S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 2] += (this.FirstDerivativeC44HS2VoigtType1() * this.FirstDerivativeC12HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[3, 4] += (this.FirstDerivativeC44S1VoigtType1() * this.FirstDerivativeC13S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 4] += (this.FirstDerivativeC44HS2VoigtType1() * this.FirstDerivativeC13HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[4, 3] += (this.FirstDerivativeC44S1VoigtType1() * this.FirstDerivativeC13S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 3] += (this.FirstDerivativeC44HS2VoigtType1() * this.FirstDerivativeC13HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                #endregion

                #region Vector build

                SolutionVector[0] += ((this.DiffractionConstants[n].ClassicS1 - this.S1VoigtType1()) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeC11S1VoigtType1();
                SolutionVector[0] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2VoigtType1()) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeC11HS2VoigtType1();
                SolutionVector[1] += ((this.DiffractionConstants[n].ClassicS1 - this.S1VoigtType1()) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeC33S1VoigtType1();
                SolutionVector[1] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2VoigtType1()) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeC33HS2VoigtType1();
                SolutionVector[2] += ((this.DiffractionConstants[n].ClassicS1 - this.S1VoigtType1()) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeC12S1VoigtType1();
                SolutionVector[2] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2VoigtType1()) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeC12HS2VoigtType1();
                SolutionVector[3] += ((this.DiffractionConstants[n].ClassicS1 - this.S1VoigtType1()) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeC13S1VoigtType1();
                SolutionVector[3] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2VoigtType1()) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeC13HS2VoigtType1();
                SolutionVector[4] += ((this.DiffractionConstants[n].ClassicS1 - this.S1VoigtType1()) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeC44S1VoigtType1();
                SolutionVector[4] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2VoigtType1()) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeC44HS2VoigtType1();

                #endregion
            }

            for (int n = 0; n < 5; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(5);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] C11
        ///[1] C33
        ///[2] C12
        ///[3] C13
        ///[4] C44
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorVoigtType1Macroscopic(double Lambda)
        {
            //[0][0] C11
            //[1][1] C12
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(5, 5, 0.0);

            //[0] Aclivity
            //[1] Constant
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(5);

            for (int n = 0; n < this.DiffractionConstants.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeC11S1VoigtType1() * this.FirstDerivativeC11S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 0] += (this.FirstDerivativeC11HS2VoigtType1() * this.FirstDerivativeC11HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeC33S1VoigtType1() * this.FirstDerivativeC33S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 1] += (this.FirstDerivativeC33HS2VoigtType1() * this.FirstDerivativeC33HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeC33S1VoigtType1() * this.FirstDerivativeC11S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeC33HS2VoigtType1() * this.FirstDerivativeC11HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeC33S1VoigtType1() * this.FirstDerivativeC11S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeC33HS2VoigtType1() * this.FirstDerivativeC11HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeC12S1VoigtType1() * this.FirstDerivativeC12S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 2] += (this.FirstDerivativeC12HS2VoigtType1() * this.FirstDerivativeC12HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeC12S1VoigtType1() * this.FirstDerivativeC11S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeC12HS2VoigtType1() * this.FirstDerivativeC11HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeC12S1VoigtType1() * this.FirstDerivativeC11S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeC12HS2VoigtType1() * this.FirstDerivativeC11HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeC12S1VoigtType1() * this.FirstDerivativeC33S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeC12HS2VoigtType1() * this.FirstDerivativeC33HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeC12S1VoigtType1() * this.FirstDerivativeC33S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeC12HS2VoigtType1() * this.FirstDerivativeC33HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[3, 3] += (this.FirstDerivativeC13S1VoigtType1() * this.FirstDerivativeC13S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 3] += (this.FirstDerivativeC13HS2VoigtType1() * this.FirstDerivativeC13HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 3] += (this.FirstDerivativeC13S1VoigtType1() * this.FirstDerivativeC11S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 3] += (this.FirstDerivativeC13HS2VoigtType1() * this.FirstDerivativeC11HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[3, 0] += (this.FirstDerivativeC13S1VoigtType1() * this.FirstDerivativeC11S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 0] += (this.FirstDerivativeC13HS2VoigtType1() * this.FirstDerivativeC11HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 3] += (this.FirstDerivativeC13S1VoigtType1() * this.FirstDerivativeC33S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 3] += (this.FirstDerivativeC13HS2VoigtType1() * this.FirstDerivativeC33HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[3, 1] += (this.FirstDerivativeC13S1VoigtType1() * this.FirstDerivativeC33S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 1] += (this.FirstDerivativeC13HS2VoigtType1() * this.FirstDerivativeC33HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 3] += (this.FirstDerivativeC13S1VoigtType1() * this.FirstDerivativeC12S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 3] += (this.FirstDerivativeC13HS2VoigtType1() * this.FirstDerivativeC12HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[3, 2] += (this.FirstDerivativeC13S1VoigtType1() * this.FirstDerivativeC12S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 2] += (this.FirstDerivativeC13HS2VoigtType1() * this.FirstDerivativeC12HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[4, 4] += (this.FirstDerivativeC44S1VoigtType1() * this.FirstDerivativeC44S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 4] += (this.FirstDerivativeC44HS2VoigtType1() * this.FirstDerivativeC44HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 4] += (this.FirstDerivativeC44S1VoigtType1() * this.FirstDerivativeC11S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 4] += (this.FirstDerivativeC44HS2VoigtType1() * this.FirstDerivativeC11HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[4, 0] += (this.FirstDerivativeC44S1VoigtType1() * this.FirstDerivativeC11S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 0] += (this.FirstDerivativeC44HS2VoigtType1() * this.FirstDerivativeC11HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 4] += (this.FirstDerivativeC44S1VoigtType1() * this.FirstDerivativeC33S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 4] += (this.FirstDerivativeC44HS2VoigtType1() * this.FirstDerivativeC33HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[4, 1] += (this.FirstDerivativeC44S1VoigtType1() * this.FirstDerivativeC33S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 1] += (this.FirstDerivativeC44HS2VoigtType1() * this.FirstDerivativeC33HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 4] += (this.FirstDerivativeC44S1VoigtType1() * this.FirstDerivativeC12S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 4] += (this.FirstDerivativeC44HS2VoigtType1() * this.FirstDerivativeC12HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[4, 2] += (this.FirstDerivativeC44S1VoigtType1() * this.FirstDerivativeC12S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 2] += (this.FirstDerivativeC44HS2VoigtType1() * this.FirstDerivativeC12HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[3, 4] += (this.FirstDerivativeC44S1VoigtType1() * this.FirstDerivativeC13S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 4] += (this.FirstDerivativeC44HS2VoigtType1() * this.FirstDerivativeC13HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[4, 3] += (this.FirstDerivativeC44S1VoigtType1() * this.FirstDerivativeC13S1VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 3] += (this.FirstDerivativeC44HS2VoigtType1() * this.FirstDerivativeC13HS2VoigtType1() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                #endregion

                #region Vector build

                SolutionVector[0] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1VoigtType1()) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeC11S1VoigtType1();
                SolutionVector[0] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2VoigtType1()) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeC11HS2VoigtType1();
                SolutionVector[1] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1VoigtType1()) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeC33S1VoigtType1();
                SolutionVector[1] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2VoigtType1()) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeC33HS2VoigtType1();
                SolutionVector[2] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1VoigtType1()) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeC12S1VoigtType1();
                SolutionVector[2] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2VoigtType1()) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeC12HS2VoigtType1();
                SolutionVector[3] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1VoigtType1()) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeC13S1VoigtType1();
                SolutionVector[3] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2VoigtType1()) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeC13HS2VoigtType1();
                SolutionVector[4] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1VoigtType1()) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeC44S1VoigtType1();
                SolutionVector[4] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2VoigtType1()) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeC44HS2VoigtType1();

                #endregion
            }

            for (int n = 0; n < 5; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(5);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        #region First derivative

        public double FirstDerivativeC11S1VoigtType1()
        {
            double Ret = 14 * Math.Pow(this.C11, 2);
            Ret += 30 * Math.Pow(this.C12, 2);
            Ret += 176 * Math.Pow(this.C13, 2);
            Ret += 9 * Math.Pow(this.C33, 2);
            Ret -= 30 * Math.Pow(this.C44, 2);
            Ret += 104 * this.C13 * this.C33;
            Ret += 64 * this.C13 * this.C44;
            Ret += 32 * this.C33 * this.C44;

            double Sum = (5 * this.C12) + (8 * this.C13) + this.C33 - (4 * this.C44);

            Ret += 28 * this.C11 * Sum;

            Sum = (8 * this.C13) + (3 * this.C33) + (4 * this.C44);

            Ret += 20 * this.C12 * Sum;
            Ret *= 3;

            double N = 4 * Math.Pow((2 * this.C11) + (2 * this.C12) + (4 * this.C13) + this.C33, 2);

            Sum = (7 * this.C11) - (5 * this.C12);
            Sum -= (4 * this.C13);
            Sum += 2 * this.C33;
            Sum += 12 * this.C44;

            N *= Math.Pow(Sum, 2);

            Ret /= N;

            return Ret;
        }

        public double FirstDerivativeC33S1VoigtType1()
        {
            double Ret = -3 * Math.Pow(this.C11, 2);
            Ret += 5 * Math.Pow(this.C12, 2);
            Ret += 48 * Math.Pow(this.C13, 2);
            Ret += 2 * Math.Pow(this.C33, 2);
            Ret -= 48 * Math.Pow(this.C44, 2);
            Ret += 32 * this.C13 * this.C33;
            Ret += 32 * this.C13 * this.C44;
            Ret -= 16 * this.C33 * this.C44;

            double Sum = (50 * this.C12) + (72 * this.C13) + (4 * this.C33) - (56 * this.C44);

            Ret += this.C11 * Sum;

            Sum = (2 * this.C13) + this.C33 + (2 * this.C44);

            Ret += 20 * this.C12 * Sum;
            Ret *= 3;

            double N = 4 * Math.Pow((2 * this.C11) + (2 * this.C12) + (4 * this.C13) + this.C33, 2);

            Sum = (7 * this.C11) - (5 * this.C12);
            Sum -= (4 * this.C13);
            Sum += 2 * this.C33;
            Sum += 12 * this.C44;

            N *= Math.Pow(Sum, 2);

            Ret /= N;

            return Ret;
        }

        public double FirstDerivativeC12S1VoigtType1()
        {
            double Ret = 66 * Math.Pow(this.C11, 2);
            Ret += 50 * Math.Pow(this.C12, 2);
            Ret += 144 * Math.Pow(this.C13, 2);
            Ret += 11 * Math.Pow(this.C33, 2);
            Ret += 96 * Math.Pow(this.C44, 2);
            Ret += 56 * this.C13 * this.C33;
            Ret -= 64 * this.C13 * this.C44;
            Ret += 32 * this.C33 * this.C44;

            double Sum = (5 * this.C12) + (24 * this.C13) + (13 * this.C33) + (28 * this.C44);

            Ret += 4 * this.C11 * Sum;

            Sum = (8 * this.C13) + this.C33 - (4 * this.C44);

            Ret += 20 * this.C12 * Sum;
            Ret *= -3;

            double N = 4 * Math.Pow((2 * this.C11) + (2 * this.C12) + (4 * this.C13) + this.C33, 2);

            Sum = (7 * this.C11) - (5 * this.C12);
            Sum -= (4 * this.C13);
            Sum += 2 * this.C33;
            Sum += 12 * this.C44;

            N *= Math.Pow(Sum, 2);

            Ret /= N;

            return Ret;
        }

        public double FirstDerivativeC13S1VoigtType1()
        {
            double Ret = 23 * Math.Pow(this.C11, 2);
            Ret += 15 * Math.Pow(this.C12, 2);
            Ret += 32 * Math.Pow(this.C13, 2);
            Ret += 3 * Math.Pow(this.C33, 2);
            Ret += 48 * Math.Pow(this.C44, 2);
            Ret += 8 * this.C13 * this.C33;
            Ret -= 32 * this.C13 * this.C44;
            Ret += 16 * this.C33 * this.C44;

            double Sum = (-10 * this.C12) + (8 * this.C13) + (16 * this.C33) + (56 * this.C44);

            Ret += this.C11 * Sum;

            Sum = this.C13 + this.C33 - this.C44;

            Ret += 40 * this.C12 * Sum;
            Ret *= -3;

            double N = Math.Pow((2 * this.C11) + (2 * this.C12) + (4 * this.C13) + this.C33, 2);

            Sum = (7 * this.C11) - (5 * this.C12);
            Sum -= (4 * this.C13);
            Sum += 2 * this.C33;
            Sum += 12 * this.C44;

            N *= Math.Pow(Sum, 2);

            Ret /= N;

            return Ret;
        }

        public double FirstDerivativeC44S1VoigtType1()
        {
            double Ret = 15;

            double N = (7 * this.C11) - (5 * this.C12) - (4 * this.C13) + (2 * this.C33) + (12 * this.C44);

            Ret /= Math.Pow(N, 2);

            return Ret;
        }

        public double FirstDerivativeC11HS2VoigtType1()
        {
            double Ret = -105;

            double Sum = 7 * this.C11;
            Sum -= 5 * this.C12;
            Sum -= 4 * this.C13;
            Sum += 2 * this.C33;
            Sum += 12 * this.C44;

            Ret /= Math.Pow(Sum, 2);

            return Ret;
        }

        public double FirstDerivativeC33HS2VoigtType1()
        {
            double Ret = 30;

            double Sum = 7 * this.C11;
            Sum -= 5 * this.C12;
            Sum -= 4 * this.C13;
            Sum += 2 * this.C33;
            Sum += 12 * this.C44;

            Ret /= Math.Pow(Sum, 2);

            return Ret;
        }

        public double FirstDerivativeC12HS2VoigtType1()
        {
            double Ret = 75;

            double Sum = 7 * this.C11;
            Sum -= 5 * this.C12;
            Sum -= 4 * this.C13;
            Sum += 2 * this.C33;
            Sum += 12 * this.C44;

            Ret /= Math.Pow(Sum, 2);

            return Ret;
        }

        public double FirstDerivativeC13HS2VoigtType1()
        {
            double Ret = 60;

            double Sum = 7 * this.C11;
            Sum -= 5 * this.C12;
            Sum -= 4 * this.C13;
            Sum += 2 * this.C33;
            Sum += 12 * this.C44;

            Ret /= Math.Pow(Sum, 2);

            return Ret;
        }

        public double FirstDerivativeC44HS2VoigtType1()
        {
            double Ret = -180;

            double Sum = 7 * this.C11;
            Sum -= 5 * this.C12;
            Sum -= 4 * this.C13;
            Sum += 2 * this.C33;
            Sum += 12 * this.C44;

            Ret /= Math.Pow(Sum, 2);

            return Ret;
        }

        #endregion

        #endregion

        #region Compliance components

        public double S1VoigtType1Compliance()
        {
            double Ret = (2.0 * this.S11) + this.S33;
            Ret /= 15.0;

            double Sum = this.S12 + (2.0 * this.S13);
            Sum *= 4.0 / 15.0; 

            double Sum1 = this.S44 + this.S11 - this.S12;
            Sum1 *= 1.0 / 15.0;

            Ret += Sum - Sum1;

            return Ret;
        }

        public double HS2VoigtType1Compliance()
        {
            double Ret = (2.0 * this.S11) + this.S33;
            Ret *= 2.0 / 15.0;

            double Sum = this.S12 + (2.0 * this.S13);
            Sum *= -2.0 / 15.0;

            double Sum1 = this.S44 + this.S11 - this.S12;
            Sum1 *= 3.0 / 15.0;

            Ret += Sum + Sum1;

            return Ret;
        }

        #region First derivative

        public double FirstDerivativeS11S1VoigtType1()
        {
            double Ret = 2.0;
            Ret /= 15.0;

            double Sum1 = 1.0;
            Sum1 /= 15.0;

            Ret -= Sum1;

            return Ret;
        }

        public double FirstDerivativeS33S1VoigtType1()
        {
            double Ret = 1.0;
            Ret /= 15.0;

            return Ret;
        }

        public double FirstDerivativeS12S1VoigtType1()
        {
            double Ret = 1.0;
            Ret *= 4.0 / 15.0;

            double Sum1 = 1.0;
            Sum1 /= 15.0;

            Ret += Sum1;

            return Ret;
        }

        public double FirstDerivativeS13S1VoigtType1()
        {
            double Ret = 8.0 / 15.0;

            return Ret;
        }

        public double FirstDerivativeS44S1VoigtType1()
        {
            double Ret = -1.0;
            Ret /= 15.0;

            return Ret;
        }

        public double FirstDerivativeS11HS2VoigtType1()
        {
            double Ret = 2.0;
            Ret *= 2.0 / 15.0;

            double Sum1 = 1.0;
            Sum1 *= 3.0 / 15.0;

            Ret += Sum1;

            return Ret;
        }

        public double FirstDerivativeS33HS2VoigtType1()
        {
            double Ret = 1.0;
            Ret *= 2.0 / 15.0;

            return Ret;
        }

        public double FirstDerivativeS12HS2VoigtType1()
        {
            double Ret = 1.0;
            Ret *= -2.0 / 15.0;

            double Sum1 = -1.0;
            Sum1 *= 3.0 / 15.0;

            Ret += Sum1;

            return Ret;
        }

        public double FirstDerivativeS13HS2VoigtType1()
        {
            double Ret = 2.0;
            Ret *= -2.0 / 15.0;

            return Ret;
        }

        public double FirstDerivativeS44HS2VoigtType1()
        {
            double Ret = 1.0;
            Ret *= 3.0 / 15.0;

            return Ret;
        }

        #endregion

        #endregion

        #endregion

        #region Type 2 (Tetragonal 1 and 2)

        public double S1VoigtType2()
        {
            double Ret = 2 * this.C11;
            Ret += this.C33;
            Ret += 4 * this.C12;
            Ret += 8 * this.C13;
            Ret -= 4 * this.C44;
            Ret -= 2 * this.C66;
            Ret *= -3 / 2;

            double Sum = 2 * this.C11;
            Sum += this.C33 - this.C12;
            Sum -= 2 * this.C13;
            Sum += 6 * this.C44;
            Sum += 3 * this.C66;

            double Sum1 = 2 * this.C11;
            Sum1 += this.C33;
            Sum1 += 2 * this.C12;
            Sum1 += 4 * this.C13;

            Sum *= Sum1;

            Ret /= Sum;

            return Ret;
        }

        public double HS2VoigtType2()
        {
            double Sum = 4 * this.C11;
            Sum += 2 * (this.C33 - this.C12);
            Sum -= 4 * this.C13;
            Sum += 12 * this.C44;
            Sum += 6 * this.C66;

            double Ret = 15 / Sum;

            return Ret;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] C11
        ///[1] C33
        ///[2] C12
        ///[3] C13
        ///[4] C44
        ///[5] C66
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorVoigtType2Classic(double Lambda)
        {
            //[0][0] C11
            //[1][1] C12
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(6, 6, 0.0);

            //[0] Aclivity
            //[1] Constant
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(6);

            for (int n = 0; n < this.DiffractionConstants.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeC11S1VoigtType2() * this.FirstDerivativeC11S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 0] += (this.FirstDerivativeC11HS2VoigtType2() * this.FirstDerivativeC11HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeC33S1VoigtType2() * this.FirstDerivativeC33S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 1] += (this.FirstDerivativeC33HS2VoigtType2() * this.FirstDerivativeC33HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeC33S1VoigtType2() * this.FirstDerivativeC11S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeC33HS2VoigtType2() * this.FirstDerivativeC11HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeC33S1VoigtType2() * this.FirstDerivativeC11S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeC33HS2VoigtType2() * this.FirstDerivativeC11HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeC12S1VoigtType2() * this.FirstDerivativeC12S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 2] += (this.FirstDerivativeC12HS2VoigtType2() * this.FirstDerivativeC12HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeC12S1VoigtType2() * this.FirstDerivativeC11S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeC12HS2VoigtType2() * this.FirstDerivativeC11HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeC12S1VoigtType2() * this.FirstDerivativeC11S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeC12HS2VoigtType2() * this.FirstDerivativeC11HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeC12S1VoigtType2() * this.FirstDerivativeC33S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeC12HS2VoigtType2() * this.FirstDerivativeC33HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeC12S1VoigtType2() * this.FirstDerivativeC33S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeC12HS2VoigtType2() * this.FirstDerivativeC33HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[3, 3] += (this.FirstDerivativeC13S1VoigtType2() * this.FirstDerivativeC13S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 3] += (this.FirstDerivativeC13HS2VoigtType2() * this.FirstDerivativeC13HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 3] += (this.FirstDerivativeC13S1VoigtType2() * this.FirstDerivativeC11S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 3] += (this.FirstDerivativeC13HS2VoigtType2() * this.FirstDerivativeC11HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[3, 0] += (this.FirstDerivativeC13S1VoigtType2() * this.FirstDerivativeC11S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 0] += (this.FirstDerivativeC13HS2VoigtType2() * this.FirstDerivativeC11HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 3] += (this.FirstDerivativeC13S1VoigtType2() * this.FirstDerivativeC33S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 3] += (this.FirstDerivativeC13HS2VoigtType2() * this.FirstDerivativeC33HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[3, 1] += (this.FirstDerivativeC13S1VoigtType2() * this.FirstDerivativeC33S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 1] += (this.FirstDerivativeC13HS2VoigtType2() * this.FirstDerivativeC33HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 3] += (this.FirstDerivativeC13S1VoigtType2() * this.FirstDerivativeC12S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 3] += (this.FirstDerivativeC13HS2VoigtType2() * this.FirstDerivativeC12HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[3, 2] += (this.FirstDerivativeC13S1VoigtType2() * this.FirstDerivativeC12S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 2] += (this.FirstDerivativeC13HS2VoigtType2() * this.FirstDerivativeC12HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[4, 4] += (this.FirstDerivativeC44S1VoigtType2() * this.FirstDerivativeC44S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 4] += (this.FirstDerivativeC44HS2VoigtType2() * this.FirstDerivativeC44HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 4] += (this.FirstDerivativeC44S1VoigtType2() * this.FirstDerivativeC11S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 4] += (this.FirstDerivativeC44HS2VoigtType2() * this.FirstDerivativeC11HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[4, 0] += (this.FirstDerivativeC44S1VoigtType2() * this.FirstDerivativeC11S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 0] += (this.FirstDerivativeC44HS2VoigtType2() * this.FirstDerivativeC11HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 4] += (this.FirstDerivativeC44S1VoigtType2() * this.FirstDerivativeC33S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 4] += (this.FirstDerivativeC44HS2VoigtType2() * this.FirstDerivativeC33HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[4, 1] += (this.FirstDerivativeC44S1VoigtType2() * this.FirstDerivativeC33S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 1] += (this.FirstDerivativeC44HS2VoigtType2() * this.FirstDerivativeC33HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 4] += (this.FirstDerivativeC44S1VoigtType2() * this.FirstDerivativeC12S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 4] += (this.FirstDerivativeC44HS2VoigtType2() * this.FirstDerivativeC12HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[4, 2] += (this.FirstDerivativeC44S1VoigtType2() * this.FirstDerivativeC12S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 2] += (this.FirstDerivativeC44HS2VoigtType2() * this.FirstDerivativeC12HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[3, 4] += (this.FirstDerivativeC44S1VoigtType2() * this.FirstDerivativeC13S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 4] += (this.FirstDerivativeC44HS2VoigtType2() * this.FirstDerivativeC13HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[4, 3] += (this.FirstDerivativeC44S1VoigtType2() * this.FirstDerivativeC13S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 3] += (this.FirstDerivativeC44HS2VoigtType2() * this.FirstDerivativeC13HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[5, 5] += (this.FirstDerivativeC66S1VoigtType2() * this.FirstDerivativeC66S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[5, 5] += (this.FirstDerivativeC66HS2VoigtType2() * this.FirstDerivativeC66HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 5] += (this.FirstDerivativeC66S1VoigtType2() * this.FirstDerivativeC11S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 5] += (this.FirstDerivativeC66HS2VoigtType2() * this.FirstDerivativeC11HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[5, 0] += (this.FirstDerivativeC66S1VoigtType2() * this.FirstDerivativeC11S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[5, 0] += (this.FirstDerivativeC66HS2VoigtType2() * this.FirstDerivativeC11HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 5] += (this.FirstDerivativeC66S1VoigtType2() * this.FirstDerivativeC33S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 5] += (this.FirstDerivativeC66HS2VoigtType2() * this.FirstDerivativeC33HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[5, 1] += (this.FirstDerivativeC66S1VoigtType2() * this.FirstDerivativeC33S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[5, 1] += (this.FirstDerivativeC66HS2VoigtType2() * this.FirstDerivativeC33HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 5] += (this.FirstDerivativeC66S1VoigtType2() * this.FirstDerivativeC12S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 5] += (this.FirstDerivativeC66HS2VoigtType2() * this.FirstDerivativeC12HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[5, 2] += (this.FirstDerivativeC66S1VoigtType2() * this.FirstDerivativeC12S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[5, 2] += (this.FirstDerivativeC66HS2VoigtType2() * this.FirstDerivativeC12HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[3, 5] += (this.FirstDerivativeC66S1VoigtType2() * this.FirstDerivativeC13S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 5] += (this.FirstDerivativeC66HS2VoigtType2() * this.FirstDerivativeC13HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[5, 3] += (this.FirstDerivativeC66S1VoigtType2() * this.FirstDerivativeC13S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[5, 3] += (this.FirstDerivativeC66HS2VoigtType2() * this.FirstDerivativeC13HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[4, 5] += (this.FirstDerivativeC66S1VoigtType2() * this.FirstDerivativeC44S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 5] += (this.FirstDerivativeC66HS2VoigtType2() * this.FirstDerivativeC44HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[5, 4] += (this.FirstDerivativeC66S1VoigtType2() * this.FirstDerivativeC44S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[5, 4] += (this.FirstDerivativeC66HS2VoigtType2() * this.FirstDerivativeC44HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                #endregion

                #region Vector build

                SolutionVector[0] += ((this.DiffractionConstants[n].ClassicS1 - this.S1VoigtType2()) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeC11S1VoigtType2();
                SolutionVector[0] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2VoigtType2()) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeC11HS2VoigtType2();
                SolutionVector[1] += ((this.DiffractionConstants[n].ClassicS1 - this.S1VoigtType2()) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeC33S1VoigtType2();
                SolutionVector[1] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2VoigtType2()) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeC33HS2VoigtType2();
                SolutionVector[2] += ((this.DiffractionConstants[n].ClassicS1 - this.S1VoigtType2()) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeC12S1VoigtType2();
                SolutionVector[2] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2VoigtType2()) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeC12HS2VoigtType2();
                SolutionVector[3] += ((this.DiffractionConstants[n].ClassicS1 - this.S1VoigtType2()) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeC13S1VoigtType2();
                SolutionVector[3] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2VoigtType2()) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeC13HS2VoigtType2();
                SolutionVector[4] += ((this.DiffractionConstants[n].ClassicS1 - this.S1VoigtType2()) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeC44S1VoigtType2();
                SolutionVector[4] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2VoigtType2()) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeC44HS2VoigtType2();
                SolutionVector[5] += ((this.DiffractionConstants[n].ClassicS1 - this.S1VoigtType2()) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeC66S1VoigtType2();
                SolutionVector[5] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2VoigtType2()) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeC66HS2VoigtType2();

                #endregion
            }

            for (int n = 0; n < 6; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(6);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] C11
        ///[1] C33
        ///[2] C12
        ///[3] C13
        ///[4] C44
        ///[5] C66
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorVoigtType2Macroscopic(double Lambda)
        {
            //[0][0] C11
            //[1][1] C12
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(6, 6, 0.0);

            //[0] Aclivity
            //[1] Constant
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(6);

            for (int n = 0; n < this.DiffractionConstants.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeC11S1VoigtType2() * this.FirstDerivativeC11S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 0] += (this.FirstDerivativeC11HS2VoigtType2() * this.FirstDerivativeC11HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeC33S1VoigtType2() * this.FirstDerivativeC33S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 1] += (this.FirstDerivativeC33HS2VoigtType2() * this.FirstDerivativeC33HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeC33S1VoigtType2() * this.FirstDerivativeC11S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeC33HS2VoigtType2() * this.FirstDerivativeC11HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeC33S1VoigtType2() * this.FirstDerivativeC11S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeC33HS2VoigtType2() * this.FirstDerivativeC11HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeC12S1VoigtType2() * this.FirstDerivativeC12S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 2] += (this.FirstDerivativeC12HS2VoigtType2() * this.FirstDerivativeC12HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeC12S1VoigtType2() * this.FirstDerivativeC11S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeC12HS2VoigtType2() * this.FirstDerivativeC11HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeC12S1VoigtType2() * this.FirstDerivativeC11S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeC12HS2VoigtType2() * this.FirstDerivativeC11HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeC12S1VoigtType2() * this.FirstDerivativeC33S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeC12HS2VoigtType2() * this.FirstDerivativeC33HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeC12S1VoigtType2() * this.FirstDerivativeC33S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeC12HS2VoigtType2() * this.FirstDerivativeC33HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[3, 3] += (this.FirstDerivativeC13S1VoigtType2() * this.FirstDerivativeC13S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 3] += (this.FirstDerivativeC13HS2VoigtType2() * this.FirstDerivativeC13HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 3] += (this.FirstDerivativeC13S1VoigtType2() * this.FirstDerivativeC11S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 3] += (this.FirstDerivativeC13HS2VoigtType2() * this.FirstDerivativeC11HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[3, 0] += (this.FirstDerivativeC13S1VoigtType2() * this.FirstDerivativeC11S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 0] += (this.FirstDerivativeC13HS2VoigtType2() * this.FirstDerivativeC11HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 3] += (this.FirstDerivativeC13S1VoigtType2() * this.FirstDerivativeC33S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 3] += (this.FirstDerivativeC13HS2VoigtType2() * this.FirstDerivativeC33HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[3, 1] += (this.FirstDerivativeC13S1VoigtType2() * this.FirstDerivativeC33S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 1] += (this.FirstDerivativeC13HS2VoigtType2() * this.FirstDerivativeC33HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 3] += (this.FirstDerivativeC13S1VoigtType2() * this.FirstDerivativeC12S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 3] += (this.FirstDerivativeC13HS2VoigtType2() * this.FirstDerivativeC12HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[3, 2] += (this.FirstDerivativeC13S1VoigtType2() * this.FirstDerivativeC12S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 2] += (this.FirstDerivativeC13HS2VoigtType2() * this.FirstDerivativeC12HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[4, 4] += (this.FirstDerivativeC44S1VoigtType2() * this.FirstDerivativeC44S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 4] += (this.FirstDerivativeC44HS2VoigtType2() * this.FirstDerivativeC44HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 4] += (this.FirstDerivativeC44S1VoigtType2() * this.FirstDerivativeC11S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 4] += (this.FirstDerivativeC44HS2VoigtType2() * this.FirstDerivativeC11HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[4, 0] += (this.FirstDerivativeC44S1VoigtType2() * this.FirstDerivativeC11S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 0] += (this.FirstDerivativeC44HS2VoigtType2() * this.FirstDerivativeC11HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 4] += (this.FirstDerivativeC44S1VoigtType2() * this.FirstDerivativeC33S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 4] += (this.FirstDerivativeC44HS2VoigtType2() * this.FirstDerivativeC33HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[4, 1] += (this.FirstDerivativeC44S1VoigtType2() * this.FirstDerivativeC33S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 1] += (this.FirstDerivativeC44HS2VoigtType2() * this.FirstDerivativeC33HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 4] += (this.FirstDerivativeC44S1VoigtType2() * this.FirstDerivativeC12S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 4] += (this.FirstDerivativeC44HS2VoigtType2() * this.FirstDerivativeC12HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[4, 2] += (this.FirstDerivativeC44S1VoigtType2() * this.FirstDerivativeC12S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 2] += (this.FirstDerivativeC44HS2VoigtType2() * this.FirstDerivativeC12HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[3, 4] += (this.FirstDerivativeC44S1VoigtType2() * this.FirstDerivativeC13S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 4] += (this.FirstDerivativeC44HS2VoigtType2() * this.FirstDerivativeC13HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[4, 3] += (this.FirstDerivativeC44S1VoigtType2() * this.FirstDerivativeC13S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 3] += (this.FirstDerivativeC44HS2VoigtType2() * this.FirstDerivativeC13HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[5, 5] += (this.FirstDerivativeC66S1VoigtType2() * this.FirstDerivativeC66S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[5, 5] += (this.FirstDerivativeC66HS2VoigtType2() * this.FirstDerivativeC66HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 5] += (this.FirstDerivativeC66S1VoigtType2() * this.FirstDerivativeC11S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 5] += (this.FirstDerivativeC66HS2VoigtType2() * this.FirstDerivativeC11HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[5, 0] += (this.FirstDerivativeC66S1VoigtType2() * this.FirstDerivativeC11S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[5, 0] += (this.FirstDerivativeC66HS2VoigtType2() * this.FirstDerivativeC11HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 5] += (this.FirstDerivativeC66S1VoigtType2() * this.FirstDerivativeC33S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 5] += (this.FirstDerivativeC66HS2VoigtType2() * this.FirstDerivativeC33HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[5, 1] += (this.FirstDerivativeC66S1VoigtType2() * this.FirstDerivativeC33S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[5, 1] += (this.FirstDerivativeC66HS2VoigtType2() * this.FirstDerivativeC33HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 5] += (this.FirstDerivativeC66S1VoigtType2() * this.FirstDerivativeC12S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 5] += (this.FirstDerivativeC66HS2VoigtType2() * this.FirstDerivativeC12HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[5, 2] += (this.FirstDerivativeC66S1VoigtType2() * this.FirstDerivativeC12S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[5, 2] += (this.FirstDerivativeC66HS2VoigtType2() * this.FirstDerivativeC12HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[3, 5] += (this.FirstDerivativeC66S1VoigtType2() * this.FirstDerivativeC13S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 5] += (this.FirstDerivativeC66HS2VoigtType2() * this.FirstDerivativeC13HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[5, 3] += (this.FirstDerivativeC66S1VoigtType2() * this.FirstDerivativeC13S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[5, 3] += (this.FirstDerivativeC66HS2VoigtType2() * this.FirstDerivativeC13HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[4, 5] += (this.FirstDerivativeC66S1VoigtType2() * this.FirstDerivativeC44S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 5] += (this.FirstDerivativeC66HS2VoigtType2() * this.FirstDerivativeC44HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[5, 4] += (this.FirstDerivativeC66S1VoigtType2() * this.FirstDerivativeC44S1VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[5, 4] += (this.FirstDerivativeC66HS2VoigtType2() * this.FirstDerivativeC44HS2VoigtType2() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                #endregion

                #region Vector build

                SolutionVector[0] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1VoigtType2()) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeC11S1VoigtType2();
                SolutionVector[0] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2VoigtType2()) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeC11HS2VoigtType2();
                SolutionVector[1] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1VoigtType2()) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeC33S1VoigtType2();
                SolutionVector[1] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2VoigtType2()) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeC33HS2VoigtType2();
                SolutionVector[2] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1VoigtType2()) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeC12S1VoigtType2();
                SolutionVector[2] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2VoigtType2()) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeC12HS2VoigtType2();
                SolutionVector[3] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1VoigtType2()) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeC13S1VoigtType2();
                SolutionVector[3] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2VoigtType2()) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeC13HS2VoigtType2();
                SolutionVector[4] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1VoigtType2()) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeC44S1VoigtType2();
                SolutionVector[4] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2VoigtType2()) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeC44HS2VoigtType2();
                SolutionVector[5] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1VoigtType2()) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeC66S1VoigtType2();
                SolutionVector[5] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2VoigtType2()) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeC66HS2VoigtType2();

                #endregion
            }

            for (int n = 0; n < 6; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(6);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        #region First derivative

        public double FirstDerivativeC11S1VoigtType2()
        {
            double Ret = 4 * Math.Pow(this.C11, 2);
            Ret += 6 * Math.Pow(this.C12, 2);
            Ret += 24 * Math.Pow(this.C13, 2);
            Ret += 16 * this.C13 * this.C33;
            Ret += Math.Pow(this.C33, 2);
            Ret += 16 * this.C13 * this.C44;
            Ret -= 8 * this.C44 * this.C33;
            Ret -= 24 * Math.Pow(this.C44, 2);
            Ret += 8 * this.C13 * this.C66;
            Ret -= 4 * this.C33 * this.C66;
            Ret -= 24 * this.C44 * this.C66;
            Ret -= 6 * Math.Pow(this.C66, 2);

            double Sum = 4 * this.C12;
            Sum += 8 * this.C13;
            Sum += this.C33;
            Sum -= 4 * this.C44;
            Sum -= 2 * this.C66;

            Ret += 4 * this.C11 * Sum;

            Sum = 6 * this.C13;
            Sum += 2 * this.C33;
            Sum += 2 * this.C44;
            Sum += this.C66;

            Ret += 4 * this.C12 * Sum;
            Ret *= 3;

            double SumN = 2 * this.C11;
            SumN += 2 * this.C12;
            SumN += 4 * this.C13;
            SumN += this.C33;

            double N = Math.Pow(SumN, 2);

            SumN = 2 * this.C11;
            SumN -= this.C12;
            SumN -= 2 * this.C13;
            SumN += this.C33;
            SumN += 6 * this.C44;
            SumN += 3 * this.C66;

            N *= Math.Pow(SumN, 2);

            Ret /= N;

            return Ret;
        }

        public double FirstDerivativeC33S1VoigtType2()
        {
            double Ret = 4 * Math.Pow(this.C11, 2);
            Ret += 6 * Math.Pow(this.C12, 2);
            Ret += 24 * Math.Pow(this.C13, 2);
            Ret += 16 * this.C13 * this.C33;
            Ret += Math.Pow(this.C33, 2);
            Ret += 16 * this.C13 * this.C44;
            Ret -= 8 * this.C44 * this.C33;
            Ret -= 24 * Math.Pow(this.C44, 2);
            Ret += 8 * this.C13 * this.C66;
            Ret -= 4 * this.C33 * this.C66;
            Ret -= 24 * this.C44 * this.C66;
            Ret -= 6 * Math.Pow(this.C66, 2);

            double Sum = 4 * this.C12;
            Sum += 8 * this.C13;
            Sum += this.C33;
            Sum -= 4 * this.C44;
            Sum -= 2 * this.C66;

            Ret += 4 * this.C11 * Sum;

            Sum = 6 * this.C13;
            Sum += 2 * this.C33;
            Sum += 2 * this.C44;
            Sum += this.C66;

            Ret += 4 * this.C12 * Sum;
            Ret *= 3;

            double SumN = 2 * this.C11;
            SumN += 2 * this.C12;
            SumN += 4 * this.C13;
            SumN += this.C33;

            double N = Math.Pow(SumN, 2);

            SumN = 2 * this.C11;
            SumN -= this.C12;
            SumN -= 2 * this.C13;
            SumN += this.C33;
            SumN += 6 * this.C44;
            SumN += 3 * this.C66;

            N *= Math.Pow(SumN, 2);

            Ret /= 2 * N;

            return Ret;
        }

        public double FirstDerivativeC12S1VoigtType2()
        {
            double Ret = 12 * Math.Pow(this.C11, 2);
            Ret += 8 * Math.Pow(this.C12, 2);
            Ret += 32 * Math.Pow(this.C13, 2);
            Ret += 8 * this.C13 * this.C33;
            Ret += 3 * Math.Pow(this.C33, 2);
            Ret -= 32 * this.C13 * this.C44;
            Ret += 16 * this.C44 * this.C33;
            Ret += 48 * Math.Pow(this.C44, 2);
            Ret -= 16 * this.C13 * this.C66;
            Ret += 8 * this.C33 * this.C66;
            Ret += 48 * this.C44 * this.C66;
            Ret += 12 * Math.Pow(this.C66, 2);

            double Sum = 2 * this.C12;
            Sum += 4 * this.C13;
            Sum += 3 * this.C33;
            Sum += 8 * this.C44;
            Sum += 4 * this.C66;

            Ret += 4 * this.C11 * Sum;

            Sum = 8 * this.C13;
            Sum += this.C33;
            Sum -= 4 * this.C44;
            Sum -= 2 * this.C66;

            Ret += 4 * this.C12 * Sum;
            Ret *= -3;

            double SumN = 2 * this.C11;
            SumN += 2 * this.C12;
            SumN += 4 * this.C13;
            SumN += this.C33;

            double N = Math.Pow(SumN, 2);

            SumN = 2 * this.C11;
            SumN -= this.C12;
            SumN -= 2 * this.C13;
            SumN += this.C33;
            SumN += 6 * this.C44;
            SumN += 3 * this.C66;

            N *= Math.Pow(SumN, 2);

            Ret /= 2 * N;

            return Ret;
        }

        public double FirstDerivativeC13S1VoigtType2()
        {
            double Ret = 12 * Math.Pow(this.C11, 2);
            Ret += 8 * Math.Pow(this.C12, 2);
            Ret += 32 * Math.Pow(this.C13, 2);
            Ret += 8 * this.C13 * this.C33;
            Ret += 3 * Math.Pow(this.C33, 2);
            Ret -= 32 * this.C13 * this.C44;
            Ret += 16 * this.C44 * this.C33;
            Ret += 48 * Math.Pow(this.C44, 2);
            Ret -= 16 * this.C13 * this.C66;
            Ret += 8 * this.C33 * this.C66;
            Ret += 48 * this.C44 * this.C66;
            Ret += 12 * Math.Pow(this.C66, 2);

            double Sum = 2 * this.C12;
            Sum += 4 * this.C13;
            Sum += 3 * this.C33;
            Sum += 8 * this.C44;
            Sum += 4 * this.C66;

            Ret += 4 * this.C11 * Sum;

            Sum = 8 * this.C13;
            Sum += this.C33;
            Sum -= 4 * this.C44;
            Sum -= 2 * this.C66;

            Ret += 4 * this.C12 * Sum;
            Ret *= -3;

            double SumN = 2 * this.C11;
            SumN += 2 * this.C12;
            SumN += 4 * this.C13;
            SumN += this.C33;

            double N = Math.Pow(SumN, 2);

            SumN = 2 * this.C11;
            SumN -= this.C12;
            SumN -= 2 * this.C13;
            SumN += this.C33;
            SumN += 6 * this.C44;
            SumN += 3 * this.C66;

            N *= Math.Pow(SumN, 2);

            Ret /= N;

            return Ret;
        }

        public double FirstDerivativeC44S1VoigtType2()
        {
            double Ret = 15;

            double N = 2 * this.C11;
            N -= this.C12;
            N -= 2 * this.C13;
            N += this.C33;
            N += 6 * this.C44;
            N += 3 * this.C66;

            Ret /= Math.Pow(N, 2);

            return Ret;
        }

        public double FirstDerivativeC66S1VoigtType2()
        {
            double Ret = 15;

            double N = 2 * this.C11;
            N -= this.C12;
            N -= 2 * this.C13;
            N += this.C33;
            N += 6 * this.C44;
            N += 3 * this.C66;

            Ret /= 2 * Math.Pow(N, 2);

            return Ret;
        }

        public double FirstDerivativeC11HS2VoigtType2()
        {
            double Ret = -60;

            double N = 4 * this.C11;
            N -= 2 * this.C12;
            N -= 4 * this.C13;
            N += 2 * this.C33;
            N += 12 * this.C44;
            N += 6 * this.C66;

            Ret /= Math.Pow(N, 2);

            return Ret;
        }

        public double FirstDerivativeC33HS2VoigtType2()
        {
            double Ret = -30;

            double N = 4 * this.C11;
            N -= 2 * this.C12;
            N -= 4 * this.C13;
            N += 2 * this.C33;
            N += 12 * this.C44;
            N += 6 * this.C66;

            Ret /= Math.Pow(N, 2);

            return Ret;
        }

        public double FirstDerivativeC12HS2VoigtType2()
        {
            double Ret = 30;

            double N = 4 * this.C11;
            N -= 2 * this.C12;
            N -= 4 * this.C13;
            N += 2 * this.C33;
            N += 12 * this.C44;
            N += 6 * this.C66;

            Ret /= Math.Pow(N, 2);

            return Ret;
        }

        public double FirstDerivativeC13HS2VoigtType2()
        {
            double Ret = 60;

            double N = 4 * this.C11;
            N -= 2 * this.C12;
            N -= 4 * this.C13;
            N += 2 * this.C33;
            N += 12 * this.C44;
            N += 6 * this.C66;

            Ret /= Math.Pow(N, 2);

            return Ret;
        }

        public double FirstDerivativeC44HS2VoigtType2()
        {
            double Ret = -180;

            double N = 4 * this.C11;
            N -= 2 * this.C12;
            N -= 4 * this.C13;
            N += 2 * this.C33;
            N += 12 * this.C44;
            N += 6 * this.C66;

            Ret /= Math.Pow(N, 2);

            return Ret;
        }

        public double FirstDerivativeC66HS2VoigtType2()
        {
            double Ret = -90;

            double N = 4 * this.C11;
            N -= 2 * this.C12;
            N -= 4 * this.C13;
            N += 2 * this.C33;
            N += 12 * this.C44;
            N += 6 * this.C66;

            Ret /= Math.Pow(N, 2);

            return Ret;
        }

        #endregion

        #endregion

        #region Type 3 (rhombic, monoclinic, triclinic)

        public double S1VoigtType3()
        {
            double Ret = this.C11 + this.C22 + this.C33;
            Ret += 4 * (this.C12 + this.C13 + this.C23);
            Ret -= 2 * (this.C44 + this.C55 + this.C66);

            Ret *= -3 / 2;

            double Sum = this.C11 + this.C22 + this.C33;
            Sum -= this.C12 + this.C13 + this.C23;
            Sum += 3 * (this.C44 + this.C55 + this.C66);

            double Sum1 = this.C11 + this.C22 + this.C33;
            Sum1 += 2 * (this.C12 + this.C13 + this.C23);

            Sum *= Sum1;

            Ret /= Sum;

            return Ret;
        }

        public double HS2VoigtType3()
        {
            double Sum = 2 * (this.C11 + this.C22 + this.C33);
            Sum -= 2 * (this.C12 + this.C13 + this.C23);
            Sum += 6 * (this.C44 + this.C55 + this.C66);

            double Ret = 15 / Sum;

            return Ret;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] C11
        ///[1] C22
        ///[2] C33
        ///[3] C12
        ///[4] C13
        ///[5] C23
        ///[6] C44
        ///[7] C55
        ///[8] C66
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorVoigtType3Classic(double Lambda)
        {
            //[0][0] C11
            //[1][1] C12
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(9, 9, 0.0);

            //[0] Aclivity
            //[1] Constant
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(9);

            for (int n = 0; n < this.DiffractionConstants.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeC11S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 0] += (this.FirstDerivativeC11HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeC22S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 1] += (this.FirstDerivativeC22HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeC22S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeC22HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeC22S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeC22HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeC33S1VoigtType3() * this.FirstDerivativeC33S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 2] += (this.FirstDerivativeC33HS2VoigtType3() * this.FirstDerivativeC33HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeC33S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeC33HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeC33S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeC33HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeC33S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeC33HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeC33S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeC33HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[3, 3] += (this.FirstDerivativeC12S1VoigtType3() * this.FirstDerivativeC12S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 3] += (this.FirstDerivativeC12HS2VoigtType3() * this.FirstDerivativeC12HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 3] += (this.FirstDerivativeC12S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 3] += (this.FirstDerivativeC12HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[3, 0] += (this.FirstDerivativeC12S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 0] += (this.FirstDerivativeC12HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 3] += (this.FirstDerivativeC12S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 3] += (this.FirstDerivativeC12HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[3, 1] += (this.FirstDerivativeC12S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 1] += (this.FirstDerivativeC12HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 3] += (this.FirstDerivativeC12S1VoigtType3() * this.FirstDerivativeC33S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 3] += (this.FirstDerivativeC12HS2VoigtType3() * this.FirstDerivativeC33HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[3, 2] += (this.FirstDerivativeC12S1VoigtType3() * this.FirstDerivativeC33S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 2] += (this.FirstDerivativeC12HS2VoigtType3() * this.FirstDerivativeC33HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[4, 4] += (this.FirstDerivativeC13S1VoigtType3() * this.FirstDerivativeC13S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 4] += (this.FirstDerivativeC13HS2VoigtType3() * this.FirstDerivativeC13HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 4] += (this.FirstDerivativeC13S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 4] += (this.FirstDerivativeC13HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[4, 0] += (this.FirstDerivativeC13S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 0] += (this.FirstDerivativeC13HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 4] += (this.FirstDerivativeC13S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 4] += (this.FirstDerivativeC13HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[4, 1] += (this.FirstDerivativeC13S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 1] += (this.FirstDerivativeC13HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 4] += (this.FirstDerivativeC13S1VoigtType3() * this.FirstDerivativeC33S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 4] += (this.FirstDerivativeC13HS2VoigtType3() * this.FirstDerivativeC33HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[4, 2] += (this.FirstDerivativeC13S1VoigtType3() * this.FirstDerivativeC33S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 2] += (this.FirstDerivativeC13HS2VoigtType3() * this.FirstDerivativeC33HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[3, 4] += (this.FirstDerivativeC13S1VoigtType3() * this.FirstDerivativeC12S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 4] += (this.FirstDerivativeC13HS2VoigtType3() * this.FirstDerivativeC12HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[4, 3] += (this.FirstDerivativeC13S1VoigtType3() * this.FirstDerivativeC12S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 3] += (this.FirstDerivativeC13HS2VoigtType3() * this.FirstDerivativeC12HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[5, 5] += (this.FirstDerivativeC23S1VoigtType3() * this.FirstDerivativeC23S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[5, 5] += (this.FirstDerivativeC23HS2VoigtType3() * this.FirstDerivativeC23HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 5] += (this.FirstDerivativeC23S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 5] += (this.FirstDerivativeC23HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[5, 0] += (this.FirstDerivativeC23S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[5, 0] += (this.FirstDerivativeC23HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 5] += (this.FirstDerivativeC23S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 5] += (this.FirstDerivativeC23HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[5, 1] += (this.FirstDerivativeC23S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[5, 1] += (this.FirstDerivativeC23HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 5] += (this.FirstDerivativeC23S1VoigtType3() * this.FirstDerivativeC33S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 5] += (this.FirstDerivativeC23HS2VoigtType3() * this.FirstDerivativeC33HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[5, 2] += (this.FirstDerivativeC23S1VoigtType3() * this.FirstDerivativeC33S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[5, 2] += (this.FirstDerivativeC23HS2VoigtType3() * this.FirstDerivativeC33HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[3, 5] += (this.FirstDerivativeC23S1VoigtType3() * this.FirstDerivativeC12S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 5] += (this.FirstDerivativeC23HS2VoigtType3() * this.FirstDerivativeC12HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[5, 3] += (this.FirstDerivativeC23S1VoigtType3() * this.FirstDerivativeC12S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[5, 3] += (this.FirstDerivativeC23HS2VoigtType3() * this.FirstDerivativeC12HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[4, 5] += (this.FirstDerivativeC23S1VoigtType3() * this.FirstDerivativeC13S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 5] += (this.FirstDerivativeC23HS2VoigtType3() * this.FirstDerivativeC13HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[5, 4] += (this.FirstDerivativeC23S1VoigtType3() * this.FirstDerivativeC13S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[5, 4] += (this.FirstDerivativeC23HS2VoigtType3() * this.FirstDerivativeC13HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[6, 6] += (this.FirstDerivativeC44S1VoigtType3() * this.FirstDerivativeC44S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[6, 6] += (this.FirstDerivativeC44HS2VoigtType3() * this.FirstDerivativeC44HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 6] += (this.FirstDerivativeC44S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 6] += (this.FirstDerivativeC44HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[6, 0] += (this.FirstDerivativeC44S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[6, 0] += (this.FirstDerivativeC44HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 6] += (this.FirstDerivativeC44S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 6] += (this.FirstDerivativeC44HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[6, 1] += (this.FirstDerivativeC44S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[6, 1] += (this.FirstDerivativeC44HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 6] += (this.FirstDerivativeC44S1VoigtType3() * this.FirstDerivativeC33S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 6] += (this.FirstDerivativeC44HS2VoigtType3() * this.FirstDerivativeC33HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[6, 2] += (this.FirstDerivativeC44S1VoigtType3() * this.FirstDerivativeC33S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[6, 2] += (this.FirstDerivativeC44HS2VoigtType3() * this.FirstDerivativeC33HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[3, 6] += (this.FirstDerivativeC44S1VoigtType3() * this.FirstDerivativeC12S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 6] += (this.FirstDerivativeC44HS2VoigtType3() * this.FirstDerivativeC12HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[6, 3] += (this.FirstDerivativeC44S1VoigtType3() * this.FirstDerivativeC12S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[6, 3] += (this.FirstDerivativeC44HS2VoigtType3() * this.FirstDerivativeC12HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[4, 6] += (this.FirstDerivativeC44S1VoigtType3() * this.FirstDerivativeC13S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 6] += (this.FirstDerivativeC44HS2VoigtType3() * this.FirstDerivativeC13HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[6, 4] += (this.FirstDerivativeC44S1VoigtType3() * this.FirstDerivativeC13S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[6, 4] += (this.FirstDerivativeC44HS2VoigtType3() * this.FirstDerivativeC13HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[5, 6] += (this.FirstDerivativeC44S1VoigtType3() * this.FirstDerivativeC23S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[5, 6] += (this.FirstDerivativeC44HS2VoigtType3() * this.FirstDerivativeC23HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[6, 5] += (this.FirstDerivativeC44S1VoigtType3() * this.FirstDerivativeC23S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[6, 5] += (this.FirstDerivativeC44HS2VoigtType3() * this.FirstDerivativeC23HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[7, 7] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC55S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[7, 7] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC55HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 7] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 7] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[7, 0] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[7, 0] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 7] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 7] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[7, 1] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[7, 1] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 7] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC33S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 7] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC33HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[7, 2] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC33S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[7, 2] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC33HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[3, 7] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC12S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 7] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC12HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[7, 3] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC12S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[7, 3] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC12HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[4, 7] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC13S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 7] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC13HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[7, 4] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC13S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[7, 4] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC13HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[5, 7] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC23S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[5, 7] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC23HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[7, 5] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC23S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[7, 5] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC23HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[6, 7] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC44S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[6, 7] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC44HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[7, 6] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC44S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[7, 6] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC44HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[8, 8] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC66S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[8, 8] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC66HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 8] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 8] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[8, 0] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[8, 0] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 8] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 8] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[8, 1] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[8, 1] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 8] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC33S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 8] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC33HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[8, 2] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC33S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[8, 2] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC33HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[3, 8] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC12S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 8] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC12HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[8, 3] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC12S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[8, 3] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC12HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[4, 8] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC13S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 8] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC13HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[8, 4] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC13S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[8, 4] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC13HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[5, 8] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC23S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[5, 8] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC23HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[8, 5] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC23S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[8, 5] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC23HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[6, 8] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC44S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[6, 8] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC44HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[8, 6] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC44S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[8, 6] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC44HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[7, 8] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC55S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[7, 8] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC55HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[8, 7] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC55S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[8, 7] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC55HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                #endregion

                #region Vector build

                SolutionVector[0] += ((this.DiffractionConstants[n].ClassicS1 - this.S1VoigtType3()) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeC11S1VoigtType3();
                SolutionVector[0] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2VoigtType3()) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeC11HS2VoigtType3();
                SolutionVector[1] += ((this.DiffractionConstants[n].ClassicS1 - this.S1VoigtType3()) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeC22S1VoigtType3();
                SolutionVector[1] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2VoigtType3()) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeC22HS2VoigtType3();
                SolutionVector[2] += ((this.DiffractionConstants[n].ClassicS1 - this.S1VoigtType3()) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeC33S1VoigtType3();
                SolutionVector[2] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2VoigtType3()) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeC33HS2VoigtType3();
                SolutionVector[3] += ((this.DiffractionConstants[n].ClassicS1 - this.S1VoigtType3()) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeC12S1VoigtType3();
                SolutionVector[3] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2VoigtType3()) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeC12HS2VoigtType3();
                SolutionVector[4] += ((this.DiffractionConstants[n].ClassicS1 - this.S1VoigtType3()) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeC13S1VoigtType3();
                SolutionVector[4] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2VoigtType3()) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeC13HS2VoigtType3();
                SolutionVector[5] += ((this.DiffractionConstants[n].ClassicS1 - this.S1VoigtType3()) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeC23S1VoigtType3();
                SolutionVector[5] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2VoigtType3()) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeC23HS2VoigtType3();
                SolutionVector[6] += ((this.DiffractionConstants[n].ClassicS1 - this.S1VoigtType3()) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeC44S1VoigtType3();
                SolutionVector[6] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2VoigtType3()) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeC44HS2VoigtType3();
                SolutionVector[7] += ((this.DiffractionConstants[n].ClassicS1 - this.S1VoigtType3()) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeC55S1VoigtType3();
                SolutionVector[7] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2VoigtType3()) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeC55HS2VoigtType3();
                SolutionVector[8] += ((this.DiffractionConstants[n].ClassicS1 - this.S1VoigtType3()) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeC66S1VoigtType3();
                SolutionVector[8] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2VoigtType3()) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeC66HS2VoigtType3();

                #endregion
            }

            for (int n = 0; n < 9; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(9);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] C11
        ///[1] C22
        ///[2] C33
        ///[3] C12
        ///[4] C13
        ///[5] C23
        ///[6] C44
        ///[7] C55
        ///[8] C66
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorVoigtType3Macroscopic(double Lambda)
        {
            //[0][0] C11
            //[1][1] C12
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(9, 9, 0.0);

            //[0] Aclivity
            //[1] Constant
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(9);

            for (int n = 0; n < this.DiffractionConstants.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeC11S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 0] += (this.FirstDerivativeC11HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeC22S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 1] += (this.FirstDerivativeC22HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeC22S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeC22HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeC22S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeC22HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeC33S1VoigtType3() * this.FirstDerivativeC33S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 2] += (this.FirstDerivativeC33HS2VoigtType3() * this.FirstDerivativeC33HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeC33S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeC33HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeC33S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeC33HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeC33S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeC33HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeC33S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeC33HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[3, 3] += (this.FirstDerivativeC12S1VoigtType3() * this.FirstDerivativeC12S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 3] += (this.FirstDerivativeC12HS2VoigtType3() * this.FirstDerivativeC12HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 3] += (this.FirstDerivativeC12S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 3] += (this.FirstDerivativeC12HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[3, 0] += (this.FirstDerivativeC12S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 0] += (this.FirstDerivativeC12HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 3] += (this.FirstDerivativeC12S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 3] += (this.FirstDerivativeC12HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[3, 1] += (this.FirstDerivativeC12S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 1] += (this.FirstDerivativeC12HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 3] += (this.FirstDerivativeC12S1VoigtType3() * this.FirstDerivativeC33S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 3] += (this.FirstDerivativeC12HS2VoigtType3() * this.FirstDerivativeC33HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[3, 2] += (this.FirstDerivativeC12S1VoigtType3() * this.FirstDerivativeC33S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 2] += (this.FirstDerivativeC12HS2VoigtType3() * this.FirstDerivativeC33HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[4, 4] += (this.FirstDerivativeC13S1VoigtType3() * this.FirstDerivativeC13S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 4] += (this.FirstDerivativeC13HS2VoigtType3() * this.FirstDerivativeC13HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 4] += (this.FirstDerivativeC13S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 4] += (this.FirstDerivativeC13HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[4, 0] += (this.FirstDerivativeC13S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 0] += (this.FirstDerivativeC13HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 4] += (this.FirstDerivativeC13S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 4] += (this.FirstDerivativeC13HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[4, 1] += (this.FirstDerivativeC13S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 1] += (this.FirstDerivativeC13HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 4] += (this.FirstDerivativeC13S1VoigtType3() * this.FirstDerivativeC33S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 4] += (this.FirstDerivativeC13HS2VoigtType3() * this.FirstDerivativeC33HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[4, 2] += (this.FirstDerivativeC13S1VoigtType3() * this.FirstDerivativeC33S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 2] += (this.FirstDerivativeC13HS2VoigtType3() * this.FirstDerivativeC33HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[3, 4] += (this.FirstDerivativeC13S1VoigtType3() * this.FirstDerivativeC12S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 4] += (this.FirstDerivativeC13HS2VoigtType3() * this.FirstDerivativeC12HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[4, 3] += (this.FirstDerivativeC13S1VoigtType3() * this.FirstDerivativeC12S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 3] += (this.FirstDerivativeC13HS2VoigtType3() * this.FirstDerivativeC12HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[5, 5] += (this.FirstDerivativeC23S1VoigtType3() * this.FirstDerivativeC23S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[5, 5] += (this.FirstDerivativeC23HS2VoigtType3() * this.FirstDerivativeC23HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 5] += (this.FirstDerivativeC23S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 5] += (this.FirstDerivativeC23HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[5, 0] += (this.FirstDerivativeC23S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[5, 0] += (this.FirstDerivativeC23HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 5] += (this.FirstDerivativeC23S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 5] += (this.FirstDerivativeC23HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[5, 1] += (this.FirstDerivativeC23S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[5, 1] += (this.FirstDerivativeC23HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 5] += (this.FirstDerivativeC23S1VoigtType3() * this.FirstDerivativeC33S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 5] += (this.FirstDerivativeC23HS2VoigtType3() * this.FirstDerivativeC33HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[5, 2] += (this.FirstDerivativeC23S1VoigtType3() * this.FirstDerivativeC33S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[5, 2] += (this.FirstDerivativeC23HS2VoigtType3() * this.FirstDerivativeC33HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[3, 5] += (this.FirstDerivativeC23S1VoigtType3() * this.FirstDerivativeC12S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 5] += (this.FirstDerivativeC23HS2VoigtType3() * this.FirstDerivativeC12HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[5, 3] += (this.FirstDerivativeC23S1VoigtType3() * this.FirstDerivativeC12S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[5, 3] += (this.FirstDerivativeC23HS2VoigtType3() * this.FirstDerivativeC12HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[4, 5] += (this.FirstDerivativeC23S1VoigtType3() * this.FirstDerivativeC13S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 5] += (this.FirstDerivativeC23HS2VoigtType3() * this.FirstDerivativeC13HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[5, 4] += (this.FirstDerivativeC23S1VoigtType3() * this.FirstDerivativeC13S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[5, 4] += (this.FirstDerivativeC23HS2VoigtType3() * this.FirstDerivativeC13HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[6, 6] += (this.FirstDerivativeC44S1VoigtType3() * this.FirstDerivativeC44S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[6, 6] += (this.FirstDerivativeC44HS2VoigtType3() * this.FirstDerivativeC44HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 6] += (this.FirstDerivativeC44S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 6] += (this.FirstDerivativeC44HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[6, 0] += (this.FirstDerivativeC44S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[6, 0] += (this.FirstDerivativeC44HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 6] += (this.FirstDerivativeC44S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 6] += (this.FirstDerivativeC44HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[6, 1] += (this.FirstDerivativeC44S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[6, 1] += (this.FirstDerivativeC44HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 6] += (this.FirstDerivativeC44S1VoigtType3() * this.FirstDerivativeC33S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 6] += (this.FirstDerivativeC44HS2VoigtType3() * this.FirstDerivativeC33HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[6, 2] += (this.FirstDerivativeC44S1VoigtType3() * this.FirstDerivativeC33S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[6, 2] += (this.FirstDerivativeC44HS2VoigtType3() * this.FirstDerivativeC33HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[3, 6] += (this.FirstDerivativeC44S1VoigtType3() * this.FirstDerivativeC12S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 6] += (this.FirstDerivativeC44HS2VoigtType3() * this.FirstDerivativeC12HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[6, 3] += (this.FirstDerivativeC44S1VoigtType3() * this.FirstDerivativeC12S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[6, 3] += (this.FirstDerivativeC44HS2VoigtType3() * this.FirstDerivativeC12HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[4, 6] += (this.FirstDerivativeC44S1VoigtType3() * this.FirstDerivativeC13S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 6] += (this.FirstDerivativeC44HS2VoigtType3() * this.FirstDerivativeC13HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[6, 4] += (this.FirstDerivativeC44S1VoigtType3() * this.FirstDerivativeC13S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[6, 4] += (this.FirstDerivativeC44HS2VoigtType3() * this.FirstDerivativeC13HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[5, 6] += (this.FirstDerivativeC44S1VoigtType3() * this.FirstDerivativeC23S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[5, 6] += (this.FirstDerivativeC44HS2VoigtType3() * this.FirstDerivativeC23HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[6, 5] += (this.FirstDerivativeC44S1VoigtType3() * this.FirstDerivativeC23S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[6, 5] += (this.FirstDerivativeC44HS2VoigtType3() * this.FirstDerivativeC23HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[7, 7] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC55S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[7, 7] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC55HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 7] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 7] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[7, 0] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[7, 0] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 7] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 7] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[7, 1] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[7, 1] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 7] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC33S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 7] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC33HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[7, 2] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC33S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[7, 2] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC33HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[3, 7] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC12S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 7] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC12HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[7, 3] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC12S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[7, 3] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC12HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[4, 7] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC13S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 7] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC13HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[7, 4] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC13S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[7, 4] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC13HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[5, 7] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC23S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[5, 7] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC23HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[7, 5] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC23S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[7, 5] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC23HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[6, 7] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC44S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[6, 7] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC44HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[7, 6] += (this.FirstDerivativeC55S1VoigtType3() * this.FirstDerivativeC44S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[7, 6] += (this.FirstDerivativeC55HS2VoigtType3() * this.FirstDerivativeC44HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[8, 8] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC66S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[8, 8] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC66HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 8] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 8] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[8, 0] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC11S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[8, 0] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC11HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 8] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 8] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[8, 1] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC22S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[8, 1] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC22HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 8] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC33S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 8] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC33HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[8, 2] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC33S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[8, 2] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC33HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[3, 8] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC12S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 8] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC12HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[8, 3] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC12S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[8, 3] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC12HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[4, 8] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC13S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 8] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC13HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[8, 4] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC13S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[8, 4] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC13HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[5, 8] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC23S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[5, 8] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC23HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[8, 5] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC23S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[8, 5] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC23HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[6, 8] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC44S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[6, 8] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC44HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[8, 6] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC44S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[8, 6] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC44HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[7, 8] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC55S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[7, 8] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC55HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[8, 7] += (this.FirstDerivativeC66S1VoigtType3() * this.FirstDerivativeC55S1VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[8, 7] += (this.FirstDerivativeC66HS2VoigtType3() * this.FirstDerivativeC55HS2VoigtType3() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                #endregion

                #region Vector build

                SolutionVector[0] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1VoigtType3()) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeC11S1VoigtType3();
                SolutionVector[0] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2VoigtType3()) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeC11HS2VoigtType3();
                SolutionVector[1] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1VoigtType3()) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeC22S1VoigtType3();
                SolutionVector[1] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2VoigtType3()) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeC22HS2VoigtType3();
                SolutionVector[2] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1VoigtType3()) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeC33S1VoigtType3();
                SolutionVector[2] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2VoigtType3()) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeC33HS2VoigtType3();
                SolutionVector[3] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1VoigtType3()) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeC12S1VoigtType3();
                SolutionVector[3] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2VoigtType3()) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeC12HS2VoigtType3();
                SolutionVector[4] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1VoigtType3()) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeC13S1VoigtType3();
                SolutionVector[4] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2VoigtType3()) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeC13HS2VoigtType3();
                SolutionVector[5] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1VoigtType3()) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeC23S1VoigtType3();
                SolutionVector[5] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2VoigtType3()) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeC23HS2VoigtType3();
                SolutionVector[6] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1VoigtType3()) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeC44S1VoigtType3();
                SolutionVector[6] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2VoigtType3()) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeC44HS2VoigtType3();
                SolutionVector[7] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1VoigtType3()) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeC55S1VoigtType3();
                SolutionVector[7] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2VoigtType3()) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeC55HS2VoigtType3();
                SolutionVector[8] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1VoigtType3()) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeC66S1VoigtType3();
                SolutionVector[8] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2VoigtType3()) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeC66HS2VoigtType3();

                #endregion
            }

            for (int n = 0; n < 9; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(9);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        #region First derivative

        public double FirstDerivativeC11S1VoigtType3()
        {
            double Sum = this.C11;
            Sum += 2 * this.C12;
            Sum += 2 * this.C13;
            Sum += this.C22;
            Sum += 2 * this.C23;
            Sum += this.C33;

            double Mult = this.C11;
            Mult += 4 * this.C12;
            Mult += 4 * this.C13;
            Mult += this.C22;
            Mult += 4 * this.C23;
            Mult += this.C33;
            Mult -= 2 * this.C44;
            Mult -= 2 * this.C55;
            Mult -= 2 * this.C66;

            double Ret = -Sum * Mult; ;

            Sum = this.C11;
            Sum += 2 * this.C12;
            Sum += 2 * this.C13;
            Sum += this.C22;
            Sum += 2 * this.C23;
            Sum += this.C33;

            Mult = this.C11;
            Mult -= this.C12;
            Mult -= this.C13;
            Mult += this.C22;
            Mult -= this.C23;
            Mult += this.C33;
            Mult += 3 * this.C44;
            Mult += 3 * this.C55;
            Mult += 3 * this.C66;

            Ret += Sum * Mult;

            Sum = this.C11;
            Sum += 4 * this.C12;
            Sum += 4 * this.C13;
            Sum += this.C22;
            Sum += 4 * this.C23;
            Sum += this.C33;
            Sum -= 2 * this.C44;
            Sum -= 2 * this.C55;
            Sum -= 2 * this.C66;

            Mult = this.C11;
            Mult -= this.C12;
            Mult -= this.C13;
            Mult += this.C22;
            Mult -= this.C23;
            Mult += this.C33;
            Mult += 3 * this.C44;
            Mult += 3 * this.C55;
            Mult += 3 * this.C66;

            Ret -= Sum * Mult;

            Sum = this.C11;
            Sum += 2 * this.C12;
            Sum += 2 * this.C13;
            Sum += this.C22;
            Sum += 2 * this.C23;
            Sum += this.C33;

            Mult = this.C11;
            Mult -= this.C12;
            Mult -= this.C13;
            Mult += this.C22;
            Mult -= this.C23;
            Mult += this.C33;
            Mult += 3 * this.C44;
            Mult += 3 * this.C55;
            Mult += 3 * this.C66;

            Ret /= Math.Pow(Sum, 2) * Math.Pow(Mult, 2);

            return Ret;
        }

        public double FirstDerivativeC22S1VoigtType3()
        {
            double Sum = this.C11;
            Sum += 2 * this.C12;
            Sum += 2 * this.C13;
            Sum += this.C22;
            Sum += 2 * this.C23;
            Sum += this.C33;

            double Mult = this.C11;
            Mult += 4 * this.C12;
            Mult += 4 * this.C13;
            Mult += this.C22;
            Mult += 4 * this.C23;
            Mult += this.C33;
            Mult -= 2 * this.C44;
            Mult -= 2 * this.C55;
            Mult -= 2 * this.C66;

            double Ret = -Sum * Mult; ;

            Sum = this.C11;
            Sum += 2 * this.C12;
            Sum += 2 * this.C13;
            Sum += this.C22;
            Sum += 2 * this.C23;
            Sum += this.C33;

            Mult = this.C11;
            Mult -= this.C12;
            Mult -= this.C13;
            Mult += this.C22;
            Mult -= this.C23;
            Mult += this.C33;
            Mult += 3 * this.C44;
            Mult += 3 * this.C55;
            Mult += 3 * this.C66;

            Ret += Sum * Mult;

            Sum = this.C11;
            Sum += 4 * this.C12;
            Sum += 4 * this.C13;
            Sum += this.C22;
            Sum += 4 * this.C23;
            Sum += this.C33;
            Sum -= 2 * this.C44;
            Sum -= 2 * this.C55;
            Sum -= 2 * this.C66;

            Mult = this.C11;
            Mult -= this.C12;
            Mult -= this.C13;
            Mult += this.C22;
            Mult -= this.C23;
            Mult += this.C33;
            Mult += 3 * this.C44;
            Mult += 3 * this.C55;
            Mult += 3 * this.C66;

            Ret -= Sum * Mult;

            Sum = this.C11;
            Sum += 2 * this.C12;
            Sum += 2 * this.C13;
            Sum += this.C22;
            Sum += 2 * this.C23;
            Sum += this.C33;

            Mult = this.C11;
            Mult -= this.C12;
            Mult -= this.C13;
            Mult += this.C22;
            Mult -= this.C23;
            Mult += this.C33;
            Mult += 3 * this.C44;
            Mult += 3 * this.C55;
            Mult += 3 * this.C66;

            Ret /= Math.Pow(Sum, 2) * Math.Pow(Mult, 2);

            return Ret;
        }

        public double FirstDerivativeC33S1VoigtType3()
        {
            double Sum = this.C11;
            Sum += 2 * this.C12;
            Sum += 2 * this.C13;
            Sum += this.C22;
            Sum += 2 * this.C23;
            Sum += this.C33;

            double Mult = this.C11;
            Mult += 4 * this.C12;
            Mult += 4 * this.C13;
            Mult += this.C22;
            Mult += 4 * this.C23;
            Mult += this.C33;
            Mult -= 2 * this.C44;
            Mult -= 2 * this.C55;
            Mult -= 2 * this.C66;

            double Ret = -Sum * Mult; ;

            Sum = this.C11;
            Sum += 2 * this.C12;
            Sum += 2 * this.C13;
            Sum += this.C22;
            Sum += 2 * this.C23;
            Sum += this.C33;

            Mult = this.C11;
            Mult -= this.C12;
            Mult -= this.C13;
            Mult += this.C22;
            Mult -= this.C23;
            Mult += this.C33;
            Mult += 3 * this.C44;
            Mult += 3 * this.C55;
            Mult += 3 * this.C66;

            Ret += Sum * Mult;

            Sum = this.C11;
            Sum += 4 * this.C12;
            Sum += 4 * this.C13;
            Sum += this.C22;
            Sum += 4 * this.C23;
            Sum += this.C33;
            Sum -= 2 * this.C44;
            Sum -= 2 * this.C55;
            Sum -= 2 * this.C66;

            Mult = this.C11;
            Mult -= this.C12;
            Mult -= this.C13;
            Mult += this.C22;
            Mult -= this.C23;
            Mult += this.C33;
            Mult += 3 * this.C44;
            Mult += 3 * this.C55;
            Mult += 3 * this.C66;

            Ret -= Sum * Mult;

            Sum = this.C11;
            Sum += 2 * this.C12;
            Sum += 2 * this.C13;
            Sum += this.C22;
            Sum += 2 * this.C23;
            Sum += this.C33;

            Mult = this.C11;
            Mult -= this.C12;
            Mult -= this.C13;
            Mult += this.C22;
            Mult -= this.C23;
            Mult += this.C33;
            Mult += 3 * this.C44;
            Mult += 3 * this.C55;
            Mult += 3 * this.C66;

            Ret /= Math.Pow(Sum, 2) * Math.Pow(Mult, 2);

            return Ret;
        }

        public double FirstDerivativeC12S1VoigtType3()
        {
            double Sum = this.C11;
            Sum += 2 * this.C12;
            Sum += 2 * this.C13;
            Sum += this.C22;
            Sum += 2 * this.C23;
            Sum += this.C33;

            double Mult = this.C11;
            Mult += 4 * this.C12;
            Mult += 4 * this.C13;
            Mult += this.C22;
            Mult += 4 * this.C23;
            Mult += this.C33;
            Mult -= 2 * this.C44;
            Mult -= 2 * this.C55;
            Mult -= 2 * this.C66;

            double Ret = Sum * Mult; ;

            Sum = this.C11;
            Sum += 2 * this.C12;
            Sum += 2 * this.C13;
            Sum += this.C22;
            Sum += 2 * this.C23;
            Sum += this.C33;

            Mult = this.C11;
            Mult -= this.C12;
            Mult -= this.C13;
            Mult += this.C22;
            Mult -= this.C23;
            Mult += this.C33;
            Mult += 3 * this.C44;
            Mult += 3 * this.C55;
            Mult += 3 * this.C66;

            Ret += 4 * Sum * Mult;

            Sum = this.C11;
            Sum += 4 * this.C12;
            Sum += 4 * this.C13;
            Sum += this.C22;
            Sum += 4 * this.C23;
            Sum += this.C33;
            Sum -= 2 * this.C44;
            Sum -= 2 * this.C55;
            Sum -= 2 * this.C66;

            Mult = this.C11;
            Mult -= this.C12;
            Mult -= this.C13;
            Mult += this.C22;
            Mult -= this.C23;
            Mult += this.C33;
            Mult += 3 * this.C44;
            Mult += 3 * this.C55;
            Mult += 3 * this.C66;

            Ret -= 2 * Sum * Mult;

            Sum = this.C11;
            Sum += 2 * this.C12;
            Sum += 2 * this.C13;
            Sum += this.C22;
            Sum += 2 * this.C23;
            Sum += this.C33;

            Mult = this.C11;
            Mult -= this.C12;
            Mult -= this.C13;
            Mult += this.C22;
            Mult -= this.C23;
            Mult += this.C33;
            Mult += 3 * this.C44;
            Mult += 3 * this.C55;
            Mult += 3 * this.C66;

            Ret /= Math.Pow(Sum, 2) * Math.Pow(Mult, 2);

            return Ret;
        }

        public double FirstDerivativeC13S1VoigtType3()
        {
            double Sum = this.C11;
            Sum += 2 * this.C12;
            Sum += 2 * this.C13;
            Sum += this.C22;
            Sum += 2 * this.C23;
            Sum += this.C33;

            double Mult = this.C11;
            Mult += 4 * this.C12;
            Mult += 4 * this.C13;
            Mult += this.C22;
            Mult += 4 * this.C23;
            Mult += this.C33;
            Mult -= 2 * this.C44;
            Mult -= 2 * this.C55;
            Mult -= 2 * this.C66;

            double Ret = Sum * Mult; ;

            Sum = this.C11;
            Sum += 2 * this.C12;
            Sum += 2 * this.C13;
            Sum += this.C22;
            Sum += 2 * this.C23;
            Sum += this.C33;

            Mult = this.C11;
            Mult -= this.C12;
            Mult -= this.C13;
            Mult += this.C22;
            Mult -= this.C23;
            Mult += this.C33;
            Mult += 3 * this.C44;
            Mult += 3 * this.C55;
            Mult += 3 * this.C66;

            Ret += 4 * Sum * Mult;

            Sum = this.C11;
            Sum += 4 * this.C12;
            Sum += 4 * this.C13;
            Sum += this.C22;
            Sum += 4 * this.C23;
            Sum += this.C33;
            Sum -= 2 * this.C44;
            Sum -= 2 * this.C55;
            Sum -= 2 * this.C66;

            Mult = this.C11;
            Mult -= this.C12;
            Mult -= this.C13;
            Mult += this.C22;
            Mult -= this.C23;
            Mult += this.C33;
            Mult += 3 * this.C44;
            Mult += 3 * this.C55;
            Mult += 3 * this.C66;

            Ret -= 2 * Sum * Mult;

            Sum = this.C11;
            Sum += 2 * this.C12;
            Sum += 2 * this.C13;
            Sum += this.C22;
            Sum += 2 * this.C23;
            Sum += this.C33;

            Mult = this.C11;
            Mult -= this.C12;
            Mult -= this.C13;
            Mult += this.C22;
            Mult -= this.C23;
            Mult += this.C33;
            Mult += 3 * this.C44;
            Mult += 3 * this.C55;
            Mult += 3 * this.C66;

            Ret /= Math.Pow(Sum, 2) * Math.Pow(Mult, 2);

            return Ret;
        }

        public double FirstDerivativeC23S1VoigtType3()
        {
            double Sum = this.C11;
            Sum += 2 * this.C12;
            Sum += 2 * this.C13;
            Sum += this.C22;
            Sum += 2 * this.C23;
            Sum += this.C33;

            double Mult = this.C11;
            Mult += 4 * this.C12;
            Mult += 4 * this.C13;
            Mult += this.C22;
            Mult += 4 * this.C23;
            Mult += this.C33;
            Mult -= 2 * this.C44;
            Mult -= 2 * this.C55;
            Mult -= 2 * this.C66;

            double Ret = Sum * Mult; ;

            Sum = this.C11;
            Sum += 2 * this.C12;
            Sum += 2 * this.C13;
            Sum += this.C22;
            Sum += 2 * this.C23;
            Sum += this.C33;

            Mult = this.C11;
            Mult -= this.C12;
            Mult -= this.C13;
            Mult += this.C22;
            Mult -= this.C23;
            Mult += this.C33;
            Mult += 3 * this.C44;
            Mult += 3 * this.C55;
            Mult += 3 * this.C66;

            Ret += 4 * Sum * Mult;

            Sum = this.C11;
            Sum += 4 * this.C12;
            Sum += 4 * this.C13;
            Sum += this.C22;
            Sum += 4 * this.C23;
            Sum += this.C33;
            Sum -= 2 * this.C44;
            Sum -= 2 * this.C55;
            Sum -= 2 * this.C66;

            Mult = this.C11;
            Mult -= this.C12;
            Mult -= this.C13;
            Mult += this.C22;
            Mult -= this.C23;
            Mult += this.C33;
            Mult += 3 * this.C44;
            Mult += 3 * this.C55;
            Mult += 3 * this.C66;

            Ret -= 2 * Sum * Mult;

            Sum = this.C11;
            Sum += 2 * this.C12;
            Sum += 2 * this.C13;
            Sum += this.C22;
            Sum += 2 * this.C23;
            Sum += this.C33;

            Mult = this.C11;
            Mult -= this.C12;
            Mult -= this.C13;
            Mult += this.C22;
            Mult -= this.C23;
            Mult += this.C33;
            Mult += 3 * this.C44;
            Mult += 3 * this.C55;
            Mult += 3 * this.C66;

            Ret /= Math.Pow(Sum, 2) * Math.Pow(Mult, 2);

            return Ret;
        }

        public double FirstDerivativeC44S1VoigtType3()
        {
            double Ret = -5;

            double N = this.C11;
            N -= this.C12;
            N -= this.C13;
            N += this.C22;
            N -= this.C23;
            N += this.C33;
            N += 3 * this.C44;
            N += 3 * this.C55;
            N += 3 * this.C66;

            Ret /= Math.Pow(N, 2);

            return Ret;
        }

        public double FirstDerivativeC55S1VoigtType3()
        {
            double Ret = -5;

            double N = this.C11;
            N -= this.C12;
            N -= this.C13;
            N += this.C22;
            N -= this.C23;
            N += this.C33;
            N += 3 * this.C44;
            N += 3 * this.C55;
            N += 3 * this.C66;

            Ret /= Math.Pow(N, 2);

            return Ret;
        }

        public double FirstDerivativeC66S1VoigtType3()
        {
            double Ret = -5;

            double N = this.C11;
            N -= this.C12;
            N -= this.C13;
            N += this.C22;
            N -= this.C23;
            N += this.C33;
            N += 3 * this.C44;
            N += 3 * this.C55;
            N += 3 * this.C66;

            Ret /= Math.Pow(N, 2);

            return Ret;
        }

        public double FirstDerivativeC11HS2VoigtType3()
        {
            double Ret = -15;

            double N = this.C11 - this.C12 - this.C13 + this.C22 - this.C23 + this.C33 + (3 * this.C44) + (3 * this.C55) + (3 * this.C66);

            Ret /= 2 * Math.Pow(N, 2);

            return Ret;
        }

        public double FirstDerivativeC22HS2VoigtType3()
        {
            double Ret = -15;

            double N = this.C11 - this.C12 - this.C13 + this.C22 - this.C23 + this.C33 + (3 * this.C44) + (3 * this.C55) + (3 * this.C66);

            Ret /= 2 * Math.Pow(N, 2);

            return Ret;
        }

        public double FirstDerivativeC33HS2VoigtType3()
        {
            double Ret = -15;

            double N = this.C11 - this.C12 - this.C13 + this.C22 - this.C23 + this.C33 + (3 * this.C44) + (3 * this.C55) + (3 * this.C66);

            Ret /= 2 * Math.Pow(N, 2);

            return Ret;
        }

        public double FirstDerivativeC12HS2VoigtType3()
        {
            double Ret = 15;

            double N = this.C11 - this.C12 - this.C13 + this.C22 - this.C23 + this.C33 + (3 * this.C44) + (3 * this.C55) + (3 * this.C66);

            Ret /= 2 * Math.Pow(N, 2);

            return Ret;
        }

        public double FirstDerivativeC13HS2VoigtType3()
        {
            double Ret = 15;

            double N = this.C11 - this.C12 - this.C13 + this.C22 - this.C23 + this.C33 + (3 * this.C44) + (3 * this.C55) + (3 * this.C66);

            Ret /= 2 * Math.Pow(N, 2);

            return Ret;
        }

        public double FirstDerivativeC23HS2VoigtType3()
        {
            double Ret = 15;

            double N = this.C11 - this.C12 - this.C13 + this.C22 - this.C23 + this.C33 + (3 * this.C44) + (3 * this.C55) + (3 * this.C66);

            Ret /= 2 * Math.Pow(N, 2);

            return Ret;
        }

        public double FirstDerivativeC44HS2VoigtType3()
        {
            double Ret = -45;

            double N = this.C11 - this.C12 - this.C13 + this.C22 - this.C23 + this.C33 + (3 * this.C44) + (3 * this.C55) + (3 * this.C66);

            Ret /= 2 * Math.Pow(N, 2);

            return Ret;
        }

        public double FirstDerivativeC55HS2VoigtType3()
        {
            double Ret = -45;

            double N = this.C11 - this.C12 - this.C13 + this.C22 - this.C23 + this.C33 + (3 * this.C44) + (3 * this.C55) + (3 * this.C66);

            Ret /= 2 * Math.Pow(N, 2);

            return Ret;
        }

        public double FirstDerivativeC66HS2VoigtType3()
        {
            double Ret = -45;

            double N = this.C11 - this.C12 - this.C13 + this.C22 - this.C23 + this.C33 + (3 * this.C44) + (3 * this.C55) + (3 * this.C66);

            Ret /= 2 * Math.Pow(N, 2);

            return Ret;
        }

        #endregion

        #endregion

        #endregion

        #region Reuss

        #region Cubic

        private double CubicGamma(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Gamma = Math.Pow(hKL.K, 2) * Math.Pow(hKL.L, 2);
            Gamma += Math.Pow(hKL.H, 2) * Math.Pow(hKL.L, 2);
            Gamma += Math.Pow(hKL.H, 2) * Math.Pow(hKL.K, 2);

            double N = Math.Pow(Math.Pow(hKL.H, 2) + Math.Pow(hKL.K, 2) + Math.Pow(hKL.L, 2), 2);

            return Gamma / N;
        }

        public double S1ReussCubic(DataManagment.CrystalData.HKLReflex hKL)
        {
            if (CalScec.Properties.Settings.Default.CubicFittingModel == 1)
            {
                double Gamma = this.CubicGamma(hKL);

                double Ret = this.S12;
                double S0 = this.S11 - this.S12 - (0.5 * this.S44);

                Ret += S0 * Gamma;

                return Ret;
            }
            else if(CalScec.Properties.Settings.Default.CubicFittingModel == 2)
            {
                #region Implementation after Howard and Kisi

                double Sum = Math.Pow(hKL.K, 2) * Math.Pow(hKL.L, 2);
                Sum += Math.Pow(hKL.H, 2) * Math.Pow(hKL.L, 2);
                Sum += Math.Pow(hKL.H, 2) * Math.Pow(hKL.K, 2);
                Sum *= this.S11 - (this.S44 / 2);

                double Sum1 = Math.Pow(hKL.H, 4) + Math.Pow(hKL.K, 4) + Math.Pow(hKL.L, 4);
                Sum1 += Math.Pow(hKL.K, 2) * Math.Pow(hKL.L, 2);
                Sum1 += Math.Pow(hKL.H, 2) * Math.Pow(hKL.L, 2);
                Sum1 += Math.Pow(hKL.H, 2) * Math.Pow(hKL.K, 2);
                Sum1 *= this.S12;

                double N = Math.Pow(Math.Pow(hKL.H, 2) + Math.Pow(hKL.K, 2) + Math.Pow(hKL.L, 2), 2);

                double Ret = Sum + Sum1;
                Ret /= N;

                return Ret;

                #endregion
            }

            return 0;
        }

        public double HS2ReussCubic(DataManagment.CrystalData.HKLReflex hKL)
        {
            if (CalScec.Properties.Settings.Default.CubicFittingModel == 1)
            {
                double Gamma = this.CubicGamma(hKL);

                double Ret = this.S11 - this.S12;
                double S0 = this.S11 - this.S12 - (0.5 * this.S44);

                Ret -= 3 * S0 * Gamma;

                return Ret;
            }
            else if (CalScec.Properties.Settings.Default.CubicFittingModel == 2)
            {
                #region Implementation after Howard and Kisi

                double Sum = Math.Pow(hKL.H, 4) + Math.Pow(hKL.K, 4) + Math.Pow(hKL.L, 4);
                Sum -= Math.Pow(hKL.K, 2) * Math.Pow(hKL.L, 2);
                Sum -= Math.Pow(hKL.H, 2) * Math.Pow(hKL.L, 2);
                Sum -= Math.Pow(hKL.H, 2) * Math.Pow(hKL.K, 2);
                Sum *= this.S11;

                double Sum1 = Math.Pow(hKL.K, 2) * Math.Pow(hKL.L, 2);
                Sum1 += Math.Pow(hKL.H, 2) * Math.Pow(hKL.L, 2);
                Sum1 += Math.Pow(hKL.H, 2) * Math.Pow(hKL.K, 2);
                Sum1 -= (Math.Pow(hKL.H, 4) + Math.Pow(hKL.K, 4) + Math.Pow(hKL.L, 4));
                Sum1 *= this.S12;

                double Sum2 = Math.Pow(hKL.K, 2) * Math.Pow(hKL.L, 2);
                Sum2 += Math.Pow(hKL.H, 2) * Math.Pow(hKL.L, 2);
                Sum2 += Math.Pow(hKL.H, 2) * Math.Pow(hKL.K, 2);
                Sum2 *= 1.5 * this.S44;

                double N = Math.Pow(Math.Pow(hKL.H, 2) + Math.Pow(hKL.K, 2) + Math.Pow(hKL.L, 2), 2);

                double Ret = Sum + Sum1 + Sum2;
                Ret /= N;

                return Ret;

                #endregion
            }

            return 0;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] C11
        ///[1] C12
        ///[2] C44
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorReussCubicClassic(double Lambda)
        {
            //[0][0] C11
            //[1][1] C12
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

            //[0] Aclivity
            //[1] Constant
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);

            for (int n = 0; n < this.DiffractionConstants.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeS11S1ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 0] += (this.FirstDerivativeS11HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeS12S1ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 1] += (this.FirstDerivativeS12HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeS12S1ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeS12HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeS12S1ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeS12HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeS44S1ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS44S1ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 2] += (this.FirstDerivativeS44HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS44HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeS44S1ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeS44HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeS44S1ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeS44HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeS44S1ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeS44HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeS44S1ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeS44HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                #endregion

                #region Vector build

                SolutionVector[0] += ((this.DiffractionConstants[n].ClassicS1 - this.S1ReussCubic(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeS11S1ReussCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[0] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2ReussCubic(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeS11HS2ReussCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[1] += ((this.DiffractionConstants[n].ClassicS1 - this.S1ReussCubic(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeS12S1ReussCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[1] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2ReussCubic(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeS12HS2ReussCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[2] += ((this.DiffractionConstants[n].ClassicS1 - this.S1ReussCubic(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeS44S1ReussCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[2] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2ReussCubic(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeS44HS2ReussCubic(this.DiffractionConstants[n].UsedReflex);

                #endregion
            }

            for (int n = 0; n < 3; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] C11
        ///[1] C12
        ///[2] C44
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorReussCubicMacroscopic(double Lambda)
        {
            //[0][0] C11
            //[1][1] C12
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

            //[0] Aclivity
            //[1] Constant
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);

            for (int n = 0; n < this.DiffractionConstants.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeS11S1ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 0] += (this.FirstDerivativeS11HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeS12S1ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 1] += (this.FirstDerivativeS12HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeS12S1ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeS12HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeS12S1ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeS12HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeS44S1ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS44S1ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 2] += (this.FirstDerivativeS44HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS44HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeS44S1ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeS44HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeS44S1ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeS44HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeS44S1ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeS44HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeS44S1ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeS44HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2ReussCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                #endregion

                #region Vector build

                SolutionVector[0] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1ReussCubic(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeS11S1ReussCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[0] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2ReussCubic(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeS11HS2ReussCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[1] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1ReussCubic(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeS12S1ReussCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[1] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2ReussCubic(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeS12HS2ReussCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[2] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1ReussCubic(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeS44S1ReussCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[2] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2ReussCubic(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeS44HS2ReussCubic(this.DiffractionConstants[n].UsedReflex);

                #endregion
            }

            for (int n = 0; n < 3; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        #region First derivative

        public double FirstDerivativeS11S1ReussCubic(DataManagment.CrystalData.HKLReflex hKL)
        {
            if (CalScec.Properties.Settings.Default.CubicFittingModel == 1)
            {
                double Gamma = this.CubicGamma(hKL);

                double Ret = 1;

                Ret *= Gamma;

                return Ret;
            }
            else if (CalScec.Properties.Settings.Default.CubicFittingModel == 2)
            {
                #region Implementation after Howard and Kisi

                double Ret = Math.Pow(hKL.K, 2) * Math.Pow(hKL.L, 2);
                Ret += Math.Pow(hKL.H, 2) * Math.Pow(hKL.L, 2);
                Ret += Math.Pow(hKL.H, 2) * Math.Pow(hKL.K, 2);

                double N = Math.Pow(Math.Pow(hKL.H, 2) + Math.Pow(hKL.K, 2) + Math.Pow(hKL.L, 2), 2);

                Ret /= N;

                return Ret;

                #endregion
            }

            return 0;
        }

        public double FirstDerivativeS12S1ReussCubic(DataManagment.CrystalData.HKLReflex hKL)
        {
            if (CalScec.Properties.Settings.Default.CubicFittingModel == 1)
            {
                double Gamma = this.CubicGamma(hKL);

                double Ret = 1;

                Ret -= Gamma;

                return Ret;
            }
            else if (CalScec.Properties.Settings.Default.CubicFittingModel == 2)
            {
                #region Implementation after Howard and Kisi

                double Ret = Math.Pow(hKL.H, 4) + Math.Pow(hKL.K, 4) + Math.Pow(hKL.L, 4);
                Ret += Math.Pow(hKL.K, 2) * Math.Pow(hKL.L, 2);
                Ret += Math.Pow(hKL.H, 2) * Math.Pow(hKL.L, 2);
                Ret += Math.Pow(hKL.H, 2) * Math.Pow(hKL.K, 2);

                double N = Math.Pow(Math.Pow(hKL.H, 2) + Math.Pow(hKL.K, 2) + Math.Pow(hKL.L, 2), 2);

                Ret /= N;

                return Ret;

                #endregion
            }

            return 0;
        }

        public double FirstDerivativeS44S1ReussCubic(DataManagment.CrystalData.HKLReflex hKL)
        {
            if (CalScec.Properties.Settings.Default.CubicFittingModel == 1)
            {
                double Gamma = this.CubicGamma(hKL);

                double Ret = -0.5;

                Ret *= Gamma;

                return Ret;
            }
            else if (CalScec.Properties.Settings.Default.CubicFittingModel == 2)
            {
                #region Implementation after Howard and Kisi

                double Ret = -Math.Pow(hKL.K, 2) * Math.Pow(hKL.L, 2);
                Ret -= Math.Pow(hKL.H, 2) * Math.Pow(hKL.L, 2);
                Ret -= Math.Pow(hKL.H, 2) * Math.Pow(hKL.K, 2);

                double N = Math.Pow(Math.Pow(hKL.H, 2) + Math.Pow(hKL.K, 2) + Math.Pow(hKL.L, 2), 2);

                Ret /= 2 * N;

                return Ret;

                #endregion
            }

            return 0;
        }

        public double FirstDerivativeS11HS2ReussCubic(DataManagment.CrystalData.HKLReflex hKL)
        {
            if (CalScec.Properties.Settings.Default.CubicFittingModel == 1)
            {
                double Gamma = this.CubicGamma(hKL);

                double Ret = 1;
                double S0 = 1;

                Ret -= 3 * S0 * Gamma;

                return Ret;
            }
            else if (CalScec.Properties.Settings.Default.CubicFittingModel == 2)
            {
                #region Implementation after Howard and Kisi

                double Sum = Math.Pow(hKL.H, 4) + Math.Pow(hKL.K, 4) + Math.Pow(hKL.L, 4);
                Sum -= Math.Pow(hKL.K, 2) * Math.Pow(hKL.L, 2);
                Sum -= Math.Pow(hKL.H, 2) * Math.Pow(hKL.L, 2);
                Sum -= Math.Pow(hKL.H, 2) * Math.Pow(hKL.K, 2);

                double N = Math.Pow(Math.Pow(hKL.H, 2) + Math.Pow(hKL.K, 2) + Math.Pow(hKL.L, 2), 2);

                double Ret = Sum;
                Ret /= N;

                return Ret;

                #endregion
            }

            return 0;
        }

        public double FirstDerivativeS12HS2ReussCubic(DataManagment.CrystalData.HKLReflex hKL)
        {
            if (CalScec.Properties.Settings.Default.CubicFittingModel == 1)
            {
                double Gamma = this.CubicGamma(hKL);

                double Ret = -1;
                double S0 = -1;

                Ret -= 3 * S0 * Gamma;

                return Ret;
            }
            else if (CalScec.Properties.Settings.Default.CubicFittingModel == 2)
            {
                #region Implementation after Howard and Kisi

                double Sum1 = Math.Pow(hKL.K, 2) * Math.Pow(hKL.L, 2);
                Sum1 += Math.Pow(hKL.H, 2) * Math.Pow(hKL.L, 2);
                Sum1 += Math.Pow(hKL.H, 2) * Math.Pow(hKL.K, 2);
                Sum1 -= (Math.Pow(hKL.H, 4) + Math.Pow(hKL.K, 4) + Math.Pow(hKL.L, 4));

                double N = Math.Pow(Math.Pow(hKL.H, 2) + Math.Pow(hKL.K, 2) + Math.Pow(hKL.L, 2), 2);

                double Ret = Sum1;
                Ret /= N;

                return Ret;

                #endregion
            }

            return 0;
        }

        public double FirstDerivativeS44HS2ReussCubic(DataManagment.CrystalData.HKLReflex hKL)
        {
            if (CalScec.Properties.Settings.Default.CubicFittingModel == 1)
            {
                double Gamma = this.CubicGamma(hKL);

                double Ret = 0;
                double S0 = -0.5;

                Ret -= 3 * S0 * Gamma;

                return Ret;
            }
            else if (CalScec.Properties.Settings.Default.CubicFittingModel == 2)
            {
                #region Implementation after Howard and Kisi

                double Sum2 = Math.Pow(hKL.K, 2) * Math.Pow(hKL.L, 2);
                Sum2 += Math.Pow(hKL.H, 2) * Math.Pow(hKL.L, 2);
                Sum2 += Math.Pow(hKL.H, 2) * Math.Pow(hKL.K, 2);
                Sum2 *= 1.5;

                double N = Math.Pow(Math.Pow(hKL.H, 2) + Math.Pow(hKL.K, 2) + Math.Pow(hKL.L, 2), 2);

                double Ret = Sum2;
                Ret /= N;

                return Ret;

                #endregion
            }

            return 0;
        }

        #endregion

        #endregion

        #region Hexagonal

        public double S1ReussHexagonal(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = Math.Pow(hKL.H, 2) + (hKL.H * hKL.K) + Math.Pow(hKL.K, 2);
            Ret *= 6 * Math.Pow(hKL.L, 2);
            Ret *= this.S11 + this.S33 - this.S44;

            double Sum = Math.Pow(hKL.H, 2) + (hKL.H * hKL.K) + Math.Pow(hKL.K, 2);
            Sum *= ( 4 * Math.Pow(hKL.H, 2)) + (4 * (hKL.H * hKL.K)) + ( 4 * Math.Pow(hKL.K, 2)) + (3 * Math.Pow(hKL.L, 2));

            Ret += 2 * this.S12 * Sum;

            Sum = 8 * Math.Pow(hKL.H, 4);
            Sum += 16 * Math.Pow(hKL.H, 3) * hKL.K;
            Sum += 24 * Math.Pow(hKL.H, 2) * Math.Pow(hKL.K, 2);
            Sum += 16 * hKL.H * Math.Pow(hKL.K, 3);
            Sum += 8 * Math.Pow(hKL.K, 4);
            Sum += 6 * Math.Pow(hKL.H, 2) * Math.Pow(hKL.L, 2);
            Sum += 6 * hKL.H * hKL.K * Math.Pow(hKL.L, 2);
            Sum += 6 * Math.Pow(hKL.K, 2) * Math.Pow(hKL.L, 2);
            Sum += 9 * Math.Pow(hKL.L, 4);

            Ret += Sum * this.S13;

            double N = 4 * Math.Pow(hKL.H, 2);
            N += 4 * Math.Pow(hKL.K, 2);
            N += 4 * Math.Pow(hKL.L, 2);
            N += 4 * hKL.H * hKL.K;

            Ret /= Math.Pow(N, 2);

            return Ret;
        }

        public double HS2ReussHexagonal(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = Math.Pow(Math.Pow(hKL.H, 2) + (hKL.H * hKL.K) + Math.Pow(hKL.K, 2), 2);
            Ret *= 16 * this.S11;
            Ret += 9 * Math.Pow(hKL.L, 4) * this.S33;

            double Sum = Math.Pow(hKL.H, 2) + (hKL.H * hKL.K) + Math.Pow(hKL.K, 2);
            Sum *= 12 * Math.Pow(hKL.L, 2);
            Sum *= ((2 * this.S13) + this.S44);

            Ret += Sum;

            double N = 4 * Math.Pow(hKL.H, 2);
            N += 4 * Math.Pow(hKL.K, 2);
            N += 4 * Math.Pow(hKL.L, 2);
            N += 4 * hKL.H * hKL.K;

            Ret /= Math.Pow(N, 2);

            Ret -= this.S1ReussHexagonal(hKL);

            return Ret;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] C11
        ///[1] C33
        ///[2] C12
        ///[3] C13
        ///[4] C44
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorReussHexagonalClassic(double Lambda)
        {
            //[0][0] C11
            //[1][1] C12
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(5, 5, 0.0);

            //[0] Aclivity
            //[1] Constant
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(5);

            for (int n = 0; n < this.DiffractionConstants.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeS11S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 0] += (this.FirstDerivativeS11HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeS33S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 1] += (this.FirstDerivativeS33HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeS33S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeS33HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeS33S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeS33HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeS12S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 2] += (this.FirstDerivativeS12HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeS12S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeS12HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeS12S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeS12HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeS12S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeS12HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeS12S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeS12HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[3, 3] += (this.FirstDerivativeS13S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 3] += (this.FirstDerivativeS13HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 3] += (this.FirstDerivativeS13S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 3] += (this.FirstDerivativeS13HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[3, 0] += (this.FirstDerivativeS13S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 0] += (this.FirstDerivativeS13HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 3] += (this.FirstDerivativeS13S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 3] += (this.FirstDerivativeS13HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[3, 1] += (this.FirstDerivativeS13S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 1] += (this.FirstDerivativeS13HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 3] += (this.FirstDerivativeS13S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 3] += (this.FirstDerivativeS13HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[3, 2] += (this.FirstDerivativeS13S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 2] += (this.FirstDerivativeS13HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                
                HessianMatrix[4, 4] += (this.FirstDerivativeS44S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS44S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 4] += (this.FirstDerivativeS44HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS44HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 4] += (this.FirstDerivativeS44S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 4] += (this.FirstDerivativeS44HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[4, 0] += (this.FirstDerivativeS44S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 0] += (this.FirstDerivativeS44HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 4] += (this.FirstDerivativeS44S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 4] += (this.FirstDerivativeS44HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[4, 1] += (this.FirstDerivativeS44S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 1] += (this.FirstDerivativeS44HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 4] += (this.FirstDerivativeS44S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 4] += (this.FirstDerivativeS44HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[4, 2] += (this.FirstDerivativeS44S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 2] += (this.FirstDerivativeS44HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[3, 4] += (this.FirstDerivativeS44S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 4] += (this.FirstDerivativeS44HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[4, 3] += (this.FirstDerivativeS44S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 3] += (this.FirstDerivativeS44HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                #endregion

                #region Vector build

                SolutionVector[0] += ((this.DiffractionConstants[n].ClassicS1 - this.S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeS11S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[0] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeS11HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[1] += ((this.DiffractionConstants[n].ClassicS1 - this.S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeS33S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[1] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeS33HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[2] += ((this.DiffractionConstants[n].ClassicS1 - this.S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeS12S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[2] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeS12HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[3] += ((this.DiffractionConstants[n].ClassicS1 - this.S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeS13S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[3] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeS13HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[4] += ((this.DiffractionConstants[n].ClassicS1 - this.S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeS44S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[4] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeS44HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex);

                #endregion
            }

            for (int n = 0; n < 5; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(5);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] C11
        ///[1] C33
        ///[2] C12
        ///[3] C13
        ///[4] C44
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorReussHexagonalMacroscopic(double Lambda)
        {
            //[0][0] C11
            //[1][1] C12
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(5, 5, 0.0);

            //[0] Aclivity
            //[1] Constant
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(5);

            for (int n = 0; n < this.DiffractionConstants.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeS11S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 0] += (this.FirstDerivativeS11HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeS33S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 1] += (this.FirstDerivativeS33HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeS33S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeS33HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeS33S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeS33HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeS12S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 2] += (this.FirstDerivativeS12HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeS12S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeS12HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeS12S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeS12HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeS12S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeS12HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeS12S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeS12HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[3, 3] += (this.FirstDerivativeS13S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 3] += (this.FirstDerivativeS13HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 3] += (this.FirstDerivativeS13S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 3] += (this.FirstDerivativeS13HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[3, 0] += (this.FirstDerivativeS13S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 0] += (this.FirstDerivativeS13HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 3] += (this.FirstDerivativeS13S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 3] += (this.FirstDerivativeS13HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[3, 1] += (this.FirstDerivativeS13S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 1] += (this.FirstDerivativeS13HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 3] += (this.FirstDerivativeS13S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 3] += (this.FirstDerivativeS13HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[3, 2] += (this.FirstDerivativeS13S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 2] += (this.FirstDerivativeS13HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[4, 4] += (this.FirstDerivativeS44S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS44S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 4] += (this.FirstDerivativeS44HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS44HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 4] += (this.FirstDerivativeS44S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 4] += (this.FirstDerivativeS44HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[4, 0] += (this.FirstDerivativeS44S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 0] += (this.FirstDerivativeS44HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 4] += (this.FirstDerivativeS44S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 4] += (this.FirstDerivativeS44HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[4, 1] += (this.FirstDerivativeS44S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 1] += (this.FirstDerivativeS44HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 4] += (this.FirstDerivativeS44S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 4] += (this.FirstDerivativeS44HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[4, 2] += (this.FirstDerivativeS44S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 2] += (this.FirstDerivativeS44HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[3, 4] += (this.FirstDerivativeS44S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 4] += (this.FirstDerivativeS44HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[4, 3] += (this.FirstDerivativeS44S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 3] += (this.FirstDerivativeS44HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                #endregion

                #region Vector build

                SolutionVector[0] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeS11S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[0] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeS11HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[1] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeS33S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[1] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeS33HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[2] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeS12S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[2] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeS12HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[3] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeS13S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[3] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeS13HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[4] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeS44S1ReussHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[4] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeS44HS2ReussHexagonal(this.DiffractionConstants[n].UsedReflex);

                #endregion
            }

            for (int n = 0; n < 5; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(5);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        #region First derivative

        public double FirstDerivativeS11S1ReussHexagonal(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = Math.Pow(hKL.H, 2) + (hKL.H * hKL.K) + Math.Pow(hKL.K, 2);
            Ret *= 6 * Math.Pow(hKL.L, 2);

            double N = 4 * Math.Pow(hKL.H, 2);
            N += 4 * Math.Pow(hKL.K, 2);
            N += 4 * Math.Pow(hKL.L, 2);
            N += 4 * hKL.H * hKL.K;

            Ret /= Math.Pow(N, 2);

            return Ret;
        }

        public double FirstDerivativeS33S1ReussHexagonal(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = Math.Pow(hKL.H, 2) + (hKL.H * hKL.K) + Math.Pow(hKL.K, 2);
            Ret *= 6 * Math.Pow(hKL.L, 2);

            double N = 4 * Math.Pow(hKL.H, 2);
            N += 4 * Math.Pow(hKL.K, 2);
            N += 4 * Math.Pow(hKL.L, 2);
            N += 4 * hKL.H * hKL.K;

            Ret /= Math.Pow(N, 2);

            return Ret;
        }

        public double FirstDerivativeS44S1ReussHexagonal(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = Math.Pow(hKL.H, 2) + (hKL.H * hKL.K) + Math.Pow(hKL.K, 2);
            Ret *= -6 * Math.Pow(hKL.L, 2);

            double N = 4 * Math.Pow(hKL.H, 2);
            N += 4 * Math.Pow(hKL.K, 2);
            N += 4 * Math.Pow(hKL.L, 2);
            N += 4 * hKL.H * hKL.K;

            Ret /= Math.Pow(N, 2);

            return Ret;
        }

        public double FirstDerivativeS12S1ReussHexagonal(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = Math.Pow(hKL.H, 2) + (hKL.H * hKL.K) + Math.Pow(hKL.K, 2);
            Ret *= (4 * Math.Pow(hKL.H, 2)) + (4 * (hKL.H * hKL.K)) + (4 * Math.Pow(hKL.K, 2)) + (3 * Math.Pow(hKL.L, 2));
            Ret *= 2;

            double N = 4 * Math.Pow(hKL.H, 2);
            N += 4 * Math.Pow(hKL.K, 2);
            N += 4 * Math.Pow(hKL.L, 2);
            N += 4 * hKL.H * hKL.K;

            Ret /= Math.Pow(N, 2);

            return Ret;
        }

        public double FirstDerivativeS13S1ReussHexagonal(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = 8 * Math.Pow(hKL.H, 4);
            Ret += 16 * Math.Pow(hKL.H, 3) * hKL.K;
            Ret += 24 * Math.Pow(hKL.H, 2) * Math.Pow(hKL.K, 2);
            Ret += 16 * hKL.H * Math.Pow(hKL.K, 3);
            Ret += 8 * Math.Pow(hKL.K, 4);
            Ret += 6 * Math.Pow(hKL.H, 2) * Math.Pow(hKL.L, 2);
            Ret += 6 * hKL.H * hKL.K * Math.Pow(hKL.L, 2);
            Ret += 6 * Math.Pow(hKL.K, 2) * Math.Pow(hKL.L, 2);
            Ret += 9 * Math.Pow(hKL.L, 4);

            double N = 4 * Math.Pow(hKL.H, 2);
            N += 4 * Math.Pow(hKL.K, 2);
            N += 4 * Math.Pow(hKL.L, 2);
            N += 4 * hKL.H * hKL.K;

            Ret /= Math.Pow(N, 2);

            return Ret;
        }

        public double FirstDerivativeS11HS2ReussHexagonal(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = Math.Pow(Math.Pow(hKL.H, 2) + (hKL.H * hKL.K) + Math.Pow(hKL.K, 2), 2);
            Ret *= 16;

            double N = 4 * Math.Pow(hKL.H, 2);
            N += 4 * Math.Pow(hKL.K, 2);
            N += 4 * Math.Pow(hKL.L, 2);
            N += 4 * hKL.H * hKL.K;

            Ret /= Math.Pow(N, 2);

            Ret -= FirstDerivativeS11S1ReussHexagonal(hKL);

            return Ret;
        }

        public double FirstDerivativeS33HS2ReussHexagonal(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = 9 * Math.Pow(hKL.L, 4);

            double N = 4 * Math.Pow(hKL.H, 2);
            N += 4 * Math.Pow(hKL.K, 2);
            N += 4 * Math.Pow(hKL.L, 2);
            N += 4 * hKL.H * hKL.K;

            Ret /= Math.Pow(N, 2);

            Ret -= FirstDerivativeS33S1ReussHexagonal(hKL);

            return Ret;
        }

        public double FirstDerivativeS12HS2ReussHexagonal(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = -1 * FirstDerivativeS12S1ReussHexagonal(hKL);

            return Ret;
        }

        public double FirstDerivativeS13HS2ReussHexagonal(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = Math.Pow(hKL.H, 2) + (hKL.H * hKL.K) + Math.Pow(hKL.K, 2);
            Ret *= 24.0 * Math.Pow(hKL.L, 2);

            double N = 4 * Math.Pow(hKL.H, 2);
            N += 4 * Math.Pow(hKL.K, 2);
            N += 4 * Math.Pow(hKL.L, 2);
            N += 4 * hKL.H * hKL.K;

            Ret /= Math.Pow(N, 2);

            Ret -= FirstDerivativeS13S1ReussHexagonal(hKL);

            return Ret;
        }

        public double FirstDerivativeS44HS2ReussHexagonal(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = Math.Pow(hKL.H, 2) + (hKL.H * hKL.K) + Math.Pow(hKL.K, 2);
            Ret *= 12 * Math.Pow(hKL.L, 2);

            double N = 4 * Math.Pow(hKL.H, 2);
            N += 4 * Math.Pow(hKL.K, 2);
            N += 4 * Math.Pow(hKL.L, 2);
            N += 4 * hKL.H * hKL.K;

            Ret /= Math.Pow(N, 2);

            Ret -= FirstDerivativeS44S1ReussHexagonal(hKL);

            return Ret;
        }

        #endregion

        #endregion

        #endregion

        #region Hill

        #region Cubic

        public double S1HillCubic(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = this.S1ReussCubic(hKL) + this.S1VoigtCubicCompliance();
            Ret /= 2.0;

            return Ret;
        }

        public double HS2HillCubic(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = this.HS2ReussCubic(hKL) + this.HS2VoigtCubicCompliance();
            Ret /= 2.0;

            return Ret;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] C11
        ///[1] C12
        ///[2] C44
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorHillCubicClassic(double Lambda)
        {
            //[0][0] C11
            //[1][1] C12
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

            //[0] Aclivity
            //[1] Constant
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);

            for (int n = 0; n < this.DiffractionConstants.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeS11S1HillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1HillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 0] += (this.FirstDerivativeS11HS2HillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2HillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeS12S1HillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1HillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 1] += (this.FirstDerivativeS12HS2HillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2HillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeS12S1HillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1HillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeS12HS2HillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2HillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeS12S1HillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1HillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeS12HS2HillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2HillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeS44S1HillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS44S1HillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 2] += (this.FirstDerivativeS44HS2HillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS44HS2HillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeS44S1HillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1HillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeS44HS2HillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2HillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeS44S1HillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1HillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeS44HS2HillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2HillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeS44S1HillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1HillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeS44HS2HillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2HillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeS44S1HillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1HillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeS44HS2HillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2HillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                #endregion

                #region Vector build

                SolutionVector[0] += ((this.DiffractionConstants[n].ClassicS1 - this.S1HillCubic(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeS11S1HillCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[0] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2HillCubic(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeS11HS2HillCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[1] += ((this.DiffractionConstants[n].ClassicS1 - this.S1HillCubic(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeS12S1HillCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[1] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2HillCubic(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeS12HS2HillCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[2] += ((this.DiffractionConstants[n].ClassicS1 - this.S1HillCubic(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeS44S1HillCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[2] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2HillCubic(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeS44HS2HillCubic(this.DiffractionConstants[n].UsedReflex);

                #endregion
            }

            for (int n = 0; n < 3; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] C11
        ///[1] C12
        ///[2] C44
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorHillCubicMacroscopic(double Lambda)
        {
            //[0][0] C11
            //[1][1] C12
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

            //[0] Aclivity
            //[1] Constant
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);

            for (int n = 0; n < this.DiffractionConstants.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeS11S1HillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1HillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 0] += (this.FirstDerivativeS11HS2HillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2HillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeS12S1HillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1HillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 1] += (this.FirstDerivativeS12HS2HillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2HillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeS12S1HillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1HillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeS12HS2HillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2HillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeS12S1HillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1HillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeS12HS2HillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2HillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeS44S1HillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS44S1HillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 2] += (this.FirstDerivativeS44HS2HillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS44HS2HillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeS44S1HillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1HillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeS44HS2HillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2HillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeS44S1HillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1HillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeS44HS2HillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2HillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeS44S1HillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1HillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeS44HS2HillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2HillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeS44S1HillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1HillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeS44HS2HillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2HillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                #endregion

                #region Vector build

                SolutionVector[0] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1HillCubic(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeS11S1HillCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[0] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2HillCubic(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeS11HS2HillCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[1] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1HillCubic(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeS12S1HillCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[1] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2HillCubic(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeS12HS2HillCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[2] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1HillCubic(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeS44S1HillCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[2] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2HillCubic(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeS44HS2HillCubic(this.DiffractionConstants[n].UsedReflex);

                #endregion
            }

            for (int n = 0; n < 3; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        #region First derivative

        public double FirstDerivativeS11S1HillCubic(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = this.FirstDerivativeS11S1ReussCubic(hKL) + this.FirstDerivativeS11S1VoigtCubic();
            Ret /= 2.0;

            return Ret;
        }

        public double FirstDerivativeS12S1HillCubic(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = this.FirstDerivativeS12S1ReussCubic(hKL) + this.FirstDerivativeS12S1VoigtCubic();
            Ret /= 2.0;

            return Ret;
        }

        public double FirstDerivativeS44S1HillCubic(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = this.FirstDerivativeS44S1ReussCubic(hKL) + this.FirstDerivativeS44S1VoigtCubic();
            Ret /= 2.0;

            return Ret;
        }

        public double FirstDerivativeS11HS2HillCubic(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = this.FirstDerivativeS11HS2ReussCubic(hKL) + this.FirstDerivativeS11HS2VoigtCubic();
            Ret /= 2.0;

            return Ret;
        }

        public double FirstDerivativeS12HS2HillCubic(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = this.FirstDerivativeS12HS2ReussCubic(hKL) + this.FirstDerivativeS12HS2VoigtCubic();
            Ret /= 2.0;

            return Ret;
        }

        public double FirstDerivativeS44HS2HillCubic(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = this.FirstDerivativeS44HS2ReussCubic(hKL) + this.FirstDerivativeS44HS2VoigtCubic();
            Ret /= 2.0;

            return Ret;
        }

        #endregion

        #endregion

        #region Hexagonal

        public double S1HillHexagonal(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = this.S1ReussHexagonal(hKL) + this.S1VoigtType1Compliance();
            Ret /= 2;

            return Ret;
        }

        public double HS2HillHexagonal(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = this.HS2ReussHexagonal(hKL) + this.HS2VoigtType1Compliance();
            Ret /= 2;

            return Ret;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] C11
        ///[1] C33
        ///[2] C12
        ///[3] C13
        ///[4] C44
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorHillHexagonalClassic(double Lambda)
        {
            //[0][0] C11
            //[1][1] C12
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(5, 5, 0.0);

            //[0] Aclivity
            //[1] Constant
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(5);

            for (int n = 0; n < this.DiffractionConstants.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeS11S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 0] += (this.FirstDerivativeS11HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeS33S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 1] += (this.FirstDerivativeS33HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeS33S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeS33HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeS33S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeS33HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeS12S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 2] += (this.FirstDerivativeS12HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeS12S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeS12HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeS12S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeS12HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeS12S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeS12HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeS12S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeS12HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[3, 3] += (this.FirstDerivativeS13S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 3] += (this.FirstDerivativeS13HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 3] += (this.FirstDerivativeS13S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 3] += (this.FirstDerivativeS13HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[3, 0] += (this.FirstDerivativeS13S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 0] += (this.FirstDerivativeS13HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 3] += (this.FirstDerivativeS13S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 3] += (this.FirstDerivativeS13HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[3, 1] += (this.FirstDerivativeS13S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 1] += (this.FirstDerivativeS13HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 3] += (this.FirstDerivativeS13S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 3] += (this.FirstDerivativeS13HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[3, 2] += (this.FirstDerivativeS13S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 2] += (this.FirstDerivativeS13HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[4, 4] += (this.FirstDerivativeS44S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS44S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 4] += (this.FirstDerivativeS44HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS44HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 4] += (this.FirstDerivativeS44S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 4] += (this.FirstDerivativeS44HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[4, 0] += (this.FirstDerivativeS44S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 0] += (this.FirstDerivativeS44HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 4] += (this.FirstDerivativeS44S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 4] += (this.FirstDerivativeS44HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[4, 1] += (this.FirstDerivativeS44S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 1] += (this.FirstDerivativeS44HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 4] += (this.FirstDerivativeS44S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 4] += (this.FirstDerivativeS44HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[4, 2] += (this.FirstDerivativeS44S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 2] += (this.FirstDerivativeS44HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[3, 4] += (this.FirstDerivativeS44S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 4] += (this.FirstDerivativeS44HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[4, 3] += (this.FirstDerivativeS44S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 3] += (this.FirstDerivativeS44HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                #endregion

                #region Vector build

                SolutionVector[0] += ((this.DiffractionConstants[n].ClassicS1 - this.S1HillHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeS11S1HillHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[0] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeS11HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[1] += ((this.DiffractionConstants[n].ClassicS1 - this.S1HillHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeS33S1HillHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[1] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeS33HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[2] += ((this.DiffractionConstants[n].ClassicS1 - this.S1HillHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeS12S1HillHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[2] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeS12HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[3] += ((this.DiffractionConstants[n].ClassicS1 - this.S1HillHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeS13S1HillHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[3] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeS13HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[4] += ((this.DiffractionConstants[n].ClassicS1 - this.S1HillHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeS44S1HillHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[4] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeS44HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex);

                #endregion
            }

            for (int n = 0; n < 5; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(5);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] C11
        ///[1] C33
        ///[2] C12
        ///[3] C13
        ///[4] C44
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorHillHexagonalMacroscopic(double Lambda)
        {
            //[0][0] C11
            //[1][1] C12
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(5, 5, 0.0);

            //[0] Aclivity
            //[1] Constant
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(5);

            for (int n = 0; n < this.DiffractionConstants.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeS11S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 0] += (this.FirstDerivativeS11HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeS33S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 1] += (this.FirstDerivativeS33HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeS33S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeS33HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeS33S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeS33HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeS12S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 2] += (this.FirstDerivativeS12HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeS12S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeS12HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeS12S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeS12HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeS12S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeS12HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeS12S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeS12HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[3, 3] += (this.FirstDerivativeS13S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 3] += (this.FirstDerivativeS13HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 3] += (this.FirstDerivativeS13S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 3] += (this.FirstDerivativeS13HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[3, 0] += (this.FirstDerivativeS13S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 0] += (this.FirstDerivativeS13HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 3] += (this.FirstDerivativeS13S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 3] += (this.FirstDerivativeS13HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[3, 1] += (this.FirstDerivativeS13S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 1] += (this.FirstDerivativeS13HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 3] += (this.FirstDerivativeS13S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 3] += (this.FirstDerivativeS13HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[3, 2] += (this.FirstDerivativeS13S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 2] += (this.FirstDerivativeS13HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[4, 4] += (this.FirstDerivativeS44S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS44S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 4] += (this.FirstDerivativeS44HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS44HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 4] += (this.FirstDerivativeS44S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 4] += (this.FirstDerivativeS44HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[4, 0] += (this.FirstDerivativeS44S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 0] += (this.FirstDerivativeS44HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 4] += (this.FirstDerivativeS44S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 4] += (this.FirstDerivativeS44HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[4, 1] += (this.FirstDerivativeS44S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 1] += (this.FirstDerivativeS44HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 4] += (this.FirstDerivativeS44S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 4] += (this.FirstDerivativeS44HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[4, 2] += (this.FirstDerivativeS44S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 2] += (this.FirstDerivativeS44HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[3, 4] += (this.FirstDerivativeS44S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 4] += (this.FirstDerivativeS44HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[4, 3] += (this.FirstDerivativeS44S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13S1HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 3] += (this.FirstDerivativeS44HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                #endregion

                #region Vector build

                SolutionVector[0] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1HillHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeS11S1HillHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[0] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeS11HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[1] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1HillHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeS33S1HillHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[1] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeS33HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[2] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1HillHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeS12S1HillHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[2] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeS12HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[3] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1HillHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeS13S1HillHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[3] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeS13HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[4] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1HillHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeS44S1HillHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[4] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeS44HS2HillHexagonal(this.DiffractionConstants[n].UsedReflex);

                #endregion
            }

            for (int n = 0; n < 5; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(5);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        #region First derivative

        public double FirstDerivativeS11S1HillHexagonal(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = this.FirstDerivativeS11S1ReussHexagonal(hKL) + this.FirstDerivativeS11S1VoigtType1();
            Ret /= 2;

            return Ret;
        }

        public double FirstDerivativeS33S1HillHexagonal(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = this.FirstDerivativeS33S1ReussHexagonal(hKL) + this.FirstDerivativeS33S1VoigtType1();
            Ret /= 2;

            return Ret;
        }

        public double FirstDerivativeS44S1HillHexagonal(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = this.FirstDerivativeS44S1ReussHexagonal(hKL) + this.FirstDerivativeS44S1VoigtType1();
            Ret /= 2;

            return Ret;
        }

        public double FirstDerivativeS12S1HillHexagonal(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = this.FirstDerivativeS12S1ReussHexagonal(hKL) + this.FirstDerivativeS12S1VoigtType1();
            Ret /= 2;

            return Ret;
        }

        public double FirstDerivativeS13S1HillHexagonal(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = this.FirstDerivativeS13S1ReussHexagonal(hKL) + this.FirstDerivativeS13S1VoigtType1();
            Ret /= 2;

            return Ret;
        }

        public double FirstDerivativeS11HS2HillHexagonal(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = this.FirstDerivativeS11HS2ReussHexagonal(hKL) + this.FirstDerivativeS11HS2VoigtType1();
            Ret /= 2;

            return Ret;
        }

        public double FirstDerivativeS33HS2HillHexagonal(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = this.FirstDerivativeS33HS2ReussHexagonal(hKL) + this.FirstDerivativeS33HS2VoigtType1();
            Ret /= 2;

            return Ret;
        }

        public double FirstDerivativeS12HS2HillHexagonal(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = this.FirstDerivativeS12HS2ReussHexagonal(hKL) + this.FirstDerivativeS12HS2VoigtType1();
            Ret /= 2;

            return Ret;
        }

        public double FirstDerivativeS13HS2HillHexagonal(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = this.FirstDerivativeS13HS2ReussHexagonal(hKL) + this.FirstDerivativeS13HS2VoigtType1();
            Ret /= 2;

            return Ret;
        }

        public double FirstDerivativeS44HS2HillHexagonal(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = this.FirstDerivativeS44HS2ReussHexagonal(hKL) + this.FirstDerivativeS44HS2VoigtType1();
            Ret /= 2;

            return Ret;
        }

        #endregion

        #endregion

        #endregion

        #region Kroener and DeWitt

        #region Cubic

        /// <summary>
        /// Calculates S1 if given an Shearmodulus and kappa
        /// Can be used as First derivative, too. With kappa as First derivaitve and Shear First derivative
        /// </summary>
        /// <param name="kappa"></param>
        /// <param name="ShearModulus"></param>
        /// <returns>S1</returns>
        private double S1KroenerDeWittCubic(double kappa, double ShearModulus)
        {
            #region Linear

            //double Ret = kappa / 9.0;
            //Ret += ((-1.0 * ShearModulus) / 6.0);

            //return Ret;

            #endregion

            #region inverse

            double Ret = 1.0 / (9.0 * kappa);
            Ret += (-1.0 / (6.0 * ShearModulus));

            return Ret;

            #endregion
        }

        private double HS2KroenerDeWittCubic(double ShearModulus)
        {
            //double Ret = 1.0 / ShearModulus;
            double Ret = 0.5 / ShearModulus;

            return Ret;
        }

        public double GetGammaParameterKroenerDeWitt(double kappa, double mu1, double mu2)
        {
            double Ret = kappa * mu1 * mu2;
            Ret *= 3.0 / 4.0;

            return Ret;
        }

        public double GetAlpha2KroenerDeWitt(double mu1, double mu2)
        {
            double Ret = 2.0 * mu1;
            Ret += 3 * mu2;

            Ret *= 1.0 / 5.0;

            return Ret;
        }

        public double GetBeta2KroenerDeWitt(double kappa, double mu1, double mu2)
        {
            double Ret = 6.0 * kappa * mu1;
            Ret += 9.0 * kappa * mu2;
            Ret += 20.0 * mu1 * mu2;

            Ret *= 3.0 / 40.0;

            return Ret;
        }

        #region First derivatives

        public double FirstDerivativeGammaParameterKroenerDeWitt(double kappa, double mu1, double mu2, double kappaFD, double mu1FD, double mu2FD)
        {
            double Ret = kappaFD * mu1 * mu2;
            Ret += kappa * mu1FD * mu2;
            Ret += kappa * mu1 * mu2FD;

            Ret *= 3.0 / 4.0;

            return Ret;
        }

        public double FirstDerivativeBeta2KroenerDeWitt(double kappa, double mu1, double mu2, double kappaFD, double mu1FD, double mu2FD)
        {
            double Sum1 = kappaFD * mu1;
            Sum1 += kappa * mu1FD;
            Sum1 *= 6.0;

            double Sum2 = kappaFD * mu2;
            Sum2 += kappa * mu2FD;
            Sum2 *= 9.0;

            double Sum3 = mu1FD * mu2;
            Sum3 += mu1 * mu2FD;
            Sum3 *= 20.0;

            double Ret = Sum1 + Sum2 + Sum3;

            Ret *= 3.0 / 40.0;

            return Ret;
        }

        public double FirstDerivativeS1ConstantsKroenerDeWittCubic(double kappa, double kappaFD, double shearModulus, double shearModulusFD)
        {
            double Ret = -1 / (Math.Pow(kappa, 2) * 9.0);
            Ret *= kappaFD;

            Ret += -shearModulusFD / (Math.Pow(shearModulus, 2) * 6.0);

            return Ret;
        }

        public double FirstDerivativeS1ShearModulusKroenerDeWittCubic(double shearModulus, double shearModulusFD)
        {
            double Ret = -1 / (Math.Pow(shearModulus, 2) * 6.0);
            Ret *= shearModulusFD;

            return Ret;
        }
        #endregion

        #region Stiffness

        public double GetMu1KroenerDeWittCubicStiffness()
        {
            double Ret = this.C11 - this.C12;
            Ret *= 0.5;

            return Ret;
        }

        public double GetMu2KroenerDeWittCubicStiffness()
        {
            return this.C44;
        }

        #region First Derivatives

        public double FirstDerivativeC11Mu1KroenerDeWittCubic()
        {
            double Ret = 1;
            Ret *= 0.5;

            return Ret;
        }

        public double FirstDerivativeC12Mu1KroenerDeWittCubic()
        {
            double Ret = -1.0;
            Ret *= 0.5;

            return Ret;
        }

        public double FirstDerivativeC44Mu1KroenerDeWittCubic()
        {
            return 0.0;
        }

        public double FirstDerivativeC11Mu2KroenerDeWittCubic()
        {
            return 0.0;
        }

        public double FirstDerivativeC12Mu2KroenerDeWittCubic()
        {
            return 0.0;
        }

        public double FirstDerivativeC44Mu2KroenerDeWittCubic()
        {
            return 1.0;
        }

        #endregion

        #endregion

        #region Compliance

        public double GetMu1KroenerDeWittCubicCompliance()
        {
            double Ret = this.S11 - this.S12;
            Ret *= 2;

            Ret = 1.0 / Ret;

            return Ret;
        }

        public double GetMu2KroenerDeWittCubicCompliance()
        {
            return 1.0 / this.S44;
        }

        #region First Derivatives

        public double FirstDerivativeS11Mu1KroenerDeWittCubic()
        {
            double Ret = -1;
            double N = 2.0 * Math.Pow(this.S11 - this.S12, 2);
            Ret /= N;

            return Ret;
        }

        public double FirstDerivativeS12Mu1KroenerDeWittCubic()
        {
            double Ret = 1.0;
            double N = 2.0 * Math.Pow(this.S11 - this.S12, 2);
            Ret /= N;

            return Ret;
        }

        public double FirstDerivativeS44Mu1KroenerDeWittCubic()
        {
            return 0.0;
        }

        public double FirstDerivativeS11Mu2KroenerDeWittCubic()
        {
            return 0.0;
        }

        public double FirstDerivativeS12Mu2KroenerDeWittCubic()
        {
            return 0.0;
        }

        public double FirstDerivativeS44Mu2KroenerDeWittCubic()
        {
            double Ret = -1.0 / Math.Pow(this.S44, 2);
            return Ret;
        }

        #endregion

        #endregion

        #region Kroener

        #region StandardParameters

        public double GetAlpha1Kroener(double kappa, double mu1, double mu2)
        {
            double Ret = 15.0 * kappa;
            Ret += 12.0 * mu1;
            Ret += 8 * mu2;

            Ret *= 3.0 / 40.0;

            return Ret;
        }

        public double GetBeta1Kroener(double kappa, double mu1, double mu2)
        {
            double Ret = 3.0 * mu1;
            Ret += 2 * mu2;

            Ret *= 3.0 / 20.0;
            Ret *= kappa;

            return Ret;
        }

        #region First derivatives

        public double FirstDerivativeBeta1Kroener(double kappa, double mu1, double mu2, double kappaFD, double mu1FD, double mu2FD)
        {
            double Ret = 3.0 / 20.0;

            double Sum1 = (3.0 * mu1) + (2.0 * mu2);
            Sum1 *= kappaFD;

            double Sum2 = (3.0 * mu1FD) + (2.0 * mu2FD);
            Sum2 *= kappa;

            Ret *= Sum1 + Sum2;

            return Ret;
        }

        #endregion

        #endregion

        #region Stiffness components

        public double S1KroenerCubicStiffness()
        {
            double UsedKappa = this.KappaCubicStiffness;
            double UsedMu1 = this.GetMu1KroenerDeWittCubicStiffness();
            double UsedMu2 = this.GetMu2KroenerDeWittCubicStiffness();

            double UsedAlpha1 = this.GetAlpha1Kroener(UsedKappa, UsedMu1, UsedMu2);
            double UsedAlpha2 = this.GetAlpha2KroenerDeWitt(UsedMu1, UsedMu2);
            double UsedBeta1 = this.GetBeta1Kroener(UsedKappa, UsedMu1, UsedMu2);
            double UsedBeta2 = this.GetBeta2KroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);
            double UsedGammaParameter = this.GetGammaParameterKroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);

            double UsedAlpha = UsedAlpha2 - UsedAlpha1;
            double UsedBeta = UsedBeta2 - UsedBeta1;

            double[] AllShearModulusSolutions = this.CalculateShearModulus(UsedAlpha, UsedBeta, UsedGammaParameter);

            this._kroenerShearModulus = AllShearModulusSolutions; 

            double UsedShearModulus = this.KroenerShearModulus;

            double Ret = this.S1KroenerDeWittCubic(UsedKappa, UsedShearModulus);

            return Ret;
        }

        public double HS2KroenerCubicStiffness()
        {
            double UsedKappa = this.KappaCubicStiffness;
            double UsedMu1 = this.GetMu1KroenerDeWittCubicStiffness();
            double UsedMu2 = this.GetMu2KroenerDeWittCubicStiffness();

            double UsedAlpha1 = this.GetAlpha1Kroener(UsedKappa, UsedMu1, UsedMu2);
            double UsedAlpha2 = this.GetAlpha2KroenerDeWitt(UsedMu1, UsedMu2);
            double UsedBeta1 = this.GetBeta1Kroener(UsedKappa, UsedMu1, UsedMu2);
            double UsedBeta2 = this.GetBeta2KroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);
            double UsedGammaParameter = this.GetGammaParameterKroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);

            double UsedAlpha = UsedAlpha2 - UsedAlpha1;
            double UsedBeta = UsedBeta2 - UsedBeta1;

            double[] AllShearModulusSolutions = this.CalculateShearModulus(UsedAlpha, UsedBeta, UsedGammaParameter);

            this._kroenerShearModulus = AllShearModulusSolutions;

            double UsedSHearModulus = this.KroenerShearModulus;

            double Ret = this.HS2KroenerDeWittCubic(UsedSHearModulus);

            return Ret;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] C11
        ///[1] C12
        ///[2] C44
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorKroenerCubicStiffnessClassic(double Lambda)
        {
            //[0][0] C11
            //[1][1] C12
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

            //[0] Aclivity
            //[1] Constant
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);

            for (int n = 0; n < this.DiffractionConstants.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeC11S1KroenerCubic() * this.FirstDerivativeC11S1KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 0] += (this.FirstDerivativeC11HS2KroenerCubic() * this.FirstDerivativeC11HS2KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeC12S1KroenerCubic() * this.FirstDerivativeC12S1KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 1] += (this.FirstDerivativeC12HS2KroenerCubic() * this.FirstDerivativeC12HS2KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeC12S1KroenerCubic() * this.FirstDerivativeC11S1KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeC12HS2KroenerCubic() * this.FirstDerivativeC11HS2KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeC12S1KroenerCubic() * this.FirstDerivativeC11S1KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeC12HS2KroenerCubic() * this.FirstDerivativeC11HS2KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeC44S1KroenerCubic() * this.FirstDerivativeC44S1KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 2] += (this.FirstDerivativeC44HS2KroenerCubic() * this.FirstDerivativeC44HS2KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeC44S1KroenerCubic() * this.FirstDerivativeC11S1KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeC44HS2KroenerCubic() * this.FirstDerivativeC11HS2KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeC44S1KroenerCubic() * this.FirstDerivativeC11S1KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeC44HS2KroenerCubic() * this.FirstDerivativeC11HS2KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeC44S1KroenerCubic() * this.FirstDerivativeC12S1KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeC44HS2KroenerCubic() * this.FirstDerivativeC12HS2KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeC44S1KroenerCubic() * this.FirstDerivativeC12S1KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeC44HS2KroenerCubic() * this.FirstDerivativeC12HS2KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                #endregion

                #region Vector build

                SolutionVector[0] += ((this.DiffractionConstants[n].ClassicS1 - this.S1KroenerCubicStiffness()) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeC11S1KroenerCubic();
                SolutionVector[0] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2KroenerCubicStiffness()) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeC11HS2KroenerCubic();
                SolutionVector[1] += ((this.DiffractionConstants[n].ClassicS1 - this.S1KroenerCubicStiffness()) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeC12S1KroenerCubic();
                SolutionVector[1] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2KroenerCubicStiffness()) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeC12HS2KroenerCubic();
                SolutionVector[2] += ((this.DiffractionConstants[n].ClassicS1 - this.S1KroenerCubicStiffness()) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeC44S1KroenerCubic();
                SolutionVector[2] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2KroenerCubicStiffness()) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeC44HS2KroenerCubic();

                #endregion
            }

            for (int n = 0; n < 3; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] C11
        ///[1] C12
        ///[2] C44
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorKroenerCubicStiffnessMacroscopic(double Lambda)
        {
            //[0][0] C11
            //[1][1] C12
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

            //[0] Aclivity
            //[1] Constant
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);

            for (int n = 0; n < this.DiffractionConstants.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeC11S1KroenerCubic() * this.FirstDerivativeC11S1KroenerCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 0] += (this.FirstDerivativeC11HS2KroenerCubic() * this.FirstDerivativeC11HS2KroenerCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeC12S1KroenerCubic() * this.FirstDerivativeC12S1KroenerCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 1] += (this.FirstDerivativeC12HS2KroenerCubic() * this.FirstDerivativeC12HS2KroenerCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeC12S1KroenerCubic() * this.FirstDerivativeC11S1KroenerCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeC12HS2KroenerCubic() * this.FirstDerivativeC11HS2KroenerCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeC12S1KroenerCubic() * this.FirstDerivativeC11S1KroenerCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeC12HS2KroenerCubic() * this.FirstDerivativeC11HS2KroenerCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeC44S1KroenerCubic() * this.FirstDerivativeC44S1KroenerCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 2] += (this.FirstDerivativeC44HS2KroenerCubic() * this.FirstDerivativeC44HS2KroenerCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeC44S1KroenerCubic() * this.FirstDerivativeC11S1KroenerCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeC44HS2KroenerCubic() * this.FirstDerivativeC11HS2KroenerCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeC44S1KroenerCubic() * this.FirstDerivativeC11S1KroenerCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeC44HS2KroenerCubic() * this.FirstDerivativeC11HS2KroenerCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeC44S1KroenerCubic() * this.FirstDerivativeC12S1KroenerCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeC44HS2KroenerCubic() * this.FirstDerivativeC12HS2KroenerCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeC44S1KroenerCubic() * this.FirstDerivativeC12S1KroenerCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeC44HS2KroenerCubic() * this.FirstDerivativeC12HS2KroenerCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                #endregion

                #region Vector build

                SolutionVector[0] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1KroenerCubicStiffness()) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeC11S1KroenerCubic();
                SolutionVector[0] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2KroenerCubicStiffness()) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeC11HS2KroenerCubic();
                SolutionVector[1] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1KroenerCubicStiffness()) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeC12S1KroenerCubic();
                SolutionVector[1] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2KroenerCubicStiffness()) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeC12HS2KroenerCubic();
                SolutionVector[2] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1KroenerCubicStiffness()) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeC44S1KroenerCubic();
                SolutionVector[2] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2KroenerCubicStiffness()) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeC44HS2KroenerCubic();

                #endregion
            }

            for (int n = 0; n < 3; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }
        
        #region First derivative

        public double FirstDerivativeC11S1KroenerCubic()
        {
            double UsedKappa = this.KappaCubicStiffness;
            double UsedMu1 = this.GetMu1KroenerDeWittCubicStiffness();
            double UsedMu2 = this.GetMu2KroenerDeWittCubicStiffness();

            double UsedKappaFD = this.FirstDerivativeC11KappaCubic();
            double UsedMu1FD = this.FirstDerivativeC11Mu1KroenerDeWittCubic();
            double UsedMu2FD = this.FirstDerivativeC11Mu2KroenerDeWittCubic();

            double UsedAlpha1 = this.GetAlpha1Kroener(UsedKappa, UsedMu1, UsedMu2);
            double UsedAlpha2 = this.GetAlpha2KroenerDeWitt(UsedMu1, UsedMu2);
            double UsedBeta1 = this.GetBeta1Kroener(UsedKappa, UsedMu1, UsedMu2);
            double UsedBeta2 = this.GetBeta2KroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);
            double UsedGammaParameter = this.GetGammaParameterKroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);

            double UsedAlpha1FD = this.GetAlpha1Kroener(UsedKappaFD, UsedMu1FD, UsedMu2FD);
            double UsedAlpha2FD = this.GetAlpha2KroenerDeWitt(UsedMu1FD, UsedMu2FD);
            double UsedBeta1FD = this.FirstDerivativeBeta1Kroener(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);
            double UsedBeta2FD = this.FirstDerivativeBeta2KroenerDeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);
            double UsedGammaParameterFD = this.FirstDerivativeGammaParameterKroenerDeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);

            double UsedAlpha = UsedAlpha2 - UsedAlpha1;
            double UsedBeta = UsedBeta2 - UsedBeta1;

            double UsedAlphaFD = UsedAlpha2FD - UsedAlpha1FD;
            double UsedBetaFD = UsedBeta2FD - UsedBeta1FD;

            double[] UsedShearModulus = this.CalculateShearModulus(UsedAlpha, UsedBeta, UsedGammaParameter);
            double UsedShearModulusFD = this.FirstDerivativeShearModulusCubic(UsedAlpha, UsedBeta, UsedGammaParameter, UsedAlphaFD, UsedBetaFD, UsedGammaParameterFD);

            #region Linear

            //double Ret = UsedKappaFD / 9.0;
            //Ret -= UsedShearModulusFD / 6.0;

            #endregion

            #region Inverse

            double Ret = FirstDerivativeS1ConstantsKroenerDeWittCubic(UsedKappa, UsedKappaFD, UsedShearModulus[0], UsedShearModulusFD);

            #endregion



            return Ret;
        }

        public double FirstDerivativeC12S1KroenerCubic()
        {
            double UsedKappa = this.KappaCubicStiffness;
            double UsedMu1 = this.GetMu1KroenerDeWittCubicStiffness();
            double UsedMu2 = this.GetMu2KroenerDeWittCubicStiffness();

            double UsedKappaFD = this.FirstDerivativeC12KappaCubic();
            double UsedMu1FD = this.FirstDerivativeC12Mu1KroenerDeWittCubic();
            double UsedMu2FD = this.FirstDerivativeC12Mu2KroenerDeWittCubic();

            double UsedAlpha1 = this.GetAlpha1Kroener(UsedKappa, UsedMu1, UsedMu2);
            double UsedAlpha2 = this.GetAlpha2KroenerDeWitt(UsedMu1, UsedMu2);
            double UsedBeta1 = this.GetBeta1Kroener(UsedKappa, UsedMu1, UsedMu2);
            double UsedBeta2 = this.GetBeta2KroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);
            double UsedGammaParameter = this.GetGammaParameterKroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);

            double UsedAlpha1FD = this.GetAlpha1Kroener(UsedKappaFD, UsedMu1FD, UsedMu2FD);
            double UsedAlpha2FD = this.GetAlpha2KroenerDeWitt(UsedMu1FD, UsedMu2FD);
            double UsedBeta1FD = this.FirstDerivativeBeta1Kroener(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);
            double UsedBeta2FD = this.FirstDerivativeBeta2KroenerDeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);
            double UsedGammaParameterFD = this.FirstDerivativeGammaParameterKroenerDeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);

            double UsedAlpha = UsedAlpha2 - UsedAlpha1;
            double UsedBeta = UsedBeta2 - UsedBeta1;

            double UsedAlphaFD = UsedAlpha2FD - UsedAlpha1FD;
            double UsedBetaFD = UsedBeta2FD - UsedBeta1FD;

            double[] UsedShearModulus = this.CalculateShearModulus(UsedAlpha, UsedBeta, UsedGammaParameter);
            double UsedShearModulusFD = this.FirstDerivativeShearModulusCubic(UsedAlpha, UsedBeta, UsedGammaParameter, UsedAlphaFD, UsedBetaFD, UsedGammaParameterFD);

            #region Linear

            //double Ret = UsedKappaFD / 9.0;
            //Ret -= UsedShearModulusFD / 6.0;

            #endregion

            #region Inverse

            double Ret = FirstDerivativeS1ConstantsKroenerDeWittCubic(UsedKappa, UsedKappaFD, UsedShearModulus[0], UsedShearModulusFD);

            #endregion

            return Ret;
        }

        public double FirstDerivativeC44S1KroenerCubic()
        {
            double UsedKappa = this.KappaCubicStiffness;
            double UsedMu1 = this.GetMu1KroenerDeWittCubicStiffness();
            double UsedMu2 = this.GetMu2KroenerDeWittCubicStiffness();

            double UsedKappaFD = this.FirstDerivativeC44KappaCubic();
            double UsedMu1FD = this.FirstDerivativeC44Mu1KroenerDeWittCubic();
            double UsedMu2FD = this.FirstDerivativeC44Mu2KroenerDeWittCubic();

            double UsedAlpha1 = this.GetAlpha1Kroener(UsedKappa, UsedMu1, UsedMu2);
            double UsedAlpha2 = this.GetAlpha2KroenerDeWitt(UsedMu1, UsedMu2);
            double UsedBeta1 = this.GetBeta1Kroener(UsedKappa, UsedMu1, UsedMu2);
            double UsedBeta2 = this.GetBeta2KroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);
            double UsedGammaParameter = this.GetGammaParameterKroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);

            double UsedAlpha1FD = this.GetAlpha1Kroener(UsedKappaFD, UsedMu1FD, UsedMu2FD);
            double UsedAlpha2FD = this.GetAlpha2KroenerDeWitt(UsedMu1FD, UsedMu2FD);
            double UsedBeta1FD = this.FirstDerivativeBeta1Kroener(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);
            double UsedBeta2FD = this.FirstDerivativeBeta2KroenerDeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);
            double UsedGammaParameterFD = this.FirstDerivativeGammaParameterKroenerDeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);

            double UsedAlpha = UsedAlpha2 - UsedAlpha1;
            double UsedBeta = UsedBeta2 - UsedBeta1;

            double UsedAlphaFD = UsedAlpha2FD - UsedAlpha1FD;
            double UsedBetaFD = UsedBeta2FD - UsedBeta1FD;

            double[] UsedShearModulus = this.CalculateShearModulus(UsedAlpha, UsedBeta, UsedGammaParameter);
            double UsedShearModulusFD = this.FirstDerivativeShearModulusCubic(UsedAlpha, UsedBeta, UsedGammaParameter, UsedAlphaFD, UsedBetaFD, UsedGammaParameterFD);

            #region Linear

            //double Ret = UsedKappaFD / 9.0;
            //Ret -= UsedShearModulusFD / 6.0;

            #endregion

            #region Inverse

            double Ret = FirstDerivativeS1ConstantsKroenerDeWittCubic(UsedKappa, UsedKappaFD, UsedShearModulus[0], UsedShearModulusFD);

            #endregion

            return Ret;
        }

        public double FirstDerivativeC11HS2KroenerCubic()
        {
            double UsedKappa = this.KappaCubicStiffness;
            double UsedMu1 = this.GetMu1KroenerDeWittCubicStiffness();
            double UsedMu2 = this.GetMu2KroenerDeWittCubicStiffness();

            double UsedKappaFD = this.FirstDerivativeC11KappaCubic();
            double UsedMu1FD = this.FirstDerivativeC11Mu1KroenerDeWittCubic();
            double UsedMu2FD = this.FirstDerivativeC11Mu2KroenerDeWittCubic();

            double UsedAlpha1 = this.GetAlpha1Kroener(UsedKappa, UsedMu1, UsedMu2);
            double UsedAlpha2 = this.GetAlpha2KroenerDeWitt(UsedMu1, UsedMu2);
            double UsedBeta1 = this.GetBeta1Kroener(UsedKappa, UsedMu1, UsedMu2);
            double UsedBeta2 = this.GetBeta2KroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);
            double UsedGammaParameter = this.GetGammaParameterKroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);

            double UsedAlpha1FD = this.GetAlpha1Kroener(UsedKappaFD, UsedMu1FD, UsedMu2FD);
            double UsedAlpha2FD = this.GetAlpha2KroenerDeWitt(UsedMu1FD, UsedMu2FD);
            double UsedBeta1FD = this.FirstDerivativeBeta1Kroener(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);
            double UsedBeta2FD = this.FirstDerivativeBeta2KroenerDeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);
            double UsedGammaParameterFD = this.FirstDerivativeGammaParameterKroenerDeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);

            double UsedAlpha = UsedAlpha2 - UsedAlpha1;
            double UsedBeta = UsedBeta2 - UsedBeta1;

            double UsedAlphaFD = UsedAlpha2FD - UsedAlpha1FD;
            double UsedBetaFD = UsedBeta2FD - UsedBeta1FD;

            double[] UsedShearModulus = this.CalculateShearModulus(UsedAlpha, UsedBeta, UsedGammaParameter);
            double UsedShearModulusFD = this.FirstDerivativeShearModulusCubic(UsedAlpha, UsedBeta, UsedGammaParameter, UsedAlphaFD, UsedBetaFD, UsedGammaParameterFD);

            //double Ret = -1.0 / Math.Pow(UsedShearModulus[0], 2);
            double Ret = -0.5 / Math.Pow(UsedShearModulus[0], 2);
            Ret *= UsedShearModulusFD;

            return Ret;
        }

        public double FirstDerivativeC12HS2KroenerCubic()
        {
            double UsedKappa = this.KappaCubicStiffness;
            double UsedMu1 = this.GetMu1KroenerDeWittCubicStiffness();
            double UsedMu2 = this.GetMu2KroenerDeWittCubicStiffness();

            double UsedKappaFD = this.FirstDerivativeC12KappaCubic();
            double UsedMu1FD = this.FirstDerivativeC12Mu1KroenerDeWittCubic();
            double UsedMu2FD = this.FirstDerivativeC12Mu2KroenerDeWittCubic();

            double UsedAlpha1 = this.GetAlpha1Kroener(UsedKappa, UsedMu1, UsedMu2);
            double UsedAlpha2 = this.GetAlpha2KroenerDeWitt(UsedMu1, UsedMu2);
            double UsedBeta1 = this.GetBeta1Kroener(UsedKappa, UsedMu1, UsedMu2);
            double UsedBeta2 = this.GetBeta2KroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);
            double UsedGammaParameter = this.GetGammaParameterKroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);

            double UsedAlpha1FD = this.GetAlpha1Kroener(UsedKappaFD, UsedMu1FD, UsedMu2FD);
            double UsedAlpha2FD = this.GetAlpha2KroenerDeWitt(UsedMu1FD, UsedMu2FD);
            double UsedBeta1FD = this.FirstDerivativeBeta1Kroener(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);
            double UsedBeta2FD = this.FirstDerivativeBeta2KroenerDeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);
            double UsedGammaParameterFD = this.FirstDerivativeGammaParameterKroenerDeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);

            double UsedAlpha = UsedAlpha2 - UsedAlpha1;
            double UsedBeta = UsedBeta2 - UsedBeta1;

            double UsedAlphaFD = UsedAlpha2FD - UsedAlpha1FD;
            double UsedBetaFD = UsedBeta2FD - UsedBeta1FD;

            double[] UsedShearModulus = this.CalculateShearModulus(UsedAlpha, UsedBeta, UsedGammaParameter);
            double UsedShearModulusFD = this.FirstDerivativeShearModulusCubic(UsedAlpha, UsedBeta, UsedGammaParameter, UsedAlphaFD, UsedBetaFD, UsedGammaParameterFD);

            //double Ret = -1.0 / Math.Pow(UsedShearModulus[0], 2);
            double Ret = -0.5 / Math.Pow(UsedShearModulus[0], 2);
            Ret *= UsedShearModulusFD;

            return Ret;
        }

        public double FirstDerivativeC44HS2KroenerCubic()
        {
            double UsedKappa = this.KappaCubicStiffness;
            double UsedMu1 = this.GetMu1KroenerDeWittCubicStiffness();
            double UsedMu2 = this.GetMu2KroenerDeWittCubicStiffness();

            double UsedKappaFD = this.FirstDerivativeC44KappaCubic();
            double UsedMu1FD = this.FirstDerivativeC44Mu1KroenerDeWittCubic();
            double UsedMu2FD = this.FirstDerivativeC44Mu2KroenerDeWittCubic();

            double UsedAlpha1 = this.GetAlpha1Kroener(UsedKappa, UsedMu1, UsedMu2);
            double UsedAlpha2 = this.GetAlpha2KroenerDeWitt(UsedMu1, UsedMu2);
            double UsedBeta1 = this.GetBeta1Kroener(UsedKappa, UsedMu1, UsedMu2);
            double UsedBeta2 = this.GetBeta2KroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);
            double UsedGammaParameter = this.GetGammaParameterKroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);

            double UsedAlpha1FD = this.GetAlpha1Kroener(UsedKappaFD, UsedMu1FD, UsedMu2FD);
            double UsedAlpha2FD = this.GetAlpha2KroenerDeWitt(UsedMu1FD, UsedMu2FD);
            double UsedBeta1FD = this.FirstDerivativeBeta1Kroener(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);
            double UsedBeta2FD = this.FirstDerivativeBeta2KroenerDeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);
            double UsedGammaParameterFD = this.FirstDerivativeGammaParameterKroenerDeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);

            double UsedAlpha = UsedAlpha2 - UsedAlpha1;
            double UsedBeta = UsedBeta2 - UsedBeta1;

            double UsedAlphaFD = UsedAlpha2FD - UsedAlpha1FD;
            double UsedBetaFD = UsedBeta2FD - UsedBeta1FD;

            double[] UsedShearModulus = this.CalculateShearModulus(UsedAlpha, UsedBeta, UsedGammaParameter);
            double UsedShearModulusFD = this.FirstDerivativeShearModulusCubic(UsedAlpha, UsedBeta, UsedGammaParameter, UsedAlphaFD, UsedBetaFD, UsedGammaParameterFD);

            //double Ret = -1.0 / Math.Pow(UsedShearModulus[0], 2);
            double Ret = -0.5 / Math.Pow(UsedShearModulus[0], 2);
            Ret *= UsedShearModulusFD;

            return Ret;
        }

        #endregion

        #endregion

        #region Compliance components

        public double S1KroenerCubicCompliance()
        {
            double UsedKappa = this.KappaCubicCompliance;
            double UsedMu1 = this.GetMu1KroenerDeWittCubicCompliance();
            double UsedMu2 = this.GetMu2KroenerDeWittCubicCompliance();

            double UsedAlpha1 = this.GetAlpha1Kroener(UsedKappa, UsedMu1, UsedMu2);
            double UsedAlpha2 = this.GetAlpha2KroenerDeWitt(UsedMu1, UsedMu2);
            double UsedBeta1 = this.GetBeta1Kroener(UsedKappa, UsedMu1, UsedMu2);
            double UsedBeta2 = this.GetBeta2KroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);
            double UsedGammaParameter = this.GetGammaParameterKroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);

            double UsedAlpha = UsedAlpha2 - UsedAlpha1;
            double UsedBeta = UsedBeta2 - UsedBeta1;

            double[] AllShearModulusSolutions = this.CalculateShearModulus(UsedAlpha, UsedBeta, UsedGammaParameter);

            this._kroenerShearModulus = AllShearModulusSolutions;

            double UsedShearModulus = this.KroenerShearModulus;

            double Ret = this.S1KroenerDeWittCubic(UsedKappa, UsedShearModulus);

            return Ret;
        }

        public double HS2KroenerCubicCompliance()
        {
            double UsedKappa = this.KappaCubicCompliance;
            double UsedMu1 = this.GetMu1KroenerDeWittCubicCompliance();
            double UsedMu2 = this.GetMu2KroenerDeWittCubicCompliance();

            double UsedAlpha1 = this.GetAlpha1Kroener(UsedKappa, UsedMu1, UsedMu2);
            double UsedAlpha2 = this.GetAlpha2KroenerDeWitt(UsedMu1, UsedMu2);
            double UsedBeta1 = this.GetBeta1Kroener(UsedKappa, UsedMu1, UsedMu2);
            double UsedBeta2 = this.GetBeta2KroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);
            double UsedGammaParameter = this.GetGammaParameterKroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);

            double UsedAlpha = UsedAlpha2 - UsedAlpha1;
            double UsedBeta = UsedBeta2 - UsedBeta1;

            double[] AllShearModulusSolutions = this.CalculateShearModulus(UsedAlpha, UsedBeta, UsedGammaParameter);

            this._kroenerShearModulus = AllShearModulusSolutions;

            double UsedSHearModulus = this.KroenerShearModulus;

            double Ret = this.HS2KroenerDeWittCubic(UsedSHearModulus);

            return Ret;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] C11
        ///[1] C12
        ///[2] C44
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorKroenerCubicComplianceClassic(double Lambda)
        {
            //[0][0] C11
            //[1][1] C12
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

            //[0] Aclivity
            //[1] Constant
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);

            for (int n = 0; n < this.DiffractionConstants.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeS11S1KroenerCubic() * this.FirstDerivativeS11S1KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 0] += (this.FirstDerivativeS11HS2KroenerCubic() * this.FirstDerivativeS11HS2KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeS12S1KroenerCubic() * this.FirstDerivativeS12S1KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 1] += (this.FirstDerivativeS12HS2KroenerCubic() * this.FirstDerivativeS12HS2KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeS12S1KroenerCubic() * this.FirstDerivativeS11S1KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeS12HS2KroenerCubic() * this.FirstDerivativeS11HS2KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeS12S1KroenerCubic() * this.FirstDerivativeS11S1KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeS12HS2KroenerCubic() * this.FirstDerivativeS11HS2KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeS44S1KroenerCubic() * this.FirstDerivativeS44S1KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 2] += (this.FirstDerivativeS44HS2KroenerCubic() * this.FirstDerivativeS44HS2KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeS44S1KroenerCubic() * this.FirstDerivativeS11S1KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeS44HS2KroenerCubic() * this.FirstDerivativeS11HS2KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeS44S1KroenerCubic() * this.FirstDerivativeS11S1KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeS44HS2KroenerCubic() * this.FirstDerivativeS11HS2KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeS44S1KroenerCubic() * this.FirstDerivativeS12S1KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeS44HS2KroenerCubic() * this.FirstDerivativeS12HS2KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeS44S1KroenerCubic() * this.FirstDerivativeS12S1KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeS44HS2KroenerCubic() * this.FirstDerivativeS12HS2KroenerCubic() / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                #endregion

                #region Vector build

                SolutionVector[0] += ((this.DiffractionConstants[n].ClassicS1 - this.S1KroenerCubicCompliance()) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeS11S1KroenerCubic();
                SolutionVector[0] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2KroenerCubicCompliance()) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeS11HS2KroenerCubic();
                SolutionVector[1] += ((this.DiffractionConstants[n].ClassicS1 - this.S1KroenerCubicCompliance()) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeS12S1KroenerCubic();
                SolutionVector[1] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2KroenerCubicCompliance()) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeS12HS2KroenerCubic();
                SolutionVector[2] += ((this.DiffractionConstants[n].ClassicS1 - this.S1KroenerCubicCompliance()) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeS44S1KroenerCubic();
                SolutionVector[2] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2KroenerCubicCompliance()) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeS44HS2KroenerCubic();

                #endregion
            }

            for (int n = 0; n < 3; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] C11
        ///[1] C12
        ///[2] C44
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorKroenerCubicComplianceMacroscopic(double Lambda)
        {
            //[0][0] C11
            //[1][1] C12
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

            //[0] Aclivity
            //[1] Constant
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);

            for (int n = 0; n < this.DiffractionConstants.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeS11S1KroenerCubic() * this.FirstDerivativeS11S1KroenerCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 0] += (this.FirstDerivativeS11HS2KroenerCubic() * this.FirstDerivativeS11HS2KroenerCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeS12S1KroenerCubic() * this.FirstDerivativeS12S1KroenerCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 1] += (this.FirstDerivativeS12HS2KroenerCubic() * this.FirstDerivativeS12HS2KroenerCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeS12S1KroenerCubic() * this.FirstDerivativeS11S1KroenerCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeS12HS2KroenerCubic() * this.FirstDerivativeS11HS2KroenerCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeS12S1KroenerCubic() * this.FirstDerivativeS11S1KroenerCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeS12HS2KroenerCubic() * this.FirstDerivativeS11HS2KroenerCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeS44S1KroenerCubic() * this.FirstDerivativeS44S1KroenerCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 2] += (this.FirstDerivativeS44HS2KroenerCubic() * this.FirstDerivativeS44HS2KroenerCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeS44S1KroenerCubic() * this.FirstDerivativeS11S1KroenerCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeS44HS2KroenerCubic() * this.FirstDerivativeS11HS2KroenerCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeS44S1KroenerCubic() * this.FirstDerivativeS11S1KroenerCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeS44HS2KroenerCubic() * this.FirstDerivativeS11HS2KroenerCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeS44S1KroenerCubic() * this.FirstDerivativeS12S1KroenerCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeS44HS2KroenerCubic() * this.FirstDerivativeS12HS2KroenerCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeS44S1KroenerCubic() * this.FirstDerivativeS12S1KroenerCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeS44HS2KroenerCubic() * this.FirstDerivativeS12HS2KroenerCubic() / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                #endregion

                #region Vector build

                SolutionVector[0] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1KroenerCubicCompliance()) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeS11S1KroenerCubic();
                SolutionVector[0] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2KroenerCubicCompliance()) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeS11HS2KroenerCubic();
                SolutionVector[1] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1KroenerCubicCompliance()) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeS12S1KroenerCubic();
                SolutionVector[1] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2KroenerCubicCompliance()) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeS12HS2KroenerCubic();
                SolutionVector[2] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1KroenerCubicCompliance()) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeS44S1KroenerCubic();
                SolutionVector[2] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2KroenerCubicCompliance()) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeS44HS2KroenerCubic();

                #endregion
            }

            for (int n = 0; n < 3; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        #region First derivative

        public double FirstDerivativeS11S1KroenerCubic()
        {
            double UsedKappa = this.KappaCubicCompliance;
            double UsedMu1 = this.GetMu1KroenerDeWittCubicCompliance();
            double UsedMu2 = this.GetMu2KroenerDeWittCubicCompliance();

            double UsedKappaFD = this.FirstDerivativeS11KappaCubic();
            double UsedMu1FD = this.FirstDerivativeS11Mu1KroenerDeWittCubic();
            double UsedMu2FD = this.FirstDerivativeS11Mu2KroenerDeWittCubic();

            double UsedAlpha1 = this.GetAlpha1Kroener(UsedKappa, UsedMu1, UsedMu2);
            double UsedAlpha2 = this.GetAlpha2KroenerDeWitt(UsedMu1, UsedMu2);
            double UsedBeta1 = this.GetBeta1Kroener(UsedKappa, UsedMu1, UsedMu2);
            double UsedBeta2 = this.GetBeta2KroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);
            double UsedGammaParameter = this.GetGammaParameterKroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);

            double UsedAlpha1FD = this.GetAlpha1Kroener(UsedKappaFD, UsedMu1FD, UsedMu2FD);
            double UsedAlpha2FD = this.GetAlpha2KroenerDeWitt(UsedMu1FD, UsedMu2FD);
            double UsedBeta1FD = this.FirstDerivativeBeta1Kroener(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);
            double UsedBeta2FD = this.FirstDerivativeBeta2KroenerDeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);
            double UsedGammaParameterFD = this.FirstDerivativeGammaParameterKroenerDeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);

            double UsedAlpha = UsedAlpha2 - UsedAlpha1;
            double UsedBeta = UsedBeta2 - UsedBeta1;

            double UsedAlphaFD = UsedAlpha2FD - UsedAlpha1FD;
            double UsedBetaFD = UsedBeta2FD - UsedBeta1FD;

            double[] UsedShearModulus = this.CalculateShearModulus(UsedAlpha, UsedBeta, UsedGammaParameter);
            double UsedShearModulusFD = this.FirstDerivativeShearModulusCubic(UsedAlpha, UsedBeta, UsedGammaParameter, UsedAlphaFD, UsedBetaFD, UsedGammaParameterFD);

            double Ret = UsedKappaFD / 9.0;
            Ret -= UsedShearModulusFD / 6.0; 

            return Ret;
        }

        public double FirstDerivativeS12S1KroenerCubic()
        {
            double UsedKappa = this.KappaCubicCompliance;
            double UsedMu1 = this.GetMu1KroenerDeWittCubicCompliance();
            double UsedMu2 = this.GetMu2KroenerDeWittCubicCompliance();

            double UsedKappaFD = this.FirstDerivativeS12KappaCubic();
            double UsedMu1FD = this.FirstDerivativeS12Mu1KroenerDeWittCubic();
            double UsedMu2FD = this.FirstDerivativeS12Mu2KroenerDeWittCubic();

            double UsedAlpha1 = this.GetAlpha1Kroener(UsedKappa, UsedMu1, UsedMu2);
            double UsedAlpha2 = this.GetAlpha2KroenerDeWitt(UsedMu1, UsedMu2);
            double UsedBeta1 = this.GetBeta1Kroener(UsedKappa, UsedMu1, UsedMu2);
            double UsedBeta2 = this.GetBeta2KroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);
            double UsedGammaParameter = this.GetGammaParameterKroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);

            double UsedAlpha1FD = this.GetAlpha1Kroener(UsedKappaFD, UsedMu1FD, UsedMu2FD);
            double UsedAlpha2FD = this.GetAlpha2KroenerDeWitt(UsedMu1FD, UsedMu2FD);
            double UsedBeta1FD = this.FirstDerivativeBeta1Kroener(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);
            double UsedBeta2FD = this.FirstDerivativeBeta2KroenerDeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);
            double UsedGammaParameterFD = this.FirstDerivativeGammaParameterKroenerDeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);

            double UsedAlpha = UsedAlpha2 - UsedAlpha1;
            double UsedBeta = UsedBeta2 - UsedBeta1;

            double UsedAlphaFD = UsedAlpha2FD - UsedAlpha1FD;
            double UsedBetaFD = UsedBeta2FD - UsedBeta1FD;

            double[] UsedShearModulus = this.CalculateShearModulus(UsedAlpha, UsedBeta, UsedGammaParameter);
            double UsedShearModulusFD = this.FirstDerivativeShearModulusCubic(UsedAlpha, UsedBeta, UsedGammaParameter, UsedAlphaFD, UsedBetaFD, UsedGammaParameterFD);

            double Ret = UsedKappaFD / 9.0;
            Ret -= UsedShearModulusFD / 6.0;

            return Ret;
        }

        public double FirstDerivativeS44S1KroenerCubic()
        {
            double UsedKappa = this.KappaCubicCompliance;
            double UsedMu1 = this.GetMu1KroenerDeWittCubicCompliance();
            double UsedMu2 = this.GetMu2KroenerDeWittCubicCompliance();

            double UsedKappaFD = this.FirstDerivativeS44KappaCubic();
            double UsedMu1FD = this.FirstDerivativeS44Mu1KroenerDeWittCubic();
            double UsedMu2FD = this.FirstDerivativeS44Mu2KroenerDeWittCubic();

            double UsedAlpha1 = this.GetAlpha1Kroener(UsedKappa, UsedMu1, UsedMu2);
            double UsedAlpha2 = this.GetAlpha2KroenerDeWitt(UsedMu1, UsedMu2);
            double UsedBeta1 = this.GetBeta1Kroener(UsedKappa, UsedMu1, UsedMu2);
            double UsedBeta2 = this.GetBeta2KroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);
            double UsedGammaParameter = this.GetGammaParameterKroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);

            double UsedAlpha1FD = this.GetAlpha1Kroener(UsedKappaFD, UsedMu1FD, UsedMu2FD);
            double UsedAlpha2FD = this.GetAlpha2KroenerDeWitt(UsedMu1FD, UsedMu2FD);
            double UsedBeta1FD = this.FirstDerivativeBeta1Kroener(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);
            double UsedBeta2FD = this.FirstDerivativeBeta2KroenerDeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);
            double UsedGammaParameterFD = this.FirstDerivativeGammaParameterKroenerDeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);

            double UsedAlpha = UsedAlpha2 - UsedAlpha1;
            double UsedBeta = UsedBeta2 - UsedBeta1;

            double UsedAlphaFD = UsedAlpha2FD - UsedAlpha1FD;
            double UsedBetaFD = UsedBeta2FD - UsedBeta1FD;

            double[] UsedShearModulus = this.CalculateShearModulus(UsedAlpha, UsedBeta, UsedGammaParameter);
            double UsedShearModulusFD = this.FirstDerivativeShearModulusCubic(UsedAlpha, UsedBeta, UsedGammaParameter, UsedAlphaFD, UsedBetaFD, UsedGammaParameterFD);

            double Ret = UsedKappaFD / 9.0;
            Ret -= UsedShearModulusFD / 6.0;

            return Ret;
        }

        public double FirstDerivativeS11HS2KroenerCubic()
        {
            double UsedKappa = this.KappaCubicCompliance;
            double UsedMu1 = this.GetMu1KroenerDeWittCubicCompliance();
            double UsedMu2 = this.GetMu2KroenerDeWittCubicCompliance();

            double UsedKappaFD = this.FirstDerivativeS11KappaCubic();
            double UsedMu1FD = this.FirstDerivativeS11Mu1KroenerDeWittCubic();
            double UsedMu2FD = this.FirstDerivativeS11Mu2KroenerDeWittCubic();

            double UsedAlpha1 = this.GetAlpha1Kroener(UsedKappa, UsedMu1, UsedMu2);
            double UsedAlpha2 = this.GetAlpha2KroenerDeWitt(UsedMu1, UsedMu2);
            double UsedBeta1 = this.GetBeta1Kroener(UsedKappa, UsedMu1, UsedMu2);
            double UsedBeta2 = this.GetBeta2KroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);
            double UsedGammaParameter = this.GetGammaParameterKroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);

            double UsedAlpha1FD = this.GetAlpha1Kroener(UsedKappaFD, UsedMu1FD, UsedMu2FD);
            double UsedAlpha2FD = this.GetAlpha2KroenerDeWitt(UsedMu1FD, UsedMu2FD);
            double UsedBeta1FD = this.FirstDerivativeBeta1Kroener(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);
            double UsedBeta2FD = this.FirstDerivativeBeta2KroenerDeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);
            double UsedGammaParameterFD = this.FirstDerivativeGammaParameterKroenerDeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);

            double UsedAlpha = UsedAlpha2 - UsedAlpha1;
            double UsedBeta = UsedBeta2 - UsedBeta1;

            double UsedAlphaFD = UsedAlpha2FD - UsedAlpha1FD;
            double UsedBetaFD = UsedBeta2FD - UsedBeta1FD;

            double[] UsedShearModulus = this.CalculateShearModulus(UsedAlpha, UsedBeta, UsedGammaParameter);
            double UsedShearModulusFD = this.FirstDerivativeShearModulusCubic(UsedAlpha, UsedBeta, UsedGammaParameter, UsedAlphaFD, UsedBetaFD, UsedGammaParameterFD);

            //double Ret = -1.0 / Math.Pow(UsedShearModulus[0], 2);
            double Ret = -0.5 / Math.Pow(UsedShearModulus[0], 2);
            Ret *= UsedShearModulusFD;

            return Ret;
        }

        public double FirstDerivativeS12HS2KroenerCubic()
        {
            double UsedKappa = this.KappaCubicCompliance;
            double UsedMu1 = this.GetMu1KroenerDeWittCubicCompliance();
            double UsedMu2 = this.GetMu2KroenerDeWittCubicCompliance();

            double UsedKappaFD = this.FirstDerivativeS12KappaCubic();
            double UsedMu1FD = this.FirstDerivativeS12Mu1KroenerDeWittCubic();
            double UsedMu2FD = this.FirstDerivativeS12Mu2KroenerDeWittCubic();

            double UsedAlpha1 = this.GetAlpha1Kroener(UsedKappa, UsedMu1, UsedMu2);
            double UsedAlpha2 = this.GetAlpha2KroenerDeWitt(UsedMu1, UsedMu2);
            double UsedBeta1 = this.GetBeta1Kroener(UsedKappa, UsedMu1, UsedMu2);
            double UsedBeta2 = this.GetBeta2KroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);
            double UsedGammaParameter = this.GetGammaParameterKroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);

            double UsedAlpha1FD = this.GetAlpha1Kroener(UsedKappaFD, UsedMu1FD, UsedMu2FD);
            double UsedAlpha2FD = this.GetAlpha2KroenerDeWitt(UsedMu1FD, UsedMu2FD);
            double UsedBeta1FD = this.FirstDerivativeBeta1Kroener(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);
            double UsedBeta2FD = this.FirstDerivativeBeta2KroenerDeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);
            double UsedGammaParameterFD = this.FirstDerivativeGammaParameterKroenerDeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);

            double UsedAlpha = UsedAlpha2 - UsedAlpha1;
            double UsedBeta = UsedBeta2 - UsedBeta1;

            double UsedAlphaFD = UsedAlpha2FD - UsedAlpha1FD;
            double UsedBetaFD = UsedBeta2FD - UsedBeta1FD;

            double[] UsedShearModulus = this.CalculateShearModulus(UsedAlpha, UsedBeta, UsedGammaParameter);
            double UsedShearModulusFD = this.FirstDerivativeShearModulusCubic(UsedAlpha, UsedBeta, UsedGammaParameter, UsedAlphaFD, UsedBetaFD, UsedGammaParameterFD);

            //double Ret = -1.0 / Math.Pow(UsedShearModulus[0], 2);
            double Ret = -0.5 / Math.Pow(UsedShearModulus[0], 2);
            Ret *= UsedShearModulusFD;

            return Ret;
        }

        public double FirstDerivativeS44HS2KroenerCubic()
        {
            double UsedKappa = this.KappaCubicCompliance;
            double UsedMu1 = this.GetMu1KroenerDeWittCubicCompliance();
            double UsedMu2 = this.GetMu2KroenerDeWittCubicCompliance();

            double UsedKappaFD = this.FirstDerivativeS44KappaCubic();
            double UsedMu1FD = this.FirstDerivativeS44Mu1KroenerDeWittCubic();
            double UsedMu2FD = this.FirstDerivativeS44Mu2KroenerDeWittCubic();

            double UsedAlpha1 = this.GetAlpha1Kroener(UsedKappa, UsedMu1, UsedMu2);
            double UsedAlpha2 = this.GetAlpha2KroenerDeWitt(UsedMu1, UsedMu2);
            double UsedBeta1 = this.GetBeta1Kroener(UsedKappa, UsedMu1, UsedMu2);
            double UsedBeta2 = this.GetBeta2KroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);
            double UsedGammaParameter = this.GetGammaParameterKroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);

            double UsedAlpha1FD = this.GetAlpha1Kroener(UsedKappaFD, UsedMu1FD, UsedMu2FD);
            double UsedAlpha2FD = this.GetAlpha2KroenerDeWitt(UsedMu1FD, UsedMu2FD);
            double UsedBeta1FD = this.FirstDerivativeBeta1Kroener(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);
            double UsedBeta2FD = this.FirstDerivativeBeta2KroenerDeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);
            double UsedGammaParameterFD = this.FirstDerivativeGammaParameterKroenerDeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);

            double UsedAlpha = UsedAlpha2 - UsedAlpha1;
            double UsedBeta = UsedBeta2 - UsedBeta1;

            double UsedAlphaFD = UsedAlpha2FD - UsedAlpha1FD;
            double UsedBetaFD = UsedBeta2FD - UsedBeta1FD;

            double[] UsedShearModulus = this.CalculateShearModulus(UsedAlpha, UsedBeta, UsedGammaParameter);
            double UsedShearModulusFD = this.FirstDerivativeShearModulusCubic(UsedAlpha, UsedBeta, UsedGammaParameter, UsedAlphaFD, UsedBetaFD, UsedGammaParameterFD);

            //double Ret = -1.0 / Math.Pow(UsedShearModulus[0], 2);
            double Ret = -0.5 / Math.Pow(UsedShearModulus[0], 2);
            Ret *= UsedShearModulusFD;

            return Ret;
        }

        #endregion

        #endregion

        #endregion

        #region DeWitt

        #region Standard parameters

        public double GetAlpha1DeWitt(double kappa, double mu1, double mu2, DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = mu1 - mu2;
            Ret *= 3.0 * CubicGamma(hKL);
            Ret += mu2;

            Ret *= 4.0;
            Ret += 3.0 * kappa;

            Ret *= 3.0 / 8.0;

            return Ret;
        }

        public double GetBeta1DeWitt(double kappa, double mu1, double mu2, DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = mu1 - mu2;
            Ret *= 3.0 * CubicGamma(hKL);
            Ret += mu2;

            Ret *= kappa;
            Ret *= 3.0 / 4.0;

            return Ret;
        }

        #region First derivatives

        public double FirstDerivativeBeta1DeWitt(double kappa, double mu1, double mu2, double kappaFD, double mu1FD, double mu2FD, DataManagment.CrystalData.HKLReflex hKL)
        {
            double Sum1 = mu1 - mu2;
            Sum1 *= 3.0 * CubicGamma(hKL);
            Sum1 += mu2;

            Sum1 *= kappaFD;
            Sum1 *= 3.0 / 4.0;

            double Ret = mu1FD - mu2FD;
            Ret *= 3.0 * CubicGamma(hKL);
            Ret += mu2FD;

            Ret *= kappa;
            Ret *= 3.0 / 4.0;

            Ret += Sum1;

            return Ret;
        }

        #endregion

        #endregion

        #region Stiffness components

        public double S1DeWittCubicStiffness(DataManagment.CrystalData.HKLReflex hKL)
        {
            double UsedKappa = this.KappaCubicStiffness;
            double UsedMu1 = this.GetMu1KroenerDeWittCubicStiffness();
            double UsedMu2 = this.GetMu2KroenerDeWittCubicStiffness();

            double UsedAlpha1 = this.GetAlpha1DeWitt(UsedKappa, UsedMu1, UsedMu2, hKL);
            double UsedAlpha2 = this.GetAlpha2KroenerDeWitt(UsedMu1, UsedMu2);
            double UsedBeta1 = this.GetBeta1DeWitt(UsedKappa, UsedMu1, UsedMu2, hKL);
            double UsedBeta2 = this.GetBeta2KroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);
            double UsedGammaParameter = this.GetGammaParameterKroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);

            double UsedAlpha = UsedAlpha2 - UsedAlpha1;
            double UsedBeta = UsedBeta2 - UsedBeta1;

            double[] AllShearModulusSolutions = this.CalculateShearModulus(UsedAlpha, UsedBeta, UsedGammaParameter);

            this._deWittShearModulus = AllShearModulusSolutions;

            double UsedShearModulus = this.DeWittShearModulus;

            double Ret = this.S1KroenerDeWittCubic(UsedKappa, UsedShearModulus);

            return Ret;
        }

        public double HS2DeWittCubicStiffness(DataManagment.CrystalData.HKLReflex hKL)
        {
            double UsedKappa = this.KappaCubicStiffness;
            double UsedMu1 = this.GetMu1KroenerDeWittCubicStiffness();
            double UsedMu2 = this.GetMu2KroenerDeWittCubicStiffness();

            double UsedAlpha1 = this.GetAlpha1DeWitt(UsedKappa, UsedMu1, UsedMu2, hKL);
            double UsedAlpha2 = this.GetAlpha2KroenerDeWitt(UsedMu1, UsedMu2);
            double UsedBeta1 = this.GetBeta1DeWitt(UsedKappa, UsedMu1, UsedMu2, hKL);
            double UsedBeta2 = this.GetBeta2KroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);
            double UsedGammaParameter = this.GetGammaParameterKroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);

            double UsedAlpha = UsedAlpha2 - UsedAlpha1;
            double UsedBeta = UsedBeta2 - UsedBeta1;

            double[] AllShearModulusSolutions = this.CalculateShearModulus(UsedAlpha, UsedBeta, UsedGammaParameter);

            this._deWittShearModulus = AllShearModulusSolutions;

            double UsedSHearModulus = this.DeWittShearModulus;

            double Ret = this.HS2KroenerDeWittCubic(UsedSHearModulus);

            return Ret;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] C11
        ///[1] C12
        ///[2] C44
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorDeWittCubicStiffnessClassic(double Lambda)
        {
            //[0][0] C11
            //[1][1] C12
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

            //[0] Aclivity
            //[1] Constant
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);

            for (int n = 0; n < this.DiffractionConstants.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeC11S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeC11S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 0] += (this.FirstDerivativeC11HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeC11HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeC12S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeC12S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 1] += (this.FirstDerivativeC12HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeC12HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeC12S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeC11S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeC12HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeC11HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeC12S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeC11S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeC12HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeC11HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeC44S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeC44S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 2] += (this.FirstDerivativeC44HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeC44HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeC44S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeC11S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeC44HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeC11HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeC44S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeC11S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeC44HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeC11HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeC44S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeC12S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeC44HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeC12HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeC44S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeC12S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeC44HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeC12HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                #endregion

                #region Vector build

                SolutionVector[0] += ((this.DiffractionConstants[n].ClassicS1 - this.S1DeWittCubicStiffness(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeC11S1DeWittCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[0] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2DeWittCubicStiffness(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeC11HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[1] += ((this.DiffractionConstants[n].ClassicS1 - this.S1DeWittCubicStiffness(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeC12S1DeWittCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[1] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2DeWittCubicStiffness(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeC12HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[2] += ((this.DiffractionConstants[n].ClassicS1 - this.S1DeWittCubicStiffness(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeC44S1DeWittCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[2] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2DeWittCubicStiffness(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeC44HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex);

                #endregion
            }

            for (int n = 0; n < 3; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] C11
        ///[1] C12
        ///[2] C44
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorDeWittCubicStiffnessMacroscopic(double Lambda)
        {
            //[0][0] C11
            //[1][1] C12
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

            //[0] Aclivity
            //[1] Constant
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);

            for (int n = 0; n < this.DiffractionConstants.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeC11S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeC11S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 0] += (this.FirstDerivativeC11HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeC11HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeC12S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeC12S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 1] += (this.FirstDerivativeC12HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeC12HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeC12S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeC11S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeC12HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeC11HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeC12S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeC11S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeC12HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeC11HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeC44S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeC44S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 2] += (this.FirstDerivativeC44HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeC44HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeC44S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeC11S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeC44HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeC11HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeC44S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeC11S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeC44HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeC11HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeC44S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeC12S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeC44HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeC12HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeC44S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeC12S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeC44HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeC12HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                #endregion

                #region Vector build

                SolutionVector[0] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1DeWittCubicStiffness(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeC11S1DeWittCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[0] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2DeWittCubicStiffness(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeC11HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[1] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1DeWittCubicStiffness(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeC12S1DeWittCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[1] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2DeWittCubicStiffness(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeC12HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[2] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1DeWittCubicStiffness(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeC44S1DeWittCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[2] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2DeWittCubicStiffness(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeC44HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex);

                #endregion
            }

            for (int n = 0; n < 3; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        #region First derivative

        public double FirstDerivativeC11S1DeWittCubic(DataManagment.CrystalData.HKLReflex hKL)
        {
            double UsedKappa = this.KappaCubicStiffness;
            double UsedMu1 = this.GetMu1KroenerDeWittCubicStiffness();
            double UsedMu2 = this.GetMu2KroenerDeWittCubicStiffness();

            double UsedKappaFD = this.FirstDerivativeC11KappaCubic();
            double UsedMu1FD = this.FirstDerivativeC11Mu1KroenerDeWittCubic();
            double UsedMu2FD = this.FirstDerivativeC11Mu2KroenerDeWittCubic();

            double UsedAlpha1 = this.GetAlpha1DeWitt(UsedKappa, UsedMu1, UsedMu2, hKL);
            double UsedAlpha2 = this.GetAlpha2KroenerDeWitt(UsedMu1, UsedMu2);
            double UsedBeta1 = this.GetBeta1DeWitt(UsedKappa, UsedMu1, UsedMu2, hKL);
            double UsedBeta2 = this.GetBeta2KroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);
            double UsedGammaParameter = this.GetGammaParameterKroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);

            double UsedAlpha1FD = this.GetAlpha1DeWitt(UsedKappaFD, UsedMu1FD, UsedMu2FD, hKL);
            double UsedAlpha2FD = this.GetAlpha2KroenerDeWitt(UsedMu1FD, UsedMu2FD);
            double UsedBeta1FD = this.FirstDerivativeBeta1DeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD, hKL);
            double UsedBeta2FD = this.FirstDerivativeBeta2KroenerDeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);
            double UsedGammaParameterFD = this.FirstDerivativeGammaParameterKroenerDeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);

            double UsedAlpha = UsedAlpha2 - UsedAlpha1;
            double UsedBeta = UsedBeta2 - UsedBeta1;

            double UsedAlphaFD = UsedAlpha2FD - UsedAlpha1FD;
            double UsedBetaFD = UsedBeta2FD - UsedBeta1FD;

            double[] UsedShearModulus = this.CalculateShearModulus(UsedAlpha, UsedBeta, UsedGammaParameter);
            double UsedShearModulusFD = this.FirstDerivativeShearModulusCubic(UsedAlpha, UsedBeta, UsedGammaParameter, UsedAlphaFD, UsedBetaFD, UsedGammaParameterFD);

            #region Linear

            //double Ret = UsedKappaFD / 9.0;
            //Ret -= UsedShearModulusFD / 6.0;

            #endregion

            #region Inverse

            double Ret = FirstDerivativeS1ConstantsKroenerDeWittCubic(UsedKappa, UsedKappaFD, UsedShearModulus[0], UsedShearModulusFD);

            #endregion

            return Ret;
        }

        public double FirstDerivativeC12S1DeWittCubic(DataManagment.CrystalData.HKLReflex hKL)
        {
            double UsedKappa = this.KappaCubicStiffness;
            double UsedMu1 = this.GetMu1KroenerDeWittCubicStiffness();
            double UsedMu2 = this.GetMu2KroenerDeWittCubicStiffness();

            double UsedKappaFD = this.FirstDerivativeC12KappaCubic();
            double UsedMu1FD = this.FirstDerivativeC12Mu1KroenerDeWittCubic();
            double UsedMu2FD = this.FirstDerivativeC12Mu2KroenerDeWittCubic();

            double UsedAlpha1 = this.GetAlpha1DeWitt(UsedKappa, UsedMu1, UsedMu2, hKL);
            double UsedAlpha2 = this.GetAlpha2KroenerDeWitt(UsedMu1, UsedMu2);
            double UsedBeta1 = this.GetBeta1DeWitt(UsedKappa, UsedMu1, UsedMu2, hKL);
            double UsedBeta2 = this.GetBeta2KroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);
            double UsedGammaParameter = this.GetGammaParameterKroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);

            double UsedAlpha1FD = this.GetAlpha1DeWitt(UsedKappaFD, UsedMu1FD, UsedMu2FD, hKL);
            double UsedAlpha2FD = this.GetAlpha2KroenerDeWitt(UsedMu1FD, UsedMu2FD);
            double UsedBeta1FD = this.FirstDerivativeBeta1DeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD, hKL);
            double UsedBeta2FD = this.FirstDerivativeBeta2KroenerDeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);
            double UsedGammaParameterFD = this.FirstDerivativeGammaParameterKroenerDeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);

            double UsedAlpha = UsedAlpha2 - UsedAlpha1;
            double UsedBeta = UsedBeta2 - UsedBeta1;

            double UsedAlphaFD = UsedAlpha2FD - UsedAlpha1FD;
            double UsedBetaFD = UsedBeta2FD - UsedBeta1FD;

            double[] UsedShearModulus = this.CalculateShearModulus(UsedAlpha, UsedBeta, UsedGammaParameter);
            double UsedShearModulusFD = this.FirstDerivativeShearModulusCubic(UsedAlpha, UsedBeta, UsedGammaParameter, UsedAlphaFD, UsedBetaFD, UsedGammaParameterFD);

            #region Linear

            //double Ret = UsedKappaFD / 9.0;
            //Ret -= UsedShearModulusFD / 6.0;

            #endregion

            #region Inverse

            double Ret = FirstDerivativeS1ConstantsKroenerDeWittCubic(UsedKappa, UsedKappaFD, UsedShearModulus[0], UsedShearModulusFD);

            #endregion

            return Ret;
        }

        public double FirstDerivativeC44S1DeWittCubic(DataManagment.CrystalData.HKLReflex hKL)
        {
            double UsedKappa = this.KappaCubicStiffness;
            double UsedMu1 = this.GetMu1KroenerDeWittCubicStiffness();
            double UsedMu2 = this.GetMu2KroenerDeWittCubicStiffness();

            double UsedKappaFD = this.FirstDerivativeC44KappaCubic();
            double UsedMu1FD = this.FirstDerivativeC44Mu1KroenerDeWittCubic();
            double UsedMu2FD = this.FirstDerivativeC44Mu2KroenerDeWittCubic();

            double UsedAlpha1 = this.GetAlpha1DeWitt(UsedKappa, UsedMu1, UsedMu2, hKL);
            double UsedAlpha2 = this.GetAlpha2KroenerDeWitt(UsedMu1, UsedMu2);
            double UsedBeta1 = this.GetBeta1DeWitt(UsedKappa, UsedMu1, UsedMu2, hKL);
            double UsedBeta2 = this.GetBeta2KroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);
            double UsedGammaParameter = this.GetGammaParameterKroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);

            double UsedAlpha1FD = this.GetAlpha1DeWitt(UsedKappaFD, UsedMu1FD, UsedMu2FD, hKL);
            double UsedAlpha2FD = this.GetAlpha2KroenerDeWitt(UsedMu1FD, UsedMu2FD);
            double UsedBeta1FD = this.FirstDerivativeBeta1DeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD, hKL);
            double UsedBeta2FD = this.FirstDerivativeBeta2KroenerDeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);
            double UsedGammaParameterFD = this.FirstDerivativeGammaParameterKroenerDeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);

            double UsedAlpha = UsedAlpha2 - UsedAlpha1;
            double UsedBeta = UsedBeta2 - UsedBeta1;

            double UsedAlphaFD = UsedAlpha2FD - UsedAlpha1FD;
            double UsedBetaFD = UsedBeta2FD - UsedBeta1FD;

            double[] UsedShearModulus = this.CalculateShearModulus(UsedAlpha, UsedBeta, UsedGammaParameter);
            double UsedShearModulusFD = this.FirstDerivativeShearModulusCubic(UsedAlpha, UsedBeta, UsedGammaParameter, UsedAlphaFD, UsedBetaFD, UsedGammaParameterFD);

            #region Linear

            //double Ret = UsedKappaFD / 9.0;
            //Ret -= UsedShearModulusFD / 6.0;

            #endregion

            #region Inverse

            double Ret = FirstDerivativeS1ConstantsKroenerDeWittCubic(UsedKappa, UsedKappaFD, UsedShearModulus[0], UsedShearModulusFD);

            #endregion

            return Ret;
        }

        public double FirstDerivativeC11HS2DeWittCubic(DataManagment.CrystalData.HKLReflex hKL)
        {
            double UsedKappa = this.KappaCubicStiffness;
            double UsedMu1 = this.GetMu1KroenerDeWittCubicStiffness();
            double UsedMu2 = this.GetMu2KroenerDeWittCubicStiffness();

            double UsedKappaFD = this.FirstDerivativeC11KappaCubic();
            double UsedMu1FD = this.FirstDerivativeC11Mu1KroenerDeWittCubic();
            double UsedMu2FD = this.FirstDerivativeC11Mu2KroenerDeWittCubic();

            double UsedAlpha1 = this.GetAlpha1DeWitt(UsedKappa, UsedMu1, UsedMu2, hKL);
            double UsedAlpha2 = this.GetAlpha2KroenerDeWitt(UsedMu1, UsedMu2);
            double UsedBeta1 = this.GetBeta1DeWitt(UsedKappa, UsedMu1, UsedMu2, hKL);
            double UsedBeta2 = this.GetBeta2KroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);
            double UsedGammaParameter = this.GetGammaParameterKroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);

            double UsedAlpha1FD = this.GetAlpha1DeWitt(UsedKappaFD, UsedMu1FD, UsedMu2FD, hKL);
            double UsedAlpha2FD = this.GetAlpha2KroenerDeWitt(UsedMu1FD, UsedMu2FD);
            double UsedBeta1FD = this.FirstDerivativeBeta1DeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD, hKL);
            double UsedBeta2FD = this.FirstDerivativeBeta2KroenerDeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);
            double UsedGammaParameterFD = this.FirstDerivativeGammaParameterKroenerDeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);

            double UsedAlpha = UsedAlpha2 - UsedAlpha1;
            double UsedBeta = UsedBeta2 - UsedBeta1;

            double UsedAlphaFD = UsedAlpha2FD - UsedAlpha1FD;
            double UsedBetaFD = UsedBeta2FD - UsedBeta1FD;

            double[] UsedShearModulus = this.CalculateShearModulus(UsedAlpha, UsedBeta, UsedGammaParameter);
            double UsedShearModulusFD = this.FirstDerivativeShearModulusCubic(UsedAlpha, UsedBeta, UsedGammaParameter, UsedAlphaFD, UsedBetaFD, UsedGammaParameterFD);

            //double Ret = -1.0 / Math.Pow(UsedShearModulus[0], 2);
            double Ret = -0.5 / Math.Pow(UsedShearModulus[0], 2);
            Ret *= UsedShearModulusFD;

            return Ret;
        }

        public double FirstDerivativeC12HS2DeWittCubic(DataManagment.CrystalData.HKLReflex hKL)
        {
            double UsedKappa = this.KappaCubicStiffness;
            double UsedMu1 = this.GetMu1KroenerDeWittCubicStiffness();
            double UsedMu2 = this.GetMu2KroenerDeWittCubicStiffness();

            double UsedKappaFD = this.FirstDerivativeC12KappaCubic();
            double UsedMu1FD = this.FirstDerivativeC12Mu1KroenerDeWittCubic();
            double UsedMu2FD = this.FirstDerivativeC12Mu2KroenerDeWittCubic();

            double UsedAlpha1 = this.GetAlpha1DeWitt(UsedKappa, UsedMu1, UsedMu2, hKL);
            double UsedAlpha2 = this.GetAlpha2KroenerDeWitt(UsedMu1, UsedMu2);
            double UsedBeta1 = this.GetBeta1DeWitt(UsedKappa, UsedMu1, UsedMu2, hKL);
            double UsedBeta2 = this.GetBeta2KroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);
            double UsedGammaParameter = this.GetGammaParameterKroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);

            double UsedAlpha1FD = this.GetAlpha1DeWitt(UsedKappaFD, UsedMu1FD, UsedMu2FD, hKL);
            double UsedAlpha2FD = this.GetAlpha2KroenerDeWitt(UsedMu1FD, UsedMu2FD);
            double UsedBeta1FD = this.FirstDerivativeBeta1DeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD, hKL);
            double UsedBeta2FD = this.FirstDerivativeBeta2KroenerDeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);
            double UsedGammaParameterFD = this.FirstDerivativeGammaParameterKroenerDeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);

            double UsedAlpha = UsedAlpha2 - UsedAlpha1;
            double UsedBeta = UsedBeta2 - UsedBeta1;

            double UsedAlphaFD = UsedAlpha2FD - UsedAlpha1FD;
            double UsedBetaFD = UsedBeta2FD - UsedBeta1FD;

            double[] UsedShearModulus = this.CalculateShearModulus(UsedAlpha, UsedBeta, UsedGammaParameter);
            double UsedShearModulusFD = this.FirstDerivativeShearModulusCubic(UsedAlpha, UsedBeta, UsedGammaParameter, UsedAlphaFD, UsedBetaFD, UsedGammaParameterFD);

            //double Ret = -1.0 / Math.Pow(UsedShearModulus[0], 2);
            double Ret = -0.5 / Math.Pow(UsedShearModulus[0], 2);
            Ret *= UsedShearModulusFD;

            return Ret;
        }

        public double FirstDerivativeC44HS2DeWittCubic(DataManagment.CrystalData.HKLReflex hKL)
        {
            double UsedKappa = this.KappaCubicStiffness;
            double UsedMu1 = this.GetMu1KroenerDeWittCubicStiffness();
            double UsedMu2 = this.GetMu2KroenerDeWittCubicStiffness();

            double UsedKappaFD = this.FirstDerivativeC44KappaCubic();
            double UsedMu1FD = this.FirstDerivativeC44Mu1KroenerDeWittCubic();
            double UsedMu2FD = this.FirstDerivativeC44Mu2KroenerDeWittCubic();

            double UsedAlpha1 = this.GetAlpha1DeWitt(UsedKappa, UsedMu1, UsedMu2, hKL);
            double UsedAlpha2 = this.GetAlpha2KroenerDeWitt(UsedMu1, UsedMu2);
            double UsedBeta1 = this.GetBeta1DeWitt(UsedKappa, UsedMu1, UsedMu2, hKL);
            double UsedBeta2 = this.GetBeta2KroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);
            double UsedGammaParameter = this.GetGammaParameterKroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);

            double UsedAlpha1FD = this.GetAlpha1DeWitt(UsedKappaFD, UsedMu1FD, UsedMu2FD, hKL);
            double UsedAlpha2FD = this.GetAlpha2KroenerDeWitt(UsedMu1FD, UsedMu2FD);
            double UsedBeta1FD = this.FirstDerivativeBeta1DeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD, hKL);
            double UsedBeta2FD = this.FirstDerivativeBeta2KroenerDeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);
            double UsedGammaParameterFD = this.FirstDerivativeGammaParameterKroenerDeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);

            double UsedAlpha = UsedAlpha2 - UsedAlpha1;
            double UsedBeta = UsedBeta2 - UsedBeta1;

            double UsedAlphaFD = UsedAlpha2FD - UsedAlpha1FD;
            double UsedBetaFD = UsedBeta2FD - UsedBeta1FD;

            double[] UsedShearModulus = this.CalculateShearModulus(UsedAlpha, UsedBeta, UsedGammaParameter);
            double UsedShearModulusFD = this.FirstDerivativeShearModulusCubic(UsedAlpha, UsedBeta, UsedGammaParameter, UsedAlphaFD, UsedBetaFD, UsedGammaParameterFD);

            //double Ret = -1.0 / Math.Pow(UsedShearModulus[0], 2);
            double Ret = -0.5 / Math.Pow(UsedShearModulus[0], 2);
            Ret *= UsedShearModulusFD;

            return Ret;
        }

        #endregion

        #endregion

        #region Compliance components

        public double S1DeWittCubicCompliance(DataManagment.CrystalData.HKLReflex hKL)
        {
            double UsedKappa = this.KappaCubicCompliance;
            double UsedMu1 = this.GetMu1KroenerDeWittCubicCompliance();
            double UsedMu2 = this.GetMu2KroenerDeWittCubicCompliance();

            double UsedAlpha1 = this.GetAlpha1DeWitt(UsedKappa, UsedMu1, UsedMu2, hKL);
            double UsedAlpha2 = this.GetAlpha2KroenerDeWitt(UsedMu1, UsedMu2);
            double UsedBeta1 = this.GetBeta1DeWitt(UsedKappa, UsedMu1, UsedMu2, hKL);
            double UsedBeta2 = this.GetBeta2KroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);
            double UsedGammaParameter = this.GetGammaParameterKroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);

            double UsedAlpha = UsedAlpha2 - UsedAlpha1;
            double UsedBeta = UsedBeta2 - UsedBeta1;

            double[] AllShearModulusSolutions = this.CalculateShearModulus(UsedAlpha, UsedBeta, UsedGammaParameter);

            this._deWittShearModulus = AllShearModulusSolutions;

            double UsedShearModulus = this.DeWittShearModulus;

            double Ret = this.S1KroenerDeWittCubic(UsedKappa, UsedShearModulus);

            return Ret;
        }

        public double HS2DeWittCubicCompliance(DataManagment.CrystalData.HKLReflex hKL)
        {
            double UsedKappa = this.KappaCubicCompliance;
            double UsedMu1 = this.GetMu1KroenerDeWittCubicCompliance();
            double UsedMu2 = this.GetMu2KroenerDeWittCubicCompliance();

            double UsedAlpha1 = this.GetAlpha1DeWitt(UsedKappa, UsedMu1, UsedMu2, hKL);
            double UsedAlpha2 = this.GetAlpha2KroenerDeWitt(UsedMu1, UsedMu2);
            double UsedBeta1 = this.GetBeta1DeWitt(UsedKappa, UsedMu1, UsedMu2, hKL);
            double UsedBeta2 = this.GetBeta2KroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);
            double UsedGammaParameter = this.GetGammaParameterKroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);

            double UsedAlpha = UsedAlpha2 - UsedAlpha1;
            double UsedBeta = UsedBeta2 - UsedBeta1;

            double[] AllShearModulusSolutions = this.CalculateShearModulus(UsedAlpha, UsedBeta, UsedGammaParameter);

            this._deWittShearModulus = AllShearModulusSolutions;

            double UsedSHearModulus = this.DeWittShearModulus;

            double Ret = this.HS2KroenerDeWittCubic(UsedSHearModulus);

            return Ret;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] C11
        ///[1] C12
        ///[2] C44
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorDeWittCubicComplianceClassic(double Lambda)
        {
            //[0][0] C11
            //[1][1] C12
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

            //[0] Aclivity
            //[1] Constant
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);

            for (int n = 0; n < this.DiffractionConstants.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeS11S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 0] += (this.FirstDerivativeS11HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeS12S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 1] += (this.FirstDerivativeS12HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeS12S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeS12HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeS12S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeS12HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeS44S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS44S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 2] += (this.FirstDerivativeS44HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS44HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeS44S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeS44HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeS44S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeS44HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeS44S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeS44HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeS44S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeS44HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                #endregion

                #region Vector build

                SolutionVector[0] += ((this.DiffractionConstants[n].ClassicS1 - this.S1DeWittCubicCompliance(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeS11S1DeWittCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[0] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2DeWittCubicCompliance(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeS11HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[1] += ((this.DiffractionConstants[n].ClassicS1 - this.S1DeWittCubicCompliance(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeS12S1DeWittCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[1] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2DeWittCubicCompliance(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeS12HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[2] += ((this.DiffractionConstants[n].ClassicS1 - this.S1DeWittCubicCompliance(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeS44S1DeWittCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[2] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2DeWittCubicCompliance(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeS44HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex);

                #endregion
            }

            for (int n = 0; n < 3; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] C11
        ///[1] C12
        ///[2] C44
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorDeWittCubicComplianceMacroscopic(double Lambda)
        {
            //[0][0] C11
            //[1][1] C12
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

            //[0] Aclivity
            //[1] Constant
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);

            for (int n = 0; n < this.DiffractionConstants.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeS11S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 0] += (this.FirstDerivativeS11HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeS12S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 1] += (this.FirstDerivativeS12HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeS12S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeS12HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeS12S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeS12HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeS44S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS44S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 2] += (this.FirstDerivativeS44HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS44HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeS44S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeS44HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeS44S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeS44HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeS44S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeS44HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeS44S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeS44HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                #endregion

                #region Vector build

                SolutionVector[0] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1DeWittCubicCompliance(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeS11S1DeWittCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[0] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2DeWittCubicCompliance(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeS11HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[1] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1DeWittCubicCompliance(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeS12S1DeWittCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[1] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2DeWittCubicCompliance(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeS12HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[2] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1DeWittCubicCompliance(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeS44S1DeWittCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[2] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2DeWittCubicCompliance(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeS44HS2DeWittCubic(this.DiffractionConstants[n].UsedReflex);

                #endregion
            }

            for (int n = 0; n < 3; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        #region First derivative

        public double FirstDerivativeS11S1DeWittCubic(DataManagment.CrystalData.HKLReflex hKL)
        {
            double UsedKappa = this.KappaCubicCompliance;
            double UsedMu1 = this.GetMu1KroenerDeWittCubicCompliance();
            double UsedMu2 = this.GetMu2KroenerDeWittCubicCompliance();

            double UsedKappaFD = this.FirstDerivativeS11KappaCubic();
            double UsedMu1FD = this.FirstDerivativeS11Mu1KroenerDeWittCubic();
            double UsedMu2FD = this.FirstDerivativeS11Mu2KroenerDeWittCubic();

            double UsedAlpha1 = this.GetAlpha1DeWitt(UsedKappa, UsedMu1, UsedMu2, hKL);
            double UsedAlpha2 = this.GetAlpha2KroenerDeWitt(UsedMu1, UsedMu2);
            double UsedBeta1 = this.GetBeta1DeWitt(UsedKappa, UsedMu1, UsedMu2, hKL);
            double UsedBeta2 = this.GetBeta2KroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);
            double UsedGammaParameter = this.GetGammaParameterKroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);

            double UsedAlpha1FD = this.GetAlpha1DeWitt(UsedKappaFD, UsedMu1FD, UsedMu2FD, hKL);
            double UsedAlpha2FD = this.GetAlpha2KroenerDeWitt(UsedMu1FD, UsedMu2FD);
            double UsedBeta1FD = this.FirstDerivativeBeta1DeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD, hKL);
            double UsedBeta2FD = this.FirstDerivativeBeta2KroenerDeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);
            double UsedGammaParameterFD = this.FirstDerivativeGammaParameterKroenerDeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);

            double UsedAlpha = UsedAlpha2 - UsedAlpha1;
            double UsedBeta = UsedBeta2 - UsedBeta1;

            double UsedAlphaFD = UsedAlpha2FD - UsedAlpha1FD;
            double UsedBetaFD = UsedBeta2FD - UsedBeta1FD;

            double[] UsedShearModulus = this.CalculateShearModulus(UsedAlpha, UsedBeta, UsedGammaParameter);
            double UsedShearModulusFD = this.FirstDerivativeShearModulusCubic(UsedAlpha, UsedBeta, UsedGammaParameter, UsedAlphaFD, UsedBetaFD, UsedGammaParameterFD);

            #region Linear

            //double Ret = UsedKappaFD / 9.0;
            //Ret -= UsedShearModulusFD / 6.0;

            #endregion

            #region Inverse

            double Ret = FirstDerivativeS1ConstantsKroenerDeWittCubic(UsedKappa, UsedKappaFD, UsedShearModulus[0], UsedShearModulusFD);

            #endregion

            return Ret;
        }

        public double FirstDerivativeS12S1DeWittCubic(DataManagment.CrystalData.HKLReflex hKL)
        {
            double UsedKappa = this.KappaCubicCompliance;
            double UsedMu1 = this.GetMu1KroenerDeWittCubicCompliance();
            double UsedMu2 = this.GetMu2KroenerDeWittCubicCompliance();

            double UsedKappaFD = this.FirstDerivativeS12KappaCubic();
            double UsedMu1FD = this.FirstDerivativeS12Mu1KroenerDeWittCubic();
            double UsedMu2FD = this.FirstDerivativeS12Mu2KroenerDeWittCubic();

            double UsedAlpha1 = this.GetAlpha1DeWitt(UsedKappa, UsedMu1, UsedMu2, hKL);
            double UsedAlpha2 = this.GetAlpha2KroenerDeWitt(UsedMu1, UsedMu2);
            double UsedBeta1 = this.GetBeta1DeWitt(UsedKappa, UsedMu1, UsedMu2, hKL);
            double UsedBeta2 = this.GetBeta2KroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);
            double UsedGammaParameter = this.GetGammaParameterKroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);

            double UsedAlpha1FD = this.GetAlpha1DeWitt(UsedKappaFD, UsedMu1FD, UsedMu2FD, hKL);
            double UsedAlpha2FD = this.GetAlpha2KroenerDeWitt(UsedMu1FD, UsedMu2FD);
            double UsedBeta1FD = this.FirstDerivativeBeta1DeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD, hKL);
            double UsedBeta2FD = this.FirstDerivativeBeta2KroenerDeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);
            double UsedGammaParameterFD = this.FirstDerivativeGammaParameterKroenerDeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);

            double UsedAlpha = UsedAlpha2 - UsedAlpha1;
            double UsedBeta = UsedBeta2 - UsedBeta1;

            double UsedAlphaFD = UsedAlpha2FD - UsedAlpha1FD;
            double UsedBetaFD = UsedBeta2FD - UsedBeta1FD;

            double[] UsedShearModulus = this.CalculateShearModulus(UsedAlpha, UsedBeta, UsedGammaParameter);
            double UsedShearModulusFD = this.FirstDerivativeShearModulusCubic(UsedAlpha, UsedBeta, UsedGammaParameter, UsedAlphaFD, UsedBetaFD, UsedGammaParameterFD);

            #region Linear

            //double Ret = UsedKappaFD / 9.0;
            //Ret -= UsedShearModulusFD / 6.0;

            #endregion

            #region Inverse

            double Ret = FirstDerivativeS1ConstantsKroenerDeWittCubic(UsedKappa, UsedKappaFD, UsedShearModulus[0], UsedShearModulusFD);

            #endregion

            return Ret;
        }

        public double FirstDerivativeS44S1DeWittCubic(DataManagment.CrystalData.HKLReflex hKL)
        {
            double UsedKappa = this.KappaCubicCompliance;
            double UsedMu1 = this.GetMu1KroenerDeWittCubicCompliance();
            double UsedMu2 = this.GetMu2KroenerDeWittCubicCompliance();

            double UsedKappaFD = this.FirstDerivativeS44KappaCubic();
            double UsedMu1FD = this.FirstDerivativeS44Mu1KroenerDeWittCubic();
            double UsedMu2FD = this.FirstDerivativeS44Mu2KroenerDeWittCubic();

            double UsedAlpha1 = this.GetAlpha1DeWitt(UsedKappa, UsedMu1, UsedMu2, hKL);
            double UsedAlpha2 = this.GetAlpha2KroenerDeWitt(UsedMu1, UsedMu2);
            double UsedBeta1 = this.GetBeta1DeWitt(UsedKappa, UsedMu1, UsedMu2, hKL);
            double UsedBeta2 = this.GetBeta2KroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);
            double UsedGammaParameter = this.GetGammaParameterKroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);

            double UsedAlpha1FD = this.GetAlpha1DeWitt(UsedKappaFD, UsedMu1FD, UsedMu2FD, hKL);
            double UsedAlpha2FD = this.GetAlpha2KroenerDeWitt(UsedMu1FD, UsedMu2FD);
            double UsedBeta1FD = this.FirstDerivativeBeta1DeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD, hKL);
            double UsedBeta2FD = this.FirstDerivativeBeta2KroenerDeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);
            double UsedGammaParameterFD = this.FirstDerivativeGammaParameterKroenerDeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);

            double UsedAlpha = UsedAlpha2 - UsedAlpha1;
            double UsedBeta = UsedBeta2 - UsedBeta1;

            double UsedAlphaFD = UsedAlpha2FD - UsedAlpha1FD;
            double UsedBetaFD = UsedBeta2FD - UsedBeta1FD;

            double[] UsedShearModulus = this.CalculateShearModulus(UsedAlpha, UsedBeta, UsedGammaParameter);
            double UsedShearModulusFD = this.FirstDerivativeShearModulusCubic(UsedAlpha, UsedBeta, UsedGammaParameter, UsedAlphaFD, UsedBetaFD, UsedGammaParameterFD);

            #region Linear

            //double Ret = UsedKappaFD / 9.0;
            //Ret -= UsedShearModulusFD / 6.0;

            #endregion

            #region Inverse

            double Ret = FirstDerivativeS1ConstantsKroenerDeWittCubic(UsedKappa, UsedKappaFD, UsedShearModulus[0], UsedShearModulusFD);

            #endregion

            return Ret;
        }

        public double FirstDerivativeS11HS2DeWittCubic(DataManagment.CrystalData.HKLReflex hKL)
        {
            double UsedKappa = this.KappaCubicCompliance;
            double UsedMu1 = this.GetMu1KroenerDeWittCubicCompliance();
            double UsedMu2 = this.GetMu2KroenerDeWittCubicCompliance();

            double UsedKappaFD = this.FirstDerivativeS11KappaCubic();
            double UsedMu1FD = this.FirstDerivativeS11Mu1KroenerDeWittCubic();
            double UsedMu2FD = this.FirstDerivativeS11Mu2KroenerDeWittCubic();

            double UsedAlpha1 = this.GetAlpha1DeWitt(UsedKappa, UsedMu1, UsedMu2, hKL);
            double UsedAlpha2 = this.GetAlpha2KroenerDeWitt(UsedMu1, UsedMu2);
            double UsedBeta1 = this.GetBeta1DeWitt(UsedKappa, UsedMu1, UsedMu2, hKL);
            double UsedBeta2 = this.GetBeta2KroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);
            double UsedGammaParameter = this.GetGammaParameterKroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);

            double UsedAlpha1FD = this.GetAlpha1DeWitt(UsedKappaFD, UsedMu1FD, UsedMu2FD, hKL);
            double UsedAlpha2FD = this.GetAlpha2KroenerDeWitt(UsedMu1FD, UsedMu2FD);
            double UsedBeta1FD = this.FirstDerivativeBeta1DeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD, hKL);
            double UsedBeta2FD = this.FirstDerivativeBeta2KroenerDeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);
            double UsedGammaParameterFD = this.FirstDerivativeGammaParameterKroenerDeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);

            double UsedAlpha = UsedAlpha2 - UsedAlpha1;
            double UsedBeta = UsedBeta2 - UsedBeta1;

            double UsedAlphaFD = UsedAlpha2FD - UsedAlpha1FD;
            double UsedBetaFD = UsedBeta2FD - UsedBeta1FD;

            double[] UsedShearModulus = this.CalculateShearModulus(UsedAlpha, UsedBeta, UsedGammaParameter);
            double UsedShearModulusFD = this.FirstDerivativeShearModulusCubic(UsedAlpha, UsedBeta, UsedGammaParameter, UsedAlphaFD, UsedBetaFD, UsedGammaParameterFD);

            //double Ret = -1.0 / Math.Pow(UsedShearModulus[0], 2);
            double Ret = -0.5 / Math.Pow(UsedShearModulus[0], 2);
            Ret *= UsedShearModulusFD;

            return Ret;
        }

        public double FirstDerivativeS12HS2DeWittCubic(DataManagment.CrystalData.HKLReflex hKL)
        {
            double UsedKappa = this.KappaCubicCompliance;
            double UsedMu1 = this.GetMu1KroenerDeWittCubicCompliance();
            double UsedMu2 = this.GetMu2KroenerDeWittCubicCompliance();

            double UsedKappaFD = this.FirstDerivativeS12KappaCubic();
            double UsedMu1FD = this.FirstDerivativeS12Mu1KroenerDeWittCubic();
            double UsedMu2FD = this.FirstDerivativeS12Mu2KroenerDeWittCubic();

            double UsedAlpha1 = this.GetAlpha1DeWitt(UsedKappa, UsedMu1, UsedMu2, hKL);
            double UsedAlpha2 = this.GetAlpha2KroenerDeWitt(UsedMu1, UsedMu2);
            double UsedBeta1 = this.GetBeta1DeWitt(UsedKappa, UsedMu1, UsedMu2, hKL);
            double UsedBeta2 = this.GetBeta2KroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);
            double UsedGammaParameter = this.GetGammaParameterKroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);

            double UsedAlpha1FD = this.GetAlpha1DeWitt(UsedKappaFD, UsedMu1FD, UsedMu2FD, hKL);
            double UsedAlpha2FD = this.GetAlpha2KroenerDeWitt(UsedMu1FD, UsedMu2FD);
            double UsedBeta1FD = this.FirstDerivativeBeta1DeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD, hKL);
            double UsedBeta2FD = this.FirstDerivativeBeta2KroenerDeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);
            double UsedGammaParameterFD = this.FirstDerivativeGammaParameterKroenerDeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);

            double UsedAlpha = UsedAlpha2 - UsedAlpha1;
            double UsedBeta = UsedBeta2 - UsedBeta1;

            double UsedAlphaFD = UsedAlpha2FD - UsedAlpha1FD;
            double UsedBetaFD = UsedBeta2FD - UsedBeta1FD;

            double[] UsedShearModulus = this.CalculateShearModulus(UsedAlpha, UsedBeta, UsedGammaParameter);
            double UsedShearModulusFD = this.FirstDerivativeShearModulusCubic(UsedAlpha, UsedBeta, UsedGammaParameter, UsedAlphaFD, UsedBetaFD, UsedGammaParameterFD);

            //double Ret = -1.0 / Math.Pow(UsedShearModulus[0], 2);
            double Ret = -0.5 / Math.Pow(UsedShearModulus[0], 2);
            Ret *= UsedShearModulusFD;

            return Ret;
        }

        public double FirstDerivativeS44HS2DeWittCubic(DataManagment.CrystalData.HKLReflex hKL)
        {
            double UsedKappa = this.KappaCubicCompliance;
            double UsedMu1 = this.GetMu1KroenerDeWittCubicCompliance();
            double UsedMu2 = this.GetMu2KroenerDeWittCubicCompliance();

            double UsedKappaFD = this.FirstDerivativeS44KappaCubic();
            double UsedMu1FD = this.FirstDerivativeS44Mu1KroenerDeWittCubic();
            double UsedMu2FD = this.FirstDerivativeS44Mu2KroenerDeWittCubic();

            double UsedAlpha1 = this.GetAlpha1DeWitt(UsedKappa, UsedMu1, UsedMu2, hKL);
            double UsedAlpha2 = this.GetAlpha2KroenerDeWitt(UsedMu1, UsedMu2);
            double UsedBeta1 = this.GetBeta1DeWitt(UsedKappa, UsedMu1, UsedMu2, hKL);
            double UsedBeta2 = this.GetBeta2KroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);
            double UsedGammaParameter = this.GetGammaParameterKroenerDeWitt(UsedKappa, UsedMu1, UsedMu2);

            double UsedAlpha1FD = this.GetAlpha1DeWitt(UsedKappaFD, UsedMu1FD, UsedMu2FD, hKL);
            double UsedAlpha2FD = this.GetAlpha2KroenerDeWitt(UsedMu1FD, UsedMu2FD);
            double UsedBeta1FD = this.FirstDerivativeBeta1DeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD, hKL);
            double UsedBeta2FD = this.FirstDerivativeBeta2KroenerDeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);
            double UsedGammaParameterFD = this.FirstDerivativeGammaParameterKroenerDeWitt(UsedKappa, UsedMu1, UsedMu2, UsedKappaFD, UsedMu1FD, UsedMu2FD);

            double UsedAlpha = UsedAlpha2 - UsedAlpha1;
            double UsedBeta = UsedBeta2 - UsedBeta1;

            double UsedAlphaFD = UsedAlpha2FD - UsedAlpha1FD;
            double UsedBetaFD = UsedBeta2FD - UsedBeta1FD;

            double[] UsedShearModulus = this.CalculateShearModulus(UsedAlpha, UsedBeta, UsedGammaParameter);
            double UsedShearModulusFD = this.FirstDerivativeShearModulusCubic(UsedAlpha, UsedBeta, UsedGammaParameter, UsedAlphaFD, UsedBetaFD, UsedGammaParameterFD);

            //double Ret = -1.0 / Math.Pow(UsedShearModulus[0], 2);
            double Ret = -0.5 / Math.Pow(UsedShearModulus[0], 2);
            Ret *= UsedShearModulusFD;

            return Ret;
        }

        #endregion

        #endregion

        #endregion

        #endregion

        #endregion

        #region Geometric Hill

        #region Cubic

        public double S1GeometricHillCubic(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = Math.Sqrt(this.S1ReussCubic(hKL) * this.S1VoigtCubicCompliance());

            return Ret;
        }

        public double HS2GeometricHillCubic(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = Math.Sqrt(this.HS2ReussCubic(hKL) * this.HS2VoigtCubicCompliance());

            return Ret;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] C11
        ///[1] C12
        ///[2] C44
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorGeometricHillCubicClassic(double Lambda)
        {
            //[0][0] C11
            //[1][1] C12
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

            //[0] Aclivity
            //[1] Constant
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);

            for (int n = 0; n < this.DiffractionConstants.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeS11S1GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 0] += (this.FirstDerivativeS11HS2GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeS12S1GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 1] += (this.FirstDerivativeS12HS2GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeS12S1GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeS12HS2GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeS12S1GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeS12HS2GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeS44S1GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS44S1GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 2] += (this.FirstDerivativeS44HS2GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS44HS2GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeS44S1GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeS44HS2GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeS44S1GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeS44HS2GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeS44S1GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeS44HS2GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeS44S1GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeS44HS2GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                #endregion

                #region Vector build

                SolutionVector[0] += ((this.DiffractionConstants[n].ClassicS1 - this.S1GeometricHillCubic(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeS11S1GeometricHillCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[0] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2GeometricHillCubic(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeS11HS2GeometricHillCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[1] += ((this.DiffractionConstants[n].ClassicS1 - this.S1GeometricHillCubic(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeS12S1GeometricHillCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[1] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2GeometricHillCubic(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeS12HS2GeometricHillCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[2] += ((this.DiffractionConstants[n].ClassicS1 - this.S1GeometricHillCubic(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeS44S1GeometricHillCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[2] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2GeometricHillCubic(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeS44HS2GeometricHillCubic(this.DiffractionConstants[n].UsedReflex);

                #endregion
            }

            for (int n = 0; n < 3; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] C11
        ///[1] C12
        ///[2] C44
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorGeometricHillCubicMacroscopic(double Lambda)
        {
            //[0][0] C11
            //[1][1] C12
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

            //[0] Aclivity
            //[1] Constant
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);

            for (int n = 0; n < this.DiffractionConstants.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeS11S1GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 0] += (this.FirstDerivativeS11HS2GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeS12S1GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 1] += (this.FirstDerivativeS12HS2GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeS12S1GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeS12HS2GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeS12S1GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeS12HS2GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeS44S1GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS44S1GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 2] += (this.FirstDerivativeS44HS2GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS44HS2GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeS44S1GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeS44HS2GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeS44S1GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeS44HS2GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeS44S1GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeS44HS2GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeS44S1GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeS44HS2GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2GeometricHillCubic(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                #endregion

                #region Vector build

                SolutionVector[0] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1GeometricHillCubic(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeS11S1GeometricHillCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[0] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2GeometricHillCubic(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeS11HS2GeometricHillCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[1] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1GeometricHillCubic(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeS12S1GeometricHillCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[1] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2GeometricHillCubic(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeS12HS2GeometricHillCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[2] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1GeometricHillCubic(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeS44S1GeometricHillCubic(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[2] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2GeometricHillCubic(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeS44HS2GeometricHillCubic(this.DiffractionConstants[n].UsedReflex);

                #endregion
            }

            for (int n = 0; n < 3; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        #region First derivative

        public double FirstDerivativeS11S1GeometricHillCubic(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = this.FirstDerivativeS11S1ReussCubic(hKL) * this.S1VoigtCubic();
            Ret += this.S1ReussCubic(hKL) * this.FirstDerivativeS11S1VoigtCubic();
            Ret /= Math.Sqrt(this.S1ReussCubic(hKL) * this.S1VoigtCubic());

            return Ret;
        }

        public double FirstDerivativeS12S1GeometricHillCubic(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = this.FirstDerivativeS12S1ReussCubic(hKL) * this.S1VoigtCubic();
            Ret += this.S1ReussCubic(hKL) * this.FirstDerivativeS12S1VoigtCubic();
            Ret /= Math.Sqrt(this.S1ReussCubic(hKL) * this.S1VoigtCubic());

            return Ret;
        }

        public double FirstDerivativeS44S1GeometricHillCubic(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = this.FirstDerivativeS44S1ReussCubic(hKL) * this.S1VoigtCubic();
            Ret += this.S1ReussCubic(hKL) * this.FirstDerivativeS44S1VoigtCubic();
            Ret /= Math.Sqrt(this.S1ReussCubic(hKL) * this.S1VoigtCubic());

            return Ret;
        }

        public double FirstDerivativeS11HS2GeometricHillCubic(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = this.FirstDerivativeS11HS2ReussCubic(hKL) * this.HS2VoigtCubic();
            Ret += this.HS2ReussCubic(hKL) * this.FirstDerivativeS11HS2VoigtCubic();
            Ret /= Math.Sqrt(this.HS2ReussCubic(hKL) * this.HS2VoigtCubic());

            return Ret;
        }

        public double FirstDerivativeS12HS2GeometricHillCubic(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = this.FirstDerivativeS12HS2ReussCubic(hKL) * this.HS2VoigtCubic();
            Ret += this.HS2ReussCubic(hKL) * this.FirstDerivativeS12HS2VoigtCubic();
            Ret /= Math.Sqrt(this.HS2ReussCubic(hKL) * this.HS2VoigtCubic());

            return Ret;
        }

        public double FirstDerivativeS44HS2GeometricHillCubic(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = this.FirstDerivativeS44HS2ReussCubic(hKL) * this.HS2VoigtCubic();
            Ret += this.HS2ReussCubic(hKL) * this.FirstDerivativeS44HS2VoigtCubic();
            Ret /= Math.Sqrt(this.HS2ReussCubic(hKL) * this.HS2VoigtCubic());

            return Ret;
        }

        #endregion

        #endregion

        #region Hexagonal

        public double S1GeometricHillHexagonal(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = Math.Sqrt(this.S1ReussHexagonal(hKL) * this.S1VoigtType1Compliance());

            return Ret;
        }

        public double HS2GeometricHillHexagonal(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = Math.Sqrt(this.HS2ReussHexagonal(hKL) * this.HS2VoigtType1Compliance());

            return Ret;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] C11
        ///[1] C33
        ///[2] C12
        ///[3] C13
        ///[4] C44
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorGeometricHillHexagonalClassic(double Lambda)
        {
            //[0][0] C11
            //[1][1] C12
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(5, 5, 0.0);

            //[0] Aclivity
            //[1] Constant
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(5);

            for (int n = 0; n < this.DiffractionConstants.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeS11S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 0] += (this.FirstDerivativeS11HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeS33S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 1] += (this.FirstDerivativeS33HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeS33S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeS33HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeS33S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeS33HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeS12S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 2] += (this.FirstDerivativeS12HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeS12S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeS12HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeS12S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeS12HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeS12S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeS12HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeS12S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeS12HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[3, 3] += (this.FirstDerivativeS13S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 3] += (this.FirstDerivativeS13HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 3] += (this.FirstDerivativeS13S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 3] += (this.FirstDerivativeS13HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[3, 0] += (this.FirstDerivativeS13S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 0] += (this.FirstDerivativeS13HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 3] += (this.FirstDerivativeS13S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 3] += (this.FirstDerivativeS13HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[3, 1] += (this.FirstDerivativeS13S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 1] += (this.FirstDerivativeS13HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 3] += (this.FirstDerivativeS13S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 3] += (this.FirstDerivativeS13HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[3, 2] += (this.FirstDerivativeS13S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 2] += (this.FirstDerivativeS13HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                HessianMatrix[4, 4] += (this.FirstDerivativeS44S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS44S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 4] += (this.FirstDerivativeS44HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS44HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[0, 4] += (this.FirstDerivativeS44S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[0, 4] += (this.FirstDerivativeS44HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[4, 0] += (this.FirstDerivativeS44S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 0] += (this.FirstDerivativeS44HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[1, 4] += (this.FirstDerivativeS44S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[1, 4] += (this.FirstDerivativeS44HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[4, 1] += (this.FirstDerivativeS44S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 1] += (this.FirstDerivativeS44HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[2, 4] += (this.FirstDerivativeS44S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[2, 4] += (this.FirstDerivativeS44HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[4, 2] += (this.FirstDerivativeS44S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 2] += (this.FirstDerivativeS44HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[3, 4] += (this.FirstDerivativeS44S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[3, 4] += (this.FirstDerivativeS44HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));
                HessianMatrix[4, 3] += (this.FirstDerivativeS44S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2));
                HessianMatrix[4, 3] += (this.FirstDerivativeS44HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2));

                #endregion

                #region Vector build

                SolutionVector[0] += ((this.DiffractionConstants[n].ClassicS1 - this.S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeS11S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[0] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeS11HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[1] += ((this.DiffractionConstants[n].ClassicS1 - this.S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeS33S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[1] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeS33HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[2] += ((this.DiffractionConstants[n].ClassicS1 - this.S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeS12S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[2] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeS12HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[3] += ((this.DiffractionConstants[n].ClassicS1 - this.S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeS13S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[3] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeS13HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[4] += ((this.DiffractionConstants[n].ClassicS1 - this.S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicS1Error, 2)) * this.FirstDerivativeS44S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[4] += ((this.DiffractionConstants[n].ClassicHS2 - this.HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].ClassicHS2Error, 2)) * this.FirstDerivativeS44HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex);

                #endregion
            }

            for (int n = 0; n < 5; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(5);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] C11
        ///[1] C33
        ///[2] C12
        ///[3] C13
        ///[4] C44
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorGeometricHillHexagonalMacroscopic(double Lambda)
        {
            //[0][0] C11
            //[1][1] C12
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(5, 5, 0.0);

            //[0] Aclivity
            //[1] Constant
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(5);

            for (int n = 0; n < this.DiffractionConstants.Count; n++)
            {
                #region Matrix Build

                HessianMatrix[0, 0] += (this.FirstDerivativeS11S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 0] += (this.FirstDerivativeS11HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[1, 1] += (this.FirstDerivativeS33S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 1] += (this.FirstDerivativeS33HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeS33S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 1] += (this.FirstDerivativeS33HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeS33S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 0] += (this.FirstDerivativeS33HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[2, 2] += (this.FirstDerivativeS12S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 2] += (this.FirstDerivativeS12HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeS12S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 2] += (this.FirstDerivativeS12HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeS12S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 0] += (this.FirstDerivativeS12HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeS12S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 2] += (this.FirstDerivativeS12HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeS12S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 1] += (this.FirstDerivativeS12HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[3, 3] += (this.FirstDerivativeS13S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 3] += (this.FirstDerivativeS13HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 3] += (this.FirstDerivativeS13S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 3] += (this.FirstDerivativeS13HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[3, 0] += (this.FirstDerivativeS13S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 0] += (this.FirstDerivativeS13HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 3] += (this.FirstDerivativeS13S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 3] += (this.FirstDerivativeS13HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[3, 1] += (this.FirstDerivativeS13S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 1] += (this.FirstDerivativeS13HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 3] += (this.FirstDerivativeS13S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 3] += (this.FirstDerivativeS13HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[3, 2] += (this.FirstDerivativeS13S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 2] += (this.FirstDerivativeS13HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                HessianMatrix[4, 4] += (this.FirstDerivativeS44S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS44S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 4] += (this.FirstDerivativeS44HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS44HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[0, 4] += (this.FirstDerivativeS44S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[0, 4] += (this.FirstDerivativeS44HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[4, 0] += (this.FirstDerivativeS44S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 0] += (this.FirstDerivativeS44HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS11HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[1, 4] += (this.FirstDerivativeS44S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[1, 4] += (this.FirstDerivativeS44HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[4, 1] += (this.FirstDerivativeS44S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 1] += (this.FirstDerivativeS44HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS33HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[2, 4] += (this.FirstDerivativeS44S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[2, 4] += (this.FirstDerivativeS44HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[4, 2] += (this.FirstDerivativeS44S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 2] += (this.FirstDerivativeS44HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS12HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[3, 4] += (this.FirstDerivativeS44S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[3, 4] += (this.FirstDerivativeS44HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));
                HessianMatrix[4, 3] += (this.FirstDerivativeS44S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2));
                HessianMatrix[4, 3] += (this.FirstDerivativeS44HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) * this.FirstDerivativeS13HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2));

                #endregion

                #region Vector build

                SolutionVector[0] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeS11S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[0] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeS11HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[1] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeS33S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[1] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeS33HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[2] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeS12S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[2] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeS12HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[3] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeS13S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[3] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeS13HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[4] += ((this.DiffractionConstants[n].MacroscopicS1 - this.S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicS1Error, 2)) * this.FirstDerivativeS44S1GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex);
                SolutionVector[4] += ((this.DiffractionConstants[n].MacroscopicHS2 - this.HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex)) / Math.Pow(this.DiffractionConstants[n].MacroscopicHS2Error, 2)) * this.FirstDerivativeS44HS2GeometricHillHexagonal(this.DiffractionConstants[n].UsedReflex);

                #endregion
            }

            for (int n = 0; n < 5; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(5);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        #region First derivative

        public double FirstDerivativeS11S1GeometricHillHexagonal(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = this.FirstDerivativeS11S1ReussHexagonal(hKL) * this.S1VoigtType1();
            Ret += this.S1ReussHexagonal(hKL) * this.FirstDerivativeS11S1VoigtType1();
            Ret /= Math.Sqrt(this.S1ReussHexagonal(hKL) * this.S1VoigtType1());

            return Ret;
        }

        public double FirstDerivativeS33S1GeometricHillHexagonal(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = this.FirstDerivativeS33S1ReussHexagonal(hKL) * this.S1VoigtType1();
            Ret += this.S1ReussHexagonal(hKL) * this.FirstDerivativeS33S1VoigtType1();
            Ret /= Math.Sqrt(this.S1ReussHexagonal(hKL) * this.S1VoigtType1());

            return Ret;
        }

        public double FirstDerivativeS44S1GeometricHillHexagonal(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = this.FirstDerivativeS44S1ReussHexagonal(hKL) * this.S1VoigtType1();
            Ret += this.S1ReussHexagonal(hKL) * this.FirstDerivativeS44S1VoigtType1();
            Ret /= Math.Sqrt(this.S1ReussHexagonal(hKL) * this.S1VoigtType1());

            return Ret;
        }

        public double FirstDerivativeS12S1GeometricHillHexagonal(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = this.FirstDerivativeS12S1ReussHexagonal(hKL) * this.S1VoigtType1();
            Ret += this.S1ReussHexagonal(hKL) * this.FirstDerivativeS12S1VoigtType1();
            Ret /= Math.Sqrt(this.S1ReussHexagonal(hKL) * this.S1VoigtType1());

            return Ret;
        }

        public double FirstDerivativeS13S1GeometricHillHexagonal(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = this.FirstDerivativeS13S1ReussHexagonal(hKL) * this.S1VoigtType1();
            Ret += this.S1ReussHexagonal(hKL) * this.FirstDerivativeS13S1VoigtType1();
            Ret /= Math.Sqrt(this.S1ReussHexagonal(hKL) * this.S1VoigtType1());

            return Ret;
        }

        public double FirstDerivativeS11HS2GeometricHillHexagonal(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = this.FirstDerivativeS11HS2ReussHexagonal(hKL) * this.HS2VoigtType1();
            Ret += this.HS2ReussHexagonal(hKL) * this.FirstDerivativeS11HS2VoigtType1();
            Ret /= Math.Sqrt(this.HS2ReussHexagonal(hKL) * this.HS2VoigtType1());

            return Ret;
        }

        public double FirstDerivativeS33HS2GeometricHillHexagonal(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = this.FirstDerivativeS33HS2ReussHexagonal(hKL) * this.HS2VoigtType1();
            Ret += this.HS2ReussHexagonal(hKL) * this.FirstDerivativeS33HS2VoigtType1();
            Ret /= Math.Sqrt(this.HS2ReussHexagonal(hKL) * this.HS2VoigtType1());

            return Ret;
        }

        public double FirstDerivativeS12HS2GeometricHillHexagonal(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = this.FirstDerivativeS12HS2ReussHexagonal(hKL) * this.HS2VoigtType1();
            Ret += this.HS2ReussHexagonal(hKL) * this.FirstDerivativeS12HS2VoigtType1();
            Ret /= Math.Sqrt(this.HS2ReussHexagonal(hKL) * this.HS2VoigtType1());

            return Ret;
        }

        public double FirstDerivativeS13HS2GeometricHillHexagonal(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = this.FirstDerivativeS13HS2ReussHexagonal(hKL) * this.HS2VoigtType1();
            Ret += this.HS2ReussHexagonal(hKL) * this.FirstDerivativeS13HS2VoigtType1();
            Ret /= Math.Sqrt(this.HS2ReussHexagonal(hKL) * this.HS2VoigtType1());

            return Ret;
        }

        public double FirstDerivativeS44HS2GeometricHillHexagonal(DataManagment.CrystalData.HKLReflex hKL)
        {
            double Ret = this.FirstDerivativeS44HS2ReussHexagonal(hKL) * this.HS2VoigtType1();
            Ret += this.HS2ReussHexagonal(hKL) * this.FirstDerivativeS44HS2VoigtType1();
            Ret /= Math.Sqrt(this.HS2ReussHexagonal(hKL) * this.HS2VoigtType1());

            return Ret;
        }

        #endregion

        #endregion

        #endregion

        #endregion

        #region Cloning

        public object Clone()
        {
            ElasticityTensors Ret = new ElasticityTensors();

            Ret._complianceTensor = this._complianceTensor.Clone();
            Ret._stiffnessTensor = this._stiffnessTensor.Clone();

            Ret._complianceTensorError = this._complianceTensorError.Clone();
            Ret._stiffnessTensorError = this._stiffnessTensorError.Clone();

            Ret._symmetry = this._symmetry;

            Ret.IsIsotropic = this.IsIsotropic;

            Ret.DiffractionConstants = new List<REK>();

            Ret._kroenerShearModulus = this._kroenerShearModulus;
            Ret._deWittShearModulus = this._deWittShearModulus;

            for (int n = 0; n < this.DiffractionConstants.Count; n++)
            {
                Ret.DiffractionConstants.Add(this.DiffractionConstants[n].Clone() as REK);
            }

            return Ret;
        }

        #endregion

    }
}
