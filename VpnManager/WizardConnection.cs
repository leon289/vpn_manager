﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Data;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VpnManagerDAL;
using VpnManager.Interface;

namespace VpnManager
{
    public partial class WizardConnection : Form
    {
        string Path = string.Empty;
        string FileName = string.Empty;
        int IDConection = 0;
        public WizardConnection()
        {
            InitializeComponent();
        }

        private void cmdBroswe_Click(object sender, EventArgs e)
        {
           
            if (openFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Path = openFile.FileName;
                txtPath.Text = Path;
            }
        }

        private bool CheckAssembly(string AssemblyName)
        {
            try
            {
                //OnInfoFromCore("Core: Caricamento Assembly", true);
              
              
                //string[] files = Directory.GetFileSystemEntries(path, "*.dll");           
                Assembly ass = Assembly.LoadFrom(AssemblyName);

                List<Type> types = ExamineAssembly(ass, typeof(IConnection));
                if (types.Count > 0)
                    return true;

                return false;

            }
            catch (Exception e)
            {
                return false;
            }
        }
        private List<Type> ExamineAssembly(Assembly assembly, Type type)
        {
            List<Type> types = new List<Type>();
            foreach (Type t in assembly.GetTypes())
            {
                if (t.IsPublic && (t.Attributes & TypeAttributes.Abstract) == 0)
                {
                    if (ImplementInterface(t, type))
                        types.Add(t);
                }
            }
            return types;
        }

        private bool ImplementInterface(Type t, Type matchType)
        {
            Type[] interfaces = t.GetInterfaces();
            foreach (Type i in interfaces)
            {
                if (i == matchType)
                    return true;
            }
            return false;
        }

        private void wizardPage2_NextButtonClick(object sender, CancelEventArgs e)
        {
            try
            {
                if (Path != string.Empty)
                {
                    if (CheckAssembly(Path))
                    {
                        FileInfo flinfo = new FileInfo(Path);
                        FileName = flinfo.Name;
                        File.Copy(Path, Environment.CurrentDirectory+ "\\" + FileName, true);
                        VpnManagerDal.InsertNewVpnType(FileName);
                        VpnManagerDAL.DTO.VpnTypeDTO VpnType =  VpnManagerDal.GetVpnTypes().Where(x => x.Name.Equals(FileName)).FirstOrDefault();
                        IDConection = VpnType.Id;
                       
                    }
                    else
                    {
                        e.Cancel = true;
                        MessageBox.Show("File Non Valido!");
                    }

                }
                else
                {
                    e.Cancel = true;
                    MessageBox.Show("Nessun File Selezionato!");
                }
            }
            catch(Exception ex)
            {

                e.Cancel = true;
                MessageBox.Show(ex.Message);
            }
        }

        private void wizardPage2_BackButtonClick(object sender, CancelEventArgs e)
        {
            Path = string.Empty;
            FileName = string.Empty;
        }

        private void cmdAdd_Click(object sender, EventArgs e)
        {
            VpnManagerDal.AddExtensionObject(IDConection, VpnManagerDAL.DTO.TargetTable.VpnType, txtName.Text, txtValue.Text);
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                VpnManagerDal.DeleteExtensionObject((int)dataGridView1.SelectedRows[0].Cells[0].Value);

            }
        }

        private void wizard1_FinishButtonClick(object sender, CancelEventArgs e)
        {
            this.Close();
        }

        private void wizard1_CancelButtonClick(object sender, CancelEventArgs e)
        {
            this.Close();
        }

        private void WizardConnection_Load(object sender, EventArgs e)
        {

        }
    }
}
