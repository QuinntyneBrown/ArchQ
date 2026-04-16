#!/usr/bin/env bash
# Provisions the cheapest-plan Azure footprint for ArchQ:
#   - Resource group
#   - Linux App Service Plan (F1 Free tier — $0/month)
#   - Web App (.NET 9) for the API
#   - Static Web App (Free) for the Angular SPA
#
# F1 caveats: no Always On, sleeps after 20 min idle, 60 CPU-min/day, 1 GB RAM.
# Upgrade to B1 (~$13/mo) later with: az appservice plan update --sku B1
#
# Idempotent: running it twice is safe. Resource names are stable and stored in
# .azure/provision.env so CI and re-runs can reuse them.
#
# Usage:
#   ./eng/scripts/provision-azure.sh [location] [sku]

set -euo pipefail

LOCATION="${1:-eastus2}"
SKU="${2:-F1}"
RG="archq-prod-rg"
PLAN="archq-plan"
ENV_FILE=".azure/provision.env"

mkdir -p .azure

if [[ -f "$ENV_FILE" ]]; then
  # shellcheck disable=SC1090
  source "$ENV_FILE"
fi

# Generate stable suffixes on first run, reuse on subsequent runs.
SUFFIX="${SUFFIX:-$(openssl rand -hex 3)}"
API_NAME="${API_NAME:-archq-api-${SUFFIX}}"
SWA_NAME="${SWA_NAME:-archq-web-${SUFFIX}}"

echo ">>> Subscription: $(az account show --query name -o tsv)"
echo ">>> Location:     $LOCATION"
echo ">>> Resource Grp: $RG"
echo ">>> API name:     $API_NAME"
echo ">>> SWA name:     $SWA_NAME"
echo

echo ">>> Creating resource group..."
az group create \
  --name "$RG" \
  --location "$LOCATION" \
  --output none

echo ">>> Creating App Service Plan (Linux $SKU)..."
az appservice plan create \
  --name "$PLAN" \
  --resource-group "$RG" \
  --sku "$SKU" \
  --is-linux \
  --output none

echo ">>> Creating Web App (.NET 9)..."
az webapp create \
  --resource-group "$RG" \
  --plan "$PLAN" \
  --name "$API_NAME" \
  --runtime "DOTNETCORE:9.0" \
  --output none

echo ">>> Enforcing HTTPS-only on Web App..."
az webapp update \
  --resource-group "$RG" \
  --name "$API_NAME" \
  --https-only true \
  --output none

if [[ "$SKU" != "F1" && "$SKU" != "FREE" ]]; then
  echo ">>> Enabling Always On..."
  az webapp config set \
    --resource-group "$RG" \
    --name "$API_NAME" \
    --always-on true \
    --output none
else
  echo ">>> Skipping Always On (not supported on F1)."
fi

echo ">>> Configuring default app settings (secrets still need Capella)..."
JWT_SECRET=$(openssl rand -base64 48)
az webapp config appsettings set \
  --resource-group "$RG" \
  --name "$API_NAME" \
  --settings \
    ASPNETCORE_ENVIRONMENT=Production \
    Jwt__Secret="$JWT_SECRET" \
    Jwt__Issuer="https://${API_NAME}.azurewebsites.net" \
    Jwt__Audience="https://${API_NAME}.azurewebsites.net" \
    Couchbase__BucketName="archq" \
  --output none

echo ">>> Creating Static Web App (Free tier)..."
# SWA Free is region-restricted — fall back to eastus2 if LOCATION is unsupported.
SWA_LOCATION="$LOCATION"
if ! az staticwebapp create \
  --name "$SWA_NAME" \
  --resource-group "$RG" \
  --location "$SWA_LOCATION" \
  --sku Free \
  --output none 2>/dev/null; then
  echo "    (retrying in eastus2)"
  SWA_LOCATION="eastus2"
  az staticwebapp create \
    --name "$SWA_NAME" \
    --resource-group "$RG" \
    --location "$SWA_LOCATION" \
    --sku Free \
    --output none
fi

SWA_HOSTNAME=$(az staticwebapp show \
  --name "$SWA_NAME" \
  --resource-group "$RG" \
  --query defaultHostname -o tsv)

echo ">>> Wiring CORS on API -> https://${SWA_HOSTNAME}"
az webapp config appsettings set \
  --resource-group "$RG" \
  --name "$API_NAME" \
  --settings Cors__AllowedOrigins="https://${SWA_HOSTNAME}" \
  --output none

cat > "$ENV_FILE" <<EOF
SUFFIX=$SUFFIX
API_NAME=$API_NAME
SWA_NAME=$SWA_NAME
RG=$RG
LOCATION=$LOCATION
SWA_HOSTNAME=$SWA_HOSTNAME
API_HOSTNAME=${API_NAME}.azurewebsites.net
EOF

echo
echo ">>> Done."
echo "    API:  https://${API_NAME}.azurewebsites.net"
echo "    Web:  https://${SWA_HOSTNAME}"
echo
echo "Next steps:"
echo "  1. Create a Couchbase Capella free cluster + user at https://cloud.couchbase.com/"
echo "  2. Run: ./eng/scripts/set-github-secrets.sh"
echo "  3. Push to main — GitHub Actions will deploy."
