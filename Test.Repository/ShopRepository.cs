using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Test.Common;
using Test.Model;
using Test.Model.Common;
using Test.Repository.Common;

namespace Test.Repository
{
    public class ShopRepository : IShopRepository
    {
        public List<IShop> GetAll()
        {
            List<IShop> shops = new List<IShop>();
            NpgsqlConnection connection = new NpgsqlConnection(Constants.ConnectionString);
            using (connection)
            {
                NpgsqlCommand command = new NpgsqlCommand();
                command.CommandText = "SELECT * FROM \"Shop\"";
                command.Connection = connection;
                try
                {
                    connection.Open();
                    NpgsqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        shops.Add(new Shop()
                        {
                            Id = (Guid)reader["Id"],
                            Name = (string)reader["Name"],
                            Address = (string)reader["Address"],
                            Mail = reader["Mail"] == DBNull.Value ? null : (string)reader["Mail"],
                            PhoneNumber = reader["PhoneNumber"] == DBNull.Value ? null : (string)reader["PhoneNumber"]
                        }
                        );
                    }
                }
                catch (NpgsqlException e)
                {
                    throw e;
                }
                finally
                {
                    connection.Close();
                }
            }
            return shops;
        }

        public IShop GetById(Guid id)
        {
            IShop shop = null;
            NpgsqlConnection connection = new NpgsqlConnection(Constants.ConnectionString);

            using (connection)
            {
                NpgsqlCommand command = new NpgsqlCommand();
                command.CommandText = $"SELECT * FROM \"Shop\" WHERE \"Id\" = @id";
                command.Connection = connection;
                command.Parameters.AddWithValue("id", id);
                try
                {
                    connection.Open();
                    NpgsqlDataReader reader = command.ExecuteReader();
                    reader.Read();
                    if (reader.HasRows)
                    {
                        shop = new Shop()
                        {
                            Id = (Guid)reader["Id"],
                            Name = (string)reader["Name"],
                            Address = (string)reader["Address"],
                            Mail = reader["Mail"] == DBNull.Value ? null : (string)reader["Mail"],
                            PhoneNumber = reader["PhoneNumber"] == DBNull.Value ? null : (string)reader["PhoneNumber"]
                        };
                    }
                }
                catch (NpgsqlException e)
                {
                    throw e;
                }
                finally
                {
                    connection.Close();
                }
            }
            return shop;
        }

        public void Add(IShop shop)
        {
            NpgsqlConnection connection = new NpgsqlConnection(Constants.ConnectionString);
            using (connection)
            {
                NpgsqlCommand command = new NpgsqlCommand();
                command.CommandText = "INSERT INTO \"Shop\" (\"Id\", \"Name\", \"Address\", \"Mail\", \"PhoneNumber\") VALUES (@id, @name, @address, @mail, @PhoneNumber)";
                command.Connection = connection;
                Guid id = Guid.NewGuid();
                command.Parameters.AddWithValue("id", id);
                command.Parameters.AddWithValue("name", shop.Name);
                command.Parameters.AddWithValue("address", shop.Address);
                if (shop.Mail != null)
                {
                    command.Parameters.AddWithValue("mail", shop.Mail);
                }
                else
                {
                    command.Parameters.AddWithValue("mail", DBNull.Value);
                }
                if (shop.PhoneNumber != null)
                {
                    command.Parameters.AddWithValue("PhoneNumber", shop.PhoneNumber);
                }
                else
                {
                    command.Parameters.AddWithValue("PhoneNumber", DBNull.Value);
                }
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (NpgsqlException e)
                {
                    throw e;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public void Update(Guid id, IShop newShop)
        {
            IShop shop = GetById(id);
            NpgsqlConnection connection = new NpgsqlConnection(Constants.ConnectionString);
            using (connection)
            {
                NpgsqlCommand command = new NpgsqlCommand();
                command.CommandText = "UPDATE \"Shop\" SET \"Name\" = @name, \"Address\" = @address, \"Mail\" = @mail, \"PhoneNumber\" = @PhoneNumber WHERE \"Id\" = @id";
                command.Connection = connection;
                command.Parameters.AddWithValue("id", id);
                command.Parameters.AddWithValue("name", newShop.Name ?? shop.Name);
                command.Parameters.AddWithValue("address", newShop.Address ?? shop.Address);
                if (newShop.Mail != null || shop.Mail != null)
                {
                    command.Parameters.AddWithValue("mail", newShop.Mail ?? shop.Mail);
                }
                else
                {
                    command.Parameters.AddWithValue("mail", DBNull.Value);
                }
                if (newShop.PhoneNumber != null || shop.PhoneNumber != null)
                {
                    command.Parameters.AddWithValue("PhoneNumber", newShop.PhoneNumber ?? shop.PhoneNumber);
                }
                else
                {
                    command.Parameters.AddWithValue("PhoneNumber", DBNull.Value);
                }
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (NpgsqlException e)
                {
                    throw e;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public void Delete(Guid id)
        {
            NpgsqlConnection connection = new NpgsqlConnection(Constants.ConnectionString);
            using (connection)
            {
                NpgsqlCommand command = new NpgsqlCommand();
                command.CommandText = $"DELETE FROM \"Shop\" WHERE \"Id\" = @id";
                command.Connection = connection;
                command.Parameters.AddWithValue("id", id);
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (NpgsqlException e)
                {
                    throw e;
                }
                finally
                {
                    connection.Close();
                }
            }
        }
    }
}
