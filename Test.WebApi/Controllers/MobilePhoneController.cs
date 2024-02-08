using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Test.WebApi.Models;

namespace Test.WebApi.Controllers
{
    public class MobilePhoneController : ApiController
    {
        private string connectionString = "Server=127.0.0.1;Port=5432;Database=MonoTest;User Id=postgres;Password=postgres;";

        // GET: api/MobilePhone
        //filter by brand, model, operating system, storage capacity, ram, color
        public HttpResponseMessage Get([FromUri] MobilePhoneFilter filter)
        {
            if (filter == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK/*, mobilePhones.OrderBy(m => m.Id)*/);
            }
            List<MobilePhone> mobilePhones = new List<MobilePhone>();
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            using (connection)
            {
                NpgsqlCommand command = new NpgsqlCommand();
                command.Connection = connection;
                ApplyFilter(command, filter);
                connection.Open();
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    mobilePhones.Add(new MobilePhone()
                    {
                        Id = (Guid)reader["Id"],
                        Brand = (string)reader["Brand"],
                        Model = (string)reader["Model"],
                        OperatingSystem = (string)reader["OperatingSystem"],
                        StorageCapacityGB = (int)reader["StorageCapacityGB"],
                        RamGB = (int)reader["RamGB"],
                        Color = (string)reader["Color"]
                    });
                }
                connection.Close();
            }
            
            return Request.CreateResponse(HttpStatusCode.OK, mobilePhones);
        }

        // GET: api/MobilePhone/5
        public HttpResponseMessage Get(Guid id)
        {
            var mobilePhone = GetMobilePhoneById(id);
            if (mobilePhone == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Mobile phone with this ID doesn't exists");
            }
            mobilePhone.Shops = GetShopsByMobilePhoneId(id);
            return Request.CreateResponse(HttpStatusCode.OK, mobilePhone);
        }

        // POST: api/MobilePhone
        public HttpResponseMessage Post([FromBody] MobilePhone mobilePhone)
        {
            if (mobilePhone == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);

            using (connection)
            {
                NpgsqlCommand command = new NpgsqlCommand();
                command.CommandText = $"INSERT INTO \"MobilePhone\" (\"Id\", \"Brand\", \"Model\", \"OperatingSystem\", \"StorageCapacityGB\", \"RamGB\", \"Color\") " +
                    "VALUES (@id, @brand, @model, @operatingSystem, @storageCapacityGB, @ramGB, @color)";
                command.Connection = connection;
                Guid id = Guid.NewGuid();
                command.Parameters.AddWithValue("id", id);
                command.Parameters.AddWithValue("brand", mobilePhone.Brand);
                command.Parameters.AddWithValue("model", mobilePhone.Model);
                command.Parameters.AddWithValue("operatingSystem", mobilePhone.OperatingSystem);
                command.Parameters.AddWithValue("storageCapacityGB", mobilePhone.StorageCapacityGB);
                command.Parameters.AddWithValue("ramGB", mobilePhone.RamGB);
                command.Parameters.AddWithValue("color", mobilePhone.Color);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
                mobilePhone.Id = id;
            }
            return Request.CreateResponse(HttpStatusCode.Created, mobilePhone);
        }

        // PUT: api/MobilePhone/5
        public HttpResponseMessage Put(Guid id, [FromBody] MobilePhoneUpdate mobilePhoneUpdate)
        {
            if (mobilePhoneUpdate == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            MobilePhone mobilePhone = GetMobilePhoneById(id);
            if (mobilePhone == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Mobile phone with this ID doesn't exists");
            }
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);

            using (connection)
            {
                NpgsqlCommand command = new NpgsqlCommand();
                command.CommandText = $"UPDATE \"MobilePhone\" " +
                    $"SET \"OperatingSystem\" = @operatingSystem, \"StorageCapacityGB\" = @storageCapacityGB, \"RamGB\" = @ramGB, \"Color\" = @color " +
                    $"WHERE \"Id\" = @id";
                command.Connection = connection;
                command.Parameters.AddWithValue("id", id);
                command.Parameters.AddWithValue("operatingSystem", mobilePhoneUpdate.OperatingSystem ?? mobilePhone.OperatingSystem);
                command.Parameters.AddWithValue("storageCapacityGB", mobilePhoneUpdate.StorageCapacityGB ?? mobilePhone.StorageCapacityGB);
                command.Parameters.AddWithValue("ramGB", mobilePhoneUpdate.RamGB ?? mobilePhone.RamGB);
                command.Parameters.AddWithValue("color", mobilePhoneUpdate.Color ?? mobilePhone.Color);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
            return Request.CreateResponse(HttpStatusCode.OK, GetMobilePhoneById(id));
        }

        // DELETE: api/MobilePhone/5
        public HttpResponseMessage Delete(Guid id)
        {
            if(id == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            MobilePhone mobilePhone = GetMobilePhoneById(id);

            if (mobilePhone == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Mobile phone with this ID doesn't exists");
            }
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);

            using (connection)
            {
                NpgsqlCommand command = new NpgsqlCommand();
                command.CommandText = $"DELETE FROM \"MobilePhone\" WHERE \"Id\" = @id";
                command.Connection = connection;
                command.Parameters.AddWithValue("id", id);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

        private MobilePhone GetMobilePhoneById(Guid id)
        {
            MobilePhone mobilePhone = null;
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);

            using (connection)
            {
                NpgsqlCommand command = new NpgsqlCommand();
                command.CommandText = $"SELECT * FROM \"MobilePhone\" WHERE \"Id\" = @id";
                command.Connection = connection;
                command.Parameters.AddWithValue("id", id);
                connection.Open();
                NpgsqlDataReader reader = command.ExecuteReader();
                reader.Read();
                if (reader.HasRows) {
                    mobilePhone = new MobilePhone()
                    {
                        Id = (Guid)reader["Id"],
                        Brand = (string)reader["Brand"],
                        Model = (string)reader["Model"],
                        OperatingSystem = (string)reader["OperatingSystem"],
                        StorageCapacityGB = (int)reader["StorageCapacityGB"],
                        RamGB = (int)reader["RamGB"],
                        Color = (string)reader["Color"]
                    };
                }
                connection.Close();
            }

            return mobilePhone;
        }

        private List<ShopView> GetShopsByMobilePhoneId(Guid id)
        {
            List<ShopView> shops = new List<ShopView>();
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);

            using (connection)
            {
                NpgsqlCommand command = new NpgsqlCommand();
                command.CommandText = $"SELECT \"Shop\".\"Name\", \"Shop\".\"PhoneNumber\", \"Shop\".\"Address\", \"Shop\".\"Mail\" " +
                    $"FROM \"MobilePhoneShop\" " +
                    $"LEFT JOIN \"Shop\" " +
                    $"ON \"Shop\".\"Id\" = \"MobilePhoneShop\".\"ShopId\" " +
                    $"Where \"MobilePhoneShop\".\"MobilePhoneId\" = @id";
                command.Connection = connection;
                command.Parameters.AddWithValue("id", id);
                connection.Open();
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    shops.Add(new ShopView()
                    {
                        Name = (string)reader["Name"],
                        PhoneNumber = reader["PhoneNumber"] == DBNull.Value ? null : (string)reader["PhoneNumber"],
                        Address = (string)reader["Address"],
                        Mail = reader["Mail"] == DBNull.Value ? null : (string)reader["Mail"]
                    });
                }
                connection.Close();
            }

            return shops;
        }   

        private void ApplyFilter(NpgsqlCommand command, MobilePhoneFilter filter)
        {
            StringBuilder commandText = new StringBuilder();
            commandText.Append("SELECT * FROM \"MobilePhone\" ");
            List<string> conditions = new List<string>();
            if (filter.Brand != null)
            {
                conditions.Add("\"Brand\" LIKE @brand ");
                command.Parameters.AddWithValue("brand", "%" + filter.Brand + "%");
            }
            if (filter.Model != null)
            {
                conditions.Add("\"Model\" LIKE @model ");
                command.Parameters.AddWithValue("model", "%" + filter.Model + "%");
            }
            if (filter.OperatingSystem != null)
            {
                conditions.Add("\"OperatingSystem\" LIKE @operatingSystem ");
                command.Parameters.AddWithValue("operatingSystem", "%" + filter.OperatingSystem + "%");
            }
            if (filter.StorageCapacityGB != null)
            {
                conditions.Add("\"StorageCapacityGB\" = @storageCapacityGB ");
                command.Parameters.AddWithValue("storageCapacityGB", filter.StorageCapacityGB);
            }
            if (filter.RamGB != null)
            {
                conditions.Add("\"RamGB\" = @ramGB ");
                command.Parameters.AddWithValue("ramGB", filter.RamGB);
            }
            if (filter.Color != null)
            {
                conditions.Add("\"Color\" LIKE @color ");
                command.Parameters.AddWithValue("color", "%" + filter.Color + "%");
            }
            if (conditions.Count > 0)
            {
                commandText.Append("WHERE ");
                commandText.Append(string.Join("AND ", conditions));
            }
            command.CommandText = commandText.ToString();
        }
    }
}
