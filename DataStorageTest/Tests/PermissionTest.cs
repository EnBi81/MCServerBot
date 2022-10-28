using Application.DAOs.Database;
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
            await Initializing.TestSubject.PermissionDataAccess.RegisterDiscordUser(123, "Username1", "profPic1", "abcdefghijklmno1");
            await Initializing.TestSubject.PermissionDataAccess.RegisterDiscordUser(456, "Username2", "profPic2", "abcdefghijklmno2");
        }


        private IPermissionDataAccess permissionDataAccess = null!;


        [TestInitialize]
        public void TestInitialize()
        {
            permissionDataAccess = Initializing.TestSubject.PermissionDataAccess;
        }


        [TestMethod]
        public async Task RegisterUlongMaxValue()
        {
            await permissionDataAccess.RegisterDiscordUser(ulong.MaxValue, "TestUsernameMax", "ProfPicMax", "accessTokenMax");
            var user = await permissionDataAccess.GetUser(ulong.MaxValue);

            Assert.IsNotNull(user);
            Assert.AreEqual(ulong.MaxValue, user.Id);
            Assert.AreEqual("TestUsernameMax", user.Username);
            Assert.AreEqual("ProfPicMax", user.ProfilePicUrl);
            Assert.AreEqual("accessTokenMax", user.WebAccessToken);
        }

        [TestMethod]
        public async Task RegisterUlongMinValue()
        {
            await permissionDataAccess.RegisterDiscordUser(ulong.MinValue, "TestUsernameMin", "ProfPicMin", "accessTokenMin");
            var user = await permissionDataAccess.GetUser(ulong.MinValue);

            Assert.IsNotNull(user);
            Assert.AreEqual(ulong.MinValue, user.Id);
            Assert.AreEqual("TestUsernameMin", user.Username);
            Assert.AreEqual("ProfPicMin", user.ProfilePicUrl);
            Assert.AreEqual("accessTokenMin", user.WebAccessToken);
        }

        [TestMethod]
        public async Task GrantAccess()
        {
            await permissionDataAccess.GrantPermission(1, 123);

            bool hasPermission = await permissionDataAccess.HasPermission(123);
            Assert.IsTrue(hasPermission);

            bool hasPermissionWeb = await permissionDataAccess.HasPermission("abcdefghijklmno1");
            Assert.IsTrue(hasPermissionWeb);
        }


        [TestMethod]
        public async Task RevokeAccess()
        {
            await permissionDataAccess.RevokePermission(1, 123);

            bool hasPermission = await permissionDataAccess.HasPermission(123);
            Assert.IsFalse(hasPermission);

            bool hasPermissionWeb = await permissionDataAccess.HasPermission("abcdefghijklmno1");
            Assert.IsFalse(hasPermissionWeb);
        }

        [TestMethod]
        public async Task GiveMixedAccess()
        {
            await permissionDataAccess.RevokePermission(1, 123);
            await permissionDataAccess.GrantPermission(1, 456);

            bool hasPermission123 = await permissionDataAccess.HasPermission(123);
            bool hasPermission456 = await permissionDataAccess.HasPermission(456);

            Assert.IsFalse(hasPermission123);
            Assert.IsTrue(hasPermission456);
        }
    }
}
