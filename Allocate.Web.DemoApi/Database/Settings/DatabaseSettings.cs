namespace Allocate.Common.Database.Settings;

public class DatabaseSettings
{
    public virtual string Server { get; set; } = "postgres.local.allocate.build";

    public virtual string Port { get; set; } = "5432";

    public virtual string Database { get; set; } = "";

    public virtual string UserId { get; set; } = "postgres";

    public virtual string Password { get; set; } = "";

    public virtual string ApplicationName { get; set; } = "";

    public virtual int MinimumPoolSize { get; set; } = 5;

    public virtual bool SslRequired { get; set; } = false;

    public virtual bool IncludeErrorDetail { get; set; } = false;

    public string GetConnectionString(string applicationName = null)
    {
        if (string.IsNullOrWhiteSpace(Database))
        {
            throw new ArgumentNullException("Database is required");
        }
        var connectionString = $"Server={Server};Port={Port};Database=\"{Database}\";User Id=\"{UserId}\";";
        if (SslRequired)
        {
            connectionString += $"SSL Mode=Require;Trust Server Certificate=true;";
        }
        if (string.IsNullOrWhiteSpace(Password) == false)
        {
            connectionString += $"Password=\"{Password}\";";
        }
        if (string.IsNullOrWhiteSpace(applicationName ?? ApplicationName) == false)
        {
            connectionString += $"Application Name=\"{applicationName ?? ApplicationName}\";";
        }
        if (MinimumPoolSize > 0)
        {
            connectionString += $"Minimum Pool Size=\"{MinimumPoolSize}\";";
        }
        if (IncludeErrorDetail)
        {
            connectionString += $"Include Error Detail=True;";
        }
        return connectionString;
    }
}
