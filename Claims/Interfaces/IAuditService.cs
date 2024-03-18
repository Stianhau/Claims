public interface IAuditService
{
    void AuditClaim(string id, string httpRequestType);
    public void AuditCover(string id, string httpRequestType);
}