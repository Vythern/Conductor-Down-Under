using TMPro;
using UnityEngine;

public class MainTrain : MonoBehaviour
{
    KeyCode moveNorth = KeyCode.W;
    KeyCode moveEast = KeyCode.D;
    KeyCode moveSouth = KeyCode.S;
    KeyCode moveWest = KeyCode.A;
    KeyCode startOrStopTrain = KeyCode.Space;
    KeyCode changePerspective = KeyCode.N; //swap between overhead and follow cameras.  


    [SerializeField] private GameObject TrainObject; //This references the object in game that these features are tied to.  An object / visual aid is not necessary for the track to work, technically.  
    [SerializeField] private BoxCollider trainCollider; //For dealing with the train entering and exiting the track.  
    [SerializeField] private Rigidbody trainRigidbody; //For train velocity tracking


    [SerializeField] private TMP_Text velocityIndicator; //Tell the player what direction they are queued to go towards.  
    [SerializeField] private float maximumSpeed; //Tell the player what direction they are queued to go towards.  

    [SerializeField] private UnityEngine.UI.Image[] directionIndicators; //Tell the player what direction they are queued to go towards.  
    [SerializeField] private GameObject UI; //Control and track ui elements.  
    [SerializeField] private Camera followCamera; //Allow player to toggle between follow or overhead view.  
    private Camera overheadCamera; //Allow player to toggle between follow or overhead view.  


    private bool[] inputDirection = new bool[4] { false, false, false, false };
    private bool[] validDirections = new bool[4] { false, false, false, false };
    private bool engineRunning = false; //track whether the player wants to stop or go.  

    private GameObject currentTrack; //Lock train to the track grid and determine relevant angles
    private GameObject lastTrack; //If user makes a mistake, necessary to resume gameplay functionality.  

    private void OnTriggerEnter(Collider collisionObject) //Whenever the train enters the collision box of a track, this will run.  
    {
        currentTrack = collisionObject.gameObject; //it allows us to figure out what track to send info to.  
    }

    private void OnTriggerExit(Collider collisionObject) //upon exiting a trigger box, decide where the train should turn / whether it should collide and stop
    {
        //When exiting a track collider, we measure the most recent directional key the player pressed.  
        //If the direction they want to go is a valid one, then turn / move the train towards it.  
        //if not, then stop the train.  
        //print("Exiting track:  " + collisionObject.gameObject.name);
        //print("Input when exiting:  North:  " + inputDirection[0] + ", East:  " + inputDirection[1] + ", South:  " + inputDirection[2] + ", West:  " + inputDirection[3]);
        Vector3 currentDirection = TrainObject.transform.forward;

        if (inputDirection[0] == true)
        {
            if (validDirections[0] && Vector3.Angle(currentDirection, Vector3.forward) != 180f)
            {
                //rotate to north (0 degrees)
                if (TrainObject.transform.rotation != Quaternion.Euler(0, 0, 0))
                {
                    TrainObject.transform.rotation = Quaternion.Euler(0, 0, 0);
                    this.transform.position = currentTrack.transform.position + new Vector3(0f, 1f, 0f);
                    //Update rigidbody velocity based on rotation
                    trainRigidbody.linearVelocity = transform.forward * trainRigidbody.linearVelocity.magnitude;
                }
            }
            else 
            {
                engineRunning = false;
                trainRigidbody.linearVelocity = Vector3.zero;
            }
        }
        if (inputDirection[1] == true)
        {
            if (validDirections[1] && Vector3.Angle(currentDirection, Vector3.right) != 180f)
            {
                //rotate to east (90 degrees)
                if (TrainObject.transform.rotation != Quaternion.Euler(0, 90, 0))
                {
                    TrainObject.transform.rotation = Quaternion.Euler(0, 90, 0);
                    this.transform.position = currentTrack.transform.position + new Vector3(0f, 1f, 0f);
                    //Update rigidbody velocity based on rotation
                    trainRigidbody.linearVelocity = transform.forward * trainRigidbody.linearVelocity.magnitude;
                }
            }
            else
            {
                engineRunning = false;
                trainRigidbody.linearVelocity = Vector3.zero;
            }
        }
        if (inputDirection[2] == true)
        {
            if (validDirections[2] && Vector3.Angle(currentDirection, Vector3.back) != 180f)
            {
                //rotate to south (180 degrees)
                if (TrainObject.transform.rotation != Quaternion.Euler(0, 180, 0))
                {
                    TrainObject.transform.rotation = Quaternion.Euler(0, 180, 0);
                    this.transform.position = currentTrack.transform.position + new Vector3(0f, 1f, 0f);
                    //Update rigidbody velocity based on rotation
                    trainRigidbody.linearVelocity = transform.forward * trainRigidbody.linearVelocity.magnitude;
                }
            }
            else
            {
                engineRunning = false;
                trainRigidbody.linearVelocity = Vector3.zero;
            }
        }
        if (inputDirection[3] == true)
        {
            if (validDirections[3] && Vector3.Angle(currentDirection, Vector3.left) != 180f)
            {
                //rotate to west (270 degrees)
                if (TrainObject.transform.rotation != Quaternion.Euler(0, 270, 0))
                {
                    TrainObject.transform.rotation = Quaternion.Euler(0, 270, 0);
                    this.transform.position = currentTrack.transform.position + new Vector3(0f, 1f, 0f);
                    //Update rigidbody velocity based on rotation
                    trainRigidbody.linearVelocity = transform.forward * trainRigidbody.linearVelocity.magnitude;
                }
            }
            else
            {
                engineRunning = false;
                trainRigidbody.linearVelocity = Vector3.zero;
            }
        }
        lastTrack = currentTrack; //if the user encounters a wall, we will need to re-evaluate the track information to determine whether they can move again.  
    }

    private bool IsCurrentDirectionOpen() //Used to stop the train from driving over barriers and open air
    {
        Vector3 currentDirection = TrainObject.transform.forward;

        if (currentDirection == Vector3.forward && validDirections[0]) { return true; }
        if (currentDirection == Vector3.right && validDirections[1]) { return true; }
        if (currentDirection == Vector3.back && validDirections[2]) { return true; }
        if (currentDirection == Vector3.left && validDirections[3]) { return true; }
        return false;
    }

    public void receiveDirections(bool[] directions) 
    {
        //the train receives a list of valid cardinal directions to continue traveling upon reaching each track.  
        validDirections[0] = directions[0];
        validDirections[1] = directions[1];
        validDirections[2] = directions[2];
        validDirections[3] = directions[3];

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UI.gameObject.SetActive(true);
        followCamera.gameObject.SetActive(false);
        overheadCamera = Camera.main;

        Collider[] hitColliders = Physics.OverlapBox(trainRigidbody.transform.position, new Vector3(1, 1, 1));

        foreach (var hitCollider in hitColliders)
        {
            if(hitCollider.tag == "Track")
            {
                validDirections = hitCollider.GetComponent<Track>().getValidDirections(); //get valid directions on wake.  
            }
        }

        inputDirection = new bool[] { false, false, false, false };
        for (int i = 0; i < 4; i++) //set the default inputDirection to the first available path way when the game starts.  
        {
            if (validDirections[i])
            {
                inputDirection[i] = true;
                updateDirectionalHud();
            }
        }
    }

    // Update is called once per frame
    void Update() //Each frame, check if the player has pressed a key.  A hud element to indicate what direction the train is traveling might be a nice idea.  
    {
        if (Input.GetKeyDown(moveNorth))
        {
            inputDirection[0] = true;
            inputDirection[1] = false;
            inputDirection[2] = false;
            inputDirection[3] = false;
        }
        if (Input.GetKeyDown(moveEast))
        {
            inputDirection[0] = false;
            inputDirection[1] = true;
            inputDirection[2] = false;
            inputDirection[3] = false;
        }
        if (Input.GetKeyDown(moveSouth))
        {
            inputDirection[0] = false;
            inputDirection[1] = false;
            inputDirection[2] = true;
            inputDirection[3] = false;
        }
        if (Input.GetKeyDown(moveWest))
        {
            inputDirection[0] = false;
            inputDirection[1] = false;
            inputDirection[2] = false;
            inputDirection[3] = true;
        }
        if (Input.GetKeyDown(startOrStopTrain))
        {
            if (!IsCurrentDirectionOpen()) //Order probably matters here, keep this below the other engineRunning checks / toggles.  
            {
                engineRunning = false; //this is being used as a flag.  
                for (int i = 0; i < 4; i++)
                {
                    if (inputDirection[i])
                    {
                        if (inputDirection[i] && validDirections[i])
                        {
                            engineRunning = true;
                            //make train face the proper direction.  
                            TrainObject.transform.rotation = Quaternion.Euler(0, i * 90, 0);
                            TrainObject.transform.position = lastTrack.gameObject.transform.position;

                        }
                    }
                }
                //Halt train if the player tries to start it when not facing a valid pathway.  
                
                if(!engineRunning) //if the player didn't choose a valid direction, then this will evaluate to true.  
                {
                    engineRunning = false;
                    trainRigidbody.linearVelocity = Vector3.zero;
                }
            }
            else
            {
                if(!engineRunning) { engineRunning = true; }
                else { engineRunning = false; }

            }
            //if the train is accelerating / the engine is on, then turn off engine and begin braking quickly.  
            //if the train is not accelerating / the engine is off, then begin gaining speed.  
        }
        if (Input.GetKeyDown(changePerspective))
        {
            ToggleCameraPerspective();
        }
        updateDirectionalHud(); //update the user's heads up display on what direction they are travelling.  
        followCamera.transform.rotation = Quaternion.Euler(90, 0, -transform.rotation.eulerAngles.y);
        switch(TrainObject.transform.rotation.eulerAngles.y)
        {
            case 0:
                followCamera.transform.Rotate(new Vector3(0f, 0f, 1f), 0f);
                break;
            case 90:
                followCamera.transform.Rotate(new Vector3(0f, 0f, 1f), 90f);
                break;
            case 180:
                followCamera.transform.Rotate(new Vector3(0f, 0f, 1f), 180f);
                break;
            case 270:
                followCamera.transform.Rotate(new Vector3(0f, 0f, 1f), 270);
                break;
        }
    }

    private void ToggleCameraPerspective()
    {
        if (overheadCamera != null && followCamera != null)
        {
            bool isOverheadActive = overheadCamera.gameObject.activeSelf;

            overheadCamera.gameObject.SetActive(!isOverheadActive);
            followCamera.gameObject.SetActive(isOverheadActive);
            if (isOverheadActive)
            {
                followCamera.tag = "MainCamera";
                overheadCamera.tag = "Untagged";
            }
            else
            {
                overheadCamera.tag = "MainCamera";
                followCamera.tag = "Untagged";
            }
        }
    }

    private void updateDirectionalHud() //this is probably going to go into a separate script.  For now though, this hack is fine for testing.  
    {
        for (int i = 0; i < 4; i++)
        {
            if (inputDirection[i])
            {
                directionIndicators[i].gameObject.SetActive(true);
            }
            else
            {
                directionIndicators[i].gameObject.SetActive(false);
            }
        }
        velocityIndicator.text = ("Speed: " + (3.6f * trainRigidbody.linearVelocity.magnitude).ToString("F2") + " km/h");
        //Unity units are 1 meter, rigidbody velocity magnitude refers to units per second, and 1 meter per second is 3.6 km/h.  
    }

    private void FixedUpdate()
    {
        if (engineRunning) 
        {
            if (trainRigidbody.linearVelocity.magnitude < maximumSpeed)
            {
                trainRigidbody.AddForce(0.25f * transform.forward);
            }
        }
        else
        {
            if(trainRigidbody.linearVelocity.magnitude > 0.1f)
            {
                trainRigidbody.AddForce(-1.5f * transform.forward);
                //if the train is stopped and then exits or enters a new track, then the valid direction may be updated and allow the user to go off the rails.  
            }
            else
            {
                trainRigidbody.linearVelocity = Vector3.zero;
            }
        }
    }
}
