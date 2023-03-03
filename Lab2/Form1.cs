using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using dotnetCHARTING.WinForms;

namespace Lab2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            string sqlServer = "DESKTOP-CG06B1F\\INSTANCE_2K19";
            string dbName = "Northwind";
            dl = new DataLayer(sqlServer, dbName);
        }

    
        DataLayer dl;
        Chart chart = new Chart();
        
        DataTable SuppliersDt, ProductTotalPricesByCategoriesDT;
        private void Form1_Load(object sender, EventArgs e)
        {
            
            if (dl.IsValid)
            {
                string sql = "SELECT SupplierId, CompanyName FROM Suppliers";
                SuppliersDt = dl.GetData(sql, "Suppliers");
                comboBoxSupplier.DataSource = SuppliersDt;
                comboBoxSupplier.DisplayMember = "CompanyName";
                comboBoxSupplier.ValueMember = "SupplierId";
                comboBoxSupplier.SelectedIndexChanged += comboBoxSupplier_SelectedIndexChanged;
                
                chart.Dock = DockStyle.None;
                chart.Use3D = true;
                chart.Type = ChartType.ComboHorizontal;
                chart.AutoSize = false;
                Controls.Add(chart);
                SupplierChanged();
            }
            
        }
        private void SupplierChanged()
        {
            string query = $@"
                select 
	                c.CategoryName,
	                sum(p.UnitsInStock*p.UnitPrice) as TotalPrice
                from products as p
                join Suppliers as s
                    on s.SupplierID = p.SupplierID
                join Categories as c
                    on c.CategoryID = p.CategoryID
                where s.SupplierID = {comboBoxSupplier.SelectedValue}
                group by c.CategoryName
            ";
            ProductTotalPricesByCategoriesDT = dl.GetData(query, "Product total prices by categories");
            prices_category.DataSource = ProductTotalPricesByCategoriesDT;

            this.Controls.Remove(chart);
            this.Controls.Add(chart);

            chart.SeriesCollection.Clear();
            foreach (DataRow dr in ProductTotalPricesByCategoriesDT.Rows)
            {

                Element element = new Element(dr["CategoryName"].ToString(), double.Parse(dr["TotalPrice"].ToString()));
                element.Annotation = new Annotation(dr["TotalPrice"].ToString());
                ElementCollection elementCollection = new ElementCollection();
                elementCollection.Add(element);
                Series serie = new Series(dr["CategoryName"].ToString(), elementCollection);
                chart.SeriesCollection.Add(serie);
            }
        }

        private void comboBoxSupplier_SelectedIndexChanged(object sender, EventArgs e)
        {
            SupplierChanged();
        }

    }
}
