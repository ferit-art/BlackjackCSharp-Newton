namespace BlackjackCSharp;

public class Game
{
    public GameRules GameRules = new();
    public static bool s_gameActive { get; set; }
    public static CardDeck s_deck { get; set; }
    public static Dealer s_dealer { get; set; }
    public static List<Player> s_players = new List<Player>();

    public Game()
    {
        s_gameActive = true;
        s_deck = new CardDeck();
        s_dealer = new Dealer();
        s_players.Add(new Player("Player"));
        GameRules.WelcomeMessage();
        LoopGame();
    }

    public void LoopGame()
    {
        do
        {
            GameRules.StartGame();
            if (s_gameActive)
            {
                Thread.Sleep(1000);
                GameRules.PlayerTurn();

                if (s_gameActive)
                {
                    Thread.Sleep(1000);
                    GameRules.DealerTurn();

                    if (s_gameActive)
                    {
                        Thread.Sleep(1000);
                        GameRules.WinCheck();
                    }
                }
            }

            Thread.Sleep(1000);
            
            if (GameRules.AskToPlayAgain())
            {
                Console.Clear();
                s_gameActive = true;
                GameRules.ResetHands();
                GameRules.ResetCardDeck();
            }
            else
            {
                GameRules.ExitMessage();
                Thread.Sleep(3500);
            }
        }
        while (s_gameActive);
    }
}
