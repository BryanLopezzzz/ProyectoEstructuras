namespace EstructurasDatosProyecto.Models
{
    public sealed class StopWords
    {
        // Instancia única del singleton (thread-safe)
        private static readonly object _lock = new object();
        private static StopWords _instance;

        // HashSet para búsquedas O(1) de stopwords
        private readonly HashSet<string> _stopwords;

        // Constructor privado para prevenir instanciación externa
        private StopWords()
        {
            _stopwords = new HashSet<string>
            {
                "el","la","los","las","de","y","que","a","en","un","una","por","con",
                "no","es","al","lo","se","del","como","su","para","más","o","pero","algo","todos",
                "si","ya","este","esta","son","fue","ha","sus","le","mi","porque","cuando","muy",
                "sin","sobre","también","me","hasta","hay","donde","mientras","quien","cual","cuales","quienes",
                "nos","ni","tiene","tienen","uno","dos","tres","años","entre","ese","esa","esos",
                "esas","otra","otras","ese","eso","estos","estas","año","día","días","mes","meses"
            };
        }

        // Propiedad pública para acceder a la instancia única
        public static StopWords Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new StopWords();
                        }
                    }
                }
                return _instance;
            }
        }

        // Método estático para verificar si una palabra es stopword (mantiene compatibilidad con el código existente)
        public static bool IsStopword(string palabra)
        {
            if (string.IsNullOrEmpty(palabra)) return false;
            return Instance._stopwords.Contains(palabra.ToLower());
        }
    }
}
