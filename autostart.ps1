$taskName = "G14Helper"
$task = Get-ScheduledTask -TaskName $taskName -ErrorAction SilentlyContinue
if ($null -ne $task)
{
    Unregister-ScheduledTask -TaskName $taskName -Confirm:$false 
}

$scriptDir = Split-Path $PSCommandPath -Parent

# TODO: EDIT THIS STUFF AS NEEDED...
$action = New-ScheduledTaskAction -Execute "$scriptDir\g14-helper.exe" 
$trigger = New-ScheduledTaskTrigger -AtLogon
$settings = New-ScheduledTaskSettingsSet -AllowStartIfOnBatteries -DontStopIfGoingOnBatteries

$principal = New-ScheduledTaskPrincipal -UserId $env:USERNAME -RunLevel Highest
$definition = New-ScheduledTask -Action $action -Principal $principal -Trigger $trigger -Settings $settings -Description "Run $($taskName) at Logon"

Register-ScheduledTask -TaskName $taskName -InputObject $definition