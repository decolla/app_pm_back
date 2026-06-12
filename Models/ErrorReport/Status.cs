using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace app_pm_back.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Status
{
    [EnumMember(Value = "RUNNING")] RUNNING,
    [EnumMember(Value = "ERROR")] ERROR,
    [EnumMember(Value = "NOT_CONNECTED")] NOT_CONNECTED,
    [EnumMember(Value = "EMPTY_READ")] EMPTY_READ
}