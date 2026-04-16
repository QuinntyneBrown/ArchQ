# 5. Deploying to Azure on the Cheapest Plan

This chapter ships ArchQ to Azure for the **lowest defensible monthly cost** — roughly **USD $15–$25/month** (as of 2026, pricing changes frequently; verify in the Azure Pricing Calculator).

It's the right setup for a small team (≤ 10 users), a proof-of-concept, or an internal demo. It is **not** production-grade for regulated workloads — see [Appendix: Upgrading for production](#appendix-upgrading-for-production) at the end.

---

## 5.1 The architecture

```
  ┌────────────────────────┐       ┌──────────────────────────┐
  │ Azure Static Web Apps  │       │ Azure App Service        │
  │   (Free tier)          │──────▶│   (Linux, B1 Basic)      │
  │   Angular SPA          │  API  │   ASP.NET Core API       │
  └────────────────────────┘       └──────────────────────────┘
                                              │
                                              ▼
                                   ┌──────────────────────────┐
                                   │ Couchbase Capella        │
                                   │   (Free tier, 1 cluster) │
                                   └──────────────────────────┘
```

| Component | SKU | Monthly cost | Why this SKU |
|-----------|-----|--------------|--------------|
| **Angular SPA** | Azure Static Web Apps — **Free** | $0 | 100 GB/month bandwidth, custom domain, HTTPS — enough for a small team |
| **API** | Azure App Service — **B1** (Linux) | ~$13/mo | Smallest always-on tier, 1.75 GB RAM, 1 vCPU |
| **Database** | Couchbase Capella — **Free tier** | $0 | 1 GB RAM, 1 node, hosted, perfect for demos |
| **Logs & telemetry** | App Insights — **pay-as-you-go** (first 5 GB free) | ~$0–2 | Cap ingestion to stay under the free allowance |
| **Total** | | **~$13–$15/mo** | |

> **Why not Azure Container Apps or AKS?** They are excellent for scale, but the smallest realistic bill is higher. App Service B1 has a fixed, predictable price and supports easy slot-based deploys.
>
> **Why Couchbase Capella instead of a VM?** Running Couchbase on a self-managed B-series VM costs more than Capella's free tier and requires you to patch, backup, and monitor it yourself.

---

## 5.2 Prerequisites

- An active Azure subscription. Free trial is fine for the first 12 months.
- A Couchbase Capella account — sign up at <https://cloud.couchbase.com/>.
- The Azure CLI installed and signed in:

```bash
$ az login
$ az account set --subscription "<your-subscription-id>"
```

- The ArchQ repository cloned locally (see [1. Installation](01-installation.md)).

Set a few shell variables you'll reuse:

```bash
$ RG=archq-prod-rg
$ LOCATION=eastus            # pick the closest region with Free SWA
$ APP_NAME=archq-api-$RANDOM # must be globally unique
$ PLAN_NAME=archq-plan
$ SWA_NAME=archq-web-$RANDOM
```

---

## 5.3 Step 1 — Create the Capella (database) cluster

1. Sign in at <https://cloud.couchbase.com>.
2. Click **Create Cluster -> Free Tier**.
3. Region: **AWS us-east-1** (closest to Azure East US).
4. Name: `archq-prod`.
5. Wait ~5 minutes for provisioning.

Once ready:

1. **Connect -> Allowed IPs** -> Add **0.0.0.0/0** (or, better, the outbound IP of the App Service from step 5.4).
2. **Database Access -> Create** — user `archq_api`, strong random password. Grant **Application Access** on the `archq` bucket.
3. **Connect -> Connection String** — copy the `couchbases://…` URI (TLS-enabled).

Create the bucket:

1. **Buckets -> Create Bucket** -> name `archq`, RAM 256 MB, storage backend Couchstore.
2. Create scopes later at first boot via `ArchQ.Api` (it provisions scopes per tenant automatically).

Record these values — you'll paste them into the API config shortly:

```text
COUCHBASE__CONNECTIONSTRING = couchbases://cb.xxxxx.cloud.couchbase.com
COUCHBASE__USERNAME         = archq_api
COUCHBASE__PASSWORD         = <the one you just set>
COUCHBASE__BUCKETNAME       = archq
```

---

## 5.4 Step 2 — Create the App Service (API)

### Create the resource group and plan

```bash
> az group create --name $RG --location $LOCATION

> az appservice plan create \
    --name $PLAN_NAME \
    --resource-group $RG \
    --sku B1 \
    --is-linux
```

`--sku B1` is the cheapest always-on SKU. The F1 free tier exists but sleeps after 20 minutes of inactivity and caps at 60 CPU-minutes/day — not enough for an API.

### Create the Web App

```bash
> az webapp create \
    --resource-group $RG \
    --plan $PLAN_NAME \
    --name $APP_NAME \
    --runtime "DOTNETCORE:9.0"
```

### Configure app settings (secrets)

```bash
> az webapp config appsettings set \
    --resource-group $RG \
    --name $APP_NAME \
    --settings \
        ASPNETCORE_ENVIRONMENT=Production \
        Couchbase__ConnectionString="couchbases://cb.xxxxx.cloud.couchbase.com" \
        Couchbase__Username="archq_api" \
        Couchbase__Password="<strong-password>" \
        Couchbase__BucketName="archq" \
        Jwt__Key="$(openssl rand -base64 48)" \
        Jwt__Issuer="https://$APP_NAME.azurewebsites.net" \
        Jwt__Audience="https://$APP_NAME.azurewebsites.net" \
        Cors__AllowedOrigins="https://<your-swa-hostname>.azurestaticapps.net"
```

Replace `<your-swa-hostname>` once you have it (step 5.5). `Jwt__Key` must be at least 32 bytes — the `openssl` one-liner above produces a 64-char base64 string.

### Enable HTTPS-only

```bash
> az webapp update --resource-group $RG --name $APP_NAME --https-only true
```

### Publish the API

```bash
$ dotnet publish src/ArchQ.Api -c Release -o ./publish
$ cd publish
$ zip -r ../archq-api.zip .
$ cd ..
> az webapp deploy \
    --resource-group $RG \
    --name $APP_NAME \
    --src-path archq-api.zip \
    --type zip
```

Hit `https://$APP_NAME.azurewebsites.net/health`. Expect `{"status":"Healthy"}`.

---

## 5.5 Step 3 — Deploy the Angular SPA to Static Web Apps

### Set the production API base URL

Edit `src/ArchQ.Web/src/environments/environment.ts`:

```typescript
export const environment = {
  production: true,
  apiBaseUrl: 'https://archq-api-xxxxx.azurewebsites.net'
};
```

### Build

```bash
$ cd src/ArchQ.Web
$ npm ci
$ npx ng build --configuration=production
$ cd ../..
```

The output is in `src/ArchQ.Web/dist/ArchQ.Web/browser`.

### Create the Static Web App

```bash
> az staticwebapp create \
    --name $SWA_NAME \
    --resource-group $RG \
    --location $LOCATION \
    --sku Free
```

### Upload the built SPA

The fastest path is the **SWA CLI**:

```bash
$ npm install -g @azure/static-web-apps-cli
$ swa deploy src/ArchQ.Web/dist/ArchQ.Web/browser \
    --deployment-token $(az staticwebapp secrets list --name $SWA_NAME --resource-group $RG --query "properties.apiKey" -o tsv) \
    --env production
```

The command prints the final hostname, e.g. `https://zealous-tree-0abc123.azurestaticapps.net`.

### Update CORS on the API

```bash
> az webapp config appsettings set \
    --resource-group $RG --name $APP_NAME \
    --settings Cors__AllowedOrigins="https://zealous-tree-0abc123.azurestaticapps.net"
```

---

## 5.6 Step 4 — Smoke test

1. Open the SWA URL in your browser.
2. Register a new account. Confirm the verification email arrives (or read the API log below).
3. Sign in and create a test ADR.

View API logs:

```bash
> az webapp log tail --name $APP_NAME --resource-group $RG
```

---

## 5.7 Step 5 — App Insights (optional, first 5 GB free)

```bash
> az monitor app-insights component create \
    --app archq-insights \
    --location $LOCATION \
    --resource-group $RG \
    --application-type web

> INSTR_KEY=$(az monitor app-insights component show \
    --app archq-insights -g $RG --query connectionString -o tsv)

> az webapp config appsettings set \
    --resource-group $RG --name $APP_NAME \
    --settings APPLICATIONINSIGHTS_CONNECTION_STRING="$INSTR_KEY"
```

In App Insights, go to **Usage and estimated costs -> Daily cap** and set a cap of **1 GB/day** to guarantee you stay in the free tier.

---

## 5.8 Cost-control checklist

| Check | How |
|-------|-----|
| No idle non-B1 plans | `az appservice plan list -o table` — delete anything unused |
| Daily cap on App Insights | App Insights blade -> Usage -> Daily cap |
| Alert on budget overruns | Cost Management -> Budgets -> set to $20/mo |
| Auto-scale OFF | App Service -> Scale out -> manual, 1 instance |
| Backups OFF | Backups feature is only on Standard+ tiers anyway on B1, so nothing to do |
| Custom domain uses Free tier SSL | SWA ships free certs; App Service uses App Service Managed Certificate (free for custom domains) |

---

## 5.9 Going to production-grade later

For more demanding workloads, upgrade these in order (cost impact shown):

1. **App Service B1 -> P0v3** (~$55/mo) — more RAM, faster CPU, deployment slots for zero-downtime deploys.
2. **Capella free -> Capella Basic** (~$50/mo) — daily backups, more RAM, support SLA.
3. **Add Azure Front Door Standard** (~$35/mo base + traffic) — global edge, WAF.
4. **Managed identity + Key Vault** (free, no runtime cost) — remove secrets from app settings.

See also the [ADRs in the repository](../adr/) for the rationale behind the current database and auth choices.

---

## Appendix: Upgrading for production

If you plan to handle real data or regulated workloads, read these ADRs before going live:

- [ADR-001 Use Couchbase Database](../adr/ADR-001-use-couchbase-database.md)
- The [detailed designs](../detailed-designs/) folder for security and tenancy specifics.

Specifically, replace the defaults in this chapter with:

- JWT signing key in **Azure Key Vault** (referenced from App Service with `@Microsoft.KeyVault(...)` syntax).
- Capella **paid tier** with cross-region replication.
- App Service **scale-out** with 2+ instances and a health-check-backed load balancer.
- Restrict Capella network access to the **App Service outbound IPs**, not `0.0.0.0/0`.

**Next:** [6. Administration →](06-administration.md)
