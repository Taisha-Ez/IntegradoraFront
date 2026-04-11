using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_Integradora
{
    class Pago
    {
        private int idPago;
        private int idVale;
        private decimal montoPago;
        private DateOnly fechaPago;

        public Pago(){ }

        // Constructor con parametros
        public Pago(int idVale, decimal montoPago, DateOnly fechaPago)
        {
            this.idVale = idVale;
            this.montoPago = montoPago;
            this.fechaPago = fechaPago;
        }

        // Getters y Setters
        public int getIdVale()
        {
            return idVale;
        }

        public void setIdVale(int idVale)
        {
            this.idVale = idVale;
        }

        public decimal getMontoPago()
        {
            return montoPago;
        }

        public void setMontoPago(decimal montoPago)
        {
            this.montoPago = montoPago;
        }

        public DateOnly getFechaPago()
        {
            return fechaPago;
        }

        public void setFechaPago(DateOnly fechaPago)
        {
            this.fechaPago = fechaPago;
        }
    }
}
