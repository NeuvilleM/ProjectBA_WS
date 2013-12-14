using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Web.Http;
using Newtonsoft.Json;

namespace ProjectWS.DataModel
{
    public class ArtiestItem
    {
        public ArtiestItem(String uniqueId, String Picture, String Name, String description, String twitter, String facebook, List<string> genres)
        {
            this.UniqueId = uniqueId;
            this.Picture = Picture;
            this.Name = Name;
            this.Description = description;
            this.Facebook = facebook;
            this.Twitter = twitter;
            this.Genres = genres;
        }

        public string UniqueId { get; private set; }
        public string Picture { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Facebook { get; private set; }
        public string Twitter { get; private set; }
        public List<string> Genres { get; private set; }

        public override string ToString()
        {
            return this.Name;
        }
    }

    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class StageGroup
    {
        public StageGroup(String uniqueId, String name)
        {
            this.UniqueId = uniqueId;
            this.Name = name;
            this.Items = new ObservableCollection<lineupItem>();
        }
        public string UniqueId { get; private set; }
        public string Name { get; private set; }
        public ObservableCollection<lineupItem> Items { get; set; }

        public override string ToString()
        {
            return this.Name;
        }
    }
    public class lineupItem
    {
        public lineupItem(String uniqueId,  string stageId, string dateOfPlay, string start, string end, ArtiestItem artist)
        {
            this.artiest = artist;
            this.UniqueId = uniqueId;
            this.StageId = stageId;
			this.DateOfPlay = dateOfPlay;
			this.Start = start;
			this.End = end;
            
        }
        public ArtiestItem artiest { get; set; }
        public String StageId { get; private set; }
        public string DateOfPlay { get; private set; }
        public string Start { get; private set; }
        public string End { get; private set; }
        public string UniqueId { get; private set; }
       
       

        public override string ToString()
        {
            return this.artiest.Name + this.StageId;
        }
    
    }

    /// <summary>
    /// Creates a collection of groups and items with content read from a static json file.
    /// 
    /// SampleDataSource initializes with data read from a static json file included in the 
    /// project.  This provides sample data at both design-time and run-time.
    /// </summary>
    public sealed class FestivalDataSource
    {
        private ObservableCollection<StageGroup> _stages;

        public ObservableCollection<StageGroup> Stages
        {
            get { return this._stages; }
            set { _stages = value; }
        }
        public FestivalDataSource()
        {
            this.Stages = new ObservableCollection<StageGroup>();
            //this.GetSampleDataAsync();
            Debug.WriteLine("Test");
        }

        private static FestivalDataSource _sampleDataSource = new FestivalDataSource();

        //public void LoadData()
        //{
        //    if (this.IsDataLoaded == false)
        //    {
        //        this.Stages.Clear();
        //        //this.Stages.Add(new ItemViewModel() { ID = "0", LineOne = "Please Wait...", LineTwo = "Please wait while the catalog is downloaded from the server.", LineThree = null });
        //        WebClient webClient = new WebClient();
        //        webClient.Headers["Accept"] = "application/json";
        //        webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(webClient_DownloadCatalogCompleted);
        //        webClient.DownloadStringAsync(new Uri(apiUrl));
        //    }
        //}

        public static async Task<IEnumerable<StageGroup>> GetGroupsAsync()
        {
            await _sampleDataSource.GetSampleDataAsync();

            return _sampleDataSource.Stages;
        }

        public static async Task<StageGroup> GetGroupAsync(string uniqueId)
        {
            await _sampleDataSource.GetSampleDataAsync();
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.Stages.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static async Task<lineupItem> GetItemAsync(string uniqueId)
        {
            await _sampleDataSource.GetSampleDataAsync();
            // Simple linear search is acceptable for small data sets
            //var matches = _sampleDataSource.Stages.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            
            foreach (StageGroup group in _sampleDataSource.Stages)
            {
                foreach (lineupItem li in group.Items)
                {
                    if (li.artiest.UniqueId.Equals(uniqueId)) { return li; }
                }
            }
            //if (matches.Count() == 1) return matches.First();
            return null;
        }
        private async Task GetSampleDataAsync()
        {
            if (this._stages.Count != 0)
                return;
            // inladen data via REST-server
            
            HttpClient client = new HttpClient();
            //client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("accept", "application/json");
            string jsonText = await client.GetStringAsync(new Uri("http://localhost:5096/api/values"));
            /*if (response.IsSuccessStatusCode)
            {
                //Stream stream = await response.Content.ReadAsStreamAsync();
                string jsonText = response.Content();
            }
            else { 
            Uri dataUri = new Uri("ms-appx:///DataModel/values.json");
            //Uri dataUri = new Uri("http://localhost:5096/api/values");
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
            string jsonText = await FileIO.ReadTextAsync(file);
            }*/
            jsonText = "{"+'"'+"Stages"+'"'+":" + jsonText + "}";
            JsonObject jsonObject = JsonObject.Parse(jsonText);
            JsonArray jsonArray = jsonObject["Stages"].GetArray();
            //JsonArray jsonArray = jsonObject[].GetArray();
            foreach (JsonValue groupValue in jsonArray)
            {
                JsonObject groupObject = groupValue.GetObject();
                StageGroup group = new StageGroup(groupObject["UniqueId"].GetString(),
                                                        groupObject["Name"].GetString());
                Debug.WriteLine("groupObject['Items'] = " + groupObject["Items"]);
                foreach (JsonValue itemValue in groupObject["Items"].GetArray())
                {
                    
                    JsonObject itemObject = itemValue.GetObject();
                    
                    JsonValue artistValue = itemObject["artiest"] as JsonValue;
                    JsonObject artistObject = artistValue.GetObject();
                    List<String> genres = new List<string>();
                    int iTeller = 0;
                    Debug.WriteLine("artistObject['genres'] = "+artistObject["Genres"]);
                    foreach (JsonValue genreValue in artistObject["Genres"].GetArray())
                    {
                        Debug.WriteLine("genreValue = " + genreValue);
                        //JsonObject genreObject = genreValue.GetObject();
                        //genres.Add(genreObject[iTeller.ToString()].GetString());
                        genres.Add(genreValue.GetString());
                        iTeller ++;
                    }
                   
                    ArtiestItem artist = new ArtiestItem(artistObject["UniqueId"].GetString(),
                                                           artistObject["Picture"].GetString(),
                                                           artistObject["Name"].GetString(),
                                                           artistObject["Description"].GetString(),
                                                           artistObject["Facebook"].GetString(),
                                                           artistObject["Twitter"].GetString(), genres);
                    
                    group.Items.Add(new lineupItem(itemObject["StageId"].GetString(),
                                                        itemObject["UniqueId"].GetString(),
                                                       itemObject["DateOfPlay"].GetString(),
                                                       itemObject["Start"].GetString(),
                                                       itemObject["Einde"].GetString(), artist));
                }
                bool isNotInsertedYet = true;
                foreach (StageGroup s in this.Stages)
                {
                    if (s.Name == group.Name) isNotInsertedYet = false;
                }
                if (isNotInsertedYet)
                {
                    this.Stages.Add(group);
                }
            }
        }


        public bool IsDataLoaded { get; set; }
    }
}
