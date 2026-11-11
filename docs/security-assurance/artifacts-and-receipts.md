# Assessments, Findings, and Verification Receipts

**Status:** Proposed artifact contracts for the internal Security Operations Trexel.

## V1 ArtifactSpecification candidates

| TLA | Artifact | Responsibility |
|---|---|---|
| ISA | Infrastructure Security Assessment | Cluster posture, deployed images/workloads, network exposure, and infrastructure vulnerabilities |
| DSA | Dependency Security Assessment | NuGet/NPM, transitive and runtime dependencies, SBOMs, and affected deployed versions |
| CSA | Credential Security Assessment | Managed credential metadata, provenance, scope, consumer inventory, age, rotation readiness, and suspected exposure |
| KEA | Key and Encryption Assessment | Encryption mechanisms, key custody, rotation compatibility, storage and recovery |
| SRA | Security Readiness Assessment | Consolidated risk disposition, coverage gaps, launch decision inputs, and approvals |
| SRR | Security Remediation Receipt | Verified outcome of an authorized remediation or routine credential rotation |

These should become actual APTIX ArtifactSpecifications in DEV only when implementation is ready. The table is a design inventory, **not evidence that they have been created**.

## Common assessment contract

Every assessment includes: scope and environment, assessment window, policy/checklist version, originating SOP/run ID, covered assets, scanners and their versions, immutable source/deployment references, evidence timestamps, findings references, reviewer/approval status, exceptions, residual risk, and an explicit completeness state.

Assessment outcome vocabulary: **Pass**, **IssuesFound**, **Incomplete**, **Blocked**, **Error**. Unknown or unavailable scan results can only yield Incomplete/Blocked/Error; never Pass. A risk score is advisory, not execution authority.

Findings are durable, deduplicated entities keyed by the underlying issue and affected asset/version/environment. A repeated assessment updates discovery/last-seen evidence. Resolution requires an independently recorded verification event; recurrence must be traceable. Work items are created for failed journeys, human decisions, retry exhaustion, or required remediation, not for each transient scanner failure.

## Security Remediation Receipt (SRR)

The SRR is a higher-level work product linking assessment -> finding -> approved plan -> deterministic operation -> independent verification. Proposed required fields:

- Receipt ID, finding IDs, assessment IDs, service/environment aliases, operation kind, and timestamps.
- Authorization decision and actor reference (not token), change window, policy version, and scope.
- Previous and replacement **opaque version references** where applicable; no credential, password, API key, seed, private key, or unredacted command output.
- Exact change evidence (commit SHA, build/deploy operation ID, image digest or configuration revision).
- Before/after validation IDs, method, scope, result, and verification timestamp.
- Old credential revocation result where applicable, rollback/recovery outcome, unresolved limitations, and safe evidence references.
- Review/approval state and integrity metadata (content digest, immutable or append-only finalized revision).

Possible verification outcomes: **Verified**, **Failed**, **Inconclusive**. Never mark Verified from a successful tool invocation alone. In particular, 'deployment completed' does not prove 'vulnerability removed'.

An existing deterministic ArtifactInvocationCompletionReceipt may be **linked as execution evidence**. It does not replace the independently validated SRR outcome. ProducedArtifact metadata and version history can supply lineage; finalized receipts require append-only/immutable treatment (or versioned non-destructive amendments), beyond ordinary editable drafts.

## Evidence custody

Raw security proof stays in an access-controlled evidence store with restricted access and retention. The Trexel and any second model receive only sanitized evidence summaries and scoped retrieval references. A receipt must remain auditable even after a scanner or credential has changed. Failure logs must not contain secrets. For public repositories, write only safe architectural descriptions.

## Proposed SOPs

- **Platform Security Baseline** -> ISA, DSA, CSA, KEA -> SRA.
- **Managed Credential Rotation and Verification** -> SRR, updates CSA and SRA where relevant.
- **Cryptographic Key Lifecycle Review** -> KEA and, when authorized changes occur, SRR.
- **Vulnerability Remediation Verification** -> independently test a fix, SRR, finding closure evidence.

## Acceptance tests

1. Scanner unavailable -> Incomplete, not clean.
2. Two scans of the same issue -> one durable finding with updated evidence.
3. Authorized operation succeeds but rescan fails -> no verified SRR.
4. Secret-like canary in backend diagnostics -> absent from all model/tool responses, traces, work items, artifacts and receipt metadata.
5. Crash before credential cutover -> resume from persisted state without regeneration/duplicate revocation.
6. Reopened issue -> preserve original historical receipt and create new occurrence/evidence.
