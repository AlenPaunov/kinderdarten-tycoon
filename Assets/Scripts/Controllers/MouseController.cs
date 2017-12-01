using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour
{
	public enum Mode {Select, Remove_Floor, Build_Floor, Build_Wall};
	Mode mode;

	TileType buildModeTileType;
	public GameObject highlightCell;
	public GameObject highlightCellPrefab;
	bool buildModeIsObject = false;
	string buildModeObjectType;


	List<GameObject> dragHighlightCellObjects;
	// the world positions
	Vector3 lastFramePos;
	Vector3 dragStartPosition;
	Vector3 currFramePos;

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
		while (dragHighlightCellObjects.Count>0) {
			GameObject obj = dragHighlightCellObjects [0];
			dragHighlightCellObjects.RemoveAt (0);
			SimplePool.Despawn(obj);
		}


		if (Input.GetMouseButton(0)) {
			// display a preview of drag area
			for (int x = startX; x <= endX; x++) {
				for (int y = startY; y <= endY; y++) {
					Tile t = WorldController.Instance.World.GetTileAt (x, y);
					if (t != null) {
						
						GameObject go = SimplePool.Spawn(highlightCellPrefab, new Vector3 (x, y, -1), Quaternion.identity);
						go.transform.SetParent (this.transform, true);
						dragHighlightCellObjects.Add (go);
					}
				}
			}

		}

		//End Dragging
		if (Input.GetMouseButtonUp (0)) {
			switch (mode) {
			case Mode.Build_Floor :
			case Mode.Remove_Floor:
				BuildRemoveFloor (startX, endX, startY, endY);
				break;
			case Mode.Build_Wall:
				BuildObject (startX, endX, startY, endY);
				break;
			}

		}
	}

	void BuildObject(int startX, int endX, int startY, int endY){
		for (int x = startX; x <= endX; x++) {
			for (int y = startY; y <= endY; y++) {
				Tile t = WorldController.Instance.World.GetTileAt (x, y);
				if (t != null) {
					switch (mode) {
					case Mode.Build_Wall:
						WorldController.Instance.World.PlaceStaticObject (buildModeObjectType, t);
						break;
					}
				}
			}
		}
	}

	void BuildRemoveFloor (int startX, int endX, int startY, int endY)
	{
		for (int x = startX; x <= endX; x++) {
			for (int y = startY; y <= endY; y++) {
				Tile t = WorldController.Instance.World.GetTileAt (x, y);
				if (t != null) {
					switch (mode) {
					case Mode.Build_Floor:
						if (t.Type != TileType.Floor) {
							t.previousType = t.Type;
						}	
						t.Type = TileType.Floor;
						break;
					case Mode.Remove_Floor:
						t.Type = t.previousType;
						break;
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

	public void SetMode_Build(){
		mode = Mode.Build_Floor;
		buildModeIsObject = false;
	}
	public void SetMode_Buldoze(){
		mode = Mode.Remove_Floor;
		buildModeIsObject = false	;
	}

	public void SetMode_BuildWall(string buildObjectType){
		//wall is not a tile
		buildModeIsObject = true;
		if (buildObjectType == "Wall") {
			mode = Mode.Build_Wall;
			buildModeObjectType = buildObjectType;
		}

	}
}
