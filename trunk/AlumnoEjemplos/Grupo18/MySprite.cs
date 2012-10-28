using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer;
using TgcViewer.Utils._2D;


namespace AlumnoEjemplos.Grupo18
{

    public class MySprite
    {
        Size frameSize;
        int totalFrames;
        float currentTime;
        float animationTimeLenght;
        int framesPerRow;
        int framesPerColumn;
        float textureWidth;
        float textureHeight;
        public Vector2 positionBkp;

        protected bool enabled;

        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        protected bool playing;

        public bool Playing
        {
            get { return playing; }
            set { playing = value; }
        }

        TgcSprite sprite;

        public TgcSprite Sprite
        {
            get { return sprite; }
        }

        protected float frameRate;

        public float FrameRate
        {
            get { return frameRate; }
        }

        protected int currentFrame;

        public int CurrentFrame
        {
            get { return currentFrame; }
            set { currentFrame = value; }
        }

        public Vector2 Position
        {
            get { return sprite.Position; }
            set { sprite.Position = value; }
        }

        public Vector2 Scaling
        {
            get { return sprite.Scaling; }
            set { sprite.Scaling = value; }
        }

        public float Rotation
        {
            get { return sprite.Rotation; }
            set { sprite.Rotation = value; }
        }


        public MySprite(string texturePath, Size frameSize, int totalFrames, float frameRate)
        {
            this.enabled = true;
            this.currentFrame = 0;
            this.frameSize = frameSize;
            this.totalFrames = totalFrames;
            this.currentTime = 0;
            this.playing = true;

            //Crear textura
            Device d3dDevice = GuiController.Instance.D3dDevice;
            TgcTexture texture = TgcTexture.createTexture(d3dDevice, texturePath);

            //Sprite
            sprite = new TgcSprite();
            sprite.Texture = texture;

            //Calcular valores de frames de la textura
            textureWidth = texture.Width;
            textureHeight = texture.Height;
            framesPerColumn = (int)textureWidth / frameSize.Width;
            framesPerRow = (int)textureHeight / frameSize.Height;
            int realTotalFrames = framesPerRow * framesPerColumn;
            if (realTotalFrames > totalFrames)
            {
                throw new Exception("Error en AnimatedSprite. No coinciden la cantidad de frames y el tamaño de la textura: " + totalFrames);
            }

            setFrameRate(frameRate);
        }
        public void setFrameRate(float frameRate)
        {
            this.frameRate = frameRate;
            animationTimeLenght = (float)totalFrames / frameRate;
        }
        public void mover(int point) {
            this.Position = new Vector2(this.Position.X- point/100, this.Position.Y);
            this.Scaling = new Vector2(point / 50,point / 50);
        }
        public void restart() {
            this.Position = positionBkp;
        }
        public void update()
        {
            if (!enabled)
                return;

            //Avanzar tiempo
            if (playing)
            {
                currentTime += GuiController.Instance.ElapsedTime;
                if (currentTime > animationTimeLenght)
                {
                    //Reiniciar al llegar al final
                    currentTime = 0;
                }
            }

            //Obtener cuadro actual
            currentFrame = (int)(currentTime * frameRate);


            //Obtener rectangulo de dibujado de la textura para este frame
            Rectangle srcRect = new Rectangle();
            srcRect.Y = frameSize.Width * (currentFrame / framesPerRow);
            srcRect.Width = frameSize.Width;
            srcRect.X = frameSize.Height * (currentFrame % framesPerColumn);
            srcRect.Height = frameSize.Height;
            sprite.SrcRect = srcRect;
        }

        public void render()
        {
            if (!enabled)
                return;

            //Dibujar sprite
            sprite.render();
        }
        public void updateAndRender()
        {
            update();
            render();
        }

        public void dispose()
        {
            sprite.dispose();
        }

    }
}
