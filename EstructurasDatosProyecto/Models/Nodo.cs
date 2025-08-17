namespace EstructurasDatosProyecto.Models;

public class Nodo<T>
{
    public T dato;
    public Nodo<T> siguiente;
    public Nodo<T> anterior;

    public Nodo(T dato)
    {
        this.dato = dato;
        siguiente = null;
        anterior = null;
    }
}