using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using dotnetapp.Models;
using System.Reflection;
using dotnetapp.Services;
using Microsoft.EntityFrameworkCore;

namespace dotnetapp.Tests
{
    [TestFixture]
    public class InventoryControllerTests
    {
        private HttpClient _httpClient;
        private Assembly _assembly;
        private InventoryItem _testInventoryItem;
        private DbContextOptions<ApplicationDbContext> _dbContextOptions;

        [SetUp]
        public async Task Setup()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://localhost:8080"); // Base URL of your API
            _assembly = Assembly.GetAssembly(typeof(InventoryService));

            // Set up in-memory database options
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            // Create a new test item before each test case
            _testInventoryItem = await CreateTestInventoryItem();
        }

        private async Task<InventoryItem> CreateTestInventoryItem()
        {
            var newInventoryItem = new InventoryItem
            {
                ItemName = "Test Item",
                Quantity = 10,
                Price = 9.99m,
                Category = "Test Category"
            };

            var json = JsonConvert.SerializeObject(newInventoryItem);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/inventory", content);
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<InventoryItem>(await response.Content.ReadAsStringAsync());
        }

        [Test]
        public async Task Test_GetAllInventories_ReturnsListOfInventories()
        {
            // Act
            var response = await _httpClient.GetAsync("api/inventory");
            response.EnsureSuccessStatusCode();

            // Assert
            var content = await response.Content.ReadAsStringAsync();
            var items = JsonConvert.DeserializeObject<InventoryItem[]>(content);

            Assert.IsNotNull(items);
            Assert.IsTrue(items.Length > 0);
        }

        [Test]
        public async Task Test_GetInventoryById_ValidId_ReturnsItem()
        {
            // Act
            var response = await _httpClient.GetAsync($"api/inventory/{_testInventoryItem.ItemId}");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var inventory = JsonConvert.DeserializeObject<InventoryItem>(content);

            Assert.IsNotNull(inventory);
            Assert.AreEqual(_testInventoryItem.ItemId, inventory.ItemId);
        }

        [Test]
        public async Task Test_GetInventoryById_InvalidId_ReturnsNotFound()
        {
            // Act
            var response = await _httpClient.GetAsync("api/inventory/999999");

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Test]
        public void Test_InventoryService_Exists()
        {
            AssertServiceInstanceNotNull("InventoryService");
        }

        private void AssertServiceInstanceNotNull(string serviceName)
        {
            Type serviceType = _assembly.GetType($"dotnetapp.Services.{serviceName}");

            if (serviceType == null)
            {
                Assert.Fail($"Service {serviceName} does not exist.");
            }

            object dbContextInstance = Activator.CreateInstance(typeof(ApplicationDbContext), _dbContextOptions);
            object serviceInstance = Activator.CreateInstance(serviceType, dbContextInstance);

            Assert.IsNotNull(serviceInstance);
        }

        [Test]
        public void Test_InventoryController_Exists()
        {
            AssertControllerClassExists("InventoryController");
        }

        private void AssertControllerClassExists(string controllerName)
        {
            Type controllerType = _assembly.GetType($"dotnetapp.Controller.{controllerName}");

            if (controllerType == null)
            {
                Assert.Fail($"Controller {controllerName} does not exist.");
            }
        }

        [TearDown]
        public async Task Cleanup()
        {
            _httpClient.Dispose();
        }
    }
}
