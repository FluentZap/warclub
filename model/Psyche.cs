using System;

namespace WarClub;

class Psyche
{
  public byte Openness = 50;
  public byte Conscientiousness = 50;
  public byte Extroversion = 50;
  public byte Agreeableness = 50;
  public byte Neuroticism = 50;

  public static Psyche operator +(Psyche a, Psyche b)
  {
    a.Openness += b.Openness;
    a.Conscientiousness += b.Conscientiousness;
    a.Extroversion += b.Extroversion;
    a.Agreeableness += b.Agreeableness;
    a.Neuroticism += b.Neuroticism;
    return a;
  }

  public static Psyche operator *(Psyche a, double b)
  {
    a.Openness = (byte)Math.Clamp((a.Openness * b), 0, 100);
    a.Conscientiousness = (byte)Math.Clamp((a.Conscientiousness * b), 0, 100);
    a.Extroversion = (byte)Math.Clamp((a.Extroversion * b), 0, 100);
    a.Agreeableness = (byte)Math.Clamp((a.Agreeableness * b), 0, 100);
    a.Neuroticism = (byte)Math.Clamp((a.Neuroticism * b), 0, 100);
    return a;
  }

  public int GetPsycheDivergence(Psyche psyche)
  {
    var o = Math.Abs(Openness - psyche.Openness);
    var c = Math.Abs(Conscientiousness - psyche.Conscientiousness);
    var e = Math.Abs(Extroversion - psyche.Extroversion);
    var a = Math.Abs(Agreeableness - psyche.Agreeableness);
    var n = Math.Abs(Neuroticism - psyche.Neuroticism);
    return o + c + e + a + n;
  }

  public void ShiftByPsyche(Psyche psyche, double ratio = 0.085)
  {
    Openness += (byte)Math.Clamp(ShiftVector(Openness, psyche.Openness, ratio), 0, 100);
    Conscientiousness += (byte)Math.Clamp(ShiftVector(Conscientiousness, psyche.Conscientiousness, ratio), 0, 100);
    Extroversion += (byte)Math.Clamp(ShiftVector(Extroversion, psyche.Extroversion, ratio), 0, 100);
    Agreeableness += (byte)Math.Clamp(ShiftVector(Agreeableness, psyche.Agreeableness, ratio), 0, 100);
    Neuroticism += (byte)Math.Clamp(ShiftVector(Neuroticism, psyche.Neuroticism, ratio), 0, 100);
  }

  private int ShiftVector(byte v1, byte v2, double ratio)
  {
    int difference = v1 - v2;
    int shift = Math.Max((int)Math.Floor(Math.Abs(difference * ratio)), 1);
    return difference > 0 ? -shift : difference < 0 ? shift : 0;
  }

}