using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour
{
	public enum Mode{Select,Remove_Floor,Build_Floor,Build_Obj};
	Mode mode;
	public GameObject highlightCell;
	public GameObject highlightCellPrefab;
	public GameObject ghoustCell;


	// the world positions
	Vector3 lastFramePos;
	Vector3 dragStartPosition;
	Vector3 currFramePos;
	List<GameObject> dragHighlightCellObjects;
	// Use this for initialization
	void Start ()
	{
		dragHighlightCellObjects = new List<GameObject> ();
		SimplePool.Preload (highlightCellPrefab, 25);	
	}
	
	// Update is called once per frame
	void Update ()
	{
		currFramePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		currFramePos.z = 0;

		UpdateHighLightCell (); //update the highlighted cell
		UpdateDragging (); //update dragging
		UpdateCamera (); // update Camera position

		lastFramePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		lastFramePos.z = 0;
	}

	//====================================================================================//

	void UpdateCamera ()
	{
		//handle screen dragging
		if (Input.GetMouseButton (1) || Input.GetMouseButton (2)) {
			Vector3 diff = lastFramePos - currFramePos;
			Camera.main.transform.Translate (diff);
		}

		Camera.main.orthographicSize -= Camera.main.orthographicSize * Input.GetAxis ("Mouse ScrollWheel");
		Camera.main.orthographicSize = Mathf.Clamp (Camera.main.orthographicSize, 1f, 6f);
	}

	void UpdateHighLightCell ()
	{
		//update the highlight cell pos;
		Tile tileUnderMouse = WorldController.Instance.GetTileAtWorldCoord (currFramePos);
		if (tileUnderMouse != null) {
			Vector3 cursorPos = new Vector3 (tileUnderMouse.X, tileUnderMouse.Y, -5);
			highlightCell.transform.position = cursorPos;
			highlightCell.SetActive (true);
		} else {
			highlightCell.SetActive (false);
		}
	}

	void UpdateDragging ()
	{
		// if over UI return
		if (EventSystem.current.IsPointerOverGameObject()) {
			return;
		}
		//handle left mouse click
		//Start Dragging
		if (Input.GetMouseButtonDown (0)) {
			dragStartPosition = currFramePos;
		}
		int startX = Mathf.FloorToInt (dragStartPosition.x);
		int endX = Mathf.FloorToInt (currFramePos.x);
		int startY = Mathf.FloorToInt (dragStartPosition.y);
		int endY = Mathf.FloorToInt (currFramePos.y);

		if (endX < startX) {
			int tmp = endX;
			endX = startX;
			startX = tmp;
		}

		if (endY < startY) {
			int tmp = endY;
			endY = startY;
			startY = tmp;
		}
		//clean drag previews
		CleanDragPreview ();
		HighlightPreview (startX, endX, startY, endY);
			
		//End Dragging
		if (Input.GetMouseButtonUp (0)) {
			BuildController bc = GameObject.FindObjectOfType<BuildController> ();

			for (int x = startX; x <= endX; x++) {
				for (int y = startY; y <= endY; y++) {
					Tile t = WorldController.Instance.World.GetTileAt (x, y);
					if (t!=null) {
						bc.DoBuild (t);
					}
				}
			}
		}
	}

	void CleanDragPreview ()
	{
		while (dragHighlightCellObjects.Count > 0) {
			GameObject obj = dragHighlightCellObjects [0];
			dragHighlightCellObjects.RemoveAt (0);
			SimplePool.Despawn (obj);
		}
	}

	void HighlightPreview (int startX, int endX, int startY, int endY)
	{
		if (Input.GetMouseButton (0)) {
			for (int x = startX; x <= endX; x++) {
				for (int y = startY; y <= endY; y++) {
					Tile t = WorldController.Instance.World.GetTileAt (x, y);
					if (t != null) {
						GameObject go = SimplePool.Spawn (highlightCellPrefab, new Vector3 (x, y, -1), Quaternion.identity);
						go.transform.SetParent (this.transform, true);
						dragHighlightCellObjects.Add (go);
					}
				}
			}
		}
	}
		
}
