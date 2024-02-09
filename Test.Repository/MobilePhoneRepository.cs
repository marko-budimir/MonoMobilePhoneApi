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
    public class MobilePhoneRepository : IMobilePhoneRepository
    {
        public List<IMobilePhone> GetAll(MobilePhoneFilter filter)
        {
            List<IMobilePhone> mobilePhones = new List<IMobilePhone>();
            NpgsqlConnection connection = new NpgsqlConnection(Constants.ConnectionString);
            using (connection)
            {
                NpgsqlCommand command = new NpgsqlCommand();
                command.Connection = connection;
                if (filter != null)
                {
                    ApplyFilter(command, filter);
                }
                try
                {
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
                    throw e;
                }
                finally
                {
                    connection.Close();
                }
            }
            return mobilePhones;
        }

        public IMobilePhone GetById(Guid id, bool includeShops = false)
        {
            IMobilePhone mobilePhone = null;
            NpgsqlConnection connection = new NpgsqlConnection(Constants.ConnectionString);
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
                                mobilePhone.Shops = new List<IShop>();
                            }
                        }
                        if (includeShops)
                        {
                            mobilePhone.Shops.Add(new Shop()
                            {
                                Id = (Guid)reader["Id"],
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

        public void Add(IMobilePhone mobilePhone)
        {
            NpgsqlConnection connection = new NpgsqlConnection(Constants.ConnectionString);
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
                    throw e;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public void AddShops(Guid mobilePhoneId, List<Guid> shopIds)
        {
            IMobilePhone mobilePhone = GetById(mobilePhoneId);
            if (mobilePhone == null)
            {
                throw new Exception("MobilePhone not found");
            }
            NpgsqlConnection connection = new NpgsqlConnection(Constants.ConnectionString);
            using (connection)
            {
                connection.Open();
                using (NpgsqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (Guid shopId in shopIds)
                        {
                            using (NpgsqlCommand command = new NpgsqlCommand())
                            {
                                command.CommandText = "INSERT INTO \"MobilePhoneShop\" (\"Id\", \"MobilePhoneId\", \"ShopId\") VALUES (@id, @mobilePhoneId, @shopId)";
                                command.Connection = connection;
                                command.Parameters.AddWithValue("id", Guid.NewGuid());
                                command.Parameters.AddWithValue("mobilePhoneId", mobilePhoneId);
                                command.Parameters.AddWithValue("shopId", shopId);
                                command.Transaction = transaction;
                                command.ExecuteNonQuery();
                            }
                        }
                        transaction.Commit();
                    }
                    catch (NpgsqlException e)
                    {
                        transaction.Rollback();
                        throw e;
                    }
                }
            }
        }

        public void Update(Guid id, IMobilePhone newMobilePhone)
        {
            IMobilePhone mobilePhone = GetById(id); 
            if (mobilePhone == null)
            {
               throw new Exception("MobilePhone not found");
            }
            NpgsqlConnection connection = new NpgsqlConnection(Constants.ConnectionString);
            using (connection)
            {
                NpgsqlCommand command = new NpgsqlCommand();
                command.CommandText = $"UPDATE \"MobilePhone\" " +
                    $"SET \"OperatingSystem\" = @operatingSystem, \"StorageCapacityGB\" = @storageCapacityGB, \"RamGB\" = @ramGB, \"Color\" = @color " +
                    $"WHERE \"Id\" = @id";
                command.Connection = connection;
                command.Parameters.AddWithValue("id", id);
                command.Parameters.AddWithValue("operatingSystem", newMobilePhone.OperatingSystem ?? mobilePhone.OperatingSystem);
                command.Parameters.AddWithValue("storageCapacityGB", newMobilePhone.StorageCapacityGB ?? mobilePhone.StorageCapacityGB);
                command.Parameters.AddWithValue("ramGB", newMobilePhone.RamGB ?? mobilePhone.RamGB);
                command.Parameters.AddWithValue("color", newMobilePhone.Color ?? mobilePhone.Color);
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
                    throw e;
                }
                finally
                {
                    connection.Close();
                }
            }
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
