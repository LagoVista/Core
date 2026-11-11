# Managed Credentials and Cryptographic Key Rotation — Operating Model

**Status:** Proposed V1 procedure. Execute only after per-service compatibility tests and approval.

## Two distinct rotation domains

**Service credentials** (database users, message brokers, API credentials, service identities) are authenticated by other systems and consumed by applications. Rotating them requires coordination between target service, Configuration Server, and every consumer.

**Encryption/key material** (secure-store envelope keys, ASP.NET Data Protection keys, signing keys, TLS keys) protects persisted data, cookies, signatures or connections. Key rollover requires compatibility and historical decrypt/verify capability, and must be designed per key class. Do **not** treat a weekly service-password policy as a weekly destruction/replacement policy for root encryption keys.

The Trexel never sees plaintext keys/passwords or the inputs needed to derive them. Deterministic, least-privilege components generate/store/apply material entirely inside their privileged boundary.

## Per-asset prerequisites

Before scheduling rotation, record the logical service alias, protected environment, owning team, identity class, target credential reference, each consumer, supported overlap mode, health/verification probe, rollback procedure, recovery authorization, and approved time window.

Inventory the bootstrap dependencies for the Configuration Server itself: its datastore, secure-store master keys, key-vault access, deployment identity, and recovery channel. Protect this separate root-of-trust path from routine automated changes.

## Preferred no-downtime rotation: overlap supported

1. Preflight: verify all consumers, health, policy, safe fallback, authority, and permitted window.
2. Generate a fresh credential with a CSPRNG inside the deterministic service; record only its opaque version reference.
3. Stage a secondary identity or other supported dual-credential acceptance on the target; verify the staged credential privately.
4. Publish the new configuration version through secure APIs with an atomic/transactional or equivalent safeguarded update.
5. Reconcile consumers and roll instances gradually, respecting readiness checks, availability budgets, and connection draining.
6. Verify all expected consumers use the replacement credential. Guard against cached or long-lived connections reporting misleading success.
7. Revoke the previous credential only after every consumer is verified and the recovery plan is satisfied.
8. Confirm revocation, finalize the SRR with before/after proof, and update inventory/next rotation eligibility.

The actual order of stages is determined by each service's authentication semantics. Never assume a second credential is supported because the control plane could store one.

## Coordinated maintenance rotation: single credential

When overlap is impossible, use an approved maintenance window, pause or drain affected consumers, update the target service and Configuration Server in a tested order, restart/reconcile consumers, and verify end-to-end health. If authentication breaks, the deterministic controller follows the documented rollback or recovery procedure; do not blindly replay a partial sequence.

**Restart is not the guarantee**: verification of configuration version, service authentication, health and affected workflows is required. Multiple replicas must not all restart at once unless designed for that.

## Cryptographic key rollover

1. Confirm key type, provenance, custody, impact and backup/recovery accessibility.
2. Generate and publish a new version without destroying older decrypt/verify-capable versions.
3. Direct new encryption/signing to the active version; retain explicit key IDs in envelopes or signatures.
4. Rewrap/reencrypt existing material through a separate privileged, resumable process or verify proven bounded decrypt-on-read migration.
5. Measure and verify historical references remaining, including cold data and backups.
6. Retire obsolete keys only after the retention, recovery and historical verification criteria are met.

An implementation that automatically rewraps only on read does **not** prove that unaccessed records or backups have migrated. Decryption must fail explicitly if the requested historical key is missing; never silently generate replacement key material on a decrypt path.

## Frequency and policy

**Candidate V1 cadence:** a weekly JobManager *assessment/rotation eligibility* run. Rotate suitable managed service credentials on a controlled schedule (e.g., target 90 days, more often if risk requires) after compatibility proof. Do not mandate a seven-day cryptographic-key replacement cycle. Rotation periodicity is an asset-specific policy decision using risk, cryptoperiod guidance, blast radius and available automation.

User authentication passwords follow separate account policy: periodic changes without evidence of compromise are generally not appropriate. Confirm suspected exposure -> urgent revocation/rotation regardless of schedule.

## Durable deterministic operation

Recommended phases: Plan -> Preflight -> Authorized -> Generate -> Stage -> PublishConfig -> ReconcileConsumers -> Verify -> RetireOld -> FinalizeReceipt. Every transition is idempotent and journaled. On inconclusive verification or exhausted retry budget, stop, preserve rollback options, create a durable work item, and retain evidence. Explicit approval is required for production mutation, exceptions, and recovery from material failures.

## Example tool contract (no secrets)

`rotation-plan(assetAlias, environment)` -> eligibility, consumer count, overlap support, plan ID.  
`rotation-start(planId, authorizationReference)` -> operation ID, phase.  
`rotation-status(operationId)` -> phase, safe error code, version references.  
`rotation-verify(operationId)` -> scoped verification outcome, evidence IDs.  
`rotation-finalize(operationId)` -> receipt ID or blocker.

No arbitrary command field, secret-read operation, unredacted provider error, or plaintext response is allowed. Log scrubbing and negative tests enforce the boundary.
