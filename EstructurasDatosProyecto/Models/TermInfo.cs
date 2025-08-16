namespace EstructurasDatosProyecto.Models;

public class TermInfo
{
    public string Term { get; set; }
    public List<Posting> Postings { get; set; } = new List<Posting>();

    // DF = document frequency
    public int Df => Postings.Count;

    // frecuencia total
    public int CollectionFreq => Postings.Sum(p => p.Tf);
}