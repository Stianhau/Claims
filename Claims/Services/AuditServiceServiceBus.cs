using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Claims.Auditing;

// Example of using Service Bus
public class AuditServiceServiceBus : IAuditService
{
    private readonly ServiceBusClient _client;

    public AuditServiceServiceBus(ServiceBusClient client)
    {
        _client = client;
    }

    public async Task AuditClaim(string id, string httpRequestType)
    {
        ServiceBusSender sender = _client.CreateSender("claim-audit");
        var claimAudit = new ClaimAudit()
        {
            Created = DateTime.Now,
            HttpRequestType = httpRequestType,
            ClaimId = id
        };
        
        var json = JsonSerializer.Serialize(claimAudit);
        var message = new ServiceBusMessage(json);

        await sender.SendMessageAsync(message);
    }

    public async Task AuditCover(string id, string httpRequestType)
    {
        ServiceBusSender sender = _client.CreateSender("cover-audit");
        var coverAudit = new CoverAudit()
        {
            Created = DateTime.Now,
            HttpRequestType = httpRequestType,
            CoverId = id
        };
        
        var json = JsonSerializer.Serialize(coverAudit);
        var message = new ServiceBusMessage(json);

        await sender.SendMessageAsync(message);
    }
}