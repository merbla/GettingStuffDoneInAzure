cls

$url = "https://api.meetup.com/2/rsvps?&sign=true&rsvp=yes&event_id=qdxxblytdbpb&page=1000&key=69495a1b47a442d6412235cd17063"
$webclient = new-object system.net.webclient;
$webclient.DownloadString($url) 

$json = $webclient.DownloadString($url)  | ConvertFrom-Json 

Write-Host "Total Count: " $json.meta.total_count

foreach ($result in $json | Get-Member) {
	echo $json.$($result.Name).member.name
}