using HospitalCore.DataAccess.Interfaces;
using HospitalCore.Domain.Entities;
using HospitalCore.Utils;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace HospitalCore.DataAccess.Implementations.SqlServer
{
    public class SqlPositionRepository : IPositionRepoitory
    {
        private readonly string _connectionString;
        public SqlPositionRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public List<Position> Get()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string cmdText = @"select DoctorPositions.Id as PositionId, Departments.Id as DepartmentId, PositionName, DepartmentName
                                   from DoctorPositions
                                   inner join Departments on DoctorPositions.DepartmentId = Departments.Id";
                using (SqlCommand command = new SqlCommand(cmdText, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    List<Position> positions = new List<Position>();

                    while (reader.Read())
                    {
                        Position position = GetPosition(reader);
                        positions.Add(position);
                    }
                    return positions;
                }
            }
        }

        public Position GetById(int id)
        {
            throw new NotImplementedException();
        }

        public int Insert(Position entity)
        {
            throw new NotImplementedException();
        }

        public bool Update(Position entity)
        {
            throw new NotImplementedException();
        }
        private Position GetPosition(SqlDataReader reader)
        {
            Position position = new Position();
            position.Id = reader.GetInt32("PositionId");
            position.Name = reader.GetString("PositionName");
            position.Department = new Department()
            {
                Id = reader.GetInt32("DepartmentId"),
                Name = reader.GetString("DepartmentName")
            };
            return position;
        }
    }
}
