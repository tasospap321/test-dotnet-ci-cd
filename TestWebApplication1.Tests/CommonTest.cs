using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestWebApplication1.Controllers;

namespace TestWebApplication1.Tests
{
    public class CommonTest
    {
        private readonly string connectionString = "Data Source=.;Initial Catalog=TestWebApplicationDb;Integrated Security=True;MultipleActiveResultSets=True;Application Name=EntityFramework";

        protected ValuesController valuesController;

        private Random random = new Random();

        [OneTimeSetUp]
        public void Setup()
        {
            valuesController = new ValuesController();
        }

        [OneTimeTearDown]
        public void Teardown()
        {

        }
        protected DataTable GetFromDb(bool offline = false)
        {
            if (offline)
            {
                DataTable users = new DataTable();


                /* columns */
                DataColumn dataColumn1 = new DataColumn
                {
                    ColumnName = "Id",
                    DataType = Type.GetType("System.Int32"),
                    AutoIncrement = true
                };
                users.Columns.Add(dataColumn1);


                DataColumn dataColumn2 = new DataColumn
                {
                    ColumnName = "Firstname",
                    DataType = Type.GetType("System.String"),
                };
                users.Columns.Add(dataColumn2);

                DataColumn dataColumn3 = new DataColumn
                {
                    ColumnName = "Lastname",
                    DataType = Type.GetType("System.String"),
                };
                users.Columns.Add(dataColumn3);

                DataColumn dataColumn4 = new DataColumn()
                {
                    ColumnName = "Email",
                    DataType = Type.GetType("System.String"),
                };
                users.Columns.Add(dataColumn4);

                DataColumn dataColumn5 = new DataColumn()
                {
                    ColumnName = "Password",
                    DataType = Type.GetType("System.String"),
                };
                users.Columns.Add(dataColumn5);

                /* rows */
                for (int i = 0; i < 5; i++)
                {
                    DataRow dataRow = users.NewRow();
                    dataRow["Firstname"] = $"UserFirstname{i}";
                    dataRow["Lastname"] = $"UserLastname{i}";
                    dataRow["Email"] = $"useremail{i}@mail.com";
                    dataRow["Password"] = $"{i}{i}{i}";

                    users.Rows.Add(dataRow);
                }
                return users;

            }
            else
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM [dbo].[Users]", connection))
                    {
                        DataSet ds = new DataSet();
                        adapter.Fill(ds);
                        return ds.Tables[0];
                    }
                }

            }

        }

        public string GetRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
