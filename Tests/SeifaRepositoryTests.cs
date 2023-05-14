using System.Collections;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

using NuGet.ContentModel;
using NUnit.Framework;
using Seifa2011_2016_API.Interfaces;
using Seifa2011_2016_API.Models;
using Seifa2011_2016_API.Repositories;
using System.Dynamic;

namespace Seifa2011_2016_API.Tests
{
    [TestFixture]
    public class SeifaRepositoryTests
    {
        private TestDbContext _dbContext;
        private SeifaRepository _seifaRepository;

        [SetUp]
        public void Setup()
        {

            // Create a fresh service provider, and therefore a fresh 
            // InMemory database instance.
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();


            // Initialize a new instance of the in-memory TestDbContext
            var options = new DbContextOptionsBuilder<TestDbContext>();


                options.UseInMemoryDatabase(databaseName: "TestDatabase")
                .UseInternalServiceProvider(serviceProvider);

                var dbOptionsBuilder = GetDbOptionsBuilder();

            _dbContext = new TestDbContext(dbOptionsBuilder.Options);

            // Create an instance of the SeifaRepository using the TestDbContext
            _seifaRepository = new SeifaRepository(_dbContext);

            // Seed the in-memory database with test data
            SeedTestData();


        }

        [Test]
        public async Task GetDistinctLocalGovtAreas_ShouldReturnDistinctLocalGovtAreas()
        {
            // Act
            var result = await _seifaRepository.GetDistinctLocalGovtAreas();

            // Assert
            Assert.That(result, Is.EquivalentTo(new[] { "State1", "State2", "State3" }));
        }

        [Test]
        public async Task GetAllSeifaRecordsForEachState_ShouldReturnAllSeifaRecordsForState()
        {
            // Arrange
            var state = "State1";

            // Act
            var result = await _seifaRepository.GetAllSeifaRecordsForEachState(state);

            // Assert
            var expected = new[]
            {
            new { Year = "2011", StateName = "State1", PlaceName = "Place1", DisadvantageScore = 1, AdvantageScore = 2 },
            new { Year = "2011", StateName = "State1", PlaceName = "Place2", DisadvantageScore = 3, AdvantageScore = 4 },
            new { Year = "2016", StateName = "State1", PlaceName = "Place1", DisadvantageScore = 1, AdvantageScore = 1 }
        };
            Assert.AreEqual(expected, result);
        }


        private void SeedTestData()
        {
            var testData2011 = new List<Seifa2011>
            {
            new() { LocalGovtAreas = "State1", Locations = "Place1", RelativeDisadvantage = 1, RelativeAdvantage = 2 },
            new() { LocalGovtAreas = "State1", Locations = "Place2", RelativeDisadvantage = 3, RelativeAdvantage = 4 },
            new() { LocalGovtAreas = "State2", Locations = "Place3", RelativeDisadvantage = 5, RelativeAdvantage = 6 },
            new() { LocalGovtAreas = "State3", Locations = "Place4", RelativeDisadvantage = 7, RelativeAdvantage = 8 }
        };

            var testData2016 = new List<Seifa2016>
            {
             new()
             {
                 LgaCode = 10050,
                 LgaName = "Place1",
                 IndexOfRelativeSocioEconomicDisadvantageScore = 1,
                 IndexOfRelativeSocioEconomicDisadvantageDecile = 5,
                 IndexOfRelativeSocioEconomicAdvantageDisadvantageScore = 1,
                 IndexOfRelativeSocioEconomicAdvantageDisadvantageDecile = 5,
                 IndexOfRelativeEconomicScore = 5,
                 IndexOfRelativeEconomicDecile = 960,
                 IndexOfEducationAndOccupationScore = 3,
                 IndexOfEducationAndOccupationDecile = 6,
                 UsualResidentPopulation = 51076
             }
           
            };



            _dbContext.Seifa2011s.AddRange(testData2011);
            _dbContext.Seifa2016s.AddRange(testData2016);

            _dbContext.SaveChanges();
        }


        private static DbContextOptionsBuilder<TestDbContext> GetDbOptionsBuilder()
        {

            // The key to keeping the databases unique and not shared is 
            // generating a unique db name for each.
            string dbName = Guid.NewGuid().ToString();

            // Create a fresh service provider, and therefore a fresh 
            // InMemory database instance.
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            // Create a new options instance telling the context to use an
            // InMemory database and the new service provider.
            var builder = new DbContextOptionsBuilder<TestDbContext>();
            builder.UseInMemoryDatabase(dbName)
                .UseInternalServiceProvider(serviceProvider);

            return builder;
        }
    }
}
