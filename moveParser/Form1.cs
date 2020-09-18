using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace moveParser
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            dynamic pkmn_ps_data = JObject.Parse(File.ReadAllText("data/PS/learnsets.js"));

            int cantidad = pkmn_ps_data.Count;
            var a = pkmn_ps_data["bulbasaur"];
            object b = a.learnset;

            string[] propertyNames = b.GetType().GetProperties().Select(p => p.Name).ToArray();
            foreach (var prop in propertyNames)
            {
                object propValue = b.GetType().GetProperty(prop).GetValue(b, null);
            }

            
        }
    }
}
