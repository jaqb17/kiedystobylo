using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevilSoup
{
    public class Combo
    {
        private const int backgroundWidth = 150;
        private const int backgroundHeight = 150;
        private const int singleRectWidth = backgroundWidth / 3;
        private const int singleRectHeight = backgroundHeight / 3;
        private List<SingleAreasIndexes[]> availableCombos;
        private static Combo instance;

        public static Combo createCombo()
        {
            if (instance == null) instance = new Combo();

            return instance;
        }

        private Combo()
        {
            defineCombos();
        }

        private void defineCombos()
        {
            this.availableCombos = new List<SingleAreasIndexes[]>();

            availableCombos.Add(new   SingleAreasIndexes[4] { SingleAreasIndexes.UpperLeft, SingleAreasIndexes.Left, SingleAreasIndexes.BottomLeft, SingleAreasIndexes.Bottom });
            availableCombos.Add(new   SingleAreasIndexes[4] { SingleAreasIndexes.UpperLeft, SingleAreasIndexes.Up, SingleAreasIndexes.UpperRight, SingleAreasIndexes.Right });
            availableCombos.Add(new   SingleAreasIndexes[8] {
                                        SingleAreasIndexes.Up, SingleAreasIndexes.UpperRight, SingleAreasIndexes.Right,
                                        SingleAreasIndexes.BottomRight, SingleAreasIndexes.Bottom, SingleAreasIndexes.BottomLeft,
                                        SingleAreasIndexes.Left, SingleAreasIndexes.UpperLeft });
        }

        public Texture2D drawMap(GraphicsDeviceManager graphics)
        {
            Texture2D rect = new Texture2D(graphics.GraphicsDevice, singleRectWidth, singleRectHeight);

            Color[] data = new Color[singleRectWidth * singleRectHeight];

            for (int i = 0; i < data.Length; ++i) data[i] = Color.LightYellow * 0.2f;
            rect.SetData(data);

            return rect;
        }

        //if index = -1 then its about background
        public Vector2 getRectangleCoord(GraphicsDeviceManager graphics, int index)
        {
            return computeXY(graphics.GraphicsDevice.Viewport.Width, index);
        }

        public Color getColor()
        {
            return Color.White;
        }

        private Vector2 computeXY(int viewportWidth, int index)
        {
            int xIndex = index % 3;
            int yIndex = index / 3;

            return new Vector2(viewportWidth - (3 - xIndex) * singleRectWidth - 10 - (3 - xIndex) * 3, yIndex * singleRectHeight + 10 + yIndex * 3);
        }

        private void randomizeCombo()
        {

        }
    }
}
