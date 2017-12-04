using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour {
	Dictionary<string, AudioClip> sounds;
	bool wasInitialized = false;


	float soundCoolDown = 0;

	// Use this for initialization
	void Start () {
		LoadSounds ();
		WorldController.Instance.World.RegisterStaticObjectCreated (OnStaticObjectCreated);	

		WorldController.Instance.World.RegisterTileChanged (OnTileChanged);
	}
	
	// Update is called once per frame
	void Update () {
		soundCoolDown -= Time.deltaTime;

	}

	void LoadSounds(){
		sounds = new Dictionary<string, AudioClip> ();
		var soundsRes = Resources.LoadAll<AudioClip>("Sounds");
		foreach (AudioClip s in soundsRes) {
			sounds [s.name] = s;
		}
	}

	void OnTileChanged(Tile tile_Data){
		//fix me 
		if (soundCoolDown >0) {
			return;
		}
		AudioClip ac = sounds["Place_Tile"];
		AudioSource.PlayClipAtPoint (ac,Camera.main.transform.position);
		soundCoolDown = 0.1f;
	}

	public void OnStaticObjectCreated(StaticObject obj){
		if (soundCoolDown >0) {
			return;
		}
		AudioClip ac = sounds["Place_Wall"];
		AudioSource.PlayClipAtPoint (ac,Camera.main.transform.position);
		soundCoolDown = 0.1f;
	}
}
