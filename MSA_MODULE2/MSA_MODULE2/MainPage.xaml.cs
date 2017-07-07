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
        public MainPage()
        {
            InitializeComponent();
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
                        
                    }
                    
                    TagLabel.Text += "Tags: " + tags;
                }
                else
                {
                    TagLabel.Text = response.ToString();        
                }

                //Get rid of file once we have finished using it
                file.Dispose();
            }
        }
    }
}
