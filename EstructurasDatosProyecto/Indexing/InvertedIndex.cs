namespace EstructurasDatosProyecto.Indexing;
using EstructurasDatosProyecto.Models;
public class InvertedIndex
{
    private Dictionary<string, TermInfo> index = new Dictionary<string, TermInfo>();

    public void AddTerm(string term, int docId)
    {
        if (!index.ContainsKey(term))
            index[term] = new TermInfo { Term = term };

        var postings = index[term].Postings;

        // Ver si ya existe un posting para este documento
        var posting = postings.FirstOrDefault(p => p.DocId == docId);
        if (posting == null)
        {
            postings.Add(new Posting { DocId = docId, Tf = 1 });
        }
        else
        {
            posting.Tf++;
        }
    }

    public Dictionary<string, TermInfo> GetIndex()
    {
        return index;
    }
}