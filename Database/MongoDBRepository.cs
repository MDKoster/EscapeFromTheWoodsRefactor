using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace EscapeFromTheWoods.Database {
    public class MongoDBRepository {
        private IMongoClient _client;
        private IMongoDatabase _database;
        private string _connectionString;

        public MongoDBRepository(IMongoClient client, IMongoDatabase database, string connectionString) {
            _connectionString = connectionString;
            _client = new MongoClient(_connectionString);
            _database = _client.GetDatabase("EscapeFromTheWoods");
        }

        public void WriteWoodRecords(List<DBWoodRecordSet> data) {
            var collection = _database.GetCollection<DBWoodRecordSet>("WoodRecords");
            collection.InsertMany(data);
        }

        public void WriteMonkeyRecords(List<DBMonkeyRecordSet> data) {
            var collection = _database.GetCollection<DBMonkeyRecordSet>("MonkeyRecords");
            collection.InsertMany(data);
        }

        //public void WriteLogs(List<string>) {

        //}
    }
}
