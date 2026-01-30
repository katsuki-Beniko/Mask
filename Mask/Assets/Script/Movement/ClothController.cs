using UnityEngine;

public class clothController : MonoBehaviour
{
    [SerializeField] private Animator LeftMove;
    [SerializeField] private Animator RightMove;

    bool isOpen = false;

    //void Start()
    //{
    //    LeftMove = GetComponent<Animator>();
    //    RightMove = GetComponent<Animator>();
    //}

    public void move()
    {
        isOpen = !isOpen;

        LeftMove.SetBool("isOpen", isOpen);
        RightMove.SetBool("isOpen", isOpen);
    }
}
