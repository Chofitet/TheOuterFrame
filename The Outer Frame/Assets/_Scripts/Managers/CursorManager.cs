using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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
    }

    [SerializeField] Texture2D DefaultCursor;
    [SerializeField] Texture2D ClickCursor;
    [SerializeField] Texture2D InteractiveCursor;
    bool isClicking;


    public void SetInteractCursor()
    {
        if (isClicking) return;
        Cursor.SetCursor(InteractiveCursor, new Vector2(InteractiveCursor.width / 2, InteractiveCursor.height / 2), CursorMode.Auto);
    }

    public void SetDefaultCursor()
    {
        Cursor.SetCursor(DefaultCursor, new Vector2(DefaultCursor.width / 2, DefaultCursor.height / 2), CursorMode.Auto);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Cursor.SetCursor(ClickCursor, new Vector2(ClickCursor.width / 2, ClickCursor.height / 2), CursorMode.Auto);
            StopAllCoroutines();
            StartCoroutine(BackToDefault());
            isClicking = true;
        }
    }

    IEnumerator BackToDefault()
    {
        yield return new WaitForSeconds(0.1f);
        Cursor.SetCursor(DefaultCursor, new Vector2(DefaultCursor.width / 2, DefaultCursor.height / 2), CursorMode.Auto);
        isClicking = false;
    }

}
