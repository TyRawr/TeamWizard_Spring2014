using UnityEngine;
 
public class Capture_Frames: MonoBehaviour
{
	public bool recordOnPlay = false;
	// The folder we place all screenshots inside.
	// If the folder exists we will append numbers to create an empty folder.
	public string takeName = "Scene01_Shot01_Take01";
	public int frameRate = 24;
	
	/* NOT CURRENTLY IMPLEMENTED - allows user to specify a start timecode (based on gametime from start) and a duration of seconds for the shot
	public bool automateRecording = false;
	public float automaticStartTime = 0;
	public float shotDuration = 0;
	*/
 
	private string realFolder = "";
 
	void Start()
	{
		if ( recordOnPlay )
		{
			// Set the playback framerate!
			// (real time doesn't influence time anymore)
			Time.captureFramerate = frameRate;
	 
			// Find a folder that doesn't exist yet by appending numbers!
			realFolder = takeName;
			int count = 1;
			
			while (System.IO.Directory.Exists(realFolder))
			{
				realFolder = takeName + count;
				count++;
			}
			
			// Create the folder
			System.IO.Directory.CreateDirectory(realFolder);
		}
	}
 
	void Update()
	{
		if ( recordOnPlay )
		{
				
			// name is "realFolder/0005 shot.png"
			var name = string.Format("{0}/{1:D04} shot.png", realFolder, Time.frameCount);
	 
			// Capture the screenshot
			Application.CaptureScreenshot(name);
		}
	}
}