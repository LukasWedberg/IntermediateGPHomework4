using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomCameraNavigator : MonoBehaviour
{
    public Transform currentRoom = null;

    [SerializeField]
    Transform[] roomCamsToNavigate = new Transform[4];

    [SerializeField]
    Image shroud;

    Animator shroudAnim;

    bool currentlyChangingRoom = false;
    Transform roomToGoTo = null;

    [SerializeField]
    LayerMask lesterLayer;

    [SerializeField]
    Camera cam = null;





    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("!INSTRUCTIONS!");
        Debug.Log("Its time to give your pet dog Lester a bath.");
        Debug.Log("All you have to do is click Lester.");
        Debug.Log("Lester is invisible.");
        Debug.Log("Fortunately Lester is a playful black newfoundland puppy."); 
        Debug.Log("Lester will be shedding a lot as he runs around, so use that!");
        Debug.Log("GOOD LUCK!");


        shroudAnim = shroud.transform.GetComponent<Animator>();

        ChangeRoom(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentlyChangingRoom)
        {
            //Here we check if the animator is done shrouding the screen.
            //If the screen is obscured, we can do a smooth transition!
            if (shroudAnim.GetCurrentAnimatorStateInfo(0).IsName("ScreenShroud") && !shroudAnim.IsInTransition(0) && shroudAnim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 )
            {
                transform.position = roomToGoTo.position;
                transform.rotation = roomToGoTo.rotation;

                shroudAnim.SetBool("ChangeScene", false);
                currentlyChangingRoom = false;
                roomToGoTo = null;
            }

            


        }


        if (Input.GetMouseButtonUp(0))
        {
            Vector2 touchUpPos = Input.mousePosition;
            Ray currentRay = cam.ScreenPointToRay(touchUpPos);

            RaycastHit hit;
            if (Physics.Raycast(currentRay, out hit, 300, lesterLayer))
            {
                Debug.Log("You found Lester! YOU WIN!!!");
            }

        }


    }

    


    public void ChangeRoom(int roomIndex) 
    {
        currentlyChangingRoom = true;

        roomToGoTo = roomCamsToNavigate[roomIndex];

        shroudAnim.SetBool("ChangeScene", true);
        


    }

    


}
