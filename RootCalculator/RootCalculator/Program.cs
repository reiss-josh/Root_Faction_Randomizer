using System;
using System.Collections.Generic;

//Root Player Randomizer
class Program
{
    public int m_resultInd = 0;
    public static bool m_chooseRandom = true; //decides whether to make a choice for you
    public static int[] m_PlayerReach = { 0, 17, 18, 21, 25, 28, 0, 0, 0};
    public static (string, int)[] m_FactionDict = new (string fName, int fReach)[]
    {
        ( "Marquise de Cat", 10 ),
        ( "Eyrie Dynasty", 8 ),
        ( "Underground Duchy", 7 ),
        ( "Vagabond (First)", 5 ),
        ( "Riverfolk Company", 5 ),
        ( "Woodland Alliance", 3 ),
        ( "Corvid Conspiracy", 3 ),
        ( "Vagabond (Second)", 2 ),
        ( "Lizard Cult", 2 )
    };

    //Driver code
    static void Main(string[] args)
    {
        Console.Write("Enter a number of players - ");
        int playerNum = int.Parse(Console.ReadLine());
        int desiredReach = m_PlayerReach[playerNum - 1];

        List<List<(string, int)>> result = GeneratePossibilities(0, playerNum, m_FactionDict, new List<(string, int)>(), new List<List<(string, int)>>());
        List<(string[], int)> finalResult = CleanupEntries(result, desiredReach);

        if (!m_chooseRandom)
            RootWriter(finalResult);
        else
        {
            var rng = new Random();
            int chosenInd = rng.Next(0, finalResult.Count);
            (string[], int) chosenResult = finalResult[chosenInd];
            rng.Shuffle(chosenResult.Item1);
            RootWriter(new List<(string[], int)> {chosenResult});
        }
    }

    //Generate all combinations of characters
    static List<List<(string, int)>> GeneratePossibilities(int ind, int desiredDepth, (string,int)[] inputArr, List<(string,int)> currEntry, List<List<(string, int)>>currList)
    {
        if (ind >= inputArr.Length)
            { return currList; }
        if ((inputArr.Length - ind + currEntry.Count) < desiredDepth)
            { return currList; }
        List<(string, int)> newEntry = new List<(string,int)>(currEntry); //need to deep copy this
        newEntry.Add(inputArr[ind]);
        if(newEntry.Count >= desiredDepth)
        {
            currList.Add(newEntry);
            return GeneratePossibilities(ind + 1, desiredDepth, inputArr, currEntry, currList);
        }
        List<List<(string, int)>> recurseDepth = GeneratePossibilities(ind+1, desiredDepth, inputArr, newEntry, currList);
        return GeneratePossibilities(ind+1, desiredDepth, inputArr, currEntry, recurseDepth);
    }

    //Make sure the combinations are Root-Legal
    static List<(string[],int)> CleanupEntries (List<List<(string,int)>> resultList, int desiredReach)
    {
        List<(string[], int)> cleanedUp = new List<(string[], int)>();
        foreach (List<(string, int)> currEntry in resultList)
        {
            (string[], int) newEntry = ConsolidateEntry(currEntry);
            if (newEntry.Item2 >= desiredReach)
                { cleanedUp.Add(ConsolidateEntry(currEntry)); }
        }
        return cleanedUp;
    }

    //Helper method for Cleanup
    static (string[],int) ConsolidateEntry (List<(string,int)> givenEntry)
    {
        int ind = 0;
        int resultInt = 0;
        bool vagabondFlag = false;
        string[] resultString = new string[givenEntry.Count];
        foreach((string,int) item in givenEntry)
        {
            if (item.Item1 == "Vagabond (First)") vagabondFlag = true;
            if (item.Item1 == "Vagabond (Second)")
                { if (!vagabondFlag) continue; }
            resultString[ind] = item.Item1; ind++;
            resultInt += item.Item2;
        }
        return (resultString, resultInt);
    }

    //Write out all elts of our cool root array
    static void RootWriter(List<(string[],int)> endList)
    {
        Console.WriteLine("\nYour Root faction setup is:");
        foreach((string[],int) item in endList)
        {
            for(int i = 0; i < item.Item1.Length; i++)
            {
                Console.Write(item.Item1[i]);
                if (i + 1 < item.Item1.Length)
                    Console.Write(", ");
                else
                    Console.Write("\t");
            }
            Console.Write(item.Item2);
            Console.Write("\n");
        }
    }
}

//thanks, stackoverflow
static class RandomExtensions
{
    public static void Shuffle<T>(this Random rng, T[] array)
    {
        int n = array.Length;
        while (n > 1)
        {
            int k = rng.Next(n--);
            T temp = array[n];
            array[n] = array[k];
            array[k] = temp;
        }
    }
}