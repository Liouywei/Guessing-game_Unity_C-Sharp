public interface IChose
{
	int FinalChose();
	void EndThisTime();
	int Hurt (int damage);
	int ReSet();
}

public interface IClose  //用來關閉Button
{
	void OffChoseButton();
}