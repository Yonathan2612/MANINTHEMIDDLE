using System;
using System.Numerics;

class IntercambioDiffieHellman
{
    // Función para generar una clave privada aleatoria
    static BigInteger GenerarClavePrivada(int longitudBits = 128)
    {
        Random random = new Random();
        byte[] bytes = new byte[longitudBits / 8];
        random.NextBytes(bytes);
        BigInteger clavePrivada = new BigInteger(bytes);

        // Asegurar que la clave privada sea positiva
        clavePrivada = BigInteger.Abs(clavePrivada);

        return clavePrivada % BigInteger.Pow(2, longitudBits);
    }

    // Función para calcular la clave pública
    static BigInteger CalcularClavePublica(BigInteger alfa, BigInteger clavePrivada, BigInteger q)
    {
        return BigInteger.ModPow(alfa, clavePrivada, q);
    }

    // Función para calcular la clave compartida
    static BigInteger CalcularClaveCompartida(BigInteger clavePublica, BigInteger clavePrivada, BigInteger q)
    {
        return BigInteger.ModPow(clavePublica, clavePrivada, q);
    }

    static void Main(string[] args)
    {
        // Parámetros iniciales
        BigInteger q = 65537; // Base prima
        BigInteger[] alfas = { 3, 5, 6, 7, 10, 11, 12, 14, 20, 22 }; // primeras 10 raices primitivas

        // Generar claves privadas para Ana y Bob
        BigInteger clavePrivadaAna = GenerarClavePrivada();
        BigInteger clavePrivadaBob = GenerarClavePrivada();
        BigInteger clavePrivadaAnonymous = GenerarClavePrivada();

        int Acumulador = 0;
        foreach (BigInteger alfa in alfas)
        {
            Acumulador += 1;
            Console.WriteLine("-------PRUEBA NRO:  " + Acumulador);
            // Calcular claves públicas para Ana, Bob y Anonymous
            BigInteger clavePublicaAna = CalcularClavePublica(alfa, clavePrivadaAna, q);
            BigInteger clavePublicaBob = CalcularClavePublica(alfa, clavePrivadaBob, q);
            BigInteger clavePublicaAnonymous = CalcularClavePublica(alfa, clavePrivadaAnonymous, q);

            // Anonymous intercepta las claves públicas de Ana y Bob
            // Simula que recibe la clave pública de Ana y se la entrega falsa a Bob
            BigInteger clavePublicaFalsaAnaParaBob = clavePublicaAnonymous;

            // Simula que recibe la clave pública de Bob y se la entrega falsa a Ana
            BigInteger clavePublicaFalsaBobParaAna = clavePublicaAnonymous;

            // Calcular claves compartidas entre los participantes
            // Ana y Anonymous
            BigInteger claveCompartidaAnaAnonymous = CalcularClaveCompartida(clavePublicaFalsaBobParaAna, clavePrivadaAna, q);

            // Bob y Anonymous
            BigInteger claveCompartidaBobAnonymous = CalcularClaveCompartida(clavePublicaFalsaAnaParaBob, clavePrivadaBob, q);

            // Anonymous calcula sus propias claves compartidas con Ana y Bob
            BigInteger claveCompartidaAnonymousAna = CalcularClaveCompartida(clavePublicaAna, clavePrivadaAnonymous, q);
            BigInteger claveCompartidaAnonymousBob = CalcularClaveCompartida(clavePublicaBob, clavePrivadaAnonymous, q);

            // Mostrar resultados
            Console.WriteLine("Clave privada de Ana: " + clavePrivadaAna);
            Console.WriteLine("Clave pública de Ana: " + clavePublicaAna);
            Console.WriteLine("Clave privada de Bob: " + clavePrivadaBob);
            Console.WriteLine("Clave pública de Bob: " + clavePublicaBob);
            Console.WriteLine("Clave privada de Anonymous: " + clavePrivadaAnonymous);
            Console.WriteLine("Clave pública de Anonymous: " + clavePublicaAnonymous);

            Console.WriteLine("\nClaves compartidas:");
            Console.WriteLine("Clave compartida entre Ana y Anonymous (calculada por Ana): " + claveCompartidaAnaAnonymous);
            Console.WriteLine("Clave compartida entre Bob y Anonymous (calculada por Bob): " + claveCompartidaBobAnonymous);
            Console.WriteLine("Clave compartida entre Anonymous y Ana (calculada por Anonymous): " + claveCompartidaAnonymousAna);
            Console.WriteLine("Clave compartida entre Anonymous y Bob (calculada por Anonymous): " + claveCompartidaAnonymousBob);

            // Verificar si las claves entre Anonymous y los demás coinciden
            Console.WriteLine("\n¿Claves compartidas entre Ana y Anonymous coinciden? " + (claveCompartidaAnaAnonymous == claveCompartidaAnonymousAna));
            Console.WriteLine("¿Claves compartidas entre Bob y Anonymous coinciden? " + (claveCompartidaBobAnonymous == claveCompartidaAnonymousBob));

        }

    }
}
