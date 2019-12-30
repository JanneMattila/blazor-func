Param (
    [Parameter(HelpMessage="Deployment target resource group")] 
    [string] $ResourceGroupName = "rg-blazorfunc-local",

    [Parameter(HelpMessage="Deployment target resource group location")] 
    [string] $Location = "North Europe",

    [Parameter(HelpMessage="SignalR Pricing tier. Check details at https://azure.microsoft.com/en-us/pricing/details/signalr-service/")] 
    [ValidateSet("Free_F1", "Standard_S1")]
    [string] $SignalRServicePricingTier = "Free_F1",

    [Parameter(HelpMessage="SignalR Service unit count")] 
    [ValidateSet(1, 2, 5, 10, 20, 50, 100)]
    [int] $SignalRServiceUnits = 1,

    [string] $Template = "$PSScriptRoot\azuredeploy.json",
    [string] $TemplateParameters = "$PSScriptRoot\azuredeploy.parameters.json"
)

$ErrorActionPreference = "Stop"

$date = (Get-Date).ToString("yyyy-MM-dd-HH-mm-ss")
$deploymentName = "Local-$date"

if ([string]::IsNullOrEmpty($env:RELEASE_DEFINITIONNAME))
{
    Write-Host (@"
Not executing inside Azure DevOps Release Management.
Make sure you have done "Login-AzAccount" and
"Select-AzSubscription -SubscriptionName name"
so that script continues to work correctly for you.
"@)
}
else
{
    $deploymentName = $env:RELEASE_RELEASENAME
}

if ((Get-AzResourceGroup -Name $ResourceGroupName -Location $Location -ErrorAction SilentlyContinue) -eq $null)
{
    Write-Warning "Resource group '$ResourceGroupName' doesn't exist and it will be created."
    New-AzResourceGroup -Name $ResourceGroupName -Location $Location -Verbose
}

# Additional parameters that we pass to the template deployment
$additionalParameters = New-Object -TypeName hashtable
$additionalParameters['signalRServicePricingTier'] = $SignalRServicePricingTier
$additionalParameters['signalRServiceUnits'] = $SignalRServiceUnits

$result = New-AzResourceGroupDeployment `
    -DeploymentName $deploymentName `
    -ResourceGroupName $ResourceGroupName `
    -TemplateFile $Template `
    -TemplateParameterFile $TemplateParameters `
    @additionalParameters `
    -Mode Complete -Force `
    -Verbose

if ($result.Outputs.webStorageName -eq $null -or
    $result.Outputs.webAppName -eq $null -or
    $result.Outputs.webAppUri -eq $null)
{
    Throw "Template deployment didn't return web app information correctly and therefore deployment is cancelled."
}

$result

$webStorageName = $result.Outputs.webStorageName.value
$webAppName = $result.Outputs.webAppName.value
$webAppUri = $result.Outputs.webAppUri.value

$storageAccount = Get-AzStorageAccount -ResourceGroupName $ResourceGroupName -AccountName $webStorageName
Enable-AzStorageStaticWebsite -Context $storageAccount.Context -IndexDocument index.html -ErrorDocument404Path 404.html
$webStorageUri = $storageAccount.PrimaryEndpoints.Web
Write-Host "Static website endpoint: $webStorageUri"

# Publish variable to the Azure DevOps agents so that they
# can be used in follow-up tasks such as application deployment
Write-Host "##vso[task.setvariable variable=Custom.WebStorageName;]$webStorageName"
Write-Host "##vso[task.setvariable variable=Custom.WebStorageUri;]$webStorageUri"
Write-Host "##vso[task.setvariable variable=Custom.WebAppName;]$webAppName"
Write-Host "##vso[task.setvariable variable=Custom.WebAppUri;]$webAppUri"
