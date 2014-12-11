//done from tutorial http://www.catlikecoding.com/unity/tutorials/clock/
//all credit goes there. I didn't stray far from it. ... Maybe.

using UnityEngine;
using System;

public class ClockAnimator : MonoBehaviour {


	//to determine the amount each hand rotates at a given...time. hehe
	//aka to determine how much to rotate for each concrete movement when the script updates
	//hour rotates 1/12th of the circle for each change, minute rotates 1/60th, etc
	//this means the clock is not smooth if not set to "analog", but makes discrete steps. Like a state machine.
	private const float
		hoursToDegrees = 360f / 12f,
		minutesToDegrees = 360f/60f,
		secondsToDegrees = 360f/60f,
		oneDegree = 1.0f;
	private float
		secondMultiplier = 1.0f;

	//transform objects for the hands of the clock
	public Transform hours, minutes, seconds;
	//dummy transform objects...may not work?
	Quaternion hoursGoal, minutesGoal, secondsGoal;

	//if we want a smooth, traditional analog clock, we have this set to "true".
	//else, we get digital discrete steps
	public bool analog;
	bool start;

	bool passhr = false, passmn = false, passsc = false;

	//starting value for the hands when you use the start-up "sweep to current time" animation
	//not used if you just let the clock snap to the time immediately
	float hourInc = 0.00f, minuteInc = 0.00f, secondInc = 0.00f;




	private void updateGoal(){
		if (analog) { //support smooth rotations
			TimeSpan timeSpan = DateTime.Now.TimeOfDay;
			hoursGoal = Quaternion.Euler (0f, 0f, (float)timeSpan.TotalHours * -hoursToDegrees);
			minutesGoal = Quaternion.Euler (0f, 0f, (float)timeSpan.TotalMinutes * -minutesToDegrees);
			secondsGoal = Quaternion.Euler (0f, 0f, (float)timeSpan.TotalSeconds * -secondsToDegrees);
			
		} else { //support rough rotations
			//get the time once per update, since one refresh equals one state
			//does not allow us access to fractional time periods
			DateTime time = DateTime.Now;
			
			//calculate each rotation and sort of perform it
			//note view of camera -- down Z axis -- and the need for negative rotation around Z to be clockwise here
			//use a quaternion on each hand to define the rotation, with the Euler function
			hoursGoal = Quaternion.Euler (0f, 0f, time.Hour * -hoursToDegrees);
			minutesGoal = Quaternion.Euler (0f, 0f, time.Minute * -minutesToDegrees);
			secondsGoal = Quaternion.Euler (0f, 0f, time.Second * -secondsToDegrees);
		}
	}


	// Use this for initialization
	void Start () {
		start = true;
		updateGoal ();

		//figure out the best way to rotate the second hand
		if (secondsGoal.eulerAngles.z < 90 && secondsGoal.eulerAngles.z > 35) {
						secondMultiplier = -2 * secondMultiplier;
				} 
		else if (secondsGoal.eulerAngles.z < 180) {
						secondMultiplier = 6 * secondMultiplier;
				}
	}

	//to animate the clock until it reaches the current, correct time
	private void illustrateHelper(){
		updateGoal ();
		if (analog)
		{
			if (hours.eulerAngles.z > hoursGoal.eulerAngles.z)
			{
				hourInc += oneDegree;
				hours.localRotation = Quaternion.Euler(0f, 0f, -hourInc);
			}
			else {passhr = true;}

		
			if (minutes.eulerAngles.z > minutesGoal.eulerAngles.z)
			{
				minuteInc += oneDegree;
				minutes.localRotation = Quaternion.Euler(0f, 0f, -minuteInc);
			}
			else {passmn = true;}


			if (!passsc && seconds.eulerAngles.z > secondsGoal.eulerAngles.z/* && secondMultiplier > 1*/)
			{
				secondInc += (float)(oneDegree * secondMultiplier);
				seconds.localRotation = Quaternion.Euler(0f, 0f, -secondInc);
				if (seconds.eulerAngles.z <= (secondsGoal.eulerAngles.z + 3.0f) 
					 && seconds.eulerAngles.z >= (secondsGoal.eulerAngles.z - 3.0f)) 
				{
					passsc = true;
				}
			}
			else if (!passsc && seconds.eulerAngles.z <= secondsGoal.eulerAngles.z/* && secondMultiplier > 1*/)
			{
				secondInc += (float)(oneDegree * secondMultiplier);
				seconds.localRotation = Quaternion.Euler(0f, 0f, -secondInc);
				if (seconds.eulerAngles.z <= (secondsGoal.eulerAngles.z + 3.0f) 
					 && seconds.eulerAngles.z >= (secondsGoal.eulerAngles.z - 3.0f)) 
				{
					passsc = true;
				}
			}
			else if (passsc)
			{
				seconds.localRotation = Quaternion.Euler(0f, 0f, secondsGoal.eulerAngles.z);
			}
			/*else if 
				(seconds.eulerAngles.z <= (secondsGoal.eulerAngles.z + 3.0f) 
				|| seconds.eulerAngles.z >= (secondsGoal.eulerAngles.z + 3.0f)) 
				{passsc = true;}*/

			if (passhr && passmn && passsc) {start = false;}
			else {start = true;}
		} 
		else {
			//do nothing
			}

	
	}
	
	// Update is called once per frame
	void Update () {
		if (start) {
				illustrateHelper ();
		} 
		else {
				if (analog) { //support smooth rotations
						TimeSpan timeSpan = DateTime.Now.TimeOfDay;
						hours.localRotation = Quaternion.Euler (0f, 0f, (float)timeSpan.TotalHours * -hoursToDegrees);
						minutes.localRotation = Quaternion.Euler (0f, 0f, (float)timeSpan.TotalMinutes * -minutesToDegrees);
						seconds.localRotation = Quaternion.Euler (0f, 0f, (float)timeSpan.TotalSeconds * -secondsToDegrees);

				} else { //support rough rotations
						//get the time once per update, since one refresh equals one state
						//does not allow us access to fractional time periods
						DateTime time = DateTime.Now;

						//calculate each rotation and sort of perform it
						//note view of camera -- down Z axis -- and the need for negative rotation around Z to be clockwise here
						//use a quaternion on each hand to define the rotation, with the Euler function
						hours.localRotation = Quaternion.Euler (0f, 0f, time.Hour * -hoursToDegrees);
						minutes.localRotation = Quaternion.Euler (0f, 0f, time.Minute * -minutesToDegrees);
						seconds.localRotation = Quaternion.Euler (0f, 0f, time.Second * -secondsToDegrees);
				}
		}
		//Android exit functionality?
		if (Application.platform == RuntimePlatform.Android) {
			
			if (Input.GetKeyUp(KeyCode.Escape)) {
				
				//quit application on return button
				
				Application.Quit();
				
				return;
				
			}
			
		}
		}
}
