using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalScec.DataManagment.CrystalData
{
    public class ReflexCondition
    {
        private string _spaceGroupSymbol;
        public string SpaceGroupSymbol
        {
            get
            {
                return this._spaceGroupSymbol;
            }
        }

        private int _spaceGroupId;
        public int SpaceGroupId
        {
            get
            {
                return this._spaceGroupId;
            }
        }

        private string _hkl;
        private string _hko;
        private string _hol;
        private string _okl;
        private string _hoo;
        private string _oko;
        private string _ool;
        private string _hhl;
        private string _hkk;
        private string _hkh;
        private string _hhh;
        private string _hho;
        private string _hoh;
        private string _okk;

        public ReflexCondition(string spaceGroupSymbol)
        {
            this._spaceGroupSymbol = "Not found";
            this._spaceGroupId = -1;
            this._hkl = "-";
            this._hko = "-";
            this._hol = "-";
            this._okl = "-";
            this._hoo = "-";
            this._oko = "-";
            this._ool = "-";
            this._hhl = "-";
            this._hkk = "-";
            this._hkh = "-";
            this._hhh = "-";
            this._hho = "-";
            this._hoh = "-";
            this._okk = "-";

            string RessourcePath = System.IO.Path.Combine(Environment.CurrentDirectory, @"Res\ReflectionConditions\SpaceGroupsHKL.xml");
            using (System.IO.StreamReader XMLResStream = new System.IO.StreamReader(RessourcePath))
            {
                using (System.Xml.XmlTextReader ReflexReader = new System.Xml.XmlTextReader(XMLResStream))
                {
                    bool ReflexConditionFound = false;

                    while (ReflexReader.Read() && !ReflexConditionFound)
                    {
                        switch (ReflexReader.NodeType)
                        {
                            case System.Xml.XmlNodeType.Element:
                                if (ReflexReader.Name == "HKLCondition")
                                {
                                    bool ConditionReading = true;
                                    int ParamsSet = 0;

                                    while (ConditionReading)
                                    {
                                        ReflexReader.Read();

                                        if(ReflexReader.NodeType == System.Xml.XmlNodeType.Element)
                                        {
                                            switch (ReflexReader.Name)
                                            {
                                                case "SpaceGroup":
                                                    ReflexReader.Read();
                                                    if (ReflexReader.NodeType == System.Xml.XmlNodeType.Text)
                                                    {
                                                        this._spaceGroupSymbol = ReflexReader.Value;
                                                    }
                                                    ParamsSet++;
                                                    break;
                                                case "SpaceGroupId":
                                                    ReflexReader.Read();
                                                    if (ReflexReader.NodeType == System.Xml.XmlNodeType.Text)
                                                    {
                                                        this._spaceGroupId = Convert.ToInt32(ReflexReader.Value);
                                                    }
                                                    ParamsSet++;
                                                    break;
                                                case "hkl":
                                                    ReflexReader.Read();
                                                    if (ReflexReader.NodeType == System.Xml.XmlNodeType.Text)
                                                    {
                                                        this._hkl = ReflexReader.Value;
                                                    }
                                                    ParamsSet++;
                                                    break;
                                                case "hko":
                                                    ReflexReader.Read();
                                                    if (ReflexReader.NodeType == System.Xml.XmlNodeType.Text)
                                                    {
                                                        this._hko = ReflexReader.Value;
                                                    }
                                                    ParamsSet++;
                                                    break;
                                                case "hol":
                                                    ReflexReader.Read();
                                                    if (ReflexReader.NodeType == System.Xml.XmlNodeType.Text)
                                                    {
                                                        this._hol = ReflexReader.Value;
                                                    }
                                                    ParamsSet++;
                                                    break;
                                                case "okl":
                                                    ReflexReader.Read();
                                                    if (ReflexReader.NodeType == System.Xml.XmlNodeType.Text)
                                                    {
                                                        this._okl = ReflexReader.Value;
                                                    }
                                                    ParamsSet++;
                                                    break;
                                                case "hoo":
                                                    ReflexReader.Read();
                                                    if (ReflexReader.NodeType == System.Xml.XmlNodeType.Text)
                                                    {
                                                        this._hoo = ReflexReader.Value;
                                                    }
                                                    ParamsSet++;
                                                    break;
                                                case "oko":
                                                    ReflexReader.Read();
                                                    if (ReflexReader.NodeType == System.Xml.XmlNodeType.Text)
                                                    {
                                                        this._oko = ReflexReader.Value;
                                                    }
                                                    ParamsSet++;
                                                    break;
                                                case "ool":
                                                    ReflexReader.Read();
                                                    if (ReflexReader.NodeType == System.Xml.XmlNodeType.Text)
                                                    {
                                                        this._ool = ReflexReader.Value;
                                                    }
                                                    ParamsSet++;
                                                    break;
                                                case "hhl":
                                                    ReflexReader.Read();
                                                    if (ReflexReader.NodeType == System.Xml.XmlNodeType.Text)
                                                    {
                                                        this._hhl = ReflexReader.Value;
                                                    }
                                                    ParamsSet++;
                                                    break;
                                                case "hkk":
                                                    ReflexReader.Read();
                                                    if (ReflexReader.NodeType == System.Xml.XmlNodeType.Text)
                                                    {
                                                        this._hkk = ReflexReader.Value;
                                                    }
                                                    ParamsSet++;
                                                    break;
                                                case "hkh":
                                                    ReflexReader.Read();
                                                    if (ReflexReader.NodeType == System.Xml.XmlNodeType.Text)
                                                    {
                                                        this._hkh = ReflexReader.Value;
                                                    }
                                                    ParamsSet++;
                                                    break;
                                                case "hhh":
                                                    ReflexReader.Read();
                                                    if (ReflexReader.NodeType == System.Xml.XmlNodeType.Text)
                                                    {
                                                        this._hhh = ReflexReader.Value;
                                                    }
                                                    ParamsSet++;
                                                    break;
                                                case "hho":
                                                    ReflexReader.Read();
                                                    if (ReflexReader.NodeType == System.Xml.XmlNodeType.Text)
                                                    {
                                                        this._hho = ReflexReader.Value;
                                                    }
                                                    ParamsSet++;
                                                    break;
                                                case "hoh":
                                                    ReflexReader.Read();
                                                    if (ReflexReader.NodeType == System.Xml.XmlNodeType.Text)
                                                    {
                                                        this._hoh = ReflexReader.Value;
                                                    }
                                                    ParamsSet++;
                                                    break;
                                                case "okk":
                                                    ReflexReader.Read();
                                                    if (ReflexReader.NodeType == System.Xml.XmlNodeType.Text)
                                                    {
                                                        this._okk = ReflexReader.Value;
                                                    }
                                                    ParamsSet++;
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }

                                        if (this._spaceGroupSymbol != "Not found" && this._spaceGroupSymbol != spaceGroupSymbol)
                                        {
                                            this._spaceGroupSymbol = "Not found";
                                            this._spaceGroupId = -1;
                                            this._hkl = "-";
                                            this._hko = "-";
                                            this._hol = "-";
                                            this._okl = "-";
                                            this._hoo = "-";
                                            this._oko = "-";
                                            this._ool = "-";
                                            this._hhl = "-";
                                            this._hkk = "-";
                                            this._hkh = "-";
                                            this._hhh = "-";
                                            this._hho = "-";
                                            this._hoh = "-";
                                            this._okk = "-";

                                            //ConditionReading = false;
                                        }

                                        if (ParamsSet == 16)
                                        {
                                            ReflexConditionFound = true;
                                            ConditionReading = false;
                                        }
                                    }

                                    //if(!ConditionReading)
                                    //{
                                    //    ReflexConditionFound = true;
                                    //}
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        public ReflexCondition(int spaceGroupId)
        {
            this._spaceGroupSymbol = "Not found";
            this._spaceGroupId = -1;
            this._hkl = "-";
            this._hko = "-";
            this._hol = "-";
            this._okl = "-";
            this._hoo = "-";
            this._oko = "-";
            this._ool = "-";
            this._hhl = "-";
            this._hkk = "-";
            this._hkh = "-";
            this._hhh = "-";
            this._hho = "-";
            this._hoh = "-";
            this._okk = "-";

            string RessourcePath = System.IO.Path.Combine(Environment.CurrentDirectory, @"Res\ReflectionConditions\SpaceGroupsHKL.xml");
            using (System.IO.StreamReader XMLResStream = new System.IO.StreamReader(RessourcePath))
            {
                using (System.Xml.XmlTextReader ReflexReader = new System.Xml.XmlTextReader(XMLResStream))
                {
                    bool ReflexConditionFound = false;

                    while (ReflexReader.Read())
                    {
                        switch (ReflexReader.NodeType)
                        {
                            case System.Xml.XmlNodeType.Element:
                                if (ReflexReader.Name == "HKLCondition")
                                {
                                    bool ConditionReading = true;
                                    int ParamsSet = 0;

                                    while (ConditionReading)
                                    {
                                        ReflexReader.Read();

                                        if (ReflexReader.NodeType == System.Xml.XmlNodeType.Element)
                                        {
                                            switch (ReflexReader.Name)
                                            {
                                                case "SpaceGroup":
                                                    ReflexReader.Read();
                                                    if (ReflexReader.NodeType == System.Xml.XmlNodeType.Text)
                                                    {
                                                        this._spaceGroupSymbol = ReflexReader.Value;
                                                    }
                                                    ParamsSet++;
                                                    break;
                                                case "SpaceGroupId":
                                                    ReflexReader.Read();
                                                    if (ReflexReader.NodeType == System.Xml.XmlNodeType.Text)
                                                    {
                                                        this._spaceGroupId = Convert.ToInt32(ReflexReader.Value);
                                                    }
                                                    ParamsSet++;
                                                    break;
                                                case "hkl":
                                                    ReflexReader.Read();
                                                    if (ReflexReader.NodeType == System.Xml.XmlNodeType.Text)
                                                    {
                                                        this._hkl = ReflexReader.Value;
                                                    }
                                                    ParamsSet++;
                                                    break;
                                                case "hko":
                                                    ReflexReader.Read();
                                                    if (ReflexReader.NodeType == System.Xml.XmlNodeType.Text)
                                                    {
                                                        this._hko = ReflexReader.Value;
                                                    }
                                                    ParamsSet++;
                                                    break;
                                                case "hol":
                                                    ReflexReader.Read();
                                                    if (ReflexReader.NodeType == System.Xml.XmlNodeType.Text)
                                                    {
                                                        this._hol = ReflexReader.Value;
                                                    }
                                                    ParamsSet++;
                                                    break;
                                                case "okl":
                                                    ReflexReader.Read();
                                                    if (ReflexReader.NodeType == System.Xml.XmlNodeType.Text)
                                                    {
                                                        this._okl = ReflexReader.Value;
                                                    }
                                                    ParamsSet++;
                                                    break;
                                                case "hoo":
                                                    ReflexReader.Read();
                                                    if (ReflexReader.NodeType == System.Xml.XmlNodeType.Text)
                                                    {
                                                        this._hoo = ReflexReader.Value;
                                                    }
                                                    ParamsSet++;
                                                    break;
                                                case "oko":
                                                    ReflexReader.Read();
                                                    if (ReflexReader.NodeType == System.Xml.XmlNodeType.Text)
                                                    {
                                                        this._oko = ReflexReader.Value;
                                                    }
                                                    ParamsSet++;
                                                    break;
                                                case "ool":
                                                    ReflexReader.Read();
                                                    if (ReflexReader.NodeType == System.Xml.XmlNodeType.Text)
                                                    {
                                                        this._ool = ReflexReader.Value;
                                                    }
                                                    ParamsSet++;
                                                    break;
                                                case "hhl":
                                                    ReflexReader.Read();
                                                    if (ReflexReader.NodeType == System.Xml.XmlNodeType.Text)
                                                    {
                                                        this._hhl = ReflexReader.Value;
                                                    }
                                                    ParamsSet++;
                                                    break;
                                                case "hkk":
                                                    ReflexReader.Read();
                                                    if (ReflexReader.NodeType == System.Xml.XmlNodeType.Text)
                                                    {
                                                        this._hkk = ReflexReader.Value;
                                                    }
                                                    ParamsSet++;
                                                    break;
                                                case "hkh":
                                                    ReflexReader.Read();
                                                    if (ReflexReader.NodeType == System.Xml.XmlNodeType.Text)
                                                    {
                                                        this._hkh = ReflexReader.Value;
                                                    }
                                                    ParamsSet++;
                                                    break;
                                                case "hhh":
                                                    ReflexReader.Read();
                                                    if (ReflexReader.NodeType == System.Xml.XmlNodeType.Text)
                                                    {
                                                        this._hhh = ReflexReader.Value;
                                                    }
                                                    ParamsSet++;
                                                    break;
                                                case "hho":
                                                    ReflexReader.Read();
                                                    if (ReflexReader.NodeType == System.Xml.XmlNodeType.Text)
                                                    {
                                                        this._hho = ReflexReader.Value;
                                                    }
                                                    ParamsSet++;
                                                    break;
                                                case "hoh":
                                                    ReflexReader.Read();
                                                    if (ReflexReader.NodeType == System.Xml.XmlNodeType.Text)
                                                    {
                                                        this._hoh = ReflexReader.Value;
                                                    }
                                                    ParamsSet++;
                                                    break;
                                                case "okk":
                                                    ReflexReader.Read();
                                                    if (ReflexReader.NodeType == System.Xml.XmlNodeType.Text)
                                                    {
                                                        this._okk = ReflexReader.Value;
                                                    }
                                                    ParamsSet++;
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }

                                        if (this._spaceGroupId != -1 && this._spaceGroupId != spaceGroupId)
                                        {
                                            this._spaceGroupSymbol = "Not found";
                                            this._spaceGroupId = -1;
                                            this._hkl = "-";
                                            this._hko = "-";
                                            this._hol = "-";
                                            this._okl = "-";
                                            this._hoo = "-";
                                            this._oko = "-";
                                            this._ool = "-";
                                            this._hhl = "-";
                                            this._hkk = "-";
                                            this._hkh = "-";
                                            this._hhh = "-";
                                            this._hho = "-";
                                            this._hoh = "-";
                                            this._okk = "-";

                                            ConditionReading = false;
                                        }

                                        if (ParamsSet == 16)
                                        {
                                            ReflexConditionFound = true;
                                            ConditionReading = false;
                                        }
                                    }

                                    //if (!ConditionReading)
                                    //{
                                    //    ReflexConditionFound = true;
                                    //}
                                }
                                break;
                            default:
                                break;
                        }

                        if (ReflexConditionFound)
                        {
                            break;
                        }
                    }
                }
            }
        }

        public bool CheckHKLReflex(HKLReflex reflex)
        {
            if(reflex.H != 0)
            {
                if (reflex.K != 0)
                {
                    if (reflex.L != 0)
                    {
                        if(reflex.H != reflex.K)
                        {
                            if(reflex.H != reflex.L)
                            {
                                if(reflex.K != reflex.L)
                                {
                                    if (this._hkl != "-")
                                    {
                                        string[] SplittedCondition = this._hkl.Split(',');

                                        if (SplittedCondition.Count() == 1)
                                        {
                                            string[] ForCheck = this._hkk.Split('=');
                                            if(ForCheck[0].Length == 1)
                                            {
                                                return CheckSingle(ForCheck[0][0], ForCheck[1], reflex);
                                            }
                                            else if(ForCheck[0].Length == 3)
                                            {
                                                return CheckDouble(ForCheck[0], ForCheck[1], reflex);
                                            }
                                            else
                                            {
                                                return CheckTriple(ForCheck[0], ForCheck[1], reflex);
                                            }
                                        }
                                        else
                                        {
                                            string[] ConditionStringAr = SplittedCondition[SplittedCondition.Length - 1].Split('=');

                                            for (int n = 0; n < SplittedCondition.Length - 2; n++)
                                            {
                                                if (SplittedCondition[n].Length == 1)
                                                {
                                                    if(!CheckSingle(SplittedCondition[n][0], ConditionStringAr[1], reflex))
                                                    {
                                                        return false;
                                                    }
                                                }
                                                else if (SplittedCondition[n].Length == 3)
                                                {
                                                    if (!CheckDouble(SplittedCondition[n], ConditionStringAr[1], reflex))
                                                    {
                                                        return false;
                                                    }
                                                }
                                                else
                                                {
                                                    if (!CheckDouble(SplittedCondition[n], ConditionStringAr[1], reflex))
                                                    {
                                                        return false;
                                                    }
                                                }
                                            }

                                            if (ConditionStringAr[0].Length == 1)
                                            {
                                                return CheckDouble(ConditionStringAr[0], ConditionStringAr[1], reflex);
                                            }
                                            else if (ConditionStringAr[0].Length == 3)
                                            {
                                                return CheckDouble(ConditionStringAr[0], ConditionStringAr[1], reflex);
                                            }
                                            else
                                            {
                                                return CheckDouble(ConditionStringAr[0], ConditionStringAr[1], reflex);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        return true;
                                    }
                                }
                                else
                                {
                                    if (this._hkk != "-")
                                    {
                                        string[] SplittedCondition = this._hkk.Split(',');

                                        if (SplittedCondition.Length == 1)
                                        {
                                            string[] ForCheck = this._hkk.Split('=');

                                            if (ForCheck[0].Length == 1)
                                            {
                                                return CheckSingle(ForCheck[0][0], ForCheck[1], reflex);
                                            }
                                            else
                                            {
                                                return CheckDouble(ForCheck[0], ForCheck[1], reflex);
                                            }
                                        }
                                        else
                                        {
                                            string[] ConditionStringAr = SplittedCondition[SplittedCondition.Length - 1].Split('=');

                                            for (int n = 0; n < SplittedCondition.Length - 2; n++)
                                            {
                                                if (SplittedCondition[n].Length == 1)
                                                {
                                                    if (!CheckSingle(SplittedCondition[n][0], ConditionStringAr[1], reflex))
                                                    {
                                                        return false;
                                                    }
                                                }
                                                else
                                                {
                                                    if (!CheckDouble(SplittedCondition[n], ConditionStringAr[1], reflex))
                                                    {
                                                        return false;
                                                    }
                                                }
                                            }

                                            if (ConditionStringAr[0].Length == 1)
                                            {
                                                return CheckSingle(ConditionStringAr[0][0], ConditionStringAr[1], reflex);
                                            }
                                            else
                                            {
                                                return CheckDouble(ConditionStringAr[0], ConditionStringAr[1], reflex);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        return true;
                                    }
                                }
                            }
                            else
                            {
                                if (this._hkh != "-")
                                {
                                    string[] SplittedCondition = this._hkh.Split(',');

                                    if (SplittedCondition.Length == 1)
                                    {
                                        string[] ForCheck = this._hkh.Split('=');

                                        if (ForCheck[0].Length == 1)
                                        {
                                            return CheckSingle(ForCheck[0][0], ForCheck[1], reflex);
                                        }
                                        else
                                        {
                                            return CheckDouble(ForCheck[0], ForCheck[1], reflex);
                                        }
                                    }
                                    else
                                    {
                                        string[] ConditionStringAr = SplittedCondition[SplittedCondition.Length - 1].Split('=');

                                        for (int n = 0; n < SplittedCondition.Length - 2; n++)
                                        {
                                            if (SplittedCondition[n].Length == 1)
                                            {
                                                if (!CheckSingle(SplittedCondition[n][0], ConditionStringAr[1], reflex))
                                                {
                                                    return false;
                                                }
                                            }
                                            else
                                            {
                                                if (!CheckDouble(SplittedCondition[n], ConditionStringAr[1], reflex))
                                                {
                                                    return false;
                                                }
                                            }
                                        }

                                        if (ConditionStringAr[0].Length == 1)
                                        {
                                            return CheckSingle(ConditionStringAr[0][0], ConditionStringAr[1], reflex);
                                        }
                                        else
                                        {
                                            return CheckDouble(ConditionStringAr[0], ConditionStringAr[1], reflex);
                                        }
                                    }
                                }
                                else
                                {
                                    return true;
                                }
                            }
                        }
                        else
                        {
                            if(reflex.H != reflex.L)
                            {
                                if (this._hhl != "-")
                                {
                                    string[] SplittedCondition = this._hhl.Split(',');

                                    if (SplittedCondition.Length == 1)
                                    {
                                        string[] ForCheck = this._hhl.Split('=');

                                        if (ForCheck[0].Length == 1)
                                        {
                                            return CheckSingle(ForCheck[0][0], ForCheck[1], reflex);
                                        }
                                        else
                                        {
                                            return CheckDouble(ForCheck[0], ForCheck[1], reflex);
                                        }
                                    }
                                    else
                                    {
                                        string[] ConditionStringAr = SplittedCondition[SplittedCondition.Length - 1].Split('=');

                                        for (int n = 0; n < SplittedCondition.Length - 2; n++)
                                        {
                                            if (SplittedCondition[n].Length == 1)
                                            {
                                                if (!CheckSingle(SplittedCondition[n][0], ConditionStringAr[1], reflex))
                                                {
                                                    return false;
                                                }
                                            }
                                            else
                                            {
                                                if (!CheckDouble(SplittedCondition[n], ConditionStringAr[1], reflex))
                                                {
                                                    return false;
                                                }
                                            }
                                        }

                                        if (ConditionStringAr[0].Length == 1)
                                        {
                                            return CheckSingle(ConditionStringAr[0][0], ConditionStringAr[1], reflex);
                                        }
                                        else
                                        {
                                            return CheckDouble(ConditionStringAr[0], ConditionStringAr[1], reflex);
                                        }
                                    }
                                }
                                else
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                if (this._hhh != "-")
                                {
                                    string[] ForCheck = this._hhh.Split('=');

                                    return CheckSingle(ForCheck[0][0], ForCheck[1], reflex);
                                }
                                else
                                {
                                    return true;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (reflex.H != reflex.K)
                        {
                            if (this._hko != "-")
                            {
                                string[] SplittedCondition = this._hko.Split(',');

                                if (SplittedCondition.Length == 1)
                                {
                                    string[] ForCheck = this._hko.Split('=');

                                    if (ForCheck[0].Length == 1)
                                    {
                                        return CheckSingle(ForCheck[0][0], ForCheck[1], reflex);
                                    }
                                    else
                                    {
                                        return CheckDouble(ForCheck[0], ForCheck[1], reflex);
                                    }
                                }
                                else
                                {
                                    string[] ConditionStringAr = SplittedCondition[SplittedCondition.Length - 1].Split('=');

                                    for (int n = 0; n < SplittedCondition.Length; n++)
                                    {
                                        if (SplittedCondition[n].Length == 1)
                                        {
                                            if (!CheckSingle(SplittedCondition[n][0], ConditionStringAr[1], reflex))
                                            {
                                                return false;
                                            }
                                        }
                                        else
                                        {
                                            if (!CheckDouble(SplittedCondition[n], ConditionStringAr[1], reflex))
                                            {
                                                return false;
                                            }
                                        }
                                    }

                                    if (ConditionStringAr[0].Length == 1)
                                    {
                                        return CheckSingle(ConditionStringAr[0][0], ConditionStringAr[1], reflex);
                                    }
                                    else
                                    {
                                        return CheckDouble(ConditionStringAr[0], ConditionStringAr[1], reflex);
                                    }
                                }
                            }
                            else
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (this._hho != "-")
                            {
                                string[] ForCheck = this._hho.Split('=');

                                return CheckSingle(ForCheck[0][0], ForCheck[1], reflex);
                            }
                            else
                            {
                                return true;
                            }
                        }
                    }
                }
                else
                {
                    if(reflex.L != 0)
                    {
                        if (reflex.L != reflex.H)
                        {
                            if (this._hol != "-")
                            {
                                string[] SplittedCondition = this._hol.Split(',');

                                if (SplittedCondition.Length == 1)
                                {
                                    string[] ForCheck = this._hol.Split('=');

                                    if (ForCheck[0].Length == 1)
                                    {
                                        return CheckSingle(ForCheck[0][0], ForCheck[1], reflex);
                                    }
                                    else
                                    {
                                        return CheckDouble(ForCheck[0], ForCheck[1], reflex);
                                    }
                                }
                                else
                                {
                                    string[] ConditionStringAr = SplittedCondition[SplittedCondition.Length - 1].Split('=');

                                    for (int n = 0; n < SplittedCondition.Length - 2; n++)
                                    {
                                        if (SplittedCondition[n].Length == 1)
                                        {
                                            if (!CheckSingle(SplittedCondition[n][0], ConditionStringAr[1], reflex))
                                            {
                                                return false;
                                            }
                                        }
                                        else
                                        {
                                            if (!CheckDouble(SplittedCondition[n], ConditionStringAr[1], reflex))
                                            {
                                                return false;
                                            }
                                        }
                                    }

                                    if (ConditionStringAr[0].Length == 1)
                                    {
                                        return CheckSingle(ConditionStringAr[0][0], ConditionStringAr[1], reflex);
                                    }
                                    else
                                    {
                                        return CheckDouble(ConditionStringAr[0], ConditionStringAr[1], reflex);
                                    }
                                }
                            }
                            else
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (this._hoh != "-")
                            {
                                string[] ForCheck = this._hoh.Split('=');

                                return CheckSingle(ForCheck[0][0], ForCheck[1], reflex);
                            }
                            else
                            {
                                return true;
                            }
                        }
                    }
                    else
                    {
                        if (this._hoo != "-")
                        {
                            string[] ForCheck = this._hoo.Split('=');

                            return CheckSingle(ForCheck[0][0], ForCheck[1], reflex);
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            }
            else
            {
                if(reflex.K != 0)
                {
                    if(reflex.L != 0)
                    {
                        if(reflex.L != reflex.K)
                        {
                            if (this._okl != "-")
                            {
                                string[] SplittedCondition = this._okl.Split(',');

                                if (SplittedCondition.Length == 1)
                                {
                                    string[] ForCheck = this._okl.Split('=');

                                    if (ForCheck[0].Length == 1)
                                    {
                                        return CheckSingle(ForCheck[0][0], ForCheck[1], reflex);
                                    }
                                    else
                                    {
                                        return CheckDouble(ForCheck[0], ForCheck[1], reflex);
                                    }
                                }
                                else
                                {
                                    string[] ConditionStringAr = SplittedCondition[SplittedCondition.Length - 1].Split('=');

                                    for (int n = 0; n < SplittedCondition.Length - 2; n++)
                                    {
                                        if (SplittedCondition[n].Length == 1)
                                        {
                                            if (!CheckSingle(SplittedCondition[n][0], ConditionStringAr[1], reflex))
                                            {
                                                return false;
                                            }
                                        }
                                        else
                                        {
                                            if (!CheckDouble(SplittedCondition[n], ConditionStringAr[1], reflex))
                                            {
                                                return false;
                                            }
                                        }
                                    }

                                    if (ConditionStringAr[0].Length == 1)
                                    {
                                        return CheckSingle(ConditionStringAr[0][0], ConditionStringAr[1], reflex);
                                    }
                                    else
                                    {
                                        return CheckDouble(ConditionStringAr[0], ConditionStringAr[1], reflex);
                                    }
                                }
                            }
                            else
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (this._okk != "-")
                            {
                                string[] ForCheck = this._okk.Split('=');

                                return CheckSingle(ForCheck[0][0], ForCheck[1], reflex);
                            }
                            else
                            {
                                return true;
                            }
                        }
                    }
                    else
                    {
                        if (this._oko != "-")
                        {
                            string[] ForCheck = this._oko.Split('=');

                            return CheckSingle(ForCheck[0][0], ForCheck[1], reflex);
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    if (reflex.L != 0)
                    {
                        if (this._ool != "-")
                        {
                            string[] ForCheck = this._ool.Split('=');

                            return CheckSingle(ForCheck[0][0], ForCheck[1], reflex);
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        private bool CheckSingle(char Parameter, string Condition, HKLReflex reflex)
        {
            bool ret = false;

            switch (Parameter)
            {
                case 'h':
                    try
                    {
                        int Multi = Convert.ToInt32(Convert.ToString(Condition[0]));

                        if (Condition.Length == 2)
                        {
                            if (reflex.H % Multi == 0)
                            {
                                ret = true;
                            }
                            else
                            {
                                ret = false;
                            }
                        }
                        else
                        {
                            int Add = Convert.ToInt32(Convert.ToString(Condition[3]));

                            if (Condition[2] == '+')
                            {
                                int Check = (reflex.H - Add) % Multi;

                                if (Check == 0)
                                {
                                    ret = true;
                                }
                                else
                                {
                                    ret = false;
                                }
                            }
                            else
                            {
                                int Check = (reflex.H + Add) % Multi;

                                if (Check == 0)
                                {
                                    ret = true;
                                }
                                else
                                {
                                    ret = false;
                                }
                            }
                        }
                    }
                    catch
                    {
                        ret = true;
                    }
                    break;
                case 'k':
                    try
                    {
                        int Multi = Convert.ToInt32(Convert.ToString(Condition[0]));

                        if (Condition.Length == 2)
                        {
                            if (reflex.K % Multi == 0)
                            {
                                ret = true;
                            }
                            else
                            {
                                ret = false;
                            }
                        }
                        else
                        {
                            int Add = Convert.ToInt32(Convert.ToString(Condition[3]));

                            if (Condition[2] == '+')
                            {
                                int Check = (reflex.K - Add) % Multi;

                                if (Check == 0)
                                {
                                    ret = true;
                                }
                                else
                                {
                                    ret = false;
                                }
                            }
                            else
                            {
                                int Check = (reflex.K + Add) % Multi;

                                if (Check == 0)
                                {
                                    ret = true;
                                }
                                else
                                {
                                    ret = false;
                                }
                            }
                        }
                    }
                    catch
                    {
                        ret = true;
                    }
                    break;
                case 'l':
                    try
                    {
                        int Multi = Convert.ToInt32(Convert.ToString(Condition[0]));

                        if (Condition.Length == 2)
                        {
                            if (reflex.L % Multi == 0)
                            {
                                ret = true;
                            }
                            else
                            {
                                ret = false;
                            }
                        }
                        else
                        {
                            int Add = Convert.ToInt32(Convert.ToString(Condition[3]));

                            if (Condition[2] == '+')
                            {
                                int Check = (reflex.L - Add) % Multi;

                                if (Check == 0)
                                {
                                    ret = true;
                                }
                                else
                                {
                                    ret = false;
                                }
                            }
                            else
                            {
                                int Check = (reflex.L + Add) % Multi;

                                if (Check == 0)
                                {
                                    ret = true;
                                }
                                else
                                {
                                    ret = false;
                                }
                            }
                        }
                    }
                    catch
                    {
                        ret = true;
                    }
                    break;
                default:
                    ret = true;
                    break;
            }

            return ret;
        }

        private bool CheckDouble(string Parameter, string Condition, HKLReflex reflex)
        {
            bool ret = false;
            int FirstMulti = 1;
            int SecondMulti = 1;
            int ParamValue = 0;
            bool AddSub = true;

            try
            {
                FirstMulti = Convert.ToInt32(Convert.ToString(Parameter[0]));
                SecondMulti = Convert.ToInt32(Convert.ToString(Parameter[3]));

                switch(Parameter[1])
                {
                    case 'h':
                        FirstMulti *= reflex.H;
                        break;
                    case 'k':
                        FirstMulti *= reflex.K;
                        break;
                    case 'l':
                        FirstMulti *= reflex.L;
                        break;
                    default:
                        FirstMulti = 0;
                        break;
                }

                switch(Parameter[4])
                {
                    case 'h':
                        SecondMulti *= reflex.H;
                        break;
                    case 'k':
                        SecondMulti *= reflex.K;
                        break;
                    case 'l':
                        SecondMulti *= reflex.L;
                        break;
                    default:
                        SecondMulti = 0;
                        break;
                }

                if (Parameter[2] == '-')
                {
                    AddSub = false;
                }
            }
            catch
            {
                try
                {
                    SecondMulti = Convert.ToInt32(Convert.ToString(Parameter[2]));

                    switch (Parameter[0])
                    {
                        case 'h':
                            FirstMulti *= reflex.H;
                            break;
                        case 'k':
                            FirstMulti *= reflex.K;
                            break;
                        case 'l':
                            FirstMulti *= reflex.L;
                            break;
                        default:
                            FirstMulti = 0;
                            break;
                    }

                    switch (Parameter[3])
                    {
                        case 'h':
                            SecondMulti *= reflex.H;
                            break;
                        case 'k':
                            SecondMulti *= reflex.K;
                            break;
                        case 'l':
                            SecondMulti *= reflex.L;
                            break;
                        default:
                            SecondMulti = 0;
                            break;
                    }
                }
                catch
                {
                    switch (Parameter[0])
                    {
                        case 'h':
                            FirstMulti *= reflex.H;
                            break;
                        case 'k':
                            FirstMulti *= reflex.K;
                            break;
                        case 'l':
                            FirstMulti *= reflex.L;
                            break;
                        default:
                            FirstMulti = 0;
                            break;
                    }

                    switch (Parameter[2])
                    {
                        case 'h':
                            SecondMulti *= reflex.H;
                            break;
                        case 'k':
                            SecondMulti *= reflex.K;
                            break;
                        case 'l':
                            SecondMulti *= reflex.L;
                            break;
                        default:
                            SecondMulti = 0;
                            break;
                    }
                }
                finally
                {

                    if (Parameter[1] == '-')
                    {
                        AddSub = false;
                    }
                }
            }
            finally
            {
                if(AddSub)
                {
                    ParamValue = FirstMulti + SecondMulti;
                }
                else
                {
                    ParamValue = FirstMulti + SecondMulti;
                }
            }

            if(ParamValue == 0)
            {
                return true;
            }

            try
            {
                int Multi = Convert.ToInt32(Convert.ToString(Condition[0]));

                if (Condition.Length == 2)
                {
                    if (ParamValue % Multi == 0)
                    {
                        ret = true;
                    }
                    else
                    {
                        ret = false;
                    }
                }
                else
                {
                    int Add = Convert.ToInt32(Convert.ToString(Condition[3]));

                    if (Condition[2] == '+')
                    {
                        int Check = (ParamValue - Add) % Multi;

                        if (Check == 0)
                        {
                            ret = true;
                        }
                        else
                        {
                            ret = false;
                        }
                    }
                    else
                    {
                        int Check = (ParamValue + Add) % Multi;

                        if (Check == 0)
                        {
                            ret = true;
                        }
                        else
                        {
                            ret = false;
                        }
                    }
                }
            }
            catch
            {
                ret = true;
            }

            return ret;
        }

        private bool CheckTriple(string Line, string Condition, HKLReflex reflex)
        {
            bool ret = false;

            int HKLSumValue = 0;

            if (Line[0] == '-')
            {
                HKLSumValue -= reflex.H;
                HKLSumValue += reflex.K;
                HKLSumValue += reflex.L;
            }
            else
            {
                HKLSumValue += reflex.H;
                if (Line[1] == '-')
                {
                    HKLSumValue -= reflex.K;
                }
                else
                {
                    HKLSumValue += reflex.K;
                }
                HKLSumValue += reflex.L;
            }

            try
            {
                int Multi = Convert.ToInt32(Convert.ToString(Condition[0]));

                if (Condition.Length == 2)
                {
                    if (HKLSumValue % Multi == 0)
                    {
                        ret = true;
                    }
                    else
                    {
                        ret = false;
                    }
                }
                else
                {
                    int Add = Convert.ToInt32(Convert.ToString(Condition[3]));

                    if (Condition[2] == '+')
                    {
                        int Check = (HKLSumValue - Add) % Multi;

                        if (Check == 0)
                        {
                            ret = true;
                        }
                        else
                        {
                            ret = false;
                        }
                    }
                    else
                    {
                        int Check = (HKLSumValue + Add) % Multi;

                        if (Check == 0)
                        {
                            ret = true;
                        }
                        else
                        {
                            ret = false;
                        }
                    }
                }
            }
            catch
            {
                ret = true;
            }

            return ret;
        }
    }

    [Serializable]
    public struct CODData
    {
        private int _id;
        public int Id
        {
            get
            {
                return this._id;
            }
        }
        public bool Loaded;

        private string _name;
        public string Name
        {
            get
            {
                if (this._name == "")
                {
                    return this._symmetryGroup;
                }
                else
                {
                    return this._name;
                }
            }
            set
            {
                this._name = value;
            }
        }

        private double _a;
        public double A
        {
            get
            {
                return this._a;
            }
            set
            {
                this._a = value;
            }
        }

        private double _b;
        public double B
        {
            get
            {
                return this._b;
            }
            set
            {
                this._b = value;
            }
        }

        private double _c;
        public double C
        {
            get
            {
                return this._c;
            }
            set
            {
                this._c = value;
            }
        }

        private double _alpha;
        public double Alpha
        {
            get
            {
                return this._alpha;
            }
            set
            {
                this._alpha = value;
            }
        }
        public double AlphaRad
        {
            get
            {
                return ((this.Alpha * Math.PI) / 180.0);
            }
        }

        private double _beta;
        public double Beta
        {
            get
            {
                return this._beta;
            }
            set
            {
                this._beta = value;
            }
        }
        public double BetaRad
        {
            get
            {
                return ((this.Beta * Math.PI) / 180.0);
            }
        }

        private double _gamma;
        public double Gamma
        {
            get
            {
                return this._gamma;
            }
            set
            {
                this._gamma = value;
            }
        }
        public double GammaRad
        {
            get
            {
                return ((this.Gamma * Math.PI) / 180.0);
            }
        }

        private double _cellVolume;
        public double CellVolume
        {
            get
            {
                return this._cellVolume;
            }
        }

        private double _measurementTemperature;
        public double MeasurementTemperature
        {
            get
            {
                return this._measurementTemperature;
            }
        }

        private string _chemicalFormula;
        public string ChemicalFormula
        {
            get
            {
                return this._chemicalFormula;
            }
            set
            {
                string[] SplittedElements = value.Split(' ');

                int ElCount = 0;
                for(int n = 0; n < SplittedElements.Length; n++)
                {
                    if(SplittedElements[n] != "")
                    {
                        ElCount++;
                    }
                }

                this._numberOfDifferentElements = ElCount;
                this._chemicalFormula = "- " + value + " -";
            }
        }

        private int _numberOfDifferentElements;
        public int NumberOfDiffrentElements
        {
            get
            {
                return this._numberOfDifferentElements;
            }
        }

        private int _symmetryGroupId;
        public int SymmetryGroupID
        {
            get
            {
                return this._symmetryGroupId;
            }
        }

        private string _symmetryGroup;
        public string SymmetryGroup
        {
            get
            {
                return this._symmetryGroup;
            }
            set
            {
                this._symmetryGroupHall = "";
                this._symmetryGroupId = -1;
                this._symmetryGroup = value;
            }
        }

        private string _symmetryGroupHall;
        public string SymmetryGroupHall
        {
            get
            {
                return this._symmetryGroupHall;
            }
        }

        private string _text;
        public string Text
        {
            get
            {
                return this._text;
            }
        }

        private DateTime _lastUpdate;
        public DateTime LastUpdate
        {
            get
            {
                return this._lastUpdate;
            }
        }

        public List<HKLReflex> HKLList;

        public CODData(System.Data.DataRow Entry)
        {
            try
            {
                this.Loaded = true;
                this._name = "";

                this._a = Convert.ToDouble(Entry[1]);
                this._b = Convert.ToDouble(Entry[3]);
                this._c = Convert.ToDouble(Entry[5]);
                this._alpha = Convert.ToDouble(Entry[7]);
                this._beta = Convert.ToDouble(Entry[9]);
                this._gamma = Convert.ToDouble(Entry[11]);
                this._cellVolume = Convert.ToDouble(Entry[13]);
                try
                {
                    this._measurementTemperature = Convert.ToDouble(Entry[15]);
                }
                catch
                {
                    this._measurementTemperature = 0.0;
                }
                this._numberOfDifferentElements = Convert.ToInt32(Convert.ToString(Entry[25]));
                this._chemicalFormula = Convert.ToString(Entry[31]);
                this._symmetryGroup = Convert.ToString(Entry[26]);
                this._symmetryGroupHall = Convert.ToString(Entry[27]);
                this._lastUpdate = Convert.ToDateTime(Entry[67]);
                this._text = Convert.ToString(Entry[65]);
                this._symmetryGroupId = -1;
                HKLList = new List<HKLReflex>();
                this._id = Tools.IdManagment.ActualCODId;
            }
            catch
            {
                this.Loaded = false;
                this._name = "";

                this._a = 0;
                this._b = 0;
                this._c = 0;
                this._alpha = 0;
                this._beta = 0;
                this._gamma = 0;
                this._cellVolume = 0;
                this._measurementTemperature = 0.0;
                this._numberOfDifferentElements = 0;
                this._chemicalFormula = "Failed";
                this._symmetryGroup = "Failed";
                this._symmetryGroupHall = "Failed";
                this._lastUpdate = new DateTime(1, 1, 1);
                this._text = "The dataset is corupted!!!!!! DO NOT USE!!!!!!!!!!!!!!!!!";
                this._symmetryGroupId = -1;
                HKLList = new List<HKLReflex>();
                this._id = -1;
            }
        }

        public CODData(string CIFFilePath)
        {
            this.Loaded = true;
            this._name = "";

            this._a = 0.0;
            this._b = 0.0;
            this._c = 0.0;
            this._alpha = 0.0;
            this._beta = 0.0;
            this._gamma = 0.0;
            this._cellVolume = 0.0;
            this._measurementTemperature = 0.0;
            this._numberOfDifferentElements = 1;
            this._chemicalFormula = "";
            this._symmetryGroup = "";
            this._symmetryGroupHall = "";
            this._lastUpdate = DateTime.Today;
            this._text = "";
            this._symmetryGroupId = -1;

            string[] PatternFileLines = System.IO.File.ReadLines(CIFFilePath).ToArray();

            foreach (string s in PatternFileLines)
            {
                if (s.Length > 0)
                {
                    if (s[0] != '#')
                    {
                        string[] Content = s.Split(' ');
                        string[] SplittedForError = { };
                        switch (Content[0])
                        {
                            case "_cell_angle_alpha":
                                SplittedForError = Content[Content.Length - 1].Split('(');
                                this._alpha = Convert.ToDouble(SplittedForError[0]);
                                break;
                            case "_cell_angle_beta":
                                SplittedForError = Content[Content.Length - 1].Split('(');
                                this._beta = Convert.ToDouble(SplittedForError[0]);
                                break;
                            case "_cell_angle_gamma":
                                SplittedForError = Content[Content.Length - 1].Split('(');
                                this._gamma = Convert.ToDouble(SplittedForError[0]);
                                break;
                            case "_cell_length_a":
                                SplittedForError = Content[Content.Length - 1].Split('(');
                                this._a = Convert.ToDouble(SplittedForError[0]);
                                break;
                            case "_cell_length_b":
                                SplittedForError = Content[Content.Length - 1].Split('(');
                                this._b = Convert.ToDouble(SplittedForError[0]);
                                break;
                            case "_cell_length_c":
                                SplittedForError = Content[Content.Length - 1].Split('(');
                                this._c = Convert.ToDouble(SplittedForError[0]);
                                break;
                            case "_chemical_formula_sum":
                                try
                                {
                                    string[] FormulaSplit = s.Split('\'');
                                    string[] SplittedElements = FormulaSplit[1].Split(' ');
                                    this._chemicalFormula = FormulaSplit[1];
                                    this._numberOfDifferentElements = SplittedElements.Count();
                                }
                                catch
                                {
                                    this._chemicalFormula = Content[Content.Length - 1];
                                    this._numberOfDifferentElements = 1;
                                }
                                break;
                            case "_cell_volume":
                                SplittedForError = Content[Content.Length - 1].Split('(');
                                this._cellVolume = Convert.ToDouble(SplittedForError[0]);
                                break;
                            case "_symmetry_space_group_name_Hall":
                                string[] SpaceSlpittHall = s.Split('\'');
                                this._symmetryGroupHall = SpaceSlpittHall[1];
                                break;
                            case "_space_group_IT_number":
                                this._symmetryGroupId = Convert.ToInt32(Content[Content.Length - 1]);
                                break;
                            case "_symmetry_Int_Tables_number":
                                this._symmetryGroupId = Convert.ToInt32(Content[Content.Length - 1]);
                                break;
                            case "_symmetry_space_group_name_H-M":
                                string[] SpaceSPlittHM = s.Split('\'');
                                this._symmetryGroup = SpaceSPlittHM[1];
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            HKLList = new List<HKLReflex>();
            this._id = Tools.IdManagment.ActualCODId;

            Tools.Calculation.AddHKLList(this);
        }

        public CODData(string name, double cellVolume, int symmetryID, string symmetryHall, string text)
        {
            this.Loaded = false;
            this._name = name;

            this._a = 0.0;
            this._b = 0.0;
            this._c = 0.0;
            this._alpha = 0.0;
            this._beta = 0.0;
            this._gamma = 0.0;
            this._cellVolume = cellVolume;
            this._measurementTemperature = 0.0;
            this._numberOfDifferentElements = 1;
            this._chemicalFormula = "";
            this._symmetryGroup = "";
            this._symmetryGroupHall = symmetryHall;
            this._lastUpdate = DateTime.Today;
            this._text = text;
            this._symmetryGroupId = symmetryID;
            this._id = Tools.IdManagment.ActualCODId;

            HKLList = new List<HKLReflex>();
        }

        public CODData(CODData ToClone)
        {
            this.Loaded = ToClone.Loaded;
            this._name = ToClone._name;

            this._a = ToClone._a;
            this._b = ToClone._b;
            this._c = ToClone._c;
            this._alpha = ToClone._alpha;
            this._beta = ToClone._beta;
            this._gamma = ToClone._gamma;
            this._cellVolume = ToClone._cellVolume;
            this._measurementTemperature = ToClone._measurementTemperature;
            this._numberOfDifferentElements = ToClone._numberOfDifferentElements;
            this._chemicalFormula = ToClone._chemicalFormula;
            this._symmetryGroup = ToClone._symmetryGroup;
            this._symmetryGroupHall = ToClone._symmetryGroupHall;
            this._lastUpdate = ToClone._lastUpdate;
            this._text = ToClone._text;
            this._symmetryGroupId = ToClone._symmetryGroupId;
            this._id = ToClone.Id;

            HKLList = ToClone.HKLList;
        }
    }

    [Serializable]
    public struct HKLReflex
    {
        private int _h;
        public int H
        {
            get
            {
                return this._h;
            }
        }

        private int _k;
        public int K
        {
            get
            {
                return this._k;
            }
        }

        private int _l;
        public int L
        {
            get
            {
                return this._l;
            }
        }

        public string HKLString
        {
            get
            {
                string ret = "( " + Convert.ToString(this._h) + " " + Convert.ToString(this._k) + " " + Convert.ToString(this._l) + " )";

                return ret;
            }
        }

        private double _distance;
        public double Distance
        {
            get
            {
                return this._distance;
            }
        }

        public double EstimatedAngle
        {
            get
            {
                double ret = 0.0;

                ret = Math.Asin(CalScec.Properties.Settings.Default.UsedWaveLength / (2.0 * this._distance));
                ret *= (180 / Math.PI);
                ret *= 2;
                return ret;
            }
        }

        public HKLReflex(int[] hKL, double distance)
        {
            this._h = hKL[0];
            this._k = hKL[1];
            this._l = hKL[2];
            this._distance = distance;
        }

        public HKLReflex(int h, int k, int l, double distance)
        {
            this._h = h;
            this._k = k;
            this._l = l;
            this._distance = distance;
        }


    }
}
