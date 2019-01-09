using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace VITSAList
{
    public partial class PlanetPage : ContentPage
    {
        public PlanetPage()
        {
            BindingContext = new PlanetViewModel();
            InitializeComponent();

        }
    }
}
