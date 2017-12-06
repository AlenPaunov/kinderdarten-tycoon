using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StaticObject{

	public Tile Tile{ get; protected set; } //represents the base tile beneth the object;

	//queried by the visuall sys to know what sprite to render
	public string ObjectType{get; protected set;}
	//if movementcost = 0 - impassable;
	public float movementCost {get; protected set;}

	// a table might be 3 by 3 area visually, but occupy 3x5 really
	int width = 1;
	int height = 1;

	public bool LinksToNeighbour{ get; protected set;}

	Action<StaticObject> cb_OnChanged;

	public Func<Tile, bool> funcPositionValidation;


	protected StaticObject(){
	}

	static public StaticObject CreatePrototype(string objectType, float movementCost, int width, int height, bool linksToNeighbour = false)
	{
		StaticObject obj = new StaticObject ();
		obj.height = height;
		obj.width = width;
		obj.movementCost = movementCost;
		obj.ObjectType = objectType;
		obj.LinksToNeighbour = linksToNeighbour;

		obj.funcPositionValidation = obj.IsValidPosition;

		return obj;
	}

	static public StaticObject PlaceInstance (StaticObject protoObject, Tile tile)
	{
		StaticObject obj = new StaticObject ();
		obj.height = protoObject.height;
		obj.width = protoObject.width;
		obj.movementCost = protoObject.movementCost;
		obj.ObjectType = protoObject.ObjectType;
		obj.LinksToNeighbour = protoObject.LinksToNeighbour;

		obj.Tile = tile;

		// FIXME
		if (tile.PlaceObject (obj) == false) {
			// FOR SOME REASON WE CANT PLACE AN OBJECT THERE
			// already occupied; => return null
			// FIX ME -> GARBAGE PRODUCTION
			return null;
		}

		if (obj.LinksToNeighbour) {
			//this object links to neighbour so change neighbour

			int x = obj.Tile.X;
			int y = obj.Tile.Y;
			Tile t;
			t = tile.World.GetTileAt (x, y + 1);
			if (t != null && t.StaticObject != null && t.StaticObject.ObjectType==obj.ObjectType) {
				t.StaticObject.cb_OnChanged(t.StaticObject);
			}
			t = tile.World.GetTileAt (x+1, y);
			if (t != null && t.StaticObject != null && t.StaticObject.ObjectType==obj.ObjectType) {
				t.StaticObject.cb_OnChanged(t.StaticObject);
			}
			t = tile.World.GetTileAt (x, y - 1);
			if (t != null && t.StaticObject != null && t.StaticObject.ObjectType==obj.ObjectType) {
				t.StaticObject.cb_OnChanged(t.StaticObject);
			}
			t = tile.World.GetTileAt (x-1, y);
			if (t != null && t.StaticObject != null && t.StaticObject.ObjectType==obj.ObjectType) {
				t.StaticObject.cb_OnChanged(t.StaticObject);
			}
		}
		return obj;
	}

	public void RegisterOnChangedCallBack(Action<StaticObject> callback){
		cb_OnChanged += callback;
	}

	public void UnregisterOnChangedCallBack(Action<StaticObject> callback){
		cb_OnChanged -= callback;
	}

	public bool IsValidPosition(Tile t){
		if (t.StaticObject != null) {
			return false;
		}
		return true;
	}

	public bool IsValidPosition_Door(Tile t){
		if (IsValidPosition(t)==false) {
			return false;
		}
		return true;
	}
}
