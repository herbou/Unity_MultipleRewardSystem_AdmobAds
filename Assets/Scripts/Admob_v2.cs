using UnityEngine;
using GoogleMobileAds.Api;
using System;

//New Admob Reward API

public class Admob_v2 : MonoBehaviour {

	public string rewardAdID = "ca-app-pub-3940256099942544/5224354917";

	[Space]
	[SerializeField] RewardBox rewardBox;

	RewardedAd rewardAd;

	void Start ( ) {
		MobileAds.Initialize ( initStatus=>{} );
	}

	//Request Reward Ad
	public void RequestRewardAd ( ) {
		//Remove existing events first to avoid executing an event twice.
		if (rewardAd != null)
			RemoveRewardAdEvents();

		rewardAd = new RewardedAd( rewardAdID );

		//Attach rewardAd events 
		AddRewardAdEvents();

		rewardAd.LoadAd ( GetNewAdRequest ( ) );
	}

	//Reward events
	void RewardAd_OnUserEarnedReward ( object sender, Reward e ) {
		rewardBox.isAdWatched = true;
	}

	void RewardAd_OnAdClosed ( object sender, EventArgs e ) {
		rewardBox.AdClose ( );
	}

	void RewardAd_OnAdFailedToLoad ( object sender, AdErrorEventArgs e ) {
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
	
	void AddRewardAdEvents(){
		//Add rewardAD events
		rewardAd.OnAdLoaded += RewardAd_OnAdLoaded;
		rewardAd.OnAdFailedToLoad += RewardAd_OnAdFailedToLoad;
		rewardAd.OnAdClosed += RewardAd_OnAdClosed;
		rewardAd.OnUserEarnedReward += RewardAd_OnUserEarnedReward;
	}

	void RemoveRewardAdEvents(){
		//Remove rewardAD events
		rewardAd.OnAdLoaded -= RewardAd_OnAdLoaded;
		rewardAd.OnAdFailedToLoad -= RewardAd_OnAdFailedToLoad;
		rewardAd.OnAdClosed -= RewardAd_OnAdClosed;
		rewardAd.OnUserEarnedReward -= RewardAd_OnUserEarnedReward;
	}

	void OnDestroy ( ) {
		RewardAdEvents();
	}
}
