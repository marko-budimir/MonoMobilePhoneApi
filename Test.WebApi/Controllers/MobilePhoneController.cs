using Npgsql;
using System;
using System.Collections.Generic;
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
                try {
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
            return Request.CreateResponse(HttpStatusCode.OK, mobilePhones);
        }

        // GET: api/MobilePhone/5
        public HttpResponseMessage Get(Guid id, bool includeShops = false)
        {
            var mobilePhone = GetMobilePhoneById(id, includeShops);
            if (mobilePhone == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Mobile phone with this ID doesn't exists");
            }
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

        private MobilePhone GetMobilePhoneById(Guid id, bool includeShops = false)
        {
            MobilePhone mobilePhone = null;
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            string commandText = $"SELECT * FROM \"MobilePhone\" WHERE \"Id\" = @id";
            if (includeShops)
            {
                commandText = $"SELECT \"MobilePhone\".*, \"Shop\".\"Name\", \"Shop\".\"PhoneNumber\", \"Shop\".\"Address\", \"Shop\".\"Mail\" " +
                    $"FROM \"MobilePhone\" " +
                    $"LEFT JOIN \"MobilePhoneShop\" " +
                    $"ON \"MobilePhone\".\"Id\" = \"MobilePhoneShop\".\"MobilePhoneId\" " +
                    $"LEFT JOIN \"Shop\" " +
                    $"ON \"Shop\".\"Id\" = \"MobilePhoneShop\".\"ShopId\" " +
                    $"Where \"MobilePhone\".\"Id\" = @id";
            }
            using (connection)
            {
                NpgsqlCommand command = new NpgsqlCommand();
                command.CommandText = commandText;
                command.Connection = connection;
                command.Parameters.AddWithValue("id", id);
                try
                {
                    connection.Open();
                    NpgsqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        if (mobilePhone == null)
                        {
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
                            if (includeShops)
                            {
                                mobilePhone.Shops = new List<ShopView>();
                            }
                        }
                        if (includeShops)
                        {
                            mobilePhone.Shops.Add(new ShopView()
                            {
                                Name = (string)reader["Name"],
                                PhoneNumber = reader["PhoneNumber"] == DBNull.Value ? null : (string)reader["PhoneNumber"],
                                Address = (string)reader["Address"],
                                Mail = reader["Mail"] == DBNull.Value ? null : (string)reader["Mail"]
                            });
                        }
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
            return mobilePhone;
        }   

        private void ApplyFilter(NpgsqlCommand command, MobilePhoneFilter filter)
        {
            StringBuilder commandText = new StringBuilder();
            commandText.Append("SELECT * FROM \"MobilePhone\" ");
            List<string> conditions = new List<string>();
            if (filter.Brand != null)
            {
                conditions.Add("\"Brand\" ILIKE @brand ");
                command.Parameters.AddWithValue("brand", "%" + filter.Brand + "%");
            }
            if (filter.Model != null)
            {
                conditions.Add("\"Model\" ILIKE @model ");
                command.Parameters.AddWithValue("model", "%" + filter.Model + "%");
            }
            if (filter.OperatingSystem != null)
            {
                conditions.Add("\"OperatingSystem\" ILIKE @operatingSystem ");
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
                conditions.Add("\"Color\" ILIKE @color ");
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
