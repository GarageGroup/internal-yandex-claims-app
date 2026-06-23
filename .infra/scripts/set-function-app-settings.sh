#!/usr/bin/env bash
set -euo pipefail

required_env_vars=(
  AZURE_RESOURCE_GROUP_NAME
  AZURE_FUNCTION_NAME
)

for env_var in "${required_env_vars[@]}"; do
  if [ -z "${!env_var:-}" ]; then
    echo "Missing required environment variable: ${env_var}" >&2
    exit 1
  fi
done

ensure_system_assigned_identity_enabled() {
  local identity_type
  identity_type="$(az functionapp identity show \
    --name "${AZURE_FUNCTION_NAME}" \
    --resource-group "${AZURE_RESOURCE_GROUP_NAME}" \
    --query type \
    --output tsv \
    --only-show-errors 2>/dev/null || true)"

  if [[ "${identity_type}" == *SystemAssigned* ]]; then
    echo "System-assigned managed identity already enabled for ${AZURE_FUNCTION_NAME}"
    return
  fi

  if [[ "${identity_type}" == *UserAssigned* ]]; then
    mapfile -t user_assigned_identity_ids < <(
      az functionapp identity show \
        --name "${AZURE_FUNCTION_NAME}" \
        --resource-group "${AZURE_RESOURCE_GROUP_NAME}" \
        --query "keys(userAssignedIdentities)" \
        --output tsv \
        --only-show-errors 2>/dev/null || true
    )

    if [ "${#user_assigned_identity_ids[@]}" -gt 0 ]; then
      az functionapp identity assign \
        --name "${AZURE_FUNCTION_NAME}" \
        --resource-group "${AZURE_RESOURCE_GROUP_NAME}" \
        --identities "[system]" "${user_assigned_identity_ids[@]}" \
        --only-show-errors \
        --output none
    else
      az functionapp identity assign \
        --name "${AZURE_FUNCTION_NAME}" \
        --resource-group "${AZURE_RESOURCE_GROUP_NAME}" \
        --identities "[system]" \
        --only-show-errors \
        --output none
    fi
  else
    az functionapp identity assign \
      --name "${AZURE_FUNCTION_NAME}" \
      --resource-group "${AZURE_RESOURCE_GROUP_NAME}" \
      --identities "[system]" \
      --only-show-errors \
      --output none
  fi

  echo "Enabled system-assigned managed identity for ${AZURE_FUNCTION_NAME}"
}

apply_function_app_settings() {
  local settings=(
    "Info__DeployDateTime=$(date -u +"%Y-%m-%dT%H:%M:%SZ")"
    "WEBSITE_RUN_FROM_PACKAGE=1"
    "AzureTokenCredential__TokenType=SystemAssignedManagedIdentity"
  )

  local delete_settings=(
    "AZURE_TOKEN_TYPE"
    "AZURE_CLIENT_ID"
    "AZURE_CLIENT_SECRET"
    "AZURE_TENANT_ID"
    "AzureTokenCredential__ClientId"
    "AzureTokenCredential__ClientSecret"
    "AzureTokenCredential__TenantId"
  )

  az functionapp config appsettings set \
    --name "${AZURE_FUNCTION_NAME}" \
    --resource-group "${AZURE_RESOURCE_GROUP_NAME}" \
    --settings "${settings[@]}" \
    --only-show-errors \
    --output none

  az functionapp config appsettings delete \
    --name "${AZURE_FUNCTION_NAME}" \
    --resource-group "${AZURE_RESOURCE_GROUP_NAME}" \
    --setting-names "${delete_settings[@]}" \
    --only-show-errors \
    --output none

  echo "Updated Function App settings for ${AZURE_FUNCTION_NAME}"
}

ensure_system_assigned_identity_enabled
apply_function_app_settings
