using DataStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStorageTest
{
    [TestClass]
    public class Initializing
    {
        public static DatabaseAccess TestSubject { get; } = DatabaseAccess.SQLite;

        [AssemblyInitialize]
        public static async Task AssemblyInitialize(TestContext testContext)
        {
            await TestSubject.Setup("Data Source=C:\\Users\\enbi8\\source\\repos\\MCServerBot\\DataStorageTest\\test.db;Version=3;");
        }
    }
}
