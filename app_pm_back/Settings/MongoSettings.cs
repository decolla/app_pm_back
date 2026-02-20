namespace app_pm_back.Settings;

public class MongoSettings {
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
    public string NotasCollection { get; set; }
    public string OrdensCollection { get; set; }
}