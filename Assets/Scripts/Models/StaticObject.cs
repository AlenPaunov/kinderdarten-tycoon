using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StaticObject{

	public Tile Tile{ get; protected set; } //represents the base tile beneth the object;

	//queried by the visuall sys to know what sprite to render
	public string ObjectType{get; protected set;}
	//if movementcost = 0 - impassable;
	float movementCost = 1f;

	// a table might be 3 by 3 area visually, but occupy 3x5 really
	int width = 1;
	int height = 1;

	Action<StaticObject> cb_OnChanged;

	protected StaticObject(){
	}

	static public StaticObject CreatePrototype(string objectType, float movementCost, int width, int height)
	{
		StaticObject obj = new StaticObject ();
		obj.height = height;
		obj.width = width;
		obj.movementCost = movementCost;
		obj.ObjectType = objectType;

		return obj;
	}

	static public StaticObject PlaceInstance (StaticObject protoObject, Tile tile)
	{
		StaticObject obj = new StaticObject ();
		obj.height = protoObject.height;
		obj.width = protoObject.width;
		obj.movementCost = protoObject.movementCost;
		obj.ObjectType = protoObject.ObjectType;
		obj.Tile = tile;

		// FIXME
		if (tile.PlaceObject (obj) == false) {
			// FOR SOME REASON WE CANT PLACE AN OBJECT THERE
			return null;
		}
		return obj;
	}

	public void RegisterOnChangedCallBack(Action<StaticObject> callback){
		cb_OnChanged += callback;
	}

	public void UnregisterOnChangedCallBack(Action<StaticObject> callback){
		cb_OnChanged -= callback;
	}


}
