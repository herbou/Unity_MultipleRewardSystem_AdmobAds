using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
using System;

public class RewardBox : MonoBehaviour {

	public enum UserRewardType {
		Coins,
		Gems
	}

	[Serializable] public struct UserReward {
		public UserRewardType RewardType;
		public Sprite Icon;
		public int Amount;
	}

	[SerializeField] GameObject rewardBoxUICanvas;
	[SerializeField] Transform rewardsParent;
	[SerializeField] Transform rewardsCheckMarksParent;
	[SerializeField] GameObject noMoreRewardsPanel;

	[Space]
	[Header ( "Progress Bar UI" )]
	[SerializeField] Image progressBarFill;

	[Space]
	[Header ( "Remaining Ads UI & Watch Ad Button" )]
	[SerializeField] GameObject remainingAdsBadge;
	Text remainingAdsBadgeText;

	[Space]
	[SerializeField] Button watchAdButton;
	Text watchAdButtonText;
	string watchAdButtonDefaultText;

	[Space]
	[SerializeField] Text watchedAdsText;

	[Space]
	[Header ( "Coins & Gems Text UI" )]
	[SerializeField] Text coinsText;
	[SerializeField] Text gemsText;

	[Space]
	[Header ( "Rewards FX" )]
	[SerializeField] ParticleSystem coinsRewardFx;
	[SerializeField] ParticleSystem gemssRewardFx;

	[Space]
	[Header ( "Admob reference" )]
	[SerializeField] Admob admob;

	[Space]
	[Header ( "Time to wait (Minutes) before activating Rewards again" )]
	public double waitTimeToActivateRewards;

	[Space]
	[Header ( "Rewards Informations" )]
	const int TOTAL_REWARDS = 6;
	public UserReward[] userRewards = new UserReward[TOTAL_REWARDS];

	static UserReward currentReward;
	static int currentRewardIndex = 0;

	public bool isAdWatched;

	void Awake ( ) {
		// Get the remaining ads text UI element inside the remainingAdsBadge.
		remainingAdsBadgeText = remainingAdsBadge.transform.GetChild ( 0 ).GetComponent <Text> ( );

		// Get watched ads Text UI element and save the default text of the button
		watchAdButtonText = watchAdButton.transform.GetChild ( 0 ).GetComponent <Text> ( );
		watchAdButtonDefaultText = watchAdButtonText.text;
	}

	void Start ( ) {
		CheckForAvailableRewards ( );

		DrawRewardsUI ( );

		UpdateCoinsTextUI ( );
		UpdateGemsTextUI ( );

		UpdateRemainingRewardsTextUI ( );
		UpdateWatchedADsTextUI ( );
	}

	void DrawRewardsUI ( ) {
		for ( int i = currentRewardIndex; i < TOTAL_REWARDS; i++ ) {
			UserReward reward = userRewards [ i ];

			//Update UI elements
			//Reward Icon UI
			rewardsParent.GetChild ( i ).GetChild ( 1 ).GetComponent <Image> ( ).sprite = reward.Icon;
			//Reward Amount UI
			rewardsParent.GetChild ( i ).GetChild ( 2 ).GetComponent <Text> ( ).text = reward.Amount.ToString ( );
		}
	}

	public void WatchAdButtonClick ( ) {
		isAdWatched = false;
		watchAdButton.interactable = false;
		watchAdButtonText.text = "LOADING..";

		//Request & Show Ad

		#if UNITY_EDITOR
		StartCoroutine ( SimulateEditorRequestRewardAd ( ) );

		#elif UNITY_ANDROID
		admob.RequestRewardAd();

		#endif
	}

#if UNITY_EDITOR
	IEnumerator SimulateEditorRequestRewardAd ( ) {
		yield return new WaitForSeconds ( UnityEngine.Random.Range ( 0.3f, 1.3f ) );

		isAdWatched = true;
		AdClose ( );
	}
#endif

	public void AdClose ( ) {
		watchAdButtonText.text = watchAdButtonDefaultText;

		//on ad closed
		if ( isAdWatched ) {
			//User watched the full AD
			watchAdButton.interactable = false;
			currentReward = userRewards [ currentRewardIndex ];
			currentRewardIndex++;
			float progressValue = ( float )currentRewardIndex / TOTAL_REWARDS;

			progressBarFill.DOFillAmount ( progressValue, 1.5f ).OnComplete ( RewardUser );

		} else {
			//User didn't complete the AD
			watchAdButton.interactable = true;
		}
	}

	void RewardUser ( ) {
		watchAdButton.interactable = true;

		//Check Reward type
		if ( currentReward.RewardType == UserRewardType.Coins ) {
			//Coins Reward
			Debug.Log ( "<color=orange>Coins Reward : +" + currentReward.Amount + "</color>" );

			GameData.Coins += currentReward.Amount;
			UpdateCoinsTextUI ( );

			coinsRewardFx.Play ( );


		} else if ( currentReward.RewardType == UserRewardType.Gems ) {
			//Gems Reward
			Debug.Log ( "<color=green>Gems Reward : +" + currentReward.Amount + "</color>" );

			GameData.Gems += currentReward.Amount;
			UpdateGemsTextUI ( );

			gemssRewardFx.Play ( );
		}


		UpdateRemainingRewardsTextUI ( );
		UpdateWatchedADsTextUI ( );

		MarkRewardAsCheked ( currentRewardIndex - 1 );

		//Save Progress
		PlayerPrefs.SetInt ( "CurrentRewardIndex", currentRewardIndex );

		//Check if it's last Reward
		if ( currentRewardIndex == TOTAL_REWARDS ) {
			//Save current system DateTime
			PlayerPrefs.SetString ( "RewardsCompletionDateTime", DateTime.Now.ToString ( ) );
		}
	}

	void MarkRewardAsCheked ( int rewardIndex ) {
		// hide the reward & show it's corresponding check mark.
		rewardsParent.GetChild ( rewardIndex ).gameObject.SetActive ( false );
		rewardsCheckMarksParent.GetChild ( rewardIndex ).gameObject.SetActive ( true );

		//Update Progress Bar
		float progressValue = ( float )currentRewardIndex / TOTAL_REWARDS;
		progressBarFill.fillAmount = progressValue;

		//If it's the last Reward
		if ( rewardIndex == TOTAL_REWARDS - 1 ) {
			watchAdButton.interactable = false;
			remainingAdsBadge.SetActive ( false );
			noMoreRewardsPanel.SetActive ( true );

			currentRewardIndex = TOTAL_REWARDS;
		}
	}

	void CheckForAvailableRewards ( ) {
		currentRewardIndex = PlayerPrefs.GetInt ( "CurrentRewardIndex", 0 );

		//Check if it's the last Reward
		if ( currentRewardIndex == TOTAL_REWARDS ) {
			//Get saved date time
			DateTime rewardsCompletionDateTime = DateTime.Parse ( PlayerPrefs.GetString ( "RewardsCompletionDateTime", DateTime.Now.ToString ( ) ) );
			DateTime currentDateTime = DateTime.Now;

			//Get total minutes between this 2 dates
			double elapsedMinutes = (currentDateTime - rewardsCompletionDateTime).TotalMinutes;

			Debug.Log ( "Time Passed Since Last Reward: " + elapsedMinutes );

			if ( elapsedMinutes >= waitTimeToActivateRewards ) {
				// Activate Rewards again
				PlayerPrefs.SetString ( "RewardsCompletionDateTime", "" );
				PlayerPrefs.SetInt ( "CurrentRewardIndex", 0 );
				currentRewardIndex = 0;

			}else{
				// show message to the user to wait more.
				Debug.Log ("wait for "+(waitTimeToActivateRewards-elapsedMinutes)+" Minutes");
			}
		}

		//Check if already watched some ads
		if ( currentRewardIndex > 0 ) {
			for ( int i = 0; i < currentRewardIndex; i++ ) {
				MarkRewardAsCheked ( i );
			}
		}
	}

	//Watched Ads & Remaining Rewards Text UI Update
	void UpdateRemainingRewardsTextUI ( ) {
		remainingAdsBadgeText.text = (TOTAL_REWARDS - currentRewardIndex).ToString ( );
	}

	void UpdateWatchedADsTextUI ( ) {
		watchedAdsText.text = string.Format ( "{0}/{1}", currentRewardIndex, TOTAL_REWARDS );
	}

	//Coins & Gems Text UI Update
	void UpdateCoinsTextUI ( ) {
		coinsText.text = GameData.Coins.ToString ( );
	}

	void UpdateGemsTextUI ( ) {
		gemsText.text = GameData.Gems.ToString ( );
	}

	//Open & Close Reward box
	public void OpenUI ( ) {
		rewardBoxUICanvas.SetActive ( true );
	}

	public void CloseUI ( ) {
		rewardBoxUICanvas.SetActive ( false );
	}

	//Editor delete playerPrefs if you hit the Delete key
#if UNITY_EDITOR
	void Update ( ) {
		if ( Input.GetKeyUp ( KeyCode.Delete ) ) {
			PlayerPrefs.DeleteAll ( );
			Debug.Log ( "Player Prefs deleted ..." );
		}
	}
#endif


}
