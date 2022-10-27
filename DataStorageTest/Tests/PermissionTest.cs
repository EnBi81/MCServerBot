using DataStorage.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStorageTest.Tests
{
    [TestClass]
    public class PermissionTest
    {
        [ClassInitialize]
        public static async Task ClassInitialize(TestContext testContext)
        {
            await Initializing.TestSubject.DatabaseSetup.ResetDatabase();
            await Initializing.TestSubject.DiscordEventRegister.RegisterDiscordUser(123, "Username1", "profPic1", "abcdefghijklmno1");
            await Initializing.TestSubject.DiscordEventRegister.RegisterDiscordUser(456, "Username2", "profPic2", "abcdefghijklmno2");
        }



        private IDiscordDatabaseAccess discordEventRegister = null!;
        private IWebsiteEventRegister websiteEventRegister = null!;


        [TestInitialize]
        public void TestInitialize()
        {
            discordEventRegister = Initializing.TestSubject.DiscordEventRegister;
            websiteEventRegister = Initializing.TestSubject.WebsiteEventRegister;
        }


        [TestMethod]
        public async Task GrantAccess()
        {
            await discordEventRegister.GrantPermission(1, 123);

            bool hasPermission = await discordEventRegister.HasPermission(123);
            Assert.IsTrue(hasPermission);

            bool hasPermissionWeb = await websiteEventRegister.HasPermission("abcdefghijklmno1");
            Assert.IsTrue(hasPermissionWeb);
        }


        [TestMethod]
        public async Task RevokeAccess()
        {
            await discordEventRegister.RevokePermission(1, 123);

            bool hasPermission = await discordEventRegister.HasPermission(123);
            Assert.IsFalse(hasPermission);

            bool hasPermissionWeb = await websiteEventRegister.HasPermission("abcdefghijklmno1");
            Assert.IsFalse(hasPermissionWeb);
        }

        [TestMethod]
        public async Task GiveMixedAccess()
        {
            await discordEventRegister.RevokePermission(1, 123);
            await discordEventRegister.GrantPermission(1, 456);

            bool hasPermission123 = await discordEventRegister.HasPermission(123);
            bool hasPermission456 = await discordEventRegister.HasPermission(456);

            Assert.IsFalse(hasPermission123);
            Assert.IsTrue(hasPermission456);
        }
    }
}
