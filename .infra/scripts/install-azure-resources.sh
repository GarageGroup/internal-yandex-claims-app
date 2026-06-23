#!/usr/bin/env bash
set -euo pipefail

required_env_vars=(
  DEPLOY_CLIENT_ID
  AZURE_RESOURCE_GROUP_NAME
  AZURE_APP_INSIGHTS_NAME
  AZURE_LOG_ANALYTICS_WORKSPACE_NAME
  AZURE_STORAGE_ACCOUNT_NAME
  AZURE_FUNCTION_PLAN_NAME
  AZURE_FUNCTION_NAME
)

for env_var in "${required_env_vars[@]}"; do
  if [ -z "${!env_var:-}" ]; then
    echo "Missing required environment variable: ${env_var}" >&2
    exit 1
  fi
done

resource_group_name="${AZURE_RESOURCE_GROUP_NAME}"
log_analytics_workspace_name="${AZURE_LOG_ANALYTICS_WORKSPACE_NAME}"
app_insights_name="${AZURE_APP_INSIGHTS_NAME}"
storage_account_name="${AZURE_STORAGE_ACCOUNT_NAME}"
function_plan_name="${AZURE_FUNCTION_PLAN_NAME}"
function_name="${AZURE_FUNCTION_NAME}"

echo "Resource group: ${resource_group_name}"

resource_group_info="$(az group show \
  --name "${resource_group_name}" \
  --query "{id:id, location:location}" \
  --output tsv \
  --only-show-errors 2>/dev/null || true)"

if [ -z "${resource_group_info}" ]; then
  subscription_id="$(az account show --query id --output tsv --only-show-errors)"
  resource_group_id="/subscriptions/${subscription_id}/resourceGroups/${resource_group_name}"

  echo "Resource group does not exist: ${resource_group_name}" >&2
  echo >&2
  echo "Ask an admin to run:" >&2
  echo "az group create --name \"${resource_group_name}\" --location \"<azure-region>\"" >&2
  echo >&2
  echo "az role assignment create --assignee \"${DEPLOY_CLIENT_ID}\" --role Contributor --scope \"${resource_group_id}\"" >&2

  exit 1
fi

read -r resource_group_id resource_group_location <<< "${resource_group_info}"

if [ -z "${resource_group_id}" ] || [ -z "${resource_group_location}" ]; then
  echo "Failed to resolve resource group id/location for: ${resource_group_name}" >&2
  exit 1
fi

echo "Found resource group: ${resource_group_name}"
echo "Resource group location: ${resource_group_location}"

has_contributor="$(az role assignment list \
  --assignee "${DEPLOY_CLIENT_ID}" \
  --scope "${resource_group_id}" \
  --include-inherited \
  --query "[?roleDefinitionName=='Contributor'] | length(@)" \
  --output tsv \
  --only-show-errors)"

if [ "${has_contributor:-0}" = "0" ]; then
  echo "Missing Contributor role for deployment principal: ${DEPLOY_CLIENT_ID}" >&2
  echo >&2
  echo "Ask an admin to run:" >&2
  echo "az role assignment create --assignee \"${DEPLOY_CLIENT_ID}\" --role Contributor --scope \"${resource_group_id}\"" >&2

  exit 1
fi

echo "Verified Contributor role for deployment principal: ${DEPLOY_CLIENT_ID}"

if az monitor log-analytics workspace show \
  --resource-group "${resource_group_name}" \
  --workspace-name "${log_analytics_workspace_name}" \
  --only-show-errors \
  --output none >/dev/null 2>&1; then
  echo "Log Analytics workspace already exists: ${log_analytics_workspace_name}"
else
  az monitor log-analytics workspace create \
    --resource-group "${resource_group_name}" \
    --workspace-name "${log_analytics_workspace_name}" \
    --location "${resource_group_location}" \
    --only-show-errors \
    --output none

  echo "Created Log Analytics workspace: ${log_analytics_workspace_name}"
fi

workspace_id="$(az monitor log-analytics workspace show \
  --resource-group "${resource_group_name}" \
  --workspace-name "${log_analytics_workspace_name}" \
  --query id \
  --output tsv \
  --only-show-errors)"

echo "Ensured Log Analytics workspace: ${log_analytics_workspace_name}"

az extension add \
  --name application-insights \
  --upgrade \
  --only-show-errors \
  --output none

if az monitor app-insights component show \
  --app "${app_insights_name}" \
  --resource-group "${resource_group_name}" \
  --only-show-errors \
  --output none >/dev/null 2>&1; then
  echo "Application Insights already exists: ${app_insights_name}"
else
  az monitor app-insights component create \
    --app "${app_insights_name}" \
    --location "${resource_group_location}" \
    --resource-group "${resource_group_name}" \
    --application-type web \
    --workspace "${workspace_id}" \
    --only-show-errors \
    --output none

  echo "Created Application Insights: ${app_insights_name}"
fi

app_insights_connection_string="$(az monitor app-insights component show \
  --app "${app_insights_name}" \
  --resource-group "${resource_group_name}" \
  --query connectionString \
  --output tsv \
  --only-show-errors)"

if az storage account show \
  --name "${storage_account_name}" \
  --resource-group "${resource_group_name}" \
  --only-show-errors \
  --output none >/dev/null 2>&1; then
  echo "Storage account already exists: ${storage_account_name}"
else
  az storage account create \
    --name "${storage_account_name}" \
    --resource-group "${resource_group_name}" \
    --location "${resource_group_location}" \
    --sku Standard_LRS \
    --kind StorageV2 \
    --min-tls-version TLS1_2 \
    --only-show-errors \
    --output none

  echo "Created storage account: ${storage_account_name}"
fi

storage_connection_string="$(az storage account show-connection-string \
  --name "${storage_account_name}" \
  --resource-group "${resource_group_name}" \
  --query connectionString \
  --output tsv \
  --only-show-errors)"

echo "Ensured storage account: ${storage_account_name}"

if az functionapp plan show \
  --name "${function_plan_name}" \
  --resource-group "${resource_group_name}" \
  --only-show-errors \
  --output none >/dev/null 2>&1; then
  az functionapp plan update \
    --name "${function_plan_name}" \
    --resource-group "${resource_group_name}" \
    --sku B1 \
    --number-of-workers 1 \
    --only-show-errors \
    --output none

  echo "Updated App Service plan to B1: ${function_plan_name}"
else
  az functionapp plan create \
    --name "${function_plan_name}" \
    --resource-group "${resource_group_name}" \
    --location "${resource_group_location}" \
    --sku B1 \
    --is-linux true \
    --number-of-workers 1 \
    --only-show-errors \
    --output none

  echo "Created App Service plan B1: ${function_plan_name}"
fi

plan_reserved="$(az functionapp plan show \
  --name "${function_plan_name}" \
  --resource-group "${resource_group_name}" \
  --query reserved \
  --output tsv \
  --only-show-errors)"

if [ "${plan_reserved}" != "true" ]; then
  echo "App Service plan must be Linux to host the Function App: ${function_plan_name}" >&2
  echo "Current plan is not Linux. Create a Linux App Service plan and rerun the deployment." >&2
  exit 1
fi

ensure_system_assigned_identity_enabled() {
  local identity_type
  identity_type="$(az functionapp identity show \
    --name "${function_name}" \
    --resource-group "${resource_group_name}" \
    --query type \
    --output tsv \
    --only-show-errors 2>/dev/null || true)"

  if [[ "${identity_type}" == *SystemAssigned* ]]; then
    echo "System-assigned managed identity already enabled for ${function_name}"
    return
  fi

  if [[ "${identity_type}" == *UserAssigned* ]]; then
    mapfile -t user_assigned_identity_ids < <(
      az functionapp identity show \
        --name "${function_name}" \
        --resource-group "${resource_group_name}" \
        --query "keys(userAssignedIdentities)" \
        --output tsv \
        --only-show-errors 2>/dev/null || true
    )

    if [ "${#user_assigned_identity_ids[@]}" -gt 0 ]; then
      az functionapp identity assign \
        --name "${function_name}" \
        --resource-group "${resource_group_name}" \
        --identities "[system]" "${user_assigned_identity_ids[@]}" \
        --only-show-errors \
        --output none
    else
      az functionapp identity assign \
        --name "${function_name}" \
        --resource-group "${resource_group_name}" \
        --identities "[system]" \
        --only-show-errors \
        --output none
    fi
  else
    az functionapp identity assign \
      --name "${function_name}" \
      --resource-group "${resource_group_name}" \
      --identities "[system]" \
      --only-show-errors \
      --output none
  fi

  echo "Enabled system-assigned managed identity for ${function_name}"
}

apply_function_app_settings() {
  local settings=(
    "AzureWebJobsStorage=${storage_connection_string}"
    "APPLICATIONINSIGHTS_CONNECTION_STRING=${app_insights_connection_string}"
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
    --name "${function_name}" \
    --resource-group "${resource_group_name}" \
    --settings "${settings[@]}" \
    --only-show-errors \
    --output none

  az functionapp config appsettings delete \
    --name "${function_name}" \
    --resource-group "${resource_group_name}" \
    --setting-names "${delete_settings[@]}" \
    --only-show-errors \
    --output none

  echo "Ensured Function App settings for ${function_name}"
}

if az functionapp show \
  --resource-group "${resource_group_name}" \
  --name "${function_name}" \
  --only-show-errors \
  --output none >/dev/null 2>&1; then
  echo "Function app already exists: ${function_name}"

  ensure_system_assigned_identity_enabled
  apply_function_app_settings
  exit 0
fi

az functionapp create \
  --resource-group "${resource_group_name}" \
  --plan "${function_plan_name}" \
  --name "${function_name}" \
  --storage-account "${storage_account_name}" \
  --os-type Linux \
  --runtime dotnet-isolated \
  --runtime-version 10 \
  --functions-version 4 \
  --disable-app-insights true \
  --assign-identity "[system]" \
  --only-show-errors \
  --output none

apply_function_app_settings

echo "Created Function app: ${function_name}"
