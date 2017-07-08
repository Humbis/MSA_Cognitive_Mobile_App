using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;
using MSA_MODULE2.Model;

namespace MSA_MODULE2
{
    public partial class MainPage : ContentPage
    {
        private string caption = "";
        public MainPage()
        {
            InitializeComponent();
            this.Title = "What Is This?";
        }

        private async void loadCamera(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }

            MediaFile file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                PhotoSize = PhotoSize.Medium,
                Directory = "Sample",
                Name = $"{DateTime.UtcNow}.jpg"
            });

            if (file == null)
                return;

            image.Source = ImageSource.FromStream(() =>
            {
                return file.GetStream();
            });

            TagLabel.Text = "";

            await MakePredictionRequest(file);
        }
        static byte[] GetImageAsByteArray(MediaFile file)
        {
            var stream = file.GetStream();
            BinaryReader binaryReader = new BinaryReader(stream);
            return binaryReader.ReadBytes((int)stream.Length);
        }

        async Task MakePredictionRequest(MediaFile file)
        {
            caption = "";
            Contract.Ensures(Contract.Result<Task>() != null);
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "ee27669e3fd94db5b6e7c82b16be3d8a");

            string url = "https://westcentralus.api.cognitive.microsoft.com/vision/v1.0/analyze?visualFeatures=Categories,Description,Tags&language=en";

            HttpResponseMessage response;

            byte[] byteData = GetImageAsByteArray(file);

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                LoadingSpinner.IsVisible = true;
                LoadingSpinner.IsRunning = true;
                response = await client.PostAsync(url, content);
                LoadingSpinner.IsRunning = false;
                LoadingSpinner.IsVisible = false;
                GuessingBtns.IsEnabled = true;
                GuessingBtns.IsVisible = true;

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    CognitiveResponse responseData = JsonConvert.DeserializeObject<CognitiveResponse>(responseString);
                    String tags = "";
                    foreach (CognitiveResponse.Tag t in responseData.tags)
                    {
                        tags += t.name + ", ";
                    }
                    //Remove the last set of commas
                    tags = tags.Remove(tags.Length - 2);
                    foreach (CognitiveResponse.Caption c in responseData.description.captions)
                    {
                      
                        if (c.confidence > 0.8)
                        {
                            TagLabel.Text += "This is most likely: " + c.text + "\n";
                        }else if(c.confidence < 0.4)
                        {
                            TagLabel.Text += "This may or may not be: " + c.text + "\n";
                        }else
                        {
                            TagLabel.Text += "This might be: " + c.text + "\n";
                        }
                        caption += c.text + "/";
                    }
                    
                    TagLabel.Text += "Tags: " + tags;
                }
                else
                {
                    TagLabel.Text = response.ToString();        //Debugging purposes
                }
                caption = caption.Remove(caption.Length - 1);
                //Get rid of file once we have finished using it
                file.Dispose();
            }

        }
        async void OnNextPageButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AccuracyTable());
        }

        private void OnCorrectClicked(object sender, EventArgs e)
        {
            UpdateAzureTable(true);
        }
        private void OnIncorrectClicked(object sender, EventArgs e)
        {
            UpdateAzureTable(false);
        }
        private async void UpdateAzureTable(bool isCorrect)
        {
            GuessingBtns.IsEnabled = false;
            GuessingBtns.IsVisible = false;
            LoadingSpinner.IsVisible = true;
            LoadingSpinner.IsRunning = true;
            ComputerVisionInfo newEntry = new ComputerVisionInfo
            {
                Caption = caption,
                Correct = isCorrect
            };
            await AzureManager.AzureManagerInstance.PostAccuracyInfo(newEntry);
            LoadingSpinner.IsRunning = false;
            LoadingSpinner.IsVisible = false;
        }
    }
}
