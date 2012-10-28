using System;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Example;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils.TgcSkeletalAnimation;
using TgcViewer.Utils;
using TgcViewer;

namespace AlumnoEjemplos.Grupo18
{
    public class BoundingBoxExtendida : IRenderObject, ITransformObject
    {

        #region Creacion
        String nombre;
        BoundingBoxExtendida boxinterna;
        BoundingBoxExtendida box_caja1;
        bool efecto = false;
        bool seleccionado = false;
        int R, U, count = 0;
        int Rorg = 2;

        float[,] matriz=new float[12,9];
        public Vector3 centerbkp;
        public Vector3 sizebkp;
        public Random rand = new Random(DateTime.Now.Millisecond);

        bool indeterminacion=false;

        public void setIndeterminacion(bool indeterminacion) {
            this.indeterminacion = indeterminacion;
        }
        public void setEfecto(bool efecto) {
            this.efecto = efecto;
        }

        TgcTexture normalMap;
        public TgcTexture NormalMap
        {
            get { return normalMap; }
            set { normalMap = value; }
        }
        Effect effect;
        /// <summary>
        /// Shader
        /// </summary>
        public Effect Effect
        {
            get { return effect; }
            set { effect = value; }
        }
        int[] alpha = new int[26] { 255, 235, 215, 185, 180, 170, 160, 150, 140, 135, 130, 125, 120, 119, 117, 110, 105,104, 101, 100, 100, 100, 100, 100, 0, 0 };
        public string getName()
        {
            return nombre;
        }

        public void setName(String nombre)
        {
            this.nombre = nombre;
        }
        public void setboxinterna(BoundingBoxExtendida box)
        {
            this.boxinterna = box;
        }
        public BoundingBoxExtendida getboxinterna()
        {
            return this.boxinterna;
        }
        public void setBox_caja1(BoundingBoxExtendida box)
        {
            this.box_caja1 = box;
        }
        public BoundingBoxExtendida getBox_caja1()
        {
            return this.box_caja1;
        }
        public void setSeleccionado(bool seleccionado)
        {
            this.seleccionado = seleccionado;
        }
        public bool getSeleccionado()
        {
            return seleccionado;
        }

        public void seleccionar(Device d3dDevice)
        {
            this.AlphaBlendEnable = true;
            efecto = true;
            TgcTexture text = TgcTexture.createTexture(d3dDevice, GuiController.Instance.AlumnoEjemplosMediaDir + "Grupo18\\" + this.getName() + ".jpg");
                     
            this.getboxinterna().setTexture(text);
            this.getBox_caja1().setEfecto(true);
            this.getBox_caja1().AlphaBlendEnable = true;
        }

        public void deseleccionar(Device d3dDevice)
        {
            this.AlphaBlendEnable = false;
            this.efecto = false;
            TgcTexture text = TgcTexture.createTexture(d3dDevice, GuiController.Instance.AlumnoEjemplosMediaDir + "Grupo18\\granito00.jpg");
            this.setTexture(text);

            this.setPositionSize(centerbkp, sizebkp);
            this.restartMatriz();
            this.getBox_caja1().AlphaBlendEnable = false;
            this.getBox_caja1().restartMatriz();
            this.getBox_caja1().setPositionSize(centerbkp, sizebkp);
            this.getBox_caja1().efecto = false;
        }
        public void relizarefecto(Device d3dDevice)
        {

            if (efecto && Rorg < 25)
            {
                count++;

                for (int c = 0; c < 12; c++)
                {
                    for (int d = 0; d < 9;d++ )
                    {
                        if ((Rorg % 2) == 0)
                        {
                            if (((c + d) % 2) == 0 && rand.Next(0, 2) == 1)
                            {
                                matriz[c, d]++;
                                //matriz[c, d]++;
                                matriz[c, d]++;
                            }
                            if (indeterminacion && ((c + d) % 2) == 0 && rand.Next(0, 2) == 1)
                            {
                                matriz[c, d]++;
                                //matriz[c, d]++;
                            }

                        }
                        else
                        {
                            if (((c + d) % 2) != 0)
                                matriz[c, d]++;
                        }
                    }
                }
                if (U == 2)
                {
                    R++;
                    Rorg++;
                }
                U++;
                if (U > 2 ) U = 0;

                    int color = d3dDevice.RenderState.TextureFactor;
                    Color origColor = System.Drawing.Color.Blue;

                    d3dDevice.RenderState.TextureFactor = Color.FromArgb(alpha[Rorg], origColor.R, origColor.G, origColor.B).ToArgb();
                    //d3dDevice.RenderState.ReferenceAlpha = 0x00;
                    d3dDevice.TextureState[0].AlphaOperation = TextureOperation.Modulate;
                    d3dDevice.TextureState[0].AlphaArgument1 = TextureArgument.TextureColor;
                    d3dDevice.TextureState[0].AlphaArgument2 = TextureArgument.TFactor;
                    //d3dDevice.TextureState[1].AlphaOperation = TextureOperation.Modulate;
                    //d3dDevice.TextureState[1].AlphaArgument1 = TextureArgument.TextureColor;
                    //d3dDevice.TextureState[1].AlphaArgument2 = TextureArgument.TFactor;
                    //d3dDevice.TextureState[2].AlphaOperation = TextureOperation.Modulate;
                    //d3dDevice.TextureState[2].AlphaArgument1 = TextureArgument.TextureColor;
                    //d3dDevice.TextureState[2].AlphaArgument2 = TextureArgument.TFactor;
                    //d3dDevice.TextureState[3].AlphaOperation = TextureOperation.Modulate;
                    //d3dDevice.TextureState[3].AlphaArgument1 = TextureArgument.TextureColor;
                    //d3dDevice.TextureState[3].AlphaArgument2 = TextureArgument.TFactor;


            }
            if (!efecto) Rorg = 0;
        }



        public static BoundingBoxExtendida fromSize(Vector3 center, Vector3 size)
        {
            BoundingBoxExtendida box = new BoundingBoxExtendida();
            box.setPositionSize(center, size);
            box.updateValues();
            return box;
        }

        /// <summary>
        /// Crea una caja con el centro y tamaño especificado, con el color especificado
        /// </summary>
        /// <param name="center">Centro de la caja</param>
        /// <param name="size">Tamaño de la caja</param>
        /// <param name="color">Color de la caja</param>
        /// <returns>Caja creada</returns>
        public static BoundingBoxExtendida fromSize(Vector3 center, Vector3 size, Color color)
        {
            BoundingBoxExtendida box = new BoundingBoxExtendida();
            box.setPositionSize(center, size);
            box.color = color;
            box.updateValues();
            return box;
        }

        /// <summary>
        /// Crea una caja con el centro y tamaño especificado, con la textura especificada
        /// </summary>
        /// <param name="center">Centro de la caja</param>
        /// <param name="size">Tamaño de la caja</param>
        /// <param name="texture">Textura de la caja</param>
        /// <returns>Caja creada</returns>
        public static BoundingBoxExtendida fromSize(Vector3 center, Vector3 size, TgcTexture texture)
        {
            BoundingBoxExtendida box = BoundingBoxExtendida.fromSize(center, size);
            
            box.centerbkp = center;
            box.sizebkp= size;
            box.setboxinterna(BoundingBoxExtendida.fromSize(center, size));
            box.setBox_caja1(BoundingBoxExtendida.fromSize(center, size));
            box.getBox_caja1().setTexture(texture);
            box.getBox_caja1().setIndeterminacion(true);
            box.setTexture(texture);
            return box;
        }

        /// <summary>
        /// Crea una caja con centro (0,0,0) y el tamaño especificado
        /// </summary>
        /// <param name="size">Tamaño de la caja</param>
        /// <returns>Caja creada</returns>
        public static BoundingBoxExtendida fromSize(Vector3 size)
        {
            return BoundingBoxExtendida.fromSize(new Vector3(0, 0, 0), size);
        }

        /// <summary>
        /// Crea una caja con centro (0,0,0) y el tamaño especificado, con el color especificado
        /// </summary>
        /// <param name="size">Tamaño de la caja</param>
        /// <param name="color">Color de la caja</param>
        /// <returns>Caja creada</returns>
        public static BoundingBoxExtendida fromSize(Vector3 size, Color color)
        {
            return BoundingBoxExtendida.fromSize(new Vector3(0, 0, 0), size, color);
        }

        /// <summary>
        /// Crea una caja con centro (0,0,0) y el tamaño especificado, con la textura especificada
        /// </summary>
        /// <param name="size">Tamaño de la caja</param>
        /// <param name="texture">Textura de la caja</param>
        /// <returns>Caja creada</returns>
        public static BoundingBoxExtendida fromSize(Vector3 size, TgcTexture texture)
        {
            return BoundingBoxExtendida.fromSize(new Vector3(0, 0, 0), size, texture);
        }

        /// <summary>
        /// Crea una caja en base al punto minimo y maximo
        /// </summary>
        /// <param name="pMin">Punto mínimo</param>
        /// <param name="pMax">Punto máximo</param>
        /// <returns>Caja creada</returns>
        public static BoundingBoxExtendida fromExtremes(Vector3 pMin, Vector3 pMax)
        {
            Vector3 size = Vector3.Subtract(pMax, pMin);
            Vector3 midSize = Vector3.Scale(size, 0.5f);
            Vector3 center = pMin + midSize;
            return BoundingBoxExtendida.fromSize(center, size);
        }

        /// <summary>
        /// Crea una caja en base al punto minimo y maximo, con el color especificado
        /// </summary>
        /// <param name="pMin">Punto mínimo</param>
        /// <param name="pMax">Punto máximo</param>
        /// <param name="color">Color de la caja</param>
        /// <returns>Caja creada</returns>
        public static BoundingBoxExtendida fromExtremes(Vector3 pMin, Vector3 pMax, Color color)
        {
            BoundingBoxExtendida box = BoundingBoxExtendida.fromExtremes(pMin, pMax);
            box.color = color;
            box.updateValues();
            return box;
        }

        /// <summary>
        /// Crea una caja en base al punto minimo y maximo, con el color especificado
        /// </summary>
        /// <param name="pMin">Punto mínimo</param>
        /// <param name="pMax">Punto máximo</param>
        /// <param name="texture">Textura de la caja</param>
        /// <returns>Caja creada</returns>
        public static BoundingBoxExtendida fromExtremes(Vector3 pMin, Vector3 pMax, TgcTexture texture)
        {
            BoundingBoxExtendida box = BoundingBoxExtendida.fromExtremes(pMin, pMax);
            box.setTexture(texture);
            return box;
        }

        #endregion


        CustomVertex.PositionColoredTextured[] vertices;
        VertexBuffer vertexBuffer;

        Vector3 size;
        /// <summary>
        /// Dimensiones de la caja
        /// </summary>
        public Vector3 Size
        {
            get { return size; }
            set
            {
                size = value;
                updateBoundingBox();
            }
        }
        public void restartMatriz() {
            for (int c = 0; c < 12; c++)
            {
                for (int d = 0; d < 9; d++)
                { matriz[c, d] = 0; }
            }
        }
        /// <summary>
        /// Escala de la caja. Siempre es (1, 1, 1).
        /// Utilizar Size
        /// </summary>
        public Vector3 Scale
        {
            get { return new Vector3(1, 1, 1); }
            set { ; }
        }

        Color color;
        /// <summary>
        /// Color de los vértices de la caja
        /// </summary>
        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        TgcTexture texture;
        /// <summary>
        /// Textura de la caja
        /// </summary>
        public TgcTexture Texture
        {
            get { return texture; }
        }

        Matrix transform;
        /// <summary>
        /// Matriz final que se utiliza para aplicar transformaciones a la malla.
        /// Si la propiedad AutoTransformEnable esta en True, la matriz se reconstruye en cada cuadro
        /// en base a los valores de: Position, Rotation, Scale.
        /// Si AutoTransformEnable está en False, se respeta el valor que el usuario haya cargado en la matriz.
        /// </summary>
        public Matrix Transform
        {
            get { return transform; }
            set { transform = value; }
        }

        bool autoTransformEnable;
        /// <summary>
        /// En True hace que la matriz de transformacion (Transform) de la malla se actualiza en
        /// cada cuadro en forma automática, según los valores de: Position, Rotation, Scale.
        /// En False se respeta lo que el usuario haya cargado a mano en la matriz.
        /// Por default está en True.
        /// </summary>
        public bool AutoTransformEnable
        {
            get { return autoTransformEnable; }
            set { autoTransformEnable = value; }
        }

        private Vector3 translation;
        /// <summary>
        /// Posicion absoluta del centro de la caja
        /// </summary>
        public Vector3 Position
        {
            get { return translation; }
            set
            {
                translation = value;
                updateBoundingBox();
            }
        }

        private Vector3 rotation;
        /// <summary>
        /// Rotación absoluta de la caja
        /// </summary>
        public Vector3 Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        private bool enabled;
        /// <summary>
        /// Indica si la caja esta habilitada para ser renderizada
        /// </summary>
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }


        private TgcBoundingBox boundingBox;
        /// <summary>
        /// BoundingBox de la caja
        /// </summary>
        public TgcBoundingBox BoundingBox
        {
            get { return boundingBox; }
        }

        private bool alphaBlendEnable;
        /// <summary>
        /// Habilita el renderizado con AlphaBlending para los modelos
        /// con textura o colores por vértice de canal Alpha.
        /// Por default está deshabilitado.
        /// </summary>
        public bool AlphaBlendEnable
        {
            get { return alphaBlendEnable; }
            set { alphaBlendEnable = value; }
        }

        Vector2 uvOffset;
        /// <summary>
        /// Offset UV de textura
        /// </summary>
        public Vector2 UVOffset
        {
            get { return uvOffset; }
            set { uvOffset = value; }
        }

        Vector2 uvTiling;
        /// <summary>
        /// Tiling UV de textura
        /// </summary>
        public Vector2 UVTiling
        {
            get { return uvTiling; }
            set { uvTiling = value; }
        }


        /// <summary>
        /// Crea una caja vacia
        /// </summary>
        public BoundingBoxExtendida()
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;

            vertices = new CustomVertex.PositionColoredTextured[144];
            vertexBuffer = new VertexBuffer(typeof(CustomVertex.PositionColoredTextured), 144, d3dDevice,
                Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionColoredTextured.Format, Pool.Default);

            this.autoTransformEnable = true;
            this.transform = Matrix.Identity;
            this.translation = new Vector3(0, 0, 0);
            this.rotation = new Vector3(0, 0, 0);
            this.enabled = true;
            this.color = Color.White;
            this.alphaBlendEnable = false;
            this.uvOffset = new Vector2(0, 0);
            this.uvTiling = new Vector2(1, 1);

            //BoundingBox
            boundingBox = new TgcBoundingBox();
        }

        /// <summary>
        /// Actualiza la caja en base a los valores configurados
        /// </summary>
        public void updateValues()
        {
            int c = color.ToArgb();
            float x = size.X / 2;
            float y = size.Y / 2;
            float z = size.Z / 2;
            float u = /*1f*/uvTiling.X;
            float v = /*1f*/uvTiling.Y;
            float offsetU = uvOffset.X;
            float offsetV = uvOffset.Y;

            // Front face
            vertices[0] = new CustomVertex.PositionColoredTextured(-x + matriz[0, 0], y - matriz[0, 1], z + matriz[0, 2], c, offsetU, offsetV);
            vertices[1] = new CustomVertex.PositionColoredTextured(-x + matriz[0, 3], -y + matriz[0, 4], z + matriz[0, 5], c, offsetU, offsetV + v);
            vertices[2] = new CustomVertex.PositionColoredTextured(x - matriz[0, 6], y - matriz[0, 7], z + matriz[0, 8], c, offsetU + u, offsetV);

            vertices[3] = new CustomVertex.PositionColoredTextured(-x + matriz[1, 0], -y + matriz[1, 1], z + matriz[1, 2], c, offsetU, offsetV + v);
            vertices[4] = new CustomVertex.PositionColoredTextured(x - matriz[1, 3], -y + matriz[1, 4], z + matriz[1, 5], c, offsetU + u, offsetV + v);
            vertices[5] = new CustomVertex.PositionColoredTextured(x - matriz[1, 6], y - matriz[1, 7], z + matriz[1, 8], c, offsetU + u, offsetV);

            // Back face (remember this is facing *away* from the camera, so vertices should be clockwise order)
            vertices[6] = new CustomVertex.PositionColoredTextured(-x + matriz[2, 0], y - matriz[2, 1], -z - matriz[2, 2], c, offsetU, offsetV);
            vertices[7] = new CustomVertex.PositionColoredTextured(x - matriz[2, 3], y - matriz[2, 4], -z - matriz[2, 5], c, offsetU + u, offsetV);
            vertices[8] = new CustomVertex.PositionColoredTextured(-x + matriz[2, 6], -y + matriz[2, 7], -z - matriz[2, 8], c, offsetU, offsetV + v);
            vertices[9] = new CustomVertex.PositionColoredTextured(-x + matriz[3, 0], -y + matriz[3, 1], -z - matriz[3, 2], c, offsetU, offsetV + v);
            vertices[10] = new CustomVertex.PositionColoredTextured(x - matriz[3, 3], y - matriz[3, 4], -z - matriz[3, 5], c, offsetU + u, offsetV);
            vertices[11] = new CustomVertex.PositionColoredTextured(x - matriz[3, 6], -y + matriz[3, 7], -z - matriz[3, 8], c, offsetU + u, offsetV + v);

            // Top face
            vertices[12] = new CustomVertex.PositionColoredTextured(-x + matriz[4, 0], y + matriz[4, 1], z - matriz[4, 2], c, offsetU, offsetV);
            vertices[13] = new CustomVertex.PositionColoredTextured(x - matriz[4, 3], y + matriz[4, 4], -z + matriz[4, 5], c, offsetU + u, offsetV + v);
            vertices[14] = new CustomVertex.PositionColoredTextured(-x + matriz[4, 6], y + matriz[4, 7], -z + matriz[4, 8], c, offsetU, offsetV + v);

            vertices[15] = new CustomVertex.PositionColoredTextured(-x + matriz[5, 0], y + matriz[5, 1], z - matriz[5, 2], c, offsetU, offsetV);
            vertices[16] = new CustomVertex.PositionColoredTextured(x - matriz[5, 3], y + matriz[5, 4], z - matriz[5, 5], c, offsetU + u, offsetV);
            vertices[17] = new CustomVertex.PositionColoredTextured(x - matriz[5, 6], y + matriz[5, 7], -z + matriz[5, 8], c, offsetU + u, offsetV + v);

            // Bottom face (remember this is facing *away* from the camera, so vertices should be clockwise order)
            vertices[18] = new CustomVertex.PositionColoredTextured(-x + matriz[6, 0], -y - matriz[6, 1], z - matriz[6, 2], c, offsetU, offsetV);
            vertices[19] = new CustomVertex.PositionColoredTextured(-x + matriz[6, 3], -y - matriz[6, 4], -z+matriz[6, 5], c, offsetU, offsetV + v);
            vertices[20] = new CustomVertex.PositionColoredTextured(x - matriz[6, 6], -y - matriz[6, 7], -z + matriz[6, 8], c, offsetU + u, offsetV + v);
            vertices[21] = new CustomVertex.PositionColoredTextured(-x + matriz[7, 0], -y - matriz[7, 1], z - matriz[7, 2], c, offsetU, offsetV);
            vertices[22] = new CustomVertex.PositionColoredTextured(x - matriz[7, 3], -y - matriz[7, 4], -z + matriz[7, 5], c, offsetU + u, offsetV + v);
            vertices[23] = new CustomVertex.PositionColoredTextured(x - matriz[7, 6], -y - matriz[7, 7], z - matriz[7, 8], c, offsetU + u, offsetV);

            // Left face
            vertices[24] = new CustomVertex.PositionColoredTextured(-x - matriz[8, 0], y - matriz[8, 1], z-matriz[8, 2], c, offsetU, offsetV);
            vertices[25] = new CustomVertex.PositionColoredTextured(-x - matriz[8, 3], -y+matriz[8, 4], -z+matriz[8, 5], c, offsetU + u, offsetV + v);
            vertices[26] = new CustomVertex.PositionColoredTextured(-x - matriz[8, 6], -y+matriz[8, 7], z-matriz[8, 8], c, offsetU, offsetV + v);

            vertices[27] = new CustomVertex.PositionColoredTextured(-x - matriz[9, 0], y-matriz[9, 1], -z+matriz[9, 2], c, offsetU + u, offsetV);
            vertices[28] = new CustomVertex.PositionColoredTextured(-x - matriz[9, 3], -y+matriz[9, 4], -z+matriz[9, 5], c, offsetU + u, offsetV + v);
            vertices[29] = new CustomVertex.PositionColoredTextured(-x - matriz[9, 6], y-matriz[9, 7], z-matriz[9, 8], c, offsetU, offsetV);

            // Right face (remember this is facing *away* from the camera, so vertices should be clockwise order)
            vertices[30] = new CustomVertex.PositionColoredTextured(x + matriz[10, 0], y - matriz[10, 1], z - matriz[10, 2], c, offsetU, offsetV);
            vertices[31] = new CustomVertex.PositionColoredTextured(x + matriz[10, 3], -y + matriz[10, 4], z - matriz[10, 5], c, offsetU, offsetV + v);
            vertices[32] = new CustomVertex.PositionColoredTextured(x + matriz[10, 6], -y + matriz[10, 7], -z + matriz[10, 8], c, offsetU + u, offsetV + v);

            vertices[33] = new CustomVertex.PositionColoredTextured(x + matriz[11, 0], y - matriz[11, 1], -z + matriz[11, 2], c, offsetU + u, offsetV);
            vertices[34] = new CustomVertex.PositionColoredTextured(x + matriz[11, 3], y - matriz[11, 4], z - matriz[11, 5], c, offsetU, offsetV);
            vertices[35] = new CustomVertex.PositionColoredTextured(x + matriz[11, 6], -y + matriz[11, 7], -z + matriz[11, 8], c, offsetU + u, offsetV + v);


            //if (efecto)
            //{

            int p = 16;
            float i = 0.1f;
            Matrix rot = Matrix.RotationX(matriz[0, 0]/p);  //ojo que es en radianes
            // Front face
            vertices[36] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(-x + matriz[0, 0] * i, y - matriz[0, 1] * i, z + matriz[0, 2] * i), rot), c, offsetU, offsetV);
            vertices[37] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(-x + matriz[0, 3] * i, -y + matriz[0, 4] * i, z + matriz[0, 5] * i), rot), c, offsetU, offsetV + v);
            vertices[38] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(x - matriz[0, 6] * i, y - matriz[0, 7] * i, z + matriz[0, 8] * i), rot), c, offsetU + u, offsetV);
            vertices[39] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(-x + matriz[1, 0] * i, -y + matriz[1, 1] * i, z + matriz[1, 2] * i), rot), c, offsetU, offsetV + v);
            vertices[40] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(x - matriz[1, 3] * i, -y + matriz[1, 4] * i, z + matriz[1, 5] * i), rot), c, offsetU + u, offsetV + v);
            vertices[41] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(x - matriz[1, 6] * i, y - matriz[1, 7] * i, z + matriz[1, 8] * i), rot), c, offsetU + u, offsetV);
            i = 0.2f;
            rot = Matrix.RotationY(matriz[2, 3]/p);  //ojo que es en radianes
            // Back face (remember this is facing *away* from the camera, so vertices should be clockwise order)
            vertices[42] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(-x + matriz[2, 0] * i, y - matriz[2, 1] * i, -z - matriz[2, 2] * i), rot), c, offsetU, offsetV);
            vertices[43] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(x - matriz[2, 3] * i, y - matriz[2, 4] * i, -z - matriz[2, 5] * i), rot), c, offsetU + u, offsetV);
            vertices[44] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(-x + matriz[2, 6] * i, -y + matriz[2, 7] * i, -z - matriz[2, 8] * i), rot), c, offsetU, offsetV + v);
            vertices[45] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(-x + matriz[3, 0] * i, -y + matriz[3, 1] * i, -z - matriz[3, 2] * i), rot), c, offsetU, offsetV + v);
            vertices[46] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(x - matriz[3, 3] * i, y - matriz[3, 4] * i, -z - matriz[3, 5] * i), rot), c, offsetU + u, offsetV);
            vertices[47] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(x - matriz[3, 6] * i, -y + matriz[3, 7] * i, -z - matriz[3, 8] * i), rot), c, offsetU + u, offsetV + v);
            i = 0.3f;
            rot = Matrix.RotationZ(matriz[4, 3] / p);  //ojo que es en radianes
            // Top face
            vertices[48] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(-x + matriz[4, 0] * i, y + matriz[4, 1] * i, z - matriz[4, 2] * i), rot), c, offsetU, offsetV);
            vertices[49] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(x - matriz[4, 3] * i, y + matriz[4, 4] * i, -z + matriz[4, 5] * i), rot), c, offsetU + u, offsetV + v);
            vertices[50] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(-x + matriz[4, 6] * i, y + matriz[4, 7] * i, -z + matriz[4, 8] * i), rot), c, offsetU, offsetV + v);
            vertices[51] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(-x + matriz[5, 0] * i, y + matriz[5, 1] * i, z - matriz[5, 2] * i), rot), c, offsetU, offsetV);
            vertices[52] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(x - matriz[5, 3] * i, y + matriz[5, 4] * i, z - matriz[5, 5] * i), rot), c, offsetU + u, offsetV);
            vertices[53] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(x - matriz[5, 6] * i, y + matriz[5, 7] * i, -z + matriz[5, 8] * i), rot), c, offsetU + u, offsetV + v);
            i = 0.4f;
            rot = Matrix.RotationX(matriz[6, 3] / p);  //ojo que es en radianes
            // Bottom face (remember this is facing *away* from the camera, so vertices should be clockwise order)
            vertices[54] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(-x + matriz[6, 0] * i, -y - matriz[6, 1] * i, z - matriz[6, 2] * i), rot), c, offsetU, offsetV);
            vertices[55] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(-x + matriz[6, 3] * i, -y - matriz[6, 4] * i, -z + matriz[6, 5] * i), rot), c, offsetU, offsetV + v);
            vertices[56] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(x - matriz[6, 6] * i, -y - matriz[6, 7] * i, -z + matriz[6, 8] * i), rot), c, offsetU + u, offsetV + v);
            vertices[57] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(-x + matriz[7, 0] * i, -y - matriz[7, 1] * i, z - matriz[7, 2] * i), rot), c, offsetU, offsetV);
            vertices[58] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(x - matriz[7, 3] * i, -y - matriz[7, 4] * i, -z + matriz[7, 5] * i), rot), c, offsetU + u, offsetV + v);
            vertices[59] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(x - matriz[7, 6] * i, -y - matriz[7, 7] * i, z - matriz[7, 8] * i), rot), c, offsetU + u, offsetV);
            i = 0.5f;
            rot = Matrix.RotationX(matriz[8, 3] / p);  //ojo que es en radianes
            // vertices face
            vertices[60] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(-x - matriz[8, 0] * i, y - matriz[8, 1] * i, z - matriz[8, 2] * i), rot), c, offsetU, offsetV);
            vertices[61] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(-x - matriz[8, 3] * i, -y + matriz[8, 4] * i, -z + matriz[8, 5] * i), rot), c, offsetU + u, offsetV + v);
            vertices[62] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(-x - matriz[8, 6] * i, -y + matriz[8, 7] * i, z - matriz[8, 8] * i), rot), c, offsetU, offsetV + v);
            vertices[63] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(-x - matriz[9, 0] * i, y - matriz[9, 1] * i, -z + matriz[9, 2] * i), rot), c, offsetU + u, offsetV);
            vertices[64] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(-x - matriz[9, 3] * i, -y + matriz[9, 4] * i, -z + matriz[9, 5] * i), rot), c, offsetU + u, offsetV + v);
            vertices[65] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(-x - matriz[9, 6] * i, y - matriz[9, 7] * i, z - matriz[9, 8] * i), rot), c, offsetU, offsetV);
            i = 0.6f;
            rot = Matrix.RotationZ(matriz[10, 0] / p);  //ojo que es en radianes
            // Right face (remember this is facing *away* from the camera, so vertices should be clockwise order)
            vertices[66] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(x + matriz[10, 0] * i, y - matriz[10, 1] * i, z - matriz[10, 2] * i), rot), c, offsetU, offsetV);
            vertices[67] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(x + matriz[10, 3] * i, -y + matriz[10, 4] * i, z - matriz[10, 5] * i), rot), c, offsetU, offsetV + v);
            vertices[68] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(x + matriz[10, 6] * i, -y + matriz[10, 7] * i, -z + matriz[10, 8] * i), rot), c, offsetU + u, offsetV + v);
            vertices[69] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(x + matriz[11, 0] * i, y - matriz[11, 1] * i, -z + matriz[11, 2] * i), rot), c, offsetU + u, offsetV);
            vertices[70] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(x + matriz[11, 3] * i, y - matriz[11, 4] * i, z - matriz[11, 5] * i), rot), c, offsetU, offsetV);
            vertices[71] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(x + matriz[11, 6] * i, -y + matriz[11, 7] * i, -z + matriz[11, 8] * i), rot), c, offsetU + u, offsetV + v);


            i = 1.1f;
            rot = Matrix.RotationX(matriz[0, 6] / p);  //ojo que es en radianes
            // Front face
            vertices[72] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(-x + matriz[0, 0] * i, y - matriz[0, 1] * i, z + matriz[0, 2] * i), rot), c, offsetU, offsetV);
            vertices[73] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(-x + matriz[0, 3] * i, -y + matriz[0, 4] * i, z + matriz[0, 5] * i), rot), c, offsetU, offsetV + v);
            vertices[74] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(x - matriz[0, 6] * i, y - matriz[0, 7] * i, z + matriz[0, 8] * i), rot), c, offsetU + u, offsetV);
            vertices[75] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(-x + matriz[1, 0] * i, -y + matriz[1, 1] * i, z + matriz[1, 2] * i), rot), c, offsetU, offsetV + v);
            vertices[76] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(x - matriz[1, 3] * i, -y + matriz[1, 4] * i, z + matriz[1, 5] * i), rot), c, offsetU + u, offsetV + v);
            vertices[77] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(x - matriz[1, 6] * i, y - matriz[1, 7] * i, z + matriz[1, 8] * i), rot), c, offsetU + u, offsetV);
            i = 1.2f;
            rot = Matrix.RotationZ(matriz[2, 6] / p);  //ojo que es en radianes
            // Back face (remember this is facing *away* from the camera, so vertices should be clockwise order)
            vertices[78] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(-x + matriz[2, 0] * i, y - matriz[2, 1] * i, -z - matriz[2, 2] * i), rot), c, offsetU, offsetV);
            vertices[79] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(x - matriz[2, 3] * i, y - matriz[2, 4] * i, -z - matriz[2, 5] * i), rot), c, offsetU + u, offsetV);
            vertices[80] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(-x + matriz[2, 6] * i, -y + matriz[2, 7] * i, -z - matriz[2, 8] * i), rot), c, offsetU, offsetV + v);
            vertices[81] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(-x + matriz[3, 0] * i, -y + matriz[3, 1] * i, -z - matriz[3, 2] * i), rot), c, offsetU, offsetV + v);
            vertices[82] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(x - matriz[3, 3] * i, y - matriz[3, 4] * i, -z - matriz[3, 5] * i), rot), c, offsetU + u, offsetV);
            vertices[83] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(x - matriz[3, 6] * i, -y + matriz[3, 7] * i, -z - matriz[3, 8] * i), rot), c, offsetU + u, offsetV + v);
            i = 1.3f;
            rot = Matrix.RotationY(matriz[4, 0] / p);  //ojo que es en radianes
            // Top face
            vertices[84] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(-x + matriz[4, 0] * i, y + matriz[4, 1] * i, z - matriz[4, 2] * i), rot), c, offsetU, offsetV);
            vertices[85] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(x - matriz[4, 3] * i, y + matriz[4, 4] * i, -z + matriz[4, 5] * i), rot), c, offsetU + u, offsetV + v);
            vertices[86] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(-x + matriz[4, 6] * i, y + matriz[4, 7] * i, -z + matriz[4, 8] * i), rot), c, offsetU, offsetV + v);
            vertices[87] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(-x + matriz[5, 0] * i, y + matriz[5, 1] * i, z - matriz[5, 2] * i), rot), c, offsetU, offsetV);
            vertices[88] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(x - matriz[5, 3] * i, y + matriz[5, 4] * i, z - matriz[5, 5] * i), rot), c, offsetU + u, offsetV);
            vertices[89] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(x - matriz[5, 6] * i, y + matriz[5, 7] * i, -z + matriz[5, 8] * i), rot), c, offsetU + u, offsetV + v);
            i = 1.4f;
            rot = Matrix.RotationX(matriz[6, 0] / p);  //ojo que es en radianes
            // Bottom face (remember this is facing *away* from the camera, so vertices should be clockwise order)
            vertices[90] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(-x + matriz[6, 0] * i, -y - matriz[6, 1] * i, z - matriz[6, 2] * i), rot), c, offsetU, offsetV);
            vertices[91] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(-x + matriz[6, 3] * i, -y - matriz[6, 4] * i, -z + matriz[6, 5] * i), rot), c, offsetU, offsetV + v);
            vertices[92] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(x - matriz[6, 6] * i, -y - matriz[6, 7] * i, -z + matriz[6, 8] * i), rot), c, offsetU + u, offsetV + v);
            vertices[93] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(-x + matriz[7, 0] * i, -y - matriz[7, 1] * i, z - matriz[7, 2] * i), rot), c, offsetU, offsetV);
            vertices[94] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(x - matriz[7, 3] * i, -y - matriz[7, 4] * i, -z + matriz[7, 5] * i), rot), c, offsetU + u, offsetV + v);
            vertices[95] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(x - matriz[7, 6] * i, -y - matriz[7, 7] * i, z - matriz[7, 8] * i), rot), c, offsetU + u, offsetV);
            i = 1.5f;
            rot = Matrix.RotationZ(matriz[9, 0] / p);  //ojo que es en radianes
            // Left face
            vertices[96] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(-x - matriz[8, 0] * i, y - matriz[8, 1] * i, z - matriz[8, 2] * i), rot), c, offsetU, offsetV);
            vertices[97] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(-x - matriz[8, 3] * i, -y + matriz[8, 4] * i, -z + matriz[8, 5] * i), rot), c, offsetU + u, offsetV + v);
            vertices[98] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(-x - matriz[8, 6] * i, -y + matriz[8, 7] * i, z - matriz[8, 8] * i), rot), c, offsetU, offsetV + v);
            vertices[99] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(-x - matriz[9, 0] * i, y - matriz[9, 1] * i, -z + matriz[9, 2] * i), rot), c, offsetU + u, offsetV);
            vertices[100] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(-x - matriz[9, 3] * i, -y + matriz[9, 4] * i, -z + matriz[9, 5] * i), rot), c, offsetU + u, offsetV + v);
            vertices[101] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(-x - matriz[9, 6] * i, y - matriz[9, 7] * i, z - matriz[9, 8] * i), rot), c, offsetU, offsetV);
            i = 1.6f;
            rot = Matrix.RotationX(matriz[10, 0] / p);  //ojo que es en radianes
            // Right face (remember this is facing *away* from the camera, so vertices should be clockwise order)
            vertices[102] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(x + matriz[10, 0] * i, y - matriz[10, 1] * i, z - matriz[10, 2] * i), rot), c, offsetU, offsetV);
            vertices[103] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(x + matriz[10, 3] * i, -y + matriz[10, 4] * i, z - matriz[10, 5] * i), rot), c, offsetU, offsetV + v);
            vertices[104] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(x + matriz[10, 6] * i, -y + matriz[10, 7] * i, -z + matriz[10, 8] * i), rot), c, offsetU + u, offsetV + v);
            vertices[105] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(x + matriz[11, 0] * i, y - matriz[11, 1] * i, -z + matriz[11, 2] * i), rot), c, offsetU + u, offsetV);
            vertices[106] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(x + matriz[11, 3] * i, y - matriz[11, 4] * i, z - matriz[11, 5] * i), rot), c, offsetU, offsetV);
            vertices[107] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(x + matriz[11, 6] * i, -y + matriz[11, 7] * i, -z + matriz[11, 8] * i), rot), c, offsetU + u, offsetV + v);


            //NUEVOS TRIANGULOS
            i = 1.7f;
            p = 9;
            rot = Matrix.RotationX(matriz[0, 6] / p);
            // Front face
            vertices[108] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(-x + matriz[0, 0] * i, y - matriz[0, 1] * i, z + matriz[0, 2] * i), rot), c, offsetU, offsetV);
            vertices[109] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(-x + matriz[0, 3] * i, -y + matriz[0, 4] * i, z + matriz[0, 5] * i), rot), c, offsetU, offsetV + v);
            vertices[110] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(x - matriz[0, 6] * i, y - matriz[0, 7] * i, z + matriz[0, 8] * i), rot), c, offsetU + u, offsetV);

            vertices[111] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(-x + matriz[1, 0] * i, -y + matriz[1, 1] * i, z + matriz[1, 2] * i), rot), c, offsetU, offsetV + v);
            vertices[112] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(x - matriz[1, 3] * i, -y + matriz[1, 4] * i, z + matriz[1, 5] * i), rot), c, offsetU + u, offsetV + v);
            vertices[113] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(x - matriz[1, 6] * i, y - matriz[1, 7] * i, z + matriz[1, 8] * i), rot), c, offsetU + u, offsetV);
            i = 1.6f;
            rot = Matrix.RotationY(matriz[0, 6] / p);
            // Back face (remember this is facing *away* from the camera, so vertices should be clockwise order)
            vertices[114] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(-x + matriz[2, 0] * i, y - matriz[2, 1] * i, -z - matriz[2, 2] * i), rot), c, offsetU, offsetV);
            vertices[115] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(x - matriz[2, 3] * i, y - matriz[2, 4] * i, -z - matriz[2, 5] * i), rot), c, offsetU + u, offsetV);
            vertices[116] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(-x + matriz[2, 6] * i, -y + matriz[2, 7] * i, -z - matriz[2, 8] * i), rot), c, offsetU, offsetV + v);
            vertices[117] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(-x + matriz[3, 0] * i, -y + matriz[3, 1] * i, -z - matriz[3, 2] * i), rot), c, offsetU, offsetV + v);
            vertices[118] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(x - matriz[3, 3] * i, y - matriz[3, 4] * i, -z - matriz[3, 5] * i), rot), c, offsetU + u, offsetV);
            vertices[119] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(x - matriz[3, 6] * i, -y + matriz[3, 7] * i, -z - matriz[3, 8] * i), rot), c, offsetU + u, offsetV + v);
            i = 1.5f;
            rot = Matrix.RotationZ(matriz[0, 6] / p);
            // Top face
            vertices[120] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(-x + matriz[4, 0] * i, y + matriz[4, 1] * i, z - matriz[4, 2] * i), rot), c, offsetU, offsetV);
            vertices[121] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(x - matriz[4, 3] * i, y + matriz[4, 4] * i, -z + matriz[4, 5] * i), rot), c, offsetU + u, offsetV + v);
            vertices[122] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(-x + matriz[4, 6] * i, y + matriz[4, 7] * i, -z + matriz[4, 8] * i), rot), c, offsetU, offsetV + v);
            i = 1.5f;
            rot = Matrix.RotationX(matriz[5, 3] / p);
            vertices[123] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(-x + matriz[5, 0] * i, y + matriz[5, 1] * i, z - matriz[5, 2] * i), rot), c, offsetU, offsetV);
            vertices[124] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(x - matriz[5, 3] * i, y + matriz[5, 4] * i, z - matriz[5, 5] * i), rot), c, offsetU + u, offsetV);
            vertices[125] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(x - matriz[5, 6] * i, y + matriz[5, 7] * i, -z + matriz[5, 8] * i), rot), c, offsetU + u, offsetV + v);
            i = 1.4f;
            rot = Matrix.RotationY(matriz[5, 0] / p);
            // Bottom face (remember this is facing *away* from the camera, so vertices should be clockwise order)
            vertices[126] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(-x + matriz[6, 0] * i, -y - matriz[6, 1] * i, z - matriz[6, 2] * i), rot), c, offsetU, offsetV);
            vertices[127] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(-x + matriz[6, 3] * i, -y - matriz[6, 4] * i, -z + matriz[6, 5] * i), rot), c, offsetU, offsetV + v);
            vertices[128] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(x - matriz[6, 6] * i, -y - matriz[6, 7] * i, -z + matriz[6, 8] * i), rot), c, offsetU + u, offsetV + v);
            vertices[128] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(-x + matriz[7, 0] * i, -y - matriz[7, 1] * i, z - matriz[7, 2] * i), rot), c, offsetU, offsetV);
            vertices[129] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(x - matriz[7, 3] * i, -y - matriz[7, 4] * i, -z + matriz[7, 5] * i), rot), c, offsetU + u, offsetV + v);
            vertices[130] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(x - matriz[7, 6] * i, -y - matriz[7, 7] * i, z - matriz[7, 8] * i), rot), c, offsetU + u, offsetV);
            i = 1.2f;
            rot = Matrix.RotationZ(matriz[8, 3] / p);
            // vertices face
            vertices[131] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(-x - matriz[8, 0] * i, y - matriz[8, 1] * i, z - matriz[8, 2] * i), rot), c, offsetU, offsetV);
            vertices[132] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(-x - matriz[8, 3] * i, -y + matriz[8, 4] * i, -z + matriz[8, 5] * i), rot), c, offsetU + u, offsetV + v);
            vertices[133] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(-x - matriz[8, 6] * i, -y + matriz[8, 7] * i, z - matriz[8, 8] * i), rot), c, offsetU, offsetV + v);

            vertices[134] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(-x - matriz[9, 0] * i, y - matriz[9, 1] * i, -z + matriz[9, 2] * i), rot), c, offsetU + u, offsetV);
            vertices[135] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(-x - matriz[9, 3] * i, -y + matriz[9, 4] * i, -z + matriz[9, 5] * i), rot), c, offsetU + u, offsetV + v);
            vertices[136] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(-x - matriz[9, 6] * i, y - matriz[9, 7] * i, z - matriz[9, 8] * i), rot), c, offsetU, offsetV);
            i = 1.1f;
            rot = Matrix.RotationX(matriz[10, 6] / p);
            // Right face (remember this is facing *away* from the camera, so vertices should be clockwise order)
            vertices[137] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(x + matriz[10, 0] * i, y - matriz[10, 1] * i, z - matriz[10, 2] * i), rot), c, offsetU, offsetV);
            vertices[138] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(x + matriz[10, 3] * i, -y + matriz[10, 4] * i, z - matriz[10, 5] * i), rot), c, offsetU, offsetV + v);
            vertices[139] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(x + matriz[10, 6] * i, -y + matriz[10, 7] * i, -z + matriz[10, 8] * i), rot), c, offsetU + u, offsetV + v);
            vertices[140] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(x + matriz[11, 0] * i, y - matriz[11, 1] * i, -z + matriz[11, 2] * i), rot), c, offsetU + u, offsetV);
            vertices[141] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(x + matriz[11, 3] * i, y - matriz[11, 4] * i, z - matriz[11, 5] * i), rot), c, offsetU, offsetV);
            vertices[142] = new CustomVertex.PositionColoredTextured(Vector3.TransformCoordinate(new Vector3(x + matriz[11, 6] * i, -y + matriz[11, 7] * i, -z + matriz[11, 8] * i), rot), c, offsetU + u, offsetV + v);
            //}

            vertexBuffer.SetData(vertices, 0, LockFlags.None);
        }



        /// <summary>
        /// Configurar textura de la pared
        /// </summary>
        public void setTexture(TgcTexture texture)
        {
            if (this.texture != null)
            {
                this.texture.dispose();
            }
            this.texture = texture;
        }


        /// <summary>
        /// Renderizar la caja
        /// </summary>
        public void render()
        {
            if (!enabled)
                return;

            Device d3dDevice = GuiController.Instance.D3dDevice;
            TgcTexture.Manager texturesManager = GuiController.Instance.TexturesManager;

            //transformacion
            if (autoTransformEnable)
            {
                this.transform = Matrix.RotationYawPitchRoll(rotation.Y, rotation.X, rotation.Z) * Matrix.Translation(translation);
            }
            d3dDevice.Transform.World = this.transform;

            //Activar AlphaBlending
            activateAlphaBlend();

            //renderizar
            if (texture != null)
            {
                texturesManager.set(0, texture);
            }
            else
            {
                texturesManager.clear(0);
            }

            texturesManager.clear(1);
            d3dDevice.Material = TgcD3dDevice.DEFAULT_MATERIAL;

            d3dDevice.VertexFormat = CustomVertex.PositionColoredTextured.Format;
            d3dDevice.SetStreamSource(0, vertexBuffer, 0);
            d3dDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 36);

            //Desactivar AlphaBlend
            resetAlphaBlend();

       }

        /// <summary>
        /// Activar AlphaBlending, si corresponde
        /// </summary>
        protected void activateAlphaBlend()
        {
            Device device = GuiController.Instance.D3dDevice;
            if (alphaBlendEnable)
            {
                device.RenderState.AlphaTestEnable = true;
                device.RenderState.AlphaBlendEnable = true;
            }
        }

        /// <summary>
        /// Desactivar AlphaBlending
        /// </summary>
        protected void resetAlphaBlend()
        {
            Device device = GuiController.Instance.D3dDevice;
            device.RenderState.AlphaTestEnable = false;
            device.RenderState.AlphaBlendEnable = false;
        }

        /// <summary>
        /// Liberar los recursos de la cja
        /// </summary>
        public void dispose()
        {
            if (texture != null)
            {
                texture.dispose();
            }
            if (vertexBuffer != null && !vertexBuffer.Disposed)
            {
                vertexBuffer.Dispose();
            }
            boundingBox.dispose();
        }

        /// <summary>
        /// Configurar valores de posicion y tamaño en forma conjunta
        /// </summary>
        /// <param name="position">Centro de la caja</param>
        /// <param name="size">Tamaño de la caja</param>
        public void setPositionSize(Vector3 position, Vector3 size)
        {
            this.translation = position;
            this.size = size;
            updateBoundingBox();
        }

        /// <summary>
        /// Configurar punto mínimo y máximo del box
        /// </summary>
        /// <param name="min">Min</param>
        /// <param name="max">Max</param>
        public void setExtremes(Vector3 min, Vector3 max)
        {
            Vector3 size = Vector3.Subtract(max, min);
            Vector3 midSize = Vector3.Scale(size, 0.5f);
            Vector3 center = min + midSize;
            setPositionSize(center, size);
        }

        /// <summary>
        /// Desplaza la malla la distancia especificada, respecto de su posicion actual
        /// </summary>
        public void move(Vector3 v)
        {
            this.move(v.X, v.Y, v.Z);
        }

        /// <summary>
        /// Desplaza la malla la distancia especificada, respecto de su posicion actual
        /// </summary>
        public void move(float x, float y, float z)
        {
            this.translation.X += x;
            this.translation.Y += y;
            this.translation.Z += z;

            updateBoundingBox();
        }

        /// <summary>
        /// Mueve la malla en base a la orientacion actual de rotacion.
        /// Es necesario rotar la malla primero
        /// </summary>
        /// <param name="movement">Desplazamiento. Puede ser positivo (hacia adelante) o negativo (hacia atras)</param>
        public void moveOrientedY(float movement)
        {
            float z = (float)Math.Cos((float)rotation.Y) * movement;
            float x = (float)Math.Sin((float)rotation.Y) * movement;

            move(x, 0, z);
        }

        /// <summary>
        /// Obtiene la posicion absoluta de la malla, recibiendo un vector ya creado para
        /// almacenar el resultado
        /// </summary>
        /// <param name="pos">Vector ya creado en el que se carga el resultado</param>
        public void getPosition(Vector3 pos)
        {
            pos.X = translation.X;
            pos.Y = translation.Y;
            pos.Z = translation.Z;
        }

        /// <summary>
        /// Rota la malla respecto del eje X
        /// </summary>
        /// <param name="angle">Ángulo de rotación en radianes</param>
        public void rotateX(float angle)
        {
            this.rotation.X += angle;
        }

        /// <summary>
        /// Rota la malla respecto del eje Y
        /// </summary>
        /// <param name="angle">Ángulo de rotación en radianes</param>
        public void rotateY(float angle)
        {
            this.rotation.Y += angle;
        }

        /// <summary>
        /// Rota la malla respecto del eje Z
        /// </summary>
        /// <param name="angle">Ángulo de rotación en radianes</param>
        public void rotateZ(float angle)
        {
            this.rotation.Z += angle;
        }

        /// <summary>
        /// Actualiza el BoundingBox de la caja.
        /// No contempla rotacion
        /// </summary>
        private void updateBoundingBox()
        {
            Vector3 midSize = Vector3.Scale(size, 0.5f);
            boundingBox.setExtremes(Vector3.Subtract(translation, midSize), Vector3.Add(translation, midSize));
        }

        /// <summary>
        /// Convierte el box en un TgcMesh
        /// </summary>
        /// <param name="meshName">Nombre de la malla que se va a crear</param>
        public TgcMesh toMesh(string meshName)
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;

            //Obtener matriz para transformar vertices
            if (autoTransformEnable)
            {
                this.transform = Matrix.RotationYawPitchRoll(rotation.Y, rotation.X, rotation.Z) * Matrix.Translation(translation);
            }

            //Crear mesh con DiffuseMap
            if (texture != null)
            {
                //Crear Mesh
                Mesh d3dMesh = new Mesh(vertices.Length / 3, vertices.Length, MeshFlags.Managed, TgcViewer.Utils.TgcSceneLoader.TgcSceneLoader.DiffuseMapVertexElements, d3dDevice);

                //Cargar VertexBuffer
                using (VertexBuffer vb = d3dMesh.VertexBuffer)
                {
                    GraphicsStream data = vb.Lock(0, 0, LockFlags.None);
                    for (int j = 0; j < vertices.Length; j++)
                    {
                        TgcViewer.Utils.TgcSceneLoader.TgcSceneLoader.DiffuseMapVertex v = new TgcViewer.Utils.TgcSceneLoader.TgcSceneLoader.DiffuseMapVertex();
                        CustomVertex.PositionColoredTextured vBox = vertices[j];

                        //vertices
                        v.Position = Vector3.TransformCoordinate(vBox.Position, this.transform);

                        //normals
                        v.Normal = Vector3.Empty;

                        //texture coordinates diffuseMap
                        v.Tu = vBox.Tu;
                        v.Tv = vBox.Tv;

                        //color
                        v.Color = vBox.Color;

                        data.Write(v);
                    }
                    vb.Unlock();
                }

                //Cargar IndexBuffer en forma plana
                using (IndexBuffer ib = d3dMesh.IndexBuffer)
                {
                    short[] indices = new short[vertices.Length];
                    for (int j = 0; j < indices.Length; j++)
                    {
                        indices[j] = (short)j;
                    }
                    ib.SetData(indices, 0, LockFlags.None);
                }

                //Calcular normales
                d3dMesh.ComputeNormals();

                //Malla de TGC
                TgcMesh tgcMesh = new TgcMesh(d3dMesh, meshName, TgcMesh.MeshRenderType.DIFFUSE_MAP);
                tgcMesh.DiffuseMaps = new TgcTexture[] { texture };
                tgcMesh.Materials = new Material[] { TgcD3dDevice.DEFAULT_MATERIAL };
                tgcMesh.createBoundingBox();
                tgcMesh.Enabled = true;
                return tgcMesh;
            }

            //Crear mesh con solo color
            else
            {
                //Crear Mesh
                Mesh d3dMesh = new Mesh(vertices.Length / 3, vertices.Length, MeshFlags.Managed, TgcViewer.Utils.TgcSceneLoader.TgcSceneLoader.VertexColorVertexElements, d3dDevice);

                //Cargar VertexBuffer
                using (VertexBuffer vb = d3dMesh.VertexBuffer)
                {
                    GraphicsStream data = vb.Lock(0, 0, LockFlags.None);
                    for (int j = 0; j < vertices.Length; j++)
                    {
                        TgcViewer.Utils.TgcSceneLoader.TgcSceneLoader.VertexColorVertex v = new TgcViewer.Utils.TgcSceneLoader.TgcSceneLoader.VertexColorVertex();
                        CustomVertex.PositionColoredTextured vBox = vertices[j];

                        //vertices
                        v.Position = Vector3.TransformCoordinate(vBox.Position, this.transform);

                        //normals
                        v.Normal = Vector3.Empty;

                        //color
                        v.Color = vBox.Color;

                        data.Write(v);
                    }
                    vb.Unlock();
                }

                //Cargar IndexBuffer en forma plana
                using (IndexBuffer ib = d3dMesh.IndexBuffer)
                {
                    short[] indices = new short[vertices.Length];
                    for (int j = 0; j < indices.Length; j++)
                    {
                        indices[j] = (short)j;
                    }
                    ib.SetData(indices, 0, LockFlags.None);
                }


                //Malla de TGC
                TgcMesh tgcMesh = new TgcMesh(d3dMesh, meshName, TgcMesh.MeshRenderType.VERTEX_COLOR);
                tgcMesh.Materials = new Material[] { TgcD3dDevice.DEFAULT_MATERIAL };
                tgcMesh.createBoundingBox();
                tgcMesh.Enabled = true;
                return tgcMesh;
            }
        }

        /// <summary>
        /// Crear un nuevo BoundingBoxExtendida igual a este
        /// </summary>
        /// <returns>Box clonado</returns>
        public BoundingBoxExtendida clone()
        {
            BoundingBoxExtendida cloneBox = new BoundingBoxExtendida();
            cloneBox.setPositionSize(this.translation, this.size);
            cloneBox.color = this.color;
            if (this.texture != null)
            {
                cloneBox.setTexture(this.texture.clone());
            }
            cloneBox.autoTransformEnable = this.autoTransformEnable;
            cloneBox.transform = this.transform;
            cloneBox.rotation = this.rotation;
            cloneBox.alphaBlendEnable = this.alphaBlendEnable;
            cloneBox.uvOffset = this.uvOffset;
            cloneBox.uvTiling = this.uvTiling;

            cloneBox.updateValues();
            return cloneBox;
        }




    }


}
