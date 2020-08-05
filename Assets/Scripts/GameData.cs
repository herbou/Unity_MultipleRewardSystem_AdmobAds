using UnityEngine;

public static class GameData {
	private static int _coins = 0;
	private static int _gems = 0;

	// The static Constructor is the best place
	// to load already saved data in a file or in PlayerPrefs.
	static GameData ( ) {
		_coins = PlayerPrefs.GetInt ( "Coins", 0 );
		_gems = PlayerPrefs.GetInt ( "Gems", 0 );
	}


	// set{ PlayerPrefs.SetInt ( "Coins", (_coins = value) ); }
	//
	// is equivalent to :
	// 
	// set {
	//    _coins = value;
	//    PlayerPrefs.SetInt ( "Coins", _coins);
	// }
	public static int Coins {
		get{ return _coins; }
		set{ PlayerPrefs.SetInt ( "Coins", (_coins = value) ); }
	}

	public static int Gems {
		get{ return _gems; }
		set{ PlayerPrefs.SetInt ( "Gems", (_gems = value) ); }
	}
}
