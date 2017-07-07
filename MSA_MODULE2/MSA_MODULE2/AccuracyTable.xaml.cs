using Microsoft.WindowsAzure.MobileServices;
using MSA_MODULE2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MSA_MODULE2
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AccuracyTable : ContentPage
	{
        MobileServiceClient client = AzureManager.AzureManagerInstance.AzureClient;
        public AccuracyTable ()
		{
			InitializeComponent ();
		}
        async void Handle_ClickedAsync(object sender, System.EventArgs e)
        {
            //try
            //{
            //    List<ComputerVisionInfo> AccuracyInfo = await client.GetSyncTable<ComputerVisionInfo>().ToListAsync();
            //    AccuracyList.ItemsSource = AccuracyInfo;
            //}
            //catch (Exception ex)
            //{
            //    DebugLabel.Text = ex.ToString();
            //}
            
            List<ComputerVisionInfo> AccuracyInfo = await AzureManager.AzureManagerInstance.GetAccuracyInfo();
            AccuracyList.ItemsSource = AccuracyInfo;

        }
        async void OnPreviousPageButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}