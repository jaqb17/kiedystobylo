using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DevilSoup
{
    public class Combo
    {
        private const int backgroundWidth = 150;
        private const int backgroundHeight = 150;
        private const int singleRectWidth = backgroundWidth / 3;
        private const int singleRectHeight = backgroundHeight / 3;
        private List<int[]> availableCombos;
        private int randomedComboIndex;
        private int comboPosition; // na ktorym etapie gracz sie znajduje w trakcie combo
        public bool ifComboActive = false;
        private static Combo instance;
        private float transparency;
        private Thread comboThread;
        private SpriteBatch spriteBatch;
        private Player player;
        private GraphicsDeviceManager graphics;
        private int lastPointsHistory = 0;
        public readonly int pointsLimitToActivateCombo = 60;
        public int comboPointsProgressBar { get; private set; } = 0;

        public Vector3 actualGarnekColor;

        public static Combo createCombo()
        {
            if (instance == null) instance = new Combo();

            return instance;
        }

        private Combo()
        {
            defineCombos();
        }

        public void Initialize(GraphicsDeviceManager graphics)
        {
            player = Player.getPlayer();
            this.graphics = graphics;
        }

        public void LoadContent(SpriteBatch spriteBatch)
        {
            this.spriteBatch = spriteBatch;
        }

        public void Draw(GameTime gameTime)
        {
            if (player.hp <= 0)
            {
                // stopComboLoop();
                comboPointsProgressBar = 0;
            }
        }

        public void reset()
        {
            comboPointsProgressBar = 0;
            lastPointsHistory = 0;
        }

        public void startComboLoop()
        {
            comboThread = new Thread(new ThreadStart(comboLoop));
            comboThread.Start();
            comboThread.Name = "Combo thread";
            comboThread.IsBackground = true;
        }

        public void stopComboLoop()
        {
            comboThread.Abort();
        }

        private void comboLoop()
        {
            if (Player.getPlayer().hp > 0)
            {
                while (!GlobalVariables.ifGameOver)
                {
                    if (!GlobalVariables.ifGamePause)
                    {
                        if (!ifComboActive)
                        {
                            Thread.Sleep(250);
                            checkIfComboIsActive();
                            Console.WriteLine("Czy kombo jest aktywne: " + this.ifComboActive);
                        }
                        else
                        {
                            for (int i = 0; i < 20; i++)
                            {
                                Thread.Sleep(300);
                                transparency -= 0.05f;
                            }

                            if (ifComboActive)
                                this.finishCombo();
                            //Thread.Sleep(10000);
                        }
                    }
                }
            }
        }

        private void defineCombos()
        {
            this.availableCombos = new List<int[]>();

            availableCombos.Add(new int[4] { (int)SingleAreasIndexes.UpperLeft, (int)SingleAreasIndexes.Left, (int)SingleAreasIndexes.BottomLeft, (int)SingleAreasIndexes.Bottom });
            availableCombos.Add(new int[4] { (int)SingleAreasIndexes.UpperLeft, (int)SingleAreasIndexes.Up, (int)SingleAreasIndexes.UpperRight, (int)SingleAreasIndexes.Right });
            availableCombos.Add(new int[8] {
                                        (int)SingleAreasIndexes.Up, (int)SingleAreasIndexes.UpperRight, (int)SingleAreasIndexes.Right,
                                        (int)SingleAreasIndexes.BottomRight, (int)SingleAreasIndexes.Bottom, (int)SingleAreasIndexes.BottomLeft,
                                        (int)SingleAreasIndexes.Left, (int)SingleAreasIndexes.UpperLeft });
        }

        public Texture2D drawMap(GraphicsDeviceManager graphics, int index)
        {
            index = this.correctIndex(index);
            Texture2D rect = new Texture2D(graphics.GraphicsDevice, singleRectWidth, singleRectHeight);


            Color color = Color.Pink;
            if (index == availableCombos[randomedComboIndex][comboPosition]) color = Color.Red;
            else if (availableCombos[randomedComboIndex].Contains<int>(index)) color = Color.YellowGreen;

            Color[] data = new Color[singleRectWidth * singleRectHeight];
            for (int i = 0; i < data.Length; ++i) data[i] = color * transparency;
            rect.SetData(data);

            return rect;
        }

        public Vector3 getColor(int index)
        {

            if (index == availableCombos[randomedComboIndex][comboPosition])
            {

                return (new Vector3(0.5f, 0, 0) * transparency);
            }
            else if (availableCombos[randomedComboIndex].Contains<int>(index))
            {

                return new Vector3(0, 0.5f, 0) * transparency;
            }
            else
            {

                return actualGarnekColor * transparency;
            }
        }

        public Vector2 getRectangleCoord(GraphicsDeviceManager graphics, int index)
        {
            return computeXY(graphics.GraphicsDevice.Viewport.Width, index);
        }

        public Color getColor()
        {
            return Color.White;
        }

        private int correctIndex(int index)
        {
            desiredSingleAreasIndexes[] desiredValues = (desiredSingleAreasIndexes[])Enum.GetValues(typeof(desiredSingleAreasIndexes));
            desiredSingleAreasIndexes desiredValue = (desiredSingleAreasIndexes)desiredValues.GetValue(index);
            SingleAreasIndexes[] values = (SingleAreasIndexes[])Enum.GetValues(typeof(SingleAreasIndexes));

            for (int i = 0; i < values.Length; i++)
            {
                if (desiredValue.ToString() == values.GetValue(i).ToString())
                {
                    return i;
                }
            }

            return -1;
        }

        private Vector2 computeXY(int viewportWidth, int index)
        {
            if (index >= 4) index++;
            if (index == 9) index = 4;

            int xIndex = index % 3;
            int yIndex = index / 3;

            return new Vector2(viewportWidth - (3 - xIndex) * singleRectWidth - 10 - (3 - xIndex) * 3, yIndex * singleRectHeight + 10 + yIndex * 3);
        }

        public bool getIfComboIsActive()
        {
            return this.ifComboActive;
        }

        public int[] areaPressed(int areaId)
        {
            //areaId = this.correctIndex(areaId);
            if (areaId == availableCombos[randomedComboIndex][comboPosition])
            {
                comboPosition++;
                if (comboPosition >= availableCombos[randomedComboIndex].Length)
                {
                    finishCombo();
                    return availableCombos[randomedComboIndex];
                }
            }
            return null;
        }

        private void finishCombo()
        {
            this.ifComboActive = false;
            comboPointsProgressBar = 0;
        }

        //Na razie jest 30% szans na rozpoczecie combosa
        private void checkIfComboIsActive()
        {
            player = Player.getPlayer();
            if (Player.getPlayer().hp > 0)
            {
                comboPointsProgressBar = player.points - lastPointsHistory;
                if (comboPointsProgressBar >= pointsLimitToActivateCombo)
                {
                    ifComboActive = true;
                    comboPosition = 0;
                    transparency = 1.0f;
                    randomizeCombo();
                    lastPointsHistory = player.points;
                }
            }

        }
        public float getComboStatus()
        {
            if (comboPointsProgressBar >= pointsLimitToActivateCombo)
            {
                return 1.0f;
            }
            else
            {
                return (1.0f * comboPointsProgressBar) / (pointsLimitToActivateCombo * 1.0f);
            }
        }

        private void randomizeCombo()
        {
            this.randomedComboIndex = Randomizer.GetRandomNumber(0, this.availableCombos.Count());
        }
    }
}
