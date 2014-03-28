using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace VAIPHO
{
    class Cripto
    {
        Form1 Formulario;
      //  RSAParameters rsaPubParams;
      //  RSAParameters rsaPrivateParams;

        public Cripto(Form1 form)
        {
            Formulario = form;
        }
       
        //Encrypts using only the public key data.
        public byte[] EncryptData(RSAParameters rsaParams, byte[] toEncrypt)
        {
            RSACryptoServiceProvider rsaCSP = new RSACryptoServiceProvider();

            rsaCSP.ImportParameters(rsaParams);
            return rsaCSP.Encrypt(toEncrypt, false);
        }

        public string RSAfirma(string datos)
        {
            BD dbUtiles = new BD(Formulario);
            string[] clavePrivada = dbUtiles.recuperaMisDatosPrivados().Split(',');            

            Utiles utiles = new Utiles();
            byte[] dataToSign = utiles.StrToByteArray(datos);
            RSAParameters Cprivada = new RSAParameters()
            {//modulo, exponente, claveprivadaD, DP, DQ, InverseQ, P, Q
                Modulus = Convert.FromBase64String(clavePrivada[0]),
                Exponent = Convert.FromBase64String(clavePrivada[1]),
                D = Convert.FromBase64String(clavePrivada[2]),
                DP = Convert.FromBase64String(clavePrivada[3]),
                DQ = Convert.FromBase64String(clavePrivada[4]),                
                InverseQ = Convert.FromBase64String(clavePrivada[5]),                
                P = Convert.FromBase64String(clavePrivada[6]),
                Q = Convert.FromBase64String(clavePrivada[7])
            };
            RSACryptoServiceProvider RSA2 = new RSACryptoServiceProvider();
            RSA2.ImportParameters(Cprivada);
            byte[] signedData = RSA2.SignData(dataToSign, "SHA1");
            return Convert.ToBase64String(signedData).ToString();
        }

        public string RSAverifica(string pseuUser, string datosYfirma, bool publicidad)
        {
            BD dbUtiles = new BD(Formulario);
            string clave = dbUtiles.recuperaSK(pseuUser);
            string datosDescifrados = Decrypt(datosYfirma, clave);
            string[] cortado = datosDescifrados.Split(';');
            string datos = cortado[0] + ";" + cortado[1] + ";" + cortado[2] + ";" + cortado[3] + ";" + cortado[4] + ";" + cortado[5]; //+ ";" + cortado[6];+ ";" + cortado[7] fecha 7 no por que es cuando expira, esto no se firma
            byte[] decryptedData = utiles.StrToByteArray(datos);
            byte[] signedData = Convert.FromBase64String(cortado[cortado.Length-1]);//la firma esta en la posición 8   
            string usuario = "Autoridad";
            if (!publicidad)
                usuario = pseuUser;
            string clavePublica = dbUtiles.recuperaPublicaModulo(usuario);
            string[] Cpublicamodulo = clavePublica.Split(',');
            RSAParameters Cpublica = new RSAParameters()//RECOGER CPUBLICA DE DB
            {
                Exponent = Convert.FromBase64String(Cpublicamodulo[1]), //"AQAB"),
                Modulus =  Convert.FromBase64String(Cpublicamodulo[0]) //"98Ej5BZ/VMG4nxCzdMZoZ8V50//GvnEQc3CX4vyHzDjOfUGB21ZjVF12s+h3ZQmQX/Woq1zZM6sNsTLVG2SiQhzwWIEE7ioyr2vn1OjE17QOlmrVtl8lI4txnZQQh8jaq1mEi1lqI7JMvwBr+AmTWz+Vf5RraWv/a7qonMDovyM=")
            };     
                    //encryptedData = Criptutiles.RSAEncrypt(dataToEncrypt, Cpublica, false);  
                    
                    RSACryptoServiceProvider RSA3 = new RSACryptoServiceProvider();
                    RSA3.ImportParameters(Cpublica);

                    if (RSA3.VerifyData(decryptedData, "SHA1", signedData))// (dataToEncrypt, "SHA1", signedData);
                        return datosDescifrados;
                    //Formulario.Invoke(Formulario.myDelegate, new Object[] { "Advertisement Data not Correct" });
                    else
                        return "";           
        }

        //Manually performs hash and then verifies hashed value.
        public bool VerifyHash(RSAParameters rsaParams, byte[] signedData, byte[] signature)
        {
            RSACryptoServiceProvider rsaCSP = new RSACryptoServiceProvider();
            SHA1Managed hash = new SHA1Managed();
            byte[] hashedData;

            rsaCSP.ImportParameters(rsaParams);

            hashedData = hash.ComputeHash(signedData);
            return rsaCSP.VerifyHash(hashedData, CryptoConfig.MapNameToOID("SHA1"), signature);
        }
           
        //CANDIDO
        Utiles utiles = new Utiles();
        //  BD DButiles = new BD(Formulario);

        public string Encrypt2(string originalString)//FUNCION A CAMBIAR POR CIFRADO NUESTRO
        {
            byte[] bytes = ASCIIEncoding.ASCII.GetBytes("ZeroCool");
            if (String.IsNullOrEmpty(originalString))
            {
                throw new ArgumentNullException
                       ("The string which needs to be encrypted can not be null.");
            }
            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                cryptoProvider.CreateEncryptor(bytes, bytes), CryptoStreamMode.Write);
            StreamWriter writer = new StreamWriter(cryptoStream);
            writer.Write(originalString);
            writer.Flush();
            cryptoStream.FlushFinalBlock();
            writer.Flush();
            return Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
        }

        public string Decrypt2(string cryptedString)//FUNCION A CAMBIAR POR CIFRADO NUESTRO
        {
            byte[] bytes = ASCIIEncoding.ASCII.GetBytes("ZeroCool");
            if (String.IsNullOrEmpty(cryptedString))
            {
                throw new ArgumentNullException
                   ("The string which needs to be decrypted can not be null.");
            }
            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            MemoryStream memoryStream = new MemoryStream
                    (Convert.FromBase64String(cryptedString));
            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                cryptoProvider.CreateDecryptor(bytes, bytes), CryptoStreamMode.Read);
            StreamReader reader = new StreamReader(cryptoStream);
            return reader.ReadToEnd();
        }
        
        public string Encrypt(string Message, string Passphrase)
        {
            //string Passphrase = "holaabcdefghijklmn";
            byte[] Results;
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();

            // Step 1. We hash the passphrase using MD5
            // We use the MD5 hash generator as the result is a 128 bit byte array
            // which is a valid length for the TripleDES encoder we use below

            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Passphrase));

            // Step 2. Create a new TripleDESCryptoServiceProvider object
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();

            // Step 3. Setup the encoder
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;

            // Step 4. Convert the input string to a byte[]
            byte[] DataToEncrypt = UTF8.GetBytes(Message);

            // Step 5. Attempt to <b style="color:black;background-color:#ffff66">encrypt</b> the string
            try
            {
                ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
                Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
            }
            finally
            {
                // Clear the TripleDes and Hashprovider services of any sensitive information
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }

            // Step 6. Return the encrypted string as a base64 encoded string
            return Convert.ToBase64String(Results);
        }

        public string Decrypt(string Message, string Passphrase)
        {
           // string Passphrase = "holaabcdefghijklmn";
            byte[] Results;
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();

            // Step 1. We hash the passphrase using MD5
            // We use the MD5 hash generator as the result is a 128 bit byte array
            // which is a valid length for the TripleDES encoder we use below

            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Passphrase));

            // Step 2. Create a new TripleDESCryptoServiceProvider object
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();

            // Step 3. Setup the decoder
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;

            // Step 4. Convert the input string to a byte[]
            byte[] DataToDecrypt = Convert.FromBase64String(Message);

            // Step 5. Attempt to <b style="color:black;background-color:#a0ffff">decrypt</b> the string
            try
            {
                ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor();
                Results = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);
            }
            finally
            {
                // Clear the TripleDes and Hashprovider services of any sensitive information
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }

            // Step 6. Return the decrypted string in UTF8 format
            return UTF8.GetString( Results,0,Results.Length );
        }
          
        public string generaHash(string listaIdentificadores)
        {
            //generamos hash del la lista de id del almacen para enviar
            MD5 md5Hasher = MD5.Create();
            byte[] data;
            data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(listaIdentificadores));
            // Create a new Stringbuilder to collect the bytes and create a string.       
            StringBuilder sBuilder = new StringBuilder();
            // Loop through each byte of the hashed data and format each one as a hexadecimal string.         
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        public int[,] generaGrafoCompleto(long grafo)
        {

            //int[] bits = new int[(int)(Math.Log(grafo) / Math.Log(2)) + 1];
            int[] bits = new int[(Server.tam - 1) * Server.tam / 2];
            int contador = 0;
            int contador2 = 0;
            int[,] grafocompleto = new int[Server.tam, Server.tam];
            bits = utiles.PasarBinarioCH(grafo);
            for (int i = 0; i < Server.tam; i++)
            {
                for (int j = 0; j < Server.tam; j++)
                {
                    if (i == j)
                        grafocompleto[i, j] = 0;
                    else
                        if (i < j)
                        {
                            grafocompleto[i, j] = bits[contador];
                            grafocompleto[j, i] = bits[contador];
                            contador++;
                        }
                        else
                        {
                            // grafocompleto[i, j] = bits[contador2];
                            contador2++;
                        }
                }
            }
            return grafocompleto;

        }

        public bool esCH(string MycircuitoHamiltoniano, string MygrafoIsomorfo)
        {
            int[] myCHbinario;// //primero se pasan a binario
            int[] myGIbinario;// = new int[tam * tam];
            //comprobar que pertenece al grafo isomorfo
            bool distinto = false;
            if (MycircuitoHamiltoniano.Length == MygrafoIsomorfo.Length)
                for (int i = 0; i < MycircuitoHamiltoniano.Length; i++)
                    if (MycircuitoHamiltoniano[i] == 1)
                        if (MycircuitoHamiltoniano[i] != MygrafoIsomorfo[i])
                            distinto = true;
            //COMPROBAR QUE ES CH   //al estar desordenado compruebo que hay dos 1 por filas y por columnas           
            myCHbinario = utiles.PasarBinario(System.Convert.ToInt64(MycircuitoHamiltoniano));
            myGIbinario = utiles.PasarBinario(System.Convert.ToInt64(MygrafoIsomorfo));
            int contadorfilas = 0;
            int contadorcolumnas = 0;
            for (int i = 0; i < Server.tam; i++)
            {
                contadorfilas = 0;
                contadorcolumnas = 0;
                for (int j = 0; j < Server.tam; j++)
                {
                    if (myCHbinario[i * Server.tam + j] == 1)
                        contadorfilas++;
                    if (myCHbinario[j * Server.tam + i] == 1)
                        contadorcolumnas++;
                }
                if ((contadorfilas != 2) || (contadorcolumnas != 2))
                    distinto = true;
            }
            return !distinto;
        }

        public bool compruebaIsomorfismo(string listaIsomorfismo, string Mygrafo, string MygrafoIsomorfo)
        {
            int[,] grafocompleto = new int[Server.tam, Server.tam];
            int[] grafoauxiliar = new int[Server.tam * Server.tam];
            grafocompleto = generaGrafoCompleto(System.Convert.ToInt64(Mygrafo));
            string[] posic = listaIsomorfismo.Split(':');
            for (int i = 0; i < Server.tam; i++)
                for (int j = 0; j < Server.tam; j++)
                    grafoauxiliar[i * Server.tam + j] = grafocompleto[System.Convert.ToInt16(posic[i]) - 1, j];
            return (utiles.PasarEntero(grafoauxiliar) == System.Convert.ToInt64(MygrafoIsomorfo));
        }

        public long generaGrafoAleatorio(string Identificador, Int64 clavepublica)
        {
            Int64 grafo = clavepublica;
            Server.ClavePublicaComun = grafo;
            // convertir de numero a bit 
            int[] bits = new int[(int)(Math.Log(grafo) / Math.Log(2)) + 1];
            bits = utiles.PasarBinarioCH(grafo);

            Random r = new Random(DateTime.Now.Second);
            for (int i = 0; i < bits.Length; i++)
            {// Recorro grafo y voy metiendo 1 aleatorios
                if (bits[i] == 0)
                {
                    bits[i] = r.Next(1);    //Meto algunos 1 a la clave de forma aleatoria            
                }
            }
            grafo = utiles.PasarEntero(bits);
            return grafo;
        }

        public long generaGrafoIsomorfo(long grafo)
        {
            //SE DEBE CONOCER EL TAMAÑO DE LA MATRIX CON LA QUE SE GENERAN LAS CLAVES PUBLICASm
            int[,] grafocompleto = new int[Server.tam, Server.tam];
            int[] grafotranspuesto = new int[Server.tam * Server.tam];
            int[] listaordenada = new int[Server.tam];
            grafocompleto = generaGrafoCompleto(grafo);

            for (int i = 0; i < Server.tam; i++)//creo vector ordenado
                listaordenada[i] = i + 1;
            int posicion, restantes = Server.tam;
            Random r = new Random(DateTime.Now.Second);

            for (int i = 0; i < Server.tam; i++)
            {
                posicion = r.Next(restantes);//genera posición del vector ordenado
                restantes--;
                Server.listaaleatoria[i] = listaordenada[posicion];
                for (int j = posicion; j < restantes; j++)//+ 1; j++)
                    listaordenada[j] = listaordenada[j + 1];//quito la columna seleccionada
            }
            for (int i = 0; i < Server.tam; i++)
                for (int j = 0; j < Server.tam; j++)
                    grafotranspuesto[i * Server.tam + j] = grafocompleto[Server.listaaleatoria[i] - 1, j];
            return utiles.PasarEntero(grafotranspuesto);
        }

        public string generaCHdeIsomorfo()
        {//genera el CH del grafo isomorfo
            int[,] grafocompleto = new int[Server.tam, Server.tam];
            int[] grafotranspuesto = new int[Server.tam * Server.tam];
            grafocompleto = generaGrafoCompleto(Server.ClavePublicaComun);//genero el grafo completo del CH

            for (int i = 0; i < Server.tam; i++)
                for (int j = 0; j < Server.tam; j++)
                    grafotranspuesto[i * Server.tam + j] = grafocompleto[Server.listaaleatoria[i] - 1, j];//Lo cambio como el grafo isomorfo
            return utiles.PasarEntero(grafotranspuesto).ToString();
        }

        //CANDIDO
    }
}
