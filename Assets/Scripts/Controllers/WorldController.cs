using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WorldController : MonoBehaviour {
	public World World{ get; protected set;}
	public static WorldController Instance{ get; protected set;}

	public Sprite sandSprite;
	public Sprite soilSprite;
	public Sprite gravelSprite;
	public Sprite roughtStoneSprite;
	public Sprite floorSprite;

	// Use this for initialization
	void Start () {
		World = new World ();
		if (Instance!=null) {
			Debug.LogError ("two worlds error");
		}
		Instance = this;
		//create GameObject for each tile in the world
		for (int x = 0; x < World.Width; x++) {
			for (int y = 0; y < World.Height; y++) {
				Tile tile_data = World.GetTileAt (x, y);

				GameObject tile_go = new GameObject ();
				tile_go.name = "Tile_" + x + "_" + y;

				tile_go.transform.position	= new Vector3 (tile_data.X, tile_data.Y, tile_data.Z);
				tile_go.transform.SetParent (this.transform, true);

				tile_go.AddComponent<SpriteRenderer> ();

				tile_data.RegisterTypeChangedCallBack ((tile) => {OnTileTypeChanged(tile, tile_go);});

			}	
		}
		World.RandomizeTiles ();
	}

	/// <summary>
	/// Raises the tile type changed event.
	/// </summary>
	/// <param name="tile_Data">Tile data.</param>
	/// <param name="tile_go">Tile go.</param>
	void OnTileTypeChanged(Tile tile_Data, GameObject tile_go){

		if (tile_Data.Type == Tile.TileType.Gravel) {
			tile_go.GetComponent<SpriteRenderer> ().sprite = gravelSprite;
		} else if (tile_Data.Type == Tile.TileType.Sand) {
			tile_go.GetComponent<SpriteRenderer> ().sprite = sandSprite;
		} else if (tile_Data.Type == Tile.TileType.Soil) {
			tile_go.GetComponent<SpriteRenderer> ().sprite = soilSprite;
		} else if (tile_Data.Type == Tile.TileType.RoughStone) {
			tile_go.GetComponent<SpriteRenderer> ().sprite = roughtStoneSprite;
		} else if (tile_Data.Type == Tile.TileType.Floor) {
			tile_go.GetComponent<SpriteRenderer> ().sprite = floorSprite;
		}else {
			Debug.LogError ("Error in Sprite of tile with type " + tile_Data.Type);
		}
	}

	/// <summary>
	/// Gets the tile at world coordinate.
	/// </summary>
	/// <returns>The tile at world coordinate.</returns>
	/// <param name="coord">Coordinate.</param>
	public Tile GetTileAtWorldCoord(Vector3 coord){
		int x = Mathf.FloorToInt (coord.x);
		int y = Mathf.FloorToInt (coord.y);

		return World.GetTileAt (x, y);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
