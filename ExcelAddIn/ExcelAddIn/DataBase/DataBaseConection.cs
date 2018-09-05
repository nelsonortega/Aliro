using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExcelAddIn.DataBase
{
    class DataBaseConection
    {

        public string[] InstalledInstances()
        {
            Microsoft.Win32.RegistryKey rk;
            rk = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server", false);
            string[] s;
            s = ((string[])rk.GetValue("InstalledInstances"));
            return s;
        }


        public String[] InstalledDataBase(string instances)
        {
            // Las bases de datos propias de SQL Server
            string[] basesSys = { "master", "model", "msdb", "tempdb" };
            string[] bases;
            DataTable dt = new DataTable();
            // Usamos la seguridad integrada de Windows
            string sCnn = "Server=" + instances + "; database=master; integrated security=yes";

            // La orden T-SQL para recuperar las bases de master
            string sel = "SELECT name FROM sysdatabases";
            try
            {
                SqlDataAdapter da = new SqlDataAdapter(sel, sCnn);
                da.Fill(dt);
                bases = new string[dt.Rows.Count - 1];
                int k = -1;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string s = dt.Rows[i]["name"].ToString();
                    // Solo asignar las bases que no son del sistema
                    if (Array.IndexOf(basesSys, s) == -1)
                    {
                        k += 1;
                        bases[k] = s;
                    }
                }
                if (k == -1) return null;
                // ReDim Preserve
                {
                    int i1_RPbases = bases.Length;
                    string[] copyOf_dataBases = new string[i1_RPbases];
                    Array.Copy(bases, copyOf_dataBases, i1_RPbases);
                    bases = new string[(k + 1)];
                    Array.Copy(copyOf_dataBases, bases, (k + 1));
                };
                return bases;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                    "Error al recuperar las bases de la instancia indicada",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }



        public DataTable TablesInDataBase(string instances, string dataBase)
        {
                      
            SqlDataAdapter adapter = new SqlDataAdapter("select* from information_schema.tables", OpenConection(instances, dataBase));
            DataTable tables = new DataTable("tables");
            adapter.Fill(tables);
            return tables;
            
        }

        public SqlConnection OpenConection(string instances, string dataBase) {
            SqlConnection conexion = new SqlConnection("Data Source=" + instances + "; Initial Catalog=" + dataBase + "; Integrated Security = True");
            conexion.Open();
            return conexion;
        }



    }
}

   

