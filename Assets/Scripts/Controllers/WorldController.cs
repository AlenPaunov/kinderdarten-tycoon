using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WorldController : MonoBehaviour {
	public World World{ get; protected set;}
	public static WorldController Instance{ get; protected set;}

	// Use this for initialization
	void OnEnable () {

		if (Instance!=null) {
			Debug.LogError ("two worlds error");
		}
			
		Instance = this;
		World = new World ();

	}

	/// <summary>
	/// Gets the tile at world coordinate.
	/// </summary>
	public Tile GetTileAtWorldCoord(Vector3 coord){
		int x = Mathf.FloorToInt (coord.x);
		int y = Mathf.FloorToInt (coord.y);

		return World.GetTileAt (x, y);
	}

}
