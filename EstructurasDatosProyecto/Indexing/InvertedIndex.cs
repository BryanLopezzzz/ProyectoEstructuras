namespace EstructurasDatosProyecto.Indexing;
using EstructurasDatosProyecto.Models;
public class ParTerminoInfo
{
    public string Termino { get; set; }
    public TermInfo Info { get; set; }
}

public class InvertedIndex
{
    private ListaDobleCircular<ParTerminoInfo> index = new ListaDobleCircular<ParTerminoInfo>();

    public void AddTerm(string term, int docId)
    {
        // Buscar si ya existe el termino
        ParTerminoInfo par = index.Buscar(p => p.Termino == term);

        if (par == null)
        {
            // Crear nuevo termino
            par = new ParTerminoInfo
            {
                Termino = term,
                Info = new TermInfo { Term = term }
            };
            index.Insertar(par);
        }

        var postings = par.Info.Postings;

        // Ver si ya existe un posting para este documento
        Posting posting = null;
        for (int i = 0; i < postings.Count; i++)
        {
            if (postings[i].DocId == docId)
            {
                posting = postings[i];
                break;
            }
        }

        if (posting == null)
        {
            postings.Add(new Posting { DocId = docId, Tf = 1 });
        }
        else
        {
            posting.Tf++;
        }
    }

    // Buscar info de un termino
    public TermInfo BuscarTermino(string termino)
    {
        ParTerminoInfo par = index.Buscar(p => p.Termino == termino);
        if (par != null)
        {
            return par.Info;
        }
        return null;
    }

    // Verificar si existe un termino
    public bool ExisteTermino(string termino)
    {
        ParTerminoInfo par = index.Buscar(p => p.Termino == termino);
        return par != null;
    }

    // Obtener todos los terminos
    public ListaDobleCircular<string> ObtenerTodosLosTerminos()
    {
        ListaDobleCircular<string> terminos = new ListaDobleCircular<string>();
        index.Recorrer(par =>
        {
            terminos.Insertar(par.Termino);
        });
        return terminos;
    }

    // Obtener toda la lista de pares (para recorrer)
    public ListaDobleCircular<ParTerminoInfo> ObtenerIndex()
    {
        return index;
    }
}