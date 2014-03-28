using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace VAIPHO
{
    public class AggregarFirmas
    {
        static readonly object _locker = new object();

        private static int numerofirmas;
        private static string Firmas;
        private static bool esperoFirmas;

        public static void setNumerofirmas() { lock(_locker) numerofirmas = 0; }
        public static void setFirmas() { lock (_locker) Firmas = ""; }
        public static void setEsperoFirmas() { lock (_locker) esperoFirmas = true; }
        public static void resetEsperoFirmas() { lock (_locker) esperoFirmas = false; }
        public static void incrementarfirmas() { lock (_locker) numerofirmas++; }
        public static void agregafirmas(string firma) { lock (_locker) Firmas = firma; }
        public static string getfirmas() { lock (_locker) return Firmas; }
        public static bool getEsperoFirmas() { lock (_locker) return esperoFirmas; }
        public static int getnumerofirmas() { lock (_locker) return numerofirmas; }
    


    }
}
