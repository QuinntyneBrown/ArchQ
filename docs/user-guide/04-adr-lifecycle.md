# 4. ADR Lifecycle: Draft -> Approved

Every ADR in ArchQ moves through a finite-state machine. This chapter walks a decision from **Draft** all the way to **Approved** (and shows what happens when it takes a detour through **Rejected** or **Superseded**).

---

## 4.1 The state diagram

```
               submit for review                  last approval recorded
  ┌─────────┐ ──────────────▶  ┌───────────┐ ─────────────────▶ ┌──────────┐
  │  Draft  │                  │ In Review │                    │ Approved │
  └─────────┘ ◀───────revise── └───────────┘ ─────reject────▶  └──────────┘
       ▲                                                              │
       │                                                              │ superseded by newer ADR
       └─────────────────── Rejected ◀─────────────────────┐          ▼
                                                           │     ┌────────────┐
                                                           └────▶│ Superseded │
                                                                 └────────────┘
                                                                        │
                                                                        ▼
                                                                 ┌────────────┐
                                                                 │ Deprecated │
                                                                 └────────────┘
```

| State | Who can transition out of it | What it means |
|-------|------------------------------|---------------|
| **Draft** | Author | Being written; invisible to approval workflow |
| **In Review** | Assigned Reviewers (approve/reject), Author (withdraw) | Awaiting decisions |
| **Approved** | Admin (supersede/deprecate) | Binding decision |
| **Rejected** | Author (re-open as Draft) | Blocked; reason on record |
| **Superseded** | — (terminal) | Replaced by another ADR |
| **Deprecated** | — (terminal) | No longer applies, but nothing replaces it |

---

## 4.2 Prerequisites for the walkthrough

You will play two roles. Easiest is two browsers or a private window:

- **Window A** — logged in as an **Author** (e.g. Alice Chen).
- **Window B** — logged in as a **Reviewer** (e.g. Bob Kim).

Make sure the reviewer is assigned the **Reviewer** role under Settings -> Team Members.

---

## 4.3 Step 1 — Create a Draft

In **Window A** (Author):

1. Click **New ADR**.
2. Fill in a title and body. Cover Context, Decision, Consequences.
3. Click **Save Draft**.

The ADR now exists with status `Draft`. The badge is orange:

![Editing a Draft ADR](../designs/exports/Huy4L.png)

You can keep editing. Every save archives a new version.

### What Draft status means

- Only the Author (and Admins) can see and edit it.
- It does **not** appear in Reviewer dashboards.
- It has no approval record yet.

---

## 4.4 Step 2 — Assign approvers

Still in **Window A**:

1. Open the ADR detail page.
2. In the Approval Status panel click **Assign Approvers**.
3. Pick one or more teammates with the Reviewer role.

Rules enforced by the API:

- You cannot assign yourself. (Self-approval is blocked throughout the lifecycle.)
- You cannot assign someone without the Reviewer role.
- Your tenant's **approval threshold** (default: 2) determines how many approvals are required for auto-transition to Approved. Admins configure this in Settings — see [6. Administration](06-administration.md#approval-threshold).

---

## 4.5 Step 3 — Submit for Review

In **Window A**:

1. Open the ADR editor (or detail page).
2. Click **Submit for Review**.

The ADR transitions `Draft -> In Review`. The badge turns blue. The author can no longer edit the body until it's rejected or withdrawn.

Every assigned reviewer receives an email and an in-app notification.

---

## 4.6 Step 4 — Reviewer approves (or rejects)

Switch to **Window B** (Reviewer). The ADR appears under **ADR Records -> Status: In Review**.

Open the ADR. The header now shows **Approve** and **Reject** buttons:

![ADR detail — reviewer can approve or reject](../designs/exports/URQnS.png)

### Approve

1. Click **Approve**.
2. Optionally add a comment ("LGTM — cost analysis is solid").
3. Confirm.

Your name moves from *Pending review* to *Approved* in the Approval Status panel. The `1 of 2 approvals required` counter ticks up.

### Reject

1. Click **Reject**.
2. Enter a **reason** (required). This is stored on the approval record and surfaced to the author.
3. Confirm.

The ADR immediately transitions `In Review -> Rejected`. Other pending reviewers no longer need to act.

### On mobile

Mobile uses the same flow with stacked full-width buttons:

![Mobile Approve/Reject](../designs/exports/udRtT.png)

---

## 4.7 Step 5 — Collecting enough approvals

When a reviewer approves, one of two things happens:

- **Below threshold**: status stays `In Review`, counter updates, other reviewers still pending.
- **Threshold reached**: ArchQ automatically transitions the ADR to `Approved`. The badge turns green. Remaining pending approvals are cancelled.

The author and all reviewers get a notification that the decision is now binding.

> **Configuring the threshold** — Admins set the tenant-wide default in Settings. A higher threshold means slower, safer decisions; lower = faster, more trust-based. See [6. Administration](06-administration.md#approval-threshold).

---

## 4.8 Handling Rejected ADRs

A Rejected ADR is not deleted — it stays in the list with a red badge and the rejection reason.

The Author can:

1. Open the rejected ADR.
2. Click **Re-open as Draft**.
3. Edit the body to address the reviewer's concerns.
4. Re-submit for review. Previously recorded approvals are cleared; all reviewers must decide again.

---

## 4.9 Superseding an Approved ADR

Approved decisions are immutable. To *change* an approved decision:

1. Create a **new** ADR (`ADR-042`) that describes the replacement.
2. In its body, add a **Supersedes:** line referencing the old ADR.
3. Once the new ADR is Approved, open the old one and click **... -> Mark Superseded** (Admins only).
4. Enter the superseding ADR number.

The old ADR transitions `Approved -> Superseded` and links to its replacement. Both remain searchable. This preserves the historical record.

---

## 4.10 Deprecating an ADR

Sometimes a decision no longer applies and nothing replaces it (e.g., an API was removed entirely). Admins can transition `Approved -> Deprecated` directly:

1. Open the ADR.
2. Click **... -> Mark Deprecated**.
3. Enter a reason.

Deprecated ADRs surface in search with a grey badge and a "no longer applies" banner.

---

## 4.11 The complete audit trail

Every transition is captured in the **audit log**. Admins can open **Settings -> Audit** and filter by ADR, user, or action. Each entry records:

- Timestamp (ISO 8601, UTC)
- Actor (user ID + name at the time)
- Action (CreateDraft, SubmitForReview, Approve, Reject, Supersede, …)
- Before / after state
- Optional metadata (comment, rejection reason)

The audit log is **append-only**. Even admins cannot delete or edit entries.

---

## 4.12 Worked example

Timeline for ADR-002, "Adopt PostgreSQL as Primary Database":

| Time | Actor | Action | State after |
|------|-------|--------|-------------|
| Apr 1, 09:12 | Alice (Author) | CreateDraft | Draft |
| Apr 2, 14:05 | Alice | EditBody (v2) | Draft |
| Apr 3, 10:40 | Alice | AssignApprovers: [Bob, Dan] | Draft |
| Apr 3, 10:41 | Alice | SubmitForReview | In Review |
| Apr 5, 16:22 | Bob (Reviewer) | Approve ("cost analysis is solid") | In Review |
| Apr 8, 11:07 | Dan (Reviewer) | Approve | **Approved** (threshold 2 hit) |
| Apr 10, 09:00 | Alice | UploadArtifact (system-context-diagram.pdf) | Approved |

---

## 4.13 Next

You now know the full ADR lifecycle. Next up: how to ship this to Azure on the cheapest possible plan.

**Next:** [5. Deploying to Azure (Cheapest Plan) →](05-deploying-to-azure.md)
