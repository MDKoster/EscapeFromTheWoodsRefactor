using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeFromTheWoods.Database {
    public class DBWoodRecordSet {
        public DBWoodRecordSet(List<DBWoodRecord> woodRecords) {
            WoodRecords = woodRecords;
        }

        public List<DBWoodRecord> WoodRecords { get; set; }
    }
}
