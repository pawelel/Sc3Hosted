namespace Sc3Hosted.Server.Entities;

public class CustomLog

{

    public Guid Id { get; set; }

    public string Action { get; set; } = string.Empty;

    public string TableName { get; set; } = string.Empty;

    public string PrimaryKey { get; set; } = string.Empty;

    public string ColumnName { get; set; } = string.Empty;

    public string OldValue { get; set; } = string.Empty;

    public string NewValue { get; set; } = string.Empty;

    public DateTime Date { get; set; }

    public Guid UserId { get; set; }

}
