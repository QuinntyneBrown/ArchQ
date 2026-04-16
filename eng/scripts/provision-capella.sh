#!/usr/bin/env bash
# Provisions a Couchbase Capella free-tier cluster for ArchQ via the v4 API.
# Idempotent: creating the bucket / user / CIDR twice is fine (422 on conflict).
#
# Required environment variables:
#   CAPELLA_AUTH   — base64("<accessKey>:<secretKey>") of an org-owner key
#   ORG_ID         — Capella organization ID
#   PROJECT_ID     — Capella project ID
#   CLUSTER_ID     — Capella cluster ID (must already exist)
#
# Writes credentials and the connection string to .azure/capella.env.

set -euo pipefail

: "${CAPELLA_AUTH:?set CAPELLA_AUTH to base64(accessKey:secretKey)}"
: "${ORG_ID:?set ORG_ID}"
: "${PROJECT_ID:?set PROJECT_ID}"
: "${CLUSTER_ID:?set CLUSTER_ID}"

BASE=https://cloudapi.cloud.couchbase.com
HDR=(-H "Authorization: Bearer $CAPELLA_AUTH" -H "Content-Type: application/json")
BASE_PATH="$BASE/v4/organizations/$ORG_ID/projects/$PROJECT_ID/clusters/$CLUSTER_ID"

mkdir -p .azure

echo ">>> Verifying cluster access..."
CONN=$(curl -sS "${HDR[@]}" "$BASE_PATH" | python3 -c 'import json,sys; print(json.load(sys.stdin).get("connectionString",""))')
if [[ -z "$CONN" ]]; then
  echo "Could not read cluster connection string — check auth + IDs." >&2
  exit 1
fi
echo "    Connection: couchbases://$CONN"

echo ">>> Ensuring bucket archq..."
curl -sS "${HDR[@]}" -X POST "$BASE_PATH/buckets" -d '{
  "name": "archq",
  "type": "couchbase",
  "storageBackend": "couchstore",
  "memoryAllocationInMb": 256,
  "bucketConflictResolution": "seqno",
  "durabilityLevel": "none",
  "replicas": 0,
  "flush": false,
  "timeToLiveInSeconds": 0,
  "evictionPolicy": "fullEviction"
}' | python3 -c 'import json,sys; d=json.load(sys.stdin); print("    OK" if "id" in d else f"    skipped: {d.get(\"message\",\"already exists\")}")'

echo ">>> Allowing 0.0.0.0/0 on cluster (demo)..."
curl -sS "${HDR[@]}" -X POST "$BASE_PATH/allowedcidrs" \
  -d '{"cidr":"0.0.0.0/0","comment":"demo — revisit before prod"}' \
  | python3 -c 'import json,sys; d=json.load(sys.stdin); print("    OK" if "id" in d else f"    skipped: {d.get(\"message\",\"already present\")}")'

echo ">>> Creating database credential archq_api..."
DB_PASS=$(openssl rand -base64 24 | tr -d '/+=' | head -c 28)Aa1!
curl -sS "${HDR[@]}" -X POST "$BASE_PATH/users" -d "{
  \"name\": \"archq_api\",
  \"password\": \"$DB_PASS\",
  \"access\": [
    {
      \"privileges\": [\"data_reader\", \"data_writer\"],
      \"resources\": { \"buckets\": [ { \"name\": \"archq\" } ] }
    }
  ]
}" | python3 -c 'import json,sys; d=json.load(sys.stdin); print("    OK" if "id" in d else f"    skipped: {d.get(\"message\",\"already exists\")}")'

cat > .azure/capella.env <<EOF
DB_USER=archq_api
DB_PASS=$DB_PASS
CONN=couchbases://$CONN
EOF

echo ">>> Wrote .azure/capella.env"
