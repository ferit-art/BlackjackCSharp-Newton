namespace BlackjackCSharp;

public class GameRules
{
    public GameRules()
    {
    }

    public void StartGame()
    {
        Console.Clear();

        // Give dealer initial cards
        Game.s_dealer.Hit(Game.s_deck);
        Game.s_dealer.Hand.Add(Game.s_deck.DrawCard());
        Console.WriteLine($"{Game.s_dealer.Name} got a hidden card.");

        // Print dealer's hand
        Console.WriteLine($"{Game.s_dealer.Name}'s hand:  {Game.s_dealer.Hand[0]}  *Hidden card*");

        Thread.Sleep(1000); // Before switching to player

        // Give player initial cards
        Console.WriteLine();
        Console.WriteLine("enter number of players (1-4):");
        int numPlayers;
        while (!int.TryParse(Console.ReadLine(), out numPlayers) || numPlayers < 1 || numPlayers > 4)
        {
            Console.WriteLine("Invalid input. Please enter a number between 1 and 4.");
        }

        // Clear the players list and add the specified number of players
        Game.s_players.Clear();
        for (int i = 0; i < numPlayers; i++)
        {
            Game.s_players.Add(new Player($"Player {i + 1}"));
        }

        foreach (var player in Game. s_players)
        {
            player.Hit(Game.s_deck);
            player.Hit(Game.s_deck);
        }

        // Change the value of an ace if the player would otherwise bust (11 + 11 > 21)
        Bust(Game.s_players[0]);

        // Print player's hand
        Game.s_players[0].PrintHand();

        // Check if player got Blackjack (11 + 10 == 21)
        Blackjack(Game.s_players[0]); // Assuming we're checking the first player
    }

    public void PlayerTurn()
    {
        foreach (var player in Game.s_players)
        {
            Console.WriteLine();
            Console.WriteLine($"It's {player.Name}'s turn.");
            while (Game.s_gameActive && !player.HasStood)
            {
                Console.WriteLine();
                Console.WriteLine($"Would {player.Name} like to hit or stand? (h/s)");
                string input = Console.ReadLine() ?? "";
                if (input.ToLower() == "h")
                {
                    player.Hit(Game.s_deck);
                    if (!Bust(player))
                    {
                        player.PrintHand();
                    }
                }
                else if (input.ToLower() == "s")
                {
                    player.Stand();
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter 'h' to hit or 's' to stand.");
                }
            }
        }
    }
    

    public void DealerTurn()
    {
        Console.WriteLine();
        Console.WriteLine($"Dealer's hidden card is revealed, it's:  {Game.s_dealer.Hand[1].ToString()}");

        // Return method early if dealer got Blackjack
        if (Blackjack(Game.s_dealer))
        {
            return;
        }

        Console.WriteLine();
        do
        {
            if (Game.s_dealer.DealerCanDraw())
            {
                Game.s_dealer.Hit(Game.s_deck);
            }
            else
            {
                Game.s_dealer.Stand();
            }
        }
        while (!Game.s_dealer.HasStood);
        
        // Check if dealer busted
        if (!Bust(Game.s_dealer))
        {
            Game.s_dealer.PrintHand(); // Print hand if dealer didn't bust
            Console.WriteLine();
        }
    }

    public void WinCheck()
    {
        //  Simple variable names to improve readability
        var dealer = Game.s_dealer;
        var player = Game.s_players[0]; // Assuming we're checking the first player

        // Dealer win
        if (dealer.CountHandValue() > player.CountHandValue())
        {
            Console.WriteLine($"{dealer.Name}'s hand has a greater value. {dealer.Name} wins the round!");
        }
        // Draw
        else if (dealer.CountHandValue() == player.CountHandValue())
        {
            Console.WriteLine($"{dealer.Name}'s and {player.Name}'s hands have the same value. The round is a draw!");
        }
        // Player win
        else
        {
            Console.WriteLine($"{player.Name}'s hand is greater than the {dealer.Name}'s. {player.Name} wins the round!");
        }

        // End round
        Game.s_gameActive = false;
    }

    // Check if person got Blackjack (11 + 10)
    public bool Blackjack(Person person)
    {
        if (person.Hand.Any(card => card.Value == 11) && person.Hand.Any(card => card.Value == 10))
        {
            // Person win
            Thread.Sleep(1000); // Wait before win message
            Console.WriteLine();
            Console.WriteLine($"{person.Name} got Blackjack. {person.Name} wins the round!");       
        
            // End round
            Game.s_gameActive = false;

            return true;
        }

        return false;
    }

    // Check if person busted (> 21)
    public bool Bust(Person person)
    {
        while (person.CountHandValue() > 21)
        {
            // If the person has any Aces
            if (person.Hand.Any(card => card.Value == 11))
            {
                // Change the value of one Ace from 11 to 1
                int index = person.Hand.FindIndex(card => card.Value == 11);
                Card newCard = person.Hand[index];
                newCard.Value = 1;
                person.Hand[index] = newCard;
            }
            else
            {
                person.PrintHand();

                // Person lose
                Thread.Sleep(1000); // Wait before lose message
                Console.WriteLine();
                Console.WriteLine($"{person.Name} busted. {person.Name} loses the round!");
                
                // End round
                Game.s_gameActive = false;

                return true; 
            }
        }

        return false;
    }

    public bool AskToPlayAgain()
    {
        Console.WriteLine();
        Console.WriteLine("Would you like to play again? (y/n)");
        string answer = Console.ReadLine() ?? "";
        if (answer.ToLower() == "y")
        {
            return true;
        }
        else if (answer.ToLower() == "n")
        {
            return false;
        }
        else
        {
            Console.WriteLine("Invalid input. Please enter 'y' to play again or 'n' to exit.");
            return AskToPlayAgain();
        }
    }

    public void ResetHands()
    {
        Game.s_dealer.ClearHand();
        Game.s_players[0].ClearHand(); // Assuming we're resetting the first player's hand
    }

    public void ResetCardDeck()
    {
        if (Game.s_deck.CutCardIsFound)
        {
            Game.s_deck = new CardDeck();
        }
    }

    public void WelcomeMessage()
    {
        Console.WriteLine("WELCOME TO BLACKJACK");
        Console.WriteLine();
        Thread.Sleep(2700);
    }

    public void ExitMessage()
    {
        Console.WriteLine();
        Console.WriteLine("THANK YOU FOR PLAYING!"); 
    }
}