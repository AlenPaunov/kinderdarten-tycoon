using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml.Serialization;
using System.Xml;

public class World :IXmlSerializable {
	//out tile data
	Tile[,] tiles;
	List<Character> characters;
	public List<StaticObject> staticObjects;
	public Path_TileGraph PathfindingGraph { get; set;}

	// The static objects prototypes.
	Dictionary<string,StaticObject> staticObjectsPrototypes;

	public int Width{ get; protected set; }
	public int Height{ get; protected set; }

	Action<StaticObject> cb_StaticObjectCreated; //cbFurnitureCreated
	Action<Character> cb_CharacterCreated;
	Action<Tile> cb_TileChanged;

	public JobQueue jobQueue;

	/// <summary>
	/// Initializes a new instance of the World class.
	/// </summary>
	/// <param name="width">Width.</param>
	/// <param name="height">Height.</param>
	public World(int width, int height){

		SetupWorld (width, height);

	}

	void SetupWorld(int width, int height){

		jobQueue = new JobQueue();

		Width = width;
		Height = height;

		tiles = new Tile[width, height];
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				tiles [x, y] = new Tile (this, x, y);
				tiles [x, y].RegisterChangedCallBack (OnTileChanged);
			}
		}

		CreateStaticObjectPrototypes ();

		characters = new List<Character> ();
		staticObjects = new List<StaticObject> ();

	}

	public void Update(float deltaTime){
		foreach (Character c in characters) {
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
		if (x>=Width||x<0||y>=Height||y<0) {
			//Debug.LogError ("wrong coordinates for a tile" + x + " " + y);
			return null;
		}
		return tiles [x, y];
	}

	public void PlaceStaticObject (string objectType, Tile t)
	{
		//TODO: 1 BY 1 TIles assumed change this later with no rotation;
		if (staticObjectsPrototypes.ContainsKey(objectType) == false) {
			Debug.LogError (" no such object prototype " + objectType);
			return;
		}

		StaticObject staticObject = StaticObject.PlaceInstance (staticObjectsPrototypes [objectType],t);

		if (staticObject == null) {
			//failed to place obj
			return;
		}
		staticObjects.Add (staticObject);

		if (cb_StaticObjectCreated != null) {
			cb_StaticObjectCreated (staticObject);
			InvalidatePathfindingGraph ();
		}
	}

	public void RegisterStaticObjectCreated(Action<StaticObject> callback){
		cb_StaticObjectCreated += callback;
	}

	public void UnregisterStaticObjectCreated(Action<StaticObject> callback){
		cb_StaticObjectCreated -= callback;
	}

	public void RegisterCharacterCreated(Action<Character> callback){
		cb_CharacterCreated += callback;
	}

	public void UnregisterCharacterCreated(Action<Character> callback){
		cb_CharacterCreated -= callback;
	}

	public void RegisterTileChanged(Action<Tile> callback){
		cb_TileChanged += callback;
	}

	public void UnregisterTileChanged(Action<Tile> callback){
		cb_TileChanged -= callback;
	}
		
	// called when any tile changes
	void OnTileChanged(Tile t){

		if (cb_TileChanged == null) {
			return;
		}
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

	public StaticObject GetStaticObjectPrototype(string objType){
		if (staticObjectsPrototypes.ContainsKey(objType) == false) {
			return null;
		}

		return staticObjectsPrototypes [objType];
	}

	///////////////////////////////////////////////////////
	/// 
	///		SAVING AND LOADING
	///  
	///////////////////////////////////////////////////////

	public World(){
		
	}


	#region IXmlSerializable implementation
	public System.Xml.Schema.XmlSchema GetSchema ()
	{
		return null;
	}

	public void WriteXml (System.Xml.XmlWriter writer)
	{
		writer.WriteAttributeString ("Width", Width.ToString());
		writer.WriteAttributeString ("Height", Height.ToString());

		writer.WriteStartElement ("Tiles");
		for (int x = 0; x < Width; x++) {
			for (int y = 0; y < Height; y++) {
				writer.WriteStartElement ("Tile");
				tiles [x, y].WriteXml (writer);
				writer.WriteEndElement();
			}
		}
		writer.WriteEndElement();

		writer.WriteStartElement("StaticObjects");
		foreach(StaticObject staticObject in staticObjects) {
			writer.WriteStartElement("StaticObject");
			staticObject.WriteXml(writer);
			writer.WriteEndElement();

		}
		writer.WriteEndElement();

		writer.WriteStartElement("Characters");
		foreach(Character c in characters) {
			writer.WriteStartElement("Character");
			c.WriteXml(writer);
			writer.WriteEndElement();

		}
		writer.WriteEndElement();

	}

	public void ReadXml (System.Xml.XmlReader reader)
	{
		reader.MoveToAttribute ("Width");
		Width = reader.ReadContentAsInt();
		reader.MoveToAttribute ("Height");
		Height = reader.ReadContentAsInt();
		reader.MoveToElement ();

		SetupWorld (Width, Height);

		while (reader.Read()) {
			switch (reader.Name) {
			case "Tiles":
				ReadXML_Tiles (reader);
				break;
			case "StaticObjects":
				ReadXml_StaticObjects(reader);
				break;
			case "Characters":
				ReadXml_Characters(reader);
				break;
			}
		}

	}

	void ReadXML_Tiles (XmlReader reader){

		while (reader.Read()) {
			if (reader.Name != "Tile") {
				return;
			}
					
			int x = int.Parse (reader.GetAttribute ("X"));
			int y = int.Parse (reader.GetAttribute ("Y"));
			tiles [x, y].ReadXml (reader);
		}
			
	}

	void ReadXml_Characters(XmlReader reader) {
		Debug.Log("ReadXml_Characters");
		while(reader.Read()) {
			if(reader.Name != "Character")
				return;

			int x = int.Parse( reader.GetAttribute("X") );
			int y = int.Parse( reader.GetAttribute("Y") );

			Character c = CreateCharacter( tiles[x,y] );
			c.ReadXml(reader);
		}
	}

	void ReadXml_StaticObjects(XmlReader reader) {
		Debug.Log("ReadXml_StaticObjects");
		while(reader.Read()) {
			if(reader.Name != "StaticObject")
				return;

			int x = int.Parse( reader.GetAttribute("X") );
			int y = int.Parse( reader.GetAttribute("Y") );

			//StaticObject staticObject = PlaceStaticObject( reader.GetAttribute("objectType"), tiles[x,y] );
			//staticObject.ReadXml(reader);
		}

	}
	#endregion
}

