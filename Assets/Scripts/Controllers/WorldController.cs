using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WorldController : MonoBehaviour {
	public World World{ get; protected set;}
	public static WorldController Instance{ get; protected set;}
	public Dictionary<Tile, GameObject> tileGameobjectMap;
	public Dictionary<StaticObject, GameObject> staticObjGameobjectMap;
	public Dictionary<string, Sprite> staticObjectsSprites;

	public Sprite sandSprite;
	public Sprite soilSprite;
	public Sprite gravelSprite;
	public Sprite roughtStoneSprite;
	public Sprite floorSprite;
	public Sprite wallSprite; //FIXME;

	// Use this for initialization
	void OnEnable () {

		if (Instance!=null) {
			Debug.LogError ("two worlds error");
		}

		LoadSprites ();
		Instance = this;
		World = new World ();
		World.RegisterStaticObjectCreated (OnStaticObjectCreated);
		// instantiate tileGameobject map
		tileGameobjectMap = new Dictionary<Tile, GameObject>();
		staticObjGameobjectMap = new Dictionary<StaticObject, GameObject> ();

		//create GameObject for each tile in the world
		for (int x = 0; x < World.Width; x++) {
			for (int y = 0; y < World.Height; y++) {
				Tile tile_data = World.GetTileAt (x, y);

				GameObject tile_go = new GameObject ();
				tile_go.name = "Tile_" + x + "_" + y;

				tileGameobjectMap.Add (tile_data, tile_go);	

				tile_go.transform.position	= new Vector3 (tile_data.X, tile_data.Y, tile_data.Z);
				tile_go.transform.SetParent (this.transform, true);

				tile_go.AddComponent<SpriteRenderer> ();
			}	
		}
		World.RegisterTileChanged (OnTileChanged);
		World.RandomizeTiles ();

		Camera.main.transform.position = new Vector3 (World.Width/2,World.Height/2,Camera.main.transform.position.z);
	}

	// Update is called once per frame
	void Update () {

	}

	void LoadSprites(){
		staticObjectsSprites = new Dictionary<string, Sprite> ();
		Sprite [] sprites = Resources.LoadAll<Sprite> ("Objects");
		foreach (Sprite s in sprites) {
			staticObjectsSprites[s.name] = s;
		}
	}

	/// <summary>
	/// Raises the tile type changed event.
	/// </summary>
	/// <param name="tile_Data">Tile data.</param>
	/// <param name="tile_go">Tile go.</param>
	void OnTileChanged(Tile tile_Data){
		if (tileGameobjectMap.ContainsKey(tile_Data)==false) {
			Debug.LogError("Missing tile in tileGOmap - forget to add or not unregister");
			return;
		}
		GameObject tile_go = tileGameobjectMap [tile_Data];

		if (tile_go == null) {
			Debug.LogError("Missing tile in tileGOmap - forget to add or not unregister");
			return;
		}

		if (tile_Data.Type == TileType.Gravel) {
			tile_go.GetComponent<SpriteRenderer> ().sprite = gravelSprite;
		} else if (tile_Data.Type == TileType.Sand) {
			tile_go.GetComponent<SpriteRenderer> ().sprite = sandSprite;
		} else if (tile_Data.Type == TileType.Soil) {
			tile_go.GetComponent<SpriteRenderer> ().sprite = soilSprite;
		} else if (tile_Data.Type == TileType.RoughStone) {
			tile_go.GetComponent<SpriteRenderer> ().sprite = roughtStoneSprite;
		} else if (tile_Data.Type == TileType.Floor) {
			SpriteRenderer sr = tile_go.GetComponent<SpriteRenderer> ();
			sr.sprite = floorSprite;
			//sr.color = new Color (0.76f, 0.36f, 0, 1); 	
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

	//UNIMPLEMENTED
	/// <summary>
	/// Destroies all tiles. THIS IS FOR LATER IN GAME WHEN WE HAVE MULTIPLE FLOORS/LAYERS/LEVELS
	/// </summary>
	void DestroyAllTiles(){
		while (tileGameobjectMap.Count>0) {
			Tile tile_data = tileGameobjectMap.First ().Key;
			GameObject	tile_go = tileGameobjectMap [tile_data];

			tileGameobjectMap.Remove (tile_data);
			tile_data.UnregisterChangedCallBack (OnTileChanged);
			Destroy (tile_go);
		}

		//after this we would call soething to rebuild all the objects for the tiles on new floor/ level
	}

	public void OnStaticObjectCreated(StaticObject obj){
		GameObject obj_go = new GameObject ();
		obj_go.name = "obj_" + obj.ObjectType;
		staticObjGameobjectMap.Add (obj, obj_go);	

		obj_go.transform.position	= new Vector3 (obj.Tile.X, obj.Tile.Y, -2);
		obj_go.transform.SetParent (this.transform, true);


		//FIXME: assume the object must be a wall so we use harcoded wall sprite
		obj_go.AddComponent<SpriteRenderer> ().sprite = GetSpriteForStaticObject(obj);
		obj.RegisterOnChangedCallBack (OnStaticObjChanged);
	}

	public void OnStaticObjChanged(StaticObject obj){
		// not implemented
		if (staticObjGameobjectMap.ContainsKey(obj)==false) {
			Debug.LogError ("missing object WC line 150" + obj.Tile.ToString ());
			return;
		}
		GameObject obj_go = staticObjGameobjectMap[obj];
		obj_go.GetComponent<SpriteRenderer> ().sprite = GetSpriteForStaticObject (obj);
	}

	Sprite GetSpriteForStaticObject(StaticObject obj){
		if (obj.LinksToNeighbour == false) {
			return staticObjectsSprites [obj.ObjectType];
		} 
		string spriteName = obj.ObjectType+"_";
		int x = obj.Tile.X;
		int y = obj.Tile.Y;

		Tile t;
		t = World.GetTileAt (x, y + 1);
		if (t != null && t.StaticObject != null && t.StaticObject.ObjectType==obj.ObjectType) {
			spriteName += "N";
		}
		t = World.GetTileAt (x+1, y);
		if (t != null && t.StaticObject != null && t.StaticObject.ObjectType==obj.ObjectType) {
			spriteName += "E";
		}
		t = World.GetTileAt (x, y - 1);
		if (t != null && t.StaticObject != null && t.StaticObject.ObjectType==obj.ObjectType) {
			spriteName += "S";
		}
		t = World.GetTileAt (x-1, y);
		if (t != null && t.StaticObject != null && t.StaticObject.ObjectType==obj.ObjectType) {
			spriteName += "W";
		}

		return staticObjectsSprites [spriteName];
	}



}
