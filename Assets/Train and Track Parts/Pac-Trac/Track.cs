using UnityEngine;

public class Track : MonoBehaviour
{
    [SerializeField] private GameObject TrackObject; //This references the object in game that these features are tied to.  An object / visual aid is not necessary for the track to work, technically.  
    [SerializeField] private BoxCollider trackCollider; //For dealing with the train entering and exiting the track.  

    [SerializeField, Range(1, 4)] private int trackType; //This determines what valid actions are available to the player when their train enters the trigger zone.  

    //Track type also determines how many connections points the track has.  

    //Type 1 is a straight track, and only forward and backward are valid places to go.  

    //Type 2 is a left turn track.
    //If the player presses left (orientation corresponding to train and track), they continue along at full speed, accelerating through the curve.  
    //If the player fails to press the directional key, then the track is treated as a dead end.  

    //Type 3 is a right turn track.  
    //The same rules apply to this track as type 2 tracks.  

    //Type 4 is a dead end.  If the player has speed when they reach this point, they will have a collision and be bounced back a bit, losing all their speed.  
    //These are not good for the player to run into, as it forces them to reverse- the only way out is slow as a result.  

    //There could be types 5, 6, 7, 8, potentially, eg forward or left, forward, left, and right, or even 90 left, 45 left, forward, 45 right, etc.  
    //Not within the current scope of features.  

    //Left + forward, Right + Forward, Left + Right, and Left + Right + Forward would be good things to add to the to-do-list.  

    private int connectedTracks; //Used to iterate through and determine valid paths.  
    private bool[] validDirections = new bool[] {false, false, false, false }; //Corresponding to North, East, South, West
    private bool[] currentInput = new bool[] { false, false, false, false };

    private float orientation; //Orientation further determines the valid actions based on the Y rotation of the tracks.  
                               //Eg, when a right turn track is facing north, the player must press "Right" in order to turn right.  
                               //If this track itself is then rotated 90 degrees to the right, then the player would need to be pressing the down key instead.  
                               //This corresponds to the visuals of the tracks in game and is set based on the transforms of the tracks.  

    //Orientation could also be used in combination with the track type to additively determine the validity of a direction.  
    //Eg a forward facing track has an angle of 0, and when facing forwards, pressing forwards has the train moving at the same angle.  
    //Combining the angles together produces a 0, so it is a valid direction.  
    //For now, I don't feel like implementing that, so I'm using a big list of cases to determine valid paths.  
    //There is definitely a cleaner way to implement this but this is dead simple and easy so whatever.  Just a demo at the moment.  


    private void setValidDirections() //uses the track orientation to determine what directions are valid for the player to travel to, based on this track object's connections list.  
    {
        switch (trackType)
        {
            case 1: //Straight Track- Train can move forwards or backwards.  
                connectedTracks = 2;
                switch (orientation)
                {
                    case 0:
                        validDirections = new bool[] { true, false, true, false};
                        break;
                    case 90:
                        validDirections = new bool[] { false, true, false, true};
                        break;
                    case 180:
                        validDirections = new bool[] { true, false, true, false};
                        break;
                    case 270:
                        validDirections = new bool[] { false, true, false, true};
                        break;
                    case -90:
                        validDirections = new bool[] { false, true, false, true};
                        break;
                    case -180:
                        validDirections = new bool[] { true, false, true, false};
                        break;
                    case -270:
                        validDirections = new bool[] { false, true, false, true};
                        break;
                }
                break;
            case 2: //Left Track- Train can move left or backwards.  
                connectedTracks = 2;
                switch (orientation)
                {
                    case 0:
                        validDirections = new bool[] { false, false, true, true};
                        break;
                    case 90:
                        validDirections = new bool[] { true, false, false, true};
                        break;
                    case 180:
                        validDirections = new bool[] { true, true, false, false};
                        break;
                    case 270:
                        validDirections = new bool[] { false, true, true, false};
                        break;
                    case -90:
                        validDirections = new bool[] { false, true, true, false};
                        break;
                    case -180:
                        validDirections = new bool[] { true, true, false, false};
                        break;
                    case -270:
                        validDirections = new bool[] { true, false, false, true};
                        break;
                }
                break;
            case 3: //Right track- Train can move right or backwards.  
                connectedTracks = 2; 
                switch (orientation)
                {
                    case 0:
                        validDirections = new bool[] { false, true, true, false};
                        break;
                    case 90:
                        validDirections = new bool[] { false, false, true, true};
                        break;
                    case 180:
                        validDirections = new bool[] { true, false, false, true};
                        break;
                    case 270:
                        validDirections = new bool[] { true, true, false, false};
                        break;
                    case -90:
                        validDirections = new bool[] { true, true, false, false};
                        break;
                    case -180:
                        validDirections = new bool[] { true, false, false, true};
                        break;
                    case -270:
                        validDirections = new bool[] { false, false, true, true};
                        break;
                }
                break;
            case 4: //Dead end track- Train can move backwards only.  
                connectedTracks = 1;
                switch (orientation)
                {
                    case 0:
                        validDirections = new bool[] { false, false, true, false};
                        break;
                    case 90:
                        validDirections = new bool[] { false, false, false, true};
                        break;
                    case 180:
                        validDirections = new bool[] { true, false, false, false};
                        break;
                    case 270:
                        validDirections = new bool[] { false, true, false, false};
                        break;
                    case -90:
                        validDirections = new bool[] { false, true, false, false};
                        break;
                    case -180:
                        validDirections = new bool[] { true, false, false, false};
                        break;
                    case -270:
                        validDirections = new bool[] { false, false, false, true};
                        break;
                }
                break;
        }
    }

    public bool[] getValidDirections() //when the game starts, the train searches until it finds a track.  It needs the directions on Start() call.  
    {
        return validDirections;
    }

    private void OnTriggerEnter(Collider collisionObject) //Whenever the train enters the collision box, this will run.  
    {
        //Now we test this direction against the valid directions
        //The front of the player's train has just entered the collision zone for this part of the track.  

        collisionObject.gameObject.SendMessage("receiveDirections", validDirections); //The player's train now receives a message that gives a list of valid inputs
        //print("Entering track:  " + this.gameObject.name);
        //print("Valid Directions:  North:  " + validDirections[0] + ", East:  " + validDirections[1] + ", South:  " + validDirections[2] + ", West:  " + validDirections[3]);
        //They now have until the next trigger collision to choose a valid direction to travel.  
    }


    private void OnTriggerExit(Collider collisionObject) 
    {
        //When the train leaves the collider, the valid directions are checked.  
    }

    //Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        orientation = TrackObject.transform.eulerAngles.y; //get angle of track object.  
        setValidDirections(); //When the player enters the collision box of a given track, a valid list of possible directions they could be going is made.  
        //When the player next enters a collision box, the input keys they are pressing will correspond to a cardinal direction.  
        //If the direction they are holding down is valid, then they will move or turn in that direction.  
        //If the directional keys they are holding are not valid, then they will lose their speed, and it will not increase until they choose a valid direction.  
        //The keys they are pressing correspond to the valid directions they could travel
        //Eg if there is an upcoming left turn, and they press nothing, they'll could be punished by flying off of the track.  
    }

    //Update is called once every frame.  
    void Update()
    {
        
    }
}
