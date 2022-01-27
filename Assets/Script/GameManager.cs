using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class GameManager : MonoBehaviour {
	public static GameManager gm;

	//計算雙方連勝數
	private int p_combo;
	private int a_combo;
	private int damage;
	[SerializeField]
	private Text txt2; //combo數字 : x1 
	[SerializeField]
	private Animation combo_anim;
	[SerializeField]
	private RectTransform comboTxtPos;
	[SerializeField]
	private RectTransform rect_Left;
	[SerializeField]
	private RectTransform rect_right;


	//Time Variable
	[SerializeField]
	private float timer;
	private float nowTime;
	[SerializeField]
	private bool fight;    //開打
	[SerializeField]
	private Image timeLeft;
	[SerializeField]
	private Image timeRight;

	//About Chose
	private int playerChose;  //0:剪刀 1:石頭 2:布
	private int aiChose;
	private IChose[] allInterface = new IChose[2];
	private IChose p_Cho;
	private IChose a_Cho;
	private IClose p_Clo;
	[SerializeField]
	private GameObject buttonGroup;

	[Space(20)]
	[SerializeField]
	private Animator playerHand;
	[SerializeField]
	private Animator aiHand;

	[Space(20)]
	private bool? winner;  //true: player win    false: ai win
	private int winChose;  //贏家出的拳是 0 1 2 ?
	[SerializeField]
	private GameObject punchObj; //揍輸家的攻擊物件
	private Animator hitAnim;      //攻擊的動畫

	[Space(20)]
	[SerializeField]
	private Image playerHp;
	[SerializeField]
	private Image aiHp;

	[Space(20)]
	[SerializeField]
	private Animator pWim_img;
	[SerializeField]
	private Animator aWin_img;
	private Animator win;
	[SerializeField]
	private GameObject replayBTN;

	[Space(20)]
	[SerializeField]
	private AudioClip[] AudioArr;
	private AudioSource aud; //音效來源


	void Awake()
	{
		gm = this;

		aud = GetComponent<AudioSource>();
		hitAnim = punchObj.GetComponent<Animator> ();
		allInterface = GetComponents<IChose> ();
		p_Cho = allInterface [0];
		a_Cho = allInterface [1];
		p_Clo = GetComponent<IClose> ();
	}

	void Start()
	{
		nowTime = timer;
		PlaySound (0);
	}

	void Update()
	{
		if (fight) {
			TimeControl ();
			if (nowTime <= 0f) {
				TheNewRound ();
			}
		}
	}

	public void TheNewRound()
	{
		p_Clo.OffChoseButton ();
		OnButtonUI ();
		GetTheAnswer ();
		StartCoroutine ("BattleAnim");
		ReTime ();
	}

	public void OnButtonUI()
	{
		buttonGroup.SetActive (!buttonGroup.activeSelf);
	}

	public void GetTheAnswer()  //取得雙方出拳選項
	{
		playerChose = p_Cho.FinalChose();
		aiChose = a_Cho.FinalChose ();
	}

	IEnumerator BattleAnim() //出拳動畫
	{
		playerHand.SetBool ("Show", true);
		aiHand.SetBool ("Show", true);

		PlaySound (1);                        //音效
		yield return new WaitForSeconds (2f);

		//關閉雙方出的拳物件
		playerHand.SetBool ("Show", false);
		aiHand.SetBool ("Show", false);

		yield return new WaitForSeconds (1f);

		p_Cho.EndThisTime();
		a_Cho.EndThisTime ();

		//判斷勝負
		WhosWin ();
	}


	//About Time
	//-------------------------------------------
	void TimeControl()
	{
		nowTime -= Time.deltaTime;
		timeLeft.fillAmount = nowTime / timer;
		timeRight.fillAmount = nowTime / timer;
	}

	void ReTime()
	{
		nowTime = timer;
		fight = false;
	}
	//-------------------------------------------

	public void WhosWin()
	{
		if (playerChose != 2)
		{
			if (playerChose > aiChose)
			{
				winner = true;
				winChose = playerChose;
			}
			else if(aiChose == 2 && playerChose == 0)
			{
				winner = true;
				winChose = playerChose;
			}
			else if (playerChose < aiChose)
			{
				winner = false;
				winChose = aiChose;
			}
			else
			{
				winner = null;
			}
		}
		else
		{
			if (aiChose == 0)
			{
				winner = false;
				winChose = aiChose;
			}
			else if (aiChose == 1)
			{
				winner = true;
				winChose = playerChose;
			}
			else
			{
				winner = null;
			}
		}

		//執行Combo判斷
		ComboSystem ();

		if (winner != null)
			StartCoroutine ("HitLoser");
		else
			StartCoroutine ("FinishThisRound");
	}

	IEnumerator HitLoser()
	{
		Vector2 faceWay = punchObj.transform.localScale;
		IChose loser = p_Cho;
		Animator lose_anim = playerHand;
		Image lose_img = playerHp;
		float lose_hp = 0;

		if (winner!= null && winner == true) { //玩家贏  拳頭面向AI
			faceWay.x = 1;
			loser = a_Cho;
			lose_anim = aiHand;
			lose_img = aiHp;
		} else if(winner!= null && winner == false){
			faceWay.x = -1;
			loser = p_Cho;
			lose_anim = playerHand;
			lose_img = playerHp;
		}
		punchObj.transform.localScale = faceWay;

		//播攻擊動畫
		switch (winChose) {
		case 0:
			hitAnim.SetTrigger ("Ss");
			PlaySound (2);
			break;
		case 1:
			hitAnim.SetTrigger ("St");
			PlaySound (3);
			break;
		case 2:
			hitAnim.SetTrigger ("Pp");
			PlaySound (4);
			break;
		default:
			break;
		}
		yield return new WaitForSeconds(0.2f);

		lose_hp =  loser.Hurt(damage)/10f;
		lose_img.fillAmount = lose_hp; //給輸家造成傷害
		lose_anim.SetTrigger("Hurt");

		if (lose_hp < 0.1f) {
			//播放勝利動畫
			if (winner == true) {  //玩家勝利
				win = pWim_img;
				StartCoroutine(WinTheGame ());
			} else if (winner == false) {
				win = aWin_img;
				StartCoroutine(WinTheGame ());
			}
		} else {
			yield return new WaitForSeconds(0.5f);
			StartCoroutine ("FinishThisRound"); //結束這一局
		}
	}

	IEnumerator FinishThisRound()
	{
		yield return new WaitForSeconds(0.8f);
		OnButtonUI ();
		//PlaySound (0);
		fight = true;     //下一輪開始
	}

	IEnumerator WinTheGame()  //分出勝負
	{
		yield return new WaitForSeconds (1f);
		win.SetTrigger ("Win");
		yield return new WaitForSeconds (0.5f);
		replayBTN.SetActive (true);
	}

	public void CreateNewGame()
	{
		PlaySound (5);
		StartCoroutine ("ReSetData");
	}

	//建立一個Function，儲存遊戲開始時最初的狀態，以便利下一場
	IEnumerator ReSetData()
	{
		win.SetTrigger ("ReStart");
		yield return new WaitForSeconds (1.5f);

		//雙方HP
		playerHp.fillAmount = p_Cho.ReSet()/10;
		aiHp.fillAmount = a_Cho.ReSet()/10;

		//關閉Combo UI
		ComboAnimationPlay("Combo_Close");

		//ReSet damage
		p_combo = 0;
		a_combo = 0;

		yield return new WaitForSeconds (1.5f);
		OnButtonUI ();
		PlaySound (0);
		fight = true;
	}

	void PlaySound(int x)
	{
		aud.PlayOneShot(AudioArr[x]);
	}

	void ComboSystem()
	{
		if (winner == true) {
			p_combo += 1;
			a_combo = 0;
			damage = 2*p_combo;
			comboTxtPos.anchoredPosition = rect_Left.anchoredPosition;
			txt2.text = "X " + p_combo;
			ComboAnimationPlay("Combo_Show");
		} else if (winner == false) {
			p_combo = 0;
			a_combo += 1;
			damage = 2*a_combo;
			comboTxtPos.anchoredPosition = rect_right.anchoredPosition;
			txt2.text = "X " + a_combo;
			ComboAnimationPlay("Combo_Show");
		} else {
			p_combo = 0;
			a_combo = 0;
			damage = 2;
			ComboAnimationPlay("Combo_Close");
		}
	}

	void ComboAnimationPlay(string x)
	{
		combo_anim.Play (x);
	}
}
