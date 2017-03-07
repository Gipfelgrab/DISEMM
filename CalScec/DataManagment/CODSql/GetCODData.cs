using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalScec.DataManagment.CODSql
{
    public static class GetCODData
    {
        public static MySql.Data.MySqlClient.MySqlConnection Conn = new MySql.Data.MySqlClient.MySqlConnection(@"Server=www.crystallography.net;Uid=cod_reader;Pwd=;Database=cod;");

        public static List<CrystalData.CODData> GetListOfCrystalData(string[] ElementsContained)
        {
            List<CrystalData.CODData> Ret = new List<CrystalData.CODData>();

            string InFormSearch = "%";
            foreach(string s in ElementsContained)
            {
                InFormSearch += s + "%";
            }

            string GetSting = "SELECT * FROM data WHERE formula LIKE @term ORDER BY date DESC LIMIT 1000";
            MySql.Data.MySqlClient.MySqlCommand GetCmd = new MySql.Data.MySqlClient.MySqlCommand(GetSting, Conn);

            GetCmd.Parameters.AddWithValue("term", InFormSearch);

            MySql.Data.MySqlClient.MySqlDataAdapter GetAdapter = new MySql.Data.MySqlClient.MySqlDataAdapter(GetCmd);
            System.Data.DataTable GetTable = new System.Data.DataTable();

            Conn.Open();
            GetAdapter.Fill(GetTable);
            Conn.Close();
            
            foreach(System.Data.DataRow DR in GetTable.Rows)
            {
                Ret.Add(new CrystalData.CODData(DR));
            }

            return Ret;
        }

        public static List<CrystalData.CODData> GetListOfCrystalData(string[] ElementsContained, int NumberOfElements)
        {
            List<CrystalData.CODData> Ret = new List<CrystalData.CODData>();

            string InFormSearch = "%";
            foreach (string s in ElementsContained)
            {
                InFormSearch += s + "%";
            }

            string GetSting = "SELECT * FROM data WHERE formula LIKE @term AND nel=@number ORDER BY date DESC LIMIT 1000";
            MySql.Data.MySqlClient.MySqlCommand GetCmd = new MySql.Data.MySqlClient.MySqlCommand(GetSting, Conn);

            GetCmd.Parameters.AddWithValue("term", InFormSearch);
            GetCmd.Parameters.AddWithValue("number", NumberOfElements);

            MySql.Data.MySqlClient.MySqlDataAdapter GetAdapter = new MySql.Data.MySqlClient.MySqlDataAdapter(GetCmd);
            System.Data.DataTable GetTable = new System.Data.DataTable();

            Conn.Open();
            GetAdapter.Fill(GetTable);
            Conn.Close();

            foreach (System.Data.DataRow DR in GetTable.Rows)
            {
                Ret.Add(new CrystalData.CODData(DR));
            }

            return Ret;
        }

        public static List<CrystalData.CODData> GetListOfCrystalData(string ChemicalFormula)
        {
            List<CrystalData.CODData> Ret = new List<CrystalData.CODData>();

            string ChemicalFormulaMod = "%" + ChemicalFormula + "%";

            string GetSting = "SELECT * FROM data WHERE formula LIKE @term ORDER BY date DESC LIMIT 1000";
            MySql.Data.MySqlClient.MySqlCommand GetCmd = new MySql.Data.MySqlClient.MySqlCommand(GetSting, Conn);

            GetCmd.Parameters.AddWithValue("term", ChemicalFormulaMod);

            MySql.Data.MySqlClient.MySqlDataAdapter GetAdapter = new MySql.Data.MySqlClient.MySqlDataAdapter(GetCmd);
            System.Data.DataTable GetTable = new System.Data.DataTable();

            Conn.Open();
            GetAdapter.Fill(GetTable);
            Conn.Close();

            foreach (System.Data.DataRow DR in GetTable.Rows)
            {
                Ret.Add(new CrystalData.CODData(DR));
            }

            return Ret;
        }

        public static List<CrystalData.CODData> GetListOfCrystalData(string[] ElementsContained, DateTime Since)
        {
            List<CrystalData.CODData> Ret = new List<CrystalData.CODData>();

            string InFormSearch = "%";
            foreach (string s in ElementsContained)
            {
                InFormSearch += s + "%";
            }

            string GetSting = "SELECT * FROM data WHERE formula LIKE @term AND date>@since ORDER BY date DESC LIMIT 1000";
            MySql.Data.MySqlClient.MySqlCommand GetCmd = new MySql.Data.MySqlClient.MySqlCommand(GetSting, Conn);

            GetCmd.Parameters.AddWithValue("term", InFormSearch);
            GetCmd.Parameters.AddWithValue("since", Since);

            MySql.Data.MySqlClient.MySqlDataAdapter GetAdapter = new MySql.Data.MySqlClient.MySqlDataAdapter(GetCmd);
            System.Data.DataTable GetTable = new System.Data.DataTable();

            Conn.Open();
            GetAdapter.Fill(GetTable);
            Conn.Close();

            foreach (System.Data.DataRow DR in GetTable.Rows)
            {
                Ret.Add(new CrystalData.CODData(DR));
            }

            return Ret;
        }

        public static List<CrystalData.CODData> GetListOfCrystalData(string[] ElementsContained, int NumberOfElements, DateTime Since)
        {
            List<CrystalData.CODData> Ret = new List<CrystalData.CODData>();

            string InFormSearch = "%";
            foreach (string s in ElementsContained)
            {
                InFormSearch += s + "%";
            }

            string GetSting = "SELECT * FROM data WHERE formula LIKE @term AND nel=@number AND date>@since ORDER BY date DESC LIMIT 1000";
            MySql.Data.MySqlClient.MySqlCommand GetCmd = new MySql.Data.MySqlClient.MySqlCommand(GetSting, Conn);

            GetCmd.Parameters.AddWithValue("term", InFormSearch);
            GetCmd.Parameters.AddWithValue("number", NumberOfElements);
            GetCmd.Parameters.AddWithValue("since", Since);

            MySql.Data.MySqlClient.MySqlDataAdapter GetAdapter = new MySql.Data.MySqlClient.MySqlDataAdapter(GetCmd);
            System.Data.DataTable GetTable = new System.Data.DataTable();

            Conn.Open();
            GetAdapter.Fill(GetTable);
            Conn.Close();

            foreach (System.Data.DataRow DR in GetTable.Rows)
            {
                Ret.Add(new CrystalData.CODData(DR));
            }

            return Ret;
        }

        public static List<CrystalData.CODData> GetListOfCrystalData(string ChemicalFormula, DateTime Since)
        {
            List<CrystalData.CODData> Ret = new List<CrystalData.CODData>();

            string ChemicalFormulaMod = "%" + ChemicalFormula + "%";

            string GetSting = "SELECT * FROM data WHERE formula LIKE @term AND date>@since ORDER BY date DESC LIMIT 1000";
            MySql.Data.MySqlClient.MySqlCommand GetCmd = new MySql.Data.MySqlClient.MySqlCommand(GetSting, Conn);

            GetCmd.Parameters.AddWithValue("term", ChemicalFormulaMod);
            GetCmd.Parameters.AddWithValue("since", Since);

            MySql.Data.MySqlClient.MySqlDataAdapter GetAdapter = new MySql.Data.MySqlClient.MySqlDataAdapter(GetCmd);
            System.Data.DataTable GetTable = new System.Data.DataTable();

            Conn.Open();
            GetAdapter.Fill(GetTable);
            Conn.Close();

            foreach (System.Data.DataRow DR in GetTable.Rows)
            {
                Ret.Add(new CrystalData.CODData(DR));
            }

            return Ret;
        }
    }
}
