using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Launcher : MonoBehaviour {

    public float maxLaunchSpeed = 100.0f;
    public AudioClip windUpSound, launchSound;
    public PhysicsEngine ballToLaunch;
    
    private float launchSpeed;
    private float extraSpeedPerFrame;
    private AudioSource audioSource;
    
    
	// Use this for initialization
	void Start () {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = windUpSound; // so we know the length of the clip
        
        // calculating the speed that we will need to have per frame, which is based on the audio source length
        extraSpeedPerFrame = (maxLaunchSpeed * Time.fixedDeltaTime) / audioSource.clip.length; 
        
	}
	
    void OnMouseDown()
    {
        // increse ball speed to max over a few seconds.
        // consider InvokeRepeating
        // as you hold down the laucher, the speed should increase
        launchSpeed = 0.0f;
        InvokeRepeating("IncreaseLaunchSpeed", 0.5f, Time.fixedDeltaTime);
        audioSource.clip = windUpSound; // want to be sure that at each launch we start with windupsound
        audioSource.Play();
        
    }
    
    void OnMouseUp()
    {
        // they when you stop holding mouse, we then launch the ball using the speed generated
        CancelInvoke();
        audioSource.Stop();
        audioSource.clip = launchSound;
        audioSource.Play();

        LaunchBall();
    }
    
    void IncreaseLaunchSpeed(){
        if (launchSpeed <= maxLaunchSpeed){
            launchSpeed += extraSpeedPerFrame;
        }
        
    }
    
    void LaunchBall(){

        PhysicsEngine newBall = Instantiate(ballToLaunch, transform.position, Quaternion.identity) as PhysicsEngine;
        newBall.name = "Football";
        newBall.transform.parent = GameObject.Find("LaunchedBalls").transform;
        Vector3 launchVelocity = new Vector3(1.0f, 1.0f, 0.0f).normalized * launchSpeed;
        newBall.velocityVector = launchVelocity;
        
    }
    
    
}
