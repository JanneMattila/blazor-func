Param (
    [Parameter(HelpMessage="Deployment target resource group")] 
    [string] $ResourceGroupName = "rg-blazorfunc-local",

    [Parameter(HelpMessage="Deployment target storage account name")] 
    [string] $WebStorageName,

    [Parameter(HelpMessage="Blazor publish app root folder path e.g. ..\src\Blazor\bin\Release\netstandard2.1\publish\Blazor\dist\")] 
    [string] $AppRootFolder
)

$ErrorActionPreference = "Stop"

function GetContentType([string] $extension)
{
    if ($extension -eq ".html") 
    {
        return "text/html"
    }
    elseif ($extension -eq ".wasm") 
    {
        return "application/wasm"
    }
    elseif ($extension -eq ".dll") 
    {
        return "application/x-msdownload"
    }
    elseif ($extension -eq ".svg") 
    {
        return "image/svg+xml"
    }
    elseif ($extension -eq ".css") 
    {
        return "text/css"
    }
    elseif ($extension -eq ".js") 
    {
        return "text/javascript"
    }
    elseif ($extension -eq ".json") 
    {
        return "application/json"
    }
    return "text/plain"
}

if ($AppRootFolder.EndsWith("\") -eq $false)
{
    $AppRootFolder += "\"
}

$storageAccount = Get-AzStorageAccount -ResourceGroupName $ResourceGroupName -AccountName $WebStorageName
Get-ChildItem -File -Recurse $AppRootFolder `
    | ForEach-Object  { 
        $name = $_.FullName.Replace($AppRootFolder,"")
        $contentType = GetContentType($_.Extension)
        $properties = @{"ContentType" = $contentType}
        Set-AzStorageBlobContent -File $_.FullName -Blob $name -Container `$web -Context $storageAccount.Context -Properties $properties -Force
    }

$webStorageUri = $storageAccount.PrimaryEndpoints.Web
Write-Host "Static website endpoint: $webStorageUri"
