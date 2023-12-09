using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using StackExchange.Redis;
using TxAssigmentUnitTests.Mocks;
using TxAssignmentInfra.Connectors;
using TxAssignmentInfra.Entities;
using TxAssignmentInfra.Entities.Enumerators;
using TxAssignmentInfra.Repositories;
using TxAssignmentServices.Models;
using TxAssignmentServices.Profiles;
using TxAssignmentServices.Services;

namespace TxAssigmentUnitTests.Repositories
{
    [TestClass]
    public class RepositoryCabinetIntegrationTests
    {
        private RepositoryCabinet? _repository;
        private IDatabase? _database;
        private ServiceCabinet _service;
        private IMapper _mapper;
        private Mock<ILogger<ServiceCabinet>> _mockLogger;

        [TestInitialize]
        public void Setup()
        {
            _database = RedisConnectorHelper.Connection.GetDatabase();
            _repository = new RepositoryCabinet(_database);
            _mockLogger = new Mock<ILogger<ServiceCabinet>>();

            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProfileCabinet>();
            });
            _mapper = mapperConfiguration.CreateMapper();

            _service = new ServiceCabinet(_repository, _mapper, _mockLogger.Object);
        }

        [TestMethod]
        public async Task CreateCabinet_IntegrationTest()
        {
            // Arrange
            var Cabinet = new MockBuilderCabinet()
                                .BuildRow(1, 50, new Size { Height = 40 })
                                .BuildLane(1, 0)
                                .BuildProduct("4902102113126", "ポカリスエット/Pocari Sweat 900ml", 0.077, 0.077, 0.264, "https://operationmanagerstorage.blob.core.windows.net/skus/4987035332510_1659397549.jpg", 900, 1659397548, EnumProductShape.Bottle)
                                .BuildProduct("4902102113119", "クラフトボスレモンティー/Craft Boss Lemon Tea 600ml", 0.072, 0.072, 0.212, "https://operationmanagerstorage.blob.core.windows.net/skus/4901777375802_1659397549.jpg", 600, 1659397548, EnumProductShape.Bottle)
                                .Build();

            // Act
            var createResponse = await _repository.CreateCabinet(Cabinet);

            var retrieveResponse = await _repository.GetCabinetById(Cabinet.Id);

            // Assert
            Assert.IsTrue(createResponse.Success);
            Assert.AreEqual("Cabinet created successfully.", createResponse.Message);

            // Cleanup
            await _repository.DeleteCabinet(Cabinet.Id);
        }

        [TestMethod]
        public async Task TestAddProductToACabinetFromService_Test()
        {
            // Arrange
            var cabinet = new MockBuilderCabinet()
                                .BuildRow(1, 50, new Size { Height = 40 })
                                .BuildLane(1, 0)
                                .BuildProduct("4902102113140", "コカコーラ/Coca -Cola 1500ml", 0.097, 0.097, 0.308, "https://operationmanagerstorage.blob.core.windows.net/skus/4902102141109_1666091236.jpg", 1500, 1659397548, EnumProductShape.Bottle)
                                .BuildProduct("4902102113102", "クラフトボスフルーツティー/Craft boss fruit tea 600ml", 0.072, 0.072, 0.212, "https://operationmanagerstorage.blob.core.windows.net/skus/4901777375826_1685668163.jpg", 600, 1659397548, EnumProductShape.Bottle)
                                .Build();

            var newProduct = new MockBuilderProduct()
                            .WithJanCode("1238172783910921")
                            .WithDimensions(0.097, 0.308, 0.097)
                            .WithSize(1500)
                            .WithImageUrl("https://operationmanagerstorage.blob.core.windows.net/skus/4902102141109_1666091236.jpg")
                            .WithTimeStamp(1659397548)
                            .Build();

            //Act
            await _repository.CreateCabinet(cabinet);
            var cabinetResponse = await _repository.GetCabinetById(cabinet.Id);

            //Save the state for the cabinet
            var oldCabinet = cabinetResponse.Data;
            //Add the new product
            cabinet.Rows[0].Lanes[0].Products.Add(newProduct);
            //update cabinet
            var updateCabinet = await _service.UpdateCabinet(cabinet.Id, _mapper.Map<ModelCabinet>(cabinet));
            //Get cabinet updated
            cabinetResponse = await _repository.GetCabinetById(cabinet.Id);
            cabinet = cabinetResponse.Data;

            // Assert
            Assert.IsTrue(cabinetResponse.Success);
            Assert.IsTrue(updateCabinet.Success);
            Assert.IsNotNull(cabinet.Rows);
            Assert.IsNotNull(cabinet.Rows[0].Lanes);
            Assert.AreEqual(oldCabinet.Rows[0].Lanes[0].Products.Count, 2);
            Assert.AreEqual(cabinet.Rows[0].Lanes[0].Products.Count, 3);

            // Cleanup
            await _repository.DeleteCabinet(cabinet.Id);
        }

        [TestMethod]
        public async Task TestMoveProductToCorrectLane_Test()
        {
            // Arrange
            var cabinet = new MockBuilderCabinet()
                            .BuildRow(1, 50, new Size { Height = 40 })
                            .BuildLane(1, 5)
                            .BuildProduct("4902102113140", "コカコーラ/Coca -Cola 1500ml", 0.097, 0.097, 0.308, "https://operationmanagerstorage.blob.core.windows.net/skus/4902102141109_1666091236.jpg", 1500, 1659397548, EnumProductShape.Bottle)
                            .BuildProduct("4902102113133", "アクエリアス/Aquarius 950ml", 0.069, 0.069, 0.274, "https://operationmanagerstorage.blob.core.windows.net/skus/4902102137621_1682413847.jpg", 950, 1659397548, EnumProductShape.Bottle)
                            .BuildProduct("4902102113126", "ポカリスエット/Pocari Sweat 900ml", 0.077, 0.077, 0.264, "https://operationmanagerstorage.blob.core.windows.net/skus/4987035332510_1659397549.jpg", 900, 1659397548, EnumProductShape.Bottle)
                            .BuildProduct("4902102113119", "クラフトボスレモンティー/Craft Boss Lemon Tea 600ml", 0.072, 0.072, 0.212, "https://operationmanagerstorage.blob.core.windows.net/skus/4901777375802_1659397549.jpg", 600, 1659397548, EnumProductShape.Bottle)
                            .BuildProduct("4902102113102", "クラフトボスフルーツティー/Craft boss fruit tea 600ml", 0.072, 0.072, 0.212, "https://operationmanagerstorage.blob.core.windows.net/skus/4901777375826_1685668163.jpg", 600, 1659397548, EnumProductShape.Bottle)
                            .BuildLane(2, 10)
                            .BuildProduct("4902102113096", "ジョージアジャパンクラフトマンカフェラテ/Georgia Japan Craft Man Cafe Latte 500ml", 0.069, 0.069, 0.187, "https://operationmanagerstorage.blob.core.windows.net/skus/4902102127271_1663059645.jpg", 500, 1659397548, EnumProductShape.Bottle)
                            .BuildProduct("4902102113089", "ジョージアジャパンクラフトマンブラック/Georgia Japan Craft Man Black 500ml", 0.069, 0.069, 0.187, "https://operationmanagerstorage.blob.core.windows.net/skus/4902102127257_1663059586.jpg", 500, 1659397548, EnumProductShape.Bottle)
                            .BuildProduct("4902102113072", "クラフトボスブラック/Craft boss black 500ml", 0.069, 0.069, 0.198, "https://operationmanagerstorage.blob.core.windows.net/skus/4901777300521_1661735240.jpg", 500, 1659397548, EnumProductShape.Bottle)
                            .BuildProduct("4902102113065", "クラフトボス微糖/Craft Boss Sugar 500ml", 0.069, 0.069, 0.198, "https://operationmanagerstorage.blob.core.windows.net/skus/4901777349926_1661735380.jpg", 500, 1659397548, EnumProductShape.Bottle)
                            .BuildProduct("4902102113058", "小岩井theカフェオレ/Koiwai THE Cafe au lait 500ml", 0.071, 0.071, 0.181, "https://operationmanagerstorage.blob.core.windows.net/skus/4909411087555_1659397551.jpg", 500, 1659397548, EnumProductShape.Bottle)
                            .BuildProduct("4902102113041", "クラフトボスミルキープレッソビターラテ/Craft Boss Milky BitterLatte 300ml", 0.06, 0.06, 0.162, "https://operationmanagerstorage.blob.core.windows.net/skus/4901777375949_1666091268.jpg", 300, 1659397548, EnumProductShape.Can)
                            .BuildLane(3, 15)
                            .BuildProduct("4902102113027", "ワンダ極微糖/Wanda extra sugar 370g", 0.066, 0.066, 0.166, "https://operationmanagerstorage.blob.core.windows.net/skus/4514603345117_1677304138.jpg", 370, 1659397548, EnumProductShape.Can)
                            .BuildProduct("4902102113010", "ワンダ極カフェオレ/Wanda Extra Cafe au lait 370g", 0.066, 0.066, 0.166, "https://operationmanagerstorage.blob.core.windows.net/skus/4514603362619_1683445628.jpg", 370, 1659397548, EnumProductShape.Can)
                            .BuildProduct("4902102113003", "タリーズ無糖LATTE/Tully's sugar -free Latte370ml", 0.067, 0.067, 0.164, "https://operationmanagerstorage.blob.core.windows.net/skus/4901085613580_1679554937.jpg", 370, 1659397548, EnumProductShape.Can)
                            .BuildProduct("4902102112990", "タリーズBLACK/Tully's Black390ml", 0.067, 0.067, 0.164, "https://operationmanagerstorage.blob.core.windows.net/skus/4901085161982_1659397552.jpg", 390, 1659397548, EnumProductShape.Can)
                            .BuildProduct("4902102112983", "コーヒーと始める新習慣MCTLATTE砂糖不使用/coffee McTLATTE sugar free 270ml", 0.066, 0.066, 0.135, "https://operationmanagerstorage.blob.core.windows.net/skus/4901201147906_1659397553.jpg", 270, 1659397548, EnumProductShape.Can)
                            .BuildProduct("4902102112976", "UCCブラック無糖/Black sugar -free 185g", 0.053, 0.053, 0.105, "https://operationmanagerstorage.blob.core.windows.net/skus/4901201208096_1659397553.jpg", 185, 1659397548, EnumProductShape.Can)
                            .BuildProduct("4902102112969", "ワンダモーニングショット/Wanda Morning Shot 185g", 0.052, 0.052, 0.104, "https://operationmanagerstorage.blob.core.windows.net/skus/4514603284317_1671600046.jpg", 185, 1659397548, EnumProductShape.Can)
                            .BuildProduct("4902102112952", "ジョージアエメラルドマウンテンブレンド/Georgia Emerald Mountain Blend 185g", 0.052, 0.052, 0.105, "https://operationmanagerstorage.blob.core.windows.net/skus/4902102107341_1683444986.jpg", 185, 1659397548, EnumProductShape.Can)
                            .BuildProduct("4902102112945", "ボスカフェオレ/Boss Cafe au lait 185g", 0.053, 0.053, 0.105, "https://operationmanagerstorage.blob.core.windows.net/skus/4901777235434_1659397554.jpg", 185, 1659397548, EnumProductShape.Can)
                            .BuildProduct("4902102112938", "ファイア挽きたて微糖/Fire freshly brewed sugar 185g", 0.053, 0.053, 0.105, "https://operationmanagerstorage.blob.core.windows.net/skus/4909411083342_1659397554.jpg", 185, 1659397548, EnumProductShape.Can)
                            .BuildProduct("4902102112921", "ボスレインボーマウンテンブレンド1/Boss Rainbow Mountain Blend 185g", 0.053, 0.053, 0.105, "https://operationmanagerstorage.blob.core.windows.net/skus/4901777235298_1684221096.jpg", 185, 1659397548, EnumProductShape.Can)
                            .BuildLane(4, 20)
                            .BuildProduct("4902102112778", "午後の紅茶レモンティー/Afternoon tea lemon 500ml", 0.059, 0.059, 0.217, "https://operationmanagerstorage.blob.core.windows.net/skus/4909411084875_1685061792.jpg", 500, 1659397548, EnumProductShape.Bottle)
                            .BuildProduct("4902102112761", "午後の紅茶ミルクティー/Afternoon tea milk 500ml", 0.059, 0.059, 0.217, "https://operationmanagerstorage.blob.core.windows.net/skus/4909411084950_1685061819.jpg", 500, 1659397548, EnumProductShape.Bottle)
                            .BuildProduct("4902102112754", "紅茶花伝ロイヤルミルクティー/Kouchakaden Royal Milk 440ml", 0.069, 0.069, 0.174, "https://operationmanagerstorage.blob.core.windows.net/skus/4902102136716_1659397560.jpg", 440, 1659397548, EnumProductShape.Bottle)
                            .BuildProduct("4902102112747", "伊右衛門カフェジャスミンティーラテ/Cafe Jasmine Tea latte 500ml", 0.072, 0.072, 0.177, "https://operationmanagerstorage.blob.core.windows.net/skus/4901777381254_1659397560.jpg", 500, 1659397548, EnumProductShape.Bottle)
                            .BuildProduct("4902102112730", "ウェルチ１房分のぶどう/welch 1 bunch 470ml", 0.072, 0.072, 0.173, "https://operationmanagerstorage.blob.core.windows.net/skus/4901340064041_1659397560.jpg", 470, 1659397548, EnumProductShape.Bottle)
                            .BuildProduct("4902102112723", "トロピカーナ100%オレンジ/Tropicana 100%orange 330ml", 0.06, 0.06, 0.184, "https://operationmanagerstorage.blob.core.windows.net/skus/4909411073114_1659397561.jpg", 330, 1659397548, EnumProductShape.Bottle)
                            .BuildProduct("4902102112716", "サントリー天然水/Suntory Natural Water 2000ml", 0.091, 0.106, 0.311, "https://operationmanagerstorage.blob.core.windows.net/skus/4901777018686_1679809773.jpg", 2000, 1659397548, EnumProductShape.Bottle)
                            .BuildProduct("4902102112709", "エビアン/Evian750ml", 0.082, 0.082, 0.21, "https://operationmanagerstorage.blob.core.windows.net/skus/3068320116266_1659397561.jpg", 750, 1659397548, EnumProductShape.Bottle)
                            .BuildProduct("4902102112693", "天然水きりっと果実オレンジ＆マンゴー/Dried fruit orange & mango Water600ml", 0.068, 0.068, 0.224, "https://operationmanagerstorage.blob.core.windows.net/skus/4901777375901_1678168618.jpg", 600, 1659397548, EnumProductShape.Bottle)
                            .Build();

            var newProduct = new MockBuilderProduct()
                            .WithJanCode("4902102113140")
                            .WithName("コカコーラ/Coca -Cola 1500ml")
                            .WithDimensions(0.097, 0.308, 0.097)
                            .WithSize(1500)
                            .WithImageUrl("https://operationmanagerstorage.blob.core.windows.net/skus/4902102141109_1666091236.jpg")
                            .WithTimeStamp(1659397548)
                            .Build();

            //Act
            await _repository.CreateCabinet(cabinet);
            var cabinetResponse = await _repository.GetCabinetById(cabinet.Id);

            //Save the state for the cabinet
            var oldCabinet = cabinetResponse.Data;
            //Add the new product
            cabinet.Rows[0].Lanes[2].Products.Add(newProduct);
            //update cabinet
            var updateCabinet = await _service.UpdateCabinet(cabinet.Id, _mapper.Map<ModelCabinet>(cabinet));
            //Get cabinet updated
            cabinetResponse = await _repository.GetCabinetById(cabinet.Id);
            cabinet = cabinetResponse.Data;

            // Assert
            Assert.IsTrue(cabinetResponse.Success);
            Assert.IsTrue(updateCabinet.Success);
            Assert.IsNotNull(cabinet.Rows);
            Assert.IsNotNull(cabinet.Rows[0].Lanes);
            Assert.AreEqual(oldCabinet.Rows[0].Lanes[0].Products.Count, 5);
            Assert.AreEqual(oldCabinet.Rows[0].Lanes[0].Products.Count, 5);
            Assert.AreEqual(oldCabinet.Rows[0].Lanes[2].Products.Count, 11);
            Assert.AreEqual(cabinet.Rows[0].Lanes[2].Products.Count, 11);

            // Cleanup
            await _repository.DeleteCabinet(cabinet.Id);
        }
    }
}
