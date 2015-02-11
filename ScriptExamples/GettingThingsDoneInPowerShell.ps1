$url = "https://api.meetup.com/2/rsvps?&sign=true&rsvp=yes&event_id=qdxxblytdbpb&page=1000&key={your meetup key}"
$webclient = new-object system.net.webclient;
$webclient.DownloadString($url) 

$json = $webclient.DownloadString($url)  | ConvertFrom-Json 
 
$json.meta.total_count

$totals = "Total Count: " + $json.meta.total_count

foreach ($result in $json | Get-Member) {
	$memberName = $json.$($result.Name).member.name

	$memberName
 }