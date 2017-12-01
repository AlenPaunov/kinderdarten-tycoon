using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum TileType {Soil, Gravel, RoughStone, Sand, Floor};
public class Tile{

	TileType type = TileType.Soil;
	public TileType previousType;

	/// <summary>
	/// The callback for tile type changed.
	/// </summary>
	Action<Tile> cb_TileTypeChanged;

	public TileType Type {
		get {
			return type;
		}
		set {
			type = value;
			//call back on type change
			if (cb_TileTypeChanged != null) {
				cb_TileTypeChanged (this);
			}
		}
	}

	DynamicObject dynamicObject;
	public StaticObject staticObject{ get; set; }

	World world;
	public int X{ get; protected set;}
	public int Y{ get; protected set;}
	public int Z{ get; protected set;}

	public Tile(World world, int x, int y){
		this.world = world;
		this.X = x;
		this.Y = y;
		this.Z = 0;
	
	}

	/// <summary>
	/// Registers the type changed call back.
	/// </summary>
	/// <param name="callback">Callback.</param>
	public void RegisterTypeChangedCallBack(Action<Tile> callback){
	
		cb_TileTypeChanged += callback;
	}

	/// <summary>
	/// Unregisters the type changed call back.
	/// </summary>
	/// <param name="callback">Callback.</param>
	public void UnregisterTypeChangedCallBack(Action<Tile> callback){
		cb_TileTypeChanged -= callback;
	}

	/// <summary>
	/// Determines whether the tile is neighbour of the current tile.
	/// </summary>
	/// <returns><c>true</c> if this instance is neighbour of the current tile; otherwise, <c>false</c>.</returns>
	/// <param name="tile">Tile.</param>
	public bool IsNeighbour(Tile tile, bool diagonal = false){
		if (this.X==tile.X &&(this.Y==tile.Y+1||this.Y == tile.Y-1)) {
			return true;
		}
		if (this.Y==tile.Y &&(this.X==tile.X+1||this.X == tile.X-1)) {
			return true;
		}

		if (diagonal) {
			if (this.X == tile.X+1 && (this.Y == tile.Y+1||this.Y == tile.Y-1)) {
				return true;
			}
			if (this.X == tile.X-1 && (this.Y == tile.Y+1||this.Y == tile.Y-1)) {
				return true;
			}
		}
		return false;
	}

	public bool PlaceObject(StaticObject objInstance){
		if (objInstance == null) {
			//WE are uninstalling
			staticObject = null;
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
		

}
