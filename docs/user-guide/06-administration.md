# 6. Administration

Everything that lives under **Settings** in the sidebar. Admins only unless stated otherwise.

![ArchQ dashboard with sidebar](../designs/exports/Ae0g2.png)

---

## 6.1 Tenancy model

ArchQ is multi-tenant. Every organisation is a **tenant** and gets:

- A dedicated **Couchbase scope** (database-level isolation — no shared tables/documents).
- Its own users, ADR numbering sequence, templates, and approval threshold.
- A unique URL slug, e.g. `acme-corp`.

Users can belong to multiple tenants and switch between them via the **organisation dropdown** at the top of the sidebar without re-authenticating.

### Creating another tenant

From any page:

1. Click the organisation dropdown -> **+ New Organization**.
2. Enter a name and slug.
3. You become the Admin of the new tenant automatically.

The ADR numbering sequence resets to `ADR-001` inside the new tenant.

---

## 6.2 Roles and permissions

| Capability | Admin | Author | Reviewer | Viewer |
|------------|:-----:|:------:|:--------:|:------:|
| Read ADRs | ✅ | ✅ | ✅ | ✅ |
| Create ADR | ✅ | ✅ | — | — |
| Edit own Draft ADR | ✅ | ✅ | — | — |
| Edit any Draft ADR | ✅ | — | — | — |
| Submit for Review | ✅ | ✅ (own) | — | — |
| Assign approvers | ✅ | ✅ (own) | — | — |
| Approve / Reject | ✅ | — | ✅ (not own) | — |
| Mark Superseded | ✅ | — | — | — |
| Mark Deprecated | ✅ | — | — | — |
| Upload documents | ✅ | ✅ | ✅ | — |
| Add comments | ✅ | ✅ | ✅ | ✅ |
| Edit own comment (15 min window) | ✅ | ✅ | ✅ | ✅ |
| Invite users | ✅ | — | — | — |
| Change user roles | ✅ | — | — | — |
| Edit tenant settings / template | ✅ | — | — | — |
| View audit log | ✅ | — | — | — |

> **Self-approval is blocked everywhere** — even an Admin cannot approve their own ADR. Assign another reviewer.

### Changing a user's role

1. **Settings -> Team Members**.
2. Click the role dropdown on the row.
3. Choose the new role. Changes are effective immediately.

Sensitive operations re-check the live role on every request, not the JWT claim. If you demote a user from Admin, their open tab will start getting 403s on admin endpoints within seconds.

---

## 6.3 Approval threshold

The number of approvals required to auto-transition an ADR from `In Review -> Approved`. Tenant-wide; defaults to `2`.

1. **Settings -> Tenant Settings**.
2. Set **Approval threshold** (1–10).
3. Save.

Guidance:

| Team size | Suggested threshold |
|-----------|---------------------|
| 1–3 engineers | 1 |
| 4–10 | 2 (default) |
| 11–30 | 2–3 |
| 30+ with architectural review board | 3 + rotate reviewers |

Changes take effect on **newly submitted** ADRs. ADRs already in review keep their threshold from the submit time.

---

## 6.4 ADR templates

Templates pre-populate the editor when an author clicks **New ADR**.

1. **Settings -> Templates**.
2. Edit the **Default Template** or click **+ New Template**.
3. Markdown supported. Include the sections you want to enforce, e.g.:

```markdown
## Context

<!-- What forces are at play? -->

## Decision

<!-- What did we decide? -->

## Consequences

<!-- What becomes easier, harder, or different? -->

## Alternatives considered

<!-- Bullet list of rejected options -->
```

4. Save. The next **New ADR** action uses the updated template.

> Existing ADRs are **not** retrofit. Templates only apply at creation time.

---

## 6.5 Tags

Tags are autocompleted as you type. ArchQ tracks usage counts so popular tags surface first.

### Merging tags

If you end up with both `postgres` and `postgresql`:

1. **Settings -> Tags**.
2. Click the tag you want to remove.
3. Click **Merge into…** and pick the canonical tag.
4. Confirm. Every ADR with the old tag is updated atomically. The old tag is deleted.

### Renaming tags

Same flow but choose **Rename**. All references update in place.

---

## 6.6 Audit log

An append-only trail of every state transition, login, role change, and document upload. Admin-only.

1. **Settings -> Audit**.
2. Filter by ADR, user, action type, or date range.
3. Export to CSV for compliance reporting.

You cannot edit or delete audit entries — not even as an Admin. This is enforced at the repository layer.

---

## 6.7 Backups and data portability

### Local development

Docker volumes hold Couchbase data. Back up with:

```bash
$ docker compose exec couchbase cbbackupmgr backup \
    -a /opt/couchbase/var/backups -r archq-backup -c localhost \
    -u Administrator -p password123
```

### Azure + Capella

The free tier of Capella does **not** include automatic backups. For production-grade workloads upgrade to Capella Basic or higher. You can always **export** via `cbexport`:

```bash
$ cbexport json -c couchbases://cb.xxxxx.cloud.couchbase.com \
    -u archq_api -p <pw> -b archq -o archq-export.json
```

Store the file in an Azure Storage account (~$0.02/GB/month) for cold backups.

---

## 6.8 Rotating secrets

When an Admin leaves, a credential leaks, or every 90 days as a baseline:

1. **Rotate the JWT signing key** — update `Jwt__Key` in App Service settings. All existing sessions are invalidated on next request (users are forced to sign in again).
2. **Rotate the Capella user password** — update it in Capella, then `Couchbase__Password` in App Service, then restart.
3. **Revoke departed user accounts** — **Settings -> Team Members** -> Remove. Their active sessions are killed within one refresh cycle (15 min).

For production hardening, store these in **Azure Key Vault** and reference them from App Service (see [5.9 Going to production-grade](05-deploying-to-azure.md#59-going-to-production-grade-later)).

---

**Next:** [7. Troubleshooting & FAQ →](07-troubleshooting.md)
