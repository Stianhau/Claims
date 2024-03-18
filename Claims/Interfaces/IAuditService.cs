public interface IAuditService
{
    Task AuditClaim(string id, string httpRequestType);
    public Task AuditCover(string id, string httpRequestType);
}