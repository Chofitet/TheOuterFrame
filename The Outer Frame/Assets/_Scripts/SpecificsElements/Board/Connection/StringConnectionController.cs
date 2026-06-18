using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using GogoGaga.OptimizedRopesAndCables;


public class StringConnectionController : MonoBehaviour, IPlacedOnBoard
{
    [SerializeField] MoveBoardElementsToPos Node1;
    [SerializeField] MoveBoardElementsToPos Node2;
    

    [SerializeField] GameObject AnimPin1;
    Vector3 startPosPin1;
    [SerializeField] GameObject AnimPin2;
    Vector3 startPosPin2;
    [SerializeField] GameEvent OnPuttingStringSound;
    [SerializeField] List<ScriptableObject> Conditionals = new List<ScriptableObject>();
    [SerializeField] bool isOrderMatters;
    [SerializeField] MeshRenderer lineRenderer;
    [SerializeField] AnimationCurve curveStringAnim;
    GameObject content;
    [SerializeField] BoardType boardType;
    bool isConnected;

    private void Start()
    {
        content = transform.GetChild(0).gameObject;

        if (!Node1 || !Node2)
        {
            Debug.LogWarning("Board connection " + name + " dont have a conection node assigned");
            return;
        }

        startPosPin1 = AnimPin1.transform.position;
        startPosPin2 = AnimPin2.transform.position;
        AnimPin2.transform.position = startPosPin1;
        lineRenderer.enabled = false;
        AnimPin1.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
        AnimPin2.transform.GetChild(0).GetComponent<MeshRenderer>().enabled=false;
        Invoke("sarasa", 1.5f);
    }

    void sarasa()
    {
        if (isConnected) return;
        lineRenderer.enabled = true;
        AnimPin1.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
        AnimPin2.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
        content.SetActive(false);
    }

    public void UpdatePositionRotation(Component sender, object obj)
    {
      /*startPosPin1 = AnimPin1.transform.position;
        startPosPin2 = AnimPin2.transform.position;*/
        
    }

    bool pendingToMakeConection;

    bool once = false;
    public void CheckConnection(Component sender, object obj)
    {
       if (isConnected) return;
       if (Node1.GetIsPlacedFinish() && Node2.GetIsPlacedFinish() && CheckForConditionals())
        {
            if (GetComponentInParent<BoardStringsGroup>().CheckIfOtherStringArePlaced(this.gameObject)) return;
            pendingToMakeConection = true;
        }
    }

    public void MakeConnectionByClicking(Component sender, object obj)
    {
        if (isConnected) return;
        if (Node1.GetIsPlacedFinish() && Node2.GetIsPlacedFinish() && CheckForConditionals())
        {
            if (GetComponentInParent<BoardStringsGroup>().CheckIfOtherStringArePlaced(this.gameObject)) return;
            MakeConnectionAnim();
        }
    }

    public void MakeConnectionAutomactly(Component sender, object obj)
    {
        if (!pendingToMakeConection) return;
        MakeConnectionAnim();
    }

    void MakeConnectionAnim()
    {
        if (!once)
        {
            AnimPin2.transform.position = startPosPin1;
            once = true;

            Sequence seq = DOTween.Sequence();

            seq.Join(
                AnimPin2.transform.DOMoveX(startPosPin2.x, 0.5f).SetEase(Ease.Linear)
            );
            seq.Join(
                AnimPin2.transform.DOMoveZ(startPosPin2.z, 0.5f).SetEase(curveStringAnim)
            );

            seq.Join(
                AnimPin2.transform.DOMoveY(startPosPin2.y, 0.5f).SetEase(curveStringAnim)
            );

            // StartCoroutine(EnableMesh(true));
        }
        content.SetActive(true);

        if (!isConnected) OnPuttingStringSound?.Invoke(this, null);
        isConnected = true;
    }

    public bool GetIsConnected() { return isConnected; }

    public bool CheckForConditionals()
    {

        foreach (ScriptableObject conditional in Conditionals)
        {
            if (conditional is not IConditionable)
            {
                Debug.LogWarning(conditional.name + " is not a valid conditional");
                return false;
            }

            IConditionable auxConditional = conditional as IConditionable;

            if (!auxConditional.GetStateCondition(2))
            {
                return false;
            }
        }

        if (isOrderMatters) return CheckIfConditionalAreInOrder();
        else return true;
    }

    bool CheckIfConditionalAreInOrder()
    {
        List<int> nums = new List<int>();

        foreach (ScriptableObject conditional in Conditionals)
        {
            IConditionable auxConditional = conditional as IConditionable;

            if (auxConditional.CheckIfHaveTime())
            {
                nums.Add(auxConditional.GetTimeWhenWasComplete().GetTimeInNum());
            }

        }

        for (int i = 0; i < nums.Count - 1; i++)
        {
            if (nums[i] > nums[i + 1])
            {
                return false;
            }
        }

        return true;
    }

    public void ConnectDirectly()
    {
        if (isConnected) return;
        isConnected = true;
        content.SetActive(true);
        AnimPin2.transform.position = startPosPin2;
        AnimPin1.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
        AnimPin2.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
        lineRenderer.enabled = true;
        //StartCoroutine(EnableMesh(true));

    }

    IEnumerator EnableMesh(bool x)
    {
        yield return new WaitForSeconds(0.15f);
        lineRenderer.enabled = x;
        //DuplicateMeshForShadows();
    }

    void DuplicateMeshForShadows()
    {
        if (lineRenderer == null) return;

        if (lineRenderer.transform.parent.Find("ShadowCopy") != null)
            return;

        GameObject shadowObj = Instantiate(lineRenderer.gameObject, lineRenderer.transform.parent);
        shadowObj.name = "ShadowCopy";

        shadowObj.transform.localPosition = lineRenderer.transform.localPosition;
        shadowObj.transform.localRotation = lineRenderer.transform.localRotation;
        shadowObj.transform.localScale = lineRenderer.transform.localScale;

        var shadowRopeMesh = shadowObj.GetComponent<RopeMesh>(); 

        if (shadowRopeMesh != null)
        {
            shadowRopeMesh.ropeWidth =  0.001f;

        }

        var renderer = shadowObj.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            renderer.receiveShadows = false;
        }

    }

    public bool GetConditionalState()
    {
        return false;
    }

    public bool ActiveInBegining()
    {
        return false;
    }

    public bool GetIsTaken()
    {
        return false;
    }

    public bool IsOutOfBoard()
    {
        return false;
    }
    public WordData GetWordData()
    {
            return null;
    }

    BoardType IPlacedOnBoard.GetType()
    {
            return boardType;
    }

    public void ActiveInteraction()
    {
       
    }
}
