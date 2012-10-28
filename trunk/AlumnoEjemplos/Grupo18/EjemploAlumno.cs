using System;
using System.Collections.Generic;
using System.Text;
using TgcViewer.Example;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils.TgcSkeletalAnimation;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using System.IO;
using TgcViewer.Utils._2D;
using TgcViewer.Utils.Sound;

namespace AlumnoEjemplos.Grupo18
{
    public class Grupo18 : TgcExample
    {
        List<BoundingBoxExtendida> boxes;
        bool selected;
        Vector3 collisionPoint;
        TgcBox collisionPointMesh;
        TgcPickingRay pickingRay;
        long tiempoUltimaLlamada = 0;
        TgcBox ligtBox;
        BoundingBoxExtendida selectedMesh;
        BoundingBoxExtendida selectedMeshAux;
        BoundingBoxExtendida selectedMeshAnt = null;
        BoundingBoxExtendida cajaporon;
        MySprite animatedSprite;
        MySprite animatedSpriteTriste;

        TgcText2d puntuacionFracazosText;
        TgcText2d puntuacionAciertosText;
        TgcText2d finalText;
        TgcText2d finalText1;
        TgcText2d tiempoTranscurrido;


        TgcText2d aciertosText;
        TgcText2d fracazosText;

        DateTime time;
        DateTime timeDif;
        Random rand = new Random(DateTime.Now.Millisecond);

        int aciertos = 0;
        int fracazos = 0;
        string currentFile;
        int[] imagenes= new int[56];
        int countDeseleccion, countDeseleccionExito = 0;


        public void generarAleatorio() {
            int b = 0;
            for(int a=0;a<55;a=a+2){
                imagenes[a]=b;
                imagenes[a+1] = b;
                b++;
            }
            int aux,aux1,aux2;
            for(int a=0;a<55;a++){
                aux1= rand.Next(0,55);
                aux2= rand.Next(0,55);
                aux = imagenes[aux1];
                imagenes[aux1] = imagenes[aux2];
                imagenes[aux2] = aux;
            }
        }

        public override string getCategory()
        {
            return "Grupo 18";
        }

        public override string getName()
        {
            return "Individual - Nicolas Bassart";
        }

        public override string getDescription()
        {
            return "Juego de memoria";
        }

        public override void init()
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;
            time = new DateTime();
            //GuiController.Instance.D3dDevice.RenderState.ReferenceAlpha =255;
            TgcTexture texe = TgcTexture.createTexture(d3dDevice, "mitex1", GuiController.Instance.AlumnoEjemplosMediaDir + "Grupo18\\granito00.jpg");

            TgcTexture texe1 = TgcTexture.createTexture(d3dDevice, "mitex1", GuiController.Instance.AlumnoEjemplosMediaDir + "Grupo18\\nubes.jpg");
            Vector3 cen = new Vector3(1, 1, 220);
            Vector3 boxSi = new Vector3(2000, 2000, 2000);
            cajaporon = BoundingBoxExtendida.fromSize(cen, boxSi, texe1);
            

            TgcSceneLoader loader = new TgcSceneLoader();


            boxes = new List<BoundingBoxExtendida>();
            int a =0;
            

            Vector3 boxSize = new Vector3(25, 25, 25);
            //Genera la aleatoriedad
            this.generarAleatorio();

            for (int i = 0; i < 4; i++)
            {

                for (int j = 0; j < 4; j++)
                {
                    for (int k = 0; k < 4; k++)
                        {
                            if (!((k ==1 || k==2) &&((i>0&&i<3)&&(j>0&&j<3)) )) { 
                            TgcTexture texture = TgcTexture.createTexture(d3dDevice, "mitex" + i+j+k,GuiController.Instance.AlumnoEjemplosMediaDir + "Grupo18\\granito00.jpg");

                            Vector3 center = new Vector3((boxSize.X + boxSize.X / 2) * i, (boxSize.Y + boxSize.Y / 2) * j, (k * 35) +220);

                            BoundingBoxExtendida box1 = BoundingBoxExtendida.fromSize(center, boxSize, texture);

                            box1.setName("imagen" + imagenes[a]);
                            a++;
                            boxes.Add(box1);
                            }
                        }

                    }
                
            }

            //Iniciarlizar PickingRay
            pickingRay = new TgcPickingRay();

            GuiController.Instance.RotCamera.CameraDistance = 250;
            GuiController.Instance.RotCamera.CameraCenter = new Vector3(56.25f, 56.25f, 272.2f);

            //Crear caja para marcar en que lugar hubo colision
            collisionPointMesh = TgcBox.fromSize(new Vector3(25, 25, 25), Color.Red);
            selected = false;

            //Crear caja para indicar ubicacion de la luz
            ligtBox = TgcBox.fromSize(new Vector3(10, 10, 10), Color.Yellow);

            aciertosText = new TgcText2d();
            aciertosText.Text = "Aciertos:";
            aciertosText.Position = new Point(340, 10);
            aciertosText.Color = Color.Red;
            aciertosText.changeFont(new System.Drawing.Font(FontFamily.GenericMonospace, 20, FontStyle.Bold));

            fracazosText = new TgcText2d();
            fracazosText.Text = "Fracasos:";
            fracazosText.Position = new Point(340, 90);
            fracazosText.Color = Color.Red;
            fracazosText.changeFont(new System.Drawing.Font(FontFamily.GenericMonospace, 20, FontStyle.Bold));

            puntuacionAciertosText = new TgcText2d();
            puntuacionAciertosText.Text = "0";
            puntuacionAciertosText.Position = new Point(340, 40);
            puntuacionAciertosText.Color = Color.Red;
            puntuacionAciertosText.changeFont(new System.Drawing.Font(FontFamily.GenericMonospace, 40, FontStyle.Bold));

            puntuacionFracazosText = new TgcText2d();
            puntuacionFracazosText.Text = "0";
            puntuacionFracazosText.Position = new Point(340, 110);
            puntuacionFracazosText.Color = Color.Red;
            puntuacionFracazosText.changeFont(new System.Drawing.Font(FontFamily.GenericMonospace, 40, FontStyle.Bold));

            finalText = new TgcText2d();
            finalText.Text = "FIN DEL JUEGO";
            finalText.Position = new Point(0,30);
            finalText.Color = Color.Red;
            finalText.changeFont(new System.Drawing.Font(FontFamily.GenericMonospace, 70, FontStyle.Bold));

            finalText1 = new TgcText2d();
            finalText1.Position = new Point(0, 120);
            finalText1.Color = Color.Red;
            finalText1.changeFont(new System.Drawing.Font(FontFamily.GenericMonospace, 60, FontStyle.Bold));

            tiempoTranscurrido = new TgcText2d();
            tiempoTranscurrido.Position = new Point(0, 360);
            tiempoTranscurrido.Color = Color.Red;
            tiempoTranscurrido.changeFont(new System.Drawing.Font(FontFamily.GenericMonospace, 60, FontStyle.Bold));

            this.loadMp3(GuiController.Instance.AlumnoEjemplosMediaDir + "Grupo18\\dalePlay.mp3");
            TgcMp3Player player = GuiController.Instance.Mp3Player;
            TgcMp3Player.States currentState = player.getStatus();
            player.play(true);
            GuiController.Instance.Modifiers.addButton("Reload", "Reload", new EventHandler(Reload_ButtonClick));


            //Crear Sprite animado
            animatedSprite = new MySprite(
                GuiController.Instance.AlumnoEjemplosMediaDir + "Grupo18\\prueba2.png", //Textura de 256x256
                new Size(128, 128), //Tamaño de un frame (64x64px en este caso)
                16, //Cantidad de frames, (son 16 de 64x64px)
                6 //Velocidad de animacion, en cuadros x segundo
                );

            //Ubicarlo centrado en la pantalla
            Size screenSize = GuiController.Instance.Panel3d.Size;
            Size textureSize = animatedSprite.Sprite.Texture.Size;
            animatedSprite.Scaling = new Vector2(4f,4f);
            animatedSprite.Position = new Vector2(screenSize.Width / 2 - textureSize.Width / 2, screenSize.Height / 2 - textureSize.Height / 2);
            animatedSprite.positionBkp = animatedSprite.Position;



            //Crear Sprite animado
            animatedSpriteTriste = new MySprite(
                GuiController.Instance.AlumnoEjemplosMediaDir + "Grupo18\\caritaTiste.png", //Textura de 256x256
                new Size(128, 128), //Tamaño de un frame (64x64px en este caso)
                16, //Cantidad de frames, (son 16 de 64x64px)
                6 //Velocidad de animacion, en cuadros x segundo
                );

            animatedSpriteTriste.Scaling = new Vector2(2f,2f);
            animatedSpriteTriste.Position = new Vector2(8, 5);
            animatedSpriteTriste.positionBkp = animatedSpriteTriste.Position;

        }
        private void loadMp3(string filePath)
        {
            if (currentFile == null || currentFile != filePath)
            {
                currentFile = filePath;

                //Cargar archivo
                GuiController.Instance.Mp3Player.closeFile();
                GuiController.Instance.Mp3Player.FileName = currentFile;
            }
        }

        void Reload_ButtonClick(object sender, EventArgs e)
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;
            aciertos = 0;
            fracazos = 0;
            countDeseleccion=0;
            countDeseleccionExito = 0;
            timeDif = new DateTime();
            time = new DateTime();
            selectedMesh = null;
            selectedMeshAnt = null;

            int a = 0;
            imagenes.Initialize();
            this.generarAleatorio();
            foreach (BoundingBoxExtendida box in boxes)
            {
                //box.restartMatriz();
                //box.getBox_caja1().deseleccionar(d3dDevice);

                //box.getBox_caja1().restartMatriz();
                box.setName("imagen" + imagenes[a]);
                box.deseleccionar(d3dDevice);
                box.setSeleccionado(false);
                box.Enabled = true;
                a++;
            }
        }

        public override void render(float elapsedTime)
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;

            cajaporon.render();
            if (aciertos == 28)
            {
                finalText1.Text = "!!!!GANASTE¡¡¡¡¡, Total fracasos 70, tus fracasos" + fracazos.ToString();
                finalText.render();
                finalText1.render();
                tiempoTranscurrido.Text = (timeDif - time).Minutes.ToString() + ":" + (timeDif - time).Seconds.ToString();
                tiempoTranscurrido.render();
                return;
            }
            if (fracazos == 70) {

                finalText1.Text = "!!!!PERDISTE¡¡¡¡¡, Total fracazos 70, tus fracasos" + fracazos.ToString();
                finalText.render();
                finalText1.render();
                tiempoTranscurrido.Text = (timeDif - time).Minutes.ToString() + ":" + (timeDif - time).Seconds.ToString();
                tiempoTranscurrido.render();

                return;
            }
            timeDif = DateTime.Now;
            puntuacionFracazosText.Text = fracazos.ToString();
            puntuacionAciertosText.Text = aciertos.ToString();
            puntuacionFracazosText.render();
            puntuacionAciertosText.render();

            aciertosText.render();
            fracazosText.render();

                //Obtiene la caja collisionada de las posibles tomo la que tiene menor distancia a la camara
            if (GuiController.Instance.D3dInput.buttonPressed(TgcViewer.Utils.Input.TgcD3dInput.MouseButtons.BUTTON_LEFT) && (countDeseleccion > 0 || countDeseleccionExito > 0))
            {
                    if (countDeseleccion > 0){
                        selectedMeshAnt.getBox_caja1().Enabled = true;
                        selectedMeshAnt.Enabled = true;
                        selectedMeshAnt.deseleccionar(d3dDevice);
                        selectedMesh.deseleccionar(d3dDevice);
                        selectedMeshAnt = null;
                        countDeseleccion = 0;
                    } 
                    if( countDeseleccionExito > 0){
                        selectedMesh.getBox_caja1().Enabled = false;
                        selectedMesh.Enabled = false;
                        countDeseleccionExito = 0; 

                    }

            }
            this.obtenerCajaCollisionada();

            
            if ((((DateTime.Now.Minute * 100000) + (DateTime.Now.Second * 1000) + DateTime.Now.Millisecond) - tiempoUltimaLlamada) > 3)
            {

                foreach (BoundingBoxExtendida box in boxes)
                {

                    box.getBox_caja1().relizarefecto(d3dDevice);
                    box.getBox_caja1().updateValues();
                    box.relizarefecto(d3dDevice);
                    box.updateValues();
                }
                tiempoUltimaLlamada = ((DateTime.Now.Minute * 100000) + (DateTime.Now.Second * 1000) + DateTime.Now.Millisecond);
            }

            foreach (BoundingBoxExtendida box in boxes)
            {
                box.getboxinterna().render();
                box.getBox_caja1().render();
                box.render();
            }

            if (GuiController.Instance.D3dInput.buttonPressed(TgcViewer.Utils.Input.TgcD3dInput.MouseButtons.BUTTON_LEFT)){

                if (selected )//Indica que se realizo alguna seleccion
                {

                    if (!selectedMesh.getSeleccionado())//Si la caja seleccionada ya se encuentra descubierta
                    {
                        selectedMesh.seleccionar(d3dDevice);//realiza el efecto sobre la caja selecionada

                        if (selectedMeshAnt == null) //Al ser el primero a seleccionar queda marcado
                        {
                            selectedMeshAnt = selectedMesh;
                            selectedMesh.setSeleccionado(true);
                        }else{
                            //Seria el caso en que no es el primero y se quiere seleccionar el proximo, pueden pasar tres cosas:
                            //que el que se selecciono sea el mismo cuadrado que el anterior
                            //que el nuevo seleccionado no teng la misma figura que el primero
                            //que el nuevo seleccionado conincida la figura con el primero.
                            selectedMeshAnt.getBox_caja1().Enabled = false;
                            selectedMeshAnt.Enabled = false;
                            if (selectedMesh.getName().Equals(selectedMeshAnt.getName()))
                            {
                                aciertos++;
                                animatedSprite.restart();
                                selectedMesh.setSeleccionado(true);
                                countDeseleccionExito = 200;
                                selectedMeshAnt = null;
                            }
                            else {
                                fracazos++;
                                countDeseleccion = 200;
                                animatedSpriteTriste.restart();
                                selectedMeshAnt.setSeleccionado(false);
                                selectedMesh.setSeleccionado(false);
                            }
                        }
                    }
                }
            }
            if (countDeseleccion > 0)
            {
                if (countDeseleccion == 1)
                {
                    selectedMeshAnt.getBox_caja1().Enabled = true;
                    selectedMeshAnt.Enabled = true;
                    selectedMeshAnt.deseleccionar(d3dDevice);
                    selectedMesh.deseleccionar(d3dDevice);
                    selectedMeshAnt = null;
                }
                countDeseleccion--;
                //Iniciar dibujado de todos los Sprites de la escena (en este caso es solo uno)
                GuiController.Instance.Drawer2D.beginDrawSprite();
                //Dibujar sprite (si hubiese mas, deberian ir todos aquí)
               // animatedSpriteTriste.mover(countDeseleccion);
                //Actualizamos el estado de la animacion y renderizamos
                animatedSpriteTriste.updateAndRender();

                //Finalizar el dibujado de Sprites
                GuiController.Instance.Drawer2D.endDrawSprite();
            }
            if (countDeseleccionExito > 0)
            {
                if (countDeseleccionExito == 1)
                {
                    selectedMesh.getBox_caja1().Enabled = false;
                    selectedMesh.Enabled = false;
   
                }

                countDeseleccionExito--;
                //Iniciar dibujado de todos los Sprites de la escena (en este caso es solo uno)
                GuiController.Instance.Drawer2D.beginDrawSprite();
                //Dibujar sprite (si hubiese mas, deberian ir todos aquí)
                //animatedSprite.mover(countDeseleccionExito);
                //Actualizamos el estado de la animacion y renderizamos
                animatedSprite.updateAndRender();
                
                //Finalizar el dibujado de Sprites
                GuiController.Instance.Drawer2D.endDrawSprite();


            }


       }

        private void obtenerCajaCollisionada()
        {
            selected = false;
            //Si hacen clic con el mouse, ver si hay colision RayAABB
            if (GuiController.Instance.D3dInput.buttonPressed(TgcViewer.Utils.Input.TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                //Actualizar Ray de colisión en base a posición del mouse
                pickingRay.updateRay();

                bool valida = false;
                //Testear Ray contra el AABB de todos los meshes
                selectedMeshAux = null;
                foreach (BoundingBoxExtendida box in boxes)
                {
                    TgcBoundingBox aabb = box.BoundingBox;

                    //Ejecutar test, si devuelve true se carga el punto de colision collisionPoint
                    valida = TgcCollisionUtils.intersectRayAABB(pickingRay.Ray, aabb, out collisionPoint);
                    if (valida)
                    {
                        if (selectedMeshAux == null)
                        {
                            selectedMesh = box;
                            selectedMeshAux = box;
                            selected = true;
                        }
                        else
                        {
                            if (this.evaluarDistancia(box, selectedMeshAux))
                            {
                                selectedMesh = box;
                                selectedMeshAux = box;
                            }
                        }
                    }
                }
            }
        }

        private bool evaluarDistancia(BoundingBoxExtendida box, BoundingBoxExtendida selectedMesh)
        {
            //Primero calculo la distancia desde la camara al box
            Vector3 Z_box = (GuiController.Instance.RotCamera.getPosition() - box.centerbkp);
            Vector3 Z_selected = (GuiController.Instance.RotCamera.getPosition() - selectedMesh.centerbkp);
            //return false;
            return (Z_box.LengthSq() < Z_selected.LengthSq());
        }

        public override void close()
        {
            foreach (BoundingBoxExtendida box in boxes)
            {
                box.dispose();
            }
            collisionPointMesh.dispose();
        }

    }
}
