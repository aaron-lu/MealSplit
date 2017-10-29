using Newtonsoft.Json;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ImageCropSample
{
    public class ImageToText
    {
        // **********************************************
        // *** Update or verify the following values. ***
        // **********************************************

        // Replace the subscriptionKey string value with your valid subscription key.
        const string subscriptionKey = "99111d8302ee4f60a5d8b114abdf531f";

        // Replace or verify the region.
        //
        // You must use the same region in your REST API call as you used to obtain your subscription keys.
        // For example, if you obtained your subscription keys from the westus region, replace 
        // "westcentralus" in the URI below with "westus".
        //
        // NOTE: Free trial subscription keys are generated in the westcentralus region, so if you are using
        // a free trial subscription key, you should not need to change this region.
        const string uriBase = "https://westcentralus.api.cognitive.microsoft.com/vision/v1.0/ocr";


        public async Task<ObservableCollection<ItemizedFood>> getTextAsync(MediaFile mediaFile)
        {
            // Get the path and filename to process from the user.
  

            // Execute the REST API call.
            return await MakeOCRRequest(mediaFile);

            
        }


        /// <summary>
        /// Gets the text visible in the specified image file by using the Computer Vision REST API.
        /// </summary>
        /// <param name="imageFilePath">The image file.</param>
        static async Task<ObservableCollection<ItemizedFood>> MakeOCRRequest(MediaFile mediaFile)
        {
            HttpClient client = new HttpClient();

            // Request headers.
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

            // Request parameters.
            string requestParameters = "language=unk&detectOrientation=true";

            // Assemble the URI for the REST API Call.
            string uri = uriBase + "?" + requestParameters;

            HttpResponseMessage response;

            // Request body. Posts a locally stored JPEG image.
            byte[] byteData = GetImageAsByteArray(mediaFile);

            using (ByteArrayContent content = new ByteArrayContent(byteData))
            {
                // This example uses content type "application/octet-stream".
                // The other content types you can use are "application/json" and "multipart/form-data".
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                // Execute the REST API call.
                response = await client.PostAsync(uri, content);

                // Get the JSON response.
                string contentString = await response.Content.ReadAsStringAsync();

                // Display the JSON response.
                

                ReceiptInfo currentReceipt = JsonConvert.DeserializeObject<ReceiptInfo>(contentString);

                List<string> Items = new List<string>();
                List<Double> Price = new List<Double>();

                foreach (Region reg in currentReceipt.regions)
                {
                    foreach (Line l in reg.lines)
                    {
                        String item = "";
                        foreach (Word w in l.words)
                        {
                            double cost;

                            item += w.text;

                            if (Double.TryParse(w.text, out cost))
                            {
                                Price.Add(cost);
                            }
                        }
                        double c2;
                        if (!Double.TryParse(item, out c2))
                        {
                            Items.Add(item);
                        }

                    }
                }


                ObservableCollection < ItemizedFood > food  = new ObservableCollection<ItemizedFood>();

                for(int i =0; i<Items.Count; i++)
                {
                    try
                    {
                        food.Add(new ItemizedFood(Items[i], Price[i]));
                    }
                    catch
                    {

                    }
                }
                return food;
            }
            
            }
        


        /// <summary>
        /// Returns the contents of the specified file as a byte array.
        /// </summary>
        /// <param name="imageFilePath">The image file to read.</param>
        /// <returns>The byte array of the image data.</returns>
        static byte[] GetImageAsByteArray(MediaFile imageFilePath)
        {
            byte[] imageAsByte = null;
            using (var memoryStream = new MemoryStream())
            {
               imageFilePath.GetStream().CopyToAsync(memoryStream);
                imageAsByte = memoryStream.ToArray();
            }

            return imageAsByte;
        }


        /// <summary>
        /// Formats the given JSON string by adding line breaks and indents.
        /// </summary>
        /// <param name="json">The raw JSON string to format.</param>
        /// <returns>The formatted JSON string.</returns>
        static string JsonPrettyPrint(string json)
        {
            if (string.IsNullOrEmpty(json))
                return string.Empty;

            json = json.Replace(Environment.NewLine, "").Replace("\t", "");

            StringBuilder sb = new StringBuilder();
            bool quote = false;
            bool ignore = false;
            int offset = 0;
            int indentLength = 3;

            foreach (char ch in json)
            {
                switch (ch)
                {
                    case '"':
                        if (!ignore) quote = !quote;
                        break;
                    case '\'':
                        if (quote) ignore = !ignore;
                        break;
                }

                if (quote)
                    sb.Append(ch);
                else
                {
                    switch (ch)
                    {
                        case '{':
                        case '[':
                            sb.Append(ch);
                            sb.Append(Environment.NewLine);
                            sb.Append(new string(' ', ++offset * indentLength));
                            break;
                        case '}':
                        case ']':
                            sb.Append(Environment.NewLine);
                            sb.Append(new string(' ', --offset * indentLength));
                            sb.Append(ch);
                            break;
                        case ',':
                            sb.Append(ch);
                            sb.Append(Environment.NewLine);
                            sb.Append(new string(' ', offset * indentLength));
                            break;
                        case ':':
                            sb.Append(ch);
                            sb.Append(' ');
                            break;
                        default:
                            if (ch != ' ') sb.Append(ch);
                            break;
                    }
                }
            }

            return sb.ToString().Trim();
        }
    }
}

