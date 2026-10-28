using Hydra;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using DekretZSql;
using System.Collections.Generic;
using System.Text;
using System;
using System.Data.SqlClient;
using System.Data;
using cdn_api;
using System.Globalization;

[assembly: CallbackAssemblyDescription(Name: "ONE-029 Dekret z SQL",
                                           Description: "Importowanie dekretu do przegsięgowania VAT",
                                           Author: "OneSupport",
                                           Version: "1.0",
                                           SystemVersion: "2022.1.0",
                                           Created: "20-07-2023")]


namespace DekretZSql

{
    [SubscribeProcedure(Procedures.DZKLista, "Dekret z SQL")]
    public class ZSnaFS_Class : Callback
    {
        
        private string XLSQLDatabase = "ERPXL_Termedia_Test";//"ERPXL_PoloMarket"
        
        private string connectionString = @"Data Source=192.168.4.24;Initial Catalog=ERPXL_Termedia_Test; User ID=onesupport;Password=a956!2A@";
        //private string connectionString2 = @"Data Source=192.168.1.203;Initial Catalog=onxtprod; User ID=onesupport;Password=YNjY5eXpsbRr\/L5sX";

        private ClaWindow RapButton;
        private ClaWindow zakladkaTabWewnetrzne;
        
        private ClaWindow filtrBtn;
        //private List<ListaZS> ZSy = new List<ListaZS>();
       
        public override void Init()
        {

            AddSubscription(true, 0, Events.JustAfterWindowOpening, new TakeEventDelegate(OnOpenWindow));
            AddSubscription(false, 0, Events.ResizeWindow, new TakeEventDelegate(Resize));
        }



        private bool OnOpenWindow(Procedures ProcId, int ControlId, Events Event)
        {

            ClaWindow Parent = GetWindow();
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            Rectangle WndBnd = GetWindow().Bounds;
            //zakladkaTabWewnetrzne = GetWindow().AllChildren["?Tab_WgNumeru"];
            RapButton = Parent.Children.Add(ControlTypes.button);
            RapButton.Visible = true;
            RapButton.TextRaw = "VAT";
            RapButton.ToolTipRaw = "Przeksięgowanie VAT";
            //RapButton.IconRaw = "C:\\Onesupport\\FSnaRW\\wydanie2.ico";
          
            filtrBtn = GetWindow().AllChildren["?insertPK"];                            //pobiera współrzędne przycisku zapisz
            RapButton.Bounds = new Rectangle(filtrBtn.Bounds.X - 110, filtrBtn.Bounds.Y, 20, 15);
            
            //tu sprawdzic

            RapButton.OnAfterMouseDown += new TakeEventDelegate(BtnOnMouseDown);
            //RapButton.OnBeforeAccepted += new TakeEventDelegate(zaczytaj_OnAfterAccepted);

           


            //pobrane z pola w zakładce Raporty
            return true;
        }

        public bool Resize(Procedures ProcId, int ControlId, Events Event)
        {
            filtrBtn = GetWindow().AllChildren["?insertPK"];                            //pobiera współrzędne przycisku zapisz
            RapButton.Bounds = new Rectangle(filtrBtn.Bounds.X - 110, filtrBtn.Bounds.Y, 20, 15);
           
            return true;
        }

       

        public bool BtnOnMouseDown(Procedures ProcId, int ControlId, Events Event)
        {
            int dokumentID = 0;
            try
            {

                //ADODB._Recordset recordset = Runtime.WindowController.GetQueueMarked((int)ProcId, listID, GetCallbackThread());

                // MessageBox.Show(Runtime.ConfigurationDictionary.Baza.ToString().Substring(0, 18));
                if (Runtime.ConfigurationDictionary.Baza.ToString().Substring(0, 9) == "ERPXL_E_P")
                {
                    //MessageBox.Show("Jestem tutaj", "Komunikat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    XLSQLDatabase = "ElektroKlim_Pro";
                    connectionString = @"Data Source = EKSRV04\COMARCH; Initial Catalog = ERPXL_E_P; User ID = ComarchAPI; Password=hT@S+NQZBT";
                    Application.Run(new Form1(connectionString, XLSQLDatabase));
                }
                else if (Runtime.ConfigurationDictionary.Baza.ToString().Substring(0, 7) == "ERPXL_E")
                {
                    XLSQLDatabase = "ElektroKlim";
                    connectionString = @"Data Source = EKSRV04\COMARCH; Initial Catalog = ERPXL_E; User ID = ComarchAPI; Password=hT@S+NQZBT";
                    Application.Run(new Form1(connectionString, XLSQLDatabase));
                }
                else
                {
                    MessageBox.Show("Dodatek nie posiada funkcjonalności dla tej bazy.", "Komunikat",
                     MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;

                }

                

            }
            catch (Exception e) {
                MessageBox.Show("Błąd połączenia: " + e.Message, "Komunikat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
           
           
            return true;
        }

       
        public override void Cleanup()
        {

        }

       
     
    }
}
