namespace EstructurasDatosProyecto.Models
{
    public class Iterator<T>
    {
        private Nodo<T> nodoActual;
        private Nodo<T> cabeza;
        private bool primerRecorrido;

        public Iterator(Nodo<T> cabeza)
        {
            this.cabeza = cabeza;
            this.nodoActual = cabeza;
            this.primerRecorrido = true;
        }

        public bool TieneSiguiente()
        {
            if (cabeza == null) return false;
            if (primerRecorrido) return true;
            return nodoActual != cabeza;
        }

        public T Siguiente()
        {
            if (!TieneSiguiente()) return default(T);

            T dato = nodoActual.dato;
            nodoActual = nodoActual.siguiente;
            primerRecorrido = false;
            return dato;
        }

        public void Reiniciar()
        {
            nodoActual = cabeza;
            primerRecorrido = true;
        }
    }
}
