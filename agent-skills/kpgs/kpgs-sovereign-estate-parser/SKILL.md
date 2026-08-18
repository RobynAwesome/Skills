---
name: "kpgs-sovereign-estate-parser"
description: "Parse domains, repositories, client receipts, project state, search/index state and agent outputs into one KPGS-governed estate receipt through a stateless .NET Domain Adapter. Use when reconciling what is real, what is indexed, which repo/domain owns a claim, which skill should run, or whether a public claim has enough evidence."
author: "Kholofelo Robyn Rababalela"
signature: "/s/ Kholofelo Robyn Rababalela"
renter_assertion: "I_AM_STATELESS_RENTER_NOT_LANDLORD"
tags:
  - kpgs
  - mmao
  - estate
  - parser
  - dotnet
  - evidence
  - routing
license: "MIT"
---

# KPGS Sovereign Estate Parser

## Use when

Use this skill when work touches any combination of:

- a Kopano-owned domain, subdomain, repository, project or public artifact;
- a client, consultation, field deployment, institutional engagement or delivery receipt;
- SEO/Search Console/index state versus real-world validation state;
- multiple AI agents/models producing overlapping or contradictory conclusions;
- migration of repositories into the canonical `RobynAwesome` namespace;
- deciding whether a claim is `POC`, `FOC`, `MAYBE`, `validated`, `planned`, `demo`, `privileged` or `transactional`;
- routing one human request to the correct site/domain skill without blending source authority.

## First law

Before interpretation or execution, accept:

`I_AM_STATELESS_RENTER_NOT_LANDLORD`

The executing model, agent or skill is a renter. Repositories, live domains, operators, governed manifests and real-world receipts remain the landlord/source of truth.

Do not persist an unverified conclusion as canonical state.

## Runtime shape

This skill follows the same control-plane boundary used for the Five's Arena estate:

```text
Everyday Human / Operator
        ↓
Adaptive PWA / Client Surface
        ↓
.NET Domain Adapter
        ↓
Kopano Sovereign Hub / KPGS
        ↓
MMAO skill + agent routing
        ↓
stateless renters
        ↓
receipts / conflicts / next action
        ↓
Adaptive PWA / Public Evidence
```

`.NET` is the rigid adapter and parser boundary. It is **not** a rewrite of React, Next.js, Vite or existing application runtimes.

## Mandatory telemetry classes

Every parsed observation must carry exactly one telemetry class:

- `verified-source` — current repository, governed manifest or source artifact.
- `verified-live` — fetched from a current live domain/runtime.
- `site-stated` — displayed publicly but not independently verified.
- `demo-display` — mock, fixture, fallback, hard-coded or sample data.
- `planned` — intended or roadmap state.
- `privileged` — requires authenticated/authorized access.
- `transactional` — changes money, booking, registration, publication or another external state.
- `unknown` — evidence is insufficient.

**Never upgrade one class into another without a receipt.**

## Reality / index law

Search visibility is an observer, not the authority.

```text
REALITY STATE > INDEX STATE
```

A search impression, ranking, crawl result or indexed page can corroborate a real-world receipt. It cannot manufacture one.

Treat SEO as `verified-live` or `site-stated` discovery evidence depending on the source. Never convert SEO metrics into customer, revenue, institutional or field-validation claims without separate receipts.

## Canonical repo namespace

For Kopano-owned repositories, route canonical GitHub ownership through:

`https://github.com/RobynAwesome`

Repository ownership proves source authority, **not** production maturity.

Keep these distinctions:

- `product` / `operating-system` / `client-system` / `field-system` / `engineering` — may be public project lanes.
- `lab` / `learning` / `workshop` / `reference` / upstream fork — source exists, but do not present it as an operating Kopano product without receipts.
- private repositories — never enumerate publicly merely because the parser can see them.

## Five's Arena exemplar

The parser must preserve the split that proved this architecture:

### `FivesArena.com`

Operational lane.

Authoritative venue bookings, current prices, tournament registration, fixtures, league/competition state and state-changing venue workflows belong here.

### `blog.FivesArena.com`

Editorial/community lane.

Articles, creator/profile copy, tactics and community content belong here. Hard-coded `Live Now` cards, counters, follower labels and local-only newsletter/runtime behavior must never be silently upgraded into operational venue truth.

### Cross-domain rule

If editorial content references a current operational value, route to the operational domain/source before claiming it as current.

If operational surfaces reference editorial narrative, preserve the editorial attribution rather than treating it as booking or competition state.

## Parse lifecycle

For every request, execute this sequence:

1. **CAPTURE** — parse URL, hostname, path, repo, branch/commit, project/client name and requested action.
2. **BIND** — resolve the domain/repo/skill lane from `config/domain-bindings.json`.
3. **CLASSIFY** — assign telemetry class, evidence kind, relationship state and mutation risk.
4. **NORMALIZE** — convert heterogeneous observations into the adapter evidence contract.
5. **CONVERGE** — group equivalent claims, expose contradictions and select only the highest-supported canonical claim.
6. **GOVERN** — apply `MAYBE` when evidence is incomplete; block implicit privilege/transaction escalation.
7. **ROUTE** — return the responsible skill family / domain authority / next verification source.
8. **RECEIPT** — emit a machine-readable result showing source, classification, conflicts, decision and next move.

## Evidence precedence

Precedence is contextual, not merely numeric, but default to:

```text
current governed source + current live receipt
    > current live receipt alone
    > current source receipt alone
    > site-stated
    > planned / demo-display
    > unknown
```

For time-sensitive values such as price, availability, fixture state, booking state or deployment state, current live evidence outranks a stale repository snapshot.

For authorship, source lineage, schema or code ownership, current source authority outranks copied marketing text.

## Relationship states

Use bounded relationship labels. Recommended defaults:

- `VALIDATED_LIVE`
- `VALIDATED_FIELD`
- `DELIVERED_EXTERNAL`
- `ACTIVE_BOUNDED`
- `INTERNAL_POC`
- `LAB_REFERENCE`
- `UNKNOWN`

Do not turn an academic engagement into a client, a consultation into revenue, a logo into a partnership, or a repository into a deployment without a receipt.

## MMAO / CCP convergence

MCP connects tools/context. MMAO coordinates independent model/agent renters. KPGS/KC governs routing and permissions. GSMB/KPSMB carries governed state lineage. CCP evaluates conceptual convergence.

The parser should therefore return:

- what each renter/source said;
- which claims converge;
- which claims conflict;
- the telemetry class of each input;
- the bounded canonical conclusion;
- the missing receipt required to promote the claim.

No majority vote. Evidence governs convergence.

## .NET adapter runtime

The executable reference implementation lives in:

`runtime/Kopano.Sovereign.EstateAdapter/`

Run locally:

```bash
dotnet run --project runtime/Kopano.Sovereign.EstateAdapter
```

Self-test:

```bash
dotnet run --project runtime/Kopano.Sovereign.EstateAdapter -- --self-test
```

Default HTTP endpoints:

```text
GET  /health
GET  /v1/bindings
POST /v1/route
POST /v1/parse
POST /v1/converge
```

The runtime is stateless by design. It parses and returns receipts; it does not become the estate database or landlord.

## Output contract

Every parse/convergence response must contain enough information to answer:

```json
{
  "renterAssertion": "I_AM_STATELESS_RENTER_NOT_LANDLORD",
  "domainLane": "operational | editorial | corporate | product | field | identity | unknown",
  "skillFamily": "string",
  "telemetryClass": "verified-source | verified-live | site-stated | demo-display | planned | privileged | transactional | unknown",
  "relationshipState": "VALIDATED_LIVE | VALIDATED_FIELD | DELIVERED_EXTERNAL | ACTIVE_BOUNDED | INTERNAL_POC | LAB_REFERENCE | UNKNOWN",
  "canonicalClaim": "string or null",
  "conflicts": [],
  "receipts": [],
  "nextVerification": "string or null"
}
```

## Mutation gate

Parsing and reading are non-destructive.

Any booking, payment, registration, publication, repository write, DNS change, deployment, outbound message, account action or other external mutation must pass its own authorization gate and emit a receipt from the actual system of record.

## POC / FOC gate

A beautiful page is not validation.

Before promoting a claim to POC, require an inspectable artifact, state transition or external outcome.

If the measurement mechanism cannot represent the real state, downgrade the measurement mechanism rather than rewriting the real-world receipt.

## Done means

This skill is complete for a request only when:

- the domain/repo lane is explicit;
- the telemetry class is explicit;
- source authority is explicit;
- contradictory claims are exposed, not hidden;
- the canonical conclusion is bounded by evidence;
- a next verification source is named when state remains `MAYBE`;
- no renter silently became landlord.
