using cdn_api;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DekretZSql

{
    public class FS
    {
        
        public string nrDokumentu{ get; set; }
        public int VATRok { get; set; }
        public int VATMiesiac { get; set; }
        public double VAT { get; set; } 
        public string DT_Konto { get; set; }
        public int DT_Rok { get; set; }
        public int DT_Miesiac { get; set; }
        public double DT_Kwota { get; set; }
        public double roznica { get; set; }
        public int TrN_GIDNumer { get; set; }
        public string TrN_NumerObcy { get; set; }
       

        public FS( string nrDokumentu, string VATRok, string VATMiesiac, string VAT, string DT_Konto, string DT_Rok, string DT_Miesiac, string DT_Kwota, string roznica, string TrN_GIDNumer, string TrN_NumerObcy )
        {
            try
            {
                this.nrDokumentu = nrDokumentu;
                this.VATRok = Convert.ToInt32(VATRok);
                this.VATMiesiac = Convert.ToInt32(VATMiesiac);
                this.VAT = Convert.ToDouble(VAT);
                this.DT_Konto = DT_Konto;
                this.DT_Rok = Convert.ToInt32(DT_Rok);
                this.DT_Miesiac = Convert.ToInt32(DT_Miesiac);
                this.DT_Kwota = Convert.ToDouble(DT_Kwota);
                this.roznica = Convert.ToDouble(roznica);
                this.TrN_GIDNumer = Convert.ToInt32(TrN_GIDNumer);
                this.TrN_NumerObcy = TrN_NumerObcy;
                
            }
            catch(Exception e)
            {
                MessageBox.Show("Błąd podczas dodawania obiektu FS: " + e.Message, "Komunikat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
    }





    }
}
