using Product_API.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;

namespace Product_API.Controllers
{
    public class RunnerController : ApiController
    {
        private string connectionString;

        public RunnerController()
        {
            connectionString = ConfigurationManager.ConnectionStrings["Albums"].ConnectionString;
        }

        // GET: api/Runner
        public List<Runner> GetRunners()
        {
            return RetrieveAllRunners();
        }

        // GET: api/Runner/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Runner
        [HttpPost]
        //public void Post([FromBody]string value)
        public void Post(Runner runner)
        {
            //Runner runner = JsonConvert.DeserializeObject<Runner>(value);
            UpsertRunner(runner);
        }

        // PUT: api/Runner/5
        public void Put(int id, [FromBody]string value)
        {
            Runner runner = JsonConvert.DeserializeObject<Runner>(value);
            UpdateRunner(runner);
        }

        // DELETE: api/Runner/5
        public void Delete(int id)
        {
            DeleteRunner(id);
        }

        private List<Runner> RetrieveAllRunners()
        {
            Runner runner = new Runner();
            List<Runner> runners = new List<Runner>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var sqlString = "select id,name,sex,age from dbo.Runners";
                SqlCommand command = new SqlCommand(sqlString, connection);
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        runner = new Runner
                        {
                            id = (int)reader[0],
                            name = reader[1].ToString(),
                            sex = reader[2].ToString(),
                            age = reader[3].ToString()
                        };

                        runners.Add(runner);
                    }
                }

                connection.Close();
                return runners;
            }
        }

        private Runner RetrieveRunner(int id)
        {
            Runner runner;
            List<Runner> runners = new List<Runner>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var sqlString = "select id,name,sex,age from dbo.Runners where id=@id";
                SqlCommand command = new SqlCommand(sqlString, connection);
                command.Parameters.AddWithValue("@id", id);
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        runner = new Runner
                        {
                            id = (int)reader[0],
                            name = reader[1].ToString(),
                            sex = reader[2].ToString(),
                            age = reader[3].ToString()
                        };

                        runners.Add(runner);
                    }
                }

                connection.Close();
                return runners.Count > 0 ? runners[0] : null;
            }
        }

        private void UpdateRunner(Runner runner)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var sqlString = "update dbo.Runners set name=@name,age=@age,sex=@sex where id=@id";
                SqlCommand command = new SqlCommand(sqlString, connection);
                command.Parameters.AddWithValue("@name", runner.name);
                command.Parameters.AddWithValue("@age", runner.age);
                command.Parameters.AddWithValue("@sex", runner.sex);
                command.Parameters.AddWithValue("@id", runner.id);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        private void InsertRunner(Runner runner)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var sqlString = "insert dbo.Runners (name,age,sex) values (@name,@age,@sex)";
                SqlCommand command = new SqlCommand(sqlString, connection);
                command.Parameters.AddWithValue("@name", runner.name);
                command.Parameters.AddWithValue("@age", runner.age);
                command.Parameters.AddWithValue("@sex", runner.sex);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        private void UpsertRunner(Runner runner)
        {
            var retrieved = RetrieveRunner(runner.id);
            if (retrieved == null) { InsertRunner(runner); }
            else
            { UpdateRunner(runner); }
        }

        private void DeleteRunner(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var sqlString = "delete dbo.Runners where id = @id";
                SqlCommand command = new SqlCommand(sqlString, connection);
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }
}
