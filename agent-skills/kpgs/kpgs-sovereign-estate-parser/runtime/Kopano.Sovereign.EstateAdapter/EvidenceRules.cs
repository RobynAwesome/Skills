namespace Kopano.Sovereign.EstateAdapter;

public static class EvidenceRules
{
    public static readonly IReadOnlyDictionary<string, int> Scores = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
    {
        ["verified-live"] = 80,
        ["verified-source"] = 75,
        ["site-stated"] = 45,
        ["planned"] = 20,
        ["demo-display"] = 15,
        ["privileged"] = 5,
        ["transactional"] = 5,
        ["unknown"] = 0
    };

    public static readonly HashSet<string> RelationshipStates = new(StringComparer.OrdinalIgnoreCase)
    {
        "VALIDATED_LIVE",
        "VALIDATED_FIELD",
        "DELIVERED_EXTERNAL",
        "ACTIVE_BOUNDED",
        "INTERNAL_POC",
        "LAB_REFERENCE",
        "UNKNOWN"
    };

    public static string Telemetry(string value) =>
        Scores.ContainsKey(value) ? value.ToLowerInvariant() : "unknown";

    public static string Relationship(string value) =>
        RelationshipStates.Contains(value) ? value.ToUpperInvariant() : "UNKNOWN";
}
