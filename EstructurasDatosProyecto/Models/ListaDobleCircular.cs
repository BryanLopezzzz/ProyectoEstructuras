namespace EstructurasDatosProyecto.Models;

public class ListaDobleCircular<T>
{
    private Nodo<T> cabeza;

    public bool EstaVacia()
    {
        return cabeza == null;
    }

    public void Insertar(T dato)
    {
        Nodo<T> nuevo = new Nodo<T>(dato);
        if (cabeza == null)
        {
            cabeza = nuevo;
            cabeza.siguiente = cabeza;
            cabeza.anterior = cabeza;
        }
        else
        {
            Nodo<T> ultimo = cabeza.anterior;
            ultimo.siguiente = nuevo;
            nuevo.anterior = ultimo;
            nuevo.siguiente = cabeza;
            cabeza.anterior = nuevo;
        }
    }

    public T Buscar(T dato)
    {
        if (cabeza == null) return default(T);
        Nodo<T> actual = cabeza;
        do
        {
            if (actual.dato.Equals(dato))
                return actual.dato;
            actual = actual.siguiente;
        } while (actual != cabeza);
        return default(T);
    }

    public bool Modificar(T datoViejo, T datoNuevo)
    {
        if (cabeza == null) return false;
        Nodo<T> actual = cabeza;
        do
        {
            if (actual.dato.Equals(datoViejo))
            {
                actual.dato = datoNuevo;
                return true;
            }
            actual = actual.siguiente;
        } while (actual != cabeza);
        return false;
    }

    public bool Eliminar(T dato)
    {
        if (cabeza == null) return false;
        Nodo<T> actual = cabeza;
        do
        {
            if (actual.dato.Equals(dato))
            {
                if (actual.siguiente == actual)
                {
                    cabeza = null;
                }
                else
                {
                    actual.anterior.siguiente = actual.siguiente;
                    actual.siguiente.anterior = actual.anterior;
                    if (actual == cabeza)
                        cabeza = actual.siguiente;
                }
                return true;
            }
            actual = actual.siguiente;
        } while (actual != cabeza);
        return false;
    }

    public int Count()
    {
        if (EstaVacia()) return 0;
        int contador = 0;
        Nodo<T> actual = cabeza;
        do
        {
            contador++;
            actual = actual.siguiente;
        } while (actual != cabeza);
        return contador;
    }

    public Iterator<T> ObtenerIterator()
    {
        return new Iterator<T>(cabeza);
    }


}