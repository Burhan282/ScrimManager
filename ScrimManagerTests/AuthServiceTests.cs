using ScrimManagerApplication.Application;
using ScrimManagerApplication.Application.Models;
using ScrimManagerApplication.Application.Models.DTOModels;
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
            authService.Register(CreateUser());

           
            List<User> users = fakeRepository.GetAll();

            Assert.HasCount(1, users);
            Assert.AreEqual("Burhan", users[0].Username);
        }

        [TestMethod]
        public void Login_WithCorrectData_ShouldReturnUser()
        {
            
            FakeUserRepository fakeRepository = new FakeUserRepository();
            AuthService authService = new AuthService(fakeRepository);

            authService.Register(CreateUser());

           
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

            authService.Register(CreateUser());

            User? user = authService.Login(
                "burhan@mail.com",
                "wrongpassword");

            Assert.IsNull(user);
        }

        private static CreateUserDTO CreateUser()
        {
            return new CreateUserDTO
            {
                Username = "Burhan",
                Email = "burhan@mail.com",
                PasswordHash = "1234",
                UserRole = Role.Player,
                UserRank = Rank.GoldIII
            };
        }
    }
}
