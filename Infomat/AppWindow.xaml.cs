using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Infomat
{
    /// <summary>
    /// Логика взаимодействия для AppWindow.xaml
    /// </summary>
    public sealed partial class AppWindow : Window
    {
        public AppWindow(Control browser)
        {
            InitializeComponent();
            AddChild(browser);

            
        }
        

        
        
    }

    
}
