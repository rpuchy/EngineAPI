﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EngineAPI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Simulation MySimulation = new Simulation(textBox1.Text, @"C:\Git\planning-tool-documentation\EngineInputTemplateTypeDefines.xml");


            string children = "";
            MySimulation.FindObjectbyNodeName("Params").Parameters["Scenarios"] = 1;

            var test = MySimulation.FindObjectbyNodeName("Params").AddableObjects();

            var slist = new List<int>();

            slist.Add(1);
            slist.Add(2);

            MySimulation.AddTransactionLog("c:\\temp\\tlog.csv", slist);
            

            MySimulation.SetoutputLocation("c:\\temp\\results.csv");

            MySimulation.SaveAs("c:\\temp\\sim1scenario.xml");


        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                 textBox1.Text = openFileDialog1.FileName;
            }
        }
    }
}
