using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothMoveObjectToPoints : MonoBehaviour
{
    [SerializeField] Transform ObjectToMove;
    [SerializeField] Transform FinalPos;
    [SerializeField] float transitionSpeed;
    [SerializeField] Transform initPos;
    Transform currentPos;

    private void Start()
    {
        currentPos = initPos;
    }

    public void ChangePosition(Component sender, object obj)
    {
        bool istrue = (bool)obj;

        if (istrue)
        {
            currentPos = FinalPos;
        }
        else currentPos = initPos;
    }


    private void LateUpdate()
    {
        if (ObjectToMove == null) return;

        Quaternion currentAngle;

        ObjectToMove.position = Vector3.Lerp(ObjectToMove.position, currentPos.position, Time.deltaTime * transitionSpeed);


        currentAngle = Quaternion.Lerp(ObjectToMove.rotation, currentPos.rotation, Time.deltaTime * transitionSpeed);


        ObjectToMove.rotation = currentAngle;


    }
}
