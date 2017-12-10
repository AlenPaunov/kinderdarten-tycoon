using System;
using System.Collections;
using UnityEngine;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;

public class StaticObject : IXmlSerializable{

	public Tile Tile{ get; protected set; } //represents the base tile beneth the object;

	//queried by the visuall sys to know what sprite to render
	public string ObjectType{get; protected set;}

	//if movementcost = 0 - impassable;
	public float movementCost {get; protected set;}

	// a table might be 3 by 3 area visually, but occupy 3x5 really
	int width;
	int height;

	public bool LinksToNeighbour{ get; protected set;}

	Action<StaticObject> cb_OnChanged;
	Func<Tile, bool> funcPositionValidation;


	protected StaticObject(){
	}

	static public StaticObject CreatePrototype(string objectType, float movementCost = 1f, int width = 1, int height = 1, bool linksToNeighbour = false)
	{
		StaticObject obj = new StaticObject ();

		obj.ObjectType = objectType;
		obj.movementCost = movementCost;
		obj.height = height;
		obj.width = width;
		obj.LinksToNeighbour = linksToNeighbour;

		obj.funcPositionValidation = obj.IsValidPosition;

		return obj;
	}

	static public StaticObject PlaceInstance (StaticObject protoObject, Tile tile)
	{
		if (protoObject.funcPositionValidation(tile) == false) {
			return null;
		}

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
			if (t != null && t.staticObject != null && t.staticObject.ObjectType==obj.ObjectType) {
				t.staticObject.cb_OnChanged(t.staticObject);
			}
			t = tile.World.GetTileAt (x+1, y);
			if (t != null && t.staticObject != null && t.staticObject.ObjectType==obj.ObjectType) {
				t.staticObject.cb_OnChanged(t.staticObject);
			}
			t = tile.World.GetTileAt (x, y - 1);
			if (t != null && t.staticObject != null && t.staticObject.ObjectType==obj.ObjectType) {
				t.staticObject.cb_OnChanged(t.staticObject);
			}
			t = tile.World.GetTileAt (x-1, y);
			if (t != null && t.staticObject != null && t.staticObject.ObjectType==obj.ObjectType) {
				t.staticObject.cb_OnChanged(t.staticObject);
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
		if (t.staticObject != null) {
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

	#region IXmlSerializable implementation

	public XmlSchema GetSchema ()
	{
		return null;
	}

	public void WriteXml(XmlWriter writer) {
		writer.WriteAttributeString( "X", Tile.X.ToString() );
		writer.WriteAttributeString( "Y", Tile.Y.ToString() );
		writer.WriteAttributeString( "objectType", ObjectType );
		writer.WriteAttributeString( "movementCost", movementCost.ToString() );
	}

	public void ReadXml(XmlReader reader) {
		// X, Y, and objectType have already been set, and we should already
		// be assigned to a tile.  So just read extra data.
		ObjectType = reader.GetAttribute("objectType");
		movementCost = int.Parse( reader.GetAttribute("movementCost") );	
	}
	#endregion
}
