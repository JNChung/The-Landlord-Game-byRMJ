using System;

/// <summary>
/// 骰子
/// </summary>
public class DND_Dice
{
    public int Roll(string diceString)
    {
        string[] parts = diceString.Split('d');
        if (parts.Length != 2)
        {
            throw new ArgumentException("Invalid dice string. Expected format: NdM");
        }

        int numberOfDice = int.Parse(parts[0]);
        int diceSides = int.Parse(parts[1]);

        int result = 0;
        for (int i = 0; i < numberOfDice; i++)
        {
            result += UnityEngine.Random.Range(1, diceSides + 1);
        }

        return result;
    }
}
