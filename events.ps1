Get-EventSubscriber | Unregister-Event


$ProcessAsusWmiEvent = {
    $event_id = $Event.SourceEventArgs.NewEvent.EventID
    Write-Host $event_id
}

Register-CimIndicationEvent -Namespace root\wmi  -query  "Select * From AsusAtkWmiEvent" ` -sourceIdentifier "GHAsus" ` -action $ProcessAsusWmiEvent

while (1) {
    
}
