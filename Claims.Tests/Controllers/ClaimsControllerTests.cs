using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Claims.Tests
{
    public class ClaimsControllerTests
    {
        string coverId = "ccd96ad7-da61-4bce-a284-dd3ca3cc5a95";
        string coverIdInThePast = "40ee5756-4076-4c81-99a6-4ffd12eb3589";
        string existingClaimId = "84245ea2-e535-433b-a945-e1bb8d1bb76a";


        public WebApplicationFactory<Program> CreateTestApplicationFactory()
        {
            return new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureTestServices(services =>
                    {
                        services.AddDbContext<AuditContext>(options => options.UseInMemoryDatabase("mssqllocaldb"));
                        services.AddSingleton<IClaimRepository, ClaimRepositoryMock>(_ => {
                            var claims = new Dictionary<string, Claim>{
                                { existingClaimId, new Claim() { Id = existingClaimId, CoverId = coverId, Created = DateTime.UtcNow, DamageCost = 10, Name = "Test Claim", Type = ClaimType.BadWeather } }
                            };
                            return new ClaimRepositoryMock(claims);
                        }
                        );
                        services.AddSingleton<ICoverRepository, CoverRepositoryMock>(_ =>
                        {
                            DateOnly startDateInThePast = DateOnly.Parse("2024-01-01");
                            DateOnly endDateInThePast = DateOnly.Parse("2024-01-02");
                            var cover = new Dictionary<string, Cover>
                            {
                                { coverId, new Cover() { Id = coverId, Type = CoverType.BulkCarrier, StartDate = DateOnly.FromDateTime(DateTime.Now), EndDate = DateOnly.FromDateTime(DateTime.Now.AddDays(20)) } },
                                { coverIdInThePast, new Cover() { Id = coverId, Type = CoverType.BulkCarrier, StartDate = startDateInThePast, EndDate = endDateInThePast } }
                            };
                            return new CoverRepositoryMock(cover);
                        });
                    });
                });
        }

        [Fact]
        public async Task PostValidClaim_ReturnsCorrectResponse()
        {
            var application = CreateTestApplicationFactory();

            var client = application.CreateClient();

            CreateClaimRequest createClaimRequest = new CreateClaimRequest()
            {
                CoverId = coverId,
                DamageCost = 10000,
                Name = "Claim 1",
                Type = ClaimType.Fire
            };

            var res = await client.PostAsJsonAsync("/Claims", createClaimRequest);

            res.EnsureSuccessStatusCode();

            var createdClaim = await res.Content.ReadFromJsonAsync<Claim>();

            Assert.NotNull(createdClaim);
            Assert.NotNull(createdClaim.Id);
            Assert.Equal(createClaimRequest.CoverId, createdClaim.CoverId);
            Assert.Equal(createClaimRequest.DamageCost, createdClaim.DamageCost);
            Assert.Equal(createClaimRequest.Name, createdClaim.Name);
            Assert.Equal(createClaimRequest.Type, createdClaim.Type);
        }
        

        [Theory]
        [InlineData("ccd96ad7-da61-4bce-a284-dd3ca3cc5a95", 100001, HttpStatusCode.BadRequest, "DamageCost cannot exceed 100,000")]
        [InlineData("some-id-that-does-not-exist", 100000, HttpStatusCode.BadRequest, "Invalid coverId")]
        [InlineData("40ee5756-4076-4c81-99a6-4ffd12eb3589", 100000, HttpStatusCode.BadRequest, "Created date must be within the period of the related Cover")]
        public async Task PostClaimWithInvalidArguments_ReturnsExpectedResponse(string id, int damageCost, HttpStatusCode expectedStatusCode, string expectedErrorMessage)
        {
            var application = CreateTestApplicationFactory();

            var client = application.CreateClient();

            CreateClaimRequest createClaimRequest = new CreateClaimRequest()
            {
                CoverId = id,
                DamageCost = damageCost,
                Name = "Claim 1",
                Type = ClaimType.Fire
            };

            var res = await client.PostAsJsonAsync("/Claims", createClaimRequest);
            Assert.Equal(expectedStatusCode, res.StatusCode);
            Assert.Equal(expectedErrorMessage, await res.Content.ReadAsStringAsync());
        }
        
        [Fact]
        public async Task GetClaimWithExistingId_ReturnsCorrectResponse()
        {
            var application = CreateTestApplicationFactory();

            var client = application.CreateClient();


            var res = await client.GetAsync($"/Claims/{existingClaimId}");
            res.EnsureSuccessStatusCode();

            var claim = await res.Content.ReadFromJsonAsync<Claim>();
            Assert.NotNull(claim);
            Assert.Equal(existingClaimId, claim.Id);
            Assert.Equal(10, claim.DamageCost);
            Assert.Equal(ClaimType.BadWeather, claim.Type);
            Assert.Equal("Test Claim", claim.Name);
        }
        [Fact]
        public async Task GetClaimWithNonExistingId_ReturnsCorrectResponse()
        {
            var application = CreateTestApplicationFactory();

            var client = application.CreateClient();


            var res = await client.GetAsync($"/Claims/non-existing-claim-id");
            Assert.Equal(HttpStatusCode.NoContent, res.StatusCode);
        }
        
        [Fact]
        public async Task GetClaims_ReturnsCorrectResponse()
        {
            var application = CreateTestApplicationFactory();

            var client = application.CreateClient();


            var res = await client.GetAsync("/Claims");
            res.EnsureSuccessStatusCode();

            await res.Content.ReadFromJsonAsync<Claim[]>();
        }
        
        [Fact]
        public async Task DeleteClaimWithExistingId_ReturnsCorrectResponse()
        {
            var application = CreateTestApplicationFactory();

            var client = application.CreateClient();


            var res = await client.DeleteAsync($"/Claims/{existingClaimId}");
            res.EnsureSuccessStatusCode();
        }
        
        [Fact]
        public async Task DeleteClaimWithNonExistingId_ReturnsNotFound()
        {
            var application = CreateTestApplicationFactory();

            var client = application.CreateClient();


            var res = await client.DeleteAsync($"/Claims/non-existing-claim-id");
            Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
        }
    }
}
