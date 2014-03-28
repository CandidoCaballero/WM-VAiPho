using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

using System.Net;
using Microsoft.Win32;
using Microsoft.WindowsMobile.Status;

using System.Security.Cryptography;//para rsa

using ApplicationAPI;//PARA POI

using System.IO;//para LOG

//using System.Net.Sockets;

namespace VAIPHO
{

    public partial class Form1 : Form
    {
        public delegate void AddListItem(String myItem);
        public AddListItem myDelegate;
        public AddListItem myDelegate2;
        public AddListItem myDelegate3;
        public AddListItem myDelegate4;
        public AddListItem myDelegate5;          
      
        public int CoorX2;


        public Boolean cerrar = false;
        private System.Windows.Forms.Timer timer1 = new System.Windows.Forms.Timer();
       // public static int atascado;
               
        const string registryKey = @"HKEY_LOCAL_MACHINE\System\State\Hardware"; //para detectar apagado de bluetooth
        const string registryValueName = @"Bluetooth"; 
        const string applicationFileName = "\\Tarjeta de almacenamiento\\FLEET_MOBILE_14d\\Drive\\WindowsMobile\\Drive.exe";
        const string applicationImages = "\\Tarjeta de almacenamiento\\FLEET_MOBILE_14d\\images\\";
        
        Microsoft.WindowsMobile.Status.RegistryState rg;//para controlar si el bluetooth se apaga


        public Form1()
        {
            InitializeComponent();
            myDelegate = new AddListItem(AddListItemMethod);
            myDelegate2 = new AddListItem(AddVelocidad);
           // myDelegate3 = new AddListItem(AddPosicion);
            myDelegate4 = new AddListItem(AddIp);
            myDelegate5 = new AddListItem(AddPseu);
            CoorX2 = -1632751;
       }

        public void Log(string cadena)
        {
            try
            {
                // Get the creation time of a well-known directory.
                DateTime dt = Directory.GetCreationTime("log.txt");
                if (DateTime.Now.Subtract(dt).TotalDays >= 30)
                {
                    File.Delete("log.txt");
                }
                //Si no existe lo crea, si existe lo escribe
                StreamWriter w = File.AppendText("log.txt");
                w.Write("\r\nLog Entry : ");
                w.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                    DateTime.Now.ToLongDateString());
                w.WriteLine("  :");
                w.WriteLine("  :{0}", cadena);
                w.WriteLine("-------------------------------");
                // Update the underlying file.
                w.Flush();
                w.Close();
                
            }

            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }

                       

        }

   

        private void Play_Click(object sender, EventArgs e)//Iniciamos aplicacion
        {
            progressBar1.Maximum = 7;
            progressBar1.Value = 1;
            cerrar = false;            
            Invoke(myDelegate, new Object[] { "Loading... Please Wait." });
            Play.Enabled = false;//DESABILITO PLAY
            Play.Visible = false;

            //Habilitamos hardware
            Hardware detect = new Hardware();
            if (detect.BluetoothOn())//habilita bluetooth
                Invoke(myDelegate, new Object[] { "Enabled Bluetooth successfully." });
            if (detect.WifiOn())// habilita wifi
                Invoke(myDelegate, new Object[] { "Enabled Wifi successfully." });
            if (detect.conectawifi("vaipho"))//conecta a la red vaipho
                Invoke(myDelegate, new Object[] { "VAiPho conected successfully." });
            progressBar1.Value = 2;
            //Si el manos libres del coche se apaga, VAiPho se apagará también (para evitar atascos inexistentes)          
            int key = (int)Registry.GetValue(registryKey, "Bluetooth", -1);
            rg = new RegistryState(registryKey, registryValueName);
            rg.Changed += new ChangeEventHandler(bluetooth_Changed);
            progressBar1.Value = 3;
           
            //LA CREACION DE LA BD NO SE DEBE EJECUTAR EN LA VERSIÓN FINAL
            Invoke(myDelegate, new Object[] { "Generating DB." });
            BD basededatos = new BD(this);
            //if (basededatos.crearBD())
                if (basededatos.crearTablas())
                {
                    progressBar1.Value = 4;
                    Server Servidor = new Server(this);//, 9050);                                   
                    Cliente Cliente1 = new Cliente(this, "9050");

                    progressBar1.Value = 5;
                    string hostName = Dns.GetHostName();
                    IPHostEntry thisHost = Dns.GetHostEntry(hostName);
                    labelIP.Text = thisHost.AddressList[0].ToString();
                    labelPseu.Text = basededatos.recuperamyPseu();
                    Thread hiloCliente = new Thread(new ThreadStart(Cliente1.beacon));
                    hiloCliente.Start();
                    progressBar1.Value = 6;
                    DetectaIncidente VigilaCarretera = new DetectaIncidente(this);//Aquí dentro se lleva a cabo todo lo de detectarIncidentes
                    VigilaCarretera.DetectaIncidenteStart();
                    //VigilaCarretera.posibleparking();//señala un posible aparcamiento(al encenderse el servicio, no en el play) 
                    progressBar1.Value = 7;
                    /*Prueba VigilaCarretera = new Prueba(this);
                    VigilaCarretera.DetectaIncidenteStart();*/
                }
                progressBar1.Value = 0;
        }
         
        void bluetooth_Changed(object sender, ChangeEventArgs args)
        {
            //Encendemos la PDA en caso de que se encienda o apague el BT 
            string rgValue = rg.CurrentValue.ToString();
            switch (rgValue)
            {
                case "8": // accion = "BlueTooth Off";                  
                    break;
                case "9":// accion = "BlueTooth On";                   
                    //desconectamos aplicación VAiPho (si está encendida)
                    pausandoServicios();
                    cerrandoAplicacion();
                    Close();
                    break;

                default:
                    break;
            }
        }
        
        public void AddListItemMethod(String myItem)
        {
            txtLog.Text = myItem + "\r\n" + txtLog.Text;
        }

        public void AddVelocidad(String myItem)
        {
            labelSpeed.Text = myItem;
        }

       /* public void AddPosicion(String myItem)
        {
            label5.Text = myItem;
        }*/

        public void AddIp(String myItem)
        {
            labelIP.Text = myItem;
        }

        public void AddPseu(String myItem)
        {
            labelPseu.Text = myItem;
        }

       /* public void PlayEnabled( bool habilitar)
        {
            Play.Enabled = habilitar;
        }*/

        /* public void AddInfo(String myItem)
         {
             label7.Text = myItem;
         }*/

        private void menuItem1_Click(object sender, EventArgs e)
        {
            cerrandoAplicacion();
            Close();
        }
                
        private void menuItem2_Click(object sender, EventArgs e)
        {
            txtLog.Text = "";//borramos la pantalla
        }
               
        private void pausandoServicios()//Paramos aplicación
        {
            cerrar = true;
            Invoke(myDelegate, new Object[] { "Stop Services." });
            Cliente cierracliente = new Cliente(this, "9050");//,"127.0.0.1","bye");
            cierracliente.respuesta("01", Server.myPseudonimo, "-", "127.0.0.1");//,
           // PlayEnabled(true);
        }

        private void cerrandoAplicacion()//Cerramos aplicación y GPS
        {
            try
            {
                cerrar = true;
                Cliente cierracliente = new Cliente(this, "9050");//,"127.0.0.1","bye");
                cierracliente.respuesta("01", Server.myPseudonimo, "-", "127.0.0.1");//,
                 string _appFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
                 string dirSonido = _appFolder + @"\Sounds\apagando.wav";
                dirSonido = "\\Tarjeta de almacenamiento\\FLEET_MOBILE_14d\\MisSonidos\\apagando.wav";//parking.wav";
                
                WSounds ws = new WSounds();
                ws.Play(dirSonido, ws.SND_FILENAME | ws.SND_ASYNC);
               
                DriveHandler.EndApplication(0);
                DriveHandler.CloseApi();
                Hardware detect = new Hardware();
                detect.WifiOff();

            }
            catch
            {
                //    Formulario.Invoke(Formulario.myDelegate, new Object[] { "Error software GPS: " + e.Message });
            }            
        }

        public void compruebaBateria()
        {
            BatteryLevel batteryId = Microsoft.WindowsMobile.Status.SystemState.PowerBatteryStrength;
            if (batteryId.ToString().Equals("VeryLow") || batteryId.ToString().Equals("Low"))//si la bateria no está baja lanzamos vaipho
            {               
                pausandoServicios();
                cerrandoAplicacion(); 
            }
        }

        private void publicidad_Click(object sender, EventArgs e)//La publicidad debe recoger de un fichero la firma y mandarla a cada usuario con el que se autentique.
        {         
            BD DButiles = new BD(this);
            int CoorX = -1632721;// 0899; //-1538956;//
            int CoorY = 2848901;// 704;// 924;// 2309;//2786158;// 
            int CoorZ = 0;
            SError err;
            SGpsPosition gps;
            try
            {
                if (CApplicationAPI.GetActualGpsPosition(out err, out gps, false, 1000) == 1)
                {
                    CoorX = gps.Longitude;
                    CoorY = gps.Latitude;
                }
            }
            catch { }

            string viaOnombre = "El Corte Ingles";//El Corte Ingles
            string sentido = "sentido";
            DateTime fechaDato = DateTime.Now;
            string datoPublicidad = viaOnombre + ";" + sentido + ";" + CoorX + ";" + CoorY + ";" + CoorZ + ";" + fechaDato;//Estos datos los coge de un fichero
            //GENERAMOS FIRMA (ESTO ES PARA PRUEBAS)En la versión final esta firma la recogerá de un fichero que le pasará la autoridad
            Utiles utiles = new Utiles();
            byte[] dataToSign = utiles.StrToByteArray(datoPublicidad);
            RSAParameters Cprivada = new RSAParameters()
            {
                D = Convert.FromBase64String("7QePOux7La+Y3jWwOwyHXqCDiduvXQv23TrfVX6cvTmr9BH0FtBzf4dbxYumjreztNrmX+wYsWH5W4pycB67S/tWODZrlrR8zuCCjWauC3ZjPWnlyU+Npg0qzLm7XkCLiuveQWpR4E/TWcs6Wr9pVL2zXT/BsWYC9t39qvkhTKE="),
                DP = Convert.FromBase64String("fJnDOGGMlWgVoQ+7MZtUfivpChykRC39W5UyTnnZ8+xxkt67nzlxXs2wl3w8EV1wRGYPXr0KfjFldUXYGd8h2Q=="),
                DQ = Convert.FromBase64String("3Ls7pKVPqzABAUxQ6jTKypsH7Zd+DJDhQfQset2sK15DmDU+cAY2BFufKvsojfYI2UKKtqvoIGKrWzZ+zrwKOQ=="),
                Exponent = Convert.FromBase64String("AQAB"),
                InverseQ = Convert.FromBase64String("rWWEy1hBu44BBcjuZwFNiixwEQobmHCG+ZqFRldjJKT/2RThzKNc9Q6vYwR52WSwGNxLmlwTvRb2p/gM13WaVQ=="),
                Modulus = Convert.FromBase64String("98Ej5BZ/VMG4nxCzdMZoZ8V50//GvnEQc3CX4vyHzDjOfUGB21ZjVF12s+h3ZQmQX/Woq1zZM6sNsTLVG2SiQhzwWIEE7ioyr2vn1OjE17QOlmrVtl8lI4txnZQQh8jaq1mEi1lqI7JMvwBr+AmTWz+Vf5RraWv/a7qonMDovyM="),
                P = Convert.FromBase64String("/SEbB4+LoAyHzUFTxyuA2mOJdZYIMiugNv9hUZQUjRzVbavfGVaxP/Pu5nxkyzmPtgcTn3m9nwbqwQSlLwEDdw=="),
                Q = Convert.FromBase64String("+pBuG5WisL4wg+B5auoXSQ5Q0/v4405ypMdQ5p6mG2notIamTZ4sp3m2+PpQOitlXrOQemlL3oaed2SH2XQUtQ==")
            };
            RSACryptoServiceProvider RSA2 = new RSACryptoServiceProvider();
            RSA2.ImportParameters(Cprivada);
            byte[] signedData = RSA2.SignData(dataToSign, "SHA1");
             string firma=Convert.ToBase64String(signedData).ToString();//
            
            Cliente clientePubli = new Cliente(this, "9050");
            Cripto criputiles = new Cripto(this);
            Invoke(myDelegate, new Object[] { "Sending Advertisements." });
           //SError err;
           /* int res = CApplicationAPI.DeletePoiCategory(out err, "El Corte Inglés", "ESP", 0);//(Borramos Para las pruebas)
             res = CApplicationAPI.DeletePoiCategory(out err, "28482309", "ESP", 0);//(Borramos Para las pruebas)
             res = CApplicationAPI.DeletePoiCategory(out err, "atasco", "ESP", 0);//(Borramos Para las pruebas)
             res = CApplicationAPI.DeletePoiCategory(out err, "comercio", "ESP", 0);//(Borramos Para las pruebas)
             res = CApplicationAPI.DeletePoiCategory(out err, "el corte ingles", "ESP", 0);//(Borramos Para las pruebas)
             res = CApplicationAPI.DeletePoiCategory(out err, "elcorteingles", "ESP", 0);//(Borramos Para las pruebas)
             res = CApplicationAPI.DeletePoiCategory(out err, "posible plaza", "ESP", 0);//(Borramos Para las pruebas)
             res = CApplicationAPI.DeletePoiCategory(out err, "restaurante", "ESP", 0);//(Borramos Para las pruebas)*/


       DButiles.insertarEvento("P1", viaOnombre, sentido, CoorX, CoorY, CoorZ, fechaDato, firma); //-1538936, 2786108, DateTime.Now);//Arinaga
            clientePubli.respuesta("P1", Server.myPseudonimo, criputiles.Encrypt(datoPublicidad+ ";" + firma, DButiles.recuperamySK()) , IPAddress.Broadcast.ToString());//,
            //utiles.anadePoi("El Corte Ingles", "Comercio", CoorX, CoorY, 100);
            utiles.anadePoi("banesto", "Comercio", CoorX, CoorY, 100);
        /*         //----------------------------------------------------------------------------------------------
            //ENVIAMOS APARCAMIENTO TB (PARA PRUEBA) 
            CoorX = CoorX + 200;//-1631287;
            //CoorY = 2850168;
            viaOnombre = "calle1";
            sentido = "sentido1";
            fechaDato = DateTime.Now;
            string datoAparcamiento = viaOnombre + ";" + sentido + ";" + CoorX + ";" + CoorY + ";" + CoorZ + ";" + fechaDato;//Estos datos los coge de un fichero
            firma= criputiles.RSAfirma(datoAparcamiento);

            Invoke(myDelegate, new Object[] { "Sending Possible Parking" });
            DButiles.insertarEvento("P2", viaOnombre, sentido, CoorX, CoorY, CoorZ, fechaDato, firma);
            clientePubli.respuesta("P2", Server.myPseudonimo, criputiles.Encrypt(datoAparcamiento + ";"+firma, DButiles.recuperamySK()), IPAddress.Broadcast.ToString());//,
            utiles.anadePoi("Posible Plaza", "Posible Plaza", CoorX, CoorY, 100);
            //----------------------------------------------------------------------------------------------
            //ENVIAMOS ATASCO TB (PARA PRUEBA) 

            CoorX = CoorX + 200;// -1538956;
           // CoorY = 2786158;
            viaOnombre = "calle1";
            sentido = "sentido1";
            fechaDato = DateTime.Now;
            string datoAtasco = viaOnombre + ";" + sentido + ";" + CoorX + ";" + CoorY + ";" + CoorZ + ";" + fechaDato;//Estos datos los coge de un fichero
            firma = criputiles.RSAfirma(datoAtasco);

            Invoke(myDelegate, new Object[] { "Sending Possible Traffic Jam" });
            DButiles.insertarEvento("T1", viaOnombre, sentido, CoorX, CoorY, CoorZ, fechaDato, firma);
            clientePubli.respuesta("T1", Server.myPseudonimo, criputiles.Encrypt(datoAtasco + ";" + firma, DButiles.recuperamySK()), IPAddress.Broadcast.ToString());//,
            utiles.anadePoi("Atasco", "Atasco", CoorX, CoorY, 100);*/

        }

        private void Form1_Load(object sender, EventArgs e)
        {           
            cerrar = false;
            Play.Visible = false;
            progressBar1.Maximum = 8;
            progressBar1.Value = 1;
            Invoke(myDelegate, new Object[] { "Loading... Please Wait." });            
            Play.Enabled = false;//DESABILITO PLAY

            
            //byte[] m_soundBytes = new byte[Properties.Resources.iniciando1.Length];
            //Properties.Resources.iniciando1.Read(m_soundBytes, 0, (int)Properties.Resources.iniciando1.Length);
          //  new System.Media.SoundPlayer(
            //System.IO.Stream stream = (System.IO.Stream)Properties.Resources.ResourceManager.BaseName. "iniciando1",false,false);
           // System.Media.SoundPlayer player = new System.Media.SoundPlayer(stream);
           //     Properties.Resources.iniciando1.Play();
            //WSounds ws1 = ;
            
           // string patch = System.AppDomain.CurrentDomain.BaseDirectory.ToString() BaseDirectory;// BaseDirectory;
            string _appFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            string dirSonido = _appFolder + @"\Sounds\iniciando.wav";//\Sounds\\Sonidos"\\Tarjeta de almacenamiento\\FLEET_MOBILE_14d\\MisSonidos\\iniciando.wav";//Properties.Resources.ResourceManager.BaseName+"\\// "..\\Resources\\iniciando.wav";//\\Tarjeta de almacenamiento\\FLEET_MOBILE_14d\\MisSonidos\\iniciando.wav";
            dirSonido = "\\Tarjeta de almacenamiento\\FLEET_MOBILE_14d\\MisSonidos\\iniciando.wav";

            try
            {
                WSounds ws = new WSounds();
                ws.Play(dirSonido, ws.SND_FILENAME | ws.SND_ASYNC);
            }
            catch (Exception ex)
            {
                Invoke(myDelegate, new Object[] { "Error: " + ex.Message });
            }

            Hardware detect = new Hardware();
            if (detect.BluetoothOn())//habilita bluetooth
                Invoke(myDelegate, new Object[] { "Enabled Bluetooth successfully." });
            if (detect.WifiOn())// habilita wifi
                Invoke(myDelegate, new Object[] { "Enabled Wifi successfully." });
            if (detect.conectawifi("vaipho"))//conecta a la red vaipho
                Invoke(myDelegate, new Object[] { "VAiPho connected successfully." });
            progressBar1.Value = 2;

            //Si el manos libres del coche se apaga, VAiPho se apagará también (para evitar atascos inexistentes)          
            int key = (int)Registry.GetValue(registryKey, "Bluetooth", -1);
            rg = new RegistryState(registryKey, registryValueName);
            rg.Changed += new ChangeEventHandler(bluetooth_Changed);

            progressBar1.Value = 3;
            //LA CREACION DE LA BD NO SE DEBE EJECUTAR EN LA VERSIÓN FINAL
            Invoke(myDelegate, new Object[] { "Generating DB." });
            BD basededatos = new BD(this);
            progressBar1.Value = 4;
           /* if (basededatos.crearTablas())//comentar inserts en crear tablas.  
            {
                basededatos.creaActualizaBD();
            }*/

            //if (basededatos.crearBD())
                if (basededatos.crearTablas())
                {
                    progressBar1.Value = 5;
                    Server Servidor = new Server(this);//, 9050);                                   
                    Cliente Cliente1 = new Cliente(this, "9050");
                    string hostName = Dns.GetHostName();
                    progressBar1.Value = 6;
                    IPHostEntry thisHost = Dns.GetHostEntry(hostName);
                    labelPseu.Text = basededatos.recuperamyPseu();
                    labelIP.Text = thisHost.AddressList[0].ToString();
                    labelPseu.Text = basededatos.recuperamyPseu();
                    Thread hiloCliente = new Thread(new ThreadStart(Cliente1.beacon));
                    hiloCliente.Start();
                    progressBar1.Value = 7;
                    DetectaIncidente VigilaCarretera = new DetectaIncidente(this);
                    VigilaCarretera.DetectaIncidenteStart();
                    VigilaCarretera.posibleparking();
                    progressBar1.Value = 8;
                    /*Prueba VigilaCarretera = new Prueba(this);
                    VigilaCarretera.DetectaIncidenteStart();*/
                }
                progressBar1.Value = 0;             
        }

        private void autenticados_Click(object sender, EventArgs e)
        {
            BD DButiles = new BD(this);
            DButiles.mostrarDatosVigentes();
        }

        private void sygic_Click_1(object sender, EventArgs e)
        {
            DriveHandler.SendDriveToForeground(1);
            System.Threading.Thread.Sleep(10000);
            DriveHandler.SendDriveToBackground(1);
        }

        private void Pause_Click_1(object sender, EventArgs e)
        {
            pausandoServicios();
            Play.Enabled = true;
            Play.Visible = true;
        }

        private void Atasco_Click(object sender, EventArgs e)
        {

            BD DButiles = new BD(this);
            int CoorX = -1632700;// 0899; //-1538956;//
            int CoorY = 2848779;//2309;//2786158;// 
            int CoorZ = 0;
            SError err;
            SGpsPosition gps;
            try
            {
                if (CApplicationAPI.GetActualGpsPosition(out err, out gps, false, 1000) == 1)
                {
                    CoorX = gps.Longitude;
                    CoorY = gps.Latitude;
                }
            }
            catch { }

            string viaOnombre = "Atasco";
            string sentido = "sentido";
            DateTime fechaDato = DateTime.Now;
            string datoPublicidad = viaOnombre + ";" + sentido + ";" + CoorX + ";" + CoorY + ";" + CoorZ + ";" + fechaDato;//Estos datos los coge de un fichero
           
           // string firma = Convert.ToBase64String(signedData).ToString();//
            Utiles utiles = new Utiles();
            Cliente clientePubli = new Cliente(this, "9050");
            Cripto criputiles = new Cripto(this);            
                //----------------------------------------------------------------------------------------------
                //ENVIAMOS ATASCO TB (PARA PRUEBA) 
                //CoorX = CoorX + 200;// -1538956;
               // CoorY = 2786158;
                viaOnombre = "calle1";
                sentido = "sentido1";
                fechaDato = DateTime.Now;//.GetDateTimeFormats("MM/dd/yyyy HH:mm:ss");

            agregacion datoagregado=  new agregacion(this, viaOnombre, sentido, CoorX, CoorY, CoorZ, fechaDato);
            string firma = datoagregado.FirmarMsje("Traffic Jam", Server.myPseudonimo);
                string datoAtasco = viaOnombre + "_" + sentido + "_" + CoorX + "_" + CoorY + "_" + CoorZ + "_" + fechaDato.ToString("MM/dd/yyyy HH:mm:ss");//Estos datos los coge de un fichero
               // firma = criputiles.RSAfirma(datoAtasco);            
                string mensaje = "A_Traffic Jam." + firma + '|'+Server.myPseudonimo + "_" + datoAtasco; 
                Invoke(myDelegate, new Object[] { "Sending Possible Traffic Jam" });
                DButiles.insertarEvento("T1", viaOnombre, sentido, CoorX, CoorY, CoorZ, fechaDato, firma);
                clientePubli.respuesta("T1", Server.myPseudonimo, criputiles.Encrypt(mensaje, DButiles.recuperamySK()), IPAddress.Broadcast.ToString());//,+ ";" + firma
                utiles.anadePoi("Atasco", "Atasco", CoorX, CoorY, 100);
        }

        private void Park_Click(object sender, EventArgs e)
        {
            BD DButiles = new BD(this);
            CoorX2 = CoorX2 + 30;// 0899; //-1538956;//
            int CoorX = CoorX2;
            int CoorY = 2848836;// 2309;//2786158;// 
            int CoorZ = 0;
            SError err;
            SGpsPosition gps;
            try
            {
                if (CApplicationAPI.GetActualGpsPosition(out err, out gps, false, 1000) == 1)
                {
                    CoorX = gps.Longitude;
                    CoorY = gps.Latitude;
                }
            }
            catch { }
            string viaOnombre = "Posible Aparcamiento";
            string sentido = "sentido";
            DateTime fechaDato = DateTime.Now;
            string datoPublicidad = viaOnombre + ";" + sentido + ";" + CoorX + ";" + CoorY + ";" + CoorZ + ";" + fechaDato;//Estos datos los coge de un fichero
            //GENERAMOS FIRMA (ESTO ES PARA PRUEBAS)En la versión final esta firma la recogerá de un fichero que le pasará la autoridad
            Utiles utiles = new Utiles();
            byte[] dataToSign = utiles.StrToByteArray(datoPublicidad);
            RSAParameters Cprivada = new RSAParameters()
            {
                D = Convert.FromBase64String("7QePOux7La+Y3jWwOwyHXqCDiduvXQv23TrfVX6cvTmr9BH0FtBzf4dbxYumjreztNrmX+wYsWH5W4pycB67S/tWODZrlrR8zuCCjWauC3ZjPWnlyU+Npg0qzLm7XkCLiuveQWpR4E/TWcs6Wr9pVL2zXT/BsWYC9t39qvkhTKE="),
                DP = Convert.FromBase64String("fJnDOGGMlWgVoQ+7MZtUfivpChykRC39W5UyTnnZ8+xxkt67nzlxXs2wl3w8EV1wRGYPXr0KfjFldUXYGd8h2Q=="),
                DQ = Convert.FromBase64String("3Ls7pKVPqzABAUxQ6jTKypsH7Zd+DJDhQfQset2sK15DmDU+cAY2BFufKvsojfYI2UKKtqvoIGKrWzZ+zrwKOQ=="),
                Exponent = Convert.FromBase64String("AQAB"),
                InverseQ = Convert.FromBase64String("rWWEy1hBu44BBcjuZwFNiixwEQobmHCG+ZqFRldjJKT/2RThzKNc9Q6vYwR52WSwGNxLmlwTvRb2p/gM13WaVQ=="),
                Modulus = Convert.FromBase64String("98Ej5BZ/VMG4nxCzdMZoZ8V50//GvnEQc3CX4vyHzDjOfUGB21ZjVF12s+h3ZQmQX/Woq1zZM6sNsTLVG2SiQhzwWIEE7ioyr2vn1OjE17QOlmrVtl8lI4txnZQQh8jaq1mEi1lqI7JMvwBr+AmTWz+Vf5RraWv/a7qonMDovyM="),
                P = Convert.FromBase64String("/SEbB4+LoAyHzUFTxyuA2mOJdZYIMiugNv9hUZQUjRzVbavfGVaxP/Pu5nxkyzmPtgcTn3m9nwbqwQSlLwEDdw=="),
                Q = Convert.FromBase64String("+pBuG5WisL4wg+B5auoXSQ5Q0/v4405ypMdQ5p6mG2notIamTZ4sp3m2+PpQOitlXrOQemlL3oaed2SH2XQUtQ==")
            };
            RSACryptoServiceProvider RSA2 = new RSACryptoServiceProvider();
            RSA2.ImportParameters(Cprivada);
            byte[] signedData = RSA2.SignData(dataToSign, "SHA1");
            string firma = Convert.ToBase64String(signedData).ToString();//

            Cliente clientePubli = new Cliente(this, "9050");
            Cripto criputiles = new Cripto(this);
            //----------------------------------------------------------------------------------------------
                //ENVIAMOS APARCAMIENTO TB (PARA PRUEBA) 
                //CoorX = CoorX + 200;//-1631287;
                //CoorY = 2850168;
                viaOnombre = "calle1";
                sentido = "sentido1";
                fechaDato = DateTime.Now;
                string datoAparcamiento = viaOnombre + ";" + sentido + ";" + CoorX + ";" + CoorY + ";" + CoorZ + ";" + fechaDato;//Estos datos los coge de un fichero
                firma= criputiles.RSAfirma(datoAparcamiento);

                Invoke(myDelegate, new Object[] { "Sending Possible Parking" });
                DButiles.insertarEvento("P2", viaOnombre, sentido, CoorX, CoorY, CoorZ, fechaDato, firma);
                clientePubli.respuesta("P2", Server.myPseudonimo, criputiles.Encrypt(datoAparcamiento + ";"+firma, DButiles.recuperamySK()), IPAddress.Broadcast.ToString());//,
                utiles.anadePoi("Posible Plaza", "Posible Plaza", CoorX, CoorY, 100);
                //----------------------------------------------------------------------------------------------
        }

        private void Borrar_Click(object sender, EventArgs e)
        {
            try
            {
                SError err;
                int res = CApplicationAPI.DeletePoiCategory(out err, "El Corte Inglés", "ESP", 0);//(Borramos Para las pruebas)
                res = CApplicationAPI.DeletePoiCategory(out err, "28482309", "ESP", 0);//(Borramos Para las pruebas)
                res = CApplicationAPI.DeletePoiCategory(out err, "atasco", "ESP", 0);//(Borramos Para las pruebas)
                res = CApplicationAPI.DeletePoiCategory(out err, "comercio", "ESP", 0);//(Borramos Para las pruebas)
                res = CApplicationAPI.DeletePoiCategory(out err, "el corte ingles", "ESP", 0);//(Borramos Para las pruebas)
                res = CApplicationAPI.DeletePoiCategory(out err, "elcorteingles", "ESP", 0);//(Borramos Para las pruebas)
                res = CApplicationAPI.DeletePoiCategory(out err, "posible plaza", "ESP", 0);//(Borramos Para las pruebas)
                res = CApplicationAPI.DeletePoiCategory(out err, "restaurante", "ESP", 0);//(Borramos Para las pruebas)
            }
            catch { }
        }                                       
    }
}