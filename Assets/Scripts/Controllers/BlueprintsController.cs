using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueprintsController : MonoBehaviour {

	Dictionary<string, Sprite> blueprintsSprites;
	Dictionary<Job, GameObject> jobGameobjectMap;
	//SpriteController sc;
	// Use this for initialization
	void Start () {
		LoadBlueprints ();
		jobGameobjectMap = new Dictionary<Job, GameObject> ();
		//sc = GameObject.FindObjectOfType<SpriteController> ();
		WorldController.Instance.World.jobQueue.RegisterJobCreationCallback (OnJobCreated);
	}
	
	void OnJobCreated(Job j){

		GameObject job_go = new GameObject ();
		job_go.name = "obj_" + j.jobObjectType;
		job_go.transform.position	= new Vector3 (j.Tile.X, j.Tile.Y, -2);
		job_go.transform.SetParent (this.transform, true);

		if (jobGameobjectMap.ContainsKey(j)) {
			//requed job before done
			return;
		}

		jobGameobjectMap.Add (j, job_go);			
			
		//FIXME: assume the object must be a wall so we use harcoded wall sprite
		job_go.AddComponent<SpriteRenderer> ().sprite = GetBlueprint(j);

		j.RegisterJobCompleteCallBack (OnJobEnded);
		j.RegisterJobCancelCallBack (OnJobEnded);
	}

	void OnJobEnded(Job j){
		//completed or canceled

		//TODO delete the sprite;
		GameObject job_go= jobGameobjectMap[j];

		j.UnregisterJobCompleteCallBack(OnJobEnded);
		j.UnregisterJobCancelCallBack(OnJobEnded);

		Destroy (job_go);
	}

	void LoadBlueprints(){
		blueprintsSprites = new Dictionary<string, Sprite> ();
		Sprite [] sprites = Resources.LoadAll<Sprite> ("Blueprints");
		foreach (Sprite s in sprites) {
			blueprintsSprites[s.name] = s;
		}
	}

	Sprite GetBlueprint(Job j){
		string spriteName;
		if (j.jobObjectType.Contains("Wall")) {
			spriteName = "Wall_Blueprint";
		}
		else if (j.jobObjectType.Contains("Door")) {
			spriteName = "Door_Blueprint";
		} else {
			spriteName = "Tile_Blueprint";
		}
			
		if (spriteName == "Tile_Blueprint") {
			return blueprintsSprites [spriteName];
		} 
		spriteName += "_";
//		int x = j.Tile.X;
//		int y = j.Tile.Y;
//
//		Tile t;
//		t = WorldController.Instance.World.GetTileAt (x, y);
//		if (t != null && t.StaticObject != null && t.StaticObject.ObjectType==j.jobObjectType||t.pendingJob != null && t.pendingJob.jobObjectType == j.jobObjectType) {
//			spriteName += "N";
//		}
//		t = WorldController.Instance.World.GetTileAt (x+1, y);
//		if (t != null && t.StaticObject != null && t.StaticObject.ObjectType==j.jobObjectType||t.pendingJob != null && t.pendingJob.jobObjectType == j.jobObjectType) {
//			spriteName += "E";
//		}
//		t = WorldController.Instance.World.GetTileAt(x, y - 1);
//		if (t != null && t.StaticObject != null && t.StaticObject.ObjectType==j.jobObjectType||t.pendingJob != null && t.pendingJob.jobObjectType == j.jobObjectType) {
//			spriteName += "S";
//		}
//		t = WorldController.Instance.World.GetTileAt (x-1, y);
//		if (t != null && t.StaticObject != null && t.StaticObject.ObjectType==j.jobObjectType||t.pendingJob != null && t.pendingJob.jobObjectType == j.jobObjectType) {
//			spriteName += "W";
//		}
		return blueprintsSprites [spriteName];
	}

	Sprite GetSpriteForTile(Tile tile){
		return blueprintsSprites ["Tile_BluePrint"];
	}

	Sprite GetSpriteForStaticObject(StaticObject obj){
		string spriteName;
		if (obj.ObjectType.Contains("Wall")) {
			spriteName = "Wall_Blueprint";
		}
		else if (obj.ObjectType.Contains("Door")) {
			spriteName = "Door_Blueprint";
		} else {
			spriteName = "Tile_BluePrint";
		}


		if (obj.LinksToNeighbour == false) {
			return blueprintsSprites [spriteName];
		} 
		spriteName += "_";
		int x = obj.Tile.X;
		int y = obj.Tile.Y;

		Tile t;
		t = WorldController.Instance.World.GetTileAt (x, y + 1);
		if (t != null && t.staticObject != null && t.staticObject.ObjectType==obj.ObjectType) {
			spriteName += "N";
		}
		t = WorldController.Instance.World.GetTileAt (x+1, y);
		if (t != null && t.staticObject != null && t.staticObject.ObjectType==obj.ObjectType) {
			spriteName += "E";
		}
		t = WorldController.Instance.World.GetTileAt(x, y - 1);
		if (t != null && t.staticObject != null && t.staticObject.ObjectType==obj.ObjectType) {
			spriteName += "S";
		}
		t = WorldController.Instance.World.GetTileAt (x-1, y);
		if (t != null && t.staticObject != null && t.staticObject.ObjectType==obj.ObjectType) {
			spriteName += "W";
		}

		return blueprintsSprites [spriteName];
	}
}
