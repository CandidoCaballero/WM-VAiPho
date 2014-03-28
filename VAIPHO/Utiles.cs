using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

//using System.Media;//SONIDO
using System.Runtime.InteropServices;//para sonido
using Microsoft.WindowsMobile.Status;//para estado bateria
using ApplicationAPI;//para addpoi

using System.IO;

namespace VAIPHO
{
    class Utiles
    {
        //CANDIDO

        //public BatteryLevel batteryId = Microsoft.WindowsMobile.Status.SystemState.PowerBatteryStrength;

       public void anadePoi(string nombre, string tipo,int CoorX, int CoorY, int time)
        {
           //IMAGEN EN SYGIC
            SError err;
            try
            {
                int res = CApplicationAPI.DeletePoiCategory(out err, nombre, "ESP", 1000);//(Borramos Para las pruebas)
                string dirImagen = @nombre + ".bmp";
                res = CApplicationAPI.AddPoiCategory(out err, nombre, dirImagen, "ESP", 1000);
                LONGPOSITION position = new LONGPOSITION(CoorX, CoorY);//X, Y);           
                SPoi poi = new SPoi(position,nombre, tipo, 0);
                res = CApplicationAPI.AddPoi(out err, ref poi, time);
            }
            catch { }   
           //SONIDO

            //Sound sound = new Sound(Assembly.GetExecutingAssembly().GetManifestResourceStream("SoundSample."+tipo+".wav"));
            //sound.Play();
            //OpenFileDialog dialog1 = new OpenFileDialog();
           //dialog1.ShowDialog()
           // Properties.Resources.

           string _appFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
           string dirSonido = _appFolder + @"\Sounds\" + tipo + ".wav";
           dirSonido =  "\\Tarjeta de almacenamiento\\FLEET_MOBILE_14d\\MisSonidos\\" + tipo + ".wav";//parking.wav";
            try
            {
                //PlaySound((LPCWSTR)dirSonido, 0, SND_LOOP | SND_ASYNC | SND_FILENAME);
                WSounds ws = new WSounds();
                
                ws.Play(dirSonido, ws.SND_FILENAME | ws.SND_ASYNC);

               /* DriveHandler.SendDriveToForeground(1);
                System.Threading.Thread.Sleep(5000);
                DriveHandler.SendDriveToBackground(1);*/
            }
            catch //()Exception ex)
            {
                //Invoke(myDelegate, new Object[] { "Error: " + ex.Message });
            }

        }

        

        public int[] PasarBinarioCH(long numero)
        {
            //int[] Reg = new int[(int)(Math.Log(numero) / Math.Log(2)) + 1];// ln(x) / ln(2)
            int[] Reg = new int[(Server.tam - 1) * Server.tam / 2];

            int i = 1;
            long resto, dividendo = numero;
            while (dividendo >= 1)
            {
                resto = dividendo % 2;
                Reg[Reg.Length - i] = (int)resto;
                dividendo = (int)dividendo / 2;
                i++;
            }
            return Reg;
        }

        public long PasarEntero(int[] numero)
        {
            long entero = 0;
            for (int i = 0; i < numero.Length; i++)
            {
                if (numero[i] == 1)
                {
                    entero = entero + (long)System.Math.Pow(2, i);
                }
            }
            return entero;
        }

        public int[] PasarBinario(long entero)
        {
            //La máscara y el # de iteraciones
            int tam = Server.tam;
            long mascara = 0x800000000;
            int iteraciones = tam * tam;
            //el contador y el resultado
            int contador = 0;
            StringBuilder resultado = new StringBuilder(iteraciones);

            //Se recorren los 32 bit
            while (contador++ < iteraciones)
            {
                //Si el entero and la mascara = 0 quiere decir
                //que el bit 1 esta apagado
                if ((entero & mascara) == 0)
                    resultado.Append('0');
                else
                    resultado.Append('1');

                //correr un bit a la izquierda para poner
                //el siguiente bit en la posicion del primero
                entero = entero << 1;
            }
            int[] reg = new int[tam * tam];
            for (int i = 0; i < tam * tam; i++)
                if (resultado[i].Equals('1'))
                    reg[i] = 1;
                else
                    reg[i] = 0;
            return reg;
        }

        //Función que devuelve un vector con el número dividendo en binario
        /*   public int[] PasarBinario(long numero)
            {
                int[] Reg = new int[(int)(Math.Log(numero)/Math.Log(2))+1];// ln(x) / ln(2)//tam*tam];//
                int i = 1;
                long resto, dividendo = numero;
                while (dividendo >= 1)
                {
                    resto = dividendo % 2;
                    Reg[Reg.Length - i] = (int)resto;
                    dividendo = (int)dividendo / 2;
                    i++;
                }
                return Reg;
            }*/
        //Función que devuelve un vector con el número dividendo en binario

        // C# to convert a string to a byte array.
        public byte[] StrToByteArray(string str)
        {
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            return encoding.GetBytes(str);
        }
        // C# to convert a byte array to a string.
        public string ByteArrayToStr(byte[] dBytes)
        {
            string str;
            System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
            str = enc.GetString(dBytes, 0, dBytes.Length);
            return str;
        }

        public string CogerNtelefono()
        {
            string numero = "";
            PhoneAddress phoneaddr = new PhoneAddress();
            try
            {
                phoneaddr = Sim.GetPhoneNumber();
                numero = phoneaddr.Address;
            }
            catch
            {
                numero = "000000000";
                // MessageBox.Show("Telephone number not available.");
            }
            return numero;
        }

        
        //CANDIDO
    }
    public class WSounds
    {
        [DllImport("coredll.dll")]
        public static extern bool PlaySound(string fname, int Mod, int flag);

        // these are the SoundFlags we are using here, check mmsystem.h for more
        public int SND_ASYNC = 0x0001;     // play asynchronously
        public int SND_FILENAME = 0x00020000; // use file name
        public int SND_PURGE = 0x0040;     // purge non-static events

        public void Play(string fname, int SoundFlags)
        {
            PlaySound(fname, 0, SoundFlags);
        }

        public void StopPlay()
        {
            PlaySound(null, 0, SND_PURGE);
        }
    }
}
