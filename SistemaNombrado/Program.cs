using System;
using System.Collections.Generic;

namespace SistemaNombradoIntegral
{
    // Definir los tipos de recursos que el sistema puede nombrar
    public enum TipoRecurso
    {
        Archivo,
        Proceso,
        Servicio,
        Memoria,
        Otro
    }

    // Estructura base para almacenar la información de la entidad
    public class RegistroSIN
    {
        public TipoRecurso Tipo { get; set; }
        public string DireccionNativa { get; set; }

        public RegistroSIN(TipoRecurso tipo, string direccionNativa)
        {
            Tipo = tipo;
            DireccionNativa = direccionNativa;
        }
    }

    // Clase principal que actúa como interfaz del middleware (El SIN)
    public class ServicioNombrado
    {
        // Usar un diccionario (Tabla Hash) para manejar el espacio de nombres
        private readonly Dictionary<string, RegistroSIN> _directorio;

        public ServicioNombrado()
        {
            _directorio = new Dictionary<string, RegistroSIN>(StringComparer.OrdinalIgnoreCase);
        }

        // Método para que los nodos publiquen/registren entidades en el sistema
        public string Registrar(string contexto, TipoRecurso tipo, string nombre, string direccionNativa)
        {
            // Estandarización usando modelo URI jerárquico 
            string uri = $"sin://{contexto}/{tipo.ToString().ToLower()}/{nombre}".ToLower();
            
            _directorio[uri] = new RegistroSIN(tipo, direccionNativa);
            Console.WriteLine($"[SIN REGISTRO] {uri} -> {direccionNativa}");
            
            return uri;
        }

        // Método de navegación/resolución (Traducción del nombre a dirección nativa)
        public string? Resolver(string uri)
        {
            uri = uri.ToLower();
            if (_directorio.TryGetValue(uri, out RegistroSIN? registro))
            {
                Console.WriteLine($"[SIN RESOLUCIÓN] Éxito: '{uri}' apunta a -> {registro.DireccionNativa}");
                return registro.DireccionNativa;
            }

            Console.WriteLine($"[SIN ERROR] Recurso '{uri}' no disponible en el directorio.");
            return null;
        }
    }

    class Program
    {
        static void Main()
        {
            Console.WriteLine("--- Iniciando Sistema de Nombrado Integral (SIN) ---\n");
            ServicioNombrado sin = new ServicioNombrado();

            // Simular el registro de diferentes recursos heterogéneos de la red distribuida
            sin.Registrar("local", TipoRecurso.Archivo, "ventas2026", @"C:\datos\ventas2026.csv");
            sin.Registrar("nodo_backend", TipoRecurso.Proceso, "worker_calculo", "PID:4052@192.168.1.10");
            sin.Registrar("global", TipoRecurso.Servicio, "api_pagos", "http://pagos.empresa.com:8080");
            sin.Registrar("cluster", TipoRecurso.Memoria, "cache_sesion", "0x00A1F4B0");

            Console.WriteLine("\n--- Sistemas externos solicitando resolución ---");
            
            // Interfaz resolviendo nombres transparentes para los clientes
            string? dirServicio = sin.Resolver("sin://global/servicio/api_pagos");
            string? dirProceso = sin.Resolver("sin://nodo_backend/proceso/worker_calculo");
            
            // Intento de acceso a recurso inexistente
            string? dirInexistente = sin.Resolver("sin://local/archivo/nomina_secreta");
        }
    }
}