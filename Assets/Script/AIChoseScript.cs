using UnityEngine;
using System.Collections;

public class AIChoseScript : MonoBehaviour, IChose {
	[SerializeField]
	private GameObject[] aiHand = new GameObject[3];
	private int? choseId;

	[SerializeField]
	private int hpMax;
	private int hp;

	void Start()
	{
		hp = hpMax;
	}

	void RandomTheChose()
	{
		int x = Random.Range (1, 21);
		for (int i = 0; i < x; i++)
		{
			choseId = Random.Range (0, 3);
		}
	}

	public int FinalChose()
	{
		RandomTheChose ();
		aiHand[(int)choseId].gameObject.SetActive(true);

		return (int)choseId;
	}

	public void EndThisTime()  //也寫入Interface
	{
		aiHand[(int)choseId].gameObject.SetActive(false);
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
}
