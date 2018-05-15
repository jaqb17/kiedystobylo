using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Reflection;
using System;
using SkinnedModel;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Text;

namespace DevilSoup
{
    public class Asset
    {
        public Model model { get; private set; }
        public Matrix world { get; set; }
        public Vector3 cameraPos { get; set; }
        public Vector3 center { get; private set; }
        public float radius { get; private set; }
        private bool ifPlay = false;

        public bool IfPlay
        {
            get { return this.ifPlay; }
            set
            {
                this.ifPlay = value;
                if (this.ifPlay)
                    this.animationStarted = DateTime.Now;
            }
        }

        public bool ifDamageAfterPlay { get; set; } = false;
        private AnimationPlayer animationPlayer;
        public double animationLength { get; private set; }
        public DateTime animationStarted { get; private set; }

        public Asset()
        {
        }

        public Asset(Model model, Vector3 cameraPos)
        {
            this.model = model;
            this.cameraPos = cameraPos;
            computeCenter();
        }

        public Asset(Model model, Camera camera)
        {
            this.model = model;
            this.cameraPos = camera.Position;
            this.world = camera.world;
            computeCenter();
        }

        public void initializeClip(String clipName)
        {
            if (animationPlayer == null)
                return;

            SkinningData skinningData = model.Tag as SkinningData;
            AnimationClip clip = skinningData.AnimationClips[clipName];
            animationLength = clip.Duration.TotalMilliseconds;
            animationPlayer.StartClip(clip);
        }

        public void animationUpdate(TimeSpan timeSpan, bool relativeToCurrentTime = true, Matrix? rootTransform = null)
        {
            if (!ifPlay)
                return;

            if (rootTransform == null)
                rootTransform = Matrix.Identity;

            animationPlayer.Update(timeSpan, relativeToCurrentTime, (Matrix)rootTransform);
        }

        public void loadModel(ContentManager content, string modelType, String path)
        {

            if (ModelsInstancesClass.models[modelType] == null)
            {
                try
                {
                    this.model = content.Load<Model>(path);
                    ModelsInstancesClass.models[modelType] = this.model;
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Element o tym kluczu juz byl dodany. Klasa: Asset, Linia: 74");
                }
            }
            else this.model = ModelsInstancesClass.models[modelType];

            SkinningData skinningData = model.Tag as SkinningData;

            if (skinningData != null)
            {
                animationPlayer = new AnimationPlayer(skinningData);
                initializeClip("Take 001");
            }

            computeCenter();
        }

        public void LoadContentFile(ContentManager content, string modelType, string filePath)
        {

            if (ModelsInstancesClass.models[modelType] == null)
            {
                string outputDir = Assembly.GetExecutingAssembly().Location.Remove(Assembly.GetExecutingAssembly().Location.Length - Assembly.GetExecutingAssembly().GetName().Name.Length - 4);
                string filename = filePath.Split('/')[filePath.Split('/').Length - 1];

                string copyPath = outputDir + filename;
                string[] completePathArr = outputDir.Split('\\');

                string completeFilePath = "";

                for (int i = 0; i < completePathArr.Length - 5; i++)
                    completeFilePath += completePathArr[i] + "/";

                completeFilePath += "Content/" + filePath;

                string shortFileName = filename.Split('.')[0];
                string pipeLineFile = "runtimeanimatedmodel.txt";

                Process pProcess = null;
                Task loadTaskOut = Task.Factory.StartNew(() =>
                {
                    //Copy file to directory
                    {
                        try
                        {
                            if (copyPath != null)
                                File.Copy(completeFilePath, copyPath);
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(e.Message);
                        }
                        if (!File.Exists(copyPath)) return;
                    }

                    //string MGCBpathDirectory = Application.StartupPath + "/Content/MGCB/";
                    string mgcbPathExe = outputDir + "Content/MGCB/mgcb.exe";

                    completeFilePath = completeFilePath.Replace("\\", "/");

                    bool inProgress = true;
                    while (inProgress)
                    {

                        //Create pProcess
                        pProcess = new Process
                        {
                            StartInfo =
                            {
                            FileName = mgcbPathExe,
                            Arguments = "/@:\"Content/mgcb/" + pipeLineFile + "\""
                            +" /outputDir:\"Content/" +filePath.Remove(filePath.Length - filename.Length) + "\""
                            +" /build:\"" +filename + "\"",
                            CreateNoWindow = true,
                            WorkingDirectory = outputDir,
                            UseShellExecute = false,
                            RedirectStandardError = true,
                            RedirectStandardOutput = true
                            }
                        };

                        //Get program output
                        string stdError;
                        StringBuilder stdOutput = new StringBuilder();
                        pProcess.OutputDataReceived += (sender, args) => stdOutput.Append(args.Data);

                        try
                        {
                            pProcess.Start();
                            pProcess.BeginOutputReadLine();
                            stdError = pProcess.StandardError.ReadToEnd();
                            pProcess.WaitForExit();
                        }
                        catch (Exception e)
                        {
                            throw new Exception("OS error while executing : " + e.Message, e);
                        }

                        if (pProcess.ExitCode == 0)
                        {
                            Console.WriteLine("Udalo sie!");
                        }
                        else
                        {
                            var message = new StringBuilder();
                            Console.WriteLine("Nie udalo sie");

                            message.AppendLine(stdError);
                            message.AppendLine("Std output:");
                            message.AppendLine(stdOutput.ToString());
                            Console.WriteLine(message);
                        }

                        inProgress = false;
                    }

                    //string path = outputDir + "\\Content\\Runtime\\Textures\\" + shortFileName;
                    //File.Delete(path + ".xnb");

                    //We should delete the generated .xnb file in the directory now

                    if (copyPath != null)
                        File.Delete(copyPath);


                });

                loadModel(content, modelType, filePath.Split('.')[0]);
            }
            else loadModel(content, modelType, "");
        }

        public void unloadModel()
        {
            model = null;
        }

        public void DrawModel(Matrix view, Matrix projection, Vector3? color = null)
        {

            if (model == null) return;

            Matrix[] bones = null;
            if (animationPlayer != null)
            {
                if (!ifPlay)
                    animationPlayer.Update(new TimeSpan(0), true, Matrix.Identity);

                bones = animationPlayer.GetSkinTransforms();
            }

            //// Draw the model.
            foreach (ModelMesh modelMesh in model.Meshes)
            {
                foreach (Effect effect in modelMesh.Effects)
                {
                    if (effect is BasicEffect)
                    {
                        BasicEffect beffect = effect as BasicEffect;

                        beffect.World = world;
                        beffect.View = view;
                        beffect.Projection = projection;
                        //beffect.EnableDefaultLighting();
                        beffect.PreferPerPixelLighting = true;
                        if (color != null)
                            beffect.DiffuseColor = color ?? new Vector3(0.0f, 0.0f, 0.0f);
                    }

                    if (effect is SkinnedEffect)
                    {
                        SkinnedEffect seffect = effect as SkinnedEffect;

                        seffect.SetBoneTransforms(bones);

                        seffect.World = world;
                        seffect.View = view;
                        seffect.Projection = projection;

                        seffect.EnableDefaultLighting();

                        seffect.SpecularColor = new Vector3(0.25f);
                        seffect.SpecularPower = 16;
                    }
                }

                modelMesh.Draw();
            }
        }

        public void scaleAset(float scale)
        {
            this.world = Matrix.CreateScale(scale) * this.world;
        }

        public void scaleAset(float scaleX, float scaleY, float scaleZ)
        {
            this.world = Matrix.CreateScale(scaleX, scaleY, scaleZ) * this.world;
        }

        public void rotateAsset(int degreesX, int degreesY, int degreesZ)
        {
            this.world = Matrix.CreateRotationZ(-degreesX * (MathHelper.Pi / 180f)) * this.world;
            this.world = Matrix.CreateRotationX(-degreesY * (MathHelper.Pi / 180f)) * this.world;
            this.world = Matrix.CreateRotationY(-degreesZ * (MathHelper.Pi / 180f)) * this.world;
        }

        private void computeCenter()
        {
            center = model.Meshes[0].BoundingSphere.Center;
            radius = model.Meshes[0].BoundingSphere.Radius;
        }
    }
}
