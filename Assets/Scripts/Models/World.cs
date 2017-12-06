using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class World {
	//out tile data
	Tile[,] tiles;

	// The static objects prototypes.
	Dictionary<string,StaticObject> staticObjectsPrototypes;
	List<Baby> babies;
	List<Character> characters;

	public Path_TileGraph PathfindingGraph { get; set;}

	public int Width{ get; protected set; }
	public int Height{ get; protected set; }

	Action<StaticObject> cb_StaticObjectCreated;
	Action<Tile> cb_TileChanged;
	Action<Character> cb_CharacterCreated;

	public JobQueue jobQueue;

	/// <summary>
	/// Initializes a new instance of the World class.
	/// </summary>
	/// <param name="width">Width.</param>
	/// <param name="height">Height.</param>
	public World(int width = 60, int height = 60){

		jobQueue = new JobQueue();

		this.Width = width;
		this.Height = height;

		tiles = new Tile[width, height];
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				tiles [x, y] = new Tile (this, x, y);
				tiles [x, y].RegisterChangedCallBack (OnTileChanged);
			}
		}
		CreateStaticObjectPrototypes ();
		characters = new List<Character> ();

		babies = new List<Baby>();
		Baby a = new Baby(tiles[Width/2,Height/2]);
	}

	public void Update(float deltaTime){
		foreach (var c in characters) {
			c.Update (deltaTime);
		}
	}

	public Character CreateCharacter(Tile t){
		Character c = new Character(t);

		characters.Add (c);
		if (cb_CharacterCreated != null) {
			cb_CharacterCreated (c);
		}
		return c;
	}

	void CreateStaticObjectPrototypes(){
		staticObjectsPrototypes = new Dictionary<string, StaticObject> ();

		staticObjectsPrototypes.Add ("Wall_Planks",	
			StaticObject.CreatePrototype (
				"Wall_Planks", 
				0, // IMPASSABLE
				1, // width
				1,  // height
				true // links to neighbours
			));
		staticObjectsPrototypes.Add ("Door_Simple",	
			StaticObject.CreatePrototype (
				"Door_Simple", 
				2,
				1, // width
				1,  // height
				false // links to neighbours
			));
	}
		
	/// <summary>
	/// Randomizes the tiles.
	/// </summary>
	public void RandomizeTiles(){
		for (int x = 0; x < Width; x++) {
			for (int y = 0; y < Height; y++) {
				int random = UnityEngine.Random.Range (0, 3);
				if ( random == 0) {
					tiles [x, y].Type = TileType.Soil;
				} else if (random == 1) {
					tiles [x, y].Type = TileType.Sand;
				} else if (random  == 2) {
					tiles [x, y].Type = TileType.RoughStone;
				} else if (random == 3) {
					tiles [x, y].Type = TileType.Gravel;
				} else {
					Debug.LogError ("Error in setting type of tyle with random = " + random);
				}
				tiles [x, y].previousType = tiles [x, y].Type;

			}
		}
	}

	/// <summary>
	/// Gets the tile at x and y.
	/// </summary>
	/// <returns>The <see cref="Tile"/>.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public Tile GetTileAt(int x, int y) {
		if (x>Width||x<0||y>Height||y<0) {
			//Debug.LogError ("wrong coordinates for a tile" + x + " " + y);
			return null;
		}
		return tiles [x, y];
	}

	public void PlaceStaticObject (string buildModeObjectType, Tile t)
	{
		//TODO: 1 BY 1 TIles assumed change this later with no rotation;
		if (staticObjectsPrototypes.ContainsKey(buildModeObjectType) == false) {
			Debug.LogError("doesn`t contains key " + buildModeObjectType);
			return;
		}
		StaticObject obj = StaticObject.PlaceInstance (staticObjectsPrototypes [buildModeObjectType],t);

		if (obj == null) {
			//failed to place obj
			return;
		}

		if (cb_StaticObjectCreated != null) {
			cb_StaticObjectCreated (obj);
			InvalidatePathfindingGraph ();
		}
	}

	public void RegisterStaticObjectCreated(Action<StaticObject> callback){
		cb_StaticObjectCreated += callback;
	}

	public void UnregisterStaticObjectCreated(Action<StaticObject> callback){
		cb_StaticObjectCreated -= callback;
	}

	public void RegisterTileChanged(Action<Tile> callback){
		cb_TileChanged += callback;
	}

	public void UnregisterTileChanged(Action<Tile> callback){
		cb_TileChanged -= callback;
	}

	public void RegisterCharacterCreated(Action<Character> callback){
		cb_CharacterCreated += callback;
	}

	public void UnregisterCharacterCreated(Action<Character> callback){
		cb_CharacterCreated -= callback;
	}

	// called when any tile changes
	void OnTileChanged(Tile t){
		if (cb_TileChanged == null)
			return;
		cb_TileChanged (t);
		InvalidatePathfindingGraph ();
	}

	public void InvalidatePathfindingGraph()
	{
		PathfindingGraph = null;
	}

	public bool IsStaticObjectPlacementValid(string objType, Tile t){
		return staticObjectsPrototypes [objType].IsValidPosition(t);
	}
}
