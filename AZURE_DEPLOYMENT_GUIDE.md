# Azure Deployment Guide for Hopewell Clinic API

This guide provides step-by-step instructions for deploying the Hopewell Clinic API backend to Azure App Service.

## Prerequisites

- Azure CLI installed and configured
- .NET 8.0 SDK installed
- Azure subscription with appropriate permissions
- Git repository with the Hopewell Clinic API code

## Step 1: Prepare the Application

### 1.1 Build the Application
```bash
# Navigate to the project directory
cd C:\Project\HopewellClinicApi

# Build the application in Release mode
dotnet build -c Release
```

### 1.2 Publish the Application
```bash
# Publish the application for Azure deployment
dotnet publish -c Release -o publish-azure
```

### 1.3 Create Deployment Package
```bash
# Create a zip package for deployment
Compress-Archive -Path "publish-azure\*" -DestinationPath "hopewell-enhanced-backend.zip" -Force
```

## Step 2: Azure Authentication

### 2.1 Login to Azure CLI
```bash
# Login using device code authentication
az login --use-device-code
```

### 2.2 Verify Subscription
```bash
# Check current subscription
az account show

# List available subscriptions
az account list --output table
```

### 2.3 Set Correct Subscription (if needed)
```bash
# Set the correct subscription
az account set --subscription "Your-Subscription-Name"
```

## Step 3: Identify Azure Resources

### 3.1 List Resource Groups
```bash
# List all resource groups
az group list --output table
```

### 3.2 List Web Apps
```bash
# List web apps in a specific resource group
az webapp list --resource-group "Your-Resource-Group-Name" --output table
```

### 3.3 Get Web App Details
```bash
# Get detailed information about a specific web app
az webapp show --resource-group "Your-Resource-Group-Name" --name "Your-Web-App-Name"
```

## Step 4: Deploy to Azure

### 4.1 Deploy Using Azure CLI
```bash
# Deploy the application to Azure App Service
az webapp deploy \
  --resource-group "Your-Resource-Group-Name" \
  --name "Your-Web-App-Name" \
  --src-path "hopewell-enhanced-backend.zip" \
  --type zip
```

### 4.2 Verify Deployment
```bash
# Check deployment status
az webapp deployment list --resource-group "Your-Resource-Group-Name" --name "Your-Web-App-Name"
```

## Step 5: Test the Deployment

### 5.1 Test Basic Endpoints
```bash
# Test the health endpoint
curl -X GET "https://your-app-name.azurewebsites.net/health"

# Test the search endpoint
curl -X GET "https://your-app-name.azurewebsites.net/api/Appointments/search?page=1&pageSize=5" \
  -H "accept: application/json"

# Test the doctors on duty endpoint
curl -X GET "https://your-app-name.azurewebsites.net/api/Booking/doctors-on-duty?date=2025-10-15" \
  -H "accept: application/json"
```

### 5.2 Verify Application Logs
```bash
# View application logs
az webapp log tail --resource-group "Your-Resource-Group-Name" --name "Your-Web-App-Name"
```

## Step 6: Configuration Management

### 6.1 Set Application Settings
```bash
# Set connection string
az webapp config connection-string set \
  --resource-group "Your-Resource-Group-Name" \
  --name "Your-Web-App-Name" \
  --connection-string-type SQLServer \
  --settings "DefaultConnection=your-connection-string"
```

### 6.2 Set App Settings
```bash
# Set JWT secret
az webapp config appsettings set \
  --resource-group "Your-Resource-Group-Name" \
  --name "Your-Web-App-Name" \
  --settings "JWT_SECRET=your-jwt-secret"
```

## Step 7: Monitoring and Maintenance

### 7.1 Enable Application Insights (Optional)
```bash
# Create Application Insights resource
az monitor app-insights component create \
  --app "your-app-insights-name" \
  --location "southafricanorth" \
  --resource-group "Your-Resource-Group-Name"
```

### 7.2 Configure Logging
```bash
# Enable application logging
az webapp log config \
  --resource-group "Your-Resource-Group-Name" \
  --name "Your-Web-App-Name" \
  --application-logging filesystem \
  --level information
```

## Troubleshooting

### Common Issues and Solutions

#### 1. Authentication Errors
```bash
# Re-authenticate if you get permission errors
az login --use-device-code

# Check your subscription and permissions
az account show
az role assignment list --assignee "your-email@domain.com"
```

#### 2. Deployment Failures
```bash
# Check deployment logs
az webapp deployment list --resource-group "Your-Resource-Group-Name" --name "Your-Web-App-Name"

# View detailed deployment logs
az webapp deployment log show --resource-group "Your-Resource-Group-Name" --name "Your-Web-App-Name"
```

#### 3. Application Startup Issues
```bash
# Check application logs
az webapp log tail --resource-group "Your-Resource-Group-Name" --name "Your-Web-App-Name"

# Restart the application
az webapp restart --resource-group "Your-Resource-Group-Name" --name "Your-Web-App-Name"
```

#### 4. Database Connection Issues
```bash
# Verify connection string
az webapp config connection-string list --resource-group "Your-Resource-Group-Name" --name "Your-Web-App-Name"

# Test database connectivity
az webapp config connection-string set \
  --resource-group "Your-Resource-Group-Name" \
  --name "Your-Web-App-Name" \
  --connection-string-type SQLServer \
  --settings "DefaultConnection=your-updated-connection-string"
```

## Environment-Specific Deployment

### Development Environment
```bash
# Deploy to development slot
az webapp deployment slot create \
  --resource-group "Your-Resource-Group-Name" \
  --name "Your-Web-App-Name" \
  --slot "dev"

# Deploy to development slot
az webapp deploy \
  --resource-group "Your-Resource-Group-Name" \
  --name "Your-Web-App-Name" \
  --slot "dev" \
  --src-path "hopewell-enhanced-backend.zip" \
  --type zip
```

### Production Environment
```bash
# Deploy to production
az webapp deploy \
  --resource-group "Your-Resource-Group-Name" \
  --name "Your-Web-App-Name" \
  --src-path "hopewell-enhanced-backend.zip" \
  --type zip

# Swap slots if using staging
az webapp deployment slot swap \
  --resource-group "Your-Resource-Group-Name" \
  --name "Your-Web-App-Name" \
  --slot "staging" \
  --target-slot "production"
```

## Security Considerations

### 1. HTTPS Configuration
```bash
# Enable HTTPS only
az webapp update \
  --resource-group "Your-Resource-Group-Name" \
  --name "Your-Web-App-Name" \
  --https-only true
```

### 2. Authentication Settings
```bash
# Configure authentication
az webapp auth update \
  --resource-group "Your-Resource-Group-Name" \
  --name "Your-Web-App-Name" \
  --enabled true \
  --action LoginWithAzureActiveDirectory
```

## Performance Optimization

### 1. Enable Always On
```bash
# Enable Always On for better performance
az webapp config set \
  --resource-group "Your-Resource-Group-Name" \
  --name "Your-Web-App-Name" \
  --always-on true
```

### 2. Configure Auto-scaling
```bash
# Create auto-scale rule
az monitor autoscale create \
  --resource-group "Your-Resource-Group-Name" \
  --resource "Your-Web-App-Name" \
  --resource-type Microsoft.Web/sites \
  --name "autoscale-rule" \
  --min-count 1 \
  --max-count 10 \
  --count 2
```

## Backup and Recovery

### 1. Create Backup
```bash
# Create a backup
az webapp config backup create \
  --resource-group "Your-Resource-Group-Name" \
  --webapp-name "Your-Web-App-Name" \
  --backup-name "backup-$(date +%Y%m%d)"
```

### 2. Restore from Backup
```bash
# Restore from backup
az webapp config backup restore \
  --resource-group "Your-Resource-Group-Name" \
  --webapp-name "Your-Web-App-Name" \
  --backup-name "backup-20250101" \
  --container-url "https://your-storage-account.blob.core.windows.net/backups"
```

## Complete Deployment Script

Here's a complete PowerShell script that automates the entire deployment process:

```powershell
# Azure Deployment Script for Hopewell Clinic API
param(
    [Parameter(Mandatory=$true)]
    [string]$ResourceGroupName,
    
    [Parameter(Mandatory=$true)]
    [string]$WebAppName,
    
    [Parameter(Mandatory=$false)]
    [string]$SlotName = "production"
)

Write-Host "Starting Azure deployment for Hopewell Clinic API..." -ForegroundColor Green

# Step 1: Build and Publish
Write-Host "Building application..." -ForegroundColor Yellow
dotnet build -c Release
if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed!"
    exit 1
}

Write-Host "Publishing application..." -ForegroundColor Yellow
dotnet publish -c Release -o publish-azure
if ($LASTEXITCODE -ne 0) {
    Write-Error "Publish failed!"
    exit 1
}

# Step 2: Create deployment package
Write-Host "Creating deployment package..." -ForegroundColor Yellow
Compress-Archive -Path "publish-azure\*" -DestinationPath "hopewell-deployment.zip" -Force

# Step 3: Deploy to Azure
Write-Host "Deploying to Azure..." -ForegroundColor Yellow
if ($SlotName -eq "production") {
    az webapp deploy --resource-group $ResourceGroupName --name $WebAppName --src-path "hopewell-deployment.zip" --type zip
} else {
    az webapp deploy --resource-group $ResourceGroupName --name $WebAppName --slot $SlotName --src-path "hopewell-deployment.zip" --type zip
}

if ($LASTEXITCODE -ne 0) {
    Write-Error "Deployment failed!"
    exit 1
}

# Step 4: Verify deployment
Write-Host "Verifying deployment..." -ForegroundColor Yellow
$webAppUrl = "https://$WebAppName.azurewebsites.net"
if ($SlotName -ne "production") {
    $webAppUrl = "https://$WebAppName-$SlotName.azurewebsites.net"
}

Write-Host "Testing health endpoint..." -ForegroundColor Yellow
$healthResponse = Invoke-WebRequest -Uri "$webAppUrl/health" -Method GET -UseBasicParsing
if ($healthResponse.StatusCode -eq 200) {
    Write-Host "✅ Health check passed!" -ForegroundColor Green
} else {
    Write-Warning "⚠️ Health check failed with status: $($healthResponse.StatusCode)"
}

Write-Host "Deployment completed successfully!" -ForegroundColor Green
Write-Host "Application URL: $webAppUrl" -ForegroundColor Cyan

# Cleanup
Remove-Item "hopewell-deployment.zip" -Force
Remove-Item "publish-azure" -Recurse -Force
```

## Usage Example

```bash
# Run the deployment script
.\deploy-to-azure.ps1 -ResourceGroupName "AZ-JHB-RSG-RCNA-ST10466568-TER" -WebAppName "HopewellAPI"

# Deploy to staging slot
.\deploy-to-azure.ps1 -ResourceGroupName "AZ-JHB-RSG-RCNA-ST10466568-TER" -WebAppName "HopewellAPI" -SlotName "staging"
```

## Post-Deployment Checklist

- [ ] Verify application is running
- [ ] Test all critical endpoints
- [ ] Check application logs for errors
- [ ] Verify database connectivity
- [ ] Test authentication flows
- [ ] Monitor performance metrics
- [ ] Set up alerts and monitoring
- [ ] Document any configuration changes

## Support and Resources

- [Azure App Service Documentation](https://docs.microsoft.com/en-us/azure/app-service/)
- [Azure CLI Reference](https://docs.microsoft.com/en-us/cli/azure/)
- [.NET Core Deployment Guide](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/azure-apps/)

---

**Note**: Replace placeholder values like "Your-Resource-Group-Name", "Your-Web-App-Name", etc., with your actual Azure resource names.

**Last Updated**: January 2025
**Version**: 1.0












