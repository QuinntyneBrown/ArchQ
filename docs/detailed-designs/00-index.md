# Detailed Designs — Index

| # | Feature | Status | Traces To | Description |
|---|---------|--------|-----------|-------------|
| 01 | [Tenant Provisioning](01-tenant-provisioning/README.md) | Complete | L2-001, L2-002 | Create organizations with Couchbase scope isolation |
| 02 | [User Registration](02-user-registration/README.md) | Complete | L2-003 | Register with email/password, email verification, bcrypt hashing |
| 03 | [User Login & Session](03-user-login/README.md) | Complete | L2-003, L2-025 | Authenticate, JWT access/refresh tokens, account lockout |
| 04 | [Organization Switching](04-organization-switching/README.md) | Complete | L2-004, L2-002 | Switch active tenant, scoped session context |
| 05 | [Role Management](05-role-management/README.md) | Complete | L2-005, L2-006 | Assign/revoke Admin, Author, Reviewer, Viewer roles |
| 06 | [ADR Creation](06-adr-creation/README.md) | Complete | L2-007, L2-009 | Create ADR with Markdown editor, template pre-population |
| 07 | [ADR Editing](07-adr-editing/README.md) | Complete | L2-008 | Edit ADR content with revision tracking |
| 08 | [ADR List & Browse](08-adr-list-browse/README.md) | Complete | L2-020, L2-028, L2-029 | List, filter, paginate ADRs with responsive layout |
| 09 | [ADR Search](09-adr-search/README.md) | Complete | L2-019 | Full-text search via Couchbase FTS |
| 10 | [ADR Workflow](10-adr-workflow/README.md) | Complete | L2-010, L2-011 | Status transitions, state machine, notifications |
| 11 | [Approver Assignment](11-approver-assignment/README.md) | Complete | L2-012 | Assign reviewers when submitting for review |
| 12 | [Approval Decisions](12-approval-decisions/README.md) | Complete | L2-013, L2-014 | Record approve/reject decisions, threshold enforcement |
| 13 | [Meeting Notes](13-meeting-notes/README.md) | Complete | L2-015 | Attach meeting notes with title, date, attendees |
| 14 | [General Notes](14-general-notes/README.md) | Complete | L2-016 | Attach Markdown notes to ADRs |
| 15 | [Threaded Comments](15-threaded-comments/README.md) | Complete | L2-017 | Add threaded, Markdown-formatted comments |
| 16 | [Document Attachments](16-document-attachments/README.md) | Complete | L2-018 | Upload architecture documents (PDF, PNG, SVG, etc.) |
| 17 | [ADR Tagging](17-adr-tagging/README.md) | Draft | L2-021 | Tag management with autocomplete suggestions |
| 18 | [Version History & Diff](18-version-history/README.md) | Draft | L2-023 | View and compare ADR content versions |
| 19 | [Audit Trail](19-audit-trail/README.md) | Draft | L2-022 | Immutable audit log for all ADR changes |
| 20 | [Security & Validation](20-security-validation/README.md) | Draft | L2-024, L2-025, L2-026 | Input validation, XSS prevention, tenant isolation |
