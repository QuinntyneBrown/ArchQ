#!/bin/bash
set -e

echo "Waiting for Couchbase to start..."
until curl -s http://localhost:8091/pools > /dev/null; do
  sleep 2
done

echo "Initializing cluster..."
/opt/couchbase/bin/couchbase-cli cluster-init \
  -c localhost:8091 \
  --cluster-username Administrator \
  --cluster-password password123 \
  --cluster-ramsize 512 \
  --cluster-index-ramsize 256 \
  --cluster-fts-ramsize 256 \
  --services data,index,query,fts

echo "Creating bucket..."
/opt/couchbase/bin/couchbase-cli bucket-create \
  -c localhost:8091 \
  -u Administrator \
  -p password123 \
  --bucket archq \
  --bucket-type couchbase \
  --bucket-ramsize 256

echo "Creating system scope..."
/opt/couchbase/bin/couchbase-cli collection-manage \
  -c localhost:8091 \
  -u Administrator \
  -p password123 \
  --bucket archq \
  --create-scope system

echo "Creating system collections..."
/opt/couchbase/bin/couchbase-cli collection-manage \
  -c localhost:8091 \
  -u Administrator \
  -p password123 \
  --bucket archq \
  --create-collection system.tenants

/opt/couchbase/bin/couchbase-cli collection-manage \
  -c localhost:8091 \
  -u Administrator \
  -p password123 \
  --bucket archq \
  --create-collection system.global_users

echo "Couchbase initialization complete."
