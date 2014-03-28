using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.Win32;
using ApplicationAPI;

using System.Net;//para parking



namespace VAIPHO
{
    class DetectaIncidente
    {
        public Form1 Formulario;

        public DetectaIncidente(Form1 formu)
        {
            Formulario = formu;
        }

        public void DetectaIncidenteStart()
        {//Se crea un hilo que estará constantemente detectando los incidentes en la carretera
            Thread HiloTick = new Thread(new ThreadStart(vigila));
            HiloTick.Start();
        }

        private string ObtenerSentidoMarcha(double pCardinal)
        {
            //Formulario.Invoke(Formulario.myDelegate, new Object[] { "sentido de GPS " + pCardinal });
            //double pCardinal=puntoCardina/100000;
            //Formulario.Invoke(Formulario.myDelegate, new Object[] { "sentido dividido " + pCardinal });
            string sentido = "";

            if (pCardinal >= 0 && pCardinal < 90)
                sentido = "este";
            //if (pCardinal >= 45 && pCardinal < 90)
            //    sentido = "sur-este";
            if (pCardinal >= 90 && pCardinal < 180)
                sentido = "sur";
            // if (pCardinal >= 135 && pCardinal < 180)
            //    sentido = "sur-oeste";
            if (pCardinal >= 180 && pCardinal < 270)
                sentido = "oeste";
            // if (pCardinal >= 225 && pCardinal < 270)
            //   sentido = "nor-oeste";
            if (pCardinal >= 270 && pCardinal < 360)
                sentido = "norte";
            //if (pCardinal >= 315 && pCardinal < 360)
            //    sentido = "nor-este";

            return sentido;

        }

        private void añadePoi(int X, int Y)
        {
            string dirimconges = "\\Tarjeta de almacenamiento\\FLEET_MOBILE_14d\\Res\\icons\\poi\\1_favo_3.bmp";
            SError err;

            SPoi poi;

            LONGPOSITION position = new LONGPOSITION(X, Y);

            int res = CApplicationAPI.AddPoiCategory(out err, "Restaurante", dirimconges, "ESP", 0);
            Formulario.Invoke(Formulario.myDelegate, new Object[] { "El resultado de añadir una categoría es: " + err.GetDescription() });

            //res = CApplicationAPI.LocationFromAddress(out err, out position, "SVK,Bratislava,Zochova,1", false, false, 0);

            poi = new SPoi(position, "Restaurante", "New POI", 0);

            res = CApplicationAPI.AddPoi(out err, ref poi, 0);
            Formulario.Invoke(Formulario.myDelegate, new Object[] { "El resultado de añadir el POI es: " + err.GetDescription() });

            int b = res;

            // CApplicationAPI.AddPoiCategory(out error, strCategory, strBitmapPath, strISOCode, nMaxTime);
            //int a= CApplicationAPI.AddPoi(out Error, ref Poi ,MaxTime);
            //Formulario.Invoke(Formulario.myDelegate, new Object[] {"El resultado de añadePOI es "+a+ " "+Error.nCode});

        }

        //Función que cada cierto tiempo va 
        private void vigila()
        {
            //  int velocidad, X, Y;
            //bool atasco;
            SError err;
            SGpsPosition gps;
            // SRoadInfo road;
            int speedLimit = 0, atascado = 0;
            string direccionTJam;
            // LONGPOSITION posTJam;

            string text = "";// "Current position:\r\n\t";
            string text2 = "";// "Current speed:\r\n\t";//VELOCIDAD ACTUAL
            string text3 = "GPS Time:\r\n\t";//HORA GPS
            string text4 = "Condition:\r\n\t";//ATASCO
           // string text5;
           // string text6, a = ".";



            if (DriveHandler.StartDrive("\\Tarjeta de almacenamiento\\FLEET_MOBILE_14d\\Drive\\WindowsMobile\\Drive.exe", new int[] { 0, 0, 0, 0 }) == 1)//20, 60, 320, 240 }) == 1)
            {
                Formulario.Invoke(Formulario.myDelegate, new Object[] { "Entro en Detecta incidente" });
                DriveHandler.SendDriveToBackground(10);
                //Hilo que está todo el tiempo detectando posibles atascos.*/
                while (!Formulario.cerrar)
                {
                    //Se obtiene información de posición gps y velocidad de circulación
                    if (CApplicationAPI.GetActualGpsPosition(out err, out gps, false, 1000) == 1)
                    {
                        LONGPOSITION position = new LONGPOSITION(gps.Longitude, gps.Latitude);
                        CApplicationAPI.MetersToWorldDegree(ref position);
                        double longitude = position.lX;// / 100000.0;
                        double latitude = position.lY;// / 100000.0;
                        double altitud = gps.Altitude;
                        string sentido = "";
                        text = longitude + "°," + latitude + "°";

                        // Formulario.Invoke(Formulario.myDelegate3, new Object[] { text });
                        text2 = gps.Speed + "km/h"; //VELOCIDAD ACTUAL
                        try
                        {
                        Formulario.Invoke(Formulario.myDelegate2, new Object[] { text2 });
                        // text3 = gps.Time + "hora" + gps.Date + "fecha";//HORA Y FECHA 
                        if (CApplicationAPI.GetLocationInfo(out err, position, out direccionTJam, 0) == 1)
                        {
                          //  Formulario.Invoke(Formulario.myDelegate3, new Object[] { direccionTJam });
                        }

                      
                            //Se obtiene la velocidad límite de la vía
                            if (CApplicationAPI.GetCurrentSpeedLimit(out err, out speedLimit, 1000) == 1)
                            {
                                //Si estamos cin una vía y la velocidad de circulación es considereblemente inferior
                                //a la velocidad normal de la via marcamos un posible atasco

                                //Mostramos la velocidad límite de la vía
                                string velocidadlimite2 = "Velocidad limite" + speedLimit.ToString();
                                //Formulario.Invoke(Formulario.myDelegate, new Object[] { velocidadlimite2 });
                                /* a += ".";
                                 Formulario.Invoke(Formulario.myDelegate, new Object[] { a });*/
                                string velocidad = "Velocidad circulacion" + gps.Speed;
                                //Formulario.Invoke(Formulario.myDelegate, new Object[] { velocidad });


                                if ((gps.Speed >= 0) && (gps.Speed <= (speedLimit / 4))) // gps.Speed < 4))//    
                                // if (speedLimit <= 0)//jeza-->quitarlinea
                                {
                                    //Mostramos la velocidad límite de la vía

                                    //string velocidadlimite = speedLimit.ToString();
                                    //Formulario.Invoke(Formulario.myDelegate3, new Object[] { velocidadlimite });

                                    if (atascado == 0)
                                    {
                                        text4 = "Possible Traffic Jam";
                                        //Formulario.Invoke(Formulario.myDelegate, new Object[] { text4 });
                                    }
                                    atascado++;
                                    if (atascado == 10)
                                    {//En caso de detectar atasco, coger información de la vía, añadirlo en el gps, enviar para que firmen 

                                        //Se muestra en el Mapa que hay un posible atasco
                                        // if (CApplicationAPI.FlashMessage(out err, "Possible Traffic Jam", true, 1000) == 1)
                                        //      Formulario.Invoke(Formulario.myDelegate, new Object[] { "Add event: Traffic Jam" });

                                        //Formulario.Invoke(Formulario.myDelegate, new Object[] { "ATASCO!!" });

                                        //Se recoge el nombre de la vía donde hay atasco y el sentido de la marcha
                                        if (CApplicationAPI.GetLocationInfo(out err, position, out direccionTJam, 0) == 1)
                                        {
                                            //Se obtiene el sentido de la marcha
                                            sentido = ObtenerSentidoMarcha(gps.Course);
                                            Formulario.Invoke(Formulario.myDelegate, new Object[] { "Traffic Jam in: " + direccionTJam });
                                            Formulario.Invoke(Formulario.myDelegate, new Object[] { "Sense: " + sentido });     
                                        }
                                        //Se llama a la función encargada de manejar los incidentes y el hilo actual sigue vigilando.
                                        ManejaIncidente(direccionTJam, sentido, longitude, latitude, altitud);

                                        // Thread HiloManejaIncidente = new Thread(new ThreadStart(hiloParam.ManejaIncidente));
                                        //HiloManejaIncidente.Start();
                                        //Thread.Sleep(300 * 1000);
                                        atascado = 0;
                                    }
                                }
                                else
                                {
                                    atascado = 0;
                                }
                            }
                        }
                        catch //(Exception p)
                        {
                           // Formulario.Invoke(Formulario.myDelegate, new Object[] { "Error: " + p.Message });
                        }

                    }
                    else
                    {
                        text += err.GetDescription();
                        text2 += err.GetDescription(); //VELOCIDAD ACTUAL
                        text3 += err.GetDescription(); //GPS FECHA
                        text4 += err.GetDescription();
                    }
                    //dormimos el hilo un segundo
                    Thread.Sleep(1000);
                }
            }
        }

        public void ManejaIncidente(string NombreVia, string sentidoMarcha, double X, double Y, double Z)
        {

            //OBTIENE HORA ACTUAL
            DateTime myDateTime = DateTime.Now;
            //ACCESO A LA BD
            BD AccesoBD = new BD(Formulario);
            string evento, evento2;
            //Formulario.Invoke(Formulario.myDelegate, new Object[] { "ATASCO!!" });
            //Formulario.Invoke(Formulario.myDelegate, new Object[] { "Traffic Jam in: " + NombreVia });

            //Se comprueba si ya existe el incidente en las coordenadas X,Y aprox.
            evento = AccesoBD.ExisteEvento("T1", NombreVia, sentidoMarcha, X, Y);
            //Formulario.Invoke(Formulario.myDelegate, new Object[] { "¿¿Existe Evento??" });
            if (!evento.Equals("error"))
            {
                string[] words = evento.Split('|');
                //Si no existe el evento se introduce en BD 
                if (evento.Equals(""))
                {
                    // Formulario.Invoke(Formulario.myDelegate, new Object[] { "NO" });
                    evento2 = AccesoBD.ExistePosibleEvento("T1", NombreVia, sentidoMarcha, X, Y);
                    if (evento2.Equals(""))
                    {
                        if (AccesoBD.insertarPosibleEvento("T1", NombreVia, sentidoMarcha, X, Y, Z, myDateTime, ""))
                        {
                            Formulario.Invoke(Formulario.myDelegate, new Object[] { "Inserta en PosibleEvneto" });

                            //Se ha insertado correctamente
                            // Formulario.Invoke(Formulario.myDelegate, new Object[] { "The event has been inserted properly in DB" });

                            //Si se trata de un nuevo evento se envía un mensaje buscando agregación.
                            AggregarFirmas.setFirmas();
                            AggregarFirmas.setNumerofirmas();
                            AggregarFirmas.setEsperoFirmas();
                            AggregarFirmas.getEsperoFirmas();
                            Formulario.Invoke(Formulario.myDelegate, new Object[] { "Me quedo a la espera de firmas" });
                        }
                        else
                        {
                            Formulario.Invoke(Formulario.myDelegate, new Object[] { "IncidenteV1=>Error insertando posibleevento en BD " });
                        }
                    }

                    agregacion Agpaq = new agregacion(Formulario, NombreVia, sentidoMarcha, X, Y, Z, myDateTime);
                    Agpaq.EnviaNuevoPaqueteAgregacion();
                }
                else
                {
                    // Formulario.Invoke(Formulario.myDelegate, new Object[] { "SI" });
                    //Si existe el incidente no hacemos nada
                    Formulario.Invoke(Formulario.myDelegate, new Object[] { "Incident already dealt " });
                }
            }//error
        }


        public void posibleparking()
        {
            SError err;
            SGpsPosition gps;
            string hostName = Dns.GetHostName();
            IPHostEntry thisHost = Dns.GetHostEntry(hostName);
            string thisIpAddr = thisHost.AddressList[0].ToString();
            Cliente mycliente = new Cliente(Formulario, "9050");
            DateTime inicio = DateTime.Now.AddSeconds(10.00);//si en menos de X segundos detecta la señal gps avisa de posible aparcamiento
            string auxiliar, via, sentido;
            Cripto criputiles = new Cripto(Formulario);
            BD DButiles = new BD(Formulario);

            while (DateTime.Compare(DateTime.Now, inicio) < 0)
            {
                try
                {
                    if (CApplicationAPI.GetActualGpsPosition(out err, out gps, false, 1000) == 1)
                    {
                        LONGPOSITION position = new LONGPOSITION(gps.Longitude, gps.Latitude);
                        sentido = ObtenerSentidoMarcha(gps.Course);
                        CApplicationAPI.GetLocationInfo(out err, position, out via, 0);

                        int CoorX = gps.Longitude;
                        int CoorY = gps.Latitude;
                        int CoorZ = gps.Altitude;
                        //string viaOnombre = "calle";
                        //string sentido = "sentido";
                        DateTime fechaDato = DateTime.Now;
                        string datoAparcamiento = via + ";" + sentido + ";" + CoorX + ";" + CoorY + ";" + CoorZ + ";" + fechaDato;//Estos datos los coge de un fichero
                        string firma = criputiles.RSAfirma(datoAparcamiento);
                        
                        Formulario.Invoke(Formulario.myDelegate, new Object[] { "Possible parking in: " + CoorX + "," + CoorY });
                        auxiliar = criputiles.Encrypt(datoAparcamiento+";"+firma, DButiles.recuperamySK());
                        mycliente.respuesta("P2", Server.myPseudonimo,auxiliar , thisIpAddr);
                        DButiles.insertarEvento("P2", via, sentido, CoorX, CoorY, CoorZ, fechaDato, firma);                     

                        break;
                    }
                }
                catch { }
                Thread.Sleep(1000);
            }
        }

        /*
         * Función que devuelve la vía por la que circulamos y el sentido en el que circulamos
         * return via|sentido         
         */
        public string ViaSentidoMarcha()
        {
            SError err;
            SGpsPosition gps;
            string sentido, direccionTJam, result = "";

            //Se obtiene la posición gps
            try
            {
                if (CApplicationAPI.GetActualGpsPosition(out err, out gps, false, 1000) == 1)
                {
                    LONGPOSITION position = new LONGPOSITION(gps.Longitude, gps.Latitude);

                    //Se obtiene el nombre de la vía
                    if (CApplicationAPI.GetLocationInfo(out err, position, out direccionTJam, 0) == 1)
                    {
                        //Se obtiene el sentido de la marcha
                        sentido = ObtenerSentidoMarcha(gps.Course);
                        result = direccionTJam + "|" + sentido;
                    }
                }
            }
            catch { }
            return result;
        }


        public bool CompruebaIncidente()
        {
            SError err;
            SGpsPosition gps;
            // SRoadInfo road;
            int speedLimit = 0;
            int comprobaciones = 0, atascado = 0;
            bool atasco = false;
            while (comprobaciones < 4)
            {
                if (CApplicationAPI.GetActualGpsPosition(out err, out gps, false, 1000) == 1)
                {
                    if (CApplicationAPI.GetCurrentSpeedLimit(out err, out speedLimit, 1000) == 1)
                    {
                        /*Si estamos cin una vía y la velocidad de circulación es considereblemente inferior
                         a la velocidad normal de la via marcamos un posible atasco*/
                        if ((gps.Speed >= 0) && (gps.Speed <= (speedLimit / 4))) // gps.Speed < 4))//    
                        {
                            atascado++;
                            if (atascado == 2)
                            {
                                atasco = true;
                            }
                        }
                    }
                }
                else {
                    Formulario.Invoke(Formulario.myDelegate, new Object[] { "CompruebaIncidente. Señal GPS no encontrada. " });
                }
                comprobaciones++;

            }//while
            return atasco;
        }
    }              
}
