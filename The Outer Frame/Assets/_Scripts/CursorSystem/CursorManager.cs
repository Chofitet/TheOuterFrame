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

    [Header("Normal Cursors")]
    [SerializeField] Texture2D DefaultCursor;
    [SerializeField] Texture2D ClickCursor;
    [SerializeField] Texture2D InteractiveCursor;
    [Header("PC View Cursors")]
    [SerializeField] Texture2D PCDefaultCursor;
    [SerializeField] Texture2D PCClickCursor;
    [SerializeField] Texture2D PCInteractiveCursor;


    private enum CursorState { Default, Hover, Click }
    private CursorState currentState = CursorState.Default;
    private bool isInPCView;

    int hoverCount = 0;

    public Canvas targetCanvas;
    private GraphicRaycaster raycaster;
    private EventSystem eventSystem;
    private PointerEventData pointerEventData;
    bool zoomView;

    private void Start()
    {
        if (targetCanvas) raycaster = targetCanvas.GetComponent<GraphicRaycaster>();
        eventSystem = EventSystem.current;
        ApplyCursor(CursorState.Default);
    }
    public void EnterInteractive()
    {
        hoverCount++;
        if (hoverCount > 0) ChangeState(CursorState.Hover);
    }

    public void ExitInteractive()
    {
        hoverCount = Mathf.Max(hoverCount - 1, 0);
        if (hoverCount == 0) ChangeState(CursorState.Default);
    }

    public void ClickInteractive()
    {
        StartCoroutine(ClickRoutine());
    }

    private IEnumerator ClickRoutine()
    {
        ChangeState(CursorState.Click);
        yield return new WaitForSeconds(0.1f);

        if (hoverCount > 0)
            ChangeState(CursorState.Hover);
        else
            ChangeState(CursorState.Default);
    }

    private void ChangeState(CursorState newState)
    {
        currentState = newState;
        ApplyCursor(newState);
    }

    public void CheckView(Component sender, object obj)
    {
        ViewStates view = (ViewStates)obj;
        zoomView = false;

        if (view == ViewStates.PCView)
        {
            PCcursorCoroutine = StartCoroutine(EnterPCView());
        }
        else if(view == ViewStates.BoardZoomView)
        {
            zoomView = true;
            ForceDefault();
        }
        else 
        {
           if(PCcursorCoroutine != null) StopCoroutine(PCcursorCoroutine);
            ExitPCView();
        }
    }

    private void ApplyCursor(CursorState state)
    {
        bool pc = IsPointerOverCanvas() && isInPCView;
        Debug.Log(pc);
        switch (state)
        {
            case CursorState.Default:
                Cursor.SetCursor(
                    pc ? PCDefaultCursor : DefaultCursor,
                    pc ? Vector2.zero : Hotspot(DefaultCursor, true),
                    CursorMode.Auto
                );
                break;

            case CursorState.Hover:
                Cursor.SetCursor(
                    pc ? PCInteractiveCursor : InteractiveCursor,
                    pc ? Vector2.zero : Hotspot(InteractiveCursor, true),
                    CursorMode.Auto
                );
                break;

            case CursorState.Click:
                Cursor.SetCursor(
                    pc ? PCClickCursor : ClickCursor,
                    pc ? Vector2.zero : Hotspot(ClickCursor, true),
                    CursorMode.Auto
                );
                break;
        }
    }

    private void Update()
    {
        if (zoomView)
        {
            ForceDefault();
        }
        ApplyCursor(currentState);
    }
    public void ForceDefault()
    {
        hoverCount = 0;
        ChangeState(CursorState.Default);
    }
    Coroutine PCcursorCoroutine;
    IEnumerator EnterPCView()
    {
        yield return new WaitForSeconds(0.5f);
        isInPCView = true;
        ApplyCursor(currentState);
    }

    public void ExitPCView()
    {
        isInPCView = false;
        ApplyCursor(currentState);
    }

    private bool IsPointerOverCanvas()
    {
        if (!raycaster) return false;

        pointerEventData = new PointerEventData(eventSystem)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, results);

        return results.Count > 0;
    }

    private Vector2 Hotspot(Texture2D tex, bool centered)
    {
        return centered ? new Vector2(tex.width / 2, tex.height / 2) : Vector2.zero;
    }
}

