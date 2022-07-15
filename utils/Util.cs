using System;
using System.Linq;
using System.Collections.Generic;

static class RNG
{
  static Random r = new Random();

  public static T PickFrom<T>(T[] arr)
  {
    return arr[r.Next(arr.Length)];
  }

  public static T PickFrom<T>(ICollection<T> list)
  {
    return list.ElementAt(r.Next(list.Count));
  }

  public static List<T> PickFrom<T>(ICollection<T> list, int count, bool duplicates = false)
  {
    return PickFrom<T>(list.ToArray(), count, duplicates);
  }

  public static List<T> PickFrom<T>(T[] arr, int count, bool duplicates = false)
  {
    count = Math.Clamp(count, 1, arr.Length);
    List<T> returnArray = new List<T>();
    for (int i = 0; i < count; i++)
    {
      if (!duplicates)
      {
        bool added = false;
        while (!added)
        {
          T new_value = arr[r.Next(arr.Length)];
          if (!returnArray.Contains(new_value))
          {
            returnArray.Add(arr[r.Next(arr.Length)]);
            added = true;
          }
        }
      }
      else
      {
        returnArray.Add(arr[r.Next(arr.Length)]);
      }
    }
    return returnArray;
  }

  public static bool Boolean()
  {
    return r.Next(2) > 0;
  }

  public static int Integer()
  {
    return r.Next();
  }

  public static int Integer(int min, int max)
  {
    return r.Next(min, max + 1);
  }

  public static int Integer(int max)
  {
    return r.Next(1, max + 1);
  }

  public static int Integer(double min, double max)
  {
    return Integer((int)min, (int)max);
  }

  public static int Integer(int min, double max)
  {
    return Integer((int)min, (int)max);
  }

  public static int Integer(double min, int max)
  {
    return Integer((int)min, (int)max);
  }

  /// <summary>
  ///   Generates normally distributed numbers. Each operation makes two Gaussians for the price of one, and apparently they can be cached or something for better performance, but who cares.
  /// </summary>
  /// <param name="r"></param>
  /// <param name = "mu">Mean of the distribution</param>
  /// <param name = "sigma">Standard deviation</param>
  /// <returns></returns>
  public static double Gaussian(double mu = 0, double sigma = 1)
  {
    var u1 = r.NextDouble();
    var u2 = r.NextDouble();

    var rand_std_normal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);

    var rand_normal = mu + sigma * rand_std_normal;

    return rand_normal;
  }

  /// <summary>
  ///   Generates values from a triangular distribution.
  /// </summary>
  /// <remarks>
  /// See http://en.wikipedia.org/wiki/Triangular_distribution for a description of the triangular probability distribution and the algorithm for generating one.
  /// </remarks>
  /// <param name="r"></param>
  /// <param name = "a">Minimum</param>
  /// <param name = "b">Maximum</param>
  /// <param name = "c">Mode (most frequent value)</param>
  /// <returns></returns>
  public static double Triangular(double a, double b, double c)
  {
    var u = r.NextDouble();

    return u < (c - a) / (b - a)
      ? a + Math.Sqrt(u * (b - a) * (c - a))
      : b - Math.Sqrt((1 - u) * (b - a) * (b - c));
  }
}
