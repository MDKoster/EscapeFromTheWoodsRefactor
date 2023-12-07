using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeFromTheWoods.Database {
    public class DBMonkeyRecordSet {
        public DBMonkeyRecordSet(List<DBMonkeyRecord> monkeyRecords) {
            MonkeyRecords = monkeyRecords;
        }

        public List<DBMonkeyRecord> MonkeyRecords { get; set; }
    }
}
