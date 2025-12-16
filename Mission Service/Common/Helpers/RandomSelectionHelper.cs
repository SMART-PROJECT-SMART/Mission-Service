namespace Mission_Service.Common.Helpers;

public static class RandomSelectionHelper
{
    public static T SelectRandom<T>(IReadOnlyList<T> list) =>
     list[Random.Shared.Next(list.Count)];

    public static T SelectRandom<T>(T[] array) =>
        array[Random.Shared.Next(array.Length)];

    public static T SelectRandom<T>(IEnumerable<T> collection)
    {
        int count = collection.Count();
        return collection.ElementAt(Random.Shared.Next(count));
    }

    public static int GetRandomIndex(int maxValue) =>
        Random.Shared.Next(maxValue);

    public static (int firstIndex, int secondIndex) SelectTwoDifferentIndices(int count)
    {
        int firstIndex = Random.Shared.Next(count);
        int secondIndex = Random.Shared.Next(count);

      while (secondIndex == firstIndex)
        {
       secondIndex = Random.Shared.Next(count);
        }

        return (firstIndex, secondIndex);
    }

    public static bool ShouldOccur(double probability) =>
        Random.Shared.NextDouble() < probability;

    public static DateTime SelectRandomTime(DateTime start, DateTime end)
    {
        if (end <= start) return start;

   TimeSpan range = end - start;
        double randomSeconds = Random.Shared.NextDouble() * range.TotalSeconds;
        return start.AddSeconds(randomSeconds);
    }
}
