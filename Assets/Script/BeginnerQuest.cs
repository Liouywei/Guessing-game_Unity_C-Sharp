using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BeginnerQuest : MonoBehaviour {
	//此程式碼為新手任務，第一次遊戲時有足夠時間掌握操作資訊
	//因此沒有時間限制，意即未啟用[時間條]

	[SerializeField]
	private GameManager gm;

	[SerializeField]
	private EventTrigger btn1;  //剪刀
	[SerializeField]
	private EventTrigger btn2;  //石頭
	[SerializeField]
	private EventTrigger btn3;  //布

	[SerializeField]
	private IChose p_cho;
	[SerializeField]
	private IChose a_cho;

	EventTrigger.Entry entry = new EventTrigger.Entry();

	void Start()
	{
		//1.打開猜拳 Button 的 GameObject
		AddNewEventToButton();
	}

	void AddNewEventToButton()
	{
		entry.eventID = EventTriggerType.PointerClick;   //宣告是哪一種事件類型
		entry.callback.AddListener((BaseEventData) => { //添加事件
			ClickButton();
		});
		btn1.triggers.Add (entry);
		btn2.triggers.Add (entry);
		btn3.triggers.Add (entry);

	}

	void DeleEventToButton()
	{
		btn1.triggers.Remove(entry);
		btn2.triggers.Remove(entry);
		btn3.triggers.Remove(entry);
	}

	//玩家選擇完出拳，直接連接到GameManager執行判斷勝負的部分
	void ClickButton()
	{
		gm.OnButtonUI ();
		gm.GetTheAnswer ();
		gm.StartCoroutine ("BattleAnim");
		DeleEventToButton ();
		this.enabled = false;
	}
}
