using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
	public GameObject highlightCell;
	public GameObject highlightCellPrefab;
	List<GameObject> dragHighlightCellObjects;
	// the world positions
	Vector3 lastFramePos;
	Vector3 dragStartPosition;
	Vector3 currFramePos;

	// Use this for initialization
	void Start ()
	{
		dragHighlightCellObjects = new List<GameObject> ();
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

	void UpdateCamera ()
	{
		//handle screen dragging
		if (Input.GetMouseButton (1) || Input.GetMouseButton (2)) {
			Vector3 diff = lastFramePos - currFramePos;
			Camera.main.transform.Translate (diff);
		}
	}

	void UpdateDragging ()
	{
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
		while (dragHighlightCellObjects.Count>0) {
			GameObject obj = dragHighlightCellObjects [0];
			dragHighlightCellObjects.RemoveAt (0);
			Destroy (obj);
		}


		if (Input.GetMouseButton(0)) {
			// display a preview of drag area
			for (int x = startX; x <= endX; x++) {
				for (int y = startY; y <= endY; y++) {
					Tile t = WorldController.Instance.World.GetTileAt (x, y);
					if (t != null) {
						
						GameObject go = (GameObject)Instantiate(highlightCellPrefab, new Vector3 (x, y, -1), Quaternion.identity);
						dragHighlightCellObjects.Add (go);
					}
				}
			}

		}

		//End Dragging
		if (Input.GetMouseButtonUp (0)) {
			for (int x = startX; x <= endX; x++) {
				for (int y = startY; y <= endY; y++) {
					Tile t = WorldController.Instance.World.GetTileAt (x, y);
					if (t != null) {
						t.Type = Tile.TileType.Floor;
					}
				}
			}
		}
	}

	void UpdateHighLightCell ()
	{
		//update the highlight cell pos;
		Tile tileUnderMouse = WorldController.Instance.GetTileAtWorldCoord (currFramePos);
		if (tileUnderMouse != null) {
			Vector3 cursorPos = new Vector3 (tileUnderMouse.X, tileUnderMouse.Y, -1);
			highlightCell.transform.position = cursorPos;
			highlightCell.SetActive (true);
		} else {
			highlightCell.SetActive (false);
		}
	}
}
