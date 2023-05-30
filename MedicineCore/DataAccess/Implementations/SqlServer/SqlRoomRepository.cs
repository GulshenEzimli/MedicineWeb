using HospitalCore.DataAccess.Interfaces;
using HospitalCore.Domain.Entities;
using HospitalCore.Domain.Enums;
using HospitalCore.Utils;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace HospitalCore.DataAccess.Implementations.SqlServer
{
    public class SqlRoomRepository : IRoomRepository
    {

        private readonly string _connectionString;
        public SqlRoomRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool Delete(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string cmdText = @"delete from Rooms where Id = @id";
                using (SqlCommand command = new SqlCommand(cmdText, connection))
                {
                    command.Parameters.AddWithValue("id", id);
                    return command.ExecuteNonQuery() == 1;
                }
            }
        }

        public List<Room> Get()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string cmdText = @"select Id, Number, IsAvailable, Type, BlockFloor 
                                   from Rooms";
                using (SqlCommand command = new SqlCommand(cmdText, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    List<Room> rooms = new List<Room>();

                    while (reader.Read())
                    {
                        Room room = GetRoom(reader);
                        rooms.Add(room);
                    }
                    return rooms;
                }
            }
         
        }

        public Room GetById(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string cmdText = @"select Id, Number, IsAvailable, Type, BlockFloor, IsDelete 
                                   from Rooms where IsDelete = 0 and Id = @id";
                using (SqlCommand command = new SqlCommand(cmdText, connection))
                {
                    command.Parameters.AddWithValue("id", id);
                    SqlDataReader reader = command.ExecuteReader();
                    Room room = GetRoom(reader);
                    return room;
                }
            }
        }

        public int Insert(Room room)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string cmdText = @"insert into Rooms output inserted.id values(
                                 @number, @blockFloor, @isAvailable, @type,  @isDelete)";
                using (SqlCommand command = new SqlCommand(cmdText, connection))
                {
                    AddParameters(command, room);
                    return (int)command.ExecuteScalar();
                }
            }
        }

        public bool Update(Room room)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string cmdText = @"update Rooms set Number = @number, IsAvailable = @isAvailable,
                                 Type = @type, BlockFloor = @blockFloor, IsDelete = @isDelete
                                 where Id = @id";
                using (SqlCommand command = new SqlCommand(cmdText, connection))
                {
                    command.Parameters.AddWithValue("id", room.Id);
                    AddParameters(command, room);
                    return command.ExecuteNonQuery() == 1;
                }
            }
        }

        private Room GetRoom(SqlDataReader reader)
        {
            Room room = new Room();
            room.Id = reader.GetInt32("id");
            room.Number = reader.GetInt32("number");
            room.BlockFloor = reader.GetInt32("blockFloor");
            room.IsAvailable = reader.GetBoolean("IsAvailable");
            room.Type = (RoomTypes)reader.GetByte("type");

            return room;
        }

        private void AddParameters(SqlCommand command, Room room)
        {
            command.Parameters.AddWithValue ("number", room.Number);
            command.Parameters.AddWithValue("blockFloor", room.BlockFloor);
            command.Parameters.AddWithValue("isAvailable",room.IsAvailable);    
            command.Parameters.AddWithValue("type",room.Type);
            command.Parameters.AddWithValue("isDelete", room.IsDelete);
        }
    }
}
