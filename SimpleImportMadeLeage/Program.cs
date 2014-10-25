using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Data;
using System.IO;

namespace SimpleImportMadeLeage
{
    class Program
    {
        static void Main(string[] args)
        {

            var fileName = @"C:\temp\Book1.xls";
            var connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", fileName);

            var adapter = new OleDbDataAdapter("SELECT * FROM [Sheet1$]", connectionString);
            var ds = new DataSet();

            adapter.Fill(ds, "anyNameHere");
            List<MadeMember> mems = new List<MadeMember>();
            DataTable data = ds.Tables["anyNameHere"];
            foreach (DataRow row in data.Rows)
            {

                MadeMember member = new MadeMember();
                member.added = row[0].ToString();
                member.number = row[1].ToString();
                member.active = row[2].ToString();
                member.classs = row[3].ToString();
                member.sex = row[4].ToString();
                member.league = row[5].ToString();
                member.name = row[6].ToString();
                mems.Add(member);

            }
            string nl = "bal";






        }
    }
}
