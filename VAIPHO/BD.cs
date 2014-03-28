using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using System.Data.SqlServerCe; //Añadido para sqlce.
using System.IO;//para file en db
using System.Data;//para connectionstate en db

namespace VAIPHO
{
    class BD
    {
        Form1 Formulario;
        //CANDIDO solo1
        public int limiteBD = 32;

        public BD(Form1 form)
        {
            Formulario = form;
        }

        public bool crearBD()
        {
            try
            {
                if (File.Exists("VAIPHO_DB.sdf"))
                    File.Delete("VAIPHO_DB.sdf");
                //Se crea la nueva base de datos
                SqlCeEngine engine = new SqlCeEngine("Data Source = VAIPHO_DB.sdf");
                engine.CreateDatabase();
                return true;
            }
            catch (Exception e)
            {
                Formulario.Invoke(Formulario.myDelegate, new Object[] { "Error creando BD: " + e.Message });
                return false;
            }
        }


        /*
         public bool actualizaAlmacen()
         {
            const string fic = @"E:\tmp\Prueba.txt";
            string texto = "Érase una vez una vieja con un moño...";

            System.IO.StreamWriter sw = new System.IO.StreamWriter(fic);
            sw.WriteLine(texto);
            sw.Close();
            //LEE
            const string fic = @"E:\tmp\Prueba.txt";
            string texto;

            System.IO.StreamReader sr = new System.IO.StreamReader(fic);
            texto = sr.ReadToEnd();
            sr.Close();

            Console.WriteLine(texto); 
         
         }
         */

        public bool crearTablas()
        {
            try
            {
                if (File.Exists("VAIPHO_DB.sdf"))
                    File.Delete("VAIPHO_DB.sdf");

                if (!File.Exists("VAIPHO_DB.sdf"))
                {
                    //Se crea la nueva base de datos
                    SqlCeEngine engine = new SqlCeEngine("Data Source = VAIPHO_DB.sdf");
                    engine.CreateDatabase();

                    SqlCeConnection conn = new SqlCeConnection("Data Source = VAIPHO_DB.sdf");
                    conn.Open();
                    //1.Create an instance of the command class, by using the SqlCeCommand object: 
                    SqlCeCommand cmd = conn.CreateCommand();

                    /*Creamos una tabla de Eventos 
                     * TipoEvento
                     * 0.- Atasco
                     * 1.- Futura funcionalidad
                     * 2.- Futura funcionalidad */
                    //cmd.CommandText = "CREATE TABLE Eventos (id INT IDENTITY NOT NULL PRIMARY KEY, TipoEvento NTEXT NOT NULL, CoordX FLOAT, CoordY FLOAT,TimeStamp DATETIME, Expire DATETIME)";
                    cmd.CommandText = "CREATE TABLE Eventos (id INT IDENTITY NOT NULL PRIMARY KEY, TipoEvento NTEXT NOT NULL, NombreVia NTEXT,SentidoMarcha NTEXT ,CoordX NTEXT, CoordY NTEXT,CoordZ NTEXT,TimeStamp DATETIME, Expire DATETIME, Signatures NTEXT)";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "CREATE TABLE PosibleEventos (id INT IDENTITY NOT NULL PRIMARY KEY, TipoEvento NTEXT NOT NULL, NombreVia NTEXT,SentidoMarcha NTEXT ,CoordX NTEXT, CoordY NTEXT,CoordZ NTEXT,TimeStamp DATETIME, Expire DATETIME, Signatures NTEXT)";
                    cmd.ExecuteNonQuery();

                    //CANDIDO
                    DateTime myDateTime = DateTime.Now.Date; //(2001, 5, 16, 3, 2, 15);//BUSCAR HORA ACTUAL
                    string fecha = myDateTime.ToShortDateString();
                    string[] fechaingles = fecha.Split('/');
                    fecha = fechaingles[1] + "/" + fechaingles[0] + "/" + fechaingles[2];


                    //Creamos una tabla con contenido fijo para pruebas
                    //cmd.CommandText = "CREATE TABLE almacenCertificados ( idcolumna INT IDENTITY NOT NULL PRIMARY KEY, idA NTEXT NOT NULL, idB NTEXT NOT NULL, certAB BIGINT, certBA BIGINT,fechaCreacion DATETIME, gradoB INT)";// INT NOT NULL,
                    cmd.CommandText = "CREATE TABLE almacenCertificados ( idcolumna INT IDENTITY NOT NULL PRIMARY KEY, idA NTEXT NOT NULL, idB NTEXT NOT NULL, certAB NTEXT, certBA NTEXT,fechaCreacion DATETIME, gradoB INT)";// INT NOT NULL,
                    
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "INSERT INTO almacenCertificados ( idA, idB, certAB, certBA, fechaCreacion, gradoB) VALUES ( 'ID1', 'ID24', '1522014933', '0062609429', '" + fecha + "', 6)";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "INSERT INTO almacenCertificados ( idA, idB, certAB, certBA, fechaCreacion, gradoB) VALUES ( 'ID24', 'ID3', '0062608776', '3286713017', '" + fecha + "', 5)";//'2010-01-01 12:00', 5)";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "INSERT INTO almacenCertificados ( idA, idB, certAB, certBA, fechaCreacion, gradoB) VALUES ( 'ID3', 'ID4', '3286709748', '2289633252', '" + fecha + "', 9)";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "INSERT INTO almacenCertificados ( idA, idB, certAB, certBA, fechaCreacion, gradoB) VALUES ( 'ID4', 'ID5', '2289609017', '3617521482', '" + fecha + "', 8)";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "INSERT INTO almacenCertificados ( idA, idB, certAB, certBA, fechaCreacion, gradoB) VALUES ( 'ID5', 'ID6', '3617509024', '1440513636', '" + fecha + "', 5)";
                    cmd.ExecuteNonQuery();

                    //cmd.CommandText = "INSERT INTO almacenCertificados ( idcolumna, idA, idB, certAB, certBA) VALUES ( 6, 'ID7', 'ID7', '1440512957', '0')";
                    //cmd.ExecuteNonQuery();

                    //Creamos una tabla con claves publicas y modulos de los dispositivos
                   // cmd.CommandText = "CREATE TABLE almacenClaves (idcolumna INT IDENTITY NOT NULL PRIMARY KEY , idA NTEXT NOT NULL, PseuA NTEXT, modulo BIGINT, clavepublica BIGINT, clavesecreta NTEXT, grado BIGINT, autenticado INT, fechaAutenticado DATETIME)";
                    cmd.CommandText = "CREATE TABLE almacenClaves (idcolumna INT IDENTITY NOT NULL PRIMARY KEY , idA NTEXT NOT NULL, PseuA NTEXT, modulo NTEXT, exponente NTEXT, clavepublica BIGINT, clavesecreta NTEXT, grado INT, autenticado INT, fechaAutenticado DATETIME)";
                    
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "INSERT INTO almacenClaves ( idA, PseuA, modulo, exponente, clavepublica, clavesecreta, grado, autenticado, fechaAutenticado) VALUES ( 'ID1', 'PSEU1', '32639','', 12869, '11111', 6, 0," + fecha + ")";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "INSERT INTO almacenClaves ( idA, PseuA, modulo, exponente, clavepublica, clavesecreta, grado, autenticado, fechaAutenticado) VALUES ( 'ID24', 'PSEU24', '36581','', 12931, '22222', 5, 0," + fecha + ")";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "INSERT INTO almacenClaves ( idA, PseuA, modulo, exponente, clavepublica, clavesecreta, grado, autenticado, fechaAutenticado) VALUES ( 'ID3', 'PSEU3', '33919','', 17713, '33333', 5, 0," + fecha + ")";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "INSERT INTO almacenClaves ( idA, PseuA, modulo, exponente, clavepublica, clavesecreta, grado, autenticado, fechaAutenticado) VALUES ( 'ID4', 'PSEU4', '37909','', 18979, '44444', 5, 0," + fecha + ")";
                    cmd.ExecuteNonQuery();
                    //cmd.CommandText = "INSERT INTO almacenClaves ( idA, PseuA, modulo, exponente, clavepublica, clavesecreta, grado, autenticado, fechaAutenticado) VALUES ( 'ID5', 'PSEU5', '42799','', 425, '55555', 5, 0," + fecha + ")";
                    //cmd.ExecuteNonQuery();
                    //cmd.CommandText = "INSERT INTO almacenClaves ( idA, PseuA, modulo, exponente, clavepublica, clavesecreta, grado, autenticado, fechaAutenticado) VALUES ( 'ID6', 'PSEU6', '57181','', 57181, '66666', 5, 0," + fecha + ")";
                    //cmd.ExecuteNonQuery();
                    //INSERTAR AUTORIDAD CON CLAVE DE MAYOR TAMAÑO
                    cmd.CommandText = "INSERT INTO almacenClaves ( idA, PseuA, modulo, exponente, clavepublica, clavesecreta, grado, autenticado, fechaAutenticado) VALUES ( 'Autoridad', 'Autoridad', '98Ej5BZ/VMG4nxCzdMZoZ8V50//GvnEQc3CX4vyHzDjOfUGB21ZjVF12s+h3ZQmQX/Woq1zZM6sNsTLVG2SiQhzwWIEE7ioyr2vn1OjE17QOlmrVtl8lI4txnZQQh8jaq1mEi1lqI7JMvwBr+AmTWz+Vf5RraWv/a7qonMDovyM=', 'AQAB', 32639, '66666', 5, 0," + fecha + ")";
                    cmd.ExecuteNonQuery();

                    //Creamos una tabla con claves publicas y modulos de los dispositivos
                   // cmd.CommandText = "CREATE TABLE myAlmacen (idcolumna INT IDENTITY PRIMARY KEY, idA NTEXT NOT NULL, PseuA NTEXT, modulo BIGINT, clavepublica BIGINT, claveprivada BIGINT, clavesecreta NTEXT, grado BIGINT, version INT)";
                    cmd.CommandText = "CREATE TABLE myAlmacen (idcolumna INT IDENTITY PRIMARY KEY, idA NTEXT NOT NULL, PseuA NTEXT, clavepublica BIGINT, modulo NTEXT, exponente NTEXT, claveprivadaD NTEXT, DP NTEXT, DQ NTEXT,InverseQ NTEXT, P NTEXT, Q NTEXT, clavesecreta NTEXT, grado INT, version INT)";
                   
                    cmd.ExecuteNonQuery();
                    //cmd.CommandText = "INSERT INTO myAlmacen ( idA, PseuA, modulo, clavepublica, claveprivada, clavesecreta, grado, version) VALUES ( 'ID1', 'PSEU1', 32639, 17713, 0, '11111', 6, 0 )";
                    //cmd.CommandText = "INSERT INTO myAlmacen ( idA, PseuA, modulo, clavepublica, claveprivada, clavesecreta, grado, version) VALUES ( 'ID2', 'PSEU2', 36581, 5905, 0, '22222', 5, 0 )";
                    //cmd.CommandText = "INSERT INTO myAlmacen ( idA, PseuA, modulo, clavepublica, claveprivada, clavesecreta, grado, version) VALUES ( 'ID3', 'PSEU3', 33919, 12869, 0, '33333', 7, 0 )";
                    //cmd.CommandText = "INSERT INTO myAlmacen ( idA, PseuA, modulo, clavepublica, claveprivada, clavesecreta, grado, version) VALUES ( 'ID4', 'PSEU4',  37909, 5793, 0, '44444', 6, 0 )";
                    Random randomNumber = new Random(DateTime.Now.Second);
                    string newPseu = "pseu" + randomNumber.Next(99999);
                    cmd.CommandText = "INSERT INTO myAlmacen ( idA, PseuA, clavepublica, modulo, exponente, claveprivadaD, DP, DQ, InverseQ, P, Q, clavesecreta, grado, version) VALUES ( 'ID25', '" + newPseu + "',36581,'98Ej5BZ/VMG4nxCzdMZoZ8V50//GvnEQc3CX4vyHzDjOfUGB21ZjVF12s+h3ZQmQX/Woq1zZM6sNsTLVG2SiQhzwWIEE7ioyr2vn1OjE17QOlmrVtl8lI4txnZQQh8jaq1mEi1lqI7JMvwBr+AmTWz+Vf5RraWv/a7qonMDovyM=','AQAB','7QePOux7La+Y3jWwOwyHXqCDiduvXQv23TrfVX6cvTmr9BH0FtBzf4dbxYumjreztNrmX+wYsWH5W4pycB67S/tWODZrlrR8zuCCjWauC3ZjPWnlyU+Npg0qzLm7XkCLiuveQWpR4E/TWcs6Wr9pVL2zXT/BsWYC9t39qvkhTKE=','fJnDOGGMlWgVoQ+7MZtUfivpChykRC39W5UyTnnZ8+xxkt67nzlxXs2wl3w8EV1wRGYPXr0KfjFldUXYGd8h2Q==','3Ls7pKVPqzABAUxQ6jTKypsH7Zd+DJDhQfQset2sK15DmDU+cAY2BFufKvsojfYI2UKKtqvoIGKrWzZ+zrwKOQ==','rWWEy1hBu44BBcjuZwFNiixwEQobmHCG+ZqFRldjJKT/2RThzKNc9Q6vYwR52WSwGNxLmlwTvRb2p/gM13WaVQ==','/SEbB4+LoAyHzUFTxyuA2mOJdZYIMiugNv9hUZQUjRzVbavfGVaxP/Pu5nxkyzmPtgcTn3m9nwbqwQSlLwEDdw==','+pBuG5WisL4wg+B5auoXSQ5Q0/v4405ypMdQ5p6mG2notIamTZ4sp3m2+PpQOitlXrOQemlL3oaed2SH2XQUtQ==','33333', 5, 0 )";
                    //cmd.CommandText = "INSERT INTO myAlmacen ( idA, PseuA, clavepublica, modulo, exponente, claveprivadaD, DP, DQ, InverseQ, P, Q, clavesecreta, grado, version) VALUES ( 'ID3', 'PSEU3',33919,'98Ej5BZ/VMG4nxCzdMZoZ8V50//GvnEQc3CX4vyHzDjOfUGB21ZjVF12s+h3ZQmQX/Woq1zZM6sNsTLVG2SiQhzwWIEE7ioyr2vn1OjE17QOlmrVtl8lI4txnZQQh8jaq1mEi1lqI7JMvwBr+AmTWz+Vf5RraWv/a7qonMDovyM=','AQAB','7QePOux7La+Y3jWwOwyHXqCDiduvXQv23TrfVX6cvTmr9BH0FtBzf4dbxYumjreztNrmX+wYsWH5W4pycB67S/tWODZrlrR8zuCCjWauC3ZjPWnlyU+Npg0qzLm7XkCLiuveQWpR4E/TWcs6Wr9pVL2zXT/BsWYC9t39qvkhTKE=','fJnDOGGMlWgVoQ+7MZtUfivpChykRC39W5UyTnnZ8+xxkt67nzlxXs2wl3w8EV1wRGYPXr0KfjFldUXYGd8h2Q==','3Ls7pKVPqzABAUxQ6jTKypsH7Zd+DJDhQfQset2sK15DmDU+cAY2BFufKvsojfYI2UKKtqvoIGKrWzZ+zrwKOQ==','rWWEy1hBu44BBcjuZwFNiixwEQobmHCG+ZqFRldjJKT/2RThzKNc9Q6vYwR52WSwGNxLmlwTvRb2p/gM13WaVQ==','/SEbB4+LoAyHzUFTxyuA2mOJdZYIMiugNv9hUZQUjRzVbavfGVaxP/Pu5nxkyzmPtgcTn3m9nwbqwQSlLwEDdw==','+pBuG5WisL4wg+B5auoXSQ5Q0/v4405ypMdQ5p6mG2notIamTZ4sp3m2+PpQOitlXrOQemlL3oaed2SH2XQUtQ==','22222', 5, 0 )";


                    cmd.ExecuteNonQuery();
                    if (conn.State == ConnectionState.Open)
                        conn.Close();                       
                }
                return true;
            }
            catch (Exception e)
            {
                Formulario.Invoke(Formulario.myDelegate, new Object[] { "Error creando las Tablas: " + e.Message });                               
                return false;
            }
        }


        public bool insertarEvento(string tipoEvento, string nombreVia, string sentido, double X, double Y, double Z, DateTime hora, string sign)
        {
            string fecha = hora.ToString("MM/dd/yyyy HH:mm:ss");

            //string[] fechaingles = fecha.Split('/');
            //   fecha = fechaingles[1]+"/"+fechaingles[0]+"/"+fechaingles[2];
            DateTime fechatimestamp = DateTime.Now.AddMinutes(60);
            string timestamp = fechatimestamp.ToString("MM/dd/yyyy HH:mm:ss");

            try
            {
                SqlCeConnection conn = new SqlCeConnection("Data Source = VAIPHO_DB.sdf");
                conn.Open();

                //1.Create an instance of the command class, by using the SqlCeCommand object: 
                SqlCeCommand cmd = conn.CreateCommand();
               // Formulario.Invoke(Formulario.myDelegate, new Object[] { "INSERT INTO Eventos (TipoEvento,NombreVia,SentidoMarcha,CoordX,CoordY,CoordZ,TimeStamp,Expire,Signatures) VALUES ('" + tipoEvento + "', '" + nombreVia + "', '" + sentido + "', '" + X.ToString() + "', '" + Y.ToString() + "', '" + Z.ToString() + "', '" + fecha + "', '" + timestamp + "', '" + sign + "')" });
                
                cmd.CommandText = "INSERT INTO Eventos (TipoEvento,NombreVia,SentidoMarcha,CoordX,CoordY,CoordZ,TimeStamp,Expire,Signatures) VALUES ('" + tipoEvento + "', '" + nombreVia + "', '" + sentido + "', '" + X.ToString() + "', '" + Y.ToString() + "', '" + Z.ToString() + "', '" + fecha + "', '" + timestamp + "', '" + sign + "')";
                cmd.ExecuteNonQuery();
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                return true;
            }
            catch (Exception e)
            {
                Formulario.Invoke(Formulario.myDelegate, new Object[] { "DB=>Error insertando evento: " + e.Message });
                return false;
            }
        }

        public bool EliminaEvento(string tipoEvento, DateTime horaCreacion)
        {
            try
            {
                SqlCeConnection conn = new SqlCeConnection("Data Source = VAIPHO_DB.sdf");
                conn.Open();

                //1.Create an instance of the command class, by using the SqlCeCommand object: 
                SqlCeCommand cmd = conn.CreateCommand();

                cmd.CommandText = "SELECT * FROM Eventos";
                SqlCeDataReader myReader = cmd.ExecuteReader();
                while (myReader.Read())
                {
                    if (tipoEvento.Equals(myReader.GetString(1)))
                    {
                        DateTime timestamp = myReader.GetDateTime(7);
                        //Se comprueba que el evento sea actual
                        if (horaCreacion.CompareTo(timestamp) < 0)
                        {
                            cmd.CommandText = "DELETE FROM Eventos WHERE id = " + myReader.GetInt32(0).ToString();
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                if (conn.State == ConnectionState.Open)
                    conn.Close();

                return true;
            }
            catch (Exception e)
            {
                Formulario.Invoke(Formulario.myDelegate, new Object[] { "DB=>Error en existe evento: " + e.Message });
                return false;
            }
        }

        public string ExisteEvento(string tipoEvento, string nombreVia, string sentido, double X, double Y)
        {
            //string fecha=hora.ToShortTimeString();
            string resultadoConsulta = "";
            //  string fecha = hora.ToString("MM/dd/yyyy HH:mm:ss");
            try
            {
                SqlCeConnection conn = new SqlCeConnection("Data Source = VAIPHO_DB.sdf");
                conn.Open();

                //1.Create an instance of the command class, by using the SqlCeCommand object: 
                SqlCeCommand cmd = conn.CreateCommand();

                // cmd.CommandText = "SELECT * FROM Eventos WHERE NombreVia LIKE '" + nombreVia + "' AND SentidoMarcha LIKE  '" + sentido+"'";
                cmd.CommandText = "SELECT * FROM Eventos WHERE NombreVia LIKE '" + nombreVia + "' AND TipoEvento LIKE '" + tipoEvento +"'";
                SqlCeDataReader myReader = cmd.ExecuteReader();
                while (myReader.Read())
                {
                    DateTime timestamp = myReader.GetDateTime(8);
                    //Se comprueba que el evento sea actual
                    if (DateTime.Now.CompareTo(timestamp) < 0)
                    {   //Sse devuelve 
                        resultadoConsulta = myReader.GetString(4) + "|" + myReader.GetString(5) + "|" + myReader.GetDateTime(7).ToString("MM/dd/yyyy HH:mm:ss");
                    }
                }

                if (conn.State == ConnectionState.Open)
                    conn.Close();
                return resultadoConsulta;
            }
            catch (Exception e)
            {
                Formulario.Invoke(Formulario.myDelegate, new Object[] { "DB=>Error en existe evento: " + e.Message });
                return "error";
            }
        }

        /*   public bool UpdateEvento(string tipoEvento, string nombreVia, string sentido, double X, double Y, string sign)
           { 
           try
               {
                   SqlCeConnection conn = new SqlCeConnection("Data Source = VAIPHO_DB.sdf");
                   conn.Open();

                   //1.Create an instance of the command class, by using the SqlCeCommand object: 
                   SqlCeCommand cmd = conn.CreateCommand();

                  // cmd.CommandText = "SELECT * FROM Eventos WHERE NombreVia LIKE '" + nombreVia + "' AND SentidoMarcha LIKE  '" + sentido+"'";
                   //"UPDATE myAlmacen SET Signatures='" + sign + "'  WHERE PseuA LIKE '" + pseuUser + "'";
                   cmd.CommandText = "UPDATE Eventos SET Signatures='" + sign + "' WHERE NombreVia LIKE '" + nombreVia + "'";
                   SqlCeDataReader myReader = cmd.ExecuteReader();
                
                   if (conn.State == ConnectionState.Open)
                       conn.Close();
                   return resultadoConsulta;

               }
               catch (Exception e)
               {
                   Formulario.Invoke(Formulario.myDelegate, new Object[] { "DB=>Error en existe evento: " + e.Message });
                   return "error";
               }
        
           }*/

        public bool insertarPosibleEvento(string tipoEvento, string nombreVia, string sentido, double X, double Y, double Z, DateTime hora, string sign)
        {
            string fecha = hora.ToString("MM/dd/yyyy HH:mm:ss");
            //string[] fechaingles = fecha.Split('/');
            //   fecha = fechaingles[1]+"/"+fechaingles[0]+"/"+fechaingles[2];
            DateTime fechatimestamp = DateTime.Now.AddMinutes(60);
            string timestamp = fechatimestamp.ToString("MM/dd/yyyy HH:mm:ss");

            try
            {
                SqlCeConnection conn = new SqlCeConnection("Data Source = VAIPHO_DB.sdf");
                conn.Open();

                //1.Create an instance of the command class, by using the SqlCeCommand object: 
                SqlCeCommand cmd = conn.CreateCommand();

                cmd.CommandText = "INSERT INTO PosibleEventos (TipoEvento,NombreVia,SentidoMarcha,CoordX,CoordY,CoordZ,TimeStamp,Expire,Signatures) VALUES ('" + tipoEvento + "', '" + nombreVia + "', '" + sentido + "', '" + X.ToString() + "', '" + Y.ToString() + "', '" + Z.ToString() + "', '" + fecha + "', '" + timestamp + "', '" + sign + "')";
                cmd.ExecuteNonQuery();
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                return true;
            }
            catch (Exception e)
            {
                Formulario.Invoke(Formulario.myDelegate, new Object[] { "DB=>Error insertando evento: " + e.Message });
                return false;
            }
        }

        public bool EliminaPosibleEvento(string tipoEvento, DateTime horaCreacion)
        {
            try
            {
                SqlCeConnection conn = new SqlCeConnection("Data Source = VAIPHO_DB.sdf");
                conn.Open();

                //1.Create an instance of the command class, by using the SqlCeCommand object: 
                SqlCeCommand cmd = conn.CreateCommand();

                cmd.CommandText = "SELECT * FROM PosibleEventos";
                SqlCeDataReader myReader = cmd.ExecuteReader();
                while (myReader.Read())
                {
                    if (tipoEvento.Equals(myReader.GetString(1)))
                    {
                        DateTime timestamp = myReader.GetDateTime(7);
                        //Se comprueba que el evento sea actual
                        if (horaCreacion.CompareTo(timestamp) < 0)
                        {
                            cmd.CommandText = "DELETE FROM PosibleEventos WHERE id = " + myReader.GetInt32(0).ToString();
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                if (conn.State == ConnectionState.Open)
                    conn.Close();

                return true;
            }
            catch (Exception e)
            {
                Formulario.Invoke(Formulario.myDelegate, new Object[] { "DB=>Error en existe evento: " + e.Message });
                return false;
            }
        }

        public string ExistePosibleEvento(string tipoEvento, string nombreVia, string sentido, double X, double Y)
        {
            //string fecha=hora.ToShortTimeString();
            string resultadoConsulta = "";
            //  string fecha = hora.ToString("MM/dd/yyyy HH:mm:ss");
            try
            {
                SqlCeConnection conn = new SqlCeConnection("Data Source = VAIPHO_DB.sdf");
                conn.Open();

                //1.Create an instance of the command class, by using the SqlCeCommand object: 
                SqlCeCommand cmd = conn.CreateCommand();

                // cmd.CommandText = "SELECT * FROM Eventos WHERE NombreVia LIKE '" + nombreVia + "' AND SentidoMarcha LIKE  '" + sentido+"'";
                cmd.CommandText = "SELECT * FROM PosibleEventos WHERE NombreVia LIKE '" + nombreVia + "' AND TipoEvento LIKE '" + tipoEvento + "'";
                SqlCeDataReader myReader = cmd.ExecuteReader();
                while (myReader.Read())
                {
                    DateTime timestamp = myReader.GetDateTime(8);
                    //Se comprueba que el evento sea actual
                    if (DateTime.Now.CompareTo(timestamp) < 0)
                    {   //Sse devuelve 
                        resultadoConsulta = myReader.GetString(4) + "|" + myReader.GetString(5) + "|" + myReader.GetDateTime(7).ToString("MM/dd/yyyy HH:mm:ss");
                    }
                }

                if (conn.State == ConnectionState.Open)
                    conn.Close();
                return resultadoConsulta;
            }
            catch (Exception e)
            {
                Formulario.Invoke(Formulario.myDelegate, new Object[] { "DB=>Error en existe evento: " + e.Message });
                return "error";
            }
        }



        public string[] recuperaEventos()
        {
            try
            {
                SqlCeConnection conn = new SqlCeConnection("Data Source = VAIPHO_DB.sdf");
                conn.Open();
                //1.Create an instance of the command class, by using the SqlCeCommand object: 
                SqlCeCommand cmd = conn.CreateCommand();

                int nev = 0;

                cmd.CommandText = "SELECT Count(*) FROM Eventos";//calculo tamaño de mi almacén
                int tamanoEv = Convert.ToInt32(cmd.ExecuteScalar());
                string[] eventos = new string[tamanoEv];
                cmd.CommandText = "SELECT * FROM Eventos";// WHERE TimeStamp = " + fecha.ToLongTimeString();
                SqlCeDataReader myReader = cmd.ExecuteReader();

                while (myReader.Read())
                {//TipoEvento NTEXT NOT NULL, NombreVia NTEXT,SentidoMarcha NTEXT ,CoordX NTEXT, CoordY NTEXT,CoordZ NTEXT,TimeStamp DATETIME, Expire DATETIME, Signatures NTEXT)";
                  //  DateTime timestamp = myReader.GetDateTime(5);
                    //  if (DateTime.Now.CompareTo(timestamp) < 0)//Se comprueba que el evento sea actual
                    //  {
                    eventos[nev] = myReader.GetString(1) + ";" + myReader.GetString(2) + ";" + myReader.GetString(3) + ";" + myReader.GetString(4) + ";" + myReader.GetString(5) + ";" + myReader.GetString(6) + ";" + myReader.GetDateTime(7) + ";" + myReader.GetDateTime(8) + ";" + myReader.GetString(9);
                    nev++;
                    //  }
                }

                if (conn.State == ConnectionState.Open)
                    conn.Close();

                return eventos;
            }
            catch (Exception e)
            {
                Formulario.Invoke(Formulario.myDelegate, new Object[] { "DB=>Error en recupera evento: " + e.Message });
                return null;
            }
        }

        //CANDIDO
        public bool autenticado(string pseudonimo)
        {
            bool devolver;
            SqlCeConnection conn = new SqlCeConnection("Data Source = VAIPHO_DB.sdf");
            conn.Open();

            //1.Create an instance of the command class, by using the SqlCeCommand object: 
            SqlCeCommand cmd = conn.CreateCommand();

            cmd.CommandText = "SELECT autenticado FROM almacenClaves WHERE PseuA LIKE '" + pseudonimo + "'";
            SqlCeDataReader myReader = cmd.ExecuteReader();
            if (myReader.Read())
                if (myReader.GetInt32(0) == 1)
                    devolver = true;
                else
                    devolver = false;
            else
                devolver = false;

            if (conn.State == ConnectionState.Open)
                conn.Close();
            return devolver;
        }

        public string recuperaIDs()
        {
            SqlCeConnection conn = new SqlCeConnection("Data Source = VAIPHO_DB.sdf");
            conn.Open();

            //1.Create an instance of the command class, by using the SqlCeCommand object: 
            SqlCeCommand cmd = conn.CreateCommand();

            cmd.CommandText = "SELECT idA FROM almacenCertificados";
            SqlCeDataReader myReader = cmd.ExecuteReader();
            string listaIdentificadores = null;
            while (myReader.Read())
            {
                // txtLog.Text = txtLog.Text + myReader.GetString(0);
                if (listaIdentificadores == null)
                    listaIdentificadores = myReader.GetString(0);
                else
                    listaIdentificadores = listaIdentificadores + ';' + myReader.GetString(0);
                //txtLog.Text = txtLog.Text + "\r\n";
            }
            if (conn.State == ConnectionState.Open)
                conn.Close();
            return listaIdentificadores;
        }

        public string recuperaPublicaModulo(string identificador)
        {
            SqlCeConnection conn = new SqlCeConnection("Data Source = VAIPHO_DB.sdf");
            string datos;
            conn.Open();
            SqlCeCommand cmd = conn.CreateCommand();//1.Create an instance of the command class, by using the SqlCeCommand object: 

            cmd.CommandText = "SELECT modulo, exponente FROM almacenClaves WHERE pseuA LIKE '" + identificador + "'";
            SqlCeDataReader myReader = cmd.ExecuteReader();
            if (myReader.Read())
            {
                datos = myReader.GetString(0)+","+myReader.GetString(1);
            }
            else
                datos = "";

            if (conn.State == ConnectionState.Open)
                conn.Close();
            return datos;
        }

        public long recuperaClavePublica(string identificador)
        {
            SqlCeConnection conn = new SqlCeConnection("Data Source = VAIPHO_DB.sdf");
            Int64 grafo;
            conn.Open();
            SqlCeCommand cmd = conn.CreateCommand();//1.Create an instance of the command class, by using the SqlCeCommand object: 

            cmd.CommandText = "SELECT clavepublica FROM almacenClaves WHERE idA LIKE '" + identificador + "'";
            SqlCeDataReader myReader = cmd.ExecuteReader();
            if (myReader.Read())
            {
                grafo = myReader.GetInt64(0);
            }
            else
                grafo = 0;

            if (conn.State == ConnectionState.Open)
                conn.Close();
            return grafo;
        }
        public long recuperaClavePublicaPseu(string identificador)
        {
            SqlCeConnection conn = new SqlCeConnection("Data Source = VAIPHO_DB.sdf");
            Int64 grafo;
            conn.Open();
            SqlCeCommand cmd = conn.CreateCommand();//1.Create an instance of the command class, by using the SqlCeCommand object: 

            cmd.CommandText = "SELECT clavepublica FROM almacenClaves WHERE pseuA LIKE '" + identificador + "'";
            SqlCeDataReader myReader = cmd.ExecuteReader();
            if (myReader.Read())
            {
                grafo = myReader.GetInt64(0);
            }
            else
                grafo = 0;

            if (conn.State == ConnectionState.Open)
                conn.Close();
            return grafo;
        }

        public string recuperamySK()
        {
            SqlCeConnection conn = new SqlCeConnection("Data Source = VAIPHO_DB.sdf");
            conn.Open();
            SqlCeCommand cmd = conn.CreateCommand();
            string myclavesecreta = "";
            cmd.CommandText = "SELECT clavesecreta FROM myAlmacen";// WHERE idA LIKE '" + identificador + "'";
            SqlCeDataReader myReader = cmd.ExecuteReader();
            if (myReader.Read())
            {
                myclavesecreta = myReader.GetString(0);
            }

            if (conn.State == ConnectionState.Open)
                conn.Close();
            return myclavesecreta;
        }

        public string recuperaSK(string seudonimo)
        {
            SqlCeConnection conn = new SqlCeConnection("Data Source = VAIPHO_DB.sdf");
            conn.Open();
            SqlCeCommand cmd = conn.CreateCommand();
            string clavesecreta = "";
            cmd.CommandText = "SELECT clavesecreta FROM almacenClaves WHERE PseuA LIKE '" + seudonimo + "'";
            SqlCeDataReader myReader = cmd.ExecuteReader();
            if (myReader.Read())
            {
                clavesecreta = myReader.GetString(0);
            }
            if (conn.State == ConnectionState.Open)
                conn.Close();
            return clavesecreta;
        }

        public string recuperaMisDatosPublicos()
        {
            string misDatos;
            SqlCeConnection conn = new SqlCeConnection("Data Source = VAIPHO_DB.sdf");
            conn.Open();
            SqlCeCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT idA, PseuA, clavepublica, modulo ,exponente, clavesecreta FROM myAlmacen";// WHERE idA LIKE '" + Identificador + "'";
            SqlCeDataReader myReader = cmd.ExecuteReader();
            if (myReader.Read())
            {
                misDatos = myReader.GetString(0) + "," + myReader.GetString(1) + "," + myReader.GetInt64(2) + "," + myReader.GetString(3) + "," + myReader.GetString(4) + "," + myReader.GetString(5);
            }
            else
                misDatos = "-";
            if (conn.State == ConnectionState.Open) //cierro bd
                conn.Close();
            return misDatos;
        }

        public string recuperaMisDatosPrivados()
        {
            string misDatos;
            SqlCeConnection conn = new SqlCeConnection("Data Source = VAIPHO_DB.sdf");
            conn.Open();
            SqlCeCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT modulo, exponente, claveprivadaD, DP, DQ, InverseQ, P, Q FROM myAlmacen";// WHERE idA LIKE '" + Identificador + "'";
            SqlCeDataReader myReader = cmd.ExecuteReader();
            if (myReader.Read())
            {
                misDatos = myReader.GetString(0) + "," + myReader.GetString(1) + "," + myReader.GetString(2) + "," + myReader.GetString(3) + "," + myReader.GetString(4) + "," + myReader.GetString(5) + "," + myReader.GetString(6) + "," + myReader.GetString(7);
            }
            else
                misDatos = "-";
            if (conn.State == ConnectionState.Open) //cierro bd
                conn.Close();
            return misDatos;
        }

        public void marcarAutenticado(string user)
        {
            SqlCeConnection conn = new SqlCeConnection("Data Source = VAIPHO_DB.sdf");
            conn.Open();
            SqlCeCommand cmd = conn.CreateCommand();

            cmd.CommandText = "UPDATE almacenClaves SET   autenticado=1  WHERE PseuA LIKE '" + user + "'";
            cmd.ExecuteNonQuery();

            if (conn.State == ConnectionState.Open) //cierro bd
                conn.Close();
        }

        public void actualizaUser(string pseuUser, string moment)
        {
            SqlCeConnection conn = new SqlCeConnection("Data Source = VAIPHO_DB.sdf");
            conn.Open();
            SqlCeCommand cmd = conn.CreateCommand();
            //string[] fechaingles = moment.Split('/');
            //string fecha = fechaingles[1] + "/" + fechaingles[0] + "/" + fechaingles[2];
            cmd.CommandText = "UPDATE almacenClaves SET fechaAutenticado='" + moment + "' WHERE PseuA LIKE '" + pseuUser + "'";
            cmd.ExecuteNonQuery();

            if (conn.State == ConnectionState.Open) //cierro bd
                conn.Close();
        }

        public void cambiaPseuUser(string pseuUser, string moment, string newPseu)
        {
            SqlCeConnection conn = new SqlCeConnection("Data Source = VAIPHO_DB.sdf");
            conn.Open();
            SqlCeCommand cmd = conn.CreateCommand();
            //string[] fechaingles = moment.Split('/');
            //string fecha = fechaingles[1] + "/" + fechaingles[0] + "/" + fechaingles[2];
            cmd.CommandText = "UPDATE almacenClaves SET fechaAutenticado='" + moment + "' WHERE PseuA LIKE '" + pseuUser + "'";
            cmd.ExecuteNonQuery();
            cmd.CommandText = "UPDATE almacenClaves SET PseuA='" + newPseu + "'  WHERE PseuA LIKE '" + pseuUser + "'";
            cmd.ExecuteNonQuery();

            if (conn.State == ConnectionState.Open) //cierro bd
                conn.Close();
        }

        public void cambiamyPseu(string pseuUser, string newPseu)
        {
            SqlCeConnection conn = new SqlCeConnection("Data Source = VAIPHO_DB.sdf");
            conn.Open();
            SqlCeCommand cmd = conn.CreateCommand();

            cmd.CommandText = "UPDATE myAlmacen SET PseuA='" + newPseu + "'  WHERE PseuA LIKE '" + pseuUser + "'";
            cmd.ExecuteNonQuery();

            if (conn.State == ConnectionState.Open) //cierro bd
                conn.Close();
        }

        public void cambiamyUser(string pseuUser, string newUser)
        {
            SqlCeConnection conn = new SqlCeConnection("Data Source = VAIPHO_DB.sdf");
            conn.Open();
            SqlCeCommand cmd = conn.CreateCommand();

            cmd.CommandText = "UPDATE myAlmacen SET idA='" + newUser + "'  WHERE PseuA LIKE '" + pseuUser + "'";
            cmd.ExecuteNonQuery();

            if (conn.State == ConnectionState.Open) //cierro bd
                conn.Close();
        }

        public void marcarDesAutenticado(string user)
        {
            SqlCeConnection conn = new SqlCeConnection("Data Source = VAIPHO_DB.sdf");
            conn.Open();
            SqlCeCommand cmd = conn.CreateCommand();

            cmd.CommandText = "UPDATE almacenClaves SET autenticado=0";//  WHERE PseuA LIKE '" + user + "'";
            cmd.ExecuteNonQuery();

            if (conn.State == ConnectionState.Open) //cierro bd
                conn.Close();
        }

        public string recuperamyPseu()
        {
            string devolver;
            SqlCeConnection conn = new SqlCeConnection("Data Source = VAIPHO_DB.sdf");
            conn.Open();
            SqlCeCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT PseuA FROM myAlmacen";
            SqlCeDataReader myReader = cmd.ExecuteReader();
            if (myReader.Read())
                devolver = myReader.GetString(0);
            else
                devolver = "";

            if (conn.State == ConnectionState.Open) //cierro bd
                conn.Close();

            return devolver;
        }

        public void addUser(string datosDescifrados)
        {
            string[] userDatos = datosDescifrados.Split(',');
            SqlCeConnection conn = new SqlCeConnection("Data Source = VAIPHO_DB.sdf");
            conn.Open();
            SqlCeCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT idA FROM almacenClaves WHERE idA LIKE '" + userDatos[0] + "'";
            SqlCeDataReader myReader = cmd.ExecuteReader();
            if (myReader.Read())
                cmd.CommandText = "UPDATE almacenClaves SET idA='" + userDatos[0] + "', PseuA='" + userDatos[1] + "',  clavepublica=" + userDatos[2] + ", modulo='" + userDatos[3] + "', exponente='" + userDatos[4] + "', clavesecreta='" + userDatos[5] + "', autenticado=0  WHERE idA LIKE '" + userDatos[0] + "'";
            else
                cmd.CommandText = "INSERT INTO almacenClaves (idA, PseuA, clavepublica, modulo, exponente, clavesecreta, autenticado) VALUES ('" + userDatos[0] + "','" + userDatos[1] + "'," + userDatos[2] + ",'" + userDatos[3] + "','" + userDatos[4] + "','" + userDatos[5] + "'," + 0 + ")";//WHERE idA LIKE '" + userDatos[0] + "'";
            cmd.ExecuteNonQuery();

            if (conn.State == ConnectionState.Open) //cierro bd
                conn.Close();

        }

        public string recuperaKeyStore()
        {
            string KeyStore;
            SqlCeConnection conn = null;
            conn = new SqlCeConnection("Data Source = VAIPHO_DB.sdf");
            conn.Open();
            SqlCeCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT idA, idB, certAB, certBA, fechaCreacion, gradoB FROM almacenCertificados";// WHERE idA LIKE '" + Identificador + "'";
            SqlCeDataReader myReader = cmd.ExecuteReader();
            KeyStore = "";
            while (myReader.Read())
            {
                KeyStore = KeyStore + myReader.GetString(0) + "," + myReader.GetString(1) + "," + myReader.GetString(2) + "," + myReader.GetString(3) + "," + myReader.GetDateTime(4).ToShortDateString() + "," + myReader.GetInt32(5) + ":";
            }

            if (conn.State == ConnectionState.Open) //cierro bd
                conn.Close();
            return KeyStore;
        }

        public void mostrarDatosVigentes()
        {
            SqlCeConnection conn = new SqlCeConnection("Data Source = VAIPHO_DB.sdf");
            conn.Open();
            SqlCeCommand cmd = conn.CreateCommand();
            //EVENTOS
            cmd.CommandText = "SELECT TipoEvento, NombreVia, TimeStamp FROM Eventos"; //WHERE autenticado=1"; +Identificador + "'";
            SqlCeDataReader myReader = cmd.ExecuteReader();
            int filas = 0;
            while (myReader.Read())
            {
                Formulario.Invoke(Formulario.myDelegate, new Object[] { "Event: " + myReader.GetString(0) + " in " + myReader.GetString(1) + ":" + myReader.GetDateTime(2) });
                filas++;
            }
            if (filas == 0)
                Formulario.Invoke(Formulario.myDelegate, new Object[] { "No Event in DB" });
            //NODOS AUTENTICADOS
            cmd.CommandText = "SELECT idA, PseuA,fechaAutenticado FROM almacenClaves WHERE autenticado=1";// +Identificador + "'";
            myReader = cmd.ExecuteReader();
            filas = 0;
            while (myReader.Read())
            {
                DateTime horaAut = DateTime.Now;
                try
                {
                    horaAut = myReader.GetDateTime(2);
                }
                catch { }
                    Formulario.Invoke(Formulario.myDelegate, new Object[] { "Node: " + myReader.GetString(0) + ":" + myReader.GetString(1) + ":" + horaAut });
                filas++;
            }
            if (filas == 0)
                Formulario.Invoke(Formulario.myDelegate, new Object[] { "No Node Authenticated" });
            //MIS DATOS
            cmd.CommandText = "SELECT idA, PseuA FROM myAlmacen"; //WHERE autenticado=1"; +Identificador + "'";
            myReader = cmd.ExecuteReader();
            if (myReader.Read())
            {
                Formulario.Invoke(Formulario.myDelegate, new Object[] { "I am: " + myReader.GetString(0) + ": " + myReader.GetString(1) });
            }

            if (conn.State == ConnectionState.Open) //cierro bd
                conn.Close();
        }

        public bool hayAutenticados()
        {
            SqlCeConnection conn = new SqlCeConnection("Data Source = VAIPHO_DB.sdf");
            conn.Open();
            SqlCeCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT idA, PseuA,fechaAutenticado FROM almacenClaves WHERE autenticado=1";// +Identificador + "'";
            SqlCeDataReader myReader = cmd.ExecuteReader();
            bool existen = false;
            if (myReader.Read())
            {
                existen = true;
            }
            if (conn.State == ConnectionState.Open) //cierro bd
                conn.Close();
            return existen;
        }

        public string actualizaDB(string AlmacenB)
        {
            string[] conexiones = AlmacenB.Split(':');
            string[] aux;
            int posiblegrado, gradomenor = 500;
            string identificadorA, posiblenodo, nodoaeliminar, newelements = "";
            aux = conexiones[0].Split(',');
            string[,] datos = new string[conexiones.Length, aux.Length];

            for (int i = 0; i < conexiones.Length; i++) //COLOCO LOS DATOS EN UNA MATRIZ
            {
                aux = conexiones[i].Split(',');
                for (int j = 0; j < aux.Length; j++)
                    datos[i, j] = aux[j];
            }

            //  = AlmacenB.
            SqlCeConnection conn = null;
            conn = new SqlCeConnection("Data Source = VAIPHO_DB.sdf");
            conn.Open();
            SqlCeCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Count(*) FROM almacenCertificados";//calculo tamaño de mi almacén
            int tamanoBD = Convert.ToInt32(cmd.ExecuteScalar());

            cmd.CommandText = "SELECT idB, gradoB FROM almacenCertificados ORDER BY gradoB";// WHERE idA LIKE '" + Identificador + "'";
            SqlCeDataReader myReader = cmd.ExecuteReader();

            /* KeyStore = "";*/
            nodoaeliminar = "0";
            while (myReader.Read())//bucle para escoger el de menor grado sin links (si tiene grado negativo no se quita)
            {
                if ((gradomenor > myReader.GetInt32(1)) && (myReader.GetInt32(1) >= 0))//si el grado es pequeño y no hay ningun nodo que linke a este
                {
                    posiblegrado = myReader.GetInt32(1);
                    posiblenodo = myReader.GetString(0);
                    cmd.CommandText = "SELECT idB FROM almacenCertificados WHERE idA LIKE '" + myReader.GetString(0) + "'";
                    myReader = cmd.ExecuteReader();
                    if (!myReader.Read())//si ningun nodo linka al nodo de este grado se convierte en el candidato a ser eliminado
                    {
                        nodoaeliminar = posiblenodo;
                        gradomenor = posiblegrado;
                    }
                }
            }
            cmd.CommandText = "SELECT idB, gradoB FROM almacenCertificados";//Seleccionar idS del almacen
            myReader = cmd.ExecuteReader();
            SqlCeDataReader myReader2;
            while (myReader.Read())//Compruebo si algún elemento linka con algún elemento de mi DB//ACTUALMENTE SOLO AÑADE 1 ELTO POR CONEXION(no seguro)
            {
                identificadorA = myReader.GetString(0);
                for (int i = 0; i < conexiones.Length; i++)
                {
                    if (identificadorA.Equals(datos[i, 0]))
                    {
                        cmd.CommandText = "SELECT idB FROM almacenCertificados WHERE idA LIKE '" + datos[i, 1] + "'";//COMPRUEBA que no tenga links 
                        myReader2 = cmd.ExecuteReader();
                        if (!myReader2.Read())//&& (datos[i,5].to>0))//NO ESTÁ y tiene grado positivo
                        {
                            cmd.CommandText = "SELECT idA FROM almacenCertificados WHERE idA LIKE '" + datos[i, 0] + "'";//COMPRUEBA que no tenga links 
                            myReader2 = cmd.ExecuteReader();
                            if (!myReader2.Read())//&& (datos[i,5].to>0))//NO ESTÁ y tiene grado positivo
                            {
                                if (tamanoBD >= limiteBD)//COMPRUEBO SI EL ALMACEN ESTÁ AL LIMITE O SI EL NODO TIENE MEJOR GRADO QUE OTROS.
                                {
                                    cmd.CommandText = "DELETE FROM almacenCertificados WHERE idB= '" + nodoaeliminar + "')";
                                    cmd.ExecuteNonQuery();
                                }
                                cmd.CommandText = "INSERT INTO almacenCertificados ( idA, idB, certAB, certBA, fechaCreacion, gradoB) VALUES ( '" + datos[i, 0] + "', '" + datos[i, 1] + "','" + datos[i, 2] + "','" + datos[i, 3] + "'," + datos[i, 4] + "," + datos[i, 5] + ")";
                                cmd.ExecuteNonQuery();
                                if (newelements.Equals(""))
                                    newelements = datos[i, 0] + "," + datos[i, 1];
                                else
                                    newelements = newelements + ":" + datos[i, 0] + "," + datos[i, 1];
                            }
                        }
                        //AQUI COMPRUEBO SI TIENEN GRADO NEGATIVO PARA RESTAR 1 A LOS QUE TENGAN DICHO GRADO                        
                        if (int.Parse(datos[i, 5]) < 0)
                        {
                            cmd.CommandText = "UPDATE almacenCertificados set gradoB=" + (myReader.GetInt32(1) - 1) + "WHERE idB=" + myReader.GetString(0) + ")";
                            cmd.ExecuteNonQuery();
                        }

                    }
                    //KeyStore = KeyStore + myReader.GetString(0) + "," + myReader.GetString(1) + "," + myReader.GetInt64(2) + "," + myReader.GetInt64(3) + "," + myReader.GetInt32(4) + ":";
                }
            }
            if (conn.State == ConnectionState.Open) //cierro bd
                conn.Close();
            return newelements;
        }


        public void creaActualizaBD()
        {
            //lectura
            const string file = "\\Tarjeta de almacenamiento\\FLEET_MOBILE_14d\\VAIPHOBD.txt";
            string texto;
            System.IO.StreamReader sr = new System.IO.StreamReader(file);
            string[] troceado;
            bool salir = false;
            SqlCeConnection conn = null;//= new SqlCeConnection("Data Source = VAIPHO_DB.sdf");
            SqlCeCommand cmd;
            SqlCeDataReader myReader;
            int version = 0;
            while ((sr.Peek() != -1) && !salir)
            {
                texto = sr.ReadLine();
                troceado = texto.Split(' ');
                switch (troceado[0])
                {
                    case "Version":
                        conn = new SqlCeConnection("Data Source = VAIPHO_DB.sdf");
                        conn.Open();
                        cmd = conn.CreateCommand();
                        cmd.CommandText = "SELECT version FROM myAlmacen";//calculo tamaño de mi almacén(si es 0 meto mis datos)
                        myReader = cmd.ExecuteReader();

/*<<<<<<< .mine
                       if (myReader.Read()){
                           if (int.Parse(troceado[1]) <= myReader.GetInt32(0))//si la versión no es mas nueva no actualizas bd
                               salir = true;
                           else
                           {//si la versión del fichero es mas nueva que la actual actualizas myAlmacen en bd.
                               version = int.Parse(troceado[1]);
                               texto = sr.ReadLine();//leo misDatos del fichero
                               troceado = texto.Split(' '); //exponente, claveprivadaD, DP, DQ, InverseQ, P, Q, clavesecreta, grado, version
                               cmd.CommandText = "UPDATE myAlmacen SET idA='" + troceado[1] + "', PseuA='" + troceado[2] + "',clavepublica=" + troceado[3] + "modulo='" + troceado[4] + "',exponente='" + troceado[5] + "',claveprivadaD='" + troceado[6] + "',DP='" + troceado[7] + "',DQ='" + troceado[8] + "',InverseQ='" + troceado[9] + "',P='" + troceado[10] + "',Q='" + troceado[11] + "',grado=" + troceado[12] + ",version=" + version + " WHERE idA LIKE 'ID1'";
                               cmd.ExecuteNonQuery();
                           }
                       }                                              
                       break;                 
=======*/
                        if (myReader.Read())
                        {
                            if (int.Parse(troceado[1]) <= myReader.GetInt32(0))//si la versión no es mas nueva no actualizas bd
                                salir = true;
                            else
                            {//si la versión del fichero es mas nueva que la actual actualizas myAlmacen en bd.
                                version = int.Parse(troceado[1]);
                                texto = sr.ReadLine();//leo misDatos del fichero
                                troceado = texto.Split(' ');
                                cmd.CommandText = "UPDATE myAlmacen SET idA='" + troceado[1] + "', PseuA='" + troceado[2] + "',modulo=" + troceado[3] + ",clavepublica=" + troceado[4] + ",claveprivada=" + troceado[5] + ",clavesecreta='" + troceado[6] + "',grado=" + troceado[7] + ",version=" + version + " WHERE idA LIKE 'ID1'";
                                cmd.ExecuteNonQuery();
                            }
                        }
                        break;
/*>>>>>>> .r73
<<<<<<< .mine
                   case "DatosAmigos":
                       texto = sr.ReadLine();
                       troceado = texto.Split(' ');
                       int estaEnBD;
                       while ((sr.Peek() != -1) && (!troceado[0].Equals("")))
                       {
                           cmd = conn.CreateCommand();
                           cmd.CommandText = "SELECT Count(*) FROM almacenClaves WHERE idA LIKE '" + troceado[0] + "'";//calculo tamaño de mi almacén(si es 0 meto mis datos)
                           estaEnBD = Convert.ToInt32(cmd.ExecuteScalar());
                           if(estaEnBD==0){
                               cmd.CommandText = "INSERT INTO almacenClaves ( idA, PseuA, modulo, exponente, clavepublica,  clavesecreta, grado, autenticado, fechaAutenticado) VALUES ( '" + troceado[0] + "', '" + troceado[1] + "','" + troceado[2] + "','" + troceado[3] + "', " + troceado[4] + ", " + ", '" + troceado[5] + "', " + troceado[6] + ", " + troceado[7] + ", '" + troceado[8] + " " + troceado[9] + "')";
                               cmd.ExecuteNonQuery();
                           }                          
                           texto = sr.ReadLine();
                           troceado = texto.Split(' ');
                       }
                       break;
                   case "Certificados":
                       texto = sr.ReadLine();
                       troceado = texto.Split(' ');
                       int estaEnBD2;
                       while ((sr.Peek() != -1) && (!troceado[0].Equals("")))
                       {
                           cmd = conn.CreateCommand();
                           cmd.CommandText = "SELECT Count(*) FROM almacenCertificados WHERE idB LIKE '" + troceado[1] + "'";//calculo tamaño de mi almacén(si es 0 meto mis datos)
                           estaEnBD2 = Convert.ToInt32(cmd.ExecuteScalar());
                           if (estaEnBD2 == 0)
                           {
                               cmd = conn.CreateCommand();
                               cmd.CommandText = "INSERT INTO almacenCertificados ( idA, idB, certAB, certBA, fechaCreacion, gradoB) VALUES ( '" + troceado[0] + "', '" + troceado[1] + "','" + troceado[2] + "','" + troceado[3] + "', '" + troceado[4] + " " + troceado[5] + "'," + troceado[6] + ")";
                               cmd.ExecuteNonQuery();
                           }
                           texto = sr.ReadLine();
                           troceado = texto.Split(' ');
                       }
                       break;
                   default:
                       break;
               }
=======*/
                    case "DatosAmigos":
                        texto = sr.ReadLine();
                        troceado = texto.Split(' ');
                        int estaEnBD;
                        while ((sr.Peek() != -1) && (!troceado[0].Equals("")))
                        {
                            cmd = conn.CreateCommand();
                            cmd.CommandText = "SELECT Count(*) FROM almacenClaves WHERE idA LIKE '" + troceado[0] + "'";//calculo tamaño de mi almacén(si es 0 meto mis datos)
                            estaEnBD = Convert.ToInt32(cmd.ExecuteScalar());
                            if (estaEnBD == 0)
                            {
                                cmd.CommandText = "INSERT INTO almacenClaves ( idA, PseuA, modulo, clavepublica, clavesecreta, grado, autenticado, fechaAutenticado) VALUES ( '" + troceado[0] + "', '" + troceado[1] + "'," + troceado[2] + "," + troceado[3] + ", '" + troceado[4] + "', " + troceado[5] + ", " + troceado[6] + ", '" + troceado[7] + " " + troceado[8] + "')";
                                cmd.ExecuteNonQuery();
                            }
                            texto = sr.ReadLine();
                            troceado = texto.Split(' ');
                        }
                        break;
                    case "Certificados":
                        texto = sr.ReadLine();
                        troceado = texto.Split(' ');
                        int estaEnBD2;
                        while ((sr.Peek() != -1) && (!troceado[0].Equals("")))
                        {
                            cmd = conn.CreateCommand();
                            cmd.CommandText = "SELECT Count(*) FROM almacenCertificados WHERE idB LIKE '" + troceado[1] + "'";//calculo tamaño de mi almacén(si es 0 meto mis datos)
                            estaEnBD2 = Convert.ToInt32(cmd.ExecuteScalar());
                            if (estaEnBD2 == 0)
                            {
                                cmd = conn.CreateCommand();
                                cmd.CommandText = "INSERT INTO almacenCertificados ( idA, idB, certAB, certBA, fechaCreacion, gradoB) VALUES ( '" + troceado[0] + "', '" + troceado[1] + "'," + troceado[2] + "," + troceado[3] + ", '" + troceado[4] + " " + troceado[5] + "'," + troceado[6] + ")";
                                cmd.ExecuteNonQuery();
                            }
                            texto = sr.ReadLine();
                            troceado = texto.Split(' ');
                        }
                        break;
                    default:
                        break;
                }
            }

            texto = sr.ReadToEnd();
            sr.Close();
            if (conn.State == ConnectionState.Open) //cierro bd
                conn.Close();
        }
        // CANDIDO   

    }
}
