using Application.DAOs.Database;
using SharedPublic.DTOs;
using SharedPublic.DTOs.Enums;

namespace DataStorageTest.Tests
{
    [TestClass]
    public class ServerParkTest
    {
        [ClassInitialize]
        public static async Task ClassInitialize(TestContext testContext)
        {
            await Initializing.TestSubject.DatabaseSetup.ResetDatabase();
            await Initializing.TestSubject.ServerParkDataAccess.CreateServer(321, "Created server name", Data);
        }



        private IServerParkDataAccess serverParkDataAccess = null!;
        private static UserEventData Data = default;


        [TestInitialize]
        public void TestInitialize()
        {
            serverParkDataAccess = Initializing.TestSubject.ServerParkDataAccess;
        }



        [TestMethod]
        public async Task AddMinecraftServer()
        {
            string? name = await serverParkDataAccess.GetServerName(321);

            Assert.AreEqual("Created server name", name);
        }

        [TestMethod]
        public async Task RenameMinecraftServer()
        {
            await serverParkDataAccess.RenameServer(321, "Renamed", Data);

            string? name = await serverParkDataAccess.GetServerName(321);
            Assert.AreEqual("Renamed", name);
        }

        [TestMethod]
        public async Task DeleteMinecraftServer()
        {
            await serverParkDataAccess.DeleteServer(321, Data);

            string? name = await serverParkDataAccess.GetServerName(321);
            Assert.IsNull(name);
        }
    }
}
