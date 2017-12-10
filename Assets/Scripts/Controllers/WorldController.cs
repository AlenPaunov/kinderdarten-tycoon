using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Xml.Serialization;
using System.IO;

public class WorldController : MonoBehaviour {
	public World World{ get; protected set;}
	public static WorldController Instance{ get; protected set;}
	static bool loadWorld = false;
	// Use this for initialization
	void OnEnable () {

		if (Instance!=null) {
			Debug.LogError ("two worlds error");
		}
			
		Instance = this;

		if (loadWorld) {
			CreateWorldFromSave ();
			loadWorld = false;
		} else {
			CreateWorld ();	
		}



	}

	void CreateWorld ()
	{
		World = new World (40,40);
		World.RandomizeTiles ();
		Camera.main.transform.position = new Vector3 (World.Width / 2, World.Height / 2, Camera.main.transform.position.z);
		Camera.main.orthographicSize = 2;
	}

	void CreateWorldFromSave ()
	{
		XmlSerializer serializer = new XmlSerializer (typeof(World));
		TextReader reader = new StringReader (PlayerPrefs.GetString("SaveGame00"));

		World = (World)serializer.Deserialize (reader);
		reader.Close();

		Debug.Log ("Game loaded");
		Camera.main.transform.position = new Vector3 (World.Width / 2, World.Height / 2, Camera.main.transform.position.z);
		Camera.main.orthographicSize = 2;
	}

	public void RestartWorld()
	{
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
		//World.RandomizeTiles ();
	}

	public void SaveWorld(){

		XmlSerializer serializer = new XmlSerializer (typeof(World));
		TextWriter writer = new StringWriter ();

		serializer.Serialize (writer, World);
		writer.Close();
		Debug.Log (writer.ToString ());

		PlayerPrefs.SetString ("SaveGame00", writer.ToString ());
			
		Debug.Log ("Game saved");

	}

	public void LoadWorld(){
		loadWorld = true;
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name); // destroy old data and ref
	}

	void Update(){
		//TODO PAUSE/UNPAUSE;	
		World.Update (Time.deltaTime);
	}
	/// <summary>
	/// Gets the tile at world coordinate.
	/// </summary>
	public Tile GetTileAtWorldCoord(Vector3 coord){
		int x = Mathf.FloorToInt (coord.x);
		int y = Mathf.FloorToInt (coord.y);

		return World.GetTileAt (x, y);
	}

}
