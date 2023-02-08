using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace WarClub;

public enum Icon
{
  None,
  CrossedAxes,
  MilitaryFort,
  HumanTarget,
  LightningTear,
  Barracks,
}

public class Assets
{
  public Dictionary<string, Texture2D> MapTextures = new Dictionary<string, Texture2D>();
  public Dictionary<Icon, Texture2D> Icons = new Dictionary<Icon, Texture2D>();
  public Texture2D DataCard;


  ContentManager Content;


  public void Initialize(ContentManager Content)
  {
    this.Content = Content;
    LoadIcons();
    LoadDataCards();
  }

  void LoadIcons()
  {
    Icons.Add(Icon.CrossedAxes, Content.Load<Texture2D>("icons/crossed-axes"));
    Icons.Add(Icon.MilitaryFort, Content.Load<Texture2D>("icons/military-fort"));
    Icons.Add(Icon.HumanTarget, Content.Load<Texture2D>("icons/human-target"));
    Icons.Add(Icon.LightningTear, Content.Load<Texture2D>("icons/lightning-tear"));
    Icons.Add(Icon.Barracks, Content.Load<Texture2D>("icons/barracks"));
  }

  void LoadDataCards()
  {
    DataCard = Content.Load<Texture2D>("BlankSquadDataslate");
  }


}