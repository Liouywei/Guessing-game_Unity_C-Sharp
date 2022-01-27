using UnityEngine;
using System.Collections;

public class ChoseScript : MonoBehaviour, IChose, IClose{
	[SerializeField]
	private GameObject choseImg;  //表示玩家目前點選的選項
	private int? choseId;         //0:剪刀 1:石頭 2:布
	[SerializeField]
	private GameObject[] Playerhand = new GameObject[3];

	[SerializeField]
	private int hpMax;
	private int hp;

	void Start()
	{
		hp = hpMax;
	}

	public void MoveToChoseImage(Transform m_pos) //黃色背景圖片移動到被點選的拳UI位置
	{
		choseImg.SetActive (true);
		choseImg.transform.position = new Vector2 (m_pos.position.x, m_pos.position.y);
		choseId = int.Parse(m_pos.name);

		GameManager.gm.TheNewRound ();
	}

	public int FinalChose()
	{
		if (choseId == null)
			RandomChose ();

		Playerhand [(int)choseId].SetActive (true);

		return (int)choseId;
	}

	void RandomChose()  //如果玩家未選擇出拳，亂數出拳
	{
		int x = Random.Range (0, 3);
		choseId = x;
	}

	public void EndThisTime()  //也寫入Interface
	{
		Playerhand[(int)choseId].gameObject.SetActive(false);
		choseId = null;
	}

	public int Hurt(int damage)  //也寫入Interface
	{
		hp -= damage;

		return hp;
	}
		
	public int ReSet()
	{
		hp = hpMax;
		return hp;
	}

	public void OffChoseButton()
	{
		choseImg.SetActive (false);
	}
}
