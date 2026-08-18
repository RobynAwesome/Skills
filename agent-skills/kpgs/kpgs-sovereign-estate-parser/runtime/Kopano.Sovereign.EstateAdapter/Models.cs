namespace Kopano.Sovereign.EstateAdapter;

public sealed record DomainRoute(
    string Prefix,
    string Skill,
    string? Authority = null);

public sealed record DomainBinding(
    string Host,
    string Lane,
    string SkillFamily,
    string Authority,
    string? RepoOwner = null,
    string? OperationalAuthority = null,
    IReadOnlyList<DomainRoute>? Routes = null);

public sealed record DomainBindingDocument(
    string Schema,
    string RenterAssertion,
    IReadOnlyList<DomainBinding> Bindings);

public sealed record RouteRequest(string Url);

public sealed record RouteReceipt(
    string RenterAssertion,
    string Host,
    string Path,
    string DomainLane,
    string SkillFamily,
    string Authority,
    string? RepoOwner,
    string? OperationalAuthority,
    string? MatchedRoute,
    string? NextVerification);

public sealed record EvidenceInput(
    string SourceId,
    string Claim,
    string TelemetryClass,
    string RelationshipState = "UNKNOWN",
    string EvidenceKind = "source",
    string? Url = null,
    string? Receipt = null,
    DateTimeOffset? ObservedAt = null,
    bool TimeSensitive = false);

public sealed record ParseRequest(
    string? Url,
    string? ClaimKey,
    IReadOnlyList<EvidenceInput> Evidence);

public sealed record NormalizedEvidence(
    string SourceId,
    string Claim,
    string TelemetryClass,
    string RelationshipState,
    string EvidenceKind,
    string? Url,
    string? Receipt,
    DateTimeOffset? ObservedAt,
    bool TimeSensitive,
    int EvidenceScore,
    bool CanPromoteRelationship,
    string? Note);

public sealed record ConflictReceipt(
    string Claim,
    IReadOnlyList<string> Sources,
    int HighestEvidenceScore);

public sealed record EstateReceipt(
    string RenterAssertion,
    string DomainLane,
    string SkillFamily,
    string TelemetryClass,
    string RelationshipState,
    string? CanonicalClaim,
    IReadOnlyList<ConflictReceipt> Conflicts,
    IReadOnlyList<NormalizedEvidence> Receipts,
    string? Authority,
    string? NextVerification,
    bool RequiresAuthorization,
    bool Converged);
