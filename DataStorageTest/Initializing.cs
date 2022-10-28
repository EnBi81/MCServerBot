using Application.DAOs;

namespace DataStorageTest
{
    [TestClass]
    public class Initializing
    {
        public static IDatabaseAccess TestSubject { get; } = new DataStorageSQLite.Implementation.DataStorageSQLiteImpl();

        [AssemblyInitialize]
        public static async Task AssemblyInitialize(TestContext testContext)
        {
            await TestSubject.DatabaseSetup.Setup("Data Source=C:\\Users\\enbi8\\source\\repos\\MCServerBot\\DataStorageTest\\test.db;Version=3;");
        }
    }
}
