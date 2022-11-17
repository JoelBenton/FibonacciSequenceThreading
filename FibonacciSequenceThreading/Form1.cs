using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FibonacciSequenceThreading
{
    public partial class Form1 : Form
    {
        private int numberToCompute = 0;
        private int highestPercentageReached = 0;
        public Form1()
        {
            InitializeComponent();
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
        }


        private long ComputeFibonacci(int n, BackgroundWorker worker, DoWorkEventArgs e)
        {
            // The parameter n must be >= 0 and <= 91.
            // Fib(n), with n > 91, overflows a long.
            if ((n < 0) || (n > 91))
            {
                throw new ArgumentException("value must be >= 0 and <= 91", "n");
            }

            long result = 0;

            // Abort the operation if the user has canceled.
            // Note that a call to CancelAsync may have set 
            // CancellationPending to true just after the
            // last invocation of this method exits, so this 
            // code will not have the opportunity to set the 
            // DoWorkEventArgs.Cancel flag to true. This means
            // that RunWorkerCompletedEventArgs.Cancelled will
            // not be set to true in your RunWorkerCompleted
            // event handler. This is a race condition.

            if (worker.CancellationPending)
            {
                e.Cancel = true;
            }
            else
            {
                if (n < 2)
                {
                    result = 1;
                }
                else
                {
                    result = ComputeFibonacci(n - 1, worker, e) + ComputeFibonacci(n - 2, worker, e);
                }

                // Report progress as a percentage of the total task.
                int percentComplete = (int)((float)n / (float)numberToCompute * 100);
                if (percentComplete > highestPercentageReached)
                {
                    highestPercentageReached = percentComplete;
                    worker.ReportProgress(percentComplete);
                }
            }

            return result;
        }

        private void Startbutton_Click(object sender, EventArgs e)
        {
            textBox1.Enabled = false;
            InputNumber.Enabled = false;
            Startbutton.Enabled = false;
            Cancelbutton.Enabled = true;
            numberToCompute = (int)InputNumber.Value;
            highestPercentageReached = 0;

            if (backgroundWorker1.IsBusy != true)
            { 
                //start the asynchronous operation
                backgroundWorker1.RunWorkerAsync(numberToCompute);
            }

            
        }

        private void Cancelbutton_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.WorkerSupportsCancellation == true)
            {
                // Cancel the asynchronous operation
                backgroundWorker1.CancelAsync();
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            e.Result = ComputeFibonacci((int)e.Argument, worker, e);
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.TaskProgressBar.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else if (e.Cancelled)
            {
                textBox1.Text = "Canceled";
            }
            else
            {
                textBox1.Text = e.Result.ToString();
            }

            InputNumber.Enabled = true;
            Startbutton.Enabled = true;
            Cancelbutton.Enabled = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Cancelbutton.Enabled = false;
        }
    }
}
