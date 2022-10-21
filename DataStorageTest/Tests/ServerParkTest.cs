using DataStorage.DataObjects;
using DataStorage.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStorageTest.Tests
{
    [TestClass]
    public class ServerParkTest
    {
        [ClassInitialize]
        public static async Task ClassInitialize(TestContext testContext)
        {
            await Initializing.TestSubject.DatabaseSetup.ResetDatabase();
            await Initializing.TestSubject.ServerParkEventRegister.CreateServer(321, "Created server name", Data);
        }



        private IServerParkEventRegister serverParkEventRegister = null!;
        private static UserEventData Data = new UserEventData
        {
            Id = 1,
            Username = "System",
            Platform = DataStorage.DataObjects.Enums.Platform.Discord
        };


        [TestInitialize]
        public void TestInitialize()
        {
            serverParkEventRegister = Initializing.TestSubject.ServerParkEventRegister;
        }



        [TestMethod]
        public async Task AddMinecraftServer()
        {
            string? name = await serverParkEventRegister.GetServerName(321);

            Assert.AreEqual("Created server name", name);
        }

        [TestMethod]
        public async Task RenameMinecraftServer()
        {
            await serverParkEventRegister.RenameServer(321, "Renamed", Data);

            string? name = await serverParkEventRegister.GetServerName(321);
            Assert.AreEqual("Renamed", name);
        }

        [TestMethod]
        public async Task DeleteMinecraftServer()
        {
            await serverParkEventRegister.DeleteServer(321, Data);

            string? name = await serverParkEventRegister.GetServerName(321);
            Assert.IsNull(name);
        }
    }
}
