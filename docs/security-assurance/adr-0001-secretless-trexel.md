# ADR-0001 — The Security Trexel Is Secretless

**Status:** Proposed for approval  
**Date:** 2026-11-11  
**Scope:** APTIX Security Assurance V1

## Context

An AI-coordinated security workflow is useful for assessment, sequencing, work items, remediation planning and evidence review. Platform credential and encryption material is highly privileged; sending those values through a model context, prompt, tool result, conversation, cross-model reviewer, or trace creates unacceptable new exposure paths.

Software Logistics controls the first targeted cluster, service credential lifecycle, and configuration infrastructure. Deterministic services can execute narrow secure operations on behalf of an authorized workflow.

## Decision

The Security Operations Trexel **must never receive, store, generate, transmit, or inspect sensitive material**, including plaintext passwords, API keys, private keys, seeds, tokens, or derived key bytes.

All such operations occur exclusively in narrowly scoped deterministic components. Model-visible tool contracts accept opaque references and return only sanitized status, identifiers and evidence references. Each call is independently authenticated and authorized; an AI instruction, a quality score or an approval claimed in model text never substitutes for a verified authorization reference.

This invariant extends to retry history, logs, prompts, exception paths, artifact content, cross-model review data, work items, and remediation receipts. Access-controlled diagnostic stores may hold necessary confidential evidence, but must not feed raw secrets into APTIX.

## Consequences

- A separate security execution plane and safe tool schema is required.
- Production mutations require explicit policy checks and authorized actors, with full auditability and recovery.
- End-to-end negative tests with canary secrets are release gates.
- The Trexel cannot perform ad hoc debugging by reading secret values; tools must offer secret-free diagnostics.
- Rotation of a config service's own bootstrap credentials requires a protected root-of-trust and recovery path outside routine automation.
- Rotation receipts prove outcomes without disclosing sensitive values.
- Generalized customer integrations are deferred until after our internal workflow is proven.

## Revisit criteria

Review when the deterministic interfaces, the trust boundary, or organizational threat model materially changes. Changing this invariant requires a deliberate, documented security architecture decision; it is not a routine agent instruction update.
