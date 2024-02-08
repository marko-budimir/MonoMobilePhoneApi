using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Shop/5
        public HttpResponseMessage Get(Guid id)
        {
            var shop = GetShopById(id);
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
                command.CommandText = "INSERT INTO \"Shop\" (\"Id\", \"Name\", \"Address\", \"Mail\") VALUES (@id, @name, @address, @mail)";
                command.Connection = connection;
                Guid id = Guid.NewGuid();
                command.Parameters.AddWithValue("id", id);
                command.Parameters.AddWithValue("name", shop.Name);
                command.Parameters.AddWithValue("address", shop.Address);
                command.Parameters.AddWithValue("mail", shop.Mail);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
                shop.Id = id;
            }
            return Request.CreateResponse(HttpStatusCode.Created, shop);
        }

        // PUT: api/Shop/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Shop/5
        public HttpResponseMessage Delete(Guid id)
        {
            var shop = GetShopById(id);
            if (shop == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            using (connection)
            {
                NpgsqlCommand command = new NpgsqlCommand();
                command.CommandText = $"DELETE FROM \"Shop\" WHERE \"Id\" = @id";
                command.Connection = connection;
                command.Parameters.AddWithValue("id", id);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
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
                connection.Open();
                NpgsqlDataReader reader = command.ExecuteReader();
                reader.Read();
                if (reader.HasRows)
                {
                    shop = new Shop()
                    {
                        Id = (Guid)reader["Id"],
                        Name = reader["Name"].ToString(),
                        Address = (string)reader["Address"],
                        Mail = reader["Mail"].ToString()
                    };
                }
                connection.Close();
            }

            return shop;
        }
    }
}
