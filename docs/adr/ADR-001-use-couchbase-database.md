# ADR-001: Use Couchbase as the Primary Database

## Status

Accepted

## Date

2026-04-15

## Context

ArchQ is a multi-tenant application for managing Architecture Decision Records. The system requires:

- **Multi-tenancy with data isolation**: Each organization's ADRs, users, and configuration must be strictly isolated.
- **Flexible document structure**: ADRs carry variable supporting artifacts (meeting notes, general notes, comments, architecture documents) that differ per record and evolve over time.
- **Full-text search**: Users must search across ADR titles, Markdown body content, and tags within their tenant.
- **Version history**: Every edit to an ADR's Markdown content must be retrievable as a distinct version.
- **Performance at scale**: List and detail endpoints must meet sub-200ms p95 latency targets with growing data volumes.

A relational database would require extensive schema migrations as artifact types evolve, and full-text search would need a secondary system (e.g., Elasticsearch). A pure key-value store would lack the querying and indexing capabilities the application demands.

## Decision

The application must use **Couchbase** as its primary database.

## Consequences

### Positive

- **Document model fits ADR structure**: ADRs with their nested artifacts (notes, comments, attachments metadata) map naturally to JSON documents, avoiding complex joins and rigid schemas.
- **Built-in full-text search (FTS)**: Couchbase Full-Text Search eliminates the need for a separate search engine, reducing infrastructure complexity.
- **N1QL query language**: SQL-like querying over JSON documents provides familiar, powerful querying for filtering, sorting, and aggregating ADRs.
- **Scopes and collections for multi-tenancy**: Couchbase scopes and collections provide a natural mechanism for tenant data isolation at the database level.
- **Built-in caching**: The integrated managed cache layer supports the sub-200ms latency requirements without an external cache tier.
- **Horizontal scalability**: Couchbase's distributed architecture supports scaling out as the number of tenants and ADRs grows.

### Negative

- **Operational expertise**: The team must acquire or hire Couchbase operational knowledge for cluster management, rebalancing, and XDCR configuration.
- **Smaller ecosystem**: Compared to PostgreSQL or MongoDB, Couchbase has a smaller community and fewer third-party integrations.
- **Licensing costs**: Couchbase Enterprise edition (required for some advanced features) carries licensing costs that must be budgeted.

## Participants

- Quinntyne Brown
