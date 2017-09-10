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


namespace CalScec.Analysis.Fitting
{
    public static class LMA
    {
        private static double _lambda = 0.1;

        public static bool FitPeakFunction(Analysis.Peaks.Functions.PeakFunction PF)
        {
            return FitPeakFunction(PF, LMA._lambda);
        }

        public static bool FitPeakFunction(Analysis.Peaks.Functions.PeakFunction PF, double lambda)
        {
            if(PF.backgroundFit)
            {
                switch (PF.functionType)
                {
                    case 0:
                        if (PF.FreeParameters[0])
                        {
                            if (PF.FreeParameters[1])
                            {
                                if (PF.FreeParameters[3])
                                {
                                    return FitAngleSigmaIntensityBackground(PF, lambda);
                                }
                                else
                                {
                                    return FitAngleSigmaBackground(PF, lambda);
                                }
                            }
                            else
                            {
                                if (PF.FreeParameters[3])
                                {
                                    return FitSigmaIntensityBackground(PF, lambda);
                                }
                                else
                                {
                                    return FitSigmaBackground(PF, lambda);
                                }
                            }
                        }
                        else
                        {
                            if (PF.FreeParameters[1])
                            {
                                if (PF.FreeParameters[3])
                                {
                                    return FitAngleIntensityBackground(PF, lambda);
                                }
                                else
                                {
                                    return FitAngleBackground(PF, lambda);
                                }
                            }
                            else
                            {
                                if (PF.FreeParameters[3])
                                {
                                    return FitIntensityBackground(PF, lambda);
                                }
                                else
                                {
                                    return FitBackground(PF, lambda);
                                }
                            }
                        }
                    case 1:
                        goto case 0;
                    case 2:
                        goto default;
                    default:
                        if (PF.FreeParameters[2])
                        {
                            if (PF.FreeParameters[0])
                            {
                                if (PF.FreeParameters[1])
                                {
                                    if (PF.FreeParameters[3])
                                    {
                                        return FitAngleSigmaIntensityLorentzRatioBackground(PF, lambda);
                                    }
                                    else
                                    {
                                        return FitAngleSigmaLorentzRatioBackground(PF, lambda);
                                    }
                                }
                                else
                                {
                                    if (PF.FreeParameters[3])
                                    {
                                        return FitSigmaLorentzRatioIntensityBackground(PF, lambda);
                                    }
                                    else
                                    {
                                        return FitSigmaLorentzRatioBackground(PF, lambda);
                                    }
                                }
                            }
                            else
                            {
                                if (PF.FreeParameters[1])
                                {
                                    if (PF.FreeParameters[3])
                                    {
                                        return FitAngleLorentzRatioIntensityBackground(PF, lambda);
                                    }
                                    else
                                    {
                                        return FitAngleLorentzRatioBackground(PF, lambda);
                                    }
                                }
                                else
                                {
                                    if (PF.FreeParameters[3])
                                    {
                                        return FitIntensityLorentzRatioBackground(PF, lambda);
                                    }
                                    else
                                    {
                                        return FitLorentzRatioBackground(PF, lambda);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (PF.FreeParameters[0])
                            {
                                if (PF.FreeParameters[1])
                                {
                                    if (PF.FreeParameters[3])
                                    {
                                        return FitAngleSigmaIntensityBackground(PF, lambda);
                                    }
                                    else
                                    {
                                        return FitAngleSigmaBackground(PF, lambda);
                                    }
                                }
                                else
                                {
                                    if (PF.FreeParameters[3])
                                    {
                                        return FitSigmaIntensityBackground(PF, lambda);
                                    }
                                    else
                                    {
                                        return FitSigmaBackground(PF, lambda);
                                    }
                                }
                            }
                            else
                            {
                                if (PF.FreeParameters[1])
                                {
                                    if (PF.FreeParameters[3])
                                    {
                                        return FitAngleIntensityBackground(PF, lambda);
                                    }
                                    else
                                    {
                                        return FitAngleBackground(PF, lambda);
                                    }
                                }
                                else
                                {
                                    if (PF.FreeParameters[3])
                                    {
                                        return FitIntensityBackground(PF, lambda);
                                    }
                                    else
                                    {
                                        return FitBackground(PF, lambda);
                                    }
                                }
                            }
                        }
                }
            }
            else
            {
                switch (PF.functionType)
                {
                    case 0:
                        if (PF.FreeParameters[0])
                        {
                            if (PF.FreeParameters[1])
                            {
                                if (PF.FreeParameters[3])
                                {
                                    return FitAngleSigmaIntensity(PF, lambda);
                                }
                                else
                                {
                                    return FitAngleSigma(PF, lambda);
                                }
                            }
                            else
                            {
                                if (PF.FreeParameters[3])
                                {
                                    return FitSigmaIntensity(PF, lambda);
                                }
                                else
                                {
                                    return FitSigma(PF, lambda);
                                }
                            }
                        }
                        else
                        {
                            if (PF.FreeParameters[1])
                            {
                                if (PF.FreeParameters[3])
                                {
                                    return FitAngleIntensity(PF, lambda);
                                }
                                else
                                {
                                    return FitAngle(PF, lambda);
                                }
                            }
                            else
                            {
                                if (PF.FreeParameters[3])
                                {
                                    return FitIntensity(PF, lambda);
                                }
                                else
                                {
                                    return true;
                                }
                            }
                        }
                    case 1:
                        goto case 0;
                    case 2:
                        goto default;
                    default:
                        if (PF.FreeParameters[2])
                        {
                            if (PF.FreeParameters[0])
                            {
                                if (PF.FreeParameters[1])
                                {
                                    if (PF.FreeParameters[3])
                                    {
                                        return FitAngleSigmaIntensityLorentzRatio(PF, lambda);
                                    }
                                    else
                                    {
                                        return FitAngleSigmaLorentzRatio(PF, lambda);
                                    }
                                }
                                else
                                {
                                    if (PF.FreeParameters[3])
                                    {
                                        return FitSigmaLorentzRatioIntensity(PF, lambda);
                                    }
                                    else
                                    {
                                        return FitSigmaLorentzRatio(PF, lambda);
                                    }
                                }
                            }
                            else
                            {
                                if (PF.FreeParameters[1])
                                {
                                    if (PF.FreeParameters[3])
                                    {
                                        return FitAngleLorentzRatioIntensity(PF, lambda);
                                    }
                                    else
                                    {
                                        return FitAngleLorentzRatio(PF, lambda);
                                    }
                                }
                                else
                                {
                                    if (PF.FreeParameters[3])
                                    {
                                        return FitIntensityLorentzRatio(PF, lambda);
                                    }
                                    else
                                    {
                                        return FitLorentzRatio(PF, lambda);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (PF.FreeParameters[0])
                            {
                                if (PF.FreeParameters[1])
                                {
                                    if (PF.FreeParameters[3])
                                    {
                                        return FitAngleSigmaIntensity(PF, lambda);
                                    }
                                    else
                                    {
                                        return FitAngleSigma(PF, lambda);
                                    }
                                }
                                else
                                {
                                    if (PF.FreeParameters[3])
                                    {
                                        return FitSigmaIntensity(PF, lambda);
                                    }
                                    else
                                    {
                                        return FitSigma(PF, lambda);
                                    }
                                }
                            }
                            else
                            {
                                if (PF.FreeParameters[1])
                                {
                                    if (PF.FreeParameters[3])
                                    {
                                        return FitAngleIntensity(PF, lambda);
                                    }
                                    else
                                    {
                                        return FitAngle(PF, lambda);
                                    }
                                }
                                else
                                {
                                    if (PF.FreeParameters[3])
                                    {
                                        return FitIntensity(PF, lambda);
                                    }
                                    else
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                }
            }
        }

        public static bool FitPeakRegion(Analysis.Peaks.Functions.PeakRegionFunction PRF)
        {
            return FitPeakRegion(PRF, LMA._lambda);
        }

        public static bool FitPeakRegion(Analysis.Peaks.Functions.PeakRegionFunction PRF, double lambda)
        {
            if (PRF.backgroundFit)
            {
                switch (PRF.functionType)
                {
                    case 0:
                        if (PRF.FreeParameters[0])
                        {
                            if (PRF.FreeParameters[1])
                            {
                                if (PRF.FreeParameters[3])
                                {
                                    return FitAngleSigmaIntensityBackground(PRF, lambda);
                                }
                                else
                                {
                                    return FitAngleSigmaBackground(PRF, lambda);
                                }
                            }
                            else
                            {
                                if (PRF.FreeParameters[3])
                                {
                                    return FitSigmaIntensityBackground(PRF, lambda);
                                }
                                else
                                {
                                    return FitSigmaBackground(PRF, lambda);
                                }
                            }
                        }
                        else
                        {
                            if (PRF.FreeParameters[1])
                            {
                                if (PRF.FreeParameters[3])
                                {
                                    return FitAngleIntensityBackground(PRF, lambda);
                                }
                                else
                                {
                                    return FitAngleBackground(PRF, lambda);
                                }
                            }
                            else
                            {
                                if (PRF.FreeParameters[3])
                                {
                                    return FitIntensityBackground(PRF, lambda);
                                }
                                else
                                {
                                    return FitBackground(PRF, lambda);
                                }
                            }
                        }
                    case 1:
                        goto case 0;
                    case 2:
                        goto default;
                    default:
                        if (PRF.FreeParameters[2])
                        {
                            if (PRF.FreeParameters[0])
                            {
                                if (PRF.FreeParameters[1])
                                {
                                    if (PRF.FreeParameters[3])
                                    {
                                        return FitAngleSigmaIntensityLorentzRatioBackground(PRF, lambda);
                                    }
                                    else
                                    {
                                        return FitAngleSigmaLorentzRatioBackground(PRF, lambda);
                                    }
                                }
                                else
                                {
                                    if (PRF.FreeParameters[3])
                                    {
                                        return FitSigmaLorentzRatioIntensityBackground(PRF, lambda);
                                    }
                                    else
                                    {
                                        return FitSigmaLorentzRatioBackground(PRF, lambda);
                                    }
                                }
                            }
                            else
                            {
                                if (PRF.FreeParameters[1])
                                {
                                    if (PRF.FreeParameters[3])
                                    {
                                        return FitAngleLorentzRatioIntensityBackground(PRF, lambda);
                                    }
                                    else
                                    {
                                        return FitAngleLorentzRatioBackground(PRF, lambda);
                                    }
                                }
                                else
                                {
                                    if (PRF.FreeParameters[3])
                                    {
                                        return FitIntensityLorentzRatioBackground(PRF, lambda);
                                    }
                                    else
                                    {
                                        return FitLorentzRatioBackground(PRF, lambda);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (PRF.FreeParameters[0])
                            {
                                if (PRF.FreeParameters[1])
                                {
                                    if (PRF.FreeParameters[3])
                                    {
                                        return FitAngleSigmaIntensityBackground(PRF, lambda);
                                    }
                                    else
                                    {
                                        return FitAngleSigmaBackground(PRF, lambda);
                                    }
                                }
                                else
                                {
                                    if (PRF.FreeParameters[3])
                                    {
                                        return FitSigmaIntensityBackground(PRF, lambda);
                                    }
                                    else
                                    {
                                        return FitSigmaBackground(PRF, lambda);
                                    }
                                }
                            }
                            else
                            {
                                if (PRF.FreeParameters[1])
                                {
                                    if (PRF.FreeParameters[3])
                                    {
                                        return FitAngleIntensityBackground(PRF, lambda);
                                    }
                                    else
                                    {
                                        return FitAngleBackground(PRF, lambda);
                                    }
                                }
                                else
                                {
                                    if (PRF.FreeParameters[3])
                                    {
                                        return FitIntensityBackground(PRF, lambda);
                                    }
                                    else
                                    {
                                        return FitBackground(PRF, lambda);
                                    }
                                }
                            }
                        }
                }
            }
            else
            {
                switch (PRF.functionType)
                {
                    case 0:
                        if (PRF.FreeParameters[0])
                        {
                            if (PRF.FreeParameters[1])
                            {
                                if (PRF.FreeParameters[3])
                                {
                                    return FitAngleSigmaIntensity(PRF, lambda);
                                }
                                else
                                {
                                    return FitAngleSigma(PRF, lambda);
                                }
                            }
                            else
                            {
                                if (PRF.FreeParameters[3])
                                {
                                    return FitSigmaIntensity(PRF, lambda);
                                }
                                else
                                {
                                    return FitSigma(PRF, lambda);
                                }
                            }
                        }
                        else
                        {
                            if (PRF.FreeParameters[1])
                            {
                                if (PRF.FreeParameters[3])
                                {
                                    return FitAngleIntensity(PRF, lambda);
                                }
                                else
                                {
                                    return FitAngle(PRF, lambda);
                                }
                            }
                            else
                            {
                                if (PRF.FreeParameters[3])
                                {
                                    return FitIntensity(PRF, lambda);
                                }
                                else
                                {
                                    return true;
                                }
                            }
                        }
                    case 1:
                        goto case 0;
                    case 2:
                        goto default;
                    default:
                        if (PRF.FreeParameters[2])
                        {
                            if (PRF.FreeParameters[0])
                            {
                                if (PRF.FreeParameters[1])
                                {
                                    if (PRF.FreeParameters[3])
                                    {
                                        return FitAngleSigmaIntensityLorentzRatio(PRF, lambda);
                                    }
                                    else
                                    {
                                        return FitAngleSigmaLorentzRatio(PRF, lambda);
                                    }
                                }
                                else
                                {
                                    if (PRF.FreeParameters[3])
                                    {
                                        return FitSigmaLorentzRatioIntensity(PRF, lambda);
                                    }
                                    else
                                    {
                                        return FitSigmaLorentzRatio(PRF, lambda);
                                    }
                                }
                            }
                            else
                            {
                                if (PRF.FreeParameters[1])
                                {
                                    if (PRF.FreeParameters[3])
                                    {
                                        return FitAngleLorentzRatioIntensity(PRF, lambda);
                                    }
                                    else
                                    {
                                        return FitAngleLorentzRatio(PRF, lambda);
                                    }
                                }
                                else
                                {
                                    if (PRF.FreeParameters[3])
                                    {
                                        return FitIntensityLorentzRatio(PRF, lambda);
                                    }
                                    else
                                    {
                                        return FitLorentzRatio(PRF, lambda);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (PRF.FreeParameters[0])
                            {
                                if (PRF.FreeParameters[1])
                                {
                                    if (PRF.FreeParameters[3])
                                    {
                                        return FitAngleSigmaIntensity(PRF, lambda);
                                    }
                                    else
                                    {
                                        return FitAngleSigma(PRF, lambda);
                                    }
                                }
                                else
                                {
                                    if (PRF.FreeParameters[3])
                                    {
                                        return FitSigmaIntensity(PRF, lambda);
                                    }
                                    else
                                    {
                                        return FitSigma(PRF, lambda);
                                    }
                                }
                            }
                            else
                            {
                                if (PRF.FreeParameters[1])
                                {
                                    if (PRF.FreeParameters[3])
                                    {
                                        return FitAngleIntensity(PRF, lambda);
                                    }
                                    else
                                    {
                                        return FitAngle(PRF, lambda);
                                    }
                                }
                                else
                                {
                                    if (PRF.FreeParameters[3])
                                    {
                                        return FitIntensity(PRF, lambda);
                                    }
                                    else
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                }
            }
        }

        public static bool FitMacroElasticModul(Fitting.LinearFunction fittingFunction, Pattern.Counts UsedCounts)
        {
            bool Converged = false;
            double Lambda = LMA._lambda;

            Fitting.LinearFunction TrialFittingFunction = fittingFunction.Clone() as Fitting.LinearFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = fittingFunction.ParameterDeltaVektorAclivityConstant(Lambda, UsedCounts);

                #region Parameter calculations

                TrialFittingFunction.Aclivity += ParamDelta[0];
                TrialFittingFunction.Constant += ParamDelta[1];

                #endregion

                double Chi2Trial = Chi2.Chi2LinearFunction(TrialFittingFunction, UsedCounts);
                double Chi2Real = Chi2.Chi2LinearFunction(fittingFunction, UsedCounts);

                if (Math.Abs(Chi2Trial - Chi2Real) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (Chi2.Chi2LinearFunction(TrialFittingFunction, UsedCounts) > Chi2.Chi2LinearFunction(fittingFunction, UsedCounts))
                    {
                        TrialFittingFunction.Aclivity = fittingFunction.Aclivity;
                        TrialFittingFunction.Constant = fittingFunction.Constant;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        fittingFunction.Aclivity = TrialFittingFunction.Aclivity;
                        fittingFunction.Constant = TrialFittingFunction.Constant;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                    else
                    {
                        if (Chi2Real > Chi2Trial)
                        {
                            fittingFunction.Aclivity = TrialFittingFunction.Aclivity;
                            fittingFunction.Constant = TrialFittingFunction.Constant;

                            Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                        }
                    }
                }
            }

            return Converged;
        }

        #region Elastic contant fit routines

        #region Voigt

        public static bool FitElasticityTensorVoigtCubicIsotrope(Stress.Microsopic.ElasticityTensors ET, bool classicCalculation)
        {
            bool Converged = false;
            double Lambda = LMA._lambda;

            Stress.Microsopic.ElasticityTensors TrialET = ET.Clone() as Stress.Microsopic.ElasticityTensors;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta;

                if(classicCalculation)
                {
                    ParamDelta = ET.ParameterDeltaVektorVoigtCubicIsotropeClassic(Lambda);
                }
                else
                {
                    ParamDelta = ET.ParameterDeltaVektorVoigtCubicIsotropeMacroscopic(Lambda);
                }

                #region Parameter calculations

                TrialET.C11 += ParamDelta[0];
                TrialET.C12 += ParamDelta[1];

                #endregion

                double Chi2Trial = Chi2.Chi2ClassicVoigtCubicIsotrope(TrialET);
                double Chi2Real = Chi2.Chi2ClassicVoigtCubicIsotrope(ET);

                if (Math.Abs(Chi2Trial - Chi2Real) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (Chi2Trial > Chi2Real)
                    {
                        TrialET.C11 = ET.C11;
                        TrialET.C12 = ET.C12;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        ET.C11 = TrialET.C11;
                        ET.C12 = TrialET.C12;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                    else
                    {
                        if (Chi2Real > Chi2Trial)
                        {
                            ET.C11 = TrialET.C11;
                            ET.C12 = TrialET.C12;

                            Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                        }
                    }
                }
            }

            return Converged;
        }

        public static bool FitElasticityTensorVoigtCubic(Stress.Microsopic.ElasticityTensors ET, bool classicCalculation)
        {
            bool Converged = false;
            double Lambda = 100000;

            Stress.Microsopic.ElasticityTensors TrialET = ET.Clone() as Stress.Microsopic.ElasticityTensors;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta;

                if (classicCalculation)
                {
                    ParamDelta = ET.ParameterDeltaVektorVoigtCubicClassic(Lambda);
                }
                else
                {
                    ParamDelta = ET.ParameterDeltaVektorVoigtCubicMacroscopic(Lambda);
                }

                #region Parameter calculations

                TrialET.C11 += ParamDelta[0];
                TrialET.C12 += ParamDelta[1];
                TrialET.C44 += ParamDelta[2];

                #endregion

                double Chi2Trial = Chi2.Chi2ClassicVoigtCubic(TrialET);
                double Chi2Real = Chi2.Chi2ClassicVoigtCubic(ET);

                if (Math.Abs(Chi2Trial - Chi2Real) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (Chi2Trial > Chi2Real)
                    {
                        TrialET.C11 = ET.C11;
                        TrialET.C12 = ET.C12;
                        TrialET.C44 = ET.C44;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        ET.C11 = TrialET.C11;
                        ET.C12 = TrialET.C12;
                        ET.C44 = TrialET.C44;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;

                        if(Lambda < 50)
                        {
                            Lambda = 50;
                        }
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                    else
                    {
                        if (Chi2Real > Chi2Trial)
                        {
                            ET.C11 = TrialET.C11;
                            ET.C12 = TrialET.C12;
                            ET.C44 = TrialET.C44;

                            Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                        }
                    }
                }
            }

            return Converged;
        }

        public static bool FitElasticityTensorVoigtType1(Stress.Microsopic.ElasticityTensors ET, bool classicCalculation)
        {
            bool Converged = false;
            double Lambda = LMA._lambda;

            Stress.Microsopic.ElasticityTensors TrialET = ET.Clone() as Stress.Microsopic.ElasticityTensors;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta;

                if (classicCalculation)
                {
                    ParamDelta = ET.ParameterDeltaVektorVoigtType1Classic(Lambda);
                }
                else
                {
                    ParamDelta = ET.ParameterDeltaVektorVoigtType1Macroscopic(Lambda);
                }

                #region Parameter calculations

                TrialET.C11 += ParamDelta[0];
                TrialET.C33 += ParamDelta[1];
                TrialET.C12 += ParamDelta[2];
                TrialET.C13 += ParamDelta[3];
                TrialET.C44 += ParamDelta[4];

                #endregion

                double Chi2Trial = Chi2.Chi2ClassicVoigtCubicIsotrope(TrialET);
                double Chi2Real = Chi2.Chi2ClassicVoigtCubicIsotrope(ET);

                if (Math.Abs(Chi2Trial - Chi2Real) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (Chi2Trial > Chi2Real)
                    {
                        TrialET.C11 = ET.C11;
                        TrialET.C33 = ET.C33;
                        TrialET.C12 = ET.C12;
                        TrialET.C13 = ET.C13;
                        TrialET.C44 = ET.C44;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        ET.C11 = TrialET.C11;
                        ET.C33 = TrialET.C33;
                        ET.C12 = TrialET.C12;
                        ET.C13 = TrialET.C13;
                        ET.C44 = TrialET.C44;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                    else
                    {
                        if (Chi2Real > Chi2Trial)
                        {
                            ET.C11 = TrialET.C11;
                            ET.C33 = TrialET.C33;
                            ET.C12 = TrialET.C12;
                            ET.C13 = TrialET.C13;
                            ET.C44 = TrialET.C44;

                            Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                        }
                    }
                }
            }

            return Converged;
        }

        public static bool FitElasticityTensorVoigtType2(Stress.Microsopic.ElasticityTensors ET, bool classicCalculation)
        {
            bool Converged = false;
            double Lambda = LMA._lambda;

            Stress.Microsopic.ElasticityTensors TrialET = ET.Clone() as Stress.Microsopic.ElasticityTensors;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta;

                if (classicCalculation)
                {
                    ParamDelta = ET.ParameterDeltaVektorVoigtType2Classic(Lambda);
                }
                else
                {
                    ParamDelta = ET.ParameterDeltaVektorVoigtType2Macroscopic(Lambda);
                }

                #region Parameter calculations

                TrialET.C11 += ParamDelta[0];
                TrialET.C33 += ParamDelta[1];
                TrialET.C12 += ParamDelta[2];
                TrialET.C13 += ParamDelta[3];
                TrialET.C44 += ParamDelta[4];
                TrialET.C66 += ParamDelta[5];

                #endregion

                double Chi2Trial = Chi2.Chi2ClassicVoigtCubicIsotrope(TrialET);
                double Chi2Real = Chi2.Chi2ClassicVoigtCubicIsotrope(ET);

                if (Math.Abs(Chi2Trial - Chi2Real) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (Chi2Trial > Chi2Real)
                    {
                        TrialET.C11 = ET.C11;
                        TrialET.C33 = ET.C33;
                        TrialET.C12 = ET.C12;
                        TrialET.C13 = ET.C13;
                        TrialET.C44 = ET.C44;
                        TrialET.C66 = ET.C66;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        ET.C11 = TrialET.C11;
                        ET.C33 = TrialET.C33;
                        ET.C12 = TrialET.C12;
                        ET.C13 = TrialET.C13;
                        ET.C44 = TrialET.C44;
                        ET.C66 = TrialET.C66;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                    else
                    {
                        if (Chi2Real > Chi2Trial)
                        {
                            ET.C11 = TrialET.C11;
                            ET.C33 = TrialET.C33;
                            ET.C12 = TrialET.C12;
                            ET.C13 = TrialET.C13;
                            ET.C44 = TrialET.C44;
                            ET.C66 = TrialET.C66;

                            Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                        }
                    }
                }
            }

            return Converged;
        }

        public static bool FitElasticityTensorVoigtType3(Stress.Microsopic.ElasticityTensors ET, bool classicCalculation)
        {
            bool Converged = false;
            double Lambda = LMA._lambda;

            Stress.Microsopic.ElasticityTensors TrialET = ET.Clone() as Stress.Microsopic.ElasticityTensors;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta;

                if (classicCalculation)
                {
                    ParamDelta = ET.ParameterDeltaVektorVoigtType3Classic(Lambda);
                }
                else
                {
                    ParamDelta = ET.ParameterDeltaVektorVoigtType3Macroscopic(Lambda);
                }

                #region Parameter calculations

                TrialET.C11 += ParamDelta[0];
                TrialET.C22 += ParamDelta[1];
                TrialET.C33 += ParamDelta[2];
                TrialET.C12 += ParamDelta[3];
                TrialET.C13 += ParamDelta[4];
                TrialET.C23 += ParamDelta[5];
                TrialET.C44 += ParamDelta[6];
                TrialET.C55 += ParamDelta[7];
                TrialET.C66 += ParamDelta[8];

                #endregion

                double Chi2Trial = Chi2.Chi2ClassicVoigtCubicIsotrope(TrialET);
                double Chi2Real = Chi2.Chi2ClassicVoigtCubicIsotrope(ET);

                if (Math.Abs(Chi2Trial - Chi2Real) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (Chi2Trial > Chi2Real)
                    {
                        TrialET.C11 = ET.C11;
                        TrialET.C22 = ET.C22;
                        TrialET.C33 = ET.C33;
                        TrialET.C12 = ET.C12;
                        TrialET.C13 = ET.C13;
                        TrialET.C23 = ET.C23;
                        TrialET.C44 = ET.C44;
                        TrialET.C55 = ET.C55;
                        TrialET.C66 = ET.C66;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        ET.C11 = TrialET.C11;
                        ET.C22 = TrialET.C22;
                        ET.C33 = TrialET.C33;
                        ET.C12 = TrialET.C12;
                        ET.C13 = TrialET.C13;
                        ET.C23 = TrialET.C23;
                        ET.C44 = TrialET.C44;
                        ET.C55 = TrialET.C55;
                        ET.C66 = TrialET.C66;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                    else
                    {
                        if (Chi2Real > Chi2Trial)
                        {
                            ET.C11 = TrialET.C11;
                            ET.C22 = TrialET.C22;
                            ET.C33 = TrialET.C33;
                            ET.C12 = TrialET.C12;
                            ET.C13 = TrialET.C13;
                            ET.C23 = TrialET.C23;
                            ET.C44 = TrialET.C44;
                            ET.C55 = TrialET.C55;
                            ET.C66 = TrialET.C66;

                            Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                        }
                    }
                }
            }

            return Converged;
        }

        #endregion

        #region Reuss

        public static bool FitElasticityTensorReussCubic(Stress.Microsopic.ElasticityTensors ET, bool classicCalculation)
        {
            bool Converged = false;
            double Lambda = LMA._lambda;

            Stress.Microsopic.ElasticityTensors TrialET = ET.Clone() as Stress.Microsopic.ElasticityTensors;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta;

                if (classicCalculation)
                {
                    ParamDelta = ET.ParameterDeltaVektorReussCubicClassic(Lambda);
                }
                else
                {
                    ParamDelta = ET.ParameterDeltaVektorReussCubicMacroscopic(Lambda);
                }

                #region Parameter calculations

                TrialET.S11 += ParamDelta[0];
                TrialET.S12 += ParamDelta[1];
                TrialET.S44 += ParamDelta[2];

                #endregion

                double Chi2Trial = Chi2.Chi2ClassicReussCubic(TrialET);
                double Chi2Real = Chi2.Chi2ClassicReussCubic(ET);

                if (Math.Abs(Chi2Trial - Chi2Real) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (Chi2Trial > Chi2Real)
                    {
                        TrialET.S11 = ET.S11;
                        TrialET.S12 = ET.S12;
                        TrialET.S44 = ET.S44;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        ET.S11 = TrialET.S11;
                        ET.S12 = TrialET.S12;
                        ET.S44 = TrialET.S44;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                    else
                    {
                        if (Chi2Real > Chi2Trial)
                        {
                            ET.S11 = TrialET.S11;
                            ET.S12 = TrialET.S12;
                            ET.S44 = TrialET.S44;

                            Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                        }
                    }
                }
            }

            return Converged;
        }

        public static bool FitElasticityTensorReussHexagonal(Stress.Microsopic.ElasticityTensors ET, bool classicCalculation)
        {
            bool Converged = false;
            double Lambda = LMA._lambda;

            Stress.Microsopic.ElasticityTensors TrialET = ET.Clone() as Stress.Microsopic.ElasticityTensors;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta;

                if (classicCalculation)
                {
                    ParamDelta = ET.ParameterDeltaVektorReussHexagonalClassic(Lambda);
                }
                else
                {
                    ParamDelta = ET.ParameterDeltaVektorReussHexagonalMacroscopic(Lambda);
                }

                #region Parameter calculations

                TrialET.S11 += ParamDelta[0];
                TrialET.S33 += ParamDelta[1];
                TrialET.S12 += ParamDelta[2];
                TrialET.S13 += ParamDelta[3];
                TrialET.S44 += ParamDelta[4];

                #endregion

                double Chi2Trial = Chi2.Chi2ClassicReussHexagonal(TrialET);
                double Chi2Real = Chi2.Chi2ClassicReussHexagonal(ET);

                if (Math.Abs(Chi2Trial - Chi2Real) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (Chi2Trial > Chi2Real)
                    {
                        TrialET.S11 = ET.S11;
                        TrialET.S33 = ET.S33;
                        TrialET.S12 = ET.S12;
                        TrialET.S13 = ET.S13;
                        TrialET.S44 = ET.S44;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        ET.S11 = TrialET.S11;
                        ET.S33 = TrialET.S33;
                        ET.S12 = TrialET.S12;
                        ET.S13 = TrialET.S13;
                        ET.S44 = TrialET.S44;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                    else
                    {
                        if (Chi2Real > Chi2Trial)
                        {
                            ET.S11 = TrialET.S11;
                            ET.S33 = TrialET.S33;
                            ET.S12 = TrialET.S12;
                            ET.S13 = TrialET.S13;
                            ET.S44 = TrialET.S44;

                            Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                        }
                    }
                }
            }

            return Converged;
        }

        #endregion

        #region Hill

        public static bool FitElasticityTensorHillCubic(Stress.Microsopic.ElasticityTensors ET, bool classicCalculation)
        {
            bool Converged = false;
            double Lambda = LMA._lambda;

            Stress.Microsopic.ElasticityTensors TrialET = ET.Clone() as Stress.Microsopic.ElasticityTensors;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta;

                if (classicCalculation)
                {
                    ParamDelta = ET.ParameterDeltaVektorHillCubicClassic(Lambda);
                }
                else
                {
                    ParamDelta = ET.ParameterDeltaVektorHillCubicMacroscopic(Lambda);
                }

                #region Parameter calculations

                TrialET.S11 += ParamDelta[0];
                TrialET.S12 += ParamDelta[1];
                TrialET.S44 += ParamDelta[2];

                #endregion

                double Chi2Trial = Chi2.Chi2ClassicHillCubic(TrialET);
                double Chi2Real = Chi2.Chi2ClassicHillCubic(ET);

                if (Math.Abs(Chi2Trial - Chi2Real) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (Chi2Trial > Chi2Real)
                    {
                        TrialET.S11 = ET.S11;
                        TrialET.S12 = ET.S12;
                        TrialET.S44 = ET.S44;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        ET.S11 = TrialET.S11;
                        ET.S12 = TrialET.S12;
                        ET.S44 = TrialET.S44;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                    else
                    {
                        if (Chi2Real > Chi2Trial)
                        {
                            ET.S11 = TrialET.S11;
                            ET.S12 = TrialET.S12;
                            ET.S44 = TrialET.S44;

                            Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                        }
                    }
                }
            }

            return Converged;
        }

        public static bool FitElasticityTensorHillHexagonal(Stress.Microsopic.ElasticityTensors ET, bool classicCalculation)
        {
            bool Converged = false;
            double Lambda = LMA._lambda;

            Stress.Microsopic.ElasticityTensors TrialET = ET.Clone() as Stress.Microsopic.ElasticityTensors;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta;

                if (classicCalculation)
                {
                    ParamDelta = ET.ParameterDeltaVektorHillHexagonalClassic(Lambda);
                }
                else
                {
                    ParamDelta = ET.ParameterDeltaVektorHillHexagonalMacroscopic(Lambda);
                }

                #region Parameter calculations

                TrialET.S11 += ParamDelta[0];
                TrialET.S33 += ParamDelta[1];
                TrialET.S12 += ParamDelta[2];
                TrialET.S13 += ParamDelta[3];
                TrialET.S44 += ParamDelta[4];

                #endregion

                double Chi2Trial = Chi2.Chi2ClassicHillHexagonal(TrialET);
                double Chi2Real = Chi2.Chi2ClassicHillHexagonal(ET);

                if (Math.Abs(Chi2Trial - Chi2Real) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (Chi2Trial > Chi2Real)
                    {
                        TrialET.S11 = ET.S11;
                        TrialET.S33 = ET.S33;
                        TrialET.S12 = ET.S12;
                        TrialET.S13 = ET.S13;
                        TrialET.S44 = ET.S44;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        ET.S11 = TrialET.S11;
                        ET.S33 = TrialET.S33;
                        ET.S12 = TrialET.S12;
                        ET.S13 = TrialET.S13;
                        ET.S44 = TrialET.S44;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                    else
                    {
                        if (Chi2Real > Chi2Trial)
                        {
                            ET.S11 = TrialET.S11;
                            ET.S33 = TrialET.S33;
                            ET.S12 = TrialET.S12;
                            ET.S13 = TrialET.S13;
                            ET.S44 = TrialET.S44;

                            Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                        }
                    }
                }
            }

            return Converged;
        }

        #endregion

        #region Geometric Hill

        public static bool FitElasticityTensorGeometricHillCubic(Stress.Microsopic.ElasticityTensors ET, bool classicCalculation)
        {
            bool Converged = false;
            double Lambda = LMA._lambda;

            Stress.Microsopic.ElasticityTensors TrialET = ET.Clone() as Stress.Microsopic.ElasticityTensors;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta;

                if (classicCalculation)
                {
                    ParamDelta = ET.ParameterDeltaVektorGeometricHillCubicClassic(Lambda);
                }
                else
                {
                    ParamDelta = ET.ParameterDeltaVektorGeometricHillCubicMacroscopic(Lambda);
                }

                #region Parameter calculations

                TrialET.S11 += ParamDelta[0];
                TrialET.S12 += ParamDelta[1];
                TrialET.S44 += ParamDelta[2];

                #endregion

                double Chi2Trial = Chi2.Chi2ClassicGeometricHillCubic(TrialET);
                double Chi2Real = Chi2.Chi2ClassicGeometricHillCubic(ET);

                if (Math.Abs(Chi2Trial - Chi2Real) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (Chi2Trial > Chi2Real)
                    {
                        TrialET.S11 = ET.S11;
                        TrialET.S12 = ET.S12;
                        TrialET.S44 = ET.S44;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        ET.S11 = TrialET.S11;
                        ET.S12 = TrialET.S12;
                        ET.S44 = TrialET.S44;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                    else
                    {
                        if (Chi2Real > Chi2Trial)
                        {
                            ET.S11 = TrialET.S11;
                            ET.S12 = TrialET.S12;
                            ET.S44 = TrialET.S44;

                            Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                        }
                    }
                }
            }

            return Converged;
        }

        public static bool FitElasticityTensorGeometricHillHexagonal(Stress.Microsopic.ElasticityTensors ET, bool classicCalculation)
        {
            bool Converged = false;
            double Lambda = LMA._lambda;

            Stress.Microsopic.ElasticityTensors TrialET = ET.Clone() as Stress.Microsopic.ElasticityTensors;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta;

                if (classicCalculation)
                {
                    ParamDelta = ET.ParameterDeltaVektorGeometricHillHexagonalClassic(Lambda);
                }
                else
                {
                    ParamDelta = ET.ParameterDeltaVektorGeometricHillHexagonalMacroscopic(Lambda);
                }

                #region Parameter calculations

                TrialET.S11 += ParamDelta[0];
                TrialET.S33 += ParamDelta[1];
                TrialET.S12 += ParamDelta[2];
                TrialET.S13 += ParamDelta[3];
                TrialET.S44 += ParamDelta[4];

                #endregion

                double Chi2Trial = Chi2.Chi2ClassicGeometricHillHexagonal(TrialET);
                double Chi2Real = Chi2.Chi2ClassicGeometricHillHexagonal(ET);

                if (Math.Abs(Chi2Trial - Chi2Real) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (Chi2Trial > Chi2Real)
                    {
                        TrialET.S11 = ET.S11;
                        TrialET.S33 = ET.S33;
                        TrialET.S12 = ET.S12;
                        TrialET.S13 = ET.S13;
                        TrialET.S44 = ET.S44;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        ET.S11 = TrialET.S11;
                        ET.S33 = TrialET.S33;
                        ET.S12 = TrialET.S12;
                        ET.S13 = TrialET.S13;
                        ET.S44 = TrialET.S44;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                    else
                    {
                        if (Chi2Real > Chi2Trial)
                        {
                            ET.S11 = TrialET.S11;
                            ET.S33 = TrialET.S33;
                            ET.S12 = TrialET.S12;
                            ET.S13 = TrialET.S13;
                            ET.S44 = TrialET.S44;

                            Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                        }
                    }
                }
            }

            return Converged;
        }

        #endregion

        #region Kroener

        public static bool FitElasticityTensorKroenerCubicStiffness(Stress.Microsopic.ElasticityTensors ET, bool classicCalculation)
        {
            bool Converged = false;
            double Lambda = LMA._lambda;

            Stress.Microsopic.ElasticityTensors TrialET = ET.Clone() as Stress.Microsopic.ElasticityTensors;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta;

                if (classicCalculation)
                {
                    ParamDelta = ET.ParameterDeltaVektorKroenerCubicStiffnessClassic(Lambda);
                }
                else
                {
                    ParamDelta = ET.ParameterDeltaVektorKroenerCubicStiffnessMacroscopic(Lambda);
                }

                #region Parameter calculations

                TrialET.C11 += ParamDelta[0];
                TrialET.C12 += ParamDelta[1];
                TrialET.C44 += ParamDelta[2];

                #endregion

                double Chi2Trial = Chi2.Chi2ClassicKroenerCubicStiffness(TrialET);
                double Chi2Real = Chi2.Chi2ClassicKroenerCubicStiffness(ET);

                if (Math.Abs(Chi2Trial - Chi2Real) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (Chi2Trial > Chi2Real)
                    {
                        TrialET.C11 = ET.C11;
                        TrialET.C12 = ET.C12;
                        TrialET.C44 = ET.C44;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        ET.C11 = TrialET.C11;
                        ET.C12 = TrialET.C12;
                        ET.C44 = TrialET.C44;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                    else
                    {
                        if (Chi2Real > Chi2Trial)
                        {
                            ET.C11 = TrialET.C11;
                            ET.C12 = TrialET.C12;
                            ET.C44 = TrialET.C44;

                            Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                        }
                    }
                }
            }

            return Converged;
        }
        
        public static bool FitElasticityTensorKroenerCubicCompliance(Stress.Microsopic.ElasticityTensors ET, bool classicCalculation)
        {
            bool Converged = false;
            double Lambda = LMA._lambda;

            Stress.Microsopic.ElasticityTensors TrialET = ET.Clone() as Stress.Microsopic.ElasticityTensors;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta;

                if (classicCalculation)
                {
                    ParamDelta = ET.ParameterDeltaVektorKroenerCubicComplianceClassic(Lambda);
                }
                else
                {
                    ParamDelta = ET.ParameterDeltaVektorKroenerCubicComplianceMacroscopic(Lambda);
                }

                #region Parameter calculations

                TrialET.S11 += ParamDelta[0];
                TrialET.S12 += ParamDelta[1];
                TrialET.S44 += ParamDelta[2];

                #endregion

                double Chi2Trial = Chi2.Chi2ClassicKroenerCubicCompliance(TrialET);
                double Chi2Real = Chi2.Chi2ClassicKroenerCubicCompliance(ET);

                if (Math.Abs(Chi2Trial - Chi2Real) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (Chi2Trial > Chi2Real)
                    {
                        TrialET.S11 = ET.S11;
                        TrialET.S12 = ET.S12;
                        TrialET.S44 = ET.S44;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        ET.S11 = TrialET.S11;
                        ET.S12 = TrialET.S12;
                        ET.S44 = TrialET.S44;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                    else
                    {
                        if (Chi2Real > Chi2Trial)
                        {
                            ET.S11 = TrialET.S11;
                            ET.S12 = TrialET.S12;
                            ET.S44 = TrialET.S44;

                            Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                        }
                    }
                }
            }

            return Converged;
        }

        #endregion

        #region DeWitt

        public static bool FitElasticityTensorDeWittCubicStiffness(Stress.Microsopic.ElasticityTensors ET, bool classicCalculation)
        {
            bool Converged = false;
            double Lambda = LMA._lambda;

            Stress.Microsopic.ElasticityTensors TrialET = ET.Clone() as Stress.Microsopic.ElasticityTensors;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta;

                if (classicCalculation)
                {
                    ParamDelta = ET.ParameterDeltaVektorDeWittCubicStiffnessClassic(Lambda);
                }
                else
                {
                    ParamDelta = ET.ParameterDeltaVektorDeWittCubicStiffnessMacroscopic(Lambda);
                }

                #region Parameter calculations

                TrialET.C11 += ParamDelta[0];
                TrialET.C12 += ParamDelta[1];
                TrialET.C44 += ParamDelta[2];

                #endregion

                double Chi2Trial = Chi2.Chi2ClassicDeWittCubicStiffness(TrialET);
                double Chi2Real = Chi2.Chi2ClassicDeWittCubicStiffness(ET);

                if (Math.Abs(Chi2Trial - Chi2Real) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (Chi2Trial > Chi2Real)
                    {
                        TrialET.C11 = ET.C11;
                        TrialET.C12 = ET.C12;
                        TrialET.C44 = ET.C44;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        ET.C11 = TrialET.C11;
                        ET.C12 = TrialET.C12;
                        ET.C44 = TrialET.C44;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                    else
                    {
                        if (Chi2Real > Chi2Trial)
                        {
                            ET.C11 = TrialET.C11;
                            ET.C12 = TrialET.C12;
                            ET.C44 = TrialET.C44;

                            Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                        }
                    }
                }
            }

            return Converged;
        }

        public static bool FitElasticityTensorDeWittCubicCompliance(Stress.Microsopic.ElasticityTensors ET, bool classicCalculation)
        {
            bool Converged = false;
            double Lambda = LMA._lambda;

            Stress.Microsopic.ElasticityTensors TrialET = ET.Clone() as Stress.Microsopic.ElasticityTensors;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta;

                if (classicCalculation)
                {
                    ParamDelta = ET.ParameterDeltaVektorDeWittCubicComplianceClassic(Lambda);
                }
                else
                {
                    ParamDelta = ET.ParameterDeltaVektorDeWittCubicComplianceMacroscopic(Lambda);
                }

                #region Parameter calculations

                TrialET.S11 += ParamDelta[0];
                TrialET.S12 += ParamDelta[1];
                TrialET.S44 += ParamDelta[2];

                #endregion

                double Chi2Trial = Chi2.Chi2ClassicDeWittCubicCompliance(TrialET);
                double Chi2Real = Chi2.Chi2ClassicDeWittCubicCompliance(ET);

                if (Math.Abs(Chi2Trial - Chi2Real) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (Chi2Trial > Chi2Real)
                    {
                        TrialET.S11 = ET.S11;
                        TrialET.S12 = ET.S12;
                        TrialET.S44 = ET.S44;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        ET.S11 = TrialET.S11;
                        ET.S12 = TrialET.S12;
                        ET.S44 = TrialET.S44;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                    else
                    {
                        if (Chi2Real > Chi2Trial)
                        {
                            ET.S11 = TrialET.S11;
                            ET.S12 = TrialET.S12;
                            ET.S44 = TrialET.S44;

                            Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                        }
                    }
                }
            }

            return Converged;
        }

        #endregion

        #region With Texture

        #region Voigt

        public static bool FitElasticityTensorTextureVoigtCubicIsotrope(Texture.OrientationDistributionFunction ODF, bool classicCalculation)
        {
            bool Converged = false;
            double Lambda = LMA._lambda;

            Texture.OrientationDistributionFunction TrialODF = ODF.Clone() as Texture.OrientationDistributionFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta;

                if (classicCalculation)
                {
                    ODF.SetTextureTensor();
                    ParamDelta = ODF.TextureTensor.ParameterDeltaVektorVoigtCubicIsotropeClassic(Lambda);
                }
                else
                {
                    ODF.SetTextureTensor();
                    ParamDelta = ODF.TextureTensor.ParameterDeltaVektorVoigtCubicIsotropeMacroscopic(Lambda);
                }

                #region Parameter calculations

                TrialODF.BaseTensor.C11 += ParamDelta[0];
                TrialODF.BaseTensor.C12 += ParamDelta[1];

                #endregion

                TrialODF.SetTextureTensor();

                double Chi2Trial = Chi2.Chi2ClassicHillCubic(TrialODF.TextureTensor);
                double Chi2Real = Chi2.Chi2ClassicHillCubic(ODF.TextureTensor);

                if (Math.Abs(Chi2Trial - Chi2Real) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (Chi2Trial > Chi2Real)
                    {
                        TrialODF.BaseTensor.C11 = ODF.BaseTensor.C11;
                        TrialODF.BaseTensor.C12 = ODF.BaseTensor.C12;

                        TrialODF.BaseTensor.CalculateStiffnesses();

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        ODF.BaseTensor.C11 = TrialODF.BaseTensor.C11;
                        ODF.BaseTensor.C12 = TrialODF.BaseTensor.C12;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                    else
                    {
                        if (Chi2Real > Chi2Trial)
                        {
                            ODF.BaseTensor.C11 = TrialODF.BaseTensor.C11;
                            ODF.BaseTensor.C12 = TrialODF.BaseTensor.C12;

                            Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                        }
                    }
                }
            }

            return Converged;
        }

        public static bool FitElasticityTensorTextureVoigtCubic(Texture.OrientationDistributionFunction ODF, bool classicCalculation)
        {
            bool Converged = false;
            double Lambda = 100000;

            Texture.OrientationDistributionFunction TrialODF = ODF.Clone() as Texture.OrientationDistributionFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta;

                if (classicCalculation)
                {
                    ODF.SetTextureTensor();
                    ParamDelta = ODF.TextureTensor.ParameterDeltaVektorVoigtCubicClassic(Lambda);
                }
                else
                {
                    ODF.SetTextureTensor();
                    ParamDelta = ODF.TextureTensor.ParameterDeltaVektorVoigtCubicMacroscopic(Lambda);
                }

                #region Parameter calculations

                TrialODF.BaseTensor.C11 += ParamDelta[0];
                TrialODF.BaseTensor.C12 += ParamDelta[1];
                TrialODF.BaseTensor.C44 += ParamDelta[2];

                #endregion

                TrialODF.SetTextureTensor();

                double Chi2Trial = Chi2.Chi2ClassicHillCubic(TrialODF.TextureTensor);
                double Chi2Real = Chi2.Chi2ClassicHillCubic(ODF.TextureTensor);

                if (Math.Abs(Chi2Trial - Chi2Real) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (Chi2Trial > Chi2Real)
                    {
                        TrialODF.BaseTensor.C11 = ODF.BaseTensor.C11;
                        TrialODF.BaseTensor.C12 = ODF.BaseTensor.C12;
                        TrialODF.BaseTensor.C44 = ODF.BaseTensor.C44;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        ODF.BaseTensor.C11 = TrialODF.BaseTensor.C11;
                        ODF.BaseTensor.C12 = TrialODF.BaseTensor.C12;
                        ODF.BaseTensor.C44 = TrialODF.BaseTensor.C44;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;

                        if (Lambda < 50)
                        {
                            Lambda = 50;
                        }
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                    else
                    {
                        if (Chi2Real > Chi2Trial)
                        {
                            ODF.BaseTensor.C11 = TrialODF.BaseTensor.C11;
                            ODF.BaseTensor.C12 = TrialODF.BaseTensor.C12;
                            ODF.BaseTensor.C44 = TrialODF.BaseTensor.C44;

                            Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                        }
                    }
                }
            }

            return Converged;
        }

        public static bool FitElasticityTensorTextureVoigtType1(Texture.OrientationDistributionFunction ODF, bool classicCalculation)
        {
            bool Converged = false;
            double Lambda = LMA._lambda;

            Texture.OrientationDistributionFunction TrialODF = ODF.Clone() as Texture.OrientationDistributionFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta;

                if (classicCalculation)
                {
                    ODF.SetTextureTensor();
                    ParamDelta = ODF.TextureTensor.ParameterDeltaVektorVoigtType1Classic(Lambda);
                }
                else
                {
                    ODF.SetTextureTensor();
                    ParamDelta = ODF.TextureTensor.ParameterDeltaVektorVoigtType1Macroscopic(Lambda);
                }

                #region Parameter calculations

                TrialODF.BaseTensor.C11 += ParamDelta[0];
                TrialODF.BaseTensor.C33 += ParamDelta[1];
                TrialODF.BaseTensor.C12 += ParamDelta[2];
                TrialODF.BaseTensor.C13 += ParamDelta[3];
                TrialODF.BaseTensor.C44 += ParamDelta[4];

                #endregion

                TrialODF.SetTextureTensor();

                double Chi2Trial = Chi2.Chi2ClassicHillCubic(TrialODF.TextureTensor);
                double Chi2Real = Chi2.Chi2ClassicHillCubic(ODF.TextureTensor);

                if (Math.Abs(Chi2Trial - Chi2Real) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (Chi2Trial > Chi2Real)
                    {
                        TrialODF.BaseTensor.C11 = ODF.BaseTensor.C11;
                        TrialODF.BaseTensor.C33 = ODF.BaseTensor.C33;
                        TrialODF.BaseTensor.C12 = ODF.BaseTensor.C12;
                        TrialODF.BaseTensor.C13 = ODF.BaseTensor.C13;
                        TrialODF.BaseTensor.C44 = ODF.BaseTensor.C44;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        ODF.BaseTensor.C11 = TrialODF.BaseTensor.C11;
                        ODF.BaseTensor.C33 = TrialODF.BaseTensor.C33;
                        ODF.BaseTensor.C12 = TrialODF.BaseTensor.C12;
                        ODF.BaseTensor.C13 = TrialODF.BaseTensor.C13;
                        ODF.BaseTensor.C44 = TrialODF.BaseTensor.C44;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                    else
                    {
                        if (Chi2Real > Chi2Trial)
                        {
                            ODF.BaseTensor.C11 = TrialODF.BaseTensor.C11;
                            ODF.BaseTensor.C33 = TrialODF.BaseTensor.C33;
                            ODF.BaseTensor.C12 = TrialODF.BaseTensor.C12;
                            ODF.BaseTensor.C13 = TrialODF.BaseTensor.C13;
                            ODF.BaseTensor.C44 = TrialODF.BaseTensor.C44;

                            Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                        }
                    }
                }
            }

            return Converged;
        }

        public static bool FitElasticityTensorTextureVoigtType2(Texture.OrientationDistributionFunction ODF, bool classicCalculation)
        {
            bool Converged = false;
            double Lambda = LMA._lambda;

            Texture.OrientationDistributionFunction TrialODF = ODF.Clone() as Texture.OrientationDistributionFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta;

                if (classicCalculation)
                {
                    ODF.SetTextureTensor();
                    ParamDelta = ODF.TextureTensor.ParameterDeltaVektorVoigtType2Classic(Lambda);
                }
                else
                {
                    ODF.SetTextureTensor();
                    ParamDelta = ODF.TextureTensor.ParameterDeltaVektorVoigtType2Macroscopic(Lambda);
                }

                #region Parameter calculations

                TrialODF.BaseTensor.C11 += ParamDelta[0];
                TrialODF.BaseTensor.C33 += ParamDelta[1];
                TrialODF.BaseTensor.C12 += ParamDelta[2];
                TrialODF.BaseTensor.C13 += ParamDelta[3];
                TrialODF.BaseTensor.C44 += ParamDelta[4];
                TrialODF.BaseTensor.C66 += ParamDelta[5];

                #endregion

                TrialODF.SetTextureTensor();

                double Chi2Trial = Chi2.Chi2ClassicHillCubic(TrialODF.TextureTensor);
                double Chi2Real = Chi2.Chi2ClassicHillCubic(ODF.TextureTensor);

                if (Math.Abs(Chi2Trial - Chi2Real) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (Chi2Trial > Chi2Real)
                    {
                        TrialODF.BaseTensor.C11 = ODF.BaseTensor.C11;
                        TrialODF.BaseTensor.C33 = ODF.BaseTensor.C33;
                        TrialODF.BaseTensor.C12 = ODF.BaseTensor.C12;
                        TrialODF.BaseTensor.C13 = ODF.BaseTensor.C13;
                        TrialODF.BaseTensor.C44 = ODF.BaseTensor.C44;
                        TrialODF.BaseTensor.C66 = ODF.BaseTensor.C66;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        ODF.BaseTensor.C11 = TrialODF.BaseTensor.C11;
                        ODF.BaseTensor.C33 = TrialODF.BaseTensor.C33;
                        ODF.BaseTensor.C12 = TrialODF.BaseTensor.C12;
                        ODF.BaseTensor.C13 = TrialODF.BaseTensor.C13;
                        ODF.BaseTensor.C44 = TrialODF.BaseTensor.C44;
                        ODF.BaseTensor.C66 = TrialODF.BaseTensor.C66;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                    else
                    {
                        if (Chi2Real > Chi2Trial)
                        {
                            ODF.BaseTensor.C11 = TrialODF.BaseTensor.C11;
                            ODF.BaseTensor.C33 = TrialODF.BaseTensor.C33;
                            ODF.BaseTensor.C12 = TrialODF.BaseTensor.C12;
                            ODF.BaseTensor.C13 = TrialODF.BaseTensor.C13;
                            ODF.BaseTensor.C44 = TrialODF.BaseTensor.C44;
                            ODF.BaseTensor.C66 = TrialODF.BaseTensor.C66;

                            Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                        }
                    }
                }
            }

            return Converged;
        }

        public static bool FitElasticityTensorTextureVoigtType3(Texture.OrientationDistributionFunction ODF, bool classicCalculation)
        {
            bool Converged = false;
            double Lambda = LMA._lambda;

            Texture.OrientationDistributionFunction TrialODF = ODF.Clone() as Texture.OrientationDistributionFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta;

                if (classicCalculation)
                {
                    ODF.SetTextureTensor();
                    ParamDelta = ODF.TextureTensor.ParameterDeltaVektorVoigtType3Classic(Lambda);
                }
                else
                {
                    ODF.SetTextureTensor();
                    ParamDelta = ODF.TextureTensor.ParameterDeltaVektorVoigtType3Macroscopic(Lambda);
                }

                #region Parameter calculations

                TrialODF.BaseTensor.C11 += ParamDelta[0];
                TrialODF.BaseTensor.C22 += ParamDelta[1];
                TrialODF.BaseTensor.C33 += ParamDelta[2];
                TrialODF.BaseTensor.C12 += ParamDelta[3];
                TrialODF.BaseTensor.C13 += ParamDelta[4];
                TrialODF.BaseTensor.C23 += ParamDelta[5];
                TrialODF.BaseTensor.C44 += ParamDelta[6];
                TrialODF.BaseTensor.C55 += ParamDelta[7];
                TrialODF.BaseTensor.C66 += ParamDelta[8];

                #endregion

                TrialODF.SetTextureTensor();

                double Chi2Trial = Chi2.Chi2ClassicHillCubic(TrialODF.TextureTensor);
                double Chi2Real = Chi2.Chi2ClassicHillCubic(ODF.TextureTensor);

                if (Math.Abs(Chi2Trial - Chi2Real) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (Chi2Trial > Chi2Real)
                    {
                        TrialODF.BaseTensor.C11 = ODF.BaseTensor.C11;
                        TrialODF.BaseTensor.C22 = ODF.BaseTensor.C22;
                        TrialODF.BaseTensor.C33 = ODF.BaseTensor.C33;
                        TrialODF.BaseTensor.C12 = ODF.BaseTensor.C12;
                        TrialODF.BaseTensor.C13 = ODF.BaseTensor.C13;
                        TrialODF.BaseTensor.C23 = ODF.BaseTensor.C23;
                        TrialODF.BaseTensor.C44 = ODF.BaseTensor.C44;
                        TrialODF.BaseTensor.C55 = ODF.BaseTensor.C55;
                        TrialODF.BaseTensor.C66 = ODF.BaseTensor.C66;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        ODF.BaseTensor.C11 = TrialODF.BaseTensor.C11;
                        ODF.BaseTensor.C22 = TrialODF.BaseTensor.C22;
                        ODF.BaseTensor.C33 = TrialODF.BaseTensor.C33;
                        ODF.BaseTensor.C12 = TrialODF.BaseTensor.C12;
                        ODF.BaseTensor.C13 = TrialODF.BaseTensor.C13;
                        ODF.BaseTensor.C23 = TrialODF.BaseTensor.C23;
                        ODF.BaseTensor.C44 = TrialODF.BaseTensor.C44;
                        ODF.BaseTensor.C55 = TrialODF.BaseTensor.C55;
                        ODF.BaseTensor.C66 = TrialODF.BaseTensor.C66;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                    else
                    {
                        if (Chi2Real > Chi2Trial)
                        {
                            ODF.BaseTensor.C11 = TrialODF.BaseTensor.C11;
                            ODF.BaseTensor.C22 = TrialODF.BaseTensor.C22;
                            ODF.BaseTensor.C33 = TrialODF.BaseTensor.C33;
                            ODF.BaseTensor.C12 = TrialODF.BaseTensor.C12;
                            ODF.BaseTensor.C13 = TrialODF.BaseTensor.C13;
                            ODF.BaseTensor.C23 = TrialODF.BaseTensor.C23;
                            ODF.BaseTensor.C44 = TrialODF.BaseTensor.C44;
                            ODF.BaseTensor.C55 = TrialODF.BaseTensor.C55;
                            ODF.BaseTensor.C66 = TrialODF.BaseTensor.C66;

                            Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                        }
                    }
                }
            }

            return Converged;
        }

        #endregion

        #region Reuss

        public static bool FitElasticityTensorTextureReussCubic(Texture.OrientationDistributionFunction ODF, bool classicCalculation)
        {
            bool Converged = false;
            double Lambda = LMA._lambda;

            Texture.OrientationDistributionFunction TrialODF = ODF.Clone() as Texture.OrientationDistributionFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta;

                if (classicCalculation)
                {
                    ODF.SetTextureTensor();
                    ParamDelta = ODF.TextureTensor.ParameterDeltaVektorReussCubicClassic(Lambda);
                }
                else
                {
                    ODF.SetTextureTensor();
                    ParamDelta = ODF.TextureTensor.ParameterDeltaVektorReussCubicMacroscopic(Lambda);
                }

                #region Parameter calculations

                TrialODF.BaseTensor.S11 += ParamDelta[0];
                TrialODF.BaseTensor.S12 += ParamDelta[1];
                TrialODF.BaseTensor.S44 += ParamDelta[2];

                #endregion

                TrialODF.BaseTensor.CalculateStiffnesses();

                TrialODF.SetTextureTensor();

                double Chi2Trial = Chi2.Chi2ClassicHillCubic(TrialODF.TextureTensor);
                double Chi2Real = Chi2.Chi2ClassicHillCubic(ODF.TextureTensor);

                if (Math.Abs(Chi2Trial - Chi2Real) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (Chi2Trial > Chi2Real)
                    {
                        TrialODF.BaseTensor.S11 = ODF.BaseTensor.S11;
                        TrialODF.BaseTensor.S12 = ODF.BaseTensor.S12;
                        TrialODF.BaseTensor.S44 = ODF.BaseTensor.S44;

                        TrialODF.BaseTensor.CalculateStiffnesses();

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        ODF.BaseTensor.S11 = TrialODF.BaseTensor.S11;
                        ODF.BaseTensor.S12 = TrialODF.BaseTensor.S12;
                        ODF.BaseTensor.S44 = TrialODF.BaseTensor.S44;

                        ODF.BaseTensor.CalculateStiffnesses();

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                    else
                    {
                        if (Chi2Real > Chi2Trial)
                        {
                            ODF.BaseTensor.S11 = TrialODF.BaseTensor.S11;
                            ODF.BaseTensor.S12 = TrialODF.BaseTensor.S12;
                            ODF.BaseTensor.S44 = TrialODF.BaseTensor.S44;

                            ODF.BaseTensor.CalculateStiffnesses();

                            Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                        }
                    }
                }
            }

            return Converged;
        }

        public static bool FitElasticityTensorTextureReussHexagonal(Texture.OrientationDistributionFunction ODF, bool classicCalculation)
        {
            bool Converged = false;
            double Lambda = LMA._lambda;

            Texture.OrientationDistributionFunction TrialODF = ODF.Clone() as Texture.OrientationDistributionFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta;

                if (classicCalculation)
                {
                    ODF.SetTextureTensor();
                    ParamDelta = ODF.TextureTensor.ParameterDeltaVektorReussHexagonalClassic(Lambda);
                }
                else
                {
                    ODF.SetTextureTensor();
                    ParamDelta = ODF.TextureTensor.ParameterDeltaVektorReussHexagonalMacroscopic(Lambda);
                }

                #region Parameter calculations

                TrialODF.BaseTensor.S11 += ParamDelta[0];
                TrialODF.BaseTensor.S33 += ParamDelta[1];
                TrialODF.BaseTensor.S12 += ParamDelta[2];
                TrialODF.BaseTensor.S13 += ParamDelta[3];
                TrialODF.BaseTensor.S44 += ParamDelta[4];

                #endregion

                TrialODF.BaseTensor.CalculateStiffnesses();

                TrialODF.SetTextureTensor();

                double Chi2Trial = Chi2.Chi2ClassicHillCubic(TrialODF.TextureTensor);
                double Chi2Real = Chi2.Chi2ClassicHillCubic(ODF.TextureTensor);

                if (Math.Abs(Chi2Trial - Chi2Real) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (Chi2Trial > Chi2Real)
                    {
                        TrialODF.BaseTensor.S11 = ODF.BaseTensor.S11;
                        TrialODF.BaseTensor.S33 = ODF.BaseTensor.S33;
                        TrialODF.BaseTensor.S12 = ODF.BaseTensor.S12;
                        TrialODF.BaseTensor.S13 = ODF.BaseTensor.S13;
                        TrialODF.BaseTensor.S44 = ODF.BaseTensor.S44;

                        TrialODF.BaseTensor.CalculateStiffnesses();

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        ODF.BaseTensor.S11 = TrialODF.BaseTensor.S11;
                        ODF.BaseTensor.S33 = TrialODF.BaseTensor.S33;
                        ODF.BaseTensor.S12 = TrialODF.BaseTensor.S12;
                        ODF.BaseTensor.S13 = TrialODF.BaseTensor.S13;
                        ODF.BaseTensor.S44 = TrialODF.BaseTensor.S44;

                        ODF.BaseTensor.CalculateStiffnesses();

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                    else
                    {
                        if (Chi2Real > Chi2Trial)
                        {
                            ODF.BaseTensor.S11 = TrialODF.BaseTensor.S11;
                            ODF.BaseTensor.S33 = TrialODF.BaseTensor.S33;
                            ODF.BaseTensor.S12 = TrialODF.BaseTensor.S12;
                            ODF.BaseTensor.S13 = TrialODF.BaseTensor.S13;
                            ODF.BaseTensor.S44 = TrialODF.BaseTensor.S44;

                            ODF.BaseTensor.CalculateStiffnesses();

                            Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                        }
                    }
                }
            }

            return Converged;
        }

        #endregion

        #region Hill

        public static bool FitElasticityTensorTextureHillCubic(Texture.OrientationDistributionFunction ODF, bool classicCalculation)
        {
            bool Converged = false;
            double Lambda = LMA._lambda;

            Texture.OrientationDistributionFunction TrialODF = ODF.Clone() as Texture.OrientationDistributionFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta;

                if (classicCalculation)
                {
                    ODF.SetTextureTensor();
                    ParamDelta = ODF.TextureTensor.ParameterDeltaVektorHillCubicClassic(Lambda);
                }
                else
                {
                    ODF.SetTextureTensor();
                    ParamDelta = ODF.TextureTensor.ParameterDeltaVektorHillCubicMacroscopic(Lambda);
                }

                #region Parameter calculations

                TrialODF.BaseTensor.S11 += ParamDelta[0];
                TrialODF.BaseTensor.S12 += ParamDelta[1];
                TrialODF.BaseTensor.S44 += ParamDelta[2];

                TrialODF.BaseTensor.CalculateStiffnesses();

                #endregion
                TrialODF.SetTextureTensor();

                double Chi2Trial = Chi2.Chi2ClassicHillCubic(TrialODF.TextureTensor);
                double Chi2Real = Chi2.Chi2ClassicHillCubic(ODF.TextureTensor);

                if (Math.Abs(Chi2Trial - Chi2Real) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (Chi2Trial > Chi2Real)
                    {
                        TrialODF.BaseTensor.S11 = ODF.BaseTensor.S11;
                        TrialODF.BaseTensor.S12 = ODF.BaseTensor.S12;
                        TrialODF.BaseTensor.S44 = ODF.BaseTensor.S44;

                        TrialODF.BaseTensor.CalculateStiffnesses();

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        ODF.BaseTensor.S11 = TrialODF.BaseTensor.S11;
                        ODF.BaseTensor.S12 = TrialODF.BaseTensor.S12;
                        ODF.BaseTensor.S44 = TrialODF.BaseTensor.S44;

                        ODF.BaseTensor.CalculateStiffnesses();

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                    else
                    {
                        if (Chi2Real > Chi2Trial)
                        {
                            ODF.BaseTensor.S11 = TrialODF.BaseTensor.S11;
                            ODF.BaseTensor.S12 = TrialODF.BaseTensor.S12;
                            ODF.BaseTensor.S44 = TrialODF.BaseTensor.S44;

                            ODF.BaseTensor.CalculateStiffnesses();

                            Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                        }
                    }
                }
            }

            return Converged;
        }

        public static bool FitElasticityTensorTextureHillHexagonal(Texture.OrientationDistributionFunction ODF, bool classicCalculation)
        {
            bool Converged = false;
            double Lambda = LMA._lambda;

            Texture.OrientationDistributionFunction TrialODF = ODF.Clone() as Texture.OrientationDistributionFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta;

                if (classicCalculation)
                {
                    ODF.SetTextureTensor();
                    ParamDelta = ODF.TextureTensor.ParameterDeltaVektorHillHexagonalClassic(Lambda);
                }
                else
                {
                    ODF.SetTextureTensor();
                    ParamDelta = ODF.TextureTensor.ParameterDeltaVektorHillHexagonalMacroscopic(Lambda);
                }

                #region Parameter calculations

                TrialODF.BaseTensor.S11 += ParamDelta[0];
                TrialODF.BaseTensor.S33 += ParamDelta[1];
                TrialODF.BaseTensor.S12 += ParamDelta[2];
                TrialODF.BaseTensor.S13 += ParamDelta[3];
                TrialODF.BaseTensor.S44 += ParamDelta[4];

                #endregion

                TrialODF.BaseTensor.CalculateStiffnesses();

                TrialODF.SetTextureTensor();

                double Chi2Trial = Chi2.Chi2ClassicHillCubic(TrialODF.TextureTensor);
                double Chi2Real = Chi2.Chi2ClassicHillCubic(ODF.TextureTensor);

                if (Math.Abs(Chi2Trial - Chi2Real) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (Chi2Trial > Chi2Real)
                    {
                        TrialODF.BaseTensor.S11 = ODF.BaseTensor.S11;
                        TrialODF.BaseTensor.S33 = ODF.BaseTensor.S33;
                        TrialODF.BaseTensor.S12 = ODF.BaseTensor.S12;
                        TrialODF.BaseTensor.S13 = ODF.BaseTensor.S13;
                        TrialODF.BaseTensor.S44 = ODF.BaseTensor.S44;

                        TrialODF.BaseTensor.CalculateStiffnesses();

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        ODF.BaseTensor.S11 = TrialODF.BaseTensor.S11;
                        ODF.BaseTensor.S33 = TrialODF.BaseTensor.S33;
                        ODF.BaseTensor.S12 = TrialODF.BaseTensor.S12;
                        ODF.BaseTensor.S13 = TrialODF.BaseTensor.S13;
                        ODF.BaseTensor.S44 = TrialODF.BaseTensor.S44;

                        ODF.BaseTensor.CalculateStiffnesses();

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                    else
                    {
                        if (Chi2Real > Chi2Trial)
                        {
                            ODF.BaseTensor.S11 = TrialODF.BaseTensor.S11;
                            ODF.BaseTensor.S33 = TrialODF.BaseTensor.S33;
                            ODF.BaseTensor.S12 = TrialODF.BaseTensor.S12;
                            ODF.BaseTensor.S13 = TrialODF.BaseTensor.S13;
                            ODF.BaseTensor.S44 = TrialODF.BaseTensor.S44;

                            ODF.BaseTensor.CalculateStiffnesses();

                            Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                        }
                    }
                }
            }

            return Converged;
        }

        #endregion

        #region Geometric Hill

        public static bool FitElasticityTensorTextureGeometricHillCubic(Texture.OrientationDistributionFunction ODF, bool classicCalculation)
        {
            bool Converged = false;
            double Lambda = LMA._lambda;

            Texture.OrientationDistributionFunction TrialODF = ODF.Clone() as Texture.OrientationDistributionFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta;

                if (classicCalculation)
                {
                    ODF.SetTextureTensor();
                    ParamDelta = ODF.TextureTensor.ParameterDeltaVektorGeometricHillCubicClassic(Lambda);
                }
                else
                {
                    ODF.SetTextureTensor();
                    ParamDelta = ODF.TextureTensor.ParameterDeltaVektorGeometricHillCubicMacroscopic(Lambda);
                }

                #region Parameter calculations

                TrialODF.BaseTensor.S11 += ParamDelta[0];
                TrialODF.BaseTensor.S12 += ParamDelta[1];
                TrialODF.BaseTensor.S44 += ParamDelta[2];

                #endregion

                TrialODF.BaseTensor.CalculateStiffnesses();

                TrialODF.SetTextureTensor();

                double Chi2Trial = Chi2.Chi2ClassicHillCubic(TrialODF.TextureTensor);
                double Chi2Real = Chi2.Chi2ClassicHillCubic(ODF.TextureTensor);

                if (Math.Abs(Chi2Trial - Chi2Real) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (Chi2Trial > Chi2Real)
                    {
                        TrialODF.BaseTensor.S11 = ODF.BaseTensor.S11;
                        TrialODF.BaseTensor.S12 = ODF.BaseTensor.S12;
                        TrialODF.BaseTensor.S44 = ODF.BaseTensor.S44;

                        TrialODF.BaseTensor.CalculateStiffnesses();

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        ODF.BaseTensor.S11 = TrialODF.BaseTensor.S11;
                        ODF.BaseTensor.S12 = TrialODF.BaseTensor.S12;
                        ODF.BaseTensor.S44 = TrialODF.BaseTensor.S44;

                        ODF.BaseTensor.CalculateStiffnesses();

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                    else
                    {
                        if (Chi2Real > Chi2Trial)
                        {
                            ODF.BaseTensor.S11 = TrialODF.BaseTensor.S11;
                            ODF.BaseTensor.S12 = TrialODF.BaseTensor.S12;
                            ODF.BaseTensor.S44 = TrialODF.BaseTensor.S44;

                            ODF.BaseTensor.CalculateStiffnesses();

                            Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                        }
                    }
                }
            }

            return Converged;
        }

        public static bool FitElasticityTensorTextureGeometricHillHexagonal(Texture.OrientationDistributionFunction ODF, bool classicCalculation)
        {
            bool Converged = false;
            double Lambda = LMA._lambda;

            Texture.OrientationDistributionFunction TrialODF = ODF.Clone() as Texture.OrientationDistributionFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta;

                if (classicCalculation)
                {
                    ODF.SetTextureTensor();
                    ParamDelta = ODF.TextureTensor.ParameterDeltaVektorGeometricHillHexagonalClassic(Lambda);
                }
                else
                {
                    ODF.SetTextureTensor();
                    ParamDelta = ODF.TextureTensor.ParameterDeltaVektorGeometricHillHexagonalMacroscopic(Lambda);
                }

                #region Parameter calculations

                TrialODF.BaseTensor.S11 += ParamDelta[0];
                TrialODF.BaseTensor.S33 += ParamDelta[1];
                TrialODF.BaseTensor.S12 += ParamDelta[2];
                TrialODF.BaseTensor.S13 += ParamDelta[3];
                TrialODF.BaseTensor.S44 += ParamDelta[4];

                #endregion

                TrialODF.BaseTensor.CalculateStiffnesses();

                TrialODF.SetTextureTensor();

                double Chi2Trial = Chi2.Chi2ClassicHillCubic(TrialODF.TextureTensor);
                double Chi2Real = Chi2.Chi2ClassicHillCubic(ODF.TextureTensor);

                if (Math.Abs(Chi2Trial - Chi2Real) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (Chi2Trial > Chi2Real)
                    {
                        TrialODF.BaseTensor.S11 = ODF.BaseTensor.S11;
                        TrialODF.BaseTensor.S33 = ODF.BaseTensor.S33;
                        TrialODF.BaseTensor.S12 = ODF.BaseTensor.S12;
                        TrialODF.BaseTensor.S13 = ODF.BaseTensor.S13;
                        TrialODF.BaseTensor.S44 = ODF.BaseTensor.S44;

                        TrialODF.BaseTensor.CalculateStiffnesses();

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        ODF.BaseTensor.S11 = TrialODF.BaseTensor.S11;
                        ODF.BaseTensor.S33 = TrialODF.BaseTensor.S33;
                        ODF.BaseTensor.S12 = TrialODF.BaseTensor.S12;
                        ODF.BaseTensor.S13 = TrialODF.BaseTensor.S13;
                        ODF.BaseTensor.S44 = TrialODF.BaseTensor.S44;

                        ODF.BaseTensor.CalculateStiffnesses();

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                    else
                    {
                        if (Chi2Real > Chi2Trial)
                        {
                            ODF.BaseTensor.S11 = TrialODF.BaseTensor.S11;
                            ODF.BaseTensor.S33 = TrialODF.BaseTensor.S33;
                            ODF.BaseTensor.S12 = TrialODF.BaseTensor.S12;
                            ODF.BaseTensor.S13 = TrialODF.BaseTensor.S13;
                            ODF.BaseTensor.S44 = TrialODF.BaseTensor.S44;

                            ODF.BaseTensor.CalculateStiffnesses();

                            Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                        }
                    }
                }
            }

            return Converged;
        }

        #endregion

        #region Kroener

        public static bool FitElasticityTensorTextureKroenerCubicStiffness(Texture.OrientationDistributionFunction ODF, bool classicCalculation)
        {
            bool Converged = false;
            double Lambda = LMA._lambda;

            Texture.OrientationDistributionFunction TrialODF = ODF.Clone() as Texture.OrientationDistributionFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta;

                if (classicCalculation)
                {
                    ODF.SetTextureTensor();
                    ParamDelta = ODF.TextureTensor.ParameterDeltaVektorKroenerCubicStiffnessClassic(Lambda);
                }
                else
                {
                    ODF.SetTextureTensor();
                    ParamDelta = ODF.TextureTensor.ParameterDeltaVektorKroenerCubicStiffnessMacroscopic(Lambda);
                }

                #region Parameter calculations

                TrialODF.BaseTensor.C11 += ParamDelta[0];
                TrialODF.BaseTensor.C12 += ParamDelta[1];
                TrialODF.BaseTensor.C44 += ParamDelta[2];

                #endregion

                TrialODF.SetTextureTensor();

                double Chi2Trial = Chi2.Chi2ClassicHillCubic(TrialODF.TextureTensor);
                double Chi2Real = Chi2.Chi2ClassicHillCubic(ODF.TextureTensor);

                if (Math.Abs(Chi2Trial - Chi2Real) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (Chi2Trial > Chi2Real)
                    {
                        TrialODF.BaseTensor.C11 = ODF.BaseTensor.C11;
                        TrialODF.BaseTensor.C12 = ODF.BaseTensor.C12;
                        TrialODF.BaseTensor.C44 = ODF.BaseTensor.C44;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        ODF.BaseTensor.C11 = TrialODF.BaseTensor.C11;
                        ODF.BaseTensor.C12 = TrialODF.BaseTensor.C12;
                        ODF.BaseTensor.C44 = TrialODF.BaseTensor.C44;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                    else
                    {
                        if (Chi2Real > Chi2Trial)
                        {
                            ODF.BaseTensor.C11 = TrialODF.BaseTensor.C11;
                            ODF.BaseTensor.C12 = TrialODF.BaseTensor.C12;
                            ODF.BaseTensor.C44 = TrialODF.BaseTensor.C44;

                            Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                        }
                    }
                }
            }

            return Converged;
        }

        public static bool FitElasticityTensorTextureKroenerCubicCompliance(Texture.OrientationDistributionFunction ODF, bool classicCalculation)
        {
            bool Converged = false;
            double Lambda = LMA._lambda;

            Texture.OrientationDistributionFunction TrialODF = ODF.Clone() as Texture.OrientationDistributionFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta;

                if (classicCalculation)
                {
                    ODF.SetTextureTensor();
                    ParamDelta = ODF.TextureTensor.ParameterDeltaVektorKroenerCubicComplianceClassic(Lambda);
                }
                else
                {
                    ODF.SetTextureTensor();
                    ParamDelta = ODF.TextureTensor.ParameterDeltaVektorKroenerCubicComplianceMacroscopic(Lambda);
                }

                #region Parameter calculations

                TrialODF.BaseTensor.S11 += ParamDelta[0];
                TrialODF.BaseTensor.S12 += ParamDelta[1];
                TrialODF.BaseTensor.S44 += ParamDelta[2];

                #endregion

                TrialODF.BaseTensor.CalculateStiffnesses();

                TrialODF.SetTextureTensor();

                double Chi2Trial = Chi2.Chi2ClassicHillCubic(TrialODF.TextureTensor);
                double Chi2Real = Chi2.Chi2ClassicHillCubic(ODF.TextureTensor);

                if (Math.Abs(Chi2Trial - Chi2Real) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (Chi2Trial > Chi2Real)
                    {
                        TrialODF.BaseTensor.S11 = ODF.BaseTensor.S11;
                        TrialODF.BaseTensor.S12 = ODF.BaseTensor.S12;
                        TrialODF.BaseTensor.S44 = ODF.BaseTensor.S44;

                        TrialODF.BaseTensor.CalculateStiffnesses();

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        ODF.BaseTensor.S11 = TrialODF.BaseTensor.S11;
                        ODF.BaseTensor.S12 = TrialODF.BaseTensor.S12;
                        ODF.BaseTensor.S44 = TrialODF.BaseTensor.S44;

                        ODF.BaseTensor.CalculateStiffnesses();

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                    else
                    {
                        if (Chi2Real > Chi2Trial)
                        {
                            ODF.BaseTensor.S11 = TrialODF.BaseTensor.S11;
                            ODF.BaseTensor.S12 = TrialODF.BaseTensor.S12;
                            ODF.BaseTensor.S44 = TrialODF.BaseTensor.S44;

                            ODF.BaseTensor.CalculateStiffnesses();

                            Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                        }
                    }
                }
            }

            return Converged;
        }

        #endregion

        #region DeWitt

        public static bool FitElasticityTensorTextureDeWittCubicStiffness(Texture.OrientationDistributionFunction ODF, bool classicCalculation)
        {
            bool Converged = false;
            double Lambda = LMA._lambda;

            Texture.OrientationDistributionFunction TrialODF = ODF.Clone() as Texture.OrientationDistributionFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta;

                if (classicCalculation)
                {
                    ODF.SetTextureTensor();
                    ParamDelta = ODF.TextureTensor.ParameterDeltaVektorDeWittCubicStiffnessClassic(Lambda);
                }
                else
                {
                    ODF.SetTextureTensor();
                    ParamDelta = ODF.TextureTensor.ParameterDeltaVektorDeWittCubicStiffnessMacroscopic(Lambda);
                }

                #region Parameter calculations

                TrialODF.BaseTensor.C11 += ParamDelta[0];
                TrialODF.BaseTensor.C12 += ParamDelta[1];
                TrialODF.BaseTensor.C44 += ParamDelta[2];

                #endregion

                TrialODF.SetTextureTensor();

                double Chi2Trial = Chi2.Chi2ClassicHillCubic(TrialODF.TextureTensor);
                double Chi2Real = Chi2.Chi2ClassicHillCubic(ODF.TextureTensor);

                if (Math.Abs(Chi2Trial - Chi2Real) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (Chi2Trial > Chi2Real)
                    {
                        TrialODF.BaseTensor.C11 = ODF.BaseTensor.C11;
                        TrialODF.BaseTensor.C12 = ODF.BaseTensor.C12;
                        TrialODF.BaseTensor.C44 = ODF.BaseTensor.C44;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        ODF.BaseTensor.C11 = TrialODF.BaseTensor.C11;
                        ODF.BaseTensor.C12 = TrialODF.BaseTensor.C12;
                        ODF.BaseTensor.C44 = TrialODF.BaseTensor.C44;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                    else
                    {
                        if (Chi2Real > Chi2Trial)
                        {
                            ODF.BaseTensor.C11 = TrialODF.BaseTensor.C11;
                            ODF.BaseTensor.C12 = TrialODF.BaseTensor.C12;
                            ODF.BaseTensor.C44 = TrialODF.BaseTensor.C44;

                            Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                        }
                    }
                }
            }

            return Converged;
        }

        public static bool FitElasticityTensorTextureDeWittCubicCompliance(Texture.OrientationDistributionFunction ODF, bool classicCalculation)
        {
            bool Converged = false;
            double Lambda = LMA._lambda;

            Texture.OrientationDistributionFunction TrialODF = ODF.Clone() as Texture.OrientationDistributionFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta;

                if (classicCalculation)
                {
                    ODF.SetTextureTensor();
                    ParamDelta = ODF.TextureTensor.ParameterDeltaVektorDeWittCubicComplianceClassic(Lambda);
                }
                else
                {
                    ODF.SetTextureTensor();
                    ParamDelta = ODF.TextureTensor.ParameterDeltaVektorDeWittCubicComplianceMacroscopic(Lambda);
                }

                #region Parameter calculations

                TrialODF.BaseTensor.S11 += ParamDelta[0];
                TrialODF.BaseTensor.S12 += ParamDelta[1];
                TrialODF.BaseTensor.S44 += ParamDelta[2];

                #endregion

                TrialODF.BaseTensor.CalculateStiffnesses();

                TrialODF.SetTextureTensor();

                double Chi2Trial = Chi2.Chi2ClassicHillCubic(TrialODF.TextureTensor);
                double Chi2Real = Chi2.Chi2ClassicHillCubic(ODF.TextureTensor);

                if (Math.Abs(Chi2Trial - Chi2Real) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (Chi2Trial > Chi2Real)
                    {
                        TrialODF.BaseTensor.S11 = ODF.BaseTensor.S11;
                        TrialODF.BaseTensor.S12 = ODF.BaseTensor.S12;
                        TrialODF.BaseTensor.S44 = ODF.BaseTensor.S44;

                        TrialODF.BaseTensor.CalculateStiffnesses();

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        ODF.BaseTensor.S11 = TrialODF.BaseTensor.S11;
                        ODF.BaseTensor.S12 = TrialODF.BaseTensor.S12;
                        ODF.BaseTensor.S44 = TrialODF.BaseTensor.S44;

                        ODF.BaseTensor.CalculateStiffnesses();

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                    else
                    {
                        if (Chi2Real > Chi2Trial)
                        {
                            ODF.BaseTensor.S11 = TrialODF.BaseTensor.S11;
                            ODF.BaseTensor.S12 = TrialODF.BaseTensor.S12;
                            ODF.BaseTensor.S44 = TrialODF.BaseTensor.S44;

                            ODF.BaseTensor.CalculateStiffnesses();

                            Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                        }
                    }
                }
            }

            return Converged;
        }

        #endregion

        #endregion

        #endregion

        #region Peak Fit Routines

        private static bool FitAngleSigmaIntensityLorentzRatioBackground(Analysis.Peaks.Functions.PeakFunction PF, double lambda)
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakFunction TrialPeak = PF.Clone() as Analysis.Peaks.Functions.PeakFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeak.ParameterDeltaVektorSigmaAngleIntensityLorentzRatioBackground(Lambda);
                
                #region Parameter calculations

                TrialPeak.SetSigmaWithConstraints = ParamDelta[0];
                TrialPeak.SetAngleWithConstraints = ParamDelta[1];
                TrialPeak.Intensity += ParamDelta[2];
                TrialPeak.SetLorentzRatioWithConstraints = ParamDelta[3];
                TrialPeak.ConstantBackground += ParamDelta[4];
                TrialPeak.CenterBackground += ParamDelta[5];
                TrialPeak.AclivityBackground += ParamDelta[6];

                #endregion
                
                if (Math.Abs(TrialPeak.Chi2Function - PF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeak.Chi2Function > PF.Chi2Function)
                    {
                        TrialPeak.Sigma = PF.Sigma;
                        TrialPeak.Angle = PF.Angle;
                        TrialPeak.Intensity = PF.Intensity;
                        TrialPeak.ConstantBackground = PF.ConstantBackground;
                        TrialPeak.CenterBackground = PF.CenterBackground;
                        TrialPeak.AclivityBackground = PF.AclivityBackground;
                        TrialPeak.LorentzRatio = PF.LorentzRatio;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        PF.Sigma = TrialPeak.Sigma;
                        PF.Angle = TrialPeak.Angle;
                        PF.Intensity = TrialPeak.Intensity;
                        PF.ConstantBackground = TrialPeak.ConstantBackground;
                        PF.CenterBackground = TrialPeak.CenterBackground;
                        PF.AclivityBackground = TrialPeak.AclivityBackground;
                        PF.LorentzRatio = TrialPeak.LorentzRatio;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }

        private static bool FitAngleSigmaLorentzRatioBackground(Analysis.Peaks.Functions.PeakFunction PF, double lambda)
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakFunction TrialPeak = PF.Clone() as Analysis.Peaks.Functions.PeakFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeak.ParameterDeltaVektorSigmaAngleLorentzRatioBackground(Lambda);
                
                #region Parameter calculations

                TrialPeak.SetSigmaWithConstraints = ParamDelta[0];
                TrialPeak.SetAngleWithConstraints = ParamDelta[1];
                TrialPeak.LorentzRatio += ParamDelta[2];
                TrialPeak.ConstantBackground += ParamDelta[3];
                TrialPeak.CenterBackground += ParamDelta[4];
                TrialPeak.AclivityBackground += ParamDelta[5];

                #endregion

                if (Math.Abs(TrialPeak.Chi2Function - PF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeak.Chi2Function > PF.Chi2Function)
                    {
                        TrialPeak.Sigma = PF.Sigma;
                        TrialPeak.Angle = PF.Angle;
                        TrialPeak.ConstantBackground = PF.ConstantBackground;
                        TrialPeak.CenterBackground = PF.CenterBackground;
                        TrialPeak.AclivityBackground = PF.AclivityBackground;
                        TrialPeak.LorentzRatio = PF.LorentzRatio;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        PF.Sigma = TrialPeak.Sigma;
                        PF.Angle = TrialPeak.Angle;
                        PF.ConstantBackground = TrialPeak.ConstantBackground;
                        PF.CenterBackground = TrialPeak.CenterBackground;
                        PF.AclivityBackground = TrialPeak.AclivityBackground;
                        PF.LorentzRatio = TrialPeak.LorentzRatio;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }

        private static bool FitAngleSigmaIntensityBackground(Analysis.Peaks.Functions.PeakFunction PF, double lambda)
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakFunction TrialPeak = PF.Clone() as Analysis.Peaks.Functions.PeakFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeak.ParameterDeltaVektorSigmaAngleIntensityBackground(Lambda);
                
                #region Parameter calculations

                TrialPeak.SetSigmaWithConstraints = ParamDelta[0];
                TrialPeak.SetAngleWithConstraints = ParamDelta[1];
                TrialPeak.Intensity += ParamDelta[2];
                TrialPeak.ConstantBackground += ParamDelta[3];
                TrialPeak.CenterBackground += ParamDelta[4];
                TrialPeak.AclivityBackground += ParamDelta[5];

                #endregion

                if (Math.Abs(TrialPeak.Chi2Function - PF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeak.Chi2Function > PF.Chi2Function)
                    {
                        TrialPeak.Sigma = PF.Sigma;
                        TrialPeak.Angle = PF.Angle;
                        TrialPeak.Intensity = PF.Intensity;
                        TrialPeak.ConstantBackground = PF.ConstantBackground;
                        TrialPeak.CenterBackground = PF.CenterBackground;
                        TrialPeak.AclivityBackground = PF.AclivityBackground;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        PF.Sigma = TrialPeak.Sigma;
                        PF.Angle = TrialPeak.Angle;
                        PF.Intensity = TrialPeak.Intensity;
                        PF.ConstantBackground = TrialPeak.ConstantBackground;
                        PF.CenterBackground = TrialPeak.CenterBackground;
                        PF.AclivityBackground = TrialPeak.AclivityBackground;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }

        private static bool FitAngleSigmaIntensityLorentzRatio(Analysis.Peaks.Functions.PeakFunction PF, double lambda)
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakFunction TrialPeak = PF.Clone() as Analysis.Peaks.Functions.PeakFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeak.ParameterDeltaVektorSigmaAngleIntensityLorentzRatio(Lambda);

                int InLimits = 0;

                #region Parameter calculations

                TrialPeak.SetSigmaWithConstraints = ParamDelta[0];
                TrialPeak.SetAngleWithConstraints = ParamDelta[1];
                TrialPeak.Intensity += ParamDelta[2];
                TrialPeak.LorentzRatio += ParamDelta[3];

                #endregion

                if (InLimits == 2)
                {

                }

                if (Math.Abs(TrialPeak.Chi2Function - PF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeak.Chi2Function > PF.Chi2Function)
                    {
                        TrialPeak.Sigma = PF.Sigma;
                        TrialPeak.Angle = PF.Angle;
                        TrialPeak.LorentzRatio = PF.LorentzRatio;
                        TrialPeak.Intensity = PF.Intensity;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        PF.Sigma = TrialPeak.Sigma;
                        PF.Angle = TrialPeak.Angle;
                        PF.LorentzRatio = TrialPeak.LorentzRatio;
                        PF.Intensity = TrialPeak.Intensity;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }

        private static bool FitSigmaLorentzRatioIntensityBackground(Analysis.Peaks.Functions.PeakFunction PF, double lambda)
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakFunction TrialPeak = PF.Clone() as Analysis.Peaks.Functions.PeakFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeak.ParameterDeltaVektorSigmaLorentzRatioIntensityBackground(Lambda);

                #region Parameter calculations

                TrialPeak.SetSigmaWithConstraints = ParamDelta[0];
                TrialPeak.LorentzRatio += ParamDelta[1];
                TrialPeak.Intensity += ParamDelta[2];
                TrialPeak.ConstantBackground += ParamDelta[3];
                TrialPeak.CenterBackground += ParamDelta[4];
                TrialPeak.AclivityBackground += ParamDelta[5];

                #endregion

                if (Math.Abs(TrialPeak.Chi2Function - PF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeak.Chi2Function > PF.Chi2Function)
                    {
                        TrialPeak.Sigma = PF.Sigma;
                        TrialPeak.ConstantBackground = PF.ConstantBackground;
                        TrialPeak.CenterBackground = PF.CenterBackground;
                        TrialPeak.AclivityBackground = PF.AclivityBackground;
                        TrialPeak.LorentzRatio = PF.LorentzRatio;
                        TrialPeak.Intensity = PF.Intensity;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        PF.Sigma = TrialPeak.Sigma;
                        PF.ConstantBackground = TrialPeak.ConstantBackground;
                        PF.CenterBackground = TrialPeak.CenterBackground;
                        PF.AclivityBackground = TrialPeak.AclivityBackground;
                        PF.LorentzRatio = TrialPeak.LorentzRatio;
                        PF.Intensity = TrialPeak.Intensity;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }

        private static bool FitAngleLorentzRatioIntensityBackground(Analysis.Peaks.Functions.PeakFunction PF, double lambda)
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakFunction TrialPeak = PF.Clone() as Analysis.Peaks.Functions.PeakFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeak.ParameterDeltaVektorAngleLorentzRatioIntensityBackground(Lambda);

                #region Parameter calculations

                TrialPeak.SetAngleWithConstraints = ParamDelta[0];
                TrialPeak.LorentzRatio += ParamDelta[1];
                TrialPeak.Intensity += ParamDelta[2];
                TrialPeak.ConstantBackground += ParamDelta[3];
                TrialPeak.CenterBackground += ParamDelta[4];
                TrialPeak.AclivityBackground += ParamDelta[5];

                #endregion

                if (Math.Abs(TrialPeak.Chi2Function - PF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeak.Chi2Function > PF.Chi2Function)
                    {
                        TrialPeak.Angle = PF.Angle;
                        TrialPeak.ConstantBackground = PF.ConstantBackground;
                        TrialPeak.CenterBackground = PF.CenterBackground;
                        TrialPeak.AclivityBackground = PF.AclivityBackground;
                        TrialPeak.LorentzRatio = PF.LorentzRatio;
                        TrialPeak.Intensity = PF.Intensity;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        PF.Angle = TrialPeak.Angle;
                        PF.ConstantBackground = TrialPeak.ConstantBackground;
                        PF.CenterBackground = TrialPeak.CenterBackground;
                        PF.AclivityBackground = TrialPeak.AclivityBackground;
                        PF.LorentzRatio = TrialPeak.LorentzRatio;
                        PF.Intensity = TrialPeak.Intensity;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }

        private static bool FitSigmaLorentzRatioBackground(Analysis.Peaks.Functions.PeakFunction PF, double lambda)
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakFunction TrialPeak = PF.Clone() as Analysis.Peaks.Functions.PeakFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeak.ParameterDeltaVektorSigmaLorentzRatioBackground(Lambda);

                #region Parameter calculations

                TrialPeak.SetSigmaWithConstraints = ParamDelta[0];
                TrialPeak.LorentzRatio += ParamDelta[1];
                TrialPeak.ConstantBackground += ParamDelta[2];
                TrialPeak.CenterBackground += ParamDelta[3];
                TrialPeak.AclivityBackground += ParamDelta[4];

                #endregion

                if (Math.Abs(TrialPeak.Chi2Function - PF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeak.Chi2Function > PF.Chi2Function)
                    {
                        TrialPeak.Sigma = PF.Sigma;
                        TrialPeak.ConstantBackground = PF.ConstantBackground;
                        TrialPeak.CenterBackground = PF.CenterBackground;
                        TrialPeak.AclivityBackground = PF.AclivityBackground;
                        TrialPeak.LorentzRatio = PF.LorentzRatio;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        PF.Sigma = TrialPeak.Sigma;
                        PF.ConstantBackground = TrialPeak.ConstantBackground;
                        PF.CenterBackground = TrialPeak.CenterBackground;
                        PF.AclivityBackground = TrialPeak.AclivityBackground;
                        PF.LorentzRatio = TrialPeak.LorentzRatio;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }

        private static bool FitAngleLorentzRatioBackground(Analysis.Peaks.Functions.PeakFunction PF, double lambda)
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakFunction TrialPeak = PF.Clone() as Analysis.Peaks.Functions.PeakFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeak.ParameterDeltaVektorAngleLorentzRatioBackground(Lambda);

                #region Parameter calculations

                TrialPeak.SetAngleWithConstraints = ParamDelta[0];
                TrialPeak.LorentzRatio += ParamDelta[1];
                TrialPeak.ConstantBackground += ParamDelta[2];
                TrialPeak.CenterBackground += ParamDelta[3];
                TrialPeak.AclivityBackground += ParamDelta[4];

                #endregion

                if (Math.Abs(TrialPeak.Chi2Function - PF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeak.Chi2Function > PF.Chi2Function)
                    {
                        TrialPeak.Angle = PF.Angle;
                        TrialPeak.ConstantBackground = PF.ConstantBackground;
                        TrialPeak.CenterBackground = PF.CenterBackground;
                        TrialPeak.AclivityBackground = PF.AclivityBackground;
                        TrialPeak.LorentzRatio = PF.LorentzRatio;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        PF.Angle = TrialPeak.Angle;
                        PF.ConstantBackground = TrialPeak.ConstantBackground;
                        PF.CenterBackground = TrialPeak.CenterBackground;
                        PF.AclivityBackground = TrialPeak.AclivityBackground;
                        PF.LorentzRatio = TrialPeak.LorentzRatio;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }

        private static bool FitAngleSigmaBackground(Analysis.Peaks.Functions.PeakFunction PF, double lambda)
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakFunction TrialPeak = PF.Clone() as Analysis.Peaks.Functions.PeakFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeak.ParameterDeltaVektorSigmaAngleBackground(Lambda);

                #region Parameter calculations

                TrialPeak.SetSigmaWithConstraints = ParamDelta[0];
                TrialPeak.SetAngleWithConstraints = ParamDelta[1];
                TrialPeak.ConstantBackground += ParamDelta[2];
                TrialPeak.CenterBackground += ParamDelta[3];
                TrialPeak.AclivityBackground += ParamDelta[4];

                #endregion

                if (Math.Abs(TrialPeak.Chi2Function - PF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeak.Chi2Function > PF.Chi2Function)
                    {
                        TrialPeak.Sigma = PF.Sigma;
                        TrialPeak.Angle = PF.Angle;
                        TrialPeak.ConstantBackground = PF.ConstantBackground;
                        TrialPeak.CenterBackground = PF.CenterBackground;
                        TrialPeak.AclivityBackground = PF.AclivityBackground;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        PF.Sigma = TrialPeak.Sigma;
                        PF.Angle = TrialPeak.Angle;
                        PF.ConstantBackground = TrialPeak.ConstantBackground;
                        PF.CenterBackground = TrialPeak.CenterBackground;
                        PF.AclivityBackground = TrialPeak.AclivityBackground;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }

        private static bool FitAngleSigmaLorentzRatio(Analysis.Peaks.Functions.PeakFunction PF, double lambda)
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakFunction TrialPeak = PF.Clone() as Analysis.Peaks.Functions.PeakFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeak.ParameterDeltaVektorSigmaAngleLorentzRatio(Lambda);

                #region Parameter calculations

                TrialPeak.SetSigmaWithConstraints = ParamDelta[0];
                TrialPeak.SetAngleWithConstraints = ParamDelta[1];
                TrialPeak.LorentzRatio += ParamDelta[2];

                #endregion
                
                if (Math.Abs(TrialPeak.Chi2Function - PF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeak.Chi2Function > PF.Chi2Function)
                    {
                        TrialPeak.Sigma = PF.Sigma;
                        TrialPeak.Angle = PF.Angle;
                        TrialPeak.LorentzRatio = PF.LorentzRatio;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        PF.Sigma = TrialPeak.Sigma;
                        PF.Angle = TrialPeak.Angle;
                        PF.LorentzRatio = TrialPeak.LorentzRatio;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }

        private static bool FitAngleSigmaIntensity(Analysis.Peaks.Functions.PeakFunction PF, double lambda)
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakFunction TrialPeak = PF.Clone() as Analysis.Peaks.Functions.PeakFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeak.ParameterDeltaVektorSigmaAngleIntensity(Lambda);

                #region Parameter calculations

                TrialPeak.SetSigmaWithConstraints = ParamDelta[0];
                TrialPeak.SetAngleWithConstraints = ParamDelta[1];
                TrialPeak.Intensity += ParamDelta[2];

                #endregion

                if (Math.Abs(TrialPeak.Chi2Function - PF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeak.Chi2Function > PF.Chi2Function)
                    {
                        TrialPeak.Sigma = PF.Sigma;
                        TrialPeak.Angle = PF.Angle;
                        TrialPeak.Intensity = PF.Intensity;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        PF.Sigma = TrialPeak.Sigma;
                        PF.Angle = TrialPeak.Angle;
                        PF.Intensity = TrialPeak.Intensity;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }

        private static bool FitSigmaIntensityBackground(Analysis.Peaks.Functions.PeakFunction PF, double lambda)
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakFunction TrialPeak = PF.Clone() as Analysis.Peaks.Functions.PeakFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeak.ParameterDeltaVektorSigmaIntensityBackground(Lambda);

                #region Parameter calculations

                TrialPeak.SetSigmaWithConstraints = ParamDelta[0];
                TrialPeak.Intensity += ParamDelta[1];
                TrialPeak.ConstantBackground += ParamDelta[2];
                TrialPeak.CenterBackground += ParamDelta[3];
                TrialPeak.AclivityBackground += ParamDelta[4];

                #endregion

                if (Math.Abs(TrialPeak.Chi2Function - PF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeak.Chi2Function > PF.Chi2Function)
                    {
                        TrialPeak.Sigma = PF.Sigma;
                        TrialPeak.ConstantBackground = PF.ConstantBackground;
                        TrialPeak.CenterBackground = PF.CenterBackground;
                        TrialPeak.AclivityBackground = PF.AclivityBackground;
                        TrialPeak.Intensity = PF.Intensity;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        PF.Sigma = TrialPeak.Sigma;
                        PF.ConstantBackground = TrialPeak.ConstantBackground;
                        PF.CenterBackground = TrialPeak.CenterBackground;
                        PF.AclivityBackground = TrialPeak.AclivityBackground;
                        PF.Intensity = TrialPeak.Intensity;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }

        private static bool FitAngleIntensityBackground(Analysis.Peaks.Functions.PeakFunction PF, double lambda)
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakFunction TrialPeak = PF.Clone() as Analysis.Peaks.Functions.PeakFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeak.ParameterDeltaVektorAngleIntensityBackground(Lambda);

                #region Parameter calculations

                TrialPeak.SetAngleWithConstraints = ParamDelta[0];
                TrialPeak.Intensity += ParamDelta[1];
                TrialPeak.ConstantBackground += ParamDelta[2];
                TrialPeak.CenterBackground += ParamDelta[3];
                TrialPeak.AclivityBackground += ParamDelta[4];

                #endregion

                if (Math.Abs(TrialPeak.Chi2Function - PF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeak.Chi2Function > PF.Chi2Function)
                    {
                        TrialPeak.Angle = PF.Angle;
                        TrialPeak.ConstantBackground = PF.ConstantBackground;
                        TrialPeak.CenterBackground = PF.CenterBackground;
                        TrialPeak.AclivityBackground = PF.AclivityBackground;
                        TrialPeak.Intensity = PF.Intensity;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        PF.Angle = TrialPeak.Angle;
                        PF.ConstantBackground = TrialPeak.ConstantBackground;
                        PF.CenterBackground = TrialPeak.CenterBackground;
                        PF.AclivityBackground = TrialPeak.AclivityBackground;
                        PF.Intensity = TrialPeak.Intensity;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }

        private static bool FitAngleBackground(Analysis.Peaks.Functions.PeakFunction PF, double lambda)
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakFunction TrialPeak = PF.Clone() as Analysis.Peaks.Functions.PeakFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeak.ParameterDeltaVektorAngleBackground(Lambda);

                #region Parameter calculations

                TrialPeak.SetAngleWithConstraints = ParamDelta[0];
                TrialPeak.ConstantBackground += ParamDelta[1];
                TrialPeak.CenterBackground += ParamDelta[2];
                TrialPeak.AclivityBackground += ParamDelta[3];

                #endregion

                if (Math.Abs(TrialPeak.Chi2Function - PF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeak.Chi2Function > PF.Chi2Function)
                    {
                        TrialPeak.Angle = PF.Angle;
                        TrialPeak.ConstantBackground = PF.ConstantBackground;
                        TrialPeak.CenterBackground = PF.CenterBackground;
                        TrialPeak.AclivityBackground = PF.AclivityBackground;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        PF.Angle = TrialPeak.Angle;
                        PF.ConstantBackground = TrialPeak.ConstantBackground;
                        PF.CenterBackground = TrialPeak.CenterBackground;
                        PF.AclivityBackground = TrialPeak.AclivityBackground;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }

        private static bool FitSigmaBackground(Analysis.Peaks.Functions.PeakFunction PF, double lambda)
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakFunction TrialPeak = PF.Clone() as Analysis.Peaks.Functions.PeakFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeak.ParameterDeltaVektorSigmaBackground(Lambda);

                #region Parameter calculations

                TrialPeak.SetSigmaWithConstraints = ParamDelta[0];
                TrialPeak.ConstantBackground += ParamDelta[1];
                TrialPeak.CenterBackground += ParamDelta[2];
                TrialPeak.AclivityBackground += ParamDelta[3];

                #endregion
                
                if (Math.Abs(TrialPeak.Chi2Function - PF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeak.Chi2Function > PF.Chi2Function)
                    {
                        TrialPeak.Sigma = PF.Sigma;
                        TrialPeak.ConstantBackground = PF.ConstantBackground;
                        TrialPeak.CenterBackground = PF.CenterBackground;
                        TrialPeak.AclivityBackground = PF.AclivityBackground;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        PF.Sigma = TrialPeak.Sigma;
                        PF.ConstantBackground = TrialPeak.ConstantBackground;
                        PF.CenterBackground = TrialPeak.CenterBackground;
                        PF.AclivityBackground = TrialPeak.AclivityBackground;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }

        private static bool FitAngleSigma(Analysis.Peaks.Functions.PeakFunction PF, double lambda)
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakFunction TrialPeak = PF.Clone() as Analysis.Peaks.Functions.PeakFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeak.ParameterDeltaVektorSigmaAngle(Lambda);

                #region Parameter calculations

                TrialPeak.SetSigmaWithConstraints = ParamDelta[0];
                TrialPeak.SetAngleWithConstraints = ParamDelta[1];

                #endregion

                if (Math.Abs(TrialPeak.Chi2Function - PF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeak.Chi2Function > PF.Chi2Function)
                    {
                        TrialPeak.Sigma = PF.Sigma;
                        TrialPeak.Angle = PF.Angle;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        PF.Sigma = TrialPeak.Sigma;
                        PF.Angle = TrialPeak.Angle;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }

        private static bool FitAngleLorentzRatioIntensity(Analysis.Peaks.Functions.PeakFunction PF, double lambda)
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakFunction TrialPeak = PF.Clone() as Analysis.Peaks.Functions.PeakFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeak.ParameterDeltaVektorAngleLorentzRatioIntensity(Lambda);

                #region Parameter calculations

                TrialPeak.SetAngleWithConstraints = ParamDelta[0];
                TrialPeak.LorentzRatio += ParamDelta[1];
                TrialPeak.Intensity += ParamDelta[2];

                #endregion
                
                if (Math.Abs(TrialPeak.Chi2Function - PF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeak.Chi2Function > PF.Chi2Function)
                    {
                        TrialPeak.LorentzRatio = PF.LorentzRatio;
                        TrialPeak.Angle = PF.Angle;
                        TrialPeak.Intensity = PF.Intensity;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        PF.LorentzRatio = TrialPeak.LorentzRatio;
                        PF.Angle = TrialPeak.Angle;
                        PF.Intensity = TrialPeak.Intensity;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }

        private static bool FitSigmaLorentzRatioIntensity(Analysis.Peaks.Functions.PeakFunction PF, double lambda)
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakFunction TrialPeak = PF.Clone() as Analysis.Peaks.Functions.PeakFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeak.ParameterDeltaVektorSigmaLorentzRatioIntensity(Lambda);

                #region Parameter calculations

                TrialPeak.SetSigmaWithConstraints = ParamDelta[0];
                TrialPeak.LorentzRatio += ParamDelta[1];
                TrialPeak.Intensity += ParamDelta[2];

                #endregion

                if (Math.Abs(TrialPeak.Chi2Function - PF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeak.Chi2Function > PF.Chi2Function)
                    {
                        TrialPeak.LorentzRatio = PF.LorentzRatio;
                        TrialPeak.Sigma = PF.Sigma;
                        TrialPeak.Intensity = PF.Intensity;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        PF.LorentzRatio = TrialPeak.LorentzRatio;
                        PF.Sigma = TrialPeak.Sigma;
                        PF.Intensity = TrialPeak.Intensity;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }

        private static bool FitIntensityLorentzRatioBackground(Analysis.Peaks.Functions.PeakFunction PF, double lambda)
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakFunction TrialPeak = PF.Clone() as Analysis.Peaks.Functions.PeakFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeak.ParameterDeltaVektorIntensityLorentzRatioBackground(Lambda);

                #region Parameter calculations

                TrialPeak.Intensity += ParamDelta[0];
                TrialPeak.LorentzRatio += ParamDelta[1];
                TrialPeak.ConstantBackground += ParamDelta[2];
                TrialPeak.CenterBackground += ParamDelta[3];
                TrialPeak.AclivityBackground += ParamDelta[4];

                #endregion

                if (Math.Abs(TrialPeak.Chi2Function - PF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeak.Chi2Function > PF.Chi2Function)
                    {
                        TrialPeak.ConstantBackground = PF.ConstantBackground;
                        TrialPeak.CenterBackground = PF.CenterBackground;
                        TrialPeak.AclivityBackground = PF.AclivityBackground;
                        TrialPeak.LorentzRatio = PF.LorentzRatio;
                        TrialPeak.Intensity = PF.Intensity;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        PF.ConstantBackground = TrialPeak.ConstantBackground;
                        PF.CenterBackground = TrialPeak.CenterBackground;
                        PF.AclivityBackground = TrialPeak.AclivityBackground;
                        PF.LorentzRatio = TrialPeak.LorentzRatio;
                        PF.Intensity = TrialPeak.Intensity;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }

        private static bool FitLorentzRatioBackground(Analysis.Peaks.Functions.PeakFunction PF, double lambda)
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakFunction TrialPeak = PF.Clone() as Analysis.Peaks.Functions.PeakFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeak.ParameterDeltaVektorLorentzRatioBackground(Lambda);

                #region Parameter calculations
                
                TrialPeak.LorentzRatio += ParamDelta[0];
                TrialPeak.ConstantBackground += ParamDelta[1];
                TrialPeak.CenterBackground += ParamDelta[2];
                TrialPeak.AclivityBackground += ParamDelta[3];

                #endregion

                if (Math.Abs(TrialPeak.Chi2Function - PF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeak.Chi2Function > PF.Chi2Function)
                    {
                        TrialPeak.ConstantBackground = PF.ConstantBackground;
                        TrialPeak.CenterBackground = PF.CenterBackground;
                        TrialPeak.AclivityBackground = PF.AclivityBackground;
                        TrialPeak.LorentzRatio = PF.LorentzRatio;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        PF.ConstantBackground = TrialPeak.ConstantBackground;
                        PF.CenterBackground = TrialPeak.CenterBackground;
                        PF.AclivityBackground = TrialPeak.AclivityBackground;
                        PF.LorentzRatio = TrialPeak.LorentzRatio;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }

        private static bool FitIntensityBackground(Analysis.Peaks.Functions.PeakFunction PF, double lambda)
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakFunction TrialPeak = PF.Clone() as Analysis.Peaks.Functions.PeakFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeak.ParameterDeltaVektorIntensityBackground(Lambda);

                #region Parameter calculations

                TrialPeak.Intensity += ParamDelta[0];
                TrialPeak.ConstantBackground += ParamDelta[1];
                TrialPeak.CenterBackground += ParamDelta[2];
                TrialPeak.AclivityBackground += ParamDelta[3];

                #endregion

                if (Math.Abs(TrialPeak.Chi2Function - PF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeak.Chi2Function > PF.Chi2Function)
                    {
                        TrialPeak.ConstantBackground = PF.ConstantBackground;
                        TrialPeak.CenterBackground = PF.CenterBackground;
                        TrialPeak.AclivityBackground = PF.AclivityBackground;
                        TrialPeak.Intensity = PF.Intensity;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        PF.ConstantBackground = TrialPeak.ConstantBackground;
                        PF.CenterBackground = TrialPeak.CenterBackground;
                        PF.AclivityBackground = TrialPeak.AclivityBackground;
                        PF.Intensity = TrialPeak.Intensity;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }

        private static bool FitAngleIntensity(Analysis.Peaks.Functions.PeakFunction PF, double lambda)
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakFunction TrialPeak = PF.Clone() as Analysis.Peaks.Functions.PeakFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeak.ParameterDeltaVektorAngleIntensity(Lambda);

                #region Parameter calculations

                TrialPeak.SetAngleWithConstraints = ParamDelta[0];
                TrialPeak.Intensity += ParamDelta[1];

                #endregion

                if (Math.Abs(TrialPeak.Chi2Function - PF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeak.Chi2Function > PF.Chi2Function)
                    {
                        TrialPeak.Angle = PF.Angle;
                        TrialPeak.Intensity = PF.Intensity;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        PF.Angle = TrialPeak.Angle;
                        PF.Intensity = TrialPeak.Intensity;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }

        private static bool FitSigmaIntensity(Analysis.Peaks.Functions.PeakFunction PF, double lambda)
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakFunction TrialPeak = PF.Clone() as Analysis.Peaks.Functions.PeakFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeak.ParameterDeltaVektorSigmaIntensity(Lambda);

                #region Parameter calculations

                TrialPeak.SetSigmaWithConstraints = ParamDelta[0];
                TrialPeak.Intensity += ParamDelta[1];

                #endregion
                
                if (Math.Abs(TrialPeak.Chi2Function - PF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeak.Chi2Function > PF.Chi2Function)
                    {
                        TrialPeak.Sigma = PF.Sigma;
                        TrialPeak.Intensity = PF.Intensity;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        PF.Sigma = TrialPeak.Sigma;
                        PF.Intensity = TrialPeak.Intensity;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }

        private static bool FitSigmaLorentzRatio(Analysis.Peaks.Functions.PeakFunction PF, double lambda)
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakFunction TrialPeak = PF.Clone() as Analysis.Peaks.Functions.PeakFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeak.ParameterDeltaVektorSigmaLorentzRatio(Lambda);

                #region Parameter calculations

                TrialPeak.SetSigmaWithConstraints = ParamDelta[0];
                TrialPeak.LorentzRatio += ParamDelta[1];

                #endregion

                if (Math.Abs(TrialPeak.Chi2Function - PF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeak.Chi2Function > PF.Chi2Function)
                    {
                        TrialPeak.Sigma = PF.Sigma;
                        TrialPeak.LorentzRatio = PF.LorentzRatio;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        PF.Sigma = TrialPeak.Sigma;
                        PF.LorentzRatio = TrialPeak.LorentzRatio;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }

        private static bool FitIntensityLorentzRatio(Analysis.Peaks.Functions.PeakFunction PF, double lambda)
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakFunction TrialPeak = PF.Clone() as Analysis.Peaks.Functions.PeakFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeak.ParameterDeltaVektorIntensityLorentzRatio(Lambda);

                #region Parameter calculations

                TrialPeak.Intensity += ParamDelta[0];
                TrialPeak.LorentzRatio += ParamDelta[1];

                #endregion

                if (Math.Abs(TrialPeak.Chi2Function - PF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeak.Chi2Function > PF.Chi2Function)
                    {
                        TrialPeak.Intensity = PF.Intensity;
                        TrialPeak.LorentzRatio = PF.LorentzRatio;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        PF.Intensity = TrialPeak.Intensity;
                        PF.LorentzRatio = TrialPeak.LorentzRatio;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }

        private static bool FitAngleLorentzRatio(Analysis.Peaks.Functions.PeakFunction PF, double lambda)
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakFunction TrialPeak = PF.Clone() as Analysis.Peaks.Functions.PeakFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeak.ParameterDeltaVektorAngleLorentzRatio(Lambda);

                #region Parameter calculations

                TrialPeak.SetAngleWithConstraints = ParamDelta[0];
                TrialPeak.LorentzRatio += ParamDelta[1];

                #endregion

                if (Math.Abs(TrialPeak.Chi2Function - PF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeak.Chi2Function > PF.Chi2Function)
                    {
                        TrialPeak.LorentzRatio = PF.LorentzRatio;
                        TrialPeak.Angle = PF.Angle;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        PF.LorentzRatio = TrialPeak.LorentzRatio;
                        PF.Angle = TrialPeak.Angle;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }

        private static bool FitIntensity(Analysis.Peaks.Functions.PeakFunction PF, double lambda)
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakFunction TrialPeak = PF.Clone() as Analysis.Peaks.Functions.PeakFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                //[0][0] Intensity
                double HessianValue = 0.0;

                //[0] Intensity
                double SolutionValue = 0.0;

                for (int n = 0; n < TrialPeak.FittingCounts.Count; n++)
                {
                    HessianValue += (TrialPeak.FirstDerivativeIntensity(PF.FittingCounts[n][0]) * TrialPeak.FirstDerivativeIntensity(PF.FittingCounts[n][0]) / Math.Pow(PF.FittingCounts[n][2], 2));
                    
                    SolutionValue += ((TrialPeak.FittingCounts[n][1] - TrialPeak.Y(TrialPeak.FittingCounts[n][0])) / Math.Pow(TrialPeak.FittingCounts[n][2], 2)) * TrialPeak.FirstDerivativeIntensity(TrialPeak.FittingCounts[n][0]);
                }

                HessianValue *= (1 + Lambda);

                double ParamDelta = SolutionValue / HessianValue;

                #region Parameter calculations

                TrialPeak.Intensity += ParamDelta;

                #endregion

                if (Math.Abs(TrialPeak.Chi2Function - PF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeak.Chi2Function > PF.Chi2Function)
                    {
                        TrialPeak.Intensity = PF.Intensity;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        PF.Intensity = TrialPeak.Intensity;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }

        private static bool FitAngle(Analysis.Peaks.Functions.PeakFunction PF, double lambda)
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakFunction TrialPeak = PF.Clone() as Analysis.Peaks.Functions.PeakFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                //[0][0] Angle
                double HessianValue = 0.0;

                //[0] Angle
                double SolutionValue = 0.0;

                for (int n = 0; n < TrialPeak.FittingCounts.Count; n++)
                {
                    HessianValue += (TrialPeak.FirstDerivativeAngle(PF.FittingCounts[n][0]) * TrialPeak.FirstDerivativeAngle(PF.FittingCounts[n][0]) / Math.Pow(PF.FittingCounts[n][2], 2));

                    SolutionValue += ((TrialPeak.FittingCounts[n][1] - TrialPeak.Y(TrialPeak.FittingCounts[n][0])) / Math.Pow(TrialPeak.FittingCounts[n][2], 2)) * TrialPeak.FirstDerivativeAngle(TrialPeak.FittingCounts[n][0]);
                }

                HessianValue *= (1 + Lambda);

                double ParamDelta = SolutionValue / HessianValue;

                #region Parameter calculations

                TrialPeak.SetAngleWithConstraints = ParamDelta;

                #endregion

                if (Math.Abs(TrialPeak.Chi2Function - PF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeak.Chi2Function > PF.Chi2Function)
                    {
                        TrialPeak.Angle = PF.Angle;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        PF.Angle = TrialPeak.Angle;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }

        private static bool FitSigma(Analysis.Peaks.Functions.PeakFunction PF, double lambda)
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakFunction TrialPeak = PF.Clone() as Analysis.Peaks.Functions.PeakFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                //[0][0] Sigma
                double HessianValue = 0.0;

                //[0] Sigma
                double SolutionValue = 0.0;

                for (int n = 0; n < TrialPeak.FittingCounts.Count; n++)
                {
                    HessianValue += (TrialPeak.FirstDerivativeSigma(PF.FittingCounts[n][0]) * TrialPeak.FirstDerivativeSigma(PF.FittingCounts[n][0]) / Math.Pow(PF.FittingCounts[n][2], 2));

                    SolutionValue += ((TrialPeak.FittingCounts[n][1] - TrialPeak.Y(TrialPeak.FittingCounts[n][0])) / Math.Pow(TrialPeak.FittingCounts[n][2], 2)) * TrialPeak.FirstDerivativeSigma(TrialPeak.FittingCounts[n][0]);
                }

                HessianValue *= (1 + Lambda);

                double ParamDelta = SolutionValue / HessianValue;

                #region Parameter calculations

                TrialPeak.SetSigmaWithConstraints = ParamDelta;

                #endregion

                if (Math.Abs(TrialPeak.Chi2Function - PF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeak.Chi2Function > PF.Chi2Function)
                    {
                        TrialPeak.Sigma = PF.Sigma;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        PF.Sigma = TrialPeak.Sigma;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }

        private static bool FitLorentzRatio(Analysis.Peaks.Functions.PeakFunction PF, double lambda)
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakFunction TrialPeak = PF.Clone() as Analysis.Peaks.Functions.PeakFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                //[0][0] LorentzRatio
                double HessianValue = 0.0;

                //[0] LorentzRatio
                double SolutionValue = 0.0;

                for (int n = 0; n < TrialPeak.FittingCounts.Count; n++)
                {
                    HessianValue += (TrialPeak.FirstDerivativeLorentzRatio(PF.FittingCounts[n][0]) * TrialPeak.FirstDerivativeLorentzRatio(PF.FittingCounts[n][0]) / Math.Pow(PF.FittingCounts[n][2], 2));

                    SolutionValue += ((TrialPeak.FittingCounts[n][1] - TrialPeak.Y(TrialPeak.FittingCounts[n][0])) / Math.Pow(TrialPeak.FittingCounts[n][2], 2)) * TrialPeak.FirstDerivativeLorentzRatio(TrialPeak.FittingCounts[n][0]);
                }

                HessianValue *= (1 + Lambda);

                double ParamDelta = SolutionValue / HessianValue;


                #region Parameter calculations

                TrialPeak.LorentzRatio += ParamDelta;

                #endregion

                if (Math.Abs(TrialPeak.Chi2Function - PF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeak.Chi2Function > PF.Chi2Function)
                    {
                        TrialPeak.LorentzRatio = PF.LorentzRatio;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        PF.LorentzRatio = TrialPeak.LorentzRatio;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }

        private static bool FitBackground(Analysis.Peaks.Functions.PeakFunction PF, double lambda)
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakFunction TrialPeak = PF.Clone() as Analysis.Peaks.Functions.PeakFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeak.ParameterDeltaVektorBackground(Lambda);

                #region Parameter calculations

                TrialPeak.ConstantBackground += ParamDelta[0];
                TrialPeak.CenterBackground += ParamDelta[1];
                TrialPeak.AclivityBackground += ParamDelta[2];

                #endregion

                if (Math.Abs(TrialPeak.Chi2Function - PF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeak.Chi2Function > PF.Chi2Function)
                    {
                        TrialPeak.ConstantBackground = PF.ConstantBackground;
                        TrialPeak.CenterBackground = PF.CenterBackground;
                        TrialPeak.AclivityBackground = PF.AclivityBackground;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        PF.ConstantBackground = TrialPeak.ConstantBackground;
                        PF.CenterBackground = TrialPeak.CenterBackground;
                        PF.AclivityBackground = TrialPeak.AclivityBackground;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }

        #endregion

        #region Peak region routines

        private static void CorrectFWHMLorentzRatioDown(Analysis.Peaks.Functions.PeakRegionFunction PRF)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int n = 0; n < PRF.Count; n++)
                {
                    PRF[n].Sigma *= 0.9;
                    PRF[n].LorentzRatio *= 0.9;
                }

                FitAngleSigmaIntensityLorentzRatioBackground(PRF, LMA._lambda, true);
            }
        }

        private static bool FitAngleSigmaIntensityLorentzRatioBackground(Analysis.Peaks.Functions.PeakRegionFunction PRF, double lambda)
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakRegionFunction TrialPeakRegion = PRF.Clone() as Analysis.Peaks.Functions.PeakRegionFunction;

            double FirstChi2 = TrialPeakRegion.Chi2Function;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeakRegion.ParameterDeltaVektorSigmaAngleIntensityLorentzRatioBackground(Lambda);

                #region Parameter calculations

                for (int n = 0; n < TrialPeakRegion.Count; n++)
                {
                    TrialPeakRegion[n].SetSigmaWithConstraints = ParamDelta[(n * 4) + 0];
                    TrialPeakRegion[n].SetAngleWithConstraints = ParamDelta[(n * 4) + 1];
                    TrialPeakRegion[n].Intensity += ParamDelta[(n * 4) + 2];
                    TrialPeakRegion[n].LorentzRatio += ParamDelta[(n * 4) + 3];
                }


                TrialPeakRegion.PolynomialBackgroundFunction.Constant += ParamDelta[ParamDelta.Count - 3];
                TrialPeakRegion.PolynomialBackgroundFunction.Center += ParamDelta[ParamDelta.Count - 2];
                TrialPeakRegion.PolynomialBackgroundFunction.Aclivity += ParamDelta[ParamDelta.Count - 1];

                #endregion

                if (!double.IsNaN(TrialPeakRegion.Chi2Function) && !double.IsInfinity(TrialPeakRegion.Chi2Function))
                {
                    if (Math.Abs(TrialPeakRegion.Chi2Function - PRF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                    {
                        if (TrialPeakRegion.Chi2Function > PRF.Chi2Function)
                        {
                            for (int n = 0; n < TrialPeakRegion.Count; n++)
                            {
                                TrialPeakRegion[n].Sigma = PRF[n].Sigma;
                                TrialPeakRegion[n].Angle = PRF[n].Angle;
                                TrialPeakRegion[n].Intensity = PRF[n].Intensity;
                                TrialPeakRegion[n].LorentzRatio = PRF[n].LorentzRatio;
                            }

                            TrialPeakRegion.PolynomialBackgroundFunction.Constant = PRF.PolynomialBackgroundFunction.Constant;
                            TrialPeakRegion.PolynomialBackgroundFunction.Center = PRF.PolynomialBackgroundFunction.Center;
                            TrialPeakRegion.PolynomialBackgroundFunction.Aclivity = PRF.PolynomialBackgroundFunction.Aclivity;

                            Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                        }
                        else
                        {
                            for (int n = 0; n < TrialPeakRegion.Count; n++)
                            {
                                PRF[n].Sigma = TrialPeakRegion[n].Sigma;
                                PRF[n].Angle = TrialPeakRegion[n].Angle;
                                PRF[n].Intensity = TrialPeakRegion[n].Intensity;
                                PRF[n].LorentzRatio = TrialPeakRegion[n].LorentzRatio;
                            }
                            PRF.PolynomialBackgroundFunction.Constant = TrialPeakRegion.PolynomialBackgroundFunction.Constant;
                            PRF.PolynomialBackgroundFunction.Center = TrialPeakRegion.PolynomialBackgroundFunction.Center;
                            PRF.PolynomialBackgroundFunction.Aclivity = TrialPeakRegion.PolynomialBackgroundFunction.Aclivity;

                            Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                        }
                    }
                    else
                    {
                        if (TotalTrials > 3)
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    for (int n = 0; n < TrialPeakRegion.Count; n++)
                    {
                        TrialPeakRegion[n].Sigma = PRF[n].Sigma;
                        TrialPeakRegion[n].Angle = PRF[n].Angle;
                        TrialPeakRegion[n].Intensity = PRF[n].Intensity;
                        TrialPeakRegion[n].LorentzRatio = PRF[n].LorentzRatio;
                    }

                    TrialPeakRegion.PolynomialBackgroundFunction.Constant = PRF.PolynomialBackgroundFunction.Constant;
                    TrialPeakRegion.PolynomialBackgroundFunction.Center = PRF.PolynomialBackgroundFunction.Center;
                    TrialPeakRegion.PolynomialBackgroundFunction.Aclivity = PRF.PolynomialBackgroundFunction.Aclivity;

                    Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                }
            }

            if(CalScec.Properties.Settings.Default.ReflexFitAutoCorrection && PRF.Chi2Function > 8.0)
            {
                CorrectFWHMLorentzRatioDown(PRF);
            }

            return Converged;
        }

        private static bool FitAngleSigmaIntensityLorentzRatioBackground(Analysis.Peaks.Functions.PeakRegionFunction PRF, double lambda, bool done)
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakRegionFunction TrialPeakRegion = PRF.Clone() as Analysis.Peaks.Functions.PeakRegionFunction;

            double FirstChi2 = TrialPeakRegion.Chi2Function;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeakRegion.ParameterDeltaVektorSigmaAngleIntensityLorentzRatioBackground(Lambda);

                #region Parameter calculations

                for (int n = 0; n < TrialPeakRegion.Count; n++)
                {
                    TrialPeakRegion[n].SetSigmaWithConstraints = ParamDelta[(n * 4) + 0];
                    TrialPeakRegion[n].SetAngleWithConstraints = ParamDelta[(n * 4) + 1];
                    TrialPeakRegion[n].Intensity += ParamDelta[(n * 4) + 2];
                    TrialPeakRegion[n].LorentzRatio += ParamDelta[(n * 4) + 3];
                }


                TrialPeakRegion.PolynomialBackgroundFunction.Constant += ParamDelta[ParamDelta.Count - 3];
                TrialPeakRegion.PolynomialBackgroundFunction.Center += ParamDelta[ParamDelta.Count - 2];
                TrialPeakRegion.PolynomialBackgroundFunction.Aclivity += ParamDelta[ParamDelta.Count - 1];

                #endregion
                if (!double.IsNaN(TrialPeakRegion.Chi2Function) && !double.IsInfinity(TrialPeakRegion.Chi2Function))
                {
                    if (Math.Abs(TrialPeakRegion.Chi2Function - PRF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                    {
                        if (TrialPeakRegion.Chi2Function > PRF.Chi2Function)
                        {
                            for (int n = 0; n < TrialPeakRegion.Count; n++)
                            {
                                TrialPeakRegion[n].Sigma = PRF[n].Sigma;
                                TrialPeakRegion[n].Angle = PRF[n].Angle;
                                TrialPeakRegion[n].Intensity = PRF[n].Intensity;
                                TrialPeakRegion[n].LorentzRatio = PRF[n].LorentzRatio;
                            }

                            TrialPeakRegion.PolynomialBackgroundFunction.Constant = PRF.PolynomialBackgroundFunction.Constant;
                            TrialPeakRegion.PolynomialBackgroundFunction.Center = PRF.PolynomialBackgroundFunction.Center;
                            TrialPeakRegion.PolynomialBackgroundFunction.Aclivity = PRF.PolynomialBackgroundFunction.Aclivity;

                            Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                        }
                        else
                        {
                            for (int n = 0; n < TrialPeakRegion.Count; n++)
                            {
                                PRF[n].Sigma = TrialPeakRegion[n].Sigma;
                                PRF[n].Angle = TrialPeakRegion[n].Angle;
                                PRF[n].Intensity = TrialPeakRegion[n].Intensity;
                                PRF[n].LorentzRatio = TrialPeakRegion[n].LorentzRatio;
                            }
                            PRF.PolynomialBackgroundFunction.Constant = TrialPeakRegion.PolynomialBackgroundFunction.Constant;
                            PRF.PolynomialBackgroundFunction.Center = TrialPeakRegion.PolynomialBackgroundFunction.Center;
                            PRF.PolynomialBackgroundFunction.Aclivity = TrialPeakRegion.PolynomialBackgroundFunction.Aclivity;

                            Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                        }
                    }
                    else
                    {
                        if (TotalTrials > 3)
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    for (int n = 0; n < TrialPeakRegion.Count; n++)
                    {
                        TrialPeakRegion[n].Sigma = PRF[n].Sigma;
                        TrialPeakRegion[n].Angle = PRF[n].Angle;
                        TrialPeakRegion[n].Intensity = PRF[n].Intensity;
                        TrialPeakRegion[n].LorentzRatio = PRF[n].LorentzRatio;
                    }

                    TrialPeakRegion.PolynomialBackgroundFunction.Constant = PRF.PolynomialBackgroundFunction.Constant;
                    TrialPeakRegion.PolynomialBackgroundFunction.Center = PRF.PolynomialBackgroundFunction.Center;
                    TrialPeakRegion.PolynomialBackgroundFunction.Aclivity = PRF.PolynomialBackgroundFunction.Aclivity;

                    Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                }
            }

            return Converged;
        }

        private static bool FitAngleSigmaLorentzRatioBackground(Analysis.Peaks.Functions.PeakRegionFunction PRF, double lambda)
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakRegionFunction TrialPeakRegion = PRF.Clone() as Analysis.Peaks.Functions.PeakRegionFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeakRegion.ParameterDeltaVektorSigmaAngleLorentzRatioBackground(Lambda);

                #region Parameter calculations

                for (int n = 0; n < TrialPeakRegion.Count; n++)
                {
                    TrialPeakRegion[n].SetSigmaWithConstraints = ParamDelta[(n * 3) + 0];
                    TrialPeakRegion[n].SetAngleWithConstraints = ParamDelta[(n * 3) + 1];
                    TrialPeakRegion[n].LorentzRatio += ParamDelta[(n * 3) + 2];
                }


                TrialPeakRegion.PolynomialBackgroundFunction.Constant += ParamDelta[ParamDelta.Count - 3];
                TrialPeakRegion.PolynomialBackgroundFunction.Center += ParamDelta[ParamDelta.Count - 2];
                TrialPeakRegion.PolynomialBackgroundFunction.Aclivity += ParamDelta[ParamDelta.Count - 1];

                #endregion

                if (Math.Abs(TrialPeakRegion.Chi2Function - PRF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeakRegion.Chi2Function > PRF.Chi2Function)
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            TrialPeakRegion[n].Sigma = PRF[n].Sigma;
                            TrialPeakRegion[n].Angle = PRF[n].Angle;
                            TrialPeakRegion[n].LorentzRatio = PRF[n].LorentzRatio;
                        }

                        TrialPeakRegion.PolynomialBackgroundFunction.Constant = PRF.PolynomialBackgroundFunction.Constant;
                        TrialPeakRegion.PolynomialBackgroundFunction.Center = PRF.PolynomialBackgroundFunction.Center;
                        TrialPeakRegion.PolynomialBackgroundFunction.Aclivity = PRF.PolynomialBackgroundFunction.Aclivity;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            PRF[n].Sigma = TrialPeakRegion[n].Sigma;
                            PRF[n].Angle = TrialPeakRegion[n].Angle;
                            PRF[n].LorentzRatio = TrialPeakRegion[n].LorentzRatio;
                        }
                        PRF.PolynomialBackgroundFunction.Constant = TrialPeakRegion.PolynomialBackgroundFunction.Constant;
                        PRF.PolynomialBackgroundFunction.Center = TrialPeakRegion.PolynomialBackgroundFunction.Center;
                        PRF.PolynomialBackgroundFunction.Aclivity = TrialPeakRegion.PolynomialBackgroundFunction.Aclivity;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }

        private static bool FitAngleSigmaIntensityBackground(Analysis.Peaks.Functions.PeakRegionFunction PRF, double lambda)
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakRegionFunction TrialPeakRegion = PRF.Clone() as Analysis.Peaks.Functions.PeakRegionFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeakRegion.ParameterDeltaVektorSigmaAngleIntensityBackground(Lambda);

                #region Parameter calculations

                for (int n = 0; n < TrialPeakRegion.Count; n++)
                {
                    TrialPeakRegion[n].SetSigmaWithConstraints = ParamDelta[(n * 3) + 0];
                    TrialPeakRegion[n].SetAngleWithConstraints = ParamDelta[(n * 3) + 1];
                    TrialPeakRegion[n].Intensity += ParamDelta[(n * 3) + 2];
                }


                TrialPeakRegion.PolynomialBackgroundFunction.Constant += ParamDelta[ParamDelta.Count - 3];
                TrialPeakRegion.PolynomialBackgroundFunction.Center += ParamDelta[ParamDelta.Count - 2];
                TrialPeakRegion.PolynomialBackgroundFunction.Aclivity += ParamDelta[ParamDelta.Count - 1];

                #endregion

                if (Math.Abs(TrialPeakRegion.Chi2Function - PRF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeakRegion.Chi2Function > PRF.Chi2Function)
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            TrialPeakRegion[n].Sigma = PRF[n].Sigma;
                            TrialPeakRegion[n].Angle = PRF[n].Angle;
                            TrialPeakRegion[n].Intensity = PRF[n].Intensity;
                        }

                        TrialPeakRegion.PolynomialBackgroundFunction.Constant = PRF.PolynomialBackgroundFunction.Constant;
                        TrialPeakRegion.PolynomialBackgroundFunction.Center = PRF.PolynomialBackgroundFunction.Center;
                        TrialPeakRegion.PolynomialBackgroundFunction.Aclivity = PRF.PolynomialBackgroundFunction.Aclivity;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            PRF[n].Sigma = TrialPeakRegion[n].Sigma;
                            PRF[n].Angle = TrialPeakRegion[n].Angle;
                            PRF[n].Intensity = TrialPeakRegion[n].Intensity;
                        }
                        PRF.PolynomialBackgroundFunction.Constant = TrialPeakRegion.PolynomialBackgroundFunction.Constant;
                        PRF.PolynomialBackgroundFunction.Center = TrialPeakRegion.PolynomialBackgroundFunction.Center;
                        PRF.PolynomialBackgroundFunction.Aclivity = TrialPeakRegion.PolynomialBackgroundFunction.Aclivity;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }

        private static bool FitAngleSigmaIntensityLorentzRatio(Analysis.Peaks.Functions.PeakRegionFunction PRF, double lambda)
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakRegionFunction TrialPeakRegion = PRF.Clone() as Analysis.Peaks.Functions.PeakRegionFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeakRegion.ParameterDeltaVektorSigmaAngleIntensityLorentzRatio(Lambda);

                #region Parameter calculations

                for (int n = 0; n < TrialPeakRegion.Count; n++)
                {
                    TrialPeakRegion[n].SetSigmaWithConstraints = ParamDelta[(n * 4) + 0];
                    TrialPeakRegion[n].SetAngleWithConstraints = ParamDelta[(n * 4) + 1];
                    TrialPeakRegion[n].Intensity += ParamDelta[(n * 4) + 2];
                    TrialPeakRegion[n].LorentzRatio += ParamDelta[(n * 4) + 3];
                }

                #endregion

                if (Math.Abs(TrialPeakRegion.Chi2Function - PRF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeakRegion.Chi2Function > PRF.Chi2Function)
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            TrialPeakRegion[n].Sigma = PRF[n].Sigma;
                            TrialPeakRegion[n].Angle = PRF[n].Angle;
                            TrialPeakRegion[n].Intensity = PRF[n].Intensity;
                            TrialPeakRegion[n].LorentzRatio = PRF[n].LorentzRatio;
                        }

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            PRF[n].Sigma = TrialPeakRegion[n].Sigma;
                            PRF[n].Angle = TrialPeakRegion[n].Angle;
                            PRF[n].Intensity = TrialPeakRegion[n].Intensity;
                            PRF[n].LorentzRatio = TrialPeakRegion[n].LorentzRatio;
                        }

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }

        private static bool FitSigmaLorentzRatioIntensityBackground(Analysis.Peaks.Functions.PeakRegionFunction PRF, double lambda)
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakRegionFunction TrialPeakRegion = PRF.Clone() as Analysis.Peaks.Functions.PeakRegionFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeakRegion.ParameterDeltaVektorSigmaIntensityLorentzRatioBackground(Lambda);

                #region Parameter calculations

                for (int n = 0; n < TrialPeakRegion.Count; n++)
                {
                    TrialPeakRegion[n].SetSigmaWithConstraints = ParamDelta[(n * 3) + 0];
                    TrialPeakRegion[n].Intensity += ParamDelta[(n * 3) + 1];
                    TrialPeakRegion[n].LorentzRatio += ParamDelta[(n * 3) + 2];
                }


                TrialPeakRegion.PolynomialBackgroundFunction.Constant += ParamDelta[ParamDelta.Count - 3];
                TrialPeakRegion.PolynomialBackgroundFunction.Center += ParamDelta[ParamDelta.Count - 2];
                TrialPeakRegion.PolynomialBackgroundFunction.Aclivity += ParamDelta[ParamDelta.Count - 1];

                #endregion

                if (Math.Abs(TrialPeakRegion.Chi2Function - PRF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeakRegion.Chi2Function > PRF.Chi2Function)
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            TrialPeakRegion[n].Sigma = PRF[n].Sigma;
                            TrialPeakRegion[n].Intensity = PRF[n].Intensity;
                            TrialPeakRegion[n].LorentzRatio = PRF[n].LorentzRatio;
                        }

                        TrialPeakRegion.PolynomialBackgroundFunction.Constant = PRF.PolynomialBackgroundFunction.Constant;
                        TrialPeakRegion.PolynomialBackgroundFunction.Center = PRF.PolynomialBackgroundFunction.Center;
                        TrialPeakRegion.PolynomialBackgroundFunction.Aclivity = PRF.PolynomialBackgroundFunction.Aclivity;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            PRF[n].Sigma = TrialPeakRegion[n].Sigma;
                            PRF[n].Intensity = TrialPeakRegion[n].Intensity;
                            PRF[n].LorentzRatio = TrialPeakRegion[n].LorentzRatio;
                        }
                        PRF.PolynomialBackgroundFunction.Constant = TrialPeakRegion.PolynomialBackgroundFunction.Constant;
                        PRF.PolynomialBackgroundFunction.Center = TrialPeakRegion.PolynomialBackgroundFunction.Center;
                        PRF.PolynomialBackgroundFunction.Aclivity = TrialPeakRegion.PolynomialBackgroundFunction.Aclivity;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }

        private static bool FitAngleLorentzRatioIntensityBackground(Analysis.Peaks.Functions.PeakRegionFunction PRF, double lambda)
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakRegionFunction TrialPeakRegion = PRF.Clone() as Analysis.Peaks.Functions.PeakRegionFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeakRegion.ParameterDeltaVektorAngleIntensityLorentzRatioBackground(Lambda);

                #region Parameter calculations

                for (int n = 0; n < TrialPeakRegion.Count; n++)
                {
                    TrialPeakRegion[n].SetAngleWithConstraints = ParamDelta[(n * 3) + 0];
                    TrialPeakRegion[n].Intensity += ParamDelta[(n * 3) + 1];
                    TrialPeakRegion[n].LorentzRatio += ParamDelta[(n * 3) + 2];
                }


                TrialPeakRegion.PolynomialBackgroundFunction.Constant += ParamDelta[ParamDelta.Count - 3];
                TrialPeakRegion.PolynomialBackgroundFunction.Center += ParamDelta[ParamDelta.Count - 2];
                TrialPeakRegion.PolynomialBackgroundFunction.Aclivity += ParamDelta[ParamDelta.Count - 1];

                #endregion

                if (Math.Abs(TrialPeakRegion.Chi2Function - PRF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeakRegion.Chi2Function > PRF.Chi2Function)
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            TrialPeakRegion[n].Angle = PRF[n].Angle;
                            TrialPeakRegion[n].Intensity = PRF[n].Intensity;
                            TrialPeakRegion[n].LorentzRatio = PRF[n].LorentzRatio;
                        }

                        TrialPeakRegion.PolynomialBackgroundFunction.Constant = PRF.PolynomialBackgroundFunction.Constant;
                        TrialPeakRegion.PolynomialBackgroundFunction.Center = PRF.PolynomialBackgroundFunction.Center;
                        TrialPeakRegion.PolynomialBackgroundFunction.Aclivity = PRF.PolynomialBackgroundFunction.Aclivity;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            PRF[n].Angle = TrialPeakRegion[n].Angle;
                            PRF[n].Intensity = TrialPeakRegion[n].Intensity;
                            PRF[n].LorentzRatio = TrialPeakRegion[n].LorentzRatio;
                        }
                        PRF.PolynomialBackgroundFunction.Constant = TrialPeakRegion.PolynomialBackgroundFunction.Constant;
                        PRF.PolynomialBackgroundFunction.Center = TrialPeakRegion.PolynomialBackgroundFunction.Center;
                        PRF.PolynomialBackgroundFunction.Aclivity = TrialPeakRegion.PolynomialBackgroundFunction.Aclivity;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }

        private static bool FitSigmaLorentzRatioBackground(Analysis.Peaks.Functions.PeakRegionFunction PRF, double lambda)
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakRegionFunction TrialPeakRegion = PRF.Clone() as Analysis.Peaks.Functions.PeakRegionFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeakRegion.ParameterDeltaVektorSigmaLorentzRatioBackground(Lambda);

                #region Parameter calculations

                for (int n = 0; n < TrialPeakRegion.Count; n++)
                {
                    TrialPeakRegion[n].SetSigmaWithConstraints = ParamDelta[(n * 2) + 0];
                    TrialPeakRegion[n].LorentzRatio += ParamDelta[(n * 2) + 1];
                }


                TrialPeakRegion.PolynomialBackgroundFunction.Constant += ParamDelta[ParamDelta.Count - 3];
                TrialPeakRegion.PolynomialBackgroundFunction.Center += ParamDelta[ParamDelta.Count - 2];
                TrialPeakRegion.PolynomialBackgroundFunction.Aclivity += ParamDelta[ParamDelta.Count - 1];

                #endregion

                if (Math.Abs(TrialPeakRegion.Chi2Function - PRF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeakRegion.Chi2Function > PRF.Chi2Function)
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            TrialPeakRegion[n].Sigma = PRF[n].Sigma;
                            TrialPeakRegion[n].LorentzRatio = PRF[n].LorentzRatio;
                        }

                        TrialPeakRegion.PolynomialBackgroundFunction.Constant = PRF.PolynomialBackgroundFunction.Constant;
                        TrialPeakRegion.PolynomialBackgroundFunction.Center = PRF.PolynomialBackgroundFunction.Center;
                        TrialPeakRegion.PolynomialBackgroundFunction.Aclivity = PRF.PolynomialBackgroundFunction.Aclivity;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            PRF[n].Sigma = TrialPeakRegion[n].Sigma;
                            PRF[n].LorentzRatio = TrialPeakRegion[n].LorentzRatio;
                        }
                        PRF.PolynomialBackgroundFunction.Constant = TrialPeakRegion.PolynomialBackgroundFunction.Constant;
                        PRF.PolynomialBackgroundFunction.Center = TrialPeakRegion.PolynomialBackgroundFunction.Center;
                        PRF.PolynomialBackgroundFunction.Aclivity = TrialPeakRegion.PolynomialBackgroundFunction.Aclivity;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }

        private static bool FitAngleLorentzRatioBackground(Analysis.Peaks.Functions.PeakRegionFunction PRF, double lambda)
            
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakRegionFunction TrialPeakRegion = PRF.Clone() as Analysis.Peaks.Functions.PeakRegionFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeakRegion.ParameterDeltaVektorAngleLorentzRatioBackground(Lambda);

                #region Parameter calculations

                for (int n = 0; n < TrialPeakRegion.Count; n++)
                {
                    TrialPeakRegion[n].SetAngleWithConstraints = ParamDelta[(n * 2) + 0];
                    TrialPeakRegion[n].LorentzRatio += ParamDelta[(n * 2) + 1];
                }


                TrialPeakRegion.PolynomialBackgroundFunction.Constant += ParamDelta[ParamDelta.Count - 3];
                TrialPeakRegion.PolynomialBackgroundFunction.Center += ParamDelta[ParamDelta.Count - 2];
                TrialPeakRegion.PolynomialBackgroundFunction.Aclivity += ParamDelta[ParamDelta.Count - 1];

                #endregion

                if (Math.Abs(TrialPeakRegion.Chi2Function - PRF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeakRegion.Chi2Function > PRF.Chi2Function)
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            TrialPeakRegion[n].Angle = PRF[n].Angle;
                            TrialPeakRegion[n].LorentzRatio = PRF[n].LorentzRatio;
                        }

                        TrialPeakRegion.PolynomialBackgroundFunction.Constant = PRF.PolynomialBackgroundFunction.Constant;
                        TrialPeakRegion.PolynomialBackgroundFunction.Center = PRF.PolynomialBackgroundFunction.Center;
                        TrialPeakRegion.PolynomialBackgroundFunction.Aclivity = PRF.PolynomialBackgroundFunction.Aclivity;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            PRF[n].Angle = TrialPeakRegion[n].Angle;
                            PRF[n].LorentzRatio = TrialPeakRegion[n].LorentzRatio;
                        }
                        PRF.PolynomialBackgroundFunction.Constant = TrialPeakRegion.PolynomialBackgroundFunction.Constant;
                        PRF.PolynomialBackgroundFunction.Center = TrialPeakRegion.PolynomialBackgroundFunction.Center;
                        PRF.PolynomialBackgroundFunction.Aclivity = TrialPeakRegion.PolynomialBackgroundFunction.Aclivity;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }
        
        private static bool FitAngleSigmaBackground(Analysis.Peaks.Functions.PeakRegionFunction PRF, double lambda)
            
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakRegionFunction TrialPeakRegion = PRF.Clone() as Analysis.Peaks.Functions.PeakRegionFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeakRegion.ParameterDeltaVektorSigmaAngleBackground(Lambda);

                #region Parameter calculations

                for (int n = 0; n < TrialPeakRegion.Count; n++)
                {
                    TrialPeakRegion[n].SetSigmaWithConstraints = ParamDelta[(n * 2) + 0];
                    TrialPeakRegion[n].SetAngleWithConstraints = ParamDelta[(n * 2) + 1];
                }


                TrialPeakRegion.PolynomialBackgroundFunction.Constant += ParamDelta[ParamDelta.Count - 3];
                TrialPeakRegion.PolynomialBackgroundFunction.Center += ParamDelta[ParamDelta.Count - 2];
                TrialPeakRegion.PolynomialBackgroundFunction.Aclivity += ParamDelta[ParamDelta.Count - 1];

                #endregion

                if (Math.Abs(TrialPeakRegion.Chi2Function - PRF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeakRegion.Chi2Function > PRF.Chi2Function)
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            TrialPeakRegion[n].Sigma = PRF[n].Sigma;
                            TrialPeakRegion[n].Angle = PRF[n].Angle;
                        }

                        TrialPeakRegion.PolynomialBackgroundFunction.Constant = PRF.PolynomialBackgroundFunction.Constant;
                        TrialPeakRegion.PolynomialBackgroundFunction.Center = PRF.PolynomialBackgroundFunction.Center;
                        TrialPeakRegion.PolynomialBackgroundFunction.Aclivity = PRF.PolynomialBackgroundFunction.Aclivity;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            PRF[n].Sigma = TrialPeakRegion[n].Sigma;
                            PRF[n].Angle = TrialPeakRegion[n].Angle;
                        }
                        PRF.PolynomialBackgroundFunction.Constant = TrialPeakRegion.PolynomialBackgroundFunction.Constant;
                        PRF.PolynomialBackgroundFunction.Center = TrialPeakRegion.PolynomialBackgroundFunction.Center;
                        PRF.PolynomialBackgroundFunction.Aclivity = TrialPeakRegion.PolynomialBackgroundFunction.Aclivity;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }

        private static bool FitAngleSigmaLorentzRatio(Analysis.Peaks.Functions.PeakRegionFunction PRF, double lambda)
            
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakRegionFunction TrialPeakRegion = PRF.Clone() as Analysis.Peaks.Functions.PeakRegionFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeakRegion.ParameterDeltaVektorSigmaAngleLorentzRatio(Lambda);

                #region Parameter calculations

                for (int n = 0; n < TrialPeakRegion.Count; n++)
                {
                    TrialPeakRegion[n].SetSigmaWithConstraints = ParamDelta[(n * 3) + 0];
                    TrialPeakRegion[n].SetAngleWithConstraints = ParamDelta[(n * 3) + 1];
                    TrialPeakRegion[n].LorentzRatio += ParamDelta[(n * 3) + 2];
                }

                #endregion

                if (Math.Abs(TrialPeakRegion.Chi2Function - PRF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeakRegion.Chi2Function > PRF.Chi2Function)
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            TrialPeakRegion[n].Sigma = PRF[n].Sigma;
                            TrialPeakRegion[n].Angle = PRF[n].Angle;
                            TrialPeakRegion[n].LorentzRatio = PRF[n].LorentzRatio;
                        }

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            PRF[n].Sigma = TrialPeakRegion[n].Sigma;
                            PRF[n].Angle = TrialPeakRegion[n].Angle;
                            PRF[n].LorentzRatio = TrialPeakRegion[n].LorentzRatio;
                        }

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }
        
        private static bool FitAngleSigmaIntensity(Analysis.Peaks.Functions.PeakRegionFunction PRF, double lambda)
            
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakRegionFunction TrialPeakRegion = PRF.Clone() as Analysis.Peaks.Functions.PeakRegionFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeakRegion.ParameterDeltaVektorSigmaAngleIntensity(Lambda);

                #region Parameter calculations

                for (int n = 0; n < TrialPeakRegion.Count; n++)
                {
                    TrialPeakRegion[n].SetSigmaWithConstraints = ParamDelta[(n * 3) + 0];
                    TrialPeakRegion[n].SetAngleWithConstraints = ParamDelta[(n * 3) + 1];
                    TrialPeakRegion[n].Intensity += ParamDelta[(n * 3) + 2];
                }

                #endregion

                if (Math.Abs(TrialPeakRegion.Chi2Function - PRF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeakRegion.Chi2Function > PRF.Chi2Function)
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            TrialPeakRegion[n].Sigma = PRF[n].Sigma;
                            TrialPeakRegion[n].Angle = PRF[n].Angle;
                            TrialPeakRegion[n].Intensity = PRF[n].Intensity;
                        }

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            PRF[n].Sigma = TrialPeakRegion[n].Sigma;
                            PRF[n].Angle = TrialPeakRegion[n].Angle;
                            PRF[n].Intensity = TrialPeakRegion[n].Intensity;
                        }

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }
        
        private static bool FitSigmaIntensityBackground(Analysis.Peaks.Functions.PeakRegionFunction PRF, double lambda)
            
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakRegionFunction TrialPeakRegion = PRF.Clone() as Analysis.Peaks.Functions.PeakRegionFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeakRegion.ParameterDeltaVektorSigmaIntensityBackground(Lambda);

                #region Parameter calculations

                for (int n = 0; n < TrialPeakRegion.Count; n++)
                {
                    TrialPeakRegion[n].SetSigmaWithConstraints = ParamDelta[(n * 2) + 0];
                    TrialPeakRegion[n].Intensity += ParamDelta[(n * 2) + 1];
                }


                TrialPeakRegion.PolynomialBackgroundFunction.Constant += ParamDelta[ParamDelta.Count - 3];
                TrialPeakRegion.PolynomialBackgroundFunction.Center += ParamDelta[ParamDelta.Count - 2];
                TrialPeakRegion.PolynomialBackgroundFunction.Aclivity += ParamDelta[ParamDelta.Count - 1];

                #endregion

                if (Math.Abs(TrialPeakRegion.Chi2Function - PRF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeakRegion.Chi2Function > PRF.Chi2Function)
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            TrialPeakRegion[n].Sigma = PRF[n].Sigma;
                            TrialPeakRegion[n].Intensity = PRF[n].Intensity;
                        }

                        TrialPeakRegion.PolynomialBackgroundFunction.Constant = PRF.PolynomialBackgroundFunction.Constant;
                        TrialPeakRegion.PolynomialBackgroundFunction.Center = PRF.PolynomialBackgroundFunction.Center;
                        TrialPeakRegion.PolynomialBackgroundFunction.Aclivity = PRF.PolynomialBackgroundFunction.Aclivity;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            PRF[n].Sigma = TrialPeakRegion[n].Sigma;
                            PRF[n].Intensity = TrialPeakRegion[n].Intensity;
                        }
                        PRF.PolynomialBackgroundFunction.Constant = TrialPeakRegion.PolynomialBackgroundFunction.Constant;
                        PRF.PolynomialBackgroundFunction.Center = TrialPeakRegion.PolynomialBackgroundFunction.Center;
                        PRF.PolynomialBackgroundFunction.Aclivity = TrialPeakRegion.PolynomialBackgroundFunction.Aclivity;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }
        
        private static bool FitAngleIntensityBackground(Analysis.Peaks.Functions.PeakRegionFunction PRF, double lambda)
            
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakRegionFunction TrialPeakRegion = PRF.Clone() as Analysis.Peaks.Functions.PeakRegionFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeakRegion.ParameterDeltaVektorAngleIntensityBackground(Lambda);

                #region Parameter calculations

                for (int n = 0; n < TrialPeakRegion.Count; n++)
                {
                    TrialPeakRegion[n].SetAngleWithConstraints = ParamDelta[(n * 2) + 0];
                    TrialPeakRegion[n].Intensity += ParamDelta[(n * 2) + 1];
                }


                TrialPeakRegion.PolynomialBackgroundFunction.Constant += ParamDelta[ParamDelta.Count - 3];
                TrialPeakRegion.PolynomialBackgroundFunction.Center += ParamDelta[ParamDelta.Count - 2];
                TrialPeakRegion.PolynomialBackgroundFunction.Aclivity += ParamDelta[ParamDelta.Count - 1];

                #endregion

                if (Math.Abs(TrialPeakRegion.Chi2Function - PRF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeakRegion.Chi2Function > PRF.Chi2Function)
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            TrialPeakRegion[n].Angle = PRF[n].Angle;
                            TrialPeakRegion[n].Intensity = PRF[n].Intensity;
                        }

                        TrialPeakRegion.PolynomialBackgroundFunction.Constant = PRF.PolynomialBackgroundFunction.Constant;
                        TrialPeakRegion.PolynomialBackgroundFunction.Center = PRF.PolynomialBackgroundFunction.Center;
                        TrialPeakRegion.PolynomialBackgroundFunction.Aclivity = PRF.PolynomialBackgroundFunction.Aclivity;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            PRF[n].Angle = TrialPeakRegion[n].Angle;
                            PRF[n].Intensity = TrialPeakRegion[n].Intensity;
                        }
                        PRF.PolynomialBackgroundFunction.Constant = TrialPeakRegion.PolynomialBackgroundFunction.Constant;
                        PRF.PolynomialBackgroundFunction.Center = TrialPeakRegion.PolynomialBackgroundFunction.Center;
                        PRF.PolynomialBackgroundFunction.Aclivity = TrialPeakRegion.PolynomialBackgroundFunction.Aclivity;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }
        
        private static bool FitAngleBackground(Analysis.Peaks.Functions.PeakRegionFunction PRF, double lambda)
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakRegionFunction TrialPeakRegion = PRF.Clone() as Analysis.Peaks.Functions.PeakRegionFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeakRegion.ParameterDeltaVektorAngleBackground(Lambda);

                #region Parameter calculations

                for (int n = 0; n < TrialPeakRegion.Count; n++)
                {
                    TrialPeakRegion[n].SetAngleWithConstraints = ParamDelta[(n) + 0];
                }


                TrialPeakRegion.PolynomialBackgroundFunction.Constant += ParamDelta[ParamDelta.Count - 3];
                TrialPeakRegion.PolynomialBackgroundFunction.Center += ParamDelta[ParamDelta.Count - 2];
                TrialPeakRegion.PolynomialBackgroundFunction.Aclivity += ParamDelta[ParamDelta.Count - 1];

                #endregion

                if (Math.Abs(TrialPeakRegion.Chi2Function - PRF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeakRegion.Chi2Function > PRF.Chi2Function)
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            TrialPeakRegion[n].Angle = PRF[n].Angle;
                        }

                        TrialPeakRegion.PolynomialBackgroundFunction.Constant = PRF.PolynomialBackgroundFunction.Constant;
                        TrialPeakRegion.PolynomialBackgroundFunction.Center = PRF.PolynomialBackgroundFunction.Center;
                        TrialPeakRegion.PolynomialBackgroundFunction.Aclivity = PRF.PolynomialBackgroundFunction.Aclivity;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            PRF[n].Angle = TrialPeakRegion[n].Angle;
                        }
                        PRF.PolynomialBackgroundFunction.Constant = TrialPeakRegion.PolynomialBackgroundFunction.Constant;
                        PRF.PolynomialBackgroundFunction.Center = TrialPeakRegion.PolynomialBackgroundFunction.Center;
                        PRF.PolynomialBackgroundFunction.Aclivity = TrialPeakRegion.PolynomialBackgroundFunction.Aclivity;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }

        private static bool FitSigmaBackground(Analysis.Peaks.Functions.PeakRegionFunction PRF, double lambda)
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakRegionFunction TrialPeakRegion = PRF.Clone() as Analysis.Peaks.Functions.PeakRegionFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeakRegion.ParameterDeltaVektorSigmaBackground(Lambda);

                #region Parameter calculations

                for (int n = 0; n < TrialPeakRegion.Count; n++)
                {
                    TrialPeakRegion[n].SetSigmaWithConstraints = ParamDelta[(n) + 0];
                }


                TrialPeakRegion.PolynomialBackgroundFunction.Constant += ParamDelta[ParamDelta.Count - 3];
                TrialPeakRegion.PolynomialBackgroundFunction.Center += ParamDelta[ParamDelta.Count - 2];
                TrialPeakRegion.PolynomialBackgroundFunction.Aclivity += ParamDelta[ParamDelta.Count - 1];

                #endregion

                if (Math.Abs(TrialPeakRegion.Chi2Function - PRF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeakRegion.Chi2Function > PRF.Chi2Function)
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            TrialPeakRegion[n].Sigma = PRF[n].Sigma;
                        }

                        TrialPeakRegion.PolynomialBackgroundFunction.Constant = PRF.PolynomialBackgroundFunction.Constant;
                        TrialPeakRegion.PolynomialBackgroundFunction.Center = PRF.PolynomialBackgroundFunction.Center;
                        TrialPeakRegion.PolynomialBackgroundFunction.Aclivity = PRF.PolynomialBackgroundFunction.Aclivity;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            PRF[n].Sigma = TrialPeakRegion[n].Sigma;
                        }
                        PRF.PolynomialBackgroundFunction.Constant = TrialPeakRegion.PolynomialBackgroundFunction.Constant;
                        PRF.PolynomialBackgroundFunction.Center = TrialPeakRegion.PolynomialBackgroundFunction.Center;
                        PRF.PolynomialBackgroundFunction.Aclivity = TrialPeakRegion.PolynomialBackgroundFunction.Aclivity;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }

        private static bool FitAngleSigma(Analysis.Peaks.Functions.PeakRegionFunction PRF, double lambda)
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakRegionFunction TrialPeakRegion = PRF.Clone() as Analysis.Peaks.Functions.PeakRegionFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeakRegion.ParameterDeltaVektorSigmaAngle(Lambda);

                #region Parameter calculations

                for (int n = 0; n < TrialPeakRegion.Count; n++)
                {
                    TrialPeakRegion[n].SetSigmaWithConstraints = ParamDelta[(n * 2) + 0];
                    TrialPeakRegion[n].SetAngleWithConstraints = ParamDelta[(n * 2) + 1];
                }

                #endregion

                if (Math.Abs(TrialPeakRegion.Chi2Function - PRF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeakRegion.Chi2Function > PRF.Chi2Function)
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            TrialPeakRegion[n].Sigma = PRF[n].Sigma;
                            TrialPeakRegion[n].Angle = PRF[n].Angle;
                        }

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            PRF[n].Sigma = TrialPeakRegion[n].Sigma;
                            PRF[n].Angle = TrialPeakRegion[n].Angle;
                        }

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }

        private static bool FitAngleLorentzRatioIntensity(Analysis.Peaks.Functions.PeakRegionFunction PRF, double lambda)
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakRegionFunction TrialPeakRegion = PRF.Clone() as Analysis.Peaks.Functions.PeakRegionFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeakRegion.ParameterDeltaVektorAngleIntensityLorentzRatio(Lambda);

                #region Parameter calculations

                for (int n = 0; n < TrialPeakRegion.Count; n++)
                {
                    TrialPeakRegion[n].SetAngleWithConstraints = ParamDelta[(n * 3) + 0];
                    TrialPeakRegion[n].Intensity += ParamDelta[(n * 3) + 1];
                    TrialPeakRegion[n].LorentzRatio += ParamDelta[(n * 3) + 2];
                }

                #endregion

                if (Math.Abs(TrialPeakRegion.Chi2Function - PRF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeakRegion.Chi2Function > PRF.Chi2Function)
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            TrialPeakRegion[n].Angle = PRF[n].Angle;
                            TrialPeakRegion[n].Intensity = PRF[n].Intensity;
                            TrialPeakRegion[n].LorentzRatio = PRF[n].LorentzRatio;
                        }

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            PRF[n].Angle = TrialPeakRegion[n].Angle;
                            PRF[n].Intensity = TrialPeakRegion[n].Intensity;
                            PRF[n].LorentzRatio = TrialPeakRegion[n].LorentzRatio;
                        }

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }
        
        private static bool FitSigmaLorentzRatioIntensity(Analysis.Peaks.Functions.PeakRegionFunction PRF, double lambda)
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakRegionFunction TrialPeakRegion = PRF.Clone() as Analysis.Peaks.Functions.PeakRegionFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeakRegion.ParameterDeltaVektorSigmaIntensityLorentzRatio(Lambda);

                #region Parameter calculations

                for (int n = 0; n < TrialPeakRegion.Count; n++)
                {
                    TrialPeakRegion[n].SetSigmaWithConstraints = ParamDelta[(n * 3) + 0];
                    TrialPeakRegion[n].Intensity += ParamDelta[(n * 3) + 1];
                    TrialPeakRegion[n].LorentzRatio += ParamDelta[(n * 3) + 2];
                }

                #endregion

                if (Math.Abs(TrialPeakRegion.Chi2Function - PRF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeakRegion.Chi2Function > PRF.Chi2Function)
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            TrialPeakRegion[n].Sigma = PRF[n].Sigma;
                            TrialPeakRegion[n].Intensity = PRF[n].Intensity;
                            TrialPeakRegion[n].LorentzRatio = PRF[n].LorentzRatio;
                        }

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            PRF[n].Sigma = TrialPeakRegion[n].Sigma;
                            PRF[n].Intensity = TrialPeakRegion[n].Intensity;
                            PRF[n].LorentzRatio = TrialPeakRegion[n].LorentzRatio;
                        }

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }

        private static bool FitIntensityLorentzRatioBackground(Analysis.Peaks.Functions.PeakRegionFunction PRF, double lambda)
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakRegionFunction TrialPeakRegion = PRF.Clone() as Analysis.Peaks.Functions.PeakRegionFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeakRegion.ParameterDeltaVektorIntensityLorentzRatioBackground(Lambda);

                #region Parameter calculations

                for (int n = 0; n < TrialPeakRegion.Count; n++)
                {
                    TrialPeakRegion[n].Intensity += ParamDelta[(n * 2) + 0];
                    TrialPeakRegion[n].LorentzRatio += ParamDelta[(n * 2) + 1];
                }


                TrialPeakRegion.PolynomialBackgroundFunction.Constant += ParamDelta[ParamDelta.Count - 3];
                TrialPeakRegion.PolynomialBackgroundFunction.Center += ParamDelta[ParamDelta.Count - 2];
                TrialPeakRegion.PolynomialBackgroundFunction.Aclivity += ParamDelta[ParamDelta.Count - 1];

                #endregion

                if (Math.Abs(TrialPeakRegion.Chi2Function - PRF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeakRegion.Chi2Function > PRF.Chi2Function)
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            TrialPeakRegion[n].Intensity = PRF[n].Intensity;
                            TrialPeakRegion[n].LorentzRatio = PRF[n].LorentzRatio;
                        }

                        TrialPeakRegion.PolynomialBackgroundFunction.Constant = PRF.PolynomialBackgroundFunction.Constant;
                        TrialPeakRegion.PolynomialBackgroundFunction.Center = PRF.PolynomialBackgroundFunction.Center;
                        TrialPeakRegion.PolynomialBackgroundFunction.Aclivity = PRF.PolynomialBackgroundFunction.Aclivity;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            PRF[n].Intensity = TrialPeakRegion[n].Intensity;
                            PRF[n].LorentzRatio = TrialPeakRegion[n].LorentzRatio;
                        }
                        PRF.PolynomialBackgroundFunction.Constant = TrialPeakRegion.PolynomialBackgroundFunction.Constant;
                        PRF.PolynomialBackgroundFunction.Center = TrialPeakRegion.PolynomialBackgroundFunction.Center;
                        PRF.PolynomialBackgroundFunction.Aclivity = TrialPeakRegion.PolynomialBackgroundFunction.Aclivity;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }

        private static bool FitLorentzRatioBackground(Analysis.Peaks.Functions.PeakRegionFunction PRF, double lambda)
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakRegionFunction TrialPeakRegion = PRF.Clone() as Analysis.Peaks.Functions.PeakRegionFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeakRegion.ParameterDeltaVektorLorentzRatioBackground(Lambda);

                #region Parameter calculations

                for (int n = 0; n < TrialPeakRegion.Count; n++)
                {
                    TrialPeakRegion[n].LorentzRatio += ParamDelta[(n) + 0];
                }

                TrialPeakRegion.PolynomialBackgroundFunction.Constant += ParamDelta[ParamDelta.Count - 3];
                TrialPeakRegion.PolynomialBackgroundFunction.Center += ParamDelta[ParamDelta.Count - 2];
                TrialPeakRegion.PolynomialBackgroundFunction.Aclivity += ParamDelta[ParamDelta.Count - 1];

                #endregion

                if (Math.Abs(TrialPeakRegion.Chi2Function - PRF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeakRegion.Chi2Function > PRF.Chi2Function)
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            TrialPeakRegion[n].LorentzRatio = PRF[n].LorentzRatio;
                        }

                        TrialPeakRegion.PolynomialBackgroundFunction.Constant = PRF.PolynomialBackgroundFunction.Constant;
                        TrialPeakRegion.PolynomialBackgroundFunction.Center = PRF.PolynomialBackgroundFunction.Center;
                        TrialPeakRegion.PolynomialBackgroundFunction.Aclivity = PRF.PolynomialBackgroundFunction.Aclivity;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            PRF[n].LorentzRatio = TrialPeakRegion[n].LorentzRatio;
                        }
                        PRF.PolynomialBackgroundFunction.Constant = TrialPeakRegion.PolynomialBackgroundFunction.Constant;
                        PRF.PolynomialBackgroundFunction.Center = TrialPeakRegion.PolynomialBackgroundFunction.Center;
                        PRF.PolynomialBackgroundFunction.Aclivity = TrialPeakRegion.PolynomialBackgroundFunction.Aclivity;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }

        private static bool FitIntensityBackground(Analysis.Peaks.Functions.PeakRegionFunction PRF, double lambda)
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakRegionFunction TrialPeakRegion = PRF.Clone() as Analysis.Peaks.Functions.PeakRegionFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeakRegion.ParameterDeltaVektorIntensityBackground(Lambda);

                #region Parameter calculations

                for (int n = 0; n < TrialPeakRegion.Count; n++)
                {
                    TrialPeakRegion[n].Intensity += ParamDelta[(n) + 0];
                }


                TrialPeakRegion.PolynomialBackgroundFunction.Constant += ParamDelta[ParamDelta.Count - 3];
                TrialPeakRegion.PolynomialBackgroundFunction.Center += ParamDelta[ParamDelta.Count - 2];
                TrialPeakRegion.PolynomialBackgroundFunction.Aclivity += ParamDelta[ParamDelta.Count - 1];

                #endregion

                if (Math.Abs(TrialPeakRegion.Chi2Function - PRF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeakRegion.Chi2Function > PRF.Chi2Function)
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            TrialPeakRegion[n].Intensity = PRF[n].Intensity;
                        }

                        TrialPeakRegion.PolynomialBackgroundFunction.Constant = PRF.PolynomialBackgroundFunction.Constant;
                        TrialPeakRegion.PolynomialBackgroundFunction.Center = PRF.PolynomialBackgroundFunction.Center;
                        TrialPeakRegion.PolynomialBackgroundFunction.Aclivity = PRF.PolynomialBackgroundFunction.Aclivity;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            PRF[n].Intensity = TrialPeakRegion[n].Intensity;
                        }
                        PRF.PolynomialBackgroundFunction.Constant = TrialPeakRegion.PolynomialBackgroundFunction.Constant;
                        PRF.PolynomialBackgroundFunction.Center = TrialPeakRegion.PolynomialBackgroundFunction.Center;
                        PRF.PolynomialBackgroundFunction.Aclivity = TrialPeakRegion.PolynomialBackgroundFunction.Aclivity;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }

        private static bool FitAngleIntensity(Analysis.Peaks.Functions.PeakRegionFunction PRF, double lambda)
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakRegionFunction TrialPeakRegion = PRF.Clone() as Analysis.Peaks.Functions.PeakRegionFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeakRegion.ParameterDeltaVektorAngleIntensity(Lambda);

                #region Parameter calculations

                for (int n = 0; n < TrialPeakRegion.Count; n++)
                {
                    TrialPeakRegion[n].SetAngleWithConstraints = ParamDelta[(n * 2) + 0];
                    TrialPeakRegion[n].Intensity += ParamDelta[(n * 2) + 1];
                }

                #endregion

                if (Math.Abs(TrialPeakRegion.Chi2Function - PRF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeakRegion.Chi2Function > PRF.Chi2Function)
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            TrialPeakRegion[n].Angle = PRF[n].Angle;
                            TrialPeakRegion[n].Intensity = PRF[n].Intensity;
                        }

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            PRF[n].Angle = TrialPeakRegion[n].Angle;
                            PRF[n].Intensity = TrialPeakRegion[n].Intensity;
                        }

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }

        private static bool FitSigmaIntensity(Analysis.Peaks.Functions.PeakRegionFunction PRF, double lambda)
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakRegionFunction TrialPeakRegion = PRF.Clone() as Analysis.Peaks.Functions.PeakRegionFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeakRegion.ParameterDeltaVektorSigmaIntensity(Lambda);

                #region Parameter calculations

                for (int n = 0; n < TrialPeakRegion.Count; n++)
                {
                    TrialPeakRegion[n].SetSigmaWithConstraints = ParamDelta[(n * 2) + 0];
                    TrialPeakRegion[n].Intensity += ParamDelta[(n * 2) + 1];
                }

                #endregion

                if (Math.Abs(TrialPeakRegion.Chi2Function - PRF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeakRegion.Chi2Function > PRF.Chi2Function)
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            TrialPeakRegion[n].Sigma = PRF[n].Sigma;
                            TrialPeakRegion[n].Intensity = PRF[n].Intensity;
                        }

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            PRF[n].Sigma = TrialPeakRegion[n].Sigma;
                            PRF[n].Intensity = TrialPeakRegion[n].Intensity;
                        }

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }

        private static bool FitSigmaLorentzRatio(Analysis.Peaks.Functions.PeakRegionFunction PRF, double lambda)
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakRegionFunction TrialPeakRegion = PRF.Clone() as Analysis.Peaks.Functions.PeakRegionFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeakRegion.ParameterDeltaVektorSigmaLorentzRatio(Lambda);

                #region Parameter calculations

                for (int n = 0; n < TrialPeakRegion.Count; n++)
                {
                    TrialPeakRegion[n].SetSigmaWithConstraints = ParamDelta[(n * 2) + 0];
                    TrialPeakRegion[n].LorentzRatio += ParamDelta[(n * 2) + 1];
                }

                #endregion

                if (Math.Abs(TrialPeakRegion.Chi2Function - PRF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeakRegion.Chi2Function > PRF.Chi2Function)
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            TrialPeakRegion[n].Sigma = PRF[n].Sigma;
                            TrialPeakRegion[n].LorentzRatio = PRF[n].LorentzRatio;
                        }

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            PRF[n].Sigma = TrialPeakRegion[n].Sigma;
                            PRF[n].LorentzRatio = TrialPeakRegion[n].LorentzRatio;
                        }

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }

        private static bool FitIntensityLorentzRatio(Analysis.Peaks.Functions.PeakRegionFunction PRF, double lambda)
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakRegionFunction TrialPeakRegion = PRF.Clone() as Analysis.Peaks.Functions.PeakRegionFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeakRegion.ParameterDeltaVektorIntensityLorentzRatio(Lambda);

                #region Parameter calculations

                for (int n = 0; n < TrialPeakRegion.Count; n++)
                {
                    TrialPeakRegion[n].Intensity += ParamDelta[(n * 2) + 0];
                    TrialPeakRegion[n].LorentzRatio += ParamDelta[(n * 2) + 1];
                }

                #endregion

                if (Math.Abs(TrialPeakRegion.Chi2Function - PRF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeakRegion.Chi2Function > PRF.Chi2Function)
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            TrialPeakRegion[n].Intensity = PRF[n].Intensity;
                            TrialPeakRegion[n].LorentzRatio = PRF[n].LorentzRatio;
                        }

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            PRF[n].Intensity = TrialPeakRegion[n].Intensity;
                            PRF[n].LorentzRatio = TrialPeakRegion[n].LorentzRatio;
                        }

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }

        private static bool FitAngleLorentzRatio(Analysis.Peaks.Functions.PeakRegionFunction PRF, double lambda)
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakRegionFunction TrialPeakRegion = PRF.Clone() as Analysis.Peaks.Functions.PeakRegionFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeakRegion.ParameterDeltaVektorAngleLorentzRatio(Lambda);

                #region Parameter calculations

                for (int n = 0; n < TrialPeakRegion.Count; n++)
                {
                    TrialPeakRegion[n].SetAngleWithConstraints = ParamDelta[(n * 2) + 0];
                    TrialPeakRegion[n].LorentzRatio += ParamDelta[(n * 2) + 1];
                }

                #endregion

                if (Math.Abs(TrialPeakRegion.Chi2Function - PRF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeakRegion.Chi2Function > PRF.Chi2Function)
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            TrialPeakRegion[n].Angle = PRF[n].Angle;
                            TrialPeakRegion[n].LorentzRatio = PRF[n].LorentzRatio;
                        }

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            PRF[n].Angle = TrialPeakRegion[n].Angle;
                            PRF[n].LorentzRatio = TrialPeakRegion[n].LorentzRatio;
                        }

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }

        private static bool FitIntensity(Analysis.Peaks.Functions.PeakRegionFunction PRF, double lambda)
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakRegionFunction TrialPeakRegion = PRF.Clone() as Analysis.Peaks.Functions.PeakRegionFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeakRegion.ParameterDeltaVektorIntensity(Lambda);

                #region Parameter calculations

                for (int n = 0; n < TrialPeakRegion.Count; n++)
                {
                    TrialPeakRegion[n].Intensity += ParamDelta[(n) + 0];
                }

                #endregion

                if (Math.Abs(TrialPeakRegion.Chi2Function - PRF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeakRegion.Chi2Function > PRF.Chi2Function)
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            TrialPeakRegion[n].Intensity = PRF[n].Intensity;
                        }

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            PRF[n].Intensity = TrialPeakRegion[n].Intensity;
                        }

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }

        private static bool FitAngle(Analysis.Peaks.Functions.PeakRegionFunction PRF, double lambda)
        
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakRegionFunction TrialPeakRegion = PRF.Clone() as Analysis.Peaks.Functions.PeakRegionFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeakRegion.ParameterDeltaVektorAngle(Lambda);

                #region Parameter calculations

                for (int n = 0; n < TrialPeakRegion.Count; n++)
                {
                    TrialPeakRegion[n].SetAngleWithConstraints = ParamDelta[(n) + 0];
                }

                #endregion

                if (Math.Abs(TrialPeakRegion.Chi2Function - PRF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeakRegion.Chi2Function > PRF.Chi2Function)
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            TrialPeakRegion[n].Angle = PRF[n].Angle;
                        }

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            PRF[n].Angle = TrialPeakRegion[n].Angle;
                        }

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }

        private static bool FitSigma(Analysis.Peaks.Functions.PeakRegionFunction PRF, double lambda)
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakRegionFunction TrialPeakRegion = PRF.Clone() as Analysis.Peaks.Functions.PeakRegionFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeakRegion.ParameterDeltaVektorSigma(Lambda);

                #region Parameter calculations

                for (int n = 0; n < TrialPeakRegion.Count; n++)
                {
                    TrialPeakRegion[n].SetSigmaWithConstraints = ParamDelta[(n) + 0];
                }

                #endregion

                if (Math.Abs(TrialPeakRegion.Chi2Function - PRF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeakRegion.Chi2Function > PRF.Chi2Function)
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            TrialPeakRegion[n].Sigma = PRF[n].Sigma;
                        }

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            PRF[n].Sigma = TrialPeakRegion[n].Sigma;
                        }

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }

        private static bool FitLorentzRatio(Analysis.Peaks.Functions.PeakRegionFunction PRF, double lambda)
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakRegionFunction TrialPeakRegion = PRF.Clone() as Analysis.Peaks.Functions.PeakRegionFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeakRegion.ParameterDeltaVektorLorentzRatio(Lambda);

                #region Parameter calculations

                for (int n = 0; n < TrialPeakRegion.Count; n++)
                {
                    TrialPeakRegion[n].LorentzRatio += ParamDelta[(n) + 0];
                }

                #endregion

                if (Math.Abs(TrialPeakRegion.Chi2Function - PRF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeakRegion.Chi2Function > PRF.Chi2Function)
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            TrialPeakRegion[n].LorentzRatio = PRF[n].LorentzRatio;
                        }

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        for (int n = 0; n < TrialPeakRegion.Count; n++)
                        {
                            PRF[n].LorentzRatio = TrialPeakRegion[n].LorentzRatio;
                        }

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }

        private static bool FitBackground(Analysis.Peaks.Functions.PeakRegionFunction PRF, double lambda)
        
        {
            bool Converged = false;
            double Lambda = lambda;

            Analysis.Peaks.Functions.PeakRegionFunction TrialPeakRegion = PRF.Clone() as Analysis.Peaks.Functions.PeakRegionFunction;

            for (int TotalTrials = 0; TotalTrials < CalScec.Properties.Settings.Default.MaxFittingTrials; TotalTrials++)
            {
                MathNet.Numerics.LinearAlgebra.Vector<double> ParamDelta = TrialPeakRegion.ParameterDeltaVektorBackground(Lambda);

                #region Parameter calculations

                TrialPeakRegion.PolynomialBackgroundFunction.Constant += ParamDelta[0];
                TrialPeakRegion.PolynomialBackgroundFunction.Center += ParamDelta[1];
                TrialPeakRegion.PolynomialBackgroundFunction.Aclivity += ParamDelta[2];

                #endregion

                if (Math.Abs(TrialPeakRegion.Chi2Function - PRF.Chi2Function) > CalScec.Properties.Settings.Default.FunctionFittingAcuraccy)
                {
                    if (TrialPeakRegion.Chi2Function > PRF.Chi2Function)
                    {
                        TrialPeakRegion.PolynomialBackgroundFunction.Constant = PRF.PolynomialBackgroundFunction.Constant;
                        TrialPeakRegion.PolynomialBackgroundFunction.Center = PRF.PolynomialBackgroundFunction.Center;
                        TrialPeakRegion.PolynomialBackgroundFunction.Aclivity = PRF.PolynomialBackgroundFunction.Aclivity;

                        Lambda *= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                    else
                    {
                        PRF.PolynomialBackgroundFunction.Constant = TrialPeakRegion.PolynomialBackgroundFunction.Constant;
                        PRF.PolynomialBackgroundFunction.Center = TrialPeakRegion.PolynomialBackgroundFunction.Center;
                        PRF.PolynomialBackgroundFunction.Aclivity = TrialPeakRegion.PolynomialBackgroundFunction.Aclivity;

                        Lambda /= CalScec.Properties.Settings.Default.FittingLambdaMulti;
                    }
                }
                else
                {
                    if (TotalTrials > 3)
                    {
                        return true;
                    }
                }
            }

            return Converged;
        }

        #endregion
    }
}
