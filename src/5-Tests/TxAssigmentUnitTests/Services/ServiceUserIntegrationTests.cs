using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AutoMapper;
using Moq;
using Microsoft.Extensions.Logging;
using TxAssignmentInfra.Repositories;
using TxAssignmentServices.Strategies.User;
using TxAssignmentServices.Services;
using TxAssignmentServices.Profiles;
using TxAssignmentServices.Strategies.Products;
using Microsoft.Extensions.Configuration;
using TxAssignmentServices.Models;
using StackExchange.Redis;
using TxAssignmentInfra.Connectors;
using Microsoft.AspNetCore.Identity;

namespace TxAssigmentUnitTests.Services
{
    [TestClass]
    public class ServiceUserIntegrationTests
    {
        private ServiceUser _serviceUser;
        private IMapper _mapper;
        private Mock<ILogger<ServiceUser>> _mockLogger;
        private IStrategyCreateUserOperation _strategyCreateUserOperation;
        private IStrategyLoginUserOperation _strategyLoginUserOperation;
        private IDatabase? _database;
        private IRepositoryUser _repositoryUser;

        private PasswordHasher<ModelUser> _passwordHasherModel;
        private PasswordHasher<TxAssignmentInfra.Entities.User> _passwordHasherEntity;


        [TestInitialize]
        public void Setup()
        {
            _database = RedisConnectorHelper.Connection.GetDatabase();
            _repositoryUser = new RepositoryUser(_database);
            var services = new ServiceCollection();
            // Set up AutoMapper
            var mapperConfiguration = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new ProfileCabinet());
                mc.AddProfile(new ProfileProduct());
                mc.AddProfile(new ProfileUser());
            });

            _mapper = mapperConfiguration.CreateMapper();
            _mockLogger = new Mock<ILogger<ServiceUser>>();


            var mockLoggerCreate = new Mock<ILogger<StrategyCreateUserOperation>>();
            var mockLoggerLogin = new Mock<ILogger<StrategyLoginUserOperation>>();

            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.SetupGet(c => c[It.Is<string>(s => s == "Jwt:Key")]).Returns("b71c35ef-659d-4f12-b19f-2583313c3233");
            mockConfiguration.SetupGet(c => c[It.Is<string>(s => s == "Jwt:Issuer")]).Returns("com.fabio.txassignment");
            mockConfiguration.SetupGet(c => c[It.Is<string>(s => s == "Jwt:Audience")]).Returns("tx.recruiters");

            _passwordHasherModel = new PasswordHasher<ModelUser>();
            _passwordHasherEntity = new PasswordHasher<TxAssignmentInfra.Entities.User>();


            _strategyCreateUserOperation = new StrategyCreateUserOperation(_repositoryUser, _mapper, mockLoggerCreate.Object, _passwordHasherModel);
            _strategyLoginUserOperation = new StrategyLoginUserOperation(_repositoryUser, _mapper, mockLoggerLogin.Object, mockConfiguration.Object, _passwordHasherEntity);
            _serviceUser = new ServiceUser(_repositoryUser, _mapper, _mockLogger.Object, _strategyCreateUserOperation, _strategyLoginUserOperation);
        }


        [TestMethod]
        public async Task CreateUserAsync_WhenCalled_ShouldCreateUser()
        {
            // Arrange
            var modelUser = new ModelUser
            {
                Email = "testuser@example.com",
                Password = "Password123!"
            };

            // Act
            var createUserResponse = await _serviceUser.CreateUserAsync(modelUser);

            // Assert
            Assert.IsTrue(createUserResponse.Success);
            Assert.AreEqual("User created successfully.", createUserResponse.Message);

            // Cleanup
            var userResponse = await _repositoryUser.GetUserByEmail(modelUser.Email);
            if (userResponse.Data != null)
            {
                await _repositoryUser.DeleteUser(userResponse.Data.Id);
            }
        }


        [TestMethod]
        public async Task LoginAsync_WhenCalledWithValidCredentials_ShouldReturnToken()
        {
            // Arrange
            var modelUser = new ModelUser
            {
                Email = "testlogin@example.com",
                Password = "LoginPassword123!"
            };
            await _serviceUser.CreateUserAsync(modelUser);

            // Act
            var loginResponse = await _serviceUser.LoginAsync(modelUser.Email, modelUser.Password);

            // Assert
            Assert.IsTrue(loginResponse.Success);
            Assert.IsNotNull(loginResponse.Data.Token);

            // Cleanup
            var userResponse = await _repositoryUser.GetUserByEmail(modelUser.Email);
            if (userResponse.Data != null)
            {
                await _repositoryUser.DeleteUser(userResponse.Data.Id);
            }
        }


        [TestMethod]
        public async Task GetUserByEmailAsync_WhenCalled_ShouldReturnUser()
        {
            // Arrange
            var modelUser = new ModelUser
            {
                Email = "testget@example.com",
                Password = "GetUserPassword123!"
            };
            await _serviceUser.CreateUserAsync(modelUser);

            // Act
            var getUserResponse = await _serviceUser.GetUserByEmailAsync(modelUser.Email);

            // Assert
            Assert.IsTrue(getUserResponse.Success);
            Assert.AreEqual(modelUser.Email, getUserResponse.Data.Email);

            // Cleanup
            var userResponse = await _repositoryUser.GetUserByEmail(modelUser.Email);
            if (userResponse.Data != null)
            {
                await _repositoryUser.DeleteUser(userResponse.Data.Id);
            }
        }
    }
}
