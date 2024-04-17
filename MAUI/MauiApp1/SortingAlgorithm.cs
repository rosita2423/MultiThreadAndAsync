using System;


namespace SortingAlgorithm{
    class SortingAlgorithm
{
    void Merge()
    {
        int count = 30;
        if (count > 0)
        {
            int[] array = GenerateRandomArray(count, 1, 100); // Generates random numbers between 1 and 100

            Console.WriteLine("\nOriginal array:");
            PrintArray(array);

            QuickSort(array, 0, array.Length - 1);

            Console.WriteLine("\nSorted array:");
            PrintArray(array);
        }
        else
        {
            Console.WriteLine("Invalid input. Please enter a positive integer.");
        }
    }

    static int[] GenerateRandomArray(int count, int min, int max)
    {
        Random rand = new Random();
        int[] array = new int[count];
        for (int i = 0; i < count; i++)
        {
            array[i] = rand.Next(min, max + 1);
        }
        return array;
    }

    static void QuickSort(int[] arr, int low, int high)
    {
        if (low < high)
        {
            int partitionIndex = Partition(arr, low, high);
            QuickSort(arr, low, partitionIndex - 1);
            QuickSort(arr, partitionIndex + 1, high);
        }
    }

    static int Partition(int[] arr, int low, int high)
    {
        int pivot = arr[high];
        int i = low - 1;

        for (int j = low; j < high; j++)
        {
            if (arr[j] < pivot)
            {
                i++;
                int temp = arr[i];
                arr[i] = arr[j];
                arr[j] = temp;
            }
        }

        int temp2 = arr[i + 1];
        arr[i + 1] = arr[high];
        arr[high] = temp2;

        return i + 1;
    }

    static void PrintArray(int[] arr)
    {
        foreach (int num in arr)
        {
            Console.Write(num + " ");
        }
        Console.WriteLine();
    }
}

}
