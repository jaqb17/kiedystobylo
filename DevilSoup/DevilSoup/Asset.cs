using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using MGSkinnedAnimationAux;
using static DevilSoup.AnimatedModelShader;

namespace DevilSoup
{
    public class Asset
    {
        public Model model { get; private set; }
        public Matrix world { get; set; }
        public Vector3 cameraPos { get; set; }
        public Vector3 center { get; private set; }
        public float radius { get; private set; }

        private ModelExtra modelExtra = null;

        /// <summary>
        /// The model bones
        /// </summary>
        private List<Bone> bones = new List<Bone>();
        /// <summary>
        /// The underlying bones for the model
        /// </summary>
        public List<Bone> Bones { get { return bones; } }

        private Matrix[] boneTransforms;
        private Matrix[] skeleton;
        private bool hasSkinnedVertexType = false;
        private bool hasNormals = false;
        private bool hasTexCoords = false;

        public Asset() { }

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

        //public void loadModel(ContentManager content, String path)
        //{
        //    model = content.Load<Model>(path);
        //    computeCenter();
        //}


        public void LoadContentFile<T>(ContentManager content, string filePath)
        {
            string filename = filePath.Split('/')[filePath.Split('/').Length - 1];
            string copyPath = Application.StartupPath + "/" + filename;
            string[] completePathArr = Application.StartupPath.Split('\\');

            string completeFilePath = "";

            for (int i = 0; i < completePathArr.Length - 4; i++)
                completeFilePath += completePathArr[i] + "/";

            completeFilePath += "Content/" + filePath;

            string shortFileName = filename.Split('.')[0];
            string pipeLineFile = "runtimeanimatedmodel.txt";

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
                string mgcbPathExe = Application.StartupPath + "/Content/MGCB/mgcb.exe";

                completeFilePath = completeFilePath.Replace("\\", "/");

                bool inProgress = true;
                while (inProgress)
                {

                    //Create pProcess
                    Process pProcess = new Process
                    {
                        StartInfo =
                        {
                            FileName = mgcbPathExe,
                            Arguments = "/@:\"Content/mgcb/" + pipeLineFile + "\""
                            +" /build:\"" + filename + "\"",
                            CreateNoWindow = true,
                            WorkingDirectory = Application.StartupPath,
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

                //ContentArray[position] = new AnimatedModel("Runtime/Textures/" + shortFileName);
                //((AnimatedModel)ContentArray[position]).LoadContent(_contentManager);

                loadModel(content, "Runtime/Textures/" + shortFileName);

                string path = Application.StartupPath + "\\Content\\Runtime\\Textures\\" + shortFileName;
                File.Delete(path + ".xnb");

                //We should delete the generated .xnb file in the directory now

                if (copyPath != null)
                    File.Delete(copyPath);


            });
        }

        public void loadModel(ContentManager content, String path)
        {

            this.model = content.Load<Model>(path);
            modelExtra = model.Tag as ModelExtra;

            //System.Diagnostics.Debug.Assert(modelExtra != null);
            if (modelExtra != null)
            {
                ObtainBones();

                boneTransforms = new Matrix[bones.Count];

                skeleton = new Matrix[modelExtra.Skeleton.Count];
            }

            VertexElement[] test = model.Meshes[0].MeshParts[0].VertexBuffer.VertexDeclaration.GetVertexElements();

            for (int index = 0; index < test.Length; index++)
            {
                var t = test[index];
                if (t.VertexElementUsage == VertexElementUsage.BlendWeight)
                {
                    hasSkinnedVertexType = true;
                }
                else if (t.VertexElementUsage == VertexElementUsage.Normal)
                {
                    hasNormals = true;
                }
                else if (t.VertexElementUsage == VertexElementUsage.TextureCoordinate)
                {
                    hasTexCoords = true;
                }
            }

            computeCenter();
        }


        #region Bones Management

        /// <summary>
        /// Get the bones from the model and create a bone class object for
        /// each bone. We use our bone class to do the real animated bone work.
        /// </summary>
        private void ObtainBones()
        {
            bones.Clear();
            foreach (ModelBone bone in model.Bones)
            {
                // Create the bone object and add to the heirarchy
                Bone newBone = new Bone(bone.Name, bone.Transform, bone.Parent != null ? bones[bone.Parent.Index] : null);

                // Add to the bones for this model
                bones.Add(newBone);
            }
        }

        /// <summary>
        /// Find a bone in this model by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Bone FindBone(string name)
        {
            foreach (Bone bone in Bones)
            {
                if (bone.Name == name)
                    return bone;
            }
            return null;
        }

        #endregion

        public void unloadModel()
        {
            model = null;
        }

        public void DrawModel(Matrix view, Matrix projection, Vector3? color = null)
        {

            if (model == null) return;

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

                    /* if (effect is SkinnedEffect)
                     {
                         try
                         {
                             SkinnedEffect seffect = effect as SkinnedEffect;
                             seffect.World = boneTransforms[modelMesh.ParentBone.Index] * world;
                             seffect.View = view;
                             seffect.Projection = projection;
                             seffect.EnableDefaultLighting();
                             seffect.PreferPerPixelLighting = true;
                             seffect.SetBoneTransforms(skeleton);
                         }
                         catch(Exception e)
                         {

                         }
                     }*/
                }

                modelMesh.Draw();
            }
        }

        public void DrawModel(Matrix view, Matrix projection, AnimatedModelShader _skinnedShader, AnimatedModelShader.EffectPasses pass, bool computeTransform)

        {
            if (model == null)
                return;

            //
            // Compute all of the bone absolute transforms
            //
            if (modelExtra != null && hasSkinnedVertexType && hasNormals && hasTexCoords)
            {
                if (computeTransform)
                {

                    for (int i = 0; i < bones.Count; i++)
                    {
                        Bone bone = bones[i];
                        bone.ComputeAbsoluteTransform();

                        boneTransforms[i] = bone.AbsoluteTransform;
                    }

                    for (int s = 0; s < modelExtra.Skeleton.Count; s++)
                    {
                        Bone bone = bones[modelExtra.Skeleton[s]];
                        skeleton[s] = bone.SkinTransform * bone.AbsoluteTransform;
                    }
                }

                if (bones.Count > 1)
                {
                    switch (pass)
                    {
                        case AnimatedModelShader.EffectPasses.Unskinned:
                            pass = AnimatedModelShader.EffectPasses.Skinned;
                            break;
                        case AnimatedModelShader.EffectPasses.UnskinnedNormalMapped:
                            pass = AnimatedModelShader.EffectPasses.SkinnedNormalMapped;
                            break;
                        case AnimatedModelShader.EffectPasses.UnskinnedDepth:
                            pass = AnimatedModelShader.EffectPasses.SkinnedDepth;
                            break;
                    }
                }
                _skinnedShader.DrawMesh(model, world, view, projection, cameraPos, pass, skeleton);
            }
            else
            {

                if (!hasNormals) pass = AnimatedModelShader.EffectPasses.NoNormalUnskinned;

                if (!hasTexCoords)
                    pass = AnimatedModelShader.EffectPasses.NoNormalNoTexUnskinned;

                _skinnedShader.DrawMesh(model, world, view, projection, cameraPos, pass, null);
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

        private void computeCenter()
        {
            center = model.Meshes[0].BoundingSphere.Center;
            radius = model.Meshes[0].BoundingSphere.Radius;
        }
    }
}
