﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.Net;

namespace VAIPHO
{
    class agregacion
    {
        private double CoodX, CoordY, CoordZ;
        private string NombreVia, SentidoMarcha;
        Form1 Formulario;
        DateTime HoraGeneracionPaquete;

        public agregacion(Form1 form, string via, string sentido, double x, double y, double z, DateTime fecha)
        {
            NombreVia = via;
            SentidoMarcha = sentido;
            CoodX = x;
            CoordY = y;
            CoordZ = z;
            Formulario = form;
            HoraGeneracionPaquete = fecha;

        }
        //Create a sender and receiver.


        /*Función que comprueba si el punto donde está el incidente puede ser detectado por nuestra
         antena*/
        private bool EnRango(double X, double Y, string via, string sentido)
        {
            //Si no circulo en la misma vía en el mismo sentido soy incapaz de detectar el incidente
            //if (via.Equals(NombreVia) && sentido.Equals(SentidoMarcha))
            Formulario.Invoke(Formulario.myDelegate, new Object[] {"Mi gps= " +via });
            Formulario.Invoke(Formulario.myDelegate, new Object[] {"El paquete= "+NombreVia});
            if (via.Equals(NombreVia))
            { //radio del círgulo
                int r = 100;
                /* Calculamos la distancia de (X,Y) al centro del círculo (X,Y),
               que se obtiene como raíz cuadrada de (X-Y)^2+(X-Y)^2 */
                double d = Math.Sqrt((X - CoodX) * (X - CoodX) + (Y - CoordY) * (Y - CoordY));

                // el círculo contiene el punto si d es menor o igual al radio
                return d <= r;
            }
            else
                return false;
        }

        /*Función que firma lo que se pasa por parámetro y le hace un hash*/
        public string FirmarMsje(string msje, string ID)
        {
            string Firma = msje + ID;
            int signature = Firma.GetHashCode();
            Formulario.Invoke(Formulario.myDelegate, new Object[] { "Firmo la información" });
            return signature.ToString();

        }

        /*Función que devuelve el resultado de firmar un mensaje si estamos de acuerdo con la información*/
        public void InformacionAggreacion(string msje, string IP)
        {

            //OBTIENE HORA ACTUAL
            //DateTime myDateTime = DateTime.Now; 
            //ACCESO A LA BD
            string hash, viaSentido;
            BD AccesoBD = new BD(Formulario);
            string evento, posibleevento;
            Cripto criputiles = new Cripto(Formulario);
            DetectaIncidente incidente = new DetectaIncidente(Formulario);
            //Se comprueba si ya existe el incidente en las coordenadas X,Y aprox.
            evento = AccesoBD.ExisteEvento("T1", NombreVia, SentidoMarcha, CoodX, CoordY);

            Formulario.Invoke(Formulario.myDelegate, new Object[] { "¿Tengo el evento en BD? " });
            if (evento.Equals(""))
            {

                Formulario.Invoke(Formulario.myDelegate, new Object[] { "El evento no lo tengo en BD" });

                //SE VA A COMPROBAR SI SE PUEDE SER TESTIGO DE ESTE EVENTO

                //1º Obtenemos el la vía y el sentido de circulación de nuestr vehículo
                viaSentido = incidente.ViaSentidoMarcha();
                string via = "";
                string sentido = "";
                if (!viaSentido.Equals(""))
                {
                    string[] viaSentidoAux = viaSentido.Split('|');
                    via = viaSentidoAux[0];
                    sentido = viaSentidoAux[1];
                }

                //2º Si la información está dentro de mi Rango, y circulo por esa vía yo puedo detectar el problema
                if (EnRango(CoodX, CoordY, via, sentido))
                {
                    Formulario.Invoke(Formulario.myDelegate, new Object[] { "La información está dentro de mi rango" });
                    //3º Si estoy dentro del rango compruebo si detecto el problema
                    if (incidente.CompruebaIncidente())
                    {
                        Formulario.Invoke(Formulario.myDelegate, new Object[] { "Yo también detecto el incidente" });

                        Formulario.Invoke(Formulario.myDelegate, new Object[] { "¿Tengo el posible evento en PosibleEvento? " });
                        posibleevento = AccesoBD.ExistePosibleEvento("T1", NombreVia, SentidoMarcha, CoodX, CoordY);
                        if (posibleevento.Equals(""))
                        {
                            Formulario.Invoke(Formulario.myDelegate, new Object[] { "NO" });
                            Formulario.Invoke(Formulario.myDelegate, new Object[] { "Inserto PosibleEvento en BD " });
                            //Se inserta el incidente en posible Evento BD
                            AccesoBD.insertarPosibleEvento("T1", NombreVia, SentidoMarcha, CoodX, CoordY, CoordZ, HoraGeneracionPaquete, "");
                        }
                        //Firmo el paquete                       
                        Formulario.Invoke(Formulario.myDelegate, new Object[] { "The packet is to be signed because I can detect the incident" });
                        //Firmo la información
                        hash = FirmarMsje(msje, Server.myPseudonimo);
                        //Busco my pseudónimo
                        string myPseudonimo = AccesoBD.recuperamyPseu();
                        //Mando el mensaje  con  la Firma  
                        //Nota hay que agregar el hash                       
                        Cliente Client = new Cliente(Formulario, "9050");
                        Formulario.Invoke(Formulario.myDelegate, new Object[] { "Send a signed packet" });
                        //Client.respuesta("T1", myPseudonimo, "F_Traffic Jam_" + NombreVia + "_" + SentidoMarcha + "_" + Server.myPseudonimo + "_" + hash + "_" + CoodX + "_" + CoordY + "_" + CoordZ, IP);
                        Client.respuesta("T1", myPseudonimo, criputiles.Encrypt("F_Traffic Jam_" + NombreVia + "_" + SentidoMarcha + "_" + Server.myPseudonimo + "_" + hash + "_" + CoodX + "_" + CoordY + "_" + CoordZ, AccesoBD.recuperamySK()), IP);

                    }//EnIF CompruebaIncidente
                    //se trata de un ataque
                    else
                    {
                        Formulario.Invoke(Formulario.myDelegate, new Object[] { "Intento de Ataque" });
                    }
                }//EndIF EnRango
                //No estoy en rango por lo que no puedo comprobar la información
                else
                {
                    Formulario.Invoke(Formulario.myDelegate, new Object[] { "La información no está en  mi rango" });
                }
            }//EndIF ExisteEvento
            //Si tengo el evento en la BD o bien yo lo detecté y envié un msje para ser firmado o ya lo firmé y recibí el agregado
            else
            {
                Formulario.Invoke(Formulario.myDelegate, new Object[] { "El evento no es nuevo, ya existe en BD" });
                /*Si estoy esperando firmas que que ya mandé el paquete agregado, */
                if (AggregarFirmas.getEsperoFirmas())
                {
                    Formulario.Invoke(Formulario.myDelegate, new Object[] { "Tambien tengo este evento en mi BD y espero firmas" });
                    // si mi evento es más antiguo que el que me llega noo
                    string[] info = evento.Split('|');
                    //Si el mio es más antiguo me quedo con el mio
                    if (DateTime.Parse(info[2]).CompareTo(HoraGeneracionPaquete) < 0)
                    {
                        Formulario.Invoke(Formulario.myDelegate, new Object[] { "La fecha en que yo creé el evento" + info[2] });
                        Formulario.Invoke(Formulario.myDelegate, new Object[] { "La fecha del evento en el paquete" + HoraGeneracionPaquete });
                        Formulario.Invoke(Formulario.myDelegate, new Object[] { "Mi evento se creó antes" });

                    }
                    //Sino, tengo que modificar el mio y quedarme con este
                    else
                    {
                        Formulario.Invoke(Formulario.myDelegate, new Object[] { "Mi evento es más viejo, lo borro de BD" });
                        //Ya no se queda a la espera de ninguna firma
                        AggregarFirmas.resetEsperoFirmas();
                        //eliminamos el evento existente
                        AccesoBD.EliminaEvento("T1", HoraGeneracionPaquete);
                        //Agregamos el nuevo evento y lo mandamos firmado
                        InformacionAggreacion(msje, IP);

                    }

                }//EndIF esperoFirmas
                //Si no espero firmas es que ya firmé 
                else
                {
                    Formulario.Invoke(Formulario.myDelegate, new Object[] { "Incident already dealt" });
                }

            }
        }

        /*Función que comprueba las firmas y si son correctas agrega el mensaje en BD*/
        public void MensajeAgregado(string msje)
        {
            BD AccesoBD = new BD(Formulario);
            Utiles utiles = new Utiles();
            string evento, posibleevento;
            /* Cripto criputiles = new Cripto();
             DetectaIncidente incidente = new DetectaIncidente(Formulario);*/

            //Se comprueba si ya existe el incidente en las coordenadas X,Y aprox.la Base de datos
            evento = AccesoBD.ExisteEvento("T1", NombreVia, SentidoMarcha, CoodX, CoordY);
            Formulario.Invoke(Formulario.myDelegate, new Object[] { "¿Tengo el evento en BD? " });
            if (evento.Equals(""))
            {
                Formulario.Invoke(Formulario.myDelegate, new Object[] { "¿Tengo el posible evento en BD? " });
                posibleevento = AccesoBD.ExistePosibleEvento("T1", NombreVia, SentidoMarcha, CoodX, CoordY);
                //Si lo tengo es porque ya lo firmé, ahora lo borro de posible evento y lo inserto en Evento
                if (!posibleevento.Equals(""))
                {
                    string[] info = msje.Split('.');
                    string[] inf_firma = info[1].Split('|');

                    AccesoBD.insertarEvento("T1", NombreVia, SentidoMarcha, CoodX, CoordY, CoordZ, HoraGeneracionPaquete, inf_firma[0]);
                    //utiles.anadePoi("Atasco", "Atasco", (int)(CoodX * 100000), (int)(CoordY * 100000), 100);
                    utiles.anadePoi("Atasco", "Atasco", (int)(CoodX), (int)(CoordY), 100);
                    AccesoBD.EliminaPosibleEvento("T1", HoraGeneracionPaquete);
                }
                else
                {
                    //Compruebo las firmas contenidas en el mensaje
                    Formulario.Invoke(Formulario.myDelegate, new Object[] { "This packet contains incident information" });
                    Formulario.Invoke(Formulario.myDelegate, new Object[] { "Check Signatures" });
                    if (CompruebaFirmas(msje))
                    {
                        Formulario.Invoke(Formulario.myDelegate, new Object[] { "Las Firmas son buenas" });
                        string[] info = msje.Split('.');
                        string mensaje = info[0];
                        AccesoBD.insertarEvento("T1", NombreVia, SentidoMarcha, CoodX, CoordY, CoordZ, HoraGeneracionPaquete, info[1]);
                        //utiles.anadePoi("Atasco", "Atasco", (int)(CoodX * 100000), (int)(CoordY * 100000), 100);
                        utiles.anadePoi("Atasco", "Atasco", (int)(CoodX), (int)(CoordY), 100);

                        Formulario.Invoke(Formulario.myDelegate4, new Object[] { mensaje + "in " + CoodX + "," + CoordY + ". Recalculating route" });

                    }
                    //Informo de que podría tratarse de un ataque
                    else
                    {
                        Formulario.Invoke(Formulario.myDelegate, new Object[] { "Attempted Attack on the network" });
                    }
                }//end else existePosibleEvento


            }//EndIF Existeevento
            else
            {
                Formulario.Invoke(Formulario.myDelegate, new Object[] { "Ya tengo el Evento en mi BD no hago nada" });
            }
        }





















        //1º Compruebo si el paquete está firmado



        /*
         Función que cuando le pasa un mensaje con firma comprueba si la firmas es válida o no.
         */
        private bool CompruebaFirmas(string msje)
        {
            bool sonfiables = true;
            //Atasco en la M40-Firmas
            string[] info = msje.Split('.');
            string mensaje = info[0];
            //string nodo= info[1];
            string[] firmas = info[1].Split(',');
            //Compruebo cada una de las firmas
            for (int i = 0; i < firmas.Length; i++)
            {
                //Se coje el ID del nodo que firmo y el hash
                string[] hashID = firmas[i].Split('|');
                int hash = int.Parse(hashID[0]);
                string ID = hashID[1];

                // Se vuelve a generar el mensaje más el ID y se comprueba el hash, si son iguales la inf es correcta
                string Firma = mensaje + ID;
                if (!(hash == Firma.GetHashCode()))
                    sonfiables = false;


            }
            return sonfiables;


        }


        /*Fucnión que genera un paquete de agregación para que los demás vehículos firmen si están 
         * de acuerto con la información*/
        public void EnviaNuevoPaqueteAgregacion()
        {
            string IP = IPAddress.Broadcast.ToString();
            Cripto criputiles = new Cripto(Formulario);
            //Se obtiene el pseudónim
            BD AccesoBD = new BD(Formulario);
            string pseudonym = AccesoBD.recuperamyPseu();

            Cliente Client = new Cliente(Formulario, "9050");
            string prueba = "I_Traffic Jam_" + NombreVia + "_" + SentidoMarcha + "_" + CoodX.ToString() + "_" + CoordY.ToString() + "_" + CoordZ.ToString() + "_" + HoraGeneracionPaquete.ToString("MM/dd/yyyy HH:mm:ss");
            // Client.respuesta("T1", pseudonym,prueba, IP);
            Formulario.Invoke(Formulario.myDelegate, new Object[] { "Se envía el paquete" });
            Client.respuesta("T1", pseudonym, criputiles.Encrypt(prueba, AccesoBD.recuperamySK()), IP);


        }

    }
}
