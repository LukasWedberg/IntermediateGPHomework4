using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class NewfieAI : MonoBehaviour
{
    //We're going to make the invisible newfoundland dog move around randomly--but only semi-randomly!
    //I believe the newfie should not visit the same room for a while, so we will use a grab-bag approach, much like last week's tetris attempt!

    [SerializeField]
    Transform[] roomsToNavigate = new Transform[4];

    [SerializeField]
    List<Transform> randomRoomsBag = new List<Transform>();

    //Additionally, we want to make it so that the newfie tries to leave the room when the player enters. Like its running away!
    [SerializeField]
    public float newfieReactionTime = .5f;

    [SerializeField]
    Transform currentRoom = null;

    NavMeshAgent nav;

    Transform mainCamObject = null;

    RoomCameraNavigator playerCam = null;

    ParticleSystem fuzzParticles = null;

    //Random movement timer
    float newfieMoveTimer = 0;
    float newfieMoveTimerTotal = 0;

    //Setting an upper and lower random number for the move timer!

    [SerializeField]
    Vector2 movementTimerMinAndMax = new Vector2(1.01f, 3.01f);

    [SerializeField]
    LayerMask floorLayer;



    //Newfie AI state:
    enum NewfieAiState
    { 
        fleeing,
        wandering,    
    }

    NewfieAiState currentNewfieState = NewfieAiState.wandering;


    // Start is called before the first frame update
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();

        mainCamObject = Camera.main.transform;

        playerCam = mainCamObject.GetComponent<RoomCameraNavigator>();

        fuzzParticles = GetComponent<ParticleSystem>();

        MakerandomRoomsBag();

        ResetMoveTimer();


    }

    // Update is called once per frame
    void Update()
    {
        GetCurrentRoom();
        CheckDestinationReached();

        //If I had extra time, I'd have liked to make it so that the newfie drops whatever its doing and runs, if the player enters the same room as it.
        //But it looks like I'm out of time! This first part of the if-statement will just have to remain for when I want to do more in my spare time!

        if (currentNewfieState == NewfieAiState.fleeing)
        {
            if (CheckDestinationReached())
            {
                currentNewfieState = NewfieAiState.wandering;
            }




        }
        else if (currentNewfieState == NewfieAiState.wandering)
        {

            

            if (currentRoom == playerCam.currentRoom)
            {
                //This means we need to run to a new room!
                currentNewfieState = NewfieAiState.fleeing;



            }
            else
            {
                //Debug.Log("We're a-wandering!");
                if (CheckDestinationReached())
                {
                    fuzzParticles.enableEmission = false;


                    if (newfieMoveTimer > newfieMoveTimerTotal)
                    {
                        //Pick a random destination!

                        PickRandomDestinationInRandomRoom();

                        fuzzParticles.enableEmission = true;



                        ResetMoveTimer();
                    }
                    else
                    {
                        newfieMoveTimer += Time.deltaTime;

                    }

                }

            }
        }


    }


    void GetCurrentRoom()
    {
        //We're getting the current room by raycasting downard, and hopefully hitting a floor.
        //Then we check the parent of the floor, and that counts as our room!

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 30, floorLayer))
        {
            if (hit.transform.name == "Floor")
            {
                currentRoom = hit.transform.parent;
            }
        }
        else
        {
            currentRoom = null;
        
        }
    }


    bool CheckDestinationReached()
    { 
        return Vector3.Distance(nav.destination, transform.position) <= 1 ? true : false;


       
    }


    void ResetMoveTimer()
    {
        newfieMoveTimer = 0;
        newfieMoveTimerTotal = Random.Range(movementTimerMinAndMax.x, movementTimerMinAndMax.y);
    }

    void PickRandomDestinationInRandomRoom()
    {
        //So first we need to pick a random room from the bag.
        Transform randomRoom = randomRoomsBag[0];

        //Don't forget to temporarily remove the room from the bag, so we don't choose the same room again!
        randomRoomsBag.Remove(randomRoom);


        //Now we will need to pick a random spot in the room...
        //We'll do this by getting the size of the floor and picking a random spot within those bounds!

        Transform floor = randomRoom.Find("Floor");

        Vector3 floorDimensions = floor.localScale;

        Vector3 randomDestination = floor.transform.position + Vector3.Scale( 
            randomRoom.rotation * new Vector3(floorDimensions.x,.6f,floorDimensions.z ), 
            new Vector3(Random.Range(-.99f, .99f) ,1, Random.Range(-.99f, .99f)));
        
        nav.destination = randomDestination;

        //We're choosing to reset the bag at the end so that way we minimize how long it is empty.
        if (randomRoomsBag.Count == 0)
        {
            MakerandomRoomsBag();
        }
    }

    void MakerandomRoomsBag()
    {
        for (int i = 0; i < roomsToNavigate.Length; i++)
        {
            randomRoomsBag.Add(roomsToNavigate[i]);
        }

        // Knuth shuffle algorithm :: courtesy of Wikipedia :)
        for (int t = 0; t < randomRoomsBag.Count; t++)
        {
            Transform temporary = randomRoomsBag[t];
            int randomIndex = Random.Range(t, randomRoomsBag.Count);
            randomRoomsBag[t] = randomRoomsBag[randomIndex];
            randomRoomsBag[randomIndex] = temporary;
        }
    }

}
