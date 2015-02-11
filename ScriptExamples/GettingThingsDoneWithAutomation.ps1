<# 
	This PowerShell script was automatically converted to PowerShell Workflow so it can be run as a runbook.
	Specific changes that have been made are marked with a comment starting with “Converter:”
#>

workflow GettingThingsDoneInPowerShell {
	
	# Converter: Wrapping initial script in an InlineScript activity, and passing any parameters for use within the InlineScript
	# Converter: If you want this InlineScript to execute on another host rather than the Automation worker, simply add some combination of -PSComputerName, -PSCredential, -PSConnectionURI, or other workflow common parameters (http://technet.microsoft.com/en-us/library/jj129719.aspx) as parameters of the InlineScript
	inlineScript {
				 
		$url = "https://api.meetup.com/2/rsvps?&sign=true&rsvp=yes&event_id=qdxxblytdbpb&page=1000&key={your meetup key}"
		$webclient = new-object system.net.webclient;
		$webclient.DownloadString($url) 

		$json = $webclient.DownloadString($url)  | ConvertFrom-Json 
		  
		$totals = "Total Count: " + $json.meta.total_count

		foreach ($result in $json | Get-Member) {
			$memberName = $json.$($result.Name).member.name
		 	
		 	$memberName
		 
		}

	}
}