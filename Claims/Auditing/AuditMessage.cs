namespace Claims.Auditing
{
    public class AuditMessage
    {
        public string EntityId { get; set; }
        public string HttpRequestType { get; set; }
        public string EntityType { get; set; }
    }

}
