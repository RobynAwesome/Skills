using System.Text.Json;

namespace Kopano.Sovereign.EstateAdapter;

public sealed class DomainBindingStore
{
    public const string RenterAssertion = "I_AM_STATELESS_RENTER_NOT_LANDLORD";

    private readonly DomainBindingDocument _document;

    public DomainBindingStore()
    {
        var configuredPath = Environment.GetEnvironmentVariable("KPGS_BINDINGS_PATH");
        var path = string.IsNullOrWhiteSpace(configuredPath)
            ? Path.Combine(AppContext.BaseDirectory, "config", "domain-bindings.json")
            : configuredPath;

        if (!File.Exists(path))
            throw new FileNotFoundException("KPGS domain bindings were not found.", path);

        var json = File.ReadAllText(path);
        _document = JsonSerializer.Deserialize<DomainBindingDocument>(json, JsonOptions())
            ?? throw new InvalidOperationException("KPGS domain bindings could not be parsed.");

        if (!string.Equals(_document.RenterAssertion, RenterAssertion, StringComparison.Ordinal))
            throw new InvalidOperationException("Domain binding document failed the stateless renter assertion.");
    }

    public IReadOnlyList<DomainBinding> All => _document.Bindings;

    public RouteReceipt Resolve(string rawUrl)
    {
        if (!Uri.TryCreate(rawUrl, UriKind.Absolute, out var uri))
            throw new ArgumentException("A valid absolute URL is required.", nameof(rawUrl));

        var host = uri.Host.ToLowerInvariant();
        var binding = FindBinding(host);

        if (binding is null)
        {
            return new RouteReceipt(
                RenterAssertion,
                host,
                uri.AbsolutePath,
                "unknown",
                "kpgs-sovereign-estate-parser",
                rawUrl,
                null,
                null,
                null,
                "Bind the domain to a governed authority before promoting any claim.");
        }

        var route = binding.Routes?
            .Where(item => uri.AbsolutePath.StartsWith(item.Prefix, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(item => item.Prefix.Length)
            .FirstOrDefault();

        var skill = route?.Skill ?? binding.SkillFamily;
        var authority = route?.Authority ?? binding.Authority;

        return new RouteReceipt(
            RenterAssertion,
            host,
            uri.AbsolutePath,
            binding.Lane,
            skill,
            authority,
            binding.RepoOwner,
            binding.OperationalAuthority,
            route?.Prefix,
            binding.OperationalAuthority is not null && !string.Equals(authority, binding.OperationalAuthority, StringComparison.OrdinalIgnoreCase)
                ? binding.OperationalAuthority
                : authority);
    }

    private DomainBinding? FindBinding(string host)
    {
        var exact = _document.Bindings.FirstOrDefault(item =>
            !item.Host.StartsWith("*.", StringComparison.Ordinal) &&
            string.Equals(item.Host, host, StringComparison.OrdinalIgnoreCase));

        if (exact is not null) return exact;

        return _document.Bindings
            .Where(item => item.Host.StartsWith("*.", StringComparison.Ordinal))
            .OrderByDescending(item => item.Host.Length)
            .FirstOrDefault(item => host.EndsWith(item.Host[1..], StringComparison.OrdinalIgnoreCase));
    }

    private static JsonSerializerOptions JsonOptions() => new()
    {
        PropertyNameCaseInsensitive = true
    };
}
