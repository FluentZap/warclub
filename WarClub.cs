using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace WarClub;

public partial class WarClub : Game
{
  Simulation s;

  private GraphicsDeviceManager graphics;
  private SpriteBatch spriteBatch;
  Vector2 viewportSize = new Vector2(1920, 1080);
  // Vector2 viewportSize = new Vector2(2560, 1440);
  // Vector2 viewportSize = new Vector2(3840, 2160);
  Vector2 screenSize = new Vector2(3819, 2144);
  BasicEffect basicEffect;
  Effect planetEffect;
  Effect starfieldEffect;
  Texture2D planetTexture;
  Texture2D screenTexture;
  Texture2D DataCard;

  Model model;
  Model plane;

  float timeAdvance;
  float animationTime = 0;

  private SpriteFont basicFont;
  private SpriteFont basicFontLarge;
  private SpriteFont basicFontSmall;
  public Dictionary<Icon, Texture2D> icons = new Dictionary<Icon, Texture2D>();
  public Dictionary<string, Texture2D> MapTextures = new Dictionary<string, Texture2D>();
  public Texture2D BlankTexture;
  Dictionary<Icon, Trait> TraitIcons = new Dictionary<Icon, Trait>();

  Texture2D grassTexture;

  public WarClub()
  {
    graphics = new GraphicsDeviceManager(this);
    Content.RootDirectory = "Content";
    IsMouseVisible = true;
  }

  void GeneratePlanet()
  {
    FastNoiseLite elvNoise = new FastNoiseLite();

    elvNoise.SetSeed(RNG.Integer());
    elvNoise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
    elvNoise.SetFrequency(0.010f);
    elvNoise.SetFractalType(FastNoiseLite.FractalType.FBm);
    elvNoise.SetFractalOctaves(8);
    elvNoise.SetFractalLacunarity(2.00f);
    elvNoise.SetFractalGain(0.60f);

    Point size = new Point(3840, 2160);
    planetTexture = new Texture2D(GraphicsDevice, size.X, size.Y, false, SurfaceFormat.Color);
    GraphicsDevice.Textures[0] = null;

    float scale = 512f / (size.X + size.Y);

    uint[] arr = new uint[size.X * size.Y];

    uint[] grassArr = new uint[grassTexture.Width * grassTexture.Height];
    grassTexture.GetData<UInt32>(grassArr);


    foreach (int x in Enumerable.Range(0, size.X))
      foreach (int y in Enumerable.Range(0, size.Y))
      {
        int index = (y * size.X) + x;
        int seamlessIndex = ((y * 1024) % (1024 * 1024)) + (x % 1024);
        // float height = (elvNoise.GetNoise(x * scale, y * scale) + 1) * 0.5f;
        var grassColor = new Color(grassArr[seamlessIndex]);

        // Color c = new Color(height, height, height, 1);
        // arr[(y * size.X) + x] = c.PackedValue;
        arr[index] = grassArr[seamlessIndex];
      }

    planetTexture.SetData<UInt32>(arr, 0, size.X * size.Y);

  }

  protected override void Initialize()
  {
    // TODO: Add your initialization logic here

    base.Initialize();

    graphics.PreferredBackBufferHeight = (int)viewportSize.Y;
    graphics.PreferredBackBufferWidth = (int)viewportSize.X;
    // graphics.IsFullScreen = true;
    // Window.Position = new Point(-5760, 0);
    Window.IsBorderless = true;
    graphics.ApplyChanges();

    s.ViewMatrix = Matrix.CreateScale(viewportSize.X / screenSize.X, viewportSize.Y / screenSize.Y, 1);

    basicEffect = new BasicEffect(GraphicsDevice);
    basicEffect.Alpha = 1f;
    basicEffect.VertexColorEnabled = true;
    basicEffect.LightingEnabled = false;

    screenTexture = new Texture2D(GraphicsDevice, (int)screenSize.X, (int)screenSize.Y);

  }

  protected override void LoadContent()
  {
    spriteBatch = new SpriteBatch(GraphicsDevice);

    // TODO: use this.Content to load your game content here
    s = new Simulation();
    s.LoadData();
    foreach (var tile in s.MapTiles)
    {
      MapTextures.Add(tile.Texture, Content.Load<Texture2D>($"planetTextures/{tile.Terrain}/{tile.Texture}"));
    }
    icons.Add(Icon.CrossedAxes, Content.Load<Texture2D>("icons/crossed-axes"));
    icons.Add(Icon.MilitaryFort, Content.Load<Texture2D>("icons/military-fort"));
    icons.Add(Icon.HumanTarget, Content.Load<Texture2D>("icons/human-target"));
    icons.Add(Icon.LightningTear, Content.Load<Texture2D>("icons/lightning-tear"));
    icons.Add(Icon.Barracks, Content.Load<Texture2D>("icons/barracks"));
    BlankTexture = Content.Load<Texture2D>("planetTextures/blank");

    DataCard = Content.Load<Texture2D>("panel");

    basicFont = Content.Load<SpriteFont>("romulus");
    basicFontSmall = Content.Load<SpriteFont>("romulus_small");
    basicFontLarge = Content.Load<SpriteFont>("romulus_large");
    model = Content.Load<Model>("IcoSphere");
    plane = Content.Load<Model>("Plane");
    planetEffect = Content.Load<Effect>("planetEffect");
    starfieldEffect = Content.Load<Effect>("starfield");

    TraitIcons.Add(Icon.CrossedAxes, s.Traits["war zone"]);
    TraitIcons.Add(Icon.MilitaryFort, s.Traits["strongholds"]);
    TraitIcons.Add(Icon.HumanTarget, s.Traits["high value targets"]);
    TraitIcons.Add(Icon.LightningTear, s.Traits["enlisted gods"]);
    TraitIcons.Add(Icon.Barracks, s.Traits["training camps"]);

    foreach (var unit in s.UnitList)
    {
      if (unit.Image != "")
      {
        FileStream fileStream = new FileStream(Path.Combine("./UnitImages", unit.Image), FileMode.Open);
        unit.Texture = Texture2D.FromStream(graphics.GraphicsDevice, fileStream);
        fileStream.Dispose();
      }
    }
  }

  protected override void Update(GameTime gameTime)
  {
    s.KeyState.SetKeys(Keyboard.GetState().GetPressedKeys());
    if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
      Exit();

    // var pressedKeys = simulation.KeyState.GetTriggeredKeys(true);
    // if ((pressedKeys.Contains(Keys.LeftAlt) || pressedKeys.Contains(Keys.RightAlt)) && pressedKeys.Contains(Keys.Enter))
    // {
    //   graphics.ToggleFullScreen();
    // }
    InputGovernor.DoEvents(s);

    // if (s.SelectedView == View.GalaxyOverview)
    // {
    //   if (timeAdvance >= 5000)
    //   {
    //     timeAdvance = 0;
    //     s.AdvanceTime();
    //   }
    //   timeAdvance += gameTime.ElapsedGameTime.Milliseconds;
    // }


    animationTime += gameTime.ElapsedGameTime.Milliseconds;
    base.Update(gameTime);
  }

}
