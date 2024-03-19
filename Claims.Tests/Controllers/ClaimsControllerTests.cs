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

        public WebApplicationFactory<Program> CreateTestApplicationFactory()
        {
            return new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureTestServices(services =>
                    {
                        services.AddDbContext<AuditContext>(options => options.UseInMemoryDatabase("mssqllocaldb"));
                        services.AddSingleton<IClaimRepository, ClaimRepositoryMock>();
                        services.AddSingleton<ICoverRepository, CoverRepositoryMock>(_ =>
                        {
                            var cover = new Dictionary<string, Cover>
                            {
                                { coverId, new Cover() { Id = coverId, Type = CoverType.BulkCarrier, StartDate = DateOnly.FromDateTime(DateTime.Now), EndDate = DateOnly.FromDateTime(DateTime.Now.AddDays(20)) } }
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

    }
}
