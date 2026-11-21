#!/usr/bin/env bash
set -euo pipefail

export DOCKER_BUILDKIT=1
export COMPOSE_DOCKER_CLI_BUILD=1

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
    printf '*** BUILD Start *** %s\n' "$(timestamp)"
    printf '%s %s\n' "$service" "$(timestamp)"

    build_args=(--sbom=true --provenance=true --progress plain)
    
    docker compose -f ./docker-compose.yml build "${build_args[@]}" "$service"
    printf '*** BUILD End *** %s\n' "$(timestamp)"
done
