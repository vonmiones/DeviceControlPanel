using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using Dapper;
using System.IO;

namespace DeviceControlPanel.Classes
{
    public class CRUDDevice
    {
        private string connectionString;

        public CRUDDevice(string dbPath)
        {
            connectionString = $"Data Source={dbPath};Version=3;";
            CreateDatabaseAndTableIfNotExists(dbPath);
        }

        private void CreateDatabaseAndTableIfNotExists(string dbPath)
        {
            if (!Directory.Exists(@"data"))
            {
                Directory.CreateDirectory(@"data");
                
            }
            if (!File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile(dbPath);
            }
            using (IDbConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Create the database file if it doesn't exist
                SQLiteConnectionStringBuilder connectionStringBuilder = new SQLiteConnectionStringBuilder(connectionString);
                string databasePath = connectionStringBuilder.DataSource;

                // Create the Devices table if it doesn't exist
                string createTableQuery = @"CREATE TABLE IF NOT EXISTS Devices (
                                                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                                DeviceName TEXT,
                                                Description TEXT,
                                                Alias TEXT,
                                                Ip TEXT,
                                                Port INTEGER,
                                                Serial TEXT,
                                                Tag TEXT,
                                                MachineId INTEGER,
                                                Model TEXT,
                                                Processor TEXT,
                                                Version TEXT,
                                                Kind TEXT,
                                                CategoryId INTEGER,
                                                TypeId INTEGER,
                                                DtCreated DATETIME,
                                                DtUpdated DATETIME,
                                                DtDeleted DATETIME,
                                                Status TEXT,
                                                Remarks TEXT
                                            );";
                connection.Execute(createTableQuery);
            }
        }

        public void Insert(Device device)
        {
            using (IDbConnection connection = new SQLiteConnection(connectionString))
            {
                string query = @"INSERT INTO Devices (DeviceName, Description, Alias, Ip, Port, Serial, Tag, MachineId, Model, Processor, Version, Kind, CategoryId, TypeId, DtCreated, DtUpdated, DtDeleted, Status, Remarks)
                                 VALUES (@DeviceName, @Description, @Alias, @Ip, @Port, @Serial, @Tag, @MachineId, @Model, @Processor, @Version, @Kind, @CategoryId, @TypeId, @DtCreated, @DtUpdated, @DtDeleted, @Status, @Remarks)";

                device.DtCreated = DateTime.Now;
                device.DtUpdated = DateTime.Now;

                connection.Execute(query, device);
            }
        }

        public void Update(Device device)
        {
            using (IDbConnection connection = new SQLiteConnection(connectionString))
            {
                string query = @"UPDATE Devices
                                 SET DeviceName  = @DeviceName ,
                                     Description = @Description,
                                     Alias = @Alias,
                                     Ip = @Ip,
                                     Port = @Port,
                                     Serial = @Serial,
                                     Tag = @Tag,
                                     MachineId = @MachineId,
                                     Model = @Model,
                                     Processor = @Processor,
                                     Version = @Version,
                                     Kind = @Kind,
                                     CategoryId = @CategoryId,
                                     TypeId = @TypeId,
                                     DtUpdated = @DtUpdated,
                                     Status = @Status,
                                     Remarks = @Remarks
                                 WHERE Id = @Id";

                device.DtUpdated = DateTime.Now;

                connection.Execute(query, device);
            }
        }

        public void Delete(int id)
        {
            using (IDbConnection connection = new SQLiteConnection(connectionString))
            {
                string query = "DELETE FROM Devices WHERE Id = @Id";

                connection.Execute(query, new { Id = id });
            }
        }

        public Device GetById(int id)
        {
            using (IDbConnection connection = new SQLiteConnection(connectionString))
            {
                string query = "SELECT * FROM Devices WHERE Id = @Id";

                return connection.QueryFirstOrDefault<Device>(query, new { Id = id });
            }
        }

        public List<Device> GetAll()
        {
            using (IDbConnection connection = new SQLiteConnection(connectionString))
            {
                string query = "SELECT * FROM Devices";

                return connection.Query<Device>(query).AsList();
            }
        }
    }
}

