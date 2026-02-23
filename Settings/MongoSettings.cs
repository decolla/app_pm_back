namespace app_pm_back.Settings;

public class MongoSettings {
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string NotasCollection { get; set; } = null!;
    public string OrdensCollection { get; set; } = null!;
}