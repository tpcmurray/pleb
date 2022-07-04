using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using pleb.ProcGen;
using pleb.ProcGen.Biomes;

namespace pleb
{
    public class PlebGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont _font1;
        private Texture2D _pixel;

        private int _height = 2000;
        private int _width = 1300;
        private int _scale = 1;

        private Texture2D _canvas;
        private Color[] _canvasPixels;
        private bool _dirty = false;

        private static int SEED = 111;
        private Random _random = new Random(SEED + 1);
        private Terrain _river = new Terrain(TerrainEnum.River, new PercRangeFloat(0.13f, 0.87f), 0, new Color(65, 133, 255));

        Map _map;

        public PlebGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = _width * _scale;
            _graphics.PreferredBackBufferHeight = _height * _scale;
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();

            _canvasPixels = new Color[_width * _height];                        // Create the canvas pixel array.
            _canvas = new Texture2D(_graphics.GraphicsDevice, _width, _height); // Create the canvas texture.

            base.Initialize();
        }

        protected override void LoadContent()
        {
            var sw = new Stopwatch();
            sw.Start();

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _font1 = Content.Load<SpriteFont>("Font1");
            _pixel = Content.Load<Texture2D>("oneByOneBlackPixel");

            double max = -10;
            double min = 10;
            float z;

            var per = new PerlinNoise3D(SEED);
            var smp = new OpenSimplexNoise(SEED);
            var range = new PercRangeFloat(0.13f, 0.87f);

            var coastalTowns = new Feature(0.0003f, Color.Magenta);
            var plainsTowns = new Feature(0.00006f, Color.Orange);
            var forestHamlet = new Feature(0.00001f, Color.Red);
            var mountainFortress = new Feature(0.00006f, Color.Black);
            var riverTown = new Feature(0.00006f, Color.Purple);

            var forest = new Forest(range);
            var frozen = new Frozen(range);
            var tundra = new Tundra(range);
            var jungle = new Jungle(range);

            float scale = 0.0008f;
            //float scale = 0.001f;

            _map = new Map(_width, _height);

            var frozenToTundra = new BiomeBlend(0f, 100, 300, 0.73f);

            float frozenBlend = 0f;
            int frozenBlendTop = 100;
            int frozenBlendBottom = 300;
            float frozenSplit = 0.73f;

            float tundraBlend = 0f;
            int tundraBlendTop = 400;
            int tundraBlendBottom = 700;
            float tundraSplit = 0.73f;

            float forestBlend = 0f;
            int forestBlendTop = 1400;
            int forestBlendBottom = 1700;
            float forestSplit = 0.73f;

            Terrain ter = null;

            for (int y = 0; y < _height; y++) {
                for (int x = 0; x < _width; x++) {

                    //                         new min  new max
                    // NOISE - PERLIN   range   0.13    0.87
                    int octaves = 5;
                    double lacunarity = 3.85;
                    z = (float)per.RidgedMF(x * scale, y * scale, 0, octaves, lacunarity, 0.44, 1);
                    // NOISE - PERLIN

                    //                        old min    old max
                    // NOISE - SIMPLEX  range -0.4778 to 0.7369
                    //z = (float)smp.Evaluate(x * scale, y * scale);
                    //z = (float)((((z - -0.4778) * (0.87 - 0.13)) / (0.7369 - -0.4778)) + 0.13);
                    // NOISE - SIMPLEX


                    if (z > max) max = z;
                    if (z < min) min = z;

                    var hash = FeatureGenerator.Hash(x, y);

                    _map[x, y] = new PointInfo(x, y, z, _map);

                    // calculate splits
                    if (y > frozenBlendTop && y <= frozenBlendBottom) {
                        frozenBlend = z / ((y - frozenBlendTop) / (float)(frozenBlendBottom - frozenBlendTop));
                    }

                    if (y > tundraBlendTop && y <= tundraBlendBottom) {
                        tundraBlend = z / ((y - tundraBlendTop) / (float)(tundraBlendBottom - tundraBlendTop));
                    }

                    if (y > forestBlendTop && y <= forestBlendBottom) {
                        forestBlend = z / ((y - forestBlendTop) / (float)(forestBlendBottom - forestBlendTop));
                    }

                    // get correct terrain considering latitude (y) and terrain height (higher altitude is colder)
                    if (y <= frozenBlendTop) {                                   // frozen pure
                        ter = frozen.GetTerrain(z);
                        _map[x, y].Biome = frozen;
                    } else if (y > frozenBlendTop && y <= frozenBlendBottom) {   // frozen/tundra blend
                        if (frozenBlend > frozenSplit) {
                            ter = frozen.GetTerrain(z);
                            _map[x, y].Biome = frozen;
                        } else {
                            ter = tundra.GetTerrain(z);
                            _map[x, y].Biome = tundra;
                        }
                    } else if (y > frozenBlendBottom && y <= tundraBlendTop) {   // tundra pure
                        ter = tundra.GetTerrain(z);
                        _map[x, y].Biome = tundra;
                    } else if (y > tundraBlendTop && y <= tundraBlendBottom) {   // tundra/forest blend
                        if (tundraBlend > tundraSplit) {
                            ter = tundra.GetTerrain(z);
                            _map[x, y].Biome = tundra;
                        } else {
                            ter = forest.GetTerrain(z);
                            _map[x, y].Biome = forest;
                        }
                    } else if (y > tundraBlendBottom && y <= forestBlendTop) {    // forest pure
                        ter = forest.GetTerrain(z);
                        _map[x, y].Biome = forest;
                    } else if (y > forestBlendTop && y <= forestBlendBottom) {   // forest/jungle blend
                        if (forestBlend > forestSplit) {
                            ter = forest.GetTerrain(z);
                            _map[x, y].Biome = forest;
                        } else {
                            ter = jungle.GetTerrain(z);
                            _map[x, y].Biome = jungle;
                        }
                    } else if (y > forestBlendBottom) {                          // jungle pure
                        ter = jungle.GetTerrain(z);
                        _map[x, y].Biome = jungle;
                    }

                    _map[x, y].Terrain = ter;
                    PSet(x, y, ter.Color);

                    if (ter.TerrainEnum == TerrainEnum.Shoreline &&
                        FeatureGenerator.IsHit(hash, coastalTowns.Probability)) {
                        coastalTowns.Add(x, y);
                    }

                    if (ter.TerrainEnum == TerrainEnum.Grass &&
                        FeatureGenerator.IsHit(hash, plainsTowns.Probability)) {
                        plainsTowns.Add(x, y);
                    }

                    if (ter.TerrainEnum == TerrainEnum.Forest &&
                        FeatureGenerator.IsHit(hash, forestHamlet.Probability)) {
                        forestHamlet.Add(x, y);
                    }

                    if (ter.TerrainEnum == TerrainEnum.Mountain &&
                        FeatureGenerator.IsHit(hash, mountainFortress.Probability)) {
                        mountainFortress.Add(x, y);
                    }
                }
            }

            foreach (var place in coastalTowns.Locations) {
                RecSet(place.Item1, place.Item2, 6, coastalTowns.Color);
            }

            foreach (var place in plainsTowns.Locations) {
                RecSet(place.Item1, place.Item2, 6, plainsTowns.Color);
            }

            foreach (var place in forestHamlet.Locations) {
                RecSet(place.Item1, place.Item2, 6, forestHamlet.Color);
            }

            foreach (var place in mountainFortress.Locations) {
                RecSet(place.Item1, place.Item2, 6, mountainFortress.Color);
            }

            sw.Stop();
            TimeSpan timeTaken = sw.Elapsed;

            SetFlowDirections(_map);

            var riverPoints = GenerateRivers(_map);

            int riverTownCount = 8;
            for (int r = 0; r < riverTownCount; r++) {
                var riverTownLocation = FindRiverTownLocation(_map, riverPoints);
                RecSet(riverTownLocation.x, riverTownLocation.y, 6, riverTown.Color);
                riverTown.Add(riverTownLocation.x, riverTownLocation.y);
            }
        }

        private HashSet<PointInfo> GenerateRivers(Map mi)
        {
            var riverTotal = 24;
            var riverLengthMax = 2400;
            PointInfo startPi = null;
            var totalRiverPoints = new HashSet<PointInfo>();

            for (int r = 0; r < riverTotal; r++) {
                for (int i = 0; i < 18; i++) {
                    startPi = FindRiverStart(mi);
                }
                var lastPi = startPi;
                var riverPoints = new HashSet<PointInfo>();
                var riverWidth = _random.Next(1, 3);

                for (int i = 0; i < riverLengthMax; i++) {
                    var newPoints = RecSet(lastPi.x, lastPi.y, riverWidth, _river.Color, _river);
                    newPoints.UnionWith(MarkRiverPoints(mi, newPoints, 0.00005f, riverWidth));
                    foreach (var point in newPoints) {
                        RecSet(point.x, point.y, riverWidth, _river.Color, _river);
                    }
                    riverPoints.UnionWith(newPoints);

                    if (i % 800 == 0 && riverWidth > 1) riverWidth--;

                    // find all !(river|water) points surrounding this new set
                    var newNonWaterPoints = GetAllNonWaterPointsSurroundingArea(newPoints, 0);

                    if (newNonWaterPoints.Any(p => p.Terrain.TerrainEnum == TerrainEnum.Mountain ||
                                                   p.Terrain.TerrainEnum == TerrainEnum.Snow)) {
                        continue;
                    }

                    if (newNonWaterPoints.Count == 0) break;

                    // find lowest point that's still > lastPi.z
                    PointInfo nextPi = new PointInfo(0, 0, 5, mi);
                    foreach (var point in newNonWaterPoints) {
                        if (point.z > lastPi.z && point.z < nextPi.z) nextPi = point;
                    }

                    if (nextPi.z == 5) {
                        nextPi = newNonWaterPoints.OrderByDescending(p => p.z).FirstOrDefault();
                    }

                    lastPi = nextPi;
                }

                foreach (var point in riverPoints) {
                    RecSet(point.x, point.y, riverWidth, _river.Color, _river);
                }

                totalRiverPoints.UnionWith(riverPoints);
            }

            return totalRiverPoints;
        }

        private PointInfo FindRiverTownLocation(Map mi, HashSet<PointInfo> rivers)
        {
            // random river point
            var rp = rivers.ElementAt(_random.Next(rivers.Count - 1));
            
            while(rp.Terrain.TerrainEnum == TerrainEnum.River) {
                rp = rp.North();
            }

            return rp;
        }

        private PointInfo FindRiverStart(Map mi) // random shoreline adjacent to a water body
        {
            int x; int y;
            Rectangle viewport = _graphics.GraphicsDevice.Viewport.Bounds;

tryagain:            
            // randomly find a water pixel
            while (true) {
                x = _random.Next(0, _width);
                y = _random.Next(0, _height);

                if (mi[x, y].Terrain.TerrainEnum == TerrainEnum.Ocean) break;
            }

            // travel until shore is found
            int angle = x % 4;
            angle--;
            Point step;
            Point check = new Point(x, y);


            for (int i = 0; i < 4; i++) {
                angle++; // try a different direction
                if (angle == 4) angle = 0; // wrap around
                switch (angle) {
                    case 0: step = new Point(0, -1); break;
                    case 1: step = new Point(1, 0); break;
                    case 2: step = new Point(0, 1); break;
                    default: step = new Point(-1, 0); break;
                }

                while(true) {
                    check += step;
                    if (!viewport.Contains(check)) {
                        break;
                    }

                    if (mi[check.X, check.Y].Terrain.TerrainEnum != TerrainEnum.Ocean &&
                        mi[check.X, check.Y].Terrain.TerrainEnum != TerrainEnum.Shallows) {
                        return mi[check.X, check.Y];
                    }
                }
            }

            goto tryagain;
        }

        private HashSet<PointInfo> MarkRiverPoints(Map mi, HashSet<PointInfo> testPoints, float limit, int riverWidth)
        {
            var newPoints = new HashSet<PointInfo>();
            foreach (var point in testPoints) {
                var surroundingPoints = point.GetSurroundingPoints();
                foreach (var spoint in surroundingPoints) {
                    newPoints.UnionWith(TestPointForRiver(mi, spoint, point, limit, riverWidth)); 
                }
            }
            return newPoints;
        }

        private HashSet<PointInfo> TestPointForRiver(Map mi, PointInfo pi, PointInfo start, float limit, int riverWidth)
        {
            var newPoints = new HashSet<PointInfo>();
            if (pi.Terrain.TerrainEnum != TerrainEnum.River &&
                pi.Terrain.TerrainEnum != TerrainEnum.Shallows &&
                pi.z <= start.z + limit && pi.z >= start.z - limit) {
                RecSet(pi.x, pi.y, riverWidth, _river.Color, _river);
                newPoints.Add(pi);
                newPoints.UnionWith(MarkRiverPoints(mi, newPoints, limit, riverWidth));
            }
            return newPoints;
        }

        private int _expandLimit = 4;
        private List<PointInfo> GetAllNonWaterPointsSurroundingArea(HashSet<PointInfo> points, int expandNumber)
        {
            var surrounding = new HashSet<PointInfo>();
            foreach (var point in points) {
                surrounding.UnionWith(point.GetSurroundingPoints());
            }

            var newPoints = new List<PointInfo>(surrounding);

            newPoints = newPoints.FindAll(p =>
                                            p.Terrain.TerrainEnum != TerrainEnum.Ocean &&
                                            p.Terrain.TerrainEnum != TerrainEnum.Shallows &&
                                            p.Terrain.TerrainEnum != TerrainEnum.River);
            if (newPoints.Count == 0 && expandNumber <= _expandLimit) {
                return GetAllNonWaterPointsSurroundingArea(surrounding, expandNumber + 1);
            }

            return newPoints;
        }

        private void SetFlowDirections(Map mapInfo)
        {
            PointInfo pi;
            short rise = 0;
            short run = 0;

            for (int x = 1; x < (_width - 1); x++) {
                for (int y = 1; y < (_height - 1); y++) {
                    pi = mapInfo[x, y];
                    rise = 0;
                    run = 0;

                    // n
                    if (mapInfo[x, y - 1].z > pi.z) {
                        rise += 1;
                    } else if (mapInfo[x, y - 1].z < pi.z) {
                        rise -= 1;
                    }

                    // ne
                    if (mapInfo[x + 1, y - 1].z > pi.z) {
                        rise += 1;
                        run -= 1;
                    } else if (mapInfo[x + 1, y - 1].z < pi.z) {
                        rise -= 1;
                        run += 1;
                    }

                    // e
                    if (mapInfo[x + 1, y].z > pi.z) {
                        run -= 1;
                    } else if (mapInfo[x + 1, y].z < pi.z) {
                        run += 1;
                    }

                    // se
                    if (mapInfo[x + 1, y + 1].z > pi.z) {
                        rise -= 1;
                        run -= 1;
                    } else if (mapInfo[x + 1, y + 1].z < pi.z) {
                        rise += 1;
                        run += 1;
                    }

                    // s
                    if (mapInfo[x, y + 1].z > pi.z) {
                        rise -= 1;
                    } else if (mapInfo[x, y + 1].z < pi.z) {
                        rise += 1;
                    }

                    // sw
                    if (mapInfo[x - 1, y + 1].z > pi.z) {
                        rise -= 1;
                        run += 1;
                    } else if (mapInfo[x - 1, y + 1].z < pi.z) {
                        rise += 1;
                        run -= 1;
                    }

                    // w
                    if (mapInfo[x - 1, y].z > pi.z) {
                        run += 1;
                    } else if (mapInfo[x - 1, y].z < pi.z) {
                        run -= 1;
                    }

                    // nw
                    if (mapInfo[x - 1, y - 1].z > pi.z) {
                        rise += 1;
                        run += 1;
                    } else if (mapInfo[x - 1, y - 1].z < pi.z) {
                        rise -= 1;
                        run -= 1;
                    }

                    pi.Rise = rise;
                    pi.Run = run;
                }
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
            if (_dirty)
            {
                _canvas.SetData(_canvasPixels, 0, _canvasPixels.Length);
                _dirty = false;
            }
            _spriteBatch.Draw(_canvas, new Rectangle(0, 0, _width * _scale, _height * _scale), Color.White);
            DrawDebugInfo();
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawDebugInfo()
        {
            var ms = Mouse.GetState();
            if (GraphicsDevice.Viewport.Bounds.Contains(new Point(ms.X, ms.Y))) {

                // point info
                var pi = _map[ms.X, ms.Y];
                var debugString = string.Format("x:{0} y:{1} z:{2}{5}Rise:{3} Run:{4}{5}Terrain:{6}" , 
                    pi.x, pi.y, pi.z, pi.Rise, pi.Run, Environment.NewLine, pi.Terrain.TerrainEnum.ToString());
                _spriteBatch.DrawString(_font1, debugString, new Vector2(100, 100), Color.Black);

                // slope line
                var vectors = LineFromSlopeAndMousePosition(pi, ms);
                DrawLine(_spriteBatch, vectors.Item1, vectors.Item2, Color.Black);
            }
        }

        public void DrawLine(SpriteBatch spriteBatch, Vector2 begin, Vector2 end, Color color, int width = 1)
        {
            Rectangle r = new Rectangle((int)begin.X, (int)begin.Y, (int)(end - begin).Length() + width, width);
            Vector2 v = Vector2.Normalize(begin - end);
            float angle = (float)Math.Acos(Vector2.Dot(v, -Vector2.UnitX));
            if (begin.Y > end.Y) angle = MathHelper.TwoPi - angle;
            spriteBatch.Draw(_pixel, r, null, color, angle, Vector2.Zero, SpriteEffects.None, 0);
        }

        public Tuple<Vector2, Vector2> LineFromSlopeAndMousePosition(PointInfo pi, MouseState ms)
        {
            var lineLengthMultiple = 4;
            var endVector = new Vector2(ms.X + (pi.Run * lineLengthMultiple), ms.Y + (pi.Rise * lineLengthMultiple));
            //endVector = Vector2.Multiply(endVector, 4);
            return new Tuple<Vector2, Vector2>(new Vector2(ms.X, ms.Y), endVector);
        }

        public void PSet(int x, int y, Color color)
        {
            var pos = x + (y * _width);

            if (pos >= 0 && pos < _canvasPixels.Length)
            {
                _canvasPixels[pos] = color;
                _dirty = true;
            }
        }

        public HashSet<PointInfo> RecSet(int x, int y, int size, Color color, Terrain t = null)
        {
            var points = new HashSet<PointInfo>();
            for (int i = x; i < x + size; i++) {
                for (int j = y; j < y + size; j++) {
                    points.Add(_map[i, j]);
                    PSet(i, j, color);
                    if (t != null &&
                        _map[i, j].Terrain.TerrainEnum != TerrainEnum.Ocean &&
                        _map[i, j].Terrain.TerrainEnum != TerrainEnum.Shallows) {
                        _map[i, j].Terrain = t;
                    }
                }
            }

            return points;
        }
    }
}
