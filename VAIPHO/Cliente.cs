﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Security.Cryptography;
using System.IO;

namespace VAIPHO
{
    class Cliente
    {
        public Form1 Formulario;
        public string IP, puerto, msj;

        public Cliente(Form1 formu, string port)
        {
            this.Formulario = formu;
            this.puerto = port;
        }
        private void IniciarCliente()
        {
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(IP), int.Parse(puerto));
            UTF8Encoding encoding = new UTF8Encoding();//Pasamos la cadena de entrada a bytes
            byte[] plainTextBytes = encoding.GetBytes(msj);// + "/n");
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
            sock.SendTo(plainTextBytes, serverEndPoint);
            sock.Close();
        }

        public void respuesta(string codigo, string Pseu, string mensaje, string IPaddr)//, 
        {
            string hostName = Dns.GetHostName();
            IPHostEntry thisHost = Dns.GetHostEntry(hostName);
            string thisIpAddr = thisHost.AddressList[0].ToString();
            IP = IPaddr;//txtIpCliente.Text;
           // puerto = "9050";//txtPuertoCliente.Text;            
            /*POR AQUI DEBO GENERAR EL PAQUETE A ENVIAR*/
            //msj = codigo + "," + thisIpAddr + "," + Pseu + "," + mensaje;
            msj = codigo + "," + Pseu + "," + mensaje;
            Thread hiloCliente = new Thread(new ThreadStart(IniciarCliente));
            hiloCliente.Start();
        }

        public void beacon()
        {
            // txtPuertoCliente.Text; //COJO LA IP DEL OTRO (PUESTA A MANO)
            string hostName = Dns.GetHostName();
            IPHostEntry thisHost = Dns.GetHostEntry(hostName);
            string thisIpAddr = thisHost.AddressList[0].ToString();//RECUPERO MI IP
            Random randomNumber = new Random(DateTime.Now.Second);
            BD DButiles = new BD(Formulario);
            /*POR AQUI DEBO GENERAR EL BEACON A ENVIAR*/
            Thread hiloCliente;// = new Thread(new ThreadStart(IniciarCliente));
            // hiloCliente.Start();

            int cambioPseu = 50 + randomNumber.Next(10);//calculamos aleatoriamente cuantos beacons enviaremos antes de hacer el cambio de pseudonimo
            int vecesPseu = 0;
            string newPseu;
            while (!Formulario.cerrar)
           
            {// EL BEACON SE ENVIA PERIODICAMENTE(en un rango de tiempo)
                IP = IPAddress.Broadcast.ToString();//"192.168.1.255";//txtIpCliente.Text; //BROADCAST
                puerto = "9050";
                //msj = "01," + thisIpAddr + "," + "PSEU1,"+DateTime.Now.ToString()+", Ek1(ID1:KUid1:TimeStamp)";//";// +generaHash();//txtSmsCliente.Text;            

                msj = "01," + Server.myPseudonimo + "," + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") +", Ek1(ID1:KUid1:TimeStamp)";//";// +generaHash();//txtSmsCliente.Text;            

                hiloCliente = new Thread(new ThreadStart(IniciarCliente));
                hiloCliente.Start();
                Thread.Sleep(1000 * (8 + randomNumber.Next(10)));//beacon enviado en tiempo aleatorio entre 5 y 15
                vecesPseu++;
                if (vecesPseu == cambioPseu)//cuando se alcance esta cantidad se cambia el pseudonimo
                {
                    newPseu = "pseu" + randomNumber.Next(99999);
                    Server.viejoPseu = Server.myPseudonimo;
                    msj = "01," + Server.myPseudonimo + "," + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") +",00," + newPseu + ", Ek1(00:TimeStamp:newPseu)";
                    hiloCliente = new Thread(new ThreadStart(IniciarCliente));
                    hiloCliente.Start();
                    DButiles.cambiamyPseu(Server.myPseudonimo, newPseu);
                    Server.myPseudonimo = DButiles.recuperamyPseu();
                    Formulario.Invoke(Formulario.myDelegate5, new Object[] { newPseu });
                    vecesPseu = 0;
                    cambioPseu = 50 + randomNumber.Next(100);
                    Thread.Sleep(1000 * (5 + randomNumber.Next(10)));
                }
            }

        }

    }
}
