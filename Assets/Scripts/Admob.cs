using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class Admob : MonoBehaviour {

	public string appID = "ca-app-pub-3940256099942544~3347511713";
	public string rewardAdID = "ca-app-pub-3940256099942544/5224354917";

	[Space]
	[SerializeField] RewardBox rewardBox;

	RewardBasedVideoAd rewardAd;

	void Start ( ) {
		MobileAds.Initialize ( appID );

		rewardAd = RewardBasedVideoAd.Instance;

		//Attach rewardAd events
		rewardAd.OnAdLoaded += RewardAd_OnAdLoaded;
		rewardAd.OnAdFailedToLoad += RewardAd_OnAdFailedToLoad;
		rewardAd.OnAdClosed += RewardAd_OnAdClosed;
		rewardAd.OnAdRewarded += RewardAd_OnAdRewarded;
	}

	//Request Reward Ad
	public void RequestRewardAd ( ) {
		rewardAd.LoadAd ( GetNewAdRequest ( ), rewardAdID );
	}

	//Reward events
	void RewardAd_OnAdRewarded ( object sender, Reward e ) {
		rewardBox.isAdWatched = true;
	}

	void RewardAd_OnAdClosed ( object sender, EventArgs e ) {
		rewardBox.AdClose ( );
	}

	void RewardAd_OnAdFailedToLoad ( object sender, AdFailedToLoadEventArgs e ) {
		rewardBox.AdClose ( );
	}

	void RewardAd_OnAdLoaded ( object sender, EventArgs e ) {
		//Show the Ad
		rewardAd.Show ( );
	}

	// General Methods
	AdRequest GetNewAdRequest ( ) {
		return new AdRequest.Builder ( ).Build ( );
	}

	void OnDestroy ( ) {
		//Dettach rewardAD events
		rewardAd.OnAdLoaded -= RewardAd_OnAdLoaded;
		rewardAd.OnAdFailedToLoad -= RewardAd_OnAdFailedToLoad;
		rewardAd.OnAdClosed -= RewardAd_OnAdClosed;
		rewardAd.OnAdRewarded -= RewardAd_OnAdRewarded;
	}
}
