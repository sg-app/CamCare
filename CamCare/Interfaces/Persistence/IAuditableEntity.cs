public interface IAuditableEntity
{
    DateTime CreatedAt { get; set; }
    DateTime? UpdatedAt { get; set; }
    DateTime? ArchivedAt { get; set; }
}
