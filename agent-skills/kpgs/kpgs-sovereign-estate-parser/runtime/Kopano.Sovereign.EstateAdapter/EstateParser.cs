namespace Kopano.Sovereign.EstateAdapter;

public sealed class EstateParser
{
    public EstateReceipt Parse(ParseRequest request, DomainBindingStore bindings)
    {
        var route = string.IsNullOrWhiteSpace(request.Url) ? null : bindings.Resolve(request.Url);
        var normalized = request.Evidence.Select(Normalize).ToArray();
        var requiresAuthorization = normalized.Any(item => item.TelemetryClass == "privileged" || item.TelemetryClass == "transactional");

        var groups = normalized
            .Where(item => !string.IsNullOrWhiteSpace(item.Claim))
            .GroupBy(item => NormalizeClaim(item.Claim), StringComparer.OrdinalIgnoreCase)
            .Select(group => new ClaimGroup(
                group.First().Claim.Trim(),
                group.Max(item => item.EvidenceScore),
                group.Select(item => item.SourceId).Distinct(StringComparer.OrdinalIgnoreCase).ToArray()))
            .OrderByDescending(group => group.Score)
            .ToArray();

        string? canonical = null;
        var converged = false;
        if (groups.Length == 1)
        {
            canonical = groups[0].Claim;
            converged = true;
        }
        else if (groups.Length > 1 && groups[0].Score > groups[1].Score)
        {
            canonical = groups[0].Claim;
            converged = true;
        }

        var conflicts = groups.Skip(1)
            .Select(group => new ConflictReceipt(group.Claim, group.Sources, group.Score))
            .ToArray();

        var strongest = normalized.OrderByDescending(item => item.EvidenceScore).FirstOrDefault();
        var relationship = SelectRelationship(normalized, canonical);
        var next = NextVerification(route, normalized, canonical, converged, relationship);

        return new EstateReceipt(
            DomainBindingStore.RenterAssertion,
            route?.DomainLane ?? "unknown",
            route?.SkillFamily ?? "kpgs-sovereign-estate-parser",
            strongest?.TelemetryClass ?? "unknown",
            relationship,
            canonical,
            conflicts,
            normalized,
            route?.Authority,
            next,
            requiresAuthorization,
            converged);
    }

    private static NormalizedEvidence Normalize(EvidenceInput input)
    {
        var telemetry = EvidenceRules.Telemetry(input.TelemetryClass);
        var relationship = EvidenceRules.Relationship(input.RelationshipState);
        var kind = string.IsNullOrWhiteSpace(input.EvidenceKind) ? "source" : input.EvidenceKind.Trim().ToLowerInvariant();
        var score = EvidenceRules.Scores[telemetry];
        if (!string.IsNullOrWhiteSpace(input.Receipt)) score += 10;
        if (input.TimeSensitive && telemetry == "verified-live") score += 20;
        if (input.TimeSensitive && telemetry == "verified-source") score = Math.Max(0, score - 5);

        var indexOnly = kind == "index" || kind == "seo" || kind == "search";
        var qualifyingReceipt = !indexOnly && !string.IsNullOrWhiteSpace(input.Receipt) && (telemetry == "verified-live" || telemetry == "verified-source");
        string? note = null;

        if (indexOnly && relationship != "UNKNOWN")
        {
            relationship = "UNKNOWN";
            note = "Index evidence is observational and cannot promote a real-world relationship state.";
        }
        else if ((relationship == "VALIDATED_LIVE" || relationship == "VALIDATED_FIELD" || relationship == "DELIVERED_EXTERNAL") && !qualifyingReceipt)
        {
            relationship = "ACTIVE_BOUNDED";
            note = "Validation state was bounded because a qualifying receipt was not supplied.";
        }

        return new NormalizedEvidence(
            input.SourceId,
            input.Claim.Trim(),
            telemetry,
            relationship,
            kind,
            input.Url,
            input.Receipt,
            input.ObservedAt,
            input.TimeSensitive,
            score,
            qualifyingReceipt,
            note);
    }

    private static string SelectRelationship(IReadOnlyList<NormalizedEvidence> evidence, string? canonical)
    {
        if (canonical is null) return "UNKNOWN";
        return evidence
            .Where(item => NormalizeClaim(item.Claim) == NormalizeClaim(canonical))
            .OrderByDescending(item => item.EvidenceScore)
            .Select(item => item.RelationshipState)
            .FirstOrDefault(state => state != "UNKNOWN") ?? "UNKNOWN";
    }

    private static string? NextVerification(RouteReceipt? route, IReadOnlyList<NormalizedEvidence> evidence, string? canonical, bool converged, string relationship)
    {
        if (evidence.Count == 0) return route?.NextVerification ?? "Supply a governed source or current live receipt.";
        if (!converged || canonical is null) return route?.NextVerification ?? "Conflicting claims require a stronger governing receipt.";
        if (relationship == "UNKNOWN" || relationship == "ACTIVE_BOUNDED") return route?.NextVerification ?? "Supply the receipt required to promote the relationship state.";
        if (evidence.Any(item => item.TimeSensitive && item.TelemetryClass != "verified-live")) return route?.NextVerification ?? "Revalidate the time-sensitive value against the live authority.";
        return null;
    }

    private static string NormalizeClaim(string claim) => string.Join(' ', claim.Trim().ToLowerInvariant().Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
    private sealed record ClaimGroup(string Claim, int Score, IReadOnlyList<string> Sources);
}
