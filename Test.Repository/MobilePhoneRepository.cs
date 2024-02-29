using Npgsql;
using System;
using System.Collections.Generic;
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
        public async Task<PagedList<IMobilePhone>> GetAllAsync(MobilePhoneFilter filter, Sorting sorting, Paging paging)
        {
            List<IMobilePhone> mobilePhones = new List<IMobilePhone>();
            NpgsqlConnection connection = new NpgsqlConnection(Constants.ConnectionString);
            int itemCount = 0;
            using (connection)
            {
                NpgsqlCommand command = new NpgsqlCommand();
                command.Connection = connection;
                command.CommandText = "SELECT * FROM \"MobilePhone\"";
                ApplyFilter(command, filter);
                ApplySorting(command, sorting);
                itemCount = await GetItemCountAsync(filter);
                ApplyPaging(command, paging, itemCount);

                try
                {
                    await connection.OpenAsync();
                    NpgsqlDataReader reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
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
                    await connection.CloseAsync();
                }
            }
            return new PagedList<IMobilePhone>(mobilePhones, paging.PageNumber, paging.PageSize, itemCount);
        }

        public async Task<IMobilePhone> GetByIdAsync(Guid id, bool includeShops = false)
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
                    await connection.OpenAsync();
                    NpgsqlDataReader reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
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
                    await connection.CloseAsync();
                }
            }
            return mobilePhone;
        }

        public async Task<int> AddAsync(IMobilePhone mobilePhone)
        {
            NpgsqlConnection connection = new NpgsqlConnection(Constants.ConnectionString);
            using (connection)
            {
                NpgsqlCommand command = new NpgsqlCommand();
                command.CommandText = $"INSERT INTO \"MobilePhone\" (\"Id\", \"Brand\", \"Model\", \"OperatingSystem\", \"StorageCapacityGB\", \"RamGB\", \"Color\") " +
                    "VALUES (@id, @brand, @model, @operatingSystem, @storageCapacityGB, @ramGB, @color)";
                command.Connection = connection;
                command.Parameters.AddWithValue("id", mobilePhone.Id);
                command.Parameters.AddWithValue("brand", mobilePhone.Brand);
                command.Parameters.AddWithValue("model", mobilePhone.Model);
                command.Parameters.AddWithValue("operatingSystem", mobilePhone.OperatingSystem);
                command.Parameters.AddWithValue("storageCapacityGB", mobilePhone.StorageCapacityGB);
                command.Parameters.AddWithValue("ramGB", mobilePhone.RamGB);
                command.Parameters.AddWithValue("color", mobilePhone.Color);
                try
                {
                    await connection.OpenAsync();
                    return await command.ExecuteNonQueryAsync();
                }
                catch
                {
                    return -1;
                }
                finally
                {
                    await connection.CloseAsync();
                }
            }
        }

        public async Task<int> AddShopsAsync(Guid mobilePhoneId, List<Guid> shopIds)
        {
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
                        await transaction.CommitAsync();
                    }
                    catch (NpgsqlException e)
                    {
                        await transaction.RollbackAsync();
                        return -1;
                    }
                }
            }
            return 1;
        }

        public async Task<int> UpdateAsync(Guid id, IMobilePhone newMobilePhone)
        {
            IMobilePhone mobilePhone = await GetByIdAsync(id); 
            if (mobilePhone == null)
            {
                return 0;
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
                    await connection.OpenAsync();
                    return await command.ExecuteNonQueryAsync();
                }
                catch (NpgsqlException e)
                {
                    throw e;
                }
                finally
                {
                    await connection.CloseAsync();
                }
            }
        }

        public async Task<int> DeleteAsync(Guid id)
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
                    await connection.OpenAsync();
                    return await command.ExecuteNonQueryAsync();         
                }
                catch (NpgsqlException e)
                {
                    return -1;
                }
                finally
                {
                    await connection.CloseAsync();
                }
            }
        }


        private void ApplyFilter(NpgsqlCommand command, MobilePhoneFilter filter)
        {
            StringBuilder commandText = new StringBuilder(command.CommandText);

            if(filter.ShopId != null)
            {
                commandText.Append(" LEFT JOIN \"MobilePhoneShop\" ON \"MobilePhone\".\"Id\" = \"MobilePhoneShop\".\"MobilePhoneId\" WHERE \"MobilePhoneShop\".\"ShopId\" = @shopId");
                command.Parameters.AddWithValue("shopId", filter.ShopId);
            }
            else
            {
                commandText.Append(" WHERE 1 = 1");
            }
            if (filter.SearchQuery != null)
            {
                string[] keyWords = filter.SearchQuery.Split(' ');
                for(int i = 0; i < keyWords.Length; i++)
                {
                    commandText.Append($" AND (\"Brand\" ILIKE @searchQuery{i} OR \"Model\" ILIKE @searchQuery{i}")
                        .Append($" OR \"OperatingSystem\" ILIKE @searchQuery{i} OR \"Color\" ILIKE @searchQuery{i})");
                    command.Parameters.AddWithValue($"searchQuery{i}", "%" + keyWords[i] + "%");
                }
            }
            if (filter.MinStorageCapacityGB != null)
            {
                commandText.Append(" AND \"StorageCapacityGB\" >= @minStorageCapacityGB");
                command.Parameters.AddWithValue("minStorageCapacityGB", filter.MinStorageCapacityGB);
            }
            if (filter.MaxStorageCapacityGB != null)
            {
                commandText.Append(" AND \"StorageCapacityGB\" <= @maxStorageCapacityGB");
                command.Parameters.AddWithValue("maxStorageCapacityGB", filter.MaxStorageCapacityGB);
            }
            if (filter.MinRamGB != null)
            {
                commandText.Append(" AND \"RamGB\" >= @minRamGB");
                command.Parameters.AddWithValue("minRamGB", filter.MinRamGB);
            }
            if (filter.MaxRamGB != null)
            {
                commandText.Append(" AND \"RamGB\" <= @maxRamGB");
                command.Parameters.AddWithValue("maxRamGB", filter.MaxRamGB);
            }
            command.CommandText = commandText.ToString();
        }

        private void ApplySorting(NpgsqlCommand command, Sorting sorting)
        {
            StringBuilder commandText = new StringBuilder(command.CommandText);
            commandText.Append(" ORDER BY \"");
            commandText.Append(sorting.SortBy).Append("\" ");
            commandText.Append(sorting.IsAscending ? "ASC" : "DESC");
            command.CommandText = commandText.ToString();
        }

        private void ApplyPaging(NpgsqlCommand command, Paging paging, int itemCount)
        {
            StringBuilder commandText = new StringBuilder(command.CommandText);
            int currentItem = (paging.PageNumber - 1) * paging.PageSize;
            if (currentItem >= 0 && currentItem < itemCount)
            {
                commandText.Append(" LIMIT ").Append(paging.PageSize).Append(" OFFSET ").Append(currentItem);
                command.CommandText = commandText.ToString();
            }
            else
            {
                commandText.Append(" LIMIT 10");
                command.CommandText = commandText.ToString();
            }
        }

        private async Task<int> GetItemCountAsync(MobilePhoneFilter filter) 
        {
            NpgsqlConnection connection = new NpgsqlConnection(Constants.ConnectionString);
            using (connection)
            {
                NpgsqlCommand command = new NpgsqlCommand();
                command.CommandText = "SELECT COUNT(\"Id\") FROM \"MobilePhone\"";
                ApplyFilter(command, filter);
                command.Connection = connection;
                try
                {
                    await connection.OpenAsync();
                    NpgsqlDataReader reader = await command.ExecuteReaderAsync();
                    await reader.ReadAsync();
                    return reader.GetInt32(0);
                }
                catch (Exception e)
                {
                    return 0;
                }
                finally { 
                    await connection.CloseAsync(); 
                }
            }
        }
    }
}
