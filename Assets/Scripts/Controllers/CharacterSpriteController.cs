using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpriteController : MonoBehaviour {

	public GameObject charPrefab;
	Dictionary<string, Sprite> charactersSprites;

	Dictionary<Character, GameObject> charactersGameobjectMap;
	World world{
		get { return WorldController.Instance.World;}
	}
	// Use this for initialization
	void Start () {
		LoadSprites ();

		charactersGameobjectMap = new Dictionary<Character, GameObject> ();

		world.RegisterCharacterCreated (OnCharacterCreated);

		//Debug
		Character c = world.CreateCharacter (world.GetTileAt (world.Height / 2, world.Width / 2));
	}

	void LoadSprites(){
		charactersSprites = new Dictionary<string, Sprite> ();
		Sprite [] sprites = Resources.LoadAll<Sprite> ("Characters");
		foreach (Sprite s in sprites) {
			charactersSprites[s.name] = s;
		}
	}

	public void OnCharacterCreated(Character c){
		GameObject c_go = Instantiate(charPrefab, new Vector3 (c.currTile.X, c.currTile.Y, -5), Quaternion.identity) as GameObject;
		c_go.name = "character";
		charactersGameobjectMap.Add (c, c_go);	

		c_go.transform.SetParent (this.transform, true);


		//FIXME: assume the object must be a wall so we use harcoded wall sprite
		//c_go.AddComponent<SpriteRenderer> ().sprite = GetSprite(c);
		c.RegisterCharacterChangedCallback(OnCharacterChanged);
		world.RegisterCharacterCreated (OnCharacterChanged);
	}

	public void OnCharacterChanged(Character c){
		// not implemented
		if (charactersGameobjectMap.ContainsKey(c)==false) {
			Debug.LogError ("missing object");
			return;
		}

		GameObject c_go = charactersGameobjectMap[c];
		//obj_go.GetComponent<SpriteRenderer> ().sprite = GetSprite (c);
		c_go.transform.position = new Vector3(c.X, c.Y, -5);
	}

	Sprite GetSprite(Character c){
		return null;
	}
}
