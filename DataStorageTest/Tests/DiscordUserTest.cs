using DataStorage.Interfaces;

namespace DataStorageTest.Tests
{
    [TestClass]
    public class DiscordUserTest
    {
        [ClassInitialize]
        public static async Task ClassInitialize(TestContext testContext)
        {
            await Initializing.TestSubject.DatabaseSetup.ResetDatabase();
        }



        private IDiscordEventRegister discordEventRegister = null!;


        [TestInitialize]
        public void TestInitialize()
        {
            discordEventRegister = Initializing.TestSubject.DiscordEventRegister;
        }


        [TestMethod]
        public async Task RegisterSimpleDiscordUser()
        {
            await discordEventRegister.RegisterDiscordUser(123, "TestUsername", "ProfPic", "accessToken");
            var user = await discordEventRegister.GetUser(123);

            Assert.IsNotNull(user);
            Assert.AreEqual<ulong>(123, user.Id);
            Assert.AreEqual("TestUsername", user.Username);
            Assert.AreEqual("ProfPic", user.ProfilePicUrl);
            Assert.AreEqual("accessToken", user.WebAccessToken);
        }

        [TestMethod]
        public async Task RegisterUlongMaxValue()
        {
            await discordEventRegister.RegisterDiscordUser(ulong.MaxValue, "TestUsernameMax", "ProfPicMax", "accessTokenMax");
            var user = await discordEventRegister.GetUser(ulong.MaxValue);

            Assert.IsNotNull(user);
            Assert.AreEqual(ulong.MaxValue, user.Id);
            Assert.AreEqual("TestUsernameMax", user.Username);
            Assert.AreEqual("ProfPicMax", user.ProfilePicUrl);
            Assert.AreEqual("accessTokenMax", user.WebAccessToken);
        }

        [TestMethod]
        public async Task RegisterUlongMinValue()
        {
            await discordEventRegister.RegisterDiscordUser(ulong.MinValue, "TestUsernameMin", "ProfPicMin", "accessTokenMin");
            var user = await discordEventRegister.GetUser(ulong.MinValue);

            Assert.IsNotNull(user);
            Assert.AreEqual(ulong.MinValue, user.Id);
            Assert.AreEqual("TestUsernameMin", user.Username);
            Assert.AreEqual("ProfPicMin", user.ProfilePicUrl);
            Assert.AreEqual("accessTokenMin", user.WebAccessToken);
        }
    }
}