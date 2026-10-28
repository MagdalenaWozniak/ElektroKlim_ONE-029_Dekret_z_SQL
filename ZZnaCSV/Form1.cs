using cdn_api;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DekretZSql
{
    public partial class Form1 : Form
    {
        private static int sesja = 0;
        private static int XLWersjaAPI = 20220;
        private static int XLTrybWsadowy = 1;
        private static int XLWinieta = -1;
        private static int XLUtworzWlasnaSesje = 1;
        private static string XLSQLServer = "172.51.1.8";
        private static string XLSQLDatabase = "ElektroKlim_Pro";
        private static string XLUser = "ADMIN"; //
        private static string XLPass = "NE!w0rk12";//"Dupa2003!";
        private static int dokID = 0;
        string connection_string = @"Data Source = EKSRV04\COMARCH; Initial Catalog = ERPXL_E; User ID = ComarchAPI; Password=hT@S+NQZBT";
        string date1 = "1800-12-28";
        public Form1(string connectionString, string xlSqlDatabase)
        {
            InitializeComponent();
            connection_string = connectionString;
            XLSQLDatabase = xlSqlDatabase;
            int rok = DateTime.Now.Year;
            string[] stringMiesiace = new[] { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12" };
            Miesiace.Items.AddRange(stringMiesiace);
            string[] stringLata = new[] { Convert.ToString(rok-2), Convert.ToString(rok - 1), Convert.ToString(rok), Convert.ToString(rok +1), Convert.ToString(rok +2), Convert.ToString(rok +3), Convert.ToString(rok+4), Convert.ToString(rok +5) };
            Lata.Items.AddRange(stringLata);


        }

        private void button_Click(object sender, EventArgs e)
        {

            string zamVATQuery = "if exists(SELECT 1 FROM cdn.PodNag WHERE PON_Status=2 and PON_TypPodatku=606 and PoN_Rok=" + Lata.SelectedItem + " and  PON_Miesiac=" + Miesiace.SelectedItem + ") "
                                + " SELECT 'TAK' else SELECT 'NIE'";
            if (Convert.ToString(Lata.SelectedItem) == "" || Convert.ToString(Miesiace.SelectedItem) == "")
            {
                MessageBox.Show("Proszę wybrać rok i miesiąc ", "Komunikat", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            else if(ConnectToSQL(zamVATQuery)=="TAK")
            {
                MessageBox.Show("Dla wybranego roku i miesiąca deklaracja VAT jest już zamknięta. Nie dodano dekretu. ", "Komunikat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                try
                {
                    string FSQuery = "select A1.[Numer dokumentu], A1.VATRok, A1.VATMiesiac, A1.VAT, DT_Konto, DT_Rok, DT_Miesiac, DT_Kwota, DT_Kwota - A1.VAT as roznica, TrN_GIDNumer , TrN_DokumentObcy"
                                    + " from cdn.Zrodla "
                                    + " join cdn.Dekrety on DT_GIDNumer = ZRO_DTNumer AND DT_GIDLp = ZRO_DTLp "
                                    + " join(select CDN.NumerDokumentu(TrN_GIDTyp, TrN_SpiTyp, TrN_TrnTyp, TrN_TrnNumer, TrN_TrnRok, TrN_TrnSeria, TrN_TrNMiesiac) AS[Numer dokumentu], "
                                    + " TrN_GIDNumer, TrN_DokumentObcy, TrV_DeklRok as VATRok, TrV_DeklMiesiac as VATMiesiac, trv_odliczeniavat, sum(trv_vatP) as VAT "
                                    + " from cdn.TraNag "
                                    + " join cdn.TraVat on TrN_GIDTyp = TrV_GIDTyp AND TrN_GIDNumer = TrV_GIDNumer and trv_odliczeniavat = 1 "
                                    + " where trn_metodakasowa = 0 "
                                    + " group by TrN_GIDNumer,TrN_DokumentObcy, TrV_DeklRok, TrV_DeklMiesiac, trv_odliczeniavat, CDN.NumerDokumentu(TrN_GIDTyp, TrN_SpiTyp, TrN_TrnTyp, TrN_TrnNumer, TrN_TrnRok, TrN_TrnSeria, TrN_TrNMiesiac) "
                                    + " union all "
                                    + " select CDN.NumerDokumentu(TrN_GIDTyp, TrN_SpiTyp, TrN_TrnTyp, TrN_TrnNumer, TrN_TrnRok, TrN_TrnSeria, TrN_TrNMiesiac) AS[Numer dokumentu], "
                                    + " TrN_GIDNumer, TrN_DokumentObcy, TsV_DeklRok as VATRok, TsV_DeklMiesiac as VATMiesiac, tsv_odliczeniavat, sum(TSV_VatP) as VAT "
                                    + " from cdn.TraNag "
                                    + " join cdn.TraVat on TrN_GIDTyp = TrV_GIDTyp AND TrN_GIDNumer = TrV_GIDNumer "
                                    + " join cdn.TraSVat on TrV_GIDTyp = TSV_GIDTyp AND TrV_GIDNumer = TSV_GIDNumer AND TrV_GIDLp = TSV_GIDLp     and TSV_Rozliczac = 1 "
                                    + " where trn_metodakasowa = 1 or trn_metodakasowa=2 "
                                    + " group by TrN_GIDNumer, TrN_DokumentObcy, TsV_DeklRok, TsV_DeklMiesiac, tsv_odliczeniavat, CDN.NumerDokumentu(TrN_GIDTyp, TrN_SpiTyp, TrN_TrnTyp, TrN_TrnNumer, TrN_TrnRok, TrN_TrnSeria, TrN_TrNMiesiac)) as A1 on A1.TrN_GIDNumer = ZRO_TRNNumer "
                                    + " where DT_Konto in('221-33-23', '221-33-08', '221-33-05', '222-33-23', '222-33-08', '222-33-05')  and VATMiesiac = " + Miesiace.SelectedItem.ToString() + " and VatRok = " + Lata.SelectedItem.ToString();

                    #region logowanie
                    XLLoginInfo_20202 nowyUzytkownik = new XLLoginInfo_20202();
                    nowyUzytkownik.Wersja = XLWersjaAPI;
                    nowyUzytkownik.Winieta = XLWinieta;
                    nowyUzytkownik.TrybWsadowy = XLTrybWsadowy;
                    nowyUzytkownik.UtworzWlasnaSesje = XLUtworzWlasnaSesje;
                    nowyUzytkownik.SerwerKlucza = XLSQLServer;
                    nowyUzytkownik.Baza = XLSQLDatabase;
                    nowyUzytkownik.OpeIdent = XLUser;
                    nowyUzytkownik.OpeHaslo = XLPass;
                    nowyUzytkownik.PortHTTPSerweraKlucza = 1;
                    nowyUzytkownik.RodzajSerweraKlucza = 0;
                    nowyUzytkownik.SerwerKlucza = @"EKSRV04\COMARCH::5000235184";
                    nowyUzytkownik.ProgramID = "ONE-029 Dekret z SQL";

                    int blad = cdn_api.cdn_api.XLLogin(nowyUzytkownik, ref sesja);

                    if (blad != 0)
                    {
                        MessageBox.Show("Błąd logowania: " + blad, "Komunikat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    #endregion

                    //MessageBox.Show("FSQuery:" + FSyQuery, "Komunikat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    List<FS> FSy = SelectListRowColumn(FSQuery, "cdn.Zrodla");

                    Pasek pp = new Pasek();
                    pp.Show();
                    pp.pBar1.Visible = true;
                    // Set Minimum to 1 to represent the first file being copied.
                    pp.pBar1.Minimum = 1;
                    // Set Maximum to the total number of files to copy.
                    pp.pBar1.Maximum = (FSy.Count() * 2) + 1;
                    // Set the initial value of the ProgressBar.
                    pp.pBar1.Value = 1;
                    // Set the Step property to a value of 1 to represent each file being copied.
                    pp.pBar1.Step = 1;

                    if(ConnectToSQL("if exists (SELECT * FROM cdn.Dziennik WHERE DZK_Opis = 'Przeksięgowanie vat " + Miesiace.SelectedItem.ToString() + "-" + Lata.SelectedItem.ToString() + " Konto 221') SELECT 'TAK' else SELECT 'NIE'")=="TAK")
                    {
                        DeleteFrom("delete from cdn.Dziennik where DZK_Opis = 'Przeksięgowanie vat " + Miesiace.SelectedItem.ToString() + "-" + Lata.SelectedItem.ToString() + " Konto 221'");
                    }


                    int dzien=DateTime.DaysInMonth(Convert.ToInt32(Lata.SelectedItem.ToString()), Convert.ToInt32(Miesiace.SelectedItem.ToString()));
                    string date2 = Lata.SelectedItem.ToString() + "-" + Miesiace.SelectedItem.ToString() + "-" + dzien.ToString();
                     XLDekretZbiorczyInfo_20202 zbiorczy = new XLDekretZbiorczyInfo_20202();
                    zbiorczy.Bufor = "BUFOR";
                    zbiorczy.Data = Convert.ToInt32(ConnectToSQL( "SELECT DATEDIFF(day, '" + date1 + "','" + date2 + "'); "));
                    zbiorczy.DataDokumentu = Convert.ToInt32(ConnectToSQL( "SELECT DATEDIFF(day, '" + date1 + "','" + date2 + "'); "));
                    zbiorczy.Dziennik = "PK";
                    zbiorczy.GIDFirma = 0;
                    zbiorczy.GIDLp = 0;
                    zbiorczy.GIDNumer = 0;
                    zbiorczy.GIDTyp = 0;
                    zbiorczy.Opis = "Przeksięgowanie vat "+ Miesiace.SelectedItem.ToString()+"-"+ Lata.SelectedItem.ToString()+" Konto 221";
                    zbiorczy.Tryb = 2;
                    zbiorczy.Wersja = XLWersjaAPI;

                    blad = cdn_api.cdn_api.XLDodajDekretZbiorczy(sesja, zbiorczy);
                    if (blad != 0)
                        MessageBox.Show("Błąd dodawania dekretu zbiorczego:" + blad, "Komunikat",
                             MessageBoxButtons.OK, MessageBoxIcon.Warning);




                    foreach (FS faktura in FSy)
                    {
                        pp.pBar1.PerformStep();

                        try
                        {
                            if (faktura.DT_Konto.Contains("221"))
                            {

                                XLDekretInfo_20202 dekret = new XLDekretInfo_20202();
                                dekret.GIDFirma = zbiorczy.GIDFirma;
                                dekret.GIDLp = zbiorczy.GIDLp;
                                dekret.GIDNumer = zbiorczy.GIDNumer;
                                dekret.GIDTyp = zbiorczy.GIDTyp;
                                dekret.Tryb = 2;
                                dekret.Wersja = XLWersjaAPI;
                                dekret.Opis = faktura.nrDokumentu + " - Przeksięgowanie VAT";


                                // MessageBox.Show("Kwota: " + dataTable.Rows[i][4], "Komunikat",
                                // MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                //MessageBox.Show("Kwota2: " + dataTable.Rows[i][3], "Komunikat",
                                // MessageBoxButtons.OK, MessageBoxIcon.Warning);

                                //MessageBox.Show("Konto: " +(dataTable.Rows[i][2]), "Komunikat",
                                // MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                string kontoCredit = "";
                                string kontoDebet = "";
                                switch (faktura.DT_Konto)
                                {
                                    case "221-33-23":
                                        {
                                            kontoCredit = "221-33-23";
                                            kontoDebet = "221-23";
                                            break;
                                        }
                                    case "221-33-08":
                                        {
                                            kontoCredit = "221-33-08";
                                            kontoDebet = "221-08";
                                            break;
                                        }
                                    case "221-33-05":
                                        {
                                            kontoCredit = "221-33-05";
                                            kontoDebet = "221-05";
                                            break;
                                        }
                                    case "222-33-23":
                                        {
                                            kontoCredit = "222-33-23";
                                            kontoDebet = "222-23";
                                            break;
                                        }
                                    case "222-33-08":
                                        {
                                            kontoCredit = "222-33-08";
                                            kontoDebet = "222-08";
                                            break;
                                        }
                                    case "222-33-05":
                                        {
                                            kontoCredit = "222-33-05";
                                            kontoDebet = "222-05";
                                            break;
                                        }
                                }


                                //MessageBox.Show("Konto winien: " + Convert.ToString(dataTable.Rows[i][2]), "Komunikat",
                                // MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                dekret.KontoCredit = kontoCredit;
                                dekret.KontoDebet = kontoDebet;
                                dekret.NumerDokumentu =faktura.TrN_NumerObcy;
                                // MessageBox.Show("Kwota: " + Convert.ToString(dataTable.Rows[i][4]), "Komunikat",
                                //MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                dekret.Kwota = faktura.DT_Kwota.ToString();


                                blad = cdn_api.cdn_api.XLDodajDekretSkladowy(sesja, dekret);
                                if (blad != 0)
                                {

                                    MessageBox.Show("Błąd dodawania dekretu składowego " + faktura.nrDokumentu + " - Przeksięgowanie VAT na konto " + kontoCredit + " :" + blad, "Komunikat",
                                     MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }

                             /*   dekret = new XLDekretInfo_20202();
                                dekret.GIDFirma = zbiorczy.GIDFirma;
                                dekret.GIDLp = zbiorczy.GIDLp;
                                dekret.GIDNumer = zbiorczy.GIDNumer;
                                dekret.GIDTyp = zbiorczy.GIDTyp;
                                dekret.Tryb = 2;
                                dekret.Wersja = XLWersjaAPI;
                                dekret.Opis = faktura.nrDokumentu + " - Przeksięgowanie VAT";

                                dekret.KontoCredit = "";
                                dekret.KontoDebet = kontoDebet;
                                // MessageBox.Show("Kwota: " + Convert.ToString(dataTable.Rows[i][4]), "Komunikat",
                                //MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                dekret.Kwota = faktura.DT_Kwota.ToString();


                                blad = cdn_api.cdn_api.XLDodajDekretSkladowy(sesja, dekret);
                                if (blad != 0)
                                {

                                    MessageBox.Show("Błąd dodawania dekretu składowego " + faktura.nrDokumentu + " - Przeksięgowanie VAT na konto " + kontoDebet + " :" + blad, "Komunikat",
                                     MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }*/
                            }


                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Błąd dodawania dekretu składowego " + faktura.nrDokumentu + " - Przeksięgowanie VAT na konto :" + ex.Message, "Komunikat",
                                     MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }



                    }

                    blad = cdn_api.cdn_api.XLZamknijDekretZbiorczy(sesja, zbiorczy);
                    if (blad != 0)
                        MessageBox.Show("Błąd zamykania dekretu zbiorczego :" + blad, "Komunikat",
                             MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    /*--------------------------------- 222 ----------------------------------*/


                    if (ConnectToSQL("if exists (SELECT * FROM cdn.Dziennik WHERE DZK_Opis = 'Przeksięgowanie vat " + Miesiace.SelectedItem.ToString() + "-" + Lata.SelectedItem.ToString() + " Konto 222') SELECT 'TAK' else SELECT 'NIE'") == "TAK")
                    {
                        DeleteFrom("delete from cdn.Dziennik where DZK_Opis = 'Przeksięgowanie vat " + Miesiace.SelectedItem.ToString() + "-" + Lata.SelectedItem.ToString() + " Konto 222'");
                    }

                    XLDekretZbiorczyInfo_20202 zbiorczy2 = new XLDekretZbiorczyInfo_20202();
                    zbiorczy2.Bufor = "BUFOR";
                    zbiorczy2.Data = Convert.ToInt32(ConnectToSQL("SELECT DATEDIFF(day, '" + date1 + "','" + date2 + "'); "));
                    zbiorczy2.DataDokumentu = Convert.ToInt32(ConnectToSQL("SELECT DATEDIFF(day, '" + date1 + "','" + date2 + "'); "));
                    zbiorczy2.Dziennik = "PK";
                    zbiorczy2.GIDFirma = 0;
                    zbiorczy2.GIDLp = 0;
                    zbiorczy2.GIDNumer = 0;
                    zbiorczy2.GIDTyp = 0;
                    zbiorczy2.Opis = "Przeksięgowanie vat " + Miesiace.SelectedItem.ToString() + "-" + Lata.SelectedItem.ToString() + " Konto 222";
                    zbiorczy2.Tryb = 2;
                    zbiorczy2.Wersja = XLWersjaAPI;

                    blad = cdn_api.cdn_api.XLDodajDekretZbiorczy(sesja, zbiorczy2);
                    if (blad != 0)
                        MessageBox.Show("Błąd dodawania dekretu zbiorczego:" + blad, "Komunikat",
                             MessageBoxButtons.OK, MessageBoxIcon.Warning);




                    foreach (FS faktura in FSy)
                    {
                        pp.pBar1.PerformStep();

                        try
                        {
                            if (faktura.DT_Konto.Contains("222"))
                            {

                                XLDekretInfo_20202 dekret = new XLDekretInfo_20202();
                                dekret.GIDFirma = zbiorczy2.GIDFirma;
                                dekret.GIDLp = zbiorczy2.GIDLp;
                                dekret.GIDNumer = zbiorczy2.GIDNumer;
                                dekret.GIDTyp = zbiorczy2.GIDTyp;
                                dekret.Tryb = 2;
                                dekret.Wersja = XLWersjaAPI;
                                dekret.Opis = faktura.nrDokumentu + " - Przeksięgowanie VAT";


                                // MessageBox.Show("Kwota: " + dataTable.Rows[i][4], "Komunikat",
                                // MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                //MessageBox.Show("Kwota2: " + dataTable.Rows[i][3], "Komunikat",
                                // MessageBoxButtons.OK, MessageBoxIcon.Warning);

                                //MessageBox.Show("Konto: " +(dataTable.Rows[i][2]), "Komunikat",
                                // MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                string kontoCredit = "";
                                string kontoDebet = "";
                                switch (faktura.DT_Konto)
                                {
                                    case "221-33-23":
                                        {
                                            kontoCredit = "221-33-23";
                                            kontoDebet = "221-23";
                                            break;
                                        }
                                    case "221-33-08":
                                        {
                                            kontoCredit = "221-33-08";
                                            kontoDebet = "221-08";
                                            break;
                                        }
                                    case "221-33-05":
                                        {
                                            kontoCredit = "221-33-05";
                                            kontoDebet = "221-05";
                                            break;
                                        }
                                    case "222-33-23":
                                        {
                                            kontoDebet = "222-33-23";
                                            kontoCredit = "222-23";
                                            break;
                                        }
                                    case "222-33-08":
                                        {
                                            kontoDebet = "222-33-08";
                                            kontoCredit = "222-08";
                                            break;
                                        }
                                    case "222-33-05":
                                        {
                                            kontoDebet = "222-33-05";
                                            kontoCredit = "222-05";
                                            break;
                                        }
                                }


                                //MessageBox.Show("Konto winien: " + Convert.ToString(dataTable.Rows[i][2]), "Komunikat",
                                // MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                dekret.KontoCredit = kontoCredit;
                                dekret.KontoDebet = kontoDebet;
                                dekret.NumerDokumentu = faktura.TrN_NumerObcy;
                                // MessageBox.Show("Kwota: " + Convert.ToString(dataTable.Rows[i][4]), "Komunikat",
                                //MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                dekret.Kwota = faktura.DT_Kwota.ToString();


                                blad = cdn_api.cdn_api.XLDodajDekretSkladowy(sesja, dekret);
                                if (blad != 0)
                                {

                                    MessageBox.Show("Błąd dodawania dekretu składowego " + faktura.nrDokumentu + " - Przeksięgowanie VAT na konto " + kontoCredit + " :" + blad, "Komunikat",
                                     MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }

                                /*   dekret = new XLDekretInfo_20202();
                                   dekret.GIDFirma = zbiorczy.GIDFirma;
                                   dekret.GIDLp = zbiorczy.GIDLp;
                                   dekret.GIDNumer = zbiorczy.GIDNumer;
                                   dekret.GIDTyp = zbiorczy.GIDTyp;
                                   dekret.Tryb = 2;
                                   dekret.Wersja = XLWersjaAPI;
                                   dekret.Opis = faktura.nrDokumentu + " - Przeksięgowanie VAT";

                                   dekret.KontoCredit = "";
                                   dekret.KontoDebet = kontoDebet;
                                   // MessageBox.Show("Kwota: " + Convert.ToString(dataTable.Rows[i][4]), "Komunikat",
                                   //MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                   dekret.Kwota = faktura.DT_Kwota.ToString();


                                   blad = cdn_api.cdn_api.XLDodajDekretSkladowy(sesja, dekret);
                                   if (blad != 0)
                                   {

                                       MessageBox.Show("Błąd dodawania dekretu składowego " + faktura.nrDokumentu + " - Przeksięgowanie VAT na konto " + kontoDebet + " :" + blad, "Komunikat",
                                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                   }*/
                            }


                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Błąd dodawania dekretu składowego " + faktura.nrDokumentu + " - Przeksięgowanie VAT na konto :" + ex.Message, "Komunikat",
                                     MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }



                    }

                    blad = cdn_api.cdn_api.XLZamknijDekretZbiorczy(sesja, zbiorczy2);
                    if (blad != 0)
                        MessageBox.Show("Błąd zamykania dekretu zbiorczego :" + blad, "Komunikat",
                             MessageBoxButtons.OK, MessageBoxIcon.Warning);






                    pp.Dispose();
                    pp.Close();

                    cdn_api.cdn_api.XLLogout(sesja);

                    string numerQuery = "SELECT [DZK_Identyfikator] FROM [CDN].[Dziennik] WHERE [DZK_GIDTyp] = " + zbiorczy.GIDTyp + " and [DZK_GIDFirma]=" + zbiorczy.GIDFirma + " and [DZK_GIDNumer]=" + zbiorczy.GIDNumer + " and [DZK_GIDLp]=" + zbiorczy.GIDLp;
                    string numer = ConnectToSQL( numerQuery);

                    string numer2Query = "SELECT [DZK_Identyfikator] FROM [CDN].[Dziennik] WHERE [DZK_GIDTyp] = " + zbiorczy2.GIDTyp + " and [DZK_GIDFirma]=" + zbiorczy2.GIDFirma + " and [DZK_GIDNumer]=" + zbiorczy2.GIDNumer + " and [DZK_GIDLp]=" + zbiorczy2.GIDLp;
                    string numer2 = ConnectToSQL(numer2Query);

                    cdn_api.cdn_api.XLLogout(sesja);
                    MessageBox.Show("Dodano dekrety zbiorcze: " + numer +" oraz "+numer2, "Komunikat",
                             MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                

            }
                catch (Exception en)
                {
                    MessageBox.Show("Błąd: " + en, "Komunikat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                
                }

                


                Application.Exit();
            }
        }
       

        public int getDate(string date3)
        {
            string date1 = "1800-12-28";
            //string date3 = DateTime.Now.ToString("yyyy-MM-dd");
            string Datequery = "SELECT DATEDIFF(day, '" + date1 + "','" + date3 + "'); ";
            return Convert.ToInt32(ConnectToSQL(Datequery));
        }

        public List<string> SelectListColumn(string query, string srcTableName)
        {
            List<string> str = new List<string>();
            string sttr = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.SelectCommand = new SqlCommand(query, connection);
                    DataSet dataSet = new DataSet();
                    adapter.Fill(dataSet, srcTableName);
                    foreach (DataColumn column in (InternalDataCollectionBase)dataSet.Tables[0].Columns)
                        foreach (DataRow row in (InternalDataCollectionBase)dataSet.Tables[0].Rows)
                            str.Add(row[column].ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(sttr = sttr + "Bład SQL:" + ex.ToString() + "\n");
            }
            return str;
        }

        public string ConnectToSQL(string query)
        {
            string str = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.SelectCommand = new SqlCommand(query, connection);
                    DataSet dataSet = new DataSet();
                    adapter.Fill(dataSet, "Query");
                    if (dataSet.Tables.Count == 0)
                    {
                        return "";
                    }
                    foreach (DataRow row in (InternalDataCollectionBase)dataSet.Tables[0].Rows)
                    {
                        str = row[0].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                str = str + "Bład SQL:" + ex.ToString() + "\n";
            }
            return str;
        }


        public string DeleteFrom(string sql)       //Użycie delete i inserta nie jest optymalne, ale w niektórych przypadkach pozycje nie miały atrybutu. Po testach zmienić na update
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    connection.Open();
                    adapter.DeleteCommand = new SqlCommand(sql, connection);
                    adapter.DeleteCommand.ExecuteNonQuery();
                    //MessageBox.Show("Row deleted!! ");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Komunikat", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return sql;
        }


        public List<FS> SelectListRowColumn(string query, string srcTableName)
        {
            List<string> str = new List<string>();
            List<FS> myList = new List<FS>();
            string sttr = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.SelectCommand = new SqlCommand(query, connection);
                    DataSet dataSet = new DataSet();
                    adapter.Fill(dataSet, srcTableName);
                    foreach (DataRow row in (InternalDataCollectionBase)dataSet.Tables[0].Rows)
                    {
                        foreach (DataColumn column in (InternalDataCollectionBase)dataSet.Tables[0].Columns)
                        {
                            str.Add(row[column].ToString());

                            //MessageBox.Show(column + ": " + row[column].ToString());
                        }
                        myList.Add(new FS(str[0], str[1], str[2], str[3], str[4], str[5], str[6], str[7], str[8], str[9], str[10]));
                        str.Clear();

                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(sttr = sttr + "Bład SQL:" + ex.ToString() + "\n");
            }
            return myList;
        }


        public List<int> SelectListColumnZdr(string query, string srcTableName)
        {
            List<int> str = new List<int>();
            string sttr = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.SelectCommand = new SqlCommand(query, connection);
                    DataSet dataSet = new DataSet();
                    adapter.Fill(dataSet, srcTableName);
                    foreach (DataRow row in (InternalDataCollectionBase)dataSet.Tables[0].Rows)
                    {
                        foreach (DataColumn column in (InternalDataCollectionBase)dataSet.Tables[0].Columns)
                        {
                            str.Add(Convert.ToInt32(row[column].ToString()));

                            //MessageBox.Show(column + ": " + row[column].ToString());
                        }
                       
                    }
                }

                
            }
            catch (Exception ex)
            {
                MessageBox.Show(sttr = sttr + "Bład SQL:" + ex.ToString() + "\n");
            }
            return str;
        }

        public string Update(string sql)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    connection.Open();
                    adapter.UpdateCommand = new SqlCommand(sql, connection);
                    adapter.UpdateCommand.ExecuteNonQuery();
                    //MessageBox.Show("Row updated !! ");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Komunikat", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return sql;
        }


    }


}
