//Copyright © 2019 Electric Power Research Institute, Inc. All rights reserved.
//
//Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met: 
//  Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//  Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
//  Neither the name of the EPRI nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
// 

using System;
using System.Threading;
using System.Windows.Forms;

namespace PQds
{
    static class Program
    {
        /* Uncomment to be able to run 2 windows side by side.
        private class MultiFormsContext : ApplicationContext
        {
            private int openForms;
            public MultiFormsContext(params Form[] forms)
            {
                openForms = forms.Length;

                foreach (Form form in forms)
                {
                    form.FormClosed += (s, args) =>
                    {
                        if (Interlocked.Decrement(ref openForms) == 0)
                            ExitThread();
                    };

                    form.Show();
                }
            }
        }*/

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new PQdsMain());

            /* Uncomment to be able to run two windows side by side.
            FileViewer app1 = new FileViewer();
            FileViewer app2 = new FileViewer();

            Application.Run(new MultiFormsContext(app1, app2));
            */
        }
    }
}
