using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleTrigger : MonoBehaviour
{
    Camera _camera;
    public bool onTrigger;


    [Header("Select trigger type")]
    [SerializeField] bool isOnScreen;
    [SerializeField] bool isPassingThrough;
    [SerializeField] bool isClicked;
    [SerializeField] bool isPointing;
    [SerializeField] bool isSwitching;

    [Header("Is for once")]
    [SerializeField] public bool yes;
    
    [Header("Parameters")]
    float DistantToClick = 1.15f;
    [SerializeField] float DistantToSee = 3;


    bool x;
    bool y;

    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (isClicked && !yes) onTrigger = false;

        if (isOnScreen)
        {
            Vector3 screenPoint = _camera.WorldToViewportPoint(transform.position);
            onTrigger = screenPoint.z > 0 && screenPoint.x > -0.3f && screenPoint.x < 1.3f && screenPoint.y > -1 && screenPoint.y < 2;
        }

        if (isOnScreen && yes)
        {
            Vector3 screenPoint = _camera.WorldToViewportPoint(transform.position);
            if (screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1)
            {
                onTrigger = true;
            }
        }

        if(isPassingThrough && x)
        {
            onTrigger = true;
        }

        if (isPassingThrough && yes)
        {
            if(y)
            {
                onTrigger = true;
            }
            else onTrigger = false;
        }

        if (isPointing)
        {
            Ray ray = new Ray(transform.position, _camera.transform.position - transform.position);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, DistantToSee))
            { 
                if (hit.collider.tag == "Player")
                {
                    onTrigger = true;
                }
            }
        }

        if (isPointing && yes)
        {
            Ray ray = new Ray(transform.position, _camera.transform.position - transform.position);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, DistantToSee))
            {
                if (hit.collider.tag == "Player")
                {
                    onTrigger = true;
                }
                else onTrigger = false;
            }
            
        }


        if (isClicked && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            /*
            Ray ray = new Ray(transform.position, _camera.transform.position - transform.position);
            Debug.DrawRay(ray.origin, ray.direction, Color.blue);
            */
            RaycastHit hit;
           /* Vector3 screenPoint = _camera.WorldToViewportPoint(transform.position);
            bool see = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
           */
            if (Physics.Raycast(ray, out hit, DistantToClick))
            {
                if (hit.collider.gameObject == this.gameObject)
                {
                    onTrigger = true;
                }
            }
            
        }

        if (isSwitching && Input.GetMouseButtonDown(0))
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            /*Ray ray = new Ray(transform.position, _camera.transform.position - transform.position);
            Debug.DrawRay(ray.origin, ray.direction, Color.blue);
           */
            RaycastHit hit;
            /*
            Vector3 screenPoint = _camera.WorldToViewportPoint(transform.position);
            bool see = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
            */
            if (Physics.Raycast(ray, out hit, DistantToClick))
            {
                if (hit.collider.gameObject == this.gameObject)
                {
                    if (!onTrigger)
                    {
                        onTrigger = true;
                    }
                    else onTrigger = false;
                }
            }
            

        }


    }

   private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            y = true;
            x = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {

            y = false;
        }
    }
}
