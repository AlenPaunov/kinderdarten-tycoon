﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System;
using System.Xml.Serialization;

public enum TileType
{
	Floor,
	Soil,
	Gravel,
	RoughStone,
	Sand,
	Water}
;

public class Tile :IXmlSerializable
{

	TileType _type = TileType.Soil;
	public TileType previousType;

	public TileType Type {
		get {
			return _type;
		}
		set {
			TileType oldType = _type;
			_type = value;

			//call back on type change
			if (cb_TileChanged != null && oldType != _type) {
				cb_TileChanged (this);
			}
		}
	}

	public float movementCost {
		get{
			if (Type == TileType.Water) {
				return 0;
			}

			if (this.staticObject == null) {
				return 1;
			}

			return 1 * staticObject.movementCost;
		}
	}

	public DynamicObject DynamicObject { get; protected set; }

	public StaticObject staticObject{ get; protected set; }

	public Job pendingJob;
	/// <summary>
	/// The callback for tile type changed.
	/// </summary>
	Action<Tile> cb_TileChanged;



	public World World{ get; protected set; }

	public int X{ get; protected set; }

	public int Y{ get; protected set; }

	public int Z{ get; protected set; }

	public Tile (World world, int x, int y)
	{
		this.World = world;
		this.X = x;
		this.Y = y;
		this.Z = 0;
	
	}

	/// <summary>
	/// Registers the type changed call back.
	/// </summary>
	/// <param name="callback">Callback.</param>
	public void RegisterChangedCallBack (Action<Tile> callback)
	{
	
		cb_TileChanged += callback;
	}

	/// <summary>
	/// Unregisters the type changed call back.
	/// </summary>
	/// <param name="callback">Callback.</param>
	public void UnregisterChangedCallBack (Action<Tile> callback)
	{
		cb_TileChanged -= callback;
	}


	public Tile [] GetNeighbours (bool diagonal = false){
		Tile[] ns;
		if (diagonal == false) {
			ns = new Tile[4]; // N E S W 
		} else {
			ns = new Tile[8]; // N E S W  NE ES SW WN
		}
		Tile n;

		n = World.GetTileAt (X, Y + 1); //N
		ns [0] = n;
		n = World.GetTileAt (X+1, Y); // E
		ns [1] = n;
		n = World.GetTileAt (X, Y - 1); //S
		ns [2] = n;
		n = World.GetTileAt (X - 1 , Y); //W
		ns [3] = n;

		if (diagonal == true) {
			n = World.GetTileAt (X + 1, Y + 1); //NE
			ns [4] = n;
			n = World.GetTileAt (X + 1, Y - 1); // ES
			ns [5] = n;
			n = World.GetTileAt (X - 1, Y - 1); //SW
			ns [6] = n;
			n = World.GetTileAt (X - 1, Y + 1); //WN
			ns [7] = n;
		}

		return ns;
	}
	/// <summary>
	/// Determines whether the tile is neighbour of the current tile.
	/// </summary>
	/// <returns><c>true</c> if this instance is neighbour of the current tile; otherwise, <c>false</c>.</returns>
	/// <param name="tile">Tile.</param>
	public bool IsNeighbour (Tile tile, bool diagonal = false)
	{
		//same col
		if (this.X == tile.X && Mathf.Abs (this.Y - tile.Y) == 1) {
			return true;
		}

		//same row
		if (this.Y == tile.Y && Mathf.Abs (this.X - tile.X) == 1) {
			return true;
		}
			
		//diagonals
		if (diagonal) {
			if (this.X == tile.X + 1 && (this.Y == tile.Y + 1 || this.Y == tile.Y - 1)) {
				return true;
			}
			if (this.X == tile.X - 1 && (this.Y == tile.Y + 1 || this.Y == tile.Y - 1)) {
				return true;
			}
		}
		return false;
	}

	public bool PlaceObject (StaticObject objInstance)
	{
		if (objInstance == null) {
			//WE are uninstalling
			this.staticObject = null;
			return true;
		}

		if (staticObject != null) {
			Debug.LogError ("tryig to install on installed static object");
			return false;
		} 
		else {
			staticObject = objInstance;
			return true;
		}
	}
		

	#region IXmlSerializable implementation
	public System.Xml.Schema.XmlSchema GetSchema ()
	{
		return null;
	}

	public void WriteXml (System.Xml.XmlWriter writer)
	{
		writer.WriteAttributeString("X",X.ToString());
		writer.WriteAttributeString ("Y", Y.ToString());
		writer.WriteAttributeString ("Type", ((int)Type).ToString());
	}

	public void ReadXml (System.Xml.XmlReader reader)
	{
		Type = (TileType)int.Parse (reader.GetAttribute ("Type"));
	}

	#endregion
}
