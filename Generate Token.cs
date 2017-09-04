write-host "Generate OAuth 2.0 token from Tesla REST API"
write-host "Version 2.0"
write-host ""

$creds = $Host.ui.PromptForCredential("Need credentials", "Please enter your user name and password.", "", "")

if (($creds.UserName.Length -lt 1) -or ($creds.Password.Length -lt 1)) {
    write-host "Username or password not entered!" -ForegroundColor Red
    
} else {
    $username = $creds.UserName
    $password = $creds.GetNetworkCredential().password  # this is how we pull the unencrypted password out of the credentials object
    try {
        $response = Invoke-RestMethod -Method Post -Uri "https://owner-api.teslamotors.com/oauth/token?grant_type=password&client_id=e4a9949fcfa04068f59abb5a658f2bac0a3428e4652315490b659d5ab3f35a9e&client_secret=c75f14bbadc8bee3a7594412c31416f8300256d7668ea7e6e7f06727bfb9d220&email=$username&password=$password"
    }
    catch {
        if (($_.Exception.Message).contains("401")) {
            write-host "Invalid username/password!" -ForegroundColor Red
        } else {
            write-host "Error while interrogating the Tesla API.  Please try again." -ForegroundColor Red
        }
        return
    }
    write-host "Your access token is:"
    Write-Host ""
    write-host $response.access_token -ForegroundColor Yellow
    write-host ""
    write-host "Copy this to the clipboard?"
    $copyclip = read-host -prompt "y/n"
    if ($copyclip.ToUpper() -eq "Y") {
        [Windows.Forms.Clipboard]::SetText($response.access_token)
    }

    write-host ""
}