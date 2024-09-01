namespace Open_TK
{
	class Program
	{
		static void Main(string[] args)
		{
			using (Game game = new Game(400	, 400))
			{
				game.Run();
			}
		}
	}
}