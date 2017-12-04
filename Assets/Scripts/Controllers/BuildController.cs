using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildController : MonoBehaviour
{
	public enum Mode{Select,Remove_Floor,Build_Floor,Build_Obj};
	Mode mode;

	bool buildModeIsObject = false;
	TileType buildModeTileType;
	string buildModeObjectType;

	// Use this for initialization
	void Start ()
	{
	}

	//====================================================================================//

	public void DoBuild(Tile t){
		switch (mode) {
		case Mode.Build_Floor:
		case Mode.Remove_Floor:
			BuildRemoveFloor (t);
			break;
		case Mode.Build_Obj:
			BuildObject (t);
			break;
		}
	}

	void BuildObject (Tile t)
	{
		if (WorldController.Instance.World.IsStaticObjectPlacementValid (buildModeObjectType, t) && t.pendingJob == null) {
			
			string objType = buildModeObjectType;
			Job	j = new Job (t, (theJob) => {
				WorldController.Instance.World.PlaceStaticObject (objType, theJob.tile);
				t.pendingJob = null;
			}
			);

			t.pendingJob = j;
			j.RegisterJobCancelCallBack ((theJob) => {theJob.tile.pendingJob = null;
			});

			WorldController.Instance.World.jobQueue.Enqueue (j);
			Debug.Log ("Job Queue size: " + WorldController.Instance.World.jobQueue.Count);
		}
	}
		
	/// <summary>
	/// Remove floor - reverts to original tile under the floor.
	/// </summary>
	void BuildRemoveFloor (Tile t)
	{
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
		
	/// <summary>
	/// Sets the mode to build floor.
	/// </summary>
	public void SetMode_BuildFloor ()
	{
		mode = Mode.Build_Floor;
		buildModeIsObject = false;
	}

	/// <summary>
	/// Sets the mode to remove floor.
	/// </summary>
	public void SetMode_RemoveFloor ()
	{
		mode = Mode.Remove_Floor;
		buildModeIsObject = false;
	}

	/// <summary>
	/// Sets the mode build object.
	/// </summary>
	/// <param name="buildObjectType">Build object type.</param>
	public void SetMode_BuildObject (string buildObjectType)
	{
		//wall is not a tile
		buildModeIsObject = true;
		if (buildObjectType == "Wall_Planks") {
			mode = Mode.Build_Obj;
			buildModeObjectType = buildObjectType;
		}
		if (buildObjectType == "Door_Simple") {
			mode = Mode.Build_Obj;
			buildModeObjectType = buildObjectType;
		}
	}
}
