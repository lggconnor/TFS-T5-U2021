using UnityEngine;

// [System.Serializable]
public class MouseLock
{
    public bool debug = true;

    public Texture2D mouseCursor;
    public Texture2D reticleCursor;

    public MouseLock()
    {
        mouseCursor = Resources.Load<Texture2D>("UI/Cursors/cursor_reticle");
        reticleCursor = Resources.Load<Texture2D>("UI/Cursors/crosshair_reticle");
    }

    public void setReticleCursor()
    {
        if (debug)
            Debug.Log("ReticleCursor Funtion Called");
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.SetCursor(reticleCursor, Vector2.zero, CursorMode.Auto);
    }

    public void setMouseCursor()
    {
        if (debug)
            Debug.Log("MouseCursor Funtion Called");
        Cursor.lockState = CursorLockMode.None;
        Cursor.SetCursor(mouseCursor, Vector2.zero, CursorMode.Auto);
    }

    public void SetCurrentCursor(Texture2D newCursor, Vector2 hotspotOffset)
    {
        if (debug)
            Debug.Log("New Cursor Set");
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.SetCursor(newCursor, hotspotOffset, CursorMode.Auto);
    }






}
