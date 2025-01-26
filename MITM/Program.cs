using System;
using System.Numerics;

class IntercambioDiffieHellman
{
    // Función para generar una clave privada aleatoria

    // Función para convertir palabra de verificación a ASCII concatenado
    static BigInteger ConvertirPalabraAAscii(string palabra)
    {
        string asciiConcatenado = "";
        foreach (char c in palabra)
        {
            asciiConcatenado += ((int)c).ToString();
        }
        return BigInteger.Parse(asciiConcatenado);
    }
    static BigInteger GenerarClavePrivada(BigInteger q)
    {
        Random random = new Random();
        byte[] bytes = new byte[q.GetByteCount()];
        random.NextBytes(bytes);
        BigInteger clavePrivada = new BigInteger(bytes);

        // Asegurar que la clave privada sea positiva
        clavePrivada = BigInteger.Abs(clavePrivada);

        return BigInteger.Abs(clavePrivada % q);
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
        BigInteger q = 4294967311; // Base prima
        BigInteger[] alfas = { 3, 5, 7, 11, 13, 15, 17, 19, 23, 29 }; // primeras 10 raices primitivas

        string palabraVerificacion = "REAL";
        BigInteger M = ConvertirPalabraAAscii(palabraVerificacion);

        if (M >= q)
        {
            Console.WriteLine("La palabra de verificación debe ser menor que q.");
            return;
        }

        Console.WriteLine("Palabra de verificación (ASCII concatenado): " + M);

        // Generar claves privadas para Ana y Bob
        BigInteger clavePrivadaAna = GenerarClavePrivada(q);
        BigInteger clavePrivadaBob = GenerarClavePrivada(q);
        //BigInteger clavePrivadaAnonymous = GenerarClavePrivada();
        BigInteger clavePrivadaAnonymous = GenerarClavePrivada(q);


        int Acumulador = 0;
        foreach (BigInteger alfa in alfas)
        {
            Acumulador += 1;
            Console.WriteLine("-------PRUEBA NRO:  " + Acumulador);
            BigInteger clavePublicaAna = CalcularClavePublica(alfa, clavePrivadaAna, q);
            BigInteger clavePublicaBob = CalcularClavePublica(alfa, clavePrivadaBob, q);
            BigInteger clavePublicaAnonymous = CalcularClavePublica(alfa, clavePrivadaAnonymous, q);

            // Anonymous intercepta las claves públicas
            BigInteger clavePublicaFalsaAnaParaBob = clavePublicaAnonymous; // Anonymous reemplaza clave pública de Ana para Bob
            BigInteger clavePublicaFalsaBobParaAna = clavePublicaAnonymous; // Anonymous reemplaza clave pública de Bob para Ana

            // Calcular claves compartidas entre los participantes
            // Ana cree que está compartiendo con Bob pero está compartiendo con Anonymous
            BigInteger claveCompartidaAnaAnonymous = CalcularClaveCompartida(clavePublicaFalsaBobParaAna, clavePrivadaAna, q);

            // Bob cree que está compartiendo con Ana pero está compartiendo con Anonymous
            BigInteger claveCompartidaBobAnonymous = CalcularClaveCompartida(clavePublicaFalsaAnaParaBob, clavePrivadaBob, q);

            // Anonymous calcula sus propias claves compartidas con Ana y Bob
            BigInteger claveCompartidaAnonymousAna = CalcularClaveCompartida(clavePublicaAna, clavePrivadaAnonymous, q);
            BigInteger claveCompartidaAnonymousBob = CalcularClaveCompartida(clavePublicaBob, clavePrivadaAnonymous, q);

            // Mostrar resultados
            Console.WriteLine("\nClaves generadas:");
            Console.WriteLine("Clave privada de Ana: " + clavePrivadaAna);
            Console.WriteLine("Clave pública de Ana: " + clavePublicaAna);
            Console.WriteLine("Clave privada de Bob: " + clavePrivadaBob);
            Console.WriteLine("Clave pública de Bob: " + clavePublicaBob);
            Console.WriteLine("Clave privada de Anonymous: " + clavePrivadaAnonymous);
            Console.WriteLine("Clave pública de Anonymous: " + clavePublicaAnonymous);

            Console.WriteLine("\nClaves compartidas:");
            Console.WriteLine("Clave compartida calculada por Ana con Anonymous: " + claveCompartidaAnaAnonymous);
            Console.WriteLine("Clave compartida calculada por Bob con Anonymous: " + claveCompartidaBobAnonymous);
            Console.WriteLine("Clave compartida calculada por Anonymous con Ana: " + claveCompartidaAnonymousAna);
            Console.WriteLine("Clave compartida calculada por Anonymous con Bob: " + claveCompartidaAnonymousBob);

            // Validar integridad usando la palabra de verificación
            bool esClaveValidaAna = claveCompartidaAnaAnonymous % M == claveCompartidaAnonymousAna % M;
            bool esClaveValidaBob = claveCompartidaBobAnonymous % M == claveCompartidaAnonymousBob % M;

            Console.WriteLine("\n¿La clave compartida entre Ana y Anonymous es válida? " + (esClaveValidaAna ? "Sí" : "No"));
            Console.WriteLine("¿La clave compartida entre Bob y Anonymous es válida? " + (esClaveValidaBob ? "Sí" : "No"));

        }

    }
}
