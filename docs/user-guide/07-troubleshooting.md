# 7. Troubleshooting & FAQ

Common errors, what causes them, and how to fix them. If your issue isn't here, open a GitHub issue at <https://github.com/QuinntyneBrown/ArchQ/issues>.

---

## 7.1 Local development

### "Port 4200 is already in use"

Another process has the Angular port.

```bash
# macOS/Linux
$ lsof -i :4200
$ kill <PID>

# Windows (PowerShell)
> Get-NetTCPConnection -LocalPort 4200 | Select OwningProcess
> Stop-Process -Id <PID>
```

Or run on a different port: `npx ng serve --port 4300`.

### "Unable to connect to Couchbase" at API startup

- Confirm the container is running: `docker compose ps` — `couchbase` should be `healthy`.
- First boot can take 30–60 seconds. Re-run `dotnet run --project src/ArchQ.Api` once the health check turns green.
- If the log says `authentication failed`, your appsettings password doesn't match the container. Reset with `docker compose down -v && docker compose up -d couchbase` (⚠ wipes data).

### "JWT validation failed" in browser after upgrading

Happens when the signing key changed. Clear the `access_token` and `refresh_token` cookies (DevTools -> Application -> Cookies) or sign in again.

### Angular build fails with "Cannot find module '@angular/core'"

You skipped `npm install`, or `node_modules` is stale:

```bash
$ cd src/ArchQ.Web
$ rm -rf node_modules package-lock.json
$ npm install
```

### dotnet build fails with "SDK not found"

Install .NET 9 and confirm:

```bash
$ dotnet --list-sdks
```

If `global.json` pins a specific SDK version, install that exact version.

---

## 7.2 Authentication and permissions

### "Email not verified — please check your inbox"

In local dev, the verification email body is printed to the API console. Scroll up in the terminal running `dotnet run` and copy the verification link.

In Azure, check SMTP settings in App Service configuration. If you haven't configured an SMTP server, set `Smtp__Host` / `Smtp__Username` / `Smtp__Password` or use Azure Communication Services.

### "Account locked — too many failed attempts"

After 5 failed sign-ins within 15 minutes, the account is locked for 15 minutes. Wait, or as an Admin:

1. Sign in on another browser.
2. **Settings -> Team Members -> <user> -> Unlock Account**.

### "403 Forbidden" on every ADR action

Your role was changed while you were signed in. Refresh or sign out and back in.

### "Cannot approve your own ADR"

Self-approval is always blocked. Ask a teammate with the Reviewer role to approve.

---

## 7.3 ADR workflow

### Approve button isn't visible

Possible causes:

| Cause | Fix |
|-------|-----|
| You are not assigned as a reviewer | Ask the author to **Assign Approvers** and pick you |
| You don't have the Reviewer role | Ask an admin in **Settings -> Team Members** |
| The ADR is not in `In Review` status | Only `In Review` ADRs can be approved |
| You are the author | Self-approval is blocked — assign someone else |

### "Threshold reached but ADR still In Review"

Refresh the page — the status transition is server-side but the UI only refreshes on events. If the page still shows `In Review`, check the audit log — most likely a reviewer had their role revoked mid-flow, invalidating their approval.

### Rejected ADR can't be re-opened

Only the **original author** or an **Admin** can re-open. Viewer/Reviewer roles cannot.

---

## 7.4 File uploads

### "File type not allowed"

Supported: PDF, PNG, SVG, DRAWIO. The 10 MB cap is enforced server-side; trying to bypass it in devtools will get a 413.

### Large DRAWIO uploads appear blank

DRAWIO files are stored as-is but not rendered in-browser. Download the file to open it in draw.io desktop or <https://app.diagrams.net/>.

---

## 7.5 Search

### Search returns nothing

- Confirm the ADR is in your **active tenant** — search is scoped per tenant.
- Couchbase full-text search index can take a few seconds to update after a write. Wait 10 seconds and try again.
- Drafts belonging to other users don't appear in your results — they're not just hidden, they're excluded from the index for your session.

### Results are stale

Force a re-index (admin only):

```bash
# Against the deployed API
> curl -X POST https://<api-host>/api/admin/search/reindex \
     -H "Authorization: Bearer <admin-jwt>"
```

In local dev, restart the API — the index rebuilds on startup.

---

## 7.6 Azure deployment

### `swa deploy` fails with "no deployment token"

You haven't regenerated the token for the new SWA, or the SWA was deleted and recreated.

```bash
> az staticwebapp secrets list --name $SWA_NAME --resource-group $RG \
    --query "properties.apiKey" -o tsv
```

Pass that into `swa deploy --deployment-token`.

### API returns 502 Bad Gateway on first request

App Service cold-starts take 30–60 seconds on B1. Subsequent requests are fast. To avoid cold starts, enable **Always On**:

```bash
> az webapp config set --name $APP_NAME --resource-group $RG --always-on true
```

B1 supports Always On; F1 (free) does not — that's why the guide uses B1.

### CORS errors from the browser

The SPA origin doesn't match `Cors__AllowedOrigins` on the API.

```bash
> az webapp config appsettings set \
    --resource-group $RG --name $APP_NAME \
    --settings Cors__AllowedOrigins="https://<exact-swa-hostname>.azurestaticapps.net"
```

Restart the App Service after changing settings: `az webapp restart -g $RG -n $APP_NAME`.

### Capella connection refused

Check the **Allowed IPs** list in Capella. Either add `0.0.0.0/0` (demo only) or the outbound IP set of your App Service:

```bash
> az webapp show --name $APP_NAME --resource-group $RG --query outboundIpAddresses
```

Add each IP to Capella's allow list. Update on scale-out since outbound IPs can change.

---

## 7.7 Cost surprises

### Bill jumped this month

Check **Azure Cost Management -> Cost analysis**. Common culprits on the "cheapest" plan:

| Cause | Mitigation |
|-------|-----------|
| App Insights exceeded free tier | Set a 1 GB/day cap |
| App Service scaled up | `az appservice plan update --sku B1` |
| Accidental second App Service plan | `az appservice plan list -o table` and delete dupes |
| Outbound bandwidth from App Service | Move SPA traffic to SWA (which has its own CDN) |

---

## 7.8 FAQ

**Q: Can I delete an ADR?**
A: No. ADRs are immutable by design. The closest equivalent is `Superseded` or `Deprecated` — both preserve the audit trail.

**Q: Can I export all ADRs as Markdown files?**
A: Yes. Admins can run `archq export --tenant <slug> --format markdown` (CLI) or `GET /api/admin/export`.

**Q: Does ArchQ support SSO (SAML / OIDC)?**
A: Not in the current release. It's on the roadmap. For now, use email + password with strong passwords and rotate keys regularly.

**Q: Is there an API?**
A: Yes — swagger is at `/swagger` in development and the OpenAPI spec at `/swagger/v1/swagger.json` in all environments.

**Q: Can two people edit the same ADR at once?**
A: The server uses CAS (compare-and-set) optimistic concurrency. If two people save at the same time, the second save returns 409 Conflict. The loser refreshes and re-applies.

**Q: How do I rename my organisation?**
A: **Settings -> Tenant Settings -> Name**. The slug cannot change (URLs would break); to force a slug change, contact support or recreate the tenant.

**Q: Where are comments stored?**
A: In the tenant's Couchbase scope, separate from the ADR document. Each comment is a document with a reference to the ADR and parent comment id.

**Q: What browsers are supported?**
A: Chrome, Edge, Firefox, Safari — latest two stable versions each. The UI is responsive down to 375 px (iPhone SE).

---

## 7.9 Still stuck?

1. Check the API logs — locally in the terminal, on Azure via `az webapp log tail`.
2. Open browser DevTools -> Network and look at the failing request's response body for a specific error code.
3. Open a GitHub issue with:
   - ArchQ version (`git rev-parse --short HEAD`)
   - Browser + OS
   - Steps to reproduce
   - Relevant log snippet

See also [CONTRIBUTING.md](../../CONTRIBUTING.md) for how to submit a fix yourself.

---

**Back to the [Table of Contents](README.md).**
