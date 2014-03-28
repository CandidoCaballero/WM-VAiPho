﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.IO;

//using OpenNETCF.Net.NetworkInformation;
using OpenNETCF.WindowsMobile;
using System.Runtime.InteropServices;
using Microsoft.WindowsMobile;
using Microsoft.WindowsMobile.Telephony;
using Microsoft.WindowsMobile.Status;

using System.Data.SqlServerCe; //Añadido para sqlce.

using System.Security.Cryptography;//para rsa

using ApplicationAPI;

using System.Diagnostics;//para timer
//using System.ComponentModel;
using System.Data;
//using System.Drawing;


namespace VAIPHO
{
    class Server
    {
        //  private System.Net.Sockets.TcpListener tcpListener;
        private Thread listenThread;
        public static Form1 Formulario;
        public static string hashaComprobar, pregunta, grafo, grafoIsomorfo, enviado, secuencia, myPseudonimo, autenticando, elementoComun, viejoPseu;
        public long grafoGenerado, grafoIsomorfoGenerado;
        public static long ClavePublicaComun;
        public static int tam = 6;
        public static int[] listaaleatoria = new int[tam]; 
        public static int segundos;
        //static bool enviado2 = false;

        Utiles utiles = new Utiles();
        BD DButiles = new BD(Formulario);
        Cripto Criptutiles = new Cripto(Formulario);

        public Server(Form1 formu)//,int port)
        {
            Formulario = formu;
            this.listenThread = new Thread(new ThreadStart(ListenForClients));
            this.listenThread.Start();
        }

        private void ListenForClients()
        {
           // string ntelefono = utiles.CogerNtelefono();// "01";// 
           // DButiles.cambiamyPseu("PSEU1", ntelefono);
           // DButiles.cambiamyUser(ntelefono, ntelefono);
            myPseudonimo = DButiles.recuperamyPseu();
            viejoPseu = myPseudonimo;
            Formulario.Invoke(Formulario.myDelegate5, new Object[] { myPseudonimo });

            secuencia = "01";
            autenticando = "00";

            DetectaIncidente VigilaCarretera = new DetectaIncidente(Formulario);


            //AÑADIENDO UN POI
            //VigilaCarretera.addPoi(-1538936, 2786108, "Restaurante");

           // Formulario.Invoke(Formulario.myDelegate, new Object[] { "Phone:" + ntelefono });
            Formulario.Invoke(Formulario.myDelegate, new Object[] { "Waiting connections..." });

            lanzaServidor();
        }
        private void lanzaServidor()
        {
            string hostName = Dns.GetHostName();
            IPHostEntry thisHost = Dns.GetHostEntry(hostName);
            string thisIpAddr = thisHost.AddressList[0].ToString();//recogo mi IP 
           // Formulario.Invoke(Formulario.myDelegate4, new Object[] { thisIpAddr });

            if (!Formulario.cerrar)
            {
                hostName = Dns.GetHostName();
                thisHost = Dns.GetHostEntry(hostName);
                thisIpAddr = thisHost.AddressList[0].ToString();//recogo mi IP 
                Formulario.Invoke(Formulario.myDelegate4, new Object[] { thisIpAddr });

                Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);//gestionamos paquetes udp
                IPEndPoint iep = new IPEndPoint(IPAddress.Any, 9050);
                sock.Bind(iep);
                EndPoint ep = (EndPoint)iep;
                Console.WriteLine("Ready to receive...");

                byte[] data = new byte[1024];
                int recv = sock.ReceiveFrom(data, ref ep);//se queda esperando por el paquete entrante

                string stringData = Encoding.ASCII.GetString(data, 0, recv);
                //Formulario.Invoke(Formulario.myDelegate, new Object[] { stringData });

                IPEndPoint AddressIP = ep as IPEndPoint;//recuperamos ip del paquete entrante
                string clientIPAddress = "";
                if (AddressIP != null)
                    clientIPAddress = AddressIP.Address.ToString();

                // Formulario.Invoke(Formulario.myDelegate, new Object[] { "R:" + stringData });
                sock.Close();
                                
                Thread HiloServidor = new Thread(new ThreadStart(lanzaServidor));//lanzamos hilo para nueva conexion
                HiloServidor.IsBackground = true;
                HiloServidor.Start();
                //  HiloServidor.Join();
                
                string[] datosconexion=stringData.Split(',');
               // if (string.Equals(clientIPAddress, thisIpAddr))
                    //COMPROBAMOS LA BATERIA
                    //  Formulario.compruebaBateria();//la aplicación se apagará si tiene poca bateria.

                if (!string.Equals(clientIPAddress, thisIpAddr) && !string.Equals(clientIPAddress, "127.0.0.1") && !string.Equals(myPseudonimo, datosconexion[1]))// comprueba que el paquete no es mio
                {                   
                        gestionaConexion(stringData, clientIPAddress);//tratamos el paquete entrante                    
                }
                //Console.WriteLine("received: {0}  from: {1}", stringData, ep.ToString()); 
            }
        }


        private void gestionaConexion(string myString, string clientIPAddress)
        {
           
            string[] datosConexion = myString.Split(',');
            //COMPROBACIÓN DE AUTENTICADO, NO FUNCIONA COMENTAR SIEMPRE
            if (!DButiles.autenticado(datosConexion[1])) //que no esta autenticado
            {
                autenticar(myString, clientIPAddress, datosConexion);
            }
            else
            {
                if (datosConexion[0].Equals("01"))
                {
                    //DESCIFRAR DATOS
                    if (datosConexion[3].Equals("00"))//compruebo si es un cambio de pseudonimo
                        DButiles.cambiaPseuUser(datosConexion[1], datosConexion[2], datosConexion[4]);
                    else
                        DButiles.actualizaUser(datosConexion[1], datosConexion[2]);//recibe beacon y actualiza TS en BD
                    // Formulario.Invoke(Formulario.myDelegate, new Object[] { datosConexion[1] + " Authenticated" });
                }
                else
                    gestionaPaqueteAutenticado(myString, clientIPAddress, datosConexion);
            }
        }


        private void autenticar(string myString, string clientIPAddress, string[] datosConexion)
        {
            string auxiliar;
            Cliente myCliente = new Cliente(Formulario, "9050");
            Random randomNumber = new Random(DateTime.Now.Second);

            if ((datosConexion[0].Equals(secuencia) && autenticando.Equals(datosConexion[1]) ||
                ((datosConexion[0].Equals("D2") || datosConexion[0].Equals("D1") || datosConexion[0].Equals("01")) && autenticando.Equals("00"))))//  CONTROL DE SECUENCIA(solo funciona si se autentica 1 a la vez)
                switch (datosConexion[0])
                {
                    case "01"://RECIBE BEACON, ENVIA INICIO DE AUTENTICACIÓN
                        // if(){//COMPROBAR QUE TODAVIA NO ME HE AUTENTICADO
                        segundos = DateTime.Now.Second;
                        enviado = "D1";
                        secuencia = "D2";                               
                        //String IDs = DButiles.recuperaIDs(); 
                        string HashIDs = Criptutiles.generaHash(DButiles.recuperaIDs());//IDs); 
                        myCliente.respuesta("D1", myPseudonimo, HashIDs, clientIPAddress);
                        // comprobarReenvio("D1", myPseudonimo, HashIDs, clientIPAddress);                      
                        Formulario.Invoke(Formulario.myDelegate, new Object[] { "D1: Beacon, Send Authentication" });
                        break;
                    //}
                    case "D1"://PIDE IDs DE ALMACEN(B)
                        enviado = "D2";
                        secuencia = "D3";
                        autenticando = datosConexion[1];
                        hashaComprobar = datosConexion[2];
                       
                        string datos = "-";
                        myCliente.respuesta("D2", myPseudonimo, datos,clientIPAddress);//"-", clientIPAddress);
                        comprobarReenvio("D2", myPseudonimo, datos, clientIPAddress);//"-", clientIPAddress);
                        Formulario.Invoke(Formulario.myDelegate, new Object[] { "D2: Send request" });
                        break;

                    case "D2"://ENVIA IDs DE ALMACEN
                        enviado = "D3";
                        secuencia = "D4";

                        autenticando = datosConexion[1];
                        auxiliar = DButiles.recuperaIDs();
                        myCliente.respuesta("D3", myPseudonimo, auxiliar, clientIPAddress);
                        comprobarReenvio("D3", myPseudonimo, auxiliar, clientIPAddress);
                        Formulario.Invoke(Formulario.myDelegate, new Object[] { "D3: Send IDs of the Keystore" });
                        break;

                    case "D3"://COMPRUEBA IDs, ENVIA NODO COMÚN(X)
                        enviado = "D4";
                        secuencia = "D5";
                        autenticando = datosConexion[1];
                        //comprobar hash coincide con hash(IDs), 
                        string comprueba = Criptutiles.generaHash(datosConexion[2]);
                        if (comprueba.Equals(hashaComprobar))
                        {
                            //X=Comprobar ID comun,                       
                            string[] idAlmacenA = DButiles.recuperaIDs().Split(';');//adelantado a D1
                            string[] idAlmacenB = datosConexion[2].Split(';');
                            elementoComun = "0";
                            for (int i = 0; i < idAlmacenB.Length; i++)
                            {//Buscamos el elemento comun entre dos almacenes
                                for (int j = 0; j < idAlmacenA.Length; j++)
                                {
                                    if (string.Equals(idAlmacenA[j], idAlmacenB[i]))
                                    {
                                        elementoComun = idAlmacenA[j];
                                        i = idAlmacenB.Length;
                                        j = idAlmacenA.Length;
                                    }
                                }
                            }
                            if (!string.Equals("0", elementoComun))//)(string.Equals(comprueba, hashaComprobar) 
                            {
                                myCliente.respuesta("D4", myPseudonimo, elementoComun, clientIPAddress);//X
                                comprobarReenvio("D4", myPseudonimo, elementoComun, clientIPAddress);//
                                Formulario.Invoke(Formulario.myDelegate, new Object[] { "D4: OK, Send IDs common Node(X)" });
                            }
                            else
                            {
                                Formulario.Invoke(Formulario.myDelegate, new Object[] { "D4: Error: Not common X" });
                                autenticando = "00";
                                secuencia = "01";
                            }
                        }
                        else
                        {
                            Formulario.Invoke(Formulario.myDelegate, new Object[] { "D4: Error: Diferent Hash " });
                            autenticando = "00";
                            secuencia = "01";
                        }
                        break;

                    case "D4"://ENVIA (GRAFO + GRAFO_ISOMORFO)
                        enviado = "D5";
                        secuencia = "Z2";
                        elementoComun = datosConexion[2];
                        grafoGenerado = Criptutiles.generaGrafoAleatorio(datosConexion[2], DButiles.recuperaClavePublica(elementoComun));
                        grafoIsomorfoGenerado = Criptutiles.generaGrafoIsomorfo(grafoGenerado);
                        //RECUPERAR CLAVE PUBLICA Y PASAR DE NUMERO A Bits.
                        //string grafo= generaGrafo(recuperaPublicExponente(elementoComun);
                        //string grafoIsomorfo = generaIsomorfo(grafo);
                        auxiliar = grafoGenerado + ";" + grafoIsomorfoGenerado;
                        myCliente.respuesta("D5", myPseudonimo, auxiliar, clientIPAddress);//envio D5 y Z1 en el mismo paquete
                        comprobarReenvio("D5", myPseudonimo, auxiliar, clientIPAddress);
                        Formulario.Invoke(Formulario.myDelegate, new Object[] { "D5&Z1: Send Graph of KUx & Isomorphic" });
                        grafoIsomorfoGenerado = Criptutiles.generaGrafoIsomorfo(grafoGenerado);//GENERA NUEVO GRAFO ISOMORFO adelantado de Z2
                        break;

                    case "D5"://&&Z1 ENVIA PREGUNTA ALEATORIA
                        enviado = "Z2";
                        secuencia = "Z3";
                        //recibe grafo y isomorfo ;envia pregunta 1 (equivalencia entre grafo y grafo isomorfo (0) o circuito hamiltoniano del isomorfismo(1))
                        string[] grafos = datosConexion[2].Split(';');
                        grafo = grafos[0];//.ToString();
                        grafoIsomorfo = grafos[1];//.ToString();
                        pregunta = "1";// randomNumber.Next(2).ToString();
                        myCliente.respuesta("Z2", myPseudonimo, pregunta, clientIPAddress); //Pregunta
                        comprobarReenvio("Z2", myPseudonimo, pregunta, clientIPAddress);
                        Formulario.Invoke(Formulario.myDelegate, new Object[] { "Z2:Send Question 1" });
                        break;

                    //SE PUEDE PONER UN FOR PARA HACER VARIAS ITERACIONES
                    case "Z2"://ENVIA RESPUESTA + NUEVO_GRAFO_ISOMORFO
                        enviado = "Z3";
                        secuencia = "Z4";
                        // Genera Respuesta
                        // grafoIsomorfoGenerado = generaGrafoIsomorfo(grafoGenerado);//GENERA NUEVO GRAFO ISOMORFO
                        string respuesta = "";
                        if (datosConexion[2].Equals("0")) //equivalencia entre grafo y grafo isomorfo (0)(PRECALCULAR RESPUESTAS??)
                        {
                            for (int i = 0; i < listaaleatoria.Length; i++)
                            {
                                if (i == 0)
                                    respuesta = listaaleatoria[i].ToString();
                                else
                                    respuesta = respuesta + ":" + listaaleatoria[i].ToString();
                            }
                            respuesta = respuesta + ";" + grafoIsomorfoGenerado;
                        }
                        else if (datosConexion[2].Equals("1"))//circuito hamiltoniano del isomorfismo(1)
                        {
                            respuesta = Criptutiles.generaCHdeIsomorfo() + ";" + grafoIsomorfoGenerado;
                        }
                        else
                            respuesta = "0";
                        myCliente.respuesta("Z3", myPseudonimo, respuesta, clientIPAddress);//Respuesta1, Grafoisomorfo(X)
                        comprobarReenvio("Z3", myPseudonimo, respuesta, clientIPAddress);
                        Formulario.Invoke(Formulario.myDelegate, new Object[] { "Z3: Send Answer 1 + Isomorphic2" });

                        //misDatosCifrados1=Criptutiles.Encrypt(DButiles.recuperaMisDatosPublicos());//adelantado para Z4 
                        break;

                    case "Z3"://COMPRUEBA RESPUESTA Y RECIBO GRAFO_ISOMORFO PARA SIGUIENTE PREGUNTA
                        enviado = "Z4";
                        secuencia = "E1";
                        bool comprobacion = false;
                        string[] aux = datosConexion[2].Split(';');
                        if (pregunta.Equals("0"))//equivalencia entre grafo y grafo isomorfo (0)
                            // if (datosConexion[1])//permutar el grafo isomorfo y comprobar si es igual al grafo.
                            comprobacion = Criptutiles.compruebaIsomorfismo(aux[0], grafo, grafoIsomorfo);
                        /*if (compruebaIsomorfismo(datosConexion[1], grafo, grafoIsomorfo))
                            comprobacion = true;
                        else
                            comprobacion = false;*/
                        else if (pregunta.Equals("1"))//circuito hamiltoniano del isomorfismo(1)                                            
                            comprobacion = Criptutiles.esCH(aux[0], grafoIsomorfo);//CH del isomorfo

                        grafoIsomorfo = aux[1];//cargo el nuevo grafo isomorfo
                        if (comprobacion)
                        {
                            pregunta = (randomNumber.Next(2) % 1).ToString();  //genero nueva pregunta 
                            //Comprueba Respuesta1
                            myCliente.respuesta("Z4", myPseudonimo, pregunta, clientIPAddress);
                            comprobarReenvio("Z4", myPseudonimo, pregunta, clientIPAddress);
                            Formulario.Invoke(Formulario.myDelegate, new Object[] { "Z4: Checked-Send Question 2" });
                        }
                        else
                        {
                            Formulario.Invoke(Formulario.myDelegate, new Object[] { "Z3: Answer is Not correct" });
                            autenticando = "00";
                            secuencia = "01";
                        }
                        //misDatosCifrados2 = Criptutiles.Encrypt(DButiles.recuperaMisDatosPublicos());//adelantado para E1     
                        break;

                    case "Z4"://GENERA RESPUESTA SOBRE EL NUEVO GRAFO ISOMORFO, 
                        //EN EL ULTIMO ENVIO DE Zs SE ENVIA ID, CLAVE PUBLICA Y SECRETA DE A CIFRADA CON X = Ex(IDa,KUa,Ka)
                        enviado = "E1";
                        secuencia = "E2";
                        // Genera Respuesta
                        if (datosConexion[2].Equals("0"))//(PRECALCULAR RESPUESTAS??)
                        {
                            respuesta = "";
                            for (int i = 0; i < listaaleatoria.Length; i++)
                            {
                                if (i == 0)
                                    respuesta = listaaleatoria[i].ToString();
                                else
                                    respuesta = respuesta + ":" + listaaleatoria[i].ToString();
                            }
                        }
                        else if (datosConexion[2].Equals("1"))
                        {
                            respuesta = Criptutiles.generaCHdeIsomorfo();
                        }
                        else
                            respuesta = "0";

                        //busco en bd personal los datos a enviar
                        //EL ENCRYPT DEBE CAMBIARSE POR NUESTRO ENCRYPT Y DEBE TENER LA CLAVE COMO PARAMETRO (KUx)
                        //CIFRADO CON clavePublicaComun
                        auxiliar = respuesta + ";" + Criptutiles.Encrypt(DButiles.recuperaMisDatosPublicos(), ""+DButiles.recuperaClavePublica(elementoComun));// Encrypt("IDa,KUa,Ka");
                        myCliente.respuesta("E1", myPseudonimo, auxiliar, clientIPAddress);//cifradoSimetrico("IDentificador"));
                        comprobarReenvio("E1", myPseudonimo, auxiliar, clientIPAddress);
                        Formulario.Invoke(Formulario.myDelegate, new Object[] { "E1: Send Answer 2, Ex(IDa,KUa,Ka)" });
                        //almacenCifrado1 = Criptutiles.Encrypt(DButiles.recuperaKeyStore());//adelantado de E2
                        break;

                    case "E1"://COMPRUEBA ULTIMA PREGUNTA, DESCIFRA Ex(IDa,KUa,Ka) Y ENVIA CIFRADO CON Ka
                        enviado = "E2";
                        secuencia = "E3";
                        //comprobar respuesta y si es buena enviar datos
                        string[] aux2 = datosConexion[2].Split(';');
                        bool comprobacion2 = false;
                        if (pregunta.Equals("0"))//equivalencia entre grafo y grafo isomorfo (0)
                            comprobacion2 = Criptutiles.compruebaIsomorfismo(aux2[0], grafo, grafoIsomorfo);
                        else if (pregunta.Equals("1"))//circuito hamiltoniano del isomorfismo(1)                                            
                            comprobacion2 = Criptutiles.esCH(aux2[0], grafoIsomorfo);//CH del isomorfo

                        if (comprobacion2)
                        {   //DESCIFRAR DATOS

                            string datosDescifrados = Criptutiles.Decrypt(aux2[1], ""+DButiles.recuperaClavePublica(elementoComun));//DESCIFRAR CON clavePublicaComun
                            //aux=datosDescifrados.Split(',');
                            DButiles.addUser(datosDescifrados);//AÑADE O ACTUALIZA USUARIO EN LA BD
                            auxiliar = Criptutiles.Encrypt(DButiles.recuperaMisDatosPublicos(), ""+DButiles.recuperaClavePublica(elementoComun));//CIFRAR CON Kb
                            myCliente.respuesta("E2", myPseudonimo, auxiliar, clientIPAddress);//misDatosCifrados2);//, "Pas5pr@se", "s@1tValue", "SHA1", 2, "@1B2c3D4e5F6g7H8", 256));
                            comprobarReenvio("E2", myPseudonimo, auxiliar, clientIPAddress);
                            Formulario.Invoke(Formulario.myDelegate, new Object[] { "E2: Send Eka(IDb, Pseub,Modb, KUb,Kb);Decrypt: " + datosDescifrados });//, "Pas5pr@se", "s@1tValue", "SHA1", 2, "@1B2c3D4e5F6g7H8",256) });
                        }
                        else
                        {
                            Formulario.Invoke(Formulario.myDelegate, new Object[] { "E1:It is Not correct" });
                            autenticando = "00";
                            secuencia = "01";
                        }
                        //almacenCifrado2 = Criptutiles.Encrypt(DButiles.recuperaKeyStore());
                        break;

                    case "E2"://DESCIFRO Eka(IDb,KUb,Kb) Y ENVIO CIFRADO EL ALMACEN (Ekb(KeyStoreB))
                        enviado = "E3";
                        secuencia = "E4";

                        string datosDescifrados2 = Criptutiles.Decrypt(datosConexion[2], ""+DButiles.recuperaClavePublica(elementoComun));//DESCIFRAR CON clave secreta de A
                        DButiles.addUser(datosDescifrados2);
                        auxiliar = Criptutiles.Encrypt(DButiles.recuperaKeyStore(), DButiles.recuperamySK());//"KeystoreB");

                        myCliente.respuesta("E3", myPseudonimo, auxiliar, clientIPAddress);// almacenCifrado1);
                        comprobarReenvio("E3", myPseudonimo, auxiliar, clientIPAddress);
                        Formulario.Invoke(Formulario.myDelegate, new Object[] { "E3: Send Ekb(KeystoreB),Decrypt: " + datosDescifrados2 });//, "Pas5pr@se", "s@1tValue", "SHA1", 2, "@1B2c3D4e5F6g7H8", 256) });
                        break;

                    case "E3"://ENVIO CIFRADO ALMACEN A, DESCIFRO ALMACEN B Y AÑADO A REPOSITORIO LO QUE INTERESE.

                        string almacenA = Criptutiles.Decrypt(datosConexion[2], DButiles.recuperaSK(autenticando));
                        Formulario.Invoke(Formulario.myDelegate, new Object[] { "Decrypt Receive: " + almacenA });//,"Pas5pr@se", "s@1tValue", "SHA1", 2, "@1B2c3D4e5F6g7H8", 256) });//"Send Eka(KeystoreA)"
                        //LEER KEYSTORE Y AÑADIR EN REPOSITORIO                       
                        auxiliar = Criptutiles.Encrypt(DButiles.recuperaKeyStore(), DButiles.recuperamySK());//"KeystoreA");//adelantado en E1
                        myCliente.respuesta("E4", myPseudonimo, auxiliar, clientIPAddress);//almacenCifrado2);
                        comprobarReenvio("E4", myPseudonimo, auxiliar, clientIPAddress);
                        Formulario.Invoke(Formulario.myDelegate, new Object[] { "E4: Sending Ea(Keystore)" });
                        Formulario.Invoke(Formulario.myDelegate, new Object[] { "Updating DB with Max Degree. Add: " + DButiles.actualizaDB(almacenA) });
                        Formulario.Invoke(Formulario.myDelegate, new Object[] { datosConexion[1] + "- Authenticated" });
                        enviado = "E4";
                        secuencia = "E5";

                        break;

                    case "E4"://DESCIFRO ALMACEN A Y AÑADO A REPOSITORIO LO QUE INTERESE.
                        string almacenB = Criptutiles.Decrypt(datosConexion[2],DButiles.recuperaSK(autenticando));
                        Formulario.Invoke(Formulario.myDelegate, new Object[] { "Decrypt Receive: " + almacenB });//,"Pas5pr@se", "s@1tValue", "SHA1", 2, "@1B2c3D4e5F6g7H8", 256) });//"Send Eka(KeystoreA)"
                        Formulario.Invoke(Formulario.myDelegate, new Object[] { "Updating DB with Max Degree " });
                        DButiles.marcarAutenticado(datosConexion[1]);
                        Formulario.Invoke(Formulario.myDelegate, new Object[] { "Added to DB: " + DButiles.actualizaDB(almacenB) });
                        myCliente.respuesta("E5", myPseudonimo, "-", clientIPAddress);
                       // comprobarReenvio("E5", myPseudonimo, "-", clientIPAddress);
                        Formulario.Invoke(Formulario.myDelegate, new Object[] { "E5: sending" });

                        if (segundos < DateTime.Now.Second)
                            segundos = DateTime.Now.Second - segundos;
                        else
                            segundos = DateTime.Now.Second + 60 - segundos;
                        Formulario.Invoke(Formulario.myDelegate, new Object[] { datosConexion[1] + "- Authenticated. Seconds: " + segundos });
                        secuencia = "01";
                        enviado = "00";
                        autenticando = "00";
                        
                        enviarEventos(myPseudonimo, clientIPAddress);//enviar eventos actuales
                      
                        break;
                    case "E5"://DESCIFRO ALMACEN A Y AÑADO A REPOSITORIO LO QUE INTERESE.
                        DButiles.marcarAutenticado(datosConexion[1]);                       

                        enviarEventos(myPseudonimo, clientIPAddress); //Enviar eventos actuales

                        secuencia = "01";
                        enviado = "00";
                        autenticando = "00";
                        break;
                    default:
                        Formulario.Invoke(Formulario.myDelegate, new Object[] { "Bad FC" });
                        break;
                }
        }

        private void enviarEventos(string pseuUsuario, string clientIPAddress)
        {
            Cliente myCliente = new Cliente(Formulario, "9050");
            string[] eventos = DButiles.recuperaEventos();
            //Formulario.Invoke(Formulario.myDelegate, new Object[] { "Sending Events to: "+pseuUsuario });
            string[] evCortados;
            if (eventos != null)
            for (int i = 0; i < eventos.Length; i++)//Hay que distinguir los eventos porque la publicidad se envia con firma
            {
                evCortados = eventos[i].Split(';');
               // if (evCortados[0].Equals("P1"))//paquete de publicidad
                    myCliente.respuesta(evCortados[0], pseuUsuario, Criptutiles.Encrypt(evCortados[1] + ";" + evCortados[2] + ";" + evCortados[3] + ";" + evCortados[4] + ";" + evCortados[5] + ";" + evCortados[6] + ";" + evCortados[7] + ";" + evCortados[8], DButiles.recuperamySK()), clientIPAddress);    //+ ";" + evCortados[4]                   
               // else
               //     myCliente.respuesta(evCortados[0], pseuUsuario, Criptutiles.Encrypt(evCortados[1] + ";" + evCortados[2] + ";" + evCortados[3] + ";" + evCortados[4] + ";" + evCortados[5] + ";" + evCortados[6] + ";" + evCortados[7] + ";" + evCortados[8], DButiles.recuperamySK()), clientIPAddress);    //+ ";" + evCortados[4]                   
               
                Formulario.Invoke(Formulario.myDelegate, new Object[] { "Sending: " + evCortados[0]+":"+ pseuUsuario });
            }
        }

        private void gestionaPaqueteAutenticado(string myString, string clientIPAddress, string[] datosConexion)
        {
            Cliente myCliente = new Cliente(Formulario, "9050");
            string[] coordenadas;
            //string dirimconges;
            //int IDbitmap;
            //SError err;
            BD AccesoBD= new BD(Formulario);

            /*******************************
                Para agregacion
             *******************************/
            string msje, tipo,nombreVia,sentidoMarcha;
            double X = 0.0, Y = 0.0,Z=0.0;
            string[] info;
            
            switch (datosConexion[0])
            {
                case "D1"://el nodo ha perdido el cambio de beacon
                   string msj = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ",00," + Server.myPseudonimo + ", Ek1(00:TimeStamp:newPseu)";
                   myCliente.respuesta("01", viejoPseu, msj, clientIPAddress);
                    break;
                case "T1"://paquete de tráfico, comprobamos validez
                    datosConexion[2] = Criptutiles.Decrypt(datosConexion[2], DButiles.recuperaSK(datosConexion[1]));
                    coordenadas = datosConexion[2].Split(';');

                    lock (this)
                    {

                        info = datosConexion[2].Split('_');
                        tipo = info[0];

                        //Si es un mensaje de aggregación
                        if (tipo.Equals("I"))
                        {
                            Formulario.Invoke(Formulario.myDelegate, new Object[] { "An information packet arrives" });

                            msje = info[1];
                            nombreVia = info[2];
                            sentidoMarcha = info[3];
                            X = double.Parse(info[4]);
                            Y = double.Parse(info[5]);
                            Z = double.Parse(info[6]);
                            Formulario.Invoke(Formulario.myDelegate, new Object[] { msje + " in " + nombreVia + "direction " + sentidoMarcha + X + ", " + Y });
                            System.Globalization.CultureInfo myCIintl = new System.Globalization.CultureInfo("es-ES", false);
                            agregacion PaqAg = new agregacion(Formulario, nombreVia, sentidoMarcha, X, Y, Z, DateTime.ParseExact(info[7], "MM/dd/yyyy HH:mm:ss", myCIintl));
                            PaqAg.InformacionAggreacion(msje, clientIPAddress);
                        }
                        //Si es una respuesta de aggregación

                        //Formato del paquete|F-Atasco en la M40-ID-hash.ToString|
                        if (tipo.Equals("F"))
                        {
                            Formulario.Invoke(Formulario.myDelegate, new Object[] { "LLega un paquete F" });
                            Formulario.Invoke(Formulario.myDelegate, new Object[] { "Compruebo si estoy a la espera de firmas" });
                            if (AggregarFirmas.getEsperoFirmas())
                            {
                                //Formamos el paquete con la información que nos va llegando de los diferentes vehículos
                                //string IDPaquete=info[1];
                                msje = info[1];
                                nombreVia = info[2];
                                sentidoMarcha = info[3];
                                string IDVehículo = info[4];
                                string firma = info[5];
                                string data = "A signed packet arrives from " + IDVehículo.ToString();
                                Formulario.Invoke(Formulario.myDelegate, new Object[] { data });
                                X = double.Parse(info[6]);
                                Y = double.Parse(info[7]);
                                Z = double.Parse(info[8]);
                                DateTime myDateTime = DateTime.Now;
                                string posiblevento;

                                posiblevento = AccesoBD.ExistePosibleEvento("T1", nombreVia, sentidoMarcha, X, Y);
                                Formulario.Invoke(Formulario.myDelegate, new Object[] { "¿Existe el posible Evento?" });
                                if (!(posiblevento.Equals("") && posiblevento.Equals("error")))
                                {
                                    Formulario.Invoke(Formulario.myDelegate, new Object[] { "SI" });
                                    Formulario.Invoke(Formulario.myDelegate, new Object[] { "LO esperaba" });
                                    if (AggregarFirmas.getnumerofirmas() <= 0)
                                    {
                                        string IP = IPAddress.Broadcast.ToString();
                                        string firmas = AggregarFirmas.getfirmas();
                                        AggregarFirmas.agregafirmas(firmas + firma + "|" + IDVehículo);
                                        AggregarFirmas.incrementarfirmas();
                                        Formulario.Invoke(Formulario.myDelegate, new Object[] { "AgregoFirma con id VEhículo, Lo inserto en BD" });                                        
                                        AccesoBD.insertarEvento("T1", nombreVia, sentidoMarcha, X, Y, Z, myDateTime, firma);
                                        //utiles.anadePoi("Atasco", "Atasco", (int)(X*100000), (int)(Y*100000), 100);
                                        utiles.anadePoi("Atasco", "Atasco", (int)(X), (int)(Y), 100);
                                        Cliente Client = new Cliente(Formulario, "9050");
                                        string prueba = "A_Traffic Jam." + AggregarFirmas.getfirmas() + "_" + nombreVia + "_" + sentidoMarcha + "_" + X.ToString() + "_" + Y.ToString() + "_" + Z.ToString() + "_" + myDateTime.ToString("MM/dd/yyyy HH:mm:ss");
                                        Client.respuesta("T1", myPseudonimo, Criptutiles.Encrypt(prueba, AccesoBD.recuperamySK()), IP);
                                        Formulario.Invoke(Formulario.myDelegate, new Object[] { "Send an aggregation Packet" });
                                        AggregarFirmas.resetEsperoFirmas();
                                    }
                                }
                            }
                            else
                            {
                                Formulario.Invoke(Formulario.myDelegate, new Object[] { "No espero paquete F, no hago nada" });
                            }
                        }//end IF F
                        if (tipo.Equals("A"))
                        {
                            Formulario.Invoke(Formulario.myDelegate, new Object[] { "An agregation packet arrives" });

                            msje = info[1];
                            nombreVia = info[2];
                            sentidoMarcha = info[3];
                            X = double.Parse(info[4]);
                            Y = double.Parse(info[5]);
                            Z = double.Parse(info[6]);
                            System.Globalization.CultureInfo myCIintl = new System.Globalization.CultureInfo("es-ES", false);
                            agregacion PaqAg = new agregacion(Formulario, nombreVia, sentidoMarcha, X, Y, Z, DateTime.ParseExact(info[7], "MM/dd/yyyy HH:mm:ss", myCIintl));
                            PaqAg.MensajeAgregado(msje);
                        }//end IF A
                    }//end lock

                    break;

                case "P1"://paquete de publicidad, oomprobamos validez e insertamos en el mapa en caso de ser válido                    
                    //COMPROBANDO FIRMA CIFRADA (CLAVE PUBLICA)
                    string datosDescifrados=Criptutiles.RSAverifica(datosConexion[1], datosConexion[2], true);
                    if (datosDescifrados.Equals(""))                    
                   // if (!RSA3.VerifyData(decryptedData, "SHA1", signedData))// (dataToEncrypt, "SHA1", signedData);
                        Formulario.Invoke(Formulario.myDelegate, new Object[] { "Advertisement Data not Correct" });
                    else
                    {
                        //Formulario.Invoke(Formulario.myDelegate, new Object[] { "Advertisement Data Correct" });                    
                        //datosConexion[2] = Criptutiles.Decrypt(datosConexion[2], DButiles.recuperaSK(datosConexion[1]));
                        coordenadas = datosDescifrados.Split(';');

                        Formulario.Invoke(Formulario.myDelegate, new Object[] { "Advertisement: " + coordenadas[0]+" in "+ coordenadas[2] + "," + coordenadas[3] });
                        DButiles.insertarEvento("P1", coordenadas[0], coordenadas[1], double.Parse(coordenadas[2]), double.Parse(coordenadas[3]), double.Parse(coordenadas[4]), DateTime.Parse(coordenadas[5]), coordenadas[coordenadas.Length-1]);
                        utiles.anadePoi("El Corte Ingles","Comercio", int.Parse(coordenadas[2]), int.Parse(coordenadas[3]), 100);
                       /* try
                        { 
                            int res = CApplicationAPI.DeletePoiCategory(out err, coordenadas[0], "ESP", 1000);
                            string dirImagen = @coordenadas[0]+".bmp";
                            res = CApplicationAPI.AddPoiCategory(out err, coordenadas[3],dirImagen, "ESP", 1000);
                            LONGPOSITION position = new LONGPOSITION(int.Parse(coordenadas[2]), int.Parse(coordenadas[3]));//X, Y);           
                            SPoi poi = new SPoi(position, "Comercio", coordenadas[0], 1000);
                            res = CApplicationAPI.AddPoi(out err, ref poi, 1000);   
                        }
                        catch {}*/
                    }
                    break;
                    
                case "P2"://paquete de aparcamiento
                    //COMPROBANDO FIRMA CIFRADA (CLAVE PUBLICA)
                    string datosDescifrados2=Criptutiles.RSAverifica(datosConexion[1], datosConexion[2],false);
                    if (datosDescifrados2.Equals(""))
                        // if (!RSA3.VerifyData(decryptedData, "SHA1", signedData))// (dataToEncrypt, "SHA1", signedData);
                        Formulario.Invoke(Formulario.myDelegate, new Object[] { "Possible Parking Data not Correct" });
                    else
                    {
                        // datosConexion[2] = Criptutiles.Decrypt(datosConexion[2], DButiles.recuperaSK(datosConexion[1]));                      
                        coordenadas = datosDescifrados2.Split(';');
                        Formulario.Invoke(Formulario.myDelegate, new Object[] { "Parking in: " + coordenadas[0] + " sense " + coordenadas[1] + "coordinates " + coordenadas[2] + "," + coordenadas[3] });
                        DButiles.insertarEvento("P2", coordenadas[0], coordenadas[1], double.Parse(coordenadas[2]), double.Parse(coordenadas[3]), double.Parse(coordenadas[4]), DateTime.Parse(coordenadas[5]), coordenadas[coordenadas.Length - 1]);
                        utiles.anadePoi("Posible Plaza", "Posible Plaza", int.Parse(coordenadas[2]), int.Parse(coordenadas[3]),100);
                       /* try
                        {
                            int res = CApplicationAPI.DeletePoiCategory(out err, "Posible Plaza", "ESP", 1000);
                            string dirImagen = @"parkingsinfondo.bmp";
                            res = CApplicationAPI.AddPoiCategory(out err, "Posible Plaza", dirImagen, "ESP", 1000);
                            LONGPOSITION position = new LONGPOSITION(int.Parse(coordenadas[2]), int.Parse(coordenadas[3]));//X, Y);
                            SPoi poi = new SPoi(position, "Posible Plaza", "Posible plaza", 1000);
                            res = CApplicationAPI.AddPoi(out err, ref poi, 1000);
                            //if (CApplicationAPI.AddBitmapToMap(out err, dirimconges, int.Parse(coordenadas[2]), int.Parse(coordenadas[3]), out IDbitmap, 1000) == 1)
                        }
                        catch
                        {}*/
                    }
                    break;

                default:
                    break;
            }
        }

        public void comprobarReenvio(string señal, string pseu, string paquete, string clientIPAddress)//string direccion,
        {//INICIO UN NUEVO HILO EN CADA ENVIO DE PAQUETE
            Reenviador hilo = new Reenviador(señal, pseu, paquete, clientIPAddress);//direccion,
            Thread HiloReenvío = new Thread(new ThreadStart(hilo.reenviarPaquete));//, señal,direccion,paquete));
            HiloReenvío.IsBackground = true;
            HiloReenvío.Start();
            // HiloReenvío.Join();       
        }
        public class Reenviador
        {
            string señal, pseu, paquete, direccion;
            public Reenviador(string señal, string pseu, string paquete, string direccion)//
            {
                this.señal = señal;
                this.paquete = paquete;
                this.pseu = pseu;
                this.direccion = direccion;
            }
            public void reenviarPaquete()
            {
                Cliente myCliente = new Cliente(Formulario, "9050");
                //System.Threading.
                Thread.Sleep(7000);//TIMESTAMP
                int intentos = 0;
                while (enviado.Equals(señal) && (intentos <= 1) && !Formulario.cerrar)
                {
                    intentos++;
                    Formulario.Invoke(Formulario.myDelegate, new Object[] { "Reenviando" });
                    myCliente.respuesta(señal, pseu, paquete, direccion);//, "Pas5pr@se", "s@1tValue", "SHA1", 2, "@1B2c3D4e5F6g7H8", 256));
                    System.Threading.Thread.Sleep(5000);//TIMESTAMP
                }
                if (intentos >= 2)
                {
                    secuencia = "01";
                    enviado = "00";
                    autenticando = "00";
                }
            }
        }        
    }
}