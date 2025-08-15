namespace EstructurasDatosProyecto.Models;

public class ListaDobleCircular<T> //Hacemos una lista con los metodos básicos y generica
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
    
    public T Buscar(Func<T, bool> condicion)
    {
        if (cabeza == null) return default;

        Nodo<T> actual = cabeza;
        do
        {
            if (condicion(actual.dato))
                return actual.dato;

            actual = actual.siguiente;
        } while (actual != cabeza);

        return default;
    }
    
    public bool Modificar(Func<T, bool> condicion, T nuevoDato)
    {
        if (cabeza == null) return false;

        Nodo<T> actual = cabeza;
        do
        {
            if (condicion(actual.dato))
            {
                actual.dato = nuevoDato;
                return true;
            }
            actual = actual.siguiente;
        } while (actual != cabeza);

        return false;
    }
    
    public bool Eliminar(Func<T, bool> condicion)
    {
        if (cabeza == null) return false;

        Nodo<T> actual = cabeza;
        do
        {
            if (condicion(actual.dato))
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
    
    public void Actualizar(Action<T> accion)
    {
        if (cabeza == null) return;

        Nodo<T> actual = cabeza;
        do
        {
            accion(actual.dato);
            actual = actual.siguiente;
        } while (actual != cabeza);
    }
    
    public void Recorrer(Action<T> accion)
    {
        if (cabeza == null) return;

        Nodo<T> actual = cabeza;
        do
        {
            accion(actual.dato);
            actual = actual.siguiente;
        } while (actual != cabeza);
    }
}