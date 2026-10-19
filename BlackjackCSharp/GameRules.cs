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

        // Give players initial cards
        foreach (var player in Game.s_players)
        {
            Console.WriteLine();
            player.Hit(Game.s_deck);
            player.Hit(Game.s_deck);

            // Change the value of an ace if the player would otherwise bust (11 + 11 > 21)
            Bust(player);

            // Print player's hand
            player.PrintHand();

            // Check if player got Blackjack (11 + 10 == 21)
            Blackjack(player);
        }
    }

    public void PlayerTurn(Player player)
    {
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
    
    public void WinCheck(Player player)
    {
        //  Simple variable names to improve readability
        var dealer = Game.s_dealer;
    
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
            person.HasStood = true;

            // End round
            CheckEndRound();

            return true;
        }

        return false;
    }

    public void CheckEndRound()
    {
        foreach (var player in Game.s_players)
        {
            if (player.HasStood)
            {
                Game.s_gameActive = false;
            }
        }
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
                person.HasStood = true;
                CheckEndRound();

                return true;
            }
        }

        return false;
    }
    //
    // public bool AskToPlayAgain()
    // {
    //     Console.WriteLine();
    //     Console.WriteLine("Would you like to play again? (y/n)");
    //     string answer = Console.ReadLine() ?? "";
    //     if (answer.ToLower() == "y")
    //     {
    //         return true;
    //     }
    //     else if (answer.ToLower() == "n")
    //     {
    //         return false;
    //     }
    //     else
    //     {
    //         Console.WriteLine("Invalid input. Please enter 'y' to play again or 'n' to exit.");
    //         return AskToPlayAgain();
    //     }
    // }
    //
    // public void ResetHands()
    // {
    //     Game.s_dealer.ClearHand();
    //     Game.s_player.ClearHand();
    // }
    //
    // public void ResetCardDeck()
    // {
    //     if (Game.s_deck.CutCardIsFound)
    //     {
    //         Game.s_deck = new CardDeck();
    //     }
    // }

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

    public void SetAmountOfPlayers()
    {
        string answer;

        do
        {
            Console.Clear();
            Console.WriteLine("How many players?(1-4)");
            answer = Console.ReadLine() ?? "";

            if (int.TryParse(answer, out var amountOfPlayers))
            {
                if (amountOfPlayers > 0 && amountOfPlayers < 5)
                {
                    for (var i = 0; i < amountOfPlayers; i++)
                    {
                        Console.WriteLine($"\nWhat should be Player {i + 1}'s name?");
                        var playerName = Console.ReadLine();

                        if (string.IsNullOrEmpty(playerName))
                        {
                            playerName = $"Player {i + 1}";
                        }

                        Game.s_players.Add(new Player(playerName));
                    }
                }
                else
                {
                    Console.WriteLine("Invalid number. Please enter 1-4 to continue.");
                    answer = "";
                    Thread.Sleep(2900);
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter 1-4 to continue.");
                Thread.Sleep(2900);
            }
        } while (string.IsNullOrEmpty(answer));
    }
}