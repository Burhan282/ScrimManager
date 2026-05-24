using ScrimManagerApplication.Application;
using ScrimManagerApplication.Application.Models;
using ScrimManagerDataAcces.DataAcces.FakeDataBases;

namespace ScrimManagerTests
{
    [TestClass]
    public class AuthServiceTests
    {
        [TestMethod]
        public void Register_ShouldAddUser()
        {
            // Arrange
            FakeUserRepository fakeRepository = new FakeUserRepository();
            AuthService authService = new AuthService(fakeRepository);

            // Act
            authService.Register(
                "Burhan",
                "burhan@mail.com",
                "1234",
                "Player",
                Rank.GoldIII);

           
            List<User> users = fakeRepository.GetAll();

            Assert.HasCount(1, users);
            Assert.AreEqual("Burhan", users[0].Username);
        }

        [TestMethod]
        public void Login_WithCorrectData_ShouldReturnUser()
        {
            
            FakeUserRepository fakeRepository = new FakeUserRepository();
            AuthService authService = new AuthService(fakeRepository);

            authService.Register(
                "Burhan",
                "burhan@mail.com",
                "1234",
                "Player",
                Rank.GoldIII);

           
            User? user = authService.Login(
                "burhan@mail.com",
                "1234");

         
            Assert.IsNotNull(user);
            Assert.AreEqual("Burhan", user.Username);
        }

        [TestMethod]
        public void Login_WithWrongData_ShouldReturnNull()
        {
           
            FakeUserRepository fakeRepository = new FakeUserRepository();
            AuthService authService = new AuthService(fakeRepository);

            authService.Register(
                "Burhan",
                "burhan@mail.com",
                "1234",
                "Player",
                Rank.GoldIII);

            User? user = authService.Login(
                "burhan@mail.com",
                "wrongpassword");

            Assert.IsNull(user);
        }
    }
}