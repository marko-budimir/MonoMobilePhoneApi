using Npgsql;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Test.WebApi.Models;

namespace Test.WebApi.Controllers
{
    public class ShopController : ApiController
    {
        private string connectionString = "Server=127.0.0.1;Port=5432;Database=MonoTest;User Id=postgres;Password=postgres;";
        // GET: api/Shop
        public HttpResponseMessage Get()
        {
            List<Shop> shops = new List<Shop>();
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);
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
                    return Request.CreateResponse(HttpStatusCode.BadGateway, e.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, shops);
        }

        // GET: api/Shop/5
        public HttpResponseMessage Get(Guid id)
        {
            Shop shop = null;
            try 
            {
                shop = GetShopById(id);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, e.Message);
            }
            if (shop == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            return Request.CreateResponse(HttpStatusCode.OK, shop);
        }

        // POST: api/Shop
        public HttpResponseMessage Post([FromBody]Shop shop)
        {
            if (shop == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);
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
                    return Request.CreateResponse(HttpStatusCode.BadGateway, e.Message);
                }
                finally
                {
                    connection.Close();
                }
                shop.Id = id;
            }
            return Request.CreateResponse(HttpStatusCode.Created, shop);
        }

        // PUT: api/Shop/5
        public HttpResponseMessage Put(Guid id, [FromBody]ShopUpdate newShop)
        {
            if (newShop == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            Shop shop = GetShopById(id);
            if (shop == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);
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
                    return Request.CreateResponse(HttpStatusCode.BadGateway, e.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, GetShopById(id));
        }

        // DELETE: api/Shop/5
        public HttpResponseMessage Delete(Guid id)
        {
            var shop = GetShopById(id);
            if (shop == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Shop with this ID doesn't exists");
            }
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);
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
                    return Request.CreateResponse(HttpStatusCode.BadGateway, e.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

        private Shop GetShopById(Guid id)
        {
            Shop shop = null;
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);

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
    }
}
