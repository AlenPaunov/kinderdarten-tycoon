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
		Camera.main.transform.position = new Vector3 (World.Width/2,World.Height/2,Camera.main.transform.position.z);
		Camera.main.orthographicSize = 2;

	}

	void Update(){
		//TODO PAUSE/UNPAUSE;	
		World.Update (Time.deltaTime);
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
