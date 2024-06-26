using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class TakeDossier : MonoBehaviour
{
    [SerializeField] Transform ObjectToMove;
    [SerializeField] Transform TakenPos;
    [SerializeField] float transitionSpeed;
    [SerializeField] Transform initPos;
    Transform currentPos;

    private void Start()
    {
        currentPos = initPos;
    }

    private void OnMouseUpAsButton()
    {
        currentPos = TakenPos;
        GetComponent<BoxCollider>().enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            Debug.Log("back");
            currentPos = initPos;
            GetComponent<BoxCollider>().enabled = true;
        }

    }

    private void LateUpdate()
    {
        
        Quaternion currentAngle;

        ObjectToMove.position = Vector3.Lerp(ObjectToMove.position, currentPos.position, Time.deltaTime * transitionSpeed);


        currentAngle = Quaternion.Lerp(ObjectToMove.rotation, currentPos.rotation, Time.deltaTime * transitionSpeed);


        ObjectToMove.rotation = currentAngle;


    }


}
