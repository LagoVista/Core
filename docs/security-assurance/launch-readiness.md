# Internal Launch Security Readiness — Scope and Acceptance

**Status:** Proposed release-assurance workstream. No audit results or operational security status are represented in this document.

## Objective

Establish a repeatable evidence-backed baseline for Software Logistics' LIVE platform before launch, then turn successful controls into recurring operations. A clean report requires verified coverage, not merely a lack of alerts.

## Assessment domains

| Domain | V1 target proof |
|---|---|
| Workloads and container images | Inventory of deployed immutable digests, container/base-image and node scans, policy/privilege checks |
| Third-party dependencies | Exact NuGet/NPM/transitive dependency graph, SBOM, vulnerability mapping to deployed applications |
| Credentials and secret storage | Metadata-only inventory, source/history secret exposure checks performed in restricted tooling, owners and rotation plans |
| Cryptography and key custody | Encryption/key inventory, algorithm/rotation review, recovery and historical-decryption proof |
| Identity, access, tenant isolation | Negative authorization tests, service-identity and administrative-access review |
| Network and API boundaries | Ingress exposure, TLS, network isolation, API threat cases and rate-limit/authentication checks |
| Data protection and resilience | Backup custody, integrity, secure retention, completed test restore and recovery objective review |
| Build and deployment supply chain | Build-agent identity, repository/package registry permissions, provenance and production authorization |
| Security operations | Monitoring, incidents, audit completeness, alert routing, work-item resolution and exception handling |

## Suggested acceptance policy

- Known scope inventory is complete, with evidence recency and an assigned owner.
- Every production workload/image and reachable dependency path is assessed; scan failures are marked Incomplete.
- No unresolved **confirmed credential exposure**; suspected exposure has a risk decision and investigation path.
- No unmitigated **exploitable critical** issue. High-risk findings need owner, plan, due date, and authorized time-bounded exception where policy permits.
- Key storage/recovery, tenant isolation and rollback behavior are tested at representative critical paths.
- At least one controlled backup restore is verified and evidenced.
- Signed review/approval of readiness with residual risks; a Trexel score alone cannot authorize launch.

Final gate policy, owners, and risk thresholds are to be agreed before rollout. Do not infer a launch decision from this document.

## Suggested increments

**Increment 0: Manual/read-only baseline.** Inventory assets and safe metadata. Assess current code/package/security scans, investigate findings with secured evidence, identify missing integrations. Do not mutate LIVE.

**Increment 1: Assessment artifacts.** Implement six ArtifactSpecifications and their SOPs; persist normalized results with completeness and provenance. Deduplicate findings and issue work items as needed.

**Increment 2: One DEV rotation pilot.** Select a low-risk credential with known consumers. Implement deterministic two-phase rotation, authorization, failure recovery and SRR verification. Test restart/cutover failure paths.

**Increment 3: Controlled LIVE rollout.** Obtain explicit production authorization for one pilot and verify rollback and monitoring. Expand only after successful operational evidence.

**Increment 4: Continuous assurance.** Schedule recurring scans and eligibility review; attach immutable release evidence to Build Server where practical, gate deployments, and monitor new vulnerabilities and credential expiry.

## Work ownership

APTIX owns reasoning coordination, artifact production, and work-item lifecycle. Security/operations owners define policy and approve changes. Deterministic security tools own secret operations. Build Server owns supported release and rollout operations. The Configuration Server remains the owner of managed configuration, subject to secure, audited mutation APIs.

## Out of scope for internal V1

Do not build a self-service external customer environment registry, cross-provider generalization, new consulting marketplace, or a public security certification claim. A future commercial rollout may require scoped environment profiles and professional onboarding, but these must not be prerequisites for securing our own platform.
