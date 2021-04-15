Param
(
   [Parameter(Mandatory = $true)][string]$secretPath,
   [Parameter(Mandatory = $true)][string]$secretPrefix,
   [Parameter(Mandatory = $true)][string]$keyvaultName,
   [Parameter(Mandatory = $true)][bool]$displayOnly
 )

#Responsible for generating Disruption Json
Function Generate-Secrets
(
    [string]$secretPath,
    [string]$secretPrefix,
    [string]$keyvaultName,
    [bool]$displayOnly
)
{
    echo "Inside Generate Json. $secretPath"

    $secretsData = Get-Content $secretPath | Out-String | ConvertFrom-Json -AsHashtable

    foreach ($key in $secretsData.Keys) 
    {
        $replacedKey = $key.Replace(":","--")
        
        $keyToSet = "$secretPrefix-$replacedKey"
        
        echo $keyToSet.ToString()  

        echo $secretsData[$key].ToString()

        if ($displayOnly -eq $false)
        {
            echo "Setting the value for $keyToSet"

            az keyvault secret set --name $keyToSet.ToString() --vault-name $keyvaultName --value $secretsData[$key].ToString()
        }

    }
}

Generate-Secrets -secretPath $secretPath -secretPrefix $secretPrefix -keyvaultName $keyvaultName -displayOnly $displayOnly