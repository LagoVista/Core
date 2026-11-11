# Security Assurance V1 — Architecture and Trust Boundaries

**Status:** Proposed design, not yet an implemented capability.

## Purpose and deliberately narrow scope

A Software Logistics Security Operations Trexel coordinates initial and recurring assessments of our in-cluster services, build and software supply chain, application/API security boundaries, dependencies, managed credentials, and encryption. It seeks evidence, summarizes risks, requests authorized interventions, and assembles assessments. It does **not** act as a privileged secret-management client.

V1 integrates only the known internal platform and pre-approved providers. It is not a generalized enterprise security automation service. Onboarding third-party environments, arbitrary cloud accounts, multi-tenant security administration, and custom consulting adapters are outside V1.

## Two distinct planes

**Intelligence/control plane (Trexel, SOP, artifact workflow)**

- Requests inventories and scans through constrained, allowlisted tools.
- Observes non-sensitive scan metadata, risk summaries, policy outcomes, work-item status, and opaque receipt IDs.
- Proposes prioritization, investigation questions, and remediation plans.
- Requests execution of a pre-authorized deterministic operation; cannot self-authorize production changes.
- Produces structured assessment artifacts and consolidates verified results.

**Deterministic security execution plane (trusted services)**

- Authenticates the calling service and independently enforces environment, asset, action, change-window, and separation-of-duties policy.
- Resolves resource references, generates high-entropy credentials or key material, writes secure stores, changes managed service identities, initiates safe rollout, verifies connections and revocation, and records privileged evidence.
- Keeps plaintext and key bytes in its own security boundary. Protects sensitive process output, diagnostic data, exception messages, retry state, telemetry, and logs.
- Returns only safe metadata: operation ID, asset alias, phase, opaque key/credential version ID, outcome, verification summary, evidence reference, and error code (never values or reversible ciphertext).
- Operates idempotently, with durable checkpointing, preflight, timeout handling, retry budgets, and safe recovery.

Model-visible tools should be designed as narrowly typed operations (e.g., `credential-rotation-plan`, `credential-rotation-start`, `credential-rotation-status`, `credential-rotation-verify`) rather than general shell, database, secret-read, or Kubernetes administrative access.

## Authorization

Production mutating operations require **independent deterministic authorization** with explicit privilege, environment scope, action scope, and approved maintenance policy. Cross-model review and high quality/confidence scores do not grant execution authority. The Trexel cannot approve its own exception. Where required, separate the operator who authorizes from the mechanism that executes.

Suggested execution state machine: Planned -> PreflightPassed -> Authorized -> Staged -> Cutover -> Verified -> OldCredentialRetired -> ReceiptFinalized. Failure paths: Blocked, Inconclusive, RecoveryRequired, RolledBack. State, durable journal, and correlation IDs must survive restarts.

## Security review coverage

- Container images, workloads, cluster node/package vulnerabilities, configuration, privileges, and supply-chain provenance.
- NuGet/NPM/direct/transitive dependencies, runtime and base images, SBOM generation, new CVEs and affected deployed digests.
- Managed secrets inventory **by metadata only**: owners, consumers, rotation status, scope, storage provenance, and evidence.
- Cryptographic implementations, key hierarchy, key versions, envelope compatibility, cryptoperiod, rewrapping/reencryption, recovery.
- Authentication, authorization, service identities, cross-organization isolation, API surface, public exposure, TLS/network policy.
- Backup encryption, restore proof, audit logging and alerts, incident response, Build Server and build-agent privileges.

## Handling sensitive evidence

Keep the public documentation descriptive, not operationally revealing. Privileged tools store raw scan outputs and logs only within access-controlled services where justified. The model receives a sanitized projection: finding IDs, risk classifications, affected **logical** service aliases, impact summaries, review tasks, and secure reference IDs. Redact before persistence to execution traces, artifact model content, or cross-model requests. Apply strict output schemas, log scrubbing, retention limits, and test secret-canary non-disclosure.

## Dependencies and implementation boundaries

Existing SecureStorage and key rings may support versioned decryption and rewrap-on-read; **do not infer** full rotation guarantees from those features. ASP.NET Data Protection keys, encrypted configuration data, service passwords, signing keys, and TLS certificates are separate asset classes with separate schedules and rollback rules. See [rotation playbook](rotation-playbook.md).

Build Server should eventually attach scanner evidence to exact source commits, package locks, published image digests, and deployment truth. A vulnerability gate requires enforced scan coverage; an optional `npm audit` script does not establish it.

## Future commercial offering

After the internal control is proven, a separate customer onboarding design could introduce an Environment Security Profile, provider-specific deterministic adapters, authorization review, and consulting-led setup. None is a dependency for V1.
