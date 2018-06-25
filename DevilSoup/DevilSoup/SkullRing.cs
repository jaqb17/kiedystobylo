using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevilSoup
{
    class SkullRing
    {
        private Asset skullAsset;
        private DanceArea danceArea;

        private List<Matrix> transforms;

        public SkullRing(ContentManager content, DanceArea danceArea, Camera camera)
        {
            this.danceArea = danceArea;

            transforms = new List<Matrix>();
            skullAsset = new Asset(content, "Assets/Souls/TreadingWater",
                                            "Assets/Czacha/grzesznik2txtr_Albedo",
                                            "Assets/Czacha/grzesznik2txtr_Normal",
                                            "Assets /Czacha/grzesznik2txtr_Specular",
                                            camera);

            //na godzinie 12 jest transform 0 i dalej zgodnie z zegarem

            transforms.Add(Matrix.CreateTranslation(0, 30, -40)); // 0
            transforms.Add(Matrix.CreateTranslation(35, 30, -35)); // 1
            transforms.Add(Matrix.CreateTranslation(50, 30, 0)); // 2
            transforms.Add(Matrix.CreateTranslation(35, 30, 35)); // 3
            transforms.Add(Matrix.CreateTranslation(0, 30, 50)); // 4
            transforms.Add(Matrix.CreateTranslation(-35, 30, 35)); // 5
            transforms.Add(Matrix.CreateTranslation(-50, 30, 0)); // 6
            transforms.Add(Matrix.CreateTranslation(-35, 30, -35)); // 7


            for (int i = 0; i < transforms.Count; i++)
                transforms[i] = Matrix.CreateScale(.7f) * transforms[i];

        }


        public void render(Camera camera)
        {
            int howManyToDraw = Convert.ToInt32(Math.Floor(transforms.Count * danceArea.combo.getComboStatus()));
            Debug.WriteLine(danceArea.combo.getComboStatus());
            for (int i = 0; i < howManyToDraw; i++)
            {
                skullAsset.world = transforms[i];
                skullAsset.SimpleDraw(camera.view, camera.projection);
            }




        }
    }
}
