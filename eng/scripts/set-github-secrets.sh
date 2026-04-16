#!/usr/bin/env bash
# Reads provisioning output from .azure/provision.env and writes GitHub Actions
# secrets used by .github/workflows/deploy-api.yml and deploy-web.yml.
#
# Secrets set:
#   AZURE_WEBAPP_NAME              — target App Service
#   AZURE_WEBAPP_PUBLISH_PROFILE   — publish profile XML for API deploys
#   AZURE_STATIC_WEB_APPS_API_TOKEN — deployment token for SWA
#
# Requires the `gh` CLI authenticated against the current repo.

set -euo pipefail

ENV_FILE=".azure/provision.env"
if [[ ! -f "$ENV_FILE" ]]; then
  echo "Missing $ENV_FILE — run eng/scripts/provision-azure.sh first." >&2
  exit 1
fi
# shellcheck disable=SC1090
source "$ENV_FILE"

echo ">>> Setting AZURE_WEBAPP_NAME=$API_NAME"
gh secret set AZURE_WEBAPP_NAME --body "$API_NAME"

echo ">>> Fetching publish profile for $API_NAME..."
PROFILE=$(az webapp deployment list-publishing-profiles \
  --name "$API_NAME" \
  --resource-group "$RG" \
  --xml)

echo ">>> Setting AZURE_WEBAPP_PUBLISH_PROFILE"
printf '%s' "$PROFILE" | gh secret set AZURE_WEBAPP_PUBLISH_PROFILE

echo ">>> Fetching SWA deployment token..."
SWA_TOKEN=$(az staticwebapp secrets list \
  --name "$SWA_NAME" \
  --resource-group "$RG" \
  --query "properties.apiKey" -o tsv)

echo ">>> Setting AZURE_STATIC_WEB_APPS_API_TOKEN"
printf '%s' "$SWA_TOKEN" | gh secret set AZURE_STATIC_WEB_APPS_API_TOKEN

echo
echo ">>> Secrets set. Verify with: gh secret list"
