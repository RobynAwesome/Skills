namespace Kopano.Sovereign.EstateAdapter;

public static class SelfTest
{
    public static int Run()
    {
        var store = new DomainBindingStore();
        var parser = new EstateParser();
        var failures = new List<string>();

        var operational = store.Resolve("https://FivesArena.com/bookings");
        if (operational.DomainLane != "operational") failures.Add("FivesArena.com must resolve to the operational lane.");
        if (operational.SkillFamily != "fivesarena-bookings") failures.Add("/bookings must resolve to fivesarena-bookings.");

        var editorial = store.Resolve("https://blog.FivesArena.com/tactics");
        if (editorial.DomainLane != "editorial") failures.Add("blog.FivesArena.com must resolve to the editorial lane.");
        if (editorial.OperationalAuthority != "https://FivesArena.com") failures.Add("The blog must retain FivesArena.com as operational authority.");

        var receipt = parser.Parse(new ParseRequest(
            "https://kopanolabs.com/FOC/",
            "estate-validation",
            new[]
            {
                new EvidenceInput(
                    "field-receipt",
                    "The farm workflow is validated in field operations.",
                    "verified-source",
                    "VALIDATED_FIELD",
                    "field",
                    "https://kopanolabs.com/validation.json",
                    "validation.json#north-west-lucerne"),
                new EvidenceInput(
                    "search-console",
                    "The farm workflow is validated in field operations.",
                    "verified-live",
                    "VALIDATED_FIELD",
                    "seo",
                    "https://search.google.com/",
                    "search-observation")
            }), store);

        if (receipt.RelationshipState != "VALIDATED_FIELD") failures.Add("Field receipt must govern relationship state over SEO observation.");
        if (!receipt.Converged) failures.Add("Equivalent claims from different evidence lanes should converge.");

        var conflict = parser.Parse(new ParseRequest(
            "https://blog.FivesArena.com/fixtures",
            "fixture-truth",
            new[]
            {
                new EvidenceInput("blog-card", "Fixture is live", "demo-display", "UNKNOWN", "editorial"),
                new EvidenceInput("venue-runtime", "Fixture is not live", "verified-live", "VALIDATED_LIVE", "runtime", "https://FivesArena.com/fixtures", "live-fixture-receipt", DateTimeOffset.UtcNow, true)
            }), store);

        if (conflict.CanonicalClaim != "Fixture is not live") failures.Add("Current operational receipt must outrank demo editorial display.");

        if (failures.Count == 0)
        {
            Console.WriteLine("KPGS Sovereign Estate Adapter self-test: PASS");
            return 0;
        }

        Console.Error.WriteLine("KPGS Sovereign Estate Adapter self-test: FAIL");
        foreach (var failure in failures) Console.Error.WriteLine($"- {failure}");
        return 1;
    }
}
