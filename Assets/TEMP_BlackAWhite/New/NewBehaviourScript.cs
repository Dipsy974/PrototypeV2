using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    bool enter;
    Camera cam;
    Texture2D tex;
    public MeshRenderer rend;

    bool victoire;


    private void Start()
    {
        cam = Camera.main;
        tex = new Texture2D(100, 100);
        rend.material.SetTexture("_Texture2D", tex);
    }
    // Update is called once per frame
    void OnGUI()
    {
        if(enter)
        {
            Vector3 _point = new Vector3();
            Event _currentEvent = Event.current;
            Vector2 _mousePos = new Vector2();

            // Get the mouse position from Event.
            // Note that the y position from Event is inverted.
            _mousePos.x = _currentEvent.mousePosition.x;
            _mousePos.y = cam.pixelHeight - _currentEvent.mousePosition.y;

            _point = cam.ScreenToWorldPoint(new Vector3(_mousePos.x, _mousePos.y, cam.nearClipPlane));

            Ray _ray = new Ray(cam.transform.position, _point - cam.transform.position);
            var _hit = Physics.Raycast(_ray, out var rInfo);
            if(_hit)
            { 
                Debug.DrawRay(cam.transform.position, (_point - cam.transform.position)*10000);
                //Debug.Log(rInfo.textureCoord);
                ProcessTexture(rInfo.textureCoord);
            }
            if(victoire)
            {
                GUILayout.Label("You win");
            }
        }


    }

    public void ProcessTexture(Vector2 coord)
    {
        var intCoord = new Vector2Int((int)(coord.x * 100), (int)(coord.y * 100));
        tex.SetPixel(intCoord.x, intCoord.y, Color.white);
        tex.Apply();

        var percent = 0;
        for (int x = 0; x < 100; x++)
        {
            for (int y = 0; y < 100; y++)
            {
                if (tex.GetPixel(x,y).r==1)
                {
                    percent++;
                }
            }
        }

        if(percent>=20)
        {
            victoire = true;
        }
    }
    private void OnMouseDown()
    {
        enter = true;
    }
    private void OnMouseUp()
    {
        enter = false;
    }

    private void OnMouseExit()
    {
        enter = false;
    }
}
