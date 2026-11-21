#!/usr/bin/env bash
set -euo pipefail


timestamp() {
    date '+%Y-%m-%d %H:%M:%S %Z'
}

services=(
    "accounts-service.default.svc.cluster.local"
    "users-service.default.svc.cluster.local"
    "search-service.default.svc.cluster.local"
    "gateway_yarp.default.svc.cluster.local"
)

for service in "${services[@]}"; do
    docker compose -f ./docker-compose.yml create "$service"
done
