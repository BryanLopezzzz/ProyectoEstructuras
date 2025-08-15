namespace EstructurasDatosProyecto.Models;

public class Nodo<T>
{
    //Una lista generica, similar a usar templates en c++ (ustedes la conocen jeje) 
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