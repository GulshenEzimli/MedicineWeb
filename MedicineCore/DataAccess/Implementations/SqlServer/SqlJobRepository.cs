using HospitalCore.DataAccess.Interfaces;
using HospitalCore.Domain.Entities;
using HospitalCore.Utils;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace HospitalCore.DataAccess.Implementations.SqlServer
{
    public class SqlJobRepository : IJobRepository
    {
        private readonly string _connectionString;
        public SqlJobRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public bool Delete(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string cmdText = @"delete from OtherJobs where Id = @id";
                using (SqlCommand command = new SqlCommand(cmdText, connection))
                {
                    command.Parameters.AddWithValue("id", id);
                    return command.ExecuteNonQuery() == 1;
                }
            }
        }

        public List<Job> Get()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string cmdText = @"select * from OtherJobs";
                using (SqlCommand command = new SqlCommand(cmdText, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    List<Job> jobs = new List<Job>();
                    while (reader.Read())
                    {
                        Job job = GetJob(reader);
                        jobs.Add(job);
                    }
                    return jobs;
                }
            }
        }

        public Job GetById(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string cmdText = @"select * from OtherJobs where Id = @id";
                using (SqlCommand command = new SqlCommand(cmdText, connection))
                {
                    command.Parameters.AddWithValue("id", id);
                    SqlDataReader reader = command.ExecuteReader();
                    Job job = GetJob(reader);
                    return job;
                }
            }
        }

        public int Insert(Job job)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string cmdText = @"insert into OtherJobs values(@jobName)";
                using (SqlCommand command = new SqlCommand(cmdText, connection))
                {
                    command.Parameters.AddWithValue("jobName", job.Name);
                    return (int)command.ExecuteScalar();
                }
            }
        }

        public bool Update(Job job)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string cmdText = @"update OtherJobs set JobName = @jobName";
                using (SqlCommand command = new SqlCommand(cmdText, connection))
                {
                    command.Parameters.AddWithValue("jobName", job.Name);
                    return command.ExecuteNonQuery() == 1;
                }
            }
        }

        private Job GetJob(SqlDataReader reader)
        {
            Job job = new Job();
            job.Id = reader.GetInt32("Id");
            job.Name = reader.GetString("JobName");
            return job;
        }
    }
}
