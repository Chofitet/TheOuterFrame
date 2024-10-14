using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour
{
    public static CursorManager CM { get; private set; }

    private void Awake()
    {
        if (CM != null && CM != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            CM = this;
        }
    }

    private void Start()
    {
        Cursor.SetCursor(DefaultCursor, new Vector2(DefaultCursor.width / 2, DefaultCursor.height / 2), CursorMode.Auto);
        raycaster = targetCanvas.GetComponent<GraphicRaycaster>();
        eventSystem = EventSystem.current;
    }

    [SerializeField] Texture2D DefaultCursor;
    [SerializeField] Texture2D ClickCursor;
    [SerializeField] Texture2D InteractiveCursor;

    [SerializeField] Texture2D PCDefaultCursor;
    [SerializeField] Texture2D PCClickCursor;
    [SerializeField] Texture2D PCInteractiveCursor;
    bool isClicking;
    bool onceEnter;

    public Canvas targetCanvas; // Asigna tu Canvas en el Inspector
    private GraphicRaycaster raycaster;
    private PointerEventData pointerEventData;
    private EventSystem eventSystem;



    public void SetInteractCursor()
    {
        if (isClicking) return;
        if (IsPointerOverCanvas() && isInPcView) Cursor.SetCursor(PCInteractiveCursor, Vector2.zero, CursorMode.Auto);
        else Cursor.SetCursor(InteractiveCursor, new Vector2(InteractiveCursor.width / 2, InteractiveCursor.height / 2), CursorMode.Auto);
    }

    public void SetDefaultCursor()
    {
        if(IsPointerOverCanvas() && isInPcView) Cursor.SetCursor(PCDefaultCursor, Vector2.zero, CursorMode.Auto);
        else Cursor.SetCursor(DefaultCursor, new Vector2(DefaultCursor.width / 2, DefaultCursor.height / 2), CursorMode.Auto);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (IsPointerOverCanvas() && isInPcView) Cursor.SetCursor(PCClickCursor, Vector2.zero, CursorMode.Auto);
            else Cursor.SetCursor(ClickCursor, new Vector2(ClickCursor.width / 2, ClickCursor.height / 2), CursorMode.Auto);
            StopAllCoroutines();
            StartCoroutine(BackToDefault());
            isClicking = true;
        }

        if (!isInPcView) return;
        if(IsPointerOverCanvas())
        {
            if (onceEnter) return;
            SetDefaultCursor();
            onceEnter = true;
        }
        else
        {
            if(onceEnter) SetDefaultCursor();
            onceEnter = false;
        }
    }

    IEnumerator BackToDefault()
    {
        yield return new WaitForSeconds(0.1f);
        if (IsPointerOverCanvas() && isInPcView) Cursor.SetCursor(PCDefaultCursor, Vector2.zero, CursorMode.Auto);
        else Cursor.SetCursor(DefaultCursor, new Vector2(DefaultCursor.width / 2, DefaultCursor.height / 2), CursorMode.Auto);
        isClicking = false;
    }

    bool isInPcView;

    public void CheckView(Component sender, object obj)
    {
        if ((ViewStates)obj == ViewStates.PCView)
        {
            StartCoroutine(ChangeCursorInPC());
        }
        else
        {
            StopAllCoroutines();
            isInPcView = false;
            isClicking = false;
        }
    }

    IEnumerator ChangeCursorInPC()
    {
        yield return new WaitForSeconds(0.5f);
        isInPcView = true;
        SetDefaultCursor();
    }

    private bool IsPointerOverCanvas()
    {
        pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = Input.mousePosition;

        var results = new System.Collections.Generic.List<RaycastResult>();

        raycaster.Raycast(pointerEventData, results);

        return results.Count > 0;
    }

}
