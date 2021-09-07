///////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////Im Gedenken an Tepi//////////////////////////////////////
//////////////////////Das Leben ist wie eine Reise in totaler Dunkelheit://////////////////////
/////Man weiß wie wo der nächste Schritt hinführt, aber jeder findet irgendwann das Licht//////
///////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalScec.Analysis.Peaks.Functions
{
    
    public class PeakRegionFunction : List<PeakFunction>, ICloneable
    {
        #region Parameters

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

        /// <summary>
        /// Use as follows:
        /// 0 = Gaussian function
        /// 1 = Lorentz function
        /// 2 = Pseudo Voigt function
        /// </summary>
        public int functionType
        {
            get
            {
                return this[0].functionType;
            }
            set
            {
                for (int n = 0; n < this.Count; n++)
                {
                    this[n].functionType = value;
                }
            }
        }

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
        public BackgroundPolynomial PolynomialBackgroundFunction;

        public double Chi2Function
        {
            get
            {
                return Analysis.Fitting.Chi2.Chi2PeakRegion(this);
            }
        }

        public double ReducedChi2Function
        {
            get
            {
                return Analysis.Fitting.Chi2.ReducedChi2PeakRegion(this);
            }
        }

        public System.Threading.ManualResetEvent _doneEvent;

        /// <summary>
        /// If positive this lamdba is used in LMA as starting parameter
        /// </summary>
        public double startingLambda;

        private bool _fitConverged;
        public bool FitConverged
        {
            get
            {
                return this._fitConverged;
            }
        }

        public string Position
        {
            get
            {
                string ret = Convert.ToString(this.FittingCounts[0][0].ToString("F3")) + " - " + Convert.ToString(this.FittingCounts[this.FittingCounts.Count - 1][0].ToString("F3"));

                return ret;
            }
        }

        #endregion

        public PeakRegionFunction()
        {

        }

        public PeakRegionFunction(double EstimatedConstantBackground, Pattern.Counts fittingData, PeakFunction PF)
        {
            this.functionType = 2;
            this.backgroundSwitch = true;
            this.backgroundFit = true;

            this.FittingCounts = fittingData;
            this.PolynomialBackgroundFunction = new BackgroundPolynomial(PF.Angle, EstimatedConstantBackground);
            this.Add(PF);

            this._fitConverged = false;
        }

        public PeakRegionFunction(double EstimatedConstantBackground, Pattern.Counts fittingData, List<PeakFunction> PFL)
        {
            this.functionType = 2;
            this.backgroundSwitch = true;
            this.backgroundFit = true;

            this.FittingCounts = fittingData;
            this.PolynomialBackgroundFunction = new BackgroundPolynomial(PFL[0].Angle, EstimatedConstantBackground);
            this.AddRange(PFL);

            this._fitConverged = false;
        }

        public PeakRegionFunction(DataManagment.Files.SCEC.PeakRegionInformation PRI)
        {
            this.backgroundFit = PRI.backgroundFit;
            this.backgroundSwitch = PRI.backgroundSwitch;
            this.FittingCounts = PRI.FittingCounts;
            this.PolynomialBackgroundFunction = PRI.PolynomialBackgroundFunction;

            this.startingLambda = PRI.startingLambda;
            this._fitConverged = PRI._fitConverged;
        }

        public void MergeRegions(PeakRegionFunction PRF)
        {
            Pattern.Counts NewFittingCounts = new Pattern.Counts();

            if(this.FittingCounts[0][0] < PRF.FittingCounts[0][0])
            {
                for(int n = 0; n < this.FittingCounts.Count; n++)
                {
                    NewFittingCounts.Add(this.FittingCounts[n]);
                }

                if(this.FittingCounts[this.FittingCounts.Count - 1][0] < PRF.FittingCounts[PRF.FittingCounts.Count - 1][0])
                {
                    for (int n = 0; n < PRF.FittingCounts.Count; n++)
                    {
                        if(this.FittingCounts[this.FittingCounts.Count - 1][0] < PRF.FittingCounts[n][0])
                        {
                            NewFittingCounts.Add(PRF.FittingCounts[n]);
                        }
                    }
                }
            }
            else
            {
                for (int n = 0; n < PRF.FittingCounts.Count; n++)
                {
                    NewFittingCounts.Add(PRF.FittingCounts[n]);
                }

                if (PRF.FittingCounts[PRF.FittingCounts.Count - 1][0] < this.FittingCounts[this.FittingCounts.Count - 1][0])
                {
                    for (int n = 0; n < this.FittingCounts.Count; n++)
                    {
                        if (PRF.FittingCounts[PRF.FittingCounts.Count - 1][0] < this.FittingCounts[n][0])
                        {
                            NewFittingCounts.Add(this.FittingCounts[n]);
                        }
                    }
                }
            }

            this.FittingCounts = NewFittingCounts;

            for(int n = 0; n < PRF.Count; n++)
            {
                this.Add(PRF[n]);
            }

            this.Sort((a, b) => a.Angle.CompareTo(b.Angle));
        }

        #region Calculation

        public double Y(double theta)
        {
            double Ret = 0.0;
            for(int n = 0; n < this.Count; n++)
            {
                Ret += this[n].YNoBackground(theta);
            }

            if (backgroundSwitch)
            {
                Ret += this.PolynomialBackgroundFunction.Y(theta);
            }

            return Ret;
        }

        #region Reduced Hessian

        //public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektor(double Lambda)
        //{
        //    int fitDimension = 0;
        //    for (int i = 0; i < this.Count; i++)
        //    {
        //        for(int j = 0; j < 4; j++)
        //        {
        //            if(this[i].FreeParameters[j])
        //            {
        //                fitDimension++;
        //            }
        //        }
        //    }

        //    if(this.FreeParameters[4])
        //    {
        //        fitDimension++;
        //    }
        //    if (this.FreeParameters[5])
        //    {
        //        fitDimension++;
        //    }
        //    if (this.FreeParameters[6])
        //    {
        //        fitDimension++;
        //    }

        //    //[0][0] Sigma
        //    //[1][1] Angle
        //    //[2][2] Intensity
        //    //[3][3] Lorentz Ratio
        //    //  *
        //    //  *
        //    //  *
        //    //[4][4] Background Constant
        //    //[5][5] BackgroundCenter
        //    //[6][6] Background aclivity
        //    MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(fitDimension, fitDimension, 0.0);

        //    //[0] Sigma
        //    //[1] Angle
        //    //[2] Intensity
        //    //[3] Lorentz Ratio
        //    //  *
        //    //  *
        //    //  *
        //    //[4] Background Constant
        //    //[5] BackgroundCenter
        //    //[6] Background aclivity
        //    MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(fitDimension, 0.0);

        //    for (int n = 0; n < this.FittingCounts.Count; n++)
        //    {
        //        int parameterIndex = 0;
        //        for (int i = 0; i < this.Count; i++)
        //        {
        //            for (int j = 0; j < 4; j++)
        //            {
        //                if (this[i].FreeParameters[j])
        //                {
        //                    switch(j)
        //                    {
        //                        case 0:
        //                            HessianMatrix[parameterIndex, parameterIndex] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this[j].FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
        //                            break;
        //                        case 1:
        //                            break;
        //                        case 2:
        //                            break;
        //                        case 3:
        //                            break;
        //                        default:
        //                            break;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] Sigma
        ///[1] Angle
        ///[2] Intensity
        ///[3] Lorentz Ratio
        ///     *
        ///     *
        ///     *
        ///[4] Background Constant
        ///[5] BackgroundCenter
        ///[6] Background aclivity
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorSigmaAngleIntensityLorentzRatioBackground(double Lambda)
        {
            //[0][0] Sigma
            //[1][1] Angle
            //[2][2] Intensity
            //[3][3] Lorentz Ratio
            //  *
            //  *
            //  *
            //[4][4] Background Constant
            //[5][5] BackgroundCenter
            //[6][6] Background aclivity
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense((this.Count * 4) + 3, (this.Count * 4) + 3, 0.0);

            //[0] Sigma
            //[1] Angle
            //[2] Intensity
            //[3] Lorentz Ratio
            //  *
            //  *
            //  *
            //[4] Background Constant
            //[5] BackgroundCenter
            //[6] Background aclivity
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>((this.Count * 4) + 3);
            
            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                for (int i = 0; i < this.Count; i++)
                {
                    #region Main Matrix

                    for (int j = 0; j < this.Count; j++)
                    {

                        HessianMatrix[(i * 4) + 0, (j * 4) + 0] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this[j].FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                        HessianMatrix[(i * 4) + 1, (j * 4) + 1] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this[j].FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 4) + 0, (j * 4) + 1] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this[j].FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 4) + 1, (j * 4) + 0] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this[j].FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                        HessianMatrix[(i * 4) + 2, (j * 4) + 2] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this[j].FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 4) + 0, (j * 4) + 2] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this[j].FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 4) + 2, (j * 4) + 0] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this[j].FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 4) + 1, (j * 4) + 2] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this[j].FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 4) + 2, (j * 4) + 1] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this[j].FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                        HessianMatrix[(i * 4) + 3, (j * 4) + 3] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this[j].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 4) + 0, (j * 4) + 3] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this[j].FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 4) + 3, (j * 4) + 0] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this[j].FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 4) + 1, (j * 4) + 3] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this[j].FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 4) + 3, (j * 4) + 1] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this[j].FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 4) + 3, (j * 4) + 2] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this[j].FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 4) + 2, (j * 4) + 3] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this[j].FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        
                    }

                    #endregion

                    #region Background

                    HessianMatrix[(i * 4) + 0, HessianMatrix.ColumnCount - 3] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 3, (i * 4) + 0] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[(i * 4) + 1, HessianMatrix.ColumnCount - 3] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 3, (i * 4) + 1] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[(i * 4) + 2, HessianMatrix.ColumnCount - 3] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 3, (i * 4) + 2] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[(i * 4) + 3, HessianMatrix.ColumnCount - 3] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 3, (i * 4) + 3] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    HessianMatrix[(i * 4) + 0, HessianMatrix.ColumnCount - 2] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 2, (i * 4) + 0] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[(i * 4) + 1, HessianMatrix.ColumnCount - 2] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 2, (i * 4) + 1] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[(i * 4) + 2, HessianMatrix.ColumnCount - 2] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 2, (i * 4) + 2] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[(i * 4) + 3, HessianMatrix.ColumnCount - 2] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 2, (i * 4) + 3] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    HessianMatrix[(i * 4) + 0, HessianMatrix.ColumnCount - 1] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 1, (i * 4) + 0] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[(i * 4) + 1, HessianMatrix.ColumnCount - 1] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 1, (i * 4) + 1] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[(i * 4) + 2, HessianMatrix.ColumnCount - 1] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 1, (i * 4) + 2] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[(i * 4) + 3, HessianMatrix.ColumnCount - 1] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 1, (i * 4) + 3] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    #endregion

                    #region Vector build

                    SolutionVector[(i * 4) + 0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeSigma(this.FittingCounts[n][0]);
                    SolutionVector[(i * 4) + 1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeAngle(this.FittingCounts[n][0]);
                    SolutionVector[(i * 4) + 2] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]);
                    SolutionVector[(i * 4) + 3] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]);

                    #endregion
                }

                HessianMatrix[HessianMatrix.RowCount - 3, HessianMatrix.ColumnCount - 3] += (this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[HessianMatrix.RowCount - 2, HessianMatrix.ColumnCount - 2] += (this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 3, HessianMatrix.ColumnCount - 2] += (this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 2, HessianMatrix.ColumnCount - 3] += (this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[HessianMatrix.RowCount - 1, HessianMatrix.ColumnCount - 1] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 3, HessianMatrix.ColumnCount - 1] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 1, HessianMatrix.ColumnCount - 3] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 1, HessianMatrix.ColumnCount - 2] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 2, HessianMatrix.ColumnCount - 1] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));


                SolutionVector[SolutionVector.Count - 3] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]);
                SolutionVector[SolutionVector.Count - 2] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]);
                SolutionVector[SolutionVector.Count - 1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]);



            }

            for (int n = 0; n < HessianMatrix.RowCount; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(SolutionVector.Count);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }


        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] Sigma
        ///[1] Angle
        ///[2] Intensity
        ///     *
        ///     *
        ///     *
        ///[4] Background Constant
        ///[5] BackgroundCenter
        ///[6] Background aclivity
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorSigmaAngleIntensityBackground(double Lambda)
        {
            //[0][0] Sigma
            //[1][1] Angle
            //[2][2] Intensity
            //  *
            //  *
            //  *
            //[4][4] Background Constant
            //[5][5] BackgroundCenter
            //[6][6] Background aclivity
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense((this.Count * 3) + 3, (this.Count * 3) + 3, 0.0);

            //[0] Sigma
            //[1] Angle
            //[2] Intensity
            //  *
            //  *
            //  *
            //[4] Background Constant
            //[5] BackgroundCenter
            //[6] Background aclivity
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>((this.Count * 3) + 3);

            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                for (int i = 0; i < this.Count; i++)
                {
                    #region Main Matrix

                    for (int j = 0; j < this.Count; j++)
                    {

                        HessianMatrix[(i * 3) + 0, (j * 3) + 0] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this[j].FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                        HessianMatrix[(i * 3) + 1, (j * 3) + 1] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this[j].FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 3) + 0, (j * 3) + 1] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this[j].FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 3) + 1, (j * 3) + 0] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this[j].FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                        HessianMatrix[(i * 3) + 2, (j * 3) + 2] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this[j].FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 3) + 0, (j * 3) + 2] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this[j].FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 3) + 2, (j * 3) + 0] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this[j].FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 3) + 1, (j * 3) + 2] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this[j].FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 3) + 2, (j * 3) + 1] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this[j].FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    }

                    #endregion

                    #region Background

                    HessianMatrix[(i * 3) + 0, HessianMatrix.ColumnCount - 3] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 3, (i * 3) + 0] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[(i * 3) + 1, HessianMatrix.ColumnCount - 3] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 3, (i * 3) + 1] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[(i * 3) + 2, HessianMatrix.ColumnCount - 3] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 3, (i * 3) + 2] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    HessianMatrix[(i * 3) + 0, HessianMatrix.ColumnCount - 2] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 2, (i * 3) + 0] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[(i * 3) + 1, HessianMatrix.ColumnCount - 2] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 2, (i * 3) + 1] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[(i * 3) + 2, HessianMatrix.ColumnCount - 2] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 2, (i * 3) + 2] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    HessianMatrix[(i * 3) + 0, HessianMatrix.ColumnCount - 1] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 1, (i * 3) + 0] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[(i * 3) + 1, HessianMatrix.ColumnCount - 1] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 1, (i * 3) + 1] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[(i * 3) + 2, HessianMatrix.ColumnCount - 1] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 1, (i * 3) + 2] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    #endregion

                    #region Vector build

                    SolutionVector[(i * 3) + 0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeSigma(this.FittingCounts[n][0]);
                    SolutionVector[(i * 3) + 1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeAngle(this.FittingCounts[n][0]);
                    SolutionVector[(i * 3) + 2] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]);

                }

                    #endregion

                HessianMatrix[HessianMatrix.RowCount - 3, HessianMatrix.ColumnCount - 3] += (this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[HessianMatrix.RowCount - 2, HessianMatrix.ColumnCount - 2] += (this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 3, HessianMatrix.ColumnCount - 2] += (this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 2, HessianMatrix.ColumnCount - 3] += (this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[HessianMatrix.RowCount - 1, HessianMatrix.ColumnCount - 1] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 3, HessianMatrix.ColumnCount - 1] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 1, HessianMatrix.ColumnCount - 3] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 1, HessianMatrix.ColumnCount - 2] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 2, HessianMatrix.ColumnCount - 1] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));


                SolutionVector[SolutionVector.Count - 3] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]);
                SolutionVector[SolutionVector.Count - 2] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]);
                SolutionVector[SolutionVector.Count - 1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]);
            }

            for (int n = 0; n < HessianMatrix.RowCount; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(SolutionVector.Count);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] Sigma
        ///[1] Angle
        ///[2] Lorentz Ratio
        ///     *
        ///     *
        ///     *
        ///[4] Background Constant
        ///[5] BackgroundCenter
        ///[6] Background aclivity
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorSigmaAngleLorentzRatioBackground(double Lambda)
        {
            //[0][0] Sigma
            //[1][1] Angle
            //[2][2] Lorentz Ratio
            //  *
            //  *
            //  *
            //[4][4] Background Constant
            //[5][5] BackgroundCenter
            //[6][6] Background aclivity
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense((this.Count * 3) + 3, (this.Count * 3) + 3, 0.0);

            //[0] Sigma
            //[1] Angle
            //[2] Intensity
            //  *
            //  *
            //  *
            //[4] Background Constant
            //[5] BackgroundCenter
            //[6] Background aclivity
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>((this.Count * 3) + 3);

            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                for (int i = 0; i < this.Count; i++)
                {
                    #region Main Matrix

                    for (int j = 0; j < this.Count; j++)
                    {

                        HessianMatrix[(i * 3) + 0, (j * 3) + 0] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this[j].FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                        HessianMatrix[(i * 3) + 1, (j * 3) + 1] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this[j].FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 3) + 0, (j * 3) + 1] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this[j].FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 3) + 1, (j * 3) + 0] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this[j].FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                        HessianMatrix[(i * 3) + 2, (j * 3) + 2] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this[j].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 3) + 0, (j * 3) + 2] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this[j].FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 3) + 2, (j * 3) + 0] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this[j].FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 3) + 1, (j * 3) + 2] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this[j].FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 3) + 2, (j * 3) + 1] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this[j].FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    }

                    #endregion

                    #region Background

                    HessianMatrix[(i * 3) + 0, HessianMatrix.ColumnCount - 3] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 3, (i * 3) + 0] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[(i * 3) + 1, HessianMatrix.ColumnCount - 3] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 3, (i * 3) + 1] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[(i * 3) + 2, HessianMatrix.ColumnCount - 3] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 3, (i * 3) + 2] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    HessianMatrix[(i * 3) + 0, HessianMatrix.ColumnCount - 2] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 2, (i * 3) + 0] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[(i * 3) + 1, HessianMatrix.ColumnCount - 2] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 2, (i * 3) + 1] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[(i * 3) + 2, HessianMatrix.ColumnCount - 2] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 2, (i * 3) + 2] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    HessianMatrix[(i * 3) + 0, HessianMatrix.ColumnCount - 1] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 1, (i * 3) + 0] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[(i * 3) + 1, HessianMatrix.ColumnCount - 1] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 1, (i * 3) + 1] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[(i * 3) + 2, HessianMatrix.ColumnCount - 1] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 1, (i * 3) + 2] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    #endregion

                    #region Vector build

                    SolutionVector[(i * 3) + 0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeSigma(this.FittingCounts[n][0]);
                    SolutionVector[(i * 3) + 1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeAngle(this.FittingCounts[n][0]);
                    SolutionVector[(i * 3) + 2] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]);

                }

                    #endregion

                HessianMatrix[HessianMatrix.RowCount - 3, HessianMatrix.ColumnCount - 3] += (this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[HessianMatrix.RowCount - 2, HessianMatrix.ColumnCount - 2] += (this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 3, HessianMatrix.ColumnCount - 2] += (this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 2, HessianMatrix.ColumnCount - 3] += (this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[HessianMatrix.RowCount - 1, HessianMatrix.ColumnCount - 1] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 3, HessianMatrix.ColumnCount - 1] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 1, HessianMatrix.ColumnCount - 3] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 1, HessianMatrix.ColumnCount - 2] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 2, HessianMatrix.ColumnCount - 1] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));


                SolutionVector[SolutionVector.Count - 3] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]);
                SolutionVector[SolutionVector.Count - 2] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]);
                SolutionVector[SolutionVector.Count - 1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]);
            }

            for (int n = 0; n < HessianMatrix.RowCount; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(SolutionVector.Count);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] Sigma
        ///[1] Lorentz Ratio
        ///[2] Intensity
        ///     *
        ///     *
        ///     *
        ///[4] Background Constant
        ///[5] BackgroundCenter
        ///[6] Background aclivity
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorSigmaIntensityLorentzRatioBackground(double Lambda)
        {
            //[0][0] Sigma
            //[1][1] Lorentz Ratio
            //[2][2] Intensity
            //  *
            //  *
            //  *
            //[4][4] Background Constant
            //[5][5] BackgroundCenter
            //[6][6] Background aclivity
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense((this.Count * 3) + 3, (this.Count * 3) + 3, 0.0);

            //[0] Sigma
            //[1] Lorentz Ratio
            //[2] Intensity
            //  *
            //  *
            //  *
            //[4] Background Constant
            //[5] BackgroundCenter
            //[6] Background aclivity
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>((this.Count * 3) + 3);

            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                for (int i = 0; i < this.Count; i++)
                {
                    #region Main Matrix

                    for (int j = 0; j < this.Count; j++)
                    {

                        HessianMatrix[(i * 3) + 0, (j * 3) + 0] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this[j].FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                        HessianMatrix[(i * 3) + 1, (j * 3) + 1] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this[j].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 3) + 0, (j * 3) + 1] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this[j].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 3) + 1, (j * 3) + 0] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this[j].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                        HessianMatrix[(i * 3) + 2, (j * 3) + 2] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this[j].FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 3) + 0, (j * 3) + 2] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this[j].FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 3) + 2, (j * 3) + 0] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this[j].FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 3) + 1, (j * 3) + 2] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this[j].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 3) + 2, (j * 3) + 1] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this[j].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    }

                    #endregion

                    #region Background

                    HessianMatrix[(i * 3) + 0, HessianMatrix.ColumnCount - 3] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 3, (i * 3) + 0] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[(i * 3) + 1, HessianMatrix.ColumnCount - 3] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 3, (i * 3) + 1] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[(i * 3) + 2, HessianMatrix.ColumnCount - 3] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 3, (i * 3) + 2] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    HessianMatrix[(i * 3) + 0, HessianMatrix.ColumnCount - 2] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 2, (i * 3) + 0] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[(i * 3) + 1, HessianMatrix.ColumnCount - 2] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 2, (i * 3) + 1] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[(i * 3) + 2, HessianMatrix.ColumnCount - 2] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 2, (i * 3) + 2] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    HessianMatrix[(i * 3) + 0, HessianMatrix.ColumnCount - 1] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 1, (i * 3) + 0] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[(i * 3) + 1, HessianMatrix.ColumnCount - 1] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 1, (i * 3) + 1] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[(i * 3) + 2, HessianMatrix.ColumnCount - 1] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 1, (i * 3) + 2] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    #endregion

                    #region Vector build

                    SolutionVector[(i * 3) + 0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeSigma(this.FittingCounts[n][0]);
                    SolutionVector[(i * 3) + 1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]);
                    SolutionVector[(i * 3) + 2] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]);

                }

                    #endregion

                HessianMatrix[HessianMatrix.RowCount - 3, HessianMatrix.ColumnCount - 3] += (this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[HessianMatrix.RowCount - 2, HessianMatrix.ColumnCount - 2] += (this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 3, HessianMatrix.ColumnCount - 2] += (this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 2, HessianMatrix.ColumnCount - 3] += (this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[HessianMatrix.RowCount - 1, HessianMatrix.ColumnCount - 1] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 3, HessianMatrix.ColumnCount - 1] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 1, HessianMatrix.ColumnCount - 3] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 1, HessianMatrix.ColumnCount - 2] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 2, HessianMatrix.ColumnCount - 1] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));


                SolutionVector[SolutionVector.Count - 3] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]);
                SolutionVector[SolutionVector.Count - 2] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]);
                SolutionVector[SolutionVector.Count - 1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]);
            }

            for (int n = 0; n < HessianMatrix.RowCount; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(SolutionVector.Count);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] Angle
        ///[1] Lorentz Ratio
        ///[2] Intensity
        ///     *
        ///     *
        ///     *
        ///[4] Background Constant
        ///[5] BackgroundCenter
        ///[6] Background aclivity
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorAngleIntensityLorentzRatioBackground(double Lambda)
        {
            //[0][0] Angle
            //[1][1] Lorentz Ratio
            //[2][2] Intensity
            //  *
            //  *
            //  *
            //[4][4] Background Constant
            //[5][5] BackgroundCenter
            //[6][6] Background aclivity
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense((this.Count * 3) + 3, (this.Count * 3) + 3, 0.0);

            //[0] Angle
            //[1] Lorentz Ratio
            //[2] Intensity
            //  *
            //  *
            //  *
            //[4] Background Constant
            //[5] BackgroundCenter
            //[6] Background aclivity
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>((this.Count * 3) + 3);

            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                for (int i = 0; i < this.Count; i++)
                {
                    #region Main Matrix

                    for (int j = 0; j < this.Count; j++)
                    {

                        HessianMatrix[(i * 3) + 0, (j * 3) + 0] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this[j].FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                        HessianMatrix[(i * 3) + 1, (j * 3) + 1] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this[j].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 3) + 0, (j * 3) + 1] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this[j].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 3) + 1, (j * 3) + 0] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this[j].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                        HessianMatrix[(i * 3) + 2, (j * 3) + 2] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this[j].FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 3) + 0, (j * 3) + 2] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this[j].FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 3) + 2, (j * 3) + 0] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this[j].FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 3) + 1, (j * 3) + 2] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this[j].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 3) + 2, (j * 3) + 1] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this[j].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    }

                    #endregion

                    #region Background

                    HessianMatrix[(i * 3) + 0, HessianMatrix.ColumnCount - 3] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 3, (i * 3) + 0] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[(i * 3) + 1, HessianMatrix.ColumnCount - 3] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 3, (i * 3) + 1] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[(i * 3) + 2, HessianMatrix.ColumnCount - 3] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 3, (i * 3) + 2] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    HessianMatrix[(i * 3) + 0, HessianMatrix.ColumnCount - 2] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 2, (i * 3) + 0] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[(i * 3) + 1, HessianMatrix.ColumnCount - 2] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 2, (i * 3) + 1] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[(i * 3) + 2, HessianMatrix.ColumnCount - 2] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 2, (i * 3) + 2] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    HessianMatrix[(i * 3) + 0, HessianMatrix.ColumnCount - 1] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 1, (i * 3) + 0] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[(i * 3) + 1, HessianMatrix.ColumnCount - 1] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 1, (i * 3) + 1] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[(i * 3) + 2, HessianMatrix.ColumnCount - 1] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 1, (i * 3) + 2] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    #endregion

                    #region Vector build

                    SolutionVector[(i * 3) + 0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeAngle(this.FittingCounts[n][0]);
                    SolutionVector[(i * 3) + 1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]);
                    SolutionVector[(i * 3) + 2] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]);

                }

                    #endregion

                HessianMatrix[HessianMatrix.RowCount - 3, HessianMatrix.ColumnCount - 3] += (this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[HessianMatrix.RowCount - 2, HessianMatrix.ColumnCount - 2] += (this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 3, HessianMatrix.ColumnCount - 2] += (this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 2, HessianMatrix.ColumnCount - 3] += (this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[HessianMatrix.RowCount - 1, HessianMatrix.ColumnCount - 1] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 3, HessianMatrix.ColumnCount - 1] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 1, HessianMatrix.ColumnCount - 3] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 1, HessianMatrix.ColumnCount - 2] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 2, HessianMatrix.ColumnCount - 1] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));


                SolutionVector[SolutionVector.Count - 3] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]);
                SolutionVector[SolutionVector.Count - 2] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]);
                SolutionVector[SolutionVector.Count - 1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]);
            }

            for (int n = 0; n < HessianMatrix.RowCount; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(SolutionVector.Count);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] Sigma
        ///[1] Angle
        ///     *
        ///     *
        ///     *
        ///[4] Background Constant
        ///[5] BackgroundCenter
        ///[6] Background aclivity
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorSigmaAngleBackground(double Lambda)
        {
            //[0][0] Sigma
            //[1][1] Angle
            //  *
            //  *
            //  *
            //[4][4] Background Constant
            //[5][5] BackgroundCenter
            //[6][6] Background aclivity
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense((this.Count * 2) + 3, (this.Count * 2) + 3, 0.0);

            //[0] Sigma
            //[1] Angle
            //  *
            //  *
            //  *
            //[4] Background Constant
            //[5] BackgroundCenter
            //[6] Background aclivity
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>((this.Count * 2) + 3);

            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                for (int i = 0; i < this.Count; i++)
                {
                    #region Main Matrix

                    for (int j = 0; j < this.Count; j++)
                    {

                        HessianMatrix[(i * 2) + 0, (j * 2) + 0] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this[j].FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                        HessianMatrix[(i * 2) + 1, (j * 2) + 1] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this[j].FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 2) + 0, (j * 2) + 1] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this[j].FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 2) + 1, (j * 2) + 0] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this[j].FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    }

                    #endregion

                    #region Background

                    HessianMatrix[(i * 2) + 0, HessianMatrix.ColumnCount - 3] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 3, (i * 2) + 0] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[(i * 2) + 1, HessianMatrix.ColumnCount - 3] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 3, (i * 2) + 1] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    HessianMatrix[(i * 2) + 0, HessianMatrix.ColumnCount - 2] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 2, (i * 2) + 0] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[(i * 2) + 1, HessianMatrix.ColumnCount - 2] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 2, (i * 2) + 1] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    HessianMatrix[(i * 2) + 0, HessianMatrix.ColumnCount - 1] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 1, (i * 2) + 0] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[(i * 2) + 1, HessianMatrix.ColumnCount - 1] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 1, (i * 2) + 1] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    #endregion

                    #region Vector build

                    SolutionVector[(i * 2) + 0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeSigma(this.FittingCounts[n][0]);
                    SolutionVector[(i * 2) + 1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeAngle(this.FittingCounts[n][0]);

                }

                    #endregion

                HessianMatrix[HessianMatrix.RowCount - 3, HessianMatrix.ColumnCount - 3] += (this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[HessianMatrix.RowCount - 2, HessianMatrix.ColumnCount - 2] += (this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 3, HessianMatrix.ColumnCount - 2] += (this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 2, HessianMatrix.ColumnCount - 3] += (this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[HessianMatrix.RowCount - 1, HessianMatrix.ColumnCount - 1] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 3, HessianMatrix.ColumnCount - 1] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 1, HessianMatrix.ColumnCount - 3] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 1, HessianMatrix.ColumnCount - 2] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 2, HessianMatrix.ColumnCount - 1] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));


                SolutionVector[SolutionVector.Count - 3] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]);
                SolutionVector[SolutionVector.Count - 2] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]);
                SolutionVector[SolutionVector.Count - 1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]);
            }

            for (int n = 0; n < HessianMatrix.RowCount; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(SolutionVector.Count);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] Sigma
        ///[1] Intensity
        ///     *
        ///     *
        ///     *
        ///[4] Background Constant
        ///[5] BackgroundCenter
        ///[6] Background aclivity
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorSigmaIntensityBackground(double Lambda)
        {
            //[0][0] Sigma
            //[1][1] Intensity
            //  *
            //  *
            //  *
            //[4][4] Background Constant
            //[5][5] BackgroundCenter
            //[6][6] Background aclivity
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense((this.Count * 2) + 3, (this.Count * 2) + 3, 0.0);

            //[0] Sigma
            //[1] Intensity
            //  *
            //  *
            //  *
            //[4] Background Constant
            //[5] BackgroundCenter
            //[6] Background aclivity
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>((this.Count * 2) + 3);

            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                for (int i = 0; i < this.Count; i++)
                {
                    #region Main Matrix

                    for (int j = 0; j < this.Count; j++)
                    {

                        HessianMatrix[(i * 2) + 0, (j * 2) + 0] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this[j].FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                        HessianMatrix[(i * 2) + 1, (j * 2) + 1] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this[j].FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 2) + 0, (j * 2) + 1] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this[j].FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 2) + 1, (j * 2) + 0] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this[j].FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    }

                    #endregion

                    #region Background

                    HessianMatrix[(i * 2) + 0, HessianMatrix.ColumnCount - 3] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 3, (i * 2) + 0] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[(i * 2) + 1, HessianMatrix.ColumnCount - 3] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 3, (i * 2) + 1] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    HessianMatrix[(i * 2) + 0, HessianMatrix.ColumnCount - 2] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 2, (i * 2) + 0] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[(i * 2) + 1, HessianMatrix.ColumnCount - 2] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 2, (i * 2) + 1] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    HessianMatrix[(i * 2) + 0, HessianMatrix.ColumnCount - 1] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 1, (i * 2) + 0] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[(i * 2) + 1, HessianMatrix.ColumnCount - 1] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 1, (i * 2) + 1] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    #endregion

                    #region Vector build

                    SolutionVector[(i * 2) + 0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeSigma(this.FittingCounts[n][0]);
                    SolutionVector[(i * 2) + 1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]);

                }

                    #endregion

                HessianMatrix[HessianMatrix.RowCount - 3, HessianMatrix.ColumnCount - 3] += (this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[HessianMatrix.RowCount - 2, HessianMatrix.ColumnCount - 2] += (this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 3, HessianMatrix.ColumnCount - 2] += (this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 2, HessianMatrix.ColumnCount - 3] += (this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[HessianMatrix.RowCount - 1, HessianMatrix.ColumnCount - 1] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 3, HessianMatrix.ColumnCount - 1] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 1, HessianMatrix.ColumnCount - 3] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 1, HessianMatrix.ColumnCount - 2] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 2, HessianMatrix.ColumnCount - 1] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));


                SolutionVector[SolutionVector.Count - 3] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]);
                SolutionVector[SolutionVector.Count - 2] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]);
                SolutionVector[SolutionVector.Count - 1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]);
            }

            for (int n = 0; n < HessianMatrix.RowCount; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(SolutionVector.Count);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] Angle
        ///[1] Intensity
        ///     *
        ///     *
        ///     *
        ///[4] Background Constant
        ///[5] BackgroundCenter
        ///[6] Background aclivity
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorAngleIntensityBackground(double Lambda)
        {
            //[0][0] Angle
            //[1][1] Intensity
            //  *
            //  *
            //  *
            //[4][4] Background Constant
            //[5][5] BackgroundCenter
            //[6][6] Background aclivity
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense((this.Count * 2) + 3, (this.Count * 2) + 3, 0.0);

            //[0] Angle
            //[1] Intensity
            //  *
            //  *
            //  *
            //[4] Background Constant
            //[5] BackgroundCenter
            //[6] Background aclivity
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>((this.Count * 2) + 3);

            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                for (int i = 0; i < this.Count; i++)
                {
                    #region Main Matrix

                    for (int j = 0; j < this.Count; j++)
                    {

                        HessianMatrix[(i * 2) + 0, (j * 2) + 0] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this[j].FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                        HessianMatrix[(i * 2) + 1, (j * 2) + 1] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this[j].FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 2) + 0, (j * 2) + 1] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this[j].FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 2) + 1, (j * 2) + 0] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this[j].FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    }

                    #endregion

                    #region Background

                    HessianMatrix[(i * 2) + 0, HessianMatrix.ColumnCount - 3] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 3, (i * 2) + 0] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[(i * 2) + 1, HessianMatrix.ColumnCount - 3] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 3, (i * 2) + 1] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    HessianMatrix[(i * 2) + 0, HessianMatrix.ColumnCount - 2] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 2, (i * 2) + 0] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[(i * 2) + 1, HessianMatrix.ColumnCount - 2] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 2, (i * 2) + 1] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    HessianMatrix[(i * 2) + 0, HessianMatrix.ColumnCount - 1] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 1, (i * 2) + 0] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[(i * 2) + 1, HessianMatrix.ColumnCount - 1] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 1, (i * 2) + 1] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    #endregion

                    #region Vector build

                    SolutionVector[(i * 2) + 0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeAngle(this.FittingCounts[n][0]);
                    SolutionVector[(i * 2) + 1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]);

                }

                    #endregion

                HessianMatrix[HessianMatrix.RowCount - 3, HessianMatrix.ColumnCount - 3] += (this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[HessianMatrix.RowCount - 2, HessianMatrix.ColumnCount - 2] += (this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 3, HessianMatrix.ColumnCount - 2] += (this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 2, HessianMatrix.ColumnCount - 3] += (this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[HessianMatrix.RowCount - 1, HessianMatrix.ColumnCount - 1] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 3, HessianMatrix.ColumnCount - 1] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 1, HessianMatrix.ColumnCount - 3] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 1, HessianMatrix.ColumnCount - 2] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 2, HessianMatrix.ColumnCount - 1] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));


                SolutionVector[SolutionVector.Count - 3] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]);
                SolutionVector[SolutionVector.Count - 2] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]);
                SolutionVector[SolutionVector.Count - 1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]);
            }

            for (int n = 0; n < HessianMatrix.RowCount; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(SolutionVector.Count);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] Sigma
        ///[1] Lorentz Ratio
        ///     *
        ///     *
        ///     *
        ///[4] Background Constant
        ///[5] BackgroundCenter
        ///[6] Background aclivity
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorSigmaLorentzRatioBackground(double Lambda)
        {
            //[0][0] Sigma
            //[1][1] Lorentz Ratio
            //  *
            //  *
            //  *
            //[4][4] Background Constant
            //[5][5] BackgroundCenter
            //[6][6] Background aclivity
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense((this.Count * 2) + 3, (this.Count * 2) + 3, 0.0);

            //[0] Sigma
            //[1] Lorentz Ratio
            //  *
            //  *
            //  *
            //[4] Background Constant
            //[5] BackgroundCenter
            //[6] Background aclivity
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>((this.Count * 2) + 3);

            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                for (int i = 0; i < this.Count; i++)
                {
                    #region Main Matrix

                    for (int j = 0; j < this.Count; j++)
                    {

                        HessianMatrix[(i * 2) + 0, (j * 2) + 0] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this[j].FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                        HessianMatrix[(i * 2) + 1, (j * 2) + 1] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this[j].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 2) + 0, (j * 2) + 1] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this[j].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 2) + 1, (j * 2) + 0] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this[j].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    }

                    #endregion

                    #region Background

                    HessianMatrix[(i * 2) + 0, HessianMatrix.ColumnCount - 3] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 3, (i * 2) + 0] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[(i * 2) + 1, HessianMatrix.ColumnCount - 3] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 3, (i * 2) + 1] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    HessianMatrix[(i * 2) + 0, HessianMatrix.ColumnCount - 2] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 2, (i * 2) + 0] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[(i * 2) + 1, HessianMatrix.ColumnCount - 2] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 2, (i * 2) + 1] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    HessianMatrix[(i * 2) + 0, HessianMatrix.ColumnCount - 1] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 1, (i * 2) + 0] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[(i * 2) + 1, HessianMatrix.ColumnCount - 1] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 1, (i * 2) + 1] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    #endregion

                    #region Vector build

                    SolutionVector[(i * 2) + 0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeSigma(this.FittingCounts[n][0]);
                    SolutionVector[(i * 2) + 1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]);

                }

                    #endregion

                HessianMatrix[HessianMatrix.RowCount - 3, HessianMatrix.ColumnCount - 3] += (this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[HessianMatrix.RowCount - 2, HessianMatrix.ColumnCount - 2] += (this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 3, HessianMatrix.ColumnCount - 2] += (this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 2, HessianMatrix.ColumnCount - 3] += (this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[HessianMatrix.RowCount - 1, HessianMatrix.ColumnCount - 1] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 3, HessianMatrix.ColumnCount - 1] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 1, HessianMatrix.ColumnCount - 3] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 1, HessianMatrix.ColumnCount - 2] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 2, HessianMatrix.ColumnCount - 1] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));


                SolutionVector[SolutionVector.Count - 3] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]);
                SolutionVector[SolutionVector.Count - 2] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]);
                SolutionVector[SolutionVector.Count - 1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]);
            }

            for (int n = 0; n < HessianMatrix.RowCount; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(SolutionVector.Count);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] Angle
        ///[1] Lorentz Ratio
        ///     *
        ///     *
        ///     *
        ///[4] Background Constant
        ///[5] BackgroundCenter
        ///[6] Background aclivity
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorAngleLorentzRatioBackground(double Lambda)
        {
            //[0][0] Angle
            //[1][1] Lorentz Ratio
            //  *
            //  *
            //  *
            //[4][4] Background Constant
            //[5][5] BackgroundCenter
            //[6][6] Background aclivity
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense((this.Count * 2) + 3, (this.Count * 2) + 3, 0.0);

            //[0] Angle
            //[1] Lorentz Ratio
            //  *
            //  *
            //  *
            //[4] Background Constant
            //[5] BackgroundCenter
            //[6] Background aclivity
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>((this.Count * 2) + 3);

            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                for (int i = 0; i < this.Count; i++)
                {
                    #region Main Matrix

                    for (int j = 0; j < this.Count; j++)
                    {

                        HessianMatrix[(i * 2) + 0, (j * 2) + 0] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this[j].FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                        HessianMatrix[(i * 2) + 1, (j * 2) + 1] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this[j].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 2) + 0, (j * 2) + 1] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this[j].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 2) + 1, (j * 2) + 0] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this[j].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    }

                    #endregion

                    #region Background

                    HessianMatrix[(i * 2) + 0, HessianMatrix.ColumnCount - 3] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 3, (i * 2) + 0] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[(i * 2) + 1, HessianMatrix.ColumnCount - 3] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 3, (i * 2) + 1] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    HessianMatrix[(i * 2) + 0, HessianMatrix.ColumnCount - 2] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 2, (i * 2) + 0] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[(i * 2) + 1, HessianMatrix.ColumnCount - 2] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 2, (i * 2) + 1] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    HessianMatrix[(i * 2) + 0, HessianMatrix.ColumnCount - 1] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 1, (i * 2) + 0] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[(i * 2) + 1, HessianMatrix.ColumnCount - 1] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 1, (i * 2) + 1] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    #endregion

                    #region Vector build

                    SolutionVector[(i * 2) + 0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeAngle(this.FittingCounts[n][0]);
                    SolutionVector[(i * 2) + 1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]);

                }

                    #endregion

                HessianMatrix[HessianMatrix.RowCount - 3, HessianMatrix.ColumnCount - 3] += (this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[HessianMatrix.RowCount - 2, HessianMatrix.ColumnCount - 2] += (this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 3, HessianMatrix.ColumnCount - 2] += (this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 2, HessianMatrix.ColumnCount - 3] += (this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[HessianMatrix.RowCount - 1, HessianMatrix.ColumnCount - 1] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 3, HessianMatrix.ColumnCount - 1] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 1, HessianMatrix.ColumnCount - 3] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 1, HessianMatrix.ColumnCount - 2] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 2, HessianMatrix.ColumnCount - 1] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));


                SolutionVector[SolutionVector.Count - 3] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]);
                SolutionVector[SolutionVector.Count - 2] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]);
                SolutionVector[SolutionVector.Count - 1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]);
            }

            for (int n = 0; n < HessianMatrix.RowCount; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(SolutionVector.Count);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] Intensity
        ///[1] Lorentz Ratio
        ///     *
        ///     *
        ///     *
        ///[4] Background Constant
        ///[5] BackgroundCenter
        ///[6] Background aclivity
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorIntensityLorentzRatioBackground(double Lambda)
        {
            //[0][0] Intensity
            //[1][1] Lorentz Ratio
            //  *
            //  *
            //  *
            //[4][4] Background Constant
            //[5][5] BackgroundCenter
            //[6][6] Background aclivity
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense((this.Count * 2) + 3, (this.Count * 2) + 3, 0.0);

            //[0] Intensity
            //[1] Lorentz Ratio
            //  *
            //  *
            //  *
            //[4] Background Constant
            //[5] BackgroundCenter
            //[6] Background aclivity
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>((this.Count * 2) + 3);

            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                for (int i = 0; i < this.Count; i++)
                {
                    #region Main Matrix

                    for (int j = 0; j < this.Count; j++)
                    {

                        HessianMatrix[(i * 2) + 0, (j * 2) + 0] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this[j].FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                        HessianMatrix[(i * 2) + 1, (j * 2) + 1] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this[j].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 2) + 0, (j * 2) + 1] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this[j].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 2) + 1, (j * 2) + 0] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this[j].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    }

                    #endregion

                    #region Background

                    HessianMatrix[(i * 2) + 0, HessianMatrix.ColumnCount - 3] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 3, (i * 2) + 0] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[(i * 2) + 1, HessianMatrix.ColumnCount - 3] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 3, (i * 2) + 1] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    HessianMatrix[(i * 2) + 0, HessianMatrix.ColumnCount - 2] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 2, (i * 2) + 0] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[(i * 2) + 1, HessianMatrix.ColumnCount - 2] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 2, (i * 2) + 1] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    HessianMatrix[(i * 2) + 0, HessianMatrix.ColumnCount - 1] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 1, (i * 2) + 0] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[(i * 2) + 1, HessianMatrix.ColumnCount - 1] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 1, (i * 2) + 1] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    #endregion

                    #region Vector build

                    SolutionVector[(i * 2) + 0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]);
                    SolutionVector[(i * 2) + 1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]);

                }

                    #endregion

                HessianMatrix[HessianMatrix.RowCount - 3, HessianMatrix.ColumnCount - 3] += (this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[HessianMatrix.RowCount - 2, HessianMatrix.ColumnCount - 2] += (this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 3, HessianMatrix.ColumnCount - 2] += (this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 2, HessianMatrix.ColumnCount - 3] += (this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[HessianMatrix.RowCount - 1, HessianMatrix.ColumnCount - 1] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 3, HessianMatrix.ColumnCount - 1] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 1, HessianMatrix.ColumnCount - 3] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 1, HessianMatrix.ColumnCount - 2] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 2, HessianMatrix.ColumnCount - 1] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));


                SolutionVector[SolutionVector.Count - 3] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]);
                SolutionVector[SolutionVector.Count - 2] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]);
                SolutionVector[SolutionVector.Count - 1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]);
            }

            for (int n = 0; n < HessianMatrix.RowCount; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(SolutionVector.Count);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] Intensity
        ///     *
        ///     *
        ///     *
        ///[4] Background Constant
        ///[5] BackgroundCenter
        ///[6] Background aclivity
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorIntensityBackground(double Lambda)
        {
            //[0][0] Intensity
            //  *
            //  *
            //  *
            //[4][4] Background Constant
            //[5][5] BackgroundCenter
            //[6][6] Background aclivity
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense((this.Count) + 3, (this.Count) + 3, 0.0);

            //[0] Intensity
            //  *
            //  *
            //  *
            //[4] Background Constant
            //[5] BackgroundCenter
            //[6] Background aclivity
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>((this.Count) + 3);

            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                for (int i = 0; i < this.Count; i++)
                {
                    #region Main Matrix

                    for (int j = 0; j < this.Count; j++)
                    {
                        HessianMatrix[(i * 1) + 0, (j * 1) + 0] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this[j].FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    }

                    #endregion

                    #region Background

                    HessianMatrix[(i * 1) + 0, HessianMatrix.ColumnCount - 3] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 3, (i * 1) + 0] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    HessianMatrix[(i * 1) + 0, HessianMatrix.ColumnCount - 2] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 2, (i * 1) + 0] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    HessianMatrix[(i * 1) + 0, HessianMatrix.ColumnCount - 1] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 1, (i * 1) + 0] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    #endregion

                    #region Vector build

                    SolutionVector[(i) + 0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]);

                    #endregion
                }

                HessianMatrix[HessianMatrix.RowCount - 3, HessianMatrix.ColumnCount - 3] += (this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[HessianMatrix.RowCount - 2, HessianMatrix.ColumnCount - 2] += (this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 3, HessianMatrix.ColumnCount - 2] += (this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 2, HessianMatrix.ColumnCount - 3] += (this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[HessianMatrix.RowCount - 1, HessianMatrix.ColumnCount - 1] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 3, HessianMatrix.ColumnCount - 1] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 1, HessianMatrix.ColumnCount - 3] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 1, HessianMatrix.ColumnCount - 2] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 2, HessianMatrix.ColumnCount - 1] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));


                SolutionVector[SolutionVector.Count - 3] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]);
                SolutionVector[SolutionVector.Count - 2] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]);
                SolutionVector[SolutionVector.Count - 1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]);
            }

            for (int n = 0; n < HessianMatrix.RowCount; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(SolutionVector.Count);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] Sigma
        ///     *
        ///     *
        ///     *
        ///[4] Background Constant
        ///[5] BackgroundCenter
        ///[6] Background aclivity
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorSigmaBackground(double Lambda)
        {
            //[0][0] Sigma
            //  *
            //  *
            //  *
            //[4][4] Background Constant
            //[5][5] BackgroundCenter
            //[6][6] Background aclivity
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense((this.Count) + 3, (this.Count) + 3, 0.0);

            //[0] Sigma
            //  *
            //  *
            //  *
            //[4] Background Constant
            //[5] BackgroundCenter
            //[6] Background aclivity
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>((this.Count) + 3);

            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                for (int i = 0; i < this.Count; i++)
                {
                    #region Main Matrix

                    for (int j = 0; j < this.Count; j++)
                    {
                        HessianMatrix[(i * 1) + 0, (j * 1) + 0] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this[j].FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    }

                    #endregion

                    #region Background

                    HessianMatrix[(i * 1) + 0, HessianMatrix.ColumnCount - 3] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 3, (i * 1) + 0] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    HessianMatrix[(i * 1) + 0, HessianMatrix.ColumnCount - 2] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 2, (i * 1) + 0] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    HessianMatrix[(i * 1) + 0, HessianMatrix.ColumnCount - 1] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 1, (i * 1) + 0] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    #endregion

                    #region Vector build

                    SolutionVector[(i) + 0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeSigma(this.FittingCounts[n][0]);

                    #endregion
                }

                HessianMatrix[HessianMatrix.RowCount - 3, HessianMatrix.ColumnCount - 3] += (this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[HessianMatrix.RowCount - 2, HessianMatrix.ColumnCount - 2] += (this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 3, HessianMatrix.ColumnCount - 2] += (this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 2, HessianMatrix.ColumnCount - 3] += (this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[HessianMatrix.RowCount - 1, HessianMatrix.ColumnCount - 1] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 3, HessianMatrix.ColumnCount - 1] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 1, HessianMatrix.ColumnCount - 3] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 1, HessianMatrix.ColumnCount - 2] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 2, HessianMatrix.ColumnCount - 1] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));


                SolutionVector[SolutionVector.Count - 3] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]);
                SolutionVector[SolutionVector.Count - 2] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]);
                SolutionVector[SolutionVector.Count - 1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]);
            }

            for (int n = 0; n < HessianMatrix.RowCount; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(SolutionVector.Count);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] Angle
        ///     *
        ///     *
        ///     *
        ///[4] Background Constant
        ///[5] BackgroundCenter
        ///[6] Background aclivity
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorAngleBackground(double Lambda)
        {
            //[0][0] Angle
            //  *
            //  *
            //  *
            //[4][4] Background Constant
            //[5][5] BackgroundCenter
            //[6][6] Background aclivity
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense((this.Count) + 3, (this.Count) + 3, 0.0);

            //[0] Angle
            //  *
            //  *
            //  *
            //[4] Background Constant
            //[5] BackgroundCenter
            //[6] Background aclivity
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>((this.Count) + 3);

            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                for (int i = 0; i < this.Count; i++)
                {
                    #region Main Matrix

                    for (int j = 0; j < this.Count; j++)
                    {
                        HessianMatrix[(i * 1) + 0, (j * 1) + 0] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this[j].FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    }

                    #endregion

                    #region Background

                    HessianMatrix[(i * 1) + 0, HessianMatrix.ColumnCount - 3] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 3, (i * 1) + 0] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    HessianMatrix[(i * 1) + 0, HessianMatrix.ColumnCount - 2] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 2, (i * 1) + 0] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    HessianMatrix[(i * 1) + 0, HessianMatrix.ColumnCount - 1] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 1, (i * 1) + 0] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    #endregion

                    #region Vector build

                    SolutionVector[(i) + 0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeAngle(this.FittingCounts[n][0]);

                    #endregion
                }

                HessianMatrix[HessianMatrix.RowCount - 3, HessianMatrix.ColumnCount - 3] += (this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[HessianMatrix.RowCount - 2, HessianMatrix.ColumnCount - 2] += (this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 3, HessianMatrix.ColumnCount - 2] += (this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 2, HessianMatrix.ColumnCount - 3] += (this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[HessianMatrix.RowCount - 1, HessianMatrix.ColumnCount - 1] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 3, HessianMatrix.ColumnCount - 1] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 1, HessianMatrix.ColumnCount - 3] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 1, HessianMatrix.ColumnCount - 2] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 2, HessianMatrix.ColumnCount - 1] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));


                SolutionVector[SolutionVector.Count - 3] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]);
                SolutionVector[SolutionVector.Count - 2] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]);
                SolutionVector[SolutionVector.Count - 1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]);
            }

            for (int n = 0; n < HessianMatrix.RowCount; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(SolutionVector.Count);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] Lorentz Ratio
        ///     *
        ///     *
        ///     *
        ///[4] Background Constant
        ///[5] BackgroundCenter
        ///[6] Background aclivity
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorLorentzRatioBackground(double Lambda)
        {
            //[0][0] Lorentz Ratio
            //  *
            //  *
            //  *
            //[4][4] Background Constant
            //[5][5] BackgroundCenter
            //[6][6] Background aclivity
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense((this.Count) + 3, (this.Count) + 3, 0.0);

            //[0] Lorentz Ratio
            //  *
            //  *
            //  *
            //[4] Background Constant
            //[5] BackgroundCenter
            //[6] Background aclivity
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>((this.Count) + 3);

            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                for (int i = 0; i < this.Count; i++)
                {
                    #region Main Matrix

                    for (int j = 0; j < this.Count; j++)
                    {
                        HessianMatrix[(i * 1) + 0, (j * 1) + 0] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this[j].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    }

                    #endregion

                    #region Background

                    HessianMatrix[(i * 1) + 0, HessianMatrix.ColumnCount - 3] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 3, (i * 1) + 0] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    HessianMatrix[(i * 1) + 0, HessianMatrix.ColumnCount - 2] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 2, (i * 1) + 0] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    HessianMatrix[(i * 1) + 0, HessianMatrix.ColumnCount - 1] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    HessianMatrix[HessianMatrix.ColumnCount - 1, (i * 1) + 0] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    #endregion

                    #region Vector build

                    SolutionVector[(i) + 0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]);

                    #endregion
                }

                HessianMatrix[HessianMatrix.RowCount - 3, HessianMatrix.ColumnCount - 3] += (this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[HessianMatrix.RowCount - 2, HessianMatrix.ColumnCount - 2] += (this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 3, HessianMatrix.ColumnCount - 2] += (this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 2, HessianMatrix.ColumnCount - 3] += (this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[HessianMatrix.RowCount - 1, HessianMatrix.ColumnCount - 1] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 3, HessianMatrix.ColumnCount - 1] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 1, HessianMatrix.ColumnCount - 3] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 1, HessianMatrix.ColumnCount - 2] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[HessianMatrix.RowCount - 2, HessianMatrix.ColumnCount - 1] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));


                SolutionVector[SolutionVector.Count - 3] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]);
                SolutionVector[SolutionVector.Count - 2] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]);
                SolutionVector[SolutionVector.Count - 1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]);
            }

            for (int n = 0; n < HessianMatrix.RowCount; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(SolutionVector.Count);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[4] Background Constant
        ///[5] BackgroundCenter
        ///[6] Background aclivity
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorBackground(double Lambda)
        {
            //[4][4] Background Constant
            //[5][5] BackgroundCenter
            //[6][6] Background aclivity
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense(3, 3, 0.0);

            //[4] Background Constant
            //[5] BackgroundCenter
            //[6] Background aclivity
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(3);

            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                HessianMatrix[0, 0] += (this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[1, 1] += (this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 1] += (this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 0] += (this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                HessianMatrix[2, 2] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[0, 2] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 0] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[2, 1] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                HessianMatrix[1, 2] += (this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));


                SolutionVector[0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.PolynomialBackgroundFunction.FirstDerivativeConstant(this.FittingCounts[n][0]);
                SolutionVector[1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.PolynomialBackgroundFunction.FirstDerivativeCenter(this.FittingCounts[n][0]);
                SolutionVector[2] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this.PolynomialBackgroundFunction.FirstDerivativeAclivity(this.FittingCounts[n][0]);
            }

            for (int n = 0; n < HessianMatrix.RowCount; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(SolutionVector.Count);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] Sigma
        ///[1] Angle
        ///[2] Intensity
        ///[3] Lorentz Ratio
        ///     *
        ///     *
        ///     *
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorSigmaAngleIntensityLorentzRatio(double Lambda)
        {
            //[0][0] Sigma
            //[1][1] Angle
            //[2][2] Intensity
            //[3][3] Lorentz Ratio
            //  *
            //  *
            //  *
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense((this.Count * 4), (this.Count * 4), 0.0);

            //[0] Sigma
            //[1] Angle
            //[2] Intensity
            //[3] Lorentz Ratio
            //  *
            //  *
            //  *
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>((this.Count * 4));

            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                for (int i = 0; i < this.Count; i++)
                {
                    #region Main Matrix

                    for (int j = 0; j < this.Count; j++)
                    {

                        HessianMatrix[(i * 4) + 0, (j * 4) + 0] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this[j].FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                        HessianMatrix[(i * 4) + 1, (j * 4) + 1] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this[j].FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 4) + 0, (j * 4) + 1] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this[j].FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 4) + 1, (j * 4) + 0] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this[j].FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                        HessianMatrix[(i * 4) + 2, (j * 4) + 2] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this[j].FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 4) + 0, (j * 4) + 2] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this[j].FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 4) + 2, (j * 4) + 0] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this[j].FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 4) + 1, (j * 4) + 2] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this[j].FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 4) + 2, (j * 4) + 1] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this[j].FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                        HessianMatrix[(i * 4) + 3, (j * 4) + 3] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this[j].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 4) + 0, (j * 4) + 3] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this[j].FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 4) + 3, (j * 4) + 0] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this[j].FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 4) + 1, (j * 4) + 3] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this[j].FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 4) + 3, (j * 4) + 1] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this[j].FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 4) + 3, (j * 4) + 2] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this[j].FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 4) + 2, (j * 4) + 3] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this[j].FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    }

                    #endregion

                    #region Vector build

                    SolutionVector[(i * 4) + 0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeSigma(this.FittingCounts[n][0]);
                    SolutionVector[(i * 4) + 1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeAngle(this.FittingCounts[n][0]);
                    SolutionVector[(i * 4) + 2] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]);
                    SolutionVector[(i * 4) + 3] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]);

                    #endregion
                }
            }

            for (int n = 0; n < HessianMatrix.RowCount; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(SolutionVector.Count);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] Sigma
        ///[1] Angle
        ///[2] Intensity
        ///     *
        ///     *
        ///     *
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorSigmaAngleIntensity(double Lambda)
        {
            //[0][0] Sigma
            //[1][1] Angle
            //[2][2] Intensity
            //  *
            //  *
            //  *
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense((this.Count * 3), (this.Count * 3), 0.0);

            //[0] Sigma
            //[1] Angle
            //[2] Intensity
            //  *
            //  *
            //  *
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>((this.Count * 3));

            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                for (int i = 0; i < this.Count; i++)
                {
                    #region Main Matrix

                    for (int j = 0; j < this.Count; j++)
                    {

                        HessianMatrix[(i * 3) + 0, (j * 3) + 0] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this[j].FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                        HessianMatrix[(i * 3) + 1, (j * 3) + 1] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this[j].FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 3) + 0, (j * 3) + 1] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this[j].FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 3) + 1, (j * 3) + 0] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this[j].FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                        HessianMatrix[(i * 3) + 2, (j * 3) + 2] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this[j].FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 3) + 0, (j * 3) + 2] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this[j].FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 3) + 2, (j * 3) + 0] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this[j].FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 3) + 1, (j * 3) + 2] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this[j].FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 3) + 2, (j * 3) + 1] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this[j].FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    }

                    #endregion

                    #region Vector build

                    SolutionVector[(i * 3) + 0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeSigma(this.FittingCounts[n][0]);
                    SolutionVector[(i * 3) + 1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeAngle(this.FittingCounts[n][0]);
                    SolutionVector[(i * 3) + 2] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]);

                    #endregion
                }
            }

            for (int n = 0; n < HessianMatrix.RowCount; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(SolutionVector.Count);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] Sigma
        ///[1] Angle
        ///[2] Lorentz Ratio
        ///     *
        ///     *
        ///     *
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorSigmaAngleLorentzRatio(double Lambda)
        {
            //[0][0] Sigma
            //[1][1] Angle
            //[2][2] Lorentz Ratio
            //  *
            //  *
            //  *
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense((this.Count * 3), (this.Count * 3), 0.0);

            //[0] Sigma
            //[1] Angle
            //[2] Lorentz Ratio
            //  *
            //  *
            //  *
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>((this.Count * 3));

            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                for (int i = 0; i < this.Count; i++)
                {
                    #region Main Matrix

                    for (int j = 0; j < this.Count; j++)
                    {

                        HessianMatrix[(i * 3) + 0, (j * 3) + 0] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this[j].FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                        HessianMatrix[(i * 3) + 1, (j * 3) + 1] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this[j].FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 3) + 0, (j * 3) + 1] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this[j].FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 3) + 1, (j * 3) + 0] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this[j].FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                        HessianMatrix[(i * 3) + 2, (j * 3) + 2] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this[j].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 3) + 0, (j * 3) + 2] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this[j].FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 3) + 2, (j * 3) + 0] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this[j].FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 3) + 1, (j * 3) + 2] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this[j].FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 3) + 2, (j * 3) + 1] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this[j].FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    }

                    #endregion

                    #region Vector build

                    SolutionVector[(i * 3) + 0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeSigma(this.FittingCounts[n][0]);
                    SolutionVector[(i * 3) + 1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeAngle(this.FittingCounts[n][0]);
                    SolutionVector[(i * 3) + 2] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]);

                    #endregion
                }
            }

            for (int n = 0; n < HessianMatrix.RowCount; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(SolutionVector.Count);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] Sigma
        ///[1] Intensity
        ///[2] Lorentz Ratio
        ///     *
        ///     *
        ///     *
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorSigmaIntensityLorentzRatio(double Lambda)
        {
            //[0][0] Sigma
            //[1][1] Intensity
            //[2][2] Lorentz Ratio
            //  *
            //  *
            //  *
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense((this.Count * 3), (this.Count * 3), 0.0);

            //[0] Sigma
            //[1] Lorentz Ratio
            //[2] Intensity
            //  *
            //  *
            //  *
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>((this.Count * 3));

            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                for (int i = 0; i < this.Count; i++)
                {
                    #region Main Matrix

                    for (int j = 0; j < this.Count; j++)
                    {

                        HessianMatrix[(i * 3) + 0, (j * 3) + 0] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this[j].FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                        HessianMatrix[(i * 3) + 1, (j * 3) + 1] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this[j].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 3) + 0, (j * 3) + 1] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this[j].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 3) + 1, (j * 3) + 0] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this[j].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                        HessianMatrix[(i * 3) + 2, (j * 3) + 2] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this[j].FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 3) + 0, (j * 3) + 2] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this[j].FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 3) + 2, (j * 3) + 0] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this[j].FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 3) + 1, (j * 3) + 2] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this[j].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 3) + 2, (j * 3) + 1] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this[j].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    }

                    #endregion

                    #region Vector build

                    SolutionVector[(i * 3) + 0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeSigma(this.FittingCounts[n][0]);
                    SolutionVector[(i * 3) + 1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]);
                    SolutionVector[(i * 3) + 2] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]);

                    #endregion
                }
            }

            for (int n = 0; n < HessianMatrix.RowCount; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(SolutionVector.Count);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] Angle
        ///[1] Intensity
        ///[2] Lorentz Ratio
        ///     *
        ///     *
        ///     *
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorAngleIntensityLorentzRatio(double Lambda)
        {
            //[0][0] Angle
            //[1][1] Intensity
            //[2][2] Lorentz Ratio
            //  *
            //  *
            //  *
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense((this.Count * 3), (this.Count * 3), 0.0);

            //[0] Angle
            //[1] Lorentz Ratio
            //[2] Intensity
            //  *
            //  *
            //  *
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>((this.Count * 3));

            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                for (int i = 0; i < this.Count; i++)
                {
                    #region Main Matrix

                    for (int j = 0; j < this.Count; j++)
                    {

                        HessianMatrix[(i * 3) + 0, (j * 3) + 0] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this[j].FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                        HessianMatrix[(i * 3) + 1, (j * 3) + 1] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this[j].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 3) + 0, (j * 3) + 1] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this[j].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 3) + 1, (j * 3) + 0] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this[j].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                        HessianMatrix[(i * 3) + 2, (j * 3) + 2] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this[j].FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 3) + 0, (j * 3) + 2] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this[j].FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 3) + 2, (j * 3) + 0] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this[j].FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 3) + 1, (j * 3) + 2] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this[j].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 3) + 2, (j * 3) + 1] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this[j].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    }

                    #endregion

                    #region Vector build

                    SolutionVector[(i * 3) + 0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeAngle(this.FittingCounts[n][0]);
                    SolutionVector[(i * 3) + 1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]);
                    SolutionVector[(i * 3) + 2] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]);

                    #endregion
                }
            }

            for (int n = 0; n < HessianMatrix.RowCount; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(SolutionVector.Count);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] Sigma
        ///[1] Angle
        ///     *
        ///     *
        ///     *
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorSigmaAngle(double Lambda)
        {
            //[0][0] Sigma
            //[1][1] Angle
            //  *
            //  *
            //  *
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense((this.Count * 2), (this.Count * 2), 0.0);

            //[0] Sigma
            //[1] Angle
            //  *
            //  *
            //  *
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>((this.Count * 2));

            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                for (int i = 0; i < this.Count; i++)
                {
                    #region Main Matrix

                    for (int j = 0; j < this.Count; j++)
                    {

                        HessianMatrix[(i * 2) + 0, (j * 2) + 0] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this[j].FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                        HessianMatrix[(i * 2) + 1, (j * 2) + 1] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this[j].FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 2) + 0, (j * 2) + 1] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this[j].FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 2) + 1, (j * 2) + 0] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this[j].FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    }

                    #endregion

                    #region Vector build

                    SolutionVector[(i * 2) + 0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeSigma(this.FittingCounts[n][0]);
                    SolutionVector[(i * 2) + 1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeAngle(this.FittingCounts[n][0]);

                    #endregion
                }
            }

            for (int n = 0; n < HessianMatrix.RowCount; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(SolutionVector.Count);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] Sigma
        ///[1] Lorentz Ratio
        ///     *
        ///     *
        ///     *
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorSigmaLorentzRatio(double Lambda)
        {
            //[0][0] Sigma
            //[1][1] Lorentz Ratio
            //  *
            //  *
            //  *
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense((this.Count * 2), (this.Count * 2), 0.0);

            //[0] Sigma
            //[1] Lorentz Ratio
            //  *
            //  *
            //  *
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>((this.Count * 2));

            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                for (int i = 0; i < this.Count; i++)
                {
                    #region Main Matrix

                    for (int j = 0; j < this.Count; j++)
                    {

                        HessianMatrix[(i * 2) + 0, (j * 2) + 0] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this[j].FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                        HessianMatrix[(i * 2) + 1, (j * 2) + 1] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this[j].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 2) + 0, (j * 2) + 1] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this[j].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 2) + 1, (j * 2) + 0] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this[j].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    }

                    #endregion

                    #region Vector build

                    SolutionVector[(i * 2) + 0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeSigma(this.FittingCounts[n][0]);
                    SolutionVector[(i * 2) + 1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]);

                    #endregion
                }
            }

            for (int n = 0; n < HessianMatrix.RowCount; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(SolutionVector.Count);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] Sigma
        ///[1] Intensity
        ///     *
        ///     *
        ///     *
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorSigmaIntensity(double Lambda)
        {
            //[0][0] Sigma
            //[1][1] Intensity
            //  *
            //  *
            //  *
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense((this.Count * 2), (this.Count * 2), 0.0);

            //[0] Sigma
            //[1] Intensity
            //  *
            //  *
            //  *
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>((this.Count * 2));

            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                for (int i = 0; i < this.Count; i++)
                {
                    #region Main Matrix

                    for (int j = 0; j < this.Count; j++)
                    {

                        HessianMatrix[(i * 2) + 0, (j * 2) + 0] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this[j].FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                        HessianMatrix[(i * 2) + 1, (j * 2) + 1] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this[j].FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 2) + 0, (j * 2) + 1] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this[j].FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 2) + 1, (j * 2) + 0] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this[j].FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    }

                    #endregion

                    #region Vector build

                    SolutionVector[(i * 2) + 0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeSigma(this.FittingCounts[n][0]);
                    SolutionVector[(i * 2) + 1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]);

                    #endregion
                }
            }

            for (int n = 0; n < HessianMatrix.RowCount; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(SolutionVector.Count);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] Angle
        ///[1] Lorentz Ratio
        ///     *
        ///     *
        ///     *
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorAngleLorentzRatio(double Lambda)
        {
            //[0][0] Angle
            //[1][1] Lorentz Ratio
            //  *
            //  *
            //  *
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense((this.Count * 2), (this.Count * 2), 0.0);

            //[0] Angle
            //[1] Lorentz Ratio
            //  *
            //  *
            //  *
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>((this.Count * 2));

            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                for (int i = 0; i < this.Count; i++)
                {
                    #region Main Matrix

                    for (int j = 0; j < this.Count; j++)
                    {

                        HessianMatrix[(i * 2) + 0, (j * 2) + 0] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this[j].FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                        HessianMatrix[(i * 2) + 1, (j * 2) + 1] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this[j].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 2) + 0, (j * 2) + 1] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this[j].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 2) + 1, (j * 2) + 0] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this[j].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    }

                    #endregion

                    #region Vector build

                    SolutionVector[(i * 2) + 0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeAngle(this.FittingCounts[n][0]);
                    SolutionVector[(i * 2) + 1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]);

                    #endregion
                }
            }

            for (int n = 0; n < HessianMatrix.RowCount; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(SolutionVector.Count);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] Angle
        ///[1] Intensity
        ///     *
        ///     *
        ///     *
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorAngleIntensity(double Lambda)
        {
            //[0][0] Angle
            //[1][1] Intensity
            //  *
            //  *
            //  *
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense((this.Count * 2), (this.Count * 2), 0.0);

            //[0] Angle
            //[1] Intensity
            //  *
            //  *
            //  *
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>((this.Count * 2));

            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                for (int i = 0; i < this.Count; i++)
                {
                    #region Main Matrix

                    for (int j = 0; j < this.Count; j++)
                    {

                        HessianMatrix[(i * 2) + 0, (j * 2) + 0] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this[j].FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                        HessianMatrix[(i * 2) + 1, (j * 2) + 1] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this[j].FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 2) + 0, (j * 2) + 1] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this[j].FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 2) + 1, (j * 2) + 0] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this[j].FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    }

                    #endregion

                    #region Vector build

                    SolutionVector[(i * 2) + 0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeAngle(this.FittingCounts[n][0]);
                    SolutionVector[(i * 2) + 1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]);

                    #endregion
                }
            }

            for (int n = 0; n < HessianMatrix.RowCount; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(SolutionVector.Count);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] Intensity
        ///[1] Lorentz Ratio
        ///     *
        ///     *
        ///     *
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorIntensityLorentzRatio(double Lambda)
        {
            //[0][0] Intensity
            //[1][1] Lorentz Ratio
            //  *
            //  *
            //  *
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense((this.Count * 2), (this.Count * 2), 0.0);

            //[0] Intensity
            //[1] Lorentz Ratio
            //  *
            //  *
            //  *
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>((this.Count * 2));

            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                for (int i = 0; i < this.Count; i++)
                {
                    #region Main Matrix

                    for (int j = 0; j < this.Count; j++)
                    {

                        HessianMatrix[(i * 2) + 0, (j * 2) + 0] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this[j].FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                        HessianMatrix[(i * 2) + 1, (j * 2) + 1] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this[j].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 2) + 0, (j * 2) + 1] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this[j].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                        HessianMatrix[(i * 2) + 1, (j * 2) + 0] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this[j].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));

                    }

                    #endregion

                    #region Vector build

                    SolutionVector[(i * 2) + 0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]);
                    SolutionVector[(i * 2) + 1] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]);

                    #endregion
                }
            }

            for (int n = 0; n < HessianMatrix.RowCount; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(SolutionVector.Count);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] Sigma
        ///     *
        ///     *
        ///     *
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorSigma(double Lambda)
        {
            //[0][0] Sigma
            //  *
            //  *
            //  *
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense((this.Count * 1), (this.Count * 1), 0.0);

            //[0] Sigma
            //  *
            //  *
            //  *
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>((this.Count * 1));

            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                for (int i = 0; i < this.Count; i++)
                {
                    #region Main Matrix

                    for (int j = 0; j < this.Count; j++)
                    {
                        HessianMatrix[(i * 1) + 0, (j * 1) + 0] += (this[i].FirstDerivativeSigma(this.FittingCounts[n][0]) * this[j].FirstDerivativeSigma(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    }

                    #endregion

                    #region Vector build

                    SolutionVector[(i * 1) + 0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeSigma(this.FittingCounts[n][0]);

                    #endregion
                }
            }

            for (int n = 0; n < HessianMatrix.RowCount; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(SolutionVector.Count);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] Angle
        ///     *
        ///     *
        ///     *
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorAngle(double Lambda)
        {
            //[0][0] Angle
            //  *
            //  *
            //  *
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense((this.Count * 1), (this.Count * 1), 0.0);

            //[0] Angle
            //  *
            //  *
            //  *
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>((this.Count * 1));

            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                for (int i = 0; i < this.Count; i++)
                {
                    #region Main Matrix

                    for (int j = 0; j < this.Count; j++)
                    {
                        HessianMatrix[(i * 1) + 0, (j * 1) + 0] += (this[i].FirstDerivativeAngle(this.FittingCounts[n][0]) * this[j].FirstDerivativeAngle(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    }

                    #endregion

                    #region Vector build

                    SolutionVector[(i * 1) + 0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeAngle(this.FittingCounts[n][0]);

                    #endregion
                }
            }

            for (int n = 0; n < HessianMatrix.RowCount; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(SolutionVector.Count);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] Intensity
        ///     *
        ///     *
        ///     *
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorIntensity(double Lambda)
        {
            //[0][0] Intensity
            //  *
            //  *
            //  *
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense((this.Count * 1), (this.Count * 1), 0.0);

            //[0] Intensity
            //  *
            //  *
            //  *
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>((this.Count * 1));

            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                for (int i = 0; i < this.Count; i++)
                {
                    #region Main Matrix

                    for (int j = 0; j < this.Count; j++)
                    {
                        HessianMatrix[(i * 1) + 0, (j * 1) + 0] += (this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]) * this[j].FirstDerivativeIntensity(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    }

                    #endregion

                    #region Vector build

                    SolutionVector[(i * 1) + 0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeIntensity(this.FittingCounts[n][0]);

                    #endregion
                }
            }

            for (int n = 0; n < HessianMatrix.RowCount; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(SolutionVector.Count);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        /// <summary>
        /// Calculates the Hessian matrix and the solution vecor
        /// </summary>
        /// <returns>
        /// The Deltas for the paramaeters:
        ///[0] Lorentz Ratio
        ///     *
        ///     *
        ///     *
        /// </returns>
        public MathNet.Numerics.LinearAlgebra.Vector<double> ParameterDeltaVektorLorentzRatio(double Lambda)
        {
            //[0][0] Lorentz Ratio
            //  *
            //  *
            //  *
            MathNet.Numerics.LinearAlgebra.Matrix<double> HessianMatrix = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense((this.Count * 1), (this.Count * 1), 0.0);

            //[0] Lorentz Ratio
            //  *
            //  *
            //  *
            MathNet.Numerics.LinearAlgebra.Vector<double> SolutionVector = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>((this.Count * 1));

            for (int n = 0; n < this.FittingCounts.Count; n++)
            {
                for (int i = 0; i < this.Count; i++)
                {
                    #region Main Matrix

                    for (int j = 0; j < this.Count; j++)
                    {
                        HessianMatrix[(i * 1) + 0, (j * 1) + 0] += (this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) * this[j].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]) / Math.Pow(this.FittingCounts[n][2], 2));
                    }

                    #endregion

                    #region Vector build

                    SolutionVector[(i * 1) + 0] += ((this.FittingCounts[n][1] - this.Y(this.FittingCounts[n][0])) / Math.Pow(this.FittingCounts[n][2], 2)) * this[i].FirstDerivativeLorentzRatio(this.FittingCounts[n][0]);

                    #endregion
                }
            }

            for (int n = 0; n < HessianMatrix.RowCount; n++)
            {
                HessianMatrix[n, n] *= (1 + Lambda);
            }

            MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = MathNet.Numerics.LinearAlgebra.CreateVector.Dense<double>(SolutionVector.Count);

            HessianMatrix.Solve(SolutionVector, ParamDelta);

            return ParamDelta;
        }

        #endregion

        #endregion

        #region Fitting using Multi Threading

        public event System.ComponentModel.PropertyChangedEventHandler FitFinished;
        public event System.ComponentModel.PropertyChangedEventHandler FitStarted;

        protected void OnFitStarted()
        {
            for (int n = 0; n < this.Count; n++ )
            {
                this[n].FitCompleted = false;
            }

            this._fitConverged = false;

            System.ComponentModel.PropertyChangedEventHandler handler = FitStarted;
            if (handler != null)
            {
                handler(this, new System.ComponentModel.PropertyChangedEventArgs("FitStarted"));
            }
        }

        protected void OnFitFinished()
        {
            for (int n = 0; n < this.Count; n++)
            {
                this[n].FitCompleted = true;
            }

            SetFittingErrors();
            this._doneEvent.Set();

            System.ComponentModel.PropertyChangedEventHandler handler = FitFinished;
            if (handler != null)
            {
                handler(this, new System.ComponentModel.PropertyChangedEventArgs("FitFinished"));
            }
        }

        // Wrapper method for use with thread pool. 
        public void FitRegionCallback(Object threadContext)
        {
            OnFitStarted();

            //Fit Zeug kommt hier hin
            if (this.startingLambda < 0.0)
            {
                this._fitConverged = Analysis.Fitting.LMA.FitPeakRegion(this);
            }
            else
            {
                this._fitConverged = Analysis.Fitting.LMA.FitPeakRegion(this, this.startingLambda);
            }

            OnFitFinished();
        }

        public void SetResetEvent(System.Threading.ManualResetEvent DoneEvent)
        {
            this._doneEvent = DoneEvent;
        }

        private void SetFittingErrors()
        {

        }

        #endregion

        #region Cloning

        public object Clone()
        {
            PeakRegionFunction Ret = new PeakRegionFunction();

            Ret._associatedPatternName = this._associatedPatternName;
            for (int n = 0; n < this.Count; n++)
            {
                Ret.Add(this[n].Clone() as PeakFunction);
            }

            Ret.functionType = this.functionType;
            
            for(int n = 0; n < this.FreeParameters.Length; n++)
            {
                Ret.FreeParameters[n] = this.FreeParameters[n];
            }

            Ret.backgroundSwitch = this.backgroundSwitch;
            Ret.backgroundFit = this.backgroundFit;

            Ret.FittingCounts = this.FittingCounts.Clone() as Pattern.Counts;
            Ret.PolynomialBackgroundFunction = Ret.PolynomialBackgroundFunction = new BackgroundPolynomial(this.PolynomialBackgroundFunction.Center, this.PolynomialBackgroundFunction.Constant, this.PolynomialBackgroundFunction.Aclivity);

            Ret._doneEvent = this._doneEvent;
            Ret._fitConverged = this._fitConverged;
            Ret.startingLambda = this.startingLambda;

            return Ret;
        }

        #endregion
    }
}
