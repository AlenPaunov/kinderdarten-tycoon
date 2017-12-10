using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class TileSpriteController : MonoBehaviour {

	// The only tile sprite we have right now, so this
	// it a pretty simple way to handle it.
	public Sprite sandSprite;
	public Sprite soilSprite;
	public Sprite gravelSprite;
	public Sprite roughtStoneSprite;
	public Sprite floorSprite;


	Dictionary<string, Sprite> tilesSprites;
	Dictionary<Tile, GameObject> tileGameObjectMap;

	World world {
		get { return WorldController.Instance.World; }
	}

	// Use this for initialization
	void Start () {
		// Instantiate our dictionary that tracks which GameObject is rendering which Tile data.
		//tileGameObjectMap = new Dictionary<Tile, GameObject>();

		// Create a GameObject for each of our tiles, so they show visually. (and redunt reduntantly)
		//for (int x = 0; x < world.Width; x++) {
			//for (int y = 0; y < world.Height; y++) {
				// Get the tile data
//				Tile tile_data = world.GetTileAt(x, y);
//
//				// This creates a new GameObject and adds it to our scene.
//				GameObject tile_go = new GameObject();
//
//				// Add our tile/GO pair to the dictionary.
//				tileGameObjectMap.Add (tile_data, tile_go);
//				tile_go.transform.position = new Vector3 (tile_data.X, tile_data.Y, tile_data.Z);
//				tile_go.transform.SetParent (this.transform, true);
//				SpriteRenderer sr = tile_go.AddComponent<SpriteRenderer> ();
//				sr.sprite = GetSpriteForTile (tile_data);
//
//				OnTileChanged(tile_data);
//			}
//		}
//
//		// Register our callback so that our GameObject gets updated whenever
//		// the tile's type changes.
//		world.RegisterTileChanged( OnTileChanged );
	}
		
	// THIS IS AN EXAMPLE -- NOT CURRENTLY USED (and probably out of date)
	void DestroyAllTileGameObjects() {
		// This function might get called when we are changing floors/levels.
		// We need to destroy all visual **GameObjects** -- but not the actual tile data!

		while(tileGameObjectMap.Count > 0) {
			Tile tile_data = tileGameObjectMap.Keys.First();
			GameObject tile_go = tileGameObjectMap[tile_data];

			// Remove the pair from the map
			tileGameObjectMap.Remove(tile_data);

			// Unregister the callback!
			//tile_data.UnregisterTileTypeChangedCallBack( OnTileChanged );

			// Destroy the visual GameObject
			Destroy( tile_go );
		}

		// Presumably, after this function gets called, we'd be calling another
		// function to build all the GameObjects for the tiles on the new floor/level
	}

	// This function should be called automatically whenever a tile's data gets changed.
	void OnTileChanged( Tile tile_Data ) {

		if (tileGameObjectMap.ContainsKey(tile_Data)==false) {
			Debug.LogError("Missing tile in tileGOmap - forget to add or not unregister");
			return;
		}
		GameObject tile_go = tileGameObjectMap [tile_Data];

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

	void LoadSprites(){

		tilesSprites = new Dictionary<string, Sprite> ();
		Sprite[] tileSprites = Resources.LoadAll<Sprite> ("Tiles");
		foreach (var s in tileSprites) {
			tilesSprites [s.name] = s;
		}
	}

	Sprite GetSpriteForTile(Tile tile){
		return tilesSprites [tile.Type.	ToString()];
	}


}
